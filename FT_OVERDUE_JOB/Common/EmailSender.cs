using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace PU_EmiReminder_OverDue.Common
{
    public static class EmailSender
    {
        public static string SendEMail(string from, string to, string cc, string bcc, string emailPassword, string subject, string _body, IConfiguration iConfig, string attachPath = "")
        {
            string functionReturnValue = null;
            string msg = "";

            try
            {
                var smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                var smtpPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);


                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress(from);

                if (!string.IsNullOrWhiteSpace(to))
                    mail.To.Add(to);

                if (!string.IsNullOrWhiteSpace(cc))
                {
                    if (cc.Contains(","))
                    {
                        string[] ccArr = cc.Split(',');
                        foreach (string s in ccArr)
                        {
                            mail.CC.Add(s);
                        }
                    }
                    else
                    {
                        mail.CC.Add(cc);
                    }
                }

                if (!string.IsNullOrWhiteSpace(bcc) && bcc.ToLower() != "none")
                {
                    if (bcc.Contains(","))
                    {
                        string[] bccArr = bcc.Split(',');
                        foreach (string s in bccArr)
                        {
                            mail.Bcc.Add(s);
                        }
                    }
                    else
                    {
                        mail.Bcc.Add(bcc);
                    }
                }

                if (!string.IsNullOrEmpty(attachPath))
                    mail.Attachments.Add(new System.Net.Mail.Attachment(attachPath));

                mail.Subject = subject;
                mail.Body = _body;
                mail.IsBodyHtml = true;

                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(smtpHost, smtpPort);
                smtpClient.Credentials = new System.Net.NetworkCredential(from, emailPassword);
                smtpClient.EnableSsl = true;

                // Optionally handle SSL certificate validation
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(ValidateServerCertificate);

                smtpClient.Send(mail);

                functionReturnValue = "Sent";
                msg = $"Email sent successfully to {to}";
                Console.WriteLine(msg);
                WriteLogFile.LogToFile(msg);
                mail.Dispose();
            }
            catch (FormatException ex)
            {
                functionReturnValue = "FormatException: " + ex.Message + "Email- " + to + "";
                WriteLogFile.LogToFile(functionReturnValue);
                Console.WriteLine(functionReturnValue);
            }
            catch (SmtpException ex)
            {
                functionReturnValue = "SmtpException: " + ex.Message + "Email- " + to + "";
                WriteLogFile.LogToFile(functionReturnValue);
                Console.WriteLine(functionReturnValue);
            }
            catch (Exception ex)
            {
                functionReturnValue = "Exception: " + ex.Message + "Email- " + to + "";
                WriteLogFile.LogToFile(functionReturnValue);
                Console.WriteLine(functionReturnValue);
            }

            return functionReturnValue;
        }


        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else
            {

                return true;

            }
        }
    }
}
