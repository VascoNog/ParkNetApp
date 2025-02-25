

namespace ParkNetApp.Pages.AddPermit;
[Authorize]

public class CreateModel : PageModel
{
    public IList<Slot> Slots { get; set; }
    public IList<PermitInfo> PermitInfos { get; set; }
    private ParkNetRepository _repo { get; }
    public CreateModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;

    [BindProperty]
    public int SelectedSlotId { get; set; }

    [BindProperty]
    public int SelectedPermitInfoId { get; set; }

    public IActionResult OnGet(int? id)
    {
        Slots = _repo.GetAvailableSlots(id.Value);  
        PermitInfos = _repo.GetPermitAvailableDays();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ParkingPermit ParkingPermit = new()
        {
            StartedAt = DateTime.Now,
            SlotId = SelectedSlotId,
            PermitInfoId = SelectedPermitInfoId,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        await _repo.AddNewPermitAndSaveChangesAsync(ParkingPermit);
        await _repo.AddMovementForPermitAndSaveAsync(ParkingPermit);

        return RedirectToPage("./Index");
    }
}
