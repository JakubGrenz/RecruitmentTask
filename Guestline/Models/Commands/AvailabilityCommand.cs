namespace Guestline.Models.Commands
{
    public class AvailabilityCommand : Command
    {
        public DateOnlyRange DateRange { get; set; }
    }

    public class DateOnlyRange
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
