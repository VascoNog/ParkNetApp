namespace ParkNetApp.Pages.ParkingLotView;
[Authorize] 
public class OptionsModel : PageModel
{
    public ParkNetRepository _repo;
    public OptionsModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;

    public Slot Slot { get; set; }

    public bool CanPerformAction { get; set; }

    public IActionResult OnGet(int? id) 
    {
        if (id == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Slot = _repo.GetSlotById(id.Value);

        CanPerformAction = _repo.CanPerformAction(Slot, userId);

        return Page(); 
    }
}

