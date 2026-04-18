using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    // This class is just a Big View Model for sending multiple models to the
    // employee CRUD view.
    public class CreateLoanApplication
    {
        public List<Employee> employees;
        public List<LoanTypeInfo> loan_types;
        public List<LoanStatusTypeInfo> loan_status_types;
    }
}
