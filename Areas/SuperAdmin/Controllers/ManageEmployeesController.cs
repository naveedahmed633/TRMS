using BLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using BLL;
using MVCDatatableApp.Models;
using System.IO;
using Newtonsoft.Json;
using TimeTune;

namespace MvcApplication1.Areas.SuperAdmin.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SUPER_USER)]
    public class ManageEmployeesController : Controller
    {
        //
        // GET: /SuperAdmin/ManageEmployees/

      

        #region Terminals
        public ActionResult AddNewTerminal(string result, string result2)
        {

            ViewBag.Message = result;
            ViewBag.Message2 = result2;
            return View();
        }
        [HttpPost]
        public ActionResult AddNewTerminal(int id, string terminal_name, string terminal_id, string branch_code, string branch_name, string type)
        {
            string message = "";
            message = BLL.ManageHR.addTerminal(id, terminal_name, terminal_id, branch_code, branch_name, type);

            ViewBag.Message = message;

            var json = JsonConvert.SerializeObject(new { id = id.ToString(), terminal_name = terminal_name, terminal_id = terminal_id, branch_code = branch_code, branch_name = branch_name });
            AuditTrail.insert(json, "Terminals", User.Identity.Name);

            return RedirectToAction("AddNewTerminal", new { result = message, result2 = "" });
        }

        public ActionResult EditTerminal()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditTerminal(int id, string terminal_id, string terminal_name, string branch_code, string branch_name, string t_type)
        {
            string message = "";
            message = BLL.ManageHR.updateTerminal(id, terminal_name, terminal_id, branch_code, branch_name, t_type);

            var json = JsonConvert.SerializeObject(new { id = id.ToString(), terminal_name = terminal_name, terminal_id = terminal_id, branch_code = branch_code, branch_name = branch_name });
            AuditTrail.update(json, "Terminals", User.Identity.Name);

            return RedirectToAction("AddNewTerminal", new { message = message });
        }
        [HttpPost]
        public ActionResult deleteTerminal(int id)
        {
            if (id == 0)
            {
                return View("EditTerminal");
            }
            string message = BLL.ManageHR.deleteTerminal(id);

            var json = JsonConvert.SerializeObject(new { id = id.ToString() });
            AuditTrail.delete(json, "Terminals", User.Identity.Name);

            return View("EditTerminal");
        }
        [HttpPost]
        public JsonResult TerminalDataHandler(DTParameters param)
        {
            try
            {

                var data = new List<BLL.ViewModels.Terminals>();

                // get all employee view models
                int count = BLL.ManageHR.getAllTerminal(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


                DTResult<BLL.ViewModels.Terminals> result = new DTResult<BLL.ViewModels.Terminals>
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
                return Json(new { error = ex.Message + " The session has been timed out please try again" });

            }
        }
        public JsonResult EmployeeDataHandler(DTParameters param)
        {
            try
            {

                var data = new List<BLL.ViewModels.VM_ContractualStaff>();

                // get all employee view models
                int count = BLL.ManageHR.getAllContratualStaff(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


                DTResult<BLL.ViewModels.VM_ContractualStaff> result = new DTResult<BLL.ViewModels.VM_ContractualStaff>
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
        //EmployeeDataHandler
        [HttpPost]
        public ActionResult terminal()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'

            BLL.ViewModels.Terminals[] terminals = TimeTune.EmployeeManagementHelper.getllTerminalMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[terminals.Length];
            for (int i = 0; i < terminals.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = terminals[i].id.ToString();
                toSend[i].text = terminals[i].terminal_id;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadTerminalFile(HttpPostedFileBase file)
        {
            string message = null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                string path = Path.Combine(Server.MapPath("~/Uploads"),
                                   "Terminals.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                message = BLL.ManageHR.uploadTerminal(content);
                return RedirectToAction("AddNewTerminal", "ManageEmployees", new { result = "", result2 = message });


            }
            return RedirectToAction("AddNewTerminal", "ManageEmployees", new { result = "", result2 = "Select File first" });
        }
        #endregion


        #region ContractulaStaff

        public ActionResult AddContractualStaff(string result, string result2)
        {
            //added by IR
            return RedirectToAction("ConfigurationManager", "SU");

            //commented by IR
            ////string value = result;
            ////ViewBag.Message = result;
            ////ViewBag.Message1 = result2;
            ////return View();
        }
        [HttpPost]
        public ActionResult AddContractualStaff(VM_ContractualStaff emp)
        {

            string message = BLL.ManageHR.addEmployee(emp);

            return RedirectToAction("AddContractualStaff", new { result = "", result2 = message });
        }
        public ActionResult EditContractualStaff()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditContractualStaff(VM_ContractualStaff emp)
        {
            //added by IR
            return RedirectToAction("ConfigurationManager", "SU");

            //commented by IR
            //implement edit logic here
            ////string message = BLL.ManageHR.editEmployee(emp);
            ////return RedirectToAction("EditContractualStaff", new { result = "",result2=message });
        }
        public class EmployeeParameter
        {
            public string id { get; set; }
        }
        [HttpPost]
        public JsonResult getEmployee(EmployeeParameter param)
        {

            string q = param.id;

            // get all the employees that match the 
            // pattern 'q'
            BLL.ViewModels.VM_ContractualStaff employees = BLL.ManageHR.getContractualStaff(q);

            var toReturn = Json(new
            {
                q = q,
                results = employees
            });

            return toReturn;
        }
        #endregion


        #region Future Manual Attendance
        public ActionResult FutureManualAttendance(string result)
        {
            ViewBag.Message = result;
            return View();
        }
        [HttpPost]
        public ActionResult FutureManualAttendance(string employee_code, string time_in_from, string time_in_to, string remarks)
        {
            // string message = TimeTune.ManualAttendance.addManualAttendance(manualAttendance);
            //var json = JsonConvert.SerializeObject(message);
            //AuditTrail.update(json, "Manual Attendance", int.Parse(User.Identity.Name));
            string message = BLL.ManageHR.addManualAttendance(time_in_from, time_in_to, employee_code, remarks);

            return RedirectToAction("FutureManualAttendance", new { result = message });
        }


        [HttpPost]
        public JsonResult FutureManualAttendanceDataHandler(DTParameters param)
        {
            try
            {

                var data = new List<BLL.ViewModels.ViewFutureManualAttendance>();

                // get all employee view models
                int count = BLL.ManageHR.getAllManualAttendance(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


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
                return Json(new { error = ex.Message + " The session has been timed out please try again" });

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


        #region -Manage-HRs

        public ActionResult ManageHR()
        {
            // fetch the list of all the HRS, and send to the view,
            // as a list of employees.

            List<Employee> emp = new List<Employee>(BLL.ManageHR.getAllHRsExceptSuperHR());


            ViewBag.CampusesList = BLL.ManageHR.getAllCampusesForHRAssignment();


            return View(emp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHR()
        {
            string val = Request.Form["emp-id"];
            string valCampus = Request.Form["campus-id"];
            string valIsSuperHR = Request.Form["is-super-hr"];

            // remove employeeId val from HR.
            BLL.ManageHR.addHr(val, int.Parse(valCampus), bool.Parse(valIsSuperHR));

            return RedirectToAction("ManageHR");
        }

        //getAllNonHrs
        [HttpPost]
        public JsonResult getAllNonHrsDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = BLL.ManageHR.getAllNonHrsEmployeesMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].id.ToString();
                toSend[i].text =
                    employees[i].employee_code + " - " + employees[i].first_name + " " + employees[i].last_name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHR()
        {
            string val = Request.Form["emp-id"];

            // remove employeeId val from HR.
            BLL.ManageHR.deleteHr(val);

            return RedirectToAction("ManageHR");
        }

        #endregion


        #region Manual Upload And Download Employee 

        #region Upload Download Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<TimeTune.ManageEmployeeImportExport.ManageContractualStaffCSV>
                    (
                        TimeTune.ManageEmployeeImportExport.getContractualStaff(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-ContractualStaff.csv"
                    );


            return toReturn;
        }


        // Handle file upload and read/write etc.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string message = null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                string path = Path.Combine(Server.MapPath("~/Uploads"),
                                   "ContractualStaff.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                message = TimeTune.ManageEmployeeImportExport.setContractualStaff(content);
                return RedirectToAction("AddContractualStaff", "ManageEmployees", new { result = message, result2 = "" });


            }
            return RedirectToAction("AddContractualStaff", "ManageEmployees", new { result = "Select File first", result2 = "" });
        }

        #endregion

        #endregion


        #region service
        public ActionResult ServiceOnOff()
        {
            return View();
        }
        [HttpPost]
        public ActionResult startService(string serviceName)
        {
            BLL.ManageHR.startService(serviceName);
            return RedirectToAction("ServiceOnOff");
        }
        [HttpPost]
        public ActionResult stopService(string serviceName)
        {
            BLL.ManageHR.stopService(serviceName);
            return RedirectToAction("ServiceOnOff");
        }
        [HttpPost]
        public JsonResult ServiceDataHandler(DTParameters param)
        {
            try
            {
                var data = BLL.Services.TimeTuneServices.getServices();
                int count = BLL.ManageHR.getAllServies(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);
                DTResult<BLL.ViewModels.ServicesViewModel> result = new DTResult<BLL.ViewModels.ServicesViewModel>
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


        #region GroupCalendar
        public ActionResult EmployeeFollowGeneralCalander()
        {
            var employee = BLL.ManageHR.getAllGroupCalendarEmployee();
            return View(employee.employeeFollowGroupCalander);
        }
        public ActionResult Calendar(int group_id)
        {

            return View(group_id);
        }

        public ActionResult CalendarInfo(int group_id)
        {

            return View(group_id);
        }

        [HttpPost]
        public JsonResult getGeneralCalendarForYear(int year, int group_id)
        {
            // need to instantiate a group calendar crud
            // because its constructor determines the current user
            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud(group_id);

            ViewModels.GroupCalendar viewModel = crud.getCalendarForYear(year);
            string toSend = (viewModel == null) ? "null" : System.Web.Helpers.Json.Encode(viewModel);

            return Json(new { data = toSend });
        }


        [HttpPost]
        public JsonResult getShiftsForGeneralCalendar(int group_id)
        {

            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud(group_id);

            List<ViewModels.Shift> shifts = crud.getAllShifts();
            string toSend = System.Web.Helpers.Json.Encode(shifts);


            return Json(new { data = toSend });
        }

        [HttpPost]
        public JsonResult addOrUpdateCalendar(string calendarData, int group_id)
        {

            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud(group_id);

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


        #region EmployeeCalendar
        [HttpPost]
        public JsonResult getGeneralCalendarForYearEmployee(int year, int group_id)
        {
            // need to instantiate a group calendar crud
            // because its constructor determines the current user
            TimeTune.GroupCalendarCrud crud = new TimeTune.GroupCalendarCrud(group_id);

            ViewModels.GroupCalendar viewModel = crud.getCalendarForYear(year);
            string toSend = (viewModel == null) ? "null" : System.Web.Helpers.Json.Encode(viewModel);

            return Json(new { data = toSend });
        }

        public ActionResult ManageEmployeeCalendarInfo(int employee_id)
        {
            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();

            var employees = cal.getAllGroupEmployees(employee_id);

            return View(employees);
        }

        public ActionResult ManageEmployeeCalendar(int employee_id)
        {
            TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();

            var employees = cal.getAllGroupEmployees(employee_id);

            return View(employees);
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


        #endregion

        [HttpPost]
        public JsonResult EmployeeGetDataHandlerExceptSuperHR()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                if (employees[i].employee_code != "000000")
                {
                    toSend[i] = new ChosenAutoCompleteResults();

                    toSend[i].id = employees[i].id.ToString();
                    toSend[i].text = employees[i].employee_code + " - " + employees[i].first_name + " " + employees[i].last_name;
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
        public JsonResult EmployeeGetDataHandler()
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



        #region -Manage-Reports-Access

        public ActionResult ManageReportsAccess()
        {
            // fetch the list of all the HRS, and send to the view,
            // as a list of employees.

            List<PermissionReport> emp = new List<PermissionReport>(BLL.ManageHR.getAllPermissionReportUsersExceptSuperHR());

            return View(emp);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReportsAccess()
        {
            string val = Request.Form["emp-id"];
            string valIsReport01 = Request.Form["is-report-01"] ?? "false";
            string valIsReport02 = Request.Form["is-report-02"] ?? "false";
            string valIsReport03 = Request.Form["is-report-03"] ?? "false";
            string valIsReport04 = Request.Form["is-report-04"] ?? "false";

            // remove employeeId val from HR.
            BLL.ManageHR.addReportAccess(val, bool.Parse(valIsReport01), bool.Parse(valIsReport02), bool.Parse(valIsReport03), bool.Parse(valIsReport04));

            return RedirectToAction("ManageReportsAccess");
        }

        //getAllNonHrs
        [HttpPost]
        public JsonResult getEmployeesCodesForReportsAccessDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = BLL.ManageHR.getAllNonHrsEmployeeCodesMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].employee_code.ToString();
                toSend[i].text =
                    employees[i].employee_code + " - " + employees[i].first_name + " " + employees[i].last_name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteReportAccess()
        {
            string val = Request.Form["emp-id"];

            // remove employeeId val from HR.
            BLL.ManageHR.deleteReportAccess(val);

            return RedirectToAction("ManageReportsAccess");
        }

        #endregion
    }


}
