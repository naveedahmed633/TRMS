using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddEmployeePassword.Models;
using System.Web;
using DLL.Models;

namespace AddEmployeePassword
{
    public class BLL
    {
        public static void Main(string[] args)
        {
            List<UserPassword> list = new List<UserPassword>();
            using (var db = new AddEmployeePassword.Models.Context2())
            {
                list=db.UserPassword.ToList();
            }
                using(var db2 = new DLL.Models.Context())
                {
                    foreach(var entity in list)
                    {
                        string userId=entity.UserId;
                        var employee=db2.employee.Where(m => m.employee_code.Equals(userId)).FirstOrDefault();
                        if (employee != null)
                        {
                            DLL.Commons.Passwords.setPassword(employee, entity.Password);
                        }
                        db2.SaveChanges();
                    }
                    
                }
                
            
        }
    }
}
