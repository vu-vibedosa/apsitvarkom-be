﻿namespace Apsitvarkom.Models.DTO;

/// <summary>
/// Data Transfer Object for <see cref="Location" />
/// </summary>
public struct LocationDTO
{
    /// <summary>Property equivalent to <see cref="Location.Longitude" /></summary>
    public double? Longitude { get; set; }

    /// <summary>Property equivalent to <see cref="Location.Latitude" /></summary>
    public double? Latitude { get; set; }
}