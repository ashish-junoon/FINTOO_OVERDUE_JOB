using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PU_EmiReminder_OverDue.Model;

namespace PU_EmiReminder_OverDue.Common
{
    public static class Msg91SmsClass
    {
        private static readonly HttpClient client = new HttpClient();

        private static string template_id = System.Configuration.ConfigurationManager.AppSettings["EMIReminderConsentTemplate"];
        private static string EndPointSMS = System.Configuration.ConfigurationManager.AppSettings["EndPointSMS"];
        private static string Authkey = System.Configuration.ConfigurationManager.AppSettings["Authkey"];
        private static string subject = System.Configuration.ConfigurationManager.AppSettings["Subject"];
        private static string contactOverDueSMS = System.Configuration.ConfigurationManager.AppSettings["contactOverDueSMS"];
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CrediCash_Dev"].ConnectionString;
        public static async Task<Msg91RS> sendSMS_V1(object requestObj, string MobileNo)
        {
            Msg91RS msg91 = new Msg91RS();
            string response = string.Empty;
            string msg = "";
            var bodyJson = JsonConvert.SerializeObject(requestObj);
            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("authkey", Authkey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                // client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("axios/0.27.2");
                var stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
                stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $"{APIkey}");
                HttpResponseMessage result = await client.PostAsync(EndPointSMS, stringContent);
                if (result.IsSuccessStatusCode && result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response = await result.Content.ReadAsStringAsync();
                    msg91 = JsonConvert.DeserializeObject<Msg91RS>(response);
                    msg = $"SMS sent successfully to {MobileNo}";
                    WriteLogFile.LogToFile(msg);
                }
                else if (result.StatusCode.ToString() == "204")
                {
                    msg = $"SMS not sent to {MobileNo}";
                    WriteLogFile.LogToFile(msg);
                    return new Msg91RS()
                    {
                        message = "Unsuccessfull",
                        type = "fail"
                    };
                }
                else
                {
                    response = await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message + "Mobile No " + MobileNo;
                WriteLogFile.LogToFile(msg);
                return new Msg91RS()
                {
                    message = ex.Message,
                    type = "fail"
                };
            }

            return msg91;
        }


        public static async Task SendEmiReminderNotificationAsync(EmailContentModel model)
        {
            await Task.Run(async () =>
            {
                if (model != null)
                {
                    var notification = new LoanDisbursement.Notification
                    {
                        template_id = template_id,
                        short_url = "0",
                        recipients = new List<LoanDisbursement.Recipient>
                        {
                            new LoanDisbursement.Recipient
                            {
                                CONTACT = contactOverDueSMS,
                                AMOUNT = model.amount,
                                DATE = model.dueDate,
                                mobiles=$"91{model.MobileNoFor_SMS}"
                            }
                        }
                    };

                    Msg91RS msg91RS = await Msg91SmsClass.sendSMS_V1(notification, model.MobileNoFor_SMS);
                    int sms_status = (msg91RS.type == "success") ? 1 : 0;
                    string sqlquery = $" exec USP_applicant_maintain_lead_history @lead_id='{model.lead_id}',@status=16,@reason='{subject}',@To='{model.MobileNoFor_SMS}',@updated_by='{model.updated_by}',@company_id='{model.company_id}',@product_name='{model.product_name}',@mail_or_sms_flg='{sms_status}'";

                    string result = SqlHelper.MultipleTransactions(sqlquery, connectionString);
                }
            });
        }


    }
}
