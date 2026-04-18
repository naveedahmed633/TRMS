using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI.WebControls;
using BLL.ADMIN;
using BLL.ViewModels;



namespace BLL
{
    public class ManageHR
    {
        public static List<OrganizationCampusView> getAllCampusesForHRAssignment()
        {
            List<OrganizationCampusView> lCampuses = new List<OrganizationCampusView>();

            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                DLL.Models.OrganizationCampus[] cmp = db.organization_campus.ToArray();

                if (cmp.Length > 0)
                {
                    for (int i = 0; i < cmp.Length; i++)
                    {
                        lCampuses.Add(new OrganizationCampusView { Id = cmp[i].Id, CampusCode = cmp[i].CampusCode + "-" + cmp[i].CampusTitle });
                    }
                }
            }

            return lCampuses;
        }

        public static Employee[] getAllHRsExceptSuperHR()
        {
            List<Employee> emp = new List<Employee>();

            using (DLL.Models.Context db = new DLL.Models.Context())
            {

                DLL.Models.Employee[] emps = db.employee.Where(m => !m.employee_code.Equals("000000") &&
                    m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) &&
                    m.active).ToArray();



                for (int i = 0; i < emps.Length; i++)
                {
                    emp.Add(TimeTune.EmployeeCrud.convertForHR(emps[i]));
                }



            }

