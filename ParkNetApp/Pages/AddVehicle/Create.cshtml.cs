using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ParkNetApp.Pages.AddVehicle;

[Authorize]
public class CreateModel : PageModel
{
    [BindProperty]
    public Vehicle Vehicle { get; set; }

    [BindProperty]
    public string UserId { get; set; }

    public SelectList VehicleType { get; set; }

    private readonly ParkNetDbContext _context;
    public CreateModel(ParkNetDbContext context) => _context = context;

    public IActionResult OnGet()
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        VehicleType = new SelectList(_context.VehicleTypes, "Type", "Type");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            VehicleType = new SelectList(_context.VehicleTypes, "Type", "Type"); //Recarrega a SelectList se ocorrer um erro
            return Page();
        }

        _context.Vehicles.Add(Vehicle);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
