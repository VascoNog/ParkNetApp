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

namespace ParkNetApp.Pages.AddParkingLot
{
    public class EditModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public EditModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ParkingLot ParkingLot { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkinglot =  await _context.ParkingLots.FirstOrDefaultAsync(m => m.Id == id);
            if (parkinglot == null)
            {
                return NotFound();
            }
            ParkingLot = parkinglot;
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

            _context.Attach(ParkingLot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingLotExists(ParkingLot.Id))
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

        private bool ParkingLotExists(int id)
        {
            return _context.ParkingLots.Any(e => e.Id == id);
        }
    }
}
