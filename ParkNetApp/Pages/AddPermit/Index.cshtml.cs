namespace ParkNetApp.Pages.AddPermit;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ParkNetDbContext _context;

    public IndexModel(ParkNetDbContext context)
    {
        _context = context;
    }

    public IList<ParkingPermit> ParkingPermits { get; set; }

    public IList<ParkingLot> ParkingLots { get; set; }

    public string UserId { get; set; }

    public async Task OnGetAsync()
    {
        ParkingLots = await _context.ParkingLots.ToListAsync(); 

        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ParkingPermits = await _context.ParkingPermits
        .Include(p => p.PermitInfo)
        .Include(p => p.Slot)
            .ThenInclude(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
        .Include(p => p.User)
        .Where(p => p.UserId == UserId)
        .ToListAsync();

    }
}