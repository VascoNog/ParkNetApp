using ParkNetApp.Data.Repositories;
using ParkNetApp.Models;

namespace ParkNetApp.Pages.ParkingLotView;

[Authorize]
public class IndexModel : PageModel
{
    public IList<Slot> Slot { get; set; }
    public ParkingLot ParkingLot  { get; set; }
    public IList<ParkingModel> ParkingModel { get; set; } 

    private readonly ParkNetRepository _repo;
    public IndexModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository; 

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        //ParkingLot = await _repo.GetParkingLot(id.Value);

        //Slot = await _repo.GetSlotsFromParkingLot(id.Value);

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