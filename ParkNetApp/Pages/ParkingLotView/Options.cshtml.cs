using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ParkNetApp.Pages.ParkingLotView;

public class OptionsModel : PageModel
{
    [BindProperty]
    public int SlotId { get; set; }

    public IActionResult OnGet(int? id) 
    {
        if (id == null)
            return NotFound();

        SlotId = id.Value;

        return Page(); 
    }
}

