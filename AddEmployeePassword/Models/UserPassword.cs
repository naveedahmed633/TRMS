using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddEmployeePassword.Models
{
    public class UserPassword
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity),Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
