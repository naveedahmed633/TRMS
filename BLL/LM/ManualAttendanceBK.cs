using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using DLL.Models;
using System.Web;
using System.Data.SqlClient;
using DLL.Models;
using System.Globalization;
using System.Web.UI.WebControls;
namespace TimeTune
{
    public class LMManualAttendance
    {
        public static List<ViewModels.Employee> getEmployeeBySupervisor(string supervisorCode)
        {
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    var supervisor = db.employee.Where(m => m.employee_code.Equals(supervisorCode) && m.active).SingleOrDefault();
                    if (supervisor.Group != null)
                    {
                        int id = supervisor.Group.GroupId;
                        employee = db.employee.Where(m => m.active && m.Group.GroupId == id).ToList();
                    }
                    else
                    {
                        employee = new List<DLL.Models.Employee>();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                List<ViewModels.Employee> toReturn = new List<ViewModels.Employee>();

                //toReturn = employee.Select(p => new ViewModels.Employee()
                //    {
                //        id = p.EmployeeId,
                //        employee_code = p.employee_code,
                //        first_name = p.first_name,
                //        last_name = p.last_name,

                //    }).ToList();

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    employees.email = currentEmployee.email;

                    employees.address = currentEmployee.address;

                    employees.mobile_no = currentEmployee.mobile_no;

                    employees.date_of_joining =
                        (currentEmployee.date_of_joining != null && currentEmployee.date_of_joining.HasValue)
                        ? currentEmployee.date_of_joining.Value.ToString("dd-MM-yyyy") : "";

                    employees.date_of_leaving =
                        (currentEmployee.date_of_leaving != null && currentEmployee.date_of_leaving.HasValue)
                        ? currentEmployee.date_of_leaving.Value.ToString("dd-MM-yyyy") : "";



                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.department_id = (currentEmployee.department != null) ? currentEmployee.department.DepartmentId : -1;
                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.access_group_id = (currentEmployee.access_group != null) ? currentEmployee.access_group.AccessGroupId : -1;
                    employees.access_group_name = (currentEmployee.access_group != null) ? currentEmployee.access_group.name : "";


                    employees.grade_id = (currentEmployee.grade != null) ? currentEmployee.grade.GradeId : -1;
                    employees.grade_name = (currentEmployee.grade != null) ? currentEmployee.grade.name : "";


                    employees.group_id = (currentEmployee.Group != null) ? currentEmployee.Group.GroupId : -1;
                    employees.group_name = (currentEmployee.Group != null) ? currentEmployee.Group.group_name : "";

                    employees.region_id = (currentEmployee.region != null) ? currentEmployee.region.RegionId : -1;
                    employees.region_name = (currentEmployee.region != null) ? currentEmployee.region.name : "";

                    employees.type_of_employment_id = (currentEmployee.type_of_employment != null) ? currentEmployee.type_of_employment.TypeOfEmploymentId : -1;
                    employees.type_of_employment_name = (currentEmployee.type_of_employment != null) ? currentEmployee.type_of_employment.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return toReturn;
            }
        }

