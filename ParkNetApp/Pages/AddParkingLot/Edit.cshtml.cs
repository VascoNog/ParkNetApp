namespace ParkNetApp.Pages.AddParkingLot;
[Authorize (Roles="Admin")]
public class EditModel : PageModel
{
    private ParkNetRepository _repo;
    public EditModel(ParkNetRepository parkNetRepository)
        => _repo = parkNetRepository;

    [BindProperty]
    public ParkingLot ParkingLot { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkinglot = await _repo.GetParkingLot(id.Value);
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

        _repo.AttachSateModified(ParkingLot);

        try
        {
            await _repo.SaveAllChangesAsync();
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
        return _repo.IsValidParkingLot(id);
    }
}
