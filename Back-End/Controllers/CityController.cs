using Back_End.Models;
using BackEndDemo.DBServices;
using Microsoft.AspNetCore.Mvc;

namespace BackEndDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<City[]> Get()
        {
            try
            {
                List<City> cities = DatabaseServicesCity.GetAllCities();
                return Ok(cities.ToArray());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(City))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            try
            {
                City city = DatabaseServicesCity.GetCityById(id);
                if (city == null)
                    return NotFound($"City with id: {id} wasn't found.");
                return Ok(city);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(City))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] City value)
        {
            try
            {
                if (value == null)
                    return BadRequest("City is null.");
                if (value.Id != 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Cannot specify Id for new City.");

                value.Id = DatabaseServicesCity.InsertCity(value);

                return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(int id, [FromBody] City value)
        {
            try
            {
                if (value == null || value.Id != id)
                    return BadRequest();

                int rowsAffected = DatabaseServicesCity.UpdateCity(value);
                if (rowsAffected == 0)
                    return NotFound($"City with id: {id} wasn't found, can't update.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                bool isDeleted = DatabaseServicesCity.DeleteCity(id);
                if (!isDeleted)
                    return NotFound($"City with id: {id} wasn't found, can't delete.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
