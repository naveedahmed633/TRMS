using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using TimeTune;
using Newtonsoft.Json;
using BLL.ViewModels;
using System.IO;
using System.Globalization;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MvcApplication1.ViewModel;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class AttendanceManagementController : Controller
    {
        public ActionResult ViewAttendance()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PersistentDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ReportPersistentAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllPersistentLogMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


                DTResult<ReportPersistentAttendanceLog> result = new DTResult<ReportPersistentAttendanceLog>
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

        public ActionResult UploadRawAttendance()
        {
            return View();
        }

        public ActionResult ViewRawAttendance()
        {

            return View();
        }

        [HttpPost]
        public JsonResult RawAttendanceDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ReportRawAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllRawAttendanceLogsMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);



                DTResult<ReportRawAttendanceLog> result = new DTResult<ReportRawAttendanceLog>
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
        public ActionResult TodaysAttendanceDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ReportPersistentAttendanceLog>();

                // get all employee view models
                //int count = TimeTune.Reports.getAllPersistentLogMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


                int count = TimeTune.Reports.getAllPersistentLogMatchingForPDF(null, "employee_first_name", out data);
                
                if (data != null)
                {
                    TodaysAttendancePDF(data);
                    ViewBag.Message = "";
                }
                else
                {
                    ViewBag.Message = "NO";
                }
                //DTResult<ReportPersistentAttendanceLog> result = new DTResult<ReportPersistentAttendanceLog>
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

            return View("ViewAttendance");
        }

        private int TodaysAttendancePDF(List<ReportPersistentAttendanceLog> sdata)
        {
            int reponse = 0;
            int presentCount = 0, absentCount = 0;

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

                    string strFrom = DateTime.Now.ToString("dd-MMM-yyyy");

                    PdfPCell cellEYear = new PdfPCell(new Phrase("Today: " + strFrom, fBold10));
                    cellEYear.Border = 0;
                    tableEInfo.AddCell(cellEYear);

                    ////Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    ////p_title.SpacingBefore = 50f;
                    ////p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Today's Attendance Report", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);




                    //Paragraph p_count = new Paragraph("Day Count:", fBold14);
                    //p_count.SpacingBefore = 2;
                    //p_count.SpacingAfter = 5;
                    //document.Add(p_count);

                    ////if (strArrCount[0].ToString() != "-1")
                    ////{
                    ////    Paragraph p_count = new Paragraph("Summary:", fBold14);
                    ////    p_count.SpacingBefore = 2;
                    ////    p_count.SpacingAfter = 5;
                    ////    document.Add(p_count);

                    ////    document.Add(tableCounts);
                    ////}

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 50.0f, 50.0f, 85.0f, 80.0f, 50.0f, 50.0f, 115.0f, 115.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    // tableMid.SpacingBefore = 3;
                    // tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold10));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("ECode", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("First Name", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Last Name", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Time In", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Time Out", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Terminal In", fBold8));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Terminal Out", fBold8));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    foreach (ReportPersistentAttendanceLog log in sdata)
                    {
                        PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.employee_code.ToString(), fNormal8));
                        cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.employee_first_name.ToString(), fNormal8));
                        cellData3.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.employee_last_name.ToString(), fNormal8));
                        cellData4.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.time_in.ToString(), fNormal8));
                        //cellData5.BackgroundColor = Color.LIGHT_GRAY;
                        cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(log.time_out.ToString(), fNormal8));
                        //cellData6.BackgroundColor = Color.LIGHT_GRAY;
                        cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        //cellData5.BackgroundColor = new iTextSharp.text.Color(System.Drawing.Color.FromArgb(230, 245, 251)); //Color.YELLOW; iTextSharp.text.Color(230.0f, 245.0f, 251.0f); //Color.YELLOW;
                        //cellData6.BackgroundColor = new iTextSharp.text.Color(System.Drawing.Color.FromArgb(230, 245, 251)); //Color.YELLOW;


                        PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminal_in.ToString(), fNormal7));
                        cellData7.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData7);

                        PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminal_out.ToString(), fNormal7));
                        cellData8.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData8);

                        if (log.time_in.ToString() == "" && log.time_out.ToString() == "")
                        {
                            absentCount++;
                        }
                        else
                        {
                            presentCount++;
                        }
                    }


                    PdfPTable tableCounts = new PdfPTable(new[] { 34.0f, 33.0f, 33.0f });//total 595 - 10 x 2 due to Left and Right side margin
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

                    //////////
                    int iTotalReg = 0;
                    iTotalReg = TimeTune.Dashboard.getTotalRegisteredEmp();

                    PdfPCell cellCntD01 = new PdfPCell(new Phrase(iTotalReg.ToString(), fNormal10));
                    cellCntD01.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD01);

                    PdfPCell cellCntD02 = new PdfPCell(new Phrase(presentCount.ToString(), fNormal10));
                    cellCntD02.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD02);

                    PdfPCell cellCntD03 = new PdfPCell(new Phrase(absentCount.ToString(), fNormal10));
                    cellCntD03.HorizontalAlignment = 1;
                    tableCounts.AddCell(cellCntD03);


                    if (sdata.Count > 0)
                    {
                        Paragraph p_count = new Paragraph("Summary:", fBold14);
                        p_count.SpacingBefore = 2;
                        p_count.SpacingAfter = 5;
                        document.Add(p_count);

                        document.Add(tableCounts);

                        Paragraph p_detail = new Paragraph("Today's Time In and Out:", fBold14);
                        p_detail.SpacingBefore = 2;
                        p_detail.SpacingAfter = 5;
                        document.Add(p_detail);
                       
                        document.Add(tableMid);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Todays-Attendance-Report-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
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

        #region ManualTimeAdjustment
        public ActionResult ManualTimeAdjustment(string error)
        {
            if (error != null)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetAttendanceRecordForDate()
        {
            DateTime strDate = DateTime.ParseExact(Request["date"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string employeeCode = Request["empId"].ToString();

            try
            {
                var data = new ConsolidatedAttendanceLog();
                using (DLL.Models.Context db = new DLL.Models.Context())
                {
                    foreach (var item in db.consolidated_attendance
                        .Where(p => (p.employee.EmployeeId.ToString() == employeeCode) && p.date == strDate)
                        .ToList())
                    {
                        data.time_in = item.time_in?.ToString("HH:mm:ss"); 
                        data.time_out = item.time_out?.ToString("HH:mm:ss");
                        data.final_remarks = item.final_remarks;
                    }
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpPost]
        public ActionResult UpdateAttendanceLogTime()
        {
            try
            {

                string employeeCode = Request["employee_code"].ToString();
                DateTime strDate = DateTime.ParseExact(Request["date"], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime timeIn = strDate.Add(TimeSpan.Parse(Request["timeIn"]));
                DateTime timeOut = strDate.Add(TimeSpan.Parse(Request["timeOut"]));
                string remark = Request["remark"];

                using (DLL.Models.Context db = new DLL.Models.Context())
                {
                    var record = db.consolidated_attendance
                        .Where(p => (p.employee.EmployeeId.ToString() == employeeCode) && p.date == strDate)
                        .ToList()
                        .First();

                    record.time_in = timeIn;
                    record.time_out = timeOut;
                    record.final_remarks = remark;

                    switch (remark)
                    {
                        case "PO":
                            record.status_in = "On Time";
                            record.status_out = "On Time";
                            break;
                        case "POM":
                            record.status_in = "On Time";
                            record.status_out = "Miss Punch";
                            break;
                        case "POE":
                            record.status_in = "On Time";
                            record.status_out = "Early Out";
                            break;
                        case "PLO":
                            record.status_in = "Late";
                            record.status_out = "On Time";
                            break;
                        case "PLE":
                            record.status_in = "Late";
                            record.status_out = "Early Out";
                            break;
                        case "PLM":
                            record.status_in = "Late";
                            record.status_out = "Miss Punch";
                            break;
                        case "AB":
                            record.status_in = "ab";
                            record.status_out = "ab";
                            break;
                        case "OFF":
                            record.status_in = "Off";
                            record.status_out = "Off";
                            break;
                        default:
                            break;
                    }

                    db.SaveChanges();
                }

                return Json(new { Success = true, Message = "Attendance log updated successfully!" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { Success = false, Message = ex.Message });

            }
        }

        #endregion

        #region ManualAttendance
        public JsonResult DataHandler(DTParameters param)
        {
            try
            {



                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(null, null, null, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);
                data = data.OrderByDescending(o => o.id).ToList();

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

        public ActionResult ManualAttendance(string error)
        {
            if (error != null)
            {
                ModelState.AddModelError("", error);
            }

            //IR Commented
            ////ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            ////manualAttendance.employee = EmployeeCrud.getAll();
            //loading the employee code in view
            ////return View(manualAttendance);

            return View();
        }
        [HttpPost]
        public ActionResult AddManualAttendance(ViewModels.ManualAttendance manualAttendance)
        {
            string strFromDate = "", strToDate = "";

            //if (manualAttendance.employee_code == null)
            //{
            //    return RedirectToAction("ManualAttendance", new { Error = "Invalid 'Employee Code'" });
            //}

            if (Request["time_in_from"] == null)
            {
                return RedirectToAction("ManualAttendance", new { Error = "Invalid 'From Date'" });
            }

            if (Request["time_in_to"] == null)
            {
                return RedirectToAction("ManualAttendance", new { Error = "Invalid 'To Date'" });
            }

            strFromDate = DateTime.ParseExact(Request["time_in_from"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            strToDate = DateTime.ParseExact(Request["time_in_to"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            manualAttendance.time_in_from = DateTime.ParseExact(strFromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            manualAttendance.time_in_to = DateTime.ParseExact(strToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //data for view model is coming in this parameter 
            ///just need the login employee code to save in manual attendance table that employee
            /// 
            string message = TimeTune.ManualAttendance.addManualAttendance(manualAttendance, User.Identity.Name);
            var json = JsonConvert.SerializeObject(manualAttendance);
            AuditTrail.update(json, "Manual Attendance", User.Identity.Name);

            AuditTrail.updateManaulAttLog(manualAttendance, User.Identity.Name);
            return RedirectToAction("ManualAttendance", new { Error = message });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFileManualAttendance(HttpPostedFileBase file)
        {
            string result = null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_PostAdjustment.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                result = TimeTune.ManualAttendance.uploadManualAttendance(content, User.Identity.Name);

                return RedirectToAction("ManualAttendance", "AttendanceManagement", new { result = "Successful" });
                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}

            }
            return RedirectToAction("ManualAttendance", "AttendanceManagement", new { result = "Select File first" });
        }

        #endregion


        #region Future Manual Attendance
        public ActionResult FutureManualAttendance(string error)
        {
            if (error != null)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }

        [HttpPost]
        public ActionResult FutureManualAttendance(string employee_code, string time_in_from, string time_in_to, string remarks)
        {
            string strFromDate = "", strToDate = "";

            //if (employee_code == null)
            //{
            //    return RedirectToAction("FutureManualAttendance", new { Error = "Invalid 'Employee Code'" });
            //}

            if (time_in_from == null)
            {
                return RedirectToAction("FutureManualAttendance", new { Error = "Invalid 'From Date'" });
            }

            if (time_in_to == null)
            {
                return RedirectToAction("FutureManualAttendance", new { Error = "Invalid 'To Date'" });
            }

            strFromDate = DateTime.ParseExact(time_in_from, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            strToDate = DateTime.ParseExact(time_in_to, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            // string message = TimeTune.ManualAttendance.addManualAttendance(manualAttendance);
            //var json = JsonConvert.SerializeObject(message);
            //AuditTrail.update(json, "Manual Attendance", int.Parse(User.Identity.Name));
            string message = BLL.ManageHR.addManualAttendanceHr(strFromDate, strToDate, employee_code, remarks);

            return RedirectToAction("FutureManualAttendance", new { message = message });
        }


        [HttpPost]
        public JsonResult FutureManualAttendanceDataHandler(DTParameters param)
        {
            try
            {

                var data = new List<BLL.ViewModels.ViewFutureManualAttendance>();

                // get all employee view models
                int count = BLL.ManageHR.getAllManualAttendance(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);
                data = data.OrderByDescending(o => o.Id).ToList();

                DTResult<BLL.ViewModels.ViewFutureManualAttendance> result = new DTResult<BLL.ViewModels.ViewFutureManualAttendance>
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
        public JsonResult DeleteLeave(int id)
        {
            int response = 0;

            try
            {
                // get all employee view models
                response = BLL.ManageHR.DeleteLeave(id);

                return Json(new { data = response });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        //EmployeeDataHandler
        [HttpPost]
        public JsonResult getEmployeeDropDown()
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

        // TODO TODO TODO TODO
        #region Reprocess Attendance

        public ActionResult ReprocessAttendance(string result)
        {
            ViewBag.Message = result;
            return View();
        }

        // This method will recieve the form submisison        // containing the employee_code and the to and from        // dates.
        [HttpPost]
        public ActionResult RequestAttendanceReprocess(string from_date, string to_date, int employee_id)
        {
            //if (employee_id == null)
            //{
            //    return RedirectToAction("MarkAttendance", new { result = "Invalid 'Employee Code'" });
            //}

            if (Request["from_date"] == null)
            {
                return RedirectToAction("MarkAttendance", new { result = "Invalid 'From Date'" });
            }

            if (Request["to_date"] == null)
            {
                return RedirectToAction("MarkAttendance", new { result = "Invalid 'To Date'" });
            }

            BLL.MarkPreviousAttendance.createOrUpdateAttendance(from_date, to_date, employee_id, User.Identity.Name);

            return RedirectToAction("ReprocessAttendance", new { result = "Successful" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string result = null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                string path = Path.Combine(Server.MapPath("~/Uploads"),
                                   "ReprocessAttendance.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                result = TimeTune.ReprocessAttendanceImportExport.reprocessAttendance(content, User.Identity.Name);
                return RedirectToAction("ReprocessAttendance", "AttendanceManagement", new { result = "Successful" });
                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}

            }
            return RedirectToAction("ReprocessAttendance", "AttendanceManagement", new { result = "Select File first" });
        }

        #endregion



    }
}
