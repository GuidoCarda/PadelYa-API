# Servicios de Torneos - Arquitectura

## Descripción General

Este directorio contiene todos los servicios relacionados con la gestión de torneos en PadelYa. Los servicios están diseñados siguiendo principios SOLID y el patrón de diseño de Repository/Service.

---

## Servicios Disponibles

### 1. `TournamentService` (ITournamentService)
**Responsabilidad**: CRUD básico de torneos

**Métodos principales**:
- `CreateTournamentAsync()` - Crear nuevo torneo
- `GetTournamentsAsync()` - Listar torneos
- `GetTournamentByIdAsync()` - Obtener torneo específico
- `UpdateTournamentAsync()` - Actualizar torneo
- `DeleteTournamentAsync()` - Eliminar torneo
- `UpdateTournamentStatusAsync()` - Cambiar estado del torneo
- `EnrollPlayerAsync()` - Inscribir jugador/pareja
- `CancelEnrollmentAsync()` - Cancelar inscripción

**Dependencias**:
- `PadelYaDbContext`

---

### 2. `BracketGenerationService` (IBracketGenerationService)
**Responsabilidad**: Generación de cuadros de eliminación (brackets)

**Métodos principales**:
- `GenerateTournamentBracketAsync(int tournamentId)` - Generar bracket básico
- `GenerateTournamentBracketAsync(int tournamentId, bool autoSchedule, List<int>? courtIds)` - Generar bracket con auto-programación
- `GetTournamentBracketAsync()` - Obtener bracket existente

**Lógica de generación**:
1. Valida que el torneo esté en estado "Abierto para Inscripción"
2. Calcula el tamaño del bracket (potencia de 2 más cercana)
3. Genera todas las fases del torneo
4. Crea partidos de la primera fase con parejas asignadas
5. Crea partidos de fases siguientes vacíos (se llenan con ganadores)
6. Opcionalmente, programa automáticamente la primera fase

**Algoritmo de bracket**:
```
Parejas: 8
Bracket Size: 8 (potencia de 2)
Número de rondas: log2(8) = 3

Fase 1: 4 partidos (8 parejas)
Fase 2: 2 partidos (4 parejas)
Fase 3: 1 partido (2 parejas) - FINAL
```

**Dependencias**:
- `PadelYaDbContext`
- `IAutoSchedulingService`

---

### 3. `AutoSchedulingService` (IAutoSchedulingService) ⭐ NUEVO
**Responsabilidad**: Programación automática de partidos con validación de disponibilidad

**Métodos principales**:
- `AutoScheduleMatchesAsync()` - Programa una lista de partidos
- `FindNextAvailableSlotAsync()` - Encuentra el siguiente slot disponible

**Algoritmo de búsqueda**:
```
Para cada día en el rango de fechas:
    Para cada cancha en la lista:
        Para cada slot de 90 minutos:
            ¿Está disponible?
                → Verificar CourtSlots existentes
                → Verificar Bookings activos (ReservedPaid, ReservedDeposit)
                → Verificar Lessons
                → Verificar TournamentMatches
            Si está disponible:
                → Asignar y continuar con siguiente partido
            Si no está disponible:
                → Continuar búsqueda
```

**Validaciones críticas**:
- ✅ Verifica conflictos con reservas de usuarios (Bookings)
- ✅ Solo considera conflictos las reservas pagadas o señadas
- ✅ Respeta horarios de apertura/cierre de canchas
- ✅ No programa partidos en fechas pasadas
- ✅ Valida contra clases (Lessons) y otros partidos de torneo

**Configuración**:
```csharp
// Parámetros configurables
int durationMinutes = 90;           // Duración del partido
TimeSpan searchStartTime = 8:00;    // Hora de inicio de búsqueda
TimeSpan searchEndTime = 23:00;     // Hora de fin de búsqueda
```

**Dependencias**:
- `PadelYaDbContext`

---

### 4. `MatchSchedulingService` (IMatchSchedulingService)
**Responsabilidad**: Programación manual de partidos individuales

**Métodos principales**:
- `AssignMatchScheduleAsync()` - Asignar horario a un partido específico
- `UnassignMatchScheduleAsync()` - Desasignar horario de un partido

**Validaciones**:
- Verifica que el partido existe
- Verifica que la cancha existe
- Verifica disponibilidad del slot (sin conflictos)
- Valida que la fecha esté dentro del rango del torneo

