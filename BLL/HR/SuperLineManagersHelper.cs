using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DLL.Models;
using System.Dynamic;

namespace TimeTune
{
    public class SuperLineManagersHelper
    {
        public static int getAllSLMsMatching(string search, string sortOrder, int start, int length, out List<ViewModels.SuperLineManagersTableViewModel> toReturn)
        {
            int count = 0;

            using (Context db = new Context())
            {
                toReturn = null;
                try
                {

                    if (search != null && !search.Equals(""))
                    {
                        // query the DB for count.
                        count = db.employee.Where(p =>
                            p.active &&
                            p.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM) && //changed by IR from SLM to LM
                            (
                            p.first_name.ToString().ToLower().Contains(search.ToLower()) ||
                            p.last_name.ToString().ToLower().Contains(search.ToLower()) ||

                            p.employee_code.ToString().ToLower().Contains(search.ToLower())

                            )
                            ).Count();


                        // load actual content.
                        toReturn = db.employee.Where(p =>

                            p.active &&
                            p.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM) && //changed by IR from SLM to LM
                            (
                            p.first_name.ToString().ToLower().Contains(search.ToLower()) ||
                            p.last_name.ToString().ToLower().Contains(search.ToLower()) ||

                            p.employee_code.ToString().ToLower().Contains(search.ToLower())

                            )
                            )
                        .Select(p =>
                            new ViewModels.SuperLineManagersTableViewModel()
                            {
                                slm_employee_code = p.employee_code,
                                slm_name = p.first_name + " " + p.last_name,
                                action =
                                @"<div data-id='" + p.EmployeeId + @"'>
                                    <a href='javascript:void(editSLM(" + p.EmployeeId + @"));'>Edit</a> / <a href='javascript:void(deleteSLM(" + p.EmployeeId + @"));'>Delete</a>
                                </div>"
                            })
                        .SortBy(sortOrder).Skip(start).Take(length).ToList();

                        // id, 

                    }
                    else
                    {
                        // query the db for count
                        count = db.employee.Where(p =>

                            p.active &&
                            p.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM)//changed by IR from SLM to LM
                            ).Count();

                        // load actual data.
                        toReturn = db.employee.Where(p =>
                            p.active &&
                            p.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM)//changed by IR from SLM to LM
                            )
                            .Select(p =>
                            new ViewModels.SuperLineManagersTableViewModel()
                            {
                                slm_employee_code = p.employee_code,
                                slm_name = p.first_name + " " + p.last_name,
                                action =
                                @"<span data-id='" + p.EmployeeId + @"'>
                                    <a href='javascript:void(editSLM(" + p.EmployeeId + @"));'>Edit</a> / <a href='javascript:void(deleteSLM(" + p.EmployeeId + @"));'>Delete</a>
                                </span>"
                            })
                        .SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                    if (toReturn != null && toReturn.Count > 0)
                    {
                        foreach (ViewModels.SuperLineManagersTableViewModel p in toReturn)
                        {
                            var data_slm = db.super_line_manager_tagging.Where(s => s.superLineManager.employee_code == p.slm_employee_code).FirstOrDefault();
                            if (data_slm == null)
                            {
                                p.slm_employee_code = "";
                                count--;
                            }
                        }

                        toReturn.RemoveAll(r => r.slm_employee_code == "");
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.SuperLineManagersTableViewModel>();
                }




