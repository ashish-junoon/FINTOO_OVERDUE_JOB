using System.Text;
using PU_EmiReminder_OverDue.Model;

namespace PU_EmiReminder_OverDue.Common
{
    public static class EmailContentBuilder
    {
        public static string GetBody(EmailContentModel model)
        {
            StringBuilder sBody = new StringBuilder();
            sBody.Append("<!DOCTYPE html><html><body>");
            sBody.Append($"<p>Dear <b>{model.customerName},</b></p>");
            sBody.Append($"<p>This is a kind reminder that your repayment for Credit facility A/C No. <b>{model.loanAccountNumber}</b> was due on <b>{model.dueDate}</b> and remains unpaid as of today.</p>");
            sBody.Append($"<p>We request you to kindly make the payment of <b>{model.amount}</b> at the earliest to avoid late charges and maintain a good credit history.</p>");
            sBody.Append($"<p>To pay now, <a href=\"{model.paymentLink}\" target=\"_blank\">click here</a>.</p>");
            sBody.Append($"<p>If you have already made the payment, please ignore this communication. For any queries, feel free to contact us at <b>{model.MobileNo}</b>.</p>");
            sBody.Append("<p>Thank you for your prompt attention to this matter.</p>");
            sBody.Append("<p>Warm regards,</p>");
            sBody.Append($"<p><b>Team {model.product_name}!</b></p>");
            sBody.Append("<p><i><em><b>Disclaimer:</b> This is a system-generated email. No reply is required.</em></i></p>");
            sBody.Append("</body></html>");
            return sBody.ToString();
        }        
    }
}
