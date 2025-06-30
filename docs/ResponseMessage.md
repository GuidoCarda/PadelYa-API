# ResponseMessage - Documentación Completa

## Descripción General

La clase `ResponseMessage<T>` es una estructura unificada de respuesta para todos los endpoints de la API PadelYa. Proporciona una forma consistente y estandarizada de devolver datos, mensajes de éxito, errores y metadatos adicionales.

## Estructura de la Respuesta

### Propiedades Principales

| Propiedad | Tipo | Descripción | Ejemplo |
|-----------|------|-------------|---------|
| `success` | `bool` | Indica si la operación fue exitosa | `true` |
| `message` | `string` | Mensaje legible que describe el resultado | `"Usuario creado exitosamente"` |
| `errorCode` | `string?` | Código de error para manejo programático | `"VALIDATION_ERROR"` |
| `data` | `T?` | Los datos reales de la respuesta | `UserDto` |
| `validationErrors` | `List<ValidationError>?` | Errores de validación cuando aplica | `[{"field": "email", "message": "Email inválido"}]` |
| `pagination` | `PaginationInfo?` | Información de paginación | `{"page": 1, "pageSize": 10}` |
| `metadata` | `ResponseMetadata?` | Metadatos de la respuesta | `{"processingTime": "150ms"}` |
| `timestamp` | `DateTime` | Timestamp de generación de la respuesta | `"2024-01-15T10:30:00Z"` |
| `correlationId` | `string?` | ID de correlación para seguimiento | `"req-12345"` |

## Ejemplos de Respuestas

