using FluentAssertions;
using Guestline.Models.Commands;
using Guestline.Models.Database;
using Guestline.Services;
using Microsoft.EntityFrameworkCore;

namespace GuestlineTests.Services
{
    public class AvailabilityCommandHandlerServiceTests
    {
        private string bookingsFilePath = "./Files/bookings.json";
        private string hotelFilePath = "./Files/hotels.json";

        private DataLoaderService _dataLoaderService;
        private DatabaseContext _databaseContext;
        private AvailabilityCommandHandlerService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;

            _databaseContext = new DatabaseContext(options);
            _dataLoaderService = new DataLoaderService(_databaseContext);

            var hotelRawData = File.ReadAllText(hotelFilePath);
            _dataLoaderService.LoadHotels(hotelRawData);
            var bookingRawData = File.ReadAllText(bookingsFilePath);
            _dataLoaderService.LoadBookings(bookingRawData);

            _service = new AvailabilityCommandHandlerService(_databaseContext);
        }

        [TearDown]
        public void TearDown()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();
        }

        [TestCase(8, 28, 9, 2)]
        [TestCase(9, 2, 9, 5)]
        [TestCase(9, 1, 9, 2)]
        public void Handle_TheAvailabilityForDayRange_1Books(int startMonth, int startDay, int endMonth, int endDay)
        {
            var command = new AvailabilityCommand()
            {
                DateRange = new DateOnlyRange()
                {
                    StartDate = new DateOnly(2024, startMonth, startDay),
                    EndDate = new DateOnly(2024, endMonth, endDay),
                },
                HotelId = "H1",
                RoomCode = "DBL"
            };

            var result = _service.Handle(command);

            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.NumberOfRooms.Should().Be(1);
        }


        [TestCase(09, 25)]
        [TestCase(08, 30)]
        public void Handle_TheAvailabilityForSingleDay_0Books(int month, int day)
        {
            var bookingDate = new DateOnly(2024, month, day);
            var command = new AvailabilityCommand()
            {
                DateRange = new DateOnlyRange()
                {
                    StartDate = bookingDate,
                    EndDate = bookingDate
                },
                HotelId = "H1",
                RoomCode = "DBL"
            };

            var result = _service.Handle(command);

            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.NumberOfRooms.Should().Be(2);
        }

        [TestCase(9, 3)]
        [TestCase(9, 1)]
        public void Handle_TheAvailabilityForSingleDay_1Book(int month, int day)
        {
            var bookingDate = new DateOnly(2024, month, day);
            var command = new AvailabilityCommand()
            {
                DateRange = new DateOnlyRange()
                {
                    StartDate = bookingDate,
                    EndDate = bookingDate
                },
                HotelId = "H1",
                RoomCode = "DBL"
            };

            var result = _service.Handle(command);

            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.NumberOfRooms.Should().Be(1);
        }

        [Test]
        public void Handle_HotelDoesntExists()
        {
            var day = new DateOnly(2024, 09, 25);
            var command = new AvailabilityCommand()
            {
                DateRange = new DateOnlyRange()
                {
                    StartDate = day,
                    EndDate = day.AddDays(1)
                },
                HotelId = "H5",
                RoomCode = "DBL"
            };

            var result = _service.Handle(command);

            result.Success.Should().BeFalse();
            result.Error.Should().BeEquivalentTo("Hotel 'H5' could not be found");
            result.NumberOfRooms.Should().BeNull();
        }
    }
}
