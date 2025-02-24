using System.Threading.Tasks;

namespace ParkNetApp.Pages;
[Authorize]

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ParkNetRepository _repo;
    public bool IsActivated { get; private set; }

    public IndexModel(ILogger<IndexModel> logger, ParkNetRepository parkNetRepository)
    {
        _logger = logger;
        _repo = parkNetRepository;
    }

    public async Task OnGet()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        IsActivated = _repo.IsAnActiveAccount(userId);
        
    }
}
