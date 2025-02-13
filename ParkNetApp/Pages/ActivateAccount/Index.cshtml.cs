using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ParkNetApp.Pages.ActivateAccount;

[Authorize]

public class IndexModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public IndexModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    public IList<UserInfo> UserInfo { get;set; } = default!;

    public async Task OnGetAsync()
    {
        UserInfo = await _context.UserInfos.ToListAsync();
    }
}
