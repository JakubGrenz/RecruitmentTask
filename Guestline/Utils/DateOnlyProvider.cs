namespace Guestline.Utils
{
    public interface IDateOnlyProvider
    {
        DateOnly UtcNow { get; }
    }

    public class DateOnlyProvider : IDateOnlyProvider
    {
        public DateOnly UtcNow => DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
