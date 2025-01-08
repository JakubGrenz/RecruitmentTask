using FluentAssertions;
using Guestline.Models;
using Guestline.Models.Commands;
using Guestline.Models.Database;
using Guestline.Services;
using Guestline.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GuestlineTests.Services
{
    public class SearchCommandHandlerServiceTests
    {
        private string bookingsFilePath = "./Files/bookings.json";
        private string hotelFilePath = "./Files/hotels.json";

        private DataLoaderService _dataLoaderService;
        private DatabaseContext _databaseContext;
        private SearchCommandHandlerService _service;
        private Mock<IDateOnlyProvider> _dateTimeProivder;

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

            _dateTimeProivder = new Mock<IDateOnlyProvider>();
            _service = new SearchCommandHandlerService(_databaseContext, _dateTimeProivder.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();
        }

        [Test]
        public void Handle_TheSearchCommand_DateRangeIsOverAllBookings()
        {
            //Arrange
            var daysRange = 60;
            var startDate = new DateOnly(2024, 08, 01);
            _dateTimeProivder.Setup(s => s.UtcNow).Returns(startDate);
            var expectedResponse = new SearchCommandResult()
            {
                SearchCommandResultRecords = new List<SearchCommandResultRecord>() {
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = startDate,
                            EndDate = new DateOnly(2024, 09, 01).AddDays(-1),
                        },
                        NumberOfRooms = 2
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 09, 01),
                            EndDate = new DateOnly(2024, 09, 03),
                        },
                        NumberOfRooms = 1
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 09, 04),
                            EndDate = startDate.AddDays(daysRange),
                        },
                        NumberOfRooms = 2
                    },
                }
            };

            var command = new SearchCommand()
            {
                DaysInterval = daysRange,
                HotelId = "H1",
                RoomCode = "DBL"
            };

            //Act
            var result = _service.Handle(command);

            //Assert
            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Handle_TheSearchCommand_DateRangeStartsAtTheBeggingOfFirstBooking()
        {
            //Arrange
            var daysRange = 30;
            var startDate = new DateOnly(2024, 09, 01);
            _dateTimeProivder.Setup(s => s.UtcNow).Returns(startDate);
            var expectedResponse = new SearchCommandResult()
            {
                SearchCommandResultRecords = new List<SearchCommandResultRecord>() {
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = startDate,
                            EndDate = new DateOnly(2024, 09, 03),
                        },
                        NumberOfRooms = 1
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 09, 04),
                            EndDate = startDate.AddDays(daysRange),
                        },
                        NumberOfRooms = 2
                    },
                }
            };

            var command = new SearchCommand()
            {
                DaysInterval = daysRange,
                HotelId = "H1",
                RoomCode = "DBL"
            };

            //Act
            var result = _service.Handle(command);

            //Assert
            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Handle_TheSearchCommand_DateRangeEndsAtTheEndOfBooking()
        {
            //Arrange
            var startDate = new DateOnly(2024, 08, 01);
            var endDate = new DateOnly(2024, 09, 03);
            _dateTimeProivder.Setup(s => s.UtcNow).Returns(startDate);
            var expectedResponse = new SearchCommandResult()
            {
                SearchCommandResultRecords = new List<SearchCommandResultRecord>() {
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = startDate,
                            EndDate = new DateOnly(2024, 08, 31),
                        },
                        NumberOfRooms = 2
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 09, 01),
                            EndDate = endDate,
                        },
                        NumberOfRooms = 1
                    },
                }
            };

            var command = new SearchCommand()
            {
                DaysInterval = (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).Days,
                HotelId = "H1",
                RoomCode = "DBL"
            };

            //Act
            var result = _service.Handle(command);

            //Assert
            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Handle_TheSearchCommand_DateRangeFullCoversBooking()
        {
            //Arrange
            var startDate = new DateOnly(2024, 09, 01);
            _dateTimeProivder.Setup(s => s.UtcNow).Returns(startDate);
            var expectedResponse = new SearchCommandResult()
            {
                SearchCommandResultRecords = new List<SearchCommandResultRecord>() {
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = startDate,
                            EndDate = new DateOnly(2024, 09, 03),
                        },
                        NumberOfRooms = 1
                    },
                }
            };

            var command = new SearchCommand()
            {
                DaysInterval = 2,
                HotelId = "H1",
                RoomCode = "DBL"
            };

            //Act
            var result = _service.Handle(command);

            //Assert
            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Handle_TheSearchCommand_OverlappingBookings()
        {
            //Arrange
            var daysRange = 365;
            var startDate = new DateOnly(2024, 08, 01);
            var endate = startDate.AddDays(daysRange);
            _dateTimeProivder.Setup(s => s.UtcNow).Returns(startDate);
            var expectedResponse = new SearchCommandResult()
            {
                SearchCommandResultRecords = new List<SearchCommandResultRecord>() {
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = startDate,
                            EndDate = new DateOnly(2024, 09, 01).AddDays(-1),
                        },
                        NumberOfRooms = 2
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 09, 01),
                            EndDate = new DateOnly(2024, 09, 03),
                        },
                        NumberOfRooms = 1
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 09, 04),
                            EndDate = new DateOnly(2024, 10, 01).AddDays(-1),
                        },
                        NumberOfRooms = 2
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 10, 01),
                            EndDate = new DateOnly(2024, 10, 09)
                        },
                        NumberOfRooms = 1
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 10, 10),
                            EndDate = new DateOnly(2024, 10, 14)
                        },
                        NumberOfRooms = 2
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 10, 15),
                            EndDate = new DateOnly(2024, 10, 16)
                        },
                        NumberOfRooms = 1
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 10, 17),
                            EndDate = new DateOnly(2024, 10, 22)
                        },
                        NumberOfRooms = 0
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 10, 23),
                            EndDate = new DateOnly(2024, 10, 26)
                        },
                        NumberOfRooms = 1
                    },
                    new SearchCommandResultRecord()
                    {
                        DateRange = new DateOnlyRange()
                        {
                            StartDate = new DateOnly(2024, 10, 27),
                            EndDate = endate
                        },
                        NumberOfRooms = 2
                    },
                }
            };

            var command = new SearchCommand()
            {
                DaysInterval = daysRange,
                HotelId = "H1",
                RoomCode = "DBL"
            };

            //Act
            var result = _service.Handle(command);

            //Assert
            result.Success.Should().BeTrue();
            result.Error.Should().BeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
