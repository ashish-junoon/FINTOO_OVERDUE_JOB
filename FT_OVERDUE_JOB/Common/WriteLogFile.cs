using System;
using System.Configuration;
using System.IO;

namespace PU_EmiReminder_OverDue.Common
{
    public static class WriteLogFile
    {
        public static void LogToFile(string message)
        {
            string LogPath = ConfigurationManager.AppSettings["LogPath"];
            try
            {
                //string LogPath = @"D:\JC_Project\ConsoleApplication\PU_EmiReminder_OverDue\Logs";
                Directory.CreateDirectory(LogPath);
                DateTime dateTime = DateTime.Now;
                string date = Convert.ToDateTime(dateTime).ToString("dd-MM-yyyy");
                string filename = "email_log_" + date + ".txt";
                string logFilePath = Path.Combine(LogPath, filename);
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging failed: " + ex.Message);
            }
        }
    }
}