**Diferencia con AutoSchedulingService**:
- `MatchSchedulingService`: Programa **un partido específico** con horario específico (manual)
- `AutoSchedulingService`: Programa **múltiples partidos** buscando slots automáticamente

**Dependencias**:
- `PadelYaDbContext`

---

### 5. `MatchResultService` (IMatchResultService)
**Responsabilidad**: Registro de resultados y avance automático de ganadores

**Métodos principales**:
- `RegisterMatchResultAsync()` - Registrar resultado de un partido
- `UpdateMatchResultAsync()` - Actualizar resultado existente
- `AdvanceWinnerToNextRound()` (privado) - Avanzar ganador automáticamente ⭐ MEJORADO

**Lógica de avance automático**:
```csharp
// Cálculo del partido destino
int targetMatchIndex = completedMatchIndex / 2;

// Asignación según paridad del índice
if (completedMatchIndex % 2 == 0)
    targetMatch.CoupleOneId = winnerId;  // Partido par
else
    targetMatch.CoupleTwoId = winnerId;  // Partido impar

// Ejemplo práctico:
// Cuartos de Final:
//   Match 0 (índice 0, par) → Ganador a Semifinal Match 0, posición 1
//   Match 1 (índice 1, impar) → Ganador a Semifinal Match 0, posición 2
//   Match 2 (índice 2, par) → Ganador a Semifinal Match 1, posición 1
//   Match 3 (índice 3, impar) → Ganador a Semifinal Match 1, posición 2
```

**Validaciones**:
- Verifica que ambas parejas estén asignadas
- Valida que el ganador sea una de las dos parejas del partido
- Valida formato del resultado (2 o 3 sets)
- Valida puntajes de pádel (0-7)

**Actualizaciones automáticas**:
1. Avanza ganador al siguiente partido
2. Actualiza estado del partido destino a "Listo" si tiene ambas parejas
3. Actualiza fase actual del torneo si todos los partidos de la fase están completos
4. Marca torneo como "Finalizado" si es la final

**Dependencias**:
- `PadelYaDbContext`

---

## Flujo de Datos

### Creación y Ejecución de Torneo

```
1. CREATE TOURNAMENT
   TournamentService.CreateTournamentAsync()
   ↓
   Estado: "AbiertoParaInscripcion"

2. ENROLLMENT
   TournamentService.EnrollPlayerAsync()
   ↓
   Parejas inscritas

3. GENERATE BRACKET (con auto-programación)
   BracketGenerationService.GenerateTournamentBracketAsync(id, autoSchedule: true)
   ↓
   ├─ Genera estructura de fases y partidos
   ├─ Asigna parejas a primera fase
   └─ AutoSchedulingService.AutoScheduleMatchesAsync()
      ↓
      └─ Busca slots disponibles
         └─ Valida contra Bookings
            ↓
            Primera fase programada

4. PLAY MATCHES & REGISTER RESULTS
   MatchResultService.RegisterMatchResultAsync()
   ↓
   ├─ Registra resultado
   ├─ AdvanceWinnerToNextRound()
   │  ↓
   │  ├─ Asigna ganador a siguiente fase
   │  └─ Actualiza estado del torneo
   └─ Respuesta con información de avance

5. AUTO-SCHEDULE NEXT PHASE (opcional)
   AutoSchedulingService.AutoScheduleMatchesAsync()
   ↓
   Siguiente fase programada

6. REPEAT STEPS 4-5 until FINAL
   ↓
   Estado: "Finalizado"
```

---

## Diagrama de Dependencias

```
TournamentController
    ├─ ITournamentService
    │   └─ PadelYaDbContext
    ├─ IBracketGenerationService
    │   ├─ PadelYaDbContext
    │   └─ IAutoSchedulingService ⭐
    │       └─ PadelYaDbContext
    ├─ IMatchSchedulingService
    │   └─ PadelYaDbContext
    ├─ IMatchResultService
    │   └─ PadelYaDbContext
    └─ IAutoSchedulingService ⭐
        └─ PadelYaDbContext
```

---

## Modelos de Datos Relacionados

