﻿using Microsoft.Identity.Client;

namespace ParkNetApp.Data;

public class ParkNetDbContext : IdentityDbContext
{
    public ParkNetDbContext(DbContextOptions<ParkNetDbContext> options)
        : base(options)
    {
    }

    public DbSet<EntryAndExitHistory> EntriesAndExitsHistory {get; set; }
    public DbSet<Floor> Floors { get; set; }    
    public DbSet<ParkingLot> ParkingLots { get; set; }
    public DbSet<ParkingPermit> ParkingPermits { get; set; }
    public DbSet<PermitPrice> PermitPrices { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Configuration of ActivityHistory Entity  
        modelBuilder.Entity<EntryAndExitHistory> (entity =>
        {
            entity.Property(e => e.EntryAt).IsRequired().HasColumnType("datetime2");
            entity.Property(e => e.ExitAt).HasColumnType("datetime2(0)");
        });


        // Configuration of Floor Entity
        modelBuilder.Entity<Floor> (entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasColumnType("nvarchar(10)");
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
            entity.Property(e => e.SartedAt).IsRequired().HasColumnType("datetime2(0)");
            entity.Property(e => e.DaysOfPermit).IsRequired();
        });

        // Configuration of PermitPrice Entity
        modelBuilder.Entity<PermitPrice>(entity =>
        {
            entity.Property(e => e.PermitType).IsRequired().HasColumnType("nvarchar(20)");
            entity.Property(e => e.Value).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.ActiveUntil).HasColumnType("date");
            entity.Property(e => e.IsActive); // BOOLEAN TYPE - NULLABLE
        });


        // Configuration of Slot Entity
        modelBuilder.Entity<Slot>(entity =>
        {
            entity.Property(e => e.Code).IsRequired().HasColumnType("nvarchar(10)");
            /*entity.HasIndex(e => new { e.Code, e.FloorId }).IsUnique(); */// Torna Code Unico dentro de cada Floor
            entity.Property(e => e.SlotType).IsRequired().HasColumnType("nchar(1)");
            entity.Property(e => e.IsOccupied).IsRequired(); // BOOLEAN TYPE; True if occupied, False if not
        });

        // Configuration of Transaction Entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.TransactionDate).IsRequired().HasColumnType("datetime2");
            entity.Property(e => e.Amount).IsRequired(). HasColumnType("decimal(18,2)");
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
            entity.Property(e => e.IsActivated).IsRequired();
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
    }
}