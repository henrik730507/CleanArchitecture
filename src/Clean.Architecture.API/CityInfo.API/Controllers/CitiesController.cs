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

  [HttpGet("{id}")]
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


