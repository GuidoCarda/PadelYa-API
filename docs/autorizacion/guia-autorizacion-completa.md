# Guía Completa de Autorización

## Descripción General

El sistema de autorización de PadelYa API proporciona control granular sobre el acceso a recursos basado en permisos y roles. Esta guía cubre todos los aspectos del sistema de autorización.

## Arquitectura del Sistema

### Componentes Principales

1. **Módulos**: Agrupaciones lógicas de funcionalidad
2. **Permisos**: Acciones específicas dentro de los módulos
3. **Roles**: Colecciones de permisos asignados a usuarios
4. **Atributos**: Decoradores para proteger endpoints

### Flujo de Autorización

```
Usuario → JWT Token → Claims de Permisos → Atributo de Autorización → Verificación → Acceso Permitido/Denegado
```

## Estructura de Permisos

### Formato de Permisos
Los permisos siguen el formato: `modulo:accion`

**Ejemplos**:
- `booking:create` - Crear una reserva
- `user:edit` - Editar información de usuario
- `role:permission:assign` - Asignar permisos a roles

### Módulos Disponibles

| Módulo | Descripción | Permisos Principales |
|--------|-------------|---------------------|
| booking | Reservas de canchas | make, create, edit, cancel, view, view_own |
| user | Gestión de usuarios | create, edit, edit_self, view, assign_roles |
| role | Gestión de roles | create, edit, delete, permission:assign, view |
| tournament | Gestión de torneos | create, edit, cancel, view, join, manage_scores |
| class | Gestión de clases | create, edit, cancel, view, join, leave |
| routine | Rutinas de entrenamiento | create, edit, delete, view, assign_user |
| feedback | Feedback de usuarios | create, edit, delete, view |

## Atributos de Autorización

### 1. RequirePermissionAttribute

**Propósito**: Requiere un permiso específico.

**Sintaxis**:
```csharp
[RequirePermission(Permissions.Booking.Create)]
public async Task<IActionResult> CreateBooking()
{
    // Solo usuarios con booking:create pueden acceder
}
```

**Casos de Uso**:
- Acciones específicas que requieren un permiso exacto
- Operaciones sensibles que necesitan control granular
- Endpoints restringidos a roles específicos

### 2. RequireAnyPermissionAttribute

**Propósito**: Requiere al menos uno de los permisos especificados (lógica OR).

**Sintaxis**:
```csharp
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]
public async Task<IActionResult> EditBooking(int id, UpdateBookingDto dto)
{
    // Usuario con booking:edit O booking:admin puede acceder
}
```

**Casos de Uso**:
- Roles flexibles donde diferentes roles pueden acceder a la misma funcionalidad
- Permisos alternativos para la misma acción
- Mantener compatibilidad con roles existentes

### 3. RequireAllPermissionsAttribute

**Propósito**: Requiere todos los permisos especificados (lógica AND).

**Sintaxis**:
```csharp
[RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]
public async Task<IActionResult> DeleteBooking(int id)
{
    // Usuario necesita AMBOS permisos: booking:delete Y booking:admin
}
```

**Casos de Uso**:
- Seguridad reforzada para acciones críticas
- Permisos especializados que requieren combinaciones específicas
- Auditoría estricta con acceso limitado

### 4. RequireModuleAccessAttribute

**Propósito**: Requiere cualquier permiso en el módulo especificado.

**Sintaxis**:
```csharp
[RequireModuleAccess("booking")]
public async Task<IActionResult> GetBookings()
{
    // Cualquier permiso de booking es suficiente
}
```

**Casos de Uso**:
- Acceso general a un módulo completo
- Protección a nivel de controlador
- Endpoints accesibles para cualquiera con acceso al módulo

## Ejemplos Prácticos

### Controlador de Reservas Completo

```csharp
using padelya_api.Constants;
using padelya_api.Attributes;

[Route("api/bookings")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Ver todas las reservas - acceso al módulo
    [HttpGet]
    [RequireModuleAccess("booking")]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    // Ver propias reservas - permiso específico
    [HttpGet("my")]
    [RequirePermission(Permissions.Booking.ViewOwn)]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = GetCurrentUserId();
        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }

    // Crear reserva como usuario - permiso específico
    [HttpPost]
    [RequirePermission(Permissions.Booking.Make)]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        var userId = GetCurrentUserId();
        var booking = await _bookingService.CreateBookingAsync(userId, dto);
        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    // Crear reserva como admin - permisos múltiples
    [HttpPost("admin")]
    [RequireAllPermissions(Permissions.Booking.Create, Permissions.Booking.Admin)]
    public async Task<IActionResult> CreateBookingAsAdmin(CreateBookingAdminDto dto)
    {
        var booking = await _bookingService.CreateBookingAsAdminAsync(dto);
        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    // Editar reserva - acceso flexible
    [HttpPut("{id}")]
    [RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]
    public async Task<IActionResult> EditBooking(int id, UpdateBookingDto dto)
    {
        var booking = await _bookingService.UpdateBookingAsync(id, dto);
        return Ok(booking);
    }

    // Eliminar reserva - seguridad reforzada
    [HttpDelete("{id}")]
    [RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        await _bookingService.DeleteBookingAsync(id);
        return NoContent();
    }

    // Asignar reserva a usuario - permisos de módulos diferentes
    [HttpPut("{id}/assign")]
    [RequireAllPermissions(Permissions.Booking.AssignUser, Permissions.User.View)]
    public async Task<IActionResult> AssignBookingToUser(int id, int userId)
    {
        await _bookingService.AssignBookingToUserAsync(id, userId);
        return NoContent();
    }

    // Marcar como pagada - permiso específico
    [HttpPut("{id}/mark-paid")]
    [RequirePermission(Permissions.Booking.MarkPaid)]
    public async Task<IActionResult> MarkBookingAsPaid(int id)
    {
        await _bookingService.MarkBookingAsPaidAsync(id);
        return NoContent();
    }

    // Obtener reserva específica - acceso al módulo
    [HttpGet("{id}")]
    [RequireModuleAccess("booking")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound();
        
        return Ok(booking);
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst("user_id")?.Value ?? "0");
    }
}
```

