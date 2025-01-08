using System.ComponentModel.DataAnnotations;

namespace Guestline.Models.DataSeed
{
    public class DataSeedBooking
    {
        public string HotelId { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public string RoomType { get; set; }
        public string RoomRate { get; set; }
    }
}
