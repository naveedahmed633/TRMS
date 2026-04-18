using DLL.Models;
using System;
using System.Collections;
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

    public class LoanInfo
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime LoanAllocatedDate { get; set; }
        public string LoanAllocatedDateText { get; set; }

        public int LoanTypeId { get; set; }
        public string LoanTypeText { get; set; }
        public int LoanAmount { get; set; }
        public int InstallmentNumbers { get; set; }
        public int InstallmentAmount { get; set; }
        public int DeductableAmount { get; set; }
        public int BalanceAmount { get; set; }
        public int LoanStatusId { get; set; }
        public string LoanStatusText { get; set; }

        public string Remarks { get; set; }
        public string AttachmentFilePath { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }

        public string actions { get; set; }
    }

    public class LoanInfoForPDF
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string JoiningDateText { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public LoanInfo LoanInformation { get; set; }
        public DateTime LoanAllocatedDate { get; set; }
        public string LoanAllocatedDateText { get; set; }

        public int LoanTypeId { get; set; }
        public int LoanAmount { get; set; }
        public int InstallmentNumbers { get; set; }
        public int InstallmentAmount { get; set; }
        public int DeductableAmount { get; set; }
        public int BalanceAmount { get; set; }
        public int LoanStatusId { get; set; }

        public string Remarks { get; set; }
        public string AttachmentFilePath { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }
    }

    public class LoanStatementForPDF
    {
        public string MonthYear { get; set; }
        public List<LoanInfo> LoanInfoList { get; set; }
        public int DeductableAmount { get; set; }
    }

    public class LoanStatementForEmployeePDF
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public List<LoanInfo> LoanInfoList { get; set; }
        public int LoanAmountOpen { get; set; }
        public int LoanAmountClosed { get; set; }
        public int DeductableAmountOpen { get; set; }
        public int DeductableAmountClosed { get; set; }
        public int BalanceAmountOpen { get; set; }
        public int BalanceAmountClosed { get; set; }
    }


    public class LoanStatus
    {
        [Key]
        public int Id { get; set; }
        public int LoanStatusId { get; set; }
    }

    public class LoanResultSet
    {
        public static List<ViewModels.LoanInfo> getAllLoanApplications()
        {
            string strLoanType = "", strLoanStatus = "", strEditMonthStatus = "";
            int iEmployeeId = 0, iLoanTypeId = 0, iLoanStatusId = 0;
            List<Loan> loan = null;
            List<ViewModels.LoanInfo> toReturn = new List<ViewModels.LoanInfo>();
            ArrayList isEmpExists = new ArrayList();
            using (Context db = new Context())
            {
                try
                {
                    loan = db.loan.OrderByDescending(o => o.LoanAllocatedDate).ToList();
                    if (loan != null && loan.Count > 0)
                    {
                        for (int i = 0; i < loan.Count(); i++)
                        {
                            iEmployeeId = loan[i].EmployeeId;

                            var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == iEmployeeId).FirstOrDefault();
                            if (data_emp != null)
                            {
                                iLoanTypeId = loan[i].LoanTypeId;
                                strLoanType = db.loan_type.Where(b => b.Id == iLoanTypeId).FirstOrDefault().LoanTypeText ?? "";

                                iLoanStatusId = loan[i].LoanStatusId;
                                strLoanStatus = db.loan_status_type.Where(p => p.Id == iLoanStatusId).FirstOrDefault().LoanStatusTypeText ?? "";

                                //only allow to edit the latest Loan record, NOT previous
                                if (isEmpExists.Contains(iEmployeeId))
                                {
                                    strEditMonthStatus = "-";
                                }
                                else
                                {
                                    //not older than 2 month
                                    if (DateTime.Now.AddMonths(-3) < loan[i].LoanAllocatedDate)
                                    {
                                        strEditMonthStatus =
                                          "<span data-row='" + loan[i].Id + "'>" +
                                              "<a href=\"javascript:void(editLoanApplication(" + loan[i].Id + "," + loan[i].EmployeeId + ",'" + data_emp.employee_code + "','" + (data_emp.first_name + " " + data_emp.last_name) + "','" + loan[i].LoanAllocatedDate.ToString("dd-MM-yyyy") + "'," + loan[i].LoanAmount + "," + loan[i].InstallmentNumbers + "," + loan[i].InstallmentAmount + "," + loan[i].DeductableAmount + "," + loan[i].BalanceAmount + "," + loan[i].LoanStatusId + "));\">Edit</a>" +
                                          "</span>";
                                    }
                                    else
                                    {
                                        strEditMonthStatus = "--";
                                    }

                                    isEmpExists.Add(iEmployeeId);
                                }

                                toReturn.Add(new ViewModels.LoanInfo()
                                {
                                    Id = loan[i].Id,
                                    EmployeeId = loan[i].EmployeeId,
                                    EmployeeCode = data_emp.employee_code,
                                    EmployeeName = data_emp.first_name + " " + data_emp.last_name,

                                    LoanAllocatedDate = loan[i].LoanAllocatedDate,
                                    LoanAllocatedDateText = loan[i].LoanAllocatedDate.ToString("MMM yyyy"),
                                    LoanTypeId = loan[i].LoanTypeId,
                                    LoanTypeText = strLoanType,
                                    LoanAmount = loan[i].LoanAmount,
                                    InstallmentNumbers = loan[i].InstallmentNumbers,
                                    InstallmentAmount = loan[i].InstallmentAmount,
                                    DeductableAmount = loan[i].DeductableAmount,
                                    BalanceAmount = loan[i].BalanceAmount,

                                    LoanStatusId = loan[i].LoanStatusId,
                                    LoanStatusText = strLoanStatus,
                                    Remarks = loan[i].Remarks,
                                    CreateDateTimeText = loan[i].CreateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    UpdateDateTimeText = loan[i].UpdateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    AttachmentFilePath = (loan[i].AttachmentFilePath != null && loan[i].AttachmentFilePath != "") ? "<span data-row='" + loan[i].Id + "'>" +
                                            "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + loan[i].AttachmentFilePath + "'));\">View</a>" +
                                        "</span>" : "--",
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"javascript:void(editLoanApplication(" + loan[i].Id + ",'" + data_emp.employee_code + "','" + (data_emp.first_name + " " + data_emp.last_name) + "'," + loan[i].LoanAmount + "," + loan[i].InstallmentNumbers + "," + loan[i].InstallmentAmount + "," + loan[i].DeductableAmount + "," + loan[i].BalanceAmount + "));\">Edit</a>" +
                                    //        "<span> / </span>" +
                                    //        "<a href=\"javascript:void(deleteLoanApplication(" + loan[i].Id + "));\">Delete</a>" +
                                    //    "</span>",
                                    actions = strEditMonthStatus
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"/HR/LoansManagement/LoanSetting?user_code=" + data_emp.employee_code + "&salary_month=" + loan[i].LoanAllocatedDate.ToString("dd-MM-yyyy") + "\" target=\"_blank\">Edit Loan</a>" +
                                    //    "</span>"
                                });
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    loan = new List<Loan>();
                }

                return toReturn;
            }
        }

        public static List<ViewModels.LoanInfo> getLoanByEmployeeCode(string emp_code)
        {
            string strLoanType = "", strLoanStatus = "";
            int iLoanTypeId = 0, iLoanStatusId = 0;
            List<Loan> loan = null;
            List<ViewModels.LoanInfo> toReturn = new List<ViewModels.LoanInfo>();

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active == true && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        loan = db.loan.Where(m => m.EmployeeId == data_emp.EmployeeId).OrderByDescending(o => o.LoanAllocatedDate).ToList();
                        if (loan != null && loan.Count > 0)
                        {
                            for (int i = 0; i < loan.Count(); i++)
                            {
                                iLoanTypeId = loan[i].LoanTypeId;
                                strLoanType = db.loan_type.Where(b => b.Id == iLoanTypeId).FirstOrDefault().LoanTypeText ?? "";

                                iLoanStatusId = loan[i].LoanStatusId;
                                strLoanStatus = db.loan_status_type.Where(p => p.Id == iLoanStatusId).FirstOrDefault().LoanStatusTypeText ?? "";

                                toReturn.Add(new ViewModels.LoanInfo()
                                {
                                    Id = loan[i].Id,
                                    EmployeeId = loan[i].EmployeeId,
                                    EmployeeCode = data_emp.employee_code,
                                    EmployeeName = data_emp.first_name + " " + data_emp.last_name,

                                    LoanAllocatedDate = loan[i].LoanAllocatedDate,
                                    LoanAllocatedDateText = loan[i].LoanAllocatedDate.ToString("MMM yyyy"),
                                    LoanTypeId = loan[i].LoanTypeId,
                                    LoanTypeText = strLoanType,
                                    LoanAmount = loan[i].LoanAmount,
                                    InstallmentNumbers = loan[i].InstallmentNumbers,
                                    InstallmentAmount = loan[i].InstallmentAmount,
                                    DeductableAmount = loan[i].DeductableAmount,
                                    BalanceAmount = loan[i].BalanceAmount,

                                    LoanStatusId = loan[i].LoanStatusId,
                                    LoanStatusText = strLoanStatus,
                                    Remarks = loan[i].Remarks,
                                    CreateDateTimeText = loan[i].CreateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    UpdateDateTimeText = loan[i].UpdateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    AttachmentFilePath = (loan[i].AttachmentFilePath != null && loan[i].AttachmentFilePath != "") ? "<span data-row='" + loan[i].Id + "'>" +
                                            "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + loan[i].AttachmentFilePath + "'));\">View</a>" +
                                        "</span>" : "--",
                                    //actions =
                                    //    "<span data-row='" + Loan[i].Id + "'>" +
                                    //        "<a href=\"javascript:void(editLoan(" + Loan[i].Id + "," + Loan[i].EmployeeId + ",'" + Loan[i].PaymentModeId + "','" + Loan[i].PayStatusId + "'));\">Edit</a>" +
                                    //        "<span> / </span>" +
                                    //        "<a href=\"javascript:void(deleteLoan(" + Loan[i].Id + "));\">Delete</a>" +
                                    //    "</span>",
                                    actions =
                                        "<span data-row='" + loan[i].Id + "'>" +
                                            "<a href=\"/HR/LoansManagement/LoanSetting?user_code=" + data_emp.employee_code + "&salary_month=" + loan[i].LoanAllocatedDate.ToString("dd-MM-yyyy") + "\" target=\"_blank\">Edit Loan</a>" +
                                        "</span>"
                                });
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    loan = new List<Loan>();
                }

                return toReturn;
            }
        }

        public static ViewModels.LoanStatementForPDF getLoanStatementByMonth(string month_year)
        {
            string strLoanType = "", strLoanStatus = "";
            int iEmployeeId = 0, iLoanTypeId = 0, iLoanStatusId = 0, deductable_amount = 0;
            List<Loan> loan = null;
            LoanStatementForPDF lStatement = new LoanStatementForPDF();
            List<ViewModels.LoanInfo> toReturn = new List<ViewModels.LoanInfo>();
            DateTime from_date = DateTime.Now, to_date = DateTime.Now;

            using (Context db = new Context())
            {
                try
                {
                    from_date = DateTime.ParseExact(month_year, "yyyy-MM", CultureInfo.InvariantCulture);
                    to_date = new DateTime(from_date.Year, from_date.Month, System.DateTime.DaysInMonth(from_date.Year, from_date.Month), 23, 59, 59);

                    loan = db.loan.Where(l => l.LoanAllocatedDate >= from_date && l.LoanAllocatedDate <= to_date).OrderBy(o => o.EmployeeId).ToList();
                    if (loan != null && loan.Count > 0)
                    {
                        for (int i = 0; i < loan.Count(); i++)
                        {
                            iEmployeeId = loan[i].EmployeeId;

                            var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == iEmployeeId).FirstOrDefault();
                            if (data_emp != null)
                            {
                                iLoanTypeId = loan[i].LoanTypeId;
                                strLoanType = db.loan_type.Where(b => b.Id == iLoanTypeId).FirstOrDefault().LoanTypeText ?? "";

                                iLoanStatusId = loan[i].LoanStatusId;
                                strLoanStatus = db.loan_status_type.Where(p => p.Id == iLoanStatusId).FirstOrDefault().LoanStatusTypeText ?? "";

                                if (loan[i].DeductableAmount > 0)
                                {
                                    deductable_amount += loan[i].DeductableAmount;
                                }
                                else
                                {
                                    deductable_amount += loan[i].InstallmentAmount;
                                }

                                toReturn.Add(new ViewModels.LoanInfo()
                                {
                                    Id = loan[i].Id,
                                    EmployeeId = loan[i].EmployeeId,
                                    EmployeeCode = data_emp.employee_code,
                                    EmployeeName = data_emp.first_name + " " + data_emp.last_name,

                                    LoanAllocatedDate = loan[i].LoanAllocatedDate,
                                    LoanAllocatedDateText = loan[i].LoanAllocatedDate.ToString("MMM yyyy"),
                                    LoanTypeId = loan[i].LoanTypeId,
                                    LoanTypeText = strLoanType,
                                    LoanAmount = loan[i].LoanAmount,
                                    InstallmentNumbers = loan[i].InstallmentNumbers,
                                    InstallmentAmount = loan[i].InstallmentAmount,
                                    DeductableAmount = loan[i].DeductableAmount,
                                    BalanceAmount = loan[i].BalanceAmount,

                                    LoanStatusId = loan[i].LoanStatusId,
                                    LoanStatusText = strLoanStatus,
                                    Remarks = loan[i].Remarks,
                                    CreateDateTimeText = loan[i].CreateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    UpdateDateTimeText = loan[i].UpdateDateTime.ToString("dd-MMM-yyyy hh:mm tt")
                                    //AttachmentFilePath = (loan[i].AttachmentFilePath != null && loan[i].AttachmentFilePath != "") ? "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + loan[i].AttachmentFilePath + "'));\">View</a>" +
                                    //    "</span>" : "--",
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"javascript:void(editLoanApplication(" + loan[i].Id + ",'" + data_emp.employee_code + "','" + (data_emp.first_name + " " + data_emp.last_name) + "'," + loan[i].LoanAmount + "," + loan[i].InstallmentNumbers + "," + loan[i].InstallmentAmount + "," + loan[i].DeductableAmount + "," + loan[i].BalanceAmount + "));\">Edit</a>" +
                                    //        "<span> / </span>" +
                                    //        "<a href=\"javascript:void(deleteLoanApplication(" + loan[i].Id + "));\">Delete</a>" +
                                    //    "</span>",
                                    //actions = strEditMonthStatus
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"/HR/LoansManagement/LoanSetting?user_code=" + data_emp.employee_code + "&salary_month=" + loan[i].LoanAllocatedDate.ToString("dd-MM-yyyy") + "\" target=\"_blank\">Edit Loan</a>" +
                                    //    "</span>"
                                });
                            }
                        }

                        lStatement.MonthYear = from_date.ToString("MMM yyyy");
                        lStatement.LoanInfoList = toReturn;
                        lStatement.DeductableAmount = deductable_amount;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    loan = new List<Loan>();
                }

                return lStatement;
            }
        }

        public static ViewModels.LoanStatementForEmployeePDF getLoanStatementByEmployee(int emp_id)
        {
            string strLoanType = "", strLoanStatus = "", strEmployeeCode = "", strEmployeeName = "", strLoanCode = "", strLoanCounter = "";
            int cLoan = 0, cLoanCount = 0, iEmployeeId = 0, iLoanTypeId = 0, iLoanStatusId = 0;
            int iLoanAmountOpen = 0, iLoanAmountClosed = 0, iDeductableAmountOpen = 0, iDeductableAmountClosed = 0, iBalanceAmountOpen = 0, iBalanceAmountClosed = 0;
            List<Loan> loan = null;
            LoanStatementForEmployeePDF lStatement = new LoanStatementForEmployeePDF();
            List<ViewModels.LoanInfo> toReturn = new List<ViewModels.LoanInfo>();
            ArrayList loanCount = new ArrayList();

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == emp_id).FirstOrDefault();
                    if (data_emp != null)
                    {
                        strEmployeeCode = data_emp.employee_code;

                        loan = db.loan.Where(l => l.EmployeeId == data_emp.EmployeeId).OrderBy(o => o.CreateDateTime).OrderBy(o => o.LoanAmount).OrderBy(o => o.Id).ToList();
                        if (loan != null && loan.Count > 0)
                        {
                            for (int i = 0; i < loan.Count(); i++)
                            {
                                if (!loanCount.Contains(loan[i].CreateDateTime))
                                {
                                    cLoan++; cLoanCount = 1;
                                    strLoanCode = data_emp.employee_code + "-" + loan[i].CreateDateTime.ToString("MMMyyyy") + "-" + (cLoan < 10 ? ("0" + cLoan) : cLoan.ToString());

                                    loanCount.Add(loan[i].CreateDateTime);

                                    if (loan[i].LoanStatusId == 1)
                                    {
                                        iLoanAmountOpen += loan[i].LoanAmount;
                                        iBalanceAmountOpen += loan[i].BalanceAmount;
                                    }
                                    else
                                    {
                                        iLoanAmountClosed += loan[i].LoanAmount;
                                        iBalanceAmountClosed += loan[i].BalanceAmount;
                                    }
                                }

                                strLoanCounter = cLoanCount + "/" + loan[i].InstallmentNumbers;
                                cLoanCount++;

                                iEmployeeId = loan[i].EmployeeId;

                                iLoanTypeId = loan[i].LoanTypeId;
                                strLoanType = db.loan_type.Where(b => b.Id == iLoanTypeId).FirstOrDefault().LoanTypeText ?? "";

                                iLoanStatusId = loan[i].LoanStatusId;
                                strLoanStatus = db.loan_status_type.Where(p => p.Id == iLoanStatusId).FirstOrDefault().LoanStatusTypeText ?? "";

                                if (loan[i].LoanStatusId == 1)
                                {
                                    if (loan[i].DeductableAmount > 0)
                                    {
                                        iDeductableAmountOpen += loan[i].DeductableAmount;
                                    }
                                    else
                                    {
                                        iDeductableAmountOpen += loan[i].InstallmentAmount;
                                    }
                                }
                                else
                                {
                                    if (loan[i].DeductableAmount > 0)
                                    {
                                        iDeductableAmountClosed += loan[i].DeductableAmount;
                                    }
                                    else
                                    {
                                        iDeductableAmountClosed += loan[i].InstallmentAmount;
                                    }
                                }

                                strEmployeeName = data_emp.first_name + " " + data_emp.last_name;

                                toReturn.Add(new ViewModels.LoanInfo()
                                {
                                    Id = loan[i].Id,
                                    EmployeeId = loan[i].EmployeeId,
                                    EmployeeCode = data_emp.employee_code,
                                    EmployeeName = strEmployeeName,

                                    LoanAllocatedDate = loan[i].LoanAllocatedDate,
                                    LoanAllocatedDateText = loan[i].LoanAllocatedDate.ToString("MMM yyyy"),
                                    LoanTypeId = loan[i].LoanTypeId,
                                    LoanTypeText = strLoanType,
                                    LoanAmount = loan[i].LoanAmount,
                                    InstallmentNumbers = loan[i].InstallmentNumbers,
                                    InstallmentAmount = loan[i].InstallmentAmount,
                                    DeductableAmount = loan[i].DeductableAmount == 0 ? loan[i].InstallmentAmount : loan[i].DeductableAmount,
                                    BalanceAmount = loan[i].BalanceAmount,

                                    LoanStatusId = loan[i].LoanStatusId,
                                    LoanStatusText = strLoanStatus,
                                    Remarks = strLoanCode,
                                    AttachmentFilePath = strLoanCounter,
                                    CreateDateTimeText = loan[i].CreateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    UpdateDateTimeText = loan[i].UpdateDateTime.ToString("dd-MMM-yyyy hh:mm tt")
                                    //AttachmentFilePath = (loan[i].AttachmentFilePath != null && loan[i].AttachmentFilePath != "") ? "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + loan[i].AttachmentFilePath + "'));\">View</a>" +
                                    //    "</span>" : "--",
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"javascript:void(editLoanApplication(" + loan[i].Id + ",'" + data_emp.employee_code + "','" + (data_emp.first_name + " " + data_emp.last_name) + "'," + loan[i].LoanAmount + "," + loan[i].InstallmentNumbers + "," + loan[i].InstallmentAmount + "," + loan[i].DeductableAmount + "," + loan[i].BalanceAmount + "));\">Edit</a>" +
                                    //        "<span> / </span>" +
                                    //        "<a href=\"javascript:void(deleteLoanApplication(" + loan[i].Id + "));\">Delete</a>" +
                                    //    "</span>",
                                    //actions = strEditMonthStatus
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"/HR/LoansManagement/LoanSetting?user_code=" + data_emp.employee_code + "&salary_month=" + loan[i].LoanAllocatedDate.ToString("dd-MM-yyyy") + "\" target=\"_blank\">Edit Loan</a>" +
                                    //    "</span>"
                                });
                            }
                        }

                        lStatement.EmployeeCode = strEmployeeCode;
                        lStatement.EmployeeName = strEmployeeName;
                        lStatement.LoanInfoList = toReturn;
                        lStatement.LoanAmountOpen = iLoanAmountOpen;
                        lStatement.LoanAmountClosed = iLoanAmountClosed;
                        lStatement.DeductableAmountOpen = iDeductableAmountOpen;
                        lStatement.DeductableAmountClosed = iDeductableAmountClosed;
                        lStatement.BalanceAmountOpen = iBalanceAmountOpen;
                        lStatement.BalanceAmountClosed = iBalanceAmountClosed;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    loan = new List<Loan>();
                }
            }

            return lStatement;
        }

        public static ViewModels.LoanStatementForEmployeePDF getLoanStatementByDate(string from_date, string to_date)
        {
            string strLoanType = "", strLoanStatus = "", strEmployeeCode = "", strEmployeeName = "", strLoanCount = "";
            int cLoan = 0, iEmployeeId = 0, iLoanTypeId = 0, iLoanStatusId = 0;
            int lEmpId = 0, iLoanAmountOpen = 0, iLoanAmountClosed = 0, iDeductableAmountOpen = 0, iDeductableAmountClosed = 0, iBalanceAmountOpen = 0, iBalanceAmountClosed = 0;
            List<Loan> loan = null;
            LoanStatementForEmployeePDF lStatement = new LoanStatementForEmployeePDF();
            List<ViewModels.LoanInfo> toReturn = new List<ViewModels.LoanInfo>();
            ArrayList loanCount = new ArrayList();
            DateTime dtFromDate = DateTime.Now, dtToDate = DateTime.Now;

            using (Context db = new Context())
            {
                try
                {
                    dtFromDate = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    dtToDate = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    loan = db.loan.Where(l => l.LoanAllocatedDate >= dtFromDate && l.LoanAllocatedDate <= dtToDate).ToList(); //.OrderByDescending(o => o.LoanAllocatedDate).ToList();
                    if (loan != null && loan.Count > 0)
                    {
                        for (int i = 0; i < loan.Count(); i++)
                        {
                            lEmpId = loan[i].EmployeeId;

                            var data_emp = db.employee.Where(e => e.active == true && e.EmployeeId == lEmpId).FirstOrDefault();
                            if (data_emp != null)
                            {
                                strEmployeeCode = data_emp.employee_code;

                                if (!loanCount.Contains(loan[i].CreateDateTime))
                                {
                                    cLoan++;
                                    strLoanCount = "EC-" + (cLoan < 10 ? ("0" + cLoan) : cLoan.ToString()) + "-" + loan[i].CreateDateTime.ToString("MMMyyyy");
                                    loanCount.Add(loan[i].CreateDateTime);

                                    if (loan[i].LoanStatusId == 1)
                                    {
                                        iLoanAmountOpen += loan[i].LoanAmount;
                                        iBalanceAmountOpen += loan[i].BalanceAmount;
                                    }
                                    else
                                    {
                                        iLoanAmountClosed += loan[i].LoanAmount;
                                        iBalanceAmountClosed += loan[i].BalanceAmount;
                                    }
                                }
                                else
                                {
                                    //strLoanCount = "EC-01" + "-" + loan[i].CreateDateTime.ToString("MMMyyyy");
                                }

                                iEmployeeId = loan[i].EmployeeId;

                                iLoanTypeId = loan[i].LoanTypeId;
                                strLoanType = db.loan_type.Where(b => b.Id == iLoanTypeId).FirstOrDefault().LoanTypeText ?? "";

                                iLoanStatusId = loan[i].LoanStatusId;
                                strLoanStatus = db.loan_status_type.Where(p => p.Id == iLoanStatusId).FirstOrDefault().LoanStatusTypeText ?? "";

                                if (loan[i].LoanStatusId == 1)
                                {
                                    if (loan[i].DeductableAmount > 0)
                                    {
                                        iDeductableAmountOpen += loan[i].DeductableAmount;
                                    }
                                    else
                                    {
                                        iDeductableAmountOpen += loan[i].InstallmentAmount;
                                    }
                                }
                                else
                                {
                                    if (loan[i].DeductableAmount > 0)
                                    {
                                        iDeductableAmountClosed += loan[i].DeductableAmount;
                                    }
                                    else
                                    {
                                        iDeductableAmountClosed += loan[i].InstallmentAmount;
                                    }
                                }

                                strEmployeeName = data_emp.first_name + " " + data_emp.last_name;

                                toReturn.Add(new ViewModels.LoanInfo()
                                {
                                    Id = loan[i].Id,
                                    EmployeeId = loan[i].EmployeeId,
                                    EmployeeCode = data_emp.employee_code,
                                    EmployeeName = strEmployeeName,

                                    LoanAllocatedDate = loan[i].LoanAllocatedDate,
                                    LoanAllocatedDateText = loan[i].LoanAllocatedDate.ToString("MMM yyyy"),
                                    LoanTypeId = loan[i].LoanTypeId,
                                    LoanTypeText = strLoanType,
                                    LoanAmount = loan[i].LoanAmount,
                                    InstallmentNumbers = loan[i].InstallmentNumbers,
                                    InstallmentAmount = loan[i].InstallmentAmount,
                                    DeductableAmount = loan[i].DeductableAmount,
                                    BalanceAmount = loan[i].BalanceAmount,

                                    LoanStatusId = loan[i].LoanStatusId,
                                    LoanStatusText = strLoanStatus,
                                    Remarks = strLoanCount,
                                    CreateDateTimeText = loan[i].CreateDateTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                    UpdateDateTimeText = loan[i].UpdateDateTime.ToString("dd-MMM-yyyy hh:mm tt")
                                    //AttachmentFilePath = (loan[i].AttachmentFilePath != null && loan[i].AttachmentFilePath != "") ? "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + loan[i].AttachmentFilePath + "'));\">View</a>" +
                                    //    "</span>" : "--",
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"javascript:void(editLoanApplication(" + loan[i].Id + ",'" + data_emp.employee_code + "','" + (data_emp.first_name + " " + data_emp.last_name) + "'," + loan[i].LoanAmount + "," + loan[i].InstallmentNumbers + "," + loan[i].InstallmentAmount + "," + loan[i].DeductableAmount + "," + loan[i].BalanceAmount + "));\">Edit</a>" +
                                    //        "<span> / </span>" +
                                    //        "<a href=\"javascript:void(deleteLoanApplication(" + loan[i].Id + "));\">Delete</a>" +
                                    //    "</span>",
                                    //actions = strEditMonthStatus
                                    //actions =
                                    //    "<span data-row='" + loan[i].Id + "'>" +
                                    //        "<a href=\"/HR/LoansManagement/LoanSetting?user_code=" + data_emp.employee_code + "&salary_month=" + loan[i].LoanAllocatedDate.ToString("dd-MM-yyyy") + "\" target=\"_blank\">Edit Loan</a>" +
                                    //    "</span>"
                                });
                            }
                        }
                    }

                    lStatement.EmployeeCode = strEmployeeCode;
                    lStatement.EmployeeName = strEmployeeName;
                    lStatement.LoanInfoList = toReturn;
                    lStatement.LoanAmountOpen = iLoanAmountOpen;
                    lStatement.LoanAmountClosed = iLoanAmountClosed;
                    lStatement.DeductableAmountOpen = iDeductableAmountOpen;
                    lStatement.DeductableAmountClosed = iDeductableAmountClosed;
                    lStatement.BalanceAmountOpen = iBalanceAmountOpen;
                    lStatement.BalanceAmountClosed = iBalanceAmountClosed;

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    loan = new List<Loan>();
                }
            }

            return lStatement;
        }


        public static int AddNewLoan(LoanInfo linfo)
        {
            int response = 0, loan_limit = 0;

            try
            {
                using (Context db = new Context())
                {
                    if (ConfigurationManager.AppSettings["LoanLimit"] != null && ConfigurationManager.AppSettings["LoanLimit"].ToString() != "")
                    {
                        loan_limit = int.Parse(ConfigurationManager.AppSettings["LoanLimit"].ToString());

                    }
                    else
                    {
                        loan_limit = 2;
                    }

                    var data_loan = db.loan.Where(l => l.LoanStatusId == 1 && l.EmployeeId == linfo.EmployeeId).ToList();
                    if (data_loan != null && data_loan.Count > loan_limit)
                    {
                        response = -2;
                    }
                    else
                    {
                        //add new Loan record
                        Loan loan = new Loan();

                        loan.EmployeeId = linfo.EmployeeId;
                        loan.LoanAllocatedDate = linfo.LoanAllocatedDate;

                        loan.LoanTypeId = linfo.LoanTypeId;
                        loan.LoanAmount = linfo.LoanAmount;
                        loan.InstallmentNumbers = linfo.InstallmentNumbers;
                        loan.InstallmentAmount = linfo.InstallmentAmount;
                        loan.DeductableAmount = linfo.DeductableAmount;
                        loan.BalanceAmount = linfo.BalanceAmount;
                        loan.LoanStatusId = linfo.LoanStatusId;

                        loan.Remarks = linfo.Remarks;
                        loan.AttachmentFilePath = linfo.AttachmentFilePath;
                        loan.CreateDateTime = DateTime.Now;
                        loan.UpdateDateTime = DateTime.Now;

                        db.loan.Add(loan);
                        db.SaveChanges();

                        response = 1;
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

        public static List<LoanReportByMonthLog> GetResult(string search, string sortOrder, int start, int length, List<LoanReportByMonthLog> dtResult)
        {
            return FilterResult(search, dtResult).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<LoanReportByMonthLog> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<LoanReportByMonthLog> FilterResult(string search, List<LoanReportByMonthLog> dtResult)
        {
            IQueryable<LoanReportByMonthLog> results = dtResult.AsQueryable();

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
                        /*
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
                        */


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

        public static List<LoanInfo> GetResult(string search, string sortOrder, int start, int length, List<LoanInfo> dtResult)
        {
            return FilterResult(search, dtResult).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<LoanInfo> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<LoanInfo> FilterResult(string search, List<LoanInfo> dtResult)
        {
            IQueryable<LoanInfo> results = dtResult.AsQueryable();

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
                            || (p.LoanAllocatedDateText != null && p.LoanAllocatedDateText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.LoanTypeText != null && p.LoanTypeText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.LoanAmount > 0 && p.LoanAmount.ToString().Contains(search.ToLower()))
                            || (p.BalanceAmount > 0 && p.BalanceAmount.ToString().Contains(search.ToLower()))
                            || (p.LoanStatusText != null && p.LoanStatusText.ToString().ToLower().Contains(search.ToLower()))
                            || (p.Remarks != null && p.Remarks.ToLower().Contains(search.ToLower()))

                        /*
                        || (p.CNIC != null && p.CNIC.ToString().Contains(search.ToLower()))
                        || (p.NTNNumber != null && p.NTNNumber.ToString().Contains(search.ToLower()))
                        || (p.EOBINumber != null && p.EOBINumber.ToString().Contains(search.ToLower()))
                        || (p.SESSINumber != null && p.SESSINumber.ToString().Contains(search.ToLower()))
                        || (p.BasicPay.ToString().Contains(search.ToLower()))
                        || (p.UserHRComments != null && p.UserHRComments.ToLower().Contains(search.ToLower()))
                        */
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

        public static void update(ViewModels.LoanInfo linfo)
        {
            using (Context db = new Context())
            {
                Loan loan = db.loan.Find(linfo.Id);

                var data_emp = db.employee.Where(e => e.active && e.EmployeeId == loan.EmployeeId).FirstOrDefault();
                if (data_emp != null)
                {
                    //loan.EmployeeId = linfo.EmployeeId;
                    //loan.LoanAllocatedDate = linfo.LoanAllocatedDate;

                    //loan.LoanTypeId = linfo.LoanTypeId;
                    //loan.LoanAmount = linfo.LoanAmount;
                    //loan.InstallmentNumbers = linfo.InstallmentNumbers;
                    //loan.InstallmentAmount = linfo.InstallmentAmount;


                    if (linfo.DeductableAmount == -1)
                    {
                        loan.BalanceAmount = linfo.BalanceAmount;
                        //loan.BalanceAmount = linfo.LoanAmount;
                    }
                    else
                    {
                        loan.DeductableAmount = linfo.DeductableAmount;
                    }
                    //if (linfo.DeductableAmount == -1)
                    //{
                    //    loan.InstallmentAmount = linfo.LoanAmount / linfo.InstallmentNumbers;
                    //    loan.BalanceAmount = linfo.LoanAmount;
                    //}
                    //else if (linfo.DeductableAmount == 0)
                    //{
                    //    loan.BalanceAmount = linfo.BalanceAmount - linfo.InstallmentAmount;
                    //}
                    //else
                    //{
                    //    if (linfo.LoanTypeId == -1)
                    //        loan.BalanceAmount = linfo.BalanceAmount - linfo.DeductableAmount;
                    //    else if (linfo.LoanTypeId > 0)//if previous deduction > 0
                    //        loan.BalanceAmount = linfo.BalanceAmount - linfo.LoanTypeId + linfo.DeductableAmount;
                    //    else
                    //        loan.BalanceAmount = linfo.BalanceAmount - linfo.InstallmentAmount + linfo.DeductableAmount;
                    //}


                    loan.LoanStatusId = linfo.LoanStatusId;

                    //loan.Remarks = linfo.Remarks;
                    //loan.AttachmentFilePath = linfo.AttachmentFilePath;

                    loan.UpdateDateTime = DateTime.Now;

                    db.SaveChanges();
                }
            }
        }

        public static ViewModels.LoanInfo remove(ViewModels.LoanInfo toRemove)
        {

            using (Context db = new Context())
            {
                Loan toRemoveModel = db.loan.Find(toRemove.Id);

                db.loan.Remove(toRemoveModel);

                //toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove;
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

        public static List<ViewModels.Employee> getAllEmployees()
        {
            List<ViewModels.Employee> toReturn = new List<ViewModels.Employee>();

            using (Context db = new Context())
            {
                List<DLL.Models.Employee> empList = null;

                try
                {
                    empList = db.employee.Where(e => e.active && e.employee_code != "000000").ToList();
                    if (empList != null && empList.Count > 0)
                    {
                        for (int i = 0; i < empList.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.Employee()
                            {
                                id = empList[i].EmployeeId,
                                first_name = empList[i].employee_code + " - " + empList[i].first_name + " " + empList[i].last_name
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    empList = new List<DLL.Models.Employee>();
                }

                return toReturn;
            }
        }

        public static List<ViewModels.LoanTypeInfo> getAllLoanTypes()
        {
            List<ViewModels.LoanTypeInfo> toReturn = new List<ViewModels.LoanTypeInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.LoanType> lTypes = null;

                try
                {
                    lTypes = db.loan_type.ToList();
                    if (lTypes != null && lTypes.Count > 0)
                    {
                        for (int i = 0; i < lTypes.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.LoanTypeInfo()
                            {
                                Id = lTypes[i].Id,
                                LoanTypeText = lTypes[i].LoanTypeText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lTypes = new List<DLL.Models.LoanType>();
                }

                return toReturn;
            }
        }


        public static List<ViewModels.LoanStatusTypeInfo> getAllLoanStatusTypes()
        {
            List<ViewModels.LoanStatusTypeInfo> toReturn = new List<ViewModels.LoanStatusTypeInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.LoanStatusType> lStatus = null;

                try
                {
                    lStatus = db.loan_status_type.ToList();
                    if (lStatus != null && lStatus.Count > 0)
                    {
                        for (int i = 0; i < lStatus.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.LoanStatusTypeInfo()
                            {
                                Id = lStatus[i].Id,
                                LoanStatusTypeText = lStatus[i].LoanStatusTypeText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lStatus = new List<DLL.Models.LoanStatusType>();
                }

                return toReturn;
            }
        }

        ///////////////////////////////////////////////////////////////

        public static LoanInfo getLatestLoanInfoByEmployeeCode(string emp_code, string month_year)
        {
            LoanInfo loan = new LoanInfo();

            DateTime dtMonth = Convert.ToDateTime(month_year);
            DateTime month_start_date = new DateTime(dtMonth.Year, dtMonth.Month, 1);
            DateTime month_end_date = new DateTime(dtMonth.Year, dtMonth.Month, System.DateTime.DaysInMonth(dtMonth.Year, dtMonth.Month));

            using (Context db = new Context())
            {
                var data_emp = db.employee.Where(p => p.active == true && p.employee_code == emp_code).FirstOrDefault();
                if (data_emp != null)
                {
                    var linfo = db.loan.Where(p => p.EmployeeId == data_emp.EmployeeId && (p.LoanAllocatedDate >= month_start_date && p.LoanAllocatedDate <= month_end_date)).OrderByDescending(o => o.Id).FirstOrDefault();
                    if (linfo != null)
                    {
                        loan.Id = linfo.Id;
                        loan.EmployeeId = linfo.EmployeeId;
                        loan.LoanAllocatedDate = linfo.LoanAllocatedDate;

                        loan.LoanTypeId = linfo.LoanTypeId;
                        loan.LoanAmount = linfo.LoanAmount;
                        loan.InstallmentNumbers = linfo.InstallmentNumbers;
                        loan.InstallmentAmount = linfo.InstallmentAmount;
                        loan.DeductableAmount = linfo.DeductableAmount;
                        loan.BalanceAmount = linfo.BalanceAmount;
                        loan.LoanStatusId = linfo.LoanStatusId;

                        loan.Remarks = linfo.Remarks;
                        loan.AttachmentFilePath = linfo.AttachmentFilePath;
                        loan.CreateDateTimeText = linfo.CreateDateTime.ToString("dd-MM-yyyy hh:mm tt");
                        loan.UpdateDateTimeText = linfo.UpdateDateTime.ToString("dd-MM-yyyy hh:mm tt");
                    }
                }
            }

            return loan;
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

    }
}
