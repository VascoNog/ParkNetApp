namespace ParkNetApp.Pages.MovementAndBilling;

public class IndexModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public IndexModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public IList<Movement> Movement { get;set; }

    [BindProperty]
    public SelectList UserEmails { get; set; }

    [BindProperty]
    public SelectList TransactionTypes { get; set; }

    [BindProperty]
    public IList<DateTime> TransactionDates { get; set; }

    [BindProperty]
    public string SelectedEmail { get; set; }

    [BindProperty]
    public string SelectedTransactionType { get; set; }

    public async Task OnGetAsync(string emailFilter, string typeFilter)
    {
        var userEmails = await _context.Users.Select(u => u.Email).Distinct().OrderBy(e => e).ToListAsync();
        UserEmails = new SelectList(userEmails);

        List<string> transTypes = ["Parking", "Permit", "Withdraw", "Adding Funds to Card"];
        TransactionTypes = new SelectList(transTypes);

        var allMovements = await _context.Movements.Include(m => m.User).ToListAsync();

        if (!string.IsNullOrEmpty(emailFilter))
        {
            allMovements = allMovements.Where(m => m.User.Email == emailFilter).ToList();
        }

        if (!string.IsNullOrEmpty(typeFilter))
        {
            allMovements = allMovements.Where(m => m.TransactionType == typeFilter).ToList();
        }

        Movement = allMovements;
        SelectedEmail = emailFilter;
        SelectedTransactionType = typeFilter;
    }
}
