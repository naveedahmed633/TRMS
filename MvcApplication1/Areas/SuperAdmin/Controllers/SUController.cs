using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;

namespace MvcApplication1.Areas.SuperAdmin.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SUPER_USER)]
    public class SUController : Controller
    {


        //
        // GET: /SuperAdmin/SU/

        public ActionResult ConfigurationManager(string message)
        {
            if (!string.IsNullOrEmpty(message))
                ModelState.AddModelError("", message);

            //Read from Text File
            ViewBag.manualAttendanceStride = new BLL.DataFiles().getManualAttendanceStride() + "";

            //Read from DB
            ViewBag.manualAttendanceStrideDB = new BLL.DataFiles().getAccessCodeValue();
            ViewBag.AccessCodeDate = new BLL.DataFiles().getAccessCodeDate();

            return View();
        }

        [HttpPost]
        public ActionResult ChangeManualAttendanceStride()
        {
            bool response = false;
            string day = Request.Form["day"];
            int val = 0;

            if (!int.TryParse(day, out val))
            {
                return RedirectToAction("ConfigurationManager", new { message = "Invalid value" });
            }

            //Write to Text File
            new BLL.DataFiles().updateManualAttendanceStride(day);
            response = true;

            //Write to DB
            response = new BLL.DataFiles().updateAccessCodeValue(day);
            if (response == false)
            {
                ViewBag.Message01 = "<span style='color: red;'>Invalid Value Entered.</span>";
            }

            return RedirectToAction("ConfigurationManager");
        }

        [HttpGet]
        [ActionName("SmtpDetails")]
        public ActionResult SmtpDetails_Get()
        {
            string strSMTPHOST = "SMTP_HOST";
            string strSMTPPORT = "SMTP_PORT";
            string strSMTPEMAIL = "SMTP_EMAIL";
            string strSMTPPASSWORD = "SMTP_PASSWORD";
            string strSMTPTESTEMAIL = "SMTP_TEST_EMAIL";
            string strSMTPCCEMAIL = "SMTP_CC_EMAIL";
            string strSMTPBCCEMAIL = "SMTP_BCC_EMAIL";
            string strSMTPEMAILDELAY = "SMTP_EMAIL_DELAY";
            string strFORGOTPASSWORDLINK = "FORGOT_PASSWORD_LINK";
            string strVALIDATEINTERNETCONNECTION = "VALIDATE_INTERNET_CONNECTION";
            string strENABLESSL = "SMTP_ENABLE_SSL";
            string strACCESSVALIDATION = "ACCESS_VALIDATION";
            string strMINFLEXIMINUTES = "MIN_FLEXI_MINUTES";
            string strMAXFLEXIMINUTES = "MAX_FLEXI_MINUTES";
            string strSUPERADMINCODE = "SUPER_ADMIN_CODE";

            ViewBag.SmtpHost = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPHOST);
            ViewBag.SmtpPort = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPPORT);
            ViewBag.SmtpEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPEMAIL);
            ViewBag.SmtpPassword = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPPASSWORD);
            ViewBag.SmtpTestEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPTESTEMAIL);
            ViewBag.SmtpCCEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPCCEMAIL);
            ViewBag.SmtpBCCEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPBCCEMAIL);
            ViewBag.SmtpEmailDelay = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPEMAILDELAY);
            ViewBag.ForgotPasswordLink = new BLL.DataFiles().getAccessCodeInfoByIdentity(strFORGOTPASSWORDLINK);
            ViewBag.ValidateInternetConnection = new BLL.DataFiles().getAccessCodeInfoByIdentity(strVALIDATEINTERNETCONNECTION);
            ViewBag.EnableSSL = new BLL.DataFiles().getAccessCodeInfoByIdentity(strENABLESSL);
            ViewBag.AccessValidation = new BLL.DataFiles().getAccessCodeInfoByIdentity(strACCESSVALIDATION);
            ViewBag.MinFlexiMinutes = new BLL.DataFiles().getAccessCodeInfoByIdentity(strMINFLEXIMINUTES);
            ViewBag.MaxFlexiMinutes = new BLL.DataFiles().getAccessCodeInfoByIdentity(strMAXFLEXIMINUTES);
            ViewBag.SuperAdminCode = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSUPERADMINCODE);

            //ViewBag.TitleLeave01 = new BLL.DataFiles().getLeaveTypeTitleById(1);
            //ViewBag.TitleLeave02 = new BLL.DataFiles().getLeaveTypeTitleById(2);
            //ViewBag.TitleLeave03 = new BLL.DataFiles().getLeaveTypeTitleById(3);
            //ViewBag.TitleLeave04 = new BLL.DataFiles().getLeaveTypeTitleById(4);
            //ViewBag.TitleLeave05 = new BLL.DataFiles().getLeaveTypeTitleById(5);
            //ViewBag.TitleLeave06 = new BLL.DataFiles().getLeaveTypeTitleById(6);
            //ViewBag.TitleLeave07 = new BLL.DataFiles().getLeaveTypeTitleById(7);
            //ViewBag.TitleLeave08 = new BLL.DataFiles().getLeaveTypeTitleById(8);

            return View();
        }



        [HttpPost]
        [ActionName("SmtpDetails")]
        public ActionResult SmtpDetails_Post()
        {
            bool response = false;

            string strSMTPHOST = "SMTP_HOST";
            string strSMTPPORT = "SMTP_PORT";
            string strSMTPEMAIL = "SMTP_EMAIL";
            string strSMTPPASSWORD = "SMTP_PASSWORD";
            string strSMTPTESTEMAIL = "SMTP_TEST_EMAIL";
            string strSMTPCCEMAIL = "SMTP_CC_EMAIL";
            string strSMTPBCCEMAIL = "SMTP_BCC_EMAIL";
            string strSMTPEMAILDELAY = "SMTP_EMAIL_DELAY";
            string strFORGOTPASSWORDLINK = "FORGOT_PASSWORD_LINK";
            string strVALIDATEINTERNETCONNECTION = "VALIDATE_INTERNET_CONNECTION";
            string strENABLESSL = "SMTP_ENABLE_SSL";
            string strACCESSVALIDATION = "ACCESS_VALIDATION";
            string strMINFLEXIMINUTES = "MIN_FLEXI_MINUTES";
            string strMAXFLEXIMINUTES = "MAX_FLEXI_MINUTES";
            string strSUPERADMINCODE = "SUPER_ADMIN_CODE";

            string host = Request.Form["smtp_host"] ?? "smtp.gmail.com";//smtp-mail.outlook.com
            string port = Request.Form["smtp_port"] ?? "587";
            string smtpemail = Request.Form["smtp_email"] ?? "bams.no.reply@gmail.com";//hbl-no-reply@outlook.com
            string smtppassword = Request.Form["smtp_password"] ?? "Bams@1234";//Hbl@1234
            string testemail = Request.Form["test_email"] ?? "na";//test-email or na
            string ccemail = Request.Form["cc_email"] ?? "na";//test-email or na
            string bccemail = Request.Form["bcc_email"] ?? "na";//test-email or na
            string smtpemaildelay = Request.Form["smtp_email_delay"] ?? "1";
            string forgotpasswordlink = Request.Form["forgot_password_link"] ?? "1";
            string validateinternetconnection = Request.Form["validate_internet_connection"] ?? "0";
            string enablessl = Request.Form["enable_ssl"] ?? "1";
            string accessvalidation = Request.Form["access_validation"] ?? "";
            string minfleximinutes = Request.Form["min_flexi_minutes"] ?? "";
            string maxfleximinutes = Request.Form["max_flexi_minutes"] ?? "";
            string superadmincode = Request.Form["super_admin_code"] ?? "000000";

            // Open the file and write to it.
            response = new BLL.DataFiles().updateSmtpDetails(host, strSMTPHOST);
            response = new BLL.DataFiles().updateSmtpDetails(port, strSMTPPORT);
            response = new BLL.DataFiles().updateSmtpDetails(smtpemail, strSMTPEMAIL);
            response = new BLL.DataFiles().updateSmtpDetails(smtppassword, strSMTPPASSWORD);
            response = new BLL.DataFiles().updateSmtpDetails(testemail, strSMTPTESTEMAIL);
            response = new BLL.DataFiles().updateSmtpDetails(ccemail, strSMTPCCEMAIL);
            response = new BLL.DataFiles().updateSmtpDetails(bccemail, strSMTPBCCEMAIL);
            response = new BLL.DataFiles().updateSmtpDetails(smtpemaildelay, strSMTPEMAILDELAY);
            response = new BLL.DataFiles().updateSmtpDetails(forgotpasswordlink, strFORGOTPASSWORDLINK);
            response = new BLL.DataFiles().updateSmtpDetails(validateinternetconnection, strVALIDATEINTERNETCONNECTION);
            response = new BLL.DataFiles().updateSmtpDetails(enablessl, strENABLESSL);
            response = new BLL.DataFiles().updateSmtpDetails(accessvalidation, strACCESSVALIDATION);
            response = new BLL.DataFiles().updateSmtpDetails(minfleximinutes, strMINFLEXIMINUTES);
            response = new BLL.DataFiles().updateSmtpDetails(maxfleximinutes, strMAXFLEXIMINUTES);
            response = new BLL.DataFiles().updateSmtpDetails(superadmincode, strSUPERADMINCODE);

            //string strLeaveTitle01 = Request.Form["title_leave_01"] ?? "Sick";
            //string strLeaveTitle02 = Request.Form["title_leave_02"] ?? "Casual";
            //string strLeaveTitle03 = Request.Form["title_leave_03"] ?? "Annual";
            //string strLeaveTitle04 = Request.Form["title_leave_04"] ?? "Other";
            //string strLeaveTitle05 = Request.Form["title_leave_05"] ?? "Tour";
            //string strLeaveTitle06 = Request.Form["title_leave_06"] ?? "Visit";
            //string strLeaveTitle07 = Request.Form["title_leave_07"] ?? "Meeting";
            //string strLeaveTitle08 = Request.Form["title_leave_08"] ?? "Maternity";

            //response = new BLL.DataFiles().updateLeaveTitleById(1, strLeaveTitle01);
            //response = new BLL.DataFiles().updateLeaveTitleById(2, strLeaveTitle02);
            //response = new BLL.DataFiles().updateLeaveTitleById(3, strLeaveTitle03);
            //response = new BLL.DataFiles().updateLeaveTitleById(4, strLeaveTitle04);
            //response = new BLL.DataFiles().updateLeaveTitleById(5, strLeaveTitle05);
            //response = new BLL.DataFiles().updateLeaveTitleById(6, strLeaveTitle06);
            //response = new BLL.DataFiles().updateLeaveTitleById(7, strLeaveTitle07);
            //response = new BLL.DataFiles().updateLeaveTitleById(8, strLeaveTitle08);

            if (response == false)
            {
                ViewBag.Message01 = "<span style='color: red;'>Invalid Entry</span>";
            }
            else
            {
                ViewBag.Message01 = "<span style='color: green;'>Updated Successfully!!!</span>";
            }

            ViewBag.SmtpHost = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPHOST);
            ViewBag.SmtpPort = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPPORT);
            ViewBag.SmtpEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPEMAIL);
            ViewBag.SmtpPassword = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPPASSWORD);
            ViewBag.SmtpTestEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPTESTEMAIL);
            ViewBag.SmtpCCEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPCCEMAIL);
            ViewBag.SmtpBCCEmail = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPBCCEMAIL);
            ViewBag.SmtpEmailDelay = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSMTPEMAILDELAY);
            ViewBag.ForgotPasswordLink = new BLL.DataFiles().getAccessCodeInfoByIdentity(strFORGOTPASSWORDLINK);
            ViewBag.ValidateInternetConnection = new BLL.DataFiles().getAccessCodeInfoByIdentity(strVALIDATEINTERNETCONNECTION);
            ViewBag.EnableSSL = new BLL.DataFiles().getAccessCodeInfoByIdentity(strENABLESSL);
            ViewBag.AccessValidation = new BLL.DataFiles().getAccessCodeInfoByIdentity(strACCESSVALIDATION);

            ViewBag.MinFlexiMinutes = new BLL.DataFiles().getAccessCodeInfoByIdentity(strMINFLEXIMINUTES);
            ViewBag.MaxFlexiMinutes = new BLL.DataFiles().getAccessCodeInfoByIdentity(strMAXFLEXIMINUTES);
            ViewBag.SuperAdminCode = new BLL.DataFiles().getAccessCodeInfoByIdentity(strSUPERADMINCODE);

            //ViewBag.TitleLeave01 = new BLL.DataFiles().getLeaveTypeTitleById(1);
            //ViewBag.TitleLeave02 = new BLL.DataFiles().getLeaveTypeTitleById(2);
            //ViewBag.TitleLeave03 = new BLL.DataFiles().getLeaveTypeTitleById(3);
            //ViewBag.TitleLeave04 = new BLL.DataFiles().getLeaveTypeTitleById(4);
            //ViewBag.TitleLeave05 = new BLL.DataFiles().getLeaveTypeTitleById(5);
            //ViewBag.TitleLeave06 = new BLL.DataFiles().getLeaveTypeTitleById(6);
            //ViewBag.TitleLeave07 = new BLL.DataFiles().getLeaveTypeTitleById(7);
            //ViewBag.TitleLeave08 = new BLL.DataFiles().getLeaveTypeTitleById(8);

            return View("SmtpDetails");
        }

        [HttpPost]
        public async Task<ActionResult> SmtpSendTestEmail()
        {
            string rspMessage = string.Empty;

            rspMessage = await BLL.ADMIN.EmailHelper.SendTestEmailForVerification();

            if (rspMessage == "success")
            {
                ViewBag.MessageEmail = "<span style='color: green;'>The email is sent successfully!!! <a style='display: inline-block; text-decoration: underline; font-weight: bold;' href='/SuperAdmin/SU/SmtpDetails'>Back</a></span>";
            }
            else
            {
                ViewBag.MessageEmail = "<span style='color: red;'>" + rspMessage + "<br><a style='display: inline-block; text-decoration: underline; font-weight: bold;' href='/SuperAdmin/SU/SmtpDetails'>Back</a></span>";
            }

            return View("SmtpDetails");
        }

        [HttpGet]
        [ActionName("ArchiveTables")]
        public ActionResult ArchiveTables_Get()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ArchiveTables")]
        public ActionResult ArchiveTables_Post()
        {
            long count = 0;
            string fromdate = "", todate = "";
            CultureInfo ci = new CultureInfo("en-us");

            fromdate = Request.Form["FromDate"] ?? "";
            todate = Request.Form["ToDate"] ?? "";

            count = new BLL.DataFiles().ArchiveTablesByDateRange(fromdate, todate);
            if (count == -1)
            {
                ViewBag.Message01 = "<span id='alert_msg' style='color: red;'>An error occurred.</span>";
            }
            else if (count == -2)
            {
                ViewBag.Message01 = "<span id='alert_msg' style='color: red;'>No date range selected.</span>";
            }
            else if (count == 0)
            {
                ViewBag.Message01 = "<span id='alert_msg' style='color: red;'>No data found to archive it.</span>";
            }
            else
            {
                ViewBag.Message01 = "<span id='alert_msg' style='color: green;'>" + count.ToString("N0", ci) + " record(s) - Archived Successfully!!!</span>";
            }

            return View();
        }


        [HttpPost]
        public ActionResult ChangeManualAttendanceDate()
        {
            bool response = false;
            string day = Request.Form["date_to"];

            response = new BLL.DataFiles().updateAccessCodeDate(day);
            if (response == false)
            {
                ViewBag.Message02 = "<span style='color: red;'>Invalid Value Entered.</span>";
            }

            return RedirectToAction("ConfigurationManager");
        }


        public ActionResult ViewAuditLog()
        {
            return View();
        }

        //

        [HttpPost]
        public JsonResult AuditLogDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ViewModels.ReportAuditLog>();

                // get all audit view models
                int count = TimeTune.Reports.getAllAuditLogsMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ReportAuditLog> result = new DTResult<ReportAuditLog>
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



        public ActionResult ConsolidateAttendanceArchive()
        {
            return View();
        }


        public class ConsolidatedReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        [HttpPost]
        public JsonResult ConsolidatedAtDataHandler(ConsolidatedReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceArchiveLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllConsolidateAttendanceMatchingArchive(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ConsolidatedAttendanceArchiveLog> result = new DTResult<ConsolidatedAttendanceArchiveLog>
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
        public JsonResult ChangeEmployeePasswordDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].id.ToString();
                toSend[i].text = employees[i].employee_code + " - " + employees[i].first_name + " " + employees[i].last_name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        #region UserControl
        public ActionResult UserControl(string result)
        {

            string value = result;
            ViewBag.Message = result;
            // only access groups are to be sent without ajax.
            CreateEmployee createEmployeeViewModel = new CreateEmployee();
            createEmployeeViewModel.accessGroups = EmployeeAccessGroup.getAllButHr();
            createEmployeeViewModel.skillSets = EmployeeAccessGroup.getAllSkillSets();

            return View(createEmployeeViewModel);
        }


        [HttpPost]
        public ActionResult AddPermissionUser(ViewModels.PermissionUser fromForm)
        {

            int id = TimeTune.User_Permission.add(fromForm);
            return RedirectToAction("UserControl", new { message = "success" });
        }

        [HttpPost]
        public JsonResult DataHandler(string empId)
        {
            try
            {
                var dtsource = new List<Employee>();
                var data = TimeTune.User_Permission.verfiyReturn(empId);

                return Json(data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }


        #endregion

        #region AdjustAttendance

        public ActionResult AdjustAttendance()
        {
            return View();
        }

        public class AdjustAttendanceReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult AdjustAttendanceReportDataHandler(AdjustAttendanceReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllConsolidateAttendanceForAttendanceAdjustment(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ConsolidatedAttendanceLog> result = new DTResult<ConsolidatedAttendanceLog>
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
        public ActionResult AdjustAttendanceUpdate(ViewModels.ConsolidatedAttendanceLog toUpdate)
        {
            //var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.UpdateAttendance.Update(toUpdate, User.Identity.Name);
            return Json(new { status = "success" });
        }


        #endregion


        #region ManualAttLog 
        public ActionResult ManualAttendanceLogs()
        {
            return View();
        }

        //

        [HttpPost]
        public JsonResult ManualAttLogDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ViewModels.ManualAttLog>();

                // get all manual attendance audit view models
                int count = TimeTune.Reports.getAllManualAttendanceLogsMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ManualAttLog> result = new DTResult<ManualAttLog>
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

        #endregion


        #region OrganizationConfiguration

        public ActionResult OrgSetting()
        {
            ViewModels.OrgInformation dta = new ViewModels.OrgInformation();
            dta = TimeTune.Org_Setting.ReturnOrgInfo();

            ViewBag.OrgName = dta.OrgName;
            ViewBag.OrgEmail = dta.OrgEmail;
            ViewBag.OrgContact = dta.OrgContact;

            ViewBag.OrgAddress = dta.OrgAddress;
            ViewBag.OrgWebsite = dta.OrgWebsite;
            ViewBag.OrgLogo = dta.OrgLogo;

            return View();
        }

        [HttpGet]
        public JsonResult SuperAdminModuleRight()
        {
            try
            {
                var data = TimeTune.Org_Setting.ReturnSURights();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpPost]
        public ActionResult AddPermissionSU(ViewModels.PermissionSuperAdmin fromForm)
        {

            TimeTune.Org_Setting.update(fromForm);
            return RedirectToAction("OrgSetting", new { message = "success" });
        }


        [HttpPost]
        public ActionResult UpdateOrgInfo(ViewModels.OrgInformation fromForm)
        {
            TimeTune.Org_Setting.updateOrgInfo(fromForm);
            return RedirectToAction("OrgSetting", new { message = "success" });
        }

        #endregion


        #region DevicesTrackingReportHR

        public ActionResult DevicesTrackingReportHR()
        {
            return View();
        }

        public class DevicesTrackingReportTable : DTParameters
        {
            public string from_date { get; set; }
        }

        [HttpPost]
        public JsonResult DevicesTrackingReportHRDataHandler(DevicesTrackingReportTable param)
        {
            try
            {
                List<DevicesTracking> data = new List<DevicesTracking>();

                data = TimeTune.Reports.getDevicesTrackingList(param.from_date);

                List<DevicesTracking> data2 = DeviceTrackingResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<DevicesTracking> result = new DTResult<DevicesTracking>
                {
                    draw = param.Draw,
                    data = data2,
                    recordsFiltered = data.Count,
                    recordsTotal = data.Count
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

        #region Report-Control
        public ActionResult ReportControl(string result)
        {

            string value = result;
            ViewBag.Message = result;
            // only access groups are to be sent without ajax.
            CreateEmployee createEmployeeViewModel = new CreateEmployee();
            createEmployeeViewModel.accessGroups = EmployeeAccessGroup.getAllButHr();
            createEmployeeViewModel.skillSets = EmployeeAccessGroup.getAllSkillSets();

            return View(createEmployeeViewModel);
        }


        [HttpPost]
        public ActionResult AddReportPermissionUser(ViewModels.PermissionUser fromForm)
        {

            int id = TimeTune.Report_Permission.add(fromForm);
            return RedirectToAction("ReportControl", new { message = "success" });
        }

        [HttpPost]
        public JsonResult ReportControlDataHandler(string empId)
        {
            try
            {
                var dtsource = new List<Employee>();
                var data = TimeTune.User_Permission.verfiyReturn(empId);

                return Json(data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }


        #endregion
    }
}