### Controlador de Usuarios

```csharp
[Route("api/users")]
[ApiController]
[RequireModuleAccess("user")] // Protección general del módulo
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // Ver usuarios - acceso flexible
    [HttpGet]
    [RequireAnyPermission(Permissions.User.View, Permissions.User.Admin)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // Crear usuario - permiso específico
    [HttpPost]
    [RequirePermission(Permissions.User.Create)]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Editar usuario - permiso específico
    [HttpPut("{id}")]
    [RequirePermission(Permissions.User.Edit)]
    public async Task<IActionResult> EditUser(int id, UpdateUserDto dto)
    {
        var user = await _userService.UpdateUserAsync(id, dto);
        return Ok(user);
    }

    // Editar propio perfil - permiso específico
    [HttpPut("profile")]
    [RequirePermission(Permissions.User.EditSelf)]
    public async Task<IActionResult> EditProfile(UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();
        var user = await _userService.UpdateUserProfileAsync(userId, dto);
        return Ok(user);
    }

    // Asignar roles - permisos múltiples
    [HttpPut("{id}/roles")]
    [RequireAllPermissions(Permissions.User.Edit, Permissions.User.AssignRoles)]
    public async Task<IActionResult> AssignUserRoles(int id, List<int> roleIds)
    {
        await _userService.AssignRolesToUserAsync(id, roleIds);
        return NoContent();
    }

    // Desactivar usuario - permiso específico
    [HttpPut("{id}/deactivate")]
    [RequirePermission(Permissions.User.Deactivate)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        await _userService.DeactivateUserAsync(id);
        return NoContent();
    }

    // Obtener usuario específico - acceso al módulo
    [HttpGet("{id}")]
    [RequireModuleAccess("user")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst("user_id")?.Value ?? "0");
    }
}
```

## Mejores Prácticas

### 1. Usar Constantes de Permisos
```csharp
// ✅ Correcto
[RequirePermission(Permissions.Booking.Create)]
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]

// ❌ Incorrecto
[RequirePermission("booking:create")]
[RequireAnyPermission("booking:edit", "booking:admin")]
```

### 2. Combinar Atributos Estratégicamente
```csharp
[Route("api/bookings")]
[ApiController]
[RequireModuleAccess("booking")] // Protección general
public class BookingController : ControllerBase
{
    [RequirePermission(Permissions.Booking.Create)] // Permisos específicos
    public async Task<IActionResult> CreateBooking() { ... }
}
```

### 3. Documentar Permisos Complejos
```csharp
/// <summary>
/// Edita una reserva. Requiere booking:edit O booking:admin
/// </summary>
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]
public async Task<IActionResult> EditBooking(int id, UpdateBookingDto dto)
{
    // Implementación
}
```

### 4. Usar el Atributo Apropiado
```csharp
// Para un permiso único
[RequirePermission(Permissions.Booking.Create)]

// Para acceso flexible
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]

// Para seguridad reforzada
[RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]

// Para acceso general al módulo
[RequireModuleAccess("booking")]
```

## Manejo de Errores

### Códigos de Estado HTTP

| Código | Significado | Causa |
|--------|-------------|-------|
| 401 | Unauthorized | Usuario no autenticado o token inválido |
| 403 | Forbidden | Usuario autenticado pero sin permisos suficientes |

### Respuestas de Error

```csharp
// 401 Unauthorized
{
    "error": "Unauthorized",
    "message": "Token inválido o expirado"
}

// 403 Forbidden
{
    "error": "Forbidden",
    "message": "No tienes permisos para acceder a este recurso"
}
```

## Seguridad

### Consideraciones de Seguridad
1. **Validación de Tokens**: Todos los tokens se validan en cada request
2. **Verificación de Permisos**: Los permisos se verifican contra la base de datos
3. **Auditoría**: Todos los intentos de acceso se registran
4. **Rate Limiting**: Implementar rate limiting para prevenir abuso


## Resumen

El sistema de autorización proporciona:

1. **Control Granular**: Permisos específicos para cada acción
2. **Flexibilidad**: Múltiples formas de verificar permisos
3. **Performance**: Consultas optimizadas a la base de datos
4. **Seguridad**: Validación robusta de tokens y permisos
5. **Mantenibilidad**: Uso de constantes y documentación clara

Este sistema permite un control preciso sobre quién puede acceder a qué recursos en la API, manteniendo la seguridad y facilitando el desarrollo. 