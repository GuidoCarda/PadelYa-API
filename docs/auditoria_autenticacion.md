# Auditoría de Autenticación

## 📋 Descripción General

El sistema implementa un registro completo de auditoría para todas las acciones relacionadas con la autenticación de usuarios mediante la entidad `LoginAudit`.

## 🎯 Qué se Audita

### Acciones Registradas

Todas las acciones se definen en el enum `LoginAuditAction`:

| Acción         | Descripción              | Cuándo se Registra                                 |
| -------------- | ------------------------ | -------------------------------------------------- |
| `Login`        | Inicio de sesión exitoso | Al hacer login con credenciales válidas            |
| `Logout`       | Cierre de sesión         | Al cerrar sesión explícitamente o automáticamente  |
| `RefreshToken` | Renovación de token      | Al renovar el access token usando el refresh token |

### Información Capturada

Cada registro de auditoría contiene:

```csharp
public class LoginAudit
{
    public int Id { get; set; }
    public int UserId { get; set; }              // Usuario que realizó la acción
    public LoginAuditAction Action { get; set; }  // Tipo de acción
    public DateTime Timestamp { get; set; }       // Fecha y hora (UTC)
    public string? IpAddress { get; set; }        // Dirección IP del cliente
    public string? UserAgent { get; set; }        // Navegador/dispositivo usado
    public string? Notes { get; set; }            // Notas adicionales
}
```

## 🔄 Flujo Normal de Auditoría

### 1. Login Exitoso

```
Usuario → POST /api/auth/login
    ↓
Validación de credenciales
    ↓
Verificar sesiones abiertas (ver caso especial #1)
    ↓
✓ Registrar: Login
    ↓
Generar tokens JWT
```

### 2. Logout Manual

```
Usuario → POST /api/auth/logout (con JWT)
    ↓
Extraer userId del token
    ↓
✓ Registrar: Logout
    ↓
Frontend limpia tokens locales
```

### 3. Refresh Token

```
Usuario → POST /api/auth/refresh-token
    ↓
Validar refresh token
    ↓
✓ Registrar: RefreshToken
    ↓
Generar nuevos tokens
```

## ⚠️ Casos Especiales

### Caso #1: Múltiples Sesiones (Login sin Logout Previo)

**Escenario**: Un usuario inicia sesión en un dispositivo sin haber cerrado sesión en otro dispositivo.

**Comportamiento**:

1. Al hacer el nuevo login, el sistema verifica si el último registro de auditoría del usuario es un `Login`
2. Si es así, registra automáticamente un `Logout` con nota explicativa antes del nuevo `Login`
3. La nota incluye la IP desde donde se hizo el nuevo login

**Implementación**: Método `CloseOpenSessionsAsync()` en `AuthService`

**Ejemplo en la base de datos**:

```
LoginAudits para UserId = 5:
┌────┬────────┬───────┬─────────────────────┬───────────────┬──────────────────────────────────────────┐
│ Id │ UserId │ Action│ Timestamp           │ IpAddress     │ Notes                                    │
├────┼────────┼───────┼─────────────────────┼───────────────┼──────────────────────────────────────────┤
│ 10 │   5    │ Login │ 2025-10-31 10:00:00 │ 192.168.1.5   │ null                                     │
│ 11 │   5    │ Logout│ 2025-10-31 14:30:00 │ null          │ Sesión cerrada automáticamente por       │
│    │        │       │                     │               │ nuevo login desde 192.168.1.10           │
│ 12 │   5    │ Login │ 2025-10-31 14:30:00 │ 192.168.1.10  │ null                                     │
└────┴────────┴───────┴─────────────────────┴───────────────┴──────────────────────────────────────────┘
```

**Beneficios**:

- ✅ Cada `Login` tiene su correspondiente `Logout`
- ✅ Fácil identificar sesiones cerradas automáticamente
- ✅ Trazabilidad completa de qué dispositivo/IP causó el cierre

### Caso #2: Token Expirado (Sin Logout Explícito)

**Escenario**: El usuario deja su sesión abierta y el token expira naturalmente.

**Comportamiento**:

- No se registra logout automático por expiración
- Al hacer el siguiente login, se aplica el Caso #1
- Esto es intencional para no saturar la base de datos con logouts por expiración

## 📊 Consultas Útiles

### Ver sesiones activas (último registro es Login)

```csharp
var activeSessions = await _context.LoginAudits
    .GroupBy(a => a.UserId)
    .Select(g => g.OrderByDescending(a => a.Timestamp).FirstOrDefault())
    .Where(a => a.Action == LoginAuditAction.Login)
    .ToListAsync();
```

### Ver historial de un usuario

```csharp
var userAuditHistory = await _context.LoginAudits
    .Where(a => a.UserId == userId)
    .OrderByDescending(a => a.Timestamp)
    .ToListAsync();
```

### Detectar logins desde múltiples IPs en corto tiempo

```csharp
var suspiciousLogins = await _context.LoginAudits
    .Where(a => a.UserId == userId
        && a.Action == LoginAuditAction.Login
        && a.Timestamp > DateTime.UtcNow.AddHours(-1))
    .Select(a => a.IpAddress)
    .Distinct()
    .CountAsync();

if (suspiciousLogins > 2)
{
    // Posible actividad sospechosa
}
```

## 🔒 Seguridad y Privacidad

- Todas las contraseñas se hashean con PBKDF2
- Los timestamps se guardan en UTC
- Las IPs se registran para seguridad, pero pueden anonimizarse si es requerido por GDPR
- El campo `Notes` permite agregar contexto adicional sin modificar el esquema

## 🚀 Futuras Mejoras

Potenciales mejoras a considerar:

1. **FailedLogin**: Implementar registro de intentos fallidos para detectar ataques de fuerza bruta
2. **SessionId**: Agregar un identificador único de sesión para rastrear la misma sesión entre login y logout
3. **Device Fingerprint**: Capturar más información del dispositivo para mejor trazabilidad
4. **Retención de Datos**: Política de eliminación de registros antiguos (ej: después de 1 año)
5. **Alertas**: Notificar al usuario cuando se detecten logins desde nuevos dispositivos/IPs

## 📝 Notas de Implementación

- **Backend**: `AuthService.cs` contiene toda la lógica de auditoría
- **Frontend**: `auth-client.ts` y `auth-actions.ts` llaman al endpoint de logout para registrar el cierre de sesión
- **Base de Datos**: Tabla `LoginAudits` con índice en `UserId` y `Timestamp` para consultas eficientes
