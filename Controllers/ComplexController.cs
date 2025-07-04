using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs;
using padelya_api.DTOs.Court;
using padelya_api.Models;
using padelya_api.Services;
using padelya_api.Shared;

namespace padelya_api.Controllers
{
    [Route("api/complex")]
    [ApiController]
    public class ComplexController(
        IComplexService complexService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Complex>> getComplex()
        {
            try
            {
                var complex = await complexService.GetComplexAsync();
                return Ok(complex);
            }
            catch (Exception ex)
            {
                var response = ResponseMessage<Complex>
                    .Error($"Error retrieving users: {ex.Message}");

                return StatusCode(500, response);
            }
        }

        [HttpGet("courts")]
        public async Task<ActionResult<List<Court>>> GetCourts()
        {
            try
            {
                var courts = await complexService.GetCourtsAsync();
                return Ok(courts);
            }
            catch (Exception ex)
            {
                var response = ResponseMessage<List<Court>>
                    .Error($"Error retrieving users: {ex.Message}");

                return StatusCode(500, response);
            }
        }

        [HttpPost("complex/courts")]
        public async Task<ActionResult<Court>> CreateCourt(
            [FromBody] CreateCourtDto request
        )
        {
            try
            {
                var court = await complexService.CreateCourtAsync(request);
                return Ok(court);
            }
            catch (Exception ex)
            {
                var response = ResponseMessage<Court>.Error($"Error retrieving court {ex.Message} ");
                return StatusCode(500, response);
            }
        }


        [HttpGet("courts/{id}")]
        public async Task<ActionResult<Court>> GetCourtById(int id)
        {
            try
            {
                var court = await complexService.GetCourtByIdAsync(id);
                return Ok(court);
            }
            catch (Exception ex)
            {
                var response = ResponseMessage<Court>.Error($"Error retrieving court {ex.Message} ");
                return StatusCode(500, response);
            }
        }

        [HttpPut("courts/{id}")]
        public async Task<ActionResult<Court>> UpdateCourt(int id, [FromBody] UpdateCourtDto updateDto)
        {
            try
            {

                var updatedCourt = await complexService
                    .UpdateCourtAsync(id, updateDto);

                if (updatedCourt == null)
                {
                    var notFoundResponse = ResponseMessage<Court>.NotFound($"Court with ID {id} not found");
                    return NotFound(notFoundResponse);
                }

                return Ok(updatedCourt);
            }
            catch (Exception ex)
            {
                var response = ResponseMessage<Court>.Error($"Error retrieving court {ex.Message} ");
                return StatusCode(500, response);
            }
        }

        [HttpDelete("courts/{id}")]
        public async Task<ActionResult> DeleteCourt(int id)
        {
            try
            {
                var result = await complexService.DeleteCourtAsync(id);

                if (!result)
                {
                    var notFoundResponse = ResponseMessage.NotFound($"Court with ID {id} not found or could not be deleted");
                    return NotFound(notFoundResponse);
                }

                var response = ResponseMessage.SuccessMessage("Court deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ResponseMessage.Error($"Error deleting court {ex.Message} ");
                return StatusCode(500, response);
            }
        }
    }
}