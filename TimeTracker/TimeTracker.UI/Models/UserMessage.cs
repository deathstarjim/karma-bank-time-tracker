
namespace TimeTracker.UI.Models
{
    public class UserMessage
    {
        public string Message { get; set; }

        public string Heading { get; set; }

        public string Icon { get; set; }

        public int HideAfter { get; set; }

    }

    public class MessageConstants
    {
        public static readonly string Error = "error";
        public static readonly string Warning = "warning";
        public static readonly string Success = "success";
        public static readonly string Information = "info";
    }
}