using DLL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTune
{
    public class ManageGeoPhencingEmployeeImportExport
    {
        public class ManageGeoPhencingEmployeeCSV
        {
            public string employeeCode { get; set; }
            public string branchesList { get; set; }
            public string terminalsList { get; set; }
        }

        public class GeoPhencingEmpExportTable
        {
            public string employee_code { get; set; }
            public string branchesList { get; set; }
            public string terminalsList { get; set; }
        }
        public static List<ManageGeoPhencingEmployeeCSV> getManageGeoPhencingEmployeeCSV()
        {
            List<ManageGeoPhencingEmployeeCSV> toReturn;
            using (var db = new Context())
            {

                try
                {

                    toReturn =
                        db.employee.Where(m => m.active).Select(
                        p => new ManageGeoPhencingEmployeeCSV()
                        {
                            employeeCode = "<" + p.employee_code + ">",
                            branchesList = (p.first_name == null) ? "<->" : p.first_name

                        }).ToList();


                    // convert dates to a more processable form.
                    foreach (var value in toReturn)
                    {
                        if (value.employeeCode == null)
                        {
                            value.employeeCode = "";
                        }

                        if (value.branchesList == null)
                        {
                            value.branchesList = "";
                        }

                        if (value.terminalsList == null)
                        {
                            value.terminalsList = "";
                        }
                    }

                    /*string sql =
                         "select\n" +
                          "[first_name]\n" +
                          ",[last_name]\n" +
                          ",[employee_code]\n" +
                          ",[email]\n" +
                          ",[address]\n" +
                          ",[mobile_no]\n" +
                          ",[date_of_joining]\n" +
                          ",[date_of_leaving]\n" +
                          ",COALESCE([access_group_AccessGroupId], 0) as [access_group_AccessGroupId]\n" +
                          ",COALESCE([department_DepartmentId], 0) as [department_DepartmentId]\n" +
                          ",COALESCE([designation_DesignationId], 0) as [designation_DesignationId]\n" +
                          ",COALESCE([function_FunctionId], 0) as [function_FunctionId]\n" +
                          ",COALESCE([grade_GradeId], 0) as [grade_GradeId]\n" +
                          ",COALESCE([Group_GroupId], 0) as [Group_GroupId]\n" +
                          ",COALESCE([location_LocationId], 0) as [location_LocationId]\n" +
                          ",COALESCE([region_RegionId], 0) as [region_RegionId]\n" +
                          ",COALESCE([type_of_employment_TypeOfEmploymentId], 0) as [type_of_employment_TypeOfEmploymentId]\n" +
                           ",[timetune_active]\n" +
                        "from\n" +
                        "Employees where [active]=1 \n";*/

                    //var results = db.Database.SqlQuery<EmpExportTable>(sql).ToList();


                    /*

                    int count = results.Count();

                    toReturn = results.Select(
                        p => new ManageGeoPhencingEmployeeCSV()
                        {
                            employeeCode=p.employee_code,
                            firstName = (p.first_name == null) ? "" : p.first_name,
                            lastName=(p.last_name == null) ? "" : p.last_name,
                            email=(p.email == null) ? "" : p.email,
                            address=(p.address==null)?"":p.address,
                            mobileNo = (p.mobile_no == null) ? "" : p.mobile_no,
                            dateOfJoining=p.date_of_joining.ToString(),
                            dateOfLeaving=p.date_of_leaving.ToString(),
                            //accessGroupID=p.access_group_AccessGroupId,
                           departmentID=p.department_DepartmentId,
                           designationID=p.designation_DesignationId,
                           functionID=p.function_FunctionId,
                           gradeID=p.grade_GradeId,
                           //groupID=p.Group_GroupId,
                           locationID=p.location_LocationId,
                           regionID=p.region_RegionId,
                           typeOfEmploymentID=p.type_of_employment_TypeOfEmploymentId,
                           active=p.active
                        }).ToList();
                     * 
                     * */
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ManageGeoPhencingEmployeeCSV>();
                }

            }

            return toReturn;
        }

        private static bool convertStringToBool(string value)
        {
            return (value == "True" || value == "true" || value == "TRUE");
        }
        public static string setGeoPhencingEmployees(List<string> csvContents, string user_code)
        {
            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 2 values (columns).

                        if (values == null || values.Length < 2)
                            continue;

                        // Remove unwanted " characters.
                        string employeeCode = values[0].Replace("\"", "");
                        string branchesList = values[1].Replace("\"", "");

                        // continue if its the first row (CSV Header line).
                        if (employeeCode == "employee_code" || branchesList == "branches_codes")
                        {
                            continue;
                        }

                        //Get Branches and Terminal Names
                        string lstBranches = "^", lstTerminals = "^";
                        if (branchesList != null && branchesList != "")
                        {
                            string[] arrBranches = branchesList.Split('-');
                            if (arrBranches.Length > 0)
                            {
                                for (int z = 0; z < arrBranches.Length; z++)
                                {
                                    string b = arrBranches[z];
                                    int bCount = 0;
                                    var TerminalCodes = db.termainal.Where(t => t.terminal_id == b).ToList();
                                    if (TerminalCodes != null && TerminalCodes.Count > 0)
                                    {
                                        foreach (var tList in TerminalCodes)
                                        {
                                            if (bCount == 0)
                                            {
                                                if (tList.C_Name.Contains(":"))
                                                    lstBranches += b + "|" + tList.C_Name.Split(':')[0] + "^";
                                                //else
                                                //    lstBranches += "-|-^";
                                            }

                                            lstTerminals += tList.L_ID.ToString() + "|" + tList.branch_name + ":" + tList.C_Name.ToLower() + "^";
                                            bCount++;
                                        }

                                        bCount = 0;
                                    }
                                    else
                                    {
                                        //lstBranches += "-|-^";
                                        //lstTerminals += "-|-^";
                                    }

                                }
                            }
                        }

                        // Get the employee 
                        Employee employee = db.employee.Where(m => m.employee_code.Equals(employeeCode)).FirstOrDefault();
                        if (employee != null)
                        {
                            GeoPhencingTerminal gpTermEmp = db.geo_phencing_terminal.Where(g => g.EmployeeId == employee.EmployeeId).FirstOrDefault();
                            if (gpTermEmp != null)
                            {
                                gpTermEmp.EmployeeId = employee.EmployeeId;
                                gpTermEmp.BranchesList = lstBranches;
                                gpTermEmp.TerminalsList = lstTerminals;

                                db.SaveChanges();

                                TimeTune.AuditTrail.update("{\"*id\" : \"" + employee.EmployeeId.ToString() + "\"}", "GeoPhencing", user_code);
                            }
                            else //new gp-employee
                            {
                                GeoPhencingTerminal gpTerminals = new GeoPhencingTerminal()
                                {
                                    EmployeeId = employee.EmployeeId,
                                    BranchesList = lstBranches,
                                    TerminalsList = lstTerminals,
                                    TrmCreateDate = DateTime.Now
                                };

                                db.geo_phencing_terminal.Add(gpTerminals);
                                db.SaveChanges();

                                TimeTune.AuditTrail.insert("{\"*id\" : \"" + employee.EmployeeId.ToString() + "\"}", "GeoPhencing", user_code);
                            }
                        }
                        else
                        {
                            return "failed";
                        }
                    }

                    return "Successfull";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }



        public static bool validateEmployeeCode(string strEmpCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strEmpCode = strEmpCode.ToLower();

                    var dbEmp = db.employee.Where(c => c.employee_code == strEmpCode).FirstOrDefault();
                    if (dbEmp != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateBranchCode(string strBranchCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strBranchCode = strBranchCode.ToLower();

                    var dbBranches = db.termainal.Where(c => c.terminal_id == strBranchCode).FirstOrDefault();
                    if (dbBranches != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static List<ViewModels.TerminalView> GetBranchesList()
        {
            List<ViewModels.TerminalView> lstTerminals = new List<ViewModels.TerminalView>();

            using (var db = new Context())
            {
                try
                {
                    var dataBranches = db.Database
                   .SqlQuery<ViewModels.Term_Group>("SP_GetBranchesUsingGroupBy").ToList();

                    if (dataBranches != null && dataBranches.Count > 0)
                    {
                        foreach (var t in dataBranches)
                        {
                            lstTerminals.Add(new ViewModels.TerminalView()
                            {
                                Branch_Code = t.branch_code,
                                Branch_Name = t.branch_name
                            });
                        }
                    }


                    //var dbTerminals = from e in db.termainal
                    //            group e by new { e.terminal_id, e.C_Name } into eg
                    //            select new { eg.Key.terminal_id, eg.Key.C_Name };

                    ////var dbTerminals = db.termainal.GroupBy(t => t.terminal_id).Select(y=> new { terID= y.Key, name=y.if] }).ToList();
                    //if (dbTerminals != null && dbTerminals.Any())
                    //{
                    //    foreach (var t in dbTerminals)
                    //    {
                    //        lstTerminals.Add(new ViewModels.TerminalView()
                    //        {
                    //            Terminal_id = int.Parse(t.terminal_id.ToString()),
                    //            C_Name = t.C_Name.Split(':')[0]
                    //        });
                    //    }
                    //}
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {

                }
            }

            return lstTerminals;
        }





    }
}
