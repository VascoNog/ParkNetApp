namespace ParkNetApp.Pages.ParkingLotView;

public class ToParkModel : PageModel
{
    public int SlotId { get; set; } 
    public void OnGet(int? id)
    {
        if (id == null)
            NotFound();

        SlotId = id.Value;
    }
}
