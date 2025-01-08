using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guestline.Models.Database
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [ForeignKey(nameof(Hotel))]
        public string HotelId { get; set; }
        public DateOnly Arrival { get; set; }
        public DateOnly Departure { get; set; }
        [ForeignKey(nameof(RoomType))]
        public int RoomTypeId { get; set; }
        public string RoomRate { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual RoomType RoomType { get; set; }
    }
}
