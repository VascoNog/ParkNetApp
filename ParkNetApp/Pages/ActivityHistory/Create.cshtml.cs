using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ActivityHistory
{
    public class CreateModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public CreateModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["MovementId"] = new SelectList(_context.Movements, "Id", "Id");
        ViewData["SlotId"] = new SelectList(_context.Slots, "Id", "Code");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
        ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Make");
            return Page();
        }

        [BindProperty]
        public EntryAndExitHistory EntryAndExitHistory { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.EntriesAndExitsHistory.Add(EntryAndExitHistory);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
