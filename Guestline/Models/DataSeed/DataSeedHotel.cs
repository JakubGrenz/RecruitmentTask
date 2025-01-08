namespace Guestline.Models.DataSeed
{
    public class DataSeedHotel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<DataSeedRoomType> RoomTypes { get; set; }
        public List<DataSeedRoom> Rooms { get; set; }
    }

    public class DataSeedRoomType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public List<string> Amenities { get; set; }
        public List<string> Features { get; set; }
    }

    public class DataSeedRoom
    {
        public string RoomType { get; set; }
        public string RoomId { get; set; }
    }
}
