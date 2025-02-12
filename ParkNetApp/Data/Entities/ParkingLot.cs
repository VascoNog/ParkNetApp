namespace ParkNetApp.Data.Entities;

public class ParkingLot
{
    public int Id { get; set; }
    public string Designation { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PLIC { get; set; } // Parking Lot Identification Code ID+City+Country?
    public DateTime ActiveAt { get; set; }
    public string Layout { get; set; }
    public string Description { get; set; }

}
