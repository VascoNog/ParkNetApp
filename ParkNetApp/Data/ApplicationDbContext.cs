namespace ParkNetApp.Data;

public class ParkNetDbContext : IdentityDbContext
{
    public ParkNetDbContext(DbContextOptions<ParkNetDbContext> options)
        : base(options)
    {
    }

    public DbSet<ParkingLot> ParkingLots { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ParkingPermit> ParkingPermits { get; set; }
    public DbSet<ActivityHistory> ActivityHistories  {get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        // Configuration of ParkingLot Entity
       modelBuilder.Entity<ParkingLot>(entity =>
        {
            entity.Property(e => e.Designation).IsRequired().HasColumnType("nvarchar(100)");
            entity.Property(e => e.Address).IsRequired().HasColumnType("nvarchar(500)");
            entity.Property(e => e.City).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(e => e.Country).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(e => e.PLIC).IsRequired().HasColumnType("nvarchar(50)"); // Parking Lot Identification Code ID+City+Country?
            entity.HasIndex(e => e.PLIC).IsUnique();
            entity.Property(e => e.ActiveAt).IsRequired().HasColumnType("date");
            entity.Property(e => e.Description).HasColumnType("nvarchar(500)"); // NULLABLE
        });

        // Configuration of UserInfo Entity
        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.Property(e => e.CreditCardNumb).IsRequired().HasColumnType("nvarchar(25)");
            entity.HasIndex(e => e.CreditCardNumb).IsUnique();
            entity.Property(e => e.CCExpDate).IsRequired().HasColumnType("date");
            entity.Property(e => e.CCBalance).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.DriverLicenseNumber).IsRequired().HasColumnType("nvarchar(25)");
            entity.HasIndex(e => e.DriverLicenseNumber).IsUnique();
            entity.Property(e => e.DLExpDate).IsRequired().HasColumnType("date");

        });

        // Configuration of Vehicle Entity
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.Property(e => e.PlateNumber).IsRequired().HasColumnType("nvarchar(25)");
            entity.Property(e => e.Type).IsRequired().HasColumnType("nvarchar(20)");
            entity.HasIndex(e => e.PlateNumber).IsUnique();
            entity.Property(e => e.Make).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(e => e.Model).IsRequired().HasColumnType("nvarchar(50)");
        });

        // Configuration of ParkingPermit Entity
        modelBuilder.Entity<ParkingPermit>(entity =>
        {
            entity.Property(e => e.SartedAt).IsRequired().HasColumnType("datetime2(0)");
            entity.Property(e => e.DaysOfPermit).IsRequired();
        });

        // Configuration of ActivityHistory Entity  
        modelBuilder.Entity<ActivityHistory> (entity =>
        {
            entity.Property(e => e.EntryAt).IsRequired().HasColumnType("datetime2");
            entity.Property(e => e.ExitAt).HasColumnType("datetime2(0)");
            entity.Property(e => e.Fee).HasColumnType("decimal(18,2)");
        });
    }
}