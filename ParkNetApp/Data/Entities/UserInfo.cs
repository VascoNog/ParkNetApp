namespace ParkNetApp.Data.Entities;

public class UserInfo
{
    //Primary anda Foreign Key Id(User)
    public string Id { get; set; }
    public IdentityUser User { get; set; }

    public string CreditCardNumb { get; set; }
    public DateOnly CCExpDate { get; set; }
    public string DriverLicenseNumber { get; set; }
    public DateOnly DLExpDate { get; set; }
    public double ParkNetCardBalance { get; set; } // Em principio não é necessário
    public bool IsActivated { get; set; } = false; // Default value

}
    