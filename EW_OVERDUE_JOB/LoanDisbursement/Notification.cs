using System.Collections.Generic;

namespace LoanDisbursement
{
    public class Notification
    {
        public string template_id { get; set; }
        public string short_url { get; set; }
        public List<Recipient> recipients { get; set; }
    }
}