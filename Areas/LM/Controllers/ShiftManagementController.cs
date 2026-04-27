using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MvcApplication1.ViewModel;

namespace MvcApplication1.Areas.LM.Controllers
{
    [Authorize(Roles = (BLL.TimeTuneRoles.ROLE_LM))]
    public class ShiftManagementController : Controller
    {

        // Manage group calendar for Line Manager
        #region GroupCalendar

        public ActionResult ANewCalendar()
        {
            return View();
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult CalendarInfo()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getGeneralCalendarForYear(int year)
        {
            // need to instantiate a group calendar crud
            // because its constructor determines the current user
            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud();

            ViewModels.GroupCalendar viewModel = crud.getCalendarForYear(year);
            string toSend = (viewModel == null) ? "null" : System.Web.Helpers.Json.Encode(viewModel);

            return Json(new { data = toSend });
        }


        [HttpPost]
        public JsonResult getShiftsForGeneralCalendar()
        {

            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud();
            List<ViewModels.Shift> shifts = crud.getAllShifts();
            string toSend = System.Web.Helpers.Json.Encode(shifts);
            return Json(new { data = toSend });
        }

        [HttpPost]
        public JsonResult addOrUpdateCalendar(string calendarData)
        {

            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud();

            dynamic data = System.Web.Helpers.Json.Decode(calendarData);

            bool returnVal = crud.addOrUpdateCalendar(data);

            //ViewModels.GeneralCalendar viewModel = TimeTune.GeneralCalendarCrud.getCalendarForYear(10);
            //string toSend = (viewModel == null) ? "null" : System.Web.Helpers.Json.Encode(viewModel);

            if (returnVal)
                return Json(new { success = true });
            else
                return Json(new { success = false });
        }

        #endregion


        #region Shifts-Upload

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
                    return RedirectToAction("ManageEmployeeCalendar", "ShiftManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_calendar.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidShift = false, invalidShiftAllowed = false, invalidEcode = false, invalidStartDate = false, invalidEndDate = false, invalidCols = false, invalidRowsCount = false;

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
                        return RedirectToAction("ManageEmployeeCalendar", "ShiftManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////
                    TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();
                    List<ViewModels.Employee> listEmployees = cal.getAllGroupEmployees();

                    TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud();
                    List<ViewModels.Shift> listShifts = crud.getEmployeeShifts();

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
                            if (ecode_dt.Length == 4)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("employee_code") || ecode_dt[1].ToLower().Contains("date_start") || ecode_dt[2].ToLower().Contains("date_end") || ecode_dt[3].ToLower().Contains("shift_name"))
                                {
                                    continue;
                                }

                                //normal employee
                                string strEmpCode = ecode_dt[0];
                                string strShiftName = ecode_dt[3].ToLower();
                                //if (ecode_dt[0].Length == 6)
                                //{
                                //    new_code = ecode_dt[0];
                                //}
                                //else
                                //{
                                //    if (ecode_dt[0].Length == 1)
                                //        new_code = "00000" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 2)
                                //        new_code = "0000" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 3)
                                //        new_code = "000" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 4)
                                //        new_code = "00" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 5)
                                //        new_code = "0" + ecode_dt[0];
                                //    else
                                //        new_code = "";
                                //}

                                //employees not allowed
                                if (listEmployees != null && listEmployees.Count > 0)
                                {
                                    bool isEfound = false;
                                    foreach (var ee in listEmployees)
                                    {
                                        if (ee.employee_code == strEmpCode)
                                        {
                                            isEfound = true;
                                            break;
                                        }
                                    }

                                    if (!isEfound)
                                    {
                                        ViewBag.Message = "Out of Team User Code in a Data Row: " + counter.ToString();

                                        invalidEcode = true;
                                        result = "Out of Team User Code Found at Row-" + counter;
                                        break;
                                    }
                                }

                                if (!ValidateEmployeeCode(strEmpCode))
                                {
                                    ViewBag.Message = "Invalid User Code in a Data Row: " + counter.ToString();

                                    invalidEcode = true;
                                    result = "Invalid User Code Found at Row-" + counter;
                                    break;
                                }

                                //validate Date of Joining
                                if (ecode_dt[1] != null && ecode_dt[1].ToString() != "")
                                {
                                    if (!ValidateDate(ecode_dt[1]))
                                    {
                                        ViewBag.Message = "Invalid Start Date in a Data Row: " + counter.ToString();

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
                                        ViewBag.Message = "Invalid End Date in a Data Row: " + counter.ToString();

                                        invalidEndDate = true;
                                        result = "Invalid End Date Found at Row-" + counter;
                                        break;
                                    }
                                }

                                //validate employee code
                                if (!ValidateShiftName(ecode_dt[3]))
                                {
                                    ViewBag.Message = "Invalid Shift Name in a Data Row: " + counter.ToString();

                                    invalidShift = true;
                                    result = "Invalid Shift-Code Found at Row-" + counter;
                                    break;
                                }

                                //shift not allowed

                                if (listShifts != null && listShifts.Count > 0)
                                {
                                    bool isSfound = false;
                                    foreach (var ss in listShifts)
                                    {
                                        if (ss.name.ToLower() == strShiftName)
                                        {
                                            isSfound = true;
                                            break;
                                        }
                                    }

                                    if (!isSfound)
                                    {
                                        ViewBag.Message = "Out of Scope shift used in a Data Row: " + counter.ToString();

                                        invalidEcode = true;
                                        result = "Out of Scope shift used at Row-" + counter;
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
                            //for (int i = 0; i < ecode_dt.Length; i++)
                            {
                                strReadLine = ecode_dt[0] + "," + ecode_dt[1] + "," + ecode_dt[2] + "," + ecode_dt[3] + ",";

                                //if (i == 0)
                                //{
                                //    strReadLine = new_code + ",";
                                //}
                                //else
                                //{
                                //    strReadLine += ecode_dt[i] + ",";
                                //}
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

                    if (invalidEcode || invalidStartDate || invalidEndDate || invalidShift || invalidShiftAllowed || invalidCols)
                    {
                        return RedirectToAction("ManageEmployeeCalendar", "ShiftManagement", new { result = result });
                    }
                    else
                    {
                        //result = TimeTune.ManageEmployeeImportExport.setEmployeesCalendar00(content, User.Identity.Name);

                        //create shifts list
                        foreach (string line in content)
                        {
                            // split the line
                            string[] values = line.Split(',');

                            // check if the split resulted in at least 39 values (columns).

                            if (values == null || values.Length < 4)
                                continue;

                            // Remove unwanted " characters.
                            int emp_id = 0;
                            string emp_code = values[0].ToString();
                            using (var db = new DLL.Models.Context())
                            {
                                var dbEmployee = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                                if (dbEmployee == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    emp_id = dbEmployee.EmployeeId;
                                }
                            }

                            DateTime st_date = DateTime.ParseExact(values[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            DateTime en_date = DateTime.ParseExact(values[2], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            if (st_date > en_date)
                            {
                                continue;
                            }

                            int shift_id = 0;
                            string sh_name = values[3].Replace("\"", "");
                            using (var db = new DLL.Models.Context())
                            {
                                var dbShift = db.shift.Where(e => e.active && e.name == sh_name).FirstOrDefault();
                                if (dbShift == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    shift_id = dbShift.ShiftId;
                                }
                            }

                            ViewModels.EmployeeCalendar calendarData = new ViewModels.EmployeeCalendar();
                            calendarData.employee_id = emp_id.ToString();

                            int days_diff = 0;
                            days_diff = (en_date - st_date).Days + 1;

                            DateTime dtCurrent = st_date;
                            List<GeneralCalendarOverride> listOverrride = new List<GeneralCalendarOverride>();
                            for (int i = 0; i < days_diff; i++)
                            {
                                listOverrride.Add(new GeneralCalendarOverride()
                                {
                                    date = dtCurrent.ToString("MM/dd/yyyy"),
                                    reason = ".",
                                    type = "shift",
                                    shift = shift_id.ToString()
                                });

                                dtCurrent = dtCurrent.AddDays(1);
                            }
                            calendarData.overrides = listOverrride;

                            //TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();

                            cal.addOrUpdateCalendar(calendarData);

                            //return Json(new { success = true });
                        }

                        ViewBag.Message = "Success";

                        //public string employee_id { get; set; }
                        //public List<ViewModels.GeneralCalendarOverride> overrides { get; set; }


                    }
                }

                if (result == "failed")
                {
                    return RedirectToAction("ManageEmployeeCalendar", "ShiftManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("ManageEmployeeCalendar", "ShiftManagement", new { result = "Successfull" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("ManageEmployeeCalendar", "ShiftManagement", new { result = "Select File first" });
        }

        private bool ValidateShiftName(string strShiftName)
        {
            bool isValid = false;

            isValid = ManageEmployeeImportExport.validateShiftName(strShiftName);

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

            string strProperDate = DateTime.ParseExact(strDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (!DateTime.TryParse(strProperDate, out dtTest))
            {
                isValid = false;
            }

            return isValid;
        }


        #endregion


        ///SuperAdmin/ManageEmployees/addOrUpdateEmpCalendar

        #region EmployeeCalendar

        public ActionResult ManageEmployeeCalendarInfo()
        {
            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();

            var employees = cal.getAllGroupEmployees();

            return View(employees);
        }

        public ActionResult ManageEmployeeCalendar()
        {
            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();

            var employees = cal.getAllGroupEmployees();

            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud();
            List<ViewModels.Shift> shifts = crud.getEmployeeShifts();

            employees_shifts emp_shf = new employees_shifts();
            emp_shf.list_employee = employees;
            emp_shf.list_shift = shifts;

            return View(emp_shf);
        }
        [HttpPost]
        public JsonResult getShiftsForEmployeeCalendar()
        {
            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud();
            List<ViewModels.Shift> shifts = crud.getEmployeeShifts();
            string toSend = System.Web.Helpers.Json.Encode(shifts);
            return Json(new { data = toSend });
        }
        [HttpPost]
        public JsonResult getEmployeeCalendar()
        {
            string employee_id = Request.Form["employee_id"];
            string year = Request.Form["year"];
            /*
            dynamic obj = new
            {
                success = true,
                data = new[] {
                    
                    new {date = "03/09/2016" , reason = "There has...", shift="-1",type="holiday"}, 
                    new {date = "03/10/2016" , reason = "", shift="3",type="shift"},
                    new {date = "03/11/2017" , reason = "", shift="3",type="shift"},
                    
                }
            };*/

            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();
            return Json(cal.getEmployeeCalendar(employee_id, year));

        }


        [HttpPost]
        public JsonResult addOrUpdateEmpCalendar(ViewModels.EmployeeCalendar calendarData)
        {

            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();

            cal.addOrUpdateCalendar(calendarData);

            return Json(new { success = true });




            //return Json(new { });



            /*TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud();


            dynamic data = System.Web.Helpers.Json.Decode(calendarData);


            bool returnVal = crud.addOrUpdateCalendar(data);


            //ViewModels.GeneralCalendar viewModel = TimeTune.GeneralCalendarCrud.getCalendarForYear(10);
            //string toSend = (viewModel == null) ? "null" : System.Web.Helpers.Json.Encode(viewModel);

            if (returnVal)
                return Json(new { success = true });
            else
                return Json(new { success = false });*/
        }

        public class rep_emp_data
        {
            public int employee_id { get; set; }
            public int month_id { get; set; }
        }

        [HttpPost]
        public JsonResult replicateEmployeeCalendar(rep_emp_data rep_data)
        {
            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();

            //rep to current month
            if (rep_data.month_id == 1)
            {
                #region current-month

                DateTime month_prev = DateTime.Now.AddMonths(-1);
                DateTime month_curr = DateTime.Now;

                DateTime st_date = new DateTime(month_curr.Year, month_curr.Month, 1);
                DateTime en_date = new DateTime(month_curr.Year, month_curr.Month, DateTime.DaysInMonth(month_curr.Year, month_curr.Month));

                //for ALL Employees
                if (rep_data.employee_id == -1)
                {
                    #region cur-mnt-all-employees

                    var employees = cal.getAllGroupEmployees();
                    if (employees != null && employees.Count > 0)
                    {
                        int daysInPreviousMonth = DateTime.DaysInMonth(month_prev.Year, month_prev.Month);
                        int daysInCurrentMonth = DateTime.DaysInMonth(en_date.Year, en_date.Month);

                        foreach (var e in employees)
                        {
                            int days_diff = 0;
                            if (daysInPreviousMonth < daysInCurrentMonth)
                                days_diff = daysInPreviousMonth;
                            else
                                days_diff = daysInCurrentMonth;

                            //days_diff = (en_date - st_date).Days + 1;

                            DateTime dtCurrent = st_date;
                            List<GeneralCalendarOverride> listOverrride = new List<GeneralCalendarOverride>();
                            using (var db = new DLL.Models.Context())
                            {
                                for (int i = 0; i < days_diff; i++)
                                {
                                    ViewModels.EmployeeCalendar calendarData = new ViewModels.EmployeeCalendar();

                                    DateTime dtSamePrevMonDate = dtCurrent.AddMonths(-1);

                                    var dbManualGSAssigned = db.manual_group_shift_assigned.Where(m => m.Employee.EmployeeId == e.id && m.date == dtSamePrevMonDate).FirstOrDefault();
                                    if (dbManualGSAssigned == null)
                                    {
                                        dtCurrent = dtCurrent.AddDays(1);

                                        continue;
                                    }
                                    else
                                    {
                                        DateTime dtSameCurrMonDate = dtCurrent.AddMonths(1);

                                        listOverrride.Add(new GeneralCalendarOverride()
                                        {
                                            date = dtSameCurrMonDate.ToString("MM/dd/yyyy"),
                                            reason = dbManualGSAssigned.reason,
                                            type = "shift",
                                            shift = dbManualGSAssigned.Shift.ShiftId.ToString()
                                        });
                                    }

                                    calendarData.employee_id = e.id.ToString();
                                    calendarData.overrides = listOverrride;
                                    ////cal.addOrUpdateCalendar(calendarData);

                                    dtCurrent = dtCurrent.AddDays(1);
                                }
                            }
                        }
                    }

                    #endregion
                }
                else//for selected employee
                {
                    #region cur-mnt-sel-employees

                    Employee selected_employee = new Employee();

                    var employees = cal.getAllGroupEmployees();
                    if (employees != null && employees.Count > 0)
                    {
                        foreach (var e in employees)
                        {
                            if (e.id == rep_data.employee_id)
                            {
                                selected_employee = e;

                                break;
                            }
                        }
                    }

                    if (selected_employee != null)
                    {
                        int daysInPreviousMonth = DateTime.DaysInMonth(month_prev.Year, month_prev.Month);
                        int daysInCurrentMonth = DateTime.DaysInMonth(en_date.Year, en_date.Month);

                        Employee e = new Employee();
                        e = selected_employee;

                        int days_diff = 0;
                        if (daysInPreviousMonth < daysInCurrentMonth)
                            days_diff = daysInPreviousMonth;
                        else
                            days_diff = daysInCurrentMonth;

                        //days_diff = (en_date - st_date).Days + 1;

                        DateTime dtCurrent = st_date;
                        List<GeneralCalendarOverride> listOverrride = new List<GeneralCalendarOverride>();
                        using (var db = new DLL.Models.Context())
                        {
                            for (int i = 0; i < days_diff; i++)
                            {
                                ViewModels.EmployeeCalendar calendarData = new ViewModels.EmployeeCalendar();

                                DateTime dtSamePrevMonDate = dtCurrent.AddMonths(-1);

                                var dbManualGSAssigned = db.manual_group_shift_assigned.Where(m => m.Employee.EmployeeId == e.id && m.date == dtSamePrevMonDate).FirstOrDefault();
                                if (dbManualGSAssigned == null)
                                {
                                    dtCurrent = dtCurrent.AddDays(1);

                                    continue;
                                }
                                else
                                {
                                    DateTime dtSameCurrMonDate = dtCurrent.AddMonths(1);

                                    listOverrride.Add(new GeneralCalendarOverride()
                                    {
                                        date = dtSameCurrMonDate.ToString("MM/dd/yyyy"),
                                        reason = dbManualGSAssigned.reason,
                                        type = "shift",
                                        shift = dbManualGSAssigned.Shift.ShiftId.ToString()
                                    });
                                }

                                calendarData.employee_id = e.id.ToString();
                                calendarData.overrides = listOverrride;
                                ////cal.addOrUpdateCalendar(calendarData);

                                dtCurrent = dtCurrent.AddDays(1);
                            }
                        }
                    }

                    #endregion
                }

                #endregion
            }
            //rep to next month
            else
            {
                #region next-month

                DateTime month_crnt = DateTime.Now;
                DateTime month_next = DateTime.Now.AddMonths(1);

                DateTime st_date = new DateTime(month_crnt.Year, month_crnt.Month, 1);
                DateTime en_date = new DateTime(month_next.Year, month_next.Month, DateTime.DaysInMonth(month_next.Year, month_next.Month));

                //for ALL Employees
                if (rep_data.employee_id == -1)
                {
                    #region nxt-month-al-emp

                    var employees = cal.getAllGroupEmployees();
                    if (employees != null && employees.Count > 0)
                    {
                        int daysInCrntMonth = DateTime.DaysInMonth(month_crnt.Year, month_crnt.Month);
                        int daysInNextMonth = DateTime.DaysInMonth(en_date.Year, en_date.Month);

                        foreach (var e in employees)
                        {
                            int days_diff = 0;
                            if (daysInCrntMonth < daysInNextMonth)
                                days_diff = daysInCrntMonth;
                            else
                                days_diff = daysInNextMonth;

                            //days_diff = (en_date - st_date).Days + 1;

                            DateTime dtCurrent = st_date;
                            List<GeneralCalendarOverride> listOverrride = new List<GeneralCalendarOverride>();
                            using (var db = new DLL.Models.Context())
                            {
                                for (int i = 0; i < days_diff; i++)
                                {
                                    ViewModels.EmployeeCalendar calendarData = new ViewModels.EmployeeCalendar();

                                    var dbManualGSAssigned = db.manual_group_shift_assigned.Where(m => m.Employee.EmployeeId == e.id && m.date == dtCurrent).FirstOrDefault();
                                    if (dbManualGSAssigned == null)
                                    {
                                        dtCurrent = dtCurrent.AddDays(1);

                                        continue;
                                    }
                                    else
                                    {
                                        DateTime dtSameNextMonDate = dtCurrent.AddMonths(1);

                                        listOverrride.Add(new GeneralCalendarOverride()
                                        {
                                            date = dtSameNextMonDate.ToString("MM/dd/yyyy"),
                                            reason = dbManualGSAssigned.reason,
                                            type = "shift",
                                            shift = dbManualGSAssigned.Shift.ShiftId.ToString()
                                        });
                                    }

                                    calendarData.employee_id = e.id.ToString();
                                    calendarData.overrides = listOverrride;
                                    cal.addOrUpdateCalendar(calendarData);

                                    dtCurrent = dtCurrent.AddDays(1);
                                }
                            }
                        }
                    }

                    #endregion
                }
                else//for selected employee
                {
                    #region nxt-mnt-sel-employees

                    Employee selected_employee = new Employee();

                    var employees = cal.getAllGroupEmployees();
                    if (employees != null && employees.Count > 0)
                    {
                        foreach (var e in employees)
                        {
                            if (e.id == rep_data.employee_id)
                            {
                                selected_employee = e;

                                break;
                            }
                        }
                    }

                    if (selected_employee != null)
                    {
                        int daysInCrntMonth = DateTime.DaysInMonth(month_crnt.Year, month_crnt.Month);
                        int daysInNextMonth = DateTime.DaysInMonth(en_date.Year, en_date.Month);

                        Employee e = new Employee();
                        e = selected_employee;

                        int days_diff = 0;
                        if (daysInCrntMonth < daysInNextMonth)
                            days_diff = daysInCrntMonth;
                        else
                            days_diff = daysInNextMonth;

                        //days_diff = (en_date - st_date).Days + 1;

                        DateTime dtCurrent = st_date;
                        List<GeneralCalendarOverride> listOverrride = new List<GeneralCalendarOverride>();
                        using (var db = new DLL.Models.Context())
                        {
                            for (int i = 0; i < days_diff; i++)
                            {
                                ViewModels.EmployeeCalendar calendarData = new ViewModels.EmployeeCalendar();

                                var dbManualGSAssigned = db.manual_group_shift_assigned.Where(m => m.Employee.EmployeeId == e.id && m.date == dtCurrent).FirstOrDefault();
                                if (dbManualGSAssigned == null)
                                {
                                    dtCurrent = dtCurrent.AddDays(1);

                                    continue;
                                }
                                else
                                {
                                    DateTime dtSameNextMonDate = dtCurrent.AddMonths(1);

                                    listOverrride.Add(new GeneralCalendarOverride()
                                    {
                                        date = dtSameNextMonDate.ToString("MM/dd/yyyy"),
                                        reason = dbManualGSAssigned.reason,
                                        type = "shift",
                                        shift = dbManualGSAssigned.Shift.ShiftId.ToString()
                                    });
                                }

                                calendarData.employee_id = e.id.ToString();
                                calendarData.overrides = listOverrride;
                                cal.addOrUpdateCalendar(calendarData);

                                dtCurrent = dtCurrent.AddDays(1);
                            }
                        }
                    }

                    #endregion
                }

                #endregion
            }

            return Json(new { success = true });
        }

        public class rep_emp_data_emp_to_emp
        {
            public int employee_idfrom { get; set; }
            public int employee_idto { get; set; }
            public int month_id { get; set; }
        }
        public JsonResult replicate_Employee_Calendar_EmpToEmp(rep_emp_data_emp_to_emp rep_data_emp_to_emp)
        {
            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();
            ViewModels.EmployeeCalendar calendarDataa=new ViewModels.EmployeeCalendar();
            TimeTune.EmployeeCalendar call = new TimeTune.EmployeeCalendar();
            DateTime month_prev;
            DateTime month_curr;
            DateTime st_date;
            DateTime en_date;

            using (var db = new DLL.Models.Context())
            {

                //rep to current month
                if (rep_data_emp_to_emp.month_id == 1)
                {
                    #region current-month
                    month_prev = DateTime.Now.AddMonths(-1);
                    month_curr = DateTime.Now;
                    st_date = new DateTime(month_curr.Year, month_curr.Month, 1);
                    en_date = new DateTime(month_curr.Year, month_curr.Month, DateTime.DaysInMonth(month_curr.Year, month_curr.Month));
                    call.replication_emptoemp(rep_data_emp_to_emp.employee_idfrom, rep_data_emp_to_emp.employee_idto, st_date, en_date);
                    #endregion
                }
                //rep to next month
                else
                {
                    #region next-month
                    month_prev = DateTime.Now.AddMonths(-1);
                    month_curr = DateTime.Now.AddMonths(1);
                    st_date = new DateTime(month_curr.Year, month_curr.Month, 1);
                    en_date = new DateTime(month_curr.Year, month_curr.Month, DateTime.DaysInMonth(month_curr.Year, month_curr.Month));
                    call.replication_emptoemp(rep_data_emp_to_emp.employee_idfrom, rep_data_emp_to_emp.employee_idto, st_date, en_date);



                    #endregion
                }

                return Json(new { success = true });
            }
        }
        #endregion

        #region shifts-pdf-download

        [HttpPost]
        [ActionName("ManageEmployeeCalendar")]
        //[ValidateAntiForgeryToken]
        public ActionResult ManageEmployeeCalendar_Post()
        {
            int employeeID = 0;
            if (!int.TryParse(Request.Form["rpt_employee_id"], out employeeID))
                return RedirectToAction("ManageEmployeeCalendar");

            string from_date = Request.Form["rpt_date_from"];
            string to_date = Request.Form["rpt_date_to"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            List<BLL.PdfReports.MonthlyEmpShiftData> toRender = reportMaker.getReportMonthlyEmpShiftData(employeeID, from_date, to_date);

            if (toRender == null)
                return RedirectToAction("ManageEmployeeCalendar");

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

        private int GenerateMonthlyEmployeeShiftsPDF(List<BLL.PdfReports.MonthlyEmpShiftData> sdata)
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
                    //PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above
                    //string path = Server.MapPath("Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Font.pdf", FileMode.Create));

                    document.Open();

                    foreach (var emp in sdata)
                    {
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

                        PdfPCell cellEName = new PdfPCell(new Phrase("Name: " + emp.employeeName, fBold9));
                        cellEName.Border = 0;
                        tableEInfo.AddCell(cellEName);

                        PdfPCell cellECode = new PdfPCell(new Phrase("Employee Code: " + emp.employeeCode, fBold9));
                        cellECode.Border = 0;
                        tableEInfo.AddCell(cellECode);

                        PdfPCell cellEMonth = new PdfPCell(new Phrase("From Date: " + emp.month, fBold9));
                        cellEMonth.Border = 0;
                        tableEInfo.AddCell(cellEMonth);

                        PdfPCell cellEYear = new PdfPCell(new Phrase("To Date: " + emp.year, fBold9));
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

                        foreach (BLL.PdfReports.MonthlyEmpShiftLog log in emp.logs)
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

                        if (emp.logs.Length > 0)
                        {
                            document.Add(tableMid);

                            Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                            p_nsig.SpacingBefore = 1;
                            //p_nsig.SpacingAfter = 3;
                            document.Add(p_nsig);                         
                        }
                        else
                        {
                            Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                            p_no_data.SpacingBefore = 20;
                            p_no_data.SpacingAfter = 20;
                            document.Add(p_no_data);

                            reponse = 0;
                        }

                        document.NewPage();
                        //counter++;

                    }//emp-if

                    // ------------- close PDF Document and download it automatically

                    document.Close();
                    writer.Close();
                    Response.ContentType = "pdf/application";
                    Response.AddHeader("content-disposition", "attachment;filename=Report-ALL-Employees-Shifts.pdf");
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

        #endregion


    }
}
