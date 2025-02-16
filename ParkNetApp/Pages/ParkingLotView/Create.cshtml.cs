using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ParkingLotView
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
        ViewData["FloorId"] = new SelectList(_context.Floors, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Slot Slot { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Slots.Add(Slot);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
