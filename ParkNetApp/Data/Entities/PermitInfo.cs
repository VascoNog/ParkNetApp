namespace ParkNetApp.Data.Entities;

public class PermitInfo
{
    public int Id { get; set; }
    public int DaysOfPermit { get; set; } //  7 days,15 days, 31 days, 366 days, 730 days
    public double Value { get; set; }
    public DateOnly ActiveSince { get; set; }
    public DateOnly? ActiveUntil { get; set; }
}
