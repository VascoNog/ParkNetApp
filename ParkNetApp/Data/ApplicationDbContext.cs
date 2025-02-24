using Microsoft.Identity.Client;

namespace ParkNetApp.Data;

public class ParkNetDbContext : IdentityDbContext
{
    public ParkNetDbContext(DbContextOptions<ParkNetDbContext> options)
        : base(options)
    {
    }

    public DbSet<EntryAndExitHistory> EntriesAndExitsHistory { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<NonSubscriptionParkingTariff> NonSubscriptionParkingTariffs { get; set; }
    public DbSet<ParkingLot> ParkingLots { get; set; }
    public DbSet<ParkingPermit> ParkingPermits { get; set; }
    public DbSet<PermitInfo> PermitInfos { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Movement> Movements { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<EmailBox> EmailBoxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration of ActivityHistory Entity
        modelBuilder.Entity<EntryAndExitHistory>(entity =>
        {
            entity.Property(e => e.EntryAt).IsRequired().HasColumnType("datetime2");
            entity.Property(e => e.ExitAt).HasColumnType("datetime2(0)");
        });

        // Configuration of Floor Entity
        modelBuilder.Entity<Floor>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasColumnType("nvarchar(10)");
        });

        // Configuration of NonSubscriptionParkingTariff Entity
        modelBuilder.Entity<NonSubscriptionParkingTariff>(entity =>
        {
            entity.Property(e => e.Limit).IsRequired().HasColumnType("decimal(5,2)");
            entity.Property(e => e.Tariff).IsRequired().HasColumnType("decimal(5,2)");
            entity.Property(e => e.ActiveSince).IsRequired().HasColumnType("date");
        });

        // Configuration of ParkingLot Entity
        modelBuilder.Entity<ParkingLot>(entity =>
        {
            entity.Property(e => e.Designation).IsRequired().HasColumnType("nvarchar(100)");
            entity.HasIndex(e => e.Designation).IsUnique();
            entity.Property(e => e.Address).IsRequired().HasColumnType("nvarchar(500)");
            entity.Property(e => e.City).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(e => e.Country).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(e => e.ActiveAt).IsRequired().HasColumnType("date");
            entity.Property(e => e.Layout).IsRequired().HasColumnType("nvarchar(500)");
            entity.Property(e => e.Description).HasColumnType("nvarchar(500)"); // NULLABLE
        });

        // Configuration of ParkingPermit Entity
        modelBuilder.Entity<ParkingPermit>(entity =>
        {
            entity.Property(e => e.StartedAt).IsRequired().HasColumnType("datetime2(0)");
        });

        // Configuration of PermitPrice Entity
        modelBuilder.Entity<PermitInfo>(entity =>
        {
            entity.Property(e => e.DaysOfPermit).IsRequired().HasColumnType("int");
            entity.Property(e => e.Value).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.ActiveSince).IsRequired().HasColumnType("date");
            entity.Property(e => e.ActiveUntil).HasColumnType("date");
        });

        // Configuration of Slot Entity
        modelBuilder.Entity<Slot>(entity =>
        {
            entity.Property(e => e.Code).IsRequired().HasColumnType("nvarchar(10)");
            entity.Property(e => e.SlotType).IsRequired().HasColumnType("nchar(1)");
            entity.Property(e => e.IsOccupied).IsRequired();
        });

        // Configuration of Movement Entity
        modelBuilder.Entity<Movement>(entity =>
        {
            entity.Property(e => e.TransactionDate).HasColumnType("datetime2");
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.TransactionType).HasColumnType("nvarchar(500)"); // NULLABLE
        });

        // Configuration of UserInfo Entity
        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.Property(e => e.CreditCardNumb).IsRequired().HasColumnType("nvarchar(25)");
            entity.HasIndex(e => e.CreditCardNumb).IsUnique();
            entity.Property(e => e.CCExpDate).IsRequired().HasColumnType("date");
            entity.Property(e => e.DriverLicenseNumber).IsRequired().HasColumnType("nvarchar(25)");
            entity.HasIndex(e => e.DriverLicenseNumber).IsUnique();
            entity.Property(e => e.DLExpDate).IsRequired().HasColumnType("date");
            entity.Property(e => e.ParkNetCardBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IsActivated).IsRequired();
        });

        // Configuration of Vehicle Entity
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.Property(e => e.PlateNumber).IsRequired().HasColumnType("nvarchar(25)");
            entity.HasIndex(e => e.PlateNumber).IsUnique();
            entity.Property(e => e.Make).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(e => e.Model).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(e => e.isParked).IsRequired();
        });

        // Configuration of VehicleType Entity
        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.Property(e => e.Type).IsRequired().HasColumnType("nvarchar(20)");
            entity.HasIndex(e => e.Type).IsUnique();
            entity.Property(e => e.Symbol).IsRequired().HasColumnType("nchar(1)");
            entity.HasIndex(e => e.Symbol).IsUnique();
        });

        // Configuration of EmailBox Entity
        modelBuilder.Entity<EmailBox>(entity =>
        {
            entity.Property(e => e.Subject).IsRequired().HasColumnType("nvarchar(100)");
            entity.Property(e => e.Description).IsRequired().HasColumnType("nvarchar(500)");
            entity.Property(e => e.SentAt).IsRequired().HasColumnType("datetime2");
        });
    }
}