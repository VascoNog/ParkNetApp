using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.MovementAndBilling
{
    public class DeleteModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public DeleteModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Movement Movement { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movement = await _context.Movements.FirstOrDefaultAsync(m => m.Id == id);

            if (movement is not null)
            {
                Movement = movement;

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

            var movement = await _context.Movements.FindAsync(id);
            if (movement != null)
            {
                Movement = movement;
                _context.Movements.Remove(Movement);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
