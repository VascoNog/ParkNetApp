using Microsoft.EntityFrameworkCore;
using ParkNetApp.Data;
using ParkNetApp.Data.Entities;
using ParkNetApp.Data.Repositories;
using Xunit;

namespace ParkNet.Tests
{
    public class ParkingPrice
    {
        private readonly ParkNetDbContext _context;
        private readonly ParkNetRepository _repo;

        public ParkingPrice()
        {
            var options = new DbContextOptionsBuilder<ParkNetDbContext>()
                .UseInMemoryDatabase("TestDB")
                .Options;

            _context = new ParkNetDbContext(options); 
            _repo = new ParkNetRepository(_context); // Encapsular o context

            if (!_context.NonSubscriptionParkingTariffs.Any())
            {
                _context.NonSubscriptionParkingTariffs.AddRange(new List<NonSubscriptionParkingTariff>
                {
                    new NonSubscriptionParkingTariff { Limit = 5, Tariff = 0.07d, ActiveSince = new DateOnly(2025, 1, 1) },
                    new NonSubscriptionParkingTariff { Limit = 15, Tariff = 0.05d, ActiveSince = new DateOnly(2025, 1, 1) },
                    new NonSubscriptionParkingTariff { Limit = 30, Tariff = 0.04d, ActiveSince = new DateOnly(2025, 1, 1) },
                    new NonSubscriptionParkingTariff { Limit = 60, Tariff = 0.03d, ActiveSince = new DateOnly(2025, 1, 1) },
                    new NonSubscriptionParkingTariff { Limit = 120, Tariff = 0.02d, ActiveSince = new DateOnly(2025, 1, 1) }
                });

                _context.SaveChanges(); // Guarda os dados na memória (RAM)
            }
        }

        [Fact]
        public void StayOfFourMinutesAndFiftyNineSecondsWithoutActivePermit_ReturnsCorrectPrice()
        {
            // Arrange
            DateTime dateTimeReference = DateTime.UtcNow;
            DateTime entryAt = dateTimeReference.AddMinutes(-4).AddSeconds(-59);
            DateTime exitAt = dateTimeReference;
            double minutes = (exitAt - entryAt).TotalMinutes;

            // Act
            var result = _repo.GetCorrectParkingAmount(minutes);

            // Assert
            Assert.Equal(-0.35, result);
        }

        [Fact]
        public void StayOfFourteenMinutesAndFiftySecondsWithoutActivePermit_ReturnsCorrectPrice()
        {
            // Arrange
            DateTime dateTimeReference = DateTime.UtcNow;
            DateTime entryAt = dateTimeReference.AddMinutes(-14).AddSeconds(-50);
            DateTime exitAt = dateTimeReference;
            double minutes = (exitAt - entryAt).TotalMinutes;

            // Act
            var result = _repo.GetCorrectParkingAmount(minutes);

            // Assert
            Assert.Equal(-0.74, result);
        }

        [Fact]
        public void StayOfFortyFiveMinutesWithoutActivePermit_ReturnsCorrectPrice()
        {
            // Arrange
            DateTime dateTimeReference = DateTime.UtcNow;
            DateTime entryAt = dateTimeReference.AddMinutes(-45);
            DateTime exitAt = dateTimeReference;
            double minutes = (exitAt - entryAt).TotalMinutes;

            // Act
            var result = _repo.GetCorrectParkingAmount(minutes);

            // Assert
            Assert.Equal(-1.35, result);
        }

        [Fact]
        public void StayOfTenHoursAndHalfWithoutActivePermit_ReturnsCorrectPrice()
        {
            // Arrange
            DateTime dateTimeReference = DateTime.UtcNow;
            DateTime entryAt = dateTimeReference.AddHours(-10).AddMinutes(-30);
            DateTime exitAt = dateTimeReference;
            double minutes = (exitAt - entryAt).TotalMinutes;

            // Act
            var result = _repo.GetCorrectParkingAmount(minutes);

            // Assert
            Assert.Equal(-6.30, result);
        }

        [Fact]
        public void StayOfTwelveDaysAndFourHoursWithoutActivePermit_ReturnsCorrectPrice()
        {
            // Arrange
            DateTime dateTimeReference = DateTime.UtcNow;
            DateTime entryAt = dateTimeReference.AddDays(-12).AddHours(-4);
            DateTime exitAt = dateTimeReference;
            double minutes = (exitAt - entryAt).TotalMinutes;

            // Act
            var result = _repo.GetCorrectParkingAmount(minutes);

            // Assert
            Assert.Equal(-175.20, result);
        }
    }
}
