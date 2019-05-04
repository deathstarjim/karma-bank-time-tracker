namespace TimeTracker.UI.Models
{
    public class Error
    {
        public string Message { get; set; }

        public string InnerException { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

    }
}