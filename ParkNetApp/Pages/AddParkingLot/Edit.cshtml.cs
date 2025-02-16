namespace ParkNetApp.Pages.AddParkingLot;

public class EditModel : PageModel
{

    [BindProperty]
    public ParkingLot ParkingLot { get; set; } 

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

        var parkinglot =  await _context.ParkingLots.FirstOrDefaultAsync(m => m.Id == id);
        if (parkinglot == null)
        {
            return NotFound();
        }
        ParkingLot = parkinglot;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(ParkingLot).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ParkingLotExists(ParkingLot.Id))
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

    private bool ParkingLotExists(int id)
    {
        return _context.ParkingLots.Any(e => e.Id == id);
    }
}
