using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.AddParkingLot
{
    public class DeleteModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public DeleteModel(ParkNetApp.Data.ParkNetDbContext context)
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

            var parkinglot = await _context.ParkingLots.FirstOrDefaultAsync(m => m.Id == id);

            if (parkinglot is not null)
            {
                ParkingLot = parkinglot;

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

            var parkinglot = await _context.ParkingLots.FindAsync(id);
            if (parkinglot != null)
            {
                ParkingLot = parkinglot;
                _context.ParkingLots.Remove(ParkingLot);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
