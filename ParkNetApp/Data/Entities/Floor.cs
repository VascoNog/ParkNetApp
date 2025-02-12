namespace ParkNetApp.Data.Entities;
public class Floor
{
    public int Id { get; set; }
    public string Name { get; set; } // Floor 1, Floor 2, etc

    // Foreign Key ParkingLotId
    public int ParkingLotId { get; set; }
    public ParkingLot ParkingLot { get; set; }
}