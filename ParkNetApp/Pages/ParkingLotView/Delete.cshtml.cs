using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ParkingLotView
{
    public class DeleteModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public DeleteModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Slot Slot { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slot = await _context.Slots.FirstOrDefaultAsync(m => m.Id == id);

            if (slot is not null)
            {
                Slot = slot;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slot = await _context.Slots.FindAsync(id);
            if (slot != null)
            {
                Slot = slot;
                _context.Slots.Remove(Slot);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
