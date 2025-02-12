namespace ParkNetApp.Data.Entities;

public class PermitPrice
{
    public int Id { get; set; }
    public string PermitType { get; set; } // Daily, Weekly, Monthly
    public double Value { get; set; }
    public DateOnly ActiveUntil { get; set; }
    public bool IsActive { get; set; }
}
