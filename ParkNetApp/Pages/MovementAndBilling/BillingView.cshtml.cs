using Microsoft.Build.Framework;

namespace ParkNetApp.Pages.MovementAndBilling;

public class BillingViewModel : PageModel
{
    private ParkNetRepository _repo;

    public BillingViewModel(ParkNetRepository parkNetRepository) => _repo = parkNetRepository;

    [BindProperty]
    public double BillingWithinDateRange { get; set; }

    [BindProperty]
    public DateTime SelectedStartDate { get; set; }

    [BindProperty]
    public DateTime SelectedEndDate { get; set; }

    [BindProperty]
    public string SelectedTypeFilter { get; set; }


    public SelectList TransTypesOptions { get; set; }

    public async Task OnGetAsync(DateTime? startDate, DateTime? endDate, string? typeFilter)
    {
        _ = startDate;
        _ = endDate;
        _ = typeFilter;

        if (startDate != null && endDate != null && !string.IsNullOrEmpty(typeFilter))
            BillingWithinDateRange = await _repo.GetBillingWithinDateRange(startDate, endDate, typeFilter);
        else
            BillingWithinDateRange = await _repo.GetBillingWithinDateRange();

        List<string> transTypes = ["All","Parking", "Permit"];
        TransTypesOptions = new SelectList(transTypes);

        SelectedTypeFilter = typeFilter;
        SelectedStartDate = startDate ?? DateTime.UtcNow.AddYears(-1);
        SelectedEndDate = endDate ?? DateTime.UtcNow;
    }

}