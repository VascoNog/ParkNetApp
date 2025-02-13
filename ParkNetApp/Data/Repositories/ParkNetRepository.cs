namespace ParkNetApp.Data.Repositories;

public class ParkNetRepository
{
    private readonly ParkNetDbContext _context;
    public ParkNetRepository(ParkNetDbContext context) => _context = context;

    public bool IsAnActiveAccount(string userId) 
        => _context.UserInfos.Any(u => u.Id == userId && u.IsActivated == true);


}