                return count;
            }
        }

        public static ViewModels.Employee[] getAllLineManagersMatchingForSLM(string subString)
        {
            ViewModels.Employee[] toReturn = new ViewModels.Employee[1];

            using (var db = new Context())
            {
                // get all line managers
                List<Employee> employees = db.employee.Where(m => m.active &&
                    m.employee_code.ToLower().Contains(subString) &&
                    m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_LM)).ToList();

                //get SLMs only
                int[] alreadySuperLineManagerIds = db.super_line_manager_tagging.Select(m => m.superLineManager.EmployeeId).ToArray();
                if (alreadySuperLineManagerIds.Length > 0)
                {
                    for (int i = 0; i < alreadySuperLineManagerIds.Length; i++)
                    {
                        for (int j = 0; j < employees.Count; j++)
                        {
                            if (employees[j].EmployeeId == alreadySuperLineManagerIds[i])
                            {
                                employees[j].employee_code = null;
                            }
                        }
                    }
                }

                //get LMs only
                int[] alreadyLineManagerIds = db.group.Where(m => m.active).Select(m => m.supervisor_id).ToArray();
                if (alreadyLineManagerIds.Length > 0)
                {
                    for (int i = 0; i < alreadyLineManagerIds.Length; i++)
                    {
                        for (int j = 0; j < employees.Count; j++)
                        {
                            if (employees[j].EmployeeId == alreadyLineManagerIds[i])
                            {
                                employees[j].employee_code = null;
                            }
                        }
                    }
                }

                //finalize
                Employee[] emps = employees.Where(m => m.employee_code != null).ToArray();
                if (emps.Length > 0)
                {
                    toReturn = new ViewModels.Employee[emps.Length];

                    for (int i = 0; i < emps.Length; i++)
                    {
                        toReturn[i] = EmployeeCrud.convert(emps[i]);
                    }
                }
            }

            return toReturn;
        }



        public static ViewModels.Employee[] getAllEmployeesForSLMWhereGroupIsNull(string subString)
        {
            ViewModels.Employee[] toReturn = new ViewModels.Employee[1];

            using (var db = new Context())
            {
                //get all active employees
                Employee[] employees = db.employee.Where(m => m.active && m.employee_code.ToLower().Contains(subString)).ToArray();
                if (employees.Length > 0)
                {
                    toReturn = new ViewModels.Employee[employees.Length];
                    for (int i = 0; i < employees.Length; i++)
                    {
                        toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                    }
                }
            }

            return toReturn;
        }

        public static string createSLMGroup(ViewModels.CreateSLMGroup fromForm)
        {
            int super_id = 0, under_id = 0;

            if (fromForm.super_line_manager == null)
            {
                return "super-line-manager cannot be null.";
            }

            if (int.TryParse(fromForm.super_line_manager, out super_id))
            {
                //successful
            }

            using (var db = new Context())
            {
                var dbEmployees = db.employee.Where(e => e.active).ToList();
                if (super_id > 0 && dbEmployees != null && dbEmployees.Count > 0)
                {
                    var data_super_emp = dbEmployees.Where(e => e.EmployeeId == super_id && e.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_LM)).FirstOrDefault();
                    if (data_super_emp != null)
                    {
                        // check if the employees are already in some group.
                        List<Employee> groupEmployees = new List<Employee>();

                        if (fromForm.group_employees != null)
                        {
                            for (int i = 0; i < fromForm.group_employees.Count; i++)
                            {
                                under_id = int.Parse(fromForm.group_employees[i]);

                                var data_under_emp = dbEmployees.Where(e => e.EmployeeId == under_id).FirstOrDefault();
                                if (data_under_emp != null)
                                {
                                    db.super_line_manager_tagging.Add(new SLM()
                                    {
                                        superLineManager = data_super_emp,
                                        taggedEmployee = data_under_emp
                                    });

                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    // if none of that is true, create the group, else just return quitely.
                }
            }

            return null;
        }


        public static dynamic getSLM(int SLMId)
        {
            dynamic obj = new
            {
                success = false,
                slm_employees = new[] {
                    new {id = "0" , text = ""},
                    new {id = "1" , text = ""},
                }
            };


            using (var db = new Context())
            {
                Employee requestedSLM = db.employee.Where(m =>
                    m.active &&
                    m.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM) &&//changed by IR from SLM to LM
                    m.EmployeeId.Equals(SLMId)).FirstOrDefault();




                if (requestedSLM != null)
                {

                    //3
                    dynamic[] employees =

                        db.super_line_manager_tagging.Where(m =>
                            m.superLineManager.EmployeeId.Equals(requestedSLM.EmployeeId) &&
                            m.taggedEmployee.active // &&

                            // do not show slms own code to the slm.
                            //!m.taggedEmployee.employee_code.Equals(requestedSLM.employee_code)
                            )
                        .Select(p =>
                        new
                        {
                            employee_code = p.taggedEmployee.employee_code,
                            EmployeeId = p.taggedEmployee.EmployeeId
                        }
                        ).ToArray();


                    List<dynamic> SLMEmployees = new List<dynamic>();
                    for (int i = 0; i < employees.Length; i++)
                    {
                        SLMEmployees.Add(new
                        {
                            id = employees[i].EmployeeId + "",
                            text = employees[i].employee_code
                        });
                    }



                    obj = new
                    {
                        success = true,
                        slm_employees = SLMEmployees

                    };



                }

            }


            return obj;
        }


        // this function will return null on success, or it will return
        // an error message on error.
        public static dynamic editSLM(ViewModels.EditSLMViewModel fromForm)
        {
            int SLMId;

            if (!int.TryParse(fromForm.slm_id, out SLMId))
            {
                return new { error = "Invalid SLM id." };
            }

            // Using a seperate context to first remove all the
            // SLM mapping from the db.
            using (var db = new Context())
            {
                Employee theSLMEmployee = db.employee.Where(m =>
                    m.active &&
                    m.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM) &&//changed by IR from SLM to LM
                    m.EmployeeId.Equals(SLMId)).FirstOrDefault();


                if (theSLMEmployee != null)
                {
                    // remove all the employees tagged for this 
                    // super line manager.
                    db.super_line_manager_tagging.RemoveRange(
                        db.super_line_manager_tagging.Where(p =>
                        p.superLineManager.EmployeeId.Equals(SLMId)).ToArray()
                    );
                }

                db.SaveChanges();
            }


            using (var db = new Context())
            {
                Employee theSLMEmployee = db.employee.Where(m =>
                    m.active &&
                    m.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM) &&//changed by IR from SLM to LM
                    m.EmployeeId.Equals(SLMId)).FirstOrDefault();

                if (theSLMEmployee != null)
                {

                    List<SLM> tags = new List<SLM>();


                    // before we go in the loop and iterate over 
                    // 'fromForm.slm_employees'. We'll test for null
                    // and return if the test is affirmative. The 
                    // 'fromForm.slm_employees' is null when there are
                    // no tags in the tagged employees tag box. This
                    // was figured out when the application threw a null pointer
                    // exception on submission of an empty form.
                    if (fromForm.slm_employees == null)
                        return new { error = "" };


                    for (int i = 0; i < fromForm.slm_employees.Count; i++)
                    {
                        int temp;

                        if (int.TryParse(fromForm.slm_employees[i], out temp))
                        {

                            // Check if this employee is already tagged in some other
                            // super line manager then do not add him/her.

                            //commented by IR
                            //if (
                            //    db.super_line_manager_tagging.Where(m => 
                            //        m.taggedEmployee.EmployeeId.Equals(temp)
                            //        ).Count() > 0)
                            //{
                            //    continue;
                            //}

                            // Also make sure that you do not add an SLM in his own
                            // tags.
                            //if(temp == theSLMEmployee.EmployeeId) {
                            //    continue;
                            //}

                            tags.Add(new SLM()
                            {
                                superLineManager = theSLMEmployee,
                                taggedEmployee =
                                        db.employee.Where(m =>
                                        m.active &&
                                        m.EmployeeId.Equals(temp)).FirstOrDefault()
                            });



                        }
                    }


                    db.super_line_manager_tagging.AddRange(tags);
                }





                db.SaveChanges();

                return new
                {
                    error = "",
                };
            }
        }

        public static dynamic deleteSLM(string sid)
        {
            int SLMId;

            if (!int.TryParse(sid, out SLMId))
            {
                return new { error = "Invalid SLM id." };
            }

            // Using a seperate context to first remove all the
            // SLM mapping from the db.
            using (var db = new Context())
            {
                Employee theSLMEmployee = db.employee.Where(m =>
                    m.active &&
                    m.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM) &&//changed by IR from SLM to LM
                    m.EmployeeId.Equals(SLMId)).FirstOrDefault();

                if (theSLMEmployee != null)
                {
                    // remove all the employees tagged for this 
                    // super line manager.
                    db.super_line_manager_tagging.RemoveRange(
                        db.super_line_manager_tagging.Where(p =>
                        p.superLineManager.EmployeeId.Equals(SLMId)).ToArray()
                    );

                    db.SaveChanges();
                }
            }

            return new
            {
                error = "",
            };

        }


        #region ChosenAjaxDataHandler

        public static ViewModels.Employee[] getAllEmployeesMatching(string subString)
        {
            using (var db = new Context())
            {
                // Get all distinct employeeIds of the employees
                // who are already tagged in some SLM.
                int[] taggedEmployees =
                    db.super_line_manager_tagging.Select(p =>
                        p.taggedEmployee.EmployeeId).Distinct().ToArray();


                // Now pick the employees whose employeeIds are not
                // in the list acquired in the step above. And of
                // course their employee codes match the query
                // string 'subString'.
                Employee[] employees = db.employee.Where(m =>
                    m.active &&
                    taggedEmployees.Contains(m.EmployeeId) &&//IR removed ! NOT
                    m.employee_code.ToLower().Contains(subString)).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

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