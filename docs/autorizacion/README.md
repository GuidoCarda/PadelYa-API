# Documentación de Autorización

## Descripción General

Esta sección contiene toda la documentación relacionada con el sistema de autorización de la API PadelYa. El sistema proporciona control granular sobre el acceso a recursos basado en permisos y roles.

## 📚 Índice de Documentación

### 🔐 Conceptos Básicos
- [Sistema de Autorización](./sistema-autorizacion.md) - Descripción general del sistema de permisos y roles
- [Constantes de Permisos](./constantes-permisos.md) - Permisos disponibles y su uso
- [Atributos de Autorización](./atributos-autorizacion.md) - Cómo proteger endpoints con atributos

### 🛠️ Guías Prácticas
- [Guía Completa de Autorización](./guia-autorizacion-completa.md) - Guía completa con ejemplos prácticos
- [Atributos de Múltiples Permisos](./atributos-multiples-permisos.md) - Uso de atributos para múltiples permisos

### 📋 Referencias
- [Códigos de Error](./codigos-error.md) - Códigos de estado HTTP y mensajes de error

## 🎯 Inicio Rápido

### 1. Proteger un Endpoint Básico
```csharp
[RequirePermission(Permissions.Booking.Create)]
public async Task<IActionResult> CreateBooking()
{
    // Solo usuarios con booking:create pueden acceder
}
```

### 2. Acceso Flexible (Múltiples Permisos)
```csharp
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]
public async Task<IActionResult> EditBooking()
{
    // Usuario con booking:edit O booking:admin puede acceder
}
```

### 3. Seguridad Reforzada
```csharp
[RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]
public async Task<IActionResult> DeleteBooking()
{
    // Usuario necesita AMBOS permisos
}
```

### 4. Acceso al Módulo
```csharp
[RequireModuleAccess("booking")]
public async Task<IActionResult> GetBookings()
{
    // Cualquier permiso de booking es suficiente
}
```

## 📋 Atributos Disponibles

| Atributo | Lógica | Uso |
|----------|--------|-----|
| `RequirePermission` | Un permiso | `[RequirePermission(Permissions.Booking.Create)]` |
| `RequireAnyPermission` | OR (al menos uno) | `[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]` |
| `RequireAllPermissions` | AND (todos) | `[RequireAllPermissions(Permissions.Booking.Delete, Permissions.Booking.Admin)]` |
| `RequireModuleAccess` | Módulo completo | `[RequireModuleAccess("booking")]` |

## 🔧 Configuración

### 1. Registrar Servicios
```csharp
// Program.cs
builder.Services.AddScoped<IPermissionService, PermissionService>();
```

### 2. Configurar JWT
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
        };
    });
```

### 3. Usar Middleware
```csharp
// Program.cs
app.UseAuthentication();
app.UseAuthorization();
```

## 📝 Mejores Prácticas

### 1. Usar Constantes
```csharp
// ✅ Correcto
[RequirePermission(Permissions.Booking.Create)]

// ❌ Incorrecto
[RequirePermission("booking:create")]
```

### 2. Combinar Atributos
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

### 3. Documentar Permisos
```csharp
/// <summary>
/// Edita una reserva. Requiere booking:edit O booking:admin
/// </summary>
[RequireAnyPermission(Permissions.Booking.Edit, Permissions.Booking.Admin)]
public async Task<IActionResult> EditBooking() { ... }
```

## 🚨 Códigos de Error

| Código | Significado | Acción |
|--------|-------------|--------|
| 401 | Unauthorized | Token inválido o expirado - Redirigir a login |
| 403 | Forbidden | Sin permisos suficientes - Mostrar mensaje de error |

## 📚 Recursos Adicionales

- [Constantes de Permisos](../Constants/Permissions.cs) - Definición de todos los permisos
- [PermissionService](../Services/PermissionService.cs) - Implementación del servicio de permisos
- [Atributos de Autorización](../Attributes/) - Implementación de los atributos

## 🤝 Contribuir

Al agregar nuevos permisos o funcionalidades de autorización:

1. **Actualizar constantes**: Agregar nuevos permisos en `Constants/Permissions.cs`
2. **Documentar**: Actualizar esta documentación
3. **Seeding**: Actualizar datos de prueba en la base de datos

---
