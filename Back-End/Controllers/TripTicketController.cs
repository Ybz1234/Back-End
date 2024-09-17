using Back_End.DBServices;
using Back_End.Models;
using Microsoft.AspNetCore.Mvc;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripTicketController : ControllerBase
    {
        [HttpPost("GetFlightTickets")]
        public IActionResult GetFlightTickets([FromBody] FlightTicketRequest request)
        {
            if (request.Cities == null || request.Cities.Length < 2)
            {
                return BadRequest("At least two cities are required.");
            }

            try
            {
                var dbServices = new DatabaseServicesTripTicket();
                var flightTickets = dbServices.GetFlightTickets(request.Cities, request.UserId);

                if (flightTickets == null || flightTickets.Length == 0)
                {
                    return NotFound("No flights found for the specified route.");
                }

                return Ok(flightTickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
