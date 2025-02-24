namespace ParkNetApp.Pages.ParkingLotView;

public class ToParkModel : PageModel
{
    private ParkNetRepository _repo;
    public ToParkModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;

    [BindProperty]
    public Slot Slot { get; set; }

    //[BindProperty]
    //public IList<Vehicle> UserVehicles { get; set; }

    [BindProperty]
    public IList<Vehicle> AvailableVehiclesToPark { get; set; }

    [BindProperty]
    public Vehicle CurrentVehicle { get; set; }

    public void OnGet(int? id)
    {
        if (id == null)
            NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Slot = _repo.GetSlotById(id.Value);
        //UserVehicles = _repo.GetAvailableVehiclesByUserId(userId);
        AvailableVehiclesToPark = _repo.GetAvailableVehiclesToPark(userId, Slot);
        CurrentVehicle = _repo.GetCurrentlyParkedVehicle(Slot);
    }

    public async Task<IActionResult> OnPostAsync(int? vehicleId, int? slotId )
    {
        if (vehicleId == null || slotId == null)
        {
            return NotFound();
        }

        CurrentVehicle = await _repo.GetVehicleByIdAsync(vehicleId.Value);
        Slot = _repo.GetSlotById(slotId.Value);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Slot.IsOccupied)
            await _repo.UpdateEntriesAndExitsAndSaveAsync(userId, CurrentVehicle, Slot);
        else
            await _repo.CreateNewEntryHistoryAndSaveAsync(userId, CurrentVehicle, Slot);


        return Redirect($"/ParkingLotView/View/{Slot.Floor.ParkingLot.Id}");
    }

}