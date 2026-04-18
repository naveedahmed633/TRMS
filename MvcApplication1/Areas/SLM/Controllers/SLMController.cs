using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using MvcApplication1.ViewModel;

namespace MvcApplication1.Areas.SLM.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SLM)]
    public class SLMController : Controller
    {
        //
        // GET: /SLM/SLM/

        public ActionResult Dashboard()
        {
            ViewModels.DashboardManager dashboard = new ViewModels.DashboardManager();

            dashboard.lineManager = TimeTune.Dashboard.getDashboardValues(User.Identity.Name);

            //IR: check if new academic year started
            TimeTune.Dashboard.manageLeaveSessionForUsers(User.Identity.Name);

            if (dashboard.lineManager.dashboardElement != null)
            {
                decimal sumPresence = 0, sumAvaiability = 0;
                sumPresence = dashboard.lineManager.dashboardElement.present + dashboard.lineManager.dashboardElement.leave + dashboard.lineManager.dashboardElement.absent;
                sumAvaiability = dashboard.lineManager.dashboardElement.onTime + dashboard.lineManager.dashboardElement.late + dashboard.lineManager.dashboardElement.earlyGone;

                decimal perPresent = 0, perLeave = 0, perAbsent = 0;
                decimal perOnTime = 0, perLate = 0, perEarlyGone = 0;

                if (sumPresence != 0)
                {
                    perPresent = Math.Round(Math.Floor(dashboard.lineManager.dashboardElement.present / sumPresence * 100));
                    perLeave = Math.Round(Math.Ceiling(dashboard.lineManager.dashboardElement.leave / sumPresence * 100));
                    perAbsent = Math.Round(Math.Ceiling(dashboard.lineManager.dashboardElement.absent / sumPresence * 100));
                }

                if (sumAvaiability != 0)
                {
                    perOnTime = Math.Round(Math.Floor(dashboard.lineManager.dashboardElement.onTime / sumAvaiability * 100));
                    perLate = Math.Round(Math.Ceiling(dashboard.lineManager.dashboardElement.late / sumAvaiability * 100));
                    perEarlyGone = Math.Round(Math.Ceiling(dashboard.lineManager.dashboardElement.earlyGone / sumAvaiability * 100));
                }

                ViewBag.CounterPercent = "P=" + perPresent + ",L=" + perLeave + ",A=" + perAbsent + ",O=" + perOnTime + ",T=" + perLate + ",E=" + perEarlyGone;
            }

            return View(dashboard);
        }

        [HttpPost]
        public JsonResult DashboardPAChartData()
        {
            float present = 0.0f, absent = 0.0f, leave = 0.0f, plaSum = 0.0f;
            float pPercent = 0.0f, lPercent = 0.0f, aPercent = 0.0f;

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            var elements = TimeTune.Dashboard.getDashboardValues(User.Identity.Name);
            dashboard.dashboardElement = elements.dashboardElement;

            present = float.Parse(elements.dashboardElement.present.ToString());
            absent = float.Parse(elements.dashboardElement.absent.ToString());
            leave = float.Parse(elements.dashboardElement.leave.ToString());

            //present = 20;
            //absent = 3;
            //leave = 3;

            plaSum = present + leave + absent;
            if (plaSum != 0)
            {
                pPercent = (present / plaSum) * 100.0f;
                lPercent = (leave / plaSum) * 100.0f;
                aPercent = (absent / plaSum) * 100.0f;
            }

            List<DashboardDonutChart> dc = new List<DashboardDonutChart>();
            dc.Add(new DashboardDonutChart { label = "Present", value = pPercent, color = "#52bb56" });
            dc.Add(new DashboardDonutChart { label = "Leave", value = lPercent, color = "#fbef97" });
            dc.Add(new DashboardDonutChart { label = "Absent", value = aPercent, color = "#f76397" });

            return Json(new { data = dc });
        }

        [HttpPost]
        public JsonResult DashboardOLChartData()
        {
            float ontime = 0.0f, late = 0.0f, olSum = 0.0f;
            float oPercent = 0.0f, lPercent = 0.0f;

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            var elements = TimeTune.Dashboard.getDashboardValues(User.Identity.Name);
            dashboard.dashboardElement = elements.dashboardElement;

            ontime = float.Parse(elements.dashboardElement.onTime.ToString());
            late = float.Parse(elements.dashboardElement.late.ToString());

            //ontime = 12;
            //late = 8;

            olSum = ontime + late;
            if (olSum != 0)
            {
                oPercent = (ontime / olSum) * 100.0f;
                lPercent = (late / olSum) * 100.0f;
            }

            List<DashboardDonutChart> dc = new List<DashboardDonutChart>();
            dc.Add(new DashboardDonutChart { label = "Present - On Time", value = oPercent, color = "#67d1f8" });
            dc.Add(new DashboardDonutChart { label = "Present - Late", value = lPercent, color = "#ffaa00" });

            return Json(new { data = dc });
        }

        public ActionResult MyTeam()
        {
            ViewModels.DashboardManager dashboard = new ViewModels.DashboardManager();
            var model = TimeTune.Dashboard.getSupervisor(User.Identity.Name);
            return View(model);
        }

        public ActionResult UserProfile()
        {
            //ViewModels.DashboardManager dashboard = new ViewModels.DashboardManager();
            //var model = TimeTune.Dashboard.getSupervisor(User.Identity.Name);
            //// get all employee view models

            var dtsource = TimeTune.EmployeeCrud.get(User.Identity.Name);
            if (dtsource == null)
            {
                return Json("No Data to Show");
            }

            return View(dtsource);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SupervisorTeamPDF()
        {
            int count = 0;

            List<string> supList = new List<string>();
            supList = TimeTune.Dashboard.getSupervisor(User.Identity.Name);

            // get all employee view models
            var dtsource = new List<Employee>();
            count = TimeTune.Dashboard.generatePDF_SLM("", User.Identity.Name, out dtsource);

            if (dtsource == null)
                return RedirectToAction("MyTeam");

            //return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };

            if (count > 0)
                GenerateTeamPDF(supList, dtsource);

            return null;
        }

        private void GenerateTeamPDF(List<string> sList, List<Employee> eList)
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
                    int j = 0; string strHeading = "Supervisors:\n\n";
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    tableEmployee.SpacingBefore = 10;
                    tableEmployee.SpacingAfter = 10;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableSInfo = new PdfPTable(1);
                    tableSInfo.WidthPercentage = 100;
                    tableSInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableSInfo.SpacingAfter = 3;
                    tableSInfo.DefaultCell.Border = Rectangle.NO_BORDER;
                    foreach (string ls in sList)
                    {
                        if (j != 0)
                        {
                            strHeading = "";
                        }

                        PdfPCell cellEName = new PdfPCell(new Phrase(strHeading + ls, fBold9));//+ sdata.employeeName
                        cellEName.Border = 0;
                        tableSInfo.AddCell(cellEName);

                        j++;
                    }

                    //separator
                    document.Add(lineSeparator);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Supervisor's Team List", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableSInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------------------------------------------

                    Paragraph p_tag_emp = new Paragraph("Tagged Employees:", fBold10);
                    p_tag_emp.SpacingBefore = 5;
                    p_tag_emp.SpacingAfter = 5;
                    document.Add(p_tag_emp);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 15.0f, 15.0f, 15.0f, 15.0f, 20.0f, 20.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 5;
                    tableMid.SpacingAfter = 5;

                    PdfPCell cell1 = new PdfPCell(new Phrase("First Name", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Last Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Employee Code", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Function Name", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Department", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Location", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("Device In", fBold8));
                    //cell7.BackgroundColor = Color.LIGHT_GRAY;
                    //cell7.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell7);

                    //PdfPCell cell8 = new PdfPCell(new Phrase("Device Out", fBold8));
                    //cell8.BackgroundColor = Color.LIGHT_GRAY;
                    //cell8.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell8);

                    foreach (Employee le in eList)
                    {
                        PdfPCell cellData1 = new PdfPCell(new Phrase(le.first_name, fNormal8));
                        //cellData1.HorizontalAlignment = 0;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(le.last_name, fNormal8));
                        //cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(le.employee_code, fNormal8));
                        cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(le.function_name, fNormal8));
                        //cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(le.department_name, fNormal8));
                        //cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        PdfPCell cellData6 = new PdfPCell(new Phrase(le.location_name, fNormal8));
                        //cellData6.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData6);

                        //PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminalIn, fNormal7));
                        ////cellData7.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData7);

                        //PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalOut, fNormal7));
                        ////PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                        ////cellData8.HorizontalAlignment = 1;
                        //tableMid.AddCell(cellData8);
                    }

                    if (eList.Count > 0)
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

                    //separator
                    document.Add(lineSeparator);

                    Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                    //p_nsig.SpacingBefore = 10;
                    //p_nsig.SpacingAfter = 3;
                    document.Add(p_nsig);

                    // ------------- close PDF Document and download it automatically

                    document.Close();
                    writer.Close();

                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=Report.pdf"); // + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf");
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

        [HttpPost]
        public JsonResult DataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<Employee>();

                // get all employee view models
                int count = TimeTune.Dashboard.searchEmployeeDashboardSLM(param.Search.Value, param.SortOrder, User.Identity.Name, param.Start, param.Length, out dtsource);

                //List<Employee> data = ResultSet.GetResult(param.SortOrder, param.Start, param.Length, dtsource);

                //int count = ResultSet.Count(param.Search.Value, dtsource);

                DTResult<Employee> result = new DTResult<Employee>
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
        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult ConfigurationManager()
        {
            return View();
        }

    }
}
