﻿namespace ParkNetApp.Pages.ActivateAccount;

[Authorize(Roles = "Member")]

public class CreateModel : PageModel
{
    [BindProperty]
    public UserInfo UserInfo { get; set; }

    [BindProperty]
    public Movement FirstCardPayment { get; set; }

    private readonly ParkNetDbContext _context;
    public CreateModel(ParkNetDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Fazer o primeiro pagamento em cartão e atualizar a entidade Transaction e UserInfo
        try
        {
            var transactionStatus = _context.Movements.Add(FirstCardPayment);
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
