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

namespace ParkNetApp.Pages.AddPermit
{
    public class EditModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public EditModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ParkingPermit ParkingPermit { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingpermit =  await _context.ParkingPermits.FirstOrDefaultAsync(m => m.Id == id);
            if (parkingpermit == null)
            {
                return NotFound();
            }
            ParkingPermit = parkingpermit;
           ViewData["PermitInfoId"] = new SelectList(_context.PermitInfos, "Id", "Id");
           ViewData["SLotId"] = new SelectList(_context.Slots, "Id", "Code");
           ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
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

            _context.Attach(ParkingPermit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingPermitExists(ParkingPermit.Id))
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

        private bool ParkingPermitExists(int id)
        {
            return _context.ParkingPermits.Any(e => e.Id == id);
        }
    }
}
