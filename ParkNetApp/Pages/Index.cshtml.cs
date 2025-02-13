using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkNetApp.Data.Repositories;
using System.Security.Claims;

namespace ParkNetApp.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ParkNetRepository _parkNetRepository;
    public bool IsActivated { get; private set; }

    public IndexModel(ILogger<IndexModel> logger, ParkNetRepository parkNetRepository)
    {
        _logger = logger;
        _parkNetRepository = parkNetRepository;
    }

    public void OnGet()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        IsActivated = _parkNetRepository.IsAnActiveAccount(userId);
    }
}
