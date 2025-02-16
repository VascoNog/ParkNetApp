namespace ParkNetApp.Data.Entities;

public class Slot
{
    public int Id { get; set; }
    public string Code { get; set; } // A1, H12, R5, etc
    public char SlotType { get; set; } // 'C' - Car, 'M' - Motorcycle;
    public bool IsOccupied { get; set; } = false;

    //Foreign Key FloorId
    public int FloorId { get; set; }
    public Floor Floor { get; set; }

}

