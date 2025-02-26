﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ParkNetApp.Data.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string PlateNumber { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public bool isParked { get; set; } = false;

    //Foreign Key VehicleTypeId
    public int VehicleTypeId { get; set; }
    public VehicleType VehicleType { get; set; }

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }

}