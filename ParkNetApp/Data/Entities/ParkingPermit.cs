namespace ParkNetApp.Data.Entities;

public class ParkingPermit
{
    public int Id { get; set; }
    public DateTime SartedAt { get; set; }
    public int DaysOfPermit { get; set; } // Daily (1 day), Weekly (15 days), Monthly (31 days), Yearly (366 days)  

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }

    //Foreign ParkingLotId -> A avença deve ser associada a um parque de estacionamento
    public int ParkingLotId { get; set; }
    public ParkingLot ParkingLot { get; set; }

}
