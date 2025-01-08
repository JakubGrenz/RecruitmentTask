namespace Guestline.Models
{
    public class ErrorCommandResult: IPrintableCommandResult
    {
        public string Error { get; set; }

        public string ToConsoleOutput()
        {
            return Error;
        }
    }
}
