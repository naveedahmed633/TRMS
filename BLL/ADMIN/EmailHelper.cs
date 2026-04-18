using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ADMIN
{
    public static class EmailHelper
    {
        public static async Task<string> SendEmailWithPasswordResetLink(string url, string eCode, string eEmail, string eGCode)
        {
            string strResponse = string.Empty;
            string modelFromName = string.Empty, modelFromEmail = string.Empty, modelMessage = string.Empty;
            string smtp_host = string.Empty, smtp_port = string.Empty, smtp_email = string.Empty, smtp_password = string.Empty;
            string smtp_test_email = string.Empty, smtp_cc_email = string.Empty, smtp_bcc_email = string.Empty, smtp_enable_ssl = string.Empty;

            try
            {
                smtp_host = GetSMTPInfoByCode("SMTP_HOST");
                smtp_port = GetSMTPInfoByCode("SMTP_PORT");
                smtp_email = GetSMTPInfoByCode("SMTP_EMAIL");
                smtp_password = GetSMTPInfoByCode("SMTP_PASSWORD");
                smtp_test_email = GetSMTPInfoByCode("SMTP_TEST_EMAIL");
                smtp_cc_email = GetSMTPInfoByCode("SMTP_CC_EMAIL");
                smtp_bcc_email = GetSMTPInfoByCode("SMTP_BCC_EMAIL");
                smtp_enable_ssl = GetSMTPInfoByCode("SMTP_ENABLE_SSL");

                //modelFromName = "BAMS Support";
                //modelFromEmail = smtp_email;

                //if (Request.Url.ToString().Split('/').Count() > 0)
                //{
                //    url = Request.Url.ToString().Split('/')[2];
                //}

                modelMessage = "To: " + GetEmployeeNameByEmployeeCode(eCode);
                modelMessage += "<br>";
                modelMessage += "Sent: " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                modelMessage += "<br>";
                modelMessage += "User Name: " + eCode;
                modelMessage += "<br><br>";
                modelMessage += "<a href='http://" + url + "/Account/ForgotPasswordReset?gcode=" + eGCode + "' target='_blank'>RESET your Password</a> and follow the on-screen instructions. This email can be ignored in case you didn't request a password reset, the link is only available for a short time.";
                modelMessage += "<br><br>";
                //modelMessage += "Thank you for using BAMS.";
                //modelMessage += "<br><br>";
                //modelMessage += "Regards,<br>--<br>";
                //modelMessage += "BAMS Support<br>";
                //modelMessage += "Dated: " + DateTime.Now.ToShortDateString();

                //var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var body = "<p>{0}</p>";
                var message = new MailMessage();

                if (eEmail != null && eEmail.ToString() != "")
                    message.To.Add(new MailAddress(eEmail.ToLower()));
                else
                    message.To.Add(new MailAddress("bams.no.reply@gmail.com"));

                if (smtp_cc_email.ToLower() != "na")
                    message.CC.Add(new MailAddress(smtp_cc_email));

                if (smtp_bcc_email.ToLower() != "na")
                    message.Bcc.Add(new MailAddress(smtp_bcc_email));

                if (smtp_test_email.ToLower() != "na")
                    message.Bcc.Add(new MailAddress(smtp_test_email));
                //message.Bcc.Add(new MailAddress("hbl-no-reply@outlook.com"));

                message.From = new MailAddress(smtp_email);  //sender@outlook.com // replace with valid value
                message.Subject = "BAMS: Password Reset";
                //message.Subject = "BAMS Portal - Forgot Password request of Employee Code <" + eCode + "> on " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt");
                //message.Body = string.Format(body, modelFromName, modelFromEmail, modelMessage);
                message.Body = string.Format(body, modelMessage);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = smtp_email, //"inayaturrehman@live.com"  // replace with valid value
                        Password = smtp_password //"password"  // replace with valid value
                    };

                    smtp.Credentials = credential;
                    smtp.Host = smtp_host; // "smtp-mail.outlook.com";//smtp.gmail.com
                    smtp.Port = int.Parse(smtp_port);

                    if (smtp_enable_ssl == "" || smtp_enable_ssl == "0")
                    {
                        smtp.EnableSsl = false;
                    }
                    else
                    {
                        smtp.EnableSsl = true;
                    }

                    if (smtp_email.ToLower() != "na")
                        await smtp.SendMailAsync(message);
                    else
                        await Task.Delay(1000);

                    //await Task.Delay(1000);
                    strResponse = "success";
                }
            }
            catch (Exception ex)
            {
                strResponse = "Exception: Message = " + ex.Message + "<br>Source = " + ex.Source + "<br>StackTrace = " + ex.StackTrace;
            }

            return strResponse;
        }

        public static async Task<string> SendTestEmailForVerification()
        {
            string strResponse = string.Empty;
            string modelFromName = string.Empty, modelFromEmail = string.Empty, modelMessage = string.Empty;
            string smtp_host = string.Empty, smtp_port = string.Empty, smtp_email = string.Empty, smtp_password = string.Empty;
            string smtp_test_email = string.Empty, smtp_cc_email = string.Empty, smtp_bcc_email = string.Empty, smtp_enable_ssl = string.Empty;

            try
            {
                smtp_host = GetSMTPInfoByCode("SMTP_HOST");
                smtp_port = GetSMTPInfoByCode("SMTP_PORT");
                smtp_email = GetSMTPInfoByCode("SMTP_EMAIL");
                smtp_password = GetSMTPInfoByCode("SMTP_PASSWORD");
                smtp_test_email = GetSMTPInfoByCode("SMTP_TEST_EMAIL");
                smtp_cc_email = GetSMTPInfoByCode("SMTP_CC_EMAIL");
                smtp_bcc_email = GetSMTPInfoByCode("SMTP_BCC_EMAIL");
                smtp_enable_ssl = GetSMTPInfoByCode("SMTP_ENABLE_SSL");

                //modelFromName = "BAMS Support";
                //modelFromEmail = "hbl-no-reply@outlook.com";

                //if (Request.Url.ToString().Split('/').Count() > 0)
                //{
                //    url = Request.Url.ToString().Split('/')[2];
                //}

                modelMessage = "Dear Admin,";
                modelMessage += "<br><br>";
                modelMessage += "As per your request, the email is generated successfully.";
                modelMessage += "<br><br>";
                modelMessage += "Now it confirms that the provided SMTP info are correct as the email is sent out perfectly.";
                modelMessage += "<br><br>";
                modelMessage += "Thanks,<br>--<br>BAMS Support<br>Dated: " + DateTime.Now.ToShortDateString();

                //var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var body = "<p>{0}</p>";
                var message = new MailMessage();

                if (smtp_test_email.ToLower() != "na")
                    message.To.Add(new MailAddress(smtp_test_email));
                else
                    message.To.Add(new MailAddress("bams.no.reply@gmail.com"));

                if (smtp_cc_email.ToLower() != "na")
                    message.CC.Add(new MailAddress(smtp_cc_email));

                if (smtp_bcc_email.ToLower() != "na")
                    message.Bcc.Add(new MailAddress(smtp_bcc_email));

                //message.Bcc.Add(new MailAddress("hbl-no-reply@outlook.com"));

                message.From = new MailAddress(smtp_email);  //sender@outlook.com // replace with valid value
                message.Subject = "BAMS Portal - Test Email by Admin generated on " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt");
                //message.Body = string.Format(body, modelFromName, modelFromEmail, modelMessage);
                message.Body = string.Format(body, modelMessage);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = smtp_email, //"inayaturrehman@live.com"  // replace with valid value
                        Password = smtp_password //"password"  // replace with valid value
                    };

                    smtp.Credentials = credential;//hbl-no-reply@outlook.com/H**@*2*4 and bams.no.reply@gmail.com/B****@*2*4
                    smtp.Host = smtp_host; // "smtp-mail.outlook.com";//smtp.gmail.com
                    smtp.Port = int.Parse(smtp_port);//587

                    if (smtp_enable_ssl == "" || smtp_enable_ssl == "0")
                    {
                        smtp.EnableSsl = false;
                    }
                    else
                    {
                        smtp.EnableSsl = true;
                    }

                    if (smtp_email.ToLower() != "na")
                        await smtp.SendMailAsync(message);
                    else
                        await Task.Delay(1000);

                    strResponse = "success";
                }
            }
            catch (Exception ex)
            {
                strResponse = "Exception: Message = " + ex.Message + "<br>Source = " + ex.Source + "<br>StackTrace = " + ex.StackTrace;
            }

            return strResponse;
        }

        private static string GetSMTPInfoByCode(string code)
        {
            string strAccessValue = string.Empty;

            using (Context db = new Context())
            {
                var access_value = db.access_code_value.Where(m => m.AccessCode.ToUpper() == code).FirstOrDefault();
                if (access_value != null)
                {
                    strAccessValue = access_value.AccessValue;
                }
            }

            return strAccessValue;
        }

        private static string GetEmployeeNameByEmployeeCode(string eCode)
        {
            string strEmployeeName = string.Empty;

            using (Context db = new Context())
            {
                var employee_data = db.employee.Where(m => m.employee_code.ToUpper() == eCode).FirstOrDefault();
                if (employee_data != null)
                {
                    strEmployeeName = employee_data.last_name + ", " + employee_data.first_name;
                }
            }

            return strEmployeeName;
        }

    }
}
