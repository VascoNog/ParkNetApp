namespace ParkNetApp.Pages.AddVehicle;

public class DeleteModel : PageModel
{
    private readonly ParkNetRepository _repo;

    public DeleteModel(ParkNetRepository parkNetRepository)
        => _repo = parkNetRepository;

    [BindProperty]
    public Vehicle? Vehicle { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Vehicle = await _repo.GetVehicleByIdAsync(id.Value);
        if (Vehicle == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Vehicle = await _repo.GetVehicleByIdAsync(id.Value);
        if (Vehicle == null)
        {
            ModelState.AddModelError(string.Empty, "The vehicle does not exist or was already deleted.");
            return Page();
        }

        if (!await _repo.CanDeleteVehicle(id.Value))
        {
            ModelState.AddModelError(string.Empty, "This vehicle cannot be deleted because it is currently parked.");
            return Page();
        }

        await _repo.RemoveVehicleAndSaveAsync(Vehicle);

        return RedirectToPage("./Index");
    }
}
