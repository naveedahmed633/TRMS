using DLL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTune
{
    /**
     * This class manages the export and import of the line manager settings of the
     * application through the exchange of csv files.
     **/
    public class LineManagersExportImport
    {

        public class LineManagersCSV
        {
            public string personalNumber { get; set; }
            public string supervisorID { get; set; }
        }

        public class LMExportTable
        {
            public string si { get; set; } // Supervisor Id
            public string pn { get; set; } // Personal Number
        }
        public static List<LineManagersCSV> getLineManagersCSV()
        {
            List<LineManagersCSV> toReturn;
            using (var db = new Context())
            {

                try
                {


                    string sql =
                        "select\n" +
                        "emps.employee_code [pn],\n" +
                        "lms.employee_code [si]\n" +
                        "from\n" +
                        "Employees emps,\n" +
                        "Groups grp,\n" +
                        "Employees lms\n" +
                        "where\n" +
                        "emps.active = 1 and\n" +
                        "emps.Group_GroupId = grp.GroupId and\n" +
                        "grp.supervisor_id = lms.EmployeeId and\n" +
                        "grp.active = 1\n" +
                        "order by\n" +
                        "lms.employee_code,emps.employee_code\n";

                    var results = db.Database.SqlQuery<LMExportTable>(sql).ToList();

                    int count = results.Count();

                    toReturn = results.Select(
                        p => new LineManagersCSV()
                        {
                            personalNumber = p.pn,
                            supervisorID = p.si

                        }).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<LineManagersCSV>();
                }

            }

            return toReturn;
        }

        public static void setLineManagers(List<string> csvContents, string user_code)
        {

            Group grp = null;
            using (var db = new Context())
            {

                foreach (string line in csvContents)
                {
                    // split the line
                    string[] values = line.Split(',');

                    // check if the split resulted in at least 2
                    // values: personalNum, supervisorID
                    if (values == null || values.Length < 2)
                        continue;

                    // Remove unwanted " characters.
                    string employeeCode = values[0].Replace("\"", "");
                    string lineManagerCode = values[1].Replace("\"", "");

                    // Get the employee and the specified line manager
                    Employee employee = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(employeeCode) &&
                        m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_EMP)).FirstOrDefault();

                    Employee lineManager = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(lineManagerCode) &&
                        m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_LM)).FirstOrDefault();

                    // If the line manager and the employee both exist.
                    if (employee != null && lineManager != null)
                    {

                        // Basic check: If the employee is already under the specified line
                        // manager, then do nothing.
                        if (employee.Group != null && employee.Group.supervisor_id == lineManager.EmployeeId && employee.Group.active == true)
                            continue;



                        // If the control reaches here we know that the employee has to be moved
                        // under the specified line manager - group. So, first check if the line manager has
                        // a group, if not, then craete a group for the line manager.
                        if (lineManager.Group == null)
                        {
                            grp = new Group()
                            {
                                group_name = lineManager.employee_code,
                                Employees = new List<Employee>(),
                                supervisor_id = lineManager.EmployeeId,
                                follows_general_calendar = true,
                                active = true
                            };
                            grp.Employees.Add(lineManager);
                            db.group.Add(grp);
                            db.SaveChanges();
                        }
                        else
                        {
                            grp = lineManager.Group;
                        }


                        // Now check if the employee is already in some group.
                        // If he/she is, then remove from the group that employee,
                        // and add to this group.
                        if (employee.Group != null)
                        {
                            employee.Group.Employees.Remove(employee);
                            employee.Group = null;

                        }

                        grp.Employees.Add(employee);
                        employee.Group = grp;

                        TimeTune.AuditTrail.insert("{\"*id\": \"" + employee.EmployeeId + "\", \"*lm_id\" : \"" + lineManager.EmployeeId.ToString() + "\"}", "LM", user_code);
                    }



                }
                db.SaveChanges();

            }



        }
    }

    /**
     * This class manages the export and import of the super line manager settings of the
     * application through the exchange of csv files.
     **/
    public class SuperLineManagersExportImport
    {

        public class SuperLineManagersCSV
        {
            public string personalNumber { get; set; }
            public string superLineManagerID { get; set; }
        }

        public class SLMExportTable
        {
            public string slmi { get; set; } // Supe line manager Id
            public string pn { get; set; } // Personal Number
        }
        public static List<SuperLineManagersCSV> getLineManagersCSV()
        {
            List<SuperLineManagersCSV> toReturn;
            using (var db = new Context())
            {

                try
                {

                    string sql =
                         "select\n" +
                         "slm.employee_code [slmi],\n" +
                         "emps.employee_code [pn]\n" +
                         "from\n" +
                         "Employees emps,\n" +
                         "SLMs grp,\n" +
                         "Employees slm\n" +
                         "where\n" +
                         "emps.active = 1 and\n" +
                         "slm.EmployeeId = grp.superLineManager_EmployeeId and\n" +
                         "grp.taggedEmployee_EmployeeId = emps.EmployeeId\n" +
                         "order by\n" +
                         "slm.employee_code, emps.employee_code";

                    //string sql =
                    //    "select\n" +
                    //    "slm.employee_code [slmi],\n" +
                    //    "emps.employee_code [pn]\n" +
                    //    "from\n" +
                    //    "Employees emps,\n" +
                    //    "SLMs grp,\n" +
                    //    "Employees slm\n" +
                    //    "where\n" +
                    //    "emps.active = 1 and\n" +
                    //    "slm.EmployeeId = grp.superLineManager_EmployeeId and\n" +
                    //    "grp.taggedEmployee_EmployeeId = emps.EmployeeId\n" +
                    //    "order by\n" +
                    //    "emps.employee_code";

                    var results = db.Database.SqlQuery<SLMExportTable>(sql).ToList();

                    int count = results.Count();

                    toReturn = results.Select(
                        p => new SuperLineManagersCSV()
                        {
                            personalNumber = p.pn,
                            superLineManagerID = p.slmi

                        }).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<SuperLineManagersCSV>();
                }

            }

            return toReturn;
        }



        public static void setSuperLineManagers(List<string> csvContents, string user_code)
        {
            // Remove all the SLM taggings, because the new taggings
            // will be made as per the uploaded file in the 
            // next context.
            using (var db = new Context())
            {
                db.super_line_manager_tagging.RemoveRange(db.super_line_manager_tagging.ToArray());
                db.SaveChanges();
            }

            using (var db = new Context())
            {
                foreach (string line in csvContents)
                {
                    // split the line
                    string[] values = line.Split(',');

                    // check if the split resulted in at least 2
                    // values: personalNum, supervisorID
                    if (values == null || values.Length < 2)
                        continue;

                    // Remove unwanted " characters.
                    string employeeCode = values[0].Replace("\"", "");
                    string lineManagerCode = values[1].Replace("\"", "");

                    // Get the employee and the specified line manager.
                    // Notice that unlike line manager, we do not check
                    // the type of the employee, because any type of 
                    // employee can be set under a super line manager,
                    // even HR's and SLMs.
                    Employee employee = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(employeeCode)).FirstOrDefault();

                    Employee superLineManager = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(lineManagerCode) &&
                        m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_LM)).FirstOrDefault();//IR editted

                    // If the line manager and the employee both exist.
                    if (employee != null && superLineManager != null)
                    {
                        SLM alreadyExists = db.super_line_manager_tagging.Where(m =>
                            m.superLineManager.EmployeeId.Equals(superLineManager.EmployeeId) &&
                            m.taggedEmployee.EmployeeId.Equals(employee.EmployeeId)).FirstOrDefault();

                        if (alreadyExists == null)
                        {
                            db.super_line_manager_tagging.Add(new SLM()
                            {
                                superLineManager = superLineManager,
                                taggedEmployee = employee
                            });
                        }

                        TimeTune.AuditTrail.insert("{\"*id\": \"" + employee.EmployeeId + "\", \"*slm_id\" : \"" + superLineManager.EmployeeId.ToString() + "\"}", "SLM", user_code);
                    }


                }

                db.SaveChanges();
            }



        }
    }
}