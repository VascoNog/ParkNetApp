using System.Threading.Tasks;

namespace ParkNetApp.Pages.ParkingLotView;

public class CreateModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    private ParkNetRepository _repo;
    public CreateModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;


    [BindProperty]
    public Slot Slot { get; set; }

    [BindProperty]
    public int CurrentParkingLotId { get; set; }

    [BindProperty]
    public int CurrentNumberOfFloors { get; set; }

    public SelectList VehicleTypes { get; set; }
    public SelectList CurrentFloors { get; set; }


    public async Task<IActionResult> OnGet(int? id) //PK id
    {
        CurrentParkingLotId = id.Value;

        var slots = await _repo.GetSlotsFromParkingLot(id.Value);

        var floors = await _repo.GetFloorsByParkingLotId(id.Value);
        CurrentNumberOfFloors = floors.Count();
        CurrentFloors = new SelectList (floors, "Id", "Name");

        VehicleTypes = new SelectList(await _repo.GetAvailableVehicleSymbols());

        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        if (!_repo.IsSlotCodeValid(Slot.Code, CurrentParkingLotId))
        {
            ModelState.AddModelError("Slot.Code", "Invalid code! It must start with letters, " +
                $"end with numbers, and must not already be in use.");
            return Page();
        }
        Slot.Code = Slot.Code.ToUpper();


        if (Slot.FloorId == -1) //Create New Floor
        {
            Floor newFloor = new()
            {
                Name = "Floor" + (CurrentNumberOfFloors + 1),
                ParkingLotId = CurrentParkingLotId,
            };

            await _repo.AddNewFloor(newFloor);
            await _repo.SaveAllChangesAsync();
            Slot.FloorId = await _repo.GetNewFloorId(CurrentParkingLotId, newFloor.Name);
        }

        await _repo.AddNewSlot(Slot);
        await _repo.SaveAllChangesAsync();

        return Redirect($"/ParkingLotView/View?id={CurrentParkingLotId}");

    }
}
