using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ADMIN
{
    public static class ForgotPassword
    {
        public static bool verifyAccessAllowed()
        {
            bool isAllowed = false;
            using (var db = new DLL.Models.Context())
            {
                try
                {
                    string val = db.access_code_value.Where(e => e.AccessCode.ToUpper() == "FORGOT_PASSWORD_LINK").FirstOrDefault().AccessValue;
                    if (val != null && val == "1")
                    {
                        isAllowed = true;
                    }
                }
                catch (Exception)
                {
                    isAllowed = false;
                }
            }

            return isAllowed;
        }

        public static bool validateInternetConnetionChecking()
        {
            bool isAllowed = false;
            using (var db = new DLL.Models.Context())
            {
                try
                {
                    string val = db.access_code_value.Where(e => e.AccessCode.ToUpper() == "VALIDATE_INTERNET_CONNECTION").FirstOrDefault().AccessValue;
                    if (val != null && val == "1")
                    {
                        isAllowed = true;
                    }
                }
                catch (Exception)
                {
                    isAllowed = false;
                }
            }

            return isAllowed;
        }

        public static int getEmailDelayMinutes()
        {
            int minutes = 1;
            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var val = db.access_code_value.Where(e => e.AccessCode.ToUpper() == "SMTP_EMAIL_DELAY").FirstOrDefault();
                    if (val != null)
                    {
                        minutes = int.Parse(val.AccessValue);
                    }
                }
                catch (Exception)
                {
                    minutes = 1;
                }
            }

            return minutes;
        }

        public static string verifyEmployeeCodeANDEmail(string employee_code)//, string employee_email)
        {
            //bool empFound = false;
            string strEmployeeEmail = string.Empty;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    //var emp = db.employee.Where(e => e.employee_code.ToLower() == employee_code.ToLower() && e.email.ToLower() == employee_email.ToLower() && e.active == true && e.timetune_active == true).FirstOrDefault();
                    var emp = db.employee.Where(e => e.employee_code.ToLower() == employee_code.ToLower() && (e.email != null && e.email != "") && e.active == true && e.timetune_active == true).FirstOrDefault();
                    if (emp != null)
                    {
                        //empFound = true;

                        if (emp.email != null && emp.email.ToString() != "")
                            strEmployeeEmail = emp.email.ToLower();
                    }
                }
                catch (Exception)
                {
                    //empFound = false;
                    strEmployeeEmail = "";
                }
            }

            return strEmployeeEmail;
        }

        public static bool validiateAlreadyExistingPasswordRequest(string employee_code)
        {
            bool empFound = false;
            using (var db = new DLL.Models.Context())
            {
                var emp = db.employee_forgotpassword.Where(c => c.EmployeeCode.ToLower() == employee_code.ToLower() && c.ExpiryDate > DateTime.Now).OrderByDescending(f => f.CreateDate).FirstOrDefault();
                if (emp != null)
                {
                    empFound = true;
                }
            }

            return empFound;
        }

        public static int validiateGuidCode(string guid_code, out string out_emp_code)
        {
            int response_code = 0;
            out_emp_code = "";

            using (var db = new DLL.Models.Context())
            {
                var emp = db.employee_forgotpassword.Where(c => c.GuidCode == guid_code).FirstOrDefault();
                if (emp != null)
                {
                    var emp1 = db.employee_forgotpassword.Where(c => c.GuidCode == guid_code && c.ExpiryDate < DateTime.Now).FirstOrDefault();
                    if (emp1 != null)
                    {
                        response_code = -1;//token already expired
                    }
                    else
                    {
                        var emp2 = db.employee_forgotpassword.Where(c => c.GuidCode == guid_code && c.ExpiryDate > DateTime.Now && c.UpdateDate == null).FirstOrDefault();
                        if (emp2 != null)
                        {
                            response_code = 1;//token is valid
                            out_emp_code = emp2.EmployeeCode;
                        }
                        else
                        {
                            response_code = -2;//token already utilized
                        }
                    }
                }
            }

            return response_code;
        }

        public static int validiateGuidCodeWithPasswordUpdateInDatabase(string guid_code, string new_password)
        {
            int response_code = 0;

            using (var db = new DLL.Models.Context())
            {
                var emp = db.employee_forgotpassword.Where(c => c.GuidCode == guid_code).FirstOrDefault();
                if (emp != null)
                {
                    var emp1 = db.employee_forgotpassword.Where(c => c.GuidCode == guid_code && c.ExpiryDate < DateTime.Now).FirstOrDefault();
                    if (emp1 != null)
                    {
                        response_code = -1;//token already expired
                    }
                    else
                    {
                        var emp2 = db.employee_forgotpassword.Where(c => c.GuidCode == guid_code && c.ExpiryDate > DateTime.Now && c.UpdateDate == null).FirstOrDefault();
                        if (emp2 != null)
                        {
                            //password reset code here
                            var emp3 = db.employee.Where(c => c.employee_code == emp.EmployeeCode).FirstOrDefault();
                            if (emp3 != null)
                            {
                                DLL.Commons.Passwords.setPassword(emp3, new_password);
                            }

                            db.SaveChanges();

                            response_code = 1;//token is valid and password has been reset
                        }
                        else
                        {
                            response_code = -2;//token already utilized
                        }
                    }
                }
            }

            return response_code;
        }

        public static string GetGuidCode()
        {
            return Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
        }

        public static void AddNEWForgotPasswordRequest(string emp_code, string guid_code)
        {
            int delay_minutes = 1;
            delay_minutes = getEmailDelayMinutes();

            using (var db = new DLL.Models.Context())
            {
                EmployeeForgotPassword f = new EmployeeForgotPassword();
                f.EmployeeCode = emp_code;
                f.GuidCode = guid_code;
                f.ExpiryDate = Convert.ToDateTime(DateTime.Now.AddMinutes(delay_minutes).ToString("yyyy-MM-dd HH:mm:ss tt"));
                f.CreateDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));
                f.UpdateDate = null;

                db.employee_forgotpassword.Add(f);

                db.SaveChanges();
            }
        }

        public static bool UpdateDateByGuid(string guid_code)
        {
            bool response = false;

            using (var db = new DLL.Models.Context())
            {
                //EmployeeForgotPassword f = new EmployeeForgotPassword(); 
                var fEmployee = db.employee_forgotpassword.Where(e => e.GuidCode == guid_code).FirstOrDefault();

                if (fEmployee != null)
                {
                    fEmployee.UpdateDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt")); ;
                    db.SaveChanges();

                    response = true;
                }
            }

            return response;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool validiateEEmailandSendPassword(BLL.ViewModels.ForgotPassword info)
        {
            using (var db = new DLL.Models.Context())
            {
                var emp = db.employee.Where(c => c.employee_code == info.EmployeeCode).FirstOrDefault();
                if (emp.email == info.EmployeeEmail)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
