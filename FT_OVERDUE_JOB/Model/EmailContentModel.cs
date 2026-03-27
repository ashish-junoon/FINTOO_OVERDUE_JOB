using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PU_EmiReminder_OverDue.Model
{
    public class EmailContentModel
    {
        public string customerName { get; set; }
        public double amount { get; set; }
        public string loanAccountNumber { get; set; }
        public string dueDate { get; set; }
        public string paymentLink { get; set; }
        public string portal_link { get; set; }
        public string custphoneNumber { get; set; }
        public string custemailAddress { get; set; }
        public string MobileNo { get; set; }
        public string MobileNoFor_SMS { get; set; }
        public string EMail_ID { get; set; }
        public string account_number { get; set; }
        public string lead_id { get; set; }
        public string company_id { get; set; }
        public string product_name { get; set; }
        public string updated_by { get; set; }
    }

}
