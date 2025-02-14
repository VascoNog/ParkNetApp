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
    public class IndexModel : PageModel
    {
        private readonly ParkNetApp.Data.ParkNetDbContext _context;

        public IndexModel(ParkNetApp.Data.ParkNetDbContext context)
        {
            _context = context;
        }

        public IList<ParkingLot> ParkingLot { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ParkingLot = await _context.ParkingLots.ToListAsync();
        }
    }
}
