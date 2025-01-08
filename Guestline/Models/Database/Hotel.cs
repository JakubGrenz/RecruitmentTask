using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guestline.Models.Database
{
    public class Hotel
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<RoomType> RoomTypes { get; set; }
    }

    public class RoomType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomTypeId { get; set; }
        [ForeignKey(nameof(Hotel))]
        public string HotelId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RoomAmenity> Amenities { get; set; }
        public virtual ICollection<RoomFeature> Features { get; set; }
        public virtual Hotel Hotel { get; set; }
    }

    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }
        [ForeignKey(nameof(RoomType))]
        public int RoomTypeId { get; set; }
        [ForeignKey(nameof(Hotel))]
        public string HotelId { get; set; }
        public int RoomNumber { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual RoomType RoomType { get; set; }
    }

    public class RoomTypeAmenity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomTypeAmenityId { get; set; }
        [ForeignKey(nameof(RoomType))]
        public int RoomTypeId { get; set; }
        [ForeignKey(nameof(RoomAmenity))]
        public int RoomAmenityId { get; set; }

        public virtual RoomType RoomType { get; set; }
        public virtual RoomAmenity RoomAmenity { get; set; }
    }

    public class RoomTypeFeature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomTypeFeatureId { get; set; }
        [ForeignKey(nameof(RoomType))]
        public int RoomTypeId { get; set; }
        [ForeignKey(nameof(RoomFeature))]
        public int RoomFeatureId { get; set; }

        public virtual RoomType RoomType { get; set; }
        public virtual RoomFeature RoomFeature { get; set; }
    }

    public class RoomAmenity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomAmenityId { get; set; }
        public string Name { get; set; }
    }

    public class RoomFeature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomFeatureId { get; set; }
        public string Name { get; set; }
    }
}
