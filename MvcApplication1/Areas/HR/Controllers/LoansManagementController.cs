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
    public class LoansManagementController : Controller
    {
        public ActionResult LoanApplication()
        {
            // only access groups are to be sent without ajax.
            CreateLoanApplication vm = new CreateLoanApplication();

            vm.employees = LoanResultSet.getAllEmployees();
            vm.loan_types = LoanResultSet.getAllLoanTypes();
            vm.loan_status_types = LoanResultSet.getAllLoanStatusTypes();

            return View(vm);
        }


        public class LoanSearch : DTParameters
        {
            public DateTime from_date { get; set; }

            public DateTime to_date { get; set; }
        }

        public class LoanTable : DTParameters
        {
            public int Id { get; set; }
            public int employee_id { get; set; }
            public string loan_allocated_date { get; set; }

            public int loan_type_id { get; set; }
            public int loan_amount { get; set; }
            public int installment_numbers { get; set; }
            //public int installment_amount { get; set; }
            public int deductable_amount { get; set; }
            //public int balance_amount { get; set; }
            public int loan_status_id { get; set; }

            public string remarks { get; set; }
            public string attachment_file_path { get; set; }

            public bool is_active { get; set; }
        }

        [HttpPost]
        public ActionResult LoanApplication(LoanTable ldata, HttpPostedFileBase attachment_file_path)
        {
            int installment_amount_calc = 0, balance_amount_calc = 0;
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
                        var path = Path.Combine(Server.MapPath("~/Content/LoanApps"), filename_guid);
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
                DateTime dtLeave = DateTime.Now;
                LoanInfo lAppInfo = new LoanInfo();

                //if (User.Identity.Name != "")
                //    lAppInfo.EmployeeId = LoanResultSet.GetUserId(User.Identity.Name);

                lAppInfo.EmployeeId = ldata.employee_id;

                if (ldata.loan_allocated_date != null)
                    lAppInfo.LoanAllocatedDate = DateTime.ParseExact(ldata.loan_allocated_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture); //ldata.from_date;

                if (ldata.loan_type_id > 0)
                    lAppInfo.LoanTypeId = ldata.loan_type_id;
                else
                    lAppInfo.LoanTypeId = 1;

                lAppInfo.LoanAmount = ldata.loan_amount;

                lAppInfo.InstallmentNumbers = ldata.installment_numbers;
                if (ldata.loan_amount > 0 && ldata.installment_numbers > 0)
                {
                    installment_amount_calc = ldata.loan_amount / ldata.installment_numbers;
                    lAppInfo.InstallmentAmount = installment_amount_calc;
                }
                else
                {
                    installment_amount_calc = 0;
                    lAppInfo.InstallmentAmount = installment_amount_calc;
                }

                lAppInfo.DeductableAmount = ldata.deductable_amount;
                if (ldata.deductable_amount > 0)
                {
                    balance_amount_calc = ldata.loan_amount - ldata.deductable_amount;
                    lAppInfo.BalanceAmount = balance_amount_calc;
                }
                else
                {
                    balance_amount_calc = ldata.loan_amount - installment_amount_calc;
                    lAppInfo.BalanceAmount = balance_amount_calc;
                }

                if (ldata.loan_status_id > 0)
                    lAppInfo.LoanStatusId = ldata.loan_status_id;
                else
                    lAppInfo.LoanStatusId = 1;

                lAppInfo.Remarks = ldata.remarks;
                lAppInfo.AttachmentFilePath = ldata.attachment_file_path;
                lAppInfo.CreateDateTime = DateTime.Now;
                lAppInfo.UpdateDateTime = DateTime.Now;

                int added = LoanResultSet.AddNewLoan(lAppInfo);

                if (added == 1)
                {
                    //success
                    ViewBag.Message = "The Laon Application is submitted successfully!";
                }
                else if (added == -1)
                {
                    //error
                    ViewBag.Message = "An error occurred.";
                }
                else if (added == -2)
                {
                    //error
                    ViewBag.Message = "Another new loan cannot be allocated for this employee due to its limit exceeded.";
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

            CreateLoanApplication vm = new CreateLoanApplication();

            vm.employees = LoanResultSet.getAllEmployees();
            vm.loan_types = LoanResultSet.getAllLoanTypes();
            vm.loan_status_types = LoanResultSet.getAllLoanStatusTypes();

            return View(vm);
        }

        [HttpPost]
        public JsonResult LoanApplicationDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<LoanInfo>();
                dtSource = LoanResultSet.getAllLoanApplications();

                if (dtSource == null)
                {
                    return Json("No data found");
                }
                List<LoanInfo> data = LoanResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);

                data = data.OrderByDescending(o => o.Id).ToList();
                int count = LoanResultSet.Count(param.Search.Value, dtSource);
                DTResult<LoanInfo> result = new DTResult<LoanInfo>
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

            //return null;
        }

        [HttpPost]
        public ActionResult UpdateLoanApplication(ViewModels.LoanInfo toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            LoanResultSet.update(toUpdate);
            TimeTune.AuditTrail.delete(json, "LoanApplication", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLoanApplication(ViewModels.LoanInfo toRemove)
        {
            var entity = LoanResultSet.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            TimeTune.AuditTrail.delete(json, "LoanApplication", User.Identity.Name);
            return Json(new { status = "success" });
        }

        /////////////////////////////////////////////////////////////////////
        
    }
}
