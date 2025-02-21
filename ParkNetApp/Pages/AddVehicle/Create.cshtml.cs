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

        VehicleType = new SelectList(_context.VehicleTypes, "Id", "Type");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            VehicleType = new SelectList(_context.VehicleTypes, "Id", "Type"); //Recarrega a SelectList se ocorrer um erro
            return Page();
        }

        Vehicle.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _context.Vehicles.Add(Vehicle);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
