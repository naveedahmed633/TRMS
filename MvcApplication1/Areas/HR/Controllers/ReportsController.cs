//new col chart
//http://localhost:57588/HR/Reports/LeavesCountReport?e_code=111222&s_year=2020

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
using System.Data;
using System.Reflection;
using OfficeOpenXml;
using System.Configuration;
using BLL_UNIS.ViewModels;
using Newtonsoft.Json;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.Helpers;
using WorkingHourTimeSheetAll.Models;
using System.Net;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using Microsoft.Data.OData;
using BLL.PdfReports;
using Newtonsoft.Json.Linq;
using iTextSharp.text.html;
using MvcApplication1.ViewModel;

//using System.Web.UI.DataVisualization.Charting;

namespace MvcApplication1.Areas.HR.Controllers
{

    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class ReportsController : Controller
    {
        #region MonthlyTimeSheetReport

        public ActionResult GenerateReport0()
        {   // @(System.Web.HttpContext.Current.Session["PDFDownloadEnabled"] ?? "inline-block" );

            //HIDE the PDF Download button
            ViewData["PDFDownloadEnabled"] = "inline-block";
            string isPDFDownloadEnabled = ConfigurationManager.AppSettings["IsPDFDownloadEnabled"] ?? "1";
            if (isPDFDownloadEnabled == "0")
                ViewData["PDFDownloadEnabled"] = "none";

            return View();
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

            ViewData["PDFNoDataFound"] = "";
            byte[] monthlyTimeSheetPdf = GenerateMonthlyTimeSheetPDF(toRender); // GenerateEvaluationPDF(toRender);

            if (monthlyTimeSheetPdf != null && monthlyTimeSheetPdf.Length > 0)
            {
                string fileName = "MonthlyTimesheet-Report-" + toRender.employeeCode + "-" + toRender.month + "-" + toRender.year + ".pdf";
                return File(monthlyTimeSheetPdf, "application/pdf", fileName);
            }

            ViewData["PDFNoDataFound"] = "No Data Found";

            return View();


            //return RedirectToAction("MonthlyTimeSheet", "Reports");

        }

        private byte[] GenerateMonthlyTimeSheetPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
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
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 60.0f, 60.0f, 60.0f, 70.0f, 70.0f, 70.0f, 70.0f });

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

                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Late And Early:", fBold9));
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Absent:", fBold9));
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        PdfPCell lt_cell_32 = new PdfPCell(new Phrase("Leave:", fBold9));
                        tableEnd.AddCell(lt_cell_32);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Half Day:", fBold9));
                        tableEnd.AddCell(lt_cell_41);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Miss Punch:", fBold9));
                        tableEnd.AddCell(lt_cell_61);
                        tableEnd.AddCell(new Phrase(" " + sdata.MissPunch, fNormal8));

                        PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        tableEnd.AddCell(lt_cell_71);
                        tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, LV-Leave, L&E-Late and Early, MissPunch, HD-HalfDay, OV-Official Visit, OT-Official Travel, OM-Official Meeting, TR-Traning, *-Manually Updated", fNormal7);
                        p_abrv.SpacingBefore = 1;
                        //p_nsig.Alignment = 2;
                        document.Add(p_abrv);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically


                        writer.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Close();
                        writer.Close();
                        return ms.ToArray();
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return null;
        }

        private int GenerateEvaluationPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
            int reponse = 0;
            int gPersonality = 4, gCommunication = 3, gAttendance = 2, gImitative = 5, gOrganization = 1, gSelf = 3;
            int sProficiency = 2, sProject = 5, sAttention = 3, sClient = 1, sCreativity = 4, sBusiness = 2;

            string strPosition = "the position text is given below", strRequirement = "", strPrimary = "", strSecondary = "secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there. secondary text is there.", strCareer = "this is career path text";

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
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]); //Server.MapPath("~/Content/Logos/logo-default.png");
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

                    ////// ---------- Header Table ---------------------
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

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Performance Evaluation", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    //////////document.Add(tableEmployee);

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
                    PdfPCell cellC01R01 = new PdfPCell(new Phrase("Employee Performance Evaluation", fBold14));
                    cellC01R01.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R01);


                    ////////////////////////

                    PdfPCell cellC01R02 = new PdfPCell(new Phrase("Employee:", fBold9));
                    cellC01R02.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02);

                    PdfPCell cellC01R02_Value = new PdfPCell(new Phrase(sdata.employeeName + "\n\n", fNormal9));
                    cellC01R02_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_Value);

                    ////////////////////////

                    PdfPCell cellC01R03 = new PdfPCell(new Phrase("Employee Code:", fBold9));
                    cellC01R03.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R03);

                    PdfPCell cellC01R03_Value = new PdfPCell(new Phrase(sdata.employeeCode + "\n\n", fNormal9));
                    cellC01R03_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R03_Value);

                    ////////////////////////
                    PdfPCell cellC01R04 = new PdfPCell(new Phrase("Review Period: ", fBold9));
                    cellC01R04.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R04);

                    PdfPCell cellC01R04_Value = new PdfPCell(new Phrase("2018 - 2019" + "\n\n", fNormal9));
                    cellC01R04_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R04_Value);

                    ////////////////////////
                    PdfPCell cellC01R05 = new PdfPCell(new Phrase("1) Evaluate performance by circling the appropriate response:\n" +
                                                                         "\t\t\t\t\t\t1 = substandard, needs constant supervision\n" +
                                                                         "\t\t\t\t\t\t2 = below average, needs improvement\n" +
                                                                         "\t\t\t\t\t\t3 = average, satisfactorily meets criteria\n" +
                                                                         "\t\t\t\t\t\t4 = above average, exceeds criteria\n" +
                                                                         "\t\t\t\t\t\t5 = exemplary, deserving of unusual recognition\n\n" +
                                                                    "\t2) Enter comments as necessary.\n\n" +
                                                                    "\t3) Set goals for the next review period\n\n" +
                                                                    "\t4) Complete the back side (supervisor only).", fNormalItalic9));
                    cellC01R05.Border = 0;
                    //cellC01R05.MinimumHeight = 60.0f;
                    tableMiddleCol01.AddCell(cellC01R05);

                    ////////////////////////
                    PdfPCell cellC01R06 = new PdfPCell(new Phrase("\nGeneral Criteria", fBold14));
                    cellC01R06.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R06);

                    ////////////////////////
                    PdfPCell cellC01R07 = new PdfPCell(new Phrase("Personality / demeanor:", fBold9));
                    cellC01R07.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07);

                    PdfPCell cellC01R07_Text = new PdfPCell(new Phrase("Flexible and easy to get along with an adaptable team player.", fNormal9));
                    cellC01R07_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_Text);

                    Phrase phrGPersonality = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gPersonality > 0 && gPersonality < 6)
                    {
                        if (gPersonality == 1)
                            phrGPersonality = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 2)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 3)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 4)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 5)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R07_Value = new PdfPCell(phrGPersonality);
                    cellC01R07_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_Value);

                    ////////////////////////
                    PdfPCell cellC01R08 = new PdfPCell(new Phrase("Communication Skills:", fBold9));
                    cellC01R08.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08);

                    PdfPCell cellC01R08_Text = new PdfPCell(new Phrase("Listens, understands and expresses him / herself well.", fNormal9));
                    cellC01R08_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08_Text);

                    Phrase phrGCommunication = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gCommunication > 0 && gCommunication < 6)
                    {
                        if (gCommunication == 1)
                            phrGCommunication = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 2)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 3)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 4)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 5)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R08_Value = new PdfPCell(phrGCommunication);
                    cellC01R08_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08_Value);

                    ////////////////////////
                    PdfPCell cellC01R09 = new PdfPCell(new Phrase("Attendance and promptness:", fBold9));
                    cellC01R09.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09);

                    PdfPCell cellC01R09_Text = new PdfPCell(new Phrase("Observes assigned working hours, is conscientious.", fNormal9));
                    cellC01R09_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09_Text);

                    Phrase phrGAttendance = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gAttendance > 0 && gAttendance < 6)
                    {
                        if (gAttendance == 1)
                            phrGAttendance = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 2)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 3)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 4)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 5)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R09_Value = new PdfPCell(phrGAttendance);
                    cellC01R09_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09_Value);

                    ////////////////////////
                    PdfPCell cellC01R10 = new PdfPCell(new Phrase("Imitative:", fBold9));
                    cellC01R10.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10);

                    PdfPCell cellC01R10_Text = new PdfPCell(new Phrase("Works without close supervision, initiates independent action.", fNormal9));
                    cellC01R10_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10_Text);

                    Phrase phrGImitative = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gImitative > 0 && gImitative < 6)
                    {
                        if (gImitative == 1)
                            phrGImitative = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 2)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 3)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 4)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 5)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R10_Value = new PdfPCell(phrGImitative);
                    cellC01R10_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10_Value);

                    ////////////////////////
                    PdfPCell cellC01R11 = new PdfPCell(new Phrase("Organization and time-awareness:", fBold9));
                    cellC01R11.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11);

                    PdfPCell cellC01R11_Text = new PdfPCell(new Phrase("Sets and observes own priorities for the best use of his/her time.", fNormal9));
                    cellC01R11_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_Text);

                    Phrase phrGOrganization = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gOrganization > 0 && gOrganization < 6)
                    {
                        if (gOrganization == 1)
                            phrGOrganization = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 2)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 3)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 4)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 5)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R11_Value = new PdfPCell(phrGOrganization);
                    cellC01R11_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_Value);

                    ////////////////////////
                    PdfPCell cellC01R12 = new PdfPCell(new Phrase("Self-Control:", fBold9));
                    cellC01R12.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R12);

                    PdfPCell cellC01R12_Text = new PdfPCell(new Phrase("Maintains composure and performs well under pressure.", fNormal9));
                    cellC01R12_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R12_Text);

                    Phrase phrGSelf = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gSelf > 0 && gSelf < 6)
                    {
                        if (gSelf == 1)
                            phrGSelf = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 2)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 3)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 4)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 5)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R12_Value = new PdfPCell(phrGSelf);
                    cellC01R12_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R12_Value);

                    ////////////////////////
                    PdfPCell cellC01R13 = new PdfPCell(new Phrase("\nComments", fBold14));
                    cellC01R13.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R13);

                    ////////////////////////
                    PdfPCell cellC01R14 = new PdfPCell(new Phrase("Employee’s major strength:", fBold9));
                    cellC01R14.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14);

                    PdfPCell cellC01R14_Value = new PdfPCell(new Phrase(sdata.employeeName + "\n\n\n", fNormal9));
                    cellC01R14_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14_Value);

                    ////////////////////////
                    PdfPCell cellC01R15 = new PdfPCell(new Phrase("Area needing most improvement:", fBold9));
                    cellC01R15.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15);

                    PdfPCell cellC01R15_Value = new PdfPCell(new Phrase(sdata.employeeName + "\n\n\n", fNormal9));
                    cellC01R15_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15_Value);

                    ////////////////////////
                    PdfPCell cellC01R16 = new PdfPCell(new Phrase("Other Comments:", fBold9));
                    cellC01R16.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16);

                    PdfPCell cellC01R16_Value = new PdfPCell(new Phrase(sdata.employeeName + "\n\n\n", fNormal9));
                    cellC01R16_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_Value);




                    // --------------------------------------------- Middle Table - Column 2 ---------------------------------------------------------
                    PdfPTable tableMiddleCol02 = new PdfPTable(1);
                    tableMiddleCol02.WidthPercentage = 100;
                    tableMiddleCol02.HeaderRows = 0;
                    //tableMiddleCol02.SpacingBefore = 50;
                    tableMiddleCol02.SpacingAfter = 3;
                    tableMiddleCol02.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellC02R01 = new PdfPCell(new Phrase("Position:", fBold9));
                    cellC02R01.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R01);

                    Phrase phrPosition = null;
                    if (strPosition.Length > 0)
                    {
                        phrPosition = new Phrase(strPosition, fNormal9);
                    }
                    else
                    {
                        phrPosition = new Phrase("_______________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R01_Value = new PdfPCell(phrPosition);
                    cellC02R01_Value.Border = 0;
                    cellC02R01_Value.MinimumHeight = 30.0f;
                    tableMiddleCol02.AddCell(cellC02R01_Value);

                    ////////////////////////
                    PdfPCell cellC02R02 = new PdfPCell(new Phrase("Requirements/attribute:", fBold9));
                    cellC02R02.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R02);

                    Phrase phrRequirement = null;
                    if (strRequirement.Length > 0)
                    {
                        phrRequirement = new Phrase(strRequirement, fNormal9);
                    }
                    else
                    {
                        phrRequirement = new Phrase("_______________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R02_Value = new PdfPCell(phrRequirement);
                    cellC02R02_Value.Border = 0;
                    cellC02R02_Value.MinimumHeight = 30.0f;
                    tableMiddleCol02.AddCell(cellC02R02_Value);

                    ////////////////////////
                    PdfPCell cellC02R03 = new PdfPCell(new Phrase("Primary resposibilities:", fBold9));
                    cellC02R03.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R03);

                    Phrase phrPrimary = null;
                    if (strPrimary.Length > 0)
                    {
                        phrPrimary = new Phrase(strPrimary, fNormal9);
                    }
                    else
                    {
                        phrPrimary = new Phrase("__________________________________________________________________________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R03_Value = new PdfPCell(phrPrimary);
                    cellC02R03_Value.Border = 0;
                    cellC02R03_Value.MinimumHeight = 40.0f;
                    tableMiddleCol02.AddCell(cellC02R03_Value);

                    ////////////////////////
                    PdfPCell cellC02R04 = new PdfPCell(new Phrase("Secondary resposibilities:", fBold9));
                    cellC02R04.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R04);

                    Phrase phrSecondary = null;
                    if (strSecondary.Length > 0)
                    {
                        phrSecondary = new Phrase(strSecondary, fNormal9);
                    }
                    else
                    {
                        phrSecondary = new Phrase("__________________________________________________________________________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R04_Value = new PdfPCell(phrSecondary);
                    cellC02R04_Value.Border = 0;
                    cellC02R04_Value.MinimumHeight = 40.0f;
                    tableMiddleCol02.AddCell(cellC02R04_Value);

                    ////////////////////////
                    PdfPCell cellC02R05 = new PdfPCell(new Phrase("Career path:", fBold9));
                    cellC02R05.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R05);

                    Phrase phrCareer = null;
                    if (strCareer.Length > 0)
                    {
                        phrCareer = new Phrase(strCareer, fNormal9);
                    }
                    else
                    {
                        phrCareer = new Phrase("_______________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R05_Value = new PdfPCell(phrCareer);
                    cellC02R05_Value.Border = 0;
                    cellC02R05_Value.MinimumHeight = 30.0f;
                    tableMiddleCol02.AddCell(cellC02R05_Value);

                    ////////////////////////
                    PdfPCell cellC02R06 = new PdfPCell(new Phrase("\nPosition-Specific Criteria", fBold14));
                    cellC02R06.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R06);

                    ////////////////////////
                    PdfPCell cellC02R07 = new PdfPCell(new Phrase("Proficiency:", fBold9));
                    cellC02R07.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07);

                    PdfPCell cellC02R07_Text = new PdfPCell(new Phrase("Understands craft, systems and processes.", fNormal9));
                    cellC02R07_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07_Text);

                    Phrase phrSProficiency = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sProficiency > 0 && sProficiency < 6)
                    {
                        if (sProficiency == 1)
                            phrSProficiency = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 2)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 3)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 4)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 5)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R07_Value = new PdfPCell(phrSProficiency);
                    cellC02R07_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07_Value);

                    ////////////////////////
                    PdfPCell cellC02R08 = new PdfPCell(new Phrase("Project Management:", fBold9));
                    cellC02R08.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R08);

                    PdfPCell cellC02R08_Text = new PdfPCell(new Phrase("Organizes tasks and assignments.", fNormal9));
                    cellC02R08_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC01R08_Text);

                    Phrase phrSProject = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sProject > 0 && sProject < 6)
                    {
                        if (sProject == 1)
                            phrSProject = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 2)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 3)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 4)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 5)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R08_Value = new PdfPCell(phrSProject);
                    cellC02R08_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R08_Value);

                    ////////////////////////
                    PdfPCell cellC02R09 = new PdfPCell(new Phrase("Attention to detail:", fBold9));
                    cellC02R09.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09);

                    PdfPCell cellC02R09_Text = new PdfPCell(new Phrase("Attentive to all aspects of assignments / workflow.", fNormal9));
                    cellC02R09_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09_Text);

                    Phrase phrSAttention = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sAttention > 0 && sAttention < 6)
                    {
                        if (sAttention == 1)
                            phrSAttention = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 2)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 3)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 4)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 5)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R09_Value = new PdfPCell(phrSAttention);
                    cellC02R09_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09_Value);

                    ////////////////////////
                    PdfPCell cellC02R10 = new PdfPCell(new Phrase("Client Interaction:", fBold9));
                    cellC02R10.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10);

                    PdfPCell cellC02R10_Text = new PdfPCell(new Phrase("Relates to client needs, both spoken and unspoken.", fNormal9));
                    cellC02R10_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10_Text);

                    Phrase phrSClient = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sClient > 0 && sClient < 6)
                    {
                        if (sClient == 1)
                            phrSClient = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 2)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 3)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 4)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 5)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R10_Value = new PdfPCell(phrSClient);
                    cellC02R10_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10_Value);

                    ////////////////////////
                    PdfPCell cellC02R11 = new PdfPCell(new Phrase("Creativity:", fBold9));
                    cellC02R11.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11);

                    PdfPCell cellC02R11_Text = new PdfPCell(new Phrase("Seeks innovative solutions.", fNormal9));
                    cellC02R11_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11_Text);

                    Phrase phrSCreativity = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sCreativity > 0 && sCreativity < 6)
                    {
                        if (sCreativity == 1)
                            phrSCreativity = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 2)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 3)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 4)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 5)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R11_Value = new PdfPCell(phrSCreativity);
                    cellC02R11_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11_Value);

                    ////////////////////////
                    PdfPCell cellC02R12 = new PdfPCell(new Phrase("Business skills:", fBold9));
                    cellC02R12.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12);

                    PdfPCell cellC02R12_Text = new PdfPCell(new Phrase("Understands and works to increase profitability.", fNormal9));
                    cellC02R12_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_Text);

                    Phrase phrSBusiness = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sBusiness > 0 && sBusiness < 6)
                    {
                        if (sBusiness == 1)
                            phrSBusiness = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 2)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 3)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 4)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 5)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R12_Value = new PdfPCell(phrSBusiness);
                    cellC02R12_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_Value);

                    ////////////////////////
                    PdfPCell cellC02R13 = new PdfPCell(new Phrase("\nGoals", fBold14));
                    cellC02R13.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R13);

                    ////////////////////////
                    PdfPCell cellC02R14 = new PdfPCell(new Phrase("Detail:", fBold9));
                    cellC02R14.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14);

                    PdfPCell cellC02R14_Value = new PdfPCell(new Phrase(sdata.employeeName + "\n\n\n", fNormal9));
                    cellC02R14_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_Value);

                    ////////////////////////
                    PdfPCell cellC02R15 = new PdfPCell(new Phrase("I have been shown this evaluation. My signature below does not necessarily imply agreement:\n\n", fBold9));
                    cellC02R15.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R15);

                    PdfPCell cellC02R15_Value = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(Employee's Signature / date)", fNormalItalic9));
                    cellC02R15_Value.Border = 0;
                    cellC02R15_Value.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R15_Value);

                    ////////////////////////
                    PdfPCell cellC02R16 = new PdfPCell(new Phrase("\n\nScheduled date of next evaluation:\n\n", fBold9));
                    cellC02R16.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R16);

                    PdfPCell cellC02R16_Value = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(Supervisor's Signature / date)", fNormalItalic9));
                    cellC02R16_Value.Border = 0;
                    cellC02R16_Value.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R16_Value);

                    ////////////////////////////////////////////////////////////////////////////////////////

                    //add 2 tables to MAIN table
                    tableMiddle.AddCell(tableMiddleCol01);
                    tableMiddle.AddCell(tableMiddleCol02);

                    document.Add(tableMiddle);

                    ///////////////////////////////////////////////////////////////////////////////////////

                    // ---------- End Table ---------------------
                    Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                    p_nsig.SpacingBefore = 1;
                    //p_nsig.SpacingAfter = 3;
                    //////document.Add(p_nsig);

                    // ------------- close PDF Document and download it automatically



                    document.Close();
                    writer.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=Evaluation-Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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

        #endregion

        #region Bulk - MonthlyDepartmentalTimeSheetReport

        //[HttpPost]
        //public JsonResult MonthlyDepartmentalTimesheetReportDataHandler11(ConsolidatedDepartmentReportTable param)
        //{
        //    int count = 0;



        //    try
        //    {
        //        //var data = new List<MonthlyTimesheetAttendanceLog>();

        //        if (param.month == null)
        //        {
        //            param.month = DateTime.Now.AddMonths(2).ToString("yyyy-MM");
        //        }
        //            //count = TimeTune.Reports.getMonthlyDepartmentalTimesheetReport(depart_id,fun_id,reg_id,des_id,loc_id, param.month, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

        //        var data = new List<ConsolidatedAttendanceDepartmentWise>();

        //        var dd = TimeTune.Reports.getAllDepartmentLogsDepartmentWiseAbbas(param.department_id, param.designation_id, param.function_id, param.region_id, param.location_id, param.month, param.Search.Value, "date", param.Start, param.Length, out data);


        //        DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise> result = new DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise>
        //        {
        //            draw = param.Draw,
        //            data = data,
        //            recordsFiltered = count,
        //            recordsTotal = count
        //        };

        //        //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
        //        //{
        //        //    draw = param.Draw,
        //        //    data = data,
        //        //    recordsFiltered = count,
        //        //    recordsTotal = count
        //        //};

        //        return Json(result);
        //    }

        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine(ex.Message);
        //        return Json(new { error = ex.Message });

        //    }
        //}


        [HttpPost]
        public JsonResult MonthlyDepartmentalTimesheetReportDataHandler(ConsolidatedDepartmentReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceDepartmentWise>();

                int count = TimeTune.Reports.getAllDepartmentLogsDepartmentWisePageByPage(param.department_id, param.designation_id, param.function_id, param.region_id, param.location_id, param.from_date, param.Search.Value, "employee_code", param.Start, param.Length, out data);

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


        [HttpGet]
        [ActionName("MonthlyDepartmentalTimeSheet")]
        public ActionResult MonthlyDepartmentalTimeSheet_Get()
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
        [ActionName("MonthlyDepartmentalTimeSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyDepartmentalTimeSheet_Post()
        {
            int departmentID; int functionID; int regionID; int designationID; int locationID;

            if (!int.TryParse(Request.Form["function_id"], out functionID))
                functionID = -1;
            if (!int.TryParse(Request.Form["region_id"], out regionID))
                regionID = -1;
            if (!int.TryParse(Request.Form["department_id"], out departmentID))
                departmentID = -1;
            if (!int.TryParse(Request.Form["designation_id"], out designationID))
                designationID = -1;
            if (!int.TryParse(Request.Form["location_id"], out locationID))
                locationID = -1;


            //departmentID =  Request.Form["department_id"].ToString();
            //functionID =    Request.Form["function_id"].ToString();
            //regionID =      Request.Form["region_id"].ToString();
            //designationID = Request.Form["designation_id"].ToString();
            //locationID =    Request.Form["location_id"].ToString();

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            List<BLL.PdfReports.MonthlyDepartmentalTimeSheetData> toRender = reportMaker.getMonthlyDepartmentalReport(departmentID, functionID, regionID, locationID, designationID, month);

            int found = 0;

            if (toRender == null)
                return RedirectToAction("MonthlyDepartmentalTimeSheet");

            if (toRender[0].logs.Length > 0)
            {
                found = 1;
            }


            if (found == 1)
            {
                found = 0; ViewData["PDFNoDataFound"] = "";
                found = GenerateMonthlyDepartmentalTimeSheetPDF(toRender); // GenerateEvaluationPDF(toRender);
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

        private int GenerateMonthlyDepartmentalTimeSheetPDF(List<BLL.PdfReports.MonthlyDepartmentalTimeSheetData> sdata)
        {
            int counter = 1;
            int reponse = 0;
            string monthYear = "";

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
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    foreach (var emp in sdata)
                    {
                        monthYear = emp.month + "-" + emp.year;

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

                        PdfPCell cellEName = new PdfPCell(new Phrase("Name: " + emp.employeeName, fBold9));
                        cellEName.Border = 0;
                        tableEInfo.AddCell(cellEName);

                        PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + emp.employeeCode, fBold9));
                        cellECode.Border = 0;
                        tableEInfo.AddCell(cellECode);

                        PdfPCell cellEMonth = new PdfPCell(new Phrase("Month Year: " + emp.month + " " + emp.year, fBold9));
                        cellEMonth.Border = 0;
                        tableEInfo.AddCell(cellEMonth);

                        //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + emp.year, fBold9));
                        //cellEYear.Border = 0;
                        //tableEInfo.AddCell(cellEYear);

                        PdfPCell cellDeptName = new PdfPCell(new Phrase("Department Name: " + emp.departmentName, fBold9));
                        cellDeptName.Border = 0;
                        tableEInfo.AddCell(cellDeptName);

                        //PdfPCell cellFunName = new PdfPCell(new Phrase("Function Name: " + emp.functionName, fBold9));
                        //cellFunName.Border = 0;
                        //tableEInfo.AddCell(cellFunName);

                        PdfPCell cellDesigName = new PdfPCell(new Phrase("Designation Name: " + emp.designationName, fBold9));
                        cellDesigName.Border = 0;
                        tableEInfo.AddCell(cellDesigName);

                        //PdfPCell cellRegionName = new PdfPCell(new Phrase("Group Name: " + emp.regionName, fBold9));
                        //cellRegionName.Border = 0;
                        //tableEInfo.AddCell(cellRegionName);

                        //PdfPCell cellLocName = new PdfPCell(new Phrase("Location Name: " + emp.locationName, fBold9));
                        //cellLocName.Border = 0;
                        //tableEInfo.AddCell(cellLocName);

                        //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                        //p_title.SpacingBefore = 50f;
                        //p_title.SpacingAfter = 10f;
                        ////document.Add(p_title);

                        PdfPCell cellETitle = new PdfPCell(new Phrase("Bulk Timesheets: User " + counter + " of " + sdata.Count, fBold16));
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

                        foreach (BLL.PdfReports.MonthlyTimeSheetLog log in emp.logs)
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
                        tableEnd.AddCell(new Phrase(" " + emp.totalPresent, fNormal8));

                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Late:", fBold9));
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + emp.totalLate, fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Absent:", fBold9));
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + emp.totalAbsent, fNormal8));

                        PdfPCell lt_cell_32 = new PdfPCell(new Phrase("Leave:", fBold9));
                        tableEnd.AddCell(lt_cell_32);
                        tableEnd.AddCell(new Phrase(" " + emp.totalLeave, fNormal8));

                        PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        tableEnd.AddCell(lt_cell_41);
                        tableEnd.AddCell(new Phrase(" " + emp.totalEarlyOut, fNormal8));

                        PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        tableEnd.AddCell(lt_cell_51);
                        tableEnd.AddCell(new Phrase(" " + emp.totalDays, fNormal8));

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

                        document.NewPage();
                        counter++;
                    }

                    document.Close();
                    writer.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=Bulk-Report-" + monthYear + " .pdf");
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

            ViewData["PDFNoDataFound"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("MonthlyTimeSheetOvertime")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheetOvertime_Post()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyTimeSheetOvertime");

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
                        Response.AddHeader("content-disposition", "attachment;filename=MonthlyOvertime-Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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

                    PdfPCell cellEMonth = new PdfPCell(new Phrase("Month Year: " + sdata.month + " " + sdata.year, fBold9));
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);

                    //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.year, fBold9));
                    //cellEYear.Border = 0;
                    //tableEInfo.AddCell(cellEYear);

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
                        Response.AddHeader("content-disposition", "attachment;filename=MonthlyOvertime-Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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

        #region PayrollReportByMonth

        [HttpGet]
        [ActionName("PayrollReportByMonth")]
        public ActionResult PayrollReportByMonth_Get()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PayrollReportByMonthDataHandler(PayrollReportByMonthTable param)
        {
            try
            {
                var dataPayroll = new List<PayrollReportByMonthLog>();

                // get all employee view models
                int countPayroll = TimeTune.Reports.getPayrollReportByMonth(param.salary_month_year, param.Search.Value, param.SortOrder, param.Start, param.Length, out dataPayroll);

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
        public JsonResult PayrollReportByMonthDepartmentDataHandler(PayrollReportByMonthDepartmentTable param)
        {
            try
            {
                var dataPayroll = new List<PayrollReportByMonthLog>();

                // get all employee view models
                int countPayroll = TimeTune.Reports.getPayrollReportByMonthDepartment(param.month_year, param.department_id, param.Search.Value, param.SortOrder, param.Start, param.Length, out dataPayroll);

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

        ///////////////////////////////////////////////////////////////

        [HttpGet]
        [ActionName("PayrollGenerate")]
        public ActionResult PayrollGenerate_Get()
        {
            int response = 0;

            try
            {
                var data_dep = PayrollResultSet.getAllDepartments();

                if (data_dep != null && data_dep.Count > 0)
                {
                    ViewBag.DepartmentsList = data_dep;
                }
                else
                {
                    ViewBag.DepartmentsList = null;
                }

                //////////////////////////////////////////////////////////////////////

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
        [ActionName("PayrollGenerate")]
        public ActionResult PayrollGenerate_Post(string param)
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

            //////////////////////////////////////////////////

            var data_dep = PayrollResultSet.getAllDepartments();

            if (data_dep != null && data_dep.Count > 0)
            {
                ViewBag.DepartmentsList = data_dep;
            }
            else
            {
                ViewBag.DepartmentsList = null;
            }

            //////////////////////////////////////////////////

            return View();
        }


        [HttpPost]
        [ActionName("PayrollStatusUpdate")]
        public ActionResult PayrollStatusUpdate(string smonth_year, int sdepartment_id, string spayment_date, int spayment_status_id)
        {
            int response = 0;

            try
            {
                response = PayrollResultSet.UpdatePayrollStatus(smonth_year, sdepartment_id, spayment_date, spayment_status_id);
                if (response == 1)
                {
                    ViewBag.PayrollUpdateStatus = "Updated Successfully!";
                }
                else if (response == 0)
                {
                    ViewBag.PayrollUpdateStatus = "No Record Found to Update";
                }
                else
                {
                    ViewBag.PayrollUpdateStatus = "An Error Occurred";
                }
            }
            catch (Exception ex)
            {
                response = -1;
            }

            //////////////////////////////////////////////////

            var data_dep = PayrollResultSet.getAllDepartments();

            if (data_dep != null && data_dep.Count > 0)
            {
                ViewBag.DepartmentsList = data_dep;
            }
            else
            {
                ViewBag.DepartmentsList = null;
            }

            //////////////////////////////////////////////////

            return View("PayrollGenerate");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////

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
                    Response.AddHeader("content-disposition", "attachment;filename=Payroll-Report-" + pInfo.EmployeeCode + "-" + pInfo.SalaryMonthYearText + ".pdf");
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

        ///////////////////////////////////////////////////////////////////////////////////////////


        [HttpPost]
        public ActionResult GeneratePayrollStatement()
        {
            string month_year = "";
            if (Request.Form["month_year"] != null && Request.Form["month_year"].ToString() != "")
            {
                month_year = Request.Form["month_year"];
            }

            PayrollStatementForPDF pInfo = PayrollResultSet.GeneratePayrollStatementByMonth(month_year);

            if (pInfo == null)
                return RedirectToAction("PayrollReportByMonth");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0;
            ViewData["PayrollPDFNoDataFound"] = "";
            found = DownloadPayrollStatmentPDF(pInfo);

            if (found == 1)
            {
                ViewData["PayrollPDFNoDataFound"] = ".";
                //return null;
            }
            else
            {
                ViewData["PayrollPDFNoDataFound"] = "No Data Found";
            }

            return View("PayrollReportByMonth");
            //return RedirectPermanent("/HR/Reports/PayrollReportByMonth?n=1");
        }



        private int DownloadPayrollStatmentPDF(PayrollStatementForPDF pStat)
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

                    PdfPCell cellEName = new PdfPCell(new Phrase("Month Year: " + pStat.MonthYear, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECount = new PdfPCell(new Phrase("Total Employees: " + pStat.EmployeesCount, fBold9));
                    cellECount.Border = 0;
                    tableEInfo.AddCell(cellECount);

                    PdfPCell cellITax = new PdfPCell(new Phrase("Income Tax Amount: " + pStat.IncomeTax.ToString("C").Replace("$", "Rs."), fBold9));
                    cellITax.Border = 0;
                    tableEInfo.AddCell(cellITax);

                    PdfPCell cellNAmount = new PdfPCell(new Phrase("Salary Amount: " + pStat.NetAmount.ToString("C").Replace("$", "Rs."), fBold9));
                    cellNAmount.Border = 0;
                    tableEInfo.AddCell(cellNAmount);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Payroll Statement - " + pStat.MonthYear, fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 60.0f, 95.0f, 95.0f, 95.0f, 60.0f, 60.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Employee Code", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Bank Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Account #", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Account Title", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Income Tax (Rs.)", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Net Salary (Rs.)", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    foreach (var log in pStat.PayrollInfoList)
                    {
                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.EmployeeCode, fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.PayrollInformation.BankNameText, fNormal8));
                        //cellData2.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.PayrollInformation.BankAccNo, fNormal8));
                        //cellData3.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.PayrollInformation.BankAccTitle, fNormal8));
                        //cellData4.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.PayrollInformation.IncomeTax.ToString("C").Replace("$", ""), fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.PayrollInformation.NetSalary.ToString("C").Replace("$", ""), fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);
                    }

                    if (pStat.PayrollInfoList.Count > 0)
                    {
                        document.Add(tableMid);

                        // ---------- End Table ---------------------
                        Paragraph p_nsig = new Paragraph("This is a system generated payroll-slip and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Payroll-Statment-" + pStat.MonthYear + ".pdf");
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

                    ///////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////


        #endregion

        #region LoanReports

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
                var dataPayroll = new List<LoanReportByMonthLog>();

                // get all employee view models
                int countPayroll = TimeTune.Reports.getLoanReportByMonth(param.month_year, param.Search.Value, param.SortOrder, param.Start, param.Length, out dataPayroll);

                List<ViewModels.LoanReportByMonthLog> data = LoanResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dataPayroll);
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

        ///////////////////////////////////////////////////////////////


        [HttpGet]
        [ActionName("LoanReportByEmployee")]
        public ActionResult LoanReportByEmployee_Get()
        {
            var data_emp = LoanResultSet.getAllEmployees();

            if (data_emp != null && data_emp.Count > 0)
            {
                ViewBag.EmployeesList = data_emp;
            }
            else
            {
                ViewBag.EmployeesList = null;
            }

            return View();
        }

        [HttpPost]
        public JsonResult LoanReportByEmployeeDataHandler(LoanReportByEmployeeTable param)
        {
            try
            {
                var dataPayroll = new List<LoanReportByMonthLog>();

                // get all employee view models
                int countPayroll = TimeTune.Reports.getLoanReportByEmployee(param.employee_id, param.Search.Value, param.SortOrder, param.Start, param.Length, out dataPayroll);

                List<ViewModels.LoanReportByMonthLog> data = LoanResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dataPayroll);
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

        ///////////////////////////////////////////////////////////////


        [HttpGet]
        [ActionName("LoanReportByDate")]
        public ActionResult LoanReportByDate_Get()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoanReportByDateDataHandler(LoanReportByDateTable param)
        {
            try
            {
                var dataPayroll = new List<LoanReportByMonthLog>();

                // get all employee view models
                int countPayroll = TimeTune.Reports.getLoanReportByDate(param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out dataPayroll);

                List<ViewModels.LoanReportByMonthLog> data = LoanResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dataPayroll);
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

        /////////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult GenerateLoanStatement()
        {
            string month_year = "";
            if (Request.Form["lmonth_year"] != null && Request.Form["lmonth_year"].ToString() != "")
            {
                month_year = Request.Form["lmonth_year"];
            }

            LoanStatementForPDF lInfo = LoanResultSet.getLoanStatementByMonth(month_year);

            if (lInfo == null)
                return RedirectToAction("LoanReportByMonth");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0;
            ViewData["LoanPDFNoDataFound"] = "";
            found = DownloadLoanStatmentPDF(lInfo);

            if (found == 1)
            {
                ViewData["LoanPDFNoDataFound"] = ".";
                //return null;
            }
            else
            {
                ViewData["LoanPDFNoDataFound"] = "No Data Found";
            }

            return View("LoanReportByMonth");
            //return RedirectPermanent("/HR/Reports/PayrollReportByMonth?n=1");
        }

        private int DownloadLoanStatmentPDF(LoanStatementForPDF pStat)
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

                    PdfPCell cellEName = new PdfPCell(new Phrase("Month Year: " + pStat.MonthYear, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECount = new PdfPCell(new Phrase("Amount Received: " + pStat.DeductableAmount.ToString("C").Replace("$", "Rs."), fBold9));
                    cellECount.Border = 0;
                    tableEInfo.AddCell(cellECount);

                    //PdfPCell cellITax = new PdfPCell(new Phrase("Income Tax Amount: " + pStat.IncomeTax.ToString("C").Replace("$", "Rs."), fBold9));
                    //cellITax.Border = 0;
                    //tableEInfo.AddCell(cellITax);

                    //PdfPCell cellNAmount = new PdfPCell(new Phrase("Salary Amount: " + pStat.NetAmount.ToString("C").Replace("$", "Rs."), fBold9));
                    //cellNAmount.Border = 0;
                    //tableEInfo.AddCell(cellNAmount);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Loan Statement - " + pStat.MonthYear, fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 60.0f, 95.0f, 95.0f, 95.0f, 60.0f, 60.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Employee Code", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Employee Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Loan Amount (Rs.)", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Installment Amount (Rs.)", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Deducted Amount (Rs.)", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Balance (Rs.)", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    foreach (var log in pStat.LoanInfoList)
                    {
                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.EmployeeCode, fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.EmployeeName, fNormal8));
                        //cellData2.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.LoanAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.InstallmentAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.DeductableAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.BalanceAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);
                    }

                    if (pStat != null)
                    {
                        document.Add(tableMid);

                        // ---------- End Table ---------------------
                        Paragraph p_nsig = new Paragraph("This is a system generated loan statment and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Loan-Statment-" + pStat.MonthYear + ".pdf");
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

                    ///////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }



        [HttpPost]
        public ActionResult GenerateLoanStatementForEmployee()
        {
            int employee_id = 0;
            if (Request.Form["lemployee_id"] != null && Request.Form["lemployee_id"].ToString() != "")
            {
                employee_id = int.Parse(Request.Form["lemployee_id"]);
            }

            LoanStatementForEmployeePDF lInfo = LoanResultSet.getLoanStatementByEmployee(employee_id);

            if (lInfo == null)
                return RedirectToAction("LoanReportByEmployee");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0;
            ViewData["LoanPDFNoDataFound"] = "";
            found = DownloadLoanStatmentForEmployeePDF(lInfo);

            if (found == 1)
            {
                ViewData["LoanPDFNoDataFound"] = ".";
                //return null;
            }
            else
            {
                ViewData["LoanPDFNoDataFound"] = "No Data Found";
            }

            return View("LoanReportByEmployee");
            //return RedirectPermanent("/HR/Reports/PayrollReportByMonth?n=1");
        }



        private int DownloadLoanStatmentForEmployeePDF(LoanStatementForEmployeePDF pStat)
        {
            int reponse = 0, c = 0;
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

                    PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + pStat.EmployeeCode, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellEName = new PdfPCell(new Phrase("Employee Name: " + pStat.EmployeeName, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    //////////////////////////////////////////////////////////

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Loan Statement - ECode: " + pStat.EmployeeCode, fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //-------------------------------------------------------------------------------

                    PdfPTable tableAmounts = new PdfPTable(3);
                    tableAmounts.WidthPercentage = 100;
                    tableAmounts.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableAmounts.SpacingAfter = 3;
                    tableAmounts.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellOpenLAmount = new PdfPCell(new Phrase("Open - Loan Amount: " + pStat.LoanAmountOpen.ToString("C").Replace("$", "Rs."), fBold9));
                    cellOpenLAmount.Border = 0;
                    tableAmounts.AddCell(cellOpenLAmount);

                    PdfPCell cellClosedLAmount = new PdfPCell(new Phrase("Closed - Loan Amount: " + pStat.LoanAmountClosed.ToString("C").Replace("$", "Rs."), fNormal9));
                    cellClosedLAmount.Border = 0;
                    tableAmounts.AddCell(cellClosedLAmount);

                    PdfPCell cellTotalLAmount = new PdfPCell(new Phrase("Total - Loan Amount: " + (pStat.LoanAmountOpen + pStat.LoanAmountClosed).ToString("C").Replace("$", "Rs."), fBold9));
                    cellTotalLAmount.Border = 0;
                    tableAmounts.AddCell(cellTotalLAmount);


                    PdfPCell cellOpenDeduct = new PdfPCell(new Phrase("Open - Deducted Amount: " + pStat.DeductableAmountOpen.ToString("C").Replace("$", "Rs."), fBold9));
                    cellOpenDeduct.Border = 0;
                    tableAmounts.AddCell(cellOpenDeduct);

                    PdfPCell cellClosedDeduct = new PdfPCell(new Phrase("Closed - Deducted Amount: " + pStat.DeductableAmountClosed.ToString("C").Replace("$", "Rs."), fNormal9));
                    cellClosedDeduct.Border = 0;
                    tableAmounts.AddCell(cellClosedDeduct);

                    PdfPCell cellBlank = new PdfPCell(new Phrase("", fNormal9));
                    cellBlank.Border = 0;
                    tableAmounts.AddCell(cellBlank);


                    PdfPCell cellOpenBalance = new PdfPCell(new Phrase("Open - Balance Amount: " + pStat.BalanceAmountOpen.ToString("C").Replace("$", "Rs."), fBold9));
                    cellOpenBalance.Border = 0;
                    tableAmounts.AddCell(cellOpenBalance);

                    PdfPCell cellClosedBalance = new PdfPCell(new Phrase("Closed - Balance Amount: " + pStat.BalanceAmountClosed.ToString("C").Replace("$", "Rs."), fNormal9));
                    cellClosedBalance.Border = 0;
                    tableAmounts.AddCell(cellClosedBalance);

                    PdfPCell cellTotalBalance = new PdfPCell(new Phrase("Total - Balance Amount: " + (pStat.BalanceAmountOpen + pStat.BalanceAmountClosed).ToString("C").Replace("$", "Rs."), fBold9));
                    cellTotalBalance.Border = 0;
                    tableAmounts.AddCell(cellTotalBalance);

                    //-------------------------------------------------------------------------------

                    document.Add(tableAmounts);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 30.0f, 80.0f, 50.0f, 60.0f, 70.0f, 70.0f, 50.0f, 40.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Serial #", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Loan Code", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Loan Amount (Rs.)", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Inst. Count", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Fixed Amount per Month (Rs.)", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Deduction of the Month (Rs.)", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Balance (Rs.)", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Status", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    foreach (var log in pStat.LoanInfoList)
                    {
                        c++;

                        PdfPCell cellData1 = new PdfPCell(new Phrase(c.ToString(), fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        //actually loan code
                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.Remarks, fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.LoanAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData3.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData3);

                        //actually counter for paid and remaining counts
                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.AttachmentFilePath, fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.InstallmentAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData5.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.DeductableAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData6.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.BalanceAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData7.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.LoanStatusText, fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);
                    }

                    if (pStat != null)
                    {
                        document.Add(tableMid);

                        // ---------- End Table ---------------------
                        Paragraph p_nsig = new Paragraph("This is a system generated loan statment and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Loan-Statment-" + pStat.EmployeeCode + ".pdf");
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

                    ///////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        /////////////////////////////////////////////////////////////////


        public ActionResult GenerateLoanStatementForDate()
        {
            string from_date = "", to_date = "";

            if (Request.Form["lfrom_date"] != null && Request.Form["lfrom_date"].ToString() != "")
            {
                from_date = Request.Form["lfrom_date"];
            }

            if (Request.Form["lto_date"] != null && Request.Form["lto_date"].ToString() != "")
            {
                to_date = Request.Form["lto_date"];
            }

            LoanStatementForEmployeePDF lInfo = LoanResultSet.getLoanStatementByDate(from_date, to_date);

            if (lInfo == null)
                return RedirectToAction("LoanReportByDate");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            DateTime dtFromDate = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dtToDate = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            int found = 0;
            ViewData["LoanPDFNoDataFound"] = "";
            found = DownloadLoanStatmentForDatePDF(dtFromDate.ToString("dd-MMM-yyyy"), dtToDate.ToString("dd-MMM-yyyy"), lInfo);

            if (found == 1)
            {
                ViewData["LoanPDFNoDataFound"] = ".";
                //return null;
            }
            else
            {
                ViewData["LoanPDFNoDataFound"] = "No Data Found";
            }

            return View("LoanReportByDate");
            //return RedirectPermanent("/HR/Reports/PayrollReportByMonth?n=1");
        }



        private int DownloadLoanStatmentForDatePDF(string strFromDate, string strToDate, LoanStatementForEmployeePDF pStat)
        {
            int reponse = 0, c = 0;
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

                    PdfPCell cellECode = new PdfPCell(new Phrase("From Date: " + strFromDate, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellEName = new PdfPCell(new Phrase("To Date: " + strToDate, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    //////////////////////////////////////////////////////////

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Loan Statement", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //-------------------------------------------------------------------------------

                    PdfPTable tableAmounts = new PdfPTable(3);
                    tableAmounts.WidthPercentage = 100;
                    tableAmounts.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableAmounts.SpacingAfter = 3;
                    tableAmounts.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellOpenLAmount = new PdfPCell(new Phrase("Open - Loan Amount: " + pStat.LoanAmountOpen.ToString("C").Replace("$", "Rs."), fBold9));
                    cellOpenLAmount.Border = 0;
                    tableAmounts.AddCell(cellOpenLAmount);

                    PdfPCell cellClosedLAmount = new PdfPCell(new Phrase("Closed - Loan Amount: " + pStat.LoanAmountClosed.ToString("C").Replace("$", "Rs."), fNormal9));
                    cellClosedLAmount.Border = 0;
                    tableAmounts.AddCell(cellClosedLAmount);

                    PdfPCell cellTotalLAmount = new PdfPCell(new Phrase("Total - Loan Amount: " + (pStat.LoanAmountOpen + pStat.LoanAmountClosed).ToString("C").Replace("$", "Rs."), fBold9));
                    cellTotalLAmount.Border = 0;
                    tableAmounts.AddCell(cellTotalLAmount);


                    PdfPCell cellOpenDeduct = new PdfPCell(new Phrase("Open - Deducted Amount: " + pStat.DeductableAmountOpen.ToString("C").Replace("$", "Rs."), fBold9));
                    cellOpenDeduct.Border = 0;
                    tableAmounts.AddCell(cellOpenDeduct);

                    PdfPCell cellClosedDeduct = new PdfPCell(new Phrase("Closed - Deducted Amount: " + pStat.DeductableAmountClosed.ToString("C").Replace("$", "Rs."), fNormal9));
                    cellClosedDeduct.Border = 0;
                    tableAmounts.AddCell(cellClosedDeduct);

                    PdfPCell cellBlank = new PdfPCell(new Phrase("", fNormal9));
                    cellBlank.Border = 0;
                    tableAmounts.AddCell(cellBlank);


                    PdfPCell cellOpenBalance = new PdfPCell(new Phrase("Open - Balance Amount: " + pStat.BalanceAmountOpen.ToString("C").Replace("$", "Rs."), fBold9));
                    cellOpenBalance.Border = 0;
                    tableAmounts.AddCell(cellOpenBalance);

                    PdfPCell cellClosedBalance = new PdfPCell(new Phrase("Closed - Balance Amount: " + pStat.BalanceAmountClosed.ToString("C").Replace("$", "Rs."), fNormal9));
                    cellClosedBalance.Border = 0;
                    tableAmounts.AddCell(cellClosedBalance);

                    PdfPCell cellTotalBalance = new PdfPCell(new Phrase("Total - Balance Amount: " + (pStat.BalanceAmountOpen + pStat.BalanceAmountClosed).ToString("C").Replace("$", "Rs."), fBold9));
                    cellTotalBalance.Border = 0;
                    tableAmounts.AddCell(cellTotalBalance);


                    //-------------------------------------------------------------------------------

                    document.Add(tableAmounts);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 60.0f, 90.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Employee Code", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Employee Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Month Year", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Loan Amount (Rs.)", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Installment (Rs.)", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Deduction (Rs.)", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Balance (Rs.)", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Status", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    foreach (var log in pStat.LoanInfoList)
                    {
                        c++;

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.EmployeeCode, fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.EmployeeName, fNormal8));
                        //cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.LoanAllocatedDateText, fNormal8));
                        //cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.LoanAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData4.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.InstallmentAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData5.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.DeductableAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData6.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.BalanceAmount.ToString("C").Replace("$", ""), fNormal8));
                        cellData7.HorizontalAlignment = 2;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.LoanStatusText, fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);
                    }

                    if (pStat != null)
                    {
                        document.Add(tableMid);

                        // ---------- End Table ---------------------
                        Paragraph p_nsig = new Paragraph("This is a system generated loan statment and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Loan-Statment-" + pStat.EmployeeCode + ".pdf");
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

                    ///////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }




        ////////////////////////////////////////////////////////////////

        #endregion


        #region SummaryReport

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

        #endregion

        #region ViewConsolidatedAttendance

        public ActionResult ConsolidatedAttendance()
        {
            return View();
        }

        public ActionResult ConsolidatedAttendanceShort()
        {
            return View();
        }

        public ActionResult ConsolidatedAttendanceOvertime()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConsolidatedAttendanceOvertimeStatus(ViewModels.ConsolidatedAttendanceLog toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.StatusUpdate.Update(toUpdate);
            return Json(new { status = "success" });
        }

        public class ConsolidatedReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        public class MonthlyTimesheetReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        public class MonthlyDepartmentalTimesheetReportTable : DTParameters
        {
            public int department_id { get; set; }
            public int function_id { get; set; }
            public int region_id { get; set; }
            public int location_id { get; set; }
            public int designation_id { get; set; }

            public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        public class PayrollReportByMonthTable : DTParameters
        {
            public string salary_month_year { get; set; }
        }

        public class LoanReportByMonthTable : DTParameters
        {
            public string month_year { get; set; }
        }

        public class LoanReportByEmployeeTable : DTParameters
        {
            public int employee_id { get; set; }
        }

        public class LoanReportByDateTable : DTParameters
        {
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        public class PayrollReportByMonthDepartmentTable : DTParameters
        {
            public string month_year { get; set; }
            public int department_id { get; set; }
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
        public JsonResult ConsolidatedReportShortDataHandler(ConsolidatedReportTable param)
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

        #endregion


        #region DirectDownloadExcelXLSXVersion

        [HttpPost]
        public ActionResult AttendanceSummaryExcelDownload(CustomDTParameters param)
        {
            var data = new List<AttendanceSummaryExport>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int count = TimeTune.Reports.getMonthlyReportExcel(out data, param.from_date, param.to_date);

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

        [HttpPost]
        public ActionResult ConsolidatedReportOvertimeExcelDownload(ConsolidatedReportTable param)
        {
            var ddata = new List<ConsolidatedAttendanceExportOvertime>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatchingOvertime(param.employee_id, param.from_date, param.to_date, out ddata);

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


        #region Department Wise Report

        public class ConsolidatedDepartmentReportTable : DTParameters
        {
            public string department_id { get; set; }
            public string designation_id { get; set; }
            public string function_id { get; set; }
            public string region_id { get; set; }
            public string location_id { get; set; }
            public string month { get; set; }
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
        public JsonResult DepartmentdDataHandlerWithALL()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Department[] department = TimeTune.EmployeeManagementHelper.getAllDepartmentsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[department.Length + 1];

            toSend[0] = new ChosenAutoCompleteResults();
            toSend[0].id = "-1";
            toSend[0].text = "-- ALL --";

            for (int i = 0; i < department.Length; i++)
            {
                toSend[i + 1] = new ChosenAutoCompleteResults();

                toSend[i + 1].id = department[i].id.ToString();
                toSend[i + 1].text = department[i].name;
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
        public JsonResult DesignationDataHandlerWithALL()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Designation[] designation = TimeTune.EmployeeManagementHelper.getAllDesignationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[designation.Length + 1];

            toSend[0] = new ChosenAutoCompleteResults();
            toSend[0].id = "-1";
            toSend[0].text = "-- ALL --";

            for (int i = 0; i < designation.Length; i++)
            {
                toSend[i + 1] = new ChosenAutoCompleteResults();

                toSend[i + 1].id = designation[i].id.ToString();
                toSend[i + 1].text = designation[i].name;
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
        public JsonResult LocationDataHandlerWithALL()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Location[] location = TimeTune.EmployeeManagementHelper.getAllLocationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[location.Length + 1];

            toSend[0] = new ChosenAutoCompleteResults();
            toSend[0].id = "-1";
            toSend[0].text = "-- ALL --";


            for (int i = 0; i < location.Length; i++)
            {
                toSend[i + 1] = new ChosenAutoCompleteResults();

                toSend[i + 1].id = location[i].id.ToString();
                toSend[i + 1].text = location[i].name;
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

                int count = TimeTune.Reports.getAllDepartmentLogsDepartmentWise(param.department_id, param.designation_id, param.function_id, param.region_id, User.Identity.Name, param.location_id, param.from_date, param.to_date, param.Search.Value, "date", param.Start, param.Length, out data);

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
        public JsonResult graph(string dept, string des, string loc, string from, string to)
        {
            ViewModels.Dashboard dashboard = TimeTune.Dashboard.getGraphValues(dept, des, User.Identity.Name, loc, from, to);
            return Json(dashboard);
        }

        [HttpGet]
        [ActionName("DepartmentReport")]
        public ActionResult DepartmentReport_Get()
        {
            ViewData["PDFNoDataFoundDept"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("DepartmentReport")]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmentReport_Post()
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

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFoundDept"] = "";
            found = GenerateDepartmentWiseReportPDF(reportMaker);

            if (found == 1)
            {
                ViewData["PDFNoDataFoundDept"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFoundDept"] = "No Data Found";
            }

            return View("DepartmentReport");

            //GenerateDepartmentWiseReportPDF(reportMaker);

            //return null;
        }

        private int GenerateDepartmentWiseReportPDF(BLL.ViewModels.FilteredAttendanceReportDepartmentWise sdata)
        {
            int response = 0;

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
                    cellDateTime.RunDirection = PdfLayoutHelper.RunDirection;
                    tableHeader.AddCell(cellDateTime);

                    document.Add(tableHeader);



                    document.Add(lineSeparator);


                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(3);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    tableHeader.SpacingBefore = 10f;
                    tableEmployee.SpacingAfter = 10f;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    Paragraph p_title = new Paragraph("Department Wise Report", fBold16);
                    p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    tableEmployee.AddCell("");
                    tableEmployee.AddCell("");
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
                        Response.AddHeader("content-disposition", "attachment;filename=Deptwise-Report-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        response = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return response;
        }

        #endregion

        #region Present and absent report
        public ActionResult PaReport()
        {
            return View();
        }
        public class PaReportTable : DTParameters
        {
            public string final_remarks { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }
        [HttpPost]
        public JsonResult PaReportDataHandler(PaReportTable param)
        {
            try
            {
                var data = new List<ViewModels.PaAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllPaAttendanceMatching(param.final_remarks, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<PaAttendanceLog> result = new DTResult<PaAttendanceLog>
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

        #region Devices Status

        public ActionResult DevicesStatusReport()
        {
            //List<BLL_UNIS.ViewModels.DevicesStatus> vList = new List<DevicesStatus>();

            //int count = BLL_UNIS.UNIS_Reports.getDevicesList(out vList);

            return View();
        }

        [HttpPost]
        public JsonResult DevicesStatusReportDataHandler(DeviceStatusReportTable param)
        {
            try
            {
                List<BLL_UNIS.ViewModels.DevicesStatus> data = new List<DevicesStatus>();

                data = BLL_UNIS.UNIS_Reports.getDevicesStatusList(null, null, 0, 10);

                DTResult<BLL_UNIS.ViewModels.DevicesStatus> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatus>
                {
                    draw = param.Draw,
                    data = data

                };
                return Json(result);
                //List<BLL_UNIS.ViewModels.DevicesStatus> data = new List<DevicesStatus>();

                //int count = BLL_UNIS.UNIS_Reports.getDevicesList(param.device_number, param.from_date, param.to_date, param.device_status_type, param.Search.Value, "L_TID", param.Start, param.Length, out data);
                ////var data = new List<MonthlyTimesheetAttendanceLog>();
                ////// get all employee view models
                ////int count2 = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<BLL_UNIS.ViewModels.DevicesStatus> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatus>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};
                //return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        /*
        [HttpPost]
        public ActionResult DevicesStatusReportExcelDownload(DeviceStatusReportTable param)
        {
            List<DevicesStatusExcelDownload> ddata = new List<DevicesStatusExcelDownload>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int count = BLL_UNIS.UNIS_Reports.getDevicesListExceldownload(param.device_number, param.from_date, param.to_date, param.device_status_type, param.Start, param.Length, out ddata);
                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<DevicesStatusExcelDownload>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("DevicesStatusReport");
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
                Data = new { FileGuid = handle, FileName = "Devices-Status-Report.xlsx" }
            };
        }
        */

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

        [HttpPost]
        public JsonResult ChangeDeviceStatusNumberDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            BLL_UNIS.ViewModels.Terminal[] terminals = BLL_UNIS.UNIS_Reports.getAllDeviceNumbersMatching(q);

            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[terminals.Length];
            for (int i = 0; i < terminals.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = terminals[i].L_ID.ToString();
                toSend[i].text = terminals[i].C_Name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        public ActionResult Devicesunregistered()
        {

            return View();
        }

        [HttpPost]
        public JsonResult UnRegisterDevicesStatusReport(DeviceStatusReportTable param)
        {
            try
            {
                List<BLL_UNIS.ViewModels.DevicesUnregister> data = new List<DevicesUnregister>();

                data = BLL_UNIS.UNIS_Reports.getUnRegisterDevicesStatusList();

                DTResult<BLL_UNIS.ViewModels.DevicesUnregister> result = new DTResult<BLL_UNIS.ViewModels.DevicesUnregister>
                {
                    draw = param.Draw,
                    data = data

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

        #region LeavesReportByUser

        public ActionResult LeavesReportByUser()
        {
            return View();
        }

        public class LeavesReportByUserTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public int users_type { get; set; }
        }

        [HttpPost]
        public JsonResult LeavesReportByUserDataHandler(LeavesReportByUserTable param)
        {
            try
            {
                /*////////////////////////////////////////////
                if (param.employee_id != null && param.employee_id.ToString() != "")
                {
                    // only access groups are to be sent without ajax.
                    CreateLeaveApplication vm = new CreateLeaveApplication();

                    DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };
                    ////dt = LeaveApplicationResultSet.getSessionDatesByAcademicCalendar(DateTime.Now.Year);

                    dt = LeaveApplicationResultSet.getStaffSessionDatesByUserId(param.employee_id);
                    vm.SessionStartDate = dt[0];
                    vm.SessionEndDate = dt[1];
                    vm.strSessionStartDate = dt[0].ToString("dd MMM yyyy");
                    vm.strSessionEndDate = dt[1].ToString("dd MMM yyyy");

                    int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                    leaves = LeaveApplicationResultSet.getUserLeavesByUserId(param.employee_id);
                    vm.AvailableSickLeaves = leaves[0];
                    vm.AvailableCasualLeaves = leaves[1];
                    vm.AvailableAnnualLeaves = leaves[2];
                    vm.AvailedSickLeaves = leaves[3];
                    vm.AvailedCasualLeaves = leaves[4];
                    vm.AvailedAnnualLeaves = leaves[5];
                }
                ///////////////////////////////////////////*/

                var data = new List<LeavesReportByUserLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllLeavesReportByUserMatching(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<LeavesReportByUserLog> result = new DTResult<LeavesReportByUserLog>
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
        public JsonResult LeavesReportByUserCounter(LeavesReportByUserTable param)
        {
            // only access groups are to be sent without ajax.
            CreateLeaveApplication vm = new CreateLeaveApplication();


            //string fromDate = param.from_date;
            //string toDate = param.from_date;

            DateTime dtFromDate = DateTime.ParseExact(param.from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime dtToDate = DateTime.ParseExact(param.to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            int iToDateYear = dtToDate.Year;

            DateTime dtSessionFromDate = new DateTime(iToDateYear, 1, 1);
            DateTime dtSessionToDate = new DateTime(iToDateYear, 12, 31);

            try
            {
                /////////////////////////////////////////////
                if (param.employee_id != null && param.employee_id.ToString() != "")
                {
                    DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };
                    ////dt = LeaveApplicationResultSet.getSessionDatesByAcademicCalendar(DateTime.Now.Year);

                    //if (param.users_type == 1)
                    //{
                    dt = LeaveApplicationResultSet.getStaffSessionDatesByUserId(param.employee_id);
                    //}
                    //else
                    //{
                    //dt = LeaveApplicationResultSet.getStudentSessionDatesByUserId(param.employee_id);
                    //}

                    vm.SessionStartDate = dt[0];
                    vm.SessionEndDate = dt[1];
                    vm.strSessionStartDate = dt[0].ToString("dd-MM-yyyy");
                    vm.strSessionEndDate = dt[1].ToString("dd-MM-yyyy");

                    int[] leaves = new int[30] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    leaves = LeaveApplicationResultSet.getUserLeavesByUserId(param.employee_id, dtSessionFromDate, dtSessionToDate);
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
                }
                ////////////////////////////////////////////

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }

            return Json(vm);
        }

        ////////////////////////////////////////////////////////

        #endregion

        #region LeavesStatisticsReport


        [HttpGet]
        [ActionName("LeavesStatistics")]
        public ActionResult LeavesStatistics_Get()
        {
            string emp_code = "", ss_year = "";
            int session_year = DateTime.Now.Year;

            List<LeavesCountReportLog> data = new List<LeavesCountReportLog>();

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = ViewBag.LMEmployeesList = TimeTune.LMManualAttendance.getEmployee_All();

            ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(-1);
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
                emp_code = "-1"; // User.Identity.Name;
                ss_year = DateTime.Now.Year.ToString();
                session_year = int.Parse(yearList[0].ToString());
                ViewBag.SelectedSessionYear = session_year;
                ViewBag.SelectedEmployeeCode = "";
            }

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

                    ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                    ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                    ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
                    ViewBag.AvailedOtherLeaves = item.AvailedOtherLeaves;
                }
            }
            else
            {
                ViewBag.AllocatedSickLeaves = 0;
                ViewBag.AllocatedCasualLeaves = 0;
                ViewBag.AllocatedAnnualLeaves = 0;
                ViewBag.AllocatedOtherLeaves = 0;

                ViewBag.AvailedSickLeaves = 0;
                ViewBag.AvailedCasualLeaves = 0;
                ViewBag.AvailedAnnualLeaves = 0;
                ViewBag.AvailedOtherLeaves = 0;
            }

            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();
            lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpCode(User.Identity.Name, session_year);

            return View(lInfo);
        }

        [HttpPost]
        [ActionName("LeavesStatistics")]
        public ActionResult LeavesStatistics_Post()
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

                    ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                    ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                    ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
                }
            }
            else
            {
                ViewBag.AllocatedSickLeaves = 0;
                ViewBag.AllocatedCasualLeaves = 0;
                ViewBag.AllocatedAnnualLeaves = 0;

                ViewBag.AvailedSickLeaves = 0;
                ViewBag.AvailedCasualLeaves = 0;
                ViewBag.AvailedAnnualLeaves = 0;
            }

            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();
            lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpID(int.Parse(str_employee_id), int.Parse(session_year));

            return View(lInfo);
        }


        public class LeavesStatisticsReportTable : DTParameters
        {
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public ActionResult LeavesBalanceReportTableReportDataHandler(LeavesStatisticsReportTable param, string from_date, string to_date)
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

        #region LeavesListANDCountReport


        //public ActionResult LeavesListReport()
        //{
        //    return View();
        //}

        public class LeavesListReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public int users_type { get; set; }
            public int availed_status { get; set; }
        }

        [HttpGet]
        [ActionName("LeavesListReport")]
        public ActionResult LeavesListReport_Get()
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
        [ActionName("LeavesListReport")]
        [ValidateAntiForgeryToken]
        public ActionResult LeavesListReport_Post()
        {
            int sessionYearID = 0, availedStatus = 0;

            if (!int.TryParse(Request.Form["employee_id"], out sessionYearID))
                return RedirectToAction("LeavesListReport");

            if (!int.TryParse(Request.Form["availed_status"], out availedStatus))
                return RedirectToAction("LeavesListReport");

            //string month = Request.Form["month"];

            //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            ////List<LeavesCountReportLog>
            //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

            var toRender = new List<LeavesListReportLog>();
            int count = TimeTune.Reports.getAllLeavesListReportForHRPDF(sessionYearID.ToString(), availedStatus.ToString(), out toRender);

            if (toRender == null)
                return RedirectToAction("LeavesListReport");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateLeavesListReportPDF("", toRender); // GenerateEvaluationPDF(toRender);

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




        [HttpPost]
        public JsonResult LeavesListReportDataHandler(LeavesListReportTable param)
        {
            try
            {
                var data = new List<LeavesListReportLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllLeavesListReportForHRMatching(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                List<LeavesListReportLog> Sorteddata = LeavesListReportResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);


                DTResult<LeavesListReportLog> result = new DTResult<LeavesListReportLog>
                {
                    draw = param.Draw,
                    data = Sorteddata,
                    recordsFiltered = Sorteddata.Count,
                    recordsTotal = Sorteddata.Count
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        private int GenerateLeavesListReportPDF(string session_year, List<LeavesListReportLog> list_count)
        {
            int reponse = 0;

            try
            {
                //DateTime FDate = DateTime.Now; DateTime TDate = DateTime.Now;
                //if (fromDate != null && fromDate != "")
                //    FDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //if (toDate != null && toDate != "")
                //    TDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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

                    Font fBold11 = FontFactory.GetFont("HELVETICA", 11, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    ////////////////----------CHART RENDERING------------------////////////
                    //string _image = imageData.Replace("data:image/png;base64,", "");

                    //var base64Data = Regex.Match(_image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);

                    //byte[] bytes = Convert.FromBase64String(_image);

                    ////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                    //System.Drawing.Image img = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
                    //Image logo2 = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);


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

                    //Charts
                    //https://www.aspsnippets.com/Articles/Export-Chart-to-PDF-in-ASPNet-MVC.aspx
                    string strChartData = "";

                    //foreach (LeavesCountReportLog log in list_count)
                    //{
                    //    strChartData = "S=" + log.AvailedSickLeaves + ",S=" + log.AllocatedSickLeaves + ",C=" + log.AvailedCasualLeaves + ",C=" + log.AllocatedCasualLeaves + ",A=" + log.AvailedAnnualLeaves + ",A=" + log.AllocatedAnnualLeaves + ",O=" + log.AvailedOtherLeaves + ",O=" + log.AllocatedOtherLeaves + ",T=" + log.AvailedLeaveType01 + ",T=" + log.AllocatedLeaveType01 + ",V=" + log.AvailedLeaveType02 + ",V=" + log.AllocatedLeaveType02 + ",M=" + log.AvailedLeaveType03 + ",M=" + log.AllocatedLeaveType03 + ",R=" + log.AvailedLeaveType04 + ",R=" + log.AllocatedLeaveType04;
                    //}

                    //strChartData = strChartData.Replace("S=", "").Replace("C=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("V=", "").Replace("M=", "").Replace("R=", "");

                    //byte[] bytesArrLeavesChart = CreateVisualColumnChart(0, strChartData); // GetPresenceChartData(); //PopulateChart();
                    //iTextSharp.text.Image leavesChart = iTextSharp.text.Image.GetInstance(bytesArrLeavesChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrPunChart = CreateVisualColumnChart(1, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image punChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrExitChart = CreateVisualColumnChart(2, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image exitChart = iTextSharp.text.Image.GetInstance(bytesArrExitChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //PdfPTable tableCharts = new PdfPTable(new[] { 50.0f, 200.0f, 95.0f, 200.0f, 50.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////PdfPTable tableCharts = new PdfPTable(new[] { 20.0f, 800.0f, 20.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////tableCharts.WidthPercentage = 100;
                    ////tableCharts.HeaderRows = 0;
                    ////tableCharts.SpacingBefore = 3;
                    ////tableCharts.SpacingAfter = 3;
                    ////tableCharts.DefaultCell.Border = Rectangle.NO_BORDER;

                    //////tableCharts.AddCell("");
                    //////tableCharts.AddCell(prsChart);
                    //////tableCharts.AddCell("");
                    //////tableCharts.AddCell(punChart);
                    //////tableCharts.AddCell("");

                    ////tableCharts.AddCell("");
                    ////tableCharts.AddCell(leavesChart);
                    ////tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");


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


                    PdfPCell cellECode = new PdfPCell(new Phrase("Session Year: " + session_year, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    //string strFormFields = ViewModel.GlobalVariables.GV_DepartmentAttendanceNames;
                    //strFormFields = strFormFields.Replace("D=", "").Replace("S=", "").Replace("C=", "").Replace("R=", "").Replace("F=", "");
                    //string[] strSplit = strFormFields.Split(',');                    

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Leaves List Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    ////document.Add(tableCharts);

                    //document.NewPage();

                    //PdfPTable tableChart = new PdfPTable(new[] { 200.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChart.WidthPercentage = 100;
                    //tableChart.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50f;
                    //tableEmployee.SpacingAfter = 10f;
                    //tableChart.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPTable tableChartInfo = new PdfPTable(1);
                    //tableChartInfo.WidthPercentage = 100;
                    //tableChartInfo.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableChartInfo.SpacingAfter = 3;
                    //tableChartInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellEChart = new PdfPCell();
                    //cellEChart.Border = 0;
                    //tableChartInfo.AddCell(logo2);

                    //tableChart.AddCell(tableChartInfo);
                    //document.Add(tableChart);

                    //Paragraph count_heading = new Paragraph("List:", fBold14);
                    //count_heading.SpacingBefore = 3;
                    //count_heading.SpacingAfter = 3;
                    //document.Add(count_heading);

                    // ---------- Count Table ---------------------
                    //set table with Landscape (Rotate) 840x595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableTop = new PdfPTable(new[] { 60.0f, 100.0f, 100.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f });
                    tableTop.WidthPercentage = 100;
                    tableTop.HeaderRows = 1;
                    tableTop.SpacingBefore = 5;
                    tableTop.SpacingAfter = 5;

                    PdfPCell cell11 = new PdfPCell(new Phrase("Employee Code", fBold8));
                    cell11.BackgroundColor = Color.LIGHT_GRAY;
                    cell11.HorizontalAlignment = 1;
                    tableTop.AddCell(cell11);

                    PdfPCell cell12 = new PdfPCell(new Phrase("First Name", fBold8));
                    cell12.BackgroundColor = Color.LIGHT_GRAY;
                    cell12.HorizontalAlignment = 1;
                    tableTop.AddCell(cell12);

                    PdfPCell cell13 = new PdfPCell(new Phrase("Last Name", fBold8));
                    cell13.BackgroundColor = Color.LIGHT_GRAY;
                    cell13.HorizontalAlignment = 1;
                    tableTop.AddCell(cell13);

                    PdfPCell cell01 = new PdfPCell(new Phrase("Sick Leaves", fBold8));
                    cell01.BackgroundColor = Color.LIGHT_GRAY;
                    cell01.HorizontalAlignment = 1;
                    tableTop.AddCell(cell01);

                    PdfPCell cell02 = new PdfPCell(new Phrase("Casual Leaves", fBold8));
                    cell02.BackgroundColor = Color.LIGHT_GRAY;
                    cell02.HorizontalAlignment = 1;
                    tableTop.AddCell(cell02);

                    PdfPCell cell03 = new PdfPCell(new Phrase("Annual Leaves", fBold8));
                    cell03.BackgroundColor = Color.LIGHT_GRAY;
                    cell03.HorizontalAlignment = 1;
                    tableTop.AddCell(cell03);

                    PdfPCell cell04 = new PdfPCell(new Phrase("Other Leaves", fBold8));
                    cell04.BackgroundColor = Color.LIGHT_GRAY;
                    cell04.HorizontalAlignment = 1;
                    tableTop.AddCell(cell04);

                    PdfPCell cell05 = new PdfPCell(new Phrase("Tour Leave", fBold8));
                    cell05.BackgroundColor = Color.LIGHT_GRAY;
                    cell05.HorizontalAlignment = 1;
                    tableTop.AddCell(cell05);

                    PdfPCell cell06 = new PdfPCell(new Phrase("Visit Leave", fBold8));
                    cell06.BackgroundColor = Color.LIGHT_GRAY;
                    cell06.HorizontalAlignment = 1;
                    tableTop.AddCell(cell06);

                    PdfPCell cell07 = new PdfPCell(new Phrase("Meeting Leave", fBold8));
                    cell07.BackgroundColor = Color.LIGHT_GRAY;
                    cell07.HorizontalAlignment = 1;
                    tableTop.AddCell(cell07);

                    PdfPCell cell08 = new PdfPCell(new Phrase("Maternity Leave", fBold8));
                    cell08.BackgroundColor = Color.LIGHT_GRAY;
                    cell08.HorizontalAlignment = 1;
                    tableTop.AddCell(cell08);

                    foreach (LeavesListReportLog log in list_count)
                    {
                        PdfPCell cellData11 = new PdfPCell(new Phrase(log.Employee_Code.ToString(), fNormal8));
                        cellData11.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData11);

                        PdfPCell cellData12 = new PdfPCell(new Phrase(log.First_Name.ToString(), fNormal8));
                        //cellData12.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData12);

                        PdfPCell cellData13 = new PdfPCell(new Phrase(log.Last_Name.ToString(), fNormal8));
                        //cellData13.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData13);

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.AvailedSickLeaves.ToString() + "/" + log.AllocatedSickLeaves.ToString(), fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.AvailedCasualLeaves.ToString() + "/" + log.AllocatedCasualLeaves.ToString(), fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.AvailedAnnualLeaves.ToString() + "/" + log.AllocatedAnnualLeaves.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.AvailedOtherLeaves.ToString() + "/" + log.AllocatedOtherLeaves.ToString(), fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.AvailedLeaveType01.ToString() + "/" + log.AllocatedLeaveType01.ToString(), fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.AvailedLeaveType02.ToString() + "/" + log.AllocatedLeaveType02.ToString(), fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.AvailedLeaveType03.ToString() + "/" + log.AllocatedLeaveType03.ToString(), fNormal8));
                        cellData7.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.AvailedLeaveType04.ToString() + "/" + log.AllocatedLeaveType04.ToString(), fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData8);
                    }

                    if (list_count.Count > 0)
                    {
                        document.Add(tableTop);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Leaves-List-Report-" + session_year + ".pdf");
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
                    Session["LV_Emp_Code"] = Request.QueryString["e_code"];
                    Session["LV_SS_Year"] = Request.QueryString["s_year"];

                    emp_code = Request.QueryString["e_code"];
                    ss_year = Request.QueryString["s_year"];
                    session_year = int.Parse(ss_year);
                    ViewBag.SelectedSessionYear = ss_year;
                    ViewBag.SelectedEmployeeCode = " Detail of Employee Code: " + emp_code;


                    // get all employee view models
                    TimeTune.Attendance.getLeavesCountReportByEmpCode(emp_code, session_year.ToString(), out data);
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
                        ViewBag.AllocatedLeaveType05 = item.AllocatedLeaveType05;
                        ViewBag.AllocatedLeaveType06 = item.AllocatedLeaveType06;
                        ViewBag.AllocatedLeaveType07 = item.AllocatedLeaveType07;
                        ViewBag.AllocatedLeaveType08 = item.AllocatedLeaveType08;
                        ViewBag.AllocatedLeaveType09 = item.AllocatedLeaveType09;
                        ViewBag.AllocatedLeaveType10 = item.AllocatedLeaveType10;
                        ViewBag.AllocatedLeaveType11 = item.AllocatedLeaveType11;

                        ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                        ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                        ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
                        ViewBag.AvailedOtherLeaves = item.AvailedOtherLeaves;
                        ViewBag.AvailedLeaveType01 = item.AvailedLeaveType01;
                        ViewBag.AvailedLeaveType02 = item.AvailedLeaveType02;
                        ViewBag.AvailedLeaveType03 = item.AvailedLeaveType03;
                        ViewBag.AvailedLeaveType04 = item.AvailedLeaveType04;
                        ViewBag.AvailedLeaveType05 = item.AvailedLeaveType05;
                        ViewBag.AvailedLeaveType06 = item.AvailedLeaveType06;
                        ViewBag.AvailedLeaveType07 = item.AvailedLeaveType07;
                        ViewBag.AvailedLeaveType08 = item.AvailedLeaveType08;
                        ViewBag.AvailedLeaveType09 = item.AvailedLeaveType09;
                        ViewBag.AvailedLeaveType10 = item.AvailedLeaveType10;
                        ViewBag.AvailedLeaveType11 = item.AvailedLeaveType11;
                    }

                    lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpCode(emp_code, session_year);
                    if (lInfo != null && lInfo.Count > 0)
                    {
                        ViewBag.ExportPDF = "";
                    }
                    else
                    {
                        ViewBag.ExportPDF = "d-none";
                    }
                }
                else
                {
                    emp_code = User.Identity.Name;
                    ss_year = DateTime.Now.Year.ToString();
                    session_year = int.Parse(yearList[0].ToString());
                    ViewBag.SelectedSessionYear = session_year;
                    ViewBag.SelectedEmployeeCode = "";
                }
            }

            return View(lInfo);
        }

        [HttpPost]
        [ActionName("LeavesCountReport")]
        public ActionResult LeavesCountReport_PDF()
        {
            string emp_code = "", ss_year = "";
            int session_year = DateTime.Now.Year;
            List<LeavesCountReportLog> data = new List<LeavesCountReportLog>();
            List<LeaveApplicationInfo> lInfo = new List<LeaveApplicationInfo>();

            if (ViewModel.GlobalVariables.GV_EmployeeId != null && ViewModel.GlobalVariables.GV_EmployeeId != "")
            {
                ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(int.Parse(ViewModel.GlobalVariables.GV_EmployeeId));
                ViewBag.SessionYearsList = yearList;

                if (Session["LV_Emp_Code"] != null && Session["LV_Emp_Code"].ToString() != "" && Session["LV_SS_Year"] != null && Session["LV_SS_Year"].ToString() != "")
                {
                    emp_code = Session["LV_Emp_Code"].ToString();
                    ss_year = Session["LV_SS_Year"].ToString();
                    session_year = int.Parse(ss_year);
                    ViewBag.SelectedSessionYear = ss_year;
                    ViewBag.SelectedEmployeeCode = " Detail of Employee Code: " + emp_code;


                    // get all employee view models
                    TimeTune.Attendance.getLeavesCountReportByEmpCode(emp_code, session_year.ToString(), out data);
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
                        ViewBag.AllocatedLeaveType05 = item.AllocatedLeaveType05;
                        ViewBag.AllocatedLeaveType06 = item.AllocatedLeaveType06;
                        ViewBag.AllocatedLeaveType07 = item.AllocatedLeaveType07;
                        ViewBag.AllocatedLeaveType08 = item.AllocatedLeaveType08;
                        ViewBag.AllocatedLeaveType09 = item.AllocatedLeaveType09;
                        ViewBag.AllocatedLeaveType10 = item.AllocatedLeaveType10;
                        ViewBag.AllocatedLeaveType11 = item.AllocatedLeaveType11;

                        ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                        ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                        ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
                        ViewBag.AvailedOtherLeaves = item.AvailedOtherLeaves;
                        ViewBag.AvailedLeaveType01 = item.AvailedLeaveType01;
                        ViewBag.AvailedLeaveType02 = item.AvailedLeaveType02;
                        ViewBag.AvailedLeaveType03 = item.AvailedLeaveType03;
                        ViewBag.AvailedLeaveType04 = item.AvailedLeaveType04;
                        ViewBag.AvailedLeaveType05 = item.AvailedLeaveType05;
                        ViewBag.AvailedLeaveType06 = item.AvailedLeaveType06;
                        ViewBag.AvailedLeaveType07 = item.AvailedLeaveType07;
                        ViewBag.AvailedLeaveType08 = item.AvailedLeaveType08;
                        ViewBag.AvailedLeaveType09 = item.AvailedLeaveType09;
                        ViewBag.AvailedLeaveType10 = item.AvailedLeaveType10;
                        ViewBag.AvailedLeaveType11 = item.AvailedLeaveType11;
                    }

                    lInfo = TimeTune.Attendance.getLeaveApplicationsByEmployeeCodeANDSessionYear(emp_code, session_year);

                    if (lInfo != null && lInfo.Count > 0)
                    {
                        LeavesCountReportPDF(emp_code, data, lInfo);
                        ViewData["PDFNoDataFoundDept"] = "";
                        //return null;
                    }
                    else
                    {
                        ViewData["PDFNoDataFoundDept"] = "No Data Found";
                    }
                }
                else
                {
                    emp_code = User.Identity.Name;
                    ss_year = DateTime.Now.Year.ToString();
                    session_year = int.Parse(yearList[0].ToString());
                    ViewBag.SelectedSessionYear = session_year;
                    ViewBag.SelectedEmployeeCode = "";
                }
            }

            return View(lInfo);
        }

        private int LeavesCountReportPDF(string emp_code, List<LeavesCountReportLog> list_count, List<LeaveApplicationInfo> list_leaves)
        {
            int reponse = 0;
            string imageURL = "";

            try
            {
                //DateTime FDate = DateTime.Now; DateTime TDate = DateTime.Now;
                //if (fromDate != null && fromDate != "")
                //    FDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //if (toDate != null && toDate != "")
                //    TDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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

                    Font fBold11 = FontFactory.GetFont("HELVETICA", 11, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    ////////////////----------CHART RENDERING------------------////////////
                    //string _image = imageData.Replace("data:image/png;base64,", "");

                    //var base64Data = Regex.Match(_image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);

                    //byte[] bytes = Convert.FromBase64String(_image);

                    ////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                    //System.Drawing.Image img = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
                    //Image logo2 = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);


                    // ---------- Header Table ---------------------
                    imageURL = Server.MapPath(strLogotitle[0]);
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

                    //data management
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

                    string LTCS = "";

                    bool LT01IsActive = false, LT02IsActive = false, LT03IsActive = false, LT04IsActive = false, LT05IsActive = false, LT06IsActive = false, LT07IsActive = false, LT08IsActive = false;
                    bool LT09IsActive = false, LT10IsActive = false, LT11IsActive = false, LT12IsActive = false, LT13IsActive = false, LT14IsActive = false, LT15IsActive = false;

                    int LT01DCount = 0, LT02DCount = 0, LT03DCount = 0, LT04DCount = 0, LT05DCount = 0, LT06DCount = 0, LT07DCount = 0, LT08DCount = 0;
                    int LT09DCount = 0, LT10DCount = 0, LT11DCount = 0, LT12DCount = 0, LT13DCount = 0, LT14DCount = 0, LT15DCount = 0;

                    LTCS = leavesTitles.getLeavesTypesCountsStatus();
                    if (LTCS != null && LTCS.Contains(','))
                    {
                        string[] strSplit = LTCS.Split(',');
                        if (strSplit != null && strSplit.Length > 0)
                        {
                            //LT01DCount = strSplit[0].ToString(); LT02DCount = strSplit[1].ToString(); LT03DCount = strSplit[2].ToString(); LT04DCount = strSplit[3].ToString();
                            //LT05DCount = strSplit[4].ToString(); LT06DCount = strSplit[5].ToString(); LT07DCount = strSplit[6].ToString(); LT08DCount = strSplit[7].ToString();
                            //LT09DCount = strSplit[8].ToString(); LT10DCount = strSplit[9].ToString(); LT11DCount = strSplit[10].ToString(); LT12DCount = strSplit[11].ToString();
                            //LT13DCount = strSplit[12].ToString(); LT14DCount = strSplit[13].ToString(); LT15DCount = strSplit[14].ToString();

                            LT01IsActive = strSplit[0].Split('-')[2].ToString() == "True" ? true : false; LT02IsActive = strSplit[1].Split('-')[2].ToString() == "True" ? true : false; LT03IsActive = strSplit[2].Split('-')[2].ToString() == "True" ? true : false; LT04IsActive = strSplit[3].Split('-')[2].ToString() == "True" ? true : false;
                            LT05IsActive = strSplit[4].Split('-')[2].ToString() == "True" ? true : false; LT06IsActive = strSplit[5].Split('-')[2].ToString() == "True" ? true : false; LT07IsActive = strSplit[6].Split('-')[2].ToString() == "True" ? true : false; LT08IsActive = strSplit[7].Split('-')[2].ToString() == "True" ? true : false;
                            LT09IsActive = strSplit[8].Split('-')[2].ToString() == "True" ? true : false; LT10IsActive = strSplit[9].Split('-')[2].ToString() == "True" ? true : false; LT11IsActive = strSplit[10].Split('-')[2].ToString() == "True" ? true : false; LT12IsActive = strSplit[11].Split('-')[2].ToString() == "True" ? true : false;
                            LT13IsActive = strSplit[12].Split('-')[2].ToString() == "True" ? true : false; LT14IsActive = strSplit[13].Split('-')[2].ToString() == "True" ? true : false; LT15IsActive = strSplit[14].Split('-')[2].ToString() == "True" ? true : false;
                        }
                    }
                    ///////////////////////////////////////////////////////////////////////////////////
                    int colsCount = 0;

                    if (LT01IsActive)
                        colsCount++;
                    if (LT02IsActive)
                        colsCount++;
                    if (LT03IsActive)
                        colsCount++;
                    if (LT04IsActive)
                        colsCount++;
                    if (LT05IsActive)
                        colsCount++;
                    if (LT06IsActive)
                        colsCount++;
                    if (LT07IsActive)
                        colsCount++;
                    if (LT08IsActive)
                        colsCount++;
                    if (LT09IsActive)
                        colsCount++;
                    if (LT10IsActive)
                        colsCount++;
                    if (LT11IsActive)
                        colsCount++;
                    if (LT12IsActive)
                        colsCount++;
                    if (LT13IsActive)
                        colsCount++;
                    if (LT14IsActive)
                        colsCount++;
                    if (LT15IsActive)
                        colsCount++;

                    //Charts
                    //https://www.aspsnippets.com/Articles/Export-Chart-to-PDF-in-ASPNet-MVC.aspx
                    string strChartData = "";

                    foreach (LeavesCountReportLog log in list_count)
                    {
                        if (LT01IsActive)// && log.AllocatedSickLeaves > 0)
                            strChartData = "S=" + log.AvailedSickLeaves + ",S=" + log.AllocatedSickLeaves + ",";

                        if (LT02IsActive)// && log.AllocatedCasualLeaves > 0)
                            strChartData += "C=" + log.AvailedCasualLeaves + ",C=" + log.AllocatedCasualLeaves + ",";

                        if (LT03IsActive)// && log.AllocatedAnnualLeaves > 0)
                            strChartData += "A=" + log.AvailedAnnualLeaves + ",A=" + log.AllocatedAnnualLeaves + ",";

                        if (LT04IsActive)// && log.AllocatedOtherLeaves > 0)
                            strChartData += "O=" + log.AvailedOtherLeaves + ",O=" + log.AllocatedOtherLeaves + ",";

                        if (LT05IsActive)// && log.AllocatedLeaveType01 > 0)
                            strChartData += "T=" + log.AvailedLeaveType01 + ",T=" + log.AllocatedLeaveType01 + ",";

                        if (LT06IsActive)// && log.AllocatedLeaveType02 > 0)
                            strChartData += "V=" + log.AvailedLeaveType02 + ",V=" + log.AllocatedLeaveType02 + ",";

                        if (LT07IsActive)// && log.AllocatedLeaveType03 > 0)
                            strChartData += "M=" + log.AvailedLeaveType03 + ",M=" + log.AllocatedLeaveType03 + ",";

                        if (LT08IsActive)// && log.AllocatedLeaveType04 > 0)
                            strChartData += "R=" + log.AvailedLeaveType04 + ",R=" + log.AllocatedLeaveType04 + ",";

                        if (LT09IsActive)// && log.AllocatedLeaveType05 > 0)
                            strChartData += "L05=" + log.AvailedLeaveType05 + ",L05=" + log.AllocatedLeaveType05 + ",";

                        if (LT10IsActive)// && log.AllocatedLeaveType06 > 0)
                            strChartData += "L06=" + log.AvailedLeaveType06 + ",L06=" + log.AllocatedLeaveType06 + ",";

                        if (LT11IsActive)// && log.AllocatedLeaveType07 > 0)
                            strChartData += "L07=" + log.AvailedLeaveType07 + ",L07=" + log.AllocatedLeaveType07 + ",";

                        if (LT12IsActive)// && log.AllocatedLeaveType08 > 0)
                            strChartData += "L08=" + log.AvailedLeaveType08 + ",L08=" + log.AllocatedLeaveType08 + ",";

                        if (LT13IsActive)// && log.AllocatedLeaveType09 > 0)
                            strChartData += "L09=" + log.AvailedLeaveType09 + ",L09=" + log.AllocatedLeaveType09 + ",";

                        if (LT14IsActive)// && log.AllocatedLeaveType10 > 0)
                            strChartData += "L10=" + log.AvailedLeaveType10 + ",L10=" + log.AllocatedLeaveType10 + ",";

                        if (LT15IsActive)// && log.AllocatedLeaveType11 > 0)
                            strChartData += "L11=" + log.AvailedLeaveType11 + ",L11=" + log.AllocatedLeaveType11 + ",";
                    }

                    strChartData = strChartData.TrimEnd(',');
                    strChartData = strChartData.Replace("S=", "").Replace("C=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("V=", "").Replace("M=", "").Replace("R=", "").Replace("L05=", "").Replace("L06=", "").Replace("L07=", "").Replace("L08=", "").Replace("L09=", "").Replace("L10=", "").Replace("L11=", "");

                    iTextSharp.text.Image leavesChart = null; byte[] bytesArrLeavesChart = null;
                    bytesArrLeavesChart = CreateVisualColumnChart(0, strChartData); // GetPresenceChartData(); //PopulateChart();
                    if (bytesArrLeavesChart != null)
                    {
                        leavesChart = iTextSharp.text.Image.GetInstance(bytesArrLeavesChart);
                    }

                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrPunChart = CreateVisualColumnChart(1, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image punChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrExitChart = CreateVisualColumnChart(2, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image exitChart = iTextSharp.text.Image.GetInstance(bytesArrExitChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //PdfPTable tableCharts = new PdfPTable(new[] { 50.0f, 200.0f, 95.0f, 200.0f, 50.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    PdfPTable tableCharts = new PdfPTable(new[] { 20.0f, 800.0f, 20.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableCharts.WidthPercentage = 100;
                    tableCharts.HeaderRows = 0;
                    tableCharts.SpacingBefore = 3;
                    tableCharts.SpacingAfter = 3;
                    tableCharts.DefaultCell.Border = Rectangle.NO_BORDER;

                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(prsChart);
                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");

                    tableCharts.AddCell("");

                    if (leavesChart != null)
                        tableCharts.AddCell(leavesChart);
                    else
                        tableCharts.AddCell("");

                    tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");


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


                    PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + emp_code, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    //string strFormFields = ViewModel.GlobalVariables.GV_DepartmentAttendanceNames;
                    //strFormFields = strFormFields.Replace("D=", "").Replace("S=", "").Replace("C=", "").Replace("R=", "").Replace("F=", "");
                    //string[] strSplit = strFormFields.Split(',');                    

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Leaves Count Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    document.Add(tableCharts);

                    //document.NewPage();

                    //PdfPTable tableChart = new PdfPTable(new[] { 200.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChart.WidthPercentage = 100;
                    //tableChart.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50f;
                    //tableEmployee.SpacingAfter = 10f;
                    //tableChart.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPTable tableChartInfo = new PdfPTable(1);
                    //tableChartInfo.WidthPercentage = 100;
                    //tableChartInfo.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableChartInfo.SpacingAfter = 3;
                    //tableChartInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellEChart = new PdfPCell();
                    //cellEChart.Border = 0;
                    //tableChartInfo.AddCell(logo2);

                    //tableChart.AddCell(tableChartInfo);
                    //document.Add(tableChart);

                    Paragraph count_heading = new Paragraph("Count Summary:", fBold14);
                    count_heading.SpacingBefore = 3;
                    count_heading.SpacingAfter = 3;
                    document.Add(count_heading);

                    // ---------- Count Table ---------------------
                    //set table with Landscape (Rotate) 840x595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableTop = null;

                    if (colsCount == 1)
                        tableTop = new PdfPTable(new[] { 100.0f });
                    else if (colsCount == 2)
                        tableTop = new PdfPTable(new[] { 50.0f, 50.0f });
                    else if (colsCount == 3)
                        tableTop = new PdfPTable(new[] { 33.0f, 33.0f, 33.0f });
                    else if (colsCount == 4)
                        tableTop = new PdfPTable(new[] { 25.0f, 25.0f, 25.0f, 25.0f });
                    else if (colsCount == 5)
                        tableTop = new PdfPTable(new[] { 20.0f, 20.0f, 20.0f, 20.0f, 20.0f });
                    else if (colsCount == 6)
                        tableTop = new PdfPTable(new[] { 18.0f, 18.0f, 18.0f, 18.0f, 18.0f, 18.0f });
                    else if (colsCount == 7)
                        tableTop = new PdfPTable(new[] { 17.0f, 17.0f, 17.0f, 17.0f, 17.0f, 17.0f, 17.0f });
                    else if (colsCount == 8)
                        tableTop = new PdfPTable(new[] { 16.0f, 16.0f, 16.0f, 16.0f, 16.0f, 16.0f, 16.0f, 16.0f });
                    else if (colsCount == 9)
                        tableTop = new PdfPTable(new[] { 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f });
                    else if (colsCount == 10)
                        tableTop = new PdfPTable(new[] { 14.0f, 14.0f, 14.0f, 14.0f, 14.0f, 14.0f, 14.0f, 14.0f, 14.0f, 14.0f });
                    else if (colsCount == 11)
                        tableTop = new PdfPTable(new[] { 13.0f, 13.0f, 13.0f, 13.0f, 13.0f, 13.0f, 13.0f, 13.0f, 13.0f, 13.0f, 13.0f });
                    else if (colsCount == 12)
                        tableTop = new PdfPTable(new[] { 12.0f, 12.0f, 12.0f, 12.0f, 12.0f, 12.0f, 12.0f, 12.0f, 12.0f, 12.0f, 12.0f, 12.0f });
                    else if (colsCount == 13)
                        tableTop = new PdfPTable(new[] { 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f, 11.0f });
                    else if (colsCount == 14)
                        tableTop = new PdfPTable(new[] { 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f });
                    else if (colsCount == 15)
                        tableTop = new PdfPTable(new[] { 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f, 7.0f });

                    tableTop.WidthPercentage = 100;
                    tableTop.HeaderRows = 0;
                    tableTop.SpacingBefore = 5;
                    tableTop.SpacingAfter = 5;

                    if (LT01IsActive)
                    {
                        PdfPCell cell01 = new PdfPCell(new Phrase(TitleLeave01 + " Leaves", fBold8));
                        cell01.BackgroundColor = Color.LIGHT_GRAY;
                        cell01.HorizontalAlignment = 1;
                        tableTop.AddCell(cell01);
                    }

                    if (LT02IsActive)
                    {
                        PdfPCell cell02 = new PdfPCell(new Phrase(TitleLeave02 + " Leaves", fBold8));
                        cell02.BackgroundColor = Color.LIGHT_GRAY;
                        cell02.HorizontalAlignment = 1;
                        tableTop.AddCell(cell02);
                    }

                    if (LT03IsActive)
                    {
                        PdfPCell cell03 = new PdfPCell(new Phrase(TitleLeave03 + " Leaves", fBold8));
                        cell03.BackgroundColor = Color.LIGHT_GRAY;
                        cell03.HorizontalAlignment = 1;
                        tableTop.AddCell(cell03);
                    }

                    if (LT04IsActive)
                    {
                        PdfPCell cell04 = new PdfPCell(new Phrase(TitleLeave04 + " Leaves", fBold8));
                        cell04.BackgroundColor = Color.LIGHT_GRAY;
                        cell04.HorizontalAlignment = 1;
                        tableTop.AddCell(cell04);
                    }

                    if (LT05IsActive)
                    {
                        PdfPCell cell05 = new PdfPCell(new Phrase(TitleLeave05 + " Leave", fBold8));
                        cell05.BackgroundColor = Color.LIGHT_GRAY;
                        cell05.HorizontalAlignment = 1;
                        tableTop.AddCell(cell05);
                    }

                    if (LT06IsActive)
                    {
                        PdfPCell cell06 = new PdfPCell(new Phrase(TitleLeave06 + " Leave", fBold8));
                        cell06.BackgroundColor = Color.LIGHT_GRAY;
                        cell06.HorizontalAlignment = 1;
                        tableTop.AddCell(cell06);
                    }

                    if (LT07IsActive)
                    {
                        PdfPCell cell07 = new PdfPCell(new Phrase(TitleLeave07 + " Leave", fBold8));
                        cell07.BackgroundColor = Color.LIGHT_GRAY;
                        cell07.HorizontalAlignment = 1;
                        tableTop.AddCell(cell07);
                    }

                    if (LT08IsActive)
                    {
                        PdfPCell cell08 = new PdfPCell(new Phrase(TitleLeave08 + " Leave", fBold8));
                        cell08.BackgroundColor = Color.LIGHT_GRAY;
                        cell08.HorizontalAlignment = 1;
                        tableTop.AddCell(cell08);
                    }

                    if (LT09IsActive)
                    {
                        PdfPCell cell09 = new PdfPCell(new Phrase(TitleLeave09 + " Leaves", fBold8));
                        cell09.BackgroundColor = Color.LIGHT_GRAY;
                        cell09.HorizontalAlignment = 1;
                        tableTop.AddCell(cell09);
                    }

                    if (LT10IsActive)
                    {
                        PdfPCell cell10 = new PdfPCell(new Phrase(TitleLeave10 + " Leaves", fBold8));
                        cell10.BackgroundColor = Color.LIGHT_GRAY;
                        cell10.HorizontalAlignment = 1;
                        tableTop.AddCell(cell10);
                    }

                    if (LT11IsActive)
                    {
                        PdfPCell cell11 = new PdfPCell(new Phrase(TitleLeave11 + " Leaves", fBold8));
                        cell11.BackgroundColor = Color.LIGHT_GRAY;
                        cell11.HorizontalAlignment = 1;
                        tableTop.AddCell(cell11);
                    }

                    if (LT12IsActive)
                    {
                        PdfPCell cell12 = new PdfPCell(new Phrase(TitleLeave12 + " Leave", fBold8));
                        cell12.BackgroundColor = Color.LIGHT_GRAY;
                        cell12.HorizontalAlignment = 1;
                        tableTop.AddCell(cell12);
                    }

                    if (LT13IsActive)
                    {
                        PdfPCell cell13 = new PdfPCell(new Phrase(TitleLeave13 + " Leave", fBold8));
                        cell13.BackgroundColor = Color.LIGHT_GRAY;
                        cell13.HorizontalAlignment = 1;
                        tableTop.AddCell(cell13);
                    }

                    if (LT14IsActive)
                    {
                        PdfPCell cell14 = new PdfPCell(new Phrase(TitleLeave14 + " Leave", fBold8));
                        cell14.BackgroundColor = Color.LIGHT_GRAY;
                        cell14.HorizontalAlignment = 1;
                        tableTop.AddCell(cell14);
                    }

                    if (LT15IsActive)
                    {
                        PdfPCell cell15 = new PdfPCell(new Phrase(TitleLeave15 + " Leave", fBold8));
                        cell15.BackgroundColor = Color.LIGHT_GRAY;
                        cell15.HorizontalAlignment = 1;
                        tableTop.AddCell(cell15);
                    }

                    foreach (LeavesCountReportLog log in list_count)
                    {
                        if (LT01IsActive)
                        {
                            PdfPCell cellData1 = new PdfPCell(new Phrase(log.AvailedSickLeaves.ToString() + "/" + log.AllocatedSickLeaves.ToString(), fBold11));
                            cellData1.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData1);
                        }

                        if (LT02IsActive)
                        {
                            PdfPCell cellData2 = new PdfPCell(new Phrase(log.AvailedCasualLeaves.ToString() + "/" + log.AllocatedCasualLeaves.ToString(), fBold11));
                            cellData2.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData2);
                        }

                        if (LT03IsActive)
                        {
                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.AvailedAnnualLeaves.ToString() + "/" + log.AllocatedAnnualLeaves.ToString(), fBold11));
                            cellData3.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData3);
                        }

                        if (LT04IsActive)
                        {
                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.AvailedOtherLeaves.ToString() + "/" + log.AllocatedOtherLeaves.ToString(), fBold11));
                            cellData4.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData4);
                        }

                        if (LT05IsActive)
                        {
                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.AvailedLeaveType01.ToString() + "/" + log.AllocatedLeaveType01.ToString(), fBold11));
                            cellData5.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData5);
                        }

                        if (LT06IsActive)
                        {
                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.AvailedLeaveType02.ToString() + "/" + log.AllocatedLeaveType02.ToString(), fBold11));
                            cellData6.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData6);
                        }

                        if (LT07IsActive)
                        {
                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.AvailedLeaveType03.ToString() + "/" + log.AllocatedLeaveType03.ToString(), fBold11));
                            cellData7.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData7);
                        }

                        if (LT08IsActive)
                        {
                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.AvailedLeaveType04.ToString() + "/" + log.AllocatedLeaveType04.ToString(), fBold11));
                            cellData8.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData8);
                        }

                        if (LT09IsActive)
                        {
                            PdfPCell cellData9 = new PdfPCell(new Phrase(log.AvailedLeaveType05.ToString() + "/" + log.AllocatedLeaveType05.ToString(), fBold11));
                            cellData9.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData9);
                        }

                        if (LT10IsActive)
                        {
                            PdfPCell cellData10 = new PdfPCell(new Phrase(log.AvailedLeaveType06.ToString() + "/" + log.AllocatedLeaveType06.ToString(), fBold11));
                            cellData10.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData10);
                        }

                        if (LT11IsActive)
                        {
                            PdfPCell cellData11 = new PdfPCell(new Phrase(log.AvailedLeaveType07.ToString() + "/" + log.AllocatedLeaveType07.ToString(), fBold11));
                            cellData11.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData11);
                        }

                        if (LT12IsActive)
                        {
                            PdfPCell cellData12 = new PdfPCell(new Phrase(log.AvailedLeaveType08.ToString() + "/" + log.AllocatedLeaveType08.ToString(), fBold11));
                            cellData12.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData12);
                        }

                        if (LT13IsActive)
                        {
                            PdfPCell cellData13 = new PdfPCell(new Phrase(log.AvailedLeaveType09.ToString() + "/" + log.AllocatedLeaveType09.ToString(), fBold11));
                            cellData13.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData13);
                        }

                        if (LT14IsActive)
                        {
                            PdfPCell cellData14 = new PdfPCell(new Phrase(log.AvailedLeaveType10.ToString() + "/" + log.AllocatedLeaveType10.ToString(), fBold11));
                            cellData14.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData14);
                        }

                        if (LT15IsActive)
                        {
                            PdfPCell cellData15 = new PdfPCell(new Phrase(log.AvailedLeaveType11.ToString() + "/" + log.AllocatedLeaveType11.ToString(), fBold11));
                            cellData15.HorizontalAlignment = 1;
                            tableTop.AddCell(cellData15);
                        }
                    }

                    if (list_leaves.Count > 0)
                    {
                        document.Add(tableTop);
                    }

                    document.NewPage();

                    Paragraph leaves_heading = new Paragraph("Leaves List:", fBold14);
                    leaves_heading.SpacingBefore = 3;
                    leaves_heading.SpacingAfter = 3;
                    document.Add(leaves_heading);

                    // ---------- Middle Table ---------------------
                    //set table with Landscape (Rotate) 840x595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 60.0f, 100.0f, 100.0f, 100.0f, 60.0f, 60.0f, 320.0f });
                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    //tableMid.SpacingBefore = 10;
                    //tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Id", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Leave Type", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("From Date", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("To Date", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Days Count", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Leave Status", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Reason", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);


                    foreach (LeaveApplicationInfo log in list_leaves)
                    {
                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.Id.ToString(), fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.LeaveTypeText == null ? "" : log.LeaveTypeText.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.FromDateText == null ? "" : log.FromDateText.ToString(), fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.ToDateText == null ? "" : log.ToDateText.ToString(), fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.DaysCount.ToString(), fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.LeaveStatusText == null ? "" : log.LeaveStatusText.ToString(), fNormal8));
                        cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.LeaveReasonText == null ? "" : log.LeaveReasonText.ToString(), fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);

                    }


                    if (list_leaves.Count > 0)
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
                        Response.AddHeader("content-disposition", "attachment;filename=Leaves-Count-Report-" + emp_code + ".pdf");
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
                throw ex;

                //RedirectPermanent("~/Home/" + imageURL);
            }

            return reponse;
        }

        [HttpPut]
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
                    ViewBag.AllocatedLeaveType05 = item.AllocatedLeaveType05;
                    ViewBag.AllocatedLeaveType06 = item.AllocatedLeaveType06;
                    ViewBag.AllocatedLeaveType07 = item.AllocatedLeaveType07;
                    ViewBag.AllocatedLeaveType08 = item.AllocatedLeaveType08;
                    ViewBag.AllocatedLeaveType09 = item.AllocatedLeaveType09;
                    ViewBag.AllocatedLeaveType10 = item.AllocatedLeaveType10;
                    ViewBag.AllocatedLeaveType11 = item.AllocatedLeaveType11;

                    ViewBag.AvailedSickLeaves = item.AvailedSickLeaves;
                    ViewBag.AvailedCasualLeaves = item.AvailedCasualLeaves;
                    ViewBag.AvailedAnnualLeaves = item.AvailedAnnualLeaves;
                    ViewBag.AvailedOtherLeaves = item.AvailedOtherLeaves;
                    ViewBag.AvailedLeaveType01 = item.AvailedLeaveType01;
                    ViewBag.AvailedLeaveType02 = item.AvailedLeaveType02;
                    ViewBag.AvailedLeaveType03 = item.AvailedLeaveType03;
                    ViewBag.AvailedLeaveType04 = item.AvailedLeaveType04;
                    ViewBag.AvailedLeaveType05 = item.AvailedLeaveType05;
                    ViewBag.AvailedLeaveType06 = item.AvailedLeaveType06;
                    ViewBag.AvailedLeaveType07 = item.AvailedLeaveType07;
                    ViewBag.AvailedLeaveType08 = item.AvailedLeaveType08;
                    ViewBag.AvailedLeaveType09 = item.AvailedLeaveType09;
                    ViewBag.AvailedLeaveType10 = item.AvailedLeaveType10;
                    ViewBag.AvailedLeaveType11 = item.AvailedLeaveType11;
                }

                lInfo = TimeTune.Attendance.getLeaveApplicationsByEmpCode(User.Identity.Name, int.Parse(session_year));
            }

            return View(lInfo);
        }


        public class LeavesCountReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public int users_type { get; set; }
        }

        [HttpPost]
        public JsonResult LeavesCountReportDataHandler(LeavesCountReportTable param)
        {
            try
            {
                var data = new List<LeavesCountReportLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllLeavesCountReportMatching(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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
        }



        ////////////////////////////////////////////////////////

        #endregion


        #region LeavesApplyStatusReport

        public class LeavesApplyStatusReportTable : DTParameters
        {
            public string year_id { get; set; }
            public int apply_status { get; set; }
        }

        [HttpGet]
        [ActionName("LeavesApplyStatusReport")]
        public ActionResult LeavesApplyStatusReport_Get()
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
        [ActionName("LeavesApplyStatusReport")]
        [ValidateAntiForgeryToken]
        public ActionResult LeavesApplyStatusReport_Post()
        {
            int yearID = 0, applyStatus = 0;

            if (!int.TryParse(Request.Form["year_id"], out yearID))
                return RedirectToAction("LeavesApplyStatusReport");

            if (!int.TryParse(Request.Form["apply_status"], out applyStatus))
                return RedirectToAction("LeavesApplyStatusReport");

            //string month = Request.Form["month"];

            //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            ////List<LeavesCountReportLog>
            //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

            var toRender = new List<LeavesApplyStatusReportLog>();
            int count = TimeTune.Reports.getAllLeavesApplyStatusReportForHRPDF(yearID.ToString(), applyStatus.ToString(), out toRender);

            if (toRender == null)
                return RedirectToAction("LeavesApplyStatusReport");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateLeavesApplyStatusReportPDF("", toRender); // GenerateEvaluationPDF(toRender);

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

        [HttpPost]
        public JsonResult LeavesApplyStatusReportDataHandler(LeavesApplyStatusReportTable param)
        {
            int count = 0;
            List<LeavesApplyStatusReportLog> Sorteddata = new List<LeavesApplyStatusReportLog>();

            try
            {


                var data = new List<LeavesApplyStatusReportLog>();

                // get all employee view models
                if (param.year_id != null)
                {


                    count = TimeTune.Reports.getAllLeavesApplyStatusReportForHRMatching(param.year_id, param.apply_status, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                    Sorteddata = LeavesApplyStatusReportResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                }

                DTResult<LeavesApplyStatusReportLog> result = new DTResult<LeavesApplyStatusReportLog>
                {
                    draw = param.Draw,
                    data = Sorteddata,
                    recordsFiltered = Sorteddata.Count,
                    recordsTotal = Sorteddata.Count
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        private int GenerateLeavesApplyStatusReportPDF(string session_year, List<LeavesApplyStatusReportLog> list_count)
        {
            int reponse = 0;

            try
            {
                //DateTime FDate = DateTime.Now; DateTime TDate = DateTime.Now;
                //if (fromDate != null && fromDate != "")
                //    FDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //if (toDate != null && toDate != "")
                //    TDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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

                    Font fBold11 = FontFactory.GetFont("HELVETICA", 11, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    ////////////////----------CHART RENDERING------------------////////////
                    //string _image = imageData.Replace("data:image/png;base64,", "");

                    //var base64Data = Regex.Match(_image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);

                    //byte[] bytes = Convert.FromBase64String(_image);

                    ////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                    //System.Drawing.Image img = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
                    //Image logo2 = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);


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

                    //Charts
                    //https://www.aspsnippets.com/Articles/Export-Chart-to-PDF-in-ASPNet-MVC.aspx
                    string strChartData = "";

                    //foreach (LeavesCountReportLog log in list_count)
                    //{
                    //    strChartData = "S=" + log.AvailedSickLeaves + ",S=" + log.AllocatedSickLeaves + ",C=" + log.AvailedCasualLeaves + ",C=" + log.AllocatedCasualLeaves + ",A=" + log.AvailedAnnualLeaves + ",A=" + log.AllocatedAnnualLeaves + ",O=" + log.AvailedOtherLeaves + ",O=" + log.AllocatedOtherLeaves + ",T=" + log.AvailedLeaveType01 + ",T=" + log.AllocatedLeaveType01 + ",V=" + log.AvailedLeaveType02 + ",V=" + log.AllocatedLeaveType02 + ",M=" + log.AvailedLeaveType03 + ",M=" + log.AllocatedLeaveType03 + ",R=" + log.AvailedLeaveType04 + ",R=" + log.AllocatedLeaveType04;
                    //}

                    //strChartData = strChartData.Replace("S=", "").Replace("C=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("V=", "").Replace("M=", "").Replace("R=", "");

                    //byte[] bytesArrLeavesChart = CreateVisualColumnChart(0, strChartData); // GetPresenceChartData(); //PopulateChart();
                    //iTextSharp.text.Image leavesChart = iTextSharp.text.Image.GetInstance(bytesArrLeavesChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrPunChart = CreateVisualColumnChart(1, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image punChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrExitChart = CreateVisualColumnChart(2, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image exitChart = iTextSharp.text.Image.GetInstance(bytesArrExitChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //PdfPTable tableCharts = new PdfPTable(new[] { 50.0f, 200.0f, 95.0f, 200.0f, 50.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////PdfPTable tableCharts = new PdfPTable(new[] { 20.0f, 800.0f, 20.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////tableCharts.WidthPercentage = 100;
                    ////tableCharts.HeaderRows = 0;
                    ////tableCharts.SpacingBefore = 3;
                    ////tableCharts.SpacingAfter = 3;
                    ////tableCharts.DefaultCell.Border = Rectangle.NO_BORDER;

                    //////tableCharts.AddCell("");
                    //////tableCharts.AddCell(prsChart);
                    //////tableCharts.AddCell("");
                    //////tableCharts.AddCell(punChart);
                    //////tableCharts.AddCell("");

                    ////tableCharts.AddCell("");
                    ////tableCharts.AddCell(leavesChart);
                    ////tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");


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


                    PdfPCell cellECode = new PdfPCell(new Phrase("Session Year: " + session_year, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    //string strFormFields = ViewModel.GlobalVariables.GV_DepartmentAttendanceNames;
                    //strFormFields = strFormFields.Replace("D=", "").Replace("S=", "").Replace("C=", "").Replace("R=", "").Replace("F=", "");
                    //string[] strSplit = strFormFields.Split(',');                    

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Leaves List Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    ////document.Add(tableCharts);

                    //document.NewPage();

                    //PdfPTable tableChart = new PdfPTable(new[] { 200.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChart.WidthPercentage = 100;
                    //tableChart.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50f;
                    //tableEmployee.SpacingAfter = 10f;
                    //tableChart.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPTable tableChartInfo = new PdfPTable(1);
                    //tableChartInfo.WidthPercentage = 100;
                    //tableChartInfo.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableChartInfo.SpacingAfter = 3;
                    //tableChartInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellEChart = new PdfPCell();
                    //cellEChart.Border = 0;
                    //tableChartInfo.AddCell(logo2);

                    //tableChart.AddCell(tableChartInfo);
                    //document.Add(tableChart);

                    //Paragraph count_heading = new Paragraph("List:", fBold14);
                    //count_heading.SpacingBefore = 3;
                    //count_heading.SpacingAfter = 3;
                    //document.Add(count_heading);

                    // ---------- Count Table ---------------------
                    //set table with Landscape (Rotate) 840x595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableTop = new PdfPTable(new[] { 60.0f, 100.0f, 100.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f, 60.0f });
                    tableTop.WidthPercentage = 100;
                    tableTop.HeaderRows = 1;
                    tableTop.SpacingBefore = 5;
                    tableTop.SpacingAfter = 5;

                    PdfPCell cell11 = new PdfPCell(new Phrase("Employee Code", fBold8));
                    cell11.BackgroundColor = Color.LIGHT_GRAY;
                    cell11.HorizontalAlignment = 1;
                    tableTop.AddCell(cell11);

                    PdfPCell cell12 = new PdfPCell(new Phrase("First Name", fBold8));
                    cell12.BackgroundColor = Color.LIGHT_GRAY;
                    cell12.HorizontalAlignment = 1;
                    tableTop.AddCell(cell12);

                    PdfPCell cell13 = new PdfPCell(new Phrase("Last Name", fBold8));
                    cell13.BackgroundColor = Color.LIGHT_GRAY;
                    cell13.HorizontalAlignment = 1;
                    tableTop.AddCell(cell13);

                    PdfPCell cell01 = new PdfPCell(new Phrase("Sick Leaves", fBold8));
                    cell01.BackgroundColor = Color.LIGHT_GRAY;
                    cell01.HorizontalAlignment = 1;
                    tableTop.AddCell(cell01);

                    PdfPCell cell02 = new PdfPCell(new Phrase("Casual Leaves", fBold8));
                    cell02.BackgroundColor = Color.LIGHT_GRAY;
                    cell02.HorizontalAlignment = 1;
                    tableTop.AddCell(cell02);

                    PdfPCell cell03 = new PdfPCell(new Phrase("Annual Leaves", fBold8));
                    cell03.BackgroundColor = Color.LIGHT_GRAY;
                    cell03.HorizontalAlignment = 1;
                    tableTop.AddCell(cell03);

                    PdfPCell cell04 = new PdfPCell(new Phrase("Other Leaves", fBold8));
                    cell04.BackgroundColor = Color.LIGHT_GRAY;
                    cell04.HorizontalAlignment = 1;
                    tableTop.AddCell(cell04);

                    PdfPCell cell05 = new PdfPCell(new Phrase("Tour Leave", fBold8));
                    cell05.BackgroundColor = Color.LIGHT_GRAY;
                    cell05.HorizontalAlignment = 1;
                    tableTop.AddCell(cell05);

                    PdfPCell cell06 = new PdfPCell(new Phrase("Visit Leave", fBold8));
                    cell06.BackgroundColor = Color.LIGHT_GRAY;
                    cell06.HorizontalAlignment = 1;
                    tableTop.AddCell(cell06);

                    PdfPCell cell07 = new PdfPCell(new Phrase("Meeting Leave", fBold8));
                    cell07.BackgroundColor = Color.LIGHT_GRAY;
                    cell07.HorizontalAlignment = 1;
                    tableTop.AddCell(cell07);

                    PdfPCell cell08 = new PdfPCell(new Phrase("Maternity Leave", fBold8));
                    cell08.BackgroundColor = Color.LIGHT_GRAY;
                    cell08.HorizontalAlignment = 1;
                    tableTop.AddCell(cell08);

                    foreach (LeavesApplyStatusReportLog log in list_count)
                    {
                        PdfPCell cellData11 = new PdfPCell(new Phrase(log.Employee_Code.ToString(), fNormal8));
                        cellData11.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData11);

                        PdfPCell cellData12 = new PdfPCell(new Phrase(log.First_Name.ToString(), fNormal8));
                        //cellData12.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData12);

                        PdfPCell cellData13 = new PdfPCell(new Phrase(log.Last_Name.ToString(), fNormal8));
                        //cellData13.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData13);

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.AvailedSickLeaves.ToString() + "/" + log.AllocatedSickLeaves.ToString(), fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.AvailedCasualLeaves.ToString() + "/" + log.AllocatedCasualLeaves.ToString(), fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.AvailedAnnualLeaves.ToString() + "/" + log.AllocatedAnnualLeaves.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.AvailedOtherLeaves.ToString() + "/" + log.AllocatedOtherLeaves.ToString(), fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.AvailedLeaveType01.ToString() + "/" + log.AllocatedLeaveType01.ToString(), fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.AvailedLeaveType02.ToString() + "/" + log.AllocatedLeaveType02.ToString(), fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.AvailedLeaveType03.ToString() + "/" + log.AllocatedLeaveType03.ToString(), fNormal8));
                        cellData7.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.AvailedLeaveType04.ToString() + "/" + log.AllocatedLeaveType04.ToString(), fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData8);
                    }

                    if (list_count.Count > 0)
                    {
                        document.Add(tableTop);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Leaves-List-Report-" + session_year + ".pdf");
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

        ////////////////////////////////////////////////////////

        #endregion

        #region LeavesValidityStatusReport

        public class LeavesValidityStatusReportTable : DTParameters
        {
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpGet]
        [ActionName("LeavesValidityStatusReport")]
        public ActionResult LeavesValidityStatusReport_Get()
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
        [ActionName("LeavesValidityStatusReport")]
        [ValidateAntiForgeryToken]
        public ActionResult LeavesValidityStatusReport_Post()
        {
            string fromDate = "", toDate = "";

            if (Request.Form["from_date"] != null && Request.Form["from_date"].ToString() != "")
            {
                fromDate = Request.Form["from_date"].ToString();
            }
            else
            {
                return RedirectToAction("LeavesValidityStatusReport");
            }

            if (Request.Form["to_date"] != null && Request.Form["to_date"].ToString() != "")
            {
                toDate = Request.Form["to_date"].ToString();
            }
            else
            {
                return RedirectToAction("LeavesValidityStatusReport");
            }

            //string month = Request.Form["month"];

            //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            ////List<LeavesCountReportLog>
            //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

            var toRender = new List<LeavesValidityReportLog>();
            int count = TimeTune.Reports.getAllLeavesValidityReportForHRPDF(fromDate, toDate, out toRender);

            if (toRender == null)
                return RedirectToAction("LeavesValidityStatusReport");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateLeavesValidityStatusReportPDF("", toRender); // GenerateEvaluationPDF(toRender);

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

        [HttpPost]
        public JsonResult LeavesValidityStatusReportDataHandler(LeavesValidityStatusReportTable param)
        {
            int count = 0;
            List<LeavesValidityReportLog> Sorteddata = new List<LeavesValidityReportLog>();

            try
            {


                var data = new List<LeavesValidityReportLog>();

                // get all employee view models
                if (param.from_date != null)
                {


                    count = TimeTune.Reports.getAllLeavesValidityReportForHRMatching(param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                    Sorteddata = LeavesValidityReportResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                }

                DTResult<LeavesValidityReportLog> result = new DTResult<LeavesValidityReportLog>
                {
                    draw = param.Draw,
                    data = Sorteddata,
                    recordsFiltered = Sorteddata.Count,
                    recordsTotal = Sorteddata.Count
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        private int GenerateLeavesValidityStatusReportPDF(string session_year, List<LeavesValidityReportLog> list_count)
        {
            int reponse = 0;

            try
            {
                //DateTime FDate = DateTime.Now; DateTime TDate = DateTime.Now;
                //if (fromDate != null && fromDate != "")
                //    FDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //if (toDate != null && toDate != "")
                //    TDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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

                    Font fBold11 = FontFactory.GetFont("HELVETICA", 11, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);//A4.Rotate()

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    ////////////////----------CHART RENDERING------------------////////////
                    //string _image = imageData.Replace("data:image/png;base64,", "");

                    //var base64Data = Regex.Match(_image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);

                    //byte[] bytes = Convert.FromBase64String(_image);

                    ////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                    //System.Drawing.Image img = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
                    //Image logo2 = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);


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

                    //Charts
                    //https://www.aspsnippets.com/Articles/Export-Chart-to-PDF-in-ASPNet-MVC.aspx
                    string strChartData = "";

                    //foreach (LeavesCountReportLog log in list_count)
                    //{
                    //    strChartData = "S=" + log.AvailedSickLeaves + ",S=" + log.AllocatedSickLeaves + ",C=" + log.AvailedCasualLeaves + ",C=" + log.AllocatedCasualLeaves + ",A=" + log.AvailedAnnualLeaves + ",A=" + log.AllocatedAnnualLeaves + ",O=" + log.AvailedOtherLeaves + ",O=" + log.AllocatedOtherLeaves + ",T=" + log.AvailedLeaveType01 + ",T=" + log.AllocatedLeaveType01 + ",V=" + log.AvailedLeaveType02 + ",V=" + log.AllocatedLeaveType02 + ",M=" + log.AvailedLeaveType03 + ",M=" + log.AllocatedLeaveType03 + ",R=" + log.AvailedLeaveType04 + ",R=" + log.AllocatedLeaveType04;
                    //}

                    //strChartData = strChartData.Replace("S=", "").Replace("C=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("V=", "").Replace("M=", "").Replace("R=", "");

                    //byte[] bytesArrLeavesChart = CreateVisualColumnChart(0, strChartData); // GetPresenceChartData(); //PopulateChart();
                    //iTextSharp.text.Image leavesChart = iTextSharp.text.Image.GetInstance(bytesArrLeavesChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrPunChart = CreateVisualColumnChart(1, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image punChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //byte[] bytesArrExitChart = CreateVisualColumnChart(2, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    //iTextSharp.text.Image exitChart = iTextSharp.text.Image.GetInstance(bytesArrExitChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //PdfPTable tableCharts = new PdfPTable(new[] { 50.0f, 200.0f, 95.0f, 200.0f, 50.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////PdfPTable tableCharts = new PdfPTable(new[] { 20.0f, 800.0f, 20.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    ////tableCharts.WidthPercentage = 100;
                    ////tableCharts.HeaderRows = 0;
                    ////tableCharts.SpacingBefore = 3;
                    ////tableCharts.SpacingAfter = 3;
                    ////tableCharts.DefaultCell.Border = Rectangle.NO_BORDER;

                    //////tableCharts.AddCell("");
                    //////tableCharts.AddCell(prsChart);
                    //////tableCharts.AddCell("");
                    //////tableCharts.AddCell(punChart);
                    //////tableCharts.AddCell("");

                    ////tableCharts.AddCell("");
                    ////tableCharts.AddCell(leavesChart);
                    ////tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");


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


                    PdfPCell cellECode = new PdfPCell(new Phrase("Session Year: " + session_year, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    //string strFormFields = ViewModel.GlobalVariables.GV_DepartmentAttendanceNames;
                    //strFormFields = strFormFields.Replace("D=", "").Replace("S=", "").Replace("C=", "").Replace("R=", "").Replace("F=", "");
                    //string[] strSplit = strFormFields.Split(',');                    

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Leaves Validity Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    ////document.Add(tableCharts);

                    //document.NewPage();

                    //PdfPTable tableChart = new PdfPTable(new[] { 200.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChart.WidthPercentage = 100;
                    //tableChart.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50f;
                    //tableEmployee.SpacingAfter = 10f;
                    //tableChart.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPTable tableChartInfo = new PdfPTable(1);
                    //tableChartInfo.WidthPercentage = 100;
                    //tableChartInfo.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableChartInfo.SpacingAfter = 3;
                    //tableChartInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellEChart = new PdfPCell();
                    //cellEChart.Border = 0;
                    //tableChartInfo.AddCell(logo2);

                    //tableChart.AddCell(tableChartInfo);
                    //document.Add(tableChart);

                    //Paragraph count_heading = new Paragraph("List:", fBold14);
                    //count_heading.SpacingBefore = 3;
                    //count_heading.SpacingAfter = 3;
                    //document.Add(count_heading);

                    // ---------- Count Table ---------------------
                    //set table with Landscape (Rotate) 840x595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableTop = new PdfPTable(new[] { 60.0f, 80.0f, 80.0f, 60.0f, 60.0f, 60.0f, 195.0f });
                    tableTop.WidthPercentage = 100;
                    tableTop.HeaderRows = 1;
                    tableTop.SpacingBefore = 5;
                    tableTop.SpacingAfter = 5;

                    PdfPCell cell11 = new PdfPCell(new Phrase("Employee Code", fBold8));
                    cell11.BackgroundColor = Color.LIGHT_GRAY;
                    cell11.HorizontalAlignment = 1;
                    tableTop.AddCell(cell11);

                    PdfPCell cell12 = new PdfPCell(new Phrase("First Name", fBold8));
                    cell12.BackgroundColor = Color.LIGHT_GRAY;
                    cell12.HorizontalAlignment = 1;
                    tableTop.AddCell(cell12);

                    PdfPCell cell13 = new PdfPCell(new Phrase("Last Name", fBold8));
                    cell13.BackgroundColor = Color.LIGHT_GRAY;
                    cell13.HorizontalAlignment = 1;
                    tableTop.AddCell(cell13);

                    PdfPCell cell01 = new PdfPCell(new Phrase("From Date", fBold8));
                    cell01.BackgroundColor = Color.LIGHT_GRAY;
                    cell01.HorizontalAlignment = 1;
                    tableTop.AddCell(cell01);

                    PdfPCell cell02 = new PdfPCell(new Phrase("To Date", fBold8));
                    cell02.BackgroundColor = Color.LIGHT_GRAY;
                    cell02.HorizontalAlignment = 1;
                    tableTop.AddCell(cell02);

                    PdfPCell cell03 = new PdfPCell(new Phrase("Validity Status", fBold8));
                    cell03.BackgroundColor = Color.LIGHT_GRAY;
                    cell03.HorizontalAlignment = 1;
                    tableTop.AddCell(cell03);

                    PdfPCell cell04 = new PdfPCell(new Phrase("Validity Remarks", fBold8));
                    cell04.BackgroundColor = Color.LIGHT_GRAY;
                    cell04.HorizontalAlignment = 1;
                    tableTop.AddCell(cell04);

                    foreach (LeavesValidityReportLog log in list_count)
                    {
                        PdfPCell cellData11 = new PdfPCell(new Phrase(log.Employee_Code.ToString(), fNormal8));
                        cellData11.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData11);

                        PdfPCell cellData12 = new PdfPCell(new Phrase(log.First_Name.ToString(), fNormal8));
                        //cellData12.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData12);

                        PdfPCell cellData13 = new PdfPCell(new Phrase(log.Last_Name.ToString(), fNormal8));
                        //cellData13.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData13);

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.From_Date.ToString(), fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.To_Date.ToString(), fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.ValidityText.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.ValidityRemarks.ToString(), fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableTop.AddCell(cellData4);
                    }

                    if (list_count.Count > 0)
                    {
                        document.Add(tableTop);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Leaves-Validity-Report-" + DateTime.Now.ToString("ddMMMyyyy") + ".pdf");
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
        public ActionResult UpdateLeaveValidityApplication(ViewModels.LeaveApplicationInfo toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            LeaveApplicationResultSet.updateLeaveValidity(toUpdate);
            TimeTune.AuditTrail.update(json, "LeaveValidityStatusReport", User.Identity.Name);
            return Json(new { status = "success" });
        }


        ////////////////////////////////////////////////////////

        #endregion


        #region EmployeePerformanceReport


        [HttpGet]
        [ActionName("PerformanceReport")]
        public ActionResult PerformanceReport_Get()
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
        [ActionName("PerformanceReport")]
        [ValidateAntiForgeryToken]
        public ActionResult PerformanceReport_Post()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("PerformanceReport");


            var dtsource = TimeTune.Employee_Evaluation.get(employeeID);

            if (dtsource == null)
                return RedirectToAction("PerformanceReport");

            int found = 0; ViewData["PDFNoDataFound"] = "";
            BLL.PdfReports.EmployeeEvaluationPDF gb = new BLL.PdfReports.EmployeeEvaluationPDF();
            found = EmployeePerformance(dtsource);

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

        private int EmployeePerformance(ViewModels.EmployeeEvaluation sdata)
        {
            int reponse = 0;
            int gPersonality = sdata.personality, gCommunication = sdata.communicationSkills, gAttendance = sdata.attendancePromptness, gImitative = sdata.imitative, gOrganization = sdata.organizationAwareness, gSelf = sdata.selfControl;
            int sProficiency = sdata.proficiency, sProject = sdata.projectManagement, sAttention = sdata.attentionDetail, sClient = sdata.clientInteraction, sCreativity = sdata.creativity, sBusiness = sdata.businessSkill;

            string strPosition = sdata.postion, strRequirement = sdata.project, strPrimary = sdata.primaryResponsibilities, strSecondary = sdata.secondaryResponsibilities, strCareer = sdata.careerPath;

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

                    // ---------- Header Table ---------------------
                    //string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    //Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

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

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

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

                    PdfPCell cellEName = new PdfPCell(new Phrase("Name: " + sdata.EmpName, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + sdata.empCode, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Performance Evaluation", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    //////////document.Add(tableEmployee);

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
                    PdfPCell cellC01R01 = new PdfPCell(new Phrase("Employee Performance Evaluation", fBold14));
                    cellC01R01.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R01);


                    ////////////////////////

                    PdfPCell cellC01R02 = new PdfPCell(new Phrase("Employee:", fBold9));
                    cellC01R02.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02);

                    PdfPCell cellC01R02_Value = new PdfPCell(new Phrase(sdata.EmpName + "\n\n", fNormal9));
                    cellC01R02_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R02_Value);

                    ////////////////////////

                    PdfPCell cellC01R03 = new PdfPCell(new Phrase("Employee Code:", fBold9));
                    cellC01R03.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R03);

                    PdfPCell cellC01R03_Value = new PdfPCell(new Phrase(sdata.empCode + "\n\n", fNormal9));
                    cellC01R03_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R03_Value);

                    ////////////////////////
                    PdfPCell cellC01R04 = new PdfPCell(new Phrase("Review Period: ", fBold9));
                    cellC01R04.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R04);

                    PdfPCell cellC01R04_Value = new PdfPCell(new Phrase(sdata.reviewPeriod + "\n\n", fNormal9));
                    cellC01R04_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R04_Value);

                    ////////////////////////
                    PdfPCell cellC01R05 = new PdfPCell(new Phrase("1) Evaluate performance by circling the appropriate response:\n" +
                                                                         "\t\t\t\t\t\t1 = substandard, needs constant supervision\n" +
                                                                         "\t\t\t\t\t\t2 = below average, needs improvement\n" +
                                                                         "\t\t\t\t\t\t3 = average, satisfactorily meets criteria\n" +
                                                                         "\t\t\t\t\t\t4 = above average, exceeds criteria\n" +
                                                                         "\t\t\t\t\t\t5 = exemplary, deserving of unusual recognition\n\n" +
                                                                    "\t2) Enter comments as necessary.\n\n" +
                                                                    "\t3) Set goals for the next review period\n\n" +
                                                                    "\t4) Complete the back side (supervisor only).", fNormalItalic9));
                    cellC01R05.Border = 0;
                    //cellC01R05.MinimumHeight = 60.0f;
                    tableMiddleCol01.AddCell(cellC01R05);

                    ////////////////////////
                    PdfPCell cellC01R06 = new PdfPCell(new Phrase("\nGeneral Criteria", fBold14));
                    cellC01R06.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R06);

                    ////////////////////////
                    PdfPCell cellC01R07 = new PdfPCell(new Phrase("Personality / demeanor:", fBold9));
                    cellC01R07.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07);

                    PdfPCell cellC01R07_Text = new PdfPCell(new Phrase("Flexible and easy to get along with an adaptable team player.", fNormal9));
                    cellC01R07_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_Text);

                    Phrase phrGPersonality = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gPersonality > 0 && gPersonality < 6)
                    {
                        if (gPersonality == 1)
                            phrGPersonality = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 2)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 3)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 4)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gPersonality == 5)
                            phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGPersonality = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R07_Value = new PdfPCell(phrGPersonality);
                    cellC01R07_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R07_Value);

                    ////////////////////////
                    PdfPCell cellC01R08 = new PdfPCell(new Phrase("Communication Skills:", fBold9));
                    cellC01R08.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08);

                    PdfPCell cellC01R08_Text = new PdfPCell(new Phrase("Listens, understands and expresses him / herself well.", fNormal9));
                    cellC01R08_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08_Text);

                    Phrase phrGCommunication = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gCommunication > 0 && gCommunication < 6)
                    {
                        if (gCommunication == 1)
                            phrGCommunication = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 2)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 3)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 4)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gCommunication == 5)
                            phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGCommunication = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R08_Value = new PdfPCell(phrGCommunication);
                    cellC01R08_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R08_Value);

                    ////////////////////////
                    PdfPCell cellC01R09 = new PdfPCell(new Phrase("Attendance and promptness:", fBold9));
                    cellC01R09.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09);

                    PdfPCell cellC01R09_Text = new PdfPCell(new Phrase("Observes assigned working hours, is conscientious.", fNormal9));
                    cellC01R09_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09_Text);

                    Phrase phrGAttendance = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gAttendance > 0 && gAttendance < 6)
                    {
                        if (gAttendance == 1)
                            phrGAttendance = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 2)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 3)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 4)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gAttendance == 5)
                            phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGAttendance = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R09_Value = new PdfPCell(phrGAttendance);
                    cellC01R09_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R09_Value);

                    ////////////////////////
                    PdfPCell cellC01R10 = new PdfPCell(new Phrase("Imitative:", fBold9));
                    cellC01R10.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10);

                    PdfPCell cellC01R10_Text = new PdfPCell(new Phrase("Works without close supervision, initiates independent action.", fNormal9));
                    cellC01R10_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10_Text);

                    Phrase phrGImitative = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gImitative > 0 && gImitative < 6)
                    {
                        if (gImitative == 1)
                            phrGImitative = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 2)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 3)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 4)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gImitative == 5)
                            phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGImitative = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R10_Value = new PdfPCell(phrGImitative);
                    cellC01R10_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R10_Value);

                    ////////////////////////
                    PdfPCell cellC01R11 = new PdfPCell(new Phrase("Organization and time-awareness:", fBold9));
                    cellC01R11.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11);

                    PdfPCell cellC01R11_Text = new PdfPCell(new Phrase("Sets and observes own priorities for the best use of his/her time.", fNormal9));
                    cellC01R11_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_Text);

                    Phrase phrGOrganization = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gOrganization > 0 && gOrganization < 6)
                    {
                        if (gOrganization == 1)
                            phrGOrganization = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 2)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 3)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 4)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gOrganization == 5)
                            phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGOrganization = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R11_Value = new PdfPCell(phrGOrganization);
                    cellC01R11_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R11_Value);

                    ////////////////////////
                    PdfPCell cellC01R12 = new PdfPCell(new Phrase("Self-Control:", fBold9));
                    cellC01R12.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R12);

                    PdfPCell cellC01R12_Text = new PdfPCell(new Phrase("Maintains composure and performs well under pressure.", fNormal9));
                    cellC01R12_Text.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R12_Text);

                    Phrase phrGSelf = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (gSelf > 0 && gSelf < 6)
                    {
                        if (gSelf == 1)
                            phrGSelf = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 2)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 3)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 4)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (gSelf == 5)
                            phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrGSelf = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC01R12_Value = new PdfPCell(phrGSelf);
                    cellC01R12_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R12_Value);

                    ////////////////////////
                    PdfPCell cellC01R13 = new PdfPCell(new Phrase("\nComments", fBold14));
                    cellC01R13.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R13);

                    ////////////////////////
                    PdfPCell cellC01R14 = new PdfPCell(new Phrase("Employee’s major strength:", fBold9));
                    cellC01R14.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14);

                    PdfPCell cellC01R14_Value = new PdfPCell(new Phrase(sdata.majorStrength + "\n\n\n", fNormal9));
                    cellC01R14_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R14_Value);

                    ////////////////////////
                    PdfPCell cellC01R15 = new PdfPCell(new Phrase("Area needing most improvement:", fBold9));
                    cellC01R15.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15);

                    PdfPCell cellC01R15_Value = new PdfPCell(new Phrase(sdata.areaImprovement + "\n\n\n", fNormal9));
                    cellC01R15_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R15_Value);

                    ////////////////////////
                    PdfPCell cellC01R16 = new PdfPCell(new Phrase("Other Comments:", fBold9));
                    cellC01R16.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16);

                    PdfPCell cellC01R16_Value = new PdfPCell(new Phrase(sdata.otherComment + "\n\n\n", fNormal9));
                    cellC01R16_Value.Border = 0;
                    tableMiddleCol01.AddCell(cellC01R16_Value);




                    // --------------------------------------------- Middle Table - Column 2 ---------------------------------------------------------
                    PdfPTable tableMiddleCol02 = new PdfPTable(1);
                    tableMiddleCol02.WidthPercentage = 100;
                    tableMiddleCol02.HeaderRows = 0;
                    //tableMiddleCol02.SpacingBefore = 50;
                    tableMiddleCol02.SpacingAfter = 3;
                    tableMiddleCol02.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellC02R01 = new PdfPCell(new Phrase("Position:", fBold9));
                    cellC02R01.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R01);

                    Phrase phrPosition = null;
                    if (strPosition.Length > 0)
                    {
                        phrPosition = new Phrase(strPosition, fNormal9);
                    }
                    else
                    {
                        phrPosition = new Phrase("_______________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R01_Value = new PdfPCell(phrPosition);
                    cellC02R01_Value.Border = 0;
                    cellC02R01_Value.MinimumHeight = 30.0f;
                    tableMiddleCol02.AddCell(cellC02R01_Value);

                    ////////////////////////
                    PdfPCell cellC02R02 = new PdfPCell(new Phrase("Requirements/attribute:", fBold9));
                    cellC02R02.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R02);

                    Phrase phrRequirement = null;
                    if (strRequirement.Length > 0)
                    {
                        phrRequirement = new Phrase(strRequirement, fNormal9);
                    }
                    else
                    {
                        phrRequirement = new Phrase("_______________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R02_Value = new PdfPCell(phrRequirement);
                    cellC02R02_Value.Border = 0;
                    cellC02R02_Value.MinimumHeight = 30.0f;
                    tableMiddleCol02.AddCell(cellC02R02_Value);

                    ////////////////////////
                    PdfPCell cellC02R03 = new PdfPCell(new Phrase("Primary resposibilities:", fBold9));
                    cellC02R03.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R03);

                    Phrase phrPrimary = null;
                    if (strPrimary.Length > 0)
                    {
                        phrPrimary = new Phrase(strPrimary, fNormal9);
                    }
                    else
                    {
                        phrPrimary = new Phrase("__________________________________________________________________________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R03_Value = new PdfPCell(phrPrimary);
                    cellC02R03_Value.Border = 0;
                    cellC02R03_Value.MinimumHeight = 40.0f;
                    tableMiddleCol02.AddCell(cellC02R03_Value);

                    ////////////////////////
                    PdfPCell cellC02R04 = new PdfPCell(new Phrase("Secondary resposibilities:", fBold9));
                    cellC02R04.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R04);

                    Phrase phrSecondary = null;
                    if (strSecondary.Length > 0)
                    {
                        phrSecondary = new Phrase(strSecondary, fNormal9);
                    }
                    else
                    {
                        phrSecondary = new Phrase("__________________________________________________________________________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R04_Value = new PdfPCell(phrSecondary);
                    cellC02R04_Value.Border = 0;
                    cellC02R04_Value.MinimumHeight = 40.0f;
                    tableMiddleCol02.AddCell(cellC02R04_Value);

                    ////////////////////////
                    PdfPCell cellC02R05 = new PdfPCell(new Phrase("Career path:", fBold9));
                    cellC02R05.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R05);

                    Phrase phrCareer = null;
                    if (strCareer.Length > 0)
                    {
                        phrCareer = new Phrase(strCareer, fNormal9);
                    }
                    else
                    {
                        phrCareer = new Phrase("_______________________________________________________________________________________________________", fNormal9);
                    }
                    PdfPCell cellC02R05_Value = new PdfPCell(phrCareer);
                    cellC02R05_Value.Border = 0;
                    cellC02R05_Value.MinimumHeight = 30.0f;
                    tableMiddleCol02.AddCell(cellC02R05_Value);

                    ////////////////////////
                    PdfPCell cellC02R06 = new PdfPCell(new Phrase("\nPosition-Specific Criteria", fBold14));
                    cellC02R06.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R06);

                    ////////////////////////
                    PdfPCell cellC02R07 = new PdfPCell(new Phrase("Proficiency:", fBold9));
                    cellC02R07.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07);

                    PdfPCell cellC02R07_Text = new PdfPCell(new Phrase("Understands craft, systems and processes.", fNormal9));
                    cellC02R07_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07_Text);

                    Phrase phrSProficiency = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sProficiency > 0 && sProficiency < 6)
                    {
                        if (sProficiency == 1)
                            phrSProficiency = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 2)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 3)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 4)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProficiency == 5)
                            phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSProficiency = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R07_Value = new PdfPCell(phrSProficiency);
                    cellC02R07_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R07_Value);

                    ////////////////////////
                    PdfPCell cellC02R08 = new PdfPCell(new Phrase("Project Management:", fBold9));
                    cellC02R08.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R08);

                    PdfPCell cellC02R08_Text = new PdfPCell(new Phrase("Organizes tasks and assignments.", fNormal9));
                    cellC02R08_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC01R08_Text);

                    Phrase phrSProject = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sProject > 0 && sProject < 6)
                    {
                        if (sProject == 1)
                            phrSProject = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 2)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 3)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 4)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sProject == 5)
                            phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSProject = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R08_Value = new PdfPCell(phrSProject);
                    cellC02R08_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R08_Value);

                    ////////////////////////
                    PdfPCell cellC02R09 = new PdfPCell(new Phrase("Attention to detail:", fBold9));
                    cellC02R09.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09);

                    PdfPCell cellC02R09_Text = new PdfPCell(new Phrase("Attentive to all aspects of assignments / workflow.", fNormal9));
                    cellC02R09_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09_Text);

                    Phrase phrSAttention = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sAttention > 0 && sAttention < 6)
                    {
                        if (sAttention == 1)
                            phrSAttention = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 2)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 3)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 4)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sAttention == 5)
                            phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSAttention = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R09_Value = new PdfPCell(phrSAttention);
                    cellC02R09_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R09_Value);

                    ////////////////////////
                    PdfPCell cellC02R10 = new PdfPCell(new Phrase("Client Interaction:", fBold9));
                    cellC02R10.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10);

                    PdfPCell cellC02R10_Text = new PdfPCell(new Phrase("Relates to client needs, both spoken and unspoken.", fNormal9));
                    cellC02R10_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10_Text);

                    Phrase phrSClient = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sClient > 0 && sClient < 6)
                    {
                        if (sClient == 1)
                            phrSClient = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 2)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 3)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 4)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sClient == 5)
                            phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSClient = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R10_Value = new PdfPCell(phrSClient);
                    cellC02R10_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R10_Value);

                    ////////////////////////
                    PdfPCell cellC02R11 = new PdfPCell(new Phrase("Creativity:", fBold9));
                    cellC02R11.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11);

                    PdfPCell cellC02R11_Text = new PdfPCell(new Phrase("Seeks innovative solutions.", fNormal9));
                    cellC02R11_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11_Text);

                    Phrase phrSCreativity = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sCreativity > 0 && sCreativity < 6)
                    {
                        if (sCreativity == 1)
                            phrSCreativity = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 2)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 3)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 4)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sCreativity == 5)
                            phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSCreativity = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R11_Value = new PdfPCell(phrSCreativity);
                    cellC02R11_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R11_Value);

                    ////////////////////////
                    PdfPCell cellC02R12 = new PdfPCell(new Phrase("Business skills:", fBold9));
                    cellC02R12.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12);

                    PdfPCell cellC02R12_Text = new PdfPCell(new Phrase("Understands and works to increase profitability.", fNormal9));
                    cellC02R12_Text.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_Text);
                    //////////

                    ////////

                    Phrase phrSBusiness = null; // new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    if (sBusiness > 0 && sBusiness < 6)
                    {
                        if (sBusiness == 1)
                            phrSBusiness = new Phrase("1*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 2)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 3)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 4)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4*\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                        else if (sBusiness == 5)
                            phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5*" + "\n\n", fBold9);
                    }
                    else
                    {
                        phrSBusiness = new Phrase("1\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t2\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t3\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t4\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t5" + "\n\n", fBold9);
                    }
                    PdfPCell cellC02R12_Value = new PdfPCell(phrSBusiness);
                    cellC02R12_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R12_Value);

                    ////////////////////////
                    PdfPCell cellC02R13 = new PdfPCell(new Phrase("\nGoals", fBold14));
                    cellC02R13.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R13);

                    ////////////////////////
                    PdfPCell cellC02R14 = new PdfPCell(new Phrase("Detail:", fBold9));
                    cellC02R14.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14);

                    PdfPCell cellC02R14_Value = new PdfPCell(new Phrase(sdata.goal + "\n\n\n", fNormal9));
                    cellC02R14_Value.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R14_Value);

                    ////////////////////////
                    PdfPCell cellC02R15 = new PdfPCell(new Phrase("I have been shown this evaluation. My signature below does not necessarily imply agreement:\n\n", fBold9));
                    cellC02R15.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R15);

                    PdfPCell cellC02R15_Value = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(Employee's Signature / date)", fNormalItalic9));
                    cellC02R15_Value.Border = 0;
                    cellC02R15_Value.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R15_Value);

                    ////////////////////////
                    PdfPCell cellC02R16 = new PdfPCell(new Phrase("\n\nScheduled date of next evaluation:\n\n", fBold9));
                    cellC02R16.Border = 0;
                    tableMiddleCol02.AddCell(cellC02R16);

                    PdfPCell cellC02R16_Value = new PdfPCell(new Phrase("__________________________________________________\n\t\t\t\t\t\t\t\t\t\t\t\t(Supervisor's Signature / date)", fNormalItalic9));
                    cellC02R16_Value.Border = 0;
                    cellC02R16_Value.MinimumHeight = 10.0f;
                    tableMiddleCol02.AddCell(cellC02R16_Value);

                    ////////////////////////////////////////////////////////////////////////////////////////

                    //add 2 tables to MAIN table
                    tableMiddle.AddCell(tableMiddleCol01);
                    tableMiddle.AddCell(tableMiddleCol02);

                    document.Add(tableMiddle);

                    ///////////////////////////////////////////////////////////////////////////////////////

                    // ---------- End Table ---------------------
                    Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                    p_nsig.SpacingBefore = 1;
                    //p_nsig.SpacingAfter = 3;
                    //////document.Add(p_nsig);

                    // ------------- close PDF Document and download it automatically



                    document.Close();
                    writer.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=EmpPerformance-Report-" + sdata.empCode + "-" + sdata.reviewPeriod.Replace(" ", "") + ".pdf");
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
        public JsonResult EmpPerformanceReportDataHandler(MonthlyTimesheetReportTable param)
        {
            try
            {

                var data = new List<ViewModels.EmployeeEvaluation>();

                // get all employee view models
                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                int empID = Convert.ToInt32(param.employee_id);
                data = TimeTune.Employee_Evaluation.get2(empID);


                DTResult<EmployeeEvaluation> result = new DTResult<EmployeeEvaluation>
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
        }


        [HttpPost]
        public JsonResult CalculatePerfomance(MonthlyTimesheetReportTable param)
        {

            var data = new List<ViewModels.EmployeeEvaluation>();
            try
            {
                float cal = 0;
                if (param.employee_id != null && param.employee_id.ToString() != "")
                {
                    int empID = Convert.ToInt32(param.employee_id);
                    data = TimeTune.Employee_Evaluation.get2(empID);

                    if (data != null || data.Count > 0)
                    {
                        data[0].total = data[0].personality + data[0].communicationSkills + data[0].attendancePromptness + data[0].imitative + data[0].organizationAwareness + data[0].selfControl + data[0].proficiency + data[0].projectManagement + data[0].attentionDetail + data[0].clientInteraction + data[0].creativity + data[0].businessSkill + data[0].achievement;

                        cal = data[0].total / 65 * 5;
                        if (cal >= 4.60 && cal <= 5.00)
                        {
                            data[0].result = "EXCELLENT";
                        }
                        else if (cal >= 4.00 && cal <= 4.59)
                        {
                            data[0].result = "VERY GOOD";
                        }
                        else if (cal >= 2.80 && cal <= 3.99)
                        {
                            data[0].result = "GOOD";
                        }
                        else if (cal >= 1.60 && cal <= 2.79)
                        {
                            data[0].result = "SATISFACTORY";
                        }
                        else
                        {
                            data[0].result = "POOR";
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }

            return Json(data);
        }




        #endregion

        #region Department Attendance Report

        public class DepartmentAttendanceReportTable : DTParameters
        {
            public string department_id { get; set; }
            public string designation_id { get; set; }
            public string function_id { get; set; }
            public string region_id { get; set; }
            public string location_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public string imageData { get; set; }
        }

        [HttpPost]
        public JsonResult DepartmentAttendanceReportHandler(DepartmentAttendanceReportTable param)
        {
            decimal sumAll = 0;
            decimal sumPresent = 0, sumLeave = 0, sumAbsent = 0, sumOnTime = 0, sumLate = 0, sumOutTime = 0, sumEarly = 0;
            decimal perPresent = 0, perLeave = 0, perAbsent = 0, perOnTime = 0, perLate = 0, perOutTime = 0, perEarly = 0;
            //string str = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month).ToString() + DateTime.Now.AddMonths(-1).ToString("-MM-yyyy") + 
            string strDeptName = "", strDesgName = "", strLoctName = "", strRegnName = "", strFuncName = "";
            try
            {
                // var data = new List<DepartmentAttendanceCountReport>();
                var data = TimeTune.Reports.getAllDepartmentAttendanceReport(param.function_id, param.region_id, param.department_id, param.designation_id, param.location_id, param.from_date, param.to_date);

                List<DepartmentAttendanceCountReport> dataResult = DepartmentAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data.dataSet);
                int countResult = DepartmentAttendanceResultSet.Count(param.Search.Value, data.dataSet);

                if (countResult > 0)
                {
                    int a = 0;
                    foreach (var i in dataResult)
                    {
                        sumPresent += i.PresentPercent;
                        sumLeave += i.LeavePercent;
                        sumAbsent += i.AbsentPercent;

                        sumOnTime += i.OnTimePercent;
                        sumLate += i.LatePercent;

                        sumOutTime += i.OutTimePercent;
                        sumEarly += i.EarlyPercent;

                        if (a == 0)
                        {
                            if (param.department_id == null)
                                strDeptName = "-1";
                            else
                                strDeptName = i.DeptName;

                            if (param.designation_id == null)
                                strDesgName = "-1";
                            else
                                strDesgName = i.DesgName;

                            if (param.region_id == null)
                                strRegnName = "-1";
                            else
                                strRegnName = i.RegnName;

                            if (param.function_id == null)
                                strFuncName = "-1";
                            else
                                strFuncName = i.FuncName;

                            if (param.location_id == null)
                                strLoctName = "-1";
                            else
                                strLoctName = i.LoctName;

                            a++;
                        }
                    }

                    sumAll = sumPresent + sumLeave + sumAbsent;
                    sumAll = sumAll == 0 ? 1 : sumAll;
                    perPresent = sumPresent / sumAll * 100;
                    perLeave = sumLeave / sumAll * 100;
                    perAbsent = sumAbsent / sumAll * 100;

                    sumAll = sumOnTime + sumLate;
                    sumAll = sumAll == 0 ? 1 : sumAll;
                    perOnTime = sumOnTime / sumAll * 100;
                    perLate = sumLate / sumAll * 100;

                    sumAll = sumOutTime + sumEarly;
                    sumAll = sumAll == 0 ? 1 : sumAll;
                    perOutTime = sumOutTime / sumAll * 100;
                    perEarly = sumEarly / sumAll * 100;
                }

                strFuncName = param.function_id ?? "-1"; strRegnName = param.region_id ?? "-1"; strDeptName = param.department_id ?? "-1";
                strDesgName = param.designation_id ?? "-1"; strLoctName = param.location_id ?? "-1";
                ViewModel.GlobalVariables.GV_DepartmentAttendanceNames = "D=" + strDeptName + ",S=" + strDesgName + ",C=" + strLoctName + ",R=" + strRegnName + ",F=" + strFuncName;

                ViewModel.GlobalVariables.GV_DepartmentAttendancePercent = "P=" + perPresent + ",L=" + perLeave + ",A=" + perAbsent + ",O=" + perOnTime + ",T=" + perLate + ",U=" + perOutTime + ",E=" + perEarly;

                DTResult<ViewModels.DepartmentAttendanceCountReport> result = new DTResult<ViewModels.DepartmentAttendanceCountReport>
                {
                    draw = param.Draw,
                    data = dataResult,
                    recordsFiltered = countResult,
                    recordsTotal = countResult
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
        public string DepartmentAttendancePercentage()
        {
            return ViewModel.GlobalVariables.GV_DepartmentAttendancePercent;
        }

        [HttpGet]
        [ActionName("DepartmentAttendanceReport")]
        public ActionResult DepartmentAttendanceReport_Get()
        {
            ViewData["PDFNoDataFoundDept"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("DepartmentAttendanceReport")]
        //[ValidateAntiForgeryToken]
        public ActionResult DepartmentAttendanceReport_Post(DepartmentAttendanceReportTable param)
        {
            try
            {

                //TimeTune.Reports.Depart_Attendance_ReportLog data = new TimeTune.Reports.Depart_Attendance_ReportLog();
                var data = TimeTune.Reports.getAllDepartmentAttendanceReport(param.function_id, param.region_id, param.department_id, param.designation_id, param.location_id, param.from_date, param.to_date);

                if (data != null)
                {
                    DepartmentAttendancePDF(data, param.imageData, param.from_date, param.to_date);
                    ViewData["PDFNoDataFoundDept"] = "";
                    //return null;
                }
                else
                {
                    ViewData["PDFNoDataFoundDept"] = "No Data Found";
                }

                return View("DepartmentReport");
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private int DepartmentAttendancePDF(TimeTune.Reports.Depart_Attendance_ReportLog sdata, string imageData, string fromDate, string toDate)
        {
            int reponse = 0;

            try
            {
                DateTime FDate = DateTime.Now; DateTime TDate = DateTime.Now;
                if (fromDate != null && fromDate != "")
                    FDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                if (toDate != null && toDate != "")
                    TDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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

                    Font fBold11 = FontFactory.GetFont("HELVETICA", 11, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    ////////////////----------CHART RENDERING------------------////////////
                    //string _image = imageData.Replace("data:image/png;base64,", "");

                    //var base64Data = Regex.Match(_image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);

                    //byte[] bytes = Convert.FromBase64String(_image);

                    ////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                    //System.Drawing.Image img = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
                    //Image logo2 = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);


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

                    //Charts
                    //https://www.aspsnippets.com/Articles/Export-Chart-to-PDF-in-ASPNet-MVC.aspx
                    string strChartData = "";
                    strChartData = ViewModel.GlobalVariables.GV_DepartmentAttendancePercent; //"P=15.35,L=20.00,A=64.65,O=44.02,T=42.54,E=13.44";
                    strChartData = strChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("U=", "").Replace("E=", "");

                    byte[] bytesArrPrsChart = CreateVisualChart(0, strChartData); // GetPresenceChartData(); //PopulateChart();
                    iTextSharp.text.Image prsChart = iTextSharp.text.Image.GetInstance(bytesArrPrsChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    byte[] bytesArrPunChart = CreateVisualChart(1, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    iTextSharp.text.Image punChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    byte[] bytesArrExitChart = CreateVisualChart(2, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    iTextSharp.text.Image exitChart = iTextSharp.text.Image.GetInstance(bytesArrExitChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //PdfPTable tableCharts = new PdfPTable(new[] { 50.0f, 200.0f, 95.0f, 200.0f, 50.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    PdfPTable tableCharts = new PdfPTable(new[] { 100.0f, 300.0f, 40.0f, 300.0f, 100.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableCharts.WidthPercentage = 100;
                    tableCharts.HeaderRows = 0;
                    //tableCharts.SpacingBefore = 50;
                    tableCharts.SpacingAfter = 3;
                    tableCharts.DefaultCell.Border = Rectangle.NO_BORDER;

                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(prsChart);
                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");

                    tableCharts.AddCell("");
                    tableCharts.AddCell(prsChart);
                    tableCharts.AddCell("");
                    tableCharts.AddCell(punChart);
                    tableCharts.AddCell("");

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

                    ////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 285, 90.0f });//total 595 - 10 x 2 due to Left and Right side margin
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


                    PdfPCell cellEYear = new PdfPCell(new Phrase("Period: " + " " + FDate.ToString("dd-MMM-yyyy") + " to " + TDate.ToString("dd-MMM-yyyy"), fBold9));
                    cellEYear.Border = 0;
                    tableEInfo.AddCell(cellEYear);

                    string strFormFields = ViewModel.GlobalVariables.GV_DepartmentAttendanceNames;
                    strFormFields = strFormFields.Replace("D=", "").Replace("S=", "").Replace("C=", "").Replace("R=", "").Replace("F=", "");
                    string[] strSplit = strFormFields.Split(',');

                    foreach (DepartmentAttendanceCountReport log in sdata.dataSet)
                    {
                        if (strSplit[0] != "-1") //(log.DeptName != "-1")
                        {
                            PdfPCell cellDeptName = new PdfPCell(new Phrase("Department: " + log.DeptName, fBold9));
                            cellDeptName.Border = 0;
                            tableEInfo.AddCell(cellDeptName);
                        }

                        if (strSplit[1] != "-1") //if (log.DesgName != "-1")
                        {
                            PdfPCell cellDesgName = new PdfPCell(new Phrase("Designation: " + log.DesgName, fBold9));
                            cellDesgName.Border = 0;
                            tableEInfo.AddCell(cellDesgName);
                        }

                        if (strSplit[2] != "-1") //if (log.LoctName != "-1")
                        {
                            PdfPCell cellLoctName = new PdfPCell(new Phrase("Location: " + log.LoctName, fBold9));
                            cellLoctName.Border = 0;
                            tableEInfo.AddCell(cellLoctName);
                        }

                        if (strSplit[3] != "-1") //if (strSplit[0] == "-1") //if (log.RegnName != "-1")
                        {
                            PdfPCell cellRegnName = new PdfPCell(new Phrase("Group: " + log.RegnName, fBold9));
                            cellRegnName.Border = 0;
                            tableEInfo.AddCell(cellRegnName);
                        }

                        if (strSplit[4] != "-1") //if (log.FuncName != "-1")
                        {
                            PdfPCell cellFuncName = new PdfPCell(new Phrase("Function: " + log.FuncName, fBold9));
                            cellFuncName.Border = 0;
                            tableEInfo.AddCell(cellFuncName);
                        }


                        break;
                    }

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Organization Attendance Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;


                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    document.Add(tableCharts);

                    //document.NewPage();

                    //PdfPTable tableChart = new PdfPTable(new[] { 200.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChart.WidthPercentage = 100;
                    //tableChart.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50f;
                    //tableEmployee.SpacingAfter = 10f;
                    //tableChart.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPTable tableChartInfo = new PdfPTable(1);
                    //tableChartInfo.WidthPercentage = 100;
                    //tableChartInfo.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableChartInfo.SpacingAfter = 3;
                    //tableChartInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellEChart = new PdfPCell();
                    //cellEChart.Border = 0;
                    //tableChartInfo.AddCell(logo2);


                    //tableChart.AddCell(tableChartInfo);
                    //document.Add(tableChart);

                    //Headering Colspan table
                    PdfPTable tableHeaderSpan = new PdfPTable(new[] { 470.0f, 150.0f, 110.0f, 110.0f });
                    tableHeaderSpan.WidthPercentage = 100;
                    tableHeaderSpan.HeaderRows = 0;
                    //tableHeaderSpan.SpacingBefore = 3;
                    //tableHeaderSpan.SpacingAfter = 1;

                    PdfPCell cellHS1 = new PdfPCell(new Phrase("", fBold11));
                    //cellHS1.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS1.HorizontalAlignment = 1;
                    cellHS1.Border = 0;
                    tableHeaderSpan.AddCell(cellHS1);

                    PdfPCell cellHS2 = new PdfPCell(new Phrase("Presence Status", fBold11));
                    cellHS2.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS2.HorizontalAlignment = 1;
                    tableHeaderSpan.AddCell(cellHS2);

                    PdfPCell cellHS3 = new PdfPCell(new Phrase("Punctuality Status", fBold11));
                    cellHS3.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS3.HorizontalAlignment = 1;
                    tableHeaderSpan.AddCell(cellHS3);

                    PdfPCell cellHS4 = new PdfPCell(new Phrase("Exit Status", fBold11));
                    cellHS4.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS4.HorizontalAlignment = 1;
                    tableHeaderSpan.AddCell(cellHS4);


                    // ---------- Middle Table ---------------------
                    PdfPTable tableMid = null;

                    bool deptVisb = false, desgVisb = false, loctVisb = false;
                    foreach (DepartmentAttendanceCountReport log in sdata.dataSet)
                    {
                        if (log.DeptVisb && log.DesgVisb && log.LoctVisb) //v1          1       1       1
                        {
                            deptVisb = true; desgVisb = true; loctVisb = true;
                            tableMid = new PdfPTable(new[] { 50.0f, 105.0f, 105.0f, 105.0f, 105.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-0=12
                        }
                        else if (log.DeptVisb && log.DesgVisb && !log.LoctVisb)//v2     1       1       0
                        {
                            deptVisb = true; desgVisb = true; loctVisb = false;
                            tableMid = new PdfPTable(new[] { 50.0f, 110.0f, 155.0f, 155.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-1=11
                        }
                        else if (log.DeptVisb && !log.DesgVisb && log.LoctVisb)//v3     1       0       1
                        {
                            deptVisb = true; desgVisb = false; loctVisb = true;
                            tableMid = new PdfPTable(new[] { 50.0f, 110.0f, 155.0f, 155.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-1=11
                        }
                        else if (log.DeptVisb && !log.DesgVisb && !log.LoctVisb)//v4    1       0       0
                        {
                            deptVisb = true; desgVisb = false; loctVisb = false;
                            tableMid = new PdfPTable(new[] { 50.0f, 210.0f, 210.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-2=10
                        }
                        else if (!log.DeptVisb && log.DesgVisb && log.LoctVisb)//v5     0       1       1
                        {
                            deptVisb = false; desgVisb = true; loctVisb = true;
                            tableMid = new PdfPTable(new[] { 50.0f, 110.0f, 155.0f, 155.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-1=11
                        }
                        else if (!log.DeptVisb && log.DesgVisb && !log.LoctVisb)//v6    0       1       0
                        {
                            deptVisb = false; desgVisb = true; loctVisb = false;
                            tableMid = new PdfPTable(new[] { 50.0f, 210.0f, 210.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-2=10
                        }
                        else if (!log.DeptVisb && !log.DesgVisb && log.LoctVisb)//v7    0       0       1
                        {
                            deptVisb = false; desgVisb = false; loctVisb = true;
                            tableMid = new PdfPTable(new[] { 50.0f, 210.0f, 210.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-2=10
                        }
                        else if (!log.DeptVisb && !log.DesgVisb && !log.LoctVisb)//v8    0       0       0
                        {
                            deptVisb = false; desgVisb = false; loctVisb = false;
                            tableMid = new PdfPTable(new[] { 50.0f, 420.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-3=09
                        }
                        else
                        {
                            deptVisb = false; desgVisb = false; loctVisb = false;
                            tableMid = new PdfPTable(new[] { 50.0f, 420.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });//12-3=09
                        }

                        break;
                    }

                    //set table with Landscape (Rotate) 840x595 pixels width - subtract 10x2 from either sides border
                    ////tableMid = new PdfPTable(new[] { 50.0f, 105.0f, 105.0f, 105.0f, 105.0f, 50.0f, 50.0f, 50.0f, 60.0f, 50.0f, 50.0f, 60.0f });
                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    //tableMid.SpacingBefore = 3;
                    //tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("ECode", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    if (deptVisb)
                    {
                        PdfPCell cell21 = new PdfPCell(new Phrase("Department", fBold8));
                        cell21.BackgroundColor = Color.LIGHT_GRAY;
                        cell21.HorizontalAlignment = 1;
                        tableMid.AddCell(cell21);
                    }

                    if (desgVisb)
                    {
                        PdfPCell cell23 = new PdfPCell(new Phrase("Designation", fBold8));
                        cell23.BackgroundColor = Color.LIGHT_GRAY;
                        cell23.HorizontalAlignment = 1;
                        tableMid.AddCell(cell23);
                    }

                    if (loctVisb)
                    {
                        PdfPCell cell22 = new PdfPCell(new Phrase("Location", fBold8));
                        cell22.BackgroundColor = Color.LIGHT_GRAY;
                        cell22.HorizontalAlignment = 1;
                        tableMid.AddCell(cell22);
                    }

                    PdfPCell cell3 = new PdfPCell(new Phrase("Present (%)", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Leave (%)", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Absent (%)", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("On Time (%)", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Late (%)", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Exit (%)", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    PdfPCell cell9 = new PdfPCell(new Phrase("Early Out (%)", fBold8));
                    cell9.BackgroundColor = Color.LIGHT_GRAY;
                    cell9.HorizontalAlignment = 1;
                    tableMid.AddCell(cell9);


                    foreach (DepartmentAttendanceCountReport log in sdata.dataSet)
                    {
                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.EmployeeCode.ToString(), fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.FirstName + " " + log.LastName, fNormal8));
                        //cellData2.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData2);

                        if (log.DeptVisb)
                        {
                            PdfPCell cellData21 = new PdfPCell(new Phrase(log.DeptName, fNormal8));
                            //cellData21.HorizontalAlignment = 0;
                            tableMid.AddCell(cellData21);
                        }

                        if (log.DesgVisb)
                        {
                            PdfPCell cellData23 = new PdfPCell(new Phrase(log.DesgName, fNormal8));
                            //cellData23.HorizontalAlignment = 0;
                            tableMid.AddCell(cellData23);
                        }

                        if (log.LoctVisb)
                        {
                            PdfPCell cellData22 = new PdfPCell(new Phrase(log.LoctName, fNormal8));
                            //cellData22.HorizontalAlignment = 0;
                            tableMid.AddCell(cellData22);
                        }


                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.PresentPercent.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.LeavePercent.ToString(), fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.AbsentPercent.ToString(), fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.OnTimePercent.ToString(), fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.LatePercent.ToString(), fNormal8));
                        cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.OutTimePercent.ToString(), fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);

                        PdfPCell cellData9 = new PdfPCell(new Phrase(log.EarlyPercent.ToString(), fNormal8));
                        cellData9.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData9);
                    }


                    if (sdata.dataSet.Count > 0)
                    {
                        document.Add(tableHeaderSpan);
                        document.Add(tableMid);




                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        string dateRange = fromDate + " " + toDate;

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Org-Attendance-Report-" + FDate.ToString("dd-MMM-yyyy") + ".pdf");
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

        #region Department Performance Report

        [HttpGet]
        [ActionName("DepartmentPerformanceReport")]
        public ActionResult DepartmentPerformanceReport_Get()
        {
            //ViewBag.ImageSource = CreateChart();

            return View();
        }

        public class CustomDTParameters2 : DTParameters
        {
            public string reviewPeriod { get; set; }
            public string to_date { get; set; }
            public string imageData { get; set; }
        }

        [HttpPost]
        [ActionName("DepartmentPerformanceReport")]
        public ActionResult DepartmentPerformanceReport_Post(CustomDTParameters2 param)
        {
            try
            {
                var dtsource = TimeTune._DeptPerformanceReport.getDeptartmentPerformance(param.reviewPeriod);
                //DepartmentPerformancePDF(dtsource, param.imageData);

                if (dtsource != null)
                {
                    DepartmentPerformancePDF(dtsource, param.imageData);
                    ViewBag.DeptPerfMessage = "";


                }
                else
                {
                    ViewBag.DeptPerfMessage = "NO";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return View();
        }

        public JsonResult DeptPerformanceReportDataHandler(CustomDTParameters2 param)
        {
            try
            {

                // bit[] base64 = Request.Form["chart2"];
                var dtsource = new Depart_Perfromance_ReportLog();
                dtsource = TimeTune._DeptPerformanceReport.getDeptartmentPerformance(param.reviewPeriod);

                if (dtsource.dataSet == null)
                    return Json("No Data to Show");

                List<ViewModels.Dept_Per_Rept> data = DeptPerReportSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource.dataSet);

                int count = DeptPerReportSet.Count(param.Search.Value, dtsource.dataSet);

                int cntPresent = 0, cntLeave = 0, cntAbsent = 0, cntOnTime = 0, cntLate = 0, cntOutTime = 0, cntEarlyOut = 0;
                decimal sumAll = 0;
                decimal sumPresent = 0, sumLeave = 0, sumAbsent = 0;
                decimal sumOnTime = 0, sumLate = 0, sumOutTime = 0, sumEarly = 0;
                decimal perPresent = 0, perLeave = 0, perAbsent = 0, perOnTime = 0, perLate = 0, perOutTime = 0, perEarly = 0;

                if (count > 0)
                {
                    foreach (var i in data)
                    {
                        sumPresent += i.PresentCount;
                        sumLeave += i.LeaveCount;
                        sumAbsent += i.AbsentCount;

                        sumOnTime += i.OnTimeCount;
                        sumLate += i.LateCount;

                        sumOutTime += i.OutTimeCount;
                        sumEarly += i.EarlyCount;

                        //-------------------------

                        cntPresent += i.PresentCount;
                        cntLeave += i.LeaveCount;
                        cntAbsent += i.AbsentCount;

                        cntOnTime += i.OnTimeCount;
                        cntLate += i.LateCount;

                        cntOutTime += i.OutTimeCount;
                        cntEarlyOut += i.EarlyCount;
                    }

                    sumAll = sumPresent + sumLeave + sumAbsent;
                    sumAll = sumAll == 0 ? 1 : sumAll;
                    perPresent = sumPresent / sumAll * 100;
                    perLeave = sumLeave / sumAll * 100;
                    perAbsent = sumAbsent / sumAll * 100;

                    sumAll = sumOnTime + sumLate;
                    sumAll = sumAll == 0 ? 1 : sumAll;
                    perOnTime = sumOnTime / sumAll * 100;
                    perLate = sumLate / sumAll * 100;

                    sumAll = sumOutTime + sumEarly;
                    sumAll = sumAll == 0 ? 1 : sumAll;
                    perOutTime = sumOutTime / sumAll * 100;
                    perEarly = sumEarly / sumAll * 100;
                }

                if (param.reviewPeriod == "This Week")
                {
                    ViewModel.GlobalVariables.GV_DepartmentPerformanceTill = "till Yesterday";
                }
                else if (param.reviewPeriod == "This Month")
                {
                    ViewModel.GlobalVariables.GV_DepartmentPerformanceTill = DateTime.Now.AddDays(-1).ToString("dd-MMM-yyyy");
                }
                else
                {
                    ViewModel.GlobalVariables.GV_DepartmentPerformanceTill = "";
                }

                if (param.reviewPeriod == "Last Day" || param.reviewPeriod == "Second Last Day" || param.reviewPeriod == "Third Last Day" || param.reviewPeriod == "Fourth Last Day" || param.reviewPeriod == "Fifth Last Day" || param.reviewPeriod == "Sixth Last Day" || param.reviewPeriod == "Seventh Last Day" || param.reviewPeriod == "Eightth Last Day" || param.reviewPeriod == "Nineth Last Day" || param.reviewPeriod == "Tenth Last Day" || param.reviewPeriod == "Eleventh Last Day" || param.reviewPeriod == "Twelveth Last Day" || param.reviewPeriod == "Thirteenth Last Day" || param.reviewPeriod == "Forteenth Last Day" || param.reviewPeriod == "Fifteenth Last Day" || param.reviewPeriod == "Sixteenth Last Day" || param.reviewPeriod == "Seventeenth Last Day" || param.reviewPeriod == "18 Last Day" || param.reviewPeriod == "19 Last Day" || param.reviewPeriod == "20 Last Day" || param.reviewPeriod == "21 Last Day" || param.reviewPeriod == "22 Last Day" || param.reviewPeriod == "23 Last Day" || param.reviewPeriod == "24 Last Day" || param.reviewPeriod == "25 Last Day" || param.reviewPeriod == "26 Last Day" || param.reviewPeriod == "27 Last Day" || param.reviewPeriod == "28 Last Day" || param.reviewPeriod == "29 Last Day" || param.reviewPeriod == "30 Last Day" || param.reviewPeriod == "31 Last Day" || param.reviewPeriod == "32 Last Day")
                {
                    //nothing to do
                }
                else
                {
                    cntPresent = -1;
                    cntLeave = -1;
                    cntAbsent = -1;
                    cntOnTime = -1;
                    cntLate = -1;
                    cntEarlyOut = -1;
                }

                ViewModel.GlobalVariables.GV_TotalRegisteredActiveEmployees = TimeTune.Dashboard.getTotalRegisteredEmp();
                ViewModel.GlobalVariables.GV_DepartmentPerformanceCount = "PC=" + cntPresent + ",LC=" + cntLeave + ",AC=" + cntAbsent + ",OC=" + cntOnTime + ",TC=" + cntLate + ",UC=" + cntOutTime + ",EC=" + cntEarlyOut;
                ViewModel.GlobalVariables.GV_DepartmentPerformancePercent = "P=" + string.Format("{0:0.00}", perPresent) + ",L=" + string.Format("{0:0.00}", perLeave) + ",A=" + string.Format("{0:0.00}", perAbsent) + ",O=" + string.Format("{0:0.00}", perOnTime) + ",T=" + string.Format("{0:0.00}", perLate) + ",U=" + string.Format("{0:0.00}", perOutTime) + ",E=" + string.Format("{0:0.00}", perEarly);

                //generate chart image
                //ViewBag.ImageSource = CreateChart();

                DTResult<ViewModels.Dept_Per_Rept> result = new DTResult<ViewModels.Dept_Per_Rept>
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

        private int DepartmentPerformancePDF(Depart_Perfromance_ReportLog sdata, string imageData)
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

                    Font fBold11 = FontFactory.GetFont("HELVETICA", 11, Font.BOLD, Color.BLACK);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    //////////////----------CHART RENDERING------------------////////////
                    //string _image = imageData.Replace("data:image/png;base64,", "");


                    //var base64Data = Regex.Match(_image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);

                    //byte[] bytes = Convert.FromBase64String(_image);

                    /////////////////////////////////////////////////////////
                    string strChartCount = ""; string[] strArrCount = { "0", "0", "0", "0", "0", "0" };
                    strChartCount = ViewModel.GlobalVariables.GV_DepartmentPerformanceCount; //"PC=15,LC=20,AC=64,OC=44,TC=42,EC=13";
                    strChartCount = strChartCount.Replace("PC=", "").Replace("LC=", "").Replace("AC=", "").Replace("OC=", "").Replace("TC=", "").Replace("UC=", "").Replace("EC=", "");
                    if (strChartCount != null && strChartCount != "" && strChartCount.Contains(","))
                    {
                        ////strChartData = strChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("E=", "");

                        strArrCount = strChartCount.Split(',');

                    }

                    PdfPTable tableCounts = new PdfPTable(new[] { 25.0f, 25.0f, 25.0f, 25.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableCounts.WidthPercentage = 100;
                    tableCounts.HeaderRows = 0;
                    //tableCounts.SpacingBefore = 50;
                    tableCounts.SpacingAfter = 3;
                    tableCounts.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellCntH01 = new PdfPCell(new Phrase("Total Registered", fBold10));
                    cellCntH01.BackgroundColor = Color.LIGHT_GRAY;
                    cellCntH01.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntH01);

                    PdfPCell cellCntH02 = new PdfPCell(new Phrase("Total Present", fBold10));
                    cellCntH02.BackgroundColor = Color.LIGHT_GRAY;
                    cellCntH02.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntH02);

                    PdfPCell cellCntH03 = new PdfPCell(new Phrase("Total Leave", fBold10));
                    cellCntH03.BackgroundColor = Color.LIGHT_GRAY;
                    cellCntH03.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntH03);

                    PdfPCell cellCntH04 = new PdfPCell(new Phrase("Total Absent", fBold10));
                    cellCntH04.BackgroundColor = Color.LIGHT_GRAY;
                    cellCntH04.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntH04);

                    //////////
                    int iTotalReg = 0;
                    iTotalReg = ViewModel.GlobalVariables.GV_TotalRegisteredActiveEmployees;

                    PdfPCell cellCntD01 = new PdfPCell(new Phrase(iTotalReg.ToString(), fNormal10));
                    cellCntD01.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD01);

                    PdfPCell cellCntD02 = new PdfPCell(new Phrase(strArrCount[0].ToString(), fNormal10));
                    cellCntD02.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD02);

                    PdfPCell cellCntD03 = new PdfPCell(new Phrase(strArrCount[1].ToString(), fNormal10));
                    cellCntD03.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD03);

                    PdfPCell cellCntD04 = new PdfPCell(new Phrase(strArrCount[2].ToString(), fNormal10));
                    cellCntD04.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD04);

                    ////// IR add Chart

                    //string vbChartData = ""; string[] strArr = { "0", "0", "0", "0", "0", "0" };
                    //vbChartData = ViewModel.GlobalVariables.GV_DepartmentPerformancePercent;
                    //if (vbChartData != null && vbChartData != "")
                    //{
                    //    vbChartData = vbChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").
                    //                                 Replace("O=", "").Replace("T=", "").Replace("E=", "");

                    //    strArr = vbChartData.Split(',');


                    //}

                    //https://www.aspsnippets.com/Articles/Export-Chart-to-PDF-in-ASPNet-MVC.aspx
                    string strChartData = "";
                    strChartData = ViewModel.GlobalVariables.GV_DepartmentPerformancePercent; //"P=15.35,L=20.00,A=64.65,O=44.02,T=42.54,E=13.44";
                    strChartData = strChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("U=", "").Replace("E=", "");

                    byte[] bytesArrPrsChart = CreateVisualChart(0, strChartData); // GetPresenceChartData(); //PopulateChart();
                    iTextSharp.text.Image prsChart = iTextSharp.text.Image.GetInstance(bytesArrPrsChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);


                    byte[] bytesArrPunChart = CreateVisualChart(1, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    iTextSharp.text.Image punChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    byte[] bytesArrExitChart = CreateVisualChart(2, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    iTextSharp.text.Image exitChart = iTextSharp.text.Image.GetInstance(bytesArrExitChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);

                    //Add the Image file to the PDF document object.
                    //iTextSharp.text.Image prsChart = iTextSharp.text.Image.GetInstance(bytesArrPrsChart);

                    //iTextSharp.text.Image pncChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);

                    //document.Add(prsChart);
                    //document.Add(punChart);

                    //PdfPTable tableChartsLegends = new PdfPTable(new[] { 55.0f, 206.0f, 73.0f, 206.0f, 55.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChartsLegends.WidthPercentage = 100;
                    //tableChartsLegends.HeaderRows = 0;
                    ////tableChartsLegends.SpacingBefore = 50;
                    //tableChartsLegends.SpacingAfter = 3;
                    //tableChartsLegends.DefaultCell.Border = Rectangle.NO_BORDER;


                    //// ---------- Legends Images ---------------------
                    //string strBullet01 = Server.MapPath("/Images/bullets-pla.png");
                    //Image imgBullet01 = Image.GetInstance(strBullet01);

                    //string strBullet02 = Server.MapPath("/Images/bullets-ole.png");
                    //Image imgBullet02 = Image.GetInstance(strBullet02);

                    //tableChartsLegends.AddCell("");
                    //tableChartsLegends.AddCell(imgBullet01);
                    //tableChartsLegends.AddCell("");
                    //tableChartsLegends.AddCell(imgBullet02);
                    //tableChartsLegends.AddCell("");

                    //PdfPTable tableCharts = new PdfPTable(new[] { 50.0f, 200.0f, 95.0f, 200.0f, 50.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    PdfPTable tableCharts = new PdfPTable(new[] { 300.0f, 10.0f, 300.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableCharts.WidthPercentage = 100;
                    tableCharts.HeaderRows = 0;
                    //tableCharts.SpacingBefore = 50;
                    tableCharts.SpacingAfter = 3;
                    tableCharts.DefaultCell.Border = Rectangle.NO_BORDER;

                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(prsChart);
                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");

                    tableCharts.AddCell(prsChart);
                    tableCharts.AddCell("");
                    tableCharts.AddCell(punChart);

                    //tableCharts.AddCell(tableChartsPercents);


                    ////////////////////////////////////

                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                    //System.Drawing.Image img = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
                    //Image chartImage = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Png);

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

                    ////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 285, 90.0f });//total 595 - 10 x 2 due to Left and Right side margin
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
                    tableEmployee.SpacingAfter = 5;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    string strFrom = Convert.ToDateTime(sdata.fromDate).ToString("dd-MMM-yyyy");
                    string strTo = "to " + Convert.ToDateTime(sdata.toDate).ToString("dd-MMM-yyyy");

                    if (ViewModel.GlobalVariables.GV_DepartmentPerformanceTill != "")
                    {
                        strTo = ViewModel.GlobalVariables.GV_DepartmentPerformanceTill;
                    }

                    if (strTo.Contains(strFrom))
                    {
                        PdfPCell cellEYear = new PdfPCell(new Phrase("Period: " + strFrom, fBold10));
                        cellEYear.Border = 0;
                        tableEInfo.AddCell(cellEYear);
                    }
                    else
                    {
                        PdfPCell cellEYear = new PdfPCell(new Phrase("Period: " + strFrom + " " + strTo, fBold10));
                        cellEYear.Border = 0;
                        tableEInfo.AddCell(cellEYear);
                    }

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Organization Performance Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);




                    //PdfPTable tableChart = new PdfPTable(new[] { 200.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChart.WidthPercentage = 100;
                    //tableChart.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50f;
                    //tableEmployee.SpacingAfter = 10f;
                    //tableChart.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPTable tableChartInfo = new PdfPTable(1);
                    //tableChartInfo.WidthPercentage = 100;
                    //tableChartInfo.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableChartInfo.SpacingAfter = 3;
                    //tableChartInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellEChart = new PdfPCell();
                    //cellEChart.Border = 0;
                    //tableChartInfo.AddCell(chartImage);


                    //tableChart.AddCell(tableChartInfo);
                    //document.Add(tableChart);



                    document.Add(tableCharts);

                    //Paragraph p_count = new Paragraph("Day Count:", fBold14);
                    //p_count.SpacingBefore = 2;
                    //p_count.SpacingAfter = 5;
                    //document.Add(p_count);

                    if (strArrCount[0].ToString() != "-1")
                    {
                        Paragraph p_count = new Paragraph("Summary:", fBold14);
                        p_count.SpacingBefore = 2;
                        p_count.SpacingAfter = 5;
                        document.Add(p_count);

                        document.Add(tableCounts);
                    }


                    Paragraph p_detail = new Paragraph("Departments by Percentage:", fBold14);
                    p_detail.SpacingBefore = 2;
                    p_detail.SpacingAfter = 5;
                    document.Add(p_detail);

                    //Headering Colspan table
                    PdfPTable tableHeaderSpan = new PdfPTable(new[] { 220.0f, 150.0f, 105.0f, 120.0f });
                    tableHeaderSpan.WidthPercentage = 100;
                    tableHeaderSpan.HeaderRows = 0;
                    //tableHeaderSpan.SpacingBefore = 3;
                    //tableHeaderSpan.SpacingAfter = 1;

                    PdfPCell cellHS1 = new PdfPCell(new Phrase("", fBold11));
                    //cellHS1.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS1.HorizontalAlignment = 1;
                    cellHS1.Border = 0;
                    tableHeaderSpan.AddCell(cellHS1);

                    PdfPCell cellHS2 = new PdfPCell(new Phrase("Presence Status", fBold11));
                    cellHS2.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS2.HorizontalAlignment = 1;
                    tableHeaderSpan.AddCell(cellHS2);

                    PdfPCell cellHS3 = new PdfPCell(new Phrase("Punctuality Status", fBold11));
                    cellHS3.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS3.HorizontalAlignment = 1;
                    tableHeaderSpan.AddCell(cellHS3);

                    PdfPCell cellHS4 = new PdfPCell(new Phrase("Exit Status", fBold11));
                    cellHS4.BackgroundColor = Color.LIGHT_GRAY;
                    cellHS4.HorizontalAlignment = 1;
                    tableHeaderSpan.AddCell(cellHS4);



                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 220.0f, 50.0f, 50.0f, 50.0f, 55.0f, 50.0f, 60.0f, 60.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    // tableMid.SpacingBefore = 3;
                    // tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Department Name", fBold10));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Present (%)", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Leave (%)", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Absent (%)", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("On Time (%)", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Late (%)", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Exit (%)", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Early Out (%)", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    foreach (Dept_Per_Rept log in sdata.dataSet)
                    {

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.DepartmentName, fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.PresentPercent.ToString(), fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.LeavePercent.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.AbsentPercent.ToString(), fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);



                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.OnTimePercent.ToString(), fNormal8));
                        //cellData5.BackgroundColor = Color.LIGHT_GRAY;
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.LatePercent.ToString(), fNormal8));
                        //cellData6.BackgroundColor = Color.LIGHT_GRAY;
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        //cellData5.BackgroundColor = new iTextSharp.text.Color(System.Drawing.Color.FromArgb(230, 245, 251)); //Color.YELLOW; iTextSharp.text.Color(230.0f, 245.0f, 251.0f); //Color.YELLOW;
                        //cellData6.BackgroundColor = new iTextSharp.text.Color(System.Drawing.Color.FromArgb(230, 245, 251)); //Color.YELLOW;


                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.OutTimePercent.ToString(), fNormal8));
                        cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.EarlyPercent.ToString(), fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);

                    }


                    if (sdata.dataSet.Count > 0)
                    {
                        document.Add(tableHeaderSpan);
                        document.Add(tableMid);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Org-Performance-Report-" + strFrom + ".pdf");
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

        [HttpPost]
        public string DepartmentPerformancePercentage()
        {
            return ViewModel.GlobalVariables.GV_DepartmentPerformancePercent;
        }

        #endregion


        #region Department Todays Report

        [HttpGet]
        [ActionName("DepartmentTodaysReport")]
        public ActionResult DepartmentTodaysReport_Get()
        {
            //ViewBag.ImageSource = CreateChart();

            return View();
        }

        public class CustomDTParameters3 : DTParameters
        {
            public string reviewPeriod { get; set; }
            public string to_date { get; set; }
            public string imageData { get; set; }
        }

        [HttpPost]
        [ActionName("DepartmentTodaysReport")]
        public ActionResult DepartmentTodaysReport_Post(CustomDTParameters3 param)
        {
            try
            {
                var dtsource = TimeTune._DeptPerformanceReport.getDeptartmentTodays(param.reviewPeriod);
                //DepartmentTodaysPDF(dtsource, param.imageData);

                if (dtsource != null)
                {
                    DepartmentTodaysPDF(dtsource, param.imageData);
                    ViewBag.DeptPerfMessage = "";
                }
                else
                {
                    ViewBag.DeptPerfMessage = "NO";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return View();
        }

        public JsonResult DeptTodaysReportDataHandler(CustomDTParameters3 param)
        {
            try
            {
                // bit[] base64 = Request.Form["chart2"];
                var dtsource = new Depart_Perfromance_ReportLog();
                dtsource = TimeTune._DeptPerformanceReport.getDeptartmentTodays(param.reviewPeriod);

                if (dtsource.dataSet == null)
                    return Json("No Data to Show");

                List<ViewModels.Dept_Per_Rept> data = DeptPerReportSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource.dataSet);

                int count = DeptPerReportSet.Count(param.Search.Value, dtsource.dataSet);

                int cntPresent = 0, cntLeave = 0, cntAbsent = 0, cntOnTime = 0, cntLate = 0, cntEarlyOut = 0;
                decimal sumAll = 0;
                decimal sumPresent = 0, sumLeave = 0, sumAbsent = 0;
                decimal sumOnTime = 0, sumLate = 0, sumEarly = 0;
                decimal perPresent = 0, perLeave = 0, perAbsent = 0, perOnTime = 0, perLate = 0, perEarly = 0;

                if (count > 0)
                {
                    foreach (var i in data)
                    {
                        sumPresent += i.PresentCount;
                        //sumLeave += i.LeaveCount;
                        sumAbsent += i.AbsentCount;
                        //sumOnTime += i.OnTimeCount;
                        //sumLate += i.LateCount;
                        //sumEarly += i.EarlyCount;

                        cntPresent += i.PresentCount;
                        //cntLeave += i.LeaveCount;
                        cntAbsent += i.AbsentCount;
                        //cntOnTime += i.OnTimeCount;
                        //cntLate += i.LateCount;
                        //cntEarlyOut += i.EarlyCount;
                    }

                    sumAll = sumPresent + sumLeave + sumAbsent;
                    sumAll = sumAll == 0 ? 1 : sumAll;
                    perPresent = sumPresent / sumAll * 100;
                    //perLeave = sumLeave / sumAll * 100;
                    perAbsent = sumAbsent / sumAll * 100;

                    //sumAll = sumOnTime + sumLate + sumEarly;
                    //sumAll = sumAll == 0 ? 1 : sumAll;
                    //perOnTime = sumOnTime / sumAll * 100;
                    //perLate = sumLate / sumAll * 100;
                    //perEarly = sumEarly / sumAll * 100;
                }

                ViewModel.GlobalVariables.GV_TotalRegisteredActiveEmployees = TimeTune.Dashboard.getTotalRegisteredEmp();
                ViewModel.GlobalVariables.GV_DepartmentTodaysCount = "PC=" + cntPresent + ",AC=" + cntAbsent;
                ViewModel.GlobalVariables.GV_DepartmentTodaysPercent = "P=" + string.Format("{0:0.00}", perPresent) + ",A=" + string.Format("{0:0.00}", perAbsent);

                //generate chart image
                //ViewBag.ImageSource = CreateChart();

                DTResult<ViewModels.Dept_Per_Rept> result = new DTResult<ViewModels.Dept_Per_Rept>
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

        private int DepartmentTodaysPDF(Depart_Perfromance_ReportLog sdata, string imageData)
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
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    //////////////----------CHART RENDERING------------------////////////
                    //string _image = imageData.Replace("data:image/png;base64,", "");


                    //var base64Data = Regex.Match(_image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);

                    //byte[] bytes = Convert.FromBase64String(_image);

                    /////////////////////////////////////////////////////////
                    string strChartCount = ""; string[] strArrCount = { "0", "0" };
                    strChartCount = ViewModel.GlobalVariables.GV_DepartmentTodaysCount; //"PC=15,LC=20,AC=64,OC=44,TC=42,EC=13";
                    strChartCount = strChartCount.Replace("PC=", "").Replace("AC=", "");
                    if (strChartCount != null && strChartCount != "" && strChartCount.Contains(","))
                    {
                        ////strChartData = strChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("E=", "");

                        strArrCount = strChartCount.Split(',');

                    }

                    PdfPTable tableCounts = new PdfPTable(new[] { 50.0f, 25.0f, 25.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableCounts.WidthPercentage = 100;
                    tableCounts.HeaderRows = 0;
                    //tableCounts.SpacingBefore = 50;
                    tableCounts.SpacingAfter = 3;
                    tableCounts.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellCntH01 = new PdfPCell(new Phrase("Total Registered", fBold10));
                    cellCntH01.BackgroundColor = Color.LIGHT_GRAY;
                    cellCntH01.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntH01);

                    PdfPCell cellCntH02 = new PdfPCell(new Phrase("Total Present", fBold10));
                    cellCntH02.BackgroundColor = Color.LIGHT_GRAY;
                    cellCntH02.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntH02);

                    PdfPCell cellCntH03 = new PdfPCell(new Phrase("Total Absent", fBold10));
                    cellCntH03.BackgroundColor = Color.LIGHT_GRAY;
                    cellCntH03.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntH03);

                    //PdfPCell cellCntH04 = new PdfPCell(new Phrase("Total Absent", fBold10));
                    //cellCntH04.BackgroundColor = Color.LIGHT_GRAY;
                    //cellCntH04.HorizontalAlignment = 1;
                    //tableCounts.AddCell(cellCntH04);

                    //////////
                    int iTotalReg = 0;
                    iTotalReg = ViewModel.GlobalVariables.GV_TotalRegisteredActiveEmployees;

                    PdfPCell cellCntD01 = new PdfPCell(new Phrase(iTotalReg.ToString(), fNormal10));
                    cellCntD01.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD01);

                    PdfPCell cellCntD02 = new PdfPCell(new Phrase(strArrCount[0].ToString(), fNormal10));
                    cellCntD02.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD02);

                    PdfPCell cellCntD03 = new PdfPCell(new Phrase(strArrCount[1].ToString(), fNormal10));
                    cellCntD03.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD03);

                    //PdfPCell cellCntD04 = new PdfPCell(new Phrase(strArrCount[2].ToString(), fNormal10));
                    //cellCntD04.HorizontalAlignment = 1;
                    //tableCounts.AddCell(cellCntD04);

                    ////// IR add Chart

                    //string vbChartData = ""; string[] strArr = { "0", "0", "0", "0", "0", "0" };
                    //vbChartData = ViewModel.GlobalVariables.GV_DepartmentPerformancePercent;
                    //if (vbChartData != null && vbChartData != "")
                    //{
                    //    vbChartData = vbChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").
                    //                                 Replace("O=", "").Replace("T=", "").Replace("E=", "");

                    //    strArr = vbChartData.Split(',');


                    //}

                    //https://www.aspsnippets.com/Articles/Export-Chart-to-PDF-in-ASPNet-MVC.aspx
                    string strChartData = "";
                    strChartData = ViewModel.GlobalVariables.GV_DepartmentTodaysPercent; //"P=15.35,L=20.00,A=64.65,O=44.02,T=42.54,E=13.44";
                    strChartData = strChartData.Replace("P=", "").Replace("A=", "");//.Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("E=", "");

                    byte[] bytesArrPrsChart = CreateVisualChart(3, strChartData); // GetPresenceChartData(); //PopulateChart();
                    iTextSharp.text.Image prsChart = iTextSharp.text.Image.GetInstance(bytesArrPrsChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);


                    ////byte[] bytesArrPunChart = CreateVisualChart(1, strChartData); // GetPunctualityChartData(); //PopulateChart();
                    ////iTextSharp.text.Image punChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);
                    //ViewBag.PresenceChart = "data:image/png;base64," + Convert.ToBase64String(bytesArrPrsChart, 0, bytesArrPrsChart.Length);


                    //Add the Image file to the PDF document object.
                    //iTextSharp.text.Image prsChart = iTextSharp.text.Image.GetInstance(bytesArrPrsChart);

                    //iTextSharp.text.Image pncChart = iTextSharp.text.Image.GetInstance(bytesArrPunChart);

                    //document.Add(prsChart);
                    //document.Add(punChart);

                    //PdfPTable tableChartsLegends = new PdfPTable(new[] { 55.0f, 206.0f, 73.0f, 206.0f, 55.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChartsLegends.WidthPercentage = 100;
                    //tableChartsLegends.HeaderRows = 0;
                    ////tableChartsLegends.SpacingBefore = 50;
                    //tableChartsLegends.SpacingAfter = 3;
                    //tableChartsLegends.DefaultCell.Border = Rectangle.NO_BORDER;


                    //// ---------- Legends Images ---------------------
                    //string strBullet01 = Server.MapPath("/Images/bullets-pla.png");
                    //Image imgBullet01 = Image.GetInstance(strBullet01);

                    //string strBullet02 = Server.MapPath("/Images/bullets-ole.png");
                    //Image imgBullet02 = Image.GetInstance(strBullet02);

                    //tableChartsLegends.AddCell("");
                    //tableChartsLegends.AddCell(imgBullet01);
                    //tableChartsLegends.AddCell("");
                    //tableChartsLegends.AddCell(imgBullet02);
                    //tableChartsLegends.AddCell("");

                    //PdfPTable tableCharts = new PdfPTable(new[] { 50.0f, 200.0f, 95.0f, 200.0f, 50.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    PdfPTable tableCharts = new PdfPTable(new[] { 147.0f, 300.0f, 148.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableCharts.WidthPercentage = 100;
                    tableCharts.HeaderRows = 0;
                    //tableCharts.SpacingBefore = 50;
                    tableCharts.SpacingAfter = 3;
                    tableCharts.DefaultCell.Border = Rectangle.NO_BORDER;

                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(prsChart);
                    //tableCharts.AddCell("");
                    //tableCharts.AddCell(punChart);
                    //tableCharts.AddCell("");

                    tableCharts.AddCell("");
                    tableCharts.AddCell(prsChart);
                    tableCharts.AddCell("");


                    //tableCharts.AddCell(tableChartsPercents);


                    ////////////////////////////////////

                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                    //System.Drawing.Image img = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
                    //Image chartImage = Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Png);

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

                    ////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 285, 90.0f });//total 595 - 10 x 2 due to Left and Right side margin
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
                    tableEmployee.SpacingAfter = 10;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellEYear = new PdfPCell(new Phrase("Today: " + DateTime.Now.ToString("dd-MMM-yyyy"), fBold10));
                    cellEYear.Border = 0;
                    tableEInfo.AddCell(cellEYear);

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Organization Today's Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);




                    //PdfPTable tableChart = new PdfPTable(new[] { 200.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //tableChart.WidthPercentage = 100;
                    //tableChart.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50f;
                    //tableEmployee.SpacingAfter = 10f;
                    //tableChart.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPTable tableChartInfo = new PdfPTable(1);
                    //tableChartInfo.WidthPercentage = 100;
                    //tableChartInfo.HeaderRows = 0;
                    ////tableHeader.SpacingBefore = 50;
                    //tableChartInfo.SpacingAfter = 3;
                    //tableChartInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellEChart = new PdfPCell();
                    //cellEChart.Border = 0;
                    //tableChartInfo.AddCell(chartImage);


                    //tableChart.AddCell(tableChartInfo);
                    //document.Add(tableChart);



                    document.Add(tableCharts);

                    //Paragraph p_count = new Paragraph("Day Count:", fBold14);
                    //p_count.SpacingBefore = 2;
                    //p_count.SpacingAfter = 5;
                    //document.Add(p_count);

                    if (strArrCount[0].ToString() != "-1")
                    {
                        Paragraph p_count = new Paragraph("Summary:", fBold14);
                        p_count.SpacingBefore = 2;
                        p_count.SpacingAfter = 5;
                        document.Add(p_count);

                        document.Add(tableCounts);
                    }


                    Paragraph p_detail = new Paragraph("Departments by Percentage:", fBold14);
                    p_detail.SpacingBefore = 2;
                    p_detail.SpacingAfter = 5;
                    document.Add(p_detail);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 295.0f, 150.0f, 150.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Department Name", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Present (%)", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Absent (%)", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    //PdfPCell cell4 = new PdfPCell(new Phrase("Absent (%)", fBold8));
                    //cell4.BackgroundColor = Color.LIGHT_GRAY;
                    //cell4.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell4);

                    //PdfPCell cell5 = new PdfPCell(new Phrase("On Time (%)", fBold8));
                    //cell5.BackgroundColor = Color.LIGHT_GRAY;
                    //cell5.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell5);

                    //PdfPCell cell6 = new PdfPCell(new Phrase("Late (%)", fBold8));
                    //cell6.BackgroundColor = Color.LIGHT_GRAY;
                    //cell6.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell6);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("Early Out (%)", fBold8));
                    //cell7.BackgroundColor = Color.LIGHT_GRAY;
                    //cell7.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell7);

                    foreach (Dept_Per_Rept log in sdata.dataSet)
                    {

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.DepartmentName, fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.PresentPercent.ToString(), fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.AbsentPercent.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        //PdfPCell cellData4 = new PdfPCell(new Phrase(log.AbsentPercent.ToString(), fNormal8));
                        //cellData4.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData4);

                        //PdfPCell cellData5 = new PdfPCell(new Phrase(log.OnTimePercent.ToString(), fNormal8));
                        //cellData5.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData5);

                        //PdfPCell cellData6 = new PdfPCell(new Phrase(log.LatePercent.ToString(), fNormal8));
                        //cellData6.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData6);

                        //PdfPCell cellData7 = new PdfPCell(new Phrase(log.EarlyPercent.ToString(), fNormal8));
                        //cellData7.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData7);

                    }


                    if (sdata.dataSet.Count > 0)
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
                        Response.AddHeader("content-disposition", "attachment;filename=Org-Todays-Report-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
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

        [HttpPost]
        public string DepartmentTodaysPercentage()
        {
            return ViewModel.GlobalVariables.GV_DepartmentTodaysPercent;
        }

        #endregion


        #region MissPunchReport

        public class ConsolidatedMissPuncheportTable : DTParameters
        {
            public string department_id { get; set; }
            public string designation_id { get; set; }
            public string function_id { get; set; }
            public string region_id { get; set; }
            public string location_id { get; set; }
            public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult MissPunchDepartmentdDataHandler()
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
        public JsonResult MissPunchDesignationDataHandler()
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
        public JsonResult MissPunchLocationDataHandler()
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
        public JsonResult MissPunchFunctionDataHandler()
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
        public JsonResult MissPunchRegionDataHandler()
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
        public JsonResult MissPunchReportHandler(ConsolidatedMissPuncheportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceDepartmentWise>();

                int count = TimeTune.Reports.getAllMissPunchLogs(param.department_id, param.designation_id, param.function_id, param.region_id, User.Identity.Name, param.location_id, param.from_date, param.to_date, param.Search.Value, out data);

                List<ViewModels.ConsolidatedAttendanceDepartmentWise> data2 = MissPunchResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise> result = new DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise>
                {
                    draw = param.Draw,
                    data = data2,
                    recordsFiltered = data.Count,
                    recordsTotal = data.Count
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
        public JsonResult MissPunchgraph(string dept, string des, string loc, string from, string to)
        {
            ViewModels.Dashboard dashboard = TimeTune.Dashboard.getGraphValues(dept, des, User.Identity.Name, loc, from, to);
            return Json(dashboard);
        }

        [HttpGet]
        [ActionName("MissPunchReport")]
        public ActionResult MissPunchReport_Get()
        {
            ViewData["PDFNoDataFoundDept"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("MissPunchReport")]
        [ValidateAntiForgeryToken]
        public ActionResult MissPunchReport_Post()
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

            reportMaker.logs = TimeTune.Reports.getAttendanceMissPunch(depart_id, des_id, loc_id, fun_id, reg_id, from_date, to_date);

            if (reportMaker == null)
                return RedirectToAction("MonthlyTimeSheet");

            ////return new Rotativa.ViewAsPdf("ConsolidatedFilteredReports", reportMaker) { FileName = "reports.pdf" };

            // -------------------------------------------

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFoundDept"] = "";
            found = GenerateMissPunchReportPDF(reportMaker);

            if (found == 1)
            {
                ViewData["PDFNoDataFoundDept"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFoundDept"] = "No Data Found";
            }

            return View("DepartmentReport");

            //GenerateDepartmentWiseReportPDF(reportMaker);

            //return null;
        }

        private int GenerateMissPunchReportPDF(BLL.ViewModels.FilteredAttendanceReportDepartmentWise sdata)
        {
            int response = 0;

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
                    cellDateTime.RunDirection = PdfLayoutHelper.RunDirection;
                    tableHeader.AddCell(cellDateTime);

                    document.Add(tableHeader);



                    document.Add(lineSeparator);


                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(3);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    tableHeader.SpacingBefore = 10f;
                    tableEmployee.SpacingAfter = 20f;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    Paragraph p_title = new Paragraph("Miss Punched Report", fBold16);
                    p_title.SpacingBefore = 10f;
                    p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    tableEmployee.AddCell("");
                    tableEmployee.AddCell("");
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
                        Response.AddHeader("content-disposition", "attachment;filename=MissPunched-Report-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        response = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return response;
        }

        [HttpPost]
        public ActionResult MissPunchReportExcelDownload(ConsolidatedMissPuncheportTable param)
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

                ddata = TimeTune.Reports.getAttendanceMissPunchExcelDownload(depart_id, des_id, loc_id, fun_id, reg_id, param.from_date, param.to_date);

                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<ConsolidatedAttendanceDepartmentWiseExcelDownload>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("MissPuchAttendanceReport");
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

        #endregion

        #region ManualAttendanceMarkReport

        public class ConsolidatedManualPuncheportTable : DTParameters
        {
            public string department_id { get; set; }
            public string designation_id { get; set; }
            public string function_id { get; set; }
            public string region_id { get; set; }
            public string location_id { get; set; }
            public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult ManualPunchDepartmentdDataHandler()
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
        public JsonResult ManualPunchDesignationDataHandler()
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
        public JsonResult ManualPunchLocationDataHandler()
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
        public JsonResult ManualPunchFunctionDataHandler()
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
        public JsonResult ManualPunchRegionDataHandler()
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
        public JsonResult ManualPunchReportHandler(ConsolidatedManualPuncheportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceDepartmentWise>();

                int count = TimeTune.Reports.getAllManualPunchLogs(param.department_id, param.designation_id, param.function_id, param.region_id, User.Identity.Name, param.location_id, param.from_date, param.to_date, param.Search.Value, out data);

                List<ViewModels.ConsolidatedAttendanceDepartmentWise> data2 = ManualPunchResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);


                DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise> result = new DTResult<ViewModels.ConsolidatedAttendanceDepartmentWise>
                {
                    draw = param.Draw,
                    data = data2,
                    recordsFiltered = data.Count,
                    recordsTotal = data.Count
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
        public JsonResult ManualPunchgraph(string dept, string des, string loc, string from, string to)
        {
            ViewModels.Dashboard dashboard = TimeTune.Dashboard.getGraphValues(dept, des, User.Identity.Name, loc, from, to);
            return Json(dashboard);
        }

        [HttpGet]
        [ActionName("ManualPunchReport")]
        public ActionResult ManualPunchReport_Get()
        {
            ViewData["PDFNoDataFoundDept"] = "";

            return View();
        }

        [HttpPost]
        [ActionName("ManualPunchReport")]
        [ValidateAntiForgeryToken]
        public ActionResult ManualPunchReport_Post()
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

            reportMaker.logs = TimeTune.Reports.getAttendanceManualPunch(depart_id, des_id, loc_id, fun_id, reg_id, from_date, to_date);

            if (reportMaker == null)
                return RedirectToAction("MonthlyTimeSheet");

            ////return new Rotativa.ViewAsPdf("ConsolidatedFilteredReports", reportMaker) { FileName = "reports.pdf" };

            // -------------------------------------------

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFoundDept"] = "";
            found = GenerateManualPunchReportPDF(reportMaker);

            if (found == 1)
            {
                ViewData["PDFNoDataFoundDept"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFoundDept"] = "No Data Found";
            }

            return View("DepartmentReport");

            //GenerateDepartmentWiseReportPDF(reportMaker);

            //return null;
        }

        private int GenerateManualPunchReportPDF(BLL.ViewModels.FilteredAttendanceReportDepartmentWise sdata)
        {
            int response = 0;

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
                    cellDateTime.RunDirection = PdfLayoutHelper.RunDirection;
                    tableHeader.AddCell(cellDateTime);

                    document.Add(tableHeader);



                    document.Add(lineSeparator);


                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(3);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    tableHeader.SpacingBefore = 10f;
                    tableEmployee.SpacingAfter = 10f;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    Paragraph p_title = new Paragraph("Manual Punched Report", fBold16);
                    p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    tableEmployee.AddCell("");
                    tableEmployee.AddCell("");
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
                        Response.AddHeader("content-disposition", "attachment;filename=ManualPunched-Report-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        response = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return response;
        }

        [HttpPost]
        public ActionResult ManualPunchReportExcelDownload(ConsolidatedManualPuncheportTable param)
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

                ddata = TimeTune.Reports.getAttendanceManualPunchExcelDownload(depart_id, des_id, loc_id, fun_id, reg_id, param.from_date, param.to_date);

                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<ConsolidatedAttendanceDepartmentWiseExcelDownload>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("ManualMarkAttendanceReport");
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

        #endregion

        #region PresentAbsentReport

        public ActionResult PresentAbsentReport()
        {
            return View();
        }

        public class PresentAbsentReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public string status { get; set; }
        }

        [HttpPost]
        public JsonResult PresentAbsentReportDataHandler(PresentAbsentReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllPresentAbsentReport(param.employee_id, param.from_date, param.to_date, param.status, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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


        #endregion


        #region DevicesStatusRegionReport



        public ActionResult DevicesStatusRegionReport()
        {
            return View();
        }


        [HttpPost]
        public JsonResult DevicesStatusRegionReportDataHandler(DeviceStatusRegionReportTable param)
        {
            try
            {
                List<BLL_UNIS.ViewModels.DevicesStatus> data = new List<DevicesStatus>();

                data = BLL_UNIS.UNIS_Reports.getDevicesStatusRegionList(param.device_region_id, param.Search.Value, param.SortOrder, param.Start, param.Length);

                List<BLL_UNIS.ViewModels.DevicesStatus> data2 = Devices_Status_ResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<BLL_UNIS.ViewModels.DevicesStatus> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatus>
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



        #region DevicesStatusReportHR



        public ActionResult DevicesStatusReportHR()
        {


            return View();
        }


        [HttpPost]
        public JsonResult DevicesStatusReportHRDataHandler(DeviceStatusReportTable param)
        {
            try
            {
                List<BLL_UNIS.ViewModels.DevicesStatus> data = new List<DevicesStatus>();

                data = BLL_UNIS.UNIS_Reports.getDevicesStatusList(param.Search.Value, param.SortOrder, param.Start, param.Length);

                List<BLL_UNIS.ViewModels.DevicesStatus> data2 = Devices_Status_ResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<BLL_UNIS.ViewModels.DevicesStatus> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatus>
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

        #region HourlyReport

        public ActionResult HourlyReport()
        {
            return View();
        }

        public class HourlyReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult HourlyReportDataHandler(HourlyReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models

                int count = TimeTune.Reports.getHourlyReportAttendanceMatching(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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
        public ActionResult HourlyReportExcelDownload(HourlyReportTable param)
        {
            var ddata = new List<HourlyAttendanceExport>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int count = TimeTune.Reports.getAllHourlyAttendanceMatching(param.employee_id, param.from_date, param.to_date, out ddata);

                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<HourlyAttendanceExport>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("HourlyAttendanceLog");
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
                Data = new { FileGuid = handle, FileName = "Hourly-Attendance-Log.xlsx" }
            };
        }
        #endregion

        #region UserTrackingReport

        public ActionResult UserTrackingReport()
        {
            return View();
        }

        public class TrackingReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult UserTrackingReportDataHandler(TrackingReportTable param)
        {
            try
            {
                var data = new List<UserTracking>();

                data = TimeTune.Reports.getAllUserTrackingRecords(param.from_date, param.to_date, param.employee_id);


                List<ViewModels.UserTracking> Sresult = UserTrackingResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                int count = UserTrackingResultSet.Count(param.Search.Value, data);



                DTResult<UserTracking> result = new DTResult<UserTracking>
                {
                    draw = param.Draw,
                    data = Sresult,
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

        #region WorkHourReport

        public ActionResult WorkHourReport()
        {
            return View();
        }

        public class WorkHourReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult WorkHourReportDataHandler(WorkHourReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models

                int count = TimeTune.Reports.getWorkHourReportAttendance(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ConsolidatedAttendanceLog> result = new DTResult<ConsolidatedAttendanceLog>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = 1,
                    recordsTotal = 1
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
        public ActionResult WorkHourReportExcelDownload(HourlyReportTable param)
        {
            var ddata = new List<HourlyAttendanceExport>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int count = TimeTune.Reports.getAllHourlyAttendanceMatching(param.employee_id, param.from_date, param.to_date, out ddata);

                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<HourlyAttendanceExport>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("HourlyAttendanceLog");
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
                Data = new { FileGuid = handle, FileName = "Hourly-Attendance-Log.xlsx" }
            };
        }
        #endregion

        #region WokringHourTimeSheet

        [HttpGet]
        [ActionName("WorkHoursTimeSheet")]
        public ActionResult WorkHoursTimeSheet_Get()
        {
            return View();
        }

        [HttpGet]
        [ActionName("WorkHoursTimeSheetNew")]
        public ActionResult WorkHoursTimeSheetNew_Get()
        {
            return View();
        }

        public class MonthlyWokringHourTimesheetTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        [HttpPost]
        [ActionName("WorkHoursTimeSheet")]
        public ActionResult WorkHoursTimeSheet_Post()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("WorkHoursTimeSheet");

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getWorkingHourReport(employeeID, month);

            //int a = toRender.logs[toRender.logs.Count() - 1].totalShfit_Hour;
            if (toRender == null)
                return RedirectToAction("WorkHoursTimeSheet");

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateWorkHoursTimeSheetPDF(toRender);

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

        [HttpPost]
        public JsonResult WorkHoursTimeSheetAllData()
        {
            try
            {
                string month = Request.Form["month"];
                string location = Request.Form["location"];
                string department = Request.Form["department"];
                string function = Request.Form["function"];

                BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();
                return Json(reportMaker.getWorkingHourReportAll(month,location,department,function));
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("WorkHoursTimeSheetAll")]
        public ActionResult WorkHoursTimeSheetAll()
        {
            
            List<SelectListItem> locations = new List<SelectListItem>();
            List<SelectListItem> departments = new List<SelectListItem>();
            List<SelectListItem> functions = new List<SelectListItem>();

            using (var db = new DLL.Models.Context())
            {
                foreach(var loc in db.location.OrderBy(l => l.name).ToList())
                {
                    locations.Add(new SelectListItem { Text = loc.name, Value = ""+loc.LocationId });
                }

                foreach (var dep in db.department.OrderBy(d => d.name).ToList())
                {
                    departments.Add(new SelectListItem { Text = dep.name, Value = "" + dep.DepartmentId });
                }

                foreach (var fun in db.function.OrderBy(f => f.name).ToList())
                {
                    functions.Add(new SelectListItem { Text = fun.name, Value = "" + fun.FunctionId });
                }
            }
            ViewBag.locations = locations;
            ViewBag.departments = departments;
            ViewBag.functions = functions;

            return View();

        }

        [HttpPost]
        [ActionName("WorkHoursTimeSheetNew")]
        public ActionResult WorkHoursTimeSheetNew_Post()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("WorkHoursTimeSheetNew");

            string fromDate = Request.Form["fromDate"];
            string toDate = Request.Form["toDate"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.CustomRangeTimeSheetData toRender = reportMaker.getWorkingHourReportAlt(employeeID, fromDate, toDate);

            //int a = toRender.logs[toRender.logs.Count() - 1].totalShfit_Hour;
            if (toRender == null)
                return RedirectToAction("WorkHoursTimeSheetNew");

            ViewData["PDFNoDataFound"] = "";
            ActionResult pdfResult = GenerateWorkHoursTimeSheetPDFAlt(toRender);
            if (pdfResult != null)
                return pdfResult;

            ViewData["PDFNoDataFound"] = "No Data Found";
            return View();

        }

        private int GenerateWorkHoursTimeSheetPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
            int reponse = 0;

            try
            {

                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\Arial.ttf", BaseFont.IDENTITY_H, true);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);


                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
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

                    PdfPCell cellEName = new PdfPCell(new Phrase(sdata.logs[0].finalRemarks + "   :" + "اسم", font));
                    cellEName.RunDirection = PdfLayoutHelper.RunDirection;
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);


                    PdfPCell cellECode = new PdfPCell(new Phrase("   :" + "رمز الموظف" + sdata.logs[0].overtime_status, font));
                    cellECode.RunDirection = PdfLayoutHelper.RunDirection;
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase(sdata.logs[0].remarksIn + "شهر سنة", font));
                    cellEMonth.RunDirection = PdfLayoutHelper.RunDirection;
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);


                    PdfPCell cellETitle = new PdfPCell(new Phrase("الجدول الزمني لساعات العمل الشهرية", font));
                    cellETitle.RunDirection = PdfLayoutHelper.RunDirection;
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);


                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 60.0f, 60.0f, 60.0f, 60.0f, 80.0f, 95.0f, 95.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("تاريخ", font));
                    cell1.RunDirection = PdfLayoutHelper.RunDirection;
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("اسم", font));
                    cell2.RunDirection = PdfLayoutHelper.RunDirection;
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("حان الوقت", font));
                    cell3.RunDirection = PdfLayoutHelper.RunDirection;
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("نفذ الوقت", font));
                    cell4.RunDirection = PdfLayoutHelper.RunDirection;
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("نفذ الوقت", font));
                    cell5.RunDirection = PdfLayoutHelper.RunDirection;
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("ساعة متأخرة", font));
                    cell6.RunDirection = PdfLayoutHelper.RunDirection;
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("متأخر , بعد فوات الوقت", font));
                    cell7.RunDirection = PdfLayoutHelper.RunDirection;
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("المجموع الإجمالي", font));
                    cell8.RunDirection = PdfLayoutHelper.RunDirection;
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

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.finalRemarks, fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                        cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.overtime2, fNormal8));
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.remarksOut, fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminalIn, fNormal7));
                        cellData7.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalOut, fNormal7));
                        cellData8.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData8);
                    }

                    if (sdata.logs.Length > 0)
                    {
                        document.Add(tableMid);


                        // Summary heading

                        Paragraph p_summary = new Paragraph("ملخص", font);
                        p_summary.Alignment = Element.ALIGN_LEFT;
                        document.Add(p_summary);
                        //PdfPCell p_summary = new PdfPCell(new Phrase("ملخص", font));
                        //p_summary.RunDirection = PdfLayoutHelper.RunDirection;
                        //p_summary.BackgroundColor = Color.LIGHT_GRAY;
                        //p_summary.HorizontalAlignment = 1;
                        //document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        tableEnd.WidthPercentage = 100;
                        tableEnd.HeaderRows = 0;
                        tableEnd.SpacingBefore = 3;
                        tableEnd.SpacingAfter = 3;

                        PdfPCell lt_cell_11 = new PdfPCell(new Phrase(":ساعات العمل المطلوبة", font));
                        lt_cell_11.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_11);
                        tableEnd.AddCell(new Phrase(" " + sdata.logs[sdata.logs.Count() - 1].totalShfit_Hour, fNormal8));


                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase(":إجمالي الساعات المتأخرة", font));
                        lt_cell_21.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + sdata.logs[sdata.logs.Count() - 1].totalLateHours, fNormal8));


                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase(":إجمالي ساعات العمل الإضافي", font));
                        lt_cell_31.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + sdata.logs[sdata.logs.Count() - 1].totalOvertimeHour, fNormal8));


                        PdfPCell lt_cell_32 = new PdfPCell(new Phrase(":إجمالي ساعات العمل", font));
                        lt_cell_32.RunDirection = PdfLayoutHelper.RunDirection;
                        tableEnd.AddCell(lt_cell_32);
                        tableEnd.AddCell(new Phrase(" " + ((sdata.logs[sdata.logs.Count() - 1].GrandTotal_Hour + sdata.logs[sdata.logs.Count() - 1].totalOvertimeHour) - sdata.logs[sdata.logs.Count() - 1].totalLateHours), fNormal8));

                        PdfPCell lt_cell_33 = new PdfPCell(new Phrase(":موازنة ساعات العمل", font));
                        lt_cell_33.RunDirection = PdfLayoutHelper.RunDirection;
                        //lt_cell_33.BackgroundColor = Color.LIGHT_GRAY;
                        tableEnd.AddCell(lt_cell_33);
                        tableEnd.AddCell(new Phrase(" " + ((sdata.logs[sdata.logs.Count() - 1].GrandTotal_Hour + sdata.logs[sdata.logs.Count() - 1].totalOvertimeHour - sdata.logs[sdata.logs.Count() - 1].totalLateHours) - sdata.logs[sdata.logs.Count() - 1].totalShfit_Hour), fNormal8));


                        document.Add(tableEnd);


                        Paragraph p_nsig = new Paragraph(".هذا تقرير تم إنشاؤه بواسطة النظام ولا يتطلب أي توقيع", font);
                        p_nsig.Alignment = Element.ALIGN_LEFT;
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);



                        Paragraph p_nsig2 = new Paragraph(new Phrase(":التقرير مطبوع بواسطة" + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        p_nsig2.SpacingBefore = 1;
                        // p_nsig.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Add(p_nsig2);


                        //PdfPCell p_nsig = new PdfPCell(new Phrase(".هذا تقرير تم إنشاؤه بواسطة النظام ولا يتطلب أي توقيع", font));
                        //p_nsig.RunDirection = PdfLayoutHelper.RunDirection;
                        //p_nsig.BackgroundColor = Color.LIGHT_GRAY;
                        //p_nsig.HorizontalAlignment = 1;
                        //document.Add(p_nsig);
                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Monthly-Working-Hours-Timesheet-Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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

        private ActionResult GenerateWorkHoursTimeSheetPDFAlt(BLL.PdfReports.CustomRangeTimeSheetData sdata)
        {
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

                    PdfPCell cellTitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource(strLogotitle[1] +"\n"+ "Ministry of Energy and Infrastructure"), fontTitle));
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


                    PdfPCell cellEName = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("dashboard.Name") +'\n'+ sdata.employeeName, font));
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

                    bool pdfRtl = PdfLayoutHelper.RunDirection == PdfWriter.RUN_DIRECTION_RTL;
                    tableEmployee.RunDirection = PdfLayoutHelper.RunDirection;













                    //PdfPCell cellETitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblmonthtitlehours"), font));
                    //cellETitle.Border = 0;
                    //cellETitle.RunDirection = PdfLayoutHelper.RunDirection;
                    //cellETitle.HorizontalAlignment = 2;

                    PdfPCell cellETitle = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("title.workhourstimesheetreport"), font));
                    cellETitle.Border = 0;
                    cellETitle.RunDirection = PdfLayoutHelper.RunDirection;
                    cellETitle.HorizontalAlignment = pdfRtl ? PdfPCell.ALIGN_RIGHT : PdfPCell.ALIGN_LEFT;

                    tableEmployee.AddCell(cellETitle);
                    tableEmployee.AddCell(tableEInfo);

                    document.Add(tableEmployee);

                    PdfPTable tableMid = new PdfPTable(new[] { 51.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f, 36.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;
                    tableMid.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell0 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.remarks"), font));
                    cell0.BackgroundColor = Color.LIGHT_GRAY;
                    cell0.HorizontalAlignment = 1;
                    cell0.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell1 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.statusout"), font));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    cell1.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell2 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.terminalout"), font));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    cell2.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell3 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.timeout"), font));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    cell3.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell4 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.statusin"), font));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    cell4.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell5 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.terminalin"), font));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    cell5.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell6 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.timein"), font));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    cell6.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell7 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.day"), font));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    cell7.RunDirection = PdfLayoutHelper.RunDirection;

                    PdfPCell cell8 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("report.date"), font));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    cell8.RunDirection = PdfLayoutHelper.RunDirection;

                    if (pdfRtl)
                    {
                        tableMid.AddCell(cell0);
                        tableMid.AddCell(cell1);
                        tableMid.AddCell(cell2);
                        tableMid.AddCell(cell3);
                        tableMid.AddCell(cell4);
                        tableMid.AddCell(cell5);
                        tableMid.AddCell(cell6);
                        tableMid.AddCell(cell7);
                        tableMid.AddCell(cell8);
                    }
                    else
                    {
                        tableMid.AddCell(cell8);
                        tableMid.AddCell(cell7);
                        tableMid.AddCell(cell6);
                        tableMid.AddCell(cell5);
                        tableMid.AddCell(cell4);
                        tableMid.AddCell(cell3);
                        tableMid.AddCell(cell2);
                        tableMid.AddCell(cell1);
                        tableMid.AddCell(cell0);
                    }

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

                        String statusOut = (log.remarks == "غياب" || log.remarks == "عطلة الاسبوع") ? "" : log.status.Split('$')[1];
                        statusOut = lookup.Keys.Contains(statusOut) ? lookup[statusOut] : statusOut;
                        Font fontForCell1 = new Font(font);
                        fontForCell1.Color = darkblueColor;
                        PdfPCell cellData1 = new PdfPCell(new Phrase(statusOut, fontForCell1));
                        cellData1.HorizontalAlignment = 1;
                        cellData1.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData1.BackgroundColor = color;

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.terminalOut, font));
                        cellData2.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData2.HorizontalAlignment = 1;
                        cellData2.BackgroundColor = color;

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        cellData3.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData3.BackgroundColor = color;

                        String statusIn = (log.remarks == "غياب" || log.remarks == "عطلة الاسبوع") ? "" : log.status.Split('$')[0];
                        statusIn = lookup.Keys.Contains(statusIn) ? lookup[statusIn] : statusIn;
                        Font fontForCell4 = new Font(font);
                        fontForCell4.Color = GreenColor;
                        PdfPCell cellData4 = new PdfPCell(new Phrase(statusIn, fontForCell4));
                        cellData4.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData4.HorizontalAlignment = 1;
                        cellData4.BackgroundColor = color;

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.terminalIn, font));
                        cellData5.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData5.HorizontalAlignment = 1;
                        cellData5.BackgroundColor = color;

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        cellData6.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData6.BackgroundColor = color;

                        PdfPCell cellData7 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource(log.day), font));
                        cellData7.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData7.HorizontalAlignment = 1;
                        cellData7.BackgroundColor = color;

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.date, fNormal8));
                        cellData8.HorizontalAlignment = 1;
                        cellData8.RunDirection = PdfLayoutHelper.RunDirection;
                        cellData8.BackgroundColor = color;

                        if (pdfRtl)
                        {
                            tableMid.AddCell(cellData0);
                            tableMid.AddCell(cellData1);
                            tableMid.AddCell(cellData2);
                            tableMid.AddCell(cellData3);
                            tableMid.AddCell(cellData4);
                            tableMid.AddCell(cellData5);
                            tableMid.AddCell(cellData6);
                            tableMid.AddCell(cellData7);
                            tableMid.AddCell(cellData8);
                        }
                        else
                        {
                            tableMid.AddCell(cellData8);
                            tableMid.AddCell(cellData7);
                            tableMid.AddCell(cellData6);
                            tableMid.AddCell(cellData5);
                            tableMid.AddCell(cellData4);
                            tableMid.AddCell(cellData3);
                            tableMid.AddCell(cellData2);
                            tableMid.AddCell(cellData1);
                            tableMid.AddCell(cellData0);
                        }

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

                        PdfPCell tesCell2 = new PdfPCell(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg") + "   " + sdata.employeeName + " - " + sdata.employeeCode, font));
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


                        document.Close();

                        return File(ms.ToArray(), "application/pdf", "Monthly-Working-Hours-Timesheet-Report-" + sdata.employeeCode + ".pdf");
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        document.Close();
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return null;
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

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);


                    }

                    if (sdata.logs.Length > 0)
                    {

                        document.Add(tableMid);

                        Paragraph p_summary = new Paragraph(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lblSummary.rpt"), font));
                        p_summary.SpacingBefore = 1;
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

                        Paragraph p_nsigremarks = new Paragraph(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg.remarks") + "   " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        p_nsigremarks.SpacingBefore = 1;
                        //p_nsigremarks.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Add(p_nsigremarks);


                        Paragraph p_nsig = new Paragraph(new Phrase(@MvcApplication1.ViewModel.GlobalVariables.GetStringResource("lbl.rpt.msg") + "   " + MvcApplication1.ViewModel.GlobalVariables.CheckNULLValidation(MvcApplication1.ViewModel.GlobalVariables.GV_EmployeeName), font));
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.RunDirection = PdfLayoutHelper.RunDirection;
                        document.Add(p_nsig);


                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.RunDirection = PdfLayoutHelper.RunDirection;
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Monthly-Working-Hours-Timesheet-Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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
        public JsonResult MonthlyWokringHourTimeSheetDataHandlerNew(MonthlyWokringHourTimesheetTable param)
        {
            try
            {

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
        #endregion
        #region MonthlyPunchedPhoto


        [HttpPost]
        public JsonResult MonthlyWokringHourTimeSheetDataHandler(MonthlyWokringHourTimesheetTable param)
        {
            try
            {

                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                int count = TimeTune.Reports.getMonthlyWorkingHoursTimesheetByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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


        #region MonthlyPunchedPhoto


        [HttpGet]
        [ActionName("MonthlyPunchedPhoto")]
        public ActionResult MonthlyPunchedPhoto_Get()
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
        [ActionName("MonthlyPunchedPhoto")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyPunchedPhoto_Post()
        {
            int employeeID = 0;
            BLL.PdfReports.MonthlyPunchedPhotoData lstPhotos = new BLL.PdfReports.MonthlyPunchedPhotoData();

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyTimeSheet");

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

            DateTime dtFrom = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0);
            DateTime dtTo = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month), 23, 59, 0);

            string rPath = "", uPath = "";
            rPath = GetAppSettingsValuByCode("TRMS_Real_Photos_Path");
            uPath = GetAppSettingsValuByCode("UNIS_Punched_Photos_Path");

            //lstPhotos = getPhotosListByDatePlusEmployeeCode(dtFrom, dtTo, employeeCode);
            lstPhotos = reportMaker.getPunchedPhoto(employeeID, dtFrom, dtTo, rPath, uPath);

            if (lstPhotos == null)
                return RedirectToAction("MonthlyPunchedPhoto");


            //ViewBag.PhotoBase64String = "data:image/png;base64," + Convert.ToBase64String(lstPhotos.originalPhoto, 0, lstPhotos.originalPhoto.Length);

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateMonthlyPunchedPhotoPDF(lstPhotos); // GenerateEvaluationPDF(toRender);

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

        public static string GetAppSettingsValuByCode(string strCode)
        {
            string strValue = "";

            if (ConfigurationManager.AppSettings[strCode] != null && ConfigurationManager.AppSettings[strCode].ToString() != "")
                strValue = ConfigurationManager.AppSettings[strCode].ToString();
            else
                strValue = "";

            return strValue;
        }

        private int GenerateMonthlyPunchedPhotoPDF(BLL.PdfReports.MonthlyPunchedPhotoData sdata)
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
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]); //Server.MapPath("~/Content/Logos/logo-default.png");
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
                    //////string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    ////////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    //////Image logo = Image.GetInstance(imageURL);
                    ////////logo.Width = 140.0f;
                    ////////logo.Alignment = Element.ALIGN_LEFT;
                    ////////logo.ScaleToFit(140f, 20f);
                    ////////logo.ScaleAbsolute(140f, 20f);
                    ////////logo.SpacingBefore = 5f;
                    ////////logo.SpacingAfter = 5f;

                    //////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //////tableHeader.WidthPercentage = 100;
                    //////tableHeader.HeaderRows = 0;
                    ////////tableHeader.SpacingBefore = 50;
                    //////tableHeader.SpacingAfter = 3;
                    //////tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    //////tableHeader.AddCell(logo);
                    //////tableHeader.AddCell("");

                    //////PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    ////////cellDateTime.HorizontalAlignment = 2;
                    //////cellDateTime.Border = 0;
                    //////tableHeader.AddCell(cellDateTime);

                    ////////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    //////document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(new[] { 40.0f, 20.0f, 40.0f });//total 595 - 10 x 2 due to Left and Right side margin
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

                    PdfPCell cellEMonth = new PdfPCell(new Phrase("Month Year: " + sdata.month, fBold9));
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);

                    //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.year, fBold9));
                    //cellEYear.Border = 0;
                    //tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    iTextSharp.text.Image pdfImage = null;
                    System.Drawing.Image imgPhoto2 = null; System.Drawing.Bitmap bmpPhoto = null;
                    if (sdata.originalPhoto != null)
                    {
                        /*
                        string base64String = Convert.ToBase64String(sdata.originalPhoto);
                        string _image = base64String.Replace("data:image/png;base64,", "");

                        byte[] bytesImg = Convert.FromBase64String(_image);

                        //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";
                        System.Drawing.Image img2 = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytesImg));
                        pdfImage = iTextSharp.text.Image.GetInstance(img2, System.Drawing.Imaging.ImageFormat.Jpeg);
                        */

                        //-----------------------------------------------

                        /*
                            //using (MemoryStream memorystream = new MemoryStream(sdata.originalPhoto, 0, sdata.originalPhoto.Length))
                            //{
                            //    imgPhoto2 = System.Drawing.Image.FromStream(memorystream);
                            //}

                            //imgPhoto2 = BytesToImage(sdata.originalPhoto);

                            //pdfImage = iTextSharp.text.Image.GetInstance(imgPhoto2, System.Drawing.Imaging.ImageFormat.Jpeg);
                        */



                        ////bmpPhoto = BytesToBitmap(sdata.originalPhoto, "unis.png");



                        //return ms.GetBuffer();//File(ms.GetBuffer(), @"image/png"); return type FileResult


                    }

                    Image photoReal = null;
                    if (sdata.realPhoto != null)
                    {
                        string photoURL = Server.MapPath(sdata.realPhoto);

                        photoReal = Image.GetInstance(photoURL);
                        photoReal.ScaleAbsolute(160f, 120f);//120x90 and 100x75
                    }

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Punched Photos Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(photoReal);////pdfImage OR photoReal
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
                    PdfPTable tableMid = new PdfPTable(new[] { 20.0f, 60.0f, 20.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Date Time In/Out", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    //PdfPCell cell2 = new PdfPCell(new Phrase("Time In/Out", fBold8));
                    //cell2.BackgroundColor = Color.LIGHT_GRAY;
                    //cell2.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Terminal ID - Name", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Captured Photo", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    foreach (BLL.PdfReports.MonthlyPunchedPhotoLog log in sdata.logs)
                    {
                        string strPhotoURL = Server.MapPath(log.photoRelativePath);
                        Image imgPhoto = Image.GetInstance(strPhotoURL);
                        imgPhoto.ScaleAbsolute(60f, 45f);//120x90 and 100x75 and 60x45
                        //imgPhoto.Width = 120.0f;

                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.date_time_in_out, fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        //PdfPCell cellData2 = new PdfPCell(new Phrase(log.timeInOut, fNormal8));
                        //cellData2.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.terminalName, fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        cellData3.Padding = 3f;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(imgPhoto);
                        cellData4.HorizontalAlignment = 1;
                        cellData4.Padding = 3f;
                        tableMid.AddCell(cellData4);
                    }

                    if (sdata.logs.Length > 0)
                    {
                        document.Add(tableMid);


                        // Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        //PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        //tableEnd.WidthPercentage = 100;
                        //tableEnd.HeaderRows = 0;
                        //tableEnd.SpacingBefore = 3;
                        //tableEnd.SpacingAfter = 3;

                        //PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        //tableEnd.AddCell(lt_cell_11);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        //PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Late:", fBold9));
                        //tableEnd.AddCell(lt_cell_21);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        //PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Absent:", fBold9));
                        //tableEnd.AddCell(lt_cell_31);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        //PdfPCell lt_cell_32 = new PdfPCell(new Phrase("Leave:", fBold9));
                        //tableEnd.AddCell(lt_cell_32);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        //PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        //tableEnd.AddCell(lt_cell_41);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        //PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        //tableEnd.AddCell(lt_cell_51);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        //document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        //Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, LV-Leave, PLO-Present Late & left On Time, PLE-Present Late & Early Out, POE-Present On Time & Early Out, PLM-Present Late & Miss Punch, PME-Present Miss Punch & Early Out, POM-Present On Time & Miss Punch, OV-Official Visit, OT-Official Travel, OM-Official Meeting, TR-Traning, *-Manually Updated", fNormal7);
                        //p_abrv.SpacingBefore = 1;
                        ////p_nsig.Alignment = 2;
                        //document.Add(p_abrv);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=PunchedPhotos-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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
                throw ex;
            }

            return reponse;
        }


        //Common - Main and Gallery
        private void ConvertToThumb(string strSrcFile, string strDestFile, int imageWidth)
        {
            String src = strSrcFile; //absolute location of source image
            String dest = strDestFile; //absolute location of the new image created(thumbnail)
            int thumbWidth = imageWidth; //width of the image (thumbnail) to produce

            // Get the source image to a System.Drawing.Image object
            System.Drawing.Image image = System.Drawing.Image.FromFile(src);

            // Create a System.Drawing.Bitmap with the desired width and height of the thumbnail.
            int srcWidth = image.Width;
            int srcHeight = image.Height;

            int thumbHeight = Convert.ToInt32((Convert.ToDouble(srcHeight) / Convert.ToDouble(srcWidth)) * Convert.ToDouble(thumbWidth));
            System.Drawing.Bitmap bmp;

            if (srcWidth < thumbWidth)
            {
                bmp = new System.Drawing.Bitmap(srcWidth, srcHeight);
            }
            else
            {
                bmp = new System.Drawing.Bitmap(thumbWidth, thumbHeight);
            }

            // Create a System.Drawing.Graphics object from the Bitmap which we will use to draw the high quality scaled image
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);

            // Set the System.Drawing.Graphics object property SmoothingMode to HighQuality
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // Set the System.Drawing.Graphics object property CompositingQuality to HighQuality
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            // Set the System.Drawing.Graphics object property InterpolationMode to High
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            // Draw the original image into the target Graphics object scaling to the desired width and height
            System.Drawing.Rectangle rectDestination;

            if (srcWidth < thumbWidth)
            {
                rectDestination = new System.Drawing.Rectangle(0, 0, srcWidth, srcHeight);
            }
            else
            {
                rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
            }

            gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, System.Drawing.GraphicsUnit.Pixel);

            // Save to destination file
            bmp.Save(dest);

            // dispose / release resources
            bmp.Dispose();
            image.Dispose();
            gr.Dispose();

        }

        public static System.Drawing.Image BytesToImage(byte[] byteArray)
        {

            //byte[] bitmap = null;
            try
            {

                string base64String = Convert.ToBase64String(byteArray);
                System.Drawing.Image convertToImage = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(base64String)));


                return convertToImage;

                //using (System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(byteArray, 0, byteArray.Length)))
                //{
                //    image.Save("/images/output.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);  // Or Png

                //    return image;
                //}
            }
            catch (Exception exx)
            {

                throw exx;
            }

        }

        [HttpPost]
        public JsonResult MonthlyPunchedPhotoDataHandler(MonthlyTimesheetReportTable param)
        {
            int empId = 0;
            DateTime dtFrom = DateTime.Now.AddDays(1), dtTo = DateTime.Now.AddDays(2);

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


                var data = new BLL.PdfReports.MonthlyPunchedPhotoData();

                // get all employee view models

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                if (param.from_date != null && param.from_date != "")
                    dtFrom = DateTime.ParseExact(param.from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                if (param.to_date != null && param.to_date != "")
                    dtTo = DateTime.ParseExact(param.to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                if (param.employee_id != null && param.employee_id != "")
                    empId = int.Parse(param.employee_id);

                //DateTime dtFrom = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0);
                //DateTime dtTo = new DateTime(monthDate.Year, monthDate.Month, DateTime.Now.Day, 23, 59, 0);

                BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();
                //data = getPhotosListByDatePlusEmployeeCode(dtFrom, dtTo, empId);
                string rPath = "", uPath = "";
                rPath = GetAppSettingsValuByCode("TRMS_Real_Photos_Path");
                uPath = GetAppSettingsValuByCode("UNIS_Punched_Photos_Path");
                data = reportMaker.getPunchedPhoto(empId, dtFrom, dtTo, rPath, uPath);

                if (data.originalPhoto != null)
                    Session["PhotoBase64String"] = "data:image/png;base64," + Convert.ToBase64String(data.originalPhoto, 0, data.originalPhoto.Length);
                else
                    Session["PhotoBase64String"] = null;

                if (data.realPhoto != null)
                    Session["PhotoRealEmployee"] = data.realPhoto;
                else
                    Session["PhotoRealEmployee"] = "/Content/UserDocs/demo-user.jpg";

                DTResult<BLL.PdfReports.MonthlyPunchedPhotoLog> result = new DTResult<BLL.PdfReports.MonthlyPunchedPhotoLog>
                {
                    draw = param.Draw,
                    data = data.logs.ToList(),
                    recordsFiltered = data.logs.Count(),
                    recordsTotal = data.logs.Count()
                };
                return Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }



        private static BLL.PdfReports.MonthlyPunchedPhotoData getPhotosListByDatePlusEmployeeCode(DateTime dtFromDate, DateTime dtToDate, string empCode)
        {
            int daysCount = 0;
            string uPath = "", empCodeFileName = "";
            uPath = GetAppSettingsValuByCode("UNIS_Punched_Photos_Path");
            empCodeFileName = empCode;
            List<FileInfo> fileinDir = new List<FileInfo>();
            BLL.PdfReports.MonthlyPunchedPhotoData toRender = new BLL.PdfReports.MonthlyPunchedPhotoData();
            List<BLL.PdfReports.MonthlyPunchedPhotoLog> lstPunched = new List<BLL.PdfReports.MonthlyPunchedPhotoLog>();

            daysCount = (dtToDate - dtFromDate).Days + 1;

            string[] directories = Directory.GetDirectories(uPath);
            for (int i = 0; i < daysCount; i++)
            {
                DateTime dt = dtFromDate;
                string dirPath = uPath + dt.ToString("yyyyMMdd");
                toRender.employeeCode = empCode;
                toRender.month = dt.ToString("MMM yyyy");

                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                if (dirInfo.Exists)
                {
                    //taking the first (FirstOrDefault()), considering that all files have a unique name with respect to the input value that you are giving. so it should fetch only one file every time you query
                    fileinDir = dirInfo.GetFiles(dt.ToString("yyyyMMdd") + "*" + empCodeFileName + ".jpg").ToList();

                    foreach (var f in fileinDir)
                    {
                        string strFileName = f.Name;//20200108070354_00000080_-0000001.jpg
                        if (strFileName != null && strFileName.Length > 25)
                        {
                            lstPunched.Add(new BLL.PdfReports.MonthlyPunchedPhotoLog()
                            {
                                eCode = empCode,
                                date = dt.ToString("dd-MM-yyyy"),
                                timeInOut = strFileName.Substring(8, 4),
                                photoName = strFileName,
                                terminalId = strFileName.Substring(15, 8)
                            });

                            Debug.WriteLine(f.FullName);
                        }
                    }
                }

                dtFromDate = dtFromDate.AddDays(1);
            }

            toRender.logs = lstPunched.ToArray();

            return toRender;
        }
        #endregion

        #region MonthlyGeoPhencing

        [HttpPost]
        public JsonResult MonthlyGeoPhencingReportDataHandler(MonthlyTimesheetReportTable param)
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


                var data = new List<GeoPhencingTerminal>();

                // get all employee view models

                //////IR 2222 getMonthlyTimesheetReportByEmployeeId()
                int count = TimeTune.Reports.getMonthlyGeoPhencingReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                List<GeoPhencingTerminal> data2 = GeoPhencingResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                count = GeoPhencingResultSet.Count(param.Search.Value, data2);

                DTResult<GeoPhencingTerminal> result = new DTResult<GeoPhencingTerminal>
                {
                    draw = param.Draw,
                    data = data2,
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

        [HttpGet]
        [ActionName("MonthlyGeoPhencing")]
        public ActionResult MonthlyGeoPhencing_Get()
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
        [ActionName("MonthlyGeoPhencing")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyGeoPhencing_Post()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyTimeSheet");

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getGeoPhencedReport(employeeID, month);

            if (toRender == null)
                return RedirectToAction("MonthlyTimeSheet");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateMonthlyGeoPhencingPDF(toRender); // GenerateEvaluationPDF(toRender);

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

        private int GenerateMonthlyGeoPhencingPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
        {
            int reponse = 0;

            try
            {

                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\Arial.ttf", BaseFont.IDENTITY_H, true);
                iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                using (MemoryStream ms = new MemoryStream())
                {


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

                        if (log.overtime2.Contains("*"))
                        {
                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminalIn, fNormal7Green));
                            //cellData7.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData7);
                        }
                        else
                        {
                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminalIn, fNormal7Red));
                            tableMid.AddCell(cellData7);
                        }

                        //PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                        //cellData8.HorizontalAlignment = 1;
                        if (log.overtime_status.Contains("*"))
                        {
                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalOut, fNormal7Green));
                            tableMid.AddCell(cellData8);
                        }
                        else
                        {
                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalOut, fNormal7Red));
                            tableMid.AddCell(cellData8);
                        }

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
                        Response.AddHeader("content-disposition", "attachment;filename=GeoPhencing-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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

        #region MonthlyCapturedPhotoReport

        [HttpPost]
        public JsonResult MonthlyCapturedPhotoReportDataHandler(MonthlyTimesheetReportTable param)
        {
            DateTime dtFrom = DateTime.Now.AddDays(1), dtTo = DateTime.Now.AddDays(2);

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

                if (param.from_date != null && param.from_date != "")
                    dtFrom = DateTime.ParseExact(param.from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                if (param.to_date != null && param.to_date != "")
                    dtTo = DateTime.ParseExact(param.to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                string rPath = "", uPath = "";
                rPath = GetAppSettingsValuByCode("TRMS_Real_Photos_Path");
                uPath = GetAppSettingsValuByCode("UNIS_Punched_Photos_Path");

                int count = TimeTune.Reports.getMonthlyCapturedPhotoReportByEmployeeId(param.employee_id, dtFrom, dtTo, rPath, uPath, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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

        [HttpGet]
        [ActionName("MonthlyCapturedPhoto")]
        public ActionResult MonthlyCapturedPhoto_Get()
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
        [ActionName("MonthlyCapturedPhoto")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyCapturedPhoto_Post()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("MonthlyCapturedPhoto");

            string month = Request.Form["month"];
            string rPath = "", uPath = "";

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

            DateTime dtFrom = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0);
            DateTime dtTo = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month), 23, 59, 0);
            rPath = GetAppSettingsValuByCode("TRMS_Real_Photos_Path");
            uPath = GetAppSettingsValuByCode("UNIS_Punched_Photos_Path");

            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getCapturedPhotoReport(employeeID, month, rPath, uPath, dtFrom, dtTo);

            if (toRender == null)
                return RedirectToAction("MonthlyCapturedPhoto");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            // ------------ Added by Inayat - 05th Dec 2017 ---------------------

            int found = 0; ViewData["PDFNoDataFound"] = "";
            found = GenerateMonthlyCapturedPhotoPDF(toRender); // GenerateEvaluationPDF(toRender);

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

        private int GenerateMonthlyCapturedPhotoPDF(BLL.PdfReports.MonthlyTimeSheetData sdata)
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
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    BLL.PdfReports.MonthlyTimeSheet reportLogoTitle = new BLL.PdfReports.MonthlyTimeSheet();
                    string[] strLogotitle = reportLogoTitle.getOrganizationLogoTitle().Split('^');

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath(strLogotitle[0]); //Server.MapPath("~/Content/Logos/logo-default.png");
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
                    //////string imageURL = Server.MapPath("~/images/bams-logo-pdf.png");
                    ////////string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    //////Image logo = Image.GetInstance(imageURL);
                    ////////logo.Width = 140.0f;
                    ////////logo.Alignment = Element.ALIGN_LEFT;
                    ////////logo.ScaleToFit(140f, 20f);
                    ////////logo.ScaleAbsolute(140f, 20f);
                    ////////logo.SpacingBefore = 5f;
                    ////////logo.SpacingAfter = 5f;

                    //////PdfPTable tableHeader = new PdfPTable(new[] { 70.0f, 320, 95.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    //////tableHeader.WidthPercentage = 100;
                    //////tableHeader.HeaderRows = 0;
                    ////////tableHeader.SpacingBefore = 50;
                    //////tableHeader.SpacingAfter = 3;
                    //////tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    //////tableHeader.AddCell(logo);
                    //////tableHeader.AddCell("");

                    //////PdfPCell cellDateTime = new PdfPCell(new Phrase("Date: " + DateTime.Now.ToShortDateString() + "\n\nTime: " + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    ////////cellDateTime.HorizontalAlignment = 2;
                    //////cellDateTime.Border = 0;
                    //////tableHeader.AddCell(cellDateTime);

                    ////////tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    //////document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(new[] { 40.0f, 20, 40.0f });//total 595 - 10 x 2 due to Left and Right side margin
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

                    PdfPCell cellEMonth = new PdfPCell(new Phrase("Month Year: " + sdata.month + " " + sdata.year, fBold9));
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);

                    //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.year, fBold9));
                    //cellEYear.Border = 0;
                    //tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    Image photoReal = null;
                    if (sdata.realPhoto != null && sdata.realPhoto != "")
                    {
                        string photoURL = Server.MapPath(sdata.realPhoto);

                        photoReal = Image.GetInstance(photoURL);
                        photoReal.ScaleAbsolute(100f, 75f);//160x120   120x90 and 100x75
                    }
                    else
                    {
                        string photoURL = Server.MapPath("/Content/UserDocs/demo-user.png");

                        photoReal = Image.GetInstance(photoURL);
                        photoReal.ScaleAbsolute(100f, 75f);//160x120   120x90 and 100x75
                    }

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Monthly Timesheet\n\nwith Captured Photos", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(photoReal);////pdfImage OR photoReal
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
                    PdfPTable tableMid = new PdfPTable(new[] { 50.0f, 50.0f, 60.0f, 50.0f, 60.0f, 175.0f, 60.0f, 60.0f });

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

                    PdfPCell cell6 = new PdfPCell(new Phrase("Final Remarks, Devices In and Out", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Photo In", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Photo Out", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    foreach (BLL.PdfReports.MonthlyTimeSheetLog log in sdata.logs)
                    {
                        {
                            log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                        }

                        Image imgPhoto1 = null;
                        if (log.inPhoto != null && log.inPhoto != "")
                        {
                            string strPhotoURL1 = log.inPhoto; // Server.MapPath(log.inPhoto);
                            imgPhoto1 = Image.GetInstance(strPhotoURL1);
                            imgPhoto1.ScaleAbsolute(48f, 36f);//60f, 45f        120x90 and 100x75 and 60x45
                            //imgPhoto1.Width = 120.0f;
                        }
                        else
                        {
                            string strPhotoURL1 = Server.MapPath("/content/userdocs/not_found.png");//blank_48x36
                            imgPhoto1 = Image.GetInstance(strPhotoURL1);
                            imgPhoto1.ScaleAbsolute(48f, 36f);//120x90 and 100x75 and 60x45
                            //imgPhoto1.Width = 120.0f;
                        }

                        Image imgPhoto2 = null;
                        if (log.outPhoto != null && log.outPhoto != "")
                        {
                            string strPhotoURL2 = log.outPhoto; // Server.MapPath(log.outPhoto);
                            imgPhoto2 = Image.GetInstance(strPhotoURL2);
                            imgPhoto2.ScaleAbsolute(48f, 36f);//120x90 and 100x75 and 60x45
                            //imgPhoto2.Width = 120.0f;
                        }
                        else
                        {
                            string strPhotoURL2 = Server.MapPath("/content/userdocs/not_found.png");//blank_48x36
                            imgPhoto2 = Image.GetInstance(strPhotoURL2);
                            imgPhoto2.ScaleAbsolute(48f, 36f);//120x90 and 100x75 and 60x45
                            //imgPhoto2.Width = 120.0f;
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

                        PdfPCell cellData6 = new PdfPCell(new Phrase("FINAL REMARKS: " + log.finalRemarks + "\nIN: " + log.terminalIn + "\nOUT: " + log.terminalOut, fNormal8));
                        //cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);


                        PdfPCell cellData7 = new PdfPCell(imgPhoto1);
                        cellData7.HorizontalAlignment = 1;
                        cellData7.Padding = 3f;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(imgPhoto2);
                        //PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                        cellData8.HorizontalAlignment = 1;
                        cellData8.Padding = 3f;
                        tableMid.AddCell(cellData8);

                        //PdfPCell cellData17 = new PdfPCell(new Phrase(log.terminalIn, fNormal7));
                        ////cellData17.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData17);

                        //PdfPCell cellData18 = new PdfPCell(new Phrase(log.terminalOut, fNormal7));
                        ////PdfPCell cellData18 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                        ////cellData18.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData18);
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
                        Response.AddHeader("content-disposition", "attachment;filename=MonthlyCapturedPhoto-Report-" + sdata.employeeCode + ".pdf");
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




        #region Chart-Helper Component

        public class OrderModel
        {
            public string ShipCity { get; set; }
            public int TotalOrders { get; set; }
        }

        public class PieChartModel
        {
            public string TypeName { get; set; }
            public decimal TypePercent { get; set; }
            public string TypeTitle { get; set; }
        }

        public class ColumnChartModel
        {
            public string TypeName { get; set; }
            public decimal TypePercent { get; set; }
            public string TypeTitle { get; set; }
        }

        // Summary:
        //     A theme for 2D charting that features a visual container with a blue gradient,
        //     rounded edges, drop-shadowing, and high-contrast gridlines.
        public const string Blude3d =
      @"<Chart BackColor =""#D3DFF0"" BackGradientStyle=""TopBottom"" BackSecondaryColor=""White"" BorderColor=""26, 59, 105"" BorderlineDashStyle=""Solid"" BorderWidth=""2"" Palette=""None"" PaletteCustomColors=""97,142,206; 209,98,96; 168,203,104; 142,116,178; 93,186,215; 255,155,83; 148,172,215; 217,148,147; 189,213,151; 173,158,196; 145,201,221; 255,180,138"">
    <ChartAreas> <ChartArea Name =""Default"" _Template_=""All"" BackColor=""64, 165, 191, 228"" BackGradientStyle=""TopBottom"" BackSecondaryColor=""White"" BorderColor=""64, 64, 64, 64"" BorderDashStyle=""Solid"" ShadowColor=""Transparent"">
            <Area3DStyle LightStyle =""Simplistic"" Enable3D=""True"" Inclination=""5"" IsClustered=""True"" IsRightAngleAxes=""True"" Perspective=""5"" Rotation=""0"" WallWidth=""0"" />
        </ChartArea></ChartAreas><Legends> <Legend _Template_ =""All"" BackColor=""Transparent"" Font=""Trebuchet MS, 8.25pt, style=Bold"" IsTextAutoFit=""False"" />   </Legends>  <BorderSkin SkinStyle =""Emboss"" /> </Chart>";

        public const string Presence3d =//E1FDFC
     @"<Chart BackColor =""#BAE6F5"" BackGradientStyle=""TopBottom"" BackSecondaryColor=""White"" BorderColor=""59, 175, 218"" BorderlineDashStyle=""Solid"" BorderWidth=""0"" Palette=""None"" PaletteCustomColors=""102,187,106; 114,102,186; 238,110,115; 41,182,246; 255,193,7; 247,99,151; 148,172,215; 217,148,147; 189,213,151; 173,158,196; 145,201,221; 255,180,138"">
    <ChartAreas> <ChartArea Name =""Default"" _Template_=""All"" BackColor=""64, 165, 191, 228"" BackGradientStyle=""TopBottom"" BackSecondaryColor=""White"" BorderColor=""64, 64, 64, 64"" BorderDashStyle=""Solid"" ShadowColor=""Transparent"">
            <Area3DStyle LightStyle =""Simplistic"" Enable3D=""True"" Inclination=""5"" IsClustered=""True"" IsRightAngleAxes=""True"" Perspective=""30"" Rotation=""0"" WallWidth=""0"" />
        </ChartArea></ChartAreas><Legends> <Legend _Template_ =""All"" BackColor=""Transparent"" Font=""Arial, 8.25pt, style=Bold"" IsTextAutoFit=""False"" />   </Legends>  <BorderSkin SkinStyle =""Emboss"" /> </Chart>";

        public const string Punctual3d =
  @"<Chart BackColor =""#BAE6F5"" BackGradientStyle=""TopBottom"" BackSecondaryColor=""White"" BorderColor=""59, 175, 218"" BorderlineDashStyle=""Solid"" BorderWidth=""0"" Palette=""None"" PaletteCustomColors=""255,193,7; 247,99,151; 148,172,215; 102,187,106; 114,102,186; 238,110,115; 41,182,246; 217,148,147; 189,213,151; 173,158,196; 145,201,221; 255,180,138"">
    <ChartAreas> <ChartArea Name =""Default"" _Template_=""All"" BackColor=""64, 165, 191, 228"" BackGradientStyle=""TopBottom"" BackSecondaryColor=""White"" BorderColor=""64, 64, 64, 64"" BorderDashStyle=""Solid"" ShadowColor=""Transparent"">
            <Area3DStyle LightStyle =""Simplistic"" Enable3D=""True"" Inclination=""5"" IsClustered=""True"" IsRightAngleAxes=""True"" Perspective=""30"" Rotation=""0"" WallWidth=""0"" />
        </ChartArea></ChartAreas><Legends> <Legend _Template_ =""All"" BackColor=""Transparent"" Font=""Arial, 8.25pt, style=Bold"" IsTextAutoFit=""False"" />   </Legends>  <BorderSkin SkinStyle =""Emboss"" /> </Chart>";

        private static byte[] PopulateChart()
        {
            string query = "SELECT TOP 5 ShipCity, SUM(OrdersCount) TotalOrders";
            query += " FROM Orders WHERE ShipCountry = 'USA' GROUP BY ShipCity";
            string constr = ConfigurationManager.ConnectionStrings["TimeTune"].ConnectionString;
            List<OrderModel> chartData = new List<OrderModel>();

            /*
            // Script for SelectTopNRows command from SSMS 
            SELECT TOP 1000[Id]
            ,[ShipCity]         --A,B,D,C,AA,BB
            ,[ShipCountry]      --USA,USA,USA, Canada,Canada
            ,[OrdersCount]      --2,3,1,4,2,3,1
            FROM[TRMS_DUHS].[dbo].[Orders]
            */

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            chartData.Add(new OrderModel
                            {
                                ShipCity = sdr["ShipCity"].ToString(),
                                TotalOrders = Convert.ToInt32(sdr["TotalOrders"])
                            });
                        }
                    }

                    con.Close();
                }
            }

            Chart chart = new Chart(width: 220, height: 220, theme: ChartTheme.Blue);
            chart.AddTitle("Presence / Punctuality");
            chart.AddSeries("Default", chartType: "Pie", xValue: chartData, xField: "ShipCity", yValues: chartData, yFields: "TotalOrders");
            return chart.GetBytes(format: "jpeg");
        }

        private static byte[] GetPresenceChartData()
        {
            //ViewModel.GlobalVariables.GV_DepartmentPerformancePercent = "P=" + perPresent + ",L=" + perLeave + ",A=" + perAbsent + 
            //",O=" + perOnTime + ",T=" + perLate + ",E=" + perEarly;

            string vbChartData = "";
            vbChartData = ViewModel.GlobalVariables.GV_DepartmentPerformancePercent;

            if (vbChartData != null && vbChartData != "")
            {
                vbChartData = vbChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").
                                             Replace("O=", "").Replace("T=", "").Replace("E=", "");

                string[] strArr = vbChartData.Split(',');

                List<PieChartModel> chartData = new List<PieChartModel>();
                chartData.Add(new PieChartModel
                {
                    TypeName = "Present",
                    TypePercent = Convert.ToDecimal(strArr[0]),
                    TypeTitle = "" + strArr[0] + "%"
                });
                chartData.Add(new PieChartModel
                {
                    TypeName = "Leave",
                    TypePercent = Convert.ToDecimal(strArr[1]),
                    TypeTitle = "" + strArr[1] + "%"
                });
                chartData.Add(new PieChartModel
                {
                    TypeName = "Absent",
                    TypePercent = Convert.ToDecimal(strArr[2]),
                    TypeTitle = "" + strArr[2] + "%"
                });

                Chart chartPresence = new Chart(width: 200, height: 200, theme: Presence3d);
                chartPresence.AddTitle("Presence Status");
                chartPresence.AddSeries("Default", chartType: "Pie", xValue: chartData, xField: "TypeTitle", yValues: chartData, yFields: "TypePercent", axisLabel: "TypeTitle", legend: "Default");
                //chartPresence.AddLegend("Default");
                //chartPresence.SetXAxis("A");
                //chartPresence.SetYAxis("B");

                return chartPresence.GetBytes(format: "png");

            }
            else
            {
                return null;
            }
        }

        private static byte[] GetPunctualityChartData()
        {
            //ViewModel.GlobalVariables.GV_DepartmentPerformancePercent = "P=" + perPresent + ",L=" + perLeave + ",A=" + perAbsent + 
            //",O=" + perOnTime + ",T=" + perLate + ",E=" + perEarly;

            string vbChartData = "";
            vbChartData = ViewModel.GlobalVariables.GV_DepartmentPerformancePercent;

            if (vbChartData != null && vbChartData != "")
            {
                vbChartData = vbChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").
                                             Replace("O=", "").Replace("T=", "").Replace("E=", "");

                string[] strArr = vbChartData.Split(',');

                List<PieChartModel> chartData = new List<PieChartModel>();
                chartData.Add(new PieChartModel
                {
                    TypeName = "On Time",
                    TypePercent = Convert.ToDecimal(strArr[3]),
                    TypeTitle = "" + strArr[3] + "%"
                });
                chartData.Add(new PieChartModel
                {
                    TypeName = "Late",
                    TypePercent = Convert.ToDecimal(strArr[4]),
                    TypeTitle = "" + strArr[4] + "%"
                });
                chartData.Add(new PieChartModel
                {
                    TypeName = "Early Out",
                    TypePercent = Convert.ToDecimal(strArr[5]),
                    TypeTitle = "" + strArr[5] + "%"
                });

                Chart chartPunctual = new Chart(width: 200, height: 200, theme: Punctual3d);
                // chartPunctual.AddLegend(name: "Default", title: "ABC");
                chartPunctual.AddTitle("Punctuality Status");
                chartPunctual.AddSeries("Default", chartType: "Pie", xValue: chartData, xField: "TypeTitle", yValues: chartData, yFields: "TypePercent", axisLabel: "TypeTitle", legend: "Default");

                return chartPunctual.GetBytes(format: "png");
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region Chart-Visual-Data Component

        public byte[] CreateVisualChart(int chartGroupID, string strChartData)//SeriesChartType.Pie
        {
            System.Web.UI.DataVisualization.Charting.SeriesChartType chartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Doughnut;

            List<PieChartModel> chartData = new List<PieChartModel>(); // _resultService.GetResults();
            ////string vbChartData = "";
            ////vbChartData = ViewModel.GlobalVariables.GV_DepartmentPerformancePercent; //"P=15.35,L=20.00,A=64.65,O=44.02,T=42.54,E=13.44";

            if (strChartData != null && strChartData != "" && strChartData.Contains(","))
            {
                ////strChartData = strChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("E=", "");

                string[] strArr = strChartData.Split(',');

                if (chartGroupID == 0)
                {
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Present",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                    });
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Leave",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                    });
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Absent",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                    });
                }
                else if (chartGroupID == 1)
                {
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "On Time",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                    });
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Late",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                    });
                }
                else if (chartGroupID == 2)
                {
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Out Time",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                    });
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Early Out",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                    });
                }
                else
                {
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Present",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                    });
                    chartData.Add(new PieChartModel
                    {
                        TypeName = "Absent",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                    });
                }

                System.Web.UI.DataVisualization.Charting.Chart chart = new System.Web.UI.DataVisualization.Charting.Chart();
                chart.Width = 300;
                chart.Height = 300;
                //chart.BackColor = System.Drawing.Color.FromArgb(182, 225, 241);
                //chart.BorderlineDashStyle = System.Web.UI.DataVisualization.Charting.ChartDashStyle.Solid;
                //chart.BackSecondaryColor = System.Drawing.Color.White;
                //chart.BackGradientStyle = System.Web.UI.DataVisualization.Charting.GradientStyle.TopBottom;
                //chart.BorderlineWidth = 1;
                //chart.Palette = System.Web.UI.DataVisualization.Charting.ChartColorPalette.BrightPastel;
                //chart.BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105);
                chart.RenderType = System.Web.UI.DataVisualization.Charting.RenderType.BinaryStreaming;
                //chart.BorderSkin.SkinStyle = System.Web.UI.DataVisualization.Charting.BorderSkinStyle.Emboss;
                chart.AntiAliasing = System.Web.UI.DataVisualization.Charting.AntiAliasingStyles.All;
                chart.TextAntiAliasingQuality = System.Web.UI.DataVisualization.Charting.TextAntiAliasingQuality.Normal;

                if (chartGroupID == 0)
                    chart.Titles.Add(CreateTitle("Presence Status"));
                else if (chartGroupID == 1)
                    chart.Titles.Add(CreateTitle("Punctuality Status"));
                else if (chartGroupID == 2)
                    chart.Titles.Add(CreateTitle("Exit Status"));
                else
                    chart.Titles.Add(CreateTitle("Today's Status"));

                if (chartGroupID == 0)
                    chart.Legends.Add(CreateLegend("Presence Status"));
                else if (chartGroupID == 1)
                    chart.Legends.Add(CreateLegend("Punctuality Status"));
                else if (chartGroupID == 2)
                    chart.Legends.Add(CreateLegend("Exit Status"));
                else
                    chart.Legends.Add(CreateLegend("Today's Status"));

                if (chartGroupID == 0)
                    chart.Series.Add(CreateSeries(chartData, chartType, "Presence Status", chartGroupID));
                else if (chartGroupID == 1)
                    chart.Series.Add(CreateSeries(chartData, chartType, "Punctuality Status", chartGroupID));
                else if (chartGroupID == 2)
                    chart.Series.Add(CreateSeries(chartData, chartType, "Exit Status", chartGroupID));
                else
                    chart.Series.Add(CreateSeries(chartData, chartType, "Today's Status", chartGroupID));

                if (chartGroupID == 0)
                    chart.ChartAreas.Add(CreateChartArea("Presence Status"));
                else if (chartGroupID == 1)
                    chart.ChartAreas.Add(CreateChartArea("Punctuality Status"));
                else if (chartGroupID == 2)
                    chart.ChartAreas.Add(CreateChartArea("Exit Status"));
                else
                    chart.ChartAreas.Add(CreateChartArea("Today's Status"));

                MemoryStream ms = new MemoryStream();
                chart.SaveImage(ms);

                if (chartGroupID == 0)
                    BytesToBitmap(ms.GetBuffer(), "presence.png");
                else if (chartGroupID == 1)
                    BytesToBitmap(ms.GetBuffer(), "punctuality.png");
                else if (chartGroupID == 2)
                    BytesToBitmap(ms.GetBuffer(), "punctuality.png");
                else
                    BytesToBitmap(ms.GetBuffer(), "presence.png");


                return ms.GetBuffer();//File(ms.GetBuffer(), @"image/png"); return type FileResult
            }

            return null;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.Title CreateTitle(string strName)
        {
            System.Web.UI.DataVisualization.Charting.Title title = new System.Web.UI.DataVisualization.Charting.Title();
            title.Text = strName; // "Organization Chart";
            //title.ShadowColor = System.Drawing.Color.FromArgb(32, 0, 0, 0);
            title.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);//Trebuchet MS
            //title.ShadowOffset = 0;//3
            title.ForeColor = System.Drawing.Color.FromArgb(16, 36, 63);

            return title;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.Legend CreateLegend(string strName)
        {
            System.Web.UI.DataVisualization.Charting.Legend legend = new System.Web.UI.DataVisualization.Charting.Legend();
            legend.Name = strName; // "Organization Chart";
            legend.Docking = System.Web.UI.DataVisualization.Charting.Docking.Bottom;
            legend.Alignment = System.Drawing.StringAlignment.Center;
            legend.BackColor = System.Drawing.Color.Transparent;
            legend.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Arial"), 9);//Trebuchet MS
            legend.LegendStyle = System.Web.UI.DataVisualization.Charting.LegendStyle.Row;

            return legend;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.Series CreateSeries(IList<PieChartModel> results, System.Web.UI.DataVisualization.Charting.SeriesChartType chartType, string strName, int chartGroupID)
        {
            System.Web.UI.DataVisualization.Charting.Series seriesDetail = new System.Web.UI.DataVisualization.Charting.Series();
            seriesDetail.Name = strName; // "Organization Chart";
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = System.Drawing.Color.FromArgb(198, 99, 99);
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "Default";//SoftEdge,Concave,Default

            if (chartGroupID == 0)
                seriesDetail["DoughnutRadius"] = "55"; // Set Doughnut hole size
            else if (chartGroupID == 1 || chartGroupID == 2)
                seriesDetail["DoughnutRadius"] = "55"; // Set Doughnut hole size
            else
                seriesDetail["DoughnutRadius"] = "60"; // Set Doughnut hole size

            //seriesDetail["PieLabelStyle"] = "Inside"; //Outside,Disabled

            System.Web.UI.DataVisualization.Charting.DataPoint point;

            int a = 0;
            foreach (PieChartModel result in results)
            {
                point = new System.Web.UI.DataVisualization.Charting.DataPoint();
                point.AxisLabel = result.TypeTitle;

                point.LegendText = result.TypeName;
                point.YValues = new double[] { double.Parse(result.TypePercent.ToString()) };

                if (strName.Contains("Presence"))
                {
                    //"102,187,106; 114,102,186; 238,110,115; 41,182,246; 255,193,7; 247,99,151; 
                    if (a == 0)
                        point.Color = System.Drawing.Color.FromArgb(102, 187, 106);//41,182,246
                    else if (a == 1)
                        point.Color = System.Drawing.Color.FromArgb(114, 102, 186);//255,193,7
                    else
                        point.Color = System.Drawing.Color.FromArgb(238, 110, 115);//247,99,151
                }
                else if (strName.Contains("Punctuality"))
                {
                    //"102,187,106; 114,102,186; 238,110,115; 41,182,246; 255,193,7; 247,99,151; 
                    if (a == 0)
                        point.Color = System.Drawing.Color.FromArgb(41, 182, 246);//41,182,246
                    else if (a == 1)
                        point.Color = System.Drawing.Color.FromArgb(255, 193, 7);//255,193,7
                    else
                        point.Color = System.Drawing.Color.FromArgb(247, 99, 151);//247,99,151
                }
                else if (strName.Contains("Exit"))
                {
                    //"102,187,106; 114,102,186; 238,110,115; 41,182,246; 255,193,7; 247,99,151; 
                    if (a == 0)
                        point.Color = System.Drawing.Color.FromArgb(255, 217, 136);//41,182,246
                    else if (a == 1)
                        point.Color = System.Drawing.Color.FromArgb(247, 99, 151);//247,99,151
                    else
                        point.Color = System.Drawing.Color.FromArgb(255, 193, 7);//255,193,7
                }
                else
                {
                    //"102,187,106; 114,102,186; 238,110,115; 41,182,246; 255,193,7; 247,99,151; 
                    if (a == 0)
                        point.Color = System.Drawing.Color.FromArgb(102, 187, 106);//41,182,246
                    else if (a == 1)
                        point.Color = System.Drawing.Color.FromArgb(238, 110, 115);//255,193,7
                    else
                        point.Color = System.Drawing.Color.FromArgb(114, 102, 186);//247,99,151
                }

                seriesDetail.Points.Add(point);

                a++;
            }
            seriesDetail.ChartArea = strName; // "Organization Chart";

            return seriesDetail;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.ChartArea CreateChartArea(string strName)
        {
            System.Web.UI.DataVisualization.Charting.ChartArea chartArea = new System.Web.UI.DataVisualization.Charting.ChartArea();
            chartArea.Name = strName; // "Organization Chart";
            chartArea.BackColor = System.Drawing.Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial,Verdana,Helvetica,sans-serif", 8F, System.Drawing.FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Arial,Verdana,Helvetica,sans-serif", 8F, System.Drawing.FontStyle.Regular);
            chartArea.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.LabelStyle.Angle = 90;
            //chartArea.AxisY.LabelStyle.Angle = 90;
            chartArea.AxisX.Interval = 1;
            //chartArea.Area3DStyle.Rotation = 90;
            // chartArea.Position.Width = 98;
            // chartArea.Position.Height = 70;
            // chartArea.Position.Y = 15;
            //chartArea.Position.X = 90;
            chartArea.Area3DStyle.Enable3D = true;
            chartArea.Area3DStyle.Rotation = -90;
            chartArea.Area3DStyle.Perspective = 0;
            chartArea.Area3DStyle.Inclination = 0;
            chartArea.Area3DStyle.IsRightAngleAxes = true;
            chartArea.Area3DStyle.WallWidth = 0;
            chartArea.Area3DStyle.IsClustered = false;

            //chartArea.Area3DStyle.Rotation = -90;

            return chartArea;
        }

        #endregion


        #region Column-Chart-Visual-Data Component

        public byte[] CreateVisualColumnChart(int chartGroupID, string strChartData)//SeriesChartType.Pie
        {
            System.Web.UI.DataVisualization.Charting.SeriesChartType chartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;

            List<ColumnChartModel> chartData = new List<ColumnChartModel>(); // _resultService.GetResults();
            List<ColumnChartModel> chartData01 = new List<ColumnChartModel>(); // _resultService.GetResults();
            List<ColumnChartModel> chartData02 = new List<ColumnChartModel>(); // _resultService.GetResults();
            //List<ColumnChartModel> chartData03 = new List<ColumnChartModel>(); // _resultService.GetResults();
            //List<ColumnChartModel> chartData04 = new List<ColumnChartModel>(); // _resultService.GetResults();

            //List<ColumnChartModel> chartData05 = new List<ColumnChartModel>(); // _resultService.GetResults();
            //List<ColumnChartModel> chartData06 = new List<ColumnChartModel>(); // _resultService.GetResults();
            //List<ColumnChartModel> chartData07 = new List<ColumnChartModel>(); // _resultService.GetResults();
            //List<ColumnChartModel> chartData08 = new List<ColumnChartModel>(); // _resultService.GetResults();

            ////string vbChartData = "";
            ////vbChartData = ViewModel.GlobalVariables.GV_DepartmentPerformancePercent; //"P=15.35,L=20.00,A=64.65,O=44.02,T=42.54,E=13.44";

            if (strChartData != null && strChartData != "" && strChartData.Contains(","))
            {
                //Titles - Leave Types
                string TitleLeaves = "", TitleLeave01 = "", TitleLeave02 = "", TitleLeave03 = "", TitleLeave04 = "", TitleLeave05 = "", TitleLeave06 = "", TitleLeave07 = "", TitleLeave08 = "";
                string LTCS = "", TitleLeave09 = "", TitleLeave10 = "", TitleLeave11 = "", TitleLeave12 = "", TitleLeave13 = "", TitleLeave14 = "", TitleLeave15 = "";
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

                ////strChartData = strChartData.Replace("P=", "").Replace("L=", "").Replace("A=", "").Replace("O=", "").Replace("T=", "").Replace("E=", "");

                string[] strArr = strChartData.Split(',');

                if (chartGroupID == 0)
                {
                    //testing
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Used",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                    });
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Used",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                    });
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Used",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                    });

                    //real data
                    if (strArr.Length == 2)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 4)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 6)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 8)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 10)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 12)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 14)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 16)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 18)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[16]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[16]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[17]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[17]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 20)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[16]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[16]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[17]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[17]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[18]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[18]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[19]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[19]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 22)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[16]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[16]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[17]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[17]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[18]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[18]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[19]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[19]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[20]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[20]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[21]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[21]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 24)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[16]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[16]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[17]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[17]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[18]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[18]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[19]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[19]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[20]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[20]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[21]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[21]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[22]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[22]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[23]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[23]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 26)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[16]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[16]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[17]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[17]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[18]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[18]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[19]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[19]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[20]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[20]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[21]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[21]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[22]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[22]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[23]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[23]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave13, // "L09",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[24]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[24]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave13, // "L09",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[25]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[25]), 0) + "%"
                        });

                    }
                    else if (strArr.Length == 28)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[16]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[16]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[17]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[17]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[18]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[18]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[19]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[19]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[20]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[20]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[21]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[21]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[22]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[22]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[23]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[23]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave13, // "L09",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[24]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[24]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave13, // "L09",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[25]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[25]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave14, // "L10",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[26]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[26]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave14, // "L10",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[27]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[27]), 0) + "%"
                        });
                    }
                    else if (strArr.Length == 30)
                    {
                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave01, // "Sick",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[2]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[2]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave02, // "Casual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, // "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave03, //  "Annual",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave04, // "Other",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[7]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[7]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[8]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[8]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave05, // "Tour",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[9]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[9]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[10]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[10]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave06, // "Visit",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[11]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[11]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[12]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[12]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave07, // "Meeting",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[13]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[13]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[14]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[14]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave08, // "Maternity",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[15]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[15]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[16]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[16]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave09, // "L05",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[17]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[17]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[18]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[18]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave10, // "L06",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[19]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[19]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[20]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[20]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave11, // "L07",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[21]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[21]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[22]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[22]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave12, // "L08",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[23]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[23]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave13, // "L09",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[24]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[24]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave13, // "L09",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[25]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[25]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave14, // "L10",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[26]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[26]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave14, // "L10",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[27]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[27]), 0) + "%"
                        });

                        chartData01.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave15, // "L11",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[28]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[28]), 0) + "%"
                        });

                        chartData02.Add(new ColumnChartModel
                        {
                            TypeName = TitleLeave15, // "L11",
                            TypePercent = Math.Round(Convert.ToDecimal(strArr[29]), 0),
                            TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[29]), 0) + "%"
                        });

                    }

                }
                else if (chartGroupID == 1)
                {
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "On Time",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[3]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[3]), 0) + "%"
                    });
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Late",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[4]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[4]), 0) + "%"
                    });
                }
                else if (chartGroupID == 2)
                {
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Out Time",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[5]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[5]), 0) + "%"
                    });
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Early Out",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[6]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[6]), 0) + "%"
                    });
                }
                else
                {
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Present",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[0]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[0]), 0) + "%"
                    });
                    chartData.Add(new ColumnChartModel
                    {
                        TypeName = "Absent",
                        TypePercent = Math.Round(Convert.ToDecimal(strArr[1]), 0),
                        TypeTitle = "" + Math.Round(Convert.ToDecimal(strArr[1]), 0) + "%"
                    });
                }

                System.Web.UI.DataVisualization.Charting.Chart chart = new System.Web.UI.DataVisualization.Charting.Chart();
                chart.Width = 800;
                chart.Height = 400;
                //chart.BackColor = System.Drawing.Color.FromArgb(182, 225, 241);
                //chart.BorderlineDashStyle = System.Web.UI.DataVisualization.Charting.ChartDashStyle.Solid;
                //chart.BackSecondaryColor = System.Drawing.Color.White;
                //chart.BackGradientStyle = System.Web.UI.DataVisualization.Charting.GradientStyle.TopBottom;
                //chart.BorderlineWidth = 1;

                chart.Palette = System.Web.UI.DataVisualization.Charting.ChartColorPalette.Fire;//BrightPastel
                                                                                                //System.Drawing.Color[] cColors = new System.Drawing.Color[3];
                                                                                                //cColors[0] = System.Drawing.Color.Green; cColors[1] = System.Drawing.Color.Red; cColors[2] = System.Drawing.Color.Blue;
                                                                                                //chart.PaletteCustomColors = cColors;

                //chart.BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105);
                chart.RenderType = System.Web.UI.DataVisualization.Charting.RenderType.BinaryStreaming;
                //chart.BorderSkin.SkinStyle = System.Web.UI.DataVisualization.Charting.BorderSkinStyle.Emboss;
                chart.AntiAliasing = System.Web.UI.DataVisualization.Charting.AntiAliasingStyles.All;
                chart.TextAntiAliasingQuality = System.Web.UI.DataVisualization.Charting.TextAntiAliasingQuality.Normal;

                if (chartGroupID == 0)
                    chart.Titles.Add(CreateColumnChartTitle("Leaves Status"));
                else if (chartGroupID == 1)
                    chart.Titles.Add(CreateColumnChartTitle("Punctuality Status"));
                else if (chartGroupID == 2)
                    chart.Titles.Add(CreateColumnChartTitle("Exit Status"));
                else
                    chart.Titles.Add(CreateColumnChartTitle("Today's Status"));

                if (chartGroupID == 0)
                {
                    chart.Legends.Add(CreateColumnChartLegend("Availed"));
                    chart.Legends.Add(CreateColumnChartLegend("Allowed"));
                }
                else if (chartGroupID == 1)
                    chart.Legends.Add(CreateColumnChartLegend("Punctuality Status"));
                else if (chartGroupID == 2)
                    chart.Legends.Add(CreateColumnChartLegend("Exit Status"));
                else
                    chart.Legends.Add(CreateColumnChartLegend("Today's Status"));

                if (chartGroupID == 0)
                {
                    chart.Series.Add(CreateColumnChartSeries(chartData01, chartType, "Availed", chartGroupID, 1));
                    chart.Series.Add(CreateColumnChartSeries(chartData02, chartType, "Allowed", chartGroupID, 2));
                }
                else if (chartGroupID == 1)
                    chart.Series.Add(CreateColumnChartSeries(chartData, chartType, "Punctuality Status", chartGroupID, 0));
                else if (chartGroupID == 2)
                    chart.Series.Add(CreateColumnChartSeries(chartData, chartType, "Exit Status", chartGroupID, 0));
                else
                    chart.Series.Add(CreateColumnChartSeries(chartData, chartType, "Today's Status", chartGroupID, 0));

                if (chartGroupID == 0)
                {
                    chart.ChartAreas.Add(CreateColumnChartArea("Sick"));
                }
                else if (chartGroupID == 1)
                    chart.ChartAreas.Add(CreateColumnChartArea("Punctuality Status"));
                else if (chartGroupID == 2)
                    chart.ChartAreas.Add(CreateColumnChartArea("Exit Status"));
                else
                    chart.ChartAreas.Add(CreateColumnChartArea("Today's Status"));

                MemoryStream ms = new MemoryStream();
                chart.SaveImage(ms);

                if (chartGroupID == 0)
                    BytesToBitmap(ms.GetBuffer(), "presence.png");
                else if (chartGroupID == 1)
                    BytesToBitmap(ms.GetBuffer(), "punctuality.png");
                else if (chartGroupID == 2)
                    BytesToBitmap(ms.GetBuffer(), "punctuality.png");
                else
                    BytesToBitmap(ms.GetBuffer(), "presence.png");


                return ms.GetBuffer();//File(ms.GetBuffer(), @"image/png"); return type FileResult
            }

            return null;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.Title CreateColumnChartTitle(string strName)
        {
            System.Web.UI.DataVisualization.Charting.Title title = new System.Web.UI.DataVisualization.Charting.Title();
            title.Text = strName; // "Organization Chart";
            //title.ShadowColor = System.Drawing.Color.FromArgb(32, 0, 0, 0);
            title.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);//Trebuchet MS
            //title.ShadowOffset = 0;//3
            title.ForeColor = System.Drawing.Color.FromArgb(16, 36, 63);

            return title;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.Legend CreateColumnChartLegend(string strName)
        {
            System.Web.UI.DataVisualization.Charting.Legend legend = new System.Web.UI.DataVisualization.Charting.Legend();
            //legend.Name = strName; // "Organization Chart";
            legend.Docking = System.Web.UI.DataVisualization.Charting.Docking.Bottom;
            legend.Alignment = System.Drawing.StringAlignment.Center;
            legend.BackColor = System.Drawing.Color.Transparent;
            legend.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Arial"), 9);//Trebuchet MS
            legend.LegendStyle = System.Web.UI.DataVisualization.Charting.LegendStyle.Row;

            return legend;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.Series CreateColumnChartSeries(IList<ColumnChartModel> results, System.Web.UI.DataVisualization.Charting.SeriesChartType chartType, string strName, int chartGroupID, int groupID)
        {
            System.Web.UI.DataVisualization.Charting.Series seriesDetail = new System.Web.UI.DataVisualization.Charting.Series();

            try
            {
                //seriesDetail.Name = strName; // "Organization Chart";
                seriesDetail.IsValueShownAsLabel = true;
                //seriesDetail.Color = System.Drawing.Color.FromArgb(198, 99, 99);
                seriesDetail.ChartType = chartType;
                seriesDetail.BorderWidth = 2;
                seriesDetail["DrawingStyle"] = "Cylinder";
                //seriesDetail["PieDrawingStyle"] = "Default";//SoftEdge,Concave,Default

                //if (chartGroupID == 0)
                //    seriesDetail["DoughnutRadius"] = "55"; // Set Doughnut hole size
                //else if (chartGroupID == 1 || chartGroupID == 2)
                //    seriesDetail["DoughnutRadius"] = "55"; // Set Doughnut hole size
                //else
                //    seriesDetail["DoughnutRadius"] = "60"; // Set Doughnut hole size

                //seriesDetail["PieLabelStyle"] = "Inside"; //Outside,Disabled

                System.Web.UI.DataVisualization.Charting.DataPoint point;

                int a = 0;
                foreach (ColumnChartModel result in results)
                {
                    point = new System.Web.UI.DataVisualization.Charting.DataPoint();
                    point.AxisLabel = result.TypeName;

                    point.XValue = a;

                    point.LegendText = result.TypeName;
                    point.YValues = new double[] { double.Parse(result.TypePercent.ToString()) };


                    //"102,187,106; 114,102,186; 238,110,115; 41,182,246; 255,193,7; 247,99,151; 
                    if (groupID == 1)
                    {
                        //point.Color = System.Drawing.Color.FromArgb(245, 143, 94);//41,182,246
                        //point.MarkerColor = System.Drawing.Color.FromArgb(245, 143, 94);//41,182,246
                    }
                    else
                    {
                        //point.Color = System.Drawing.Color.FromArgb(59, 175, 218);//247,99,151
                        //point.MarkerColor = System.Drawing.Color.FromArgb(59, 175, 218);//247,99,151
                    }
                    seriesDetail.Points.Add(point);

                    a++;
                }

                //seriesDetail.ChartArea = strName; // "Organization Chart";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            seriesDetail.Name = strName;

            return seriesDetail;
        }

        [NonAction]
        public System.Web.UI.DataVisualization.Charting.ChartArea CreateColumnChartArea(string strName)
        {
            System.Web.UI.DataVisualization.Charting.ChartArea chartArea = new System.Web.UI.DataVisualization.Charting.ChartArea();
            //System.Web.UI.DataVisualization.Charting.ChartArea chartArea = new System.Web.UI.DataVisualization.Charting.ChartArea();
            //chartArea.Name = strName; // "Organization Chart";
            chartArea.BackColor = System.Drawing.Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisY.IsLabelAutoFit = true;
            chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial,Verdana,Helvetica,sans-serif", 8F, System.Drawing.FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Arial,Verdana,Helvetica,sans-serif", 8F, System.Drawing.FontStyle.Regular);
            //chartArea.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.LabelStyle.Angle = 90;
            //chartArea.AxisY.LabelStyle.Angle = 90;
            //chartArea.AxisX.Interval = 2;
            //chartArea.Area3DStyle.Rotation = 90;
            // chartArea.Position.Width = 98;
            // chartArea.Position.Height = 70;
            // chartArea.Position.Y = 15;
            //chartArea.Position.X = 90;
            chartArea.Area3DStyle.Enable3D = false;
            //chartArea.Area3DStyle.Rotation = -90;
            //chartArea.Area3DStyle.Perspective = 0;
            //chartArea.Area3DStyle.Inclination = 0;
            ////chartArea.Area3DStyle.IsRightAngleAxes = true;
            chartArea.Area3DStyle.WallWidth = 5;
            //chartArea.Area3DStyle.IsClustered = false;

            //chartArea.Area3DStyle.Rotation = -90;

            return chartArea;
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


        #endregion


        public System.Drawing.Bitmap BytesToBitmap(byte[] byteArray, string strImageName)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                System.Drawing.Bitmap img = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms);

                img.Save(Server.MapPath("//Images//" + strImageName), System.Drawing.Imaging.ImageFormat.Png);

                return img;
            }
        }


        public class PageHeaderFooter : PdfPageEventHelper
        {
            private readonly Font _pageNumberFont = new Font(Font.HELVETICA, 8f, Font.NORMAL, Color.BLACK);

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                AddPageNumber(writer, document);

                //////////////////////////////////////////

                ////https://stackoverflow.com/questions/2321526/pdfptable-as-a-header-in-itextsharp



                ////PdfPTable table = new PdfPTable(1);
                //////table.WidthPercentage = 100; //PdfPTable.writeselectedrows below didn't like this
                ////table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin; //this centers [table]
                ////PdfPTable table2 = new PdfPTable(2);

                //////logo
                ////PdfPCell cell2 = new PdfPCell(); //Image.GetInstance(@"C:\path\to\file.gif")
                ////cell2.Colspan = 2;
                ////table2.AddCell(cell2);

                //////title
                ////cell2 = new PdfPCell(new Phrase("\nTITLE", new Font(Font.HELVETICA, 16, Font.BOLD | Font.UNDERLINE)));
                ////cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                ////cell2.Colspan = 2;
                ////table2.AddCell(cell2);

                ////PdfPCell cell = new PdfPCell(table2);
                ////table.AddCell(cell);

                ////table.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 36, writer.DirectContent);


            }

            private void AddPageNumber(PdfWriter writer, Document document)
            {
                var text = writer.PageNumber.ToString();

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                //float len = bf.GetWidthPoint(text, 10);

                //iTextSharp.text.Rectangle pageSize = document.PageSize;

                var numberTable = new PdfPTable(1);
                numberTable.DefaultCell.Border = Rectangle.NO_BORDER;
                var numberCell = new PdfPCell(new Phrase(text, _pageNumberFont)) { HorizontalAlignment = Element.ALIGN_RIGHT };
                numberCell.Border = 0;

                numberTable.AddCell(numberCell);

                numberTable.TotalWidth = 20;
                numberTable.WriteSelectedRows(0, -1, document.Right - 20, document.Bottom + 5, writer.DirectContent);
            }
        }

    }
}
