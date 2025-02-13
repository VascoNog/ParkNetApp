using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ActivateAccount;

[Authorize]

public class DetailsModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public DetailsModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    public UserInfo UserInfo { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userinfo = await _context.UserInfos.FirstOrDefaultAsync(m => m.Id == id);

        if (userinfo is not null)
        {
            UserInfo = userinfo;

            return Page();
        }

        return NotFound();
    }
}
