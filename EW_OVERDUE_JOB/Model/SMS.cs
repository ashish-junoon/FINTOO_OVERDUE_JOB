using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PU_EmiReminder_OverDue.Model
{
    public class SMS
    {
        public string Authkey {  get; set; }
        public string EndPointSMS {  get; set; }
        public string template_id {  get; set; }
    }
}
