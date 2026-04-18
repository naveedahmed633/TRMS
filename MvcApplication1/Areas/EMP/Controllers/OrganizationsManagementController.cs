using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ViewModels;
using System.IO;
using System.Globalization;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text;
using TimeTune;
using MvcApplication1.ViewModel;

namespace MvcApplication1.Areas.EMP.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_EMP)]
    public class OrganizationsManagementController : Controller
    {


        #region Schedule-PDF-Report

        [HttpPost]
        public JsonResult ScheduleReportDataHandler(OrganizationCampusRoomCourseScheduleForm param)
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

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};

                return null; // Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("GenerateScheduleReport")]
        public ActionResult GenerateScheduleReport_Get()
        {
            return View();
        }


        [HttpPost]
        [ActionName("GenerateScheduleReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateScheduleReport_Post()
        {
            int campID = 0, stdID = 0, found = 0;
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            ScheduleReport reportMaker = new ScheduleReport();

            if (!int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out campID))
                return RedirectToAction("GenerateScheduleReport");

            if (!int.TryParse(ViewModel.GlobalVariables.GV_EmployeeId, out stdID))
                return RedirectToAction("GenerateScheduleReport");

            dtStart = DateTime.ParseExact(Request.Form["from_date"] + " 00:00:00", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture); //DateTime.ParseExact(Request.Form["from_date"], "dd-MM-yyyy",);
            dtEnd = DateTime.ParseExact(Request.Form["to_date"] + " 23:59:59", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture); //DateTime.ParseExact(Request.Form["to_date"], "dd-MM-yyyy", "");

            ScheduleReportData toRender = reportMaker.getScheduleDataForStudent(campID, stdID, dtStart, dtEnd);
            if (toRender == null)
                return RedirectToAction("GenerateScheduleReport");

            found = DownloadScheduleReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            return View();
        }


        private int DownloadScheduleReportPDF(ScheduleReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to download PDF file instead of saving the PDF file in a specific folder
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

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBoldUnderline16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 5f, 5f);

                    //// To download PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.org_logo_path); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 100.0f;
                    //logo.Height = 80.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 880.0f, 120.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.org_title + "\r\n" + sdata.campus_code + "\r\n" + sdata.program_code, fBold16));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 4.0f;
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

                    PdfPCell cellEName = new PdfPCell(new Phrase("Effective FROM " + sdata.date_range, fBold12));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase(sdata.room_code, fBold12));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //Paragraph p1 = new Paragraph("Name: " + sdata.employeeName, fBold9);
                    ////p1.SpacingBefore = 10;
                    //document.Add(p1);

                    //Paragraph p2 = new Paragraph("Employee Number: " + sdata.employeeCode, fBold9);
                    //document.Add(p2);

                    //Paragraph p3 = new Paragraph("Month: " + sdata.month, fBold9);
                    //document.Add(p3);

                    //Paragraph p4 = new Paragraph("Year: " + sdata.year, fBold9);
                    //document.Add(p4);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    //PdfPTable tableMid = new PdfPTable(new[] { 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f });
                    PdfPTable tableSchedule = new PdfPTable(sdata.cols_count);

                    tableSchedule.WidthPercentage = 100;
                    tableSchedule.HeaderRows = 1;
                    tableSchedule.SpacingBefore = 3;
                    tableSchedule.SpacingAfter = 1;

                    //PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold8));
                    //cell1.BackgroundColor = Color.LIGHT_GRAY;
                    //cell1.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell1);

                    //PdfPCell cell2 = new PdfPCell(new Phrase("Time In", fBold8));
                    //cell2.BackgroundColor = Color.LIGHT_GRAY;
                    //cell2.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell2);

                    //PdfPCell cell3 = new PdfPCell(new Phrase("Remarks In", fBold8));
                    //cell3.BackgroundColor = Color.LIGHT_GRAY;
                    //cell3.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell3);

                    //PdfPCell cell4 = new PdfPCell(new Phrase("Time Out", fBold8));
                    //cell4.BackgroundColor = Color.LIGHT_GRAY;
                    //cell4.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell4);

                    //PdfPCell cell5 = new PdfPCell(new Phrase("Remarks Out", fBold8));
                    //cell5.BackgroundColor = Color.LIGHT_GRAY;
                    //cell5.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell5);

                    //PdfPCell cell6 = new PdfPCell(new Phrase("Final Remarks", fBold8));
                    //cell6.BackgroundColor = Color.LIGHT_GRAY;
                    //cell6.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell6);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("Device In", fBold8));
                    //cell7.BackgroundColor = Color.LIGHT_GRAY;
                    //cell7.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell7);

                    //PdfPCell cell8 = new PdfPCell(new Phrase("Device Out", fBold8));
                    //cell8.BackgroundColor = Color.LIGHT_GRAY;
                    //cell8.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell8);

                    for (int i = 0; i < sdata.rows_count; i++)
                    {
                        for (int j = 0; j < sdata.cols_count; j++)
                        {
                            string strData = sdata.logs.Find(l => l.id_row == i && l.id_col == j).str_data;

                            PdfPCell cellData1 = new PdfPCell(new Phrase(strData, fNormal8));
                            cellData1.HorizontalAlignment = 1;
                            tableSchedule.AddCell(cellData1);
                        }
                    }


                    //foreach (ScheduleReportLog log in sdata.logs)
                    //{
                    //    //{
                    //    //    log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                    //    //}

                    //    //PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, FontFactory.GetFont("Arial", 11, Font.NORMAL, Color.BLACK)));
                    //    //cellData1.HorizontalAlignment = 0; // 0 for left, 1 for Center - 2 for Right
                    //    //tableMid.AddCell(log.date);

                    //    //tableMid.AddCell(log.date);
                    //    //tableMid.AddCell(log.timeIn);
                    //    //tableMid.AddCell(log.remarksIn);
                    //    //tableMid.AddCell(log.timeOut);
                    //    //tableMid.AddCell(log.remarksOut);
                    //    //tableMid.AddCell(log.finalRemarks);
                    //    //tableMid.AddCell(log.terminalIn);
                    //    //tableMid.AddCell(log.terminalOut);

                    //    PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, fNormal8));
                    //    //cellData1.HorizontalAlignment = 0;
                    //    tableMid.AddCell(cellData1);

                    //    PdfPCell cellData2 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                    //    cellData2.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData2);

                    //    PdfPCell cellData3 = new PdfPCell(new Phrase(log.remarksIn, fNormal8));
                    //    //cellData3.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData3);

                    //    PdfPCell cellData4 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                    //    cellData4.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData4);

                    //    PdfPCell cellData5 = new PdfPCell(new Phrase(log.remarksOut, fNormal8));
                    //    //cellData5.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData5);

                    //    PdfPCell cellData6 = new PdfPCell(new Phrase(log.finalRemarks, fNormal8));
                    //    //cellData6.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData6);

                    //    PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminalIn, fNormal7));
                    //    //cellData7.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData7);

                    //    PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalOut, fNormal7));
                    //    //PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                    //    //cellData8.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData8);
                    //}

                    if (sdata.logs != null && sdata.logs.Count > 0)
                    {
                        document.Add(tableSchedule);


                        // Summary heading
                        Paragraph p_summary = new Paragraph("Summary", fBold10);
                        ////document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        tableEnd.WidthPercentage = 100;
                        tableEnd.HeaderRows = 0;
                        tableEnd.SpacingBefore = 3;
                        tableEnd.SpacingAfter = 3;

                        PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        tableEnd.AddCell(lt_cell_11);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Late:", fBold9));
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Absent:", fBold9));
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        tableEnd.AddCell(lt_cell_41);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        tableEnd.AddCell(lt_cell_51);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        ////document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, LV-Leave, PLO-Present Late & left On Time, PLE-Present Late & Early Out, POE-Present On Time & Early Out, PLM-Present Late & Miss Punch, PME-Present Miss Punch & Early Out, POM-Present On Time & Miss Punch, OV-Official Visit, OT-Official Travel, OM-Official Meeting, TR-Traning, *-Manually Updated", fNormal7);
                        p_abrv.SpacingBefore = 1;
                        //p_nsig.Alignment = 2;
                        ////document.Add(p_abrv);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Schedule-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
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
            catch (Exception ex)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion


        #region Course-Attendance-Student-PDF-Report

        [HttpPost]
        public JsonResult CourseAttendanceStudentReportDataHandler(OrganizationCampusRoomCourseScheduleView param)
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

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};

                return null; // Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("GenerateCourseAttendanceStudentReport")]
        public ActionResult GenerateCourseAttendanceStudentReport_Get()
        {
            return View();
        }


        [HttpPost]
        [ActionName("GenerateCourseAttendanceStudentReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateCourseAttendanceStudentReport_Post()
        {
            int found = 0, iStudentID = 0; string strFromDate = "", strToDate = "";
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            CourseAttendanceReport reportMaker = new CourseAttendanceReport();

            //set for EMP Report Method
            iStudentID = int.Parse(ViewModel.GlobalVariables.GV_EmployeeId);

            if (Request.Form["from_date"] != null && Request.Form["from_date"].ToString() != "")
                strFromDate = Request.Form["from_date"];
            else
                return RedirectToAction("GenerateCourseAttendanceStudentReport");

            if (Request.Form["to_date"] != null && Request.Form["to_date"].ToString() != "")
                strToDate = Request.Form["to_date"];
            else
                return RedirectToAction("GenerateCourseAttendanceStudentReport");

            CourseAttendanceReportData toRender = reportMaker.getCourseAttendanceStudentReport(strFromDate, strToDate, iStudentID);
            if (toRender == null)
                return RedirectToAction("GenerateCourseAttendanceStudentReport");

            found = DownloadCourseAttendanceStudentReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            return View();
        }

        private int DownloadCourseAttendanceStudentReportPDF(CourseAttendanceReportData sdata)
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

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
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

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.orgLogo); //Server.MapPath("~/images/hbl-logo.png");
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

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.orgName + "\n" + sdata.campusName + "\n" + sdata.progCode + "-" + sdata.progTitle, fBold14));
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

                    PdfPCell cellEName = new PdfPCell(new Phrase("Student Code & Name: " + sdata.employeeCode + " - " + sdata.employeeName, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase("Father's Name: " + sdata.employeeFatherName, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellRCode = new PdfPCell(new Phrase("Enrolled In: " + sdata.enrolledProgramText + "-" + sdata.enrolledProgramNumber, fBold9));
                    cellRCode.Border = 0;
                    tableEInfo.AddCell(cellRCode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase("Attendance Date Range: " + sdata.dateRange, fBold9));
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);

                    //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.toDate, fBold9));
                    //cellEYear.Border = 0;
                    //tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("", fBold16));
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
                    PdfPTable tableMid = new PdfPTable(new[] { 10.0f, 22.0f, 10.0f, 22.0f, 10.0f, 10.0f, 26.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    //PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold8));
                    //cell1.BackgroundColor = Color.LIGHT_GRAY;
                    //cell1.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Course Code", fBold9));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Time In [Actual]", fBold9));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Remarks In", fBold9));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Time Out [Actual]", fBold9));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Remarks Out", fBold9));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Final Remarks", fBold9));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Device In/Out", fBold9));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase("Device Out", fBold9));
                    //cell9.BackgroundColor = Color.LIGHT_GRAY;
                    //cell9.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell9);

                    if (sdata.logs.Length > 0)
                    {
                        int j = 1, m = 0;
                        bool isNewHeading = false;
                        string strCurrDate = "", strPrevDate = "";

                        foreach (CourseAttendanceLog log in sdata.logs)
                        {
                            if (j == 1)
                            {
                                isNewHeading = true;
                                strCurrDate = log.schedule_date_time_text.ToString();
                                strPrevDate = log.schedule_date_time_text.ToString();

                                m = 1;
                            }
                            else
                            {
                                if (strCurrDate == log.schedule_date_time_text.ToString())
                                {
                                    strCurrDate = strPrevDate;
                                    isNewHeading = false;

                                    m++;
                                }
                                else
                                {
                                    strCurrDate = log.schedule_date_time_text.ToString();
                                    strPrevDate = log.schedule_date_time_text.ToString();
                                    isNewHeading = true;

                                    m = 1;
                                }
                            }

                            if (isNewHeading)
                            {
                                tableMid.AddCell(new PdfPCell(new Phrase("DATE: " + strCurrDate, fBold9)) { Colspan = 7 });
                            }

                            //PdfPCell cellData1 = new PdfPCell(new Phrase(log.schedule_date_time_text, fNormal8));
                            ////cellData1.HorizontalAlignment = 0;
                            //tableMid.AddCell(cellData1);

                            if (log.course_code != null && log.course_code == "-")
                            {
                                PdfPCell cellData2 = new PdfPCell(new Phrase("OFF", fNormal8));
                                cellData2.HorizontalAlignment = 1;
                                tableMid.AddCell(cellData2);
                            }
                            else
                            {
                                PdfPCell cellData2 = new PdfPCell(new Phrase(log.course_code, fNormal8));
                                cellData2.HorizontalAlignment = 1;
                                tableMid.AddCell(cellData2);
                            }

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.student_time_in_text, fNormal8));
                            cellData3.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.status_in, fNormal8));
                            //cellData4.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.student_time_out_text, fNormal8));
                            cellData5.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.status_out, fNormal8));
                            //cellData6.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData6);

                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.final_remarks, fNormal8));
                            //cellData7.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData7);

                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminal_in_code, fNormal7));
                            //cellData8.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData8);

                            //PdfPCell cellData9 = new PdfPCell(new Phrase(log.terminal_out_code, fNormal7));
                            ////cellData9.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData9);

                            j++;
                        }

                        document.Add(tableMid);
                    }

                    //Summary Count table
                    if (sdata.counts.Length > 0)
                    {
                        // Summary heading
                        Paragraph p_summary = new Paragraph("Summary", fBold10);
                        document.Add(p_summary);

                        // ---------- Count Table ---------------------
                        //set table with 595 pixels width - subtract 10x2 from either sides border
                        PdfPTable tableCount = new PdfPTable(new[] { 40.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f });
                        tableCount.WidthPercentage = 100;
                        tableCount.HeaderRows = 1;
                        tableCount.SpacingBefore = 3;
                        tableCount.SpacingAfter = 3;

                        PdfPCell cellc1 = new PdfPCell(new Phrase("Course Code", fBold8));
                        cellc1.BackgroundColor = Color.LIGHT_GRAY;
                        cellc1.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc1);

                        PdfPCell cellc1_1 = new PdfPCell(new Phrase("Total Classes", fBold8));
                        cellc1_1.BackgroundColor = Color.LIGHT_GRAY;
                        cellc1_1.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc1_1);

                        PdfPCell cellc2 = new PdfPCell(new Phrase("Present", fBold8));
                        cellc2.BackgroundColor = Color.LIGHT_GRAY;
                        cellc2.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc2);

                        PdfPCell cellc3 = new PdfPCell(new Phrase("Absent", fBold8));
                        cellc3.BackgroundColor = Color.LIGHT_GRAY;
                        cellc3.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc3);

                        PdfPCell cellc4 = new PdfPCell(new Phrase("Late", fBold8));
                        cellc4.BackgroundColor = Color.LIGHT_GRAY;
                        cellc4.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc4);

                        PdfPCell cellc5 = new PdfPCell(new Phrase("Early Out", fBold8));
                        cellc5.BackgroundColor = Color.LIGHT_GRAY;
                        cellc5.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc5);

                        PdfPCell cellc6 = new PdfPCell(new Phrase("Off", fBold8));
                        cellc6.BackgroundColor = Color.LIGHT_GRAY;
                        cellc6.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc6);

                        foreach (CourseAttendanceCountLog log in sdata.counts)
                        {
                            PdfPCell cellData1 = new PdfPCell(new Phrase(log.course_code_title, fNormal8));
                            //cellData1.HorizontalAlignment = 0;
                            tableCount.AddCell(cellData1);

                            PdfPCell cellData1_1 = new PdfPCell(new Phrase(log.total_count.ToString(), fNormal8));
                            cellData1_1.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData1_1);

                            PdfPCell cellData2 = new PdfPCell(new Phrase(log.present_count.ToString(), fNormal8));
                            cellData2.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData2);

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.absent_count.ToString(), fNormal8));
                            cellData3.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.late_count.ToString(), fNormal8));
                            cellData4.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.early_count.ToString(), fNormal8));
                            cellData5.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.off_count.ToString(), fNormal8));
                            cellData6.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData6);
                        }

                        document.Add(tableCount);
                    }

                    if (sdata.logs.Length > 0)
                    {
                        //// Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        //// ---------- Last Table ---------------------
                        //PdfPTable tableEnd = new PdfPTable(new[] { 25.0f, 75.0f });
                        //tableEnd.WidthPercentage = 100;
                        //tableEnd.HeaderRows = 0;
                        //tableEnd.SpacingBefore = 3;
                        //tableEnd.SpacingAfter = 3;

                        //PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        //lt_cell_11.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_11);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        //PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Absent:", fBold9));
                        //lt_cell_21.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_21);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        ////PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Off:", fBold9));
                        ////lt_cell_31.BackgroundColor = Color.LIGHT_GRAY;
                        ////tableEnd.AddCell(lt_cell_31);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalOff, fNormal8));

                        //PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Late:", fBold9));
                        //lt_cell_41.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_41);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        ////PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Leave:", fBold9));
                        ////lt_cell_51.BackgroundColor = Color.LIGHT_GRAY;
                        ////tableEnd.AddCell(lt_cell_51);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        //PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        //lt_cell_61.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_61);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        //PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        //lt_cell_71.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_71);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        //document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, OFF-Off, PLO-Present Late, PLE-Present Late Early Out, POE-On Time Early Out, POM-On Time Miss Punch, PMO-Miss Punch & Left On Time, PLM-Late Miss Punch, *-Manually Updated", fNormal7);
                        p_abrv.SpacingBefore = 1;
                        //p_nsig.Alignment = 2;
                        document.Add(p_abrv);

                        Paragraph p_late = new Paragraph("NOTE: After 15 min of actual class time, LATE will be marked.", fNormal7);
                        p_late.SpacingBefore = 2;
                        //p_late.Alignment = 2;
                        document.Add(p_late);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=CourseAttendanceStudent-" + sdata.employeeCode + ".pdf");
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

    }
}
