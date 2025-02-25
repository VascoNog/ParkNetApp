using System.Threading.Tasks;

namespace ParkNetApp.Pages.MovementAndBilling;
[Authorize]
public class CreateModel : PageModel
{
    private ParkNetRepository _repo;
    public CreateModel(ParkNetRepository parkNetRepository)
        => _repo = parkNetRepository;

    [BindProperty]
    public Movement Movement { get; set; }

    public SelectList TransTypesOptions { get; set; }

    public async Task<IActionResult> OnGet()
    {
        //var users = await _repo.GetUsers();
        ViewData["UserId"] = new SelectList(await _repo.GetUsers(), "Id", "Email");
        List<string> transTypes = ["Parking", "Permit", "Withdraw", "Adding Funds to Card"];
        TransTypesOptions = new SelectList(transTypes);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["UserId"] = new SelectList(await _repo.GetUsers(), "Id", "Email");
            return Page();
        }

        if (Movement.TransactionDate is null)
            Movement.TransactionDate = DateTime.UtcNow;

        Movement.Amount = Math.Abs(Movement.Amount);
        List<string> typesWithNegativeValues = ["Parking", "Permit", "Withdraw"];
        if (typesWithNegativeValues.Contains(Movement.TransactionType))
            Movement.Amount = - Movement.Amount;


        await _repo.AddMovementAndSaveAsync(Movement);

        return RedirectToPage("./Index");
    }
}
