
namespace ParkNetApp.Pages.AddVehicle;
[Authorize]
public class IndexModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;
    public IList<Vehicle> Vehicle { get;set; }
    
    [BindProperty]
    public string UserId { get; set; }


    public IndexModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Vehicle = await _context.Vehicles
            .Include(v => v.VehicleType )
            .Include(v => v.User)
            .Where(v => v.UserId == UserId)
            .ToListAsync();
    }
}
