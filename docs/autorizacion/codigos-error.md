# Códigos de Error en Autorización

## Descripción General

El sistema de autorización utiliza códigos de estado HTTP específicos para comunicar el resultado de las verificaciones de autenticación y autorización. Esta documentación explica cada código y cómo manejarlo.

## Códigos de Estado HTTP

### 401 Unauthorized

**Significado**: El usuario no está autenticado o el token es inválido.

**Causas**:
- Token no presente en el request
- Token expirado
- Token malformado
- Token inválido (firma incorrecta)

**Ejemplo de Request**:
```http
GET /api/bookings
Authorization: Bearer invalid_token
```

**Respuesta**:
```json
{
  "error": "Unauthorized",
  "message": "Token inválido o expirado",
  "statusCode": 401
}
```

**Manejo en Frontend**:
```typescript
if (response.status === 401) {
  // Token expirado o inválido
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  redirectToLogin();
}
```

### 403 Forbidden

**Significado**: El usuario está autenticado pero no tiene permisos suficientes.

**Causas**:
- Usuario no tiene el permiso específico requerido
- Usuario no tiene acceso al módulo
- Usuario no tiene todos los permisos requeridos (para RequireAllPermissions)

**Ejemplo de Request**:
```http
POST /api/bookings
Authorization: Bearer valid_token_without_booking_create_permission
```

**Respuesta**:
```json
{
  "error": "Forbidden",
  "message": "No tienes permisos para acceder a este recurso",
  "statusCode": 403,
  "requiredPermission": "booking:create"
}
```

**Manejo en Frontend**:
```typescript
if (response.status === 403) {
  // Usuario sin permisos suficientes
  showPermissionDeniedMessage();
  // Opcional: ocultar elementos de UI que requieren ese permiso
  hideUnauthorizedElements();
}
```

## Implementación en el Backend

### RequirePermissionAttribute
```csharp
public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
{
    var user = context.HttpContext.User;
    
    // Verificar autenticación
    if (!user.Identity?.IsAuthenticated ?? true)
    {
        context.Result = new UnauthorizedResult(); // 401
        return;
    }
    
    // Verificar permisos
    if (!await permissionService.HasPermissionAsync(userId, _permission))
    {
        context.Result = new ForbidResult(); // 403
        return;
    }
}
```

### RequireAnyPermissionAttribute
```csharp
public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
{
    var user = context.HttpContext.User;
    
    // Verificar autenticación
    if (!user.Identity?.IsAuthenticated ?? true)
    {
        context.Result = new UnauthorizedResult(); // 401
        return;
    }
    
    // Verificar si tiene al menos un permiso
    if (!await permissionService.HasAnyPermissionAsync(userId, _permissions))
    {
        context.Result = new ForbidResult(); // 403
        return;
    }
}
```

### RequireAllPermissionsAttribute
```csharp
public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
{
    var user = context.HttpContext.User;
    
    // Verificar autenticación
    if (!user.Identity?.IsAuthenticated ?? true)
    {
        context.Result = new UnauthorizedResult(); // 401
        return;
    }
    
    // Verificar que tenga todos los permisos
    if (!await permissionService.HasAllPermissionsAsync(userId, _permissions))
    {
        context.Result = new ForbidResult(); // 403
        return;
    }
}
```

### RequireModuleAccessAttribute
```csharp
public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
{
    var user = context.HttpContext.User;
    
    // Verificar autenticación
    if (!user.Identity?.IsAuthenticated ?? true)
    {
        context.Result = new UnauthorizedResult(); // 401
        return;
    }
    
    // Verificar acceso al módulo
    if (!await permissionService.HasModuleAccessAsync(userId, _moduleName))
    {
        context.Result = new ForbidResult(); // 403
        return;
    }
}
```


## Resumen de Códigos de Error

| Código | Significado | Causa | Acción Frontend |
|--------|-------------|-------|-----------------|
| 401 | Unauthorized | Token inválido/expirado | Redirigir a login |
| 403 | Forbidden | Sin permisos suficientes | Mostrar mensaje de error |

## Practicas recomendadas 

### 1. Manejo Consistente
- Siempre usar los mismos códigos de estado para los mismos tipos de error
- Proporcionar mensajes de error claros y útiles
- Incluir información adicional cuando sea relevante

### 2. Logs de Seguridad
- Registrar todos los intentos de acceso fallidos
- Incluir información del usuario y endpoint
- Usar niveles de log apropiados

### 3. Frontend
- Manejar 401 automáticamente con refresh de token
- Mostrar mensajes de error apropiados para 403
- Ocultar elementos de UI que requieren permisos no disponibles

### 4. Seguridad
- No exponer información sensible en mensajes de error
- Registrar intentos de acceso para auditoría
- Implementar rate limiting para prevenir abuso 