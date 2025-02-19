namespace ParkNetApp.Models;

public class PermitModel
{
    public int Id { get; set; }
    public int DaysOfPermit { get; set; } 
    public double Value { get; set; }
    public DateOnly ActiveSince { get; set; }
    public DateOnly? ActiveUntil { get; set; }
}
