namespace ParkNetApp.Pages.ActivityHistory
{
    public class IndexModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public IndexModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        public IList<EntryAndExitHistory> EntryAndExitHistory { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EntryAndExitHistory = await _context.EntriesAndExitsHistory
                .Include(e => e.Movement)
                .Include(e => e.Slot)
                    .ThenInclude(s => s.Floor)
                        .ThenInclude(f => f.ParkingLot)
                .Include(e => e.User)
                .Include(e => e.Vehicle)
                .Where(e => e.UserId == userId) 
                .ToListAsync();

        }
    }
}
