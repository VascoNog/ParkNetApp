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

        if (NewTransaction.TransactionType == "Withdraw")
        {
            NewTransaction.Amount = - NewTransaction.Amount; // Para garantir que o valor é negativo no caso de levantamento
        }

        NewTransaction.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        NewTransaction.TransactionDate = DateTime.Now;

        Console.WriteLine(NewTransaction.Amount);
        Console.WriteLine(NewTransaction.UserId);
        Console.WriteLine(NewTransaction.TransactionDate);
        Console.WriteLine(NewTransaction.TransactionType);
        
        await _repo.AddTransactionAndSaveChangesAsync(NewTransaction);

        return RedirectToPage("./Index");
    }

}
