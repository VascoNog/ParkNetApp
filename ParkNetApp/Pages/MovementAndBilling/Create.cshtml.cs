namespace ParkNetApp.Pages.MovementAndBilling;
[Authorize]
public class CreateModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public CreateModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Movement Movement { get; set; }

    public SelectList TransTypesOptions { get; set; }

    public IActionResult OnGet()
    {
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
        List<string> transTypes = ["Parking", "Permit", "Withdraw", "Adding Funds to Card"];
        TransTypesOptions = new SelectList(transTypes);

        return Page();
    }

    // For more information, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return Page();
        }

        if (Movement.TransactionDate is null)
            Movement.TransactionDate = DateTime.UtcNow;

        Movement.Amount = Math.Abs(Movement.Amount);
        List<string> typesWithNegativeValues = ["Parking", "Permit", "Withdraw"];
        if (typesWithNegativeValues.Contains(Movement.TransactionType))
            Movement.Amount = - Movement.Amount;


        _context.Movements.Add(Movement);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