            return emp.ToArray();
        }

        public static PermissionReport[] getAllPermissionReportUsersExceptSuperHR()
        {
            List<PermissionReport> permEmp = new List<PermissionReport>();

            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                DLL.Models.PermissionReport[] emps = db.permission_report.Where(m => !m.employee_code.Equals("000000")).ToArray();
                if (emps.Length > 0)
                {
                    for (int i = 0; i < emps.Length; i++)
                    {
                        permEmp.Add(new PermissionReport()
                        {
                            employee_code = emps[i].employee_code,
                            prep_01 = emps[i].prep_01,
                            prep_02 = emps[i].prep_02,
                            prep_03 = emps[i].prep_03,
                            prep_04 = emps[i].prep_04
                        });
                    }
                }
            }

            return permEmp.ToArray();
        }

        public static Employee[] getAllHr()
        {
            List<Employee> emp = new List<Employee>();

            using (DLL.Models.Context db = new DLL.Models.Context())
            {

                DLL.Models.Employee[] emps = db.employee.Where(m =>
                    m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) &&
                    m.active).ToArray();



                for (int i = 0; i < emps.Length; i++)
                {
                    emp.Add(TimeTune.EmployeeCrud.convert(emps[i]));
                }



            }

            return emp.ToArray();
        }



        public static void addHr(string employeeID, int campusID, bool isSuperHR)
        {
            int empId;

            if (int.TryParse(employeeID, out empId))
            {
                using (DLL.Models.Context db = new DLL.Models.Context())
                {
                    // Get employee
                    DLL.Models.Employee employee =
                        db.employee.Where(m =>
                            m.EmployeeId.Equals(empId)).FirstOrDefault();
                    if (employee != null)
                    {
                        // Get access group object for EMP.
                        DLL.Models.AccessGroup hr =
                            db.access_group.Where(m =>
                                m.name.Equals(BLL.TimeTuneRoles.ROLE_HR)).FirstOrDefault();
                        if (hr != null)
                        {
                            // Elevate the employee to HR.
                            TimeTune.EmployeeCrud.changeEmployeeAccessGroup(hr.AccessGroupId, db, employee);

                            employee.campus_id = campusID;
                            employee.is_super_hr = isSuperHR;
                            employee.function = db.function.Where(f => f.name.ToLower() == BLL.TimeTuneFunction.FUNCTION_STAFF).FirstOrDefault(); //staff
                            db.SaveChanges();
                        }

                        //IR Permissions-Management
                        int resp = 0;
                        resp = TimeTune.EmployeeCrud.resetPermissionsByEmpID(empId);
                    }
                }
            }
        }

        public static void deleteHr(string employeeID)
        {
            int empId;

            if (int.TryParse(employeeID, out empId))
            {
                using (DLL.Models.Context db = new DLL.Models.Context())
                {
                    // Get employee
                    DLL.Models.Employee employee =
                        db.employee.Where(m =>
                            m.EmployeeId.Equals(empId)).FirstOrDefault();
                    if (employee != null)
                    {
                        // Get access group object for EMP.
                        DLL.Models.AccessGroup empAG =
                            db.access_group.Where(m =>
                                m.name.Equals(BLL.TimeTuneRoles.ROLE_EMP)).FirstOrDefault();
                        if (empAG != null)
                        {
                            // Change the employee to HR only if the employee
                            // is an HR currently.
                            if (employee.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR))
                            {
                                TimeTune.EmployeeCrud
                                    .changeEmployeeAccessGroup(empAG.AccessGroupId, db, employee);

                                db.SaveChanges();
                            }
                        }

                        employee.is_super_hr = false;
                        db.SaveChanges();

                        //IR Permissions-Management
                        int resp = 0;
                        resp = TimeTune.EmployeeCrud.resetPermissionsByEmpID(empId);
                    }
                }

            }
        }


        public static bool deleteConsolidate(string employee_id, string date)
        {
            int emp = (employee_id != null) ? int.Parse(employee_id) : -1;
            DateTime toDelete = DateTime.Parse(date);
            DLL.Models.ConsolidatedAttendance[] consolidate;
            using (var db = new DLL.Models.Context())
            {
                if (emp != -1)
                {
                    var singleDelete = db.consolidated_attendance.Where(c => c.active == true && c.employee.EmployeeId == emp && c.date == toDelete).FirstOrDefault();
                    if (singleDelete != null)
                    {
                        db.consolidated_attendance.Remove(singleDelete);
                    }
                }
                else
                {
                    consolidate = db.consolidated_attendance.Where(c => c.date == toDelete).ToArray();
                    if (consolidate.Length > 0)
                    {
                        db.consolidated_attendance.RemoveRange(consolidate);
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /////////////////////////////////////////////
        public static int addReportAccess(string employeeCode, bool isReport01, bool isReport02, bool isReport03, bool isReport04)
        {
            int response = 0;

            if (employeeCode != null && employeeCode != "")
            {
                using (DLL.Models.Context db = new DLL.Models.Context())
                {
                    // Get employee
                    DLL.Models.PermissionReport permUpdate = db.permission_report.Where(m => m.employee_code.Equals(employeeCode)).FirstOrDefault();
                    if (permUpdate != null)
                    {
                        permUpdate.employee_code = employeeCode;
                        permUpdate.prep_01 = isReport01;//device-report
                        permUpdate.prep_02 = isReport02;//device-report
                        permUpdate.prep_03 = isReport03;//device-report
                        permUpdate.prep_04 = isReport04;//device-report

                        db.SaveChanges();

                        response = 1;
                    }
                    else
                    {
                        DLL.Models.PermissionReport permAdd = new DLL.Models.PermissionReport();

                        permAdd.employee_code = employeeCode;
                        permAdd.prep_01 = isReport01;//device-report
                        permAdd.prep_02 = isReport02;//device-report
                        permAdd.prep_03 = isReport03;//device-report
                        permAdd.prep_04 = isReport04;//device-report


                        db.permission_report.Add(permAdd);
                        db.SaveChanges();

                        response = 2;
                    }
                }
            }

            return response;
        }

        public static void deleteReportAccess(string employeeCode)
        {
            if (employeeCode != null && employeeCode != "")
            {
                using (DLL.Models.Context db = new DLL.Models.Context())
                {
                    // Get employee
                    DLL.Models.PermissionReport perm =
                        db.permission_report.Where(m =>
                            m.employee_code.Equals(employeeCode)).FirstOrDefault();
                    if (perm != null)
                    {
                        db.permission_report.Remove(perm);
                        db.SaveChanges();
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////


        #region Terminals
        public static string updateTerminal(int id, string terminal_name, string terminal_id, string branch_code, string branch_name, string t_type)
        {

            if (id > 0)
            {
                using (var db = new DLL.Models.Context())
                {
                    var terminal = db.termainal.Where(c => c.L_ID.Equals(id)).FirstOrDefault();
                    terminal.C_Name = terminal_name;
                    terminal.terminal_id = terminal_id;
                    terminal.branch_code = branch_code;
                    terminal.branch_name = branch_name;
                    terminal.type = t_type;
                    if (db.SaveChanges() > 0)
                    {
                        return "Terminal Edited";
                    }
                    else
                    {
                        return "Unable to add Terminal";
                    }
                }
            }
            else

                return null;
        }

        public static int getAllTerminal(string search, string sortOrder, int start, int length, out List<ViewModels.Terminals> toReturn)
        {
            if (!(sortOrder.Contains("id") || sortOrder.Contains("name") || sortOrder.Contains("terminal_id")))
            {
                sortOrder = "id";
            }
            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                int count = 0;


                toReturn = new List<ViewModels.Terminals>();
                try
                {
                    count = db.termainal
                        .Where(p => search == null ||
                            search.Equals("") ||
                            p.C_Name.Contains(search.ToLower()) || p.terminal_id.Contains(search.ToLower()) || p.L_ID.ToString().Contains(search.ToLower()) ||
                            db.termainal.Where(e => e.C_Name.Equals(p.C_Name)).FirstOrDefault().C_Name.Contains(search.ToLower()) ||
                            db.termainal.Where(e => e.terminal_id.Equals(p.terminal_id)).FirstOrDefault().terminal_id.Contains(search.ToLower())
                            ).Count();

                    List<ViewModels.Terminals> temp = db.termainal
                        .Where(p =>
                            search == null ||
                            search.Equals("") ||
                            p.C_Name.Contains(search.ToLower()) ||
                            db.termainal.Where(e => e.C_Name.Equals(p.C_Name)).FirstOrDefault().C_Name.Contains(search.ToLower()) || p.L_ID.ToString().Contains(search.ToLower()) ||
                             db.termainal.Where(e => e.terminal_id.Equals(p.terminal_id)).FirstOrDefault().terminal_id.Contains(search.ToLower())
                            )
                        .Select(p =>
                            new ViewModels.Terminals()
                            {
                                id = p.L_ID,
                                name = p.C_Name,
                                terminal_id = p.terminal_id,
                                branch_code = p.branch_code,
                                branch_name = p.branch_name,
                                type = p.type
                            })
                        .SortBy(sortOrder).Skip(start).Take(length).ToList();


                    foreach (var log in temp)
                    {

                        toReturn.Add(new ViewModels.Terminals()
                        {
                            id = log.id,
                            name = log.name,
                            terminal_id = log.terminal_id,
                            branch_code = log.branch_code,
                            branch_name = log.branch_name,
                            type = log.type,
                            action =
                            @"<div data-row='" + log.id + "'>" +
                                "<a href=\"javascript:void(editTerminal(" + log.id + ",'" + log.name + "','" + log.terminal_id + "','" + log.branch_code + "','" + log.branch_name + "','" + log.type + "'));\">Edit</a>" +
                                "<span> / </span>" +
                                "<a href=\"javascript:void(deleteTerminal(" + log.id + "));\">Delete</a>" +
                            "</div>"

                        });

                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.Terminals>();
                }


                return count;
            }
        }

        public static string deleteTerminal(int id)
        {
            using (var db = new DLL.Models.Context())
            {

                var terminal = db.termainal.Where(c => c.L_ID == id).FirstOrDefault();
                if (terminal == null)
                {
                    return "Alreday deleted";
                }
                db.termainal.Remove(terminal);

                if (db.SaveChanges() > 0)
                {
                    return "deleted Successfull";
                }
                else
                {
                    return "Unable to delete the entry";
                }
            }


        }

        public static string addTerminal(int id, string terminal_name, string terminal_id, string branch_code, string branch_name, string type)
        {
            using (var db = new DLL.Models.Context())
            {
                var ter = db.termainal.Where(c => c.L_ID == id).FirstOrDefault();
                if (ter != null)
                {
                    return "Terminal already added";
                }
                db.termainal.Add(new DLL.Models.Terminals
                {
                    L_ID = id,
                    C_Name = terminal_name,
                    terminal_id = terminal_id,
                    branch_code = branch_code,
                    branch_name = branch_name,
                    type = type
                });
                if (db.SaveChanges() > 0)
                {
                    return "";
                }
                else
                {
                    return "Unable to add terminal";
                }
            }
        }
        public static string uploadTerminal(List<string> csvContents)
        {
            int id = 0;
            try
            {
                using (var db = new DLL.Models.Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 3 values (columns).

                        if (values == null || values.Length < 5)
                            continue;

                        // Remove unwanted " characters.
                        string terminal = values[0].Replace("\"", "");
                        values[1] = values[1].Replace("\"", "");
                        values[2] = values[2].Replace("\"", "");
                        values[3] = values[3].Replace("\"", "");
                        values[4] = values[4].Replace("\"", "");


                        // continue if its the first row (CSV Header line).
                        if (terminal == "id" || terminal == "")
                        {
                            continue;
                        }
                        else
                        {
                            id = int.Parse(terminal);
                        }



                        if (BLL.ManageHR.addTerminal(id, values[1], values[2], values[3], values[4], "") == "Terminal already added")
                        {
                            BLL.ManageHR.updateTerminal(id, values[1], values[2], values[3], values[4], "");
                        }

                    }



                    return "Successfull";
                }


            }

            catch (Exception)
            {
                return "failed";
            }
        }
        #endregion

        //Future Manual Attendance

        #region Add Future Manual Attendance
        public static string addManualAttendance(string from_date, string to_date, string employee_code, string final_remarks)
        {
            using (var db = new DLL.Models.Context())
            {
                int emp_code = int.Parse(employee_code);
                DateTime fromDate = DateTime.Parse(from_date);
                DateTime toDate = DateTime.Parse(to_date);
                string empCode = employee_code;
                var fma = db.futureManualAttendance.Where(c => c.employee_code == empCode && c.from_date <= fromDate && c.to_date >= toDate).FirstOrDefault();
                if (fma == null)
                {
                    db.futureManualAttendance.Add(new DLL.Models.FutureManualAttendance
                    {
                        employee_code = empCode,
                        from_date = DateTime.Parse(from_date),
                        to_date = DateTime.Parse(to_date),
                        remarks = final_remarks
                    });
                    if (db.SaveChanges() > 0)
                    {
                        return "Successful"; //return "";
                    }
                    else
                    {
                        return "Unable to add Manaul Attendance";
                    }
                }
                else
                {
                    return "Future manaul attendance already added";
                }

            }
        }
        public static string addManualAttendanceHr(string from_date, string to_date, string employee_code, string final_remarks)
        {
            using (var db = new DLL.Models.Context())
            {
                int emp_code = int.Parse(employee_code);
                DateTime fromDate = DateTime.Parse(from_date);
                DateTime toDate = DateTime.Parse(to_date);
                var employee = db.employee.Where(c => c.EmployeeId == emp_code && c.active == true).FirstOrDefault();
                string empCode = (employee != null) ? employee.employee_code : "";
                var fma = db.futureManualAttendance.Where(c => c.employee_code == empCode && c.from_date <= fromDate && c.to_date >= toDate).FirstOrDefault();
                if (fma == null)
                {
                    db.futureManualAttendance.Add(new DLL.Models.FutureManualAttendance
                    {
                        employee_code = empCode,
                        from_date = DateTime.Parse(from_date),
                        to_date = DateTime.Parse(to_date),
                        remarks = final_remarks
                    });
                    if (db.SaveChanges() > 0)
                    {
                        return "Successfull";
                    }
                    else
                    {
                        return "Unable to add Manaul Attendance";
                    }
                }
                else
                {
                    return "Future manaul attendance already added";
                }

            }
        }

        public static int DeleteLeave(int id)
        {
            int response = 0;

            if (id > 0)
            {
                try
                {
                    using (DLL.Models.Context db = new DLL.Models.Context())
                    {
                        var leaveRow = db.futureManualAttendance.Find(id);
                        if (leaveRow != null)
                        {
                            db.futureManualAttendance.Remove(leaveRow);
                            db.SaveChanges();

                            response = 1;
                        }
                    }
                }
                catch (Exception)
                {
                    response = -1;
                }
            }

            return response;
        }

        public static int getAllManualAttendance(string search, string sortOrder, int start, int length, out List<ViewModels.ViewFutureManualAttendance> toReturn)
        {
            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                int count = 0;


                toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                try
                {
                    count = db.futureManualAttendance
                        .Where(p => search == null ||
                            search.Equals("") ||
                            p.employee_code.Contains(search.ToLower()) ||
                            db.futureManualAttendance.Where(e => e.employee_code.Equals(p.employee_code)).FirstOrDefault().employee_code.Contains(search.ToLower()) ||
                            db.futureManualAttendance.Where(e => e.remarks.Equals(p.remarks)).FirstOrDefault().remarks.Contains(search.ToLower())

                            ).Count();

                    List<ViewModels.FutureManualAttedance> temp = db.futureManualAttendance
                    .Where(p =>
                        search == null ||
                        search.Equals("") ||
                        p.employee_code.Contains(search.ToLower()) ||
                        db.futureManualAttendance.Where(e => e.employee_code.Equals(p.employee_code)).FirstOrDefault().employee_code.Contains(search.ToLower()) ||
                        db.futureManualAttendance.Where(e => e.remarks.Equals(p.remarks)).FirstOrDefault().remarks.Contains(search.ToLower())
                        )
                    .Select(p =>
                        new ViewModels.FutureManualAttedance()
                        {
                            Id = p.Id,
                            employee_code = p.employee_code,
                            remarks = p.remarks,
                            from_date = p.from_date,
                            to_date = p.to_date
                        })
                    .SortBy(sortOrder).Skip(start).Take(length).ToList();


                    foreach (var log in temp)
                    {
                        string lremarks = ""; bool isAllowedDate = false;
                        lremarks = (log.remarks != null && log.remarks != "") ? log.remarks.ToUpper() : "";
                        isAllowedDate = (log.to_date.Date >= DateTime.Now.Date) ? true : false;

                        if (lremarks == "LV" && isAllowedDate)
                        {
                            toReturn.Add(new ViewModels.ViewFutureManualAttendance()
                            {
                                Id = log.Id,
                                employee_code = log.employee_code,
                                remarks = log.remarks,
                                from_date = log.from_date.Date.ToString("dd-MM-yyyy"),
                                to_date = log.to_date.Date.ToString("dd-MM-yyyy"),
                                //actions = "<form id='form_leave' method='POST'><input id='lv_id' type='hidden' value='" + log.Id + "' /><input type='submit' value='DELETE' /></form>"
                                actions = "<a style='cursor: pointer; color: #3BAFDA;' onclick='javascript:return deleteLeave(" + log.Id + ");'>DELETE</a>"
                            });
                        }
                        else
                        {
                            toReturn.Add(new ViewModels.ViewFutureManualAttendance()
                            {
                                Id = log.Id,
                                employee_code = log.employee_code,
                                remarks = log.remarks,
                                from_date = log.from_date.Date.ToString("dd-MM-yyyy"),
                                to_date = log.to_date.Date.ToString("dd-MM-yyyy"),
                                actions = "-"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                }


                return count;
            }
        }
        public static int getEmpManualAttendance(string employeeCode, string search, string sortOrder, int start, int length, out List<ViewModels.ViewFutureManualAttendance> toReturn)
        {
            using (var db = new DLL.Models.Context())
            {
                int count = 0;
                toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                try
                {
                    count = db.futureManualAttendance
                        .Where(p => (search == null ||
                            search.Equals("") ||
                            p.employee_code.ToLower().Contains(search.ToLower()) ||
                            p.remarks.ToLower().Contains(search.ToLower())) && p.employee_code == employeeCode
                            ).Count();
                    var fa = db.futureManualAttendance.Where(p => (search == null ||
                            search.Equals("") ||
                            p.employee_code.ToLower().Contains(search.ToLower()) ||
                            p.remarks.ToLower().Contains(search.ToLower())) && p.employee_code == employeeCode
                            ).SortBy(sortOrder).Skip(start).Take(length).ToList();
                    foreach (var entity in fa)
                    {
                        ViewModels.ViewFutureManualAttendance toAdd = new ViewModels.ViewFutureManualAttendance();
                        toAdd.Id = entity.Id;
                        toAdd.employee_code = entity.employee_code;
                        toAdd.from_date = entity.from_date.ToString("dd-MM-yyyy");
                        toAdd.to_date = entity.to_date.ToString("dd-MM-yyyy");
                        toAdd.remarks = entity.remarks;
                        toReturn.Add(toAdd);
                    }


                }
                catch (Exception ex)
                {
                    toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                }
                return count;
            }
        }

        public static int getLmManualAttendance(string employeeCode, string search, string sortOrder, int start, int length, out List<ViewModels.ViewFutureManualAttendance> toReturn)
        {
            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                int count = 0;


                toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                try
                {
                    var emp = db.employee.Where(c => c.employee_code.Equals(employeeCode) && c.active == true).FirstOrDefault();
                    int empInt = (emp.EmployeeId != 0) ? emp.EmployeeId : 0;
                    var employees = db.group.Where(c => c.supervisor_id.Equals(empInt) && c.active == true).FirstOrDefault().Employees.ToList();

                    List<DLL.Models.FutureManualAttendance> futureAttendance = new List<DLL.Models.FutureManualAttendance>();

                    foreach (var entity in employees)
                    {
                        string emp_code = entity.employee_code;
                        var fa = db.futureManualAttendance.Where(c => c.employee_code.Equals(emp_code)).ToList();
                        if (fa != null)
                            futureAttendance.AddRange(fa);
                        else
                            continue;
                    }
                    count = futureAttendance
                        .Where(p => search == null ||
                            search.Equals("") ||
                            p.employee_code.Contains(search) ||
                            p.remarks.ToLower().Contains(search.ToLower())
                            ).Count();

                    List<ViewModels.FutureManualAttedance> temp = futureAttendance.AsQueryable()
                    .Where(p =>
                        search == null ||
                        search.Equals("") ||
                         p.employee_code.Contains(search) ||
                         p.remarks.ToLower().Contains(search.ToLower())
                        )
                    .Select(p =>
                        new ViewModels.FutureManualAttedance()
                        {
                            Id = p.Id,
                            employee_code = p.employee_code,
                            remarks = p.remarks,
                            from_date = p.from_date,
                            to_date = p.to_date
                        })
                    .SortBy(sortOrder).Skip(start).Take(length).ToList();


                    foreach (var log in temp)
                    {
                        string lremarks = ""; bool isAllowedDate = false;
                        lremarks = (log.remarks != null && log.remarks != "") ? log.remarks.ToUpper() : "";
                        isAllowedDate = (log.to_date.Date >= DateTime.Now.Date) ? true : false;

                        if (lremarks == "LV" && isAllowedDate)
                        {
                            toReturn.Add(new ViewModels.ViewFutureManualAttendance()
                            {
                                Id = log.Id,
                                employee_code = log.employee_code,
                                remarks = log.remarks,
                                from_date = log.from_date.Date.ToString("dd-MM-yyyy"),
                                to_date = log.to_date.Date.ToString("dd-MM-yyyy"),
                                //actions = "<form id='form_leave' method='POST'><input id='lv_id' type='hidden' value='" + log.Id + "' /><input type='submit' value='DELETE' /></form>"
                                actions = "<a style='cursor: pointer; color: #3BAFDA;' onclick='javascript:return deleteLeave(" + log.Id + ");'>DELETE</a>"
                            });
                        }
                        else
                        {
                            toReturn.Add(new ViewModels.ViewFutureManualAttendance()
                            {
                                Id = log.Id,
                                employee_code = log.employee_code,
                                remarks = log.remarks,
                                from_date = log.from_date.Date.ToString("dd-MM-yyyy"),
                                to_date = log.to_date.Date.ToString("dd-MM-yyyy"),
                                actions = "-"
                            });
                        }


                        //toReturn.Add(new ViewModels.ViewFutureManualAttendance()
                        //{
                        //    Id = log.Id,
                        //    employee_code = log.employee_code,
                        //    remarks = log.remarks,
                        //    from_date = log.from_date.Date.ToString("dd-MM-yyyy"),
                        //    to_date = log.to_date.Date.ToString("dd-MM-yyyy")
                        //});

                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                }


                return count;
            }
        }
        public static int getSLMManualAttendance(string employeeCode, string search, string sortOrder, int start, int length, out List<ViewModels.ViewFutureManualAttendance> toReturn)
        {
            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                int count = 0;


                toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                try
                {
                    var emp = db.employee.Where(c => c.employee_code.Equals(employeeCode) && c.active == true).FirstOrDefault();
                    int empInt = (emp.EmployeeId != 0) ? emp.EmployeeId : 0;
                    var employees = db.super_line_manager_tagging.Where(c => c.superLineManager.EmployeeId.Equals(empInt)).ToList();

                    List<DLL.Models.FutureManualAttendance> futureAttendance = new List<DLL.Models.FutureManualAttendance>();

                    foreach (var entity in employees)
                    {
                        string emp_code = entity.taggedEmployee.employee_code;
                        var fa = db.futureManualAttendance.Where(c => c.employee_code.Equals(emp_code)).ToList();
                        if (fa != null)
                            futureAttendance.AddRange(fa);
                        else
                            continue;
                    }
                    var toRemove = futureAttendance.Where(c => c.employee_code == null || c.remarks == null).ToList();
                    foreach (var entity in toRemove)
                    {
                        futureAttendance.Remove(entity);
                    }

                    count = futureAttendance
                        .Where(p => search == null ||
                            search.Equals("") ||
                            p.employee_code.ToLower().Contains(search.ToLower()) ||
                            p.remarks.ToLower().Contains(search.ToLower())
                            ).Count();

                    List<ViewModels.FutureManualAttedance> temp = futureAttendance.AsQueryable()
                    .Where(p =>
                        search == null ||
                        search.Equals("") ||
                        p.employee_code.ToLower().Contains(search.ToLower()) ||
                        p.remarks.ToLower().Contains(search.ToLower())
                        )
                    .Select(p =>
                        new ViewModels.FutureManualAttedance()
                        {
                            Id = p.Id,
                            employee_code = p.employee_code,
                            remarks = p.remarks,
                            from_date = p.from_date,
                            to_date = p.to_date
                        })
                    .SortBy(sortOrder).Skip(start).Take(length).ToList();


                    foreach (var log in temp)
                    {

                        toReturn.Add(new ViewModels.ViewFutureManualAttendance()
                        {
                            Id = log.Id,
                            employee_code = log.employee_code,
                            remarks = log.remarks,
                            from_date = log.from_date.Date.ToString("dd-MM-yyyy"),
                            to_date = log.to_date.Date.ToString("dd-MM-yyyy")
                        });

                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.ViewFutureManualAttendance>();
                }


                return count;
            }
        }
        #endregion


        #region Get All Group Calendar Following Employees
        public static EmployeeWithGroup getAllGroupCalendarEmployee()
        {
            EmployeeWithGroup toReturn = new EmployeeWithGroup();
            using (var db = new DLL.Models.Context())
            {
                List<int> employee_id = new System.Collections.Generic.List<int>();
                var employee_ids = db.group.Where(m => m.active == true).Select(m => m.supervisor_id).ToList();
                var employees = db.employee.Where(m => employee_ids.Contains(m.EmployeeId)).ToList();

                foreach (var entity in employees)
                {
                    if (entity.Group != null)
                    {

                        if (entity.Group.GroupCalendars != null && entity.Group.follows_general_calendar == false)
                        {
                            toReturn.employeeFollowGroupCalander.Add(new Employee
                            {
                                first_name = entity.first_name,
                                last_name = entity.last_name,
                                employee_code = entity.last_name,
                                id = entity.EmployeeId,
                                group_id = entity.Group.GroupId,
                                group_name = entity.Group.group_name
                            });
                        }
                        else
                        {
                            toReturn.employeeFollowGeneralCalander.Add(new Employee
                            {
                                first_name = entity.first_name,
                                last_name = entity.last_name,
                                employee_code = entity.last_name
                            });
                        }
                    }
                    else
                    {
                        toReturn.employeeFollowGeneralCalander.Add(new Employee
                        {
                            first_name = entity.first_name,
                            last_name = entity.last_name,
                            employee_code = entity.last_name
                        });
                    }
                }
            }
            return toReturn;
        }



        #endregion



        #region Services
        public static int getAllServies(string search, string sortOrder, int start, int length, out List<BLL.ViewModels.ServicesViewModel> toReturn)
        {
            int count = 0;
            toReturn = new List<ViewModels.ServicesViewModel>();
            try
            {
                count = BLL.Services.TimeTuneServices.getServices()
                    .Where(p => search == null ||
                        search.Equals("") ||
                        p.serviceName.Contains(search.ToLower()) ||
                         BLL.Services.TimeTuneServices.getServices().Where(e => e.serviceName.Equals(p.serviceName)).FirstOrDefault().serviceName.Contains(search.ToLower()) ||
                         BLL.Services.TimeTuneServices.getServices().Where(e => e.status.Equals(p.status)).FirstOrDefault().status.Contains(search.ToLower())

                        ).Count();

                List<ViewModels.ServicesViewModel> temp = BLL.Services.TimeTuneServices.getServices().AsQueryable()
                    .Where(p =>
                        search == null ||
                        search.Equals("") ||
                        p.serviceName.Contains(search.ToLower()) ||
                        BLL.Services.TimeTuneServices.getServices().Where(e => e.serviceName.Equals(p.serviceName)).FirstOrDefault().serviceName.Contains(search.ToLower()) ||
                        BLL.Services.TimeTuneServices.getServices().Where(e => e.status.Equals(p.status)).FirstOrDefault().status.Contains(search.ToLower())
                        )
                    .Select(p =>
                        new ViewModels.ServicesViewModel()
                        {
                            serviceName = p.serviceName,
                            Description = p.Description,
                            status = p.status
                        }).SortBy(sortOrder).Skip(start).Take(length).ToList();
                int i = 0;

                foreach (var log in temp)
                {

                    toReturn.Add(new ViewModels.ServicesViewModel()
                    {

                        serviceName = log.serviceName,
                        Description = log.Description,
                        status = log.status,
                        on_action =
                       @"<div data-id='" + log.serviceName + @"'>
                                <button id ='serviceStart" + i + "' class='btn' onclick='startService(" + i + ")' value='" + log.serviceName + "' '><i class='glyphicon glyphicon-play'></i></button></div>",
                        off_action =
                                        @"<div data-id='" + log.serviceName + @"'>
                                <button id='serviceStop" + i + "' class='btn' onclick='stopService(" + i + ")' value='" + log.serviceName + "'><i class='glyphicon glyphicon-stop'></i></button></div>"
                    });
                    i++;

                }
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                toReturn = new List<ViewModels.ServicesViewModel>();
            }


            return count;

        }

        public static void startService(string name)
        {
            service.StartService(name, 1000);
        }
        public static void stopService(string name)
        {
            service.StopService(name, 1000);
        }

        #endregion






        #region Contractual Staff
        public static int getAllContratualStaff(string search, string sortOrder, int start, int length, out List<ViewModels.VM_ContractualStaff> toReturn)
        {
            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                int count = 0;


                toReturn = new List<ViewModels.VM_ContractualStaff>();
                try
                {
                    count = db.contractual_staff
                        .Where(p => search == null ||
                            search.Equals("") ||
                            p.employee_name.Contains(search.ToLower()) || p.employee_code.Contains(search.ToLower()) || p.department.Contains(search.ToLower()) ||
                            p.designation.Contains(search.ToLower()) || p.email.Contains(search.ToLower()) || p.location.Contains(search.ToLower()) ||
                             p.region.Contains(search.ToLower()) || p.company.Contains(search.ToLower()) ||
                            db.contractual_staff.Where(e => e.employee_name.Equals(p.employee_name)).FirstOrDefault().employee_name.Contains(search.ToLower())

                            ).Count();

                    List<ViewModels.VM_ContractualStaff> temp = db.contractual_staff
                        .Where(p =>
                            search == null ||
                            search.Equals("") ||
                            p.employee_name.Contains(search.ToLower()) || p.employee_code.Contains(search.ToLower()) || p.department.Contains(search.ToLower()) ||
                            p.designation.Contains(search.ToLower()) || p.email.Contains(search.ToLower()) || p.location.Contains(search.ToLower()) ||
                             p.region.Contains(search.ToLower()) || p.company.Contains(search.ToLower()) ||
                            db.contractual_staff.Where(e => e.employee_name.Equals(p.employee_name)).FirstOrDefault().employee_name.Contains(search.ToLower())

                            )
                        .Select(p =>
                            new ViewModels.VM_ContractualStaff()
                            {
                                ContractualStaffId = p.ContractualStaffId,
                                employee_name = p.employee_name,
                                email = p.email,
                                employee_code = p.employee_code,
                                location = p.location,
                                region = p.region,
                                grade = p.grade,
                                Group = p.Group,
                                function = p.function,
                                company = p.company,
                                department = p.department,
                                designation = p.designation,
                                mobile_no = p.mobile_no,
                                address = p.address
                            })
                        .SortBy(sortOrder).Skip(start).Take(length).ToList();


                    foreach (var log in temp)
                    {

                        toReturn.Add(new ViewModels.VM_ContractualStaff()
                        {
                            ContractualStaffId = log.ContractualStaffId,
                            employee_name = log.employee_name,
                            email = log.email,
                            employee_code = log.employee_code,
                            location = log.location,
                            region = log.region,
                            grade = log.grade,
                            Group = log.Group,
                            function = log.function,
                            company = log.company,
                            department = log.department,
                            designation = log.designation,
                            mobile_no = log.mobile_no,
                            address = log.address
                        });

                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.VM_ContractualStaff>();
                }


                return count;
            }
        }
        public static VM_ContractualStaff getContractualStaff(string employee_code)
        {
            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                VM_ContractualStaff toReturn = new VM_ContractualStaff();
                try
                {
                    var contractualStaff = db.contractual_staff.Where(c => c.employee_code == employee_code && c.active == true).FirstOrDefault();
                    toReturn.address = contractualStaff.address;
                    toReturn.company = contractualStaff.company;
                    toReturn.ContractualStaffId = contractualStaff.ContractualStaffId;
                    toReturn.date_of_joining = contractualStaff.date_of_joining.Value.ToString("dd-MM-yyyy");
                    toReturn.date_of_leaving = (contractualStaff.date_of_leaving.HasValue) ? contractualStaff.date_of_leaving.Value.ToString("dd-MM-yyyy") : "";
                    toReturn.department = contractualStaff.department;
                    toReturn.designation = contractualStaff.designation;
                    toReturn.email = contractualStaff.email;
                    toReturn.employee_code = contractualStaff.employee_code;
                    toReturn.employee_name = contractualStaff.employee_name;
                    toReturn.function = contractualStaff.function;
                    toReturn.grade = contractualStaff.grade;
                    toReturn.Group = contractualStaff.Group;
                    toReturn.location = contractualStaff.location;
                    toReturn.mobile_no = contractualStaff.mobile_no;
                    toReturn.region = contractualStaff.region;


                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new ViewModels.VM_ContractualStaff();
                }


                return toReturn;
            }
        }

        public static string addEmployee(BLL.ViewModels.VM_ContractualStaff emp)
        {
            using (DLL.Models.Context db = new DLL.Models.Context())
            {
                try
                {
                    DLL.Models.ContractualStaff employee = new DLL.Models.ContractualStaff();
                    var getEmp = db.employee.Where(c => c.employee_code == emp.employee_code && c.active == true).FirstOrDefault();
                    if (getEmp == null)
                    {

                        employee.employee_name = emp.employee_name;
                        employee.employee_code = emp.employee_code;
                        employee.employee_code = emp.employee_code;
                        employee.email = emp.email;
                        employee.address = emp.address;
                        employee.location = emp.location;
                        employee.region = emp.region;
                        employee.company = emp.company;
                        employee.function = emp.function;
                        employee.department = emp.department;
                        employee.designation = emp.designation;
                        employee.grade = emp.grade;
                        employee.Group = emp.Group;
                        employee.active = true;
                        employee.mobile_no = emp.mobile_no;
                        employee.date_of_joining = (emp.date_of_joining != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_joining, "d-M-yyyy", CultureInfo.InvariantCulture) : null;
                        employee.date_of_leaving = (emp.date_of_leaving != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_leaving, "d-M-yyyy", CultureInfo.InvariantCulture) : null;
                        employee.persistent_log = new DLL.Models.CS_AttendanceLog()
                        {
                            employee_code = emp.employee_code,
                            date = null,
                            dirtyBit = false,
                            active = true,
                            terminal_in = null,
                            terminal_out = null,
                            time_in = null,
                            time_out = null
                        };
                    }
                    else
                    {
                        string toReturn = "Employee Id Already Existing";
                        return toReturn;
                    }
                    db.contractual_staff.Add(employee);
                    if (db.SaveChanges() > 0)
                    {
                        string toReturn = "";
                        return toReturn;
                    }
                    else
                    {
                        string toReturn = "unable to save employee";
                        return toReturn;
                    }
                }
                catch (Exception ex)
                {
                    string toReturn = "Unable to save employee";
                    return toReturn;
                }
            }
        }

        public static string editEmployee(VM_ContractualStaff emp)
        {
            string toReturn = "";
            int employee_id = int.Parse(emp.employee_code);
            using (var db = new DLL.Models.Context())
            {
                var employee = db.contractual_staff.Where(c => c.ContractualStaffId.Equals(employee_id)).FirstOrDefault();
                employee.employee_name = emp.employee_name;
                employee.email = emp.email;
                employee.designation = emp.designation;
                employee.department = emp.department;
                employee.company = emp.company;
                employee.address = emp.address;
                employee.date_of_joining = (emp.date_of_joining != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_joining, "d-M-yyyy", CultureInfo.InvariantCulture) : null;
                employee.date_of_leaving = (emp.date_of_leaving != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_leaving, "d-M-yyyy", CultureInfo.InvariantCulture) : null;
                employee.function = emp.function;
                employee.location = emp.location;
                employee.region = emp.region;
                employee.grade = emp.grade;
                employee.Group = emp.Group;
                if (db.SaveChanges() > 0)
                {
                    toReturn = "Edit Successfully";
                }
                else
                {
                    toReturn = "Unable to edit";
                }
            }
            return toReturn;
        }
        #endregion

        #region ChosenDataHandler

        public static Employee[] getAllNonHrsEmployeesMatching(string subString)
        {
            using (var db = new DLL.Models.Context())
            {
                // get all employees which are not HRs
                DLL.Models.Employee[] employees = db.employee.Where(m =>
                    m.active &&
                    !m.access_group.name.Equals(TimeTuneRoles.ROLE_HR) &&
                    (
                    m.employee_code.ToLower().Contains(subString) ||
                    m.first_name.ToLower().Contains(subString) ||
                    m.last_name.ToLower().Contains(subString)
                    )).ToArray();

                Employee[] toReturn = new Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                }

                return toReturn;
            }
        }

        public static Employee[] getAllNonHrsEmployeeCodesMatching(string subString)
        {
            using (var db = new DLL.Models.Context())
            {
                // get all employees which are not HRs
                DLL.Models.Employee[] employees = db.employee.Where(m =>
                    m.active &&
                    !m.access_group.name.Equals(TimeTuneRoles.ROLE_HR) &&
                    (
                    m.employee_code.ToLower().Contains(subString) ||
                    m.first_name.ToLower().Contains(subString) ||
                    m.last_name.ToLower().Contains(subString)
                    )).ToArray();

                Employee[] toReturn = new Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                }

                return toReturn;
            }
        }

        #endregion

    }
}
