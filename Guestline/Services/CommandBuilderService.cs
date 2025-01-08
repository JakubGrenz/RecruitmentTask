using Guestline.Models;
using Guestline.Models.Commands;
using Guestline.Utils;
using System.Text.RegularExpressions;

namespace Guestline.Services
{
    public interface ICommandBuilderService
    {
        public BuildCommandResult BuildCommand(string text);
    }

    public class CommandBuilderService: ICommandBuilderService
    {
        private string pattern = @"^(\w+)\(([^)]+)\)$";

        public BuildCommandResult BuildCommand(string text)
        {
            text = string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
            var result = new BuildCommandResult();
            Regex regex = new Regex(pattern);
            Match match = regex.Match(text);

            if (!match.Success)
            {
                result.Error = "Invalid command format";
                return result;
            }

            var methodName = match.Groups[1].Value;
            var parameters = match.Groups[2].Value.Split(',');

            switch(methodName)
            {
                case "Search":
                    return BuildSearchCommand(parameters);
                case "Availability":
                    return BuildAvailabilityCommand(parameters);
                default:
                    result.Error = "Unkown command";
                    return result;
            }
        }

        private static BuildCommandResult BuildSearchCommand(string[] parameters)
        {
            var result = new BuildCommandResult();
            var expectedAmountOfParameters = 3;

            if (parameters.Count() != expectedAmountOfParameters)
            {
                result.Error = "Invalid amount of parameters";
                return result;
            }

            var hotelId = parameters[0];

            if (!int.TryParse(parameters[1], out var daysInterval))
            {
                result.Error = $"Invalid daysInterval search parameter '{parameters[1]}'";
                return result;
            }

            var roomCode = parameters[2];

            result.Value = new SearchCommand()
            {
                DaysInterval = daysInterval,
                RoomCode = roomCode,
                HotelId = hotelId
            };

            return result;
        }

        private static BuildCommandResult BuildAvailabilityCommand(string[] parameters)
        {
            var result = new BuildCommandResult();
            var expectedAmountOfParameters = 3;

            if (parameters.Count() != expectedAmountOfParameters)
            {
                result.Error = "Invalid amount of parameters";
                return result;
            }

            var hotelId = parameters[0];
            var roomCode = parameters[2];
            var dateRange = ExtractDates(parameters[1]);

            if(dateRange == null)
            {
                result.Error = $"Invalid dateRange parameter ${parameters[1]}";
                return result;
            }

            result.Value = new AvailabilityCommand()
            {
                DateRange = dateRange,
                HotelId = hotelId,
                RoomCode = roomCode
            };

            return result;
        }

        static DateOnlyRange? ExtractDates(string input)
        {
            string dayRangePattern = @"^(\d{8})-(\d{8})$";
            string singleDayPattern = @"^(\d{8})$";

            Match match = Regex.Match(input, dayRangePattern);
            if (match.Success)
            {
                return new DateOnlyRange
                {
                    StartDate = DateFormatter.ConvertToDateOnly(match.Groups[1].Value),
                    EndDate = DateFormatter.ConvertToDateOnly(match.Groups[2].Value),
                };
            }

            match = Regex.Match(input, singleDayPattern);
            if (match.Success)
            {
                return new DateOnlyRange
                {
                    StartDate = DateFormatter.ConvertToDateOnly(match.Groups[1].Value),
                    EndDate = DateFormatter.ConvertToDateOnly(match.Groups[1].Value),
                };
            }

            return null;
        }
    }
}
