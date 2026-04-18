using BLL.ViewModels;
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
using System.Reflection;
using OfficeOpenXml;
using System.Configuration;
using BLL_UNIS.ViewModels;
using MvcApplication1.ViewModel;

namespace MvcApplication1.Areas.Report.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_REPORT)]
    public class ReportsController : Controller
    {


        #region MonthlyTimeSheetReport

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

            return View("MonthlyTimeSheet");


            //return RedirectToAction("MonthlyTimeSheet", "Reports");
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

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
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
                        Response.AddHeader("content-disposition", "attachment;filename=Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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

        public class MonthlyTimesheetReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        #endregion

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
                int count = TimeTune.Reports.getMonthlyReport(param.Search.Value, param.SortOrder, param.Start, param.Length, out data, param.from_date, param.to_date);

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

        #region ViewConsolidatedAttendance

        public ActionResult ConsolidatedAttendance()
        {
            return View();
        }

        public class ConsolidatedReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult ConsolidatedReportDataHandler(ConsolidatedReportTable param)
        {
            try
            {


                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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
                ViewModels.Dashboard dashboard = TimeTune.Dashboard.graphForConsolidated(param.employee_id, param.from_date, param.to_date);
                return Json(dashboard);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }
        }
        #endregion

        #region  change Employee Password
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
                toSend[i].text = employees[i].employee_code;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }
        #endregion

        #region Department Wise Report
        public ActionResult DepartmentReport()
        {
            return View();
        }
        [HttpPost]
        public JsonResult graph(string dept, string des, string loc, string from, string to)
        {
            ViewModels.Dashboard dashboard = TimeTune.Dashboard.getGraphValues(dept, des, User.Identity.Name, loc, from, to);
            return Json(dashboard);

        }

        public class ConsolidatedDepartmentReportTable : DTParameters
        {
            public string department_id { get; set; }
            public string designation_id { get; set; }
            public string location_id { get; set; }
            public string function_id { get; set; }
            public string region_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult DepartmentdDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Department[] department = TimeTune.EmployeeManagementHelper.getAllDepartmentsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[department.Length];
            for (int i = 0; i < department.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = department[i].id.ToString();
                toSend[i].text = department[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }
        [HttpPost]
        public JsonResult DesignationDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Designation[] designation = TimeTune.EmployeeManagementHelper.getAllDesignationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[designation.Length];
            for (int i = 0; i < designation.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = designation[i].id.ToString();
                toSend[i].text = designation[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }
        [HttpPost]
        public JsonResult LocationDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Location[] location = TimeTune.EmployeeManagementHelper.getAllLocationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[location.Length];
            for (int i = 0; i < location.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = location[i].id.ToString();
                toSend[i].text = location[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }
        [HttpPost]
        public JsonResult FunctionDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Function[] function = TimeTune.EmployeeManagementHelper.getAllFunctionsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[function.Length];
            for (int i = 0; i < function.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = function[i].id.ToString();
                toSend[i].text = function[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        [HttpPost]
        public JsonResult RegionDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Region[] region = TimeTune.EmployeeManagementHelper.getAllRegionsMatching(q);

            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[region.Length];
            for (int i = 0; i < region.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = region[i].id.ToString();
                toSend[i].text = region[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        [HttpPost]
        public JsonResult DepartmentReportHandler(ConsolidatedDepartmentReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceDepartmentWise>();
                int count = TimeTune.Reports.getAllDepartmentLogsDepartmentWise(param.department_id, param.designation_id, param.function_id, param.region_id, User.Identity.Name, param.location_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise> result = new DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                //  PdfReport(result);

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConsolidatedFilteredReports()
        {
            //string department_id, string designation_id, string location_id, string function_id, string from_date, string to_date
            //Request.Form["department_id"]
            //Request.Form["from_date"];
            int fun_id, reg_id, depart_id, des_id, loc_id;

            if (!int.TryParse(Request.Form["function_id"], out fun_id))
                fun_id = -1;
            if (!int.TryParse(Request.Form["region_id"], out reg_id))
                reg_id = -1;
            if (!int.TryParse(Request.Form["department_id"], out depart_id))
                depart_id = -1;
            if (!int.TryParse(Request.Form["designation_id"], out des_id))
                des_id = -1;
            if (!int.TryParse(Request.Form["location_id"], out loc_id))
                loc_id = -1;

            string from_date = Request.Form["from_date"];
            string to_date = Request.Form["to_date"];

            if (from_date == null && to_date == null)
            {
                return RedirectToAction("MonthlyTimeSheet");
            }

            BLL.ViewModels.FilteredAttendanceReportDepartmentWise reportMaker = new BLL.ViewModels.FilteredAttendanceReportDepartmentWise();

            reportMaker.logs = TimeTune.Reports.getAttendanceDepartmentWise(depart_id, des_id, loc_id, fun_id, reg_id, from_date, to_date);

            if (reportMaker == null)
                return RedirectToAction("MonthlyTimeSheet");

            ////return new Rotativa.ViewAsPdf("ConsolidatedFilteredReports", reportMaker) { FileName = "reports.pdf" };

            // -------------------------------------------

            GenerateDepartmentWiseReportPDF(reportMaker);

            return null;
        }


        private void GenerateDepartmentWiseReportPDF(BLL.ViewModels.FilteredAttendanceReportDepartmentWise sdata)
        {
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
                    Font fBold7 = FontFactory.GetFont("HELVETICA", 7, Font.BOLD, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 7, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 1f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above
                    //string path = Server.MapPath("Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Font.pdf", FileMode.Create));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
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

                    Paragraph p_title = new Paragraph("Department Wise Report", fBold16);
                    p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    tableEmployee.AddCell(p_title);

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
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 70.0f, 70.0f, 60.0f, 70.0f, 60.0f, 70.0f, 60.0f, 95.0f, 70.0f, 70.0f, 70.0f, 70.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold7));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Employee Code", fBold7));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("First Name", fBold7));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Last Name", fBold7));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Time In", fBold7));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Remarks In", fBold7));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Time Out", fBold7));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Remarks Out", fBold7));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    PdfPCell cell9 = new PdfPCell(new Phrase("Final Remarks", fBold8));
                    cell9.BackgroundColor = Color.LIGHT_GRAY;
                    cell9.HorizontalAlignment = 1;
                    tableMid.AddCell(cell9);

                    PdfPCell cell10 = new PdfPCell(new Phrase("Function", fBold7));
                    cell10.BackgroundColor = Color.LIGHT_GRAY;
                    cell10.HorizontalAlignment = 1;
                    tableMid.AddCell(cell10);

                    PdfPCell cell11 = new PdfPCell(new Phrase("Region", fBold7));
                    cell11.BackgroundColor = Color.LIGHT_GRAY;
                    cell11.HorizontalAlignment = 1;
                    tableMid.AddCell(cell11);

                    PdfPCell cell12 = new PdfPCell(new Phrase("Department", fBold7));
                    cell12.BackgroundColor = Color.LIGHT_GRAY;
                    cell12.HorizontalAlignment = 1;
                    tableMid.AddCell(cell12);

                    PdfPCell cell13 = new PdfPCell(new Phrase("Designation", fBold7));
                    cell13.BackgroundColor = Color.LIGHT_GRAY;
                    cell13.HorizontalAlignment = 1;
                    tableMid.AddCell(cell13);

                    PdfPCell cell14 = new PdfPCell(new Phrase("Location", fBold7));
                    cell14.BackgroundColor = Color.LIGHT_GRAY;
                    cell14.HorizontalAlignment = 1;
                    tableMid.AddCell(cell14);

                    foreach (ViewModels.ConsolidatedAttendanceDepartmentWise log in sdata.logs)
                    {
                        {
                            //log.final_remarks = log.final_remarks + ((log) ? "*" : "");
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

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, fNormal7));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.employee_code, fNormal7));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.employee_first_name, fNormal7));
                        //cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.employee_last_name, fNormal7));
                        //cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.time_in, fNormal7));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.status_in, fNormal7));
                        //cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.time_out, fNormal7));
                        cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.status_out, fNormal7));
                        //cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);

                        PdfPCell cellData9 = new PdfPCell(new Phrase(log.final_remarks, fNormal7));
                        //cellData9.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData9);

                        PdfPCell cellData10 = new PdfPCell(new Phrase(log.function, fNormal7));
                        //cellData10.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData10);

                        PdfPCell cellData11 = new PdfPCell(new Phrase(log.region, fNormal7));
                        //cellData11.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData11);

                        PdfPCell cellData12 = new PdfPCell(new Phrase(log.department, fNormal7));
                        //cellData12.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData12);

                        PdfPCell cellData13 = new PdfPCell(new Phrase(log.designation, fNormal7));
                        //cellData13.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData13);

                        PdfPCell cellData14 = new PdfPCell(new Phrase(log.location, fNormal7));
                        //cellData10.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData14);
                    }

                    if (sdata.logs.Count > 0)
                    {
                        document.Add(tableMid);
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);
                    }

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
                    Response.AddHeader("content-disposition", "attachment;filename=Report.pdf");
                    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception)
            {
                //handle exception
            }
        }

        #endregion

        #region DirectDownloadExcelXLSXVersion

        [HttpPost]
        public ActionResult ConsolidatedReportExcelDownload(ConsolidatedReportTable param)
        {
            var ddata = new List<ConsolidatedAttendanceExport>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(param.employee_id, param.from_date, param.to_date, out ddata);

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

        #endregion

        #region DevicesStatusRegionReport



        public ActionResult DevicesStatusRegionReport()
        {


            List<C_office> list = new List<C_office>();

            list = BLL_UNIS.UNIS_Reports.getCOfficeList();




            return View(list);
        }



        #endregion

        #region DevicesStatusCountReport



        public ActionResult DevicesStatusCountReport()
        {


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
                List<BLL_UNIS.ViewModels.DevicesStatusCount> data = new List<DevicesStatusCount>();

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
