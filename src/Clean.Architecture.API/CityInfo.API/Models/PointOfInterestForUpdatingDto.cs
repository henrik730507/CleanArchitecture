﻿using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models;

public class PointOfInterestForUpdatingDto
{
  [Required(ErrorMessage = "You should provide a name value.")]
  [MaxLength(50)]
  public required string Name { get; set; }

  [MaxLength(200)]
  public string? Description { get; set; }
}
