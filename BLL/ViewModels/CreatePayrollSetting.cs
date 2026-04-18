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
    public class CreatePayrollSetting
    {
        public string strMessage;

        public DateTime SessionStartDate;
        public DateTime SessionEndDate;

        public string strSessionStartDate;
        public string strSessionEndDate;

        public int AvailableSickLeaves;
        public int AvailableCasualLeaves;
        public int AvailableAnnualLeaves;

        public int AvailedSickLeaves;
        public int AvailedCasualLeaves;
        public int AvailedAnnualLeaves;

        public int LastMonthSickLeaves;
        public int LastMonthCasualLeaves;
        public int LastMonthAnnualLeaves;

        public PayrollInfo payroll_info;

        public List<BankNameInfo> bank_name_info;
        public List<PaymentModeInfo> payment_mode_info;
        public List<PaymentStatusInfo> payment_status_info;
    }
}
