using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsOfInterest")]
[Authorize]
public class PointsOfInterestController : ControllerBase
{
  private readonly CitiesDataStore _citiesDataStore;
  public PointsOfInterestController(CitiesDataStore citiesDataStore)
  {
    _citiesDataStore = citiesDataStore;
  }

  [HttpGet]
  public ActionResult<IEnumerable<PointOfInterestDto>> PointsOfInterest(int cityId)
  {
    var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    return Ok(city.PointsOfInterest);
  }

  [HttpGet("{id}", Name = "GetPointOfInterest")]
  public ActionResult<PointOfInterestDto> pointOfInterest(int cityId, int id)
  {
    var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
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
    var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

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

  [HttpPut("{id}")]
  public ActionResult UpdatePointOfInterest(
    int cityId,
    int id,
    PointOfInterestForUpdatingDto pointOfInterest)
  {
    var city = _citiesDataStore.Cities
      .FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    var pointOfInterestFromStore = city.PointsOfInterest
      .FirstOrDefault(p => p.Id == id);
    if (pointOfInterestFromStore == null)
    {
      return NotFound();
    }

    pointOfInterestFromStore.Name = pointOfInterest.Name;
    pointOfInterestFromStore.Description = pointOfInterest.Description;

    return NoContent();
  }

  [HttpPatch("{id}")]
  public ActionResult PatchPointOfInterest(
    int cityId,
    int id,
    JsonPatchDocument<PointOfInterestForUpdatingDto> patchDocument)
  {
    var city = _citiesDataStore.Cities
      .FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    var pointOfInterestFromStore = city.PointsOfInterest
      .FirstOrDefault(p => p.Id == id);
    if (pointOfInterestFromStore == null)
    {
      return NotFound();
    }

    var pointOfInterestToPatch = new PointOfInterestForUpdatingDto
    {
      Name = pointOfInterestFromStore.Name,
      Description = pointOfInterestFromStore.Description,
    };

    patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    if (!TryValidateModel(pointOfInterestToPatch))
    {
      return BadRequest(ModelState);
    }

    pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
    pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

    return NoContent();
  }

  [HttpDelete("{id}")]
  public ActionResult DeletePointOfInterest(
   int cityId,
   int id)
  {
    var city = _citiesDataStore.Cities
      .FirstOrDefault(x => x.Id == cityId);
    if (city == null)
    {
      return NotFound();
    }

    var pointOfInterestFromStore = city.PointsOfInterest
      .FirstOrDefault(p => p.Id == id);
    if (pointOfInterestFromStore == null)
    {
      return NotFound();
    }

    city.PointsOfInterest.Remove(pointOfInterestFromStore);

    return NoContent();
  }
}
