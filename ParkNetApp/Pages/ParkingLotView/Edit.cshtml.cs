
namespace ParkNetApp.Pages.ParkingLotView;
public class EditModel : PageModel
{
    [BindProperty]
    public Slot Slot { get; set; }

    [BindProperty]
    public IList<Floor> Floors { get; set; }


    private ParkNetRepository _repo;
    public EditModel(ParkNetRepository parkNetRepository)
        => _repo = parkNetRepository;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slot = await _repo.GetSlotAsyncById(id.Value);
        if (slot == null)
        {
            return NotFound();
        }
        Slot = slot;

        var floors = await _repo.GetFloorsBySlot(Slot);
        if(floors == null)
        {
            return NotFound();
        }
        Floors = floors;

        ViewData["FloorId"] = new SelectList(Floors, "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _repo.AttachSateModified(Slot);

        try
        {
            await _repo.SaveAllChangesAsync();
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

        var slot = await _repo.GetSlotAsyncById(Slot.Id);
        if (slot == null)
        {
            return NotFound();
        }
        Slot = slot;

        var floors = await _repo.GetFloorsBySlot(Slot);
        if (floors == null)
        {
            return NotFound();
        }
        Floors = floors;

        ViewData["FloorId"] = new SelectList(Floors, "Id", "Name");

        return Page();
    }

    private bool SlotExists(int id)
    {
        return _repo.IsValidSlot(id);
    }
}
