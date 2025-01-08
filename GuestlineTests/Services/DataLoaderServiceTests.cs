using FluentAssertions;
using Guestline.Models;
using Guestline.Models.Database;
using Guestline.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Internal;

namespace GuestlineTests.Services
{
    public class DataLoaderServiceTests
    {
        private string bookingsFilePath = "./Files/bookings.json";
        private string hotelFilePath = "./Files/hotels.json";

        private DataLoaderService dataLoaderService;
        private DatabaseContext databaseContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;

            databaseContext = new DatabaseContext(options);
            dataLoaderService = new DataLoaderService(databaseContext);
        }

        [TearDown]
        public void TearDown() {
            databaseContext.Database.EnsureDeleted();
            databaseContext.Dispose(); 
        }

        [Test]
        public void LoadHotelData_Test()
        {
            //Arrange
            var hotelId = "H1";
            var hotelRawData = File.ReadAllText(hotelFilePath);
            var expectedRoomTypes = new[] { "SGL", "DBL" };
            var expectedAminities = new[] { "WiFi", "TV", "Minibar" };
            var expectedFeatures = new[] { "Non-smoking", "Sea View"};
            var expectedRooms = new[] { 101, 102, 201, 202 };

            //Act
            dataLoaderService.LoadHotels(hotelRawData);

            //Assert
            var hotel = databaseContext.Hotels.Single(h =>
                h.Name == "Hotel California" &&
                h.Id == "H1"
            );


            Assert.Multiple(() =>
            {
                hotel.Should().NotBeNull();
                var rTypes = databaseContext.RoomTypes.ToList();

                foreach (var roomType in expectedRoomTypes)
                {
                    databaseContext.RoomTypes.SingleOrDefault(s => s.Code == roomType).Should().NotBeNull();
                }

                foreach (var aminitie in expectedAminities)
                {
                    databaseContext.RoomAmenities.SingleOrDefault(s => s.Name == aminitie).Should().NotBeNull();
                }

                foreach (var feature in expectedFeatures)
                {
                    databaseContext.RoomFeature.SingleOrDefault(s => s.Name == feature).Should().NotBeNull();
                }

                foreach (var room in expectedRooms)
                {
                    databaseContext.Rooms.SingleOrDefault(r => r.RoomNumber == room).Should().NotBeNull();
                }
            });
        }

        [Test]
        public void LoadBookingData_Test()
        {
            //Arrange
            var hotelRawData = File.ReadAllText(hotelFilePath);
            dataLoaderService.LoadHotels(hotelRawData);
            var bookingRawData = File.ReadAllText(bookingsFilePath);

            var expectedBookings = new List<Booking>()
            {
                new Booking()
                {
                    Arrival = new DateOnly(2024, 9, 01),
                    Departure = new DateOnly(2024, 9, 03),
                    RoomType = new RoomType()
                    {
                        Code = "DBL"
                    },
                    HotelId = "H1",
                    RoomRate = "Prepaid"
                },
                new Booking()
                {
                    Arrival = new DateOnly(2024, 9, 02),
                    Departure = new DateOnly(2024, 9, 05),
                    RoomType = new RoomType()
                    {
                        Code = "SGL"
                    },
                    HotelId = "H1",
                    RoomRate = "Standard"
                },
                new Booking()
                {
                    Arrival = new DateOnly(2024, 10, 01),
                    Departure = new DateOnly(2024, 9, 09),
                    RoomType = new RoomType()
                    {
                        Code = "DBL"
                    },
                    HotelId = "H1",
                    RoomRate = "Prepaid"
                },
                new Booking()
                {
                    Arrival = new DateOnly(2024, 10, 15),
                    Departure = new DateOnly(2024, 10, 22),
                    RoomType = new RoomType()
                    {
                        Code = "DBL"
                    },
                    HotelId = "H1",
                    RoomRate = "Prepaid"
                },
                new Booking()
                {
                    Arrival = new DateOnly(2024, 10, 17),
                    Departure = new DateOnly(2024, 10, 26),
                    RoomType = new RoomType()
                    {
                        Code = "DBL"
                    },
                    HotelId = "H1",
                    RoomRate = "Prepaid"
                },
            };

            //Act
            dataLoaderService.LoadBookings(bookingRawData);

            //Assert
            var dbBookings = databaseContext.Bookings.ToList();
            var comparer = new BookingComparer();
            Assert.That(dbBookings.Count, Is.EqualTo(5));
            Assert.That(dbBookings.Any(dbBooking => expectedBookings.Any(expectedBooking => comparer.Compare(dbBooking, expectedBooking) == 0)));
        }


        private class BookingComparer : IComparer<Booking>
        {
            public int Compare(Booking x, Booking y)
            {
                if (x.Departure != y.Departure ||
                    x.Arrival != y.Arrival ||
                    x.HotelId != y.HotelId ||
                    x.RoomRate != y.RoomRate ||
                    x.RoomType.Code != y.RoomType.Code)
                {
                    return -1;
                }

                return 0;
            }
        }
    }
}
