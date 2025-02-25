namespace ParkNetApp.Pages.ActivityHistory;
[Authorize (Roles ="Member")]

public class IndexModel : PageModel
{
    private ParkNetRepository _repo;
    public IndexModel(ParkNetRepository parkNetRepository)
        => _repo = parkNetRepository;

    public IList<EntryAndExitHistory> EntryAndExitHistory { get;set; }

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        EntryAndExitHistory = await _repo.GetUserEntryAndExitHistory(userId);
    }
}
