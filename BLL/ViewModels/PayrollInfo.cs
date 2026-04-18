using DLL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTune;

namespace ViewModels
{
    /*
     * This View is used to load the finalized present
     * and absent report
     */

    public class PayrollInfo
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime SalaryMonthYear { get; set; }
        public string SalaryMonthYearText { get; set; }

        public string CNIC { get; set; }
        public string NTNNumber { get; set; }
        public string EOBINumber { get; set; }
        public string SESSINumber { get; set; }

        public int BasicPay { get; set; }
        public int Increment { get; set; }
        public int Transport { get; set; }
        public int Mobile { get; set; }
        public int Medical { get; set; }
        public int Food { get; set; }
        public int Night { get; set; }
        public int GroupAllowance { get; set; }
        public int Commission { get; set; }
        public int Rent { get; set; }
        public int CashAllowance { get; set; }
        public int AnnualBonus { get; set; }
        public int LeavesCount { get; set; }
        public int LeavesEncash { get; set; }
        public decimal OvertimeInHours { get; set; }
        public int OvertimeAmount { get; set; }

        public int IncomeTax { get; set; }
        public int FineExtraAmount { get; set; }
        public int MobileDeduction { get; set; }
        public int AbsentsCount { get; set; }
        public int AbsentsAmount { get; set; }
        public int EOBIEmployee { get; set; }
        public int EOBIEmployer { get; set; }
        public int SESSIEmployee { get; set; }
        public int SESSIEmployer { get; set; }
        public int LoanInstallment { get; set; }
        public int OtherDeduction { get; set; }

        public int GrossSalary { get; set; }
        public int TotalDeduction { get; set; }
        public int NetSalary { get; set; }

        public int PaymentModeId { get; set; } //TABLE: PaymentModeTypes
        public string PaymentModeText { get; set; }
        public int BankNameId { get; set; } //TABLE: BankNames
        public string BankNameText { get; set; }
        public string BankAccTitle { get; set; }
        public string BankAccNo { get; set; }

        public int AckStatusId { get; set; } //TABLE: PaymentStatusTypes
        public string AckStatusText { get; set; }
        public int PayStatusId { get; set; } //TABLE: PaymentStatusTypes
        public string PayStatusText { get; set; }

        public string UserHRComments { get; set; }
        public string AttachmentFilePath { get; set; }
        public string IsFirstMonthText { get; set; }

        public DateTime PaymentDateTime { get; set; }
        public string PaymentDateTimeText { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }

