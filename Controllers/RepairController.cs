
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Repair;
using padelya_api.Models.Repair;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
  // [Authorize]
  [ApiController]
  [Route("api/repairs")]
  public class RepairController : ControllerBase
  {
    private readonly IRepairService _repairService;

    public RepairController(IRepairService repairService)
    {
      _repairService = repairService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
      [FromQuery] string? email,
      [FromQuery] string? status,
      [FromQuery] string? startDate,
      [FromQuery] string? endDate
    )
    {
      try
      {
        var repairs = await _repairService
          .GetAllAsync(email, status, startDate, endDate);

        return Ok(ResponseMessage<IEnumerable<Repair>>
          .SuccessResult(repairs, "Reparaciones obtenidas correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<IEnumerable<Repair>>.Error($"Error al obtener reparaciones: {ex.Message}"));
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      try
      {
        var repair = await _repairService.GetByIdAsync(id);
        if (repair == null)
          return NotFound(ResponseMessage<Repair>.NotFound($"Reparacion con ID {id} no encontrada"));

        return Ok(ResponseMessage<Repair>.SuccessResult(repair, "Reparacion obtenida correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<Repair>.Error($"Error al obtener reparacion: {ex.Message}"));
      }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRepairDto dto)
    {
      try
      {
        var repair = await _repairService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = repair.Id }, ResponseMessage<Repair>.SuccessResult(repair, "Reparacion creada correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<Repair>.Error($"Error al crear reparacion: {ex.Message}"));
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRepairDto dto)
    {
      try
      {
        var repair = await _repairService.UpdateAsync(id, dto);
        if (repair == null)
          return NotFound(ResponseMessage<Repair>.NotFound($"Reparacion con ID {id} no encontrada"));

        return Ok(ResponseMessage<Repair>.SuccessResult(repair, "Reparacion actualizada correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<Repair>.Error($"Error al actualizar reparacion: {ex.Message}"));
      }
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
      try
      {
        var repair = await _repairService.CancelAsync(id);
        if (repair == null)
          return NotFound(ResponseMessage<Repair>
            .NotFound($"Reparacion con ID {id} no encontrada"));

        return Ok(ResponseMessage<Repair>
          .SuccessResult(repair, "Reparacion cancelada correctamente"));
      }
      catch (Exception ex)
      {
        return BadRequest(ResponseMessage<Repair>
          .Error($"Error al cancelar reparacion: {ex.Message}"));
      }
    }


    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
      try
      {
        var repair = await _repairService.UpdateStatusAsync(id, dto);
        return Ok(ResponseMessage<Repair>.SuccessResult(repair, "Estado actualizado correctamente"));
      }
      catch (InvalidOperationException ex)
      {
        return Conflict(ResponseMessage<Repair>.Error(ex.Message));
      }
      catch (KeyNotFoundException)
      {
        return NotFound(ResponseMessage<Repair>.NotFound($"Reparación {id} no encontrada"));
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ResponseMessage<Repair>.Error(ex.Message));
      }
    }
  }
}