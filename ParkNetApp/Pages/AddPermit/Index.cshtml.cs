using System.Configuration;

namespace ParkNetApp.Pages.AddPermit;
[Authorize (Roles ="Member")]
public class IndexModel : PageModel
{
    private ParkNetRepository _repo;
    public IndexModel(ParkNetRepository parkNetRepository)
        => _repo = parkNetRepository;
    public IList<ParkingPermit> ParkingPermits { get; set; }
    public IList<ParkingLot> ParkingLots { get; set; }

    public async Task OnGetAsync()
    {
        ParkingLots = await _repo.GetAllParkingLots();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ParkingPermits = await _repo.GetUserPermitsByUserId(userId);
    }
}