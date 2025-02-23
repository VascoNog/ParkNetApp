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

namespace ParkNetApp.Pages.ActivityHistory
{
    public class EditModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public EditModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EntryAndExitHistory EntryAndExitHistory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entryandexithistory =  await _context.EntriesAndExitsHistory.FirstOrDefaultAsync(m => m.Id == id);
            if (entryandexithistory == null)
            {
                return NotFound();
            }
            EntryAndExitHistory = entryandexithistory;
           ViewData["MovementId"] = new SelectList(_context.Movements, "Id", "Id");
           ViewData["SlotId"] = new SelectList(_context.Slots, "Id", "Code");
           ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
           ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Make");
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

            _context.Attach(EntryAndExitHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryAndExitHistoryExists(EntryAndExitHistory.Id))
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

        private bool EntryAndExitHistoryExists(int id)
        {
            return _context.EntriesAndExitsHistory.Any(e => e.Id == id);
        }
    }
}
