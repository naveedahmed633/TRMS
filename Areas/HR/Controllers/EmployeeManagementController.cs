using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TimeTune;
using ViewModels;
using MvcApplication1;
using MvcApplication1.Areas.HR.DataTableResultSets;
using MvcApplication1.ViewModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Configuration;
using iTextSharp.text.pdf;
using iTextSharp.text;



namespace MvcApplication1.Areas.HR.Controllers
{
    public class EMP_BirthdayEvent
    {
        public List<Employee> TodayBirthdays { get; set; }
        public List<Employee> UpcomingBirthdays { get; set; }
    }

    public class User_ProfileData
    {
        public List<ViewModels.EmployeeEvaluation> emp_eva { get; set; }
        public List<ViewModels.Employee> team_employee { get; set; }
        public ViewModels.Employee emp_vm { get; set; }
    }

    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class EmployeeManagementController : Controller
    {
        private void ResolveCampusScope(out bool isSuperHr, out int campusId)
        {
            isSuperHr = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            campusId = 0;
            var campusIdText = ViewModel.GlobalVariables.GV_EmployeeCampusID;
            int parsedCampusId;
            bool parsed = int.TryParse(campusIdText, out parsedCampusId);
            if (parsed && parsedCampusId > 0)
            {
                campusId = parsedCampusId;
                return;
            }

            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                return;
            }

            isSuperHr = BLL.ADMIN.Utility.GetEmployeeSuperHRAccessByEmployeeCode(User.Identity.Name);
            var campusIdValue = BLL.ADMIN.Utility.GetEmployeeCampusIDByEmployeeCode(User.Identity.Name);
            int parsedCampusIdFromUser;
            if (int.TryParse(campusIdValue, out parsedCampusIdFromUser))
            {
                campusId = parsedCampusIdFromUser;
            }

            // Refresh session-backed globals so subsequent requests stay consistent.
            ViewModel.GlobalVariables.GV_RoleIsSuperHR = isSuperHr;
            ViewModel.GlobalVariables.GV_EmployeeCampusID = campusId.ToString();
        }

        public ActionResult UserProfile(string user_code)
        {
            float cal = 0;
            //ViewModels.DashboardManager dashboard = new ViewModels.DashboardManager();
            //var model = TimeTune.Dashboard.getSupervisor(User.Identity.Name);
            //// get all employee view models
            User_ProfileData obj = new User_ProfileData();

            obj.emp_vm = TimeTune.EmployeeCrud.get(user_code);
            obj.emp_eva = TimeTune.Employee_Evaluation.EmpEvaluationData(user_code);

            if (obj.emp_vm != null && obj.emp_vm.access_group_id == 2)
            {
                ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
                obj.team_employee = TimeTune.LMManualAttendance.getEmployeeBySupervisor(user_code);
            }
            else
            {
                obj.team_employee = null;
            }

            obj.emp_vm.cards_list = BLL_UNIS.UNISQueryRun.getCardsListByLUID(user_code) ?? "";
            obj.emp_vm.is_finger_registered = BLL_UNIS.UNISQueryRun.getFingerStatusByLUID(User.Identity.Name) ?? "";

            if (obj.emp_eva != null && obj.emp_eva.Count > 0)
            {

                obj.emp_eva[0].total = obj.emp_eva[0].personality + obj.emp_eva[0].communicationSkills + obj.emp_eva[0].attendancePromptness + obj.emp_eva[0].imitative + obj.emp_eva[0].organizationAwareness + obj.emp_eva[0].selfControl + obj.emp_eva[0].proficiency + obj.emp_eva[0].projectManagement + obj.emp_eva[0].attentionDetail + obj.emp_eva[0].clientInteraction + obj.emp_eva[0].creativity + obj.emp_eva[0].businessSkill + obj.emp_eva[0].achievement;

                cal = obj.emp_eva[0].total / 65 * 5;
                if (cal >= 4.60 && cal <= 5.00)
                {
                    obj.emp_eva[0].result = "EXCELLENT";
                }
                else if (cal >= 4.00 && cal <= 4.59)
                {
                    obj.emp_eva[0].result = "VERY GOOD";
                }
                else if (cal >= 2.80 && cal <= 3.99)
                {
                    obj.emp_eva[0].result = "GOOD";
                }
                else if (cal >= 1.60 && cal <= 2.79)
                {
                    obj.emp_eva[0].result = "SATISFACTORY";
                }
                else
                {
                    obj.emp_eva[0].result = "POOR";
                }
            }

            if (obj.emp_vm == null)
            {
                return Json("No Data to Show");
            }

            return View(obj);
        }

        [HttpPost]
        public ActionResult UserProfile()
        {
            int found = 0; ViewData["PDFNoDataFound"] = "";
            List<ViewModels.Employee> listEmp = new List<Employee>();

            string lm_code = "000000";
            if (Request.Form["lm_code"] != null && Request.Form["lm_code"].ToString() != "")
            {
                lm_code = Request.Form["lm_code"].ToString();
            }

            float cal = 0;
            //ViewModels.DashboardManager dashboard = new ViewModels.DashboardManager();
            //var model = TimeTune.Dashboard.getSupervisor(User.Identity.Name);
            //// get all employee view models
            User_ProfileData obj = new User_ProfileData();

            obj.emp_vm = TimeTune.EmployeeCrud.get(lm_code);
            obj.emp_eva = TimeTune.Employee_Evaluation.EmpEvaluationData(lm_code);

            if (obj.emp_vm != null && obj.emp_vm.access_group_id == 2)
            {
                ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
                obj.team_employee = TimeTune.LMManualAttendance.getEmployeeBySupervisor(lm_code);
                listEmp = obj.team_employee;
            }
            else
            {
                obj.team_employee = null;
            }

            obj.emp_vm.cards_list = BLL_UNIS.UNISQueryRun.getCardsListByLUID(lm_code) ?? "";
            obj.emp_vm.is_finger_registered = BLL_UNIS.UNISQueryRun.getFingerStatusByLUID(User.Identity.Name) ?? "";

            if (obj.emp_eva != null && obj.emp_eva.Count > 0)
            {

                obj.emp_eva[0].total = obj.emp_eva[0].personality + obj.emp_eva[0].communicationSkills + obj.emp_eva[0].attendancePromptness + obj.emp_eva[0].imitative + obj.emp_eva[0].organizationAwareness + obj.emp_eva[0].selfControl + obj.emp_eva[0].proficiency + obj.emp_eva[0].projectManagement + obj.emp_eva[0].attentionDetail + obj.emp_eva[0].clientInteraction + obj.emp_eva[0].creativity + obj.emp_eva[0].businessSkill + obj.emp_eva[0].achievement;

                cal = obj.emp_eva[0].total / 65 * 5;
                if (cal >= 4.60 && cal <= 5.00)
                {
                    obj.emp_eva[0].result = "EXCELLENT";
                }
                else if (cal >= 4.00 && cal <= 4.59)
                {
                    obj.emp_eva[0].result = "VERY GOOD";
                }
                else if (cal >= 2.80 && cal <= 3.99)
                {
                    obj.emp_eva[0].result = "GOOD";
                }
                else if (cal >= 1.60 && cal <= 2.79)
                {
                    obj.emp_eva[0].result = "SATISFACTORY";
                }
                else
                {
                    obj.emp_eva[0].result = "POOR";
                }
            }

            if (obj.emp_vm == null)
            {
                return Json("No Data to Show");
            }

            found = GenerateTeamUsersPDF(lm_code, listEmp); // GenerateEvaluationPDF(toRender);

            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
                //return null;
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            return View(obj);
        }

        private int GenerateTeamUsersPDF(string strLMCode, List<ViewModels.Employee> lemp)
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
                    ////writer.PageEvent = new PageHeaderFooter();

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

