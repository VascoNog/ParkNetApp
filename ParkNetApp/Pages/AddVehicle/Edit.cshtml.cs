namespace ParkNetApp.Pages.AddVehicle;

public class EditModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public EditModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Vehicle Vehicle { get; set; }

    [BindProperty]
    public string UserId { get; set; }

    [BindProperty]
    public int VehicleId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle =  await _context.Vehicles.FirstOrDefaultAsync(m => m.Id == id);
        if (vehicle == null)
        {
            return NotFound();
        }
        Vehicle = vehicle;

        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        VehicleId = id.Value;

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

        _context.Attach(Vehicle).State = EntityState.Modified;

        try
        {

            Vehicle.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(Vehicle.Id))
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

    private bool VehicleExists(int id)
    {
        return _context.Vehicles.Any(e => e.Id == id);
    }
}
