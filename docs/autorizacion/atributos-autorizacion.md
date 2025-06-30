# Atributos de Autorización

## Descripción General

La API PadelYa proporciona un sistema completo de atributos de autorización para proteger endpoints de manera granular y eficiente. Estos atributos verifican los permisos del usuario antes de permitir el acceso a los recursos.

## Atributos Disponibles

### 1. RequirePermissionAttribute

**Descripción**: Requiere un permiso específico para acceder al endpoint.

**Lógica**: Verifica que el usuario tenga exactamente el permiso especificado.

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
- Endpoints que deben estar restringidos a roles específicos

**Ejemplos**:
```csharp
// Gestión de reservas
[RequirePermission(Permissions.Booking.Create)]
[RequirePermission(Permissions.Booking.Edit)]
[RequirePermission(Permissions.Booking.Delete)]

// Gestión de usuarios
[RequirePermission(Permissions.User.Create)]
[RequirePermission(Permissions.User.Edit)]
[RequirePermission(Permissions.User.Deactivate)]

// Gestión de roles
[RequirePermission(Permissions.Role.PermissionAssign)]
```

### 2. RequireAnyPermissionAttribute

**Descripción**: Requiere que el usuario tenga al menos uno de los permisos especificados.

**Lógica**: OR (lógica O) - al menos un permiso debe estar presente.

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

**Ejemplos**:
```csharp
// Acceso flexible a edición de reservas
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]

// Ver usuarios con diferentes niveles de acceso
[RequireAnyPermission(Permissions.User.View, Permissions.User.Admin)]

// Acciones que requieren permisos de módulos diferentes
[RequireAnyPermission(Permissions.Booking.AssignUser, Permissions.User.Admin)]
```

### 3. RequireAllPermissionsAttribute

**Descripción**: Requiere que el usuario tenga todos los permisos especificados.

**Lógica**: AND (lógica Y) - todos los permisos deben estar presentes.

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

**Ejemplos**:
```csharp
// Acciones críticas que requieren doble verificación
[RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]

// Gestión de roles que requiere múltiples permisos
[RequireAllPermissions(Permissions.Role.Edit, Permissions.Role.PermissionAssign)]

// Operaciones que cruzan módulos
[RequireAllPermissions(Permissions.Booking.Create, Permissions.User.View)]
```

### 4. RequireModuleAccessAttribute

**Descripción**: Requiere que el usuario tenga cualquier permiso en el módulo especificado.

**Lógica**: Verifica acceso general al módulo.

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
- Endpoints que son accesibles para cualquiera con acceso al módulo

**Ejemplos**:
```csharp
// Controlador completo con acceso al módulo booking
[Route("api/bookings")]
[ApiController]
[RequireModuleAccess("booking")]
public class BookingController : ControllerBase
{
    // Todos los métodos heredan el requisito de acceso al módulo booking
}

// Endpoint específico con acceso al módulo
[RequireModuleAccess("user")]
public async Task<IActionResult> GetUserProfile()
{
    // Cualquier permiso de user es suficiente
}
```

## Comparación de Atributos

| Atributo | Lógica | Performance | Uso Recomendado |
|----------|--------|-------------|-----------------|
| `RequirePermission` | Un permiso | 1 consulta BD | Permisos únicos y específicos |
| `RequireAnyPermission` | OR (al menos uno) | 1 consulta BD | Acceso flexible con múltiples roles |
| `RequireAllPermissions` | AND (todos) | 1 consulta BD | Seguridad reforzada |
| `RequireModuleAccess` | Módulo completo | 1 consulta BD | Acceso general al módulo |

## Ejemplos Completos de Controladores

### Controlador de Reservas
```csharp
using padelya_api.Constants;
using padelya_api.Attributes;

[Route("api/bookings")]
[ApiController]
public class BookingController : ControllerBase
{
    // Acceso básico - cualquier permiso de booking
    [HttpGet]
    [RequireModuleAccess("booking")]
    public async Task<IActionResult> GetBookings()
    {
        // Ver todas las reservas
    }

    // Ver propias reservas - permiso específico
    [HttpGet("my")]
    [RequirePermission(Permissions.Booking.ViewOwn)]
    public async Task<IActionResult> GetMyBookings()
    {
        // Ver reservas propias
    }

    // Crear reserva - permiso específico
    [HttpPost]
    [RequirePermission(Permissions.Booking.Make)]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        // Crear reserva como usuario
    }

    // Editar reserva - acceso flexible
    [HttpPut("{id}")]
    [RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]
    public async Task<IActionResult> EditBooking(int id, UpdateBookingDto dto)
    {
        // Editar reserva (profesores o admins)
    }

    // Crear reserva como admin - acceso estricto
    [HttpPost("admin")]
    [RequireAllPermissions(Permissions.Booking.Create, Permissions.Booking.Admin)]
    public async Task<IActionResult> CreateBookingAsAdmin(CreateBookingDto dto)
    {
        // Crear reserva manualmente como admin
    }

    // Eliminar reserva - seguridad reforzada
    [HttpDelete("{id}")]
    [RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        // Eliminar reserva (solo admins con permiso específico)
    }

    // Asignar reserva a usuario - permisos de módulos diferentes
    [HttpPut("{id}/assign")]
    [RequireAllPermissions(Permissions.Booking.AssignUser, Permissions.User.View)]
    public async Task<IActionResult> AssignBookingToUser(int id, int userId)
    {
        // Asignar reserva a otro usuario
    }
}
```

