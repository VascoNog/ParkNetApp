namespace ParkNetApp.Pages.AddParkingLot;

public class IndexModel : PageModel
{
    private ParkNetRepository _repo;
    public IndexModel(ParkNetRepository parkNetRepository)
        => _repo = parkNetRepository;

    public IList<ParkingLot> ParkingLot { get;set; }

    public async Task OnGetAsync()
    {
        ParkingLot = await _repo.GetAllParkingLots();
    }
}