                    //PdfPCell cellEName = new PdfPCell(new Phrase("Name: " + strLMName, fBold9));
                    //cellEName.Border = 0;
                    //tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + strLMCode, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.year, fBold9));
                    //cellEYear.Border = 0;
                    //tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Team Report", fBold16));
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
                    PdfPTable tableMid = new PdfPTable(new[] { 55.0f, 120.0f, 120.0f, 150.0f, 150.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("S.No.", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("First Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Last Name", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Department", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Designation", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    int counter = 1;
                    foreach (ViewModels.Employee log in lemp)
                    {
                        PdfPCell cellData1 = new PdfPCell(new Phrase(counter.ToString(), fNormal8));
                        cellData1.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData1);

                        PdfPCell cellData2 = new PdfPCell(new Phrase(log.first_name, fNormal8));
                        //cellData2.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData2);

                        PdfPCell cellData3 = new PdfPCell(new Phrase(log.last_name, fNormal8));
                        //cellData3.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData3);

                        PdfPCell cellData4 = new PdfPCell(new Phrase(log.department_name, fNormal8));
                        //cellData4.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData4);

                        PdfPCell cellData5 = new PdfPCell(new Phrase(log.designation_name, fNormal8));
                        //cellData5.HorizontalAlignment = 1;
                        tableMid.AddCell(cellData5);

                        counter++;
                    }

                    if (lemp.Count > 0)
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
                        Response.AddHeader("content-disposition", "attachment;filename=Team-Report-" + strLMCode + ".pdf");
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

        #region Function

        public ActionResult Function()
        {

            return View();
        }

        [HttpPost]
        public JsonResult FunctionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Function>();
                // get all employee view models
                dtsource = TimeTune.EmployeeFunction.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Function> data = FunctionResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = FunctionResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Function> result = new DTResult<ViewModels.Function>
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
                // Keep DataTables response shape even on failures, otherwise the client shows a generic warning.
                return Json(new DTResult<ViewModels.UnisUser>
                {
                    draw = param != null ? param.Draw : 0,
                    data = new List<ViewModels.UnisUser>(),
                    recordsFiltered = 0,
                    recordsTotal = 0
                });

            }
        }

        [HttpPost]
        public ActionResult AddFunction(ViewModels.Function fromForm)
        {
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeFunction.add(fromForm);
            AuditTrail.insert(json, "Function", User.Identity.Name);
            return RedirectToAction("Function");
        }

        [HttpPost]
        public ActionResult UpdateFunction(ViewModels.Function toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.EmployeeFunction.update(toUpdate);
            AuditTrail.update(json, "Function", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveFunction(ViewModels.Function toRemove)
        {
            var entity = TimeTune.EmployeeFunction.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Function", User.Identity.Name);
            return Json(new { status = "success" });
        }
        #endregion

        #region Program-Shift

        public ActionResult ProgramShift()
        {

            return View();
        }

        [HttpPost]
        public JsonResult ProgramShiftDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.OrganizationProgramShiftView>();
                // get all employee view models
                dtsource = TimeTune.EmployeeProgramShift.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.OrganizationProgramShiftView> data = ProgramShiftResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = ProgramShiftResultSet.Count(param.Search.Value, dtsource);

                DTResult<ViewModels.OrganizationProgramShiftView> result = new DTResult<ViewModels.OrganizationProgramShiftView>
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
                // Keep DataTables response shape even on failures.
                return Json(new DTResult<Employee>
                {
                    draw = param != null ? param.Draw : 0,
                    data = new List<Employee>(),
                    recordsFiltered = 0,
                    recordsTotal = 0
                });

            }
        }

        [HttpPost]
        public ActionResult AddProgramShift(ViewModels.OrganizationProgramShiftForm fromForm)
        {
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeProgramShift.add(fromForm);
            AuditTrail.insert(json, "ProgramShift", User.Identity.Name);
            return RedirectToAction("ProgramShift");
        }

        [HttpPost]
        public ActionResult UpdateProgramShift(ViewModels.OrganizationProgramShiftForm toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.EmployeeProgramShift.update(toUpdate);
            AuditTrail.update(json, "ProgramShift", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveProgramShift(ViewModels.OrganizationProgramShiftForm toRemove)
        {
            var entity = TimeTune.EmployeeProgramShift.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "ProgramShift", User.Identity.Name);
            return Json(new { status = "success" });
        }

        #endregion

        #region Region
        public ActionResult Region()
        {


            return View();
        }
        [HttpPost]
        public JsonResult RegionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Region>();
                // get all employee view models
                dtsource = TimeTune.EmployeeRegion.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Region> data = RegionResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = RegionResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Region> result = new DTResult<ViewModels.Region>
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
        public ActionResult AddRegion(ViewModels.Region fromForm)
        {

            int id = TimeTune.EmployeeRegion.add(fromForm);
            var json = JsonConvert.SerializeObject(fromForm);
            AuditTrail.insert(json, "Region", User.Identity.Name);
            return RedirectToAction("Region");
        }

        [HttpPost]
        public ActionResult UpdateRegion(ViewModels.Region toUpdate)
        {
            TimeTune.EmployeeRegion.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Region", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveRegion(ViewModels.Region toRemove)
        {
            var entity = TimeTune.EmployeeRegion.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Region", User.Identity.Name);
            return Json(new { status = "success" });
        }
        #endregion

        #region Department
        public ActionResult Department()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DepartmentDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Department>();
                // get all employee view models
                dtsource = TimeTune.EmployeeDepartment.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Department> data = DepartmentResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = DepartmentResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Department> result = new DTResult<ViewModels.Department>
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
        public ActionResult AddDepartment(ViewModels.Department fromForm)
        {

            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeDepartment.add(fromForm);
            AuditTrail.insert(json, "Department", User.Identity.Name);
            return RedirectToAction("Department");
        }

        [HttpPost]
        public ActionResult UpdateDepartment(ViewModels.Department toUpdate)
        {
            TimeTune.EmployeeDepartment.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Department", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveDepartment(ViewModels.Department toRemove)
        {
            var entity = TimeTune.EmployeeDepartment.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Department", User.Identity.Name);
            return Json(new { status = "success" });
        }
        #endregion

        #region Designation
        public ActionResult Designation()
        {

            return View();
        }
        [HttpPost]
        public JsonResult DesignationDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Designation>();
                // get all employee view models
                dtsource = TimeTune.EmployeeDesignation.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Designation> data = DesignationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = DesignationResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Designation> result = new DTResult<ViewModels.Designation>
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
        public ActionResult AddDesignation(ViewModels.Designation fromForm)
        {

            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeDesignation.add(fromForm);
            AuditTrail.insert(json, "Designation", User.Identity.Name);
            return RedirectToAction("Designation");
        }

        [HttpPost]
        public ActionResult UpdateDesignation(ViewModels.Designation toUpdate)
        {
            TimeTune.EmployeeDesignation.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Designation", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveDesignation(ViewModels.Designation toRemove)
        {
            var entity = TimeTune.EmployeeDesignation.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Designation", User.Identity.Name);
            return Json(new { status = "success" });
        }
        #endregion

        #region Grade
        public ActionResult Grade()
        {
            List<ViewModels.Grades> gradeList = TimeTune.EmployeeGrade.getAll();

            return View("Grade", gradeList);
        }

        [HttpPost]
        public ActionResult AddGrade(ViewModels.Grades fromForm)
        {

            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeGrade.add(fromForm);
            AuditTrail.insert(json, "Grade", User.Identity.Name);
            return RedirectToAction("Grade");
        }

        [HttpPost]
        public ActionResult UpdateGrade(ViewModels.Grades toUpdate)
        {
            TimeTune.EmployeeGrade.update(toUpdate);

            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Grade", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveGrade(ViewModels.Grades toRemove)
        {
            var entity = TimeTune.EmployeeGrade.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Grade", User.Identity.Name);
            return Json(new { status = "success" });
        }
        #endregion

        #region Location
        public ActionResult Location()
        {
            return View();
        }
        [HttpPost]
        public JsonResult LocationDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Location>();
                // get all employee view models
                dtsource = TimeTune.EmployeeLocation.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Location> data = LocationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = LocationResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Location> result = new DTResult<ViewModels.Location>
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
        public ActionResult AddLocation(ViewModels.Location fromForm)
        {

            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeLocation.add(fromForm);
            AuditTrail.insert(json, "Location", User.Identity.Name);
            return RedirectToAction("Location");
        }

        [HttpPost]
        public ActionResult UpdateLocation(ViewModels.Location toUpdate)
        {
            TimeTune.EmployeeLocation.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Location", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLocation(ViewModels.Location toRemove)
        {
            var entity = TimeTune.EmployeeLocation.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Location", User.Identity.Name);
            return Json(new { status = "success" });
        }

        #endregion

        #region TypeOfEmployment

        public ActionResult TypeOfEmployment()
        {
            List<ViewModels.TypeOfEmployment> typeofemploymentList = TimeTune.EmployeeTypeOfEmployee.getAll();

            return View("TypeOfEmployment", typeofemploymentList);
        }

        [HttpPost]
        public ActionResult AddTypeOfEmployment(ViewModels.TypeOfEmployment fromForm)
        {

            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeTypeOfEmployee.add(fromForm);
            AuditTrail.insert(json, "Type Of Employement", User.Identity.Name);
            return RedirectToAction("TypeOfEmployment");
        }

        [HttpPost]
        public ActionResult UpdateTypeOfEmployment(ViewModels.TypeOfEmployment toUpdate)
        {
            TimeTune.EmployeeTypeOfEmployee.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Location", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveTypeOfEmployment(ViewModels.TypeOfEmployment toRemove)
        {
            var entity = TimeTune.EmployeeTypeOfEmployee.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Type Of Employement", User.Identity.Name);
            return Json(new { status = "success" });
        }
        #endregion

        #region ManageEmployees
        public ActionResult ManageEmployee(string result)
        {
            // Run FAHR employee import in the background
            BLL.Services.FahrEmployeeImportHelper.RunImportInBackground();

            string value = result;
            ViewBag.Message = result;


            //max and default leaves counter
            //if (ConfigurationManager.AppSettings["MaxSickLeaves"] != null && ConfigurationManager.AppSettings["MaxSickLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxSickLeaves = ConfigurationManager.AppSettings["MaxSickLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxSickLeaves = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxCasualLeaves"] != null && ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxCasualLeaves = ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxCasualLeaves = "15";
            //}

            //if (ConfigurationManager.AppSettings["MaxAnnualLeaves"] != null && ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxAnnualLeaves = ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxAnnualLeaves = "20";
            //}

            //if (ConfigurationManager.AppSettings["MaxOtherLeaves"] != null && ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxOtherLeaves = ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxOtherLeaves = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType01"] != null && ConfigurationManager.AppSettings["MaxLeaveType01"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType01 = ConfigurationManager.AppSettings["MaxLeaveType01"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType01 = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType02"] != null && ConfigurationManager.AppSettings["MaxLeaveType02"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType02 = ConfigurationManager.AppSettings["MaxLeaveType02"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType02 = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType03"] != null && ConfigurationManager.AppSettings["MaxLeaveType03"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType03 = ConfigurationManager.AppSettings["MaxLeaveType03"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType03 = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType04"] != null && ConfigurationManager.AppSettings["MaxLeaveType04"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType04 = ConfigurationManager.AppSettings["MaxLeaveType04"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType04 = "10";
            //}

            ////
            //if (ConfigurationManager.AppSettings["DefaultSickLeaves"] != null && ConfigurationManager.AppSettings["DefaultSickLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultSickLeaves = ConfigurationManager.AppSettings["DefaultSickLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultSickLeaves = "8";
            //}

            //if (ConfigurationManager.AppSettings["DefaultCasualLeaves"] != null && ConfigurationManager.AppSettings["DefaultCasualLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultCasualLeaves = ConfigurationManager.AppSettings["DefaultCasualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultCasualLeaves = "10";
            //}

            //if (ConfigurationManager.AppSettings["DefaultAnnualLeaves"] != null && ConfigurationManager.AppSettings["DefaultAnnualLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultAnnualLeaves = ConfigurationManager.AppSettings["DefaultAnnualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultAnnualLeaves = "14";
            //}

            //if (ConfigurationManager.AppSettings["DefaultOtherLeaves"] != null && ConfigurationManager.AppSettings["DefaultOtherLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultOtherLeaves = ConfigurationManager.AppSettings["DefaultOtherLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultOtherLeaves = "0";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType01"] != null && ConfigurationManager.AppSettings["DefaultLeaveType01"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType01 = ConfigurationManager.AppSettings["DefaultLeaveType01"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType01 = "0";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType02"] != null && ConfigurationManager.AppSettings["DefaultLeaveType02"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType02 = ConfigurationManager.AppSettings["DefaultLeaveType02"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType02 = "0";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType03"] != null && ConfigurationManager.AppSettings["DefaultLeaveType03"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType03 = ConfigurationManager.AppSettings["DefaultLeaveType03"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType03 = "0";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType04"] != null && ConfigurationManager.AppSettings["DefaultLeaveType04"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType04 = ConfigurationManager.AppSettings["DefaultLeaveType04"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType04 = "0";
            //}

            LeaveTypesCountStatus vm = new LeaveTypesCountStatus();

            int[] leaves = new int[45] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            leaves = TimeTune.LeaveSessionCrud.GetDefaultMaxLeavesCountStatus();
            ViewBag.DefaultSickLeaves = leaves[0];
            ViewBag.DefaultCasualLeaves = leaves[3];
            ViewBag.DefaultAnnualLeaves = leaves[6];
            ViewBag.DefaultOtherLeaves = leaves[9];
            ViewBag.DefaultLeaveType01 = leaves[12];
            ViewBag.DefaultLeaveType02 = leaves[15];
            ViewBag.DefaultLeaveType03 = leaves[18];
            ViewBag.DefaultLeaveType04 = leaves[21];
            ViewBag.DefaultLeaveType05 = leaves[24];
            ViewBag.DefaultLeaveType06 = leaves[27];
            ViewBag.DefaultLeaveType07 = leaves[30];
            ViewBag.DefaultLeaveType08 = leaves[33];
            ViewBag.DefaultLeaveType09 = leaves[36];
            ViewBag.DefaultLeaveType10 = leaves[39];
            ViewBag.DefaultLeaveType11 = leaves[42];

            ViewBag.MaxSickLeaves = leaves[1];
            ViewBag.MaxCasualLeaves = leaves[4];
            ViewBag.MaxAnnualLeaves = leaves[7];
            ViewBag.MaxOtherLeaves = leaves[10];
            ViewBag.MaxLeaveType01 = leaves[13];
            ViewBag.MaxLeaveType02 = leaves[16];
            ViewBag.MaxLeaveType03 = leaves[19];
            ViewBag.MaxLeaveType04 = leaves[22];
            ViewBag.MaxLeaveType05 = leaves[25];
            ViewBag.MaxLeaveType06 = leaves[28];
            ViewBag.MaxLeaveType07 = leaves[31];
            ViewBag.MaxLeaveType08 = leaves[34];
            ViewBag.MaxLeaveType09 = leaves[37];
            ViewBag.MaxLeaveType10 = leaves[40];
            ViewBag.MaxLeaveType11 = leaves[43];

            ViewBag.StatusSickLeaves = leaves[2];
            ViewBag.StatusCasualLeaves = leaves[5];
            ViewBag.StatusAnnualLeaves = leaves[8];
            ViewBag.StatusOtherLeaves = leaves[11];
            ViewBag.StatusLeaveType01 = leaves[14];
            ViewBag.StatusLeaveType02 = leaves[17];
            ViewBag.StatusLeaveType03 = leaves[20];
            ViewBag.StatusLeaveType04 = leaves[23];
            ViewBag.StatusLeaveType05 = leaves[26];
            ViewBag.StatusLeaveType06 = leaves[29];
            ViewBag.StatusLeaveType07 = leaves[32];
            ViewBag.StatusLeaveType08 = leaves[35];
            ViewBag.StatusLeaveType09 = leaves[38];
            ViewBag.StatusLeaveType10 = leaves[41];
            ViewBag.StatusLeaveType11 = leaves[44];

            ////// only access groups are to be sent without ajax.
            ////CreateEmployee createEmployeeViewModel = new CreateEmployee();
            ////createEmployeeViewModel.accessGroups = EmployeeAccessGroup.getAllButHr();

            ////createEmployeeViewModel.typeOfEmployments = EmployeeTypeOfEmployee.getAll();

            ////return View(createEmployeeViewModel);

            bool bGVIsSuperHRRole;
            int iGVCampusID;
            ResolveCampusScope(out bGVIsSuperHRRole, out iGVCampusID);

            // only access groups are to be sent without ajax.
            CreateEmployee createEmployeeViewModel = new CreateEmployee();
            createEmployeeViewModel.accessGroups = EmployeeAccessGroup.getAllButHr();
            createEmployeeViewModel.skillSets = EmployeeAccessGroup.getAllSkillSets();
            createEmployeeViewModel.campuses = EmployeeAccessGroup.getAllCampuses(bGVIsSuperHRRole, iGVCampusID);
            createEmployeeViewModel.pshifts = EmployeeAccessGroup.getAllPShifts();

            createEmployeeViewModel.positionStatus = EmployeeAccessGroup.getAllPositionStatus();
            createEmployeeViewModel.siteStatus = EmployeeAccessGroup.getAllSiteStatus();

            return View(createEmployeeViewModel);
        }

        [HttpPost]
        public ActionResult AddEmployee(Employee toCreate, HttpPostedFileBase photograph, HttpPostedFileBase file_01, HttpPostedFileBase file_02, HttpPostedFileBase file_03, HttpPostedFileBase file_04, HttpPostedFileBase file_05)
        {
            //if (!Directory.Exists(Path.GetDirectoryName(Server.MapPath("~/Content/UserDocs"))))
            //{
            //    Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath("~/Content/UserDocs")));
            //}

            if (photograph != null && photograph.ContentLength > 0)
            {
                if (photograph.ContentLength > (2 * 1024 * 1024))
                {
                    return RedirectToAction("ManageEmployee", new { message = "image" });
                }

                var fileExtension = Path.GetExtension(photograph.FileName);
                var fileName = Path.GetFileName(photograph.FileName).Replace(fileExtension, "");
                fileName = Guid.NewGuid() + "_" + fileName.Replace(" ", "") + "_" + fileExtension;
                var path = Path.Combine(Server.MapPath("~/Content/UserDocs"), fileName);
                photograph.SaveAs(path);

                toCreate.photograph = fileName;
            }

            if (file_01 != null && file_01.ContentLength > 0)
            {
                if (file_01.ContentLength > (2 * 1024 * 1024))
                {
                    return RedirectToAction("ManageEmployee", new { message = "image" });
                }

                var fileExtension = Path.GetExtension(file_01.FileName);
                var fileName = Path.GetFileName(file_01.FileName).Replace(fileExtension, "");
                fileName = Guid.NewGuid() + "_" + fileName.Replace(" ", "") + "_" + fileExtension;
                var path = Path.Combine(Server.MapPath("~/Content/UserDocs"), fileName);
                file_01.SaveAs(path);

                toCreate.file_01 = fileName;
            }

            if (file_02 != null && file_02.ContentLength > 0)
            {
                if (file_02.ContentLength > (2 * 1024 * 1024))
                {
                    return RedirectToAction("ManageEmployee", new { message = "image" });
                }

                var fileExtension = Path.GetExtension(file_02.FileName);
                var fileName = Path.GetFileName(file_02.FileName).Replace(fileExtension, "");
                fileName = Guid.NewGuid() + "_" + fileName.Replace(" ", "") + "_" + fileExtension;
                var path = Path.Combine(Server.MapPath("~/Content/UserDocs"), fileName);
                file_02.SaveAs(path);

                toCreate.file_02 = fileName;
            }

            if (file_03 != null && file_03.ContentLength > 0)
            {
                if (file_03.ContentLength > (2 * 1024 * 1024))
                {
                    return RedirectToAction("ManageEmployee", new { message = "image" });
                }

                var fileExtension = Path.GetExtension(file_03.FileName);
                var fileName = Path.GetFileName(file_03.FileName).Replace(fileExtension, "");
                fileName = Guid.NewGuid() + "_" + fileName.Replace(" ", "") + "_" + fileExtension;
                var path = Path.Combine(Server.MapPath("~/Content/UserDocs"), fileName);
                file_03.SaveAs(path);

                toCreate.file_03 = fileName;
            }

            if (file_04 != null && file_04.ContentLength > 0)
            {
                if (file_04.ContentLength > (2 * 1024 * 1024))
                {
                    return RedirectToAction("ManageEmployee", new { message = "image" });
                }

                var fileExtension = Path.GetExtension(file_04.FileName);
                var fileName = Path.GetFileName(file_04.FileName).Replace(fileExtension, "");
                fileName = Guid.NewGuid() + "_" + fileName.Replace(" ", "") + "_" + fileExtension;
                var path = Path.Combine(Server.MapPath("~/Content/UserDocs"), fileName);
                file_04.SaveAs(path);

                toCreate.file_04 = fileName;
            }

            if (file_05 != null && file_05.ContentLength > 0)
            {
                if (file_05.ContentLength > (2 * 1024 * 1024))
                {
                    return RedirectToAction("ManageEmployee", new { message = "image" });
                }

                var fileExtension = Path.GetExtension(file_05.FileName);
                var fileName = Path.GetFileName(file_05.FileName).Replace(fileExtension, "");
                fileName = Guid.NewGuid() + "_" + fileName.Replace(" ", "") + "_" + fileExtension;
                var path = Path.Combine(Server.MapPath("~/Content/UserDocs"), fileName);
                file_05.SaveAs(path);

                toCreate.file_05 = fileName;
            }

            int id = TimeTune.EmployeeCrud.add(toCreate);
            if (id == -1)
            {
                var json = JsonConvert.SerializeObject(toCreate);
                //AuditTrail.insert(json, "Employee", int.Parse(User.Identity.Name));
                return RedirectToAction("ManageEmployee", new { message = "already" });
            }
            else
            {
                var json = JsonConvert.SerializeObject(toCreate);
                AuditTrail.insert(json, "Employee", User.Identity.Name);
                return RedirectToAction("ManageEmployee", new { message = "success" });
            }
        }

        // This method is called by the delete modal.
        [HttpPost]
        public JsonResult DeleteEmployee(Employee toRemove)
        {
            //IR updated code in below method
            int id = TimeTune.EmployeeCrud.remove(toRemove);

            var json = JsonConvert.SerializeObject(toRemove);

            //IR Commented Code because of no use any more
            ////TimeTune.EmployeeCrud.InActivePersistentAttendanceLogsEmployees(int.Parse(User.Identity.Name));

            AuditTrail.delete(json, "Employee", User.Identity.Name);

            return Json(new { status = "success" });
        }

        [HttpPost]
        public void UploadFilesForUsers()
        {
            System.Threading.Thread.Sleep(1000);

            Session[Request.Files.AllKeys[0].ToString()] = "";
            var guid = Guid.NewGuid();

            string path = Server.MapPath("~/Content/UserDocs/");
            HttpFileCollectionBase files = Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string filename_guid = guid + file.FileName;
                file.SaveAs(path + filename_guid);

                Session[Request.Files.AllKeys[0].ToString()] = filename_guid;
            }
        }

        // This method is called by the delete modal.
        [HttpPost]
        public JsonResult EditEmployee(Employee toEdit)
        {
            if (toEdit.photograph != null)
            {
                string[] path_array = toEdit.photograph.Split('\\');
                string filename = path_array[path_array.Length - 1];

                if (Session["modal_photograph"] != null && Session["modal_photograph"].ToString() != "")
                {
                    filename = Session["modal_photograph"].ToString();
                    //Session["modal_photograph"] = null;
                }

                toEdit.photograph = filename;
            }

            if (toEdit.file_01 != null)
            {
                string[] path_array = toEdit.file_01.Split('\\');
                string filename = path_array[path_array.Length - 1];

                if (Session["modal_file_01"] != null && Session["modal_file_01"].ToString() != "")
                {
                    filename = Session["modal_file_01"].ToString();
                    //Session["modal_file_01"] = null;
                }

                toEdit.file_01 = filename;
            }

            if (toEdit.file_02 != null)
            {
                string[] path_array = toEdit.file_02.Split('\\');
                string filename = path_array[path_array.Length - 1];

                if (Session["modal_file_02"] != null && Session["modal_file_02"].ToString() != "")
                {
                    filename = Session["modal_file_02"].ToString();
                    //Session["modal_file_02"] = null;
                }

                toEdit.file_02 = filename;
            }

            if (toEdit.file_03 != null)
            {
                string[] path_array = toEdit.file_03.Split('\\');
                string filename = path_array[path_array.Length - 1];

                if (Session["modal_file_03"] != null && Session["modal_file_03"].ToString() != "")
                {
                    filename = Session["modal_file_03"].ToString();
                    //Session["modal_file_03"] = null;
                }

                toEdit.file_03 = filename;
            }

            if (toEdit.file_04 != null)
            {
                string[] path_array = toEdit.file_04.Split('\\');
                string filename = path_array[path_array.Length - 1];

                if (Session["modal_file_04"] != null && Session["modal_file_04"].ToString() != "")
                {
                    filename = Session["modal_file_04"].ToString();
                    //Session["modal_file_04"] = null;
                }

                toEdit.file_04 = filename;
            }

            if (toEdit.file_05 != null)
            {
                string[] path_array = toEdit.file_05.Split('\\');
                string filename = path_array[path_array.Length - 1];

                if (Session["modal_file_05"] != null && Session["modal_file_05"].ToString() != "")
                {
                    filename = Session["modal_file_05"].ToString();
                    //Session["modal_file_04"] = null;
                }

                toEdit.file_05 = filename;
            }


            TimeTune.EmployeeCrud.update(toEdit);
            var json = JsonConvert.SerializeObject(toEdit);
            AuditTrail.update(json, "Employee", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public JsonResult DataHandler(DTParameters param)
        {
            try
            {
                bool bGVIsSuperHRRole;
                int iGVCampusID;
                ResolveCampusScope(out bGVIsSuperHRRole, out iGVCampusID);

                var dtsource = new List<Employee>();

                // get all employee view models
                int count = EmployeeCrud.searchEmployees(bGVIsSuperHRRole, iGVCampusID, param.Search.Value, param.SortOrder, param.Start, param.Length, out dtsource);

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

        [HttpPost]
        public JsonResult GetEmployee()
        {
            string employee_id = Request.Form["employee_id"];

            int employeeID;

            if (employee_id != null && !employee_id.Equals("") && int.TryParse(employee_id, out employeeID))
            {
                Employee emp = EmployeeCrud.get(employeeID);

                return Json(new
                {
                    success = true,
                    id = emp.id,
                    campus_id = emp.campus_id,
                    gender_id = emp.gender_id,
                    campus_name = emp.campus_name,
                    gender_name = emp.gender_name,
                    pshift_id = emp.pshift_id,
                    pshift_name = emp.pshift_name,
                    first_name = emp.first_name,
                    last_name = emp.last_name,
                    father_name = emp.father_name,
                    employee_code = emp.employee_code,
                    cnic_no = emp.cnic_no,
                    email = emp.email,
                    address = emp.address,
                    mobile_no = emp.mobile_no,
                    date_of_joining = emp.date_of_joining,//(emp.date_of_joining != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_joining, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null,
                    date_of_leaving = emp.date_of_leaving,//(emp.date_of_leaving != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_leaving, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null,

                    function_id = (emp.function_id != -1) ? emp.function_id : 0,
                    function_name = (emp.function_name != null) ? emp.function_name : "",

                    designation_id = (emp.designation_id != -1) ? emp.designation_id : 0,
                    designation_name = (emp.designation_name != null) ? emp.designation_name : "",

                    department_id = (emp.department_id != -1) ? emp.department_id : 0,
                    department_name = (emp.department_name != null) ? emp.department_name : "",

                    type_of_employment_id = (emp.type_of_employment_id != -1) ? emp.type_of_employment_id : 0,
                    type_of_employment_name = (emp.type_of_employment_name != null) ? emp.type_of_employment_name : "",

                    access_group_id = (emp.access_group_id != -1) ? emp.access_group_id : 0,
                    access_group_name = (emp.access_group_name != null) ? emp.access_group_name : "",

                    region_id = (emp.region_id != -1) ? emp.region_id : 0,
                    region_name = (emp.region_name != null) ? emp.region_name : "",

                    grade_id = (emp.grade_id != -1) ? emp.grade_id : 0,
                    grade_name = (emp.grade_name != null) ? emp.grade_name : "",

                    time_tune_status_id = (emp.time_tune_status == "Active") ? "1" : "0",
                    time_tune_status_val = (emp.time_tune_status == "Active") ? "Active" : "Deactive",
                    location_id = (emp.location_id != -1) ? emp.location_id : 0,
                    location_name = (emp.location_name != null) ? emp.location_name : "",

                    position_id = (emp.position_id != -1) ? emp.position_id : 0,
                    position_name = (emp.position_name != null) ? emp.position_name : "",

                    site_id = (emp.site_id != -1) ? emp.site_id : 0,
                    site_name = (emp.site_name != null) ? emp.site_name : "",

                    sick_leaves = emp.sick_leaves,
                    casual_leaves = emp.casual_leaves,
                    annual_leaves = emp.annual_leaves,
                    other_leaves = emp.other_leaves,
                    leave_type01 = emp.leave_type01,
                    leave_type02 = emp.leave_type02,
                    leave_type03 = emp.leave_type03,
                    leave_type04 = emp.leave_type04,
                    leave_type05 = emp.leave_type05,
                    leave_type06 = emp.leave_type06,
                    leave_type07 = emp.leave_type07,
                    leave_type08 = emp.leave_type08,
                    leave_type09 = emp.leave_type09,
                    leave_type10 = emp.leave_type10,
                    leave_type11 = emp.leave_type11,
                    emergency_contact_01 = emp.emergency_contact_01 ?? "n/a",
                    emergency_contact_02 = emp.emergency_contact_02 ?? "n/a",
                    description = emp.description,
                    date_of_birth = emp.date_of_birth,
                    skills_set = emp.g_skills_set
                });
            }
            else
            {
                return Json(new { success = false });
            }

            /*{
                    id = ''
                    first_name= ''
                    last_name= ''
                    employee_code= ''
                    email= ''
                    address= ''
                    mobile_no= ''
                    date_of_joining= ''
                    date_of_leaving= ''
                    function_id= ''
                    designation_id= ''
                    department_id= ''
                    type_of_employment_id= ''
                    access_group_id= ''
                    region_id= ''
                    grade_id= ''
                    location_id= ''
                }
             */


        }

        #region ChosenAjaxDatahandlers
        /* 
         * checkimg chosen ajaxification list.
         */

        private const int ChosenAjaxSearchMaxResults = 10;

        private static int CapChosenAjaxResults(int totalCount)
        {
            return totalCount > ChosenAjaxSearchMaxResults ? ChosenAjaxSearchMaxResults : totalCount;
        }

        // campuses
        [HttpPost]
        public JsonResult campuses()
        {

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; iGVCampusID = int.Parse(ViewModel.GlobalVariables.GV_EmployeeCampusID);

            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.OrganizationCampusView[] campuses = TimeTune.EmployeeManagementHelper.getAllCampusesMatching(q, bGVIsSuperHRRole, iGVCampusID);

            int resultCount = CapChosenAjaxResults(campuses.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = campuses[i].Id.ToString();
                toSend[i].text = campuses[i].CampusCode;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // pshifts
        [HttpPost]
        public JsonResult pshifts()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.OrganizationProgramShiftView[] pshifts = TimeTune.EmployeeManagementHelper.getAllPShiftsMatching(q);

            int resultCount = CapChosenAjaxResults(pshifts.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = pshifts[i].Id.ToString();
                toSend[i].text = pshifts[i].ProgramShiftName;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // genders
        [HttpPost]
        public JsonResult genders()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.GenderView[] genders = TimeTune.EmployeeManagementHelper.getAllGendersMatching(q);

            int resultCount = CapChosenAjaxResults(genders.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = genders[i].Id.ToString();
                toSend[i].text = genders[i].GenderName;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // functions
        [HttpPost]
        public JsonResult functions()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Function[] functions = TimeTune.EmployeeManagementHelper.getAllFunctionsMatching(q);

            int resultCount = CapChosenAjaxResults(functions.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = functions[i].id.ToString();
                toSend[i].text = functions[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // designations
        [HttpPost]
        public JsonResult designations()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Designation[] designations = TimeTune.EmployeeManagementHelper.getAllDesignationsMatching(q);

            int resultCount = CapChosenAjaxResults(designations.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = designations[i].id.ToString();
                toSend[i].text = designations[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // departments
        [HttpPost]
        public JsonResult departments()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Department[] departments = TimeTune.EmployeeManagementHelper.getAllDepartmentsMatching(q);

            int resultCount = CapChosenAjaxResults(departments.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = departments[i].id.ToString();
                toSend[i].text = departments[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // type of employments
        [HttpPost]
        public JsonResult typeOfEmployments()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.TypeOfEmployment[] typeOEmployments = TimeTune.EmployeeManagementHelper.getAllTypeOfEmploymentsMatching(q);

            int resultCount = CapChosenAjaxResults(typeOEmployments.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = typeOEmployments[i].id.ToString();
                toSend[i].text = typeOEmployments[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // regions
        [HttpPost]
        public JsonResult regions()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Region[] regions = TimeTune.EmployeeManagementHelper.getAllRegionsMatching(q);

            int resultCount = CapChosenAjaxResults(regions.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = regions[i].id.ToString();
                toSend[i].text = regions[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        // grades
        [HttpPost]
        public JsonResult grades()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Grades[] grades = TimeTune.EmployeeManagementHelper.getAllGradesMatching(q);

            int resultCount = CapChosenAjaxResults(grades.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = grades[i].id.ToString();
                toSend[i].text = grades[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // locations
        [HttpPost]
        public JsonResult locations()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Location[] locations = TimeTune.EmployeeManagementHelper.getAllLocationsMatching(q);

            int resultCount = CapChosenAjaxResults(locations.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = locations[i].id.ToString();
                toSend[i].text = locations[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        // positions
        [HttpPost]
        public JsonResult positions()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Position_Status[] locations = TimeTune.EmployeeManagementHelper.getAllPositionsMatching(q);

            int resultCount = CapChosenAjaxResults(locations.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = locations[i].id.ToString();
                toSend[i].text = locations[i].position_text;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        // sites
        [HttpPost]
        public JsonResult sites()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Site_Status[] locations = TimeTune.EmployeeManagementHelper.getAllSitesMatching(q);

            int resultCount = CapChosenAjaxResults(locations.Length);
            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = locations[i].id.ToString();
                toSend[i].text = locations[i].site_text;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }



        #endregion

        #endregion

        #region ChangeEmployeePassword

        public ActionResult ChangeEmployeePassword(string error)
        {
            if (error != null && !error.Equals(""))
                ModelState.AddModelError("", error);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmployeePassword()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["change_password_employee"], out employeeID))
                return RedirectToAction("ChangeEmployeePassword", new { error = "Invalid emplyoee id." });

            Employee emp = TimeTune.EmployeeCrud.get(employeeID);
            if (emp == null)
                return RedirectToAction("ChangeEmployeePassword", new { error = "Invalid emplyoee id." });


            string newPassword = Request.Form["employee_password"];
            if (newPassword == null || newPassword.Equals(""))
                return RedirectToAction("ChangeEmployeePassword", new { error = "Password is required." });

            int id = TimeTune.EmployeeCrud.setPassword(employeeID, newPassword);

            //Aduit Log
            var json = JsonConvert.SerializeObject(emp);
            AuditTrail.update(json, "EmployeePasswordChange", User.Identity.Name);

            return RedirectToAction("ChangeEmployeePassword");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPasswordFile(HttpPostedFileBase file)
        {
            string result = null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                string path = Path.Combine(Server.MapPath("~/Uploads"),
                                   "ChangePassword.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                result = TimeTune.EmployeeCrud.bulkSetPassword(content);
                return RedirectToAction("ChangeEmployeePassword", new { result = result });


            }
            return RedirectToAction("ChangeEmployeePassword", "EmployeeManagement", new { result = "Select File first" });
        }



        [HttpPost]
        public JsonResult ChangeEmployeePasswordDataHandler()
        {
            string q = Request.Form["data[q]"] ?? "";

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

        [HttpPost]
        public JsonResult SearchEmployeeCodeNameDataHandler()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            List<ViewModels.SearchEmployeeData> employees = TimeTune.EmployeeSearch.getSearchEmployeeCodeName(q);

            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Count];
            for (int i = 0; i < employees.Count; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].EmpID.ToString();
                toSend[i].text = employees[i].EmpText;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        [HttpPost]
        public JsonResult SearchEmployeesListDataHandler()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getSearchEmployeesMatching(q);


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

        [HttpPost]
        public JsonResult EmployeeCodesInDDLDataHandler()
        {
            string q = Request.Form["data[q]"] ?? "";

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

        [HttpPost]
        public JsonResult ChangeEmployeePasswordExceptSuperHRsDataHandler()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatchingExceptSuperHRs(q);


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

        #endregion


        #region Upload Download Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<TimeTune.ManageEmployeeImportExport.ManageEmployeeCSV>
                    (
                        TimeTune.ManageEmployeeImportExport.getManageEmployeeCSV(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-Users-List.csv"
                    );


            return toReturn;
        }


        // Handle file upload and read/write etc.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = Path.GetFileName(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName).ToLower();

                if (FileExtension != ".csv")
                {
                    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_employee.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidCampusCode = false, invalidCampusCodeAllowed = false, invalidEcode = false, invalidFName = false, invalidLName = false, invalidJoiningDate = false, invalidLeftDate = false, invalidBirthDate = false, invalidCols = false, invalidRowsCount = false;

                    //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                    bool isDataRowFound = false; int a = 0;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            a++;

                            if (a == 2)
                            {
                                isDataRowFound = true;
                                break;
                            }
                        }
                    }

                    if (!isDataRowFound)
                    {
                        return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////

                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            strReadLine = strReadLine.TrimEnd(',');
                            strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                            strReadLine = strReadLine.Replace("\"", "");
                            strReadLine = strReadLine.TrimEnd(',');

                            string new_code = "";
                            string[] ecode_dt = strReadLine.Split(',');
                            if (ecode_dt.Length == 47)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("employeecode") || ecode_dt[6].ToLower().Contains("dateofjoining") || ecode_dt[34].ToLower().Contains("date_of_birth") || ecode_dt[38].ToLower().Contains("program_shift"))
                                {
                                    continue;
                                }


                                //student
                                if (ecode_dt[11] != null && ecode_dt[11].ToString().ToLower() == "student")
                                {
                                    if (ecode_dt[0].Length == 11)
                                    {
                                        new_code = ecode_dt[0];
                                    }
                                    else
                                    {
                                        if (ecode_dt[0].Length == 1)
                                            new_code = "0000000000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 2)
                                            new_code = "000000000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 3)
                                            new_code = "00000000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 4)
                                            new_code = "0000000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 5)
                                            new_code = "000000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 6)
                                            new_code = "00000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 7)
                                            new_code = "0000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 8)
                                            new_code = "000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 9)
                                            new_code = "00" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 10)
                                            new_code = "0" + ecode_dt[0];
                                        else
                                            new_code = "";
                                    }
                                }
                                else//normal employee
                                {
                                    if (ecode_dt[0].Length == 6)
                                    {
                                        new_code = ecode_dt[0];
                                    }
                                    else
                                    {
                                        if (ecode_dt[0].Length == 1)
                                            new_code = "00000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 2)
                                            new_code = "0000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 3)
                                            new_code = "000" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 4)
                                            new_code = "00" + ecode_dt[0];
                                        else if (ecode_dt[0].Length == 5)
                                            new_code = "0" + ecode_dt[0];
                                        else
                                            new_code = "";
                                    }
                                }

                                //validate employee code

                                if (!ValidateCampusCode(ecode_dt[44]))
                                {
                                    invalidCampusCode = true;
                                    result = "Invalid Campus-Code Found at Row-" + counter;
                                    break;
                                }

                                int iGVCampusID = 0;
                                if (!int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID))
                                {
                                    iGVCampusID = 0; // Default to 0 if parsing fails
                                }
                                if (!ValidateCampusCodeAllowed(iGVCampusID, ecode_dt[44]))
                                {
                                    invalidCampusCodeAllowed = true;
                                    result = "NOT Allowed Campus-Code Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateEmployeeCode(new_code))
                                {
                                    invalidEcode = true;
                                    result = "Invalid User Code Found at Row-" + counter;
                                    break;
                                }

                                //validate First Name
                                if (!ValidateName(ecode_dt[1]))
                                {
                                    invalidFName = true;
                                    result = "Invalid First Name Found at Row-" + counter;
                                    break;
                                }

                                //validate Last Name
                                if (!ValidateName(ecode_dt[2]))
                                {
                                    invalidLName = true;
                                    result = "Invalid Last Name Found at Row-" + counter;
                                    break;
                                }

                                //validate Date of Joining
                                if (ecode_dt[6] != null && ecode_dt[6].ToString() != "")
                                {
                                    if (!ValidateDate(ecode_dt[6]))
                                    {
                                        invalidJoiningDate = true;
                                        result = "Invalid Joining Date Found at Row-" + counter;
                                        break;
                                    }
                                }

                                //validate Date of Left
                                if (ecode_dt[7] != null && ecode_dt[7].ToString() != "")
                                {
                                    if (!ValidateDate(ecode_dt[7]))
                                    {
                                        invalidLeftDate = true;
                                        result = "Invalid Leaving Date Found at Row-" + counter;
                                        break;
                                    }
                                }

                                //validate Date of Birth
                                if (ecode_dt[41] != null && ecode_dt[41].ToString() != "")
                                {
                                    if (!ValidateDate(ecode_dt[41]))
                                    {
                                        invalidBirthDate = true;
                                        result = "Invalid Birth Date Found at Row-" + counter;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                invalidCols = true;
                                result = "Invalid Col(s) Found";
                                break;
                            }

                            //iterate to replace EmployeeCode only having 0 as prefix
                            for (int i = 0; i < ecode_dt.Length; i++)
                            {
                                if (i == 0)
                                {
                                    strReadLine = new_code + ",";
                                }
                                else
                                {
                                    strReadLine += ecode_dt[i] + ",";
                                }
                            }

                            strReadLine = strReadLine.TrimEnd(',');
                            content.Add(strReadLine);

                            //restrict to upload if 1000+ rows are found
                            /*if (counter > 1000)
                            {
                                invalidRowsCount = true;
                                result = "Max 1000 records are allowed be uploaded";
                                break;
                            }*/
                        }
                    }

                    if (invalidEcode || invalidFName || invalidCampusCode || invalidCampusCodeAllowed || invalidLName || invalidJoiningDate || invalidLeftDate || invalidBirthDate || invalidCols)
                    {
                        return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = result });
                    }
                    else
                    {
                        result = TimeTune.ManageEmployeeImportExport.setEmployees(content, User.Identity.Name);
                    }
                }

                if (result == "failed")
                {
                    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Successfull" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Select File first" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadLeaveSessionFile(HttpPostedFileBase file)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = Path.GetFileName(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName).ToLower();

                if (FileExtension != ".csv")
                {
                    return RedirectToAction("LeaveSession", "EmployeeManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_leaves_session.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidCampusCode = false, invalidCampusCodeAllowed = false, invalidEcode = false, invalidFName = false, invalidLName = false, invalidStartDate = false, invalidEndDate = false, invalidBirthDate = false, invalidCols = false, invalidRowsCount = false;

                    //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                    bool isDataRowFound = false; int a = 0;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            a++;

                            if (a == 2)
                            {
                                isDataRowFound = true;
                                break;
                            }
                        }
                    }

                    if (!isDataRowFound)
                    {
                        return RedirectToAction("LeaveSession", "EmployeeManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////

                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            strReadLine = strReadLine.TrimEnd(',');
                            strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                            strReadLine = strReadLine.Replace("\"", "");
                            strReadLine = strReadLine.TrimEnd(',');

                            string new_code = "";
                            string[] ecode_dt = strReadLine.Split(',');
                            if (ecode_dt.Length == 18)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("employeecode") || ecode_dt[1].ToLower().Contains("dateofstart") || ecode_dt[2].ToLower().Contains("dateofend"))
                                {
                                    continue;
                                }

                                if (ecode_dt[0].Length == 6)
                                {
                                    new_code = ecode_dt[0];
                                }
                                else
                                {
                                    if (ecode_dt[0].Length == 1)
                                        new_code = "00000" + ecode_dt[0];
                                    else if (ecode_dt[0].Length == 2)
                                        new_code = "0000" + ecode_dt[0];
                                    else if (ecode_dt[0].Length == 3)
                                        new_code = "000" + ecode_dt[0];
                                    else if (ecode_dt[0].Length == 4)
                                        new_code = "00" + ecode_dt[0];
                                    else if (ecode_dt[0].Length == 5)
                                        new_code = "0" + ecode_dt[0];
                                    else
                                        new_code = "";
                                }

                                //validate employee code

                                if (!ValidateEmployeeCode(new_code))
                                {
                                    invalidEcode = true;
                                    result = "Invalid User Code Found at Row-" + counter;
                                    break;
                                }

                                //validate Date of Joining
                                if (ecode_dt[1] != null && ecode_dt[1].ToString() != "")
                                {
                                    if (!ValidateDate(ecode_dt[1]))
                                    {
                                        invalidStartDate = true;
                                        result = "Invalid Start Date Found at Row-" + counter;
                                        break;
                                    }
                                }

                                //validate Date of Left
                                if (ecode_dt[2] != null && ecode_dt[2].ToString() != "")
                                {
                                    if (!ValidateDate(ecode_dt[2]))
                                    {
                                        invalidEndDate = true;
                                        result = "Invalid End Date Found at Row-" + counter;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                invalidCols = true;
                                result = "Invalid Col(s) Found";
                                break;
                            }

                            //iterate to replace EmployeeCode only having 0 as prefix
                            for (int i = 0; i < ecode_dt.Length; i++)
                            {
                                if (i == 0)
                                {
                                    strReadLine = new_code + ",";
                                }
                                else
                                {
                                    strReadLine += ecode_dt[i] + ",";
                                }
                            }

                            strReadLine = strReadLine.TrimEnd(',');
                            content.Add(strReadLine);

                            //restrict to upload if 1000+ rows are found
                            /*if (counter > 1000)
                            {
                                invalidRowsCount = true;
                                result = "Max 1000 records are allowed be uploaded";
                                break;
                            }*/
                        }
                    }

                    if (invalidEcode || invalidStartDate || invalidEndDate || invalidBirthDate || invalidCols)
                    {
                        return RedirectToAction("LeaveSession", "EmployeeManagement", new { result = result });
                    }
                    else
                    {
                        result = TimeTune.ManageEmployeeImportExport.setLeaveSession(content, User.Identity.Name);
                    }
                }

                if (result == "failed")
                {
                    return RedirectToAction("LeaveSession", "EmployeeManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("LeaveSession", "EmployeeManagement", new { result = "Successfull" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("LeaveSession", "EmployeeManagement", new { result = "Select File first" });
        }

        private bool ValidateCampusCode(string strCampusCode)
        {
            bool isValid = false;

            isValid = ManageEmployeeImportExport.validateCampusCode(strCampusCode);

            return isValid;
        }

        private bool ValidateCampusCodeAllowed(int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            isValid = ManageEmployeeImportExport.validateCampusCodeAllowed(iCampusID, strCampusCode);

            return isValid;
        }

        private bool ValidateEmployeeCode(string strEmployeeCode)
        {
            bool isValid = true;

            Regex smallAlphaPattern = new Regex("[a-z]");
            Regex capsAlphaPattern = new Regex("[A-Z]");
            //Regex numeralsPattern = new Regex("[0-9]");
            Regex specialPattern = new Regex("[!@#$%^&*()_,.<>?;':-]");

            if (strEmployeeCode.Length < 6)
            {
                isValid = false;
            }
            else if (smallAlphaPattern.IsMatch(strEmployeeCode))
            {
                isValid = false;
            }
            else if (capsAlphaPattern.IsMatch(strEmployeeCode))
            {
                isValid = false;
            }
            else if (specialPattern.IsMatch(strEmployeeCode))
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateName(string strName)
        {
            bool isValid = true;

            //Regex smallAlphaPattern = new Regex("[a-z]");
            //Regex capsAlphaPattern = new Regex("[A-Z]");
            Regex numeralsPattern = new Regex("[0-9]");
            Regex specialPattern = new Regex("[!@#$%^&*()_,<>?;':<>]");

            if (strName.Length == 0)
            {
                isValid = false;
            }
            else if (numeralsPattern.IsMatch(strName))
            {
                isValid = false;
            }
            else if (specialPattern.IsMatch(strName))
            {
                isValid = false;
            }

            return isValid;
        }


        private bool ValidateDate(string strDate)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (!DateTime.TryParse(strProperDate, out dtTest))
            {
                isValid = false;
            }

            return isValid;
        }


        private bool ValidateDate_BK(string strDateOfJoining)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            Regex smallAlphaPattern = new Regex("[a-z]");
            Regex capsAlphaPattern = new Regex("[A-Z]");
            //Regex numeralsPattern = new Regex("[0-9]");
            Regex fixedDatePattern = new Regex(@"[/]");

            if (!DateTime.TryParse(strDateOfJoining, out dtTest))
            {
                isValid = false;
            }

            if (!(strDateOfJoining.Length == 10))
            {
                isValid = false;
            }
            else if (smallAlphaPattern.IsMatch(strDateOfJoining))
            {
                isValid = false;
            }
            else if (capsAlphaPattern.IsMatch(strDateOfJoining))
            {
                isValid = false;
            }
            else if (!fixedDatePattern.IsMatch(strDateOfJoining))
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidatePassword(string strPassword)
        {
            bool isValid = true;

            Regex smallAlphaPattern = new Regex("[a-z]");
            Regex capsAlphaPattern = new Regex("[A-Z]");
            Regex numeralsPattern = new Regex("[0-9]");
            Regex specialPattern = new Regex("[!@#$%^&*()_-]");

            if (strPassword.Length < 6)
            {
                isValid = false;
            }
            else if (!smallAlphaPattern.IsMatch(strPassword))
            {
                isValid = false;
            }
            else if (!capsAlphaPattern.IsMatch(strPassword))
            {
                isValid = false;
            }
            else if (!numeralsPattern.IsMatch(strPassword))
            {
                isValid = false;
            }
            else if (!specialPattern.IsMatch(strPassword))
            {
                isValid = false;
            }

            return isValid;
        }


        #endregion

        #region SkillSets

        public ActionResult SkillsSet()
        {

            return View();
        }

        [HttpPost]
        public JsonResult SkillSetDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.SkillsSet>();
                // get all employee view models
                dtsource = TimeTune.EmployeeSkillsSet.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.SkillsSet> data = SkillResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = SkillResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.SkillsSet> result = new DTResult<ViewModels.SkillsSet>
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
        public ActionResult AddSkillSet(ViewModels.SkillsSet fromForm)
        {
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeSkillsSet.add(fromForm);
            if (id == 0)
            {
                return RedirectToAction("SkillsSet", new { Message = "already" });
            }
            return RedirectToAction("SkillsSet", new { Message = "success" });
        }

        [HttpPost]
        public ActionResult UpdateSkillSet(ViewModels.SkillsSet toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.EmployeeSkillsSet.update(toUpdate);
            AuditTrail.update(json, "SkillSet", User.Identity.Name);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveSkillSet(ViewModels.SkillsSet toRemove)
        {
            var entity = TimeTune.EmployeeSkillsSet.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "SkillSet", User.Identity.Name);
            return Json(new { status = "success" });
        }
        #endregion

        #region EmployeesBirthday
        public ActionResult EmpBirthdays(string result)
        {
            EMP_BirthdayEvent obj = new EMP_BirthdayEvent();
            var dtsource = EmployeeCrud.getEmpTodaysBirthdays();
            obj.TodayBirthdays = dtsource;


            var dtsource2 = EmployeeCrud.getEmpUpcomingBirthdays();
            obj.UpcomingBirthdays = dtsource2;

            return View(obj);
        }

        [HttpPost]
        public JsonResult BirthdaysTodayDataHandler(DTParameters param)
        {
            try
            {
                //  var dtsource = new List<Employee>();

                var dtsource = EmployeeCrud.getEmpTodaysBirthdays();

                DTResult<Employee> result = new DTResult<Employee>
                {
                    draw = param.Draw,
                    data = dtsource,
                    recordsFiltered = dtsource.Count,
                    recordsTotal = dtsource.Count
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
        public JsonResult BirthdaysUpcomingDataHandler(DTParameters param)
        {
            try
            {
                //  var dtsource = new List<Employee>();

                var dtsource = EmployeeCrud.getEmpUpcomingBirthdays();

                DTResult<Employee> result = new DTResult<Employee>
                {
                    draw = param.Draw,
                    data = dtsource,
                    recordsFiltered = dtsource.Count,
                    recordsTotal = dtsource.Count
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

        #region EmployeeEvaluation
        public ActionResult EmployeeEvaluation()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddEmpEvaluation(ViewModels.EmployeeEvaluation fromForm)
        {
            //var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.Employee_Evaluation.add(fromForm);
            if (id == 0)
            {
                return RedirectToAction("EmployeeEvaluation", new { message = "already" });
            }
            return RedirectToAction("EmployeeEvaluation", new { message = "success" });
        }

        #endregion

        #region Letters
        [HttpGet]
        public ActionResult Letters()
        {
            EmpLetterViewModel vm = new EmpLetterViewModel();
            vm.ltt = Employee_Letter.getAll();

            return View(vm);
        }

        [AllowHtml]
        public string SomeProperty { get; set; }

        [HttpPost]
        [ActionName("Letters")]
        [ValidateAntiForgeryToken]
        public ActionResult Letters_Post()
        {

            SomeProperty = Request.Unvalidated.Form["text99"].ToString();

            if (SomeProperty == null)
                return RedirectToAction("PerformanceReport");

            int found = 0; ViewData["PDFNoDataFound"] = "";

            found = GenerateLetterPDF(SomeProperty);

            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found/Error Occurred.";
            }

            EmpLetterViewModel vm = new EmpLetterViewModel();
            vm.ltt = Employee_Letter.getAll();

            return View(vm);

        }

        private int GenerateLetterPDF(string data)
        {
            int reponse = 0;
            try
            {

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    IronPdf.ChromePdfRenderer Renderer = new IronPdf.ChromePdfRenderer();
                    //var Renderer = new ironPdf.HtmlToPdf();
                    
                    //Renderer.RenderingOptions.FitToPaperWidth = true; ---this line need to be replaced with correct function
                    Renderer.RenderingOptions.CreatePdfFormsFromHtml = true;
                    Renderer.RenderingOptions.Zoom = 100;
                    


                    Renderer.RenderingOptions.MarginBottom = 6;
                    Renderer.RenderingOptions.MarginLeft = 5;
                    Renderer.RenderingOptions.MarginRight = 5;
                    Renderer.RenderingOptions.MarginTop = 10;

                    var PDFg = Renderer.RenderHtmlAsPdf(data);


                    //var OutputPath = @"E:\HtmlToPDF.pdf";
                    //Pdfg.Save(OutputPath);

                    //----------------------ItextSharpPdf---------------------
                    //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);  
                    //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    //StringReader sr = new StringReader(data.ToString());  
                    //  PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);  
                    //  pdfDoc.Open();

                    //  htmlparser.Parse(sr);  
                    //  pdfDoc.Close();  

                    //  byte[] bytes = memoryStream.ToArray();  
                    //  memoryStream.Close();  
                    //--------------------------------------------



                    byte[] db = new byte[PDFg.Stream.Length];
                    db = PDFg.Stream.GetBuffer();
                    MemoryStream ms = new MemoryStream(db);
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=Report.pdf");
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();

                    reponse = 1;
                }
            }

            catch (Exception)
            {
                reponse = -1;
            }

            return reponse;
        }


        public class LetterParameter : DTParameters
        {
            public string LetterName { get; set; }
            public string LetterFormate { get; set; }

        }

        [HttpPost]
        public JsonResult Letter_Formate(LetterParameter param)
        {
            string _Letter = param.LetterName.ToString();
            string data = "";
            try
            {
                if (param.LetterName != null && param.LetterName.ToString() != "")
                {

                    data = TimeTune.Employee_Letter.get(_Letter);
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }

            return Json(data);
        }


        [HttpPost]
        [ActionName("AddLetter")]
        public ActionResult AddLetter(LetterParameter param)
        {

            int id = TimeTune.Employee_Letter.add(param.LetterFormate, param.LetterName);
            if (id == -1)
            {
                return Json(new { status = "already" });
            }

            return Json(new { status = "success" });
        }

        #endregion



        public ActionResult EmployeeHierarchy()
        {
            return View();
        }

        public ActionResult ManualUploads()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ManualUploads(HttpPostedFileBase file)
        {
            if (Request.Files.Count > 0)
            {

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine("D:\\TimeTune", fileName);
                    string line;
                    string[] words;
                    if (fileName.Equals("function.csv"))
                        using (StreamReader reader = new StreamReader(path))
                        {
                            string buff;

                            while ((line = reader.ReadLine()) != null)
                            {
                                words = line.Split(',');

                            }

                        }

                }
            }
            return View();
        }

        public ActionResult HierarchyTransfer()
        {
            return View();
        }

        #region Exempted-Employees

        public ActionResult EmployeeExempted()
        {


            return View();
        }

        [HttpPost]
        public ActionResult EmployeeExempted(ExmptEmployee toCreate)
        {


            int id = TimeTune.ExemptedEmplyeeCrud.Add(toCreate);

            if (id == -1)
            {
                return RedirectToAction("EmployeeExempted", new { Message = "already" });

            }

            var json = JsonConvert.SerializeObject(toCreate);
            AuditTrail.insert(json, "EmployeeExempted", User.Identity.Name);

            return RedirectToAction("EmployeeExempted", new { Message = "success" });

            //return View();
        }

        [HttpPost]
        public JsonResult EmployeeExemptedDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.ExmptEmployee>();
                // get all employee view models
                dtsource = TimeTune.ExemptedEmplyeeCrud.getAllData();
                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }

                List<ViewModels.ExmptEmployee> data = ExmptEmployeeResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);
                data = data.OrderByDescending(o => o.id).ToList();

                int count = ExmptEmployeeResultSet.Count(param.Search.Value, dtsource);

                DTResult<ViewModels.ExmptEmployee> result = new DTResult<ViewModels.ExmptEmployee>
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
        public ActionResult UpdateEmployeeExempted(ViewModels.ExmptEmployee toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            int response = TimeTune.ExemptedEmplyeeCrud.Update(toUpdate);

            if (response == -1)
            {
                return Json(new { status = "already" });
            }

            AuditTrail.update(json, "EmployeeExempted", User.Identity.Name);

            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveEmployeeExempted(ViewModels.ExmptEmployee toRemove)
        {
            var entity = TimeTune.ExemptedEmplyeeCrud.Remove(toRemove);

            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "EmployeeExempted", User.Identity.Name);

            return Json(new { status = "success" });
        }

        #endregion

        #region LeaveType

        public ActionResult LeaveType()
        {
            int lastInactiveID = 0;
            lastInactiveID = TimeTune.LeaveTypeCrud.LastInactiveID();

            ViewBag.LastInactiveID = lastInactiveID;

            return View();
        }

        [HttpPost]
        public ActionResult LeaveType(LeaveType toCreate)
        {
            int id = TimeTune.LeaveTypeCrud.AddActuallyUpdate(toCreate);

            if (id == -1)
            {
                return RedirectToAction("LeaveType", new { Message = "already" });
            }
            else if (id == -2)
            {
                return RedirectToAction("LeaveType", new { Message = "already-name" });
            }
            else if (id == -3)
            {
                return RedirectToAction("LeaveType", new { Message = "already-leave" });
            }

            var json = JsonConvert.SerializeObject(toCreate);
            AuditTrail.insert(json, "LeaveType", User.Identity.Name);

            return RedirectToAction("LeaveType", new { Message = "success" });

            //return View();
        }

        [HttpPost]
        public JsonResult LeaveTypeDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.LeaveType>();
                // get all employee view models
                dtsource = TimeTune.LeaveTypeCrud.getAllLeaveTypesData();
                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }

                List<ViewModels.LeaveType> data = LeaveTypeResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);
                data = data.OrderByDescending(o => o.Id).ToList();

                int count = LeaveTypeResultSet.Count(param.Search.Value, dtsource);

                DTResult<ViewModels.LeaveType> result = new DTResult<ViewModels.LeaveType>
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
        public ActionResult UpdateLeaveType(ViewModels.LeaveType toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            int response = TimeTune.LeaveTypeCrud.Update(toUpdate);

            if (response == -1)
            {
                return Json(new { status = "already" });
            }
            else if (response == -2)
            {
                return Json(new { status = "already-name" });
            }

            AuditTrail.update(json, "LeaveType", User.Identity.Name);

            int lastInactiveID = 0;
            lastInactiveID = TimeTune.LeaveTypeCrud.LastInactiveID();

            ViewBag.LastInactiveID = lastInactiveID;

            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLeaveType(ViewModels.LeaveType toRemove)
        {
            int response = 0;

            response = TimeTune.LeaveTypeCrud.Remove(toRemove);

            if (response == -1)
            {
                return Json(new { status = "already" });
            }

            var json = JsonConvert.SerializeObject(toRemove);
            AuditTrail.delete(json, "LeaveType", User.Identity.Name);

            return Json(new { status = "success" });
        }

        #endregion

        #region LeaveSession

        public ActionResult LeaveSession(string result)
        {
            string value = result;
            ViewBag.Message = result;

            ////max and default leaves counter
            //if (ConfigurationManager.AppSettings["MaxSickLeaves"] != null && ConfigurationManager.AppSettings["MaxSickLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxSickLeaves = ConfigurationManager.AppSettings["MaxSickLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxSickLeaves = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxCasualLeaves"] != null && ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxCasualLeaves = ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxCasualLeaves = "15";
            //}

            //if (ConfigurationManager.AppSettings["MaxAnnualLeaves"] != null && ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxAnnualLeaves = ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxAnnualLeaves = "20";
            //}

            //if (ConfigurationManager.AppSettings["MaxOtherLeaves"] != null && ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString() != "")
            //{
            //    ViewBag.MaxOtherLeaves = ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxOtherLeaves = "99";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType01"] != null && ConfigurationManager.AppSettings["MaxLeaveType01"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType01 = ConfigurationManager.AppSettings["MaxLeaveType01"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType01 = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType02"] != null && ConfigurationManager.AppSettings["MaxLeaveType02"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType02 = ConfigurationManager.AppSettings["MaxLeaveType02"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType02 = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType03"] != null && ConfigurationManager.AppSettings["MaxLeaveType03"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType03 = ConfigurationManager.AppSettings["MaxLeaveType03"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType03 = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType04"] != null && ConfigurationManager.AppSettings["MaxLeaveType04"].ToString() != "")
            //{
            //    ViewBag.MaxLeaveType04 = ConfigurationManager.AppSettings["MaxLeaveType04"].ToString();
            //}
            //else
            //{
            //    ViewBag.MaxLeaveType04 = "10";
            //}

            ////Default
            //if (ConfigurationManager.AppSettings["DefaultSickLeaves"] != null && ConfigurationManager.AppSettings["DefaultSickLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultSickLeaves = ConfigurationManager.AppSettings["DefaultSickLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultSickLeaves = "8";
            //}

            //if (ConfigurationManager.AppSettings["DefaultCasualLeaves"] != null && ConfigurationManager.AppSettings["DefaultCasualLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultCasualLeaves = ConfigurationManager.AppSettings["DefaultCasualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultCasualLeaves = "10";
            //}

            //if (ConfigurationManager.AppSettings["DefaultAnnualLeaves"] != null && ConfigurationManager.AppSettings["DefaultAnnualLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultAnnualLeaves = ConfigurationManager.AppSettings["DefaultAnnualLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultAnnualLeaves = "14";
            //}

            //if (ConfigurationManager.AppSettings["DefaultOtherLeaves"] != null && ConfigurationManager.AppSettings["DefaultOtherLeaves"].ToString() != "")
            //{
            //    ViewBag.DefaultOtherLeaves = ConfigurationManager.AppSettings["DefaultOtherLeaves"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultOtherLeaves = "20";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType01"] != null && ConfigurationManager.AppSettings["DefaultLeaveType01"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType01 = ConfigurationManager.AppSettings["DefaultLeaveType01"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType01 = "10";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType02"] != null && ConfigurationManager.AppSettings["DefaultLeaveType02"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType02 = ConfigurationManager.AppSettings["DefaultLeaveType02"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType02 = "10";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType03"] != null && ConfigurationManager.AppSettings["DefaultLeaveType03"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType03 = ConfigurationManager.AppSettings["DefaultLeaveType03"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType03 = "10";
            //}

            //if (ConfigurationManager.AppSettings["DefaultLeaveType04"] != null && ConfigurationManager.AppSettings["DefaultLeaveType04"].ToString() != "")
            //{
            //    ViewBag.DefaultLeaveType04 = ConfigurationManager.AppSettings["DefaultLeaveType04"].ToString();
            //}
            //else
            //{
            //    ViewBag.DefaultLeaveType04 = "10";
            //}

            LeaveTypesCountStatus vm = new LeaveTypesCountStatus();

            int[] leaves = new int[45] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            leaves = TimeTune.LeaveSessionCrud.GetDefaultMaxLeavesCountStatus();
            ViewBag.DefaultSickLeaves = leaves[0];
            ViewBag.DefaultCasualLeaves = leaves[3];
            ViewBag.DefaultAnnualLeaves = leaves[6];
            ViewBag.DefaultOtherLeaves = leaves[9];
            ViewBag.DefaultLeaveType01 = leaves[12];
            ViewBag.DefaultLeaveType02 = leaves[15];
            ViewBag.DefaultLeaveType03 = leaves[18];
            ViewBag.DefaultLeaveType04 = leaves[21];
            ViewBag.DefaultLeaveType05 = leaves[24];
            ViewBag.DefaultLeaveType06 = leaves[27];
            ViewBag.DefaultLeaveType07 = leaves[30];
            ViewBag.DefaultLeaveType08 = leaves[33];
            ViewBag.DefaultLeaveType09 = leaves[36];
            ViewBag.DefaultLeaveType10 = leaves[39];
            ViewBag.DefaultLeaveType11 = leaves[42];

            ViewBag.MaxSickLeaves = leaves[1];
            ViewBag.MaxCasualLeaves = leaves[4];
            ViewBag.MaxAnnualLeaves = leaves[7];
            ViewBag.MaxOtherLeaves = leaves[10];
            ViewBag.MaxLeaveType01 = leaves[13];
            ViewBag.MaxLeaveType02 = leaves[16];
            ViewBag.MaxLeaveType03 = leaves[19];
            ViewBag.MaxLeaveType04 = leaves[22];
            ViewBag.MaxLeaveType05 = leaves[25];
            ViewBag.MaxLeaveType06 = leaves[28];
            ViewBag.MaxLeaveType07 = leaves[31];
            ViewBag.MaxLeaveType08 = leaves[34];
            ViewBag.MaxLeaveType09 = leaves[37];
            ViewBag.MaxLeaveType10 = leaves[40];
            ViewBag.MaxLeaveType11 = leaves[43];

            ViewBag.StatusSickLeaves = leaves[2];
            ViewBag.StatusCasualLeaves = leaves[5];
            ViewBag.StatusAnnualLeaves = leaves[8];
            ViewBag.StatusOtherLeaves = leaves[11];
            ViewBag.StatusLeaveType01 = leaves[14];
            ViewBag.StatusLeaveType02 = leaves[17];
            ViewBag.StatusLeaveType03 = leaves[20];
            ViewBag.StatusLeaveType04 = leaves[23];
            ViewBag.StatusLeaveType05 = leaves[26];
            ViewBag.StatusLeaveType06 = leaves[29];
            ViewBag.StatusLeaveType07 = leaves[32];
            ViewBag.StatusLeaveType08 = leaves[35];
            ViewBag.StatusLeaveType09 = leaves[38];
            ViewBag.StatusLeaveType10 = leaves[41];
            ViewBag.StatusLeaveType11 = leaves[44];

            return View();
        }

        [HttpPost]
        public ActionResult LeaveSession(LeaveSession toCreate)
        {
            //max and default leaves counter
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

            int id = TimeTune.LeaveSessionCrud.Add(toCreate);

            if (id == -1)
            {
                return RedirectToAction("LeaveSession", new { Message = "already" });

            }

            var json = JsonConvert.SerializeObject(toCreate);
            AuditTrail.insert(json, "LeaveSession", User.Identity.Name);

            return RedirectToAction("LeaveSession", new { Message = "success" });

            //return View();
        }

        [HttpPost]
        public JsonResult LeaveSessionDataHandler(DTParameters param)
        {
            try
            {
                ////max and default leaves counter
                //if (ConfigurationManager.AppSettings["MaxSickLeaves"] != null && ConfigurationManager.AppSettings["MaxSickLeaves"].ToString() != "")
                //{
                //    ViewBag.MaxSickLeaves = ConfigurationManager.AppSettings["MaxSickLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxSickLeaves = "10";
                //}

                //if (ConfigurationManager.AppSettings["MaxCasualLeaves"] != null && ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString() != "")
                //{
                //    ViewBag.MaxCasualLeaves = ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxCasualLeaves = "15";
                //}

                //if (ConfigurationManager.AppSettings["MaxAnnualLeaves"] != null && ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString() != "")
                //{
                //    ViewBag.MaxAnnualLeaves = ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxAnnualLeaves = "20";
                //}

                //if (ConfigurationManager.AppSettings["MaxOtherLeaves"] != null && ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString() != "")
                //{
                //    ViewBag.MaxOtherLeaves = ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxOtherLeaves = "99";
                //}

                //if (ConfigurationManager.AppSettings["MaxLeaveType01"] != null && ConfigurationManager.AppSettings["MaxLeaveType01"].ToString() != "")
                //{
                //    ViewBag.MaxLeaveType01 = ConfigurationManager.AppSettings["MaxLeaveType01"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxLeaveType01 = "10";
                //}

                //if (ConfigurationManager.AppSettings["MaxLeaveType02"] != null && ConfigurationManager.AppSettings["MaxLeaveType02"].ToString() != "")
                //{
                //    ViewBag.MaxLeaveType02 = ConfigurationManager.AppSettings["MaxLeaveType02"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxLeaveType02 = "10";
                //}

                //if (ConfigurationManager.AppSettings["MaxLeaveType03"] != null && ConfigurationManager.AppSettings["MaxLeaveType03"].ToString() != "")
                //{
                //    ViewBag.MaxLeaveType03 = ConfigurationManager.AppSettings["MaxLeaveType03"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxLeaveType03 = "10";
                //}

                //if (ConfigurationManager.AppSettings["MaxLeaveType04"] != null && ConfigurationManager.AppSettings["MaxLeaveType04"].ToString() != "")
                //{
                //    ViewBag.MaxLeaveType04 = ConfigurationManager.AppSettings["MaxLeaveType04"].ToString();
                //}
                //else
                //{
                //    ViewBag.MaxLeaveType04 = "10";
                //}



                ////Default
                //if (ConfigurationManager.AppSettings["DefaultSickLeaves"] != null && ConfigurationManager.AppSettings["DefaultSickLeaves"].ToString() != "")
                //{
                //    ViewBag.DefaultSickLeaves = ConfigurationManager.AppSettings["DefaultSickLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultSickLeaves = "8";
                //}

                //if (ConfigurationManager.AppSettings["DefaultCasualLeaves"] != null && ConfigurationManager.AppSettings["DefaultCasualLeaves"].ToString() != "")
                //{
                //    ViewBag.DefaultCasualLeaves = ConfigurationManager.AppSettings["DefaultCasualLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultCasualLeaves = "10";
                //}

                //if (ConfigurationManager.AppSettings["DefaultAnnualLeaves"] != null && ConfigurationManager.AppSettings["DefaultAnnualLeaves"].ToString() != "")
                //{
                //    ViewBag.DefaultAnnualLeaves = ConfigurationManager.AppSettings["DefaultAnnualLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultAnnualLeaves = "14";
                //}

                //if (ConfigurationManager.AppSettings["DefaultOtherLeaves"] != null && ConfigurationManager.AppSettings["DefaultOtherLeaves"].ToString() != "")
                //{
                //    ViewBag.DefaultOtherLeaves = ConfigurationManager.AppSettings["DefaultOtherLeaves"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultOtherLeaves = "20";
                //}

                //if (ConfigurationManager.AppSettings["DefaultLeaveType01"] != null && ConfigurationManager.AppSettings["DefaultLeaveType01"].ToString() != "")
                //{
                //    ViewBag.DefaultLeaveType01 = ConfigurationManager.AppSettings["DefaultLeaveType01"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultLeaveType01 = "10";
                //}

                //if (ConfigurationManager.AppSettings["DefaultLeaveType02"] != null && ConfigurationManager.AppSettings["DefaultLeaveType02"].ToString() != "")
                //{
                //    ViewBag.DefaultLeaveType02 = ConfigurationManager.AppSettings["DefaultLeaveType02"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultLeaveType02 = "10";
                //}

                //if (ConfigurationManager.AppSettings["DefaultLeaveType03"] != null && ConfigurationManager.AppSettings["DefaultLeaveType03"].ToString() != "")
                //{
                //    ViewBag.DefaultLeaveType03 = ConfigurationManager.AppSettings["DefaultLeaveType03"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultLeaveType03 = "10";
                //}

                //if (ConfigurationManager.AppSettings["DefaultLeaveType04"] != null && ConfigurationManager.AppSettings["DefaultLeaveType04"].ToString() != "")
                //{
                //    ViewBag.DefaultLeaveType04 = ConfigurationManager.AppSettings["DefaultLeaveType04"].ToString();
                //}
                //else
                //{
                //    ViewBag.DefaultLeaveType04 = "10";
                //}

                LeaveTypesCountStatus vm = new LeaveTypesCountStatus();

                int[] leaves = new int[45] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                leaves = TimeTune.LeaveSessionCrud.GetDefaultMaxLeavesCountStatus();
                ViewBag.DefaultSickLeaves = leaves[0];
                ViewBag.DefaultCasualLeaves = leaves[3];
                ViewBag.DefaultAnnualLeaves = leaves[6];
                ViewBag.DefaultOtherLeaves = leaves[9];
                ViewBag.DefaultLeaveType01 = leaves[12];
                ViewBag.DefaultLeaveType02 = leaves[15];
                ViewBag.DefaultLeaveType03 = leaves[18];
                ViewBag.DefaultLeaveType04 = leaves[21];
                ViewBag.DefaultLeaveType05 = leaves[24];
                ViewBag.DefaultLeaveType06 = leaves[27];
                ViewBag.DefaultLeaveType07 = leaves[30];
                ViewBag.DefaultLeaveType08 = leaves[33];
                ViewBag.DefaultLeaveType09 = leaves[36];
                ViewBag.DefaultLeaveType10 = leaves[39];
                ViewBag.DefaultLeaveType11 = leaves[42];

                ViewBag.MaxSickLeaves = leaves[1];
                ViewBag.MaxCasualLeaves = leaves[4];
                ViewBag.MaxAnnualLeaves = leaves[7];
                ViewBag.MaxOtherLeaves = leaves[10];
                ViewBag.MaxLeaveType01 = leaves[13];
                ViewBag.MaxLeaveType02 = leaves[16];
                ViewBag.MaxLeaveType03 = leaves[19];
                ViewBag.MaxLeaveType04 = leaves[22];
                ViewBag.MaxLeaveType05 = leaves[25];
                ViewBag.MaxLeaveType06 = leaves[28];
                ViewBag.MaxLeaveType07 = leaves[31];
                ViewBag.MaxLeaveType08 = leaves[34];
                ViewBag.MaxLeaveType09 = leaves[37];
                ViewBag.MaxLeaveType10 = leaves[40];
                ViewBag.MaxLeaveType11 = leaves[43];

                ViewBag.StatusSickLeaves = leaves[2];
                ViewBag.StatusCasualLeaves = leaves[5];
                ViewBag.StatusAnnualLeaves = leaves[8];
                ViewBag.StatusOtherLeaves = leaves[11];
                ViewBag.StatusLeaveType01 = leaves[14];
                ViewBag.StatusLeaveType02 = leaves[17];
                ViewBag.StatusLeaveType03 = leaves[20];
                ViewBag.StatusLeaveType04 = leaves[23];
                ViewBag.StatusLeaveType05 = leaves[26];
                ViewBag.StatusLeaveType06 = leaves[29];
                ViewBag.StatusLeaveType07 = leaves[32];
                ViewBag.StatusLeaveType08 = leaves[35];
                ViewBag.StatusLeaveType09 = leaves[38];
                ViewBag.StatusLeaveType10 = leaves[41];
                ViewBag.StatusLeaveType11 = leaves[44];


                ////////////////////////////////////////////////////////////////


                var dtsource = new List<ViewModels.LeaveSession>();
                // get all employee view models
                dtsource = TimeTune.LeaveSessionCrud.getAllData();
                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }

                List<ViewModels.LeaveSession> data = LeaveSessionResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);
                data = data.OrderByDescending(o => o.id).ToList();

                int count = LeaveSessionResultSet.Count(param.Search.Value, dtsource);

                DTResult<ViewModels.LeaveSession> result = new DTResult<ViewModels.LeaveSession>
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
        public ActionResult UpdateLeaveSession(ViewModels.LeaveSession toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            int response = TimeTune.LeaveSessionCrud.Update(toUpdate);

            if (response == -1)
            {
                return Json(new { status = "already" });
            }

            AuditTrail.update(json, "LeaveSession", User.Identity.Name);

            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLeaveSession(ViewModels.LeaveSession toRemove)
        {
            var entity = TimeTune.LeaveSessionCrud.Remove(toRemove);

            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "LeaveSession", User.Identity.Name);

            return Json(new { status = "success" });
        }

        #endregion

        #region LeaveReason
        public ActionResult LeaveReason()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LeaveReason(LeaveReason toCreate)
        {
            int id = TimeTune.LeaveReasonCrud.Add(toCreate);
            var json = JsonConvert.SerializeObject(toCreate);
            if (id == -1)
            {
                return RedirectToAction("LeaveReason", new { Message = "already" });

            }
            return RedirectToAction("LeaveReason", new { Message = "success" });

            //return View();
        }

        [HttpPost]
        public JsonResult LeaveReasonDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.LeaveReason>();
                // get all employee view models
                dtsource = TimeTune.LeaveReasonCrud.getAllData();
                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }

                List<ViewModels.LeaveReason> data = LeaveReasonResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = LeaveReasonResultSet.Count(param.Search.Value, dtsource);

                DTResult<ViewModels.LeaveReason> result = new DTResult<ViewModels.LeaveReason>
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
        public ActionResult UpdateLeaveReason(ViewModels.LeaveReason toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            int response = TimeTune.LeaveReasonCrud.Update(toUpdate);

            if (response == -1)
            {
                return Json(new { status = "already" });
            }

            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLeaveReason(ViewModels.LeaveReason toRemove)
        {
            var entity = TimeTune.LeaveReasonCrud.Remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            return Json(new { status = "success" });
        }

        #endregion

        #region Searchable-DropDowns

        //GetUsers
        [HttpPost]
        public JsonResult dropdown_users()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatching(q);

            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];

            if (employees.Length > 0)
            {
                for (int i = 0; i < employees.Length; i++)
                {
                    toSend[i] = new ChosenAutoCompleteResults();

                    toSend[i].id = employees[i].id.ToString();
                    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name;

                    //if (employees[i].access_group_id == 3)
                    //{
                    //    toSend[i].id = employees[i].id.ToString();
                    //    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name + "*";
                    //}
                    //else
                    //{
                    //    toSend[i].id = employees[i].id.ToString();
                    //    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name;
                    //}

                }


            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        #endregion

        #region EmployeeStatus
        public ActionResult EmployeeStatus(string result)
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
        public JsonResult DataHandlerEmployeeStatus(DTParameters param)
        {
            try
            {
                var dtsource = new List<Employee>();

                // get all employee view models
                int count = EmployeeCrud.Employees_Status(param.Search.Value, param.SortOrder, param.Start, param.Length, out dtsource);

                List<Employee> data = Employee_Status_ResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count2 = Employee_Status_ResultSet.Count(param.Search.Value, dtsource);


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


        #endregion

        #region EmployeeMissingData
        public ActionResult EmployeeMissingInfo(string result)
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
        public JsonResult DataHandlerEmployeeMissingInfo(DTParameters param)
        {
            try
            {
                var dtsource = new List<Employee>();

                // get all employee view models
                int count = EmployeeCrud.Employees_Missing_Info(param.Search.Value, param.SortOrder, param.Start, param.Length, out dtsource);

                List<Employee> data = Employee_Status_ResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count2 = Employee_Status_ResultSet.Count(param.Search.Value, dtsource);


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


        #endregion

        #region EmployeeProfile
        public ActionResult EmployeeProfile(string result)
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
        public JsonResult DataHandlerEmployeeProfile(DTParameters param)
        {
            try
            {
                bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
                int iGVCampusID = 0;
                if (!int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID))
                {
                    iGVCampusID = 0; // Default to 0 if parsing fails
                }

                var dtsource = new List<Employee>();

                // get all employee view models
                int count = EmployeeCrud.searchEmployees(bGVIsSuperHRRole, iGVCampusID, param.Search.Value, param.SortOrder, param.Start, param.Length, out dtsource);

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


        #endregion

        #region UNISUserAdd

        public ActionResult UNISUserAdd()
        {
            return View();
        }
        [HttpPost]
        public JsonResult UNISUserAddDataHandler(DTParameters param)
        {
            try
            {
                // DataTables server-side expects a consistent JSON shape:
                // { draw, data: [], recordsTotal, recordsFiltered }
                // Returning a string (e.g. "No Data to Show") breaks the client and can surface as
                // "Sequence contains no matching element".
                if (param == null)
                {
                    param = new DTParameters();
                }

                // get all employee view models
                var dtsource = TimeTune.Unis_User.getAllUsers() ?? new List<ViewModels.UnisUser>(); // IR replaced getAll() method

                // If there are no rows, still return a valid DTResult with empty data.
                if (dtsource.Count == 0)
                {
                    DTResult<ViewModels.UnisUser> emptyResult = new DTResult<ViewModels.UnisUser>
                    {
                        draw = param.Draw,
                        data = new List<ViewModels.UnisUser>(),
                        recordsFiltered = 0,
                        recordsTotal = 0
                    };
                    return Json(emptyResult);
                }

                string searchValue = param.Search != null ? param.Search.Value : null;
                IEnumerable<ViewModels.UnisUser> query = dtsource;

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    string q = searchValue.ToLower();
                    query = query.Where(x =>
                        (x.name != null && x.name.ToLower().Contains(q)) ||
                        (x.empCode != null && x.empCode.ToLower().Contains(q))
                    );
                }

                // Avoid dynamic SortBy path for this endpoint; keep deterministic default ordering.
                query = query.OrderBy(x => x.name);

                int count = query.Count();
                List<ViewModels.UnisUser> data = query
                    .Skip(param.Start)
                    .Take(param.Length)
                    .ToList();

                DTResult<ViewModels.UnisUser> result = new DTResult<ViewModels.UnisUser>
                {
                    draw = param.Draw,
                    data = data ?? new List<ViewModels.UnisUser>(),
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
        public ActionResult AddUNISUserAdd(List<UnisUserList> obj)
        {
            int _id = 0;
            if (obj != null && obj.Count > 0)
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    _id = TimeTune.Unis_User.add(obj[i].id, obj[i].name, obj[i].type);
                }
            }


            return RedirectToAction("UNISUserAdd");
        }


        #endregion

        #region ManageGeoPhencing

        public ActionResult ManageGeoPhencing(string result)
        {
            string value = result;
            ViewBag.UpMessage = result;

            List<TerminalView> branches = new List<TerminalView>();
            branches = ManageGeoPhencingEmployeeImportExport.GetBranchesList();

            return View(branches);
        }

        [HttpPost]
        public ActionResult ManageGeoPhencing(GeoPhencingTerminal toCreate)
        {
            int id = TimeTune.GeoPhencingCrud.Add(toCreate);

            if (id == -1)
            {
                return RedirectToAction("ManageGeoPhencing", new { Message = "already" });

            }

            var json = JsonConvert.SerializeObject(toCreate);
            AuditTrail.insert(json, "ManageGeoPhencing", User.Identity.Name);

            return RedirectToAction("ManageGeoPhencing", new { Message = "success" });

            //return View();
        }

        [HttpPost]
        public JsonResult GeoPhencingDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.GeoPhencingTerminal>();
                // get all employee view models
                dtsource = TimeTune.GeoPhencingCrud.getAllData();
                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }

                List<ViewModels.GeoPhencingTerminal> data = GeoPhencingResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);
                data = data.OrderByDescending(o => o.id).ToList();

                int count = GeoPhencingResultSet.Count(param.Search.Value, dtsource);

                DTResult<ViewModels.GeoPhencingTerminal> result = new DTResult<ViewModels.GeoPhencingTerminal>
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
        public ActionResult UpdateGeoPhencing(ViewModels.GeoPhencingTerminal toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            int response = TimeTune.GeoPhencingCrud.Update(toUpdate);

            if (response == -1)
            {
                return Json(new { status = "already" });
            }

            AuditTrail.update(json, "GeoPhencing", User.Identity.Name);

            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveGeoPhencing(ViewModels.GeoPhencingTerminal toRemove)
        {
            var entity = TimeTune.GeoPhencingCrud.Remove(toRemove);

            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "GeoPhencing", User.Identity.Name);

            return Json(new { status = "success" });
        }



        [HttpPost]
        public JsonResult GetUsersForGeoPhencing()
        {
            string q = Request.Form["data[q]"] ?? "";

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatching(q);

            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];

            if (employees.Length > 0)
            {
                for (int i = 0; i < employees.Length; i++)
                {
                    toSend[i] = new ChosenAutoCompleteResults();

                    toSend[i].id = employees[i].id.ToString();
                    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name;

                    //if (employees[i].access_group_id == 3)
                    //{
                    //    toSend[i].id = employees[i].id.ToString();
                    //    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name + "*";
                    //}
                    //else
                    //{
                    //    toSend[i].id = employees[i].id.ToString();
                    //    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name;
                    //}

                }


            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }



        [HttpPost]
        public JsonResult GetBranchesForGeoPhencing()
        {
            string q = Request.Form["data[q]"] ?? "";
            q = q.ToLower();

            // get all the employees that match the
            // pattern 'q'
            List<BLL.ViewModels.Terminals> terminals = TimeTune.GeoPhencingCrud.getAllBranches(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[terminals.Count];
            for (int i = 0; i < terminals.Count; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = terminals[i].branch_code;//actually branch id
                toSend[i].text = terminals[i].branch_name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        #endregion

        #region Geo-Phencing Upload Download
        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadGeoPhencingCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<TimeTune.ManageEmployeeImportExport.ManageEmployeeCSV>
                    (
                        TimeTune.ManageEmployeeImportExport.getManageEmployeeCSV(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-Users-List.csv"
                    );


            return toReturn;
        }


        // Handle file upload and read/write etc.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadGeoPhencingCSVFile(HttpPostedFileBase file)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = Path.GetFileName(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName).ToLower();

                if (FileExtension != ".csv")
                {
                    return RedirectToAction("ManageGeoPhencing", "EmployeeManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_geo_phencing.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidEcode = false, invalidBranchCode = false, invalidCols = false;

                    //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                    bool isDataRowFound = false; int a = 0;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            a++;

                            if (a == 2)
                            {
                                isDataRowFound = true;
                                break;
                            }
                        }
                    }

                    if (!isDataRowFound)
                    {
                        return RedirectToAction("ManageGeoPhencing", "EmployeeManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////

                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            strReadLine = strReadLine.TrimEnd(',');
                            strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                            strReadLine = strReadLine.Replace("\"", "");
                            strReadLine = strReadLine.TrimEnd(',');

                            //string new_code = "";
                            string[] ecode_dt = strReadLine.Split(',');
                            if (ecode_dt.Length == 2)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("employee_code") || ecode_dt[1].ToLower().Contains("branches_codes"))
                                {
                                    continue;
                                }


                                //validate employee code
                                if (!ValidateGPEmployeeCode(ecode_dt[0]))
                                {
                                    invalidEcode = true;
                                    result = "Invalid Employee-Code Found at Row-" + counter;
                                    break;
                                }

                                string[] arrBranches = ecode_dt[1].Split('-');
                                if (arrBranches.Length > 0)
                                {
                                    for (int z = 0; z < arrBranches.Length; z++)
                                    {
                                        if (!ValidateGPBranchCode(arrBranches[z]))
                                        {
                                            invalidBranchCode = true;
                                            result = "Invalid Branch-Code Found at Row-" + counter;
                                            break;
                                        }
                                    }
                                }


                                //int iGVCampusID = int.Parse(ViewModel.GlobalVariables.GV_EmployeeCampusID);
                                //if (!ValidateGPBranchCodeAllowed(iGVCampusID, ecode_dt[1]))
                                //{
                                //    invalidBranchCodeAllowed = true;
                                //    result = "NOT Allowed Branch-Code Found at Row-" + counter;
                                //    break;
                                //}


                            }
                            else
                            {
                                invalidCols = true;
                                result = "Invalid Col(s) Found";
                                break;
                            }

                            //iterate to replace EmployeeCode only having 0 as prefix
                            for (int i = 0; i < ecode_dt.Length; i++)
                            {
                                //if (i == 0)
                                {
                                    //    strReadLine = new_code + ",";
                                }
                                //else
                                {
                                    //strReadLine += ecode_dt[i] + ",";
                                }
                            }

                            strReadLine = strReadLine.TrimEnd(',');
                            content.Add(strReadLine);

                            //restrict to upload if 1000+ rows are found
                            /*if (counter > 1000)
                            {
                                invalidRowsCount = true;
                                result = "Max 1000 records are allowed be uploaded";
                                break;
                            }*/
                        }
                    }

                    if (invalidEcode || invalidBranchCode | invalidCols)
                    {
                        return RedirectToAction("ManageGeoPhencing", "EmployeeManagement", new { result = result });
                    }
                    else
                    {
                        result = TimeTune.ManageGeoPhencingEmployeeImportExport.setGeoPhencingEmployees(content, User.Identity.Name);
                    }
                }

                if (result == "failed")
                {
                    return RedirectToAction("ManageGeoPhencing", "EmployeeManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("ManageGeoPhencing", "EmployeeManagement", new { result = "Successfull" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("ManageGeoPhencing", "EmployeeManagement", new { result = "Select File first" });
        }

        private bool ValidateGPEmployeeCode(string strEmployeeCode)
        {
            bool isValid = true;

            isValid = ManageGeoPhencingEmployeeImportExport.validateEmployeeCode(strEmployeeCode);

            return isValid;
        }


        private bool ValidateGPBranchCode(string strBranchCode)
        {
            bool isValid = false;

            isValid = ManageGeoPhencingEmployeeImportExport.validateBranchCode(strBranchCode);

            return isValid;
        }

        #endregion


    }


}
