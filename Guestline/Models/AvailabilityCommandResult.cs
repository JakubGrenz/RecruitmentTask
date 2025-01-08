namespace Guestline.Models
{
    public class AvailabilityCommandResult: IPrintableCommandResult
    {
        public bool Success
        {
            get
            {
                return Error == null && NumberOfRooms != null;
            }
        }

        public string? Error { get; set; }
        public int? NumberOfRooms { get; set; }

        public string ToConsoleOutput()
        {
            if(!Success)
            {
                return Error ?? string.Empty;
            }

            return $"({NumberOfRooms})";
        }
    }
}
