
namespace ParkNetApp.Pages.AddVehicle;

[Authorize (Roles="Member")]
public class IndexModel : PageModel
{
    private ParkNetRepository _repo;
    public IList<Vehicle> Vehicles { get;set; }

    public IndexModel(ParkNetRepository parkNetRepository) 
        => _repo = parkNetRepository;

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Vehicles = await _repo.GetVehiclesByUserId(userId);
    }
}