### Respuesta Exitosa con Datos
```json
{
  "success": true,
  "message": "Usuario creado exitosamente",
  "data": {
    "id": 1,
    "email": "usuario@ejemplo.com",
    "name": "Juan Pérez"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Respuesta de Error
```json
{
  "success": false,
  "message": "El email ya está registrado",
  "errorCode": "DUPLICATE_EMAIL",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Respuesta con Errores de Validación
```json
{
  "success": false,
  "message": "Datos de entrada inválidos",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": [
    {
      "field": "email",
      "message": "El formato del email es inválido",
      "value": "email-invalido"
    },
    {
      "field": "password",
      "message": "La contraseña debe tener al menos 8 caracteres",
      "value": "123"
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Respuesta con Paginación
```json
{
  "success": true,
  "message": "Usuarios obtenidos exitosamente",
  "data": [
    {"id": 1, "name": "Usuario 1"},
    {"id": 2, "name": "Usuario 2"}
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Métodos Factory

### Métodos de Éxito

#### `SuccessResult(T data, string message = "Operation completed successfully")`
Crea una respuesta exitosa con datos.

```csharp
// Ejemplo básico
var response = ResponseMessage<UserDto>.SuccessResult(user, "Usuario creado exitosamente");

// Ejemplo con mensaje personalizado
var response = ResponseMessage<List<BookingDto>>.SuccessResult(bookings, "Reservas obtenidas correctamente");
```

#### `SuccessMessage(string message = "Operation completed successfully")`
Crea una respuesta exitosa sin datos.

```csharp
// Ejemplo básico
var response = ResponseMessage<object>.SuccessMessage("Usuario eliminado exitosamente");

// Ejemplo con mensaje personalizado
var response = ResponseMessage<object>.SuccessMessage("Configuración actualizada correctamente");
```

#### `SuccessResult(T data, PaginationInfo pagination, string message = "Data retrieved successfully")`
Crea una respuesta exitosa con datos y paginación.

```csharp
var pagination = new PaginationInfo(1, 10, 100);
var response = ResponseMessage<List<UserDto>>.SuccessResult(users, pagination, "Usuarios obtenidos exitosamente");
```

### Métodos de Error

#### `Error(string message, string? errorCode = null)`
Crea una respuesta de error genérica.

```csharp
// Error básico
var response = ResponseMessage<UserDto>.Error("Error interno del servidor");

// Error con código específico
var response = ResponseMessage<UserDto>.Error("El email ya está registrado", "DUPLICATE_EMAIL");
```

#### `NotFound(string message = "Resource not found", string? errorCode = "NOT_FOUND")`
Crea una respuesta de recurso no encontrado.

```csharp
// Not found básico
var response = ResponseMessage<UserDto>.NotFound();

// Not found personalizado
var response = ResponseMessage<UserDto>.NotFound("Usuario con ID 123 no encontrado", "USER_NOT_FOUND");
```

#### `ValidationError(string message, List<ValidationError> validationErrors, string? errorCode = "VALIDATION_ERROR")`
Crea una respuesta con errores de validación.

```csharp
var validationErrors = new List<ValidationError>
{
    new ValidationError("email", "El formato del email es inválido", "email-invalido"),
    new ValidationError("password", "La contraseña debe tener al menos 8 caracteres", "123")
};

var response = ResponseMessage<UserDto>.ValidationError("Datos de entrada inválidos", validationErrors);
```

#### `ValidationError(string message, Dictionary<string, string[]> modelStateErrors, string? errorCode = "VALIDATION_ERROR")`
Crea una respuesta con errores de validación desde ModelState.

```csharp
var modelStateErrors = new Dictionary<string, string[]>
{
    ["email"] = new[] { "El formato del email es inválido" },
    ["password"] = new[] { "La contraseña debe tener al menos 8 caracteres" }
};

var response = ResponseMessage<UserDto>.ValidationError("Datos de entrada inválidos", modelStateErrors);
```

#### `Unauthorized(string message = "Unauthorized access", string? errorCode = "UNAUTHORIZED")`
Crea una respuesta de acceso no autorizado.

```csharp
var response = ResponseMessage<UserDto>.Unauthorized("Token de acceso expirado", "TOKEN_EXPIRED");
```

#### `Forbidden(string message = "Access forbidden", string? errorCode = "FORBIDDEN")`
Crea una respuesta de acceso prohibido.

```csharp
var response = ResponseMessage<UserDto>.Forbidden("No tienes permisos para acceder a este recurso", "INSUFFICIENT_PERMISSIONS");
```

#### `Conflict(string message = "Resource conflict", string? errorCode = "CONFLICT")`
Crea una respuesta de conflicto de recursos.

```csharp
var response = ResponseMessage<UserDto>.Conflict("El email ya está registrado", "EMAIL_CONFLICT");
```

## Clases de Soporte

### ValidationError
Representa un error de validación específico.

```csharp
public class ValidationError
{
    public string Field { get; set; }        // Campo con error
    public string Message { get; set; }      // Mensaje de error
    public object? Value { get; set; }       // Valor que causó el error
}
```

**Ejemplo de uso:**
```csharp
var error = new ValidationError("email", "El formato del email es inválido", "email-invalido");
```

### PaginationInfo
Información de paginación para respuestas paginadas.

```csharp
public class PaginationInfo
{
    public int Page { get; set; }           // Página actual
    public int PageSize { get; set; }       // Tamaño de página
    public int TotalCount { get; set; }     // Total de elementos
    public int TotalPages { get; set; }     // Total de páginas
    public bool HasNext { get; set; }       // ¿Hay página siguiente?
    public bool HasPrevious { get; set; }   // ¿Hay página anterior?
}
```

**Ejemplo de uso:**
```csharp
var pagination = new PaginationInfo(1, 10, 100);
// Resultado: Page=1, PageSize=10, TotalCount=100, TotalPages=10, HasNext=true, HasPrevious=false
```

### ResponseMetadata
Metadatos adicionales de la respuesta.

```csharp
public class ResponseMetadata
{
    public PaginationInfo? Pagination { get; set; }    // Información de paginación
    public string? ProcessingTime { get; set; }        // Tiempo de procesamiento
    public ServerInfo? ServerInfo { get; set; }        // Información del servidor
}
```

### ServerInfo
Información del servidor.

```csharp
public class ServerInfo
{
    public string Version { get; set; }        // Versión de la API
    public string Environment { get; set; }    // Ambiente (Development, Production)
    public DateTime Timestamp { get; set; }    // Timestamp del servidor
}
```

## Métodos de Extensión

### `WithCorrelationId(string correlationId)`
Agrega un ID de correlación a la respuesta.

```csharp
var response = ResponseMessage<UserDto>.SuccessResult(user)
    .WithCorrelationId("req-12345");
```

### `WithMetadata(ResponseMetadata metadata)`
Agrega metadatos a la respuesta.

```csharp
var metadata = new ResponseMetadata
{
    ProcessingTime = "150ms",
    ServerInfo = new ServerInfo
    {
        Version = "1.0.0",
        Environment = "Production",
        Timestamp = DateTime.UtcNow
    }
};

var response = ResponseMessage<UserDto>.SuccessResult(user)
    .WithMetadata(metadata);
```

## Uso en Controladores

### Ejemplo Básico
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser(CreateUserDto dto)
{
    try
    {
        var user = await _userService.CreateUserAsync(dto);
        var response = ResponseMessage<UserDto>.SuccessResult(user, "Usuario creado exitosamente");
        return Ok(response);
    }
    catch (ValidationException ex)
    {
        var response = ResponseMessage<UserDto>.ValidationError("Datos inválidos", ex.ValidationErrors);
        return BadRequest(response);
    }
    catch (DuplicateEmailException)
    {
        var response = ResponseMessage<UserDto>.Conflict("El email ya está registrado", "DUPLICATE_EMAIL");
        return Conflict(response);
    }
}
```

### Ejemplo con Paginación
```csharp
[HttpGet]
public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 10)
{
    var (users, totalCount) = await _userService.GetUsersPaginatedAsync(page, pageSize);
    var pagination = new PaginationInfo(page, pageSize, totalCount);
    
    var response = ResponseMessage<List<UserDto>>.SuccessResult(users, pagination, "Usuarios obtenidos exitosamente");
    return Ok(response);
}
```

### Ejemplo con Validación de ModelState
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser(CreateUserDto dto)
{
    if (!ModelState.IsValid)
    {
        var response = ResponseMessage<UserDto>.ValidationError("Datos de entrada inválidos", ModelState);
        return BadRequest(response);
    }

    var user = await _userService.CreateUserAsync(dto);
    var successResponse = ResponseMessage<UserDto>.SuccessResult(user, "Usuario creado exitosamente");
    return Ok(successResponse);
}
```

## Códigos de Error Comunes

| Código | Descripción | Uso |
|--------|-------------|-----|
| `VALIDATION_ERROR` | Errores de validación de datos | Datos de entrada inválidos |
| `NOT_FOUND` | Recurso no encontrado | Elemento no existe |
| `UNAUTHORIZED` | Acceso no autorizado | Token inválido o expirado |
| `FORBIDDEN` | Acceso prohibido | Sin permisos suficientes |
| `CONFLICT` | Conflicto de recursos | Email duplicado, etc. |
| `DUPLICATE_EMAIL` | Email duplicado | Usuario ya existe |
| `INVALID_CREDENTIALS` | Credenciales inválidas | Login fallido |
| `TOKEN_EXPIRED` | Token expirado | Renovar token |
| `INSUFFICIENT_PERMISSIONS` | Permisos insuficientes | Acceso denegado |

## Mejores Prácticas

### 1. Usar Mensajes Descriptivos
```csharp
// ✅ Correcto
ResponseMessage<UserDto>.SuccessResult(user, "Usuario creado exitosamente");

// ❌ Incorrecto
ResponseMessage<UserDto>.SuccessResult(user, "OK");
```

### 2. Usar Códigos de Error Específicos
```csharp
// ✅ Correcto
ResponseMessage<UserDto>.Error("El email ya está registrado", "DUPLICATE_EMAIL");

// ❌ Incorrecto
ResponseMessage<UserDto>.Error("El email ya está registrado");
```

### 3. Manejar Errores de Validación
```csharp
if (!ModelState.IsValid)
{
    var response = ResponseMessage<UserDto>.ValidationError("Datos de entrada inválidos", ModelState);
    return BadRequest(response);
}
```

### 4. Incluir Metadatos cuando sea Útil
```csharp
var metadata = new ResponseMetadata
{
    ProcessingTime = $"{stopwatch.ElapsedMilliseconds}ms"
};

var response = ResponseMessage<UserDto>.SuccessResult(user)
    .WithMetadata(metadata);
```

### 5. Usar Paginación para Listas Grandes
```csharp
var pagination = new PaginationInfo(page, pageSize, totalCount);
var response = ResponseMessage<List<UserDto>>.SuccessResult(users, pagination);
```

## Ventajas del Sistema

1. **Consistencia**: Todas las respuestas siguen la misma estructura
2. **Trazabilidad**: Incluye timestamp y correlationId
3. **Flexibilidad**: Soporta diferentes tipos de datos
4. **Mantenibilidad**: Código limpio y reutilizable
5. **Debugging**: Información detallada para desarrollo
6. **Frontend**: Fácil integración con aplicaciones cliente

## Integración con Frontend

### Ejemplo con JavaScript/TypeScript
```typescript
interface ApiResponse<T> {
  success: boolean;
  message: string;
  errorCode?: string;
  data?: T;
  validationErrors?: ValidationError[];
  pagination?: PaginationInfo;
  timestamp: string;
  correlationId?: string;
}

async function createUser(userData: CreateUserDto): Promise<ApiResponse<UserDto>> {
  const response = await fetch('/api/users', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(userData)
  });

  const result: ApiResponse<UserDto> = await response.json();

  if (!result.success) {
    if (result.validationErrors) {
      // Mostrar errores de validación
      showValidationErrors(result.validationErrors);
    } else {
      // Mostrar mensaje de error general
      showErrorMessage(result.message);
    }
  }

  return result;
}
```

Esta documentación proporciona una guía completa para usar la clase `ResponseMessage` de manera efectiva en toda la API PadelYa. 