        public static List<ViewModels.Employee> getEmployee_All()
        {
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    employee = db.employee.Where(e => e.active && e.timetune_active).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                List<ViewModels.Employee> toReturn = new List<ViewModels.Employee>();


                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    employees.email = currentEmployee.email;

                    employees.address = currentEmployee.address;

                    employees.mobile_no = currentEmployee.mobile_no;

                    employees.date_of_joining =
                        (currentEmployee.date_of_joining != null && currentEmployee.date_of_joining.HasValue)
                        ? currentEmployee.date_of_joining.Value.ToString("dd-MM-yyyy") : "";

                    employees.date_of_leaving =
                        (currentEmployee.date_of_leaving != null && currentEmployee.date_of_leaving.HasValue)
                        ? currentEmployee.date_of_leaving.Value.ToString("dd-MM-yyyy") : "";



                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.department_id = (currentEmployee.department != null) ? currentEmployee.department.DepartmentId : -1;
                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.access_group_id = (currentEmployee.access_group != null) ? currentEmployee.access_group.AccessGroupId : -1;
                    employees.access_group_name = (currentEmployee.access_group != null) ? currentEmployee.access_group.name : "";


                    employees.grade_id = (currentEmployee.grade != null) ? currentEmployee.grade.GradeId : -1;
                    employees.grade_name = (currentEmployee.grade != null) ? currentEmployee.grade.name : "";


                    employees.group_id = (currentEmployee.Group != null) ? currentEmployee.Group.GroupId : -1;
                    employees.group_name = (currentEmployee.Group != null) ? currentEmployee.Group.group_name : "";

                    employees.region_id = (currentEmployee.region != null) ? currentEmployee.region.RegionId : -1;
                    employees.region_name = (currentEmployee.region != null) ? currentEmployee.region.name : "";

                    employees.type_of_employment_id = (currentEmployee.type_of_employment != null) ? currentEmployee.type_of_employment.TypeOfEmploymentId : -1;
                    employees.type_of_employment_name = (currentEmployee.type_of_employment != null) ? currentEmployee.type_of_employment.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return toReturn;
            }
        }

