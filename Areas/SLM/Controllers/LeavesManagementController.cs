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

namespace MvcApplication1.Areas.SLM.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SLM)]
    public class LeavesManagementController : Controller
    {
        //[ActionName("LeaveApplication")]
        public ActionResult LeaveApplication()
        {
            // only access groups are to be sent without ajax.
            CreateLeaveApplication vm = new CreateLeaveApplication();

            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };
            //dt = LeaveApplicationResultSet.getSessionDatesByAcademicCalendar(DateTime.Now.Year);
            dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(User.Identity.Name);
            vm.SessionStartDate = dt[0];
            vm.SessionEndDate = dt[1];
            vm.strSessionStartDate = dt[0].ToString("dd MMM yyyy");
            vm.strSessionEndDate = dt[1].ToString("dd MMM yyyy");

            int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
            leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(User.Identity.Name);
            vm.AvailableSickLeaves = leaves[0];
            vm.AvailableCasualLeaves = leaves[1];
            vm.AvailableAnnualLeaves = leaves[2];
            vm.AvailedSickLeaves = leaves[3];
            vm.AvailedCasualLeaves = leaves[4];
            vm.AvailedAnnualLeaves = leaves[5];

            vm.leave_types = LeaveApplicationResultSet.getAllLeaveTypes();
            vm.leave_reasons = LeaveApplicationResultSet.getAllLeaveReasons();

            int iGVCampusID = 0; iGVCampusID = int.Parse(ViewModel.GlobalVariables.GV_EmployeeCampusID);
            vm.leave_approver = LeaveApplicationResultSet.GetApproverInfo(User.Identity.Name, iGVCampusID);

            return View(vm);
        }

        public class LeaveApplicationSearch : DTParameters
        {
            public DateTime from_date { get; set; }

            public DateTime to_date { get; set; }
        }

        public class LeaveApplicationTable : DTParameters
        {
            public int leave_type_id { get; set; }

            public string from_date { get; set; }

            public string to_date { get; set; }

            public int leave_reason_id { get; set; }

            public string reason_detail { get; set; }

            public int user_id { get; set; }

            public int approver_id { get; set; }

            public string approver_detail { get; set; }

            public int leave_status_id { get; set; }

            public string attachment_file_path { get; set; }

            public bool is_active { get; set; }
        }

        [HttpPost]
        public ActionResult LeaveApplication(LeaveApplicationTable ldata, HttpPostedFileBase attachment_file_path)
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
                        var path = Path.Combine(Server.MapPath("~/Content/LeaveApps"), filename_guid);
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
                LeaveApplicationInfo lAppInfo = new LeaveApplicationInfo();

                if (ldata.leave_type_id > 0)
                    lAppInfo.LeaveTypeId = ldata.leave_type_id;
                else
                    lAppInfo.LeaveTypeId = 1;//sick

                if (ldata.from_date != null)
                    lAppInfo.FromDate = DateTime.ParseExact(ldata.from_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture); //ldata.from_date;

                if (ldata.to_date != null)
                    lAppInfo.ToDate = DateTime.ParseExact(ldata.to_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture); //ldata.to_date;

                lAppInfo.DaysCount = (lAppInfo.ToDate - lAppInfo.FromDate).Days + 1;

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

                lAppInfo.CreateDateTime = DateTime.Now;
                lAppInfo.UpdateDateTime = DateTime.Now;

                int added = LeaveApplicationResultSet.AddNewLeaveApplication(lAppInfo);

                if (added == 1)
                {
                    //success
                    ViewBag.Message = "The Leave Application is submitted successfully!";
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

            CreateLeaveApplication vm = new CreateLeaveApplication();

            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };
            ////dt = LeaveApplicationResultSet.getSessionDatesByAcademicCalendar(DateTime.Now.Year);
            dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(User.Identity.Name);
            vm.SessionStartDate = dt[0];
            vm.SessionEndDate = dt[1];
            vm.strSessionStartDate = dt[0].ToString("dd MMM yyyy");
            vm.strSessionEndDate = dt[1].ToString("dd MMM yyyy");

            int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
            leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(User.Identity.Name);
            vm.AvailableSickLeaves = leaves[0];
            vm.AvailableCasualLeaves = leaves[1];
            vm.AvailableAnnualLeaves = leaves[2];
            vm.AvailedSickLeaves = leaves[3];
            vm.AvailedCasualLeaves = leaves[4];
            vm.AvailedAnnualLeaves = leaves[5];

            vm.leave_types = LeaveApplicationResultSet.getAllLeaveTypes();
            vm.leave_reasons = LeaveApplicationResultSet.getAllLeaveReasons();

            int iGVCampusID = 0; iGVCampusID = int.Parse(ViewModel.GlobalVariables.GV_EmployeeCampusID);
            vm.leave_approver = LeaveApplicationResultSet.GetApproverInfo(User.Identity.Name, iGVCampusID);

            return View(vm);
        }

        [HttpPost]
        public JsonResult LeaveApplicationDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<LeaveApplicationInfo>();
                dtSource = LeaveApplicationResultSet.getLeavesApplicationsByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<LeaveApplicationInfo> data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = LeaveApplicationResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<LeaveApplicationInfo> result = new DTResult<LeaveApplicationInfo>
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
        public JsonResult ApproveApplicationDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<LeaveApplicationInfo>();
                dtSource = LeaveApplicationResultSet.getApproveApplicationsByUserCodeForLM(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<LeaveApplicationInfo> data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);

                int count = LeaveApplicationResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<LeaveApplicationInfo> result = new DTResult<LeaveApplicationInfo>
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
        public ActionResult UpdateLeaveApplication(ViewModels.LeaveApplicationInfo toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            LeaveApplicationResultSet.update(toUpdate);
            TimeTune.AuditTrail.update(json, "LeaveApplication", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLeaveApplication(ViewModels.LeaveApplicationInfo toRemove)
        {
            var entity = LeaveApplicationResultSet.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            TimeTune.AuditTrail.delete(json, "LeaveApplication", User.Identity.Name);
            return Json(new { status = "success" });
        }

        //////////////////////////////////////////////////////////

        public ActionResult ApproveApplication()
        {
            return View();
        }

        /////////////////////////////////////////////////////////////
    }
}
