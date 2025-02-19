namespace ParkNetApp.Data.Entities;

public class ParkingPermit
{
    public int Id { get; set; }
    public DateTime StartedAt { get; set; }

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }

    //Foreign SlotId -> A avença deve ser associada a um Slot
    public int SlotId { get; set; }
    public Slot Slot { get; set; }

    //Foreign PermitInfo
    public int PermitInfoId { get; set; }
    public PermitInfo PermitInfo { get; set; }

}
