namespace ParkNetApp.Data.Entities;

public class EntryAndExitHistory  // Para ter um histórico de entradas e saídas dos parques da rede.
{
    public int Id { get; set; }
    public DateTime EntryAt { get; set; }
    public DateTime? ExitAt { get; set; }

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }


    //Foreign Key VehicleId
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }


    //Foreign Key SlotId
    public int SlotId { get; set; }
    public Slot Slot { get; set; }


    //Foreign Key TransactionId
    public int MovementId { get; set; }
    public Movement Movement { get; set; }
}
