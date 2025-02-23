using ParkNetApp.Data.Repositories;
using ParkNetApp.Models;

namespace ParkNetApp.Pages.ParkingLotView;

[Authorize]
public class ViewModel : PageModel
{
    public IList<Slot> Slot { get; set; }
    public ParkingLot ParkingLot  { get; set; }
    public IList<ParkingModel> ParkingModel { get; set; } 

    private readonly ParkNetRepository _repo;
    public ViewModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository; 

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ParkingLot = await _repo.GetParkingLot(id.Value);

        ParkingModel = await _repo.GetParkingLotSchema(id.Value);
        if (ParkingModel == null)
        {
            return NotFound();
        }

        return Page();
    }



    //public async Task<IActionResult> OnPostAsync(int slotId, int parkingLotId)
    //{


    //    return RedirectToPage(new { id = parkingLotId });
    //}
}