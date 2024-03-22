using Asp.Versioning;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;


[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/cities")]

[Authorize]
public class CitiesController : ControllerBase
{
  private readonly CitiesDataStore _citiesDataStore;
  public CitiesController(CitiesDataStore citiesDataStore)
  {
      _citiesDataStore = citiesDataStore;
  }

  [HttpGet]
  public ActionResult<IEnumerable<CityDto>> GetCities()
  {
    return Ok(_citiesDataStore.Cities);
  }

  /// <summary>
  /// Get a city by id
  /// </summary>
  /// <param name="id">The id of the city to get</param>
  /// <returns>An IActionResult</returns>
  /// <response code="200">Returns the requested city</response>
  [HttpGet("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public ActionResult<CityDto> GetCity(int id)
  {
    var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
    if (city == null)
    {
      return NotFound();
    }

    return Ok(city);
  }
}


