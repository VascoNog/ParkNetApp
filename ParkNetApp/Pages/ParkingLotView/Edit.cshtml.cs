using Microsoft.AspNetCore.Mvc.Rendering;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ParkingLotView;

public class EditModel : PageModel
{
    [BindProperty]
    public Slot Slot { get; set; }

    [BindProperty]
    public IList<Floor> Floor { get; set; }


    private readonly ParkNetApp.Data.ParkNetDbContext _context;

    public EditModel(ParkNetApp.Data.ParkNetDbContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slot = await _context.Slots.Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (slot == null)
        {
            return NotFound();
        }
        Slot = slot;


        var floor = _context.Slots.Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .Where(s => s.Floor.ParkingLotId == slot.Floor.ParkingLotId)
            .Select(s => s.Floor)
            .Distinct()
            .ToList();
        if(floor == null)
        {
            return NotFound();
        }
        Floor = floor;

        ViewData["FloorId"] = new SelectList(Floor, "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Slot).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SlotExists(Slot.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

         var slot = await _context.Slots
        .Include(s => s.Floor)
        .ThenInclude(f => f.ParkingLot)
        .FirstOrDefaultAsync(s => s.Id == Slot.Id);
        if (slot == null)
        {
            return NotFound();
        }
        Slot = slot;


        var floor = _context.Slots.Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .Where(s => s.Floor.ParkingLotId == slot.Floor.ParkingLotId)
            .Select(s => s.Floor)
            .Distinct()
            .ToList();
        if (floor == null)
        {
            return NotFound();
        }
        Floor = floor;

        ViewData["FloorId"] = new SelectList(Floor, "Id", "Name");

        return Page();
    }

    private bool SlotExists(int id)
    {
        return _context.Slots.Any(e => e.Id == id);
    }
}
