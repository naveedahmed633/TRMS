using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BLL.ADMIN
{
    public class EmployeeWithGroup
    {
        public EmployeeWithGroup()
        {
            employeeFollowGeneralCalander = new List<Employee>();
            employeeFollowGroupCalander = new List<Employee>();
        }
        public List<Employee> employeeFollowGroupCalander {get;set;}
        public List<Employee> employeeFollowGeneralCalander {get;set;}
    }
}
