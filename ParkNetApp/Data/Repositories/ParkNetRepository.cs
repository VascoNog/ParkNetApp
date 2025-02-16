using ParkNetApp.Models;

namespace ParkNetApp.Data.Repositories;

public class ParkNetRepository
{
    private readonly ParkNetDbContext _ctx;
    public ParkNetRepository(ParkNetDbContext context) => _ctx = context;

    public bool IsAnActiveAccount(string userId) 
        => _ctx.UserInfos.Any(u => u.Id == userId && u.IsActivated == true);

    public async Task<IList<ParkingModel>> GetParkingLotSchema(int parkingLotId)
    {
       return await (from pl in _ctx.ParkingLots
         join f in _ctx.Floors on pl.Id equals f.ParkingLotId
         join s in _ctx.Slots on f.Id equals s.FloorId
         where pl.Id == parkingLotId
         select new ParkingModel
         {
             ParkingLotId = pl.Id,
             ParkingLotDesignation = pl.Designation,
             FloorId = f.Id,
             FloorName = f.Name,
             SlotId = s.Id,
             SlotCode = s.Code,
             SlotType = s.SlotType,
             IsOccupied = s.IsOccupied
         }).ToListAsync();
    }

    public async Task<ParkingLot> GetParkingLot(int parkingLotId)
    {
        return await _ctx.ParkingLots.FirstOrDefaultAsync(p => p.Id == parkingLotId);
    }

    public async Task<IList<Slot>> GetSlotsFromParkingLot(int parkingLotId)
        => await _ctx.Slots
            .Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .Where(s => s.Floor.ParkingLotId == parkingLotId)
            .ToListAsync();

}