        public string actions { get; set; }
    }

    public class PayrollAmountInfo
    {
        [Key]
        public int Id { get; set; }

        public int DesignationId { get; set; }
        public string DesignationText { get; set; }
        public int GradeId { get; set; }
        public string GradeText { get; set; }

        public int BasicPay { get; set; }
        public int Increment { get; set; }
        public int Transport { get; set; }
        public int Mobile { get; set; }
        public int Medical { get; set; }
        public int Food { get; set; }
        public int Night { get; set; }
        public int GroupAllowance { get; set; }
        public int Commission { get; set; }
        public int Rent { get; set; }
        public int CashAllowance { get; set; }

        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }

        public string actions { get; set; }
    }

    public class PayrollInfoForPDF
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string JoiningDateText { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public PayrollInfo PayrollInformation { get; set; }
        public string SalaryMonthYearText { get; set; }
        public string PaymentDatetimeText { get; set; }

        public string LeavesSessionText { get; set; }
        public string AllocatedLeaves { get; set; }
        public string AvailedLeaves { get; set; }
        public string AvailedLeavesLastMonth { get; set; }
        public string RemainingLeaves { get; set; }
    }


    public class PayrollStatementForPDF
    {
        public string MonthYear { get; set; }
        public int EmployeesCount { get; set; }
        public List<PayrollInfoForPDF> PayrollInfoList { get; set; }
        public int IncomeTax { get; set; }
        public int NetAmount { get; set; }
    }

    public class PayrollStatus
    {
        [Key]
        public int Id { get; set; }
        public int StatusId { get; set; }
    }

    public class PayrollResultSet
    {
        public static List<ViewModels.PayrollInfo> getPayrollByEmployeeCode(string emp_code)
        {
            string strBankName = "", strPaymentMode = "";
            int iBankNameId = 0, iPaymentModeId = 0;
            List<Payroll> payroll = null;
            List<ViewModels.PayrollInfo> toReturn = new List<ViewModels.PayrollInfo>();

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active == true && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        payroll = db.payroll.Where(m => m.EmployeeId == data_emp.EmployeeId).OrderByDescending(o => o.SalaryMonthYear).ToList();
                        if (payroll != null && payroll.Count > 0)
                        {
                            for (int i = 0; i < payroll.Count(); i++)
                            {
                                iBankNameId = payroll[i].BankNameId;
                                strBankName = db.bank_name.Where(b => b.Id == iBankNameId).FirstOrDefault().BankNameText ?? "";

                                iPaymentModeId = payroll[i].PaymentModeId;
                                strPaymentMode = db.payment_mode.Where(p => p.Id == iPaymentModeId).FirstOrDefault().PaymentModeText ?? "";

                                toReturn.Add(new ViewModels.PayrollInfo()
                                {
                                    Id = payroll[i].Id,
                                    EmployeeId = payroll[i].EmployeeId,
                                    EmployeeCode = data_emp.employee_code,
                                    EmployeeName = data_emp.first_name + " " + data_emp.last_name,
                                    SalaryMonthYear = payroll[i].SalaryMonthYear,
                                    SalaryMonthYearText = payroll[i].SalaryMonthYear.ToString("MMM yyyy"),
                                    CNIC = payroll[i].CNIC,
                                    NTNNumber = payroll[i].NTNNumber,
                                    EOBINumber = payroll[i].EOBINumber,
                                    SESSINumber = payroll[i].SESSINumber,
                                    BasicPay = payroll[i].BasicPay,
                                    Increment = payroll[i].Increment,
                                    Transport = payroll[i].Transport,
                                    Mobile = payroll[i].Mobile,
                                    Medical = payroll[i].Medical,
                                    Food = payroll[i].Food,
                                    Night = payroll[i].Night,
                                    GroupAllowance = payroll[i].GroupAllowance,
                                    Commission = payroll[i].Commission,
                                    Rent = payroll[i].Rent,
                                    CashAllowance = payroll[i].CashAllowance,
                                    AnnualBonus = payroll[i].AnnualBonus,
                                    LeavesCount = payroll[i].LeavesCount,
                                    LeavesEncash = payroll[i].LeavesEncash,
                                    OvertimeInHours = payroll[i].OvertimeInHours,
                                    OvertimeAmount = payroll[i].OvertimeAmount,
                                    IncomeTax = payroll[i].IncomeTax,
                                    FineExtraAmount = payroll[i].FineExtraAmount,
                                    MobileDeduction = payroll[i].MobileDeduction,
                                    AbsentsCount = payroll[i].AbsentsCount,
                                    AbsentsAmount = payroll[i].AbsentsAmount,
                                    EOBIEmployee = payroll[i].EOBIEmployee,
                                    EOBIEmployer = payroll[i].EOBIEmployer,
                                    SESSIEmployee = payroll[i].SESSIEmployee,
                                    SESSIEmployer = payroll[i].SESSIEmployer,
                                    LoanInstallment = payroll[i].LoanInstallment,
                                    OtherDeduction = payroll[i].OtherDeduction,
                                    GrossSalary = payroll[i].GrossSalary,
                                    TotalDeduction = payroll[i].TotalDeduction,
                                    NetSalary = payroll[i].NetSalary,
                                    PaymentModeId = payroll[i].PaymentModeId,
                                    PaymentModeText = strPaymentMode,
                                    BankNameText = strBankName,
                                    BankNameId = payroll[i].BankNameId,
                                    BankAccTitle = payroll[i].BankAccTitle,
                                    BankAccNo = payroll[i].BankAccNo,
                                    AckStatusId = payroll[i].AckStatusId,
                                    AckStatusText = payroll[i].AckStatusId == 1 ? "Unpaid" : "Paid",
                                    PayStatusId = payroll[i].PayStatusId,
                                    PayStatusText = payroll[i].PayStatusId == 1 ? "Unpaid" : "Paid",
                                    UserHRComments = payroll[i].UserHRComments,
                                    IsFirstMonthText = payroll[i].IsFirstMonthText,
                                    PaymentDateTimeText = payroll[i].PaymentDateTime.ToString("dd-MMM-yyyy"),
                                    CreateDateTimeText = payroll[i].CreateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    UpdateDateTimeText = payroll[i].UpdateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    AttachmentFilePath = (payroll[i].AttachmentFilePath != null && payroll[i].AttachmentFilePath != "") ? "<span data-row='" + payroll[i].Id + "'>" +
                                            "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + payroll[i].AttachmentFilePath + "'));\">View</a>" +
                                        "</span>" : "--",
                                    //actions =
                                    //    "<span data-row='" + payroll[i].Id + "'>" +
                                    //        "<a href=\"javascript:void(editPayroll(" + payroll[i].Id + "," + payroll[i].EmployeeId + ",'" + payroll[i].PaymentModeId + "','" + payroll[i].PayStatusId + "'));\">Edit</a>" +
                                    //        "<span> / </span>" +
                                    //        "<a href=\"javascript:void(deletePayroll(" + payroll[i].Id + "));\">Delete</a>" +
                                    //    "</span>",
                                    actions =
                                        "<span data-row='" + payroll[i].Id + "'>" +
                                            "<a href=\"/HR/PayrollsManagement/PayrollSetting?user_code=" + data_emp.employee_code + "&salary_month=" + payroll[i].SalaryMonthYear.ToString("dd-MM-yyyy") + "\" target=\"_blank\">Edit Payroll</a>" +
                                        "</span>"
                                });
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    payroll = new List<Payroll>();
                }

                return toReturn;
            }
        }

        public static int GetPayrollGenerationStatus()
        {
            int response = 0;
            string dtLastMonth = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");

            try
            {
                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;

                fromDate = DateTime.ParseExact(dtLastMonth, "yyyy-MM", CultureInfo.InvariantCulture);
                toDate = new DateTime(fromDate.Year, fromDate.Month, System.DateTime.DaysInMonth(fromDate.Year, fromDate.Month), 23, 59, 59);

                using (Context db = new Context())
                {
                    var data_payroll = db.payroll.Where(p => p.IsFirstMonthText != "Yes" && (p.SalaryMonthYear >= fromDate && p.SalaryMonthYear <= toDate)).FirstOrDefault();
                    if (data_payroll == null)
                    {
                        response = 1;//OK - no data found - for current month payroll
                    }
                }
            }
            catch (Exception ex)
            {
                response = -1;
                //throw ex;
            }

            return response;
        }

        public static int AddNewPayroll(PayrollInfo pinfo)
        {
            int response = 0;

            try
            {
                using (Context db = new Context())
                {
                    //add new payroll record
                    Payroll payroll = new Payroll();

                    payroll.EmployeeId = pinfo.EmployeeId;
                    payroll.SalaryMonthYear = pinfo.SalaryMonthYear;
                    payroll.CNIC = pinfo.CNIC;
                    payroll.NTNNumber = pinfo.NTNNumber;
                    payroll.EOBINumber = pinfo.EOBINumber;
                    payroll.SESSINumber = pinfo.SESSINumber;

                    payroll.BasicPay = pinfo.BasicPay;
                    payroll.Increment = pinfo.Increment;
                    payroll.Transport = pinfo.Transport;
                    payroll.Mobile = pinfo.Mobile;
                    payroll.Medical = pinfo.Medical;
                    payroll.Food = pinfo.Food;
                    payroll.Night = pinfo.Night;
                    payroll.GroupAllowance = pinfo.GroupAllowance;
                    payroll.Commission = pinfo.Commission;
                    payroll.Rent = pinfo.Rent;
                    payroll.CashAllowance = pinfo.CashAllowance;
                    payroll.AnnualBonus = pinfo.AnnualBonus;
                    payroll.LeavesCount = pinfo.LeavesCount;
                    payroll.LeavesEncash = pinfo.LeavesEncash;
                    payroll.OvertimeInHours = pinfo.OvertimeInHours;
                    payroll.OvertimeAmount = pinfo.OvertimeAmount;

                    payroll.IncomeTax = pinfo.IncomeTax;
                    payroll.FineExtraAmount = pinfo.FineExtraAmount;
                    payroll.MobileDeduction = pinfo.MobileDeduction;
                    payroll.AbsentsCount = pinfo.AbsentsCount;
                    payroll.AbsentsAmount = pinfo.AbsentsAmount;

                    payroll.EOBIEmployee = pinfo.EOBIEmployee;
                    payroll.EOBIEmployer = pinfo.EOBIEmployer;
                    payroll.SESSIEmployee = pinfo.SESSIEmployee;
                    payroll.SESSIEmployer = pinfo.SESSIEmployer;
                    payroll.LoanInstallment = pinfo.LoanInstallment;
                    payroll.OtherDeduction = pinfo.OtherDeduction;

                    payroll.GrossSalary = pinfo.GrossSalary;
                    payroll.TotalDeduction = pinfo.TotalDeduction;
                    payroll.NetSalary = pinfo.NetSalary;

                    payroll.PaymentModeId = pinfo.PaymentModeId;
                    payroll.BankNameId = pinfo.BankNameId;
                    payroll.BankAccTitle = pinfo.BankAccTitle;
                    payroll.BankAccNo = pinfo.BankAccNo;

                    payroll.AckStatusId = pinfo.AckStatusId;
                    payroll.PayStatusId = pinfo.PayStatusId;

                    payroll.UserHRComments = pinfo.UserHRComments;
                    payroll.AttachmentFilePath = pinfo.AttachmentFilePath;
                    payroll.IsFirstMonthText = "YES";

                    payroll.PaymentDateTime = pinfo.PaymentDateTime;
                    payroll.CreateDateTime = DateTime.Now;
                    payroll.UpdateDateTime = DateTime.Now;

                    db.payroll.Add(payroll);
                    db.SaveChanges();

                    response = 1;
                }
            }
            catch (Exception ex)
            {
                response = -1;
                //throw ex;
            }

            return response;
        }

        public static int GetUserId(string user_employee_code)
        {
            int user_id = 0;

            using (Context db = new Context())
            {
                user_id = db.employee.Where(e => e.active && e.employee_code == user_employee_code).FirstOrDefault().EmployeeId;
            }

            return user_id;
        }

        ////////////////////////////////////////////////////////////////

        public static List<PayrollReportByMonthLog> GetResult(string search, string sortOrder, int start, int length, List<PayrollReportByMonthLog> dtResult)
        {
            return FilterResult(search, dtResult).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<PayrollReportByMonthLog> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<PayrollReportByMonthLog> FilterResult(string search, List<PayrollReportByMonthLog> dtResult)
        {
            IQueryable<PayrollReportByMonthLog> results = dtResult.AsQueryable();

            //results = results.Where(p =>
            //search == null ||
            //(
            //p.FromDate != null));

            results = results.Where(p =>
                    (
                        search == null || search.Equals("") ||
                        (
                                p.Id.ToString().ToLower().Contains(search.ToLower())
                            || (p.EmployeeCode != null && p.EmployeeCode.ToLower().Contains(search.ToLower()))
                            || (p.EmployeeName != null && p.EmployeeName.ToLower().Contains(search.ToLower()))
                            || (p.SalaryMonthYearText != null && p.SalaryMonthYearText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.CNIC != null && p.CNIC.ToString().ToLower().Contains(search.ToLower()))
                            || (p.NTNNumber != null && p.NTNNumber.ToString().ToLower().Contains(search.ToLower()))
                            || (p.EOBINumber != null && p.EOBINumber.ToString().ToLower().Contains(search.ToLower()))
                            || (p.SESSINumber != null && p.SESSINumber.ToString().ToLower().Contains(search.ToLower()))
                            || (p.BasicPay.ToString().Contains(search.ToLower()))
                            || (p.GrossSalary.ToString().Contains(search.ToLower()))
                            || (p.TotalDeduction.ToString().Contains(search.ToLower()))
                            || (p.NetSalary.ToString().Contains(search.ToLower()))
                            || (p.BankNameText != null && p.BankNameText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.BankAccNo != null && p.BankAccNo.ToString().ToLower().Contains(search.ToLower()))
                            || (p.BankAccTitle != null && p.BankAccTitle.ToString().ToLower().Contains(search.ToLower()))
                            || (p.PaymentModeText != null && p.PaymentModeText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.AckStatusText != null && p.AckStatusText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.PayStatusText != null && p.PayStatusText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.UserHRComments != null && p.UserHRComments.ToLower().Contains(search.ToLower()))
                        // || EntityFunctions.TruncateTime(p.FromDate).ToString().Contains(search.ToLower()) || EntityFunctions.TruncateTime(p.ToDate).ToString().Contains(search.ToLower())


                        )
                    //&&
                    //  (depart_id.Equals(-1) || p.ReasonDetail.department.DepartmentId.Equals(depart_id))
                    //       &&
                    //  (des_id.Equals(-1) || ca.employee.designation.DesignationId.Equals(des_id))
                    )

                    );

            return results;
        }

        ///////////////////////////////////////////////////////////////

        public static List<PayrollInfo> GetResult(string search, string sortOrder, int start, int length, List<PayrollInfo> dtResult)
        {
            return FilterResult(search, dtResult).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<PayrollInfo> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<PayrollInfo> FilterResult(string search, List<PayrollInfo> dtResult)
        {
            IQueryable<PayrollInfo> results = dtResult.AsQueryable();

            //results = results.Where(p =>
            //search == null ||
            //(
            //p.FromDate != null));

            results = results.Where(p =>
                    (
                        search == null || search.Equals("") ||
                        (
                                p.Id.ToString().ToLower().Contains(search.ToLower())
                            || (p.EmployeeCode != null && p.EmployeeCode.ToLower().Contains(search.ToLower()))
                            || (p.CNIC != null && p.CNIC.ToString().Contains(search.ToLower()))
                            || (p.NTNNumber != null && p.NTNNumber.ToString().Contains(search.ToLower()))
                            || (p.EOBINumber != null && p.EOBINumber.ToString().Contains(search.ToLower()))
                            || (p.SESSINumber != null && p.SESSINumber.ToString().Contains(search.ToLower()))
                            || (p.BasicPay.ToString().Contains(search.ToLower()))
                            || (p.UserHRComments != null && p.UserHRComments.ToLower().Contains(search.ToLower()))
                        // || EntityFunctions.TruncateTime(p.FromDate).ToString().Contains(search.ToLower()) || EntityFunctions.TruncateTime(p.ToDate).ToString().Contains(search.ToLower())


                        )
                    //&&
                    //  (depart_id.Equals(-1) || p.ReasonDetail.department.DepartmentId.Equals(depart_id))
                    //       &&
                    //  (des_id.Equals(-1) || ca.employee.designation.DesignationId.Equals(des_id))
                    )

                    );

            return results;
        }


        ///////////////////////////////////////////////////////////

        public static void update(ViewModels.PayrollInfo pinfo)
        {
            int gross_salary = 0, total_deduction = 0, net_salary = 0;
            int per_day_salary = 0;

            //count days in year
            int days_in_year = 0;
            for (int m = 1; m <= 12; m++)
            {
                days_in_year += DateTime.DaysInMonth(DateTime.Now.Year, m);
            }

            using (Context db = new Context())
            {
                Payroll payroll = db.payroll.Find(pinfo.Id);

                var data_emp = db.employee.Where(e => e.active && e.EmployeeId == payroll.EmployeeId).FirstOrDefault();
                if (data_emp != null)
                {
                    payroll.EmployeeId = pinfo.EmployeeId;
                    payroll.SalaryMonthYear = pinfo.SalaryMonthYear;
                    payroll.CNIC = pinfo.CNIC;
                    payroll.NTNNumber = pinfo.NTNNumber;
                    payroll.EOBINumber = pinfo.EOBINumber;
                    payroll.SESSINumber = pinfo.SESSINumber;

                    //gross
                    payroll.BasicPay = pinfo.BasicPay;
                    payroll.Increment = pinfo.Increment;
                    per_day_salary = (pinfo.BasicPay + pinfo.Increment) * 12 / days_in_year;

                    /////////////////////////////// Leaves Counter //////////////////////////////////////////////////
                    int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                    leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(data_emp.employee_code);
                    int iAvailableSickLeaves = leaves[0];
                    int iAvailableCasualLeaves = leaves[1];
                    int iAvailableAnnualLeaves = leaves[2];
                    int iAvailedSickLeaves = leaves[3];
                    int iAvailedCasualLeaves = leaves[4];
                    int iAvailedAnnualLeaves = leaves[5];
                    int iTotalAvailed = iAvailedSickLeaves + iAvailedCasualLeaves + iAvailedAnnualLeaves;
                    int iTotalAvailable = iAvailableSickLeaves + iAvailableCasualLeaves + iAvailableAnnualLeaves;

                    int[] lastMonthLeaves = new int[3] { 0, 0, 0 };
                    lastMonthLeaves = LeaveApplicationResultSet.getUserLastMonthLeavesByUserCode(data_emp.employee_code, pinfo.SalaryMonthYear.ToString("yyyy-MM"));
                    int iLastMonthAvailableSickLeaves = lastMonthLeaves[0];
                    int iLastMonthAvailableCasualLeaves = lastMonthLeaves[1];
                    int iLastMonthAvailableAnnualLeaves = lastMonthLeaves[2];
                    int iLastMonthAvailed = iLastMonthAvailableSickLeaves + iLastMonthAvailableCasualLeaves + iLastMonthAvailableAnnualLeaves;
                    //////////////////////////////////////////////////////////////////////////////////////////////

                    payroll.Transport = pinfo.Transport;
                    payroll.Mobile = pinfo.Mobile;
                    payroll.Medical = pinfo.Medical;
                    payroll.Food = pinfo.Food;
                    payroll.Night = pinfo.Night;
                    payroll.GroupAllowance = pinfo.GroupAllowance;
                    payroll.Commission = pinfo.Commission;
                    payroll.Rent = pinfo.Rent;
                    payroll.CashAllowance = pinfo.CashAllowance;
                    payroll.AnnualBonus = pinfo.AnnualBonus;
                    payroll.LeavesCount = pinfo.LeavesCount;
                    payroll.LeavesEncash = pinfo.LeavesCount * per_day_salary;
                    payroll.OvertimeInHours = pinfo.OvertimeInHours;
                    payroll.OvertimeAmount = int.Parse(Math.Floor(pinfo.OvertimeInHours * (per_day_salary / 8)).ToString());
                    gross_salary = pinfo.BasicPay + pinfo.Increment + pinfo.Transport + pinfo.Mobile + pinfo.Medical + pinfo.Food + pinfo.Night + pinfo.GroupAllowance + pinfo.Commission + pinfo.Rent + pinfo.CashAllowance + pinfo.AnnualBonus + payroll.LeavesEncash + payroll.OvertimeAmount;

                    //deductions
                    payroll.IncomeTax = pinfo.IncomeTax;
                    payroll.FineExtraAmount = pinfo.FineExtraAmount;
                    payroll.MobileDeduction = pinfo.MobileDeduction;

                    if (pinfo.AbsentsCount == 0 && iTotalAvailed > iTotalAvailable && iLastMonthAvailed > 0)
                    {
                        payroll.AbsentsCount = Math.Abs(iTotalAvailed - iTotalAvailable);
                        payroll.AbsentsAmount = payroll.AbsentsCount * per_day_salary;
                    }
                    else
                    {
                        payroll.AbsentsCount = pinfo.AbsentsCount;
                        payroll.AbsentsAmount = pinfo.AbsentsAmount;
                    }

                    payroll.EOBIEmployee = pinfo.EOBIEmployee;
                    payroll.EOBIEmployer = pinfo.EOBIEmployer;
                    payroll.SESSIEmployee = pinfo.SESSIEmployee;
                    payroll.SESSIEmployer = pinfo.SESSIEmployer;
                    payroll.LoanInstallment = pinfo.LoanInstallment;
                    payroll.OtherDeduction = pinfo.OtherDeduction;
                    total_deduction = pinfo.IncomeTax + pinfo.FineExtraAmount + pinfo.MobileDeduction + payroll.AbsentsAmount + pinfo.EOBIEmployee + pinfo.SESSIEmployee + pinfo.LoanInstallment + pinfo.OtherDeduction;

                    //net salary calc
                    payroll.GrossSalary = gross_salary;
                    payroll.TotalDeduction = total_deduction;

                    net_salary = gross_salary - total_deduction;
                    payroll.NetSalary = net_salary;

                    //mode
                    payroll.PaymentModeId = pinfo.PaymentModeId;
                    payroll.BankNameId = pinfo.BankNameId;
                    payroll.BankAccTitle = pinfo.BankAccTitle;
                    payroll.BankAccNo = pinfo.BankAccNo;

                    payroll.AckStatusId = pinfo.AckStatusId;
                    payroll.PayStatusId = pinfo.PayStatusId;

                    payroll.UserHRComments = pinfo.UserHRComments;
                    payroll.AttachmentFilePath = pinfo.AttachmentFilePath;
                    payroll.IsFirstMonthText = pinfo.IsFirstMonthText;

                    payroll.PaymentDateTime = pinfo.PaymentDateTime;
                    payroll.UpdateDateTime = DateTime.Now;

                    db.SaveChanges();
                }
            }
        }

        public static ViewModels.PayrollInfo remove(ViewModels.PayrollInfo toRemove)
        {

            using (Context db = new Context())
            {
                Payroll toRemoveModel = db.payroll.Find(toRemove.Id);

                db.payroll.Remove(toRemoveModel);

                //toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove;
            }

        }

        public static int UpdatePayrollStatus(string month_year, int department_sid, string payment_date, int payment_status_id)
        {
            int response = 0;
            DateTime from_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            DateTime dtPayment = DateTime.Now;

            try
            {
                from_date = DateTime.ParseExact(month_year, "yyyy-MM", CultureInfo.InvariantCulture);
                to_date = new DateTime(from_date.Year, from_date.Month, System.DateTime.DaysInMonth(from_date.Year, from_date.Month), 23, 59, 59);

                dtPayment = DateTime.ParseExact(payment_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                using (Context db = new Context())
                {
                    var data_emp = db.employee.Where(e => e.active && e.department.DepartmentId == department_sid).ToList();
                    if (data_emp != null && data_emp.Count > 0)
                    {
                        foreach (var e in data_emp)
                        {
                            var data_payroll = db.payroll.Where(p => p.EmployeeId == e.EmployeeId && (p.SalaryMonthYear >= from_date && p.SalaryMonthYear <= to_date)).FirstOrDefault();
                            if (data_payroll != null)
                            {
                                data_payroll.PayStatusId = payment_status_id;
                                data_payroll.PaymentDateTime = dtPayment;
                                data_payroll.UpdateDateTime = DateTime.Now;

                                db.SaveChanges();
                                response = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = -1;
            }

            return response;
        }

        //////////////////////////////////////////

        public static DateTime[] getUserSessionDatesByUserCode(string user_code)
        {
            int user_id = 0;
            DateTime[] dt = new DateTime[2];

            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                if (data_user != null)
                {
                    user_id = data_user.EmployeeId;

                    var lSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                    if (lSessionData != null)
                    {
                        dt[0] = lSessionData.SessionStartDate;
                        dt[1] = lSessionData.SessionEndDate;
                    }
                }

                return dt;
            }
        }

        public static DateTime[] getStaffSessionDatesByUserId(string emp_user_id)
        {
            int user_id = 0;
            DateTime[] dt = new DateTime[2];

            using (Context db = new Context())
            {
                //var userData = db.employee.Where(e => e.EmployeeId == user_id).FirstOrDefault();
                user_id = int.Parse(emp_user_id);

                var lSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                if (lSessionData != null)
                {
                    dt[0] = lSessionData.SessionStartDate;
                    dt[1] = lSessionData.SessionEndDate;
                }

                return dt;
            }
        }

        ////////////////////////////////////////////////////////////////

        public static List<ViewModels.PaymentStatusInfo> getAllPaymentStatusTypes()
        {
            List<ViewModels.PaymentStatusInfo> toReturn = new List<ViewModels.PaymentStatusInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.PaymentStatusType> pStatus = null;
                try
                {
                    pStatus = db.payment_status.ToList();
                    if (pStatus != null && pStatus.Count > 0)
                    {
                        for (int i = 0; i < pStatus.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.PaymentStatusInfo()
                            {
                                Id = pStatus[i].Id,
                                PaymentStatusText = pStatus[i].PaymentStatusText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    pStatus = new List<DLL.Models.PaymentStatusType>();
                }

                return toReturn;
            }
        }

        ////////////////////////////////////////////////////////////////

        public static List<ViewModels.Department> getAllDepartments()
        {
            List<ViewModels.Department> toReturn = new List<ViewModels.Department>();

            using (Context db = new Context())
            {
                List<DLL.Models.Department> depList = null;

                try
                {
                    depList = db.department.Where(e => e.active).ToList();
                    if (depList != null && depList.Count > 0)
                    {
                        for (int i = 0; i < depList.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.Department()
                            {
                                id = depList[i].DepartmentId,
                                name = depList[i].name
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    depList = new List<DLL.Models.Department>();
                }

                return toReturn;
            }
        }

        public static List<ViewModels.BankNameInfo> getAllBankNames()
        {
            List<ViewModels.BankNameInfo> toReturn = new List<ViewModels.BankNameInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.BankName> bNames = null;

                try
                {
                    bNames = db.bank_name.ToList();
                    if (bNames != null && bNames.Count > 0)
                    {
                        for (int i = 0; i < bNames.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.BankNameInfo()
                            {
                                Id = bNames[i].Id,
                                BankNameText = bNames[i].BankNameText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    bNames = new List<DLL.Models.BankName>();
                }

                return toReturn;
            }
        }


        public static List<ViewModels.PaymentModeInfo> getAllPaymentModeTypes()
        {
            List<ViewModels.PaymentModeInfo> toReturn = new List<ViewModels.PaymentModeInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.PaymentModeType> pMode = null;

                try
                {
                    pMode = db.payment_mode.ToList();
                    if (pMode != null && pMode.Count > 0)
                    {
                        for (int i = 0; i < pMode.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.PaymentModeInfo()
                            {
                                Id = pMode[i].Id,
                                PaymentModeText = pMode[i].PaymentModeText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    pMode = new List<DLL.Models.PaymentModeType>();
                }

                return toReturn;
            }
        }

        ///////////////////////////////////////////////////////////////

        public static PayrollInfo getLatestPayrollInfoByEmployeeCode(string emp_code, string month_year)
        {
            PayrollInfo payroll = new PayrollInfo();

            DateTime dtMonth = Convert.ToDateTime(month_year);
            DateTime month_start_date = new DateTime(dtMonth.Year, dtMonth.Month, 1);
            DateTime month_end_date = new DateTime(dtMonth.Year, dtMonth.Month, System.DateTime.DaysInMonth(dtMonth.Year, dtMonth.Month));

            using (Context db = new Context())
            {
                var data_emp = db.employee.Where(p => p.active == true && p.employee_code == emp_code).FirstOrDefault();
                if (data_emp != null)
                {
                    var pinfo = db.payroll.Where(p => p.EmployeeId == data_emp.EmployeeId && (p.SalaryMonthYear >= month_start_date && p.SalaryMonthYear <= month_end_date)).OrderByDescending(o => o.Id).FirstOrDefault();
                    if (pinfo != null)
                    {
                        payroll.Id = pinfo.Id;
                        payroll.EmployeeId = pinfo.EmployeeId;
                        payroll.SalaryMonthYear = pinfo.SalaryMonthYear;
                        payroll.CNIC = pinfo.CNIC;
                        payroll.NTNNumber = pinfo.NTNNumber;
                        payroll.EOBINumber = pinfo.EOBINumber;
                        payroll.SESSINumber = pinfo.SESSINumber;

                        payroll.BasicPay = pinfo.BasicPay;
                        payroll.Increment = pinfo.Increment;
                        payroll.Transport = pinfo.Transport;
                        payroll.Mobile = pinfo.Mobile;
                        payroll.Medical = pinfo.Medical;
                        payroll.Food = pinfo.Food;
                        payroll.Night = pinfo.Night;
                        payroll.GroupAllowance = pinfo.GroupAllowance;
                        payroll.Commission = pinfo.Commission;
                        payroll.Rent = pinfo.Rent;
                        payroll.CashAllowance = pinfo.CashAllowance;
                        payroll.AnnualBonus = pinfo.AnnualBonus;
                        payroll.LeavesCount = pinfo.LeavesCount;
                        payroll.LeavesEncash = pinfo.LeavesEncash;
                        payroll.OvertimeInHours = pinfo.OvertimeInHours;
                        payroll.OvertimeAmount = pinfo.OvertimeAmount;

                        payroll.IncomeTax = pinfo.IncomeTax;
                        payroll.FineExtraAmount = pinfo.FineExtraAmount;
                        payroll.MobileDeduction = pinfo.MobileDeduction;
                        payroll.AbsentsCount = pinfo.AbsentsCount;
                        payroll.AbsentsAmount = pinfo.AbsentsAmount;

                        payroll.EOBIEmployee = pinfo.EOBIEmployee;
                        payroll.EOBIEmployer = pinfo.EOBIEmployer;
                        payroll.SESSIEmployee = pinfo.SESSIEmployee;
                        payroll.SESSIEmployer = pinfo.SESSIEmployer;
                        payroll.LoanInstallment = pinfo.LoanInstallment;
                        payroll.OtherDeduction = pinfo.OtherDeduction;

                        payroll.GrossSalary = pinfo.GrossSalary;
                        payroll.TotalDeduction = pinfo.TotalDeduction;
                        payroll.NetSalary = pinfo.NetSalary;

                        payroll.PaymentModeId = pinfo.PaymentModeId;
                        payroll.BankNameId = pinfo.BankNameId;
                        payroll.BankAccTitle = pinfo.BankAccTitle;
                        payroll.BankAccNo = pinfo.BankAccNo;

                        payroll.AckStatusId = pinfo.AckStatusId;
                        payroll.PayStatusId = pinfo.PayStatusId;

                        payroll.UserHRComments = pinfo.UserHRComments;
                        payroll.AttachmentFilePath = pinfo.AttachmentFilePath;
                        payroll.IsFirstMonthText = pinfo.IsFirstMonthText;

                        payroll.PaymentDateTime = pinfo.PaymentDateTime;
                        payroll.CreateDateTimeText = pinfo.CreateDateTime.ToString("dd-MM-yyyy hh:mm tt");
                        payroll.UpdateDateTimeText = pinfo.UpdateDateTime.ToString("dd-MM-yyyy hh:mm tt");
                    }
                }
            }

            return payroll;
        }

        ///////////////////////////////////////////////////////////////

        public static string getEmployeeName(string emp_code)
        {
            string emp_name = "-1";

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        emp_name = data_emp.first_name + " " + data_emp.last_name;
                    }
                }
                catch (Exception)
                {
                    emp_name = "-1";
                }

                return emp_name;
            }
        }

        public static int getEmployeeId(string emp_code)
        {
            int emp_id = 0;

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        emp_id = data_emp.EmployeeId;
                    }
                }
                catch (Exception)
                {
                    emp_id = -1;
                }

                return emp_id;
            }
        }

        ///////////////////////////////////////////////////////////////

        public static int GeneratePayrollForLastMonth()
        {
            int response = 0, iEmpLeaveCount = 0, iEmpOvertime = 0, iOvertimeMinSeconds = 0, iOvertimeWorkingHoursPerDay = 0, iEmpLoanAmount = 0;
            DateTime _1month_from_date = DateTime.Now, _1month_to_date = DateTime.Now;
            DateTime _2month_from_date = DateTime.Now, _2month_to_date = DateTime.Now;

            _1month_from_date = DateTime.ParseExact(DateTime.Now.AddMonths(-1).ToString("yyyy-MM"), "yyyy-MM", CultureInfo.InvariantCulture);
            _1month_to_date = new DateTime(_1month_from_date.Year, _1month_from_date.Month, System.DateTime.DaysInMonth(_1month_from_date.Year, _1month_from_date.Month), 23, 59, 59);

            _2month_from_date = DateTime.ParseExact(DateTime.Now.AddMonths(-2).ToString("yyyy-MM"), "yyyy-MM", CultureInfo.InvariantCulture);
            _2month_to_date = new DateTime(_2month_from_date.Year, _2month_from_date.Month, System.DateTime.DaysInMonth(_2month_from_date.Year, _2month_from_date.Month), 23, 59, 59);

            //count days in year
            int days_in_year = 0;
            for (int m = 1; m <= 12; m++)
            {
                days_in_year += DateTime.DaysInMonth(DateTime.Now.Year, m);
            }

            //get from AppSettings
            if (ConfigurationManager.AppSettings["OvertimeMinSeconds"] != null && ConfigurationManager.AppSettings["OvertimeMinSeconds"].ToString() != "")
            {
                iOvertimeMinSeconds = int.Parse(ConfigurationManager.AppSettings["OvertimeMinSeconds"].ToString());
            }
            else
            {
                iOvertimeMinSeconds = 900;//means 15 min
            }

            //get from AppSettings
            if (ConfigurationManager.AppSettings["OvertimeWorkingHoursPerDay"] != null && ConfigurationManager.AppSettings["OvertimeWorkingHoursPerDay"].ToString() != "")
            {
                iOvertimeWorkingHoursPerDay = int.Parse(ConfigurationManager.AppSettings["OvertimeWorkingHoursPerDay"].ToString());
            }
            else
            {
                iOvertimeWorkingHoursPerDay = 8;
            }           

            using (Context db = new Context())
            {
                try
                {
                    /////////////////////////// Loan Management //////////////////////////////////
                    var data_loan = db.loan.Where(l => l.LoanStatusId == 1 && (l.LoanAllocatedDate >= _2month_from_date && l.LoanAllocatedDate <= _2month_to_date)).ToList();
                    if (data_loan != null && data_loan.Count > 0)
                    {
                        foreach (var ln in data_loan)
                        {
                            var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == ln.EmployeeId).FirstOrDefault();
                            if (data_emp != null)
                            {
                                /////////////////////////// add new Loan record ////////////////////////////
                                Loan loan = new Loan();

                                loan.EmployeeId = ln.EmployeeId;
                                loan.LoanAllocatedDate = Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-07 00:00:00.000"));

                                loan.LoanTypeId = ln.LoanTypeId;
                                loan.LoanAmount = ln.LoanAmount;
                                loan.InstallmentNumbers = ln.InstallmentNumbers;
                                loan.InstallmentAmount = ln.InstallmentAmount;
                                loan.DeductableAmount = ln.DeductableAmount;

                                if (ln.DeductableAmount > 0)
                                {
                                    loan.BalanceAmount = ln.BalanceAmount - ln.DeductableAmount;
                                }
                                else
                                {
                                    loan.BalanceAmount = ln.BalanceAmount - ln.InstallmentAmount;
                                }

                                loan.LoanStatusId = 1;

                                //if (loan.BalanceAmount > 0)
                                //    loan.LoanStatusId = 1;
                                //else
                                //    loan.LoanStatusId = 2;

                                loan.Remarks = ln.Remarks;
                                loan.AttachmentFilePath = ln.AttachmentFilePath;
                                loan.CreateDateTime = ln.CreateDateTime;
                                loan.UpdateDateTime = DateTime.Now;

                                if (ln.BalanceAmount > 0)
                                {
                                    db.loan.Add(loan);
                                }

                                ////////////////////////// Update Previous Record ////////////////////////////

                                var data_loan_update = db.loan.Where(l => l.Id == ln.Id).FirstOrDefault();
                                if (data_loan_update != null)
                                {
                                    data_loan_update.DeductableAmount = 0;
                                }

                                /////////////////////////////////////////////////////////////////////////////

                                db.SaveChanges();
                            }
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////

                    int gross_salary = 0, total_deduction = 0, net_salary = 0;

                    var data_payroll = db.payroll.Where(p => (p.SalaryMonthYear >= _2month_from_date && p.SalaryMonthYear <= _2month_to_date)).ToList();
                    if (data_payroll != null && data_payroll.Count > 0)
                    {
                        foreach (var i in data_payroll)
                        {
                            var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == i.EmployeeId).FirstOrDefault();
                            if (data_emp != null)
                            {
                                Payroll pItem = new Payroll();

                                pItem.EmployeeId = i.EmployeeId;
                                pItem.SalaryMonthYear = Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-05 00:00:00.000"));
                                pItem.CNIC = i.CNIC;
                                pItem.NTNNumber = i.NTNNumber;
                                pItem.EOBINumber = i.EOBINumber;
                                pItem.SESSINumber = i.SESSINumber;

                                //set salary amounts using designation and grade of employee
                                if (data_emp.designation.DesignationId > 0 && data_emp.grade.GradeId > 0)
                                {
                                    ViewModels.PayrollAmountInfo pAmount = ViewModels.PayrollResultSet.IsPayrollAmountExists(data_emp.designation.DesignationId, data_emp.grade.GradeId);
                                    if (pAmount != null)
                                    {
                                        pItem.BasicPay = pAmount.BasicPay;
                                        pItem.Increment = pAmount.Increment;
                                        pItem.Transport = pAmount.Transport;
                                        pItem.Mobile = pAmount.Mobile;
                                        pItem.Medical = pAmount.Medical;
                                        pItem.CashAllowance = pAmount.CashAllowance;
                                        pItem.Commission = pAmount.Commission;
                                        pItem.Food = pAmount.Food;
                                        pItem.Night = pAmount.Night;
                                        pItem.Rent = pAmount.Rent;
                                        pItem.GroupAllowance = pAmount.GroupAllowance;
                                    }
                                    else
                                    {
                                        pItem.BasicPay = i.BasicPay;
                                        pItem.Increment = i.Increment > 0 ? 0 : i.Increment;
                                        pItem.Transport = i.Transport;
                                        pItem.Mobile = i.Mobile;
                                        pItem.Medical = i.Medical;
                                        pItem.CashAllowance = i.CashAllowance;
                                        pItem.Commission = i.Commission;
                                        pItem.Food = i.Food;
                                        pItem.Night = i.Night;
                                        pItem.Rent = i.Rent;
                                        pItem.GroupAllowance = i.GroupAllowance;
                                    }
                                }
                                else
                                {
                                    pItem.BasicPay = i.BasicPay;
                                    pItem.Increment = i.Increment > 0 ? 0 : i.Increment;
                                    pItem.Transport = i.Transport;
                                    pItem.Mobile = i.Mobile;
                                    pItem.Medical = i.Medical;
                                    pItem.CashAllowance = i.CashAllowance;
                                    pItem.Commission = i.Commission;
                                    pItem.Food = i.Food;
                                    pItem.Night = i.Night;
                                    pItem.Rent = i.Rent;
                                    pItem.GroupAllowance = i.GroupAllowance;
                                }

                                pItem.AnnualBonus = i.AnnualBonus > 0 ? 0 : i.AnnualBonus;
                                pItem.LeavesCount = i.LeavesCount > 0 ? 0 : i.LeavesCount;
                                pItem.LeavesEncash = i.LeavesEncash > 0 ? 0 : i.LeavesEncash;

                                /////////////////// Calc: Last Month - Approved Overtime ////////////////////////////////////
                                //if (i.EmployeeId == 29336)
                                //{
                                //    int a = 0;
                                //}

                                var data_overtime = db.consolidated_attendance.Where(c => c.employee.EmployeeId == i.EmployeeId && c.overtime_status == 2 && (c.date >= _1month_from_date && c.date <= _1month_to_date)).ToList();
                                if (data_overtime != null && data_overtime.Count > 0)
                                {
                                    foreach (var o in data_overtime)
                                    {
                                        iEmpOvertime += o.overtime;
                                    }
                                }

                                //if seconds > 900, it means 15 min
                                if (iEmpOvertime >= iOvertimeMinSeconds)
                                {
                                    pItem.OvertimeInHours = (iEmpOvertime / 3600);//convert seconds to hours
                                    pItem.OvertimeAmount = (iEmpOvertime / 3600) * (((i.BasicPay + i.Increment) * 12 / days_in_year) / iOvertimeWorkingHoursPerDay);
                                }
                                else
                                {
                                    pItem.OvertimeInHours = 0;
                                    pItem.OvertimeAmount = 0;
                                }
                                iEmpOvertime = 0;

                                //pItem.OvertimeInHours = 0; // i.OvertimeInHours;
                                //pItem.OvertimeAmount = 0; // i.OvertimeAmount;
                                /////////////////////////////////////////////////////////////////////////////

                                gross_salary = pItem.BasicPay + pItem.Increment + pItem.Transport + pItem.Mobile + pItem.Medical + pItem.CashAllowance + pItem.Commission + +pItem.Food + pItem.Night + pItem.Rent + i.GroupAllowance + pItem.AnnualBonus + pItem.LeavesEncash + pItem.OvertimeAmount;

                                pItem.IncomeTax = i.IncomeTax;
                                pItem.FineExtraAmount = i.FineExtraAmount;
                                pItem.MobileDeduction = i.MobileDeduction;

                                /////////////////// Calc: Last Month - Absents Count ////////////////////////////////////
                                //if (i.EmployeeId == 29336)
                                //{
                                //    int a = 0;
                                //}

                                var data_leave_emp = db.consolidated_attendance.Where(c => c.employee.EmployeeId == i.EmployeeId && (c.final_remarks.ToUpper() == "AB") && (c.date >= _1month_from_date && c.date <= _1month_to_date)).ToList();
                                if (data_leave_emp != null && data_leave_emp.Count > 0)
                                {
                                    iEmpLeaveCount = data_leave_emp.Count;
                                }
                                pItem.AbsentsCount = iEmpLeaveCount;
                                pItem.AbsentsAmount = iEmpLeaveCount * ((i.BasicPay + i.Increment) * 12 / days_in_year);
                                iEmpLeaveCount = 0;
                                /////////////////////////////////////////////////////////////////////////////

                                pItem.EOBIEmployee = i.EOBIEmployee;
                                pItem.EOBIEmployer = i.EOBIEmployer;
                                pItem.SESSIEmployee = i.SESSIEmployee;
                                pItem.SESSIEmployer = i.SESSIEmployer;

                                //////////////////////// Calc Emp Loan Amount /////////////////////////////////

                                //STEP - 1
                                var data_loan1_emp = db.loan.Where(l => l.EmployeeId == i.EmployeeId && l.LoanStatusId == 1 && (l.LoanAllocatedDate >= _1month_from_date && l.LoanAllocatedDate <= _1month_to_date)).ToList();
                                if (data_loan1_emp != null && data_loan1_emp.Count > 0)
                                {
                                    foreach (var k in data_loan1_emp)
                                    {
                                        //if (i.EmployeeId == 29336)
                                        //{
                                        //for break point usage
                                        //}
                                        if (k.DeductableAmount > 0)
                                            iEmpLoanAmount = iEmpLoanAmount + k.DeductableAmount;
                                        else
                                            iEmpLoanAmount = iEmpLoanAmount + k.InstallmentAmount;
                                    }
                                }

                                //STEP - 2
                                var data_loan2_emp = db.loan.Where(l => l.EmployeeId == i.EmployeeId && l.BalanceAmount == 0 && l.LoanStatusId == 1 && (l.LoanAllocatedDate >= _2month_from_date && l.LoanAllocatedDate <= _2month_to_date)).ToList();
                                if (data_loan2_emp != null && data_loan2_emp.Count > 0)
                                {
                                    foreach (var k in data_loan2_emp)
                                    {
                                        //if (i.EmployeeId == 29336)
                                        //{
                                        //for break point usage
                                        //}
                                        if (k.DeductableAmount > 0)
                                            iEmpLoanAmount = iEmpLoanAmount + k.DeductableAmount;
                                        else
                                            iEmpLoanAmount = iEmpLoanAmount + k.InstallmentAmount;
                                    }
                                }

                                pItem.LoanInstallment = iEmpLoanAmount; // i.LoanInstallment;
                                iEmpLoanAmount = 0;
                                //////////////////////// Calc Emp Loan Amount /////////////////////////////////

                                pItem.OtherDeduction = i.OtherDeduction;
                                total_deduction = pItem.IncomeTax + pItem.FineExtraAmount + pItem.MobileDeduction + pItem.AbsentsAmount + pItem.EOBIEmployee + pItem.SESSIEmployee + pItem.LoanInstallment + pItem.OtherDeduction;

                                //net salary calc
                                pItem.GrossSalary = gross_salary;
                                pItem.TotalDeduction = total_deduction;
                                net_salary = gross_salary - total_deduction;
                                pItem.NetSalary = net_salary;

                                pItem.GrossSalary = i.GrossSalary;
                                pItem.TotalDeduction = i.TotalDeduction;
                                pItem.NetSalary = i.NetSalary;

                                pItem.PaymentModeId = i.PaymentModeId;
                                pItem.BankNameId = i.BankNameId;
                                pItem.BankAccTitle = i.BankAccTitle;
                                pItem.BankAccNo = i.BankAccNo;

                                pItem.AckStatusId = 1;
                                pItem.PayStatusId = 1;

                                pItem.UserHRComments = null; // i.UserHRComments;
                                pItem.AttachmentFilePath = null; // i.AttachmentFilePath;
                                pItem.IsFirstMonthText = i.IsFirstMonthText == "YES" ? "NO" : i.IsFirstMonthText;

                                pItem.PaymentDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-10 00:00:00.000"));
                                pItem.CreateDateTime = DateTime.Now;
                                pItem.UpdateDateTime = DateTime.Now;

                                db.payroll.Add(pItem);
                                db.SaveChanges();
                            }
                        }
                    }

                    //////////////////////// update Loan status ////////////////////////////
                    //since last 2 months - set status to closed
                    var data_loan_status = db.loan.Where(l => l.LoanStatusId == 1 && l.BalanceAmount == 0 && (l.LoanAllocatedDate >= _2month_from_date && l.LoanAllocatedDate <= _1month_to_date)).ToList();
                    if (data_loan_status != null && data_loan_status.Count > 0)
                    {
                        foreach (var ln in data_loan_status)
                        {
                            ln.LoanStatusId = 2;//close the loans
                            ln.UpdateDateTime = DateTime.Now;

                            db.SaveChanges();
                        }
                    }

                    //////////////////////////////////////////////////////////////////////

                    response = 1;
                }
                catch (Exception)
                {
                    response = -1;
                }

                return response;
            }
        }

        //////////////////////////////////////////////////////////////

        public static PayrollInfoForPDF GeneratePayrollPDFByEmployeeIDANDMonth(int emp_id, string month_year)
        {
            int iDesignationId = 0, iDepartmentId = 0, iBankNameId = 0, iPaymentModeId = 0;
            string strDesignation = "", strDepartment = "", strBankName = "", strPaymentMode = "";
            int per_day_salary = 0;

            DateTime from_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            PayrollInfoForPDF pInfoForPDF = new PayrollInfoForPDF();
            PayrollInfo pItem = new PayrollInfo();
            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

            from_date = DateTime.ParseExact(month_year, "yyyy-MM", CultureInfo.InvariantCulture);
            to_date = new DateTime(from_date.Year, from_date.Month, System.DateTime.DaysInMonth(from_date.Year, from_date.Month), 23, 59, 59);

            //count days in year
            int days_in_year = 0;
            for (int m = 1; m <= 12; m++)
            {
                days_in_year += DateTime.DaysInMonth(DateTime.Now.Year, m);
            }

            using (Context db = new Context())
            {
                try
                {
                    var data_payroll = db.payroll.Where(p => p.EmployeeId == emp_id && (p.SalaryMonthYear >= from_date && p.SalaryMonthYear <= to_date)).FirstOrDefault();
                    if (data_payroll != null)
                    {
                        var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == emp_id).FirstOrDefault();
                        if (data_emp != null)
                        {
                            if (data_emp.designation != null && data_emp.designation.DesignationId > 0 && data_emp.designation.DesignationId.ToString() != "")
                            {
                                iDesignationId = data_emp.designation.DesignationId;
                                strDesignation = db.designation.Where(b => b.DesignationId == iDesignationId).FirstOrDefault().name ?? "";
                            }

                            if (data_emp.department != null && data_emp.department.DepartmentId > 0 && data_emp.department.DepartmentId.ToString() != "")
                            {
                                iDepartmentId = data_emp.department.DepartmentId;
                                strDepartment = db.department.Where(p => p.DepartmentId == iDepartmentId).FirstOrDefault().name ?? "";
                            }

                            iBankNameId = data_payroll.BankNameId;
                            strBankName = db.bank_name.Where(b => b.Id == iBankNameId).FirstOrDefault().BankNameText ?? "";

                            iPaymentModeId = data_payroll.PaymentModeId;
                            strPaymentMode = db.payment_mode.Where(p => p.Id == iPaymentModeId).FirstOrDefault().PaymentModeText ?? "";


                            pItem.EmployeeId = data_payroll.EmployeeId;
                            pItem.SalaryMonthYear = data_payroll.SalaryMonthYear;
                            pItem.SalaryMonthYearText = data_payroll.SalaryMonthYear.ToString("MMM yyyy");
                            pItem.CNIC = data_payroll.CNIC;
                            pItem.NTNNumber = data_payroll.NTNNumber;
                            pItem.EOBINumber = data_payroll.EOBINumber;
                            pItem.SESSINumber = data_payroll.SESSINumber;

                            ///////////////////// Leaves Session and Count /////////////////////////////////
                            dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(data_emp.employee_code);
                            DateTime dtSessionStartDate = dt[0];
                            DateTime dtSessionEndDate = dt[1];
                            string strSessionStartDate = dt[0].ToString("dd MMM yyyy");
                            string strSessionEndDate = dt[1].ToString("dd MMM yyyy");

                            int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                            leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(data_emp.employee_code);
                            int iAvailableSickLeaves = leaves[0];
                            int iAvailableCasualLeaves = leaves[1];
                            int iAvailableAnnualLeaves = leaves[2];
                            int iAvailedSickLeaves = leaves[3];
                            int iAvailedCasualLeaves = leaves[4];
                            int iAvailedAnnualLeaves = leaves[5];
                            int iTotalAvailed = iAvailedSickLeaves + iAvailedCasualLeaves + iAvailedAnnualLeaves;
                            int iTotalAvailable = iAvailableSickLeaves + iAvailableCasualLeaves + iAvailableAnnualLeaves;

                            int[] lastMonthLeaves = new int[3] { 0, 0, 0 };
                            lastMonthLeaves = LeaveApplicationResultSet.getUserLastMonthLeavesByUserCode(data_emp.employee_code, month_year);
                            int iLastMonthAvailableSickLeaves = lastMonthLeaves[0];
                            int iLastMonthAvailableCasualLeaves = lastMonthLeaves[1];
                            int iLastMonthAvailableAnnualLeaves = lastMonthLeaves[2];
                            int iLastMonthAvailed = iLastMonthAvailableSickLeaves + iLastMonthAvailableCasualLeaves + iLastMonthAvailableAnnualLeaves;
                            ///////////////////////////////////////////////////////////////////////////////

                            pItem.BasicPay = data_payroll.BasicPay;
                            pItem.Increment = data_payroll.Increment;
                            per_day_salary = ((data_payroll.BasicPay + data_payroll.Increment) * 12 / days_in_year);

                            pItem.Transport = data_payroll.Transport;
                            pItem.Mobile = data_payroll.Mobile;
                            pItem.Medical = data_payroll.Medical;
                            pItem.Food = data_payroll.Food;
                            pItem.Night = data_payroll.Night;
                            pItem.GroupAllowance = data_payroll.GroupAllowance;
                            pItem.Commission = data_payroll.Commission;
                            pItem.Rent = data_payroll.Rent;
                            pItem.CashAllowance = data_payroll.CashAllowance;
                            pItem.AnnualBonus = data_payroll.AnnualBonus;
                            pItem.LeavesCount = data_payroll.LeavesCount;
                            pItem.LeavesEncash = data_payroll.LeavesEncash;
                            pItem.OvertimeInHours = data_payroll.OvertimeInHours;
                            pItem.OvertimeAmount = data_payroll.OvertimeAmount;

                            pItem.IncomeTax = data_payroll.IncomeTax;
                            pItem.FineExtraAmount = data_payroll.FineExtraAmount;
                            pItem.MobileDeduction = data_payroll.MobileDeduction;

                            if (data_payroll.AbsentsCount == 0 && iTotalAvailed > iTotalAvailable && iLastMonthAvailed > 0)
                            {
                                pItem.AbsentsCount = Math.Abs(iTotalAvailed - iTotalAvailable);
                                pItem.AbsentsAmount = pItem.AbsentsCount * per_day_salary;
                            }
                            else
                            {
                                pItem.AbsentsCount = data_payroll.AbsentsCount;
                                pItem.AbsentsAmount = data_payroll.AbsentsAmount;
                            }

                            pItem.EOBIEmployee = data_payroll.EOBIEmployee;
                            pItem.EOBIEmployer = data_payroll.EOBIEmployer;
                            pItem.SESSIEmployee = data_payroll.SESSIEmployee;
                            pItem.SESSIEmployer = data_payroll.SESSIEmployer;
                            pItem.LoanInstallment = data_payroll.LoanInstallment;
                            pItem.OtherDeduction = data_payroll.OtherDeduction;

                            pItem.GrossSalary = data_payroll.GrossSalary;
                            pItem.TotalDeduction = data_payroll.TotalDeduction;
                            pItem.NetSalary = data_payroll.NetSalary;

                            pItem.PaymentModeId = data_payroll.PaymentModeId;
                            pItem.PaymentModeText = strPaymentMode;
                            pItem.BankNameId = data_payroll.BankNameId;
                            pItem.BankNameText = strBankName;
                            pItem.BankAccTitle = data_payroll.BankAccTitle;
                            pItem.BankAccNo = data_payroll.BankAccNo;

                            pItem.AckStatusId = data_payroll.AckStatusId;
                            pItem.AckStatusText = data_payroll.AckStatusId == 1 ? "Unpaid" : "Paid";
                            pItem.PayStatusId = data_payroll.PayStatusId;
                            pItem.PayStatusText = data_payroll.PayStatusId == 1 ? "Unpaid" : "Paid";

                            pItem.UserHRComments = data_payroll.UserHRComments;
                            pItem.AttachmentFilePath = data_payroll.AttachmentFilePath;
                            pItem.IsFirstMonthText = data_payroll.IsFirstMonthText;

                            pItem.PaymentDateTimeText = data_payroll.PaymentDateTime.ToString("dd-MMM-yyyy");
                            pItem.CreateDateTimeText = data_payroll.CreateDateTime.ToString("dd-MMM-yyyy");
                            pItem.UpdateDateTimeText = data_payroll.UpdateDateTime.ToString("dd-MMM-yyyy");

                            //leaves - data formation
                            pInfoForPDF.LeavesSessionText = strSessionStartDate + " - " + strSessionEndDate;
                            pInfoForPDF.AllocatedLeaves = iAvailableSickLeaves + " Sick, " + iAvailableCasualLeaves + " Casual, " + iAvailableAnnualLeaves + " Annual";
                            pInfoForPDF.AvailedLeaves = iAvailedSickLeaves + " Sick, " + iAvailedCasualLeaves + " Casual, " + iAvailedAnnualLeaves + " Annual";
                            pInfoForPDF.AvailedLeavesLastMonth = iLastMonthAvailableSickLeaves + " Sick, " + iLastMonthAvailableCasualLeaves + " Casual, " + iLastMonthAvailableAnnualLeaves + " Annual";
                            pInfoForPDF.RemainingLeaves = (iAvailableSickLeaves - iAvailedSickLeaves) + " Sick, " + (iAvailableCasualLeaves - iAvailedCasualLeaves) + " Casual, " + (iAvailableAnnualLeaves - iAvailedAnnualLeaves) + " Annual";

                            //other - data formation
                            pInfoForPDF.EmployeeCode = data_emp.employee_code;
                            pInfoForPDF.EmployeeName = data_emp.first_name + " " + data_emp.last_name;
                            pInfoForPDF.Designation = strDesignation;
                            pInfoForPDF.Department = strDepartment;
                            pInfoForPDF.PayrollInformation = pItem;
                            pInfoForPDF.SalaryMonthYearText = data_payroll.SalaryMonthYear.ToString("MMM yyyy");
                            pInfoForPDF.JoiningDateText = data_emp.date_of_joining.Value.ToString("dd MMM yyyy");
                            pInfoForPDF.PaymentDatetimeText = data_payroll.PaymentDateTime.ToString("dd MMM yyyy");
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return pInfoForPDF;
        }

        public static PayrollStatementForPDF GeneratePayrollStatementByMonth(string month_year)
        {
            int iDesignationId = 0, iDepartmentId = 0, iBankNameId = 0, iPaymentModeId = 0;
            int iIncomeTax = 0, iNetAmount = 0;
            string strDesignation = "", strDepartment = "", strBankName = "", strPaymentMode = "";
            int per_day_salary = 0;

            DateTime from_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            PayrollStatementForPDF pStatement = new PayrollStatementForPDF();
            List<PayrollInfoForPDF> listPayrollPDF = new List<PayrollInfoForPDF>();

            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };
            from_date = DateTime.ParseExact(month_year, "yyyy-MM", CultureInfo.InvariantCulture);
            to_date = new DateTime(from_date.Year, from_date.Month, System.DateTime.DaysInMonth(from_date.Year, from_date.Month), 23, 59, 59);

            //count days in year
            int days_in_year = 0;
            for (int m = 1; m <= 12; m++)
            {
                days_in_year += DateTime.DaysInMonth(DateTime.Now.Year, m);
            }

            using (Context db = new Context())
            {
                try
                {
                    var data_payroll = db.payroll.Where(p => (p.SalaryMonthYear >= from_date && p.SalaryMonthYear <= to_date)).ToList();
                    if (data_payroll != null && data_payroll.Count > 0)
                    {
                        foreach (var p in data_payroll)
                        {
                            PayrollInfoForPDF pItemPDF = new PayrollInfoForPDF();
                            PayrollInfo pItem = new PayrollInfo();

                            var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == p.EmployeeId).FirstOrDefault();
                            if (data_emp != null)
                            {
                                if (data_emp.designation != null && data_emp.designation.DesignationId > 0 && data_emp.designation.DesignationId.ToString() != "")
                                {
                                    iDesignationId = data_emp.designation.DesignationId;
                                    strDesignation = db.designation.Where(b => b.DesignationId == iDesignationId).FirstOrDefault().name ?? "";
                                }

                                if (data_emp.department != null && data_emp.department.DepartmentId > 0 && data_emp.department.DepartmentId.ToString() != "")
                                {
                                    iDepartmentId = data_emp.department.DepartmentId;
                                    strDepartment = db.department.Where(d => d.DepartmentId == iDepartmentId).FirstOrDefault().name ?? "";
                                }

                                iBankNameId = p.BankNameId;
                                strBankName = db.bank_name.Where(b => b.Id == iBankNameId).FirstOrDefault().BankNameText ?? "";

                                iPaymentModeId = p.PaymentModeId;
                                strPaymentMode = db.payment_mode.Where(m => m.Id == iPaymentModeId).FirstOrDefault().PaymentModeText ?? "";

                                pItem.EmployeeId = p.EmployeeId;
                                pItem.SalaryMonthYear = p.SalaryMonthYear;
                                pItem.SalaryMonthYearText = p.SalaryMonthYear.ToString("MMM yyyy");
                                pItem.CNIC = p.CNIC;
                                pItem.NTNNumber = p.NTNNumber;
                                pItem.EOBINumber = p.EOBINumber;
                                pItem.SESSINumber = p.SESSINumber;

                                ///////////////////// Leaves Session and Count /////////////////////////////////
                                dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(data_emp.employee_code);
                                DateTime dtSessionStartDate = dt[0];
                                DateTime dtSessionEndDate = dt[1];
                                string strSessionStartDate = dt[0].ToString("dd MMM yyyy");
                                string strSessionEndDate = dt[1].ToString("dd MMM yyyy");

                                int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                                leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(data_emp.employee_code);
                                int iAvailableSickLeaves = leaves[0];
                                int iAvailableCasualLeaves = leaves[1];
                                int iAvailableAnnualLeaves = leaves[2];
                                int iAvailedSickLeaves = leaves[3];
                                int iAvailedCasualLeaves = leaves[4];
                                int iAvailedAnnualLeaves = leaves[5];
                                int iTotalAvailed = iAvailedSickLeaves + iAvailedCasualLeaves + iAvailedAnnualLeaves;
                                int iTotalAvailable = iAvailableSickLeaves + iAvailableCasualLeaves + iAvailableAnnualLeaves;

                                int[] lastMonthLeaves = new int[3] { 0, 0, 0 };
                                lastMonthLeaves = LeaveApplicationResultSet.getUserLastMonthLeavesByUserCode(data_emp.employee_code, month_year);
                                int iLastMonthAvailableSickLeaves = lastMonthLeaves[0];
                                int iLastMonthAvailableCasualLeaves = lastMonthLeaves[1];
                                int iLastMonthAvailableAnnualLeaves = lastMonthLeaves[2];
                                int iLastMonthAvailed = iLastMonthAvailableSickLeaves + iLastMonthAvailableCasualLeaves + iLastMonthAvailableAnnualLeaves;
                                ///////////////////////////////////////////////////////////////////////////////              

                                pItem.BasicPay = p.BasicPay;
                                pItem.Increment = p.Increment;
                                per_day_salary = (p.BasicPay + p.Increment) * 12 / days_in_year;

                                pItem.Transport = p.Transport;
                                pItem.Mobile = p.Mobile;
                                pItem.Medical = p.Medical;
                                pItem.Food = p.Food;
                                pItem.Night = p.Night;
                                pItem.GroupAllowance = p.GroupAllowance;
                                pItem.Commission = p.Commission;
                                pItem.Rent = p.Rent;
                                pItem.CashAllowance = p.CashAllowance;
                                pItem.AnnualBonus = p.AnnualBonus;
                                pItem.LeavesCount = p.LeavesCount;
                                pItem.LeavesEncash = p.LeavesEncash;
                                pItem.OvertimeInHours = p.OvertimeInHours;
                                pItem.OvertimeAmount = p.OvertimeAmount;

                                pItem.IncomeTax = p.IncomeTax;
                                pItem.FineExtraAmount = p.FineExtraAmount;
                                pItem.MobileDeduction = p.MobileDeduction;

                                if (p.AbsentsCount == 0 && iTotalAvailed > iTotalAvailable && iLastMonthAvailed > 0)
                                {
                                    pItem.AbsentsCount = Math.Abs(iTotalAvailed - iTotalAvailable);
                                    pItem.AbsentsAmount = pItem.AbsentsCount * per_day_salary;
                                }
                                else
                                {
                                    pItem.AbsentsCount = p.AbsentsCount;
                                    pItem.AbsentsAmount = p.AbsentsAmount;
                                }

                                pItem.EOBIEmployee = p.EOBIEmployee;
                                pItem.EOBIEmployer = p.EOBIEmployer;
                                pItem.SESSIEmployee = p.SESSIEmployee;
                                pItem.SESSIEmployer = p.SESSIEmployer;
                                pItem.LoanInstallment = p.LoanInstallment;
                                pItem.OtherDeduction = p.OtherDeduction;

                                pItem.GrossSalary = p.GrossSalary;
                                pItem.TotalDeduction = p.TotalDeduction;
                                pItem.NetSalary = p.NetSalary;

                                pItem.PaymentModeId = p.PaymentModeId;
                                pItem.PaymentModeText = strPaymentMode;
                                pItem.BankNameId = p.BankNameId;
                                pItem.BankNameText = strBankName;
                                pItem.BankAccTitle = p.BankAccTitle;
                                pItem.BankAccNo = p.BankAccNo;

                                pItem.AckStatusId = p.AckStatusId;
                                pItem.AckStatusText = p.AckStatusId == 1 ? "Unpaid" : "Paid";
                                pItem.PayStatusId = p.PayStatusId;
                                pItem.PayStatusText = p.PayStatusId == 1 ? "Unpaid" : "Paid";

                                pItem.UserHRComments = p.UserHRComments;
                                pItem.AttachmentFilePath = p.AttachmentFilePath;
                                pItem.IsFirstMonthText = p.IsFirstMonthText;

                                pItem.PaymentDateTimeText = p.PaymentDateTime.ToString("dd-MMM-yyyy");
                                pItem.CreateDateTimeText = p.CreateDateTime.ToString("dd-MMM-yyyy");
                                pItem.UpdateDateTimeText = p.UpdateDateTime.ToString("dd-MMM-yyyy");

                                //leaves - data formation
                                pItemPDF.LeavesSessionText = strSessionStartDate + " - " + strSessionEndDate;
                                pItemPDF.AllocatedLeaves = iAvailableSickLeaves + " Sick, " + iAvailableCasualLeaves + " Casual, " + iAvailableAnnualLeaves + " Annual";
                                pItemPDF.AvailedLeaves = iAvailedSickLeaves + " Sick, " + iAvailedCasualLeaves + " Casual, " + iAvailedAnnualLeaves + " Annual";
                                pItemPDF.AvailedLeavesLastMonth = iLastMonthAvailableSickLeaves + " Sick, " + iLastMonthAvailableCasualLeaves + " Casual, " + iLastMonthAvailableAnnualLeaves + " Annual";
                                pItemPDF.RemainingLeaves = (iAvailableSickLeaves - iAvailedSickLeaves) + " Sick, " + (iAvailableCasualLeaves - iAvailedCasualLeaves) + " Casual, " + (iAvailableAnnualLeaves - iAvailedAnnualLeaves) + " Annual";

                                //other - data formation
                                pItemPDF.EmployeeCode = data_emp.employee_code;
                                pItemPDF.EmployeeName = data_emp.first_name + " " + data_emp.last_name;
                                pItemPDF.Designation = strDesignation;
                                pItemPDF.Department = strDepartment;
                                pItemPDF.PayrollInformation = pItem;
                                pItemPDF.SalaryMonthYearText = p.SalaryMonthYear.ToString("MMM yyyy");
                                pItemPDF.JoiningDateText = data_emp.date_of_joining.Value.ToString("dd MMM yyyy");
                                pItemPDF.PaymentDatetimeText = p.PaymentDateTime.ToString("dd MMM yyyy");

                                iIncomeTax += pItem.IncomeTax;
                                iNetAmount += pItem.NetSalary;

                                listPayrollPDF.Add(pItemPDF);
                            }
                        }

                        pStatement.MonthYear = from_date.ToString("MMM yyyy");
                        pStatement.EmployeesCount = listPayrollPDF.Count;
                        pStatement.PayrollInfoList = listPayrollPDF;
                        pStatement.IncomeTax = iIncomeTax;
                        pStatement.NetAmount = iNetAmount;
                    }
                }
                catch (Exception)
                {
                }
            }

            return pStatement;
        }

        //////////////////////////////////////////////////////////////

        //By Employee
        public static void updatePayrollAckStatus(ViewModels.PayrollStatus toUpdate)
        {
            using (Context db = new Context())
            {
                Payroll toUpdateModel = db.payroll.Find(toUpdate.Id);
                if (toUpdateModel != null)
                {
                    toUpdateModel.AckStatusId = toUpdate.StatusId;
                    toUpdateModel.UpdateDateTime = DateTime.Now;

                    db.SaveChanges();
                }
            }
        }

        //By HR
        public static void updatePayrollPayStatus(ViewModels.PayrollStatus toUpdate)
        {
            using (Context db = new Context())
            {
                Payroll toUpdateModel = db.payroll.Find(toUpdate.Id);
                if (toUpdateModel != null)
                {
                    toUpdateModel.PayStatusId = toUpdate.StatusId;
                    toUpdateModel.UpdateDateTime = DateTime.Now;

                    db.SaveChanges();
                }
            }
        }

        ////////////////////////////// Payroll Amounts Management ///////////////////////////////

        public static List<ViewModels.Designation> getAllDesignations()
        {
            List<ViewModels.Designation> toReturn = new List<ViewModels.Designation>();

            using (Context db = new Context())
            {
                List<DLL.Models.Designation> dNames = null;

                try
                {
                    dNames = db.designation.Where(d => d.active).ToList();
                    if (dNames != null && dNames.Count > 0)
                    {
                        for (int i = 0; i < dNames.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.Designation()
                            {
                                id = dNames[i].DesignationId,
                                name = dNames[i].name
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    dNames = new List<DLL.Models.Designation>();
                }

                return toReturn;
            }
        }

        public static List<ViewModels.Grades> getAllGrades()
        {
            List<ViewModels.Grades> toReturn = new List<ViewModels.Grades>();

            using (Context db = new Context())
            {
                List<DLL.Models.Grade> gName = null;

                try
                {
                    gName = db.grade.Where(g => g.active).ToList();
                    if (gName != null && gName.Count > 0)
                    {
                        for (int i = 0; i < gName.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.Grades()
                            {
                                id = gName[i].GradeId,
                                name = gName[i].name
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    gName = new List<DLL.Models.Grade>();
                }

                return toReturn;
            }
        }

        public static List<ViewModels.PayrollAmountInfo> getAllPayrollAmounts()
        {
            string strDesgText = "", strGradeText = "";
            int iDesgId = 0, iGradeId = 0;
            List<PayrollAmount> pAmount = null;
            List<ViewModels.PayrollAmountInfo> toReturn = new List<ViewModels.PayrollAmountInfo>();

            using (Context db = new Context())
            {
                try
                {
                    pAmount = db.payroll_amount.OrderByDescending(o => o.Id).ToList();
                    if (pAmount != null && pAmount.Count > 0)
                    {
                        for (int i = 0; i < pAmount.Count(); i++)
                        {
                            iDesgId = pAmount[i].DesignationId;
                            strDesgText = db.designation.Where(b => b.DesignationId == iDesgId).FirstOrDefault().name ?? "";

                            iGradeId = pAmount[i].GradeId;
                            strGradeText = db.grade.Where(p => p.GradeId == iGradeId).FirstOrDefault().name ?? "";

                            toReturn.Add(new ViewModels.PayrollAmountInfo()
                            {
                                Id = pAmount[i].Id,
                                DesignationId = pAmount[i].DesignationId,
                                DesignationText = strDesgText,
                                GradeId = pAmount[i].GradeId,
                                GradeText = strGradeText,
                                BasicPay = pAmount[i].BasicPay,
                                Increment = pAmount[i].Increment,
                                Transport = pAmount[i].Transport,
                                Mobile = pAmount[i].Mobile,
                                Medical = pAmount[i].Medical,
                                Food = pAmount[i].Food,
                                Night = pAmount[i].Night,
                                GroupAllowance = pAmount[i].GroupAllowance,
                                Commission = pAmount[i].Commission,
                                Rent = pAmount[i].Rent,
                                CashAllowance = pAmount[i].CashAllowance,
                                CreateDateTimeText = pAmount[i].CreateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                UpdateDateTimeText = pAmount[i].UpdateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                //AttachmentFilePath = (payroll[i].AttachmentFilePath != null && payroll[i].AttachmentFilePath != "") ? "<span data-row='" + payroll[i].Id + "'>" +
                                //        "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + payroll[i].AttachmentFilePath + "'));\">View</a>" +
                                //    "</span>" : "--",
                                actions =
                                    "<span data-row='" + pAmount[i].Id + "'>" +
                                        "<a href=\"javascript:void(editPayrollAmount(" + pAmount[i].Id + "," + pAmount[i].DesignationId + ",'" + strDesgText + "'," + pAmount[i].GradeId + ",'" + strGradeText + "'," + pAmount[i].BasicPay + "," + pAmount[i].Increment + "," + pAmount[i].Transport + "," + pAmount[i].Mobile + "," + pAmount[i].Medical + "," + pAmount[i].CashAllowance + "," + pAmount[i].Commission + "," + pAmount[i].Food + "," + pAmount[i].Night + "," + pAmount[i].Rent + "," + pAmount[i].GroupAllowance + "));\">Edit</a>" +
                                    "</span>"
                                //actions =
                                //    "<span data-row='" + pAmount[i].Id + "'>" +
                                //        "<a href=\"javascript:void(editPayrollAmount(" + pAmount[i].Id + "," + pAmount[i].DesignationId + ",'" + strDesgText + "'," + pAmount[i].GradeId + ",'" + strGradeText + "'," + pAmount[i].BasicPay + "," + pAmount[i].Increment + "," + pAmount[i].Transport + "," + pAmount[i].Mobile + "," + pAmount[i].Medical + "," + pAmount[i].CashAllowance + "," + pAmount[i].Commission + "," + pAmount[i].Food + "," + pAmount[i].Night + "," + pAmount[i].Rent + "," + pAmount[i].GroupAllowance + "));\">Edit</a>" +
                                //        "<span> / </span>" +
                                //        "<a href=\"javascript:void(deletePayrollAmount(" + pAmount[i].Id + "));\">Delete</a>" +
                                //    "</span>"
                                //actions =
                                //    "<span data-row='" + pAmount[i].Id + "'>" +
                                //        "<a href=\"/HR/PayrollsManagement/PayrollSetting?salary_month=" + pAmount[i].DesignationId + "\" target=\"_blank\">Edit Payroll Amount</a>" +
                                //    "</span>"
                            });
                        }
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    pAmount = new List<PayrollAmount>();
                }

                return toReturn;
            }
        }

        public static PayrollAmountInfo IsPayrollAmountExists(int desgId, int grdId)
        {
            PayrollAmountInfo p = new PayrollAmountInfo();

            try
            {
                using (Context db = new Context())
                {
                    var data_pamount = db.payroll_amount.Where(a => a.DesignationId == desgId && a.GradeId == grdId).FirstOrDefault();
                    if (data_pamount != null)
                    {
                        p.Id = data_pamount.Id;
                        p.DesignationId = data_pamount.DesignationId;
                        p.GradeId = data_pamount.GradeId;
                        p.BasicPay = data_pamount.BasicPay;
                        p.Increment = data_pamount.Increment;
                        p.Transport = data_pamount.Transport;
                        p.Mobile = data_pamount.Mobile;
                        p.Medical = data_pamount.Medical;
                        p.CashAllowance = data_pamount.CashAllowance;
                        p.Commission = data_pamount.Commission;
                        p.Food = data_pamount.Food;
                        p.Night = data_pamount.Night;
                        p.Rent = data_pamount.Rent;
                        p.GroupAllowance = data_pamount.GroupAllowance;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return p;
        }

        public static int AddNewPayrollAmount(PayrollAmountInfo pinfo)
        {
            int response = 0;

            try
            {
                using (Context db = new Context())
                {
                    //add new payroll record
                    PayrollAmount payroll = new PayrollAmount();

                    payroll.DesignationId = pinfo.DesignationId;
                    payroll.GradeId = pinfo.GradeId;

                    payroll.BasicPay = pinfo.BasicPay;
                    payroll.Increment = pinfo.Increment;
                    payroll.Transport = pinfo.Transport;
                    payroll.Mobile = pinfo.Mobile;
                    payroll.Medical = pinfo.Medical;
                    payroll.Food = pinfo.Food;
                    payroll.Night = pinfo.Night;
                    payroll.GroupAllowance = pinfo.GroupAllowance;
                    payroll.Commission = pinfo.Commission;
                    payroll.Rent = pinfo.Rent;
                    payroll.CashAllowance = pinfo.CashAllowance;

                    payroll.CreateDateTime = pinfo.CreateDateTime;
                    payroll.UpdateDateTime = pinfo.UpdateDateTime;

                    db.payroll_amount.Add(payroll);
                    db.SaveChanges();

                    response = 1;
                }
            }
            catch (Exception ex)
            {
                response = -1;
                //throw ex;
            }

            return response;
        }

        public static void updatePayrollAmount(ViewModels.PayrollAmountInfo pinfo)
        {
            using (Context db = new Context())
            {
                PayrollAmount payroll = db.payroll_amount.Find(pinfo.Id);
                if (payroll != null)
                {
                    payroll.DesignationId = pinfo.DesignationId;
                    payroll.GradeId = pinfo.GradeId;

                    payroll.BasicPay = pinfo.BasicPay;
                    payroll.Increment = pinfo.Increment;
                    payroll.Transport = pinfo.Transport;
                    payroll.Mobile = pinfo.Mobile;
                    payroll.Medical = pinfo.Medical;
                    payroll.Food = pinfo.Food;
                    payroll.Night = pinfo.Night;
                    payroll.GroupAllowance = pinfo.GroupAllowance;
                    payroll.Commission = pinfo.Commission;
                    payroll.Rent = pinfo.Rent;
                    payroll.CashAllowance = pinfo.CashAllowance;

                    payroll.UpdateDateTime = DateTime.Now;

                    db.SaveChanges();
                }
            }
        }

        public static ViewModels.PayrollAmountInfo removePayrollAmount(ViewModels.PayrollAmountInfo toRemove)
        {

            using (Context db = new Context())
            {
                PayrollAmount toRemoveModel = db.payroll_amount.Find(toRemove.Id);

                db.payroll_amount.Remove(toRemoveModel);

                //toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove;
            }

        }

        /////////////////////////////////////////////////////////////////////

        public static List<PayrollAmountInfo> GetResult(string search, string sortOrder, int start, int length, List<PayrollAmountInfo> dtResult)
        {
            return FilterResult(search, dtResult).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<PayrollAmountInfo> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<PayrollAmountInfo> FilterResult(string search, List<PayrollAmountInfo> dtResult)
        {
            IQueryable<PayrollAmountInfo> results = dtResult.AsQueryable();

            //results = results.Where(p =>
            //search == null ||
            //(
            //p.FromDate != null));

            results = results.Where(p =>
                    (
                        search == null || search.Equals("") ||
                        (
                                p.Id.ToString().ToLower().Contains(search.ToLower())
                            || (p.DesignationText != null && p.DesignationText.ToLower().Contains(search.ToLower()))
                            || (p.GradeText != null && p.GradeText.ToLower().Contains(search.ToLower()))
                        // || EntityFunctions.TruncateTime(p.FromDate).ToString().Contains(search.ToLower()) || EntityFunctions.TruncateTime(p.ToDate).ToString().Contains(search.ToLower())


                        )
                    //&&
                    //  (depart_id.Equals(-1) || p.ReasonDetail.department.DepartmentId.Equals(depart_id))
                    //       &&
                    //  (des_id.Equals(-1) || ca.employee.designation.DesignationId.Equals(des_id))
                    )

                    );

            return results;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
