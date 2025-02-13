using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ActivateAccount;

[Authorize]

public class CreateModel : PageModel
{
    [BindProperty]
    public UserInfo UserInfo { get; set; }

    [BindProperty]
    public Transaction FirstCardPayment { get; set; }

    private readonly ParkNetApp.Data.ParkNetDbContext _context;
    public CreateModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }


    // For more information, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Fazer o primeiro pagamento em cartão e atualizar a entidade Transaction e UserInfo
        try
        {
            var transactionStatus = _context.Transactions.Add(FirstCardPayment);
            if (transactionStatus.State == EntityState.Added)
            {
                UserInfo.IsActivated = true;
                _context.UserInfos.Add(UserInfo);
                await _context.SaveChangesAsync();
            }
        }
        catch 
        {
            throw new Exception("Não foi possível ativar a conta");
        }

        return RedirectToPage("./Index");
    }
}
