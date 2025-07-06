# Modelado de la Gestión de Reservas

> **Nota importante:**
> En este sistema, **los registros de CourtSlot no existen por defecto**. Un CourtSlot solo se crea en el momento en que se va a generar una reserva (Booking), una clase (Lesson) o un partido de torneo (TournamentMatch). No hay slots pre-generados para cada cancha y horario: la existencia de un CourtSlot implica que ese bloque de tiempo está efectivamente reservado para alguna actividad.

Este documento resume cómo se modela la gestión de reservas en el sistema PadelYa, considerando las entidades principales: **CourtSlot**, **Booking**, **Lesson** y **TournamentMatch**.

## Entidades Clave

- **CourtSlot**: Representa un bloque de tiempo específico en una cancha. Es la unidad básica de disponibilidad y puede estar asociada a una reserva, una clase o un partido de torneo. **Solo existe si hay una actividad concreta reservada para ese horario y cancha.**
- **Booking**: Representa una reserva estándar de un CourtSlot por parte de una persona (jugador/cliente).
- **Lesson**: Representa una clase programada en un CourtSlot, con un profesor y alumnos inscriptos.
- **TournamentMatch**: Representa un partido de torneo que se juega en un CourtSlot específico.

## Relaciones y Reglas

- Cada **CourtSlot** puede estar asociado a **una sola** de las siguientes entidades:
  - Un **Booking** (reserva estándar)
  - Una **Lesson** (clase)
  - Un **TournamentMatch** (partido de torneo)
- La relación es 1:1: un CourtSlot solo puede estar ocupado por una de estas entidades a la vez.
- Cada **Booking**, **Lesson** y **TournamentMatch** referencia al **CourtSlot** que ocupa.

## Disponibilidad y Validación

> **Importante:** Antes de crear un **Booking**, **Lesson** o **TournamentMatch**, el sistema debe verificar que el **CourtSlot** para ese horario y cancha **no exista** aún. Si ya existe un CourtSlot para ese bloque, se debe devolver un error y no permitir la creación de la reserva, clase o partido.

## Diagrama de Texto Simplificado

```
CourtSlot
   |\
   | \-- Booking (0..1)
   | \-- Lesson (0..1)
   | \-- TournamentMatch (0..1)

Booking --------> CourtSlot
Lesson  --------> CourtSlot
TournamentMatch -> CourtSlot
```

## Flujo de Reserva

1. **Reserva estándar:**
   - El usuario solicita reservar una cancha en un horario específico.
   - **El sistema verifica que no exista ya un CourtSlot para ese horario y cancha.**
   - Si no existe, se crea el CourtSlot y el **Booking** asociado.
   - Si ya existe, se devuelve un error y no se crea la reserva.
   - El CourtSlot queda marcado como reservado.

2. **Clase:**
   - Un profesor programa una **Lesson** en un horario y cancha específicos.
   - **El sistema verifica que no exista ya un CourtSlot para ese horario y cancha.**
   - Si no existe, se crea el CourtSlot y la Lesson asociada.
   - Si ya existe, se devuelve un error y no se crea la clase.
   - Los alumnos se inscriben a la Lesson.
   - El CourtSlot queda ocupado por la clase.

3. **Partido de torneo:**
   - El sistema de torneos asigna un **TournamentMatch** a un horario y cancha específicos.
   - **El sistema verifica que no exista ya un CourtSlot para ese horario y cancha.**
   - Si no existe, se crea el CourtSlot y el TournamentMatch asociado.
   - Si ya existe, se devuelve un error y no se crea el partido.
   - El CourtSlot queda reservado para ese partido.

## Resumen Visual (UML simplificado)

@startuml
class CourtSlot
class Booking
class Lesson
class TournamentMatch

CourtSlot "1" -- "0..1" Booking
CourtSlot "1" -- "0..1" Lesson
CourtSlot "1" -- "0..1" TournamentMatch
Booking "*" -- "1" CourtSlot
Lesson "*" -- "1" CourtSlot
TournamentMatch "*" -- "1" CourtSlot
@enduml

## Consideraciones

- **No existen CourtSlots pre-generados**: la existencia de un CourtSlot implica que ese horario y cancha están ocupados por una actividad concreta.
- El modelo asegura que no haya solapamientos: un CourtSlot solo puede estar ocupado por una entidad a la vez.
- **Siempre se debe validar que no exista ya un CourtSlot para ese horario y cancha antes de crear una reserva, clase o partido de torneo.**
- Permite distinguir fácilmente entre reservas estándar, clases y partidos de torneo.
- Facilita la consulta de disponibilidad y la gestión de conflictos de agenda. 