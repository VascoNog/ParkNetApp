namespace ParkNetApp.Data.Entities;
public class EmailBox
{
    public int Id { get; set; }
    public string Subject { get; set; } 
    public string Description { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    //Foreign SlotId 
    public string RecipientId { get; set; }
    public IdentityUser Recipient { get; set; }
}
