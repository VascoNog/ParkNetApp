namespace ParkNetApp.Pages.ActivateAccount;
[Authorize]

public class IndexModel : PageModel
{
    private readonly ParkNetRepository _repo;
    public IndexModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;

    [BindProperty]
    public Movement NewTransaction { get; set; }
    public string UserId { get; set; }  
    public UserInfo UserInfo { get;set; } // Posso lidar deste lado com o corrente user logado
    public double CurrentBalance { get; set; }

    public async Task OnGetAsync()
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserInfo = await _repo.GetCurrentUserInfo(UserId);
        CurrentBalance = await _repo.GetCurrentUserBalance(UserId);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        CurrentBalance = await _repo.GetCurrentUserBalance(UserId);

        if (NewTransaction.TransactionType == "Withdraw")
        {
            if (NewTransaction.Amount >= CurrentBalance)
                NewTransaction.Amount = (double) (CurrentBalance - 0.01); 
            NewTransaction.Amount = - NewTransaction.Amount; // Valor é negativo no caso de levantamento
        }

        NewTransaction.UserId = UserId;
        NewTransaction.TransactionDate = DateTime.UtcNow;
        Console.WriteLine(NewTransaction.Amount);
        Console.WriteLine(NewTransaction.UserId);
        Console.WriteLine(NewTransaction.TransactionDate);
        Console.WriteLine(NewTransaction.TransactionType);
        
        await _repo.AddTransactionAndSaveChangesAsync(NewTransaction);

        return RedirectToPage("./Index");
    }

}
