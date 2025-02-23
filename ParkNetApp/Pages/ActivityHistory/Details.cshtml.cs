using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;

namespace ParkNetApp.Pages.ActivityHistory
{
    public class DetailsModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public DetailsModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        public EntryAndExitHistory EntryAndExitHistory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entryandexithistory = await _context.EntriesAndExitsHistory.FirstOrDefaultAsync(m => m.Id == id);

            if (entryandexithistory is not null)
            {
                EntryAndExitHistory = entryandexithistory;

                return Page();
            }

            return NotFound();
        }
    }
}
