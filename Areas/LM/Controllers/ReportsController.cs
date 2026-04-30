using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using TimeTune;
using Rotativa;
using BLL.ViewModels;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using OfficeOpenXml;
using System.Data;
using System.Reflection;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections;
using BLL_UNIS.ViewModels;
using MvcApplication1.ViewModel;

namespace MvcApplication1.Areas.LM.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_LM)]
    public class ReportsController : Controller
    {
        //
        // GET: /LM/Reports/

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

        #region ConsolidatedReport

        public ActionResult ConsolidatedAttendance()
        {
            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);
            //loading the employee code in view
            return View(manualAttendance);
        }

        public class ConsolidatedReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult DataHandler(ConsolidatedReportTable param)
        {
            try
            {
                var dtsource = new List<ConsolidatedAttendanceLog>();

                //dtsource = TimeTune.AttendanceLM.getAllConsolidateAttendanceBySupervisor(param.Search.Value, param.employee_id, param.from_date, param.to_date, User.Identity.Name);

                // get all employee view models
                dtsource = TimeTune.AttendanceLM.getAllConsolidateAttendanceLMBySupervisor(param.employee_id, param.from_date, param.to_date, User.Identity.Name, param.Search.Value, param.SortOrder, param.Start, param.Length);
                if (dtsource == null)
                {
                    dtsource = new List<ConsolidatedAttendanceLog>();

                }
                List<ConsolidatedAttendanceLog> data = dtsource; // ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = dtsource.Count; // ConsolidateAttendanceResultSet.Count(param.Search.Value, data);



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
                ViewModels.Dashboard dashboard = TimeTune.Dashboard.graphForConsolidatedForLm(param.employee_id, param.from_date, param.to_date);
                return Json(dashboard);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }
        }
        #endregion

        #region DirectDownloadExcelXLSXVersion
        [HttpPost]
        public ActionResult AttendanceSummaryExcelDownload(CustomDTParameters param)
        {
            var data = new List<AttendanceSummaryExport>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                // int count = TimeTune.Reports.getMonthlyReportExcel(out data, param.from_date, param.to_date);
                int count = TimeTune.LmReports.getMonthlyReportLMExcel(out data, param.from_date, param.to_date, User.Identity.Name.ToString());
                if (data.Count() > 0)
                {
                    var products = ToDataTable<AttendanceSummaryExport>(data);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("AttendanceSummary");
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
                Data = new { FileGuid = handle, FileName = "Attendance-Summary-Log.xlsx" }
            };
        }
        [HttpPost]
        public ActionResult ConsolidatedReportExcelDownload(ConsolidatedReportTable param)
        {
            var dtsource = new List<ConsolidatedAttendanceExport>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                //get EmployeeID By EmployeeCode
                ////string emp_id = TimeTune.Reports.getEmployeeIDByEmployeeCode(param.employee_id);

                ////int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(emp_id, param.from_date, param.to_date, out ddata);

                dtsource = TimeTune.AttendanceLM.getAllConsolidateAttendanceBySupervisorExport(null, param.employee_id, param.from_date, param.to_date, User.Identity.Name);
                if (dtsource == null)
                {
                    dtsource = new List<ConsolidatedAttendanceExport>();
                }

                if (dtsource.Count() > 0)
                {
                    var products = ToDataTable<ConsolidatedAttendanceExport>(dtsource);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("ConsolidatedAttendanceLogLM");
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
                Data = new { FileGuid = handle, FileName = "Consolidated-Attendance-Log-" + User.Identity.Name + "-LM.xlsx" }
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

        /*
        public ActionResult PDF()
        {
            return View();
        }
        public ActionResult PDFView()
        {
            return new ActionAsPdf("GeneratePDF");
        }

        public ActionResult GeneratePDF()
        {
            return View("PDF");
        }*/


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
                int count = TimeTune.LmReports.getMonthlyReportLM(param.Search.Value, param.SortOrder, param.Start, param.Length, out data, param.from_date, param.to_date, User.Identity.Name.ToString());

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

        #region MonthlyTimeSheetReport

        [HttpGet]
        [ActionName("MonthlyTimeSheet")]
        public ActionResult MonthlyTimeSheet_Get()
        {
            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            ViewData["PDFNoDataFound"] = "";

            //loading the employee code in view
            return View(manualAttendance);
        }

        [HttpPost]
        public JsonResult FillEmployeeCode()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatchingLM(q, User.Identity.Name.ToString());


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].id.ToString();
                toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        [HttpPost]
        [ActionName("MonthlyTimeSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheet_Post()
        {
            int employeeID;
            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyTimeSheet");

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

            if (toRender == null)
                return RedirectToAction("MonthlyTimeSheet");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };


            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateMonthlyTimeSheetPDF(toRender);

            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            return View(manualAttendance);


            //return RedirectToAction("GenerateReport", "Reports");
        }


        [HttpPost]
        [ActionName("MonthlyTimeSheetNew")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheetNew_Post()
        {
            int employeeID;
            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyTimeSheet");

            string month = Request.Form["month"];


            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();
            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getWorkingHourReportNew(employeeID, month);


            if (toRender == null)
                return RedirectToAction("MonthlyTimeSheetNew");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };


            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateWorkHoursTimeSheetPDFNew(toRender);

            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            return View(manualAttendance);


            //return RedirectToAction("GenerateReport", "Reports");
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

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), font));
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    cellDateTime.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
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


                        PdfPCell p_nsig = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg") + ";     " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        p_nsig.RunDirection = PdfLayoutHelper.RunDirection;



                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);


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


        [HttpGet]
        [ActionName("MonthlyTimeSheetNew")]
        public ActionResult MonthlyTimeSheetNew_Get()
        {
            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            ViewData["PDFNoDataFound"] = "";

            //loading the employee code in view
            return View(manualAttendance);
        }



        private int GenerateMonthlyTimeSheetPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
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

                    //// To save file in a specific folder of project, also remove MemoryStream code above
                    //string path = Server.MapPath("Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Font.pdf", FileMode.Create));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);//"~/Content/Logos/logo-default.png"
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
                    logo.SetAbsolutePosition((PageSize.A4.Width - logo.ScaledWidth) / 2, (PageSize.A4.Height - logo.ScaledHeight) / 2);
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(strLogotitle[1], fBold14));//"DUHS - DOW University of Health Sciences"
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

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Monthly Time Sheet", fBold16));
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
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f });

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

                    PdfPCell cell3 = new PdfPCell(new Phrase("Remarks In", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Time Out", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Remarks Out", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Final Remarks", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Description", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Device In", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    PdfPCell cell9 = new PdfPCell(new Phrase("Device Out", fBold8));
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
        public JsonResult MonthlyTimesheetReportForLMDataHandler(MonthlyTimesheetLMReportTable param)
        {
            try
            {
                //int employeeID = 0;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("GenerateReport");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("GenerateReport");

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
        public JsonResult MonthlyTimesheetReportForLMDataHandlerNew(MonthlyTimesheetLMReportTable param)
        {
            try
            {
                //int employeeID = 0;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("GenerateReport");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("GenerateReport");

                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models
                //  int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);
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



        public class MonthlyTimesheetLMReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        #endregion

        #region MonthlyTimeSheetReportOvertime

        public ActionResult GenerateReportOvertime0()
        {   // @(System.Web.HttpContext.Current.Session["PDFDownloadEnabled"] ?? "inline-block" );

            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            return View();
        }

        [HttpGet]
        [ActionName("MonthlyTimeSheetOvertime")]
        public ActionResult MonthlyTimeSheetOvertime_Get()
        {
            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            ViewData["PDFNoDataFound"] = "";

            //loading the employee code in view
            return View(manualAttendance);
        }

        [HttpPost]
        [ActionName("MonthlyTimeSheetOvertime")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheetOvertime_Post()
        {
            int employeeID;
            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyTimeSheet");

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

            if (toRender == null)
                return RedirectToAction("MonthlyTimeSheet");

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

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            return View(manualAttendance);


            //return RedirectToAction("MonthlyTimeSheet", "Reports");

        }

        private int GenerateMonthlyTimeSheetOvertimePDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
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

                    PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);
                    tableHeader.AddCell("");

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

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

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Monthly Time Sheet", fBold16));
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
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 60.0f, 60.0f, 60.0f, 80.0f, 95.0f, 95.0f });

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

                    PdfPCell cell3 = new PdfPCell(new Phrase("Remarks In", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Time Out", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Remarks Out", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Final Remarks", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Device In", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Device Out", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

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

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminalIn, fNormal7));
                        //cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalOut, fNormal7));
                        //PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                        //cellData8.HorizontalAlignment = 1;
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

                        PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        tableEnd.AddCell(lt_cell_41);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Total Overtime:", fBold9));
                        tableEnd.AddCell(lt_cell_51);
                        tableEnd.AddCell(new Phrase(" " + sdata.FinalOvertime, fNormal8));

                        PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        tableEnd.AddCell(lt_cell_61);
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
        public ActionResult MonthlyTimeSheetOvertimeStatus(ViewModels.ConsolidatedAttendanceLog toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.StatusUpdate.Update(toUpdate);
            return Json(new { status = "success" });
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


        public class MonthlyTimesheetReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        [HttpPost]
        public JsonResult MonthlyTimesheetReportDataHandler(MonthlyTimesheetReportTable param)
        {
            try
            {
                //int employeeID;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("MonthlyTimeSheet");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("MonthlyTimeSheet");


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

        #endregion


        #region ConsolidateOvertime
        public ActionResult ConsolidatedAttendanceOvertime()
        {
            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);
            //loading the employee code in view
            return View(manualAttendance);
        }

        [HttpPost]
        public ActionResult ConsolidatedAttendanceOvertimeStatus(ViewModels.ConsolidatedAttendanceLog toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.StatusUpdate.Update(toUpdate);
            return Json(new { status = "success" });
        }


        [HttpPost]
        public JsonResult ConsolidatedReportDataHandler(ConsolidatedReportTable param)
        {
            try
            {
                var dtsource = new List<ConsolidatedAttendanceLog>();

                //dtsource = TimeTune.AttendanceLM.getAllConsolidateAttendanceBySupervisor(param.Search.Value, param.employee_id, param.from_date, param.to_date, User.Identity.Name);

                // get all employee view models
                dtsource = TimeTune.AttendanceLM.getAllConsolidateAttendanceLMBySupervisor(param.employee_id, param.from_date, param.to_date, User.Identity.Name, param.Search.Value, param.SortOrder, param.Start, param.Length);
                if (dtsource == null)
                {
                    dtsource = new List<ConsolidatedAttendanceLog>();

                }
                //List<ConsolidatedAttendanceLog> data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = dtsource.Count; // ConsolidateAttendanceResultSet.Count(param.Search.Value, data);



                DTResult<ConsolidatedAttendanceLog> result = new DTResult<ConsolidatedAttendanceLog>
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

        [HttpPost]
        public ActionResult ConsolidatedReportOvertimeExcelDownload(ConsolidatedReportTable param)
        {
            var dtsource = new List<ConsolidatedAttendanceExportOvertime>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                //get EmployeeID By EmployeeCode
                ////string emp_id = TimeTune.Reports.getEmployeeIDByEmployeeCode(param.employee_id);

                ////int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(emp_id, param.from_date, param.to_date, out ddata);

                dtsource = TimeTune.AttendanceLM.getAllConsolidateAttendanceBySupervisorExportOvertime(null, param.employee_id, param.from_date, param.to_date, User.Identity.Name);
                if (dtsource == null)
                {
                    dtsource = new List<ConsolidatedAttendanceExportOvertime>();
                }

                if (dtsource.Count() > 0)
                {
                    var products = ToDataTable<ConsolidatedAttendanceExportOvertime>(dtsource);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("ConsolidatedAttendanceLogLM");
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
                Data = new { FileGuid = handle, FileName = "Consolidated-Attendance-Log-" + User.Identity.Name + "-LM.xlsx" }
            };
        }


        #endregion

        #region LeaveBalanceCountANDListReport


        public ActionResult LeavesListReport()
        {
            return View();
        }

        public class LeavesListReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public int users_type { get; set; }
        }

        [HttpPost]
        public JsonResult LeavesListReportDataHandler(LeavesListReportTable param)
        {
            try
            {
                var data = new List<LeavesListReportLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllLeavesListReportForLMMatching(User.Identity.Name, param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<LeavesListReportLog> result = new DTResult<LeavesListReportLog>
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


        //////////////////////////////////////////////////////////////

        [HttpGet]
        [ActionName("LeavesCountReport")]
        public ActionResult LeavesCountReport_Get()
        {
            string emp_code = "", ss_year = "";
            int session_year = DateTime.Now.Year;
            List<LeavesCountReportLog> data = new List<LeavesCountReportLog>();
            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();

            if (ViewModel.GlobalVariables.GV_EmployeeId != null && ViewModel.GlobalVariables.GV_EmployeeId != "")
            {
                ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(int.Parse(ViewModel.GlobalVariables.GV_EmployeeId));
                ViewBag.SessionYearsList = yearList;

                if (Request.QueryString["e_code"] != null && Request.QueryString["e_code"].ToString() != "" && Request.QueryString["s_year"] != null && Request.QueryString["s_year"].ToString() != "")
                {
                    emp_code = Request.QueryString["e_code"];
                    ss_year = Request.QueryString["s_year"];
                    session_year = int.Parse(ss_year);
                    ViewBag.SelectedSessionYear = ss_year;
                    ViewBag.SelectedEmployeeCode = " Detail of Employee Code: " + emp_code;
                }
                else
                {
                    emp_code = User.Identity.Name;
                    ss_year = DateTime.Now.Year.ToString();
                    session_year = int.Parse(yearList[0].ToString());
                    ViewBag.SelectedSessionYear = session_year;
                    ViewBag.SelectedEmployeeCode = "";
                }

                // get all employee view models
                TimeTune.Attendance.getLeavesCountReportByEmpCode(emp_code, session_year.ToString(), out data);
                // int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);

                //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);


                foreach (LeavesCountReportLog item in data)
                {
                    ViewBag.AllocatedSickLeaves = item.AllocatedSickLeaves;
                    ViewBag.AllocatedCasualLeaves = item.AllocatedCasualLeaves;
                    ViewBag.AllocatedAnnualLeaves = item.AllocatedAnnualLeaves;

                    ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                    ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                    ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
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

                    ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                    ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                    ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
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

                if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(session_year.ToString() + "-12-31 23:59:00.000"))
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

        ///////////////////////////////////////////////////////////////////

        [HttpGet]
        [ActionName("LeavesBalanceReport")]
        public ActionResult LeavesBalanceReport_Get()
        {
            string emp_code = "", ss_year = "";
            int session_year = DateTime.Now.Year;

            List<LeavesCountReportLog> data = new List<LeavesCountReportLog>();
            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = ViewBag.LMEmployeesList = TimeTune.LMManualAttendance.getEmployeeBySupervisor(User.Identity.Name);

            ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(-1);
            ViewBag.SessionYearsList = yearList;

            if (Request.QueryString["e_code"] != null && Request.QueryString["e_code"].ToString() != "" && Request.QueryString["s_year"] != null && Request.QueryString["s_year"].ToString() != "")
            {
                emp_code = Request.QueryString["e_code"];
                ss_year = Request.QueryString["s_year"];
                session_year = int.Parse(ss_year);
                ViewBag.SelectedSessionYear = ss_year;
                ViewBag.SelectedEmployeeCode = " Detail of Employee Code: " + emp_code;

                // get all employee view models
                TimeTune.Attendance.getLeavesCountReportByEmpCode(emp_code, ss_year, out data);
                // int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);

                //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                if (data != null && data.Count > 0)
                {
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
                }
                else
                {
                    ViewBag.AllocatedSickLeaves = 0;
                    ViewBag.AllocatedCasualLeaves = 0;
                    ViewBag.AllocatedAnnualLeaves = 0;
                    ViewBag.AllocatedOtherLeaves = 0;
                    ViewBag.AllocatedLeaveType01 = 0;
                    ViewBag.AllocatedLeaveType02 = 0;
                    ViewBag.AllocatedLeaveType03 = 0;
                    ViewBag.AllocatedLeaveType04 = 0;

                    ViewBag.AvailedSickLeaves = 0;
                    ViewBag.AvailedCasualLeaves = 0;
                    ViewBag.AvailedAnnualLeaves = 0;
                    ViewBag.AvailedOtherLeaves = 0;
                    ViewBag.AvailedLeaveType01 = 0;
                    ViewBag.AvailedLeaveType02 = 0;
                    ViewBag.AvailedLeaveType03 = 0;
                    ViewBag.AvailedLeaveType04 = 0;
                }

                lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpCode(User.Identity.Name, session_year);
            }
            else
            {
                //emp_code = "-1"; // User.Identity.Name;
                //ss_year = DateTime.Now.Year.ToString();
                //session_year = int.Parse(yearList[0].ToString());
                //ViewBag.SelectedSessionYear = session_year;
                //ViewBag.SelectedEmployeeCode = "";
            }

            return View(lInfo);
        }

        [HttpPost]
        [ActionName("LeavesBalanceReport")]
        public ActionResult LeavesBalanceReport_Post()
        {
            string str_employee_id = "";
            string session_year = DateTime.Now.Year.ToString();
            var data = new List<LeavesBalanceReportLog>();

            if (Request.Form["employee_id"] != null && Request.Form["employee_id"].ToString() != "")
            {
                str_employee_id = Request.Form["employee_id"].ToString();
                ViewBag.SelectedEmployeeId = str_employee_id;
            }

            if (Request.Form["session_year"] != null && Request.Form["session_year"].ToString() != "")
            {
                session_year = Request.Form["session_year"].ToString();
                ViewBag.SelectedSessionYear = session_year;
            }

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = ViewBag.LMEmployeesList = ViewBag.LMEmployeesList = TimeTune.LMManualAttendance.getEmployeeBySupervisor(User.Identity.Name);

            ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(-1);
            ViewBag.SessionYearsList = yearList;
            //session_year = yearList[0].ToString();

            // get all employee view models
            TimeTune.Attendance.getLeavesBalanceReportByEmpID(int.Parse(str_employee_id), session_year, out data);
            // int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);

            //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

            if (data != null && data.Count > 0)
            {
                foreach (LeavesBalanceReportLog item in data)
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
            }
            else
            {
                ViewBag.AllocatedSickLeaves = 0;
                ViewBag.AllocatedCasualLeaves = 0;
                ViewBag.AllocatedAnnualLeaves = 0;
                ViewBag.AllocatedOtherLeaves = 0;
                ViewBag.AllocatedLeaveType01 = 0;
                ViewBag.AllocatedLeaveType02 = 0;
                ViewBag.AllocatedLeaveType03 = 0;
                ViewBag.AllocatedLeaveType04 = 0;

                ViewBag.AvailedSickLeaves = 0;
                ViewBag.AvailedCasualLeaves = 0;
                ViewBag.AvailedAnnualLeaves = 0;
                ViewBag.AvailedOtherLeaves = 0;
                ViewBag.AvailedLeaveType01 = 0;
                ViewBag.AvailedLeaveType02 = 0;
                ViewBag.AvailedLeaveType03 = 0;
                ViewBag.AvailedLeaveType04 = 0;
            }

            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();
            lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpID(int.Parse(str_employee_id), int.Parse(session_year));

            return View(lInfo);
        }


        public class LeavesBalanceReportTable : DTParameters
        {
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public ActionResult LeavesBalanceReportTableReportDataHandler(LeavesBalanceReportTable param, string from_date, string to_date)
        {
            int session_year = DateTime.Now.Year;

            try
            {
                var data = new List<LeavesBalanceReportLog>();

                if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(session_year.ToString() + "-12-31 23:59:00.000"))
                {
                    session_year = session_year + 1;
                }

                // get all employee view models
                ////TimeTune.Attendance.getLeavesBalanceReportByEmpID(User.Identity.Name, session_year.ToString(), out data);
                //TimeTune.Attendance.getLeavesCountReportByEmpCode(User.Identity.Name, session_year.ToString(), out data);
                //TimeTune.Attendance.getLeavesCountReportForLM(param.Search.Value, param.from_date, param.to_date, User.Identity.Name, out data);
                // int count = ConsolidateAttendanceResultSet.Count(param.Search.Value, data);

                //data = ConsolidateAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<LeavesBalanceReportLog> result = new DTResult<LeavesBalanceReportLog>
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

            //return View("LeavesBalanceReport");
        }


        #endregion

        #region monthly-employee-shifts

        [HttpGet]
        [ActionName("MonthlyEmployeeShifts")]
        public ActionResult MonthlyEmployeeShifts_Get()
        {
            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            ViewData["PDFNoDataFound"] = "";

            //loading the employee code in view
            return View(manualAttendance);
        }

        [HttpPost]
        public JsonResult MonthlyEmpShiftReportForLMDataHandler(MonthlyEmpShiftLMReportTable param)
        {
            try
            {
                //int employeeID = 0;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("GenerateReport");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("GenerateReport");

                var data = new List<MonthlyEmployeeShiftLog>();

                // get all employee view models
                int count = TimeTune.Reports.getMonthlyEmployeeShiftReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<MonthlyEmployeeShiftLog> result = new DTResult<MonthlyEmployeeShiftLog>
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

        public class MonthlyEmpShiftLMReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        [ActionName("MonthlyEmployeeShifts")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyEmployeeShifts_Post()
        {
            int employeeID;
            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyEmployeeShifts");

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.MonthlyEmpShiftData toRender = reportMaker.getReportMonthlyEmpShiftData(employeeID, month);

            if (toRender == null)
                return RedirectToAction("MonthlyEmployeeShifts");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };


            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateMonthlyEmployeeShiftsPDF(toRender);

            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisorRecursion(User.Identity.Name);

            return View(manualAttendance);


            //return RedirectToAction("GenerateReport", "Reports");
        }

        private int GenerateMonthlyEmployeeShiftsPDF(BLL.PdfReports.MonthlyEmpShiftData sdata)
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

                    //// To save file in a specific folder of project, also remove MemoryStream code above
                    //string path = Server.MapPath("Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Font.pdf", FileMode.Create));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]);//"~/Content/Logos/logo-default.png"
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

                    PdfPCell cellTitle = new PdfPCell(new Phrase(strLogotitle[1], fBold14));//"DUHS - DOW University of Health Sciences"
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

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Monthly Employee Shifts", fBold16));
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
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 150.0f, 100.0f, 100.0f, 100.0f, 100.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Shift Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Start", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Late Time", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Half Time", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("End Time", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    foreach (BLL.PdfReports.MonthlyEmpShiftLog log in sdata.logs)
                    {
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
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.shift_name, fNormal8));
                        //cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.shift_start_time, fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.shift_late_time, fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.shift_half_time, fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.shift_end_time, fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);
                    }

                    if (sdata.logs.Length > 0)
                    {
                        document.Add(tableMid);

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



        #endregion


        #region DevicesStatusCountReport



        public ActionResult DevicesStatusCountReport()
        {
            if (ViewModel.GlobalVariables.GV_Rpt01Perm == false)
            {
                return RedirectPermanent("/LM/LM/Dashboard");
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
