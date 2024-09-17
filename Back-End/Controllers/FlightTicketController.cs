using Microsoft.AspNetCore.Mvc;
using Back_End.Models;
using Back_End.DBServices;

namespace BackEndDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightTicketController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<FlightTicket>> GetAllFlightTickets()
        {
            try
            {
                List<FlightTicket> flightTickets = DatabaseServicesFlightTicket.GetAllFlightTickets();
                return Ok(flightTickets);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FlightTicket> GetFlightTicketById(int id)
        {
            try
            {
                FlightTicket flightTicket = DatabaseServicesFlightTicket.GetFlightTicketById(id);
                if (flightTicket == null)
                {
                    return NotFound();
                }
                return Ok(flightTicket);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult CreateFlightTicket(FlightTicket flightTicket)
        {
            try
            {
                DatabaseServicesFlightTicket.CreateFlightTicket(flightTicket);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateFlightTicket(int id, FlightTicket flightTicket)
        {
            try
            {
                bool updated = DatabaseServicesFlightTicket.UpdateFlightTicket(id, flightTicket);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteFlightTicket(int id)
        {
            try
            {
                bool deleted = DatabaseServicesFlightTicket.DeleteFlightTicket(id);
                return !deleted ? NotFound() : NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }
    }
}
