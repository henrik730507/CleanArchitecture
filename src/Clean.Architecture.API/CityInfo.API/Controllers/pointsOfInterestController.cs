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

  [HttpGet("{id}", Name = "GetPointOfInterest")]
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

  [HttpPost]
  public ActionResult<PointOfInterestDto> CreatePointOfInterest(
    int cityId, 
    PointOfInterestForCreationDto pointOfInterest)
  {
    var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

    var finalPointOfInterest = new PointOfInterestDto()
    {
      Id = ++maxPointOfInterestId,
      Name = pointOfInterest.Name,
      Description = pointOfInterest.Description
    };

    city.PointsOfInterest.Add(finalPointOfInterest);

    return CreatedAtRoute("GetPointOfInterest",
      new
      {
        cityId = city.Id,
        id = finalPointOfInterest.Id
      },
      finalPointOfInterest);
  }
}
