namespace ParkNetApp.Data.Entities;

public class Transaction
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public double Amount { get; set; } //Débito (Negativo) ou Crédito (Positivo)
    public string Description { get; set; }

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }

}
