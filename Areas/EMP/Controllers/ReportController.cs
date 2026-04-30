using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data;
using OfficeOpenXml;
using System.Reflection;
using System.Configuration;
using System.Collections;
using Newtonsoft.Json;
using TimeTune;
using BLL_UNIS.ViewModels;
using MvcApplication1.ViewModel;
using System.Globalization;

namespace MvcApplication1.Areas.EMP.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_EMP)]
    public class ReportController : Controller
    {
        private string GetCurrentLang()
        {
            string requestLang = Request?["lang"] ?? Request?["GV_Langauge"];
            if (!string.IsNullOrWhiteSpace(requestLang))
            {
                GlobalVariables.GV_Langauge = requestLang.Equals("ar", StringComparison.OrdinalIgnoreCase) ? "Ar" : "En";
            }

            return GlobalVariables.GetCurrentLanguage();
        }

        private Font GetFont(bool isBold = false, float size = 8f, Color color = null)
        {
            string fontPath = Environment.GetEnvironmentVariable("windir") + @"\fonts\Arial.ttf";
            BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, true);
            int style = isBold ? Font.BOLD : Font.NORMAL;
            bool isArabic = string.Equals(GetCurrentLang(), "Ar", StringComparison.OrdinalIgnoreCase);
            float adjustedSize = isArabic ? size : Math.Max(6.5f, size - 0.5f);
            return new Font(bf, adjustedSize, style, color ?? Color.BLACK);
        }

        private int GetPdfRunDirection()
        {
            return string.Equals(GetCurrentLang(), "Ar", StringComparison.OrdinalIgnoreCase)
                ? PdfWriter.RUN_DIRECTION_RTL
                : PdfWriter.RUN_DIRECTION_LTR;
        }

        private int GetPdfDefaultHorizontalAlignment()
        {
            return string.Equals(GetCurrentLang(), "Ar", StringComparison.OrdinalIgnoreCase)
                ? Element.ALIGN_RIGHT
                : Element.ALIGN_LEFT;
        }

        private string GetCurrentUserCode()
        {
            if (!string.IsNullOrWhiteSpace(GlobalVariables.GV_EmployeeCode))
            {
                return GlobalVariables.GV_EmployeeCode.Trim();
            }

            return string.IsNullOrWhiteSpace(User?.Identity?.Name) ? "000000" : User.Identity.Name.Trim();
        }

        private string BuildPdfFileName(string reportName)
        {
            return string.Format("{0}-Report-{1}.pdf", reportName, GetCurrentUserCode());
        }

        #region ConsolidatedAttendance

        public ActionResult ConsolidatedAttendance()
        {
            var consolidatedAttendanceLog = TimeTune.Attendance.getConsolidatedLog(User.Identity.Name);
            return View(consolidatedAttendanceLog);
        }

        public ActionResult ConsolidatedAttendanceOvertime()
        {
            var consolidatedAttendanceLog = TimeTune.Attendance.getConsolidatedLog(User.Identity.Name);
            return View(consolidatedAttendanceLog);
        }

        public class ConsolidatedReportTable : DTParameters
        {

            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }
        [HttpPost]
        public JsonResult ConsolidatedReportDataHandler(ConsolidatedReportTable param, string from_date, string to_date)
        {
            try
            {


                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models
                TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date, param.to_date, User.Identity.Name, param.SortOrder, param.Start, param.Length, out data);
                int count = data.Count;
                //int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);
                //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                //Aternname solutoin taken from HR
                ////int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeId, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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
        public JsonResult graphForConsolidated(ConsolidatedReportTable param)
        {
            try
            {
                ViewModels.Dashboard dashboard = TimeTune.Dashboard.graphForConsolidatedForLm(User.Identity.Name, param.from_date, param.to_date);
                return Json(dashboard);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult ConsolidatedReportOvertimeExcelDownload(ConsolidatedReportTable param)
        {
            var ddata = new List<ConsolidatedAttendanceExportOvertime>();
            string handle = Guid.NewGuid().ToString();

            string employeeID = TimeTune.EmpReports.getEmployeeID(User.Identity.Name.ToString()).ToString();

            try
            {
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatchingOvertime(employeeID, param.from_date, param.to_date, out ddata);

                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<ConsolidatedAttendanceExportOvertime>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("ConsolidatedAttendanceLogOvertime");
                    var totalCols = products.Columns.Count;
                    var totalRows = products.Rows.Count;

                    for (var col = 1; col <= totalCols; col++)
                    {
                        workSheet.Cells[1, col].Value = products.Columns[col - 1].ColumnName.Replace("_", " ");
                    }

                    for (var row = 1; row <= totalRows; row++)
                    {
                        for (var col = 0; col < totalCols; col++)
                        {
                            workSheet.Cells[row + 1, col + 1].Value = products.Rows[row - 1][col];
                        }
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);

                        memoryStream.Position = 0;
                        TempData[handle] = memoryStream.ToArray();
                    }
                }
                else
                {
                    return new JsonResult()
                    {
                        Data = new { FileGuid = "", FileName = "" }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = "Consolidated-Attendance-Log-Overtime.xlsx" }
            };
        }

        #endregion

        #region LeavesCountReport

        [HttpGet]
        [ActionName("LeavesCountReport")]
        public ActionResult LeavesCountReport_Get()
        {
            int session_year = DateTime.Now.Year;
            var data = new List<LeavesCountReportLog>();
            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();

            if (ViewModel.GlobalVariables.GV_EmployeeId != null && ViewModel.GlobalVariables.GV_EmployeeId != "")
            {
                ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(int.Parse(ViewModel.GlobalVariables.GV_EmployeeId));
                ViewBag.SessionYearsList = yearList;
                session_year = int.Parse(yearList[0].ToString());
                ViewBag.SelectedSessionYear = session_year;

                // get all employee view models
                TimeTune.Attendance.getLeavesCountReportByEmpCode(User.Identity.Name, session_year.ToString(), out data);
                // int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);

                //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                foreach (LeavesCountReportLog item in data)
                {
                    ViewBag.AllocatedSickLeaves = item.AllocatedSickLeaves;
                    ViewBag.AllocatedCasualLeaves = item.AllocatedCasualLeaves;
                    ViewBag.AllocatedAnnualLeaves = item.AllocatedAnnualLeaves;
                    ViewBag.AllocatedOtherLeaves = item.AllocatedOtherLeaves;
                    ViewBag.AllocatedLeaveType01 = item.AllocatedLeaveType01;
                    ViewBag.AllocatedLeaveType02 = item.AllocatedLeaveType02;
                    ViewBag.AllocatedLeaveType03 = item.AllocatedLeaveType03;
                    ViewBag.AllocatedLeaveType04 = item.AllocatedLeaveType04;

                    ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                    ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                    ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
                    ViewBag.AvailedOtherLeaves = item.AvailedOtherLeaves;
                    ViewBag.AvailedLeaveType01 = item.AvailedLeaveType01;
                    ViewBag.AvailedLeaveType02 = item.AvailedLeaveType02;
                    ViewBag.AvailedLeaveType03 = item.AvailedLeaveType03;
                    ViewBag.AvailedLeaveType04 = item.AvailedLeaveType04;
                }

                lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpCode(User.Identity.Name, session_year);
            }

            return View(lInfo);
        }

        [HttpPost]
        [ActionName("LeavesCountReport")]
        public ActionResult LeavesCountReport_Post()
        {
            string session_year = DateTime.Now.Year.ToString();
            var data = new List<LeavesCountReportLog>();
            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();

            if (Request.Form["session_year"] != null && Request.Form["session_year"].ToString() != "")
            {
                session_year = Request.Form["session_year"].ToString();
                ViewBag.SelectedSessionYear = session_year;
            }

            if (ViewModel.GlobalVariables.GV_EmployeeId != null && ViewModel.GlobalVariables.GV_EmployeeId != "")
            {
                ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(int.Parse(ViewModel.GlobalVariables.GV_EmployeeId));
                ViewBag.SessionYearsList = yearList;
                //session_year = yearList[0].ToString();

                // get all employee view models
                TimeTune.Attendance.getLeavesCountReportByEmpCode(User.Identity.Name, session_year, out data);
                // int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);

                //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                foreach (LeavesCountReportLog item in data)
                {
                    ViewBag.AllocatedSickLeaves = item.AllocatedSickLeaves;
                    ViewBag.AllocatedCasualLeaves = item.AllocatedCasualLeaves;
                    ViewBag.AllocatedAnnualLeaves = item.AllocatedAnnualLeaves;
                    ViewBag.AllocatedOtherLeaves = item.AllocatedOtherLeaves;
                    ViewBag.AllocatedLeaveType01 = item.AllocatedLeaveType01;
                    ViewBag.AllocatedLeaveType02 = item.AllocatedLeaveType02;
                    ViewBag.AllocatedLeaveType03 = item.AllocatedLeaveType03;
                    ViewBag.AllocatedLeaveType04 = item.AllocatedLeaveType04;

                    ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                    ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                    ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
                    ViewBag.AvailedOtherLeaves = item.AvailedOtherLeaves;
                    ViewBag.AvailedLeaveType01 = item.AvailedLeaveType01;
                    ViewBag.AvailedLeaveType02 = item.AvailedLeaveType02;
                    ViewBag.AvailedLeaveType03 = item.AvailedLeaveType03;
                    ViewBag.AvailedLeaveType04 = item.AvailedLeaveType04;
                }

                lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpCode(User.Identity.Name, int.Parse(session_year));
            }

            return View(lInfo);
        }

        public class LeavesCountReportTable : DTParameters
        {
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public ActionResult LeavesCountReportTableReportDataHandler(LeavesCountReportTable param, string from_date, string to_date)
        {
            int session_year = DateTime.Now.Year;

            try
            {
                var data = new List<LeavesCountReportLog>();

                if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(session_year.ToString() + "-12-31 23:58:00.000"))
                {
                    session_year = session_year + 1;
                }

                // get all employee view models
                TimeTune.Attendance.getLeavesCountReportByEmpCode(User.Identity.Name, session_year.ToString(), out data);
                //TimeTune.Attendance.getLeavesCountReportForLM(param.Search.Value, param.from_date, param.to_date, User.Identity.Name, out data);
                // int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);

                //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<LeavesCountReportLog> result = new DTResult<LeavesCountReportLog>
                {
                    draw = param.Draw,
                    data = data,
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

            //return View("LeavesCountReport");
        }


        #endregion

        #region PayrollReportByMonth

        public class PayrollReportByMonthTable : DTParameters
        {
            public string salary_month_year { get; set; }
        }

        [HttpGet]
        [ActionName("PayrollReportByMonth")]
        public ActionResult PayrollReportByMonth_Get()
        {
            int response = 0;

            try
            {
                response = PayrollResultSet.GetPayrollGenerationStatus();
                if (response == 1)
                {
                    ViewBag.GeneratePayrollStatus = "";
                }
                else
                {
                    ViewBag.GeneratePayrollStatus = "disabled";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        [HttpPost]
        public JsonResult PayrollReportByMonthDataHandler(PayrollReportByMonthTable param)
        {
            try
            {
                var dataPayroll = new List<PayrollReportByMonthLog>();

                // get all employee view models
                int countPayroll = TimeTune.Reports.getPayrollReportByMonthForEmployee(User.Identity.Name, param.salary_month_year, param.Search.Value, param.SortOrder, param.Start, param.Length, out dataPayroll);

                List<ViewModels.PayrollReportByMonthLog> data = PayrollResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dataPayroll);
                int count = PayrollResultSet.Count(param.Search.Value, data);

                DTResult<PayrollReportByMonthLog> result = new DTResult<PayrollReportByMonthLog>
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
        [ActionName("PayrollReportByMonth")]
        public ActionResult PayrollReportByMonth_Post(string param)
        {
            int response = 0;

            try
            {
                response = PayrollResultSet.GeneratePayrollForLastMonth();
                if (response == 1)
                {
                    ViewBag.GeneratePayrollStatus = "disabled";
                }
                else
                {
                    ViewBag.GeneratePayrollStatus = "";
                }
            }
            catch (Exception ex)
            {
                response = -1;
            }

            return View();
        }

        [HttpPost]
        public ActionResult GeneratePayrollPDF()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("PayrollReportByMonth");

            string month_year = Request.Form["month_year"];


            PayrollInfoForPDF pInfo = PayrollResultSet.GeneratePayrollPDFByEmployeeIDANDMonth(employeeID, month_year);

            if (pInfo == null)
                return RedirectToAction("PayrollReportByMonth");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0;
            ViewData["PayrollPDFNoDataFound"] = "";
            found = DownloadPayrollPDF(pInfo);

            if (found == 1)
            {
                ViewData["PayrollPDFNoDataFound"] = "";
                //return null;
            }
            else
            {
                ViewData["PayrollPDFNoDataFound"] = "No Data Found";
            }

            return View("PayrollReportByMonth");
            //return RedirectToAction("MonthlyTimeSheet", "Reports");
        }



        private int DownloadPayrollPDF(PayrollInfoForPDF pInfo)
        {
            int reponse = 0;
            //int gPersonality = 4, gCommunication = 3, gAttendance = 2, gImitative = 5, gOrganization = 1, gSelf = 3;
            //int sProficiency = 2, sProject = 5, sAttention = 3, sClient = 1, sCreativity = 4, sBusiness = 2;

            //string strPosition = "the position text is given below", strRequirement = "", strPrimary = "", strSecondary = "secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there.", strCareer = "this is career path text";

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.NORMAL, Color.BLACK);
                    Font fNormalUnder7 = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.NORMAL | Font.UNDERLINE, Color.BLACK);
                    Font fBold7 = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.BOLD, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont(BaseFont.HELVETICA, 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont(BaseFont.HELVETICA, 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.NORMAL, Color.BLACK);
                    Font fNormalUnder9 = FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.NORMAL | Font.UNDERLINE, Color.BLACK);
                    Font fNormalItalic9 = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, Font.NORMAL | Font.ITALIC, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD, Color.BLACK);

                    Font fNormal14 = FontFactory.GetFont(BaseFont.HELVETICA, 14, Font.NORMAL, Color.BLACK);
                    Font fBold14 = FontFactory.GetFont(BaseFont.HELVETICA, 14, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont(BaseFont.HELVETICA, 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont(BaseFont.HELVETICA, 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + pInfo.employeeCode + "-" + pInfo.month + "-" + pInfo.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(strLogotitle[1], fBold14));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    // ---------- Header Table ---------------------
                    ////string imageURL = Server.MapPath("~/Content/Logos/logo-default.png");
                    //////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    ////Image logo = Image.GetInstance(imageURL);
                    //////logo.Width = 140.0f;
                    //////logo.Alignment = Element.ALIGN_LEFT;
                    //////logo.ScaleToFit(140f, 20f);
                    //////logo.ScaleAbsolute(140f, 20f);
                    //////logo.SpacingBefore = 5f;
                    //////logo.SpacingAfter = 5f;

                    ////PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////tableHeader.WidthPercentage = 100;
                    ////tableHeader.HeaderRows = 0;
                    //////tableHeader.SpacingBefore = 50;
                    ////tableHeader.SpacingAfter = 3;
                    ////tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    ////tableHeader.AddCell(logo);

                    ////PdfPCell cellTitle = new PdfPCell(new Phrase("DUHS - DOW University of Health Sciences", fBold14));
                    ////cellTitle.HorizontalAlignment = 1;
                    ////cellTitle.Border = 0;
                    ////tableHeader.AddCell(cellTitle);

                    ////PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //////cellDateTime.HorizontalAlignment = 2;
                    ////cellDateTime.PaddingTop = 2.0f;
                    ////cellDateTime.Border = 0;
                    ////tableHeader.AddCell(cellDateTime);

                    //////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    ////document.Add(tableHeader);

                    // ---------- Header Table ---------------------
                    ////string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    //////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    ////Image logo = Image.GetInstance(imageURL);
                    //////logo.Width = 140.0f;
                    //////logo.Alignment = Element.ALIGN_LEFT;
                    //////logo.ScaleToFit(140f, 20f);
                    //////logo.ScaleAbsolute(140f, 20f);
                    //////logo.SpacingBefore = 5f;
                    //////logo.SpacingAfter = 5f;

                    ////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////tableHeader.WidthPercentage = 100;
                    ////tableHeader.HeaderRows = 0;
                    //////tableHeader.SpacingBefore = 50;
                    ////tableHeader.SpacingAfter = 3;
                    ////tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    ////tableHeader.AddCell(logo);
                    ////tableHeader.AddCell("");

                    ////PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //////cellDateTime.HorizontalAlignment = 2;
                    ////cellDateTime.Border = 0;
                    ////tableHeader.AddCell(cellDateTime);

                    //////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    ////document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //left cells
                    PdfPCell cellEName = new PdfPCell(new Phrase("Employee Name: " + pInfo.EmployeeName, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + pInfo.EmployeeCode, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellSMonth = new PdfPCell(new Phrase("Salary Month: " + pInfo.SalaryMonthYearText, fBold9));
                    cellSMonth.Border = 0;
                    tableEInfo.AddCell(cellSMonth);

                    PdfPCell cellPayDate = new PdfPCell(new Phrase("Pay Date: " + pInfo.PaymentDatetimeText, fBold9));
                    cellPayDate.Border = 0;
                    tableEInfo.AddCell(cellPayDate);

                    //right cell
                    PdfPCell cellETitle = new PdfPCell(new Phrase("Payroll Slip - " + pInfo.SalaryMonthYearText, fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------- Middle Table ---------------------
                    PdfPTable tableMiddle = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableMiddle.WidthPercentage = 100;
                    tableMiddle.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableMiddle.DefaultCell.Border = Rectangle.NO_BORDER;


                    // ---------- Middle Table - Column 1 ---------------------
                    PdfPTable tableMiddleCol01 = new PdfPTable(1);
                    tableMiddleCol01.WidthPercentage = 100;
                    tableMiddleCol01.HeaderRows = 0;
                    //tableMiddleCol01.SpacingBefore = 50;
                    tableMiddleCol01.SpacingAfter = 3;
                    tableMiddleCol01.DefaultCell.Border = Rectangle.NO_BORDER;


                    ////////////////////////
                    PdfPCell cellC01R01_101 = new PdfPCell(new Phrase("Personal Info", fBold14));
                    cellC01R01_101.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R01_101);

                    PdfPCell cellC01R02_110 = new PdfPCell(new Phrase("Designation: " + ((pInfo.Designation == null || pInfo.Designation == "") ? "-" : pInfo.Designation), fBold9));
                    cellC01R02_110.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_110);

                    PdfPCell cellC01R02_111 = new PdfPCell(new Phrase("Department: " + ((pInfo.Department == null || pInfo.Department == "") ? "-" : pInfo.Department), fBold9));
                    cellC01R02_111.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_111);

                    PdfPCell cellC01R02_112 = new PdfPCell(new Phrase("Joining Date: " + pInfo.JoiningDateText, fBold9));
                    cellC01R02_112.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_112);

                    PdfPCell cellC01R02_120 = new PdfPCell(new Phrase("CNIC #: " + pInfo.PayrollInformation.CNIC, fBold9));
                    cellC01R02_120.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_120);

                    PdfPCell cellC01R03_130 = new PdfPCell(new Phrase("NTN #: " + pInfo.PayrollInformation.NTNNumber, fBold9));
                    cellC01R03_130.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R03_130);

                    PdfPCell cellC01R04_140 = new PdfPCell(new Phrase("EOBI #: " + pInfo.PayrollInformation.EOBINumber, fBold9));
                    cellC01R04_140.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R04_140);

                    PdfPCell cellC01R05_150 = new PdfPCell(new Phrase("SESSI #: " + pInfo.PayrollInformation.SESSINumber, fBold9));
                    cellC01R05_150.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R05_150);

                    /////////////////////////////////////                    
                    PdfPCell cellC01R06_160 = new PdfPCell(new Phrase("\nBasic Pay & Allowances", fBold14));
                    cellC01R06_160.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R06_160);

                    PdfPCell cellC01R07_161 = new PdfPCell(new Phrase("Basic Pay: Rs." + pInfo.PayrollInformation.BasicPay, fNormal9));
                    cellC01R07_161.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_161);

                    PdfPCell cellC01R07_162 = new PdfPCell(new Phrase("Increment: Rs." + pInfo.PayrollInformation.Increment, fNormal9));
                    cellC01R07_162.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_162);

                    PdfPCell cellC01R07_163 = new PdfPCell(new Phrase("Trasport: Rs." + pInfo.PayrollInformation.Transport, fNormal9));
                    cellC01R07_163.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_163);

                    PdfPCell cellC01R07_164 = new PdfPCell(new Phrase("Mobile: Rs." + pInfo.PayrollInformation.Mobile, fNormal9));
                    cellC01R07_164.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_164);

                    PdfPCell cellC01R07_165 = new PdfPCell(new Phrase("Medical: Rs." + pInfo.PayrollInformation.Medical, fNormal9));
                    cellC01R07_165.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_165);

                    PdfPCell cellC01R07_170 = new PdfPCell(new Phrase("Food: Rs." + pInfo.PayrollInformation.Food, fNormal9));
                    cellC01R07_170.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_170);

                    PdfPCell cellC01R08_190 = new PdfPCell(new Phrase("Night: Rs." + pInfo.PayrollInformation.Night, fNormal9));
                    cellC01R08_190.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08_190);

                    PdfPCell cellC01R09_210 = new PdfPCell(new Phrase("Commission: Rs." + pInfo.PayrollInformation.Commission, fNormal9));
                    cellC01R09_210.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09_210);

                    PdfPCell cellC01R10_230 = new PdfPCell(new Phrase("Rent: Rs." + pInfo.PayrollInformation.Rent, fNormal9));
                    cellC01R10_230.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10_230);

                    PdfPCell cellC01R11_250 = new PdfPCell(new Phrase("Cash Allowance: Rs." + pInfo.PayrollInformation.CashAllowance, fNormal9));
                    cellC01R11_250.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_250);

                    PdfPCell cellC01R11_251 = new PdfPCell(new Phrase("Group Allowance: Rs." + +pInfo.PayrollInformation.GroupAllowance, fNormal9));
                    cellC01R11_251.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_251);

                    PdfPCell cellC01R11_261 = new PdfPCell(new Phrase("Annual Bonus: Rs." + pInfo.PayrollInformation.AnnualBonus, fNormal9));
                    cellC01R11_261.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_261);

                    PdfPCell cellC01R11_265 = new PdfPCell(new Phrase("Overtime (Hours): " + +pInfo.PayrollInformation.OvertimeInHours, fNormal9));
                    cellC01R11_265.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_265);

                    PdfPCell cellC01R11_266 = new PdfPCell(new Phrase("Overtime Amount: Rs." + pInfo.PayrollInformation.OvertimeAmount, fNormal9));
                    cellC01R11_266.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_266);

                    PdfPCell cellC01R11_267 = new PdfPCell(new Phrase("Leaves Count: " + +pInfo.PayrollInformation.LeavesCount, fNormal9));
                    cellC01R11_267.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_267);

                    PdfPCell cellC01R11_268 = new PdfPCell(new Phrase("Leaves Encash: Rs." + +pInfo.PayrollInformation.LeavesEncash, fNormal9));
                    cellC01R11_268.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_268);


                    ////////////////////////
                    PdfPCell cellC01R13_290 = new PdfPCell(new Phrase("\nLeaves Detail", fBold14));
                    cellC01R13_290.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R13_290);

                    PdfPCell cellC01R14_300 = new PdfPCell(new Phrase("Leaves Session:", fBold9));
                    cellC01R14_300.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14_300);

                    PdfPCell cellC01R14_310 = new PdfPCell(new Phrase(pInfo.LeavesSessionText + "\n\n", fNormal9));
                    cellC01R14_310.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14_310);

                    PdfPCell cellC01R15_320 = new PdfPCell(new Phrase("Allocated Leaves:", fBold9));
                    cellC01R15_320.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15_320);

                    PdfPCell cellC01R15_330 = new PdfPCell(new Phrase(pInfo.AllocatedLeaves + "\n\n", fNormal9));
                    cellC01R15_330.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15_330);

                    PdfPCell cellC01R16_340 = new PdfPCell(new Phrase("Availed Leaves:", fBold9));
                    cellC01R16_340.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_340);

                    PdfPCell cellC01R16_350 = new PdfPCell(new Phrase(pInfo.AvailedLeaves + "\n\n", fNormal9));
                    cellC01R16_350.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_350);

                    PdfPCell cellC01R16_351 = new PdfPCell(new Phrase("Leaves Availed This Month:", fBold9));
                    cellC01R16_351.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_351);

                    PdfPCell cellC01R16_352 = new PdfPCell(new Phrase(pInfo.AvailedLeavesLastMonth + "\n\n", fNormal9));
                    cellC01R16_352.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_352);

                    PdfPCell cellC01R16_353 = new PdfPCell(new Phrase("Leaves Remaining:", fBold9));
                    cellC01R16_353.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_353);

                    PdfPCell cellC01R16_354 = new PdfPCell(new Phrase(pInfo.RemainingLeaves + "\n\n", fNormal9));
                    cellC01R16_354.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_354);


                    // --------------------------------------------- Middle Table - Column 2 ---------------------------------------------------------
                    PdfPTable tableMiddleCol02 = new PdfPTable(1);
                    tableMiddleCol02.WidthPercentage = 100;
                    tableMiddleCol02.HeaderRows = 0;
                    //tableMiddleCol02.SpacingBefore = 50;
                    tableMiddleCol02.SpacingAfter = 3;
                    tableMiddleCol02.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellC02R01_501 = new PdfPCell(new Phrase("Bank & Account Info", fBold14));
                    cellC02R01_501.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R01_501);

                    ////////////////////////
                    PdfPCell cellC02R02_510 = new PdfPCell(new Phrase("\nBank Name: " + pInfo.PayrollInformation.BankNameText, fBold9));
                    cellC02R02_510.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R02_510);

                    ////////////////////////
                    PdfPCell cellC02R03_530 = new PdfPCell(new Phrase("\nAccount Title: " + pInfo.PayrollInformation.BankAccTitle, fBold9));
                    cellC02R03_530.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R03_530);

                    ////////////////////////
                    PdfPCell cellC02R04_540 = new PdfPCell(new Phrase("\nAccount Number: " + pInfo.PayrollInformation.BankAccNo, fBold9));
                    cellC02R04_540.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R04_540);

                    ////////////////////////
                    PdfPCell cellC02R05_550 = new PdfPCell(new Phrase("\nPay Mode: " + pInfo.PayrollInformation.PaymentModeText, fBold9));
                    cellC02R05_550.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R05_550);

                    ////////////////////////
                    PdfPCell cellC02R06_560 = new PdfPCell(new Phrase("\nDeduction/Contribution", fBold14));
                    cellC02R06_560.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R06_560);

                    PdfPCell cellC02R07_570 = new PdfPCell(new Phrase("Income Tax: Rs." + pInfo.PayrollInformation.IncomeTax, fNormal9));
                    cellC02R07_570.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07_570);

                    PdfPCell cellC02R08_590 = new PdfPCell(new Phrase("Fine/Extra Amount: Rs." + pInfo.PayrollInformation.FineExtraAmount, fNormal9));
                    cellC02R08_590.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R08_590);

                    PdfPCell cellC02R09_610 = new PdfPCell(new Phrase("Mobile Deduction: Rs." + pInfo.PayrollInformation.MobileDeduction, fNormal9));
                    cellC02R09_610.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09_610);

                    PdfPCell cellC02R10_630 = new PdfPCell(new Phrase("Absent Count: " + pInfo.PayrollInformation.AbsentsCount, fNormal9));
                    cellC02R10_630.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10_630);

                    PdfPCell cellC02R11_650 = new PdfPCell(new Phrase("Absent Amount: Rs." + pInfo.PayrollInformation.AbsentsAmount, fNormal9));
                    cellC02R11_650.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11_650);

                    PdfPCell cellC02R12_651 = new PdfPCell(new Phrase("EOBI Emp. Contr.: Rs." + pInfo.PayrollInformation.EOBIEmployee, fNormal9));
                    cellC02R12_651.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_651);

                    PdfPCell cellC02R12_652 = new PdfPCell(new Phrase("EOBI Co. Contr.: Rs." + pInfo.PayrollInformation.EOBIEmployer, fNormal9));
                    cellC02R12_652.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_652);

                    PdfPCell cellC02R12_653 = new PdfPCell(new Phrase("SESSI Emp. Contr.: Rs." + pInfo.PayrollInformation.SESSIEmployee, fNormal9));
                    cellC02R12_653.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_653);

                    PdfPCell cellC02R12_654 = new PdfPCell(new Phrase("SESSI Co. Contr.: Rs." + pInfo.PayrollInformation.SESSIEmployer, fNormal9));
                    cellC02R12_654.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_654);

                    PdfPCell cellC02R12_670 = new PdfPCell(new Phrase("Loan Installment: Rs." + pInfo.PayrollInformation.LoanInstallment, fNormal9));
                    cellC02R12_670.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_670);

                    PdfPCell cellC02R12_671 = new PdfPCell(new Phrase("Other Deduction: Rs." + pInfo.PayrollInformation.OtherDeduction, fNormal9));
                    cellC02R12_671.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_671);

                    ////////////////////////
                    PdfPCell cellC02R13 = new PdfPCell(new Phrase("\nNET Calculation", fBold14));
                    cellC02R13.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R13);

                    ////////////////////////
                    PdfPCell cellC02R14_690 = new PdfPCell(new Phrase("Gross Salary:", fBold10));
                    cellC02R14_690.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_690);

                    PdfPCell cellC02R14_700 = new PdfPCell(new Phrase("Rs." + pInfo.PayrollInformation.GrossSalary.ToString("C").Replace("$", "") + "\n\n\n", fNormal10));
                    cellC02R14_700.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_700);

                    ////////////////////////
                    PdfPCell cellC02R14_701 = new PdfPCell(new Phrase("Total Deductions:", fBold10));
                    cellC02R14_701.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_701);

                    PdfPCell cellC02R14_702 = new PdfPCell(new Phrase("Rs." + pInfo.PayrollInformation.TotalDeduction.ToString("C").Replace("$", "") + "\n\n\n", fNormal10));
                    cellC02R14_702.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_702);

                    ////////////////////////
                    PdfPCell cellC02R14_703 = new PdfPCell(new Phrase("Net Salary:", fBold10));
                    cellC02R14_703.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_703);

                    PdfPCell cellC02R14_704 = new PdfPCell(new Phrase("Rs." + pInfo.PayrollInformation.NetSalary.ToString("C").Replace("$", "") + "\n\n\n", fNormal10));
                    cellC02R14_704.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_704);


                    PdfPCell cellC02R15_710 = new PdfPCell(new Phrase("\n", fBold9));
                    cellC02R15_710.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R15_710);

                    PdfPCell cellC02R15_720 = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(Employee's Signature / date)", fNormalItalic9));
                    cellC02R15_720.Border = 0;
                    cellC02R15_720.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R15_720);

                    ////////////////////////
                    PdfPCell cellC02R16_730 = new PdfPCell(new Phrase("\n\n\n\n\n", fBold9));
                    cellC02R16_730.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R16_730);

                    PdfPCell cellC02R16_740 = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(HR's Signature / date)", fNormalItalic9));
                    cellC02R16_740.Border = 0;
                    cellC02R16_740.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R16_740);

                    ////////////////////////////////////////////////////////////////////////////////////////

                    //add 2 tables to MAIN table
                    tableMiddle.AddCell(tableMiddleCol01);
                    tableMiddle.AddCell(tableMiddleCol02);

                    document.Add(tableMiddle);

                    ///////////////////////////////////////////////////////////////////////////////////////

                    // ---------- End Table ---------------------
                    Paragraph p_nsig = new Paragraph("This is a system generated payroll-slip and does not require any signature.", fNormal7);
                    p_nsig.SpacingBefore = 1;
                    p_nsig.SpacingAfter = 3;
                    document.Add(p_nsig);

                    // ------------- close PDF Document and download it automatically



                    document.Close();
                    writer.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=" + BuildPdfFileName("Report"));
                    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    Response.Flush();
                    Response.End();

                    reponse = 1;

                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        [HttpPost]
        public ActionResult UpdatePayrollAckStatus(ViewModels.PayrollStatus toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            PayrollResultSet.updatePayrollAckStatus(toUpdate);
            AuditTrail.update(json, "PayrollAckStatusByEmp", User.Identity.Name);
            return Json(new { status = "success" });
        }


        #endregion

        #region LoanReportByMonth

        public class LoanReportByMonthTable : DTParameters
        {
            public string month_year { get; set; }
        }

        [HttpGet]
        [ActionName("LoanReportByMonth")]
        public ActionResult LoanReportByMonth_Get()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoanReportByMonthDataHandler(LoanReportByMonthTable param)
        {
            try
            {
                var dataLoan = new List<LoanReportByMonthLog>();

                // get all employee view models
                int countLoan = TimeTune.Reports.getLoanReportByMonthForEmployee(User.Identity.Name, param.month_year, param.Search.Value, param.SortOrder, param.Start, param.Length, out dataLoan);

                List<ViewModels.LoanReportByMonthLog> data = LoanResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dataLoan);
                int count = LoanResultSet.Count(param.Search.Value, data);

                DTResult<LoanReportByMonthLog> result = new DTResult<LoanReportByMonthLog>
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
        [ActionName("LoanReportByMonth")]
        public ActionResult LoanReportByMonth_Post(string param)
        {
            int response = 0;

            try
            {
                response = PayrollResultSet.GeneratePayrollForLastMonth();
                if (response == 1)
                {
                    ViewBag.GeneratePayrollStatus = "disabled";
                }
                else
                {
                    ViewBag.GeneratePayrollStatus = "";
                }
            }
            catch (Exception ex)
            {
                response = -1;
            }

            return View();
        }

        [HttpPost]
        public ActionResult GenerateLoanPDF()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("PayrollReportByMonth");

            string month_year = Request.Form["month_year"];


            PayrollInfoForPDF pInfo = PayrollResultSet.GeneratePayrollPDFByEmployeeIDANDMonth(employeeID, month_year);

            if (pInfo == null)
                return RedirectToAction("PayrollReportByMonth");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0;
            ViewData["PayrollPDFNoDataFound"] = "";
            found = DownloadPayrollPDF(pInfo);

            if (found == 1)
            {
                ViewData["PayrollPDFNoDataFound"] = "";
                //return null;
            }
            else
            {
                ViewData["PayrollPDFNoDataFound"] = "No Data Found";
            }

            return View("PayrollReportByMonth");
            //return RedirectToAction("MonthlyTimeSheet", "Reports");
        }



        private int DownloadLoanPDF(PayrollInfoForPDF pInfo)
        {
            int reponse = 0;
            //int gPersonality = 4, gCommunication = 3, gAttendance = 2, gImitative = 5, gOrganization = 1, gSelf = 3;
            //int sProficiency = 2, sProject = 5, sAttention = 3, sClient = 1, sCreativity = 4, sBusiness = 2;

            //string strPosition = "the position text is given below", strRequirement = "", strPrimary = "", strSecondary = "secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there.", strCareer = "this is career path text";

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.NORMAL, Color.BLACK);
                    Font fNormalUnder7 = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.NORMAL | Font.UNDERLINE, Color.BLACK);
                    Font fBold7 = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.BOLD, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont(BaseFont.HELVETICA, 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont(BaseFont.HELVETICA, 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.NORMAL, Color.BLACK);
                    Font fNormalUnder9 = FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.NORMAL | Font.UNDERLINE, Color.BLACK);
                    Font fNormalItalic9 = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 9, Font.NORMAL | Font.ITALIC, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont(BaseFont.HELVETICA, 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont(BaseFont.HELVETICA, 10, Font.BOLD, Color.BLACK);

                    Font fNormal14 = FontFactory.GetFont(BaseFont.HELVETICA, 14, Font.NORMAL, Color.BLACK);
                    Font fBold14 = FontFactory.GetFont(BaseFont.HELVETICA, 14, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont(BaseFont.HELVETICA, 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont(BaseFont.HELVETICA, 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + pInfo.employeeCode + "-" + pInfo.month + "-" + pInfo.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(strLogotitle[1], fBold14));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    // ---------- Header Table ---------------------
                    ////string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    //////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    ////Image logo = Image.GetInstance(imageURL);
                    //////logo.Width = 140.0f;
                    //////logo.Alignment = Element.ALIGN_LEFT;
                    //////logo.ScaleToFit(140f, 20f);
                    //////logo.ScaleAbsolute(140f, 20f);
                    //////logo.SpacingBefore = 5f;
                    //////logo.SpacingAfter = 5f;

                    ////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////tableHeader.WidthPercentage = 100;
                    ////tableHeader.HeaderRows = 0;
                    //////tableHeader.SpacingBefore = 50;
                    ////tableHeader.SpacingAfter = 3;
                    ////tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    ////tableHeader.AddCell(logo);
                    ////tableHeader.AddCell("");

                    ////PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //////cellDateTime.HorizontalAlignment = 2;
                    ////cellDateTime.Border = 0;
                    ////tableHeader.AddCell(cellDateTime);

                    //////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    ////document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //left cells
                    PdfPCell cellEName = new PdfPCell(new Phrase("Employee Name: " + pInfo.EmployeeName, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + pInfo.EmployeeCode, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellSMonth = new PdfPCell(new Phrase("Salary Month: " + pInfo.SalaryMonthYearText, fBold9));
                    cellSMonth.Border = 0;
                    tableEInfo.AddCell(cellSMonth);

                    PdfPCell cellPayDate = new PdfPCell(new Phrase("Pay Date: " + pInfo.PaymentDatetimeText, fBold9));
                    cellPayDate.Border = 0;
                    tableEInfo.AddCell(cellPayDate);

                    //right cell
                    PdfPCell cellETitle = new PdfPCell(new Phrase("Payroll Slip - " + pInfo.SalaryMonthYearText, fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------- Middle Table ---------------------
                    PdfPTable tableMiddle = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableMiddle.WidthPercentage = 100;
                    tableMiddle.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableMiddle.DefaultCell.Border = Rectangle.NO_BORDER;


                    // ---------- Middle Table - Column 1 ---------------------
                    PdfPTable tableMiddleCol01 = new PdfPTable(1);
                    tableMiddleCol01.WidthPercentage = 100;
                    tableMiddleCol01.HeaderRows = 0;
                    //tableMiddleCol01.SpacingBefore = 50;
                    tableMiddleCol01.SpacingAfter = 3;
                    tableMiddleCol01.DefaultCell.Border = Rectangle.NO_BORDER;


                    ////////////////////////
                    PdfPCell cellC01R01_101 = new PdfPCell(new Phrase("Personal Info", fBold14));
                    cellC01R01_101.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R01_101);

                    PdfPCell cellC01R02_110 = new PdfPCell(new Phrase("Designation: " + ((pInfo.Designation == null || pInfo.Designation == "") ? "-" : pInfo.Designation), fBold9));
                    cellC01R02_110.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_110);

                    PdfPCell cellC01R02_111 = new PdfPCell(new Phrase("Department: " + ((pInfo.Department == null || pInfo.Department == "") ? "-" : pInfo.Department), fBold9));
                    cellC01R02_111.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_111);

                    PdfPCell cellC01R02_112 = new PdfPCell(new Phrase("Joining Date: " + pInfo.JoiningDateText, fBold9));
                    cellC01R02_112.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_112);

                    PdfPCell cellC01R02_120 = new PdfPCell(new Phrase("CNIC #: " + pInfo.PayrollInformation.CNIC, fBold9));
                    cellC01R02_120.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_120);

                    PdfPCell cellC01R03_130 = new PdfPCell(new Phrase("NTN #: " + pInfo.PayrollInformation.NTNNumber, fBold9));
                    cellC01R03_130.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R03_130);

                    PdfPCell cellC01R04_140 = new PdfPCell(new Phrase("EOBI #: " + pInfo.PayrollInformation.EOBINumber, fBold9));
                    cellC01R04_140.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R04_140);

                    PdfPCell cellC01R05_150 = new PdfPCell(new Phrase("SESSI #: " + pInfo.PayrollInformation.SESSINumber, fBold9));
                    cellC01R05_150.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R05_150);

                    /////////////////////////////////////                    
                    PdfPCell cellC01R06_160 = new PdfPCell(new Phrase("\nBasic Pay & Allowances", fBold14));
                    cellC01R06_160.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R06_160);

                    PdfPCell cellC01R07_161 = new PdfPCell(new Phrase("Basic Pay: Rs." + pInfo.PayrollInformation.BasicPay, fNormal9));
                    cellC01R07_161.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_161);

                    PdfPCell cellC01R07_162 = new PdfPCell(new Phrase("Increment: Rs." + pInfo.PayrollInformation.Increment, fNormal9));
                    cellC01R07_162.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_162);

                    PdfPCell cellC01R07_163 = new PdfPCell(new Phrase("Trasport: Rs." + pInfo.PayrollInformation.Transport, fNormal9));
                    cellC01R07_163.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_163);

                    PdfPCell cellC01R07_164 = new PdfPCell(new Phrase("Mobile: Rs." + pInfo.PayrollInformation.Mobile, fNormal9));
                    cellC01R07_164.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_164);

                    PdfPCell cellC01R07_165 = new PdfPCell(new Phrase("Medical: Rs." + pInfo.PayrollInformation.Medical, fNormal9));
                    cellC01R07_165.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_165);

                    PdfPCell cellC01R07_170 = new PdfPCell(new Phrase("Food: Rs." + pInfo.PayrollInformation.Food, fNormal9));
                    cellC01R07_170.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_170);

                    PdfPCell cellC01R08_190 = new PdfPCell(new Phrase("Night: Rs." + pInfo.PayrollInformation.Night, fNormal9));
                    cellC01R08_190.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08_190);

                    PdfPCell cellC01R09_210 = new PdfPCell(new Phrase("Commission: Rs." + pInfo.PayrollInformation.Commission, fNormal9));
                    cellC01R09_210.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09_210);

                    PdfPCell cellC01R10_230 = new PdfPCell(new Phrase("Rent: Rs." + pInfo.PayrollInformation.Rent, fNormal9));
                    cellC01R10_230.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10_230);

                    PdfPCell cellC01R11_250 = new PdfPCell(new Phrase("Cash Allowance: Rs." + pInfo.PayrollInformation.CashAllowance, fNormal9));
                    cellC01R11_250.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_250);

                    PdfPCell cellC01R11_251 = new PdfPCell(new Phrase("Group Allowance: Rs." + +pInfo.PayrollInformation.GroupAllowance, fNormal9));
                    cellC01R11_251.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_251);

                    PdfPCell cellC01R11_261 = new PdfPCell(new Phrase("Annual Bonus: Rs." + pInfo.PayrollInformation.AnnualBonus, fNormal9));
                    cellC01R11_261.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_261);

                    PdfPCell cellC01R11_265 = new PdfPCell(new Phrase("Overtime (Hours): " + +pInfo.PayrollInformation.OvertimeInHours, fNormal9));
                    cellC01R11_265.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_265);

                    PdfPCell cellC01R11_266 = new PdfPCell(new Phrase("Overtime Amount: Rs." + pInfo.PayrollInformation.OvertimeAmount, fNormal9));
                    cellC01R11_266.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_266);

                    PdfPCell cellC01R11_267 = new PdfPCell(new Phrase("Leaves Count: " + +pInfo.PayrollInformation.LeavesCount, fNormal9));
                    cellC01R11_267.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_267);

                    PdfPCell cellC01R11_268 = new PdfPCell(new Phrase("Leaves Encash: Rs." + +pInfo.PayrollInformation.LeavesEncash, fNormal9));
                    cellC01R11_268.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_268);


                    ////////////////////////
                    PdfPCell cellC01R13_290 = new PdfPCell(new Phrase("\nLeaves Detail", fBold14));
                    cellC01R13_290.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R13_290);

                    PdfPCell cellC01R14_300 = new PdfPCell(new Phrase("Leaves Session:", fBold9));
                    cellC01R14_300.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14_300);

                    PdfPCell cellC01R14_310 = new PdfPCell(new Phrase(pInfo.LeavesSessionText + "\n\n", fNormal9));
                    cellC01R14_310.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14_310);

                    PdfPCell cellC01R15_320 = new PdfPCell(new Phrase("Allocated Leaves:", fBold9));
                    cellC01R15_320.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15_320);

                    PdfPCell cellC01R15_330 = new PdfPCell(new Phrase(pInfo.AllocatedLeaves + "\n\n", fNormal9));
                    cellC01R15_330.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15_330);

                    PdfPCell cellC01R16_340 = new PdfPCell(new Phrase("Availed Leaves:", fBold9));
                    cellC01R16_340.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_340);

                    PdfPCell cellC01R16_350 = new PdfPCell(new Phrase(pInfo.AvailedLeaves + "\n\n", fNormal9));
                    cellC01R16_350.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_350);

                    PdfPCell cellC01R16_351 = new PdfPCell(new Phrase("Leaves Availed This Month:", fBold9));
                    cellC01R16_351.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_351);

                    PdfPCell cellC01R16_352 = new PdfPCell(new Phrase(pInfo.AvailedLeavesLastMonth + "\n\n", fNormal9));
                    cellC01R16_352.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_352);

                    PdfPCell cellC01R16_353 = new PdfPCell(new Phrase("Leaves Remaining:", fBold9));
                    cellC01R16_353.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_353);

                    PdfPCell cellC01R16_354 = new PdfPCell(new Phrase(pInfo.RemainingLeaves + "\n\n", fNormal9));
                    cellC01R16_354.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_354);


                    // --------------------------------------------- Middle Table - Column 2 ---------------------------------------------------------
                    PdfPTable tableMiddleCol02 = new PdfPTable(1);
                    tableMiddleCol02.WidthPercentage = 100;
                    tableMiddleCol02.HeaderRows = 0;
                    //tableMiddleCol02.SpacingBefore = 50;
                    tableMiddleCol02.SpacingAfter = 3;
                    tableMiddleCol02.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellC02R01_501 = new PdfPCell(new Phrase("Bank & Account Info", fBold14));
                    cellC02R01_501.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R01_501);

                    ////////////////////////
                    PdfPCell cellC02R02_510 = new PdfPCell(new Phrase("\nBank Name: " + pInfo.PayrollInformation.BankNameText, fBold9));
                    cellC02R02_510.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R02_510);

                    ////////////////////////
                    PdfPCell cellC02R03_530 = new PdfPCell(new Phrase("\nAccount Title: " + pInfo.PayrollInformation.BankAccTitle, fBold9));
                    cellC02R03_530.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R03_530);

                    ////////////////////////
                    PdfPCell cellC02R04_540 = new PdfPCell(new Phrase("\nAccount Number: " + pInfo.PayrollInformation.BankAccNo, fBold9));
                    cellC02R04_540.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R04_540);

                    ////////////////////////
                    PdfPCell cellC02R05_550 = new PdfPCell(new Phrase("\nPay Mode: " + pInfo.PayrollInformation.PaymentModeText, fBold9));
                    cellC02R05_550.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R05_550);

                    ////////////////////////
                    PdfPCell cellC02R06_560 = new PdfPCell(new Phrase("\nDeduction/Contribution", fBold14));
                    cellC02R06_560.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R06_560);

                    PdfPCell cellC02R07_570 = new PdfPCell(new Phrase("Income Tax: Rs." + pInfo.PayrollInformation.IncomeTax, fNormal9));
                    cellC02R07_570.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07_570);

                    PdfPCell cellC02R08_590 = new PdfPCell(new Phrase("Fine/Extra Amount: Rs." + pInfo.PayrollInformation.FineExtraAmount, fNormal9));
                    cellC02R08_590.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R08_590);

                    PdfPCell cellC02R09_610 = new PdfPCell(new Phrase("Mobile Deduction: Rs." + pInfo.PayrollInformation.MobileDeduction, fNormal9));
                    cellC02R09_610.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09_610);

                    PdfPCell cellC02R10_630 = new PdfPCell(new Phrase("Absent Count: " + pInfo.PayrollInformation.AbsentsCount, fNormal9));
                    cellC02R10_630.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10_630);

                    PdfPCell cellC02R11_650 = new PdfPCell(new Phrase("Absent Amount: Rs." + pInfo.PayrollInformation.AbsentsAmount, fNormal9));
                    cellC02R11_650.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11_650);

                    PdfPCell cellC02R12_651 = new PdfPCell(new Phrase("EOBI Emp. Contr.: Rs." + pInfo.PayrollInformation.EOBIEmployee, fNormal9));
                    cellC02R12_651.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_651);

                    PdfPCell cellC02R12_652 = new PdfPCell(new Phrase("EOBI Co. Contr.: Rs." + pInfo.PayrollInformation.EOBIEmployer, fNormal9));
                    cellC02R12_652.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_652);

                    PdfPCell cellC02R12_653 = new PdfPCell(new Phrase("SESSI Emp. Contr.: Rs." + pInfo.PayrollInformation.SESSIEmployee, fNormal9));
                    cellC02R12_653.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_653);

                    PdfPCell cellC02R12_654 = new PdfPCell(new Phrase("SESSI Co. Contr.: Rs." + pInfo.PayrollInformation.SESSIEmployer, fNormal9));
                    cellC02R12_654.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_654);

                    PdfPCell cellC02R12_670 = new PdfPCell(new Phrase("Loan Installment: Rs." + pInfo.PayrollInformation.LoanInstallment, fNormal9));
                    cellC02R12_670.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_670);

                    PdfPCell cellC02R12_671 = new PdfPCell(new Phrase("Other Deduction: Rs." + pInfo.PayrollInformation.OtherDeduction, fNormal9));
                    cellC02R12_671.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_671);

                    ////////////////////////
                    PdfPCell cellC02R13 = new PdfPCell(new Phrase("\nNET Calculation", fBold14));
                    cellC02R13.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R13);

                    ////////////////////////
                    PdfPCell cellC02R14_690 = new PdfPCell(new Phrase("Gross Salary:", fBold10));
                    cellC02R14_690.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_690);

                    PdfPCell cellC02R14_700 = new PdfPCell(new Phrase("Rs." + pInfo.PayrollInformation.GrossSalary.ToString("C").Replace("$", "") + "\n\n\n", fNormal10));
                    cellC02R14_700.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_700);

                    ////////////////////////
                    PdfPCell cellC02R14_701 = new PdfPCell(new Phrase("Total Deductions:", fBold10));
                    cellC02R14_701.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_701);

                    PdfPCell cellC02R14_702 = new PdfPCell(new Phrase("Rs." + pInfo.PayrollInformation.TotalDeduction.ToString("C").Replace("$", "") + "\n\n\n", fNormal10));
                    cellC02R14_702.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_702);

                    ////////////////////////
                    PdfPCell cellC02R14_703 = new PdfPCell(new Phrase("Net Salary:", fBold10));
                    cellC02R14_703.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_703);

                    PdfPCell cellC02R14_704 = new PdfPCell(new Phrase("Rs." + pInfo.PayrollInformation.NetSalary.ToString("C").Replace("$", "") + "\n\n\n", fNormal10));
                    cellC02R14_704.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_704);


                    PdfPCell cellC02R15_710 = new PdfPCell(new Phrase("\n", fBold9));
                    cellC02R15_710.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R15_710);

                    PdfPCell cellC02R15_720 = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(Employee's Signature / date)", fNormalItalic9));
                    cellC02R15_720.Border = 0;
                    cellC02R15_720.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R15_720);

                    ////////////////////////
                    PdfPCell cellC02R16_730 = new PdfPCell(new Phrase("\n\n\n\n\n", fBold9));
                    cellC02R16_730.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R16_730);

                    PdfPCell cellC02R16_740 = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(HR's Signature / date)", fNormalItalic9));
                    cellC02R16_740.Border = 0;
                    cellC02R16_740.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R16_740);

                    ////////////////////////////////////////////////////////////////////////////////////////

                    //add 2 tables to MAIN table
                    tableMiddle.AddCell(tableMiddleCol01);
                    tableMiddle.AddCell(tableMiddleCol02);

                    document.Add(tableMiddle);

                    ///////////////////////////////////////////////////////////////////////////////////////

                    // ---------- End Table ---------------------
                    Paragraph p_nsig = new Paragraph("This is a system generated payroll-slip and does not require any signature.", fNormal7);
                    p_nsig.SpacingBefore = 1;
                    p_nsig.SpacingAfter = 3;
                    document.Add(p_nsig);

                    // ------------- close PDF Document and download it automatically



                    document.Close();
                    writer.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=" + BuildPdfFileName("Report"));
                    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    Response.Flush();
                    Response.End();

                    reponse = 1;

                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        [HttpPost]
        public ActionResult UpdateLoanAckStatus(ViewModels.PayrollStatus toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            PayrollResultSet.updatePayrollAckStatus(toUpdate);
            AuditTrail.update(json, "PayrollAckStatusByEmp", User.Identity.Name);
            return Json(new { status = "success" });
        }


        #endregion

        #region DirectDownloadExcelXLSXVersion

        [HttpPost]
        public ActionResult ConsolidatedReportExcelDownload(ConsolidatedReportTable param)
        {
            var ddata = new List<ConsolidatedAttendanceExport>();
            string handle = Guid.NewGuid().ToString();
            int employeeID = 0;

            try
            {
                //get EmployeeID By EmployeeCode
                //string emp_id = TimeTune.Reports.getEmployeeIDByEmployeeCode(param.employee_id);
                employeeID = TimeTune.EmpReports.getEmployeeID(User.Identity.Name.ToString());

                int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(employeeID.ToString(), param.from_date, param.to_date, out ddata);

                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<ConsolidatedAttendanceExport>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("ConsolidatedAttendanceLog");
                    var totalCols = products.Columns.Count;
                    var totalRows = products.Rows.Count;

                    for (var col = 1; col <= totalCols; col++)
                    {
                        workSheet.Cells[1, col].Value = products.Columns[col - 1].ColumnName.Replace("_", " ");
                    }

                    for (var row = 1; row <= totalRows; row++)
                    {
                        for (var col = 0; col < totalCols; col++)
                        {
                            workSheet.Cells[row + 1, col + 1].Value = products.Rows[row - 1][col];
                        }
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);

                        memoryStream.Position = 0;
                        TempData[handle] = memoryStream.ToArray();
                    }
                }
                else
                {
                    return new JsonResult()
                    {
                        Data = new { FileGuid = "", FileName = "" }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = "Consolidated-Attendance-Log.xlsx" }
            };
        }

        [HttpPost]
        public ActionResult DepartmentWiseReportExcelDownload(ConsolidatedDepartmentReportTable param)
        {
            var ddata = new List<ViewModels.ConsolidatedAttendanceDepartmentWiseExcelDownload>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int fun_id, reg_id, depart_id, des_id, loc_id;

                if (!int.TryParse(param.function_id, out fun_id))
                    fun_id = -1;
                if (!int.TryParse(param.region_id, out reg_id))
                    reg_id = -1;
                if (!int.TryParse(param.department_id, out depart_id))
                    depart_id = -1;
                if (!int.TryParse(param.designation_id, out des_id))
                    des_id = -1;
                if (!int.TryParse(param.location_id, out loc_id))
                    loc_id = -1;

                ddata = TimeTune.Reports.getAttendanceDepartmentWiseExcelDownload(depart_id, des_id, loc_id, fun_id, reg_id, param.from_date, param.to_date);

                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<ConsolidatedAttendanceDepartmentWiseExcelDownload>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("DepartmentWiseAttendanceReport");
                    var totalCols = products.Columns.Count;
                    var totalRows = products.Rows.Count;

                    for (var col = 1; col <= totalCols; col++)
                    {
                        workSheet.Cells[1, col].Value = products.Columns[col - 1].ColumnName.Replace("_", " ");
                    }

                    for (var row = 1; row <= totalRows; row++)
                    {
                        for (var col = 0; col < totalCols; col++)
                        {
                            workSheet.Cells[row + 1, col + 1].Value = products.Rows[row - 1][col];
                        }
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);

                        memoryStream.Position = 0;
                        TempData[handle] = memoryStream.ToArray();
                    }
                }
                else
                {
                    return new JsonResult()
                    {
                        Data = new { FileGuid = "", FileName = "" }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = "DepartmentWise-Attendance-Report.xlsx" }
            };
        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public class ConsolidatedDepartmentReportTable : DTParameters
        {
            public string department_id { get; set; }
            public string designation_id { get; set; }
            public string function_id { get; set; }
            public string region_id { get; set; }
            public string location_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        #endregion

        #region MonthlyTimeSheetReport

        [HttpGet]
        [ActionName("MonthlyTimeSheetOvertime")]
        public ActionResult MonthlyTimeSheetOvertime_Get()
        {
            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            ViewData["PDFNoDataFound"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("MonthlyTimeSheetOvertime")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheetOvertime_Post()
        {

            BLL.PdfReports.MonthlyTimeSheet p = new BLL.PdfReports.MonthlyTimeSheet();
            int employeeID = p.GetEmpId(User.Identity.Name);
            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);


            if (toRender == null)
                return RedirectToAction("MonthlyTimeSheetOvertime");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateMonthlyTimeOvertimeSheetPDF(toRender);

            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            return View();


            //return RedirectToAction("MonthlyTimeSheet", "Reports");

        }

        [HttpGet]
        [ActionName("MonthlyTimeSheet")]
        public ActionResult MonthlyTimeSheet_Get()
        {
            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            ViewData["PDFNoDataFound"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("MonthlyTimeSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheet_Post()
        {
            int employeeID = TimeTune.EmpReports.getEmployeeID(User.Identity.Name.ToString());

            string fromDate = Request.Form["fromDate"];
            string toDate = Request.Form["toDate"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            //BLL.PdfReports.CustomRangeTimeSheetData toRender = reportMaker.getReportAlt(employeeID, fromDate, toDate);
            BLL.PdfReports.CustomRangeTimeSheetData toRender = reportMaker.getWorkingHourReportAlt(employeeID, fromDate, toDate);

            if (toRender == null)
                return RedirectToAction("MonthlyTimeSheet");

            ActionResult pdfResult = GenerateWorkHoursTimeSheetPdfMonthlyStyle(toRender);
            if (pdfResult != null)
                return pdfResult;

            ViewData["PDFNoDataFound"] = "No Data Found";
            return View("MonthlyTimeSheet");
        }

        [HttpGet]
        [ActionName("MonthlyTimeSheetnews")]
        public ActionResult MonthlyTimeSheetnews_Get()
        {
            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            ViewData["PDFNoDataFound"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("MonthlyTimeSheetnews")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheetnews_Post()
        {
            int employeeID = TimeTune.EmpReports.getEmployeeID(User.Identity.Name.ToString());

            string month = Request.Form["month"];


            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();
            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getWorkingHourReportNew(employeeID, month);


            if (toRender == null)
                return RedirectToAction("MonthlyTimeSheetnews");

            ActionResult pdfResult = GenerateWorkHoursTimeSheetPdfMonthlyStyle(toRender);
            if (pdfResult != null)
                return pdfResult;

            ViewData["PDFNoDataFound"] = "No Data Found";
            return View("MonthlyTimeSheetnews");
        }

        private ActionResult GenerateWorkHoursTimeSheetPdfMonthlyStyle(BLL.PdfReports.CustomRangeTimeSheetData sdata)
        {
            if (sdata == null || sdata.logs == null || sdata.logs.Length == 0)
            {
                return null;
            }

            string lang = GetCurrentLang();
            bool isArabic = string.Equals(lang, "Ar", StringComparison.OrdinalIgnoreCase);
            int runDirection = GetPdfRunDirection();

            using (MemoryStream ms = new MemoryStream())
            {
                Font fNormal7 = GetFont(false, 7f);
                Font fNormal8 = GetFont(false, 8f);
                Font fBold8 = GetFont(true, 8f);
                Font fBold9 = GetFont(true, 9f);
                Font fBold10 = GetFont(true, 10f);

                Document document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                writer.RunDirection = runDirection;
                document.Open();

                var lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');
                string imageURL = Server.MapPath(strLogotitle[0]);
                Image logo = Image.GetInstance(imageURL);
                logo.ScaleToFit(80f, 80f);

                PdfPTable tableHeader = new PdfPTable(new[] { 30f, 40f, 30f });
                tableHeader.WidthPercentage = 100;
                tableHeader.RunDirection = runDirection;
                tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell orgTitle = new PdfPCell(new Phrase(strLogotitle[1], fBold10));
                orgTitle.Border = 0;
                tableHeader.AddCell(orgTitle);

                PdfPCell logoCell = new PdfPCell(logo) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                tableHeader.AddCell(logoCell);

                PdfPTable dtSub = new PdfPTable(1);
                dtSub.DefaultCell.Border = Rectangle.NO_BORDER;
                dtSub.RunDirection = runDirection;
                dtSub.AddCell(new Phrase(GlobalVariables.GetStringResource("lblDate") + ": " + DateTime.Now.ToString("dd-MMM-yyyy"), fNormal8));
                dtSub.AddCell(new Phrase(GlobalVariables.GetStringResource("lblTime") + ": " + DateTime.Now.ToString("hh:mm tt"), fNormal8));
                PdfPCell dtCell = new PdfPCell(dtSub) { Border = 0, HorizontalAlignment = isArabic ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT, RunDirection = runDirection };
                tableHeader.AddCell(dtCell);

                document.Add(tableHeader);
                document.Add(new Paragraph("\n"));
                document.Add(lineSeparator);

                PdfPTable top = new PdfPTable(2);
                top.WidthPercentage = 100;
                top.RunDirection = runDirection;
                top.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPTable info = new PdfPTable(1);
                info.RunDirection = runDirection;
                info.DefaultCell.Border = Rectangle.NO_BORDER;
                info.AddCell(new PdfPCell(new Phrase(GlobalVariables.GetStringResource("dashboard.Name") + ": " + sdata.employeeName, fBold9)) { Border = 0 });
                info.AddCell(new PdfPCell(new Phrase(GlobalVariables.GetStringResource("monthly.epcode") + ": " + sdata.employeeCode, fBold9)) { Border = 0 });
                info.AddCell(new PdfPCell(new Phrase(GlobalVariables.GetStringResource("report.date") + ": " + string.Format("{0}/{1}/{2} - {3}/{4}/{5}", sdata.fromDay, sdata.fromMonth, sdata.fromYear, sdata.toDay, sdata.toMonth, sdata.toYear), fBold9)) { Border = 0 });
                top.AddCell(new PdfPCell(info) { Border = 0 });

                PdfPCell reportTitle = new PdfPCell(new Phrase(GlobalVariables.GetStringResource("title.workhourstimesheetreport"), fBold10));
                reportTitle.Border = 0;
                reportTitle.HorizontalAlignment = isArabic ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
                reportTitle.RunDirection = runDirection;
                top.AddCell(reportTitle);
                document.Add(top);

                PdfPTable table = new PdfPTable(new[] { 12f, 12f, 12f, 12f, 12f, 14f, 13f, 13f });
                table.WidthPercentage = 100;
                table.HeaderRows = 1;
                table.RunDirection = runDirection;
                table.SpacingBefore = 8f;

                Func<string, PdfPCell> headerCell = (text) =>
                {
                    return new PdfPCell(new Phrase(text, fBold8))
                    {
                        BackgroundColor = Color.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        RunDirection = runDirection
                    };
                };

                table.AddCell(headerCell(GlobalVariables.GetStringResource("report.date")));
                table.AddCell(headerCell(GlobalVariables.GetStringResource("report.remarks")));
                table.AddCell(headerCell(GlobalVariables.GetStringResource("report.timein")));
                table.AddCell(headerCell(GlobalVariables.GetStringResource("report.timeout")));
                table.AddCell(headerCell(GlobalVariables.GetStringResource("lblOvertime")));
                table.AddCell(headerCell(GlobalVariables.GetStringResource("report.statusout")));
                table.AddCell(headerCell(GlobalVariables.GetStringResource("report.terminalin")));
                table.AddCell(headerCell(GlobalVariables.GetStringResource("report.terminalout")));

                foreach (var log in sdata.logs)
                {
                    string finalRemarks = (log.finalRemarks ?? "") + (log.hasManualAttendance ? "*" : "");
                    table.AddCell(new PdfPCell(new Phrase(log.date ?? "", fNormal8)) { HorizontalAlignment = Element.ALIGN_CENTER, RunDirection = runDirection });
                    table.AddCell(new PdfPCell(new Phrase(finalRemarks, fNormal8)) { RunDirection = runDirection });
                    table.AddCell(new PdfPCell(new Phrase(log.timeIn ?? "", fNormal8)) { HorizontalAlignment = Element.ALIGN_CENTER, RunDirection = runDirection });
                    table.AddCell(new PdfPCell(new Phrase(log.timeOut ?? "", fNormal8)) { HorizontalAlignment = Element.ALIGN_CENTER, RunDirection = runDirection });
                    table.AddCell(new PdfPCell(new Phrase(log.overtime2 ?? "", fNormal8)) { HorizontalAlignment = Element.ALIGN_CENTER, RunDirection = runDirection });
                    table.AddCell(new PdfPCell(new Phrase(log.remarksOut ?? "", fNormal8)) { RunDirection = runDirection });
                    table.AddCell(new PdfPCell(new Phrase(log.terminalIn ?? "", fNormal7)) { RunDirection = runDirection });
                    table.AddCell(new PdfPCell(new Phrase(log.terminalOut ?? "", fNormal7)) { RunDirection = runDirection });
                }

                document.Add(table);
                document.Add(new Paragraph(GlobalVariables.GetStringResource("lblSystemGenerated"), fNormal7) { SpacingBefore = 8f });
                document.Close();

                return File(ms.ToArray(), "application/pdf", BuildPdfFileName("Monthly-Working-Hours-Timesheet"));
            }
        }

        private ActionResult GenerateWorkHoursTimeSheetPdfMonthlyStyle(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
            if (sdata == null)
            {
                return null;
            }

            int parsedYear = DateTime.Now.Year;
            int parsedMonth = DateTime.Now.Month;
            DateTime parsedMonthYear;
            if (DateTime.TryParseExact((sdata.month ?? string.Empty) + "-" + (sdata.year ?? string.Empty), new[] { "MM-yyyy", "MMM-yyyy", "MMMM-yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedMonthYear))
            {
                parsedYear = parsedMonthYear.Year;
                parsedMonth = parsedMonthYear.Month;
            }

            BLL.PdfReports.CustomRangeTimeSheetData adapted = new BLL.PdfReports.CustomRangeTimeSheetData
            {
                employeeName = sdata.employeeName,
                employeeCode = sdata.employeeCode,
                logs = sdata.logs,
                fromDay = "1",
                fromMonth = sdata.month,
                fromYear = sdata.year,
                toDay = DateTime.DaysInMonth(parsedYear, parsedMonth).ToString(),
                toMonth = sdata.month,
                toYear = sdata.year
            };

            return GenerateWorkHoursTimeSheetPdfMonthlyStyle(adapted);
        }



        private int GenerateMonthlyTimeOvertimeSheetPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(strLogotitle[1], fBold14));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    // ---------- Header Table ---------------------
                    ////string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    //////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    ////Image logo = Image.GetInstance(imageURL);
                    //////logo.Width = 140.0f;
                    //////logo.Alignment = Element.ALIGN_LEFT;
                    //////logo.ScaleToFit(140f, 20f);
                    //////logo.ScaleAbsolute(140f, 20f);
                    //////logo.SpacingBefore = 5f;
                    //////logo.SpacingAfter = 5f;

                    ////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////tableHeader.WidthPercentage = 100;
                    ////tableHeader.HeaderRows = 0;
                    //////tableHeader.SpacingBefore = 50;
                    ////tableHeader.SpacingAfter = 3;
                    ////tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    ////tableHeader.AddCell(logo);
                    ////tableHeader.AddCell("");

                    ////PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //////cellDateTime.HorizontalAlignment = 2;
                    ////cellDateTime.Border = 0;
                    ////tableHeader.AddCell(cellDateTime);

                    //////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    ////document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellEName = new PdfPCell(new Phrase("Name: " + sdata.employeeName, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + sdata.employeeCode, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase("Month: " + sdata.month, fBold9));
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);

                    PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.year, fBold9));
                    cellEYear.Border = 0;
                    tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Monthly Time Sheet - Overtime", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //Paragraph p1 = new Paragraph("Name: " + sdata.employeeName, fBold9);
                    ////p1.SpacingBefore = 10;
                    //document.Add(p1);

                    //Paragraph p2 = new Paragraph("Employee Code: " + sdata.employeeCode, fBold9);
                    //document.Add(p2);

                    //Paragraph p3 = new Paragraph("Month: " + sdata.month, fBold9);
                    //document.Add(p3);

                    //Paragraph p4 = new Paragraph("Year: " + sdata.year, fBold9);
                    //document.Add(p4);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Time In", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Time Out", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Final Remarks", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Overtime", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Overtime Status", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    foreach (BLL.PdfReports.MonthlyTimeSheetLog log in sdata.logs)
                    {
                        {
                            log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                        }



                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.finalRemarks, fNormal8));
                        //cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.overtime2, fNormal8));
                        //cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.overtime_status, fNormal8));
                        //PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                        cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);
                    }

                    if (sdata.logs.Length > 0)
                    {
                        document.Add(tableMid);


                        // Summary heading
                        Paragraph p_summary = new Paragraph("Summary", fBold10);
                        document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        tableEnd.WidthPercentage = 100;
                        tableEnd.HeaderRows = 0;
                        tableEnd.SpacingBefore = 3;
                        tableEnd.SpacingAfter = 3;

                        PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        tableEnd.AddCell(lt_cell_11);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Late:", fBold9));
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Absent:", fBold9));
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        PdfPCell lt_cell_32 = new PdfPCell(new Phrase("Leave:", fBold9));
                        tableEnd.AddCell(lt_cell_32);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        tableEnd.AddCell(lt_cell_41);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Approved Overtime:", fBold9));
                        tableEnd.AddCell(lt_cell_51);
                        tableEnd.AddCell(new Phrase(" " + sdata.ApprovedOvertime, fNormal8));

                        PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Unapproved Overtime:", fBold9));
                        tableEnd.AddCell(lt_cell_61);
                        tableEnd.AddCell(new Phrase(" " + sdata.UnapprovedOvertime, fNormal8));

                        PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Discarded Overtime:", fBold9));
                        tableEnd.AddCell(lt_cell_71);
                        tableEnd.AddCell(new Phrase(" " + sdata.DiscartOvertime, fNormal8));


                        PdfPCell lt_cell_81 = new PdfPCell(new Phrase("Total Overtime:", fBold9));
                        tableEnd.AddCell(lt_cell_81);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalOvertime, fNormal8));

                        document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, LV-Leave, PLO-Present Late & left On Time, PLE-Present Late & Early Out, POE-Present On Time & Early Out, PLM-Present Late & Miss Punch, PME-Present Miss Punch & Early Out, POM-Present On Time & Miss Punch, OV-Official Visit, OT-Official Travel, OM-Official Meeting, TR-Traning, *-Manually Updated", fNormal7);
                        p_abrv.SpacingBefore = 1;
                        //p_nsig.Alignment = 2;
                        document.Add(p_abrv);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=" + BuildPdfFileName("Report"));
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }


        private int GenerateWorkHoursTimeSheetPDFNew(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
            int reponse = 0;

            try
            {
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\Arial.ttf", BaseFont.IDENTITY_H, true);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                using (MemoryStream ms = new MemoryStream())
                {

                    Font fBold7 = FontFactory.GetFont(BaseFont.HELVETICA, 7, Font.BOLD, Color.BLACK);
                    Font fNormal7Green = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.GREEN);
                    Font fNormal7Red = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.RED);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);

                    Image logo = Image.GetInstance(imageURL);


                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 35.0f, 100.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;



                    PdfPCell cellTitle = new PdfPCell(new Phrase(strLogotitle[1], font));
                    cellTitle.HorizontalAlignment = 0;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);
                    tableHeader.AddCell(logo);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    cellDateTime.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    cellDateTime.RunDirection = PdfLayoutHelper.RunDirection;
                    tableHeader.AddCell(cellDateTime);

                    document.Add(tableHeader);



                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;


                    PdfPCell cellEName = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("dashboard.Name") + sdata.employeeName, font));
                    cellEName.Border = 0;
                    cellEName.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellEName);


                    PdfPCell cellECode = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.epcode") + ":" + sdata.logs[0].overtime_status, font));
                    cellECode.Border = 0;
                    cellECode.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblmonthyear") + sdata.logs[0].remarksIn, font));
                    cellEMonth.Border = 0;
                    cellEMonth.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellEMonth);


                    PdfPCell cellETitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblmonthtitlehours"), font));
                    cellETitle.Border = 0;
                    cellETitle.RunDirection = PdfLayoutHelper.RunDirection;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);





                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 60.0f, 60.0f, 60.0f, 80.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Menu.emp.date"), font));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    cell1.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblday"), font));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    cell2.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.timein"), font));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    cell3.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.timeout"), font));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    cell4.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.st"), font));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    cell5.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("preremarks"), font));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    cell6.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell6);

                    foreach (BLL.PdfReports.MonthlyTimeSheetLog log in sdata.logs)
                    {
                        {
                            log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                        }


                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.day, fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.status, fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.finalRemarks, fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);


                    }

                    if (sdata.logs.Length > 0)
                    {

                        document.Add(tableMid);

                        PdfPCell p_summary = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblSummary.rpt"), font));
                        p_summary.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Add(p_summary);

                        // Summary heading
                        //Paragraph p_summary = new Paragraph(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblSummary.rpt"), font);
                        //document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        tableEnd.WidthPercentage = 100;
                        tableEnd.HeaderRows = 0;
                        tableEnd.SpacingBefore = 3;
                        tableEnd.SpacingAfter = 3;

                        PdfPCell lt_cell_11 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthours"), font));
                        lt_cell_11.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_11);
                        tableEnd.AddCell(new Phrase(" " + sdata.logs[sdata.logs.Count() - 1].totalShfit_Hour + "/140 hrs", fNormal8));

                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthourslate"), font));
                        lt_cell_21.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + sdata.logs[sdata.logs.Count() - 1].totalLateHours + " hrs", fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthoursearly"), font));
                        lt_cell_31.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + sdata.logs[sdata.logs.Count() - 1].totalOvertimeHour.ToString().Replace("-", "") + " hrs", font));

                        PdfPCell lt_cell_32 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthoursdaypresent"), font));
                        lt_cell_32.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_32);
                        tableEnd.AddCell(new Phrase(" " + (sdata.totalPresent), fNormal8));

                        PdfPCell lt_cell_33 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthoursabsent"), font));
                        lt_cell_33.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_33);
                        tableEnd.AddCell(new Phrase(" " + (sdata.totalAbsent), fNormal8));

                        tableEnd.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Add(tableEnd);




                        Paragraph p_nsig = new Paragraph(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg") + ";     " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));


                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);


                        // ------------- close PDF Document and download it automatically


                        writer.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Close();

                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=" + BuildPdfFileName("Monthly-Working-Hours-Timesheet"));
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        private int GenerateWorkHoursTimeSheetPDFAlt(BLL.PdfReports.CustomRangeTimeSheetData sdata)
        {
            int reponse = 0;

            try
            {
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\Arial.ttf", BaseFont.IDENTITY_H, true);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.NORMAL);

                using (MemoryStream ms = new MemoryStream())
                {
                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');


                    //PdfPCell cellLogoTitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("dashboard.Name"), font));
                    //string[] strLogotitle = new string[] { cellLogoTitle.Phrase.Content };


                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    document.Open();

                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 35.0f, 100.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellDateTime = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.date") + "\n \n" + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"), font));
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    cellDateTime.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    cellDateTime.RunDirection = PdfLayoutHelper.RunDirection;
                    tableHeader.AddCell(cellDateTime);
                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource(strLogotitle[1] + "\n" + "Ministry of Energy and Infrastructure"), fontTitle));
                    //cellTitle.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    cellTitle.Border = 0;
                    cellTitle.RunDirection = PdfLayoutHelper.RunDirection;
                    tableHeader.AddCell(cellTitle);

                    //PdfPCell cellDateTime = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.date") + "\n\n" + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"), font));
                    //PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    //cellDateTime.PaddingTop = 2.0f;
                    // cellDateTime.Border = 0;
                    // cellDateTime.RunDirection = PdfLayoutHelper.RunDirection;
                    //tableHeader.AddCell(cellDateTime);

                    //PdfPCell cellTime = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.time") + "\n" + DateTime.Now.ToString("hh:mm tt"), font));
                    ////PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    ////cellDateTime.HorizontalAlignment = 2;
                    //cellTime.PaddingTop = 4.0f;
                    //cellTime.Border = 0;
                    //cellTime.RunDirection = PdfLayoutHelper.RunDirection;
                    //tableHeader.AddCell(cellTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);



                    // ---------- Header Table ---------------------
                    //string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    ////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    //Image logo = Image.GetInstance(imageURL);

                    //PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableHeader.WidthPercentage = 100;
                    //tableHeader.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableHeader.SpacingAfter = 3;
                    //tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    //tableHeader.AddCell(logo);
                    //tableHeader.AddCell("");

                    //PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    ////cellDateTime.HorizontalAlignment = 2;
                    //cellDateTime.Border = 0;
                    //tableHeader.AddCell(cellDateTime);

                    ////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    //document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;


                    PdfPCell cellEName = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("dashboard.Name") + '\n' + sdata.employeeName, font));
                    cellEName.Border = 0;
                    cellEName.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellEName);

                    iTextSharp.text.Font smallFont = new iTextSharp.text.Font(bf, 3, iTextSharp.text.Font.NORMAL);
                    PdfPCell cellSpace = new PdfPCell(new Phrase("\n", smallFont));
                    cellSpace.Border = 0;
                    tableEInfo.AddCell(cellSpace);


                    PdfPCell cellECode = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.epcode") + sdata.employeeCode, font));
                    cellECode.Border = 0;
                    cellECode.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.date") + String.Format("\n{0}/{1}/{2} - {3}/{4}/{5}", sdata.fromDay, sdata.fromMonth, sdata.fromYear, sdata.toDay, sdata.toMonth, sdata.toYear), font));
                    cellEMonth.Border = 0;
                    cellEMonth.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellEMonth);














                    //PdfPCell cellETitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblmonthtitlehours"), font));
                    //cellETitle.Border = 0;
                    //cellETitle.RunDirection = PdfLayoutHelper.RunDirection;
                    //cellETitle.HorizontalAlignment = 2;

                    PdfPCell cellETitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("title.workhourstimesheetreport"), font));
                    cellETitle.Border = 0;
                    cellETitle.RunDirection = PdfLayoutHelper.RunDirection;
                    cellETitle.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;




                    tableEmployee.AddCell(cellETitle);
                    tableEmployee.AddCell(tableEInfo);
                    //tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);


                    PdfPTable tableMid = new PdfPTable(new[] { 51.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell0 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.remarks"), font));
                    cell0.BackgroundColor = Color.LIGHT_GRAY;
                    cell0.HorizontalAlignment = 1;
                    cell0.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell0);

                    PdfPCell cell1 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.statusout"), font));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    cell1.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.terminalout"), font));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    cell2.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.timeout"), font));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    cell3.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.statusin"), font));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    cell4.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.terminalin"), font));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    cell5.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.timein"), font));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    cell6.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.day"), font));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    cell7.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.date"), font));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    cell8.RunDirection = PdfLayoutHelper.RunDirection;
                    tableMid.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.empcode"), font));
                    //cell9.BackgroundColor = Color.LIGHT_GRAY;
                    //cell9.HorizontalAlignment = 1;
                    //cell9.RunDirection = PdfLayoutHelper.RunDirection;
                    //tableMid.AddCell(cell9);
                    var lookup = new Dictionary<string, string>(){
                        {"Monday", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Monday")},
                        {"Tuesday", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Tuesday")},
                        {"Wednesday", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Wednesday")},
                        {"Thursday", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Thursday")},
                        {"Friday", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Friday")},
                        {"Saturday", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Saturday")},
                        {"Sunday", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Sunday")},
                        {"Off", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Off")},
                        {"OFF", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("OFF")},
                        {"Miss Punch", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Miss Punch")},
                        {"Early Out", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Early Out")},
                        {"Late", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Late")},
                        {"On Time", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("On Time")},
                        {"On-Time", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("On-Time")},
                        {"Absent", @MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Absent")}
                    };
                    //Color greenColor = new iTextSharp.text.Color(175, 225, 175);
                    //Color redColor = new iTextSharp.text.Color(255, 120, 100);
                    //Color orangeColor = new iTextSharp.text.Color(252, 207, 73);
                    //Color blueColor = new iTextSharp.text.Color(102, 205, 170);
                    //Color whiteColor = null;
                    //Color GreenColor = new iTextSharp.text.Color(80, 148, 46);


                    Color greenColor = new iTextSharp.text.Color(127, 185, 145);
                    Color redColor = new iTextSharp.text.Color(255, 203, 199);
                    Color orangeColor = new iTextSharp.text.Color(250, 249, 217);
                    Color blueColor = new iTextSharp.text.Color(102, 205, 170);
                    Color whiteColor = null;

                    Color GreenColor = new iTextSharp.text.Color(100, 170, 60);
                    Color darkblueColor = new iTextSharp.text.Color(0, 0, 139);

                    foreach (BLL.PdfReports.MonthlyTimeSheetLog log in sdata.logs)
                    {
                        {
                            log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                        }
                        Color color = greenColor;
                        switch (log.finalRemarks)
                        {
                            case "OFF":
                                color = orangeColor;
                                break;
                            case "POM":
                                color = whiteColor;
                                break;
                            case "PLM":
                                color = whiteColor;
                                break;
                            case "AB":
                                color = redColor;
                                break;
                            default:
                                color = whiteColor;
                                break;
                        }

                        PdfPCell cellData0 = new PdfPCell(new Phrase(log.remarks, font));
                        cellData0.BackgroundColor = color;
                        cellData0.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData0.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData0);

                        String statusOut = (log.remarks == "غياب" || log.remarks == "عطلة الاسبوع") ? "" : log.status.Split('$')[1];
                        statusOut = lookup.Keys.Contains(statusOut) ? lookup[statusOut] : statusOut;
                        Font fontForCell1 = new Font(font);
                        fontForCell1.Color = darkblueColor;
                        PdfPCell cellData1 = new PdfPCell(new Phrase(statusOut, fontForCell1));
                        cellData1.HorizontalAlignment = 1;
                        cellData1.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData1.BackgroundColor = color;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.terminalOut, font));
                        cellData2.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData2.HorizontalAlignment = 1;
                        cellData2.BackgroundColor = color;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        cellData3.BackgroundColor = color;
                        tableMid.AddCell(cellData3);

                        String statusIn = (log.remarks == "غياب" || log.remarks == "عطلة الاسبوع") ? "" : log.status.Split('$')[0];
                        statusIn = lookup.Keys.Contains(statusIn) ? lookup[statusIn] : statusIn;
                        Font fontForCell4 = new Font(font);
                        fontForCell4.Color = GreenColor;
                        PdfPCell cellData4 = new PdfPCell(new Phrase(statusIn, fontForCell4));
                        cellData4.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData4.HorizontalAlignment = 1;
                        cellData4.BackgroundColor = color;

                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.terminalIn, font));
                        cellData5.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData5.HorizontalAlignment = 1;
                        cellData5.BackgroundColor = color;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        cellData6.BackgroundColor = color;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource(log.day), font));
                        cellData7.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData7.HorizontalAlignment = 1;
                        cellData7.BackgroundColor = color;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.date, fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        cellData8.BackgroundColor = color;
                        tableMid.AddCell(cellData8);

                        //PdfPCell cellData9 = new PdfPCell(new Phrase(sdata.employeeCode, fNormal8));
                        //cellData9.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData9);

                    }

                    if (sdata.logs.Length > 0)
                    {

                        document.Add(tableMid);

                        //Paragraph p_summary = new Paragraph(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblSummary.rpt"), font));
                        //p_summary.SpacingBefore = 1;
                        //document.Add(p_summary);

                        // Summary heading
                        //Paragraph p_summary = new Paragraph(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblSummary.rpt"), font);
                        //document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        // Calculate total shift hours

                        PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        tableEnd.WidthPercentage = 100;
                        tableEnd.HeaderRows = 0;
                        tableEnd.SpacingBefore = 3;
                        tableEnd.SpacingAfter = 3;

                        PdfPCell lt_cell_11 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthours"), font));
                        lt_cell_11.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_11);
                        double TotalHours = sdata.logs[sdata.logs.Count() - 1].GrandTotal_Hour;
                        //tableEnd.AddCell(new Phrase($" {sdata.logs[sdata.logs.Count() - 1].totalShfit_Hour:00} : {sdata.logs[sdata.logs.Count() - 1].totalShfit_Mins:00} / {Math.Floor(TotalHours):00} : {Math.Round((TotalHours - Math.Floor(TotalHours)) * 60):00}", fNormal8));
                        //tableEnd.AddCell(new Phrase($" {sdata.logs[sdata.logs.Count() - 1].totalShfit_Hour:00} : {sdata.logs[sdata.logs.Count() - 1].totalShfit_Mins:00}", fNormal8));
                        tableEnd.AddCell(new Phrase($" {sdata.logs[sdata.logs.Count() - 1].totalShfit_Hour:00} : {sdata.logs[sdata.logs.Count() - 1].totalShfit_Mins:00}", fNormal8));


                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthourslate"), font));
                        lt_cell_21.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase($" {sdata.logs[sdata.logs.Count() - 1].totalLateHours:00} : {sdata.logs[sdata.logs.Count() - 1].totalLateMins:00}", fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthoursearly"), font));
                        lt_cell_31.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase($" {sdata.logs[sdata.logs.Count() - 1].totalOvertimeHour:00} : {sdata.logs[sdata.logs.Count() - 1].totalOvertimeMins:00}", fNormal8));

                        PdfPCell lt_cell_32 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthoursdaypresent"), font));
                        lt_cell_32.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_32);
                        tableEnd.AddCell(new Phrase(" " + (sdata.totalPresent), fNormal8));

                        PdfPCell lt_cell_33 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblthoursabsent"), font));
                        lt_cell_33.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_33);
                        tableEnd.AddCell(new Phrase(" " + (sdata.totalAbsent), fNormal8));

                        PdfPCell lt_cell_34 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.absentHours"), font));
                        lt_cell_34.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_34);
                        double absentHours = 0;
                        if (int.Parse(sdata.totalAbsent) + int.Parse(sdata.totalPresent) > 0)
                        {
                            absentHours = sdata.logs[sdata.logs.Count() - 1].GrandTotal_Hour * Double.Parse(sdata.totalAbsent) / (Double.Parse(sdata.totalAbsent) + Double.Parse(sdata.totalPresent));
                        }
                        tableEnd.AddCell(new Phrase($"{Math.Floor(absentHours):00} : {Math.Round((absentHours - Math.Floor(absentHours)) * 60):00}", fNormal8));

                        tableEnd.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Add(tableEnd);

                        PdfPTable tableEndSummary = new PdfPTable(new[] { 100.0f });
                        tableEndSummary.WidthPercentage = 100;
                        tableEndSummary.HeaderRows = 0;
                        tableEndSummary.SpacingBefore = 15;
                        tableEndSummary.SpacingAfter = 3;
                        tableEndSummary.DefaultCell.Border = Rectangle.NO_BORDER;


                        PdfPCell tesCell1 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.remarks") + "   " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        tesCell1.RunDirection = PdfLayoutHelper.RunDirection;
                        tesCell1.Border = Rectangle.NO_BORDER;
                        tableEndSummary.AddCell(tesCell1);

                        PdfPCell tesCell2 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg") + "   " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        tesCell2.RunDirection = PdfLayoutHelper.RunDirection;
                        tesCell2.Border = Rectangle.NO_BORDER;
                        tableEndSummary.AddCell(tesCell2);

                        tableEndSummary.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Add(tableEndSummary);

                        //Paragraph p_nsigremarks = new Paragraph(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg.remarks") + "   " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        //p_nsigremarks.SpacingBefore = 1;
                        //// p_nsig.RunDirection = PdfLayoutHelper.RunDirection;
                        //document.Add(p_nsigremarks);


                        //Paragraph p_nsig = new Paragraph(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg") + "   " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        //p_nsig.SpacingBefore = 1;
                        //// p_nsig.RunDirection = PdfLayoutHelper.RunDirection;
                        //document.Add(p_nsig);


                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.RunDirection = PdfLayoutHelper.RunDirection;
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=" + BuildPdfFileName("Monthly-Working-Hours-Timesheet"));
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        private int GenerateMonthlyTimeSheetPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
            int reponse = 0;

            try
            {

                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\Arial.ttf", BaseFont.IDENTITY_H, true);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);
                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above
                    //string path = Server.MapPath("Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Font.pdf", FileMode.Create));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 35.0f, 100.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;



                    PdfPCell cellTitle = new PdfPCell(new Phrase(strLogotitle[1], font));
                    cellTitle.HorizontalAlignment = 0;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);
                    tableHeader.AddCell(logo);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    cellDateTime.RunDirection = PdfLayoutHelper.RunDirection;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);



                    // ---------- Header Table ---------------------
                    //string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    ////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    //Image logo = Image.GetInstance(imageURL);

                    //PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableHeader.WidthPercentage = 100;
                    //tableHeader.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableHeader.SpacingAfter = 3;
                    //tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    //tableHeader.AddCell(logo);
                    //tableHeader.AddCell("");

                    //PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    ////cellDateTime.HorizontalAlignment = 2;
                    //cellDateTime.Border = 0;
                    //tableHeader.AddCell(cellDateTime);

                    ////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    //document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);


                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellEName = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("dashboard.Name") + sdata.employeeName, font));
                    cellEName.Border = 0;
                    cellEName.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.epcode") + ":" + sdata.logs[0].overtime_status, font));
                    cellECode.Border = 0;
                    cellECode.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblmonthyear") + ":" + sdata.logs[0].remarksIn, font));
                    cellEMonth.Border = 0;
                    cellEMonth.RunDirection = PdfLayoutHelper.RunDirection;
                    tableEInfo.AddCell(cellEMonth);

                    PdfPCell cellEYear = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.year.grid") +":" + sdata.year, fBold9));
                    cellEYear.Border = 0;
                    tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);
                    
                    PdfPCell cellETitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Menu.emp.monthlytimesheet") + ":", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.RunDirection = PdfLayoutHelper.RunDirection;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //Paragraph p1 = new Paragraph("Name: " + sdata.employeeName, fBold9);
                    ////p1.SpacingBefore = 10;
                    //document.Add(p1);

                    //Paragraph p2 = new Paragraph("Employee Code: " + sdata.employeeCode, fBold9);
                    //document.Add(p2);

                    //Paragraph p3 = new Paragraph("Month: " + sdata.month, fBold9);
                    //document.Add(p3);

                    //Paragraph p4 = new Paragraph("Year: " + sdata.year, fBold9);
                    //document.Add(p4);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("Menu.emp.date"), font));
                    cell1.RunDirection = PdfLayoutHelper.RunDirection;
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.timein"), font));
                    cell2.RunDirection = PdfLayoutHelper.RunDirection;
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("grid.Remarksin"), font));
                    cell3.RunDirection = PdfLayoutHelper.RunDirection;
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("monthly.timeout"), font));
                    cell4.RunDirection = PdfLayoutHelper.RunDirection;
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("grid.Remarksout"), font));
                    cell5.RunDirection = PdfLayoutHelper.RunDirection;
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblfremarks"), font));
                    cell6.RunDirection = PdfLayoutHelper.RunDirection;
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblDescription"), font));
                    cell7.RunDirection = PdfLayoutHelper.RunDirection;
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbldin"), font));
                    cell8.RunDirection = PdfLayoutHelper.RunDirection;
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    PdfPCell cell9 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbldiut"), font));
                    cell9.RunDirection = PdfLayoutHelper.RunDirection;
                    cell9.BackgroundColor = Color.LIGHT_GRAY;
                    cell9.HorizontalAlignment = 1;
                    tableMid.AddCell(cell9);

                    foreach (BLL.PdfReports.MonthlyTimeSheetLog log in sdata.logs)
                    {
                        {
                            log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                        }

                        //PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, FontFactory.GetFont("Arial", 11, Font.NORMAL, Color.BLACK)));
                        //cellData1.HorizontalAlignment = 0; // 0 for left, 1 for Center - 2 for Right
                        //tableMid.AddCell(log.date);

                        //tableMid.AddCell(log.date);
                        //tableMid.AddCell(log.timeIn);
                        //tableMid.AddCell(log.remarksIn);
                        //tableMid.AddCell(log.timeOut);
                        //tableMid.AddCell(log.remarksOut);
                        //tableMid.AddCell(log.finalRemarks);
                        //tableMid.AddCell(log.terminalIn);
                        //tableMid.AddCell(log.terminalOut);

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.remarksIn, fNormal8));
                        //cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.remarksOut, fNormal8));
                        //cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.finalRemarks, fNormal8));
                        //cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.description, fNormal8));
                        //cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalIn, fNormal7));
                        //cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);

                        PdfPCell cellData9 = new PdfPCell(new Phrase(log.terminalOut, fNormal7));
                        //PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                        //cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData9);
                    }

                    if (sdata.logs.Length > 0)
                    {
                        document.Add(tableMid);


                        // Summary heading
                        Paragraph p_summary = new Paragraph("Summary", fBold10);
                        document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        tableEnd.WidthPercentage = 100;
                        tableEnd.HeaderRows = 0;
                        tableEnd.SpacingBefore = 3;
                        tableEnd.SpacingAfter = 3;

                        PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        tableEnd.AddCell(lt_cell_11);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Late:", fBold9));
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Absent:", fBold9));
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        PdfPCell lt_cell_32 = new PdfPCell(new Phrase("Leave:", fBold9));
                        tableEnd.AddCell(lt_cell_32);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        tableEnd.AddCell(lt_cell_41);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        tableEnd.AddCell(lt_cell_51);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, LV-Leave, PLO-Present Late & left On Time, PLE-Present Late & Early Out, POE-Present On Time & Early Out, PLM-Present Late & Miss Punch, PME-Present Miss Punch & Early Out, POM-Present On Time & Miss Punch, OV-Official Visit, OT-Official Travel, OM-Official Meeting, TR-Traning, *-Manually Updated", fNormal7);
                        p_abrv.SpacingBefore = 1;
                        //p_nsig.Alignment = 2;
                        document.Add(p_abrv);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=" + BuildPdfFileName("Report"));
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        [HttpPost]
        public JsonResult MonthlyTimesheetReportForEMPDataHandlernews(MonthlyTimesheetEMPReportTable param)
        {
            try
            {
                int employeeID = 0;



                if (param.employee_id.ToUpper() == "TYPE_EMP")
                {
                    employeeID = TimeTune.EmpReports.getEmployeeID(User.Identity.Name.ToString());
                    param.employee_id = employeeID.ToString();
                }
                if (param.from_date.ToLower() == "undefined--undefined")
                {
                    param.from_date = DateTime.Now.ToString("01-MM-yyyy");
                }
                if (param.to_date.ToLower() == "undefined--undefined")
                {
                    int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    //param.to_date = days + "-" + DateTime.Now.ToString("MM-yyyy");
                }

                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models
                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                int count = TimeTune.Reports.getMonthlyWorkingHoursTimesheetByEmployeeIdNew(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
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
        public JsonResult MonthlyTimesheetReportForEMPDataHandler(MonthlyTimesheetEMPReportTable param)
        {
            try
            {
                int employeeID = 0;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("MonthlyTimeSheet");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("MonthlyTimeSheet");

                if (param.employee_id.ToUpper() == "TYPE_EMP")
                {
                    employeeID = TimeTune.EmpReports.getEmployeeID(User.Identity.Name.ToString());
                    param.employee_id = employeeID.ToString();
                }
                if (param.from_date.ToLower() == "undefined--undefined")
                {
                    param.from_date = DateTime.Now.ToString("01-MM-yyyy");
                }
                if (param.to_date.ToLower() == "undefined--undefined")
                {
                    int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    //param.to_date = days + "-" + DateTime.Now.ToString("MM-yyyy");
                }

                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
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
        public JsonResult MonthlyTimesheetReportForEMPDataHandlerNew(MonthlyTimesheetEMPReportTable param)
        {
            try
            {
                int employeeID = 0;
                if (param.employee_id.ToUpper() == "TYPE_EMP")
                {
                    employeeID = TimeTune.EmpReports.getEmployeeID(User.Identity.Name.ToString());
                    param.employee_id = employeeID.ToString();
                }

                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                int count = TimeTune.Reports.getMonthlyWorkingHoursTimesheetByEmployeeIdAlt(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data, ViewModel.GlobalVariables.GV_Langauge);

                DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
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

        public class MonthlyTimesheetEMPReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        #endregion

        #region Attendance Summary
        public ActionResult AttendanceSummary()
        {
            return View();
        }
        public class CustomDTParameters : DTParameters
        {
            public string from_date { get; set; }
            public string to_date { get; set; }
        }
        [HttpPost]
        public JsonResult AttendanceSummaryDataHandler(CustomDTParameters param)
        {
            try
            {


                var data = new List<ViewModels.MonthlyReport>();
                int count = TimeTune.EmpReports.getMonthlyReport(param.Search.Value, param.SortOrder, param.Start, param.Length, out data, param.from_date, param.to_date, User.Identity.Name.ToString());

                DTResult<ViewModels.MonthlyReport> result = new DTResult<ViewModels.MonthlyReport>
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

        #region DevicesStatusCountReport



        public ActionResult DevicesStatusCountReport()
        {
            if (ViewModel.GlobalVariables.GV_Rpt01Perm == false)
            {
                return RedirectPermanent("/EMP/EMP/Dashboard");
            }

            List<C_office> list = new List<C_office>();

            list = BLL_UNIS.UNIS_Reports.getCOfficeList();




            return View(list);
        }


        [HttpPost]
        public JsonResult DevicesStatusCountReportDataHandler(DeviceStatusRegionReportTable param)
        {
            try
            {
                List<BLL_UNIS.ViewModels.DevicesStatusCount> data = new List<DevicesStatusCount>();

                data = BLL_UNIS.UNIS_Reports.getDevicesStatusCountList(param.device_region_id, param.Search.Value, param.SortOrder, param.Start, param.Length);

                List<BLL_UNIS.ViewModels.DevicesStatusCount> data2 = Devices_StatusCount_ResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<BLL_UNIS.ViewModels.DevicesStatusCount> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatusCount>
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

        [HttpPost]
        public JsonResult GetDevicesStatusCount(int region_id)
        {
            try
            {
                List<DevicesStatusCount> data = new List<DevicesStatusCount>();

                string list_count = BLL_UNIS.UNIS_Reports.getDevicesStatusCount(region_id);

                string[] split = list_count.Split(':');
                if (split.Length > 0)
                {
                    return Json(new { status = "success", connected = split[0], disconnected = split[1], total = split[2] });
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }

            return Json(new { status = "success", connected = 0, disconnected = 0, total = 0 });
        }


        public class DeviceStatusRegionReportTable : DTParameters
        {
            public int device_region_id { get; set; }
        }

        public class DeviceStatusReportTable : DTParameters
        {
            public string device_number { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public string device_status_type { get; set; }
        }


        #endregion

    }
}
