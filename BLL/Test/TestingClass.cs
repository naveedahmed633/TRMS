using DLL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace BLL.Test
{
    public static class TestingClass
    {
        private static bool superUserPasswordFileExists()
        {
            return System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/ShadowFile.rsa"));
        }

        private static void updateSuperUserPassword(string newPassword)
        {
            string[] passwordAndSalt = DLL.Commons.Passwords.generatePasswordAndSalt(newPassword);

            System.IO.File.WriteAllLines(System.Web.HttpContext.Current.Server.MapPath("~/ShadowFile.rsa"), passwordAndSalt);
        }

        public static bool validatePasswordForSuperAdmin(string enteredPassword)
        {
            if (!superUserPasswordFileExists())
            {
                return enteredPassword.Equals("SuperUser@TimeTune1");
            }


            TextReader tr = File.OpenText(System.Web.HttpContext.Current.Server.MapPath("~/ShadowFile.rsa"));
            string password = tr.ReadLine();
            string salt = tr.ReadLine();
            tr.Close();

            return DLL.Commons.Passwords.validateHash(enteredPassword, password, salt);
        }
        public static bool validatePasswordForSudo(string enteredPassword)
        {
            return enteredPassword.Equals("Sudo@TimeTune1");
        }
        public static bool changePasswordForSuperAdmin(string oldPassword, string newPassword)
        {
            if (validatePasswordForSuperAdmin(oldPassword))
            {
                updateSuperUserPassword(newPassword);
                return true;
            }

            return false;
        }
        public static bool loginTest(string empl, string pass)
        {
            string username = empl;
            string attemptedPassword = pass;
            
             if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(attemptedPassword))
                 return false;

             if (username.Equals("SUPER_ADMIN"))
             {
                 return validatePasswordForSuperAdmin(attemptedPassword);
             }
             else if (username.Equals("Sudo"))
             {
                 return validatePasswordForSudo(attemptedPassword);
             }
             else

             { 

             using (var db = new Context())
             {

                 var user = db.employee.Where(m => m.employee_code.Equals(username) &&
                     m.active &&
                     m.timetune_active).FirstOrDefault();

                 if (user == null)
                     return false;

                 return DLL.Commons.Passwords.validate(user, attemptedPassword);
             }
             if (!string.IsNullOrEmpty(username) && username.Equals("SUPER_ADMIN"))
             {
                 var mem = new MembershipUser("SUPER_ADMIN",
                                             username,
                                             "",
                                             "SUPER_ADMIN",
                                             null,
                                             null,
                                             false,
                                             false,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now);
             }
             else if (!string.IsNullOrEmpty(username) && username.Equals("Sudo"))
             {
                 var mem = new MembershipUser("Sudo",
                                             username,
                                             "",
                                             "Sudo",
                                             null,
                                             null,
                                             false,
                                             false,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now);
             }

             using (var db = new Context())
             {
                 Employee emp = db.employee.Where(m => m.employee_code.Equals(username) && m.active).FirstOrDefault();

                 if (emp == null)
                     return false;


                 var mem= new MembershipUser(emp.first_name,
                                             username,
                                             "",
                                             emp.employee_code,
                                             null,
                                             null,
                                             false,
                                             false,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now,
                                             DateTime.Now);
             }
            return true;
             }
        }
    }
}
