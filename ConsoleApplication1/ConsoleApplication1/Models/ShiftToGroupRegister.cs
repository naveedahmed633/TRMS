using DLL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class ShiftToGroupRegister
    {
        public int ShiftToGroupRegisterId { get; set; }
        public virtual Group group { get; set; }
        public virtual Shift shift { get; set; }
        public bool active { get; set; }
    }
}