### Controlador de Usuarios
```csharp
[Route("api/users")]
[ApiController]
[RequireModuleAccess("user")] // Acceso general al módulo user
public class UserController : ControllerBase
{
    // Ver usuarios - acceso flexible
    [HttpGet]
    [RequireAnyPermission(Permissions.User.View, Permissions.User.Admin)]
    public async Task<IActionResult> GetUsers()
    {
        // Listar usuarios
    }

    // Crear usuario - permiso específico
    [HttpPost]
    [RequirePermission(Permissions.User.Create)]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        // Crear nuevo usuario
    }

    // Editar usuario - permiso específico
    [HttpPut("{id}")]
    [RequirePermission(Permissions.User.Edit)]
    public async Task<IActionResult> EditUser(int id, UpdateUserDto dto)
    {
        // Editar usuario
    }

    // Editar propio perfil - permiso específico
    [HttpPut("profile")]
    [RequirePermission(Permissions.User.EditSelf)]
    public async Task<IActionResult> EditProfile(UpdateProfileDto dto)
    {
        // Editar perfil propio
    }

    // Asignar roles - permisos múltiples
    [HttpPut("{id}/roles")]
    [RequireAllPermissions(Permissions.User.Edit, Permissions.User.AssignRoles)]
    public async Task<IActionResult> AssignUserRoles(int id, List<int> roleIds)
    {
        // Asignar roles a usuario
    }

    // Desactivar usuario - permiso específico
    [HttpPut("{id}/deactivate")]
    [RequirePermission(Permissions.User.Deactivate)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        // Desactivar cuenta de usuario
    }
}
```

### Controlador de Roles
```csharp
[Route("api/roles")]
[ApiController]
[RequireModuleAccess("role")] // Acceso general al módulo role
public class RoleController : ControllerBase
{
    // Ver roles - permiso específico
    [HttpGet]
    [RequirePermission(Permissions.Role.View)]
    public async Task<IActionResult> GetRoles()
    {
        // Listar roles
    }

    // Crear rol - permiso específico
    [HttpPost]
    [RequirePermission(Permissions.Role.Create)]
    public async Task<IActionResult> CreateRole(CreateRoleDto dto)
    {
        // Crear nuevo rol
    }

    // Editar rol - permiso específico
    [HttpPut("{id}")]
    [RequirePermission(Permissions.Role.Edit)]
    public async Task<IActionResult> EditRole(int id, UpdateRoleDto dto)
    {
        // Editar rol
    }

    // Eliminar rol - permiso específico
    [HttpDelete("{id}")]
    [RequirePermission(Permissions.Role.Delete)]
    public async Task<IActionResult> DeleteRole(int id)
    {
        // Eliminar rol
    }

    // Asignar permisos a rol - permiso específico
    [HttpPut("{id}/permissions")]
    [RequirePermission(Permissions.Role.PermissionAssign)]
    public async Task<IActionResult> AssignPermissionsToRole(int id, List<int> permissionIds)
    {
        // Asignar permisos a rol
    }
}
```

## Mejores Prácticas

### 1. Usar Constantes de Permisos
```csharp
// ✅ Correcto - Type safe y refactoring safe
[RequirePermission(Permissions.Booking.Create)]
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]

// ❌ Incorrecto - Propenso a errores tipográficos
[RequirePermission("booking:create")]
[RequireAnyPermission("booking:edit", "booking:admin")]
```

### 2. Combinar Atributos Estratégicamente
```csharp
[Route("api/bookings")]
[ApiController]
[RequireModuleAccess("booking")] // Protección general del módulo
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

### 4. Usar el Atributo Apropiado para Cada Caso
```csharp
// Para un permiso único
[RequirePermission(Permissions.Booking.Create)]

// Para acceso flexible (múltiples roles)
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]

// Para seguridad reforzada
[RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]

// Para acceso general al módulo
[RequireModuleAccess("booking")]
```

## Respuestas de Error

### 401 Unauthorized
- **Causa**: Usuario no autenticado o token inválido
- **Acción**: Redirigir a login

### 403 Forbidden
- **Causa**: Usuario autenticado pero sin permisos suficientes
- **Acción**: Mostrar mensaje de permisos insuficientes


## Resumen

Los atributos de autorización proporcionan un sistema completo y flexible para proteger endpoints:

- **RequirePermission**: Para permisos únicos y específicos
- **RequireAnyPermission**: Para acceso flexible con múltiples roles
- **RequireAllPermissions**: Para seguridad reforzada
- **RequireModuleAccess**: Para acceso general al módulo

Todos los atributos están optimizados para performance y proporcionan mensajes de error claros para una mejor experiencia de desarrollo. 