### Tournament
```csharp
public class Tournament
{
    public int Id { get; set; }
    public string CurrentPhase { get; set; }          // Ej: "Cuartos de Final"
    public TournamentStatus TournamentStatus { get; set; }
    public string Title { get; set; }
    public int Quota { get; set; }
    public DateTime TournamentStartDate { get; set; }
    public DateTime TournamentEndDate { get; set; }
    
    // Navegación
    public List<TournamentEnrollment> Enrollments { get; set; }
    public List<TournamentPhase> TournamentPhases { get; set; }
}
```

### TournamentPhase
```csharp
public class TournamentPhase
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public string PhaseName { get; set; }      // Ej: "Final", "Semifinales"
    public int PhaseOrder { get; set; }        // 1, 2, 3...
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // Navegación
    public Tournament Tournament { get; set; }
    public List<TournamentBracket> Brackets { get; set; }
}
```

### TournamentBracket
```csharp
public class TournamentBracket
{
    public int Id { get; set; }
    public int PhaseId { get; set; }
    
    // Navegación
    public TournamentPhase Phase { get; set; }
    public List<TournamentMatch> Matches { get; set; }
}
```

### TournamentMatch
```csharp
public class TournamentMatch
{
    public int Id { get; set; }
    public string TournamentMatchState { get; set; }  // "Pendiente", "Programado", "Completed", etc.
    public string Result { get; set; }                // "6-4, 6-3"
    public int? CoupleOneId { get; set; }
    public int? CoupleTwoId { get; set; }
    public int? WinnerCoupleId { get; set; }
    public int BracketId { get; set; }
    public int? CourtSlotId { get; set; }             // ⭐ Relación con programación
    
    // Navegación
    public Couple? CoupleOne { get; set; }
    public Couple? CoupleTwo { get; set; }
    public Couple? WinnerCouple { get; set; }
    public TournamentBracket Bracket { get; set; }
    public CourtSlot? CourtSlot { get; set; }         // ⭐ CRÍTICO para validación
}
```

### CourtSlot ⭐ CLAVE PARA VALIDACIÓN
```csharp
public class CourtSlot
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public CourtSlotStatus Status { get; set; }
    
    // Relaciones 1:1 (solo una será no nula)
    public Booking Booking { get; set; }              // ⭐ Reserva de usuario
    public Lesson Lesson { get; set; }                 // Clase
    public TournamentMatch TournamentMatch { get; set; } // Partido de torneo
}
```

---

## Estados del Sistema

### TournamentStatus (Enum)
```csharp
public enum TournamentStatus
{
    AbiertoParaInscripcion,   // Inscripciones abiertas
    EnProgreso,                // Torneo en curso
    Finalizado,                // Torneo completado
    Cancelado                  // Torneo cancelado
}
```

### TournamentMatchState (String)
```csharp
// Estados posibles:
"Pendiente"     // Sin programar ni parejas completas
"Bye"           // Una sola pareja, pasa automáticamente
"Listo"         // Ambas parejas asignadas, listo para programar
"Programado"    // Horario asignado
"Completed"     // Resultado registrado
```

### BookingStatus (Enum) ⭐ CRÍTICO
```csharp
public enum BookingStatus
{
    PendingPayment,         // NO genera conflicto
    ReservedPaid,           // ⭐ GENERA CONFLICTO
    ReservedDeposit,        // ⭐ GENERA CONFLICTO
    CancelledByClient,      // NO genera conflicto
    CancelledByAdmin,       // NO genera conflicto
    CancelledBySystem       // NO genera conflicto
}
```

---

## Configuración e Inyección de Dependencias

En `Program.cs`:

```csharp
// Servicios de torneos
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IBracketGenerationService, BracketGenerationService>();
builder.Services.AddScoped<IMatchSchedulingService, MatchSchedulingService>();
builder.Services.AddScoped<IMatchResultService, MatchResultService>();
builder.Services.AddScoped<IAutoSchedulingService, AutoSchedulingService>(); // ⭐ NUEVO
```

---

## Testing

### Unit Tests Recomendados

#### BracketGenerationService
```csharp
[Fact]
public async Task GenerateBracket_With8Teams_Creates3Phases()
{
    // Arrange: 8 equipos inscritos
    // Act: Generar bracket
    // Assert: 3 fases creadas (Cuartos, Semis, Final)
}

[Fact]
public async Task GenerateBracket_WithAutoSchedule_SchedulesFirstPhase()
{
    // Arrange: Torneo con 8 equipos, canchas disponibles
    // Act: Generar con autoSchedule = true
    // Assert: Primera fase programada sin conflictos
}
```

