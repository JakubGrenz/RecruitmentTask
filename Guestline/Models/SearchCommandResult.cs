using Guestline.Models.Commands;
using Guestline.Utils;
using System.Text;

namespace Guestline.Models
{
    public class SearchCommandResult: IPrintableCommandResult
    {
        public bool Success
        {
            get
            {
                return Error == null && SearchCommandResultRecords != null;
            }
        }
        public string? Error { get; set; }
        public List<SearchCommandResultRecord>? SearchCommandResultRecords { get; set; }

        public string ToConsoleOutput()
        {
            if (!Success || SearchCommandResultRecords == null)
            {
                return Error ?? string.Empty;
            }

            var outputBuilder = new StringBuilder();
            foreach (var result in SearchCommandResultRecords)
            {
                var startDate = result.DateRange.StartDate;
                var endDate = result.DateRange.EndDate;
                if (startDate == endDate)
                {
                    outputBuilder.AppendLine($"({DateFormatter.CovertToString(startDate)}, {result.NumberOfRooms})");
                }
                else {
                    outputBuilder.AppendLine($"({DateFormatter.CovertToString(startDate)}-{DateFormatter.CovertToString(endDate)}, {result.NumberOfRooms})");
                }
            }

            return outputBuilder.ToString();
        }
    }

    public class SearchCommandResultRecord
    {
        public DateOnlyRange DateRange { get; set; }
        public int NumberOfRooms { get; set; }
    }
}
