namespace ParkNetApp.Models;

public class ParkingModel
{
    public int Id { get; set; }
    public int SlotId { get; set; }
    public string SlotCode { get; set; }
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public char SlotType { get; set; }
    public bool IsOccupied { get; set; }

    // Floor Information
    public int FloorId { get; set; }
    public string FloorName { get; set; }

    // Parking Lot Information
    public int ParkingLotId { get; set; }
    public string ParkingLotDesignation { get; set; }
    public string ParkingLotAddress { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
