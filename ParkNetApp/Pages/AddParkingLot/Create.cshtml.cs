namespace ParkNetApp.Pages.AddParkingLot;
[Authorize]

public class CreateModel : PageModel
{

    [BindProperty]
    public ParkingLot ParkingLot { get; set; }

    private readonly ParkNetDbContext _context;

    public CreateModel(ParkNetDbContext context)
    {
        _context = context;
        
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!Utilities.IsLayoutValid(ParkingLot.Layout))
        {
            ModelState.AddModelError("ParkingLot.Layout", "Layout is not valid");
            return Page();
        }

        try
        {
            _context.ParkingLots.Add(ParkingLot);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new Exception("Parking lot designation already exists. Choose other one");
        }

        //After update database with new parking lot, we need to update the floors and slots
        //FLOORS
        int parkingLotId = _context.ParkingLots.FirstOrDefault(p => p.Designation == ParkingLot.Designation).Id;
        var floors = Utilities.GetFloors(ParkingLot.Layout, parkingLotId);
        try
        {
            _context.Floors.AddRange(floors);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new Exception("It was not possible to update the floors");
        }

        //SLOTS
        var slots = Utilities.GetSlots(ParkingLot.Layout,floors);
        try
        {
            _context.Slots.AddRange(slots);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new Exception("It was not possible to update the slots");
        }


        return RedirectToPage("./Index");
    }
}
