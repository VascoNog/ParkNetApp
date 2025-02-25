using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ParkNetApp.Data.Entities;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

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

    public async Task<Vehicle?> GetVehicleByIdAsync(int vehicleId)
    => await _ctx.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);


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

    public bool UserHasActivePermitAtCurrentSlot(Slot slot, string userId)
    {
        var userPermits = _ctx.ParkingPermits
            .Include(pp => pp.PermitInfo)
            .Where(pp => pp.SlotId == slot.Id && pp.UserId == userId)
            .ToList();
        if (userPermits.Count == 0)
            return false;

        return userPermits.Any(permit 
            => permit.StartedAt.AddDays(permit.PermitInfo.DaysOfPermit) >= DateTime.UtcNow);
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
        _ctx.EntriesAndExitsHistory
            .FirstOrDefault(eeh => eeh.Id == currentEntryAndExitHistory.Id).ExitAt = dateNow;

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

        var dateNow = DateTime.UtcNow;
        var minutes = (dateNow - entryAndExitHistory.EntryAt).TotalMinutes;
        var movement = await _ctx.Movements.FirstOrDefaultAsync(t => t.Id == movementId);
 
        if (!UserHasActivePermitAtCurrentSlot(entryAndExitHistory.Slot, entryAndExitHistory.UserId))
        {
            var amount = GetCorrectParkingAmount(minutes);

            if (movement is not null)
            {
                movement.Amount = amount;
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

    public double GetCorrectParkingAmount(double minutes)
    {
        const double minTariff = 0.01;

        var tariff = _ctx.NonSubscriptionParkingTariffs
        .Where(nst => minutes <= nst.Limit
         && nst.ActiveUntil == null)
        .OrderBy(nst => nst.Limit)
        .Select(nst => nst.Tariff)
        .FirstOrDefault();

        if (tariff == 0)
            tariff = (double)_ctx.NonSubscriptionParkingTariffs.Min(nst => nst.Tariff) - minTariff;
        if (tariff < 0)
            tariff = minTariff;

        return -Math.Round(tariff * minutes, 2);
    }

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

        // transaction as a negative value
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

    public async Task<List<char>> GetAvailableVehicleSymbols()
        => await _ctx.VehicleTypes.Select(vt => vt.Symbol).ToListAsync();

    public async Task<List<Floor>> GetFloorsByParkingLotId(int parkingLotId)
        => await _ctx.Floors.Where(f => f.ParkingLotId == parkingLotId)
        .Distinct().ToListAsync();

    public async Task AddNewFloor(Floor newFloor)
        => await _ctx.Floors.AddAsync(newFloor);

    public async Task AddNewSlot(Slot newSlot)
        => await _ctx.Slots.AddAsync(newSlot);

    public async Task SaveAllChangesAsync() => await _ctx.SaveChangesAsync();

    public async Task<int> GetNewFloorId(int parkingLotId, string floorName)
        => await _ctx.Floors.Where(f => f.ParkingLotId == parkingLotId && f.Name == floorName)
            .Select(f => f.Id).FirstOrDefaultAsync();

    public bool IsSlotCodeValid(string slotCode, int parkingLotId)
        => !_ctx.Slots.Include(s => s.Floor)
                .ThenInclude(f => f.ParkingLot)
                .Any(c => c.Code == slotCode && c.Floor.ParkingLot.Id == parkingLotId) 
                &&
                Regex.IsMatch(slotCode, @"^[A-Za-z]+[0-9]+$");


    public async Task<IList<Vehicle>> GetVehiclesByUserId(string userId)
        => await _ctx.Vehicles
                .Include(v => v.VehicleType)
                .Include(v => v.User)
                .Where(v => v.UserId == userId)
                .ToListAsync();


    public async Task<bool> CanDeleteVehicle(int vehicleId)
        => await _ctx.Vehicles.AnyAsync(v => v.Id == vehicleId
            && v.isParked == false);

    public async Task RemoveVehicleAndSaveAsync(Vehicle vehicle)
    {
        _ctx.Vehicles.Remove(vehicle);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<ParkingLot>> GetAllParkingLots()
        => await _ctx.ParkingLots.ToListAsync();

    public async Task<List<ParkingPermit>> GetUserPermitsByUserId(string userId)
        => await _ctx.ParkingPermits
        .Include(p => p.PermitInfo)
        .Include(p => p.Slot)
            .ThenInclude(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
        .Include(p => p.User)
        .Where(p => p.UserId == userId)
        .ToListAsync();

    public void AttachSateModified(ParkingLot parkingLot)
        => _ctx.Attach(parkingLot).State = EntityState.Modified;

    public void AttachSateModified(Slot slot)
    => _ctx.Attach(slot).State = EntityState.Modified;


    public bool IsValidParkingLot(int id)
        => _ctx.ParkingLots.Any(p => p.Id == id);

    public bool IsValidSlot(int id)
        => _ctx.Slots.Any(e => e.Id == id);

    public async Task<Slot> GetSlotAsyncById(int slotId)
        => await _ctx.Slots.Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .FirstOrDefaultAsync(m => m.Id == slotId);

    public async Task<List<Floor>> GetFloorsBySlot(Slot slot)
        => await _ctx.Slots.Include(s => s.Floor)
            .ThenInclude(f => f.ParkingLot)
            .Where(s => s.Floor.ParkingLotId == slot.Floor.ParkingLotId)
            .Select(s => s.Floor)
            .Distinct()
            .ToListAsync();

    public async Task<List<IdentityUser>> GetUsers()
        => await _ctx.Users.ToListAsync();

    public async Task AddMovementAndSaveAsync(Movement movement)
    {
        _ctx.Movements.Add(movement);
        await _ctx.SaveChangesAsync();
    } 

    public async Task<List<EntryAndExitHistory>> GetUserEntryAndExitHistory(string userId)
        => await _ctx.EntriesAndExitsHistory
            .Include(e => e.Movement)
            .Include(e => e.Slot)
                .ThenInclude(s => s.Floor)
                    .ThenInclude(f => f.ParkingLot)
            .Include(e => e.User)
            .Include(e => e.Vehicle)
            .Where(e => e.UserId == userId)
            .ToListAsync();

    public async Task<List<EntryAndExitHistory>> GetSlotsWhereUserIsParkedB(int parkingLotId, string userId)
       => await _ctx.EntriesAndExitsHistory
            .Include(eeh => eeh.Slot)
                .ThenInclude(s => s.Floor)
                .ThenInclude(f => f.ParkingLot)
            .Include(eeh => eeh.User)
            .Where(ehh => ehh.UserId == userId
            && ehh.Slot.Floor.ParkingLotId == parkingLotId
            && ehh.ExitAt == null)
            .ToListAsync();

    public async Task<IList<string>> GetSlotsWhereUserIsParked(int parkingLotId, string userId)
       => await (from s in _ctx.Slots
                       join f in _ctx.Floors on s.FloorId equals f.Id
                       join pl in _ctx.ParkingLots on f.ParkingLotId equals pl.Id
                       join eeh in _ctx.EntriesAndExitsHistory on s.Id equals eeh.SlotId
                       where eeh.UserId == userId && pl.Id == parkingLotId
                       && eeh.ExitAt == null
                       select s.Code).ToListAsync();
                


}

   //return await(from pl in _ctx.ParkingLots
   //                   join f in _ctx.Floors on pl.Id equals f.ParkingLotId
   //                   join s in _ctx.Slots on f.Id equals s.FloorId
   //                   where pl.Id == parkingLotId
   //                   select new ParkingModel



