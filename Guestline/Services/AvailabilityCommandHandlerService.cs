using Guestline.Models;
using Guestline.Models.Commands;
using Guestline.Models.Database;

namespace Guestline.Services
{
    public class AvailabilityCommandHandlerService: ICommandHandlerService<AvailabilityCommand, AvailabilityCommandResult>
    {
        private DatabaseContext _databaseContext { get; set; }

        public AvailabilityCommandHandlerService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public AvailabilityCommandResult Handle(AvailabilityCommand command)
        {
            var result = new AvailabilityCommandResult();
            var hotel = _databaseContext.Hotels.SingleOrDefault(h => h.Id == command.HotelId);

            if(hotel == null) {
                result.Error = $"Hotel '{command.HotelId}' could not be found";
                return result;
            }

            var roomCount = hotel.Rooms.Where(r => r.RoomType.Code == command.RoomCode).Count();
            var overlappingBookings = _databaseContext.Bookings.Where(b =>
                command.HotelId == hotel.Id &&
                b.RoomType.Code == command.RoomCode &&
                command.DateRange.StartDate <= b.Departure &&
                b.Arrival <= command.DateRange.EndDate
            ).Count();

            return new AvailabilityCommandResult()
            {
                NumberOfRooms = roomCount - overlappingBookings
            };
        }
    }
}
