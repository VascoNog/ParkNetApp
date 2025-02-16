namespace ParkNetApp.Pages.ParkingLotView
{
    public class IndexModel : PageModel
    {
        public IList<Slot> Slot { get; set; }
        public ParkingLot ParkingLot  { get; set; }

        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public IndexModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync(int? id)
        {
            ParkingLot = await _context.ParkingLots.FirstOrDefaultAsync(p => p.Id == id);
            Slot = await _context.Slots
                .Include(s => s.Floor)
                .ThenInclude(f => f.ParkingLot)
                .Where(s => s.Floor.ParkingLotId == id) // Filtra os Slots pelo ParkingLot com o id passado no URL
                .ToListAsync();
        }
    }
}