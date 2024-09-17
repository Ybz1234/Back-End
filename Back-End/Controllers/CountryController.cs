using Back_End.DBServices;
using Back_End.Models;
using Microsoft.AspNetCore.Mvc;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Country[]> Get()
        {
            try
            {
                List<Country> countries = DatabaseServicesCountry.GetAllCountries();
                return Ok(countries.ToArray());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            try
            {
                Country country = DatabaseServicesCountry.GetCountryById(id);
                if (country == null)
                    return NotFound($"City with id: {id} wasn't found.");
                return Ok(country);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Country value)
        {
            try
            {
                if (value == null)
                    return BadRequest("Country is null.");
                if (value.Id != 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Cannot specify Id for new City.");

                value.Id = DatabaseServicesCountry.InsertCountry(value);

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
        public IActionResult Put(int id, [FromBody] Country value)
        {
            try
            {
                if (value == null || value.Id != id)
                    return BadRequest();

                int rowsAffected = DatabaseServicesCountry.UpdateCountry(value);
                if (rowsAffected == 0)
                    return NotFound($"Country with id: {id} wasn't found, can't update.");

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

                bool isDeleted = DatabaseServicesCountry.DeleteCountry(id);
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
