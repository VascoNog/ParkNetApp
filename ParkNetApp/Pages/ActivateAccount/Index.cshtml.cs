using FluentResults;

namespace ParkNetApp.Pages.ActivateAccount;
[Authorize]

public class IndexModel : PageModel
{
    private readonly ParkNetRepository _repo;
    public IndexModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;

    [BindProperty]
    public Movement NewTransaction { get; set; }

    [BindProperty]
    public UserInfo UserInfo { get;set; } // Posso lidar deste lado com o corrente user logado

    [BindProperty]
    public double CurrentBalance { get; set; }

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserInfo = await _repo.GetCurrentUserInfo(userId);
        CurrentBalance = await _repo.GetCurrentUserBalance(userId);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            UserInfo = await _repo.GetCurrentUserInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserInfo = await _repo.GetCurrentUserInfo(userId);
        CurrentBalance = await _repo.GetCurrentUserBalance(userId);

        if (NewTransaction.TransactionType == "Withdraw")
        {
            var result = await _repo.TryGetCorrectWithdrawAmount(userId, NewTransaction.Amount);
            if (result.IsSuccess)
            {
                NewTransaction.Amount = result.Value;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                return Page();
            }
        }

        NewTransaction.UserId = userId;
        NewTransaction.TransactionDate = DateTime.UtcNow;

        await _repo.AddTransactionAndSaveChangesAsync(NewTransaction);

        return RedirectToPage("./Index");
    }

}
