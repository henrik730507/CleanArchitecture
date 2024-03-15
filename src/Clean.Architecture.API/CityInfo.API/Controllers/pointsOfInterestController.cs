using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsOfInterest")]
public class PointsOfInterestController : ControllerBase
{

  [HttpGet]
  public ActionResult<IEnumerable<PointOfInterestDto>> PointsOfInterest(int cityId)
  {
    var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    return Ok(city.PointsOfInterest);
  }

  [HttpGet("{id}")]
  public ActionResult<PointOfInterestDto> pointOfInterest(int cityId, int id)
  {
    var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
    if (pointOfInterest == null)
    {
      return NotFound();
    }
    return Ok(pointOfInterest);
  }
}
