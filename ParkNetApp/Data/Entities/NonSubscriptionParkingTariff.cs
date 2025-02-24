namespace ParkNetApp.Data.Entities;

public class NonSubscriptionParkingTariff  // Entidade isolada de tarifas de estacionamento não subscritas (sem avença)
{
    public int Id { get; set; }
    public double Limit { get; set; }
    public double Tariff { get; set; }
    public DateOnly ActiveSince { get; set; }
    public DateOnly? ActiveUntil { get; set; }
}
