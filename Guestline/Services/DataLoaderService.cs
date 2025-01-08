using Guestline.Models;
using Guestline.Models.Database;
using Guestline.Models.DataSeed;
using Guestline.Utils;
using System.Text.Json;

namespace Guestline.Services
{
    public interface IDataLoaderService
    {
        void LoadHotels(string hotelData);
        void LoadBookings(string bookingsData);
    }

    public class DataLoaderService: IDataLoaderService
    {
        private DatabaseContext _databaseContext { get; set; }
        private JsonSerializerOptions _jsonSerializerOptions { get; set; }

        public DataLoaderService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public void LoadBookings(string bookingsData)
        {
            var bookings = JsonSerializer.Deserialize<List<DataSeedBooking>>(bookingsData, _jsonSerializerOptions);

            if (bookings == null || bookings.Count == 0)
            {
                return;
            }

            var hotelIds = bookings.Select(b => b.HotelId).Distinct();
            var dbHotels = _databaseContext.Hotels.Where(h => hotelIds.Contains(h.Id));

            var dbBookings = bookings.Select(b => new Booking()
            {
                Arrival = DateFormatter.ConvertToDateOnly(b.Arrival),
                Departure = DateFormatter.ConvertToDateOnly(b.Departure),
                HotelId = b.HotelId,
                RoomRate = b.RoomRate,
                RoomType = dbHotels.Single(h => h.Id == b.HotelId).RoomTypes.Single(r => r.Code == b.RoomType)
            }).ToList();

            _databaseContext.Bookings.AddRange(dbBookings);
            _databaseContext.SaveChanges();
        }

        public void LoadHotels(string hotelData)
        {
            var hotels = JsonSerializer.Deserialize<List<DataSeedHotel>>(hotelData, _jsonSerializerOptions);

            if(hotels == null || hotels.Count == 0)
            {
                return;
            }

            var amenities = hotels
                .SelectMany(h => h.RoomTypes)
                .SelectMany(r => r.Amenities)
                .Distinct()
                .Select(a => new RoomAmenity() { Name = a }).ToList();

            var features = hotels
                .SelectMany(h => h.RoomTypes)
                .SelectMany(r => r.Features)
                .Distinct()
                .Select(a => new RoomFeature() { Name = a }).ToList();

            _databaseContext.RoomAmenities.AddRange(amenities);
            _databaseContext.RoomFeature.AddRange(features);
            _databaseContext.SaveChanges();

            foreach (var hotel in hotels)
            {
                var dbHotel = new Hotel()
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                };

                _databaseContext.Hotels.Add(dbHotel);
                _databaseContext.SaveChanges();

                var roomTypes = hotel.RoomTypes.Select(roomType => new RoomType()
                {
                    Amenities = amenities.Where(dba => roomType.Amenities.Any(a => dba.Name == a)).ToList(),
                    Features = features.Where(dbf => roomType.Features.Any(f => dbf.Name == f)).ToList(),
                    Code = roomType.Code,
                    Description = roomType.Description,
                    Hotel = dbHotel
                }).ToList();

                _databaseContext.RoomTypes.AddRange(roomTypes);
                _databaseContext.SaveChanges();

                var rooms = hotel.Rooms.Select(room => new Room()
                {
                    RoomNumber = int.Parse(room.RoomId),
                    RoomType = roomTypes.Single(r => r.Code == room.RoomType),
                    Hotel = dbHotel,
                }).ToList();

                _databaseContext.Rooms.AddRange(rooms);
                _databaseContext.SaveChanges();
            }
        }
    }
}
