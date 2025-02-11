namespace ParkNetApp.Data.Entities;

public class ActivityHistory
{
    public int Id { get; set; }

    public DateTime EntryAt { get; set; }
    public DateTime ExitAt { get; set; }
    public double Fee { get; set; } // Valor cobrado, Pode ser Null se for avença?

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }


    //Foreign Key VehicleId
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }


    //Foreign Key ParkingId
    public int ParkingId { get; set; }
    public ParkingLot Parking { get; set; }
}
