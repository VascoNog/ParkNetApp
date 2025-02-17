namespace ParkNetApp.Pages.ParkingLotView;

public class DeleteModel : PageModel
{
    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public DeleteModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Slot Slot { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        //var slot = await _context.Slots.FirstOrDefaultAsync(m => m.Id == id);
        var slot = await _context.Slots.Include(s => s.Floor)
             .ThenInclude(f => f.ParkingLot)
             .FirstOrDefaultAsync(s => s.Id == id);
        if (slot is not null)
        {
            Slot = slot;

            return Page();
        }

        return NotFound();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var currentParkingLotId = await _context.Slots.Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .Where(s => s.Id == id)
            .Select(s => s.Floor.ParkingLotId)
            .FirstOrDefaultAsync();
        if (currentParkingLotId == 0)
        {
            return NotFound();
        }

        var slot = await _context.Slots.FindAsync(id);
        if (slot != null)
        {
            Slot = slot;
            _context.Slots.Remove(Slot);
            await _context.SaveChangesAsync();
        }

        return Redirect($"/ParkingLotView/View?id={currentParkingLotId}");

    }
}
