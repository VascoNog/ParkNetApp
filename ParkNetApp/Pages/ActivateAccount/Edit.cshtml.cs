using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ActivateAccount;

[Authorize]


public class EditModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public EditModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public UserInfo UserInfo { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userinfo =  await _context.UserInfos.FirstOrDefaultAsync(m => m.Id == id);
        if (userinfo == null)
        {
            return NotFound();
        }
        UserInfo = userinfo;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more information, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }


        _context.Attach(UserInfo).State = EntityState.Modified;

        UserInfo.IsActivated = true; // Pensar se verifico o saldo de forma a continuar ativo ou não

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserInfoExists(UserInfo.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool UserInfoExists(string id)
    {
        return _context.UserInfos.Any(e => e.Id == id);
    }
}
