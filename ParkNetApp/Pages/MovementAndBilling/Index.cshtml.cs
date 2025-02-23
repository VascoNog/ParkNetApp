using System.Threading.Tasks;

namespace ParkNetApp.Pages.MovementAndBilling;

public class IndexModel : PageModel
{
    private ParkNetRepository _repo;

    public IndexModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;
    public IList<Movement> Movement { get; set; } = new List<Movement>(); 
    public SelectList? UserEmails { get; set; }
    public SelectList TransactionTypes { get; set; }
    public string? SelectedEmail { get; set; }
    public string? SelectedTransactionType { get; set; }
    public double? UserBalance { get; set; }
    public EmailBox NewEmail { get; set; } = new EmailBox(); 

    public async Task OnGetAsync(string? emailFilter, string? typeFilter)
    {
        var userEmails = await _repo.GetAllUserDistinctEmails();
        UserEmails = new SelectList(userEmails);

        List<string> transTypes = ["Parking", "Permit", "Withdraw", "Adding Funds to Card"];
        TransactionTypes = new SelectList(transTypes);

        var allMovements = await _repo.GetAllMovementsIncludingUser();

        if (!string.IsNullOrEmpty(emailFilter))
        {
            allMovements = allMovements.Where(m => m.User.Email == emailFilter).ToList();
            UserBalance = await _repo.GetBalanceByUserEmail(emailFilter);
        }

        if (!string.IsNullOrEmpty(typeFilter))
        {
            allMovements = allMovements.Where(m => m.TransactionType == typeFilter).ToList();
        }

        Movement = allMovements;
        SelectedEmail = emailFilter;
        SelectedTransactionType = typeFilter;
    }

    public async Task<IActionResult> OnPostAsync(string? recipientId, double? userBalance)
    {
        if (string.IsNullOrEmpty(recipientId) || userBalance == null)
        {
            return RedirectToPage();
        }

        NewEmail.RecipientId = recipientId;
        NewEmail.Subject = "Negative Balance";
        NewEmail.Description = $"Hello, your balance is {userBalance?.ToString("c")}. " +
        $"Please top up your ParkNet card to continue using ParkNet Services without any issues.";
        NewEmail.SentAt = DateTime.UtcNow;

        await _repo.CreateNewNegativeBalanceEmailAndSaveAsync(NewEmail);

        return RedirectToPage();
    }
}
