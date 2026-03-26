using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using PU_EmiReminder_OverDue.Common;
using PU_EmiReminder_OverDue.Model;

namespace PU_EmiReminder_OverDue.EmiReminder
{
    public class EmiReminderOverDue
    {
        bool IsDev = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDev"]);
        string ReminderDay = ConfigurationManager.AppSettings["ReminderDay"];
        string emailTo = ConfigurationManager.AppSettings["EmailTo"];
        string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
        string emailPassword = ConfigurationManager.AppSettings["EmailPassword"];
        string emailCc = ConfigurationManager.AppSettings["Emailcc"];
        string emailBcc = ConfigurationManager.AppSettings["EmailBcc"];
        string connectionString = ConfigurationManager.ConnectionStrings["CrediCash_Dev"].ConnectionString;
        string sqlquery = "";
        public async Task OverDue_EmiReminder()
        {
            EmailContentModel emailContentModel = new EmailContentModel();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("USP_EmiReminder", conn))
                {
                    DataTable dt = new DataTable();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReminderDay", ReminderDay);
                    cmd.Parameters.AddWithValue("@Action", "OverDue_EmiReminder");
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0 && dt != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {

                            emailContentModel.customerName = row["full_name"].ToString();
                            double _total_remaining_interest_due = row["total_remaining_interest_due"] != DBNull.Value ? Convert.ToDouble(row["total_remaining_interest_due"]) : 0.0;
                            double principalamount = row["principal_amount"] != DBNull.Value ? Convert.ToDouble(row["principal_amount"]) : 0.0;
                            double till_date_interest_dpd = row["total_interest_day"] != DBNull.Value ? Convert.ToDouble(row["total_interest_day"]) : 0.0;
                            double penal_charge = row["penal_charge"] != DBNull.Value ? Convert.ToDouble(row["penal_charge"]) : 0.05;
                            int penaldaycount = int.TryParse(row["penalty_days"]?.ToString(), out int tempPenal) ? tempPenal : 0;
                            double intrestperday = double.TryParse(row["interest_rate"]?.ToString(), out double tempInterest) ? tempInterest : 1;
                            double till_day_interest_amount = (principalamount * intrestperday / 100) * till_date_interest_dpd;
                            double penal_amount = (principalamount * penal_charge / 100) * penaldaycount;
                            emailContentModel.amount = (principalamount) + (till_day_interest_amount) + (penal_amount) + (_total_remaining_interest_due);
                            emailContentModel.loanAccountNumber = row["loan_id"].ToString();
                            emailContentModel.dueDate = row["emi_due_date"].ToString();
                            emailContentModel.paymentLink = row["paymentLink"].ToString();
                            emailContentModel.portal_link = row["portal_link"].ToString();
                            emailContentModel.custphoneNumber = row["mobile_number"].ToString();
                            emailContentModel.custemailAddress = row["email_id"].ToString();
                            byte[] data = Convert.FromBase64String(row["account_number"].ToString());
                            string account_number = System.Text.Encoding.UTF8.GetString(data);
                            emailContentModel.account_number = account_number.Substring(account_number.Length - 4);
                            emailContentModel.MobileNo = ConfigurationManager.AppSettings["MobileNo"];
                            emailContentModel.EMail_ID = ConfigurationManager.AppSettings["EMail_ID"];
                            emailContentModel.lead_id = row["lead_id"].ToString();
                            emailContentModel.company_id = row["company_id"].ToString();
                            emailContentModel.product_name = row["product_name"].ToString();
                            string subject = ConfigurationManager.AppSettings["Subject"];
                            emailTo = (IsDev) ? emailTo : emailContentModel.custemailAddress;
                            emailContentModel.MobileNoFor_SMS = (IsDev) ? ConfigurationManager.AppSettings["MobileNoFor_SMS"] : row["mobile_number"].ToString();
                            emailContentModel.updated_by = System.Environment.MachineName;
                            string body = EmailContentBuilder.GetBody(emailContentModel);

                            string mailstatus = EmailSender.SendEMail(emailFrom, emailTo, emailCc, emailBcc, emailPassword, subject, body, null);
                            mailstatus = (mailstatus == "Sent") ? "1" : "0";

                            sqlquery += $" exec USP_applicant_maintain_lead_history @lead_id='{emailContentModel.lead_id}',@status=17,@reason='{subject}',@To='{emailTo}',@updated_by='{emailContentModel.updated_by}',@company_id='{emailContentModel.company_id}',@product_name='{emailContentModel.product_name}',@mail_or_sms_flg='{mailstatus}'";

                            Task.Delay(2000).Wait();

                            //await Msg91SmsClass.SendEmiReminderNotificationAsync(emailContentModel);
                            //Task.Delay(2000).Wait();
                        }
                        string result = SqlHelper.MultipleTransactions(sqlquery, connectionString);
                    }
                }
            }
            catch (Exception ex)
            {

                WriteLogFile.LogToFile(ex.Message);
            }

           
        }

    }
}
