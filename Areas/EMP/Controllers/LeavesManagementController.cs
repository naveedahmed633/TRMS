using iTextSharp.text;
using iTextSharp.text.pdf;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;

namespace MvcApplication1.Areas.EMP.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_EMP)]
    public class LeavesManagementController : Controller
    {
        //[ActionName("LeaveApplication")]
        public ActionResult LeaveApplication()
        {
            // only access groups are to be sent without ajax.
            CreateLeaveApplication vm = new CreateLeaveApplication();

            DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };
            ////dt = LeaveApplicationResultSet.getSessionDatesByAcademicCalendar(DateTime.Now.Year);
            dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(User.Identity.Name);
            vm.SessionStartDate = dt[0];
            vm.SessionEndDate = dt[1];
            vm.strSessionStartDate = dt[0].ToString("dd-MM-yyyy");
            vm.strSessionEndDate = dt[1].ToString("dd-MM-yyyy");

            int[] leaves = new int[30] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(User.Identity.Name);
            vm.AvailableSickLeaves = leaves[0];
            vm.AvailableCasualLeaves = leaves[1];
            vm.AvailableAnnualLeaves = leaves[2];
            vm.AvailableOtherLeaves = leaves[6];
            vm.AvailableLeaveType01 = leaves[8];
            vm.AvailableLeaveType02 = leaves[9];
            vm.AvailableLeaveType03 = leaves[10];
            vm.AvailableLeaveType04 = leaves[11];
            vm.AvailableLeaveType05 = leaves[16];
            vm.AvailableLeaveType06 = leaves[17];
            vm.AvailableLeaveType07 = leaves[18];
            vm.AvailableLeaveType08 = leaves[19];
            vm.AvailableLeaveType09 = leaves[20];
            vm.AvailableLeaveType10 = leaves[21];
            vm.AvailableLeaveType11 = leaves[22];

            vm.AvailedSickLeaves = leaves[3];
            vm.AvailedCasualLeaves = leaves[4];
            vm.AvailedAnnualLeaves = leaves[5];
            vm.AvailedOtherLeaves = leaves[7];
            vm.AvailedLeaveType01 = leaves[12];
            vm.AvailedLeaveType02 = leaves[13];
            vm.AvailedLeaveType03 = leaves[14];
            vm.AvailedLeaveType04 = leaves[15];
            vm.AvailedLeaveType05 = leaves[23];
            vm.AvailedLeaveType06 = leaves[24];
            vm.AvailedLeaveType07 = leaves[25];
            vm.AvailedLeaveType08 = leaves[26];
            vm.AvailedLeaveType09 = leaves[27];
            vm.AvailedLeaveType10 = leaves[28];
            vm.AvailedLeaveType11 = leaves[29];

            //vm.leave_types = LeaveApplicationResultSet.getAllLeaveTypes();
            vm.leave_types = LeaveApplicationResultSet.getAllLeaveTypesByEmployeeCode(User.Identity.Name);
            vm.leave_reasons = LeaveApplicationResultSet.getAllLeaveReasons();


            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);
            vm.leave_approver = LeaveApplicationResultSet.GetApproverInfo(User.Identity.Name, iGVCampusID);

            UpdateLeavesCountSessions();

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
                UpdateLeavesCountSessions();

                if (attachment_file_path != null && attachment_file_path.ContentLength > 0)
                {
                    if (attachment_file_path.ContentLength > (2 * 1024 * 1024))
                    {
                        return RedirectToAction("LeaveApplication", new { message = "image" });
                    }

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
                //    LeaveApplicationResultSet.GetApproverUserID(User.Identity.Name);

                if (ldata.leave_status_id > 0)
                    lAppInfo.LeaveStatusId = ldata.leave_status_id;
                else
                    lAppInfo.LeaveStatusId = 1;//pending

                lAppInfo.AttachmentFilePath = ldata.attachment_file_path;

                lAppInfo.IsActive = true;

                lAppInfo.CreateDateTime = DateTime.Now;
                lAppInfo.UpdateDateTime = DateTime.Now;

                //Titles - Leave Types
                string TitleLeaves = "", TitleLeave01 = "", TitleLeave02 = "", TitleLeave03 = "", TitleLeave04 = "", TitleLeave05 = "", TitleLeave06 = "", TitleLeave07 = "", TitleLeave08 = "";
                string TitleLeave09 = "", TitleLeave10 = "", TitleLeave11 = "", TitleLeave12 = "", TitleLeave13 = "", TitleLeave14 = "", TitleLeave15 = "";
                BLL.PdfReports.LeavesTypesTitles leavesTitles = new BLL.PdfReports.LeavesTypesTitles();
                TitleLeaves = leavesTitles.getLeavesTypesTitles();
                if (TitleLeaves != null && TitleLeaves.Contains(','))
                {
                    string[] strSplit = TitleLeaves.Split(',');
                    if (strSplit != null && strSplit.Length > 0)
                    {
                        TitleLeave01 = strSplit[0].ToString(); TitleLeave02 = strSplit[1].ToString(); TitleLeave03 = strSplit[2].ToString(); TitleLeave04 = strSplit[3].ToString();
                        TitleLeave05 = strSplit[4].ToString(); TitleLeave06 = strSplit[5].ToString(); TitleLeave07 = strSplit[6].ToString(); TitleLeave08 = strSplit[7].ToString();
                        TitleLeave09 = strSplit[8].ToString(); TitleLeave10 = strSplit[9].ToString(); TitleLeave11 = strSplit[10].ToString(); TitleLeave12 = strSplit[11].ToString();
                        TitleLeave13 = strSplit[12].ToString(); TitleLeave14 = strSplit[13].ToString(); TitleLeave15 = strSplit[14].ToString();
                    }
                }
                else
                {
                    TitleLeave01 = "Sick"; TitleLeave02 = "Casual"; TitleLeave03 = "Annual"; TitleLeave04 = "Other";
                    TitleLeave05 = "Tour"; TitleLeave06 = "Visit"; TitleLeave07 = "Meeting"; TitleLeave08 = "Maternity";
                    TitleLeave09 = "A"; TitleLeave10 = "B"; TitleLeave11 = "C"; TitleLeave12 = "D";
                    TitleLeave13 = "E"; TitleLeave14 = "F"; TitleLeave15 = "G";
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////

                int iCreatedId = LeaveApplicationResultSet.AddNewLeaveApplication(lAppInfo);
                if (iCreatedId > 0)
                {
                    //success
                    ViewBag.Message = "The Leave Application is submitted successfully!";

                    var json = JsonConvert.SerializeObject(ldata);
                    AuditTrail.insert(json, "LeaveApplication", User.Identity.Name);

                }
                else if (iCreatedId == -1)
                {
                    //error
                    ViewBag.Message = "An error occurred.";
                }
                else if (iCreatedId == -2)
                {
                    //error
                    ViewBag.Message = "You have already applied for same day(s) leave.";
                }
                else if (iCreatedId == -21)
                {
                    //error
                    ViewBag.Message = "You have already applied for this DATE leave previously.";
                }
                else if (iCreatedId == -22)
                {
                    //error
                    ViewBag.Message = "You have already applied for this DATE leave previously.";
                }
                //else if (iCreatedId == -25)
                //{
                //    //error
                //    ViewBag.Message = "You have already applied for this DATE leave and its Pending.";
                //}
                //else if (iCreatedId == -26)
                //{
                //    //error
                //    ViewBag.Message = "You have already applied for this DATE leave previously.";
                //}
                //else if (iCreatedId == -27)
                //{
                //    //error
                //    ViewBag.Message = "You have already applied for this DATE 'Cancel' leave previously.";
                //}
                else if (iCreatedId == -3)
                {
                    //error
                    ViewBag.Message = "'From Date' should not be greater than 'To Date'.";
                }
                else if (iCreatedId == -4)
                {
                    //error
                    ViewBag.Message = "Apply within same month date-range. For next month apply leave separately.";
                }
                else if (iCreatedId == -5)
                {
                    //error
                    ViewBag.Message = "You cannot apply as much days back/before your leave 'From Date'.";
                }
                else if (iCreatedId == -6)
                {
                    //error  Sick
                    ViewBag.Message = "'" + TitleLeave01 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -7)
                {
                    //error - Casual
                    ViewBag.Message = "'" + TitleLeave02 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -8)
                {
                    //error - Annual
                    ViewBag.Message = "'" + TitleLeave03 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -9)
                {
                    //error - Other
                    ViewBag.Message = "'" + TitleLeave04 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -10)
                {
                    //error - Tour
                    ViewBag.Message = "'" + TitleLeave05 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -11)
                {
                    //error - Visit
                    ViewBag.Message = "'" + TitleLeave06 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -12)
                {
                    //error - Meeting
                    ViewBag.Message = "'" + TitleLeave07 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -13)
                {
                    //error - Maternity
                    ViewBag.Message = "'" + TitleLeave08 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -14)
                {
                    //error - Casual
                    ViewBag.Message = "'" + TitleLeave09 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -15)
                {
                    //error - Annual
                    ViewBag.Message = "'" + TitleLeave10 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -16)
                {
                    //error - Other
                    ViewBag.Message = "'" + TitleLeave11 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -17)
                {
                    //error - Tour
                    ViewBag.Message = "'" + TitleLeave12 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -18)
                {
                    //error - Visit
                    ViewBag.Message = "'" + TitleLeave13 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -19)
                {
                    //error - Meeting
                    ViewBag.Message = "'" + TitleLeave14 + " Leaves' limit exceeding as not much left.";
                }
                else if (iCreatedId == -20)
                {
                    //error - Maternity
                    ViewBag.Message = "'" + TitleLeave15 + " Leaves' limit exceeding as not much left.";
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
            dt = LeaveApplicationResultSet.getStudentSessionDatesByUserCode(User.Identity.Name);
            vm.SessionStartDate = dt[0];
            vm.SessionEndDate = dt[1];
            vm.strSessionStartDate = dt[0].ToString("dd-MM-yyyy");
            vm.strSessionEndDate = dt[1].ToString("dd-MM-yyyy");

            int[] leaves = new int[30] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(User.Identity.Name);
            vm.AvailableSickLeaves = leaves[0];
            vm.AvailableCasualLeaves = leaves[1];
            vm.AvailableAnnualLeaves = leaves[2];
            vm.AvailableOtherLeaves = leaves[6];
            vm.AvailableLeaveType01 = leaves[8];
            vm.AvailableLeaveType02 = leaves[9];
            vm.AvailableLeaveType03 = leaves[10];
            vm.AvailableLeaveType04 = leaves[11];
            vm.AvailableLeaveType05 = leaves[16];
            vm.AvailableLeaveType06 = leaves[17];
            vm.AvailableLeaveType07 = leaves[18];
            vm.AvailableLeaveType08 = leaves[19];
            vm.AvailableLeaveType09 = leaves[20];
            vm.AvailableLeaveType10 = leaves[21];
            vm.AvailableLeaveType11 = leaves[22];

            vm.AvailedSickLeaves = leaves[3];
            vm.AvailedCasualLeaves = leaves[4];
            vm.AvailedAnnualLeaves = leaves[5];
            vm.AvailedOtherLeaves = leaves[7];
            vm.AvailedLeaveType01 = leaves[12];
            vm.AvailedLeaveType02 = leaves[13];
            vm.AvailedLeaveType03 = leaves[14];
            vm.AvailedLeaveType04 = leaves[15];
            vm.AvailedLeaveType05 = leaves[23];
            vm.AvailedLeaveType06 = leaves[24];
            vm.AvailedLeaveType07 = leaves[25];
            vm.AvailedLeaveType08 = leaves[26];
            vm.AvailedLeaveType09 = leaves[27];
            vm.AvailedLeaveType10 = leaves[28];
            vm.AvailedLeaveType11 = leaves[29];

            //vm.leave_types = LeaveApplicationResultSet.getAllLeaveTypes();
            vm.leave_types = LeaveApplicationResultSet.getAllLeaveTypesByEmployeeCode(User.Identity.Name);
            vm.leave_reasons = LeaveApplicationResultSet.getAllLeaveReasons();

            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);
            vm.leave_approver = LeaveApplicationResultSet.GetApproverInfo(User.Identity.Name, iGVCampusID);

            UpdateLeavesCountSessions();

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
        public ActionResult UpdateLeaveApplication(ViewModels.LeaveApplicationInfo toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            LeaveApplicationResultSet.update(toUpdate);
            AuditTrail.update(json, "LeaveApplication", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLeaveApplication(ViewModels.LeaveApplicationInfo toRemove)
        {
            var entity = LeaveApplicationResultSet.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "LeaveApplication", User.Identity.Name);
            return Json(new { status = "success" });
        }

        public void UpdateLeavesCountSessions()
        {
            //////////////////////leaves counter - get all employee view models /////////////////////////////////////////
            int session_year = DateTime.Now.Year;
            var data = new List<ViewModels.LeavesCountReportLog>();

            if (ViewModel.GlobalVariables.GV_EmployeeId != null && ViewModel.GlobalVariables.GV_EmployeeId != "")
            {
                System.Collections.ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(int.Parse(ViewModel.GlobalVariables.GV_EmployeeId));
                session_year = int.Parse(yearList[0].ToString());
                TimeTune.Attendance.getLeavesCountReportByEmpCode(ViewModel.GlobalVariables.GV_EmployeeCode, session_year.ToString(), out data);

                if (data != null && data.Count > 0)
                {
                    foreach (ViewModels.LeavesCountReportLog item in data)
                    {
                        Session["GV_AllocatedSickLeaves"] = item.AllocatedSickLeaves;
                        Session["GV_AllocatedCasualLeaves"] = item.AllocatedCasualLeaves;
                        Session["GV_AllocatedAnnualLeaves"] = item.AllocatedAnnualLeaves;
                        Session["GV_AllocatedOtherLeaves"] = item.AllocatedOtherLeaves;
                        Session["GV_AllocatedLeaveType01"] = item.AllocatedLeaveType01;
                        Session["GV_AllocatedLeaveType02"] = item.AllocatedLeaveType02;
                        Session["GV_AllocatedLeaveType03"] = item.AllocatedLeaveType03;
                        Session["GV_AllocatedLeaveType04"] = item.AllocatedLeaveType04;
                        Session["GV_AllocatedLeaveType05"] = item.AllocatedLeaveType05;
                        Session["GV_AllocatedLeaveType06"] = item.AllocatedLeaveType06;
                        Session["GV_AllocatedLeaveType07"] = item.AllocatedLeaveType07;
                        Session["GV_AllocatedLeaveType08"] = item.AllocatedLeaveType08;
                        Session["GV_AllocatedLeaveType09"] = item.AllocatedLeaveType09;
                        Session["GV_AllocatedLeaveType10"] = item.AllocatedLeaveType10;
                        Session["GV_AllocatedLeaveType11"] = item.AllocatedLeaveType11;

                        Session["GV_AvailedSickLeaves"] = item.AvailedSickLeaves;
                        Session["GV_AvailedCasualLeaves"] = item.AvailedCasualLeaves;
                        Session["GV_AvailedAnnualLeaves"] = item.AvailedAnnualLeaves;
                        Session["GV_AvailedOtherLeaves"] = item.AvailedOtherLeaves;
                        Session["GV_AvailedLeaveType01"] = item.AvailedLeaveType01;
                        Session["GV_AvailedLeaveType02"] = item.AvailedLeaveType02;
                        Session["GV_AvailedLeaveType03"] = item.AvailedLeaveType03;
                        Session["GV_AvailedLeaveType04"] = item.AvailedLeaveType04;
                        Session["GV_AvailedLeaveType05"] = item.AvailedLeaveType05;
                        Session["GV_AvailedLeaveType06"] = item.AvailedLeaveType06;
                        Session["GV_AvailedLeaveType07"] = item.AvailedLeaveType07;
                        Session["GV_AvailedLeaveType08"] = item.AvailedLeaveType08;
                        Session["GV_AvailedLeaveType09"] = item.AvailedLeaveType09;
                        Session["GV_AvailedLeaveType10"] = item.AvailedLeaveType10;
                        Session["GV_AvailedLeaveType11"] = item.AvailedLeaveType11;
                    }
                }
                else
                {
                    Session["GV_AllocatedSickLeaves"] = 0;
                    Session["GV_AllocatedCasualLeaves"] = 0;
                    Session["GV_AllocatedAnnualLeaves"] = 0;
                    Session["GV_AllocatedOtherLeaves"] = 0;
                    Session["GV_AllocatedLeaveType01"] = 0;
                    Session["GV_AllocatedLeaveType02"] = 0;
                    Session["GV_AllocatedLeaveType03"] = 0;
                    Session["GV_AllocatedLeaveType04"] = 0;
                    Session["GV_AllocatedLeaveType05"] = 0;
                    Session["GV_AllocatedLeaveType06"] = 0;
                    Session["GV_AllocatedLeaveType07"] = 0;
                    Session["GV_AllocatedLeaveType08"] = 0;
                    Session["GV_AllocatedLeaveType09"] = 0;
                    Session["GV_AllocatedLeaveType10"] = 0;
                    Session["GV_AllocatedLeaveType11"] = 0;

                    Session["GV_AvailedSickLeaves"] = 0;
                    Session["GV_AvailedCasualLeaves"] = 0;
                    Session["GV_AvailedAnnualLeaves"] = 0;
                    Session["GV_AvailedOtherLeaves"] = 0;
                    Session["GV_AvailedLeaveType01"] = 0;
                    Session["GV_AvailedLeaveType02"] = 0;
                    Session["GV_AvailedLeaveType03"] = 0;
                    Session["GV_AvailedLeaveType04"] = 0;
                    Session["GV_AvailedLeaveType05"] = 0;
                    Session["GV_AvailedLeaveType06"] = 0;
                    Session["GV_AvailedLeaveType07"] = 0;
                    Session["GV_AvailedLeaveType08"] = 0;
                    Session["GV_AvailedLeaveType09"] = 0;
                    Session["GV_AvailedLeaveType10"] = 0;
                    Session["GV_AvailedLeaveType11"] = 0;
                }
            }
            ////////////////////////////////////////////////////////////
        }
    }
}
