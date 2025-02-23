using FluentResults;
using Microsoft.EntityFrameworkCore;

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

    public async Task AddMovementForPermitAndSaveAsync(ParkingPermit parkingPermit)
    {
        var daysOfPermit = _ctx.ParkingPermits
            .Include(pp => pp.PermitInfo)
            .Where(pp => pp.Id == parkingPermit.Id)
            .Select(pp => pp.PermitInfo.DaysOfPermit)
            .FirstOrDefault();

        var amountOfPermit = _ctx.ParkingPermits
            .Include(pp => pp.PermitInfo)
            .Where(pp => pp.Id == parkingPermit.Id)
            .Select(pp => pp.PermitInfo.Value)
            .FirstOrDefault();

        var permitEndAt = parkingPermit.StartedAt.AddDays(daysOfPermit);

        Movement newMovement = new()
        {
            TransactionDate = permitEndAt, //Data Futura
            Amount = amountOfPermit,
            TransactionType = "Permit",
            UserId = parkingPermit.UserId
        };

        await _ctx.Movements.AddAsync(newMovement);
        await _ctx.SaveChangesAsync();
    }

    public async Task<IList<UserInfo>> GetCurrentUsersInfos()
        => await _ctx.UserInfos.ToListAsync();

    public async Task<UserInfo> GetCurrentUserInfo(string currentUserId)
        => await _ctx.UserInfos.FirstOrDefaultAsync(ui => ui.Id == currentUserId);

    public async Task<double> GetCurrentUserBalance(string userId)
    => await _ctx.Movements
            .Where(t => t.UserId == userId)
            .Where(t => t.TransactionDate <= DateTime.UtcNow) // Porque o saldo é calculado até à data corrente e existirão avenças com data de transação futura
            .SumAsync(t => t.Amount);

    public async Task AddTransactionAndSaveChangesAsync(Movement movement)
    {
        await _ctx.Movements.AddAsync(movement);
        await _ctx.SaveChangesAsync();
    }

    public Slot GetSlotById(int slotId)
        => _ctx.Slots.Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .FirstOrDefault(s => s.Id == slotId);

    public Vehicle GetVehicleById(int vehicleId)
        => _ctx.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

    //public bool UserWantsParkOrUnparkRigthVehicle (Slot slot, string userId)
    //{
    //    if (UserCanParkOrUnpark(slot, userId))
    //        return (UserHasCorrectVehicleType (slot, userId));
    //    return false;
    //}

    // Se for para estacionar unicamente
    public bool CanPerformAction(Slot slot, string userId)
     => UserCanParkOrUnpark(slot, userId)
        && UserHasCorrectVehicleType(slot, userId);

    public bool UserCanParkOrUnpark(Slot slot, string userId)
    {
        if (slot.IsOccupied)
            return UserIsParkedAtCurrentSlot(slot, userId);

        if (UserHasActivePermitAtCurrentSlot(slot, userId))
            return true;

        return CurrentSlotHasNoActivePermit(slot);
    }

    //public bool UserHasActivePermitAtCurrentSlot(Slot slot, string userId)
    //   => _ctx.ParkingPermits.Any(p => p.UserId == userId
    //    && p.PermitInfo.ActiveUntil == null
    //    && p.SlotId == slot.Id);

    public bool UserHasActivePermitAtCurrentSlot(Slot slot, string userId)
    {
        var userPermits = _ctx.ParkingPermits
            .Include(pp => pp.PermitInfo)
            .Where(pp => pp.SlotId == slot.Id && pp.UserId == userId)
            .ToList();
        if (userPermits.Count == 0)
            return false;

        return userPermits.Any(permit => permit.StartedAt.AddDays(permit.PermitInfo.DaysOfPermit) >= DateTime.UtcNow);
    }

    public bool UserIsParkedAtCurrentSlot(Slot slot, string userId)
        => _ctx.EntriesAndExitsHistory.Any(p => p.UserId == userId
            && p.SlotId == slot.Id && p.ExitAt == null);

    public bool CurrentSlotHasNoActivePermit(Slot slot)
        => !_ctx.ParkingPermits.Any(p => p.SlotId == slot.Id
            && p.PermitInfo.ActiveUntil == null);

    public bool UserHasCorrectVehicleType(Slot slot, string userId)
        => _ctx.Vehicles
       .Include(v => v.VehicleType)
       .Any(v => v.UserId == userId && v.VehicleType.Symbol == slot.SlotType);

    public IList<Vehicle> GetAvailableVehiclesByUserId(string userId)
        => _ctx.Vehicles.Include(v => v.VehicleType)
        .Where(v => v.UserId == userId).ToList();

    public IList<Vehicle> GetAvailableVehiclesToPark(string userId, Slot slot)
    {
        //var vehicles = _ctx.EntriesAndExitsHistory
        //     .Include(eeh => eeh.Vehicle)
        //     .Include(eeh => eeh.Slot)
        //     .Where(eeh => eeh.ExitAt != null
        //        && eeh.UserId == userId
        //        && eeh.SlotId == slot.Id)
        //     .Distinct()
        //     .Select(eeh => eeh.Vehicle)
        //     .ToList();

        IList<Vehicle> availableVehiclesToPark = [];

        var unparkedUserVehicles = _ctx.Vehicles.Include(v => v.VehicleType)
            .Where(v => v.isParked == false
            && v.UserId == userId)
            .ToList();

        foreach (var vhcl in unparkedUserVehicles)
        {
            if (vhcl.VehicleType.Symbol == slot.SlotType)
                availableVehiclesToPark.Add(vhcl);
        }
        return availableVehiclesToPark;
    }

    public Vehicle GetCurrentlyParkedVehicle(Slot slot)
    {
        return _ctx.EntriesAndExitsHistory
             .Include(eeh => eeh.Vehicle)
             .Include(eeh => eeh.Slot)
             .Where(eeh => eeh.ExitAt == null
                && eeh.SlotId == slot.Id
                && eeh.Vehicle.isParked == true)
             .Select(eeh => eeh.Vehicle)
             .Distinct()
             .FirstOrDefault();
    }

    public async Task UpdateEntriesAndExitsAndSaveAsync(string userId, Vehicle vehicle, Slot slot)
    {
        var dateNow = DateTime.UtcNow;

        slot.IsOccupied = false;
        vehicle.isParked = false;

        var currentEntryAndExitHistory = GetCurrentEntryAndExitHistory(userId, vehicle.Id, slot);
        _ctx.EntriesAndExitsHistory.FirstOrDefault(eeh => eeh.Id == currentEntryAndExitHistory.Id).ExitAt = dateNow;

        // Calcular o Amount da Transaction e Guardar na base de dados
        await UpdateTransactionAmountAndSave(currentEntryAndExitHistory.MovementId, currentEntryAndExitHistory);
    }

    public EntryAndExitHistory GetCurrentEntryAndExitHistory(string userId, int vehicleId, Slot slot)
    => _ctx.EntriesAndExitsHistory.FirstOrDefault(eeh => eeh.UserId == userId
        && eeh.VehicleId == vehicleId
        && eeh.SlotId == slot.Id
        && eeh.ExitAt == null);

    public async Task CreateNewEntryHistoryAndSaveAsync(string userId, Vehicle vehicle, Slot slot)
    {
        var movement = await CreateNewTransactionAndSaveAsync(userId);
        if (movement is not null)
        {
            slot.IsOccupied = true;
            vehicle.isParked = true;
            EntryAndExitHistory newEntry = new()
            {
                EntryAt = DateTime.UtcNow,
                UserId = userId,
                VehicleId = vehicle.Id,
                SlotId = slot.Id,
                MovementId = movement.Id
            };
            await _ctx.EntriesAndExitsHistory.AddAsync(newEntry);
            await _ctx.SaveChangesAsync();
        }
    }

    public async Task UpdateTransactionAmountAndSave(int movementId, EntryAndExitHistory entryAndExitHistory)
    {
        const double minTariff = 0.01;
        var dateNow = DateTime.UtcNow;
        var minutes = (dateNow - entryAndExitHistory.EntryAt).TotalMinutes;
        var movement = await _ctx.Movements.FirstOrDefaultAsync(t => t.Id == movementId);
        double tariff = 0.0d;

        if (!UserHasActivePermitAtCurrentSlot(entryAndExitHistory.Slot, entryAndExitHistory.UserId))
        {
            tariff = (double)_ctx.NonSubscriptionParkingTariffs
                .Where(nst => minutes <= nst.Limit)
                .AsEnumerable()
                .Select(nst => (double)nst.Tariff)
                .FirstOrDefault(0);
            if (tariff == 0)
                tariff = (double)_ctx.NonSubscriptionParkingTariffs.Min(nst => nst.Tariff) - minTariff;
            if (tariff < 0)
                tariff = minTariff;

            if (movement is not null)
            {
                movement.Amount = -(tariff * minutes);
                movement.TransactionDate = dateNow;
                await _ctx.SaveChangesAsync();
            }
        }
        else
        {
            if (movement is not null)
            {
                movement.Amount = 0; //Porque tem avença
                movement.TransactionDate = dateNow;
                await _ctx.SaveChangesAsync();
            }
        }
    }

    // Mudar o nome de Transaction por causa da ambiguidade com System ?
    public async Task<Movement> CreateNewTransactionAndSaveAsync(string userId)
    {
        Movement newMovement = new()
        {
            Amount = 0, // COLOCAR A NULLABLE MAIS TARDE
            TransactionType = "Parking",
            UserId = userId
        };

        await _ctx.Movements.AddAsync(newMovement);
        await _ctx.SaveChangesAsync();

        return newMovement;
    }

    public async Task<Result<double>> TryGetCorrectWithdrawAmount(string userId, double withdrawAmount)
    {
        // Verifica se o utilizador está estacionado sem ter qualquer avença válida
        if (IsUserParkingWithoutPermit(userId))
            return Result.Fail("Error: The user cannot withdraw because they are parked without a permit.");

        var currentBalance = await GetCurrentUserBalance(userId);

        double adjustedAmount = currentBalance - 0.01;
        if (withdrawAmount >= currentBalance)
        {
            if (adjustedAmount <= 0)
                return Result.Fail("Error: The user cannot withdraw because they do not have enough balance.");

            withdrawAmount = adjustedAmount;
        }

        // Regista a transação como valor negativo (levantamento)
        return Result.Ok(-withdrawAmount);
    }

    public bool IsUserParkingWithoutPermit(string userId)
        => _ctx.Vehicles.Any(v => v.UserId == userId && v.isParked == true)
            && !UserHasSomeActivePermit(userId);

    public bool UserHasSomeActivePermit(string userId)
        => _ctx.ParkingPermits.Include(pp => pp.PermitInfo)
            .Where(pp => pp.UserId == userId)
            .Any(pp => pp.StartedAt.AddDays(pp.PermitInfo.DaysOfPermit) >= DateTime.UtcNow);

    public async Task<double> GetBillingWithinDateRange(DateTime? startDate, DateTime? endDate, string? filter)
    {
        if (filter == "All")
        {
            return -(await _ctx.Movements
            .Where(m => m.TransactionDate >= startDate && m.TransactionDate <= endDate
            && m.TransactionType == "Permit" || m.TransactionType == "Parking")
            .SumAsync(m => m.Amount));
        }
        return -(await _ctx.Movements
       .Where(m => m.TransactionDate >= startDate && m.TransactionDate <= endDate
           && m.TransactionType == filter)
       .SumAsync(m => m.Amount));
    }

    public async Task<double> GetBillingWithinDateRange()
    {
        
        var startDate = DateTime.UtcNow.AddYears(-1);
        var endDate = DateTime.UtcNow;
        return -(await _ctx.Movements
        .Where(m => m.TransactionDate >= startDate && m.TransactionDate <= endDate
            && m.TransactionType == "Permit" || m.TransactionType == "Parking")
        .SumAsync(m => m.Amount));
    }
    public async Task<List<string>> GetAllUserDistinctEmails()
        => await _ctx.Users.Select(u => u.Email).Distinct().OrderBy(e => e).ToListAsync();
    public async Task<List<Movement>> GetAllMovementsIncludingUser()
        => await _ctx.Movements.Include(m => m.User).ToListAsync();

    public async Task<double> GetBalanceByUserEmail(string email)
    {
        var userMovement = await GetAllMovementsIncludingUser();
        return userMovement.Where(um => um.User.Email == email
            && um.TransactionDate <= DateTime.UtcNow).Sum(um => um.Amount);
    }

    public async Task CreateNewNegativeBalanceEmailAndSaveAsync(EmailBox newEmail)
    {
        await _ctx.EmailBoxes.AddAsync(newEmail);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<EmailBox>> GetUserMessagesByUserId(string userId)
        => await _ctx.EmailBoxes.Where(eb => eb.RecipientId == userId)
        .OrderByDescending(eb => eb.SentAt)
        .ToListAsync();
}
