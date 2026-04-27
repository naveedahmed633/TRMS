using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class PayrollsManagementController : Controller
    {
        public class PayrollSearch : DTParameters
        {
            public DateTime from_date { get; set; }

            public DateTime to_date { get; set; }
        }

        public class PayrollSettingTable : DTParameters
        {
            public int id { get; set; }
            public int is_new_payroll { get; set; }
            public int employee_id { get; set; }
            public string employee_code { get; set; }
            public string salary_month_year { get; set; }
            public string cnic { get; set; }
            public string ntn_number { get; set; }
            public string eobi_number { get; set; }
            public string sessi_number { get; set; }

            public int basic_pay { get; set; }
            public int increment { get; set; }
            public int transport { get; set; }
            public int mobile { get; set; }
            public int medical { get; set; }
            public int food { get; set; }
            public int night { get; set; }
            public int commission { get; set; }
            public int rent { get; set; }
            public int group_allowance { get; set; }
            public int cash_allowance { get; set; }
            public int annual_bonus { get; set; }
            public int leaves_count { get; set; }
            public int leaves_encash { get; set; }
            public decimal overtime_in_hours { get; set; }
            public int overtime_amount { get; set; }

            public int income_tax { get; set; }
            public int fine_extra_amount { get; set; }
            public int mobile_deduction { get; set; }
            public int absents_count { get; set; }
            public int absents_amount { get; set; }
            public int eobi_employee { get; set; }
            public int eobi_employer { get; set; }
            public int sessi_employee { get; set; }
            public int sessi_employer { get; set; }
            public int loan_installment { get; set; }
            public int other_deduction { get; set; }
            public int payment_mode_id { get; set; }

            public int gross_salary { get; set; }
            public int total_deduction { get; set; }
            public int net_salary { get; set; }

            public int bank_name_id { get; set; }
            public string bank_account_title { get; set; }
            public string bank_account_number { get; set; }

            public int ack_status_id { get; set; }
            public int pay_status_id { get; set; }

            public string user_hr_comments { get; set; }
            public string attachment_file_path { get; set; }
            public string is_first_month { get; set; }

            public string payment_date { get; set; }
        }

        public class PayrollAmountTable : DTParameters
        {
            public int id { get; set; }
            public int designation_id { get; set; }
            public int grade_id { get; set; }

            public int basic_pay { get; set; }
            public int increment { get; set; }
            public int transport { get; set; }
            public int mobile { get; set; }
            public int medical { get; set; }
            public int food { get; set; }
            public int night { get; set; }
            public int commission { get; set; }
            public int rent { get; set; }
            public int group_allowance { get; set; }
            public int cash_allowance { get; set; }
        }


        /////////////////////////////// Payroll Setting Management ///////////////////////////////////////

        public ActionResult PayrollSetting()
        {
            int iEmployeeId = 0;
            string strEmpCode = string.Empty, strSalaryMonth = string.Empty;
            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

            CreatePayrollSetting vm = new CreatePayrollSetting();

            ViewBag.strEmployeeCode = "";
            ViewBag.strEmployeeName = "";
            vm.strMessage = "";

            if (Request.QueryString["user_code"] != null && Request.QueryString["user_code"].ToString() != "")
            {
                strEmpCode = Request.QueryString["user_code"].ToString();
                Session["user_code"] = strEmpCode;

                if (Request.QueryString["salary_month"] != null && Request.QueryString["salary_month"].ToString() != "")
                {
                    strSalaryMonth = DateTime.ParseExact(Request.QueryString["salary_month"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    //strSalaryMonth = Convert.ToDateTime(Request.QueryString["salary_month"].ToString()).ToString("yyyy-MM-dd");
                }
                else
                {
                    strSalaryMonth = DateTime.Now.ToString("yyyy-MM-dd");
                }
                ViewBag.strMonthYear = Convert.ToDateTime(strSalaryMonth).ToString("MMM yyyy");
                Session["salary_month"] = strSalaryMonth;

                ViewBag.strEmployeeCode = strEmpCode;
                iEmployeeId = PayrollResultSet.getEmployeeId(strEmpCode);
                ViewBag.strEmployeeName = PayrollResultSet.getEmployeeName(strEmpCode);

                if (ViewBag.strEmployeeName != "-1")
                {
                    vm.strMessage = "";

                    dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(strEmpCode);
                    vm.SessionStartDate = dt[0];
                    vm.SessionEndDate = dt[1];
                    vm.strSessionStartDate = dt[0].ToString("dd MMM yyyy");
                    vm.strSessionEndDate = dt[1].ToString("dd MMM yyyy");

                    int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                    leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(strEmpCode);
                    vm.AvailableSickLeaves = leaves[0];
                    vm.AvailableCasualLeaves = leaves[1];
                    vm.AvailableAnnualLeaves = leaves[2];
                    vm.AvailedSickLeaves = leaves[3];
                    vm.AvailedCasualLeaves = leaves[4];
                    vm.AvailedAnnualLeaves = leaves[5];

                    int[] lastMonthLeaves = new int[3] { 0, 0, 0 };
                    lastMonthLeaves = LeaveApplicationResultSet.getUserLastMonthLeavesByUserCode(strEmpCode, DateTime.Now.AddMonths(-1).ToString("yyyy-MM"));
                    vm.LastMonthSickLeaves = lastMonthLeaves[0];
                    vm.LastMonthCasualLeaves = lastMonthLeaves[1];
                    vm.LastMonthAnnualLeaves = lastMonthLeaves[2];

                    vm.payroll_info = PayrollResultSet.getLatestPayrollInfoByEmployeeCode(strEmpCode, strSalaryMonth);
                    if (vm.payroll_info != null)// && vm.payroll_info.CNIC != null)
                    {
                        vm.payroll_info.EmployeeId = iEmployeeId;
                        vm.payroll_info.EmployeeCode = strEmpCode;

                        ViewBag.SelectedPaymentModeId = vm.payroll_info.PaymentModeId;
                        ViewBag.SelectedBankNameId = vm.payroll_info.BankNameId;
                        ViewBag.SelectedAckStatusId = vm.payroll_info.AckStatusId;
                        ViewBag.SelectedPayStatusId = vm.payroll_info.PayStatusId;
                    }
                    else
                    {
                        vm.payroll_info.EmployeeId = iEmployeeId;
                        vm.payroll_info.EmployeeCode = strEmpCode;

                        vm.strMessage = "No Payroll Record Found for This Month";
                    }

                    vm.bank_name_info = PayrollResultSet.getAllBankNames();
                    vm.payment_mode_info = PayrollResultSet.getAllPaymentModeTypes();
                    vm.payment_status_info = PayrollResultSet.getAllPaymentStatusTypes();
                }
                else
                {
                    vm.strMessage = "No Data Found";
                }

                if (Session["payroll_success"] != null && Session["payroll_success"].ToString() != "")
                {
                    if (Session["payroll_success"].ToString() == "1")
                        ViewBag.strUpateMessage = "Payroll Info Added Successfully";
                    if (Session["payroll_success"].ToString() == "2")
                        ViewBag.strUpateMessage = "Payroll Info Updated Successfully";
                    else
                        ViewBag.strUpateMessage = "An Error Occurred";
                }
                else
                {
                    ViewBag.strUpateMessage = "";
                }

                Session["payroll_success"] = null;
            }

            return View(vm);
        }

        [HttpPost]
        public JsonResult PayrollSettingDataHandler(DTParameters param)
        {
            string user_code = "";
            try
            {
                var dtSource = new List<PayrollInfo>();

                if (Session["user_code"] != null && Session["user_code"].ToString() != "")
                {
                    user_code = Session["user_code"].ToString();
                    dtSource = PayrollResultSet.getPayrollByEmployeeCode(user_code); // User.Identity.Name);

                    if (dtSource == null)
                    {
                        return Json("No data found");
                    }

                    // get all employee view models
                    //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                    List<PayrollInfo> data = PayrollResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                    data = data.OrderByDescending(o => o.Id).ToList();
                    int count = PayrollResultSet.Count(param.Search.Value, dtSource);

                    //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                    DTResult<PayrollInfo> result = new DTResult<PayrollInfo>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return null;
        }

        [HttpPost]
        public ActionResult PayrollSetting(PayrollSettingTable pinfo, HttpPostedFileBase attachment_file_path)
        {
            string FileName = string.Empty, FileExtension = string.Empty;

            try
            {
                if (attachment_file_path != null)
                {
                    FileName = Path.GetFileName(attachment_file_path.FileName);
                    FileExtension = Path.GetExtension(attachment_file_path.FileName).ToLower();

                    if (FileExtension == ".jpg" || FileExtension == ".png" || FileExtension == ".pdf")
                    {
                        var guid = Guid.NewGuid().ToString();
                        string filename_guid = guid + "_" + FileName;
                        var path = Path.Combine(Server.MapPath("~/Content/PayrollSet"), filename_guid);
                        attachment_file_path.SaveAs(path);

                        pinfo.attachment_file_path = filename_guid;
                    }
                    else
                    {
                        pinfo.attachment_file_path = null;
                    }
                }
                else
                {
                    pinfo.attachment_file_path = null;
                }

                /////////////////////////////////////////////////////

                PayrollInfo payroll = new PayrollInfo();

                DateTime? dt_salary_month_year = null;
                DateTime? dt_payment_date = null;

                dt_salary_month_year = DateTime.ParseExact(pinfo.salary_month_year, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                dt_payment_date = DateTime.ParseExact(pinfo.payment_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                payroll.Id = pinfo.id;
                payroll.EmployeeId = pinfo.employee_id;
                payroll.SalaryMonthYear = dt_salary_month_year ?? DateTime.Now;
                payroll.CNIC = pinfo.cnic;
                payroll.NTNNumber = pinfo.ntn_number;
                payroll.EOBINumber = pinfo.eobi_number;
                payroll.SESSINumber = pinfo.sessi_number;

                payroll.BasicPay = pinfo.basic_pay;
                payroll.Increment = pinfo.increment;
                payroll.Transport = pinfo.transport;
                payroll.Mobile = pinfo.mobile;
                payroll.Medical = pinfo.medical;
                payroll.Food = pinfo.food;
                payroll.Night = pinfo.night;
                payroll.GroupAllowance = pinfo.group_allowance;
                payroll.Commission = pinfo.commission;
                payroll.Rent = pinfo.rent;
                payroll.CashAllowance = pinfo.cash_allowance;
                payroll.AnnualBonus = pinfo.annual_bonus;
                payroll.LeavesCount = pinfo.leaves_count;
                payroll.LeavesEncash = pinfo.leaves_encash;
                payroll.OvertimeInHours = pinfo.overtime_in_hours;
                payroll.OvertimeAmount = pinfo.overtime_amount;

                payroll.IncomeTax = pinfo.income_tax;
                payroll.FineExtraAmount = pinfo.fine_extra_amount;
                payroll.MobileDeduction = pinfo.mobile_deduction;
                payroll.AbsentsCount = pinfo.absents_count;
                payroll.AbsentsAmount = pinfo.absents_amount;

                payroll.EOBIEmployee = pinfo.eobi_employee;
                payroll.EOBIEmployer = pinfo.eobi_employer;
                payroll.SESSIEmployee = pinfo.sessi_employee;
                payroll.SESSIEmployer = pinfo.sessi_employer;
                payroll.LoanInstallment = pinfo.loan_installment;
                payroll.OtherDeduction = pinfo.other_deduction;

                payroll.GrossSalary = pinfo.gross_salary;
                payroll.TotalDeduction = pinfo.total_deduction;
                payroll.NetSalary = pinfo.net_salary;

                payroll.PaymentModeId = pinfo.payment_mode_id;
                payroll.BankNameId = pinfo.bank_name_id;
                payroll.BankAccTitle = pinfo.bank_account_title;
                payroll.BankAccNo = pinfo.bank_account_number;

                payroll.AckStatusId = pinfo.ack_status_id;
                payroll.PayStatusId = pinfo.pay_status_id;

                payroll.UserHRComments = pinfo.user_hr_comments;
                payroll.AttachmentFilePath = pinfo.attachment_file_path;
                payroll.IsFirstMonthText = pinfo.is_first_month == null ? "YES" : pinfo.is_first_month;

                payroll.PaymentDateTime = dt_payment_date ?? DateTime.Now;
                payroll.UpdateDateTime = DateTime.Now;

                ////////////////////////////////////////////////////

                if (pinfo.is_new_payroll == 1) //add new
                {
                    int added = PayrollResultSet.AddNewPayroll(payroll);
                    if (added > 0)
                    {
                        //success
                        Session["payroll_success"] = "1";
                    }
                    else
                    {
                        Session["payroll_success"] = "0";
                    }
                }
                else    //update existing
                {
                    var json = JsonConvert.SerializeObject(payroll);
                    PayrollResultSet.update(payroll);
                    TimeTune.AuditTrail.update(json, "Payroll", User.Identity.Name);
                    //return Json(new { status = "success" });

                    Session["payroll_success"] = "2";
                }
            }
            catch (Exception)
            {
                Session["payroll_success"] = "-1";
            }

            //http://localhost:57588/HR/PayrollsManagement/PayrollSetting?user_code=727272&salary_month=2018-07-01
            return RedirectPermanent("/HR/PayrollsManagement/PayrollSetting?user_code=" + pinfo.employee_code + "&salary_month=" + pinfo.salary_month_year);
        }

        [HttpPost]
        public ActionResult RemovePayrollSetting(ViewModels.PayrollInfo toRemove)
        {
            var entity = PayrollResultSet.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            TimeTune.AuditTrail.delete(json, "Payroll", User.Identity.Name);
            return Json(new { status = "success" });
        }

        /////////////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult PayrollSetting_Add_NEW_Backup(PayrollSettingTable ldata, HttpPostedFileBase attachment_file_path)
        {
            string FileName = string.Empty, FileExtension = string.Empty;

            try
            {
                if (attachment_file_path != null)
                {
                    FileName = Path.GetFileName(attachment_file_path.FileName);
                    FileExtension = Path.GetExtension(attachment_file_path.FileName).ToLower();

                    if (FileExtension == ".jpg" || FileExtension == ".png" || FileExtension == ".pdf")
                    {
                        var guid = Guid.NewGuid().ToString();
                        string filename_guid = guid + "_" + FileName;
                        var path = Path.Combine(Server.MapPath("~/Content/PayrollSet"), filename_guid);
                        attachment_file_path.SaveAs(path);

                        ldata.attachment_file_path = filename_guid;
                    }
                    else
                    {
                        ldata.attachment_file_path = null;
                    }
                }
                else
                {
                    ldata.attachment_file_path = null;
                }

                //Add New Record
                int daysCount = 0; DateTime dtLeave = DateTime.Now;
                PayrollInfo payrollInfo = new PayrollInfo();
                /*
                if (ldata.leave_type_id > 0)
                    lAppInfo.LeaveTypeId = ldata.leave_type_id;
                else
                    lAppInfo.LeaveTypeId = 1; //sick leave

                if (ldata.from_date != null)
                    lAppInfo.FromDate = DateTime.ParseExact(ldata.from_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture); //ldata.from_date;

                if (ldata.to_date != null)
                    lAppInfo.ToDate = DateTime.ParseExact(ldata.to_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture); //ldata.to_date;

                if (lAppInfo.ToDate == lAppInfo.FromDate)
                {
                    daysCount = 1;
                }
                else
                {
                    for (int i = 0; i < ((lAppInfo.ToDate - lAppInfo.FromDate).Days + 1); i++)
                    {
                        dtLeave = lAppInfo.FromDate.AddDays(i);

                        if (dtLeave.DayOfWeek != DayOfWeek.Sunday) // || dtLeave.DayOfWeek != DayOfWeek.Saturday)
                        {
                            daysCount++;
                        }
                    }
                }

                lAppInfo.DaysCount = daysCount; // (lAppInfo.ToDate - lAppInfo.FromDate).Days + 1;

                if (ldata.leave_reason_id > 0)
                    lAppInfo.LeaveReasonId = ldata.leave_reason_id;
                else
                    lAppInfo.LeaveReasonId = 1;

                lAppInfo.ReasonDetail = ldata.reason_detail;

                if (User.Identity.Name != "")
                    lAppInfo.EmployeeId = LeaveApplicationResultSet.GetUserId(User.Identity.Name);

                lAppInfo.ApproverId = ldata.approver_id;

                //if (User.Identity.Name != "")
                //    lAppInfo.ApproverId = LeaveApplicationResultSet.GetApproverUserID(User.Identity.Name); //ldata.approver_id;

                if (ldata.leave_status_id > 0)
                    lAppInfo.LeaveStatusId = ldata.leave_status_id;
                else
                    lAppInfo.LeaveStatusId = 1;//pending

                lAppInfo.AttachmentFilePath = ldata.attachment_file_path;

                lAppInfo.IsActive = true;
                */
                payrollInfo.CreateDateTime = DateTime.Now;
                payrollInfo.UpdateDateTime = DateTime.Now;

                int added = PayrollResultSet.AddNewPayroll(payrollInfo);

                if (added == 1)
                {
                    //success
                    ViewBag.Message = "The Payroll record is submitted successfully!";
                }
                else if (added == -1)
                {
                    //error
                    ViewBag.Message = "An error occurred.";
                }
                else if (added == -2)
                {
                    //error
                    ViewBag.Message = "You have already applied for same day(s) leave.";
                }
                else if (added == -3)
                {
                    //error
                    ViewBag.Message = "'From Date' should not be Greater than 'To Date'.";
                }
                else if (added == -4)
                {
                    //error
                    ViewBag.Message = "Apply within current-month date-range. For next month apply leave separately.";
                }
                else if (added == -5)
                {
                    //error
                    ViewBag.Message = "Apply 3 days before your leave 'From Date'.";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            /////////////////////////////////////////////////////////////////////

            string strEmpCode = string.Empty, strSalaryMonth = string.Empty;
            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

            CreatePayrollSetting vm = new CreatePayrollSetting();
            ViewBag.strEmployeeCode = "";
            ViewBag.strEmployeeName = "";
            vm.strMessage = "";

            if (Session["user_code"] != null && Session["user_code"].ToString() != "")
            {
                strEmpCode = Session["user_code"].ToString();

                if (Session["salary_month"] != null && Session["salary_month"].ToString() != "")
                {
                    strSalaryMonth = Convert.ToDateTime(Session["salary_month"].ToString()).ToString("yyyy-MM-dd");
                }
                else
                {
                    strSalaryMonth = DateTime.Now.ToString("yyyy-MM-dd");
                }
                ViewBag.strMonthYear = Convert.ToDateTime(strSalaryMonth).ToString("MMM yyyy");

                ViewBag.strEmployeeCode = strEmpCode;
                ViewBag.strEmployeeName = PayrollResultSet.getEmployeeName(strEmpCode);

                if (ViewBag.strEmployeeName != "-1")
                {
                    dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(strEmpCode);
                    vm.SessionStartDate = dt[0];
                    vm.SessionEndDate = dt[1];
                    vm.strSessionStartDate = dt[0].ToString("dd MMM yyyy");
                    vm.strSessionEndDate = dt[1].ToString("dd MMM yyyy");

                    int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                    leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(strEmpCode);
                    vm.AvailableSickLeaves = leaves[0];
                    vm.AvailableCasualLeaves = leaves[1];
                    vm.AvailableAnnualLeaves = leaves[2];
                    vm.AvailedSickLeaves = leaves[3];
                    vm.AvailedCasualLeaves = leaves[4];
                    vm.AvailedAnnualLeaves = leaves[5];

                    vm.payroll_info = PayrollResultSet.getLatestPayrollInfoByEmployeeCode(strEmpCode, strSalaryMonth);
                    if (vm.payroll_info != null && vm.payroll_info.CNIC != null)
                    {
                        ViewBag.SelectedPaymentModeId = vm.payroll_info.PaymentModeId;
                        ViewBag.SelectedBankNameId = vm.payroll_info.BankNameId;
                        ViewBag.SelectedAckStatusId = vm.payroll_info.AckStatusId;
                        ViewBag.SelectedPayStatusId = vm.payroll_info.PayStatusId;
                    }
                    vm.payment_mode_info = PayrollResultSet.getAllPaymentModeTypes();
                    vm.payment_status_info = PayrollResultSet.getAllPaymentStatusTypes();
                }
                else
                {
                    vm.strMessage = "No Employee Found";
                }
            }

            return View(vm);
        }

        /////////////////////////////// Payroll Amounts Management ///////////////////////////////////////

        public ActionResult PayrollAmount()
        {
            CreatePayrollAmount vm = new CreatePayrollAmount();

            vm.designation_info = PayrollResultSet.getAllDesignations();
            vm.grade_info = PayrollResultSet.getAllGrades();

            return View(vm);
        }

        [HttpPost]
        public JsonResult PayrollAmountDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<PayrollAmountInfo>();

                dtSource = PayrollResultSet.getAllPayrollAmounts();
                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<PayrollAmountInfo> data = PayrollResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = PayrollResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<PayrollAmountInfo> result = new DTResult<PayrollAmountInfo>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };

                return Json(result);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            // return null;
        }

        [HttpPost]
        public ActionResult PayrollAmount(PayrollAmountTable pinfo)
        {
            PayrollAmountInfo pAmount = new PayrollAmountInfo();

            try
            {
                PayrollAmountInfo idPAmount = PayrollResultSet.IsPayrollAmountExists(pinfo.designation_id, pinfo.grade_id);
                if (idPAmount != null && idPAmount.Id > 0)
                {
                    ViewBag.PayrollAmountMessage = "Already Exists.";
                }
                else
                {
                    //pAmount.Id = pinfo.id;
                    pAmount.DesignationId = pinfo.designation_id;
                    pAmount.GradeId = pinfo.grade_id;

                    pAmount.BasicPay = pinfo.basic_pay;
                    pAmount.Increment = pinfo.increment;
                    pAmount.Transport = pinfo.transport;
                    pAmount.Mobile = pinfo.mobile;
                    pAmount.Medical = pinfo.medical;
                    pAmount.Food = pinfo.food;
                    pAmount.Night = pinfo.night;
                    pAmount.GroupAllowance = pinfo.group_allowance;
                    pAmount.Commission = pinfo.commission;
                    pAmount.Rent = pinfo.rent;
                    pAmount.CashAllowance = pinfo.cash_allowance;

                    pAmount.CreateDateTime = DateTime.Now;
                    pAmount.UpdateDateTime = DateTime.Now;

                    int added = PayrollResultSet.AddNewPayrollAmount(pAmount);
                    if (added > 0)
                    {
                        //success
                        ViewBag.PayrollAmountMessage = "Added Successfully!";
                    }
                    else
                    {
                        ViewBag.PayrollAmountMessage = "Error Occurred";
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.PayrollAmountMessage = "Failed";
            }

            CreatePayrollAmount vm = new CreatePayrollAmount();

            vm.designation_info = PayrollResultSet.getAllDesignations();
            vm.grade_info = PayrollResultSet.getAllGrades();

            return View(vm);
        }


        [HttpPost]
        public ActionResult UpdatePayrollAmount(ViewModels.PayrollAmountInfo toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            PayrollResultSet.updatePayrollAmount(toUpdate);
            TimeTune.AuditTrail.update(json, "PayrollAmount", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemovePayrollAmount(ViewModels.PayrollAmountInfo toRemove)
        {
            var entity = PayrollResultSet.removePayrollAmount(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            TimeTune.AuditTrail.delete(json, "PayrollAmount", User.Identity.Name);
            return Json(new { status = "success" });
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////

        #region BanksNames

        public ActionResult BankNames()
        {

            return View();
        }

        [HttpPost]
        public JsonResult BankDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.BankNameInfo>();
                // get all employee view models
                dtsource = TimeTune.Bank_CRUD.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.BankNameInfo> data = BankResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = BankResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.BankNameInfo> result = new DTResult<ViewModels.BankNameInfo>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpPost]
        public ActionResult AddBank(ViewModels.BankNameInfo fromForm)
        {
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.Bank_CRUD.add(fromForm);
            if (id == 0)
            {
                return RedirectToAction("BankNames", new { Message = "already" });
            }
            return RedirectToAction("BankNames", new { Message = "success" });
        }

        [HttpPost]
        public ActionResult UpdateBank(ViewModels.BankNameInfo toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.Bank_CRUD.update(toUpdate);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveBank(ViewModels.BankNameInfo toRemove)
        {
            var entity = TimeTune.Bank_CRUD.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            return Json(new { status = "success" });
        }
        #endregion

        #region SetsPayroll
        public ActionResult SetsPayroll(string result)
        {
            string value = result;
            ViewBag.Message = result;
            // only access groups are to be sent without ajax.
            CreateEmployee createEmployeeViewModel = new CreateEmployee();
            createEmployeeViewModel.accessGroups = TimeTune.EmployeeAccessGroup.getAllButHr();
            createEmployeeViewModel.skillSets = TimeTune.EmployeeAccessGroup.getAllSkillSets();

            return View(createEmployeeViewModel);
        }


        [HttpPost]
        public JsonResult SetsPayrollDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<Employee>();

                // get all employee view models
                int count = TimeTune.EmployeeCrud.searchEmployeesSetsPayroll(param.Search.Value, param.SortOrder, param.Start, param.Length, out dtsource);

                //List<Employee> data = ResultSet.GetResult(param.SortOrder, param.Start, param.Length, dtsource);

                //int count = ResultSet.Count(param.Search.Value, dtsource);




                DTResult<Employee> result = new DTResult<Employee>
                {
                    draw = param.Draw,
                    data = dtsource,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }
        #endregion

        #region ViewPayrollSetting 
        public ActionResult ViewPayrollSetting()
        {
            int iEmployeeId = 0;
            string strEmpCode = string.Empty, strSalaryMonth = string.Empty;
            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

            CreatePayrollSetting vm = new CreatePayrollSetting();

            ViewBag.strEmployeeCode = "";
            ViewBag.strEmployeeName = "";
            vm.strMessage = "";

            if (Request.QueryString["user_code"] != null && Request.QueryString["user_code"].ToString() != "")
            {
                strEmpCode = Request.QueryString["user_code"].ToString();
                Session["user_code"] = strEmpCode;

                if (Request.QueryString["salary_month"] != null && Request.QueryString["salary_month"].ToString() != "")
                {
                    strSalaryMonth = DateTime.ParseExact(Request.QueryString["salary_month"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    //strSalaryMonth = Convert.ToDateTime(Request.QueryString["salary_month"].ToString()).ToString("yyyy-MM-dd");
                }
                else
                {
                    strSalaryMonth = DateTime.Now.ToString("yyyy-MM-dd");
                }
                ViewBag.strMonthYear = Convert.ToDateTime(strSalaryMonth).ToString("MMM yyyy");
                Session["salary_month"] = strSalaryMonth;

                ViewBag.strEmployeeCode = strEmpCode;
                iEmployeeId = PayrollResultSet.getEmployeeId(strEmpCode);
                ViewBag.strEmployeeName = PayrollResultSet.getEmployeeName(strEmpCode);

                if (ViewBag.strEmployeeName != "-1")
                {
                    vm.strMessage = "";

                    dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(strEmpCode);
                    vm.SessionStartDate = dt[0];
                    vm.SessionEndDate = dt[1];
                    vm.strSessionStartDate = dt[0].ToString("dd MMM yyyy");
                    vm.strSessionEndDate = dt[1].ToString("dd MMM yyyy");

                    int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                    leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(strEmpCode);
                    vm.AvailableSickLeaves = leaves[0];
                    vm.AvailableCasualLeaves = leaves[1];
                    vm.AvailableAnnualLeaves = leaves[2];
                    vm.AvailedSickLeaves = leaves[3];
                    vm.AvailedCasualLeaves = leaves[4];
                    vm.AvailedAnnualLeaves = leaves[5];

                    int[] lastMonthLeaves = new int[3] { 0, 0, 0 };
                    lastMonthLeaves = LeaveApplicationResultSet.getUserLastMonthLeavesByUserCode(strEmpCode, DateTime.Now.AddMonths(-1).ToString("yyyy-MM"));
                    vm.LastMonthSickLeaves = lastMonthLeaves[0];
                    vm.LastMonthCasualLeaves = lastMonthLeaves[1];
                    vm.LastMonthAnnualLeaves = lastMonthLeaves[2];

                    vm.payroll_info = PayrollResultSet.getLatestPayrollInfoByEmployeeCode(strEmpCode, strSalaryMonth);
                    if (vm.payroll_info != null)// && vm.payroll_info.CNIC != null)
                    {
                        vm.payroll_info.EmployeeId = iEmployeeId;
                        vm.payroll_info.EmployeeCode = strEmpCode;

                        ViewBag.SelectedPaymentModeId = vm.payroll_info.PaymentModeId;
                        ViewBag.SelectedBankNameId = vm.payroll_info.BankNameId;
                        ViewBag.SelectedAckStatusId = vm.payroll_info.AckStatusId;
                        ViewBag.SelectedPayStatusId = vm.payroll_info.PayStatusId;
                    }
                    else
                    {
                        vm.payroll_info.EmployeeId = iEmployeeId;
                        vm.payroll_info.EmployeeCode = strEmpCode;

                        vm.strMessage = "No Payroll Record Found for This Month";
                    }

                    vm.bank_name_info = PayrollResultSet.getAllBankNames();
                    vm.payment_mode_info = PayrollResultSet.getAllPaymentModeTypes();
                    vm.payment_status_info = PayrollResultSet.getAllPaymentStatusTypes();
                }
                else
                {
                    vm.strMessage = "No Data Found";
                }

                if (Session["payroll_success"] != null && Session["payroll_success"].ToString() != "")
                {
                    if (Session["payroll_success"].ToString() == "1")
                        ViewBag.strUpateMessage = "Payroll Info Added Successfully";
                    if (Session["payroll_success"].ToString() == "2")
                        ViewBag.strUpateMessage = "Payroll Info Updated Successfully";
                    else
                        ViewBag.strUpateMessage = "An Error Occurred";
                }
                else
                {
                    ViewBag.strUpateMessage = "";
                }

                Session["payroll_success"] = null;
            }

            return View(vm);
        }

        [HttpPost]
        public JsonResult ViewPayrollSettingDataHandler(DTParameters param)
        {
            string user_code = "";
            try
            {
                var dtSource = new List<PayrollInfo>();

                if (Session["user_code"] != null && Session["user_code"].ToString() != "")
                {
                    user_code = Session["user_code"].ToString();
                    dtSource = PayrollResultSet.getPayrollByEmployeeCode(user_code); // User.Identity.Name);

                    if (dtSource == null)
                    {
                        return Json("No data found");
                    }

                    // get all employee view models
                    //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                    List<PayrollInfo> data = PayrollResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                    data = data.OrderByDescending(o => o.Id).ToList();
                    int count = PayrollResultSet.Count(param.Search.Value, dtSource);

                    //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                    DTResult<PayrollInfo> result = new DTResult<PayrollInfo>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return null;
        }

        [HttpPost]
        public ActionResult ViewPayrollSetting(PayrollSettingTable pinfo, HttpPostedFileBase attachment_file_path)
        {
            string FileName = string.Empty, FileExtension = string.Empty;

            try
            {
                if (attachment_file_path != null)
                {
                    FileName = Path.GetFileName(attachment_file_path.FileName);
                    FileExtension = Path.GetExtension(attachment_file_path.FileName).ToLower();

                    if (FileExtension == ".jpg" || FileExtension == ".png" || FileExtension == ".pdf")
                    {
                        var guid = Guid.NewGuid().ToString();
                        string filename_guid = guid + "_" + FileName;
                        var path = Path.Combine(Server.MapPath("~/Content/PayrollSet"), filename_guid);
                        attachment_file_path.SaveAs(path);

                        pinfo.attachment_file_path = filename_guid;
                    }
                    else
                    {
                        pinfo.attachment_file_path = null;
                    }
                }
                else
                {
                    pinfo.attachment_file_path = null;
                }

                /////////////////////////////////////////////////////

                PayrollInfo payroll = new PayrollInfo();

                DateTime? dt_salary_month_year = null;
                DateTime? dt_payment_date = null;

                dt_salary_month_year = DateTime.ParseExact(pinfo.salary_month_year, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                dt_payment_date = DateTime.ParseExact(pinfo.payment_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                payroll.Id = pinfo.id;
                payroll.EmployeeId = pinfo.employee_id;
                payroll.SalaryMonthYear = dt_salary_month_year ?? DateTime.Now;
                payroll.CNIC = pinfo.cnic;
                payroll.NTNNumber = pinfo.ntn_number;
                payroll.EOBINumber = pinfo.eobi_number;
                payroll.SESSINumber = pinfo.sessi_number;

                payroll.BasicPay = pinfo.basic_pay;
                payroll.Increment = pinfo.increment;
                payroll.Transport = pinfo.transport;
                payroll.Mobile = pinfo.mobile;
                payroll.Medical = pinfo.medical;
                payroll.Food = pinfo.food;
                payroll.Night = pinfo.night;
                payroll.GroupAllowance = pinfo.group_allowance;
                payroll.Commission = pinfo.commission;
                payroll.Rent = pinfo.rent;
                payroll.CashAllowance = pinfo.cash_allowance;
                payroll.AnnualBonus = pinfo.annual_bonus;
                payroll.LeavesCount = pinfo.leaves_count;
                payroll.LeavesEncash = pinfo.leaves_encash;
                payroll.OvertimeInHours = pinfo.overtime_in_hours;
                payroll.OvertimeAmount = pinfo.overtime_amount;

                payroll.IncomeTax = pinfo.income_tax;
                payroll.FineExtraAmount = pinfo.fine_extra_amount;
                payroll.MobileDeduction = pinfo.mobile_deduction;
                payroll.AbsentsCount = pinfo.absents_count;
                payroll.AbsentsAmount = pinfo.absents_amount;

                payroll.EOBIEmployee = pinfo.eobi_employee;
                payroll.EOBIEmployer = pinfo.eobi_employer;
                payroll.SESSIEmployee = pinfo.sessi_employee;
                payroll.SESSIEmployer = pinfo.sessi_employer;
                payroll.LoanInstallment = pinfo.loan_installment;
                payroll.OtherDeduction = pinfo.other_deduction;

                payroll.GrossSalary = pinfo.gross_salary;
                payroll.TotalDeduction = pinfo.total_deduction;
                payroll.NetSalary = pinfo.net_salary;

                payroll.PaymentModeId = pinfo.payment_mode_id;
                payroll.BankNameId = pinfo.bank_name_id;
                payroll.BankAccTitle = pinfo.bank_account_title;
                payroll.BankAccNo = pinfo.bank_account_number;

                payroll.AckStatusId = pinfo.ack_status_id;
                payroll.PayStatusId = pinfo.pay_status_id;

                payroll.UserHRComments = pinfo.user_hr_comments;
                payroll.AttachmentFilePath = pinfo.attachment_file_path;
                payroll.IsFirstMonthText = pinfo.is_first_month == null ? "YES" : pinfo.is_first_month;

                payroll.PaymentDateTime = dt_payment_date ?? DateTime.Now;
                payroll.UpdateDateTime = DateTime.Now;

                ////////////////////////////////////////////////////

                if (pinfo.is_new_payroll == 1) //add new
                {
                    int added = PayrollResultSet.AddNewPayroll(payroll);
                    if (added > 0)
                    {
                        //success
                        Session["payroll_success"] = "1";
                    }
                    else
                    {
                        Session["payroll_success"] = "0";
                    }
                }
                else    //update existing
                {
                    var json = JsonConvert.SerializeObject(payroll);
                    PayrollResultSet.update(payroll);
                    TimeTune.AuditTrail.update(json, "Payroll", User.Identity.Name);
                    //return Json(new { status = "success" });

                    Session["payroll_success"] = "2";
                }
            }
            catch (Exception)
            {
                Session["payroll_success"] = "-1";
            }

            //http://localhost:57588/HR/PayrollsManagement/PayrollSetting?user_code=727272&salary_month=2020-01-01
            return RedirectPermanent("/HR/PayrollsManagement/PayrollSetting?user_code=" + pinfo.employee_code + "&salary_month=" + pinfo.salary_month_year);
        }

        #endregion

    }
}
