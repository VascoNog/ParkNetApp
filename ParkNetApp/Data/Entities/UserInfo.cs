namespace ParkNetApp.Data.Entities;

public class UserInfo
{
    public int Id { get; set; }
    public string CreditCardNumb { get; set; }
    public DateOnly CCExpDate { get; set; }
    public double CCBalance { get; set; }
    public string DriverLicenseNumber { get; set; }
    public DateOnly DLExpDate { get; set; }

    //Foreign Key UserId
    public string UserId { get; set; }
    public IdentityUser User { get; set; }

}
