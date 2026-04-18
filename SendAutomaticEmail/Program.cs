using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Configuration;
using DLL.Models;
using System.Data.Entity.Core.Objects;
using BLL.ViewModels;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace SendAutomaticEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            SendMail();

        }
        public static void SendMail()
        {

            //string password;
            //string message = txtMessage.Text;
            string empcode = "", empEmail;
            using (var db = new Context())
            {
                // get all employees with no groups
                Employee[] emp = db.employee.Where(m => m.active&&m.access_group.AccessGroupId.Equals(3)).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[emp.Length];
                Console.WriteLine("******Email Sending Start******");
                for (int i = 0; i < emp.Length; i++)
                {
                    empcode = emp[i].employee_code;
                    empEmail = emp[i].email;


                    string resp = "";
                    string grid;
                    string connetionString = null;
                    SqlConnection connection;
                    SqlCommand command;
                    string sql = null;
                    SqlDataReader dataReader = null;
                    //connetionString =System.Configuration.ConfigurationManager.ConnectionStrings["TimeTune"].ConnectionString;
                    connetionString = "Data Source=192.168.1.144\\sqlexpress2012;Initial Catalog=TimeTune_RESCO;User ID=sa;Password=resco@1234;";
                    sql = "[GetGetzAttendanceANDSendEmail_FORPREVEIOUSMONTH]";
                    connection = new SqlConnection(connetionString);
                    try
                    {
                        int a = 0; string previous = "", current = "", color = "white";
                        command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = sql;
                        command.Parameters.Add("@EmployeeCode", System.Data.SqlDbType.NVarChar, 10).Value = empcode;


                        grid = "<div style='font-family: Arial;'><table cellpadding='1' cellspacing='0' width='100%' border='1'><tr style='background-color: lightgray'><td>Date</td><td>Employee Code</td><td>Name</td><td>Time In</td><td>Time Out</td><td>Status In</td><td>Status Out</td><td>Final Remarks</td></tr>";
                        connection.Open();
                        // command.ExecuteNonQuery();
                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            current = dataReader.GetValue(1).ToString();
                            if (a == 0)
                            {
                                previous = current;

                            }

                            //if (previous == current)
                            //{
                            //    color = "white";

                            //}
                            //else
                            //{
                            //    color = "lightgray";
                            //    previous = current;
                            //}s

                            //Date	employee_code	Name	time_in	time_out	status_in	status_out	final_remarks	terminal_in	terminal_out
                            grid += "<tr style='background-color:" + color + "'><td>" + dataReader.GetValue(0) + "</td><td>" + dataReader.GetValue(1) + "</td><td>" + dataReader.GetValue(2) + "</td><td>" + dataReader.GetValue(3) + "</td><td>" + dataReader.GetValue(4) + "</td><td>" + dataReader.GetValue(5) + "</td><td>" + dataReader.GetValue(6) + "</td><td>" + dataReader.GetValue(7) + "</td></tr>";

                            //if (dataReader.GetValue(0).ToString() == DateTime.Now.AddDays(-1).ToString("dd-MMM-yyyy").ToUpper())
                            //{
                            //    grid += "<tr><td colspan='8' style='background-color:lightgray;'>--------------------------------------------------------------- X X X X X X X X X X X X --------------------------------------------------------------- </td></tr>";
                            //}

                            a++;
                        }

                        grid += "</table></div>";

                        dataReader.Close();
                        command.Dispose();
                        connection.Close();

                        //lblGrid.Text = grid;
                        //if (Request.QueryString["test"] != null && Request.QueryString["test"].ToString() == "ir")
                        //{
                        //    resp = SendEmailWithData("inayat68@gmail.com", "na", "na", grid);
                        //}
                        //else
                        //{
                        //    resp = SendEmailWithData("immad@reliable.com.pk", "hammad@reliable.com.pk", "inayat@reliable.com.pk", grid);
                        //}
                        if (empEmail != "-")
                        {
                            resp = SendEmailWithData(empEmail, "na", "na", grid);
                        }else
                        {
                            resp = SendEmailWithData("s.mudassir7777@gmail.com", "na", "na", grid);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Can not open connection ! ");
                    }




                    if (resp == "success")
                    {
                        Console.WriteLine("This e-mail is sent successfully");
                    }
                    else
                    {
                        Console.WriteLine("An error occurred");
                    }
                    //The fields that are required are checked for the length. If either one of them is empty, the message will not be submitted, but an error message is displayed instead.Note that you could use ASP.NET Validation controls for this, but for the sake of simplicity, I left them out here.

                    //MailMessage objMail = new MailMessage();

                    //objMail.From = "Your From Name <You@YourProvider.com>";
                    //objMail.To = "Your To Name <You@YourProvider.com>";
                    //objMail.Subject = "Response from website";
                    //objMail.Body = message;
                    //SmtpMail.SmtpServer = "smtp.gmail.com";
                    //SmtpMail. = "smtp.gmail.com";
                    //SmtpMail.Send(objMail);
                    //Response.Redirect("ThankYou.aspx");
                    //Once you know that you have some valid values, you can create a new MailMessage object.You'll need to set at least a To and a From, a Subject and the Body to make it a useful message. The SmtpServer is the server that actually sends the e-mail message. You can use the name of the SMTP server of your provider here. Alternatively, if you have an SMTP server at your local machine, you can add localhost here.

                    //Console.ReadLine();
                }
            }
        }
        public static string SendEmailWithData(string to, string cc, string bcc, string grid)
        {
            string strResponse = string.Empty;
            string modelFromName = string.Empty, modelFromEmail = string.Empty, modelMessage = string.Empty;
            string smtp_host = string.Empty, smtp_port = string.Empty, smtp_email = string.Empty, smtp_password = string.Empty;
            string smtp_test_email = string.Empty, smtp_cc_email = string.Empty, smtp_bcc_email = string.Empty, smtp_enable_ssl = string.Empty;

            try
            {
                smtp_host = "smtp.gmail.com";
                smtp_port = "587";
                smtp_email = "bams.no.reply@gmail.com";
                smtp_password = "Bams@1234";
                smtp_test_email = "mudihussain77@gmail.com";
                smtp_cc_email = cc;  //"hammad@reliable.com.pk";
                smtp_bcc_email = bcc;  //"inayat@reliable.com.pk";
                smtp_enable_ssl = "1";

                //modelFromName = "BAMS Support";
                //modelFromEmail = smtp_email;

                //if (Request.Url.ToString().Split('/').Count() > 0)
                //{
                //    url = Request.Url.ToString().Split('/')[2];
                //}



                modelMessage = "To: Admin";
                modelMessage += "<br><br>";
                modelMessage += "Monthly TimeSheet of Previous Month:"; // for the Month of \"" + DateTime.Now.ToString("MMMM yyyy") + "\"";
                modelMessage += "<br><br>";
                modelMessage += grid;
                modelMessage += "<br><br>";
                //modelMessage += "Thank you for using BAMS.";
                //modelMessage += "<br><br>";
                //modelMessage += "Regards,<br>--<br>";
                //modelMessage += "BAMS Support<br>";
                //modelMessage += "Dated: " + DateTime.Now.ToShortDateString();

                //var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var body = "<p>{0}</p>";
                var message = new MailMessage();

                if (to != null && to.ToString() != "")
                    message.To.Add(new MailAddress(to.ToLower()));
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
                message.Subject = "RESCO BAMS: Attendance Email on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
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
                        smtp.Send(message);

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
    }
}
