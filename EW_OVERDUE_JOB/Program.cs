using System;
using System.Threading.Tasks;
using PU_EmiReminder_OverDue.Common;
using PU_EmiReminder_OverDue.EmiReminder;

namespace PU_EmiReminder_OverDue
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                EmiReminderOverDue emiReminder = new EmiReminderOverDue();
                await emiReminder.OverDue_EmiReminder();
            }
            catch (Exception ex)
            {
                WriteLogFile.LogToFile(ex.InnerException.ToString());
                Console.WriteLine(ex.Message);
            }
        }
    }
}