#### AutoSchedulingService
```csharp
[Fact]
public async Task AutoSchedule_WithBookingConflict_SkipsConflictingSlot()
{
    // Arrange: Reserva pagada a las 10:00 AM
    // Act: Auto-programar partido
    // Assert: Partido NO programado a las 10:00 AM
}

[Fact]
public async Task FindNextAvailableSlot_WithNoConflicts_ReturnsFirstSlot()
{
    // Arrange: Cancha disponible todo el día
    // Act: Buscar slot
    // Assert: Retorna 8:00 AM (primer slot)
}
```

#### MatchResultService
```csharp
[Fact]
public async Task RegisterResult_AdvancesWinnerToCorrectMatch()
{
    // Arrange: Bracket con 8 equipos
    // Act: Registrar resultado del Match 0
    // Assert: Ganador en Semifinal Match 0, posición 1
}

[Fact]
public async Task RegisterResult_WhenAllPhaseDone_UpdatesCurrentPhase()
{
    // Arrange: Todos los partidos de una fase excepto uno
    // Act: Registrar resultado del último partido
    // Assert: Tournament.CurrentPhase actualizado
}
```

---

## Extensibilidad

### Agregar Nuevos Tipos de Torneo

Para agregar tipos de torneo (ej: round-robin, suizo):

1. Crear nuevo servicio que implemente `IBracketGenerationService`
2. Implementar lógica específica de generación
3. Registrar en `Program.cs` con nombre específico
4. Usar Factory Pattern para seleccionar servicio según tipo

### Agregar Nuevas Validaciones

Para agregar validaciones adicionales de disponibilidad:

1. Extender método `IsSlotAvailableAsync()` en `AutoSchedulingService`
2. Agregar nuevas verificaciones (ej: eventos especiales, mantenimiento)

### Agregar Notificaciones

Para notificar a parejas:

1. Crear `INotificationService`
2. Inyectar en `MatchResultService` y `AutoSchedulingService`
3. Llamar al notificar en momentos clave:
   - Cuando se programa un partido
   - Cuando se avanza a la siguiente fase
   - Cuando se completa el torneo

---

## Mejores Prácticas

### Al Usar los Servicios

✅ **DO**:
- Siempre verificar que el torneo existe antes de operar
- Validar permisos del usuario antes de operaciones críticas
- Manejar excepciones y retornar mensajes descriptivos
- Usar transacciones para operaciones complejas
- Loguear operaciones importantes

❌ **DON'T**:
- No llamar directamente al DbContext desde controllers
- No hardcodear IDs de canchas o torneos
- No asumir que un slot está disponible sin verificar
- No modificar el estado del torneo manualmente sin validar

### Performance

Para torneos grandes (>32 parejas):
- Considerar programación en background (Hangfire)
- Paginar resultados de brackets
- Implementar caché para datos de fases/partidos

---

## Troubleshooting

### Problema: AutoScheduling no encuentra slots

**Diagnóstico**:
```sql
-- Ver slots ocupados en el rango de fechas
SELECT cs.*, b.Status as BookingStatus
FROM CourtSlots cs
LEFT JOIN Bookings b ON cs.Id = b.CourtSlotId
WHERE cs.Date BETWEEN '2024-03-15' AND '2024-03-20'
    AND cs.Status = 'Active'
ORDER BY cs.Date, cs.StartTime;
```

**Soluciones**:
1. Extender rango de fechas del torneo
2. Agregar más canchas
3. Cancelar reservas no pagadas

### Problema: Ganador no avanza correctamente

**Diagnóstico**:
```csharp
// Verificar índices de partidos
var matches = await _context.TournamentMatches
    .Where(m => m.BracketId == bracketId)
    .OrderBy(m => m.Id)
    .ToListAsync();

// Loguear índices para depurar
foreach (var (match, index) in matches.Select((m, i) => (m, i)))
{
    Console.WriteLine($"Match {match.Id} at index {index}");
}
```

---

## Documentación Adicional

- `docs/TOURNAMENT_AUTO_SCHEDULING.md` - Guía completa de uso
- `docs/CHANGELOG_TOURNAMENT_FEATURES.md` - Changelog detallado

---

**Última actualización**: Diciembre 2025  
**Mantenedor**: Equipo de Desarrollo PadelYa

