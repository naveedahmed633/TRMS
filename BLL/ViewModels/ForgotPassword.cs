using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
    public class ForgotPassword
    {
        [Display(Name = "User Name")]
        public string EmployeeCode { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmployeeEmail { get; set; }

        //public string employee_code { get; set; }
        //public string email { get; set; }
    }

    public class ResetPassword
    {
        [Display(Name = "Guid Code")]
        public string GuidCode { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm - New Password")]
        public string NewPasswordConfirm { get; set; }
        
    }
}
