using Guestline.Models.Commands;

namespace Guestline.Models
{
    public class BuildCommandResult
    {
        public bool Success
        {
            get
            {
                return Error == null && Value != null;
            }
        }

        public string? Error { get; set; }
        public Command? Value { get; set; }
    }
}
