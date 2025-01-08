using System.Globalization;

namespace Guestline.Utils
{
    public class DateFormatter
    {
        private const string dateFormat = "yyyyMMdd";

        public static DateOnly ConvertToDateOnly(string dateString)
        {
            if (DateOnly.TryParseExact(
                    dateString,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateOnly parsedDate))
            {
                return parsedDate;
            }
            else
            {
                throw new FormatException("The input string is not in the correct format: yyyyMMdd.");
            }
        }

        public static string CovertToString(DateOnly dateOnly)
        {
            return dateOnly.ToString(dateFormat);
        }
    }
}
