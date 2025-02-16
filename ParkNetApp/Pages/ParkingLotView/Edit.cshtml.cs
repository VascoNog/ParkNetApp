using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ParkingLotView
{
    public class EditModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public EditModel(ParkNetApp.Data.ParkNetDbContext context)
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

            var slot =  await _context.Slots.FirstOrDefaultAsync(m => m.Id == id);
            if (slot == null)
            {
                return NotFound();
            }
            Slot = slot;
            ViewData["FloorId"] = new SelectList(_context.Floors, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Slot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            ViewData["FloorId"] = new SelectList(_context.Floors, "Id", "Name");
            return Page();
        }

        private bool SlotExists(int id)
        {
            return _context.Slots.Any(e => e.Id == id);
        }
    }
}
