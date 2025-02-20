

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
                          RowIndex = GetRowIndex(s.Code),
                          ColumnIndex = GetColumnIndex(s.Code),
                          SlotType = s.SlotType,
                          IsOccupied = s.IsOccupied
                      }).ToListAsync();
    }

    public static int GetRowIndex(string slotCode)
    {
        const int asciiOffset = 65;
        const int maxAlphabetLetters = 26;
        var numbOfLetters = slotCode.Count(char.IsLetter);
        if (numbOfLetters == 1)
        {
            return (int)slotCode[0] - asciiOffset;
        }

        var rowIndex = 0;
        for (int i = 0; i < numbOfLetters; i++)
        {
            rowIndex = (int)(rowIndex * maxAlphabetLetters) + (int)(slotCode[i] - 'A' + 1);
        }
        return rowIndex;
    }

    public static int GetColumnIndex(string slotCode)
        => int.Parse(slotCode.Substring(slotCode.Count(char.IsLetter)));

    public static string GetCodeLetter(int row)
    {
        const int maxAlphabetLetters = 26;
        if (row < maxAlphabetLetters)
            return ((char)('A' + row)).ToString();
        else
            return ((char)('A' + ((row / maxAlphabetLetters) - 1))).ToString() + ((char)('A' + row % 26)).ToString();
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

    public IList<PermitInfo> GetPermitAvailableDays()
        => _ctx.PermitInfos.Where(p => p.ActiveSince <= DateOnly.FromDateTime(DateTime.Now) && p.ActiveUntil == null).ToList();

    public IList<ParkingLot> GetAvailableParkingLots() => _ctx.ParkingLots.ToList();

    public IList<Slot> GetAvailableSlots(int chosenParkingLotId)
    {
        // Obtém os slots do estacionamento que não estão ocupados
        var slotsFromParkingLot = _ctx.Slots
            .Include(s => s.Floor)
            .Where(s => s.Floor.ParkingLotId == chosenParkingLotId && !s.IsOccupied)
            .ToList();
        // Obtém os slots que possuem uma avença ativa
        var slotsWithCurrentPermit = GetSlotsWithCurrentPermit();
        // Filtra os slots removendo os que possuem o current permit
        var availableSlots = slotsFromParkingLot
            .Where(s => !slotsWithCurrentPermit.Any(permitSlot => permitSlot.Id == s.Id))
            .ToList();

        return availableSlots;
    }

    public List<Slot> GetSlotsWithCurrentPermit()
    {
        return _ctx.ParkingPermits
            .Include(p => p.Slot)
            .Include(p => p.PermitInfo)
            .Where(p => p.StartedAt <= DateTime.Now && p.PermitInfo.ActiveUntil == null)
            .Select(p => p.Slot)
            .ToList();
    }




    public async Task AddNewPermitAndSaveChangesAsync(ParkingPermit parkingPermit)
    {
        _ctx.ParkingPermits.Add(parkingPermit);
        await _ctx.SaveChangesAsync();

    }

    public async Task<IList<UserInfo>> GetCurrentUsersInfos()
        => await _ctx.UserInfos.ToListAsync();

    public async Task<UserInfo> GetCurrentUserInfo(string currentUserId)
        => await _ctx.UserInfos.FirstOrDefaultAsync(ui => ui.Id == currentUserId);

    public async Task<double> GetCurrentUserBalance(string userId)
    =>  await _ctx.Transactions
            .Where(t => t.UserId == userId)
            .Where(t => t.TransactionDate <= DateTime.Now) // Porque o saldo é calculado até à data corrente e existirão avenças com data de transação futura
            .SumAsync(t => t.Amount);


    public async Task AddTransactionAndSaveChangesAsync(Transaction transaction)
    {
        await _ctx.Transactions.AddAsync(transaction);
        await _ctx.SaveChangesAsync(); 
    }


}