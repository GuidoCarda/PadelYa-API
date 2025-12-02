# Configuración de Credenciales SMTP

Las credenciales del servidor de email se manejan mediante **User Secrets** de .NET para no exponerlas en el repositorio.

## Pasos para configurar

### 1. Abrir terminal en la carpeta del proyecto

```bash
cd padelya-api
```

### 2. Configurar las credenciales

Ejecutar los siguientes comandos reemplazando los valores con las credenciales proporcionadas:

```bash
dotnet user-secrets set "Smtp:Username" "EMAIL_PROPORCIONADO"
dotnet user-secrets set "Smtp:Password" "PASSWORD_PROPORCIONADO"
dotnet user-secrets set "Smtp:FromEmail" "EMAIL_PROPORCIONADO"
```

### 3. Verificar la configuración

```bash
dotnet user-secrets list
```

Deberías ver algo como:

```
Smtp:Username = ejemplo@gmail.com
Smtp:Password = xxxx xxxx xxxx xxxx
Smtp:FromEmail = ejemplo@gmail.com
```

## ¿Dónde se guardan?

Los secrets se almacenan localmente en tu máquina, **fuera del repositorio**:

- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>\secrets.json`
- **Linux/Mac**: `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`

## Comandos útiles

| Comando                                 | Descripción           |
| --------------------------------------- | --------------------- |
| `dotnet user-secrets list`              | Ver todos los secrets |
| `dotnet user-secrets set "Key" "Value"` | Agregar/actualizar    |
| `dotnet user-secrets remove "Key"`      | Eliminar uno          |
| `dotnet user-secrets clear`             | Eliminar todos        |

## Notas

- Los User Secrets solo funcionan en modo **Development**
- Cada desarrollador debe configurarlos en su máquina local
- **Nunca** commitear credenciales en `appsettings.json`
