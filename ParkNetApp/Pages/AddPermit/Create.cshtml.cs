

namespace ParkNetApp.Pages.AddPermit;
[Authorize]

public class CreateModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    private ParkNetRepository _repo { get; }
    public CreateModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;

    [BindProperty]
    public ParkingPermit ParkingPermit { get; set; }

    public string UserId { get; set; }

    public IActionResult OnGet(int? id)
    {
        ViewData["SLotId"] = new SelectList(_repo.GetAvailableSlots(id.Value), "Id", "Code");
        ViewData["PermitInfoId"] = new SelectList(_repo.GetPermitAvailableDays(), "Id", "DaysOfPermit");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ParkingPermit.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        await _repo.AddNewPermitAndSaveChangesAsync(ParkingPermit);

        return RedirectToPage("./Index");
    }
}
