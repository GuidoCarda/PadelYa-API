using Microsoft.AspNetCore.Mvc;
using padelya_api.Models;

namespace padelya_api.Controllers
{
    [Route("api/complex")]
    [ApiController]
    public class ComplexController() : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Complex>> getComplex()
        {
            try
            {
                var complex = new Complex();
                return Ok(complex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }
    }
}