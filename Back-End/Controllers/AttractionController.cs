using Back_End.DBServices;
using Back_End.Models;
using Microsoft.AspNetCore.Mvc;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttractionController : ControllerBase
    {
        [HttpGet("city/{cityId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Attraction>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Attraction>> GetByCity(int cityId)
        {
            try
            {
                if (cityId <= 0)
                    return BadRequest("Invalid city ID.");

                List<Attraction> attractions = DatabaseServicesAttraction.GetAllAttractionsByCity(cityId);

                if (attractions == null || attractions.Count == 0)
                {
                    return NotFound($"No attractions found for city ID: {cityId}.");
                }
                return Ok(attractions);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpGet("country/{countryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Attraction>> GetByCountry(int countryId)
        {
            try
            {
                List<Attraction> attractions = DatabaseServicesAttraction.GetAllAttractionsByCountry(countryId);
                if (attractions == null || attractions.Count == 0)
                {
                    return NotFound();
                }
                return Ok(attractions);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Attraction> GetByName(string name)
        {
            try
            {
                Attraction attraction = DatabaseServicesAttraction.GetAttractionByName(name);
                if (attraction == null)
                {
                    return NotFound();
                }
                return Ok(attraction);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<int> AddAttraction([FromBody] Attraction attraction)
        {
            try
            {
                int attractionId = DatabaseServicesAttraction.InsertAttraction(attraction);
                return CreatedAtAction(nameof(GetByName), new { name = attraction.Name }, attractionId);
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
        public IActionResult UpdateAttraction(int id, [FromBody] Attraction attraction)
        {
            try
            {
                attraction.Id = id;
                int rowsAffected = DatabaseServicesAttraction.UpdateAttraction(attraction);
                if (rowsAffected == 0)
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
        public IActionResult DeleteAttraction(int id)
        {
            try
            {
                bool isDeleted = DatabaseServicesAttraction.DeleteAttraction(id);
                if (!isDeleted)
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
    }
}
