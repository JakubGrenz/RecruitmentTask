using Guestline.Models;
using Guestline.Models.Commands;
using Guestline.Models.Database;
using Guestline.Utils;

namespace Guestline.Services
{
    public class SearchCommandHandlerService : ICommandHandlerService<SearchCommand, SearchCommandResult>
    {
        private DatabaseContext _databaseContext { get; set; }
        private IDateOnlyProvider _dateTimeProvider { get; set; }

        public SearchCommandHandlerService(DatabaseContext databaseContext, IDateOnlyProvider dateTimeProvider)
        {
            _databaseContext = databaseContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public SearchCommandResult Handle(SearchCommand command)
        {
            var result = new SearchCommandResult();
            var startDate = _dateTimeProvider.UtcNow;
            var endDate = startDate.AddDays(command.DaysInterval);

            var hotel = _databaseContext.Hotels.SingleOrDefault(h => h.Id == command.HotelId);

            if (hotel == null)
            {
                result.Error = $"Hotel '{command.HotelId}' could not be found";
                return result;
            }

            var overlappingBookings = _databaseContext.Bookings.Where(b =>
                command.HotelId == hotel.Id &&
                b.RoomType.Code == command.RoomCode &&
                startDate <= b.Departure &&
                b.Arrival <= endDate
            );

            var dateTimeRoomOffsets = BookingsToRoomCountOffestByDate(overlappingBookings, startDate, endDate);
            var defaultRoomCount = hotel.Rooms.Where(r => r.RoomType.Code == command.RoomCode).Count();

            var currentRoomCount = defaultRoomCount;
            result.SearchCommandResultRecords = new List<SearchCommandResultRecord>();
            for (int i = 1; i < dateTimeRoomOffsets.Count; i++) { 
                var currentElement = dateTimeRoomOffsets[i - 1];
                var nextElement = dateTimeRoomOffsets[i];

                var dateRange = new DateOnlyRange()
                {
                    StartDate = currentElement.Key,
                    EndDate = nextElement.Key == endDate ? endDate : nextElement.Key.AddDays(-1),
                };

                currentRoomCount = currentRoomCount + currentElement.Value;

                result.SearchCommandResultRecords.Add(new SearchCommandResultRecord()
                {
                    DateRange = dateRange,
                    NumberOfRooms = currentRoomCount
                });
            }

            return result;
        }

        public List<KeyValuePair<DateOnly, int>> BookingsToRoomCountOffestByDate(IEnumerable<Booking> bookings, DateOnly startDate, DateOnly endDate)
        {
            var result = new Dictionary<DateOnly, int>();

            foreach (var booking in bookings)
            {
                if (result.Keys.Contains(booking.Arrival))
                {
                    result[booking.Arrival] = result[booking.Arrival] - 1;
                }
                else
                {
                    result.Add(booking.Arrival, -1);
                }

                var positiveOffsetDay = booking.Departure.AddDays(1);
                if (result.Keys.Contains(positiveOffsetDay))
                {
                    result[positiveOffsetDay] = result[positiveOffsetDay] + 1;
                }
                else
                {
                    result.Add(positiveOffsetDay, + 1);
                }
            }

            var roomsStartingOffset = result
                .Where(b => b.Key <= startDate)
                .Sum(c => c.Value);

            if (!result.ContainsKey(startDate))
            {
                result.Add(startDate, roomsStartingOffset);
            }
            else
            {
                result[startDate] = roomsStartingOffset;
            }

            if (!result.ContainsKey(endDate)) {
                result.Add(endDate, 0);
            }

            return result.Where(date => date.Key >= startDate && date.Key <= endDate)
                .OrderBy(b => b.Key)
                .ToList();
        }
    }
}
