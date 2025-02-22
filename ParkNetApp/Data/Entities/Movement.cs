namespace ParkNetApp.Data.Entities;

public class Movement
{
    public int Id { get; set; }
    public DateTime? TransactionDate { get; set; } // Pode ser data futura, no caso das avenças por exemplo?
    public double Amount { get; set; } //Débito (Negativo) ou Crédito (Positivo)
    public string TransactionType { get; set; }

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }

}
