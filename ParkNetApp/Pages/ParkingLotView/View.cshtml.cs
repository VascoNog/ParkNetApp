using ParkNetApp.Data.Repositories;
using ParkNetApp.Models;

namespace ParkNetApp.Pages.ParkingLotView;
[Authorize]
public class ViewModel : PageModel
{

    private ParkNetRepository _repo;
    public ViewModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository; 

    public IList<Slot> Slot { get; set; }
    public ParkingLot ParkingLot  { get; set; }
    public IList<ParkingModel> ParkingModel { get; set; } 

    public IList<string> SlotsInUseByUser { get; set; }
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ParkingLot = await _repo.GetParkingLot(id.Value);
        ParkingModel = await _repo.GetParkingLotSchema(id.Value);
        if (ParkingModel == null)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        SlotsInUseByUser = await _repo.GetSlotsWhereUserIsParked(id.Value, userId);

        return Page();
    }
}