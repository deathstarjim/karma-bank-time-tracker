using System;
using System.IO;
using System.Web;

namespace TimeTracker.UI.Tools
{
    public class Helpers
    {
        private static string _logFolder = "\\logs\\";
        private static string _currentDate = DateTime.Now.Date.ToString("MM_dd_yyyy");
        private static string _basePath = HttpContext.Current.Server.MapPath("~/bin") + _logFolder + _currentDate + "\\";
        private static string _logFileName = "BikeTimeTracker_ErrorLog_" + _currentDate + ".txt";

        public static void CreateLogFolder()
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public static void CreateLogFile()
        {
            if (!File.Exists(_basePath + _logFileName))
                File.Create(_basePath + _logFileName).Close();
        }

        public static void WriteLine(string message)
        {
            var sw = new StreamWriter(_basePath + _logFileName, true);
            sw.WriteLine(message);
            sw.Close();
        }
    }
}