        public static string addManualAttendance(ViewModels.ManualAttendance fromForm, string supervisorCode)
        {
            using (var db = new DLL.Models.Context())
            {
                // return error if any of the dates is empty.
                if (!fromForm.time_in_from.HasValue || !fromForm.time_in_to.HasValue)
                {
                    return "Dates cannot be empty.";
                }
                var groupId = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group.GroupId;


                // get all the logs from the consolidated attendance
                // which fall between the dates specified by the manual attendance
                // view model
                List<DLL.Models.ConsolidatedAttendance> allLogsForEmployee =
                    db.consolidated_attendance.Where(m =>
                        m.employee.employee_code.Equals(fromForm.employee_code)
                        &&
                        m.date.Value >= fromForm.time_in_from.Value &&
                        m.date.Value <= fromForm.time_in_to.Value
                        && m.employee.Group.GroupId == groupId).ToList();

                if (allLogsForEmployee == null || allLogsForEmployee.Count == 0)
                {
                    return "There are no entries for the employee on the selected dates.";
                }

                // if the start date or the end date in the view model is greater
                // than the maximum date in the consolidated result set, return an error, 
                // with the largest date
                DateTime max = allLogsForEmployee.Max(m => m.date).Value.Date;
                DateTime min = allLogsForEmployee.Min(m => m.date).Value.Date;

                if (fromForm.time_in_from.Value > max)
                {
                    return "No entries exists for date " + fromForm.time_in_from.Value.Date.ToString("dd-MMMM-yyyy");
                }
                if (fromForm.time_in_to.Value > max)
                {
                    return "No entries exists for date " + fromForm.time_in_to.Value.Date.ToString("dd-MMMM-yyyy");
                }


                // if any of the logs have three manual attendance already then
                // return an error stating the date which has three manual attendances.
                for (int i = 0; i < allLogsForEmployee.Count; i++)
                {
                    if (allLogsForEmployee[i].manualAttendances.Count >= 3)
                    {
                        return "Limit for manual changes for this employee on date " + allLogsForEmployee[i].date.Value.Date + " has been reached.";
                    }
                }


                // checking if the stride date is not violated.
                ////int strideDate = new BLL.DataFiles().getManualAttendanceStride();

                int strideDate = 5;

                // checking if the stride date is not violated.
                //Read from Text File
                ////strideDate = new BLL.DataFiles().getManualAttendanceStride();

                //Read from DB
                if (int.TryParse(new BLL.DataFiles().getAccessCodeValue(), out strideDate))
                {
                    //IR coded logic
                }

                ConsolidatedAttendance[] invalidEntries;


                DateTime todaysDate = DateTime.Now;
                DateTime firstDayOfThisMonth = new DateTime(todaysDate.Year, todaysDate.Month, 1);


                if (DateTime.Now.Day <= strideDate)
                {
                    invalidEntries = allLogsForEmployee.Where(l =>
                    l.date.Value < firstDayOfThisMonth.AddMonths(-1) ||
                    l.date.Value > todaysDate).ToArray();
                }
                else
                {
                    invalidEntries = allLogsForEmployee.Where(l =>
                        l.date.Value < firstDayOfThisMonth ||
                        l.date.Value > todaysDate).ToArray();
                }



                if (invalidEntries.Count() > 0)
                    return "Unable to change attendance for " + invalidEntries[0].date.Value.ToString("dd-MM-yyyy") + " as CUT-OFF date passed.";

                // assign manual attendance, if you get here.
                for (int i = 0; i < allLogsForEmployee.Count; i++)
                {
                    string remark = (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PLO)) ?
                                        DLL.Commons.FinalRemarks.PLO :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.ABSENT)) ?
                                        DLL.Commons.FinalRemarks.ABSENT :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PRESENT)) ?
                                        DLL.Commons.FinalRemarks.PRESENT :

                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OTV)) ?
                                        DLL.Commons.FinalRemarks.OTV :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OM)) ?
                                        DLL.Commons.FinalRemarks.OM :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OT)) ?
                                        DLL.Commons.FinalRemarks.OT :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OD)) ?
                                        DLL.Commons.FinalRemarks.OD :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OV)) ?
                                        DLL.Commons.FinalRemarks.OV :

                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OFF)) ?
                                        DLL.Commons.FinalRemarks.OFF :
                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.POE)) ?
                                        DLL.Commons.FinalRemarks.POE :
                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PLE)) ?
                                        DLL.Commons.FinalRemarks.PLE : null;
                    if (remark == null)
                    {
                        return "Invalid remarks";
                    }

                    ///////////////////////////////////////////////////////////////////////////////

                    var mcons = new DLL.Models.ManualAttendance()
                    {
                        active = true,

                        // current user.
                        employee = db.employee.Where(m => m.employee_code.Equals(HttpContext.Current.User.Identity.Name)).FirstOrDefault(),
                        ManualAttendanceId = allLogsForEmployee[i].ConsolidatedAttendanceId,
                        remarks = remark
                    };

                    if (allLogsForEmployee[i].final_remarks.ToLower() != "off")
                    {
                        allLogsForEmployee[i].manualAttendances.Add(mcons);
                        allLogsForEmployee[i].final_remarks = remark;
                    }

                    //allLogsForEmployee[i].manualAttendances.Add(new DLL.Models.ManualAttendance()
                    //{
                    //    active = true,

                    //    // current user.
                    //    employee = db.employee.Where(m => m.employee_code.Equals(HttpContext.Current.User.Identity.Name)).FirstOrDefault(),
                    //    ManualAttendanceId = allLogsForEmployee[i].ConsolidatedAttendanceId,
                    //    remarks = remark
                    //});

                    //allLogsForEmployee[i].final_remarks = remark;

                    //////////////////////////////////////////////////////////////////////////////////////
                }

                db.SaveChanges();
                return null;
            }



        }


    }



    public class SLMManualAttendance
    {
        public static List<ViewModels.Employee> getEmployeesBySLM(string slmCode)
        {
            using (Context db = new Context())
            {
                List<ViewModels.Employee> toReturn = null;
                try
                {
                    var slm = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(slmCode) &&
                        m.access_group.name.Equals(DLL.Commons.Roles.ROLE_SLM)
                        ).FirstOrDefault();


                    if (slm != null)
                    {

                        toReturn =

                            db.super_line_manager_tagging.Where(m =>
                                m.superLineManager.EmployeeId.Equals(slm.EmployeeId) &&
                                m.taggedEmployee.active).Select(p =>

                                    new ViewModels.Employee()
                                    {
                                        id = p.taggedEmployee.EmployeeId,
                                        employee_code = p.taggedEmployee.employee_code,
                                        first_name = p.taggedEmployee.first_name,
                                        last_name = p.taggedEmployee.last_name
                                    }

                                ).ToList();

                        //Inayat commented code
                        //toReturn.Add(new ViewModels.Employee()
                        //{
                        //    employee_code = slm.employee_code,
                        //    first_name = slm.first_name,
                        //    last_name = slm.last_name
                        //});

                    }
                    else
                    {
                        toReturn = new List<ViewModels.Employee>();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.Employee>();
                }




                return toReturn;
            }
        }

        public static List<ViewModels.Employee> getEmployeesBySLMWithLM(string slmCode)
        {
            using (Context db = new Context())
            {
                List<ViewModels.Employee> toReturn = null;
                try
                {
                    var slm = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(slmCode) &&
                        m.access_group.name.Equals(DLL.Commons.Roles.ROLE_SLM)
                        ).FirstOrDefault();


                    if (slm != null)
                    {

                        toReturn =

                            db.super_line_manager_tagging.Where(m =>
                                //m.taggedEmployee.employee_code != slmCode &&
                                m.superLineManager.EmployeeId.Equals(slm.EmployeeId) &&
                                m.taggedEmployee.active).Select(p =>

                                    new ViewModels.Employee()
                                    {
                                        //id = p.taggedEmployee.EmployeeId,
                                        employee_code = p.taggedEmployee.employee_code,
                                        first_name = p.taggedEmployee.first_name,
                                        last_name = p.taggedEmployee.last_name
                                    }

                                ).OrderBy(m => m.employee_code).ToList();

                        //toReturn.Add(new ViewModels.Employee()
                        //{
                        //    employee_code = slm.employee_code,
                        //    first_name = slm.first_name,
                        //    last_name = slm.last_name
                        //});


                    }
                    else
                    {
                        toReturn = new List<ViewModels.Employee>();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.Employee>();
                }




                return toReturn;
            }
        }

        public static string addManualAttendance(ViewModels.ManualAttendance fromForm, string slmCode)
        {
            using (var db = new DLL.Models.Context())
            {
                // return error if any of the dates is empty.
                if (!fromForm.time_in_from.HasValue || !fromForm.time_in_to.HasValue)
                {
                    return "Dates cannot be empty.";
                }




                var slm = db.employee.Where(m =>
                    m.active &&
                    m.employee_code.Equals(slmCode) &&
                    m.access_group.name.Equals(DLL.Commons.Roles.ROLE_SLM)).FirstOrDefault();



                // get all the logs from the consolidated attendance
                // which fall between the dates specified by the manual attendance
                // view model
                List<DLL.Models.ConsolidatedAttendance> allLogsForEmployee = null;

                if (slm != null)
                {
                    SLM tag = db.super_line_manager_tagging.Where(m =>
                        m.superLineManager.employee_code.Equals(slm.employee_code) &&
                        m.taggedEmployee.employee_code.Equals(fromForm.employee_code) &&
                        m.taggedEmployee.active
                        ).FirstOrDefault();

                    // If the employee is tagged under the slm, or the employee is the SLM.
                    if (tag != null || fromForm.employee_code.Equals(slm.employee_code))
                    {
                        allLogsForEmployee =
                            db.consolidated_attendance.Where(m =>
                                m.employee.employee_code.Equals(fromForm.employee_code)
                                &&
                                m.date.Value >= fromForm.time_in_from.Value &&
                                m.date.Value <= fromForm.time_in_to.Value).ToList();
                    }
                    else
                    {
                        return "Invalid employee code, please make sure that the employee has not been removed.";
                    }

                }
                else
                {
                    return "Invalid request, SLM not found.";
                }

                if (allLogsForEmployee == null || allLogsForEmployee.Count == 0)
                {
                    return "There are no entries for the employee on the selected dates.";
                }

                // if the start date or the end date in the view model is greater
                // than the maximum date in the consolidated result set, return an error, 
                // with the largest date
                DateTime max = allLogsForEmployee.Max(m => m.date).Value;
                DateTime min = allLogsForEmployee.Min(m => m.date).Value;

                if (fromForm.time_in_from.Value > max)
                {
                    return "No entries exists for date " + fromForm.time_in_from.Value.Date.ToString("dd-MMMM-yyyy");
                }
                if (fromForm.time_in_to.Value > max)
                {
                    return "No entries exists for date " + fromForm.time_in_to.Value.Date.ToString("dd-MMMM-yyyy");
                }


                // if any of the logs have three manual attendance already then
                // return an error stating the date which has three manual attendances.
                for (int i = 0; i < allLogsForEmployee.Count; i++)
                {
                    if (allLogsForEmployee[i].manualAttendances.Count >= 3)
                    {
                        return "Limit for manual changes for this employee on date " + allLogsForEmployee[i].date.Value.Date + " has been reached.";
                    }
                }

                // checking if the stride date is not violated.
                ////int strideDate = new BLL.DataFiles().getManualAttendanceStride();

                int strideDate = 5;

                // checking if the stride date is not violated.
                //Read from Text File
                ////strideDate = new BLL.DataFiles().getManualAttendanceStride();

                //Read from DB
                if (int.TryParse(new BLL.DataFiles().getAccessCodeValue(), out strideDate))
                {
                    //IR coded logic
                }

                ConsolidatedAttendance[] invalidEntries;


                DateTime todaysDate = DateTime.Now;
                DateTime firstDayOfThisMonth = new DateTime(todaysDate.Year, todaysDate.Month, 1);


                if (DateTime.Now.Day <= strideDate)
                {
                    invalidEntries = allLogsForEmployee.Where(l =>
                    l.date.Value < firstDayOfThisMonth.AddMonths(-1) ||
                    l.date.Value > todaysDate).ToArray();
                }
                else
                {
                    invalidEntries = allLogsForEmployee.Where(l =>
                        l.date.Value < firstDayOfThisMonth ||
                        l.date.Value > todaysDate).ToArray();
                }

                if (invalidEntries.Count() > 0)
                    return "Unable to change attendance for " + invalidEntries[0].date.Value.ToString("dd-MM-yyyy") + " as CUT-OFF date passed.";


                // assign manual attendance, if you get here.
                for (int i = 0; i < allLogsForEmployee.Count; i++)
                {
                    string remark = (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PLO)) ?
                                        DLL.Commons.FinalRemarks.PLO :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.ABSENT)) ?
                                        DLL.Commons.FinalRemarks.ABSENT :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PRESENT)) ?
                                        DLL.Commons.FinalRemarks.PRESENT :

                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OTV)) ?
                                        DLL.Commons.FinalRemarks.OTV :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OM)) ?
                                        DLL.Commons.FinalRemarks.OM :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OT)) ?
                                        DLL.Commons.FinalRemarks.OT :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OD)) ?
                                        DLL.Commons.FinalRemarks.OD :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OV)) ?
                                        DLL.Commons.FinalRemarks.OV :

                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OFF)) ?
                                        DLL.Commons.FinalRemarks.OFF :
                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.POE)) ?
                                        DLL.Commons.FinalRemarks.POE :
                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PLE)) ?
                                        DLL.Commons.FinalRemarks.PLE : null;
                    if (remark == null)
                    {
                        return "Invalid remarks";
                    }

                    ///////////////////////////////////////////////////////////////////////////////

                    var mcons = new DLL.Models.ManualAttendance()
                    {
                        active = true,

                        // current user.
                        employee = db.employee.Where(m => m.employee_code.Equals(HttpContext.Current.User.Identity.Name)).FirstOrDefault(),
                        ManualAttendanceId = allLogsForEmployee[i].ConsolidatedAttendanceId,
                        remarks = remark
                    };

                    if (allLogsForEmployee[i].final_remarks.ToLower() != "off")
                    {
                        allLogsForEmployee[i].manualAttendances.Add(mcons);
                        allLogsForEmployee[i].final_remarks = remark;
                    }

                    //allLogsForEmployee[i].manualAttendances.Add(new DLL.Models.ManualAttendance()
                    //{
                    //    active = true,

                    //    // current user.
                    //    employee = db.employee.Where(m => m.employee_code.Equals(HttpContext.Current.User.Identity.Name)).FirstOrDefault(),
                    //    ManualAttendanceId = allLogsForEmployee[i].ConsolidatedAttendanceId,
                    //    remarks = remark
                    //});

                    //allLogsForEmployee[i].final_remarks = remark;

                    ///////////////////////////////////////////////////////////////////////////////
                }

                db.SaveChanges();
                return null;
            }



        }
    }
}
