using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DLL.Models;
using System.Dynamic;

namespace TimeTune
{
    public class RosterManagementHelper
    {

        #region Groups

        #region ChosenAjaxDatahandlers
        public static ViewModels.Employee[] getAllLineManagersMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all line managers
                List<Employee> employees = db.employee.Where(m => m.active &&
                    m.employee_code.ToLower().Contains(subString) && 
                    m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_LM)).ToList();



                int[] alreadyLineManagerIds = db.group.Where(m => m.active).Select(m => m.supervisor_id).ToArray();

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


                Employee[] emps = employees.Where(m => m.employee_code != null).ToArray();



                ViewModels.Employee[] toReturn = new ViewModels.Employee[emps.Length];

                for (int i = 0; i < emps.Length; i++)
                {
                    toReturn[i] = EmployeeCrud.convert(emps[i]);
                }

                return toReturn;
            }
        }



        public static ViewModels.Employee[] getAllEmployeesWhereGroupIsNull(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                Employee[] employees = db.employee.Where(m => m.active && m.employee_code.ToLower().Contains(subString) && m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_EMP)
                    && m.Group == null).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                }

                return toReturn;
            }
        }



        public static ViewModels.Shift[] getAllActiveShifts(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                Shift[] shifts = db.shift.Where(m=> m.active).ToArray();

                ViewModels.Shift[] toReturn = new ViewModels.Shift[shifts.Length];

                for (int i = 0; i < shifts.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeShift.convert(shifts[i]);
                }

                return toReturn;
            }
        }

        #endregion

        // this function will return null on success, or it will return
        // an error message on error.
		
        public static string createGroup(ViewModels.CreateGroup fromForm)
        {
            if (fromForm.group_name == null || fromForm.group_name.Equals(""))
            {
                return "group name cannot be empty.";
            }

            if (fromForm.line_manager == null)
            {
                return "line manager cannot be null.";
            }
            // if line manager is null then return,
            // because it cannot be. as it will either be some value or
            // "-1"

            if (fromForm.line_manager == null)
            {
                return "Invalid operation, please contact IT staff.";
            }


            using (var db = new Context())
            {
                int supervisorId = 0;

                // check if the supervisor is already assigned a group.
                // if it is then return, as it cannot be because
                // the frontend does not allow it.

                // temp variable for storing int.parse() 
                // values.
                int temp = int.Parse(fromForm.line_manager);

                Employee LM = db.employee.Where(m => m.active &&

                    m.EmployeeId.Equals(temp) && 
                    
                    m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_LM)
                    
                    ).FirstOrDefault();

                supervisorId = (LM == null)? 0 : LM.EmployeeId;



                 

                var supervisor = db.employee.Where(m => m.EmployeeId.Equals(supervisorId) && m.active).FirstOrDefault();
                if (supervisor == null)
                {
                    return "There must be a line manager when creating a group";
                }

                if (supervisor.Group != null)
                {
                    return "This line manager already has a group.";
                }

                // check if the employees are already in some group.
                List<Employee> groupEmployees = new List<Employee>();

                if (fromForm.group_employees != null)
                {
                    for (int i = 0; i < fromForm.group_employees.Count; i++)
                    {
                        temp = int.Parse(fromForm.group_employees[i]);

                        Employee emp = db.employee.Where(m => m.active && 
                            m.EmployeeId.Equals(temp)).FirstOrDefault();

                        if (emp == null || emp.Group != null)
                        {
                            return "Employee " + fromForm.group_employees[i] + " cannot be assigned to this group.";
                        }

                        groupEmployees.Add(emp);
                    }
                }



                List<Shift> groupShifts = new List<Shift>();

                if (fromForm.group_shifts != null)
                {
                    for (int i = 0; i < fromForm.group_shifts.Count; i++)
                    {
                        temp = int.Parse(fromForm.group_shifts[i]);
                        // get a shift which is active and has the shift id  that is specified.
                        Shift sh = db.shift.Where(m => m.active && m.ShiftId.Equals(temp))
                            .FirstOrDefault();

                        if (sh == null)
                        {
                            return "Shift " + fromForm.group_shifts[i] + "does not exist.";
                        }

                        groupShifts.Add(sh);
                    }
                }

                Group toAdd = new Group()
                {
                    // when the 'follows general calendar' checkbox is uchecked,
                    // the view models 'follows_general_calendar' field is null.
                    // other wise it is not null.
                    follows_general_calendar = (fromForm.follows_general_calendar != null),
                    active = true,
                    supervisor_id = supervisor.EmployeeId,
                    Employees = groupEmployees,
                    Shifts = groupShifts,
                    group_name = fromForm.group_name,
                };

                db.group.Add(toAdd);
                supervisor.Group = toAdd;

                

                

                db.SaveChanges();


                return null;
                
                

                // if none of that is true, create the group, else just return quitely.
            }
        }



        // this function will return null on success, or it will return
        // an error message on error.	
		
        public static dynamic editGroup(ViewModels.CreateGroup fromForm)
        {
            int groupID;

            if (!int.TryParse(fromForm.group_id, out groupID))
            {
                return new {error="Invalid group id."};
            }

            if (fromForm.group_name == null || fromForm.group_name.Equals(""))
            {
                return new { error = "group name cannot be empty." };
            }

            if (fromForm.line_manager == null)
            {
                return new { error = "Invalid operation, line manager not found. please contact IT staff." };
            }


            using (var db = new Context())
            {
                int supervisorId = 0;

                // temp is a variable used to store
                // the results of int.parse()
                int temp = int.Parse(fromForm.line_manager);

                Employee LM = db.employee.Where(m => m.active &&

                    m.EmployeeId.Equals(temp) && // its active

                    m.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_LM) // its a line manager

                    ).FirstOrDefault();

                supervisorId = (LM == null) ? 0 : LM.EmployeeId;


                // load all employees.
                List<Employee> validFormEmployees = new List<Employee>();

                if (fromForm.group_employees != null)
                {
                    for (int i = 0; i < fromForm.group_employees.Count; i++)
                    {
                        temp = int.Parse(fromForm.group_employees[i]);
                        

                        Employee emp = db.employee.Where(
                            m => m.active && // should be active
                            m.EmployeeId.Equals(temp) && // should have an id
                            (m.Group == null || (m.Group != null && m.Group.GroupId == groupID))// should not already have a group.
                            ).FirstOrDefault();

                        if(emp != null)
                            validFormEmployees.Add(emp);
                    }
                }


                // get the shifts
                List<Shift> groupShifts = new List<Shift>();

                if (fromForm.group_shifts != null)
                {
                    for (int i = 0; i < fromForm.group_shifts.Count; i++)
                    {
                        temp = int.Parse(fromForm.group_shifts[i]);
                        // get a shift which is active and has the shift id  that is specified.
                        Shift sh = db.shift.Where(m => m.active && m.ShiftId.Equals(temp))
                            .FirstOrDefault();

                        if (sh == null)
                        {
                            return new { error = "Shift " + fromForm.group_shifts[i] + "does not exist." };
                        }

                        groupShifts.Add(sh);
                    }
                }

                if (!int.TryParse(fromForm.group_id, out temp))
                    return new { error = "invalid group id." };



                //get the group that we need to edit.
                Group toEdit = db.group.Find(temp);
                if(toEdit == null) {
                    return new { error = "group not found" };
                }

                // group supervisor:
                // POSSIBILITIES:
                // line manager can be unchanged
                // line manager can be some other line manager
                // line manager can be invalid: already in some other group
                // or inactive.


                //if line manager is valid.
                if (supervisorId != 0) { 
                    // if line manager is some other line manager.
                    if (toEdit.supervisor_id != LM.EmployeeId) { 
                        // disable the current line manager.
                        var forceLoadLM = db.employee.Find(toEdit.supervisor_id);
                        
                        // check for null, because in the case when
                        // super visor id is '0' - no LM assigned -
                        // the forceLoadLM varibale will be null.
                        if(forceLoadLM != null)
                            forceLoadLM.Group = null;

                        // add the new line manager.
                        toEdit.supervisor_id = LM.EmployeeId;
                        LM.Group = toEdit;


                        
                    }

                }
                else if (supervisorId == 0) // supervisor id is 0 when there is no line manager to assign to the group.
                {
                    // if there is a line manager for this group
                    // we need to unset it
                    if (toEdit.supervisor_id != 0)
                    {
                        // disable the current line manager.
                        var forceLoadLM = db.employee.Find(toEdit.supervisor_id);
                        forceLoadLM.Group = null;

                        // set the line manager to none.
                        toEdit.supervisor_id = 0;
                    }
                }
                //else
                    // do nothing.


                // group employees:
                // POSSIBILITIES:
                // employee can be the same employee.
                // employee can be a new employee.
                // employee can be invalid.
                // some employees can be missing from the
                //      form because they have been removed.

                // get all valid employees from form
                // done above in the loop.

                // get all employees from group which are not in form valids
                // validFormEmployees



                // get all the employees of the group, except the line manager.
                Employee[] groupEmployeesCopy = toEdit.Employees.Where(m=> m.active && !m.EmployeeId.Equals(supervisorId)).ToArray();
                


                // remove all form employees from groupEmployeesCopy
                for (int i = 0; i < groupEmployeesCopy.Length; i++)
                {
                    if (groupEmployeesCopy[i] == null)
                        continue;

                    // check for every valid employee
                    // if the valid employee is in group employees,
                    // if it is set it null
                    for (int j = 0; j < validFormEmployees.Count; j++)
                    {

                        if (validFormEmployees[j].EmployeeId == groupEmployeesCopy[i].EmployeeId)
                        {
                            groupEmployeesCopy[i] = null;
                            break;
                        }
                    }
                }


                List<Employee> toRemove = new List<Employee>();

                // set these employees free.
                for (int i = 0; i < groupEmployeesCopy.Length; i++)
                {
                    if(groupEmployeesCopy[i] != null) {
                        //toEdit.Employees.Remove(toEdit.Employees[i]);

                        
                        toRemove.Add(groupEmployeesCopy[i]);
                    }
                }

                for (int i = 0; i < toRemove.Count; i++)
                {
                    toEdit.Employees.Remove(toRemove[i]);
                    toRemove[i].Group = null;
                    
                }

                // set employees
                toEdit.Employees = validFormEmployees;

                // always add LM to the group.
                toEdit.Employees.Add(LM);


                toEdit.Shifts.RemoveAll(m=> true);
                foreach (Shift shif in groupShifts)
                {
                    toEdit.Shifts.Add(shif);
                }
                

                toEdit.group_name = fromForm.group_name;

                toEdit.follows_general_calendar = (fromForm.follows_general_calendar != null);

                db.SaveChanges();

                return new { 
                    error = "",
                    line_manager_name = (supervisorId !=0)?LM.first_name+" "+LM.last_name:"-",
                    line_manager_code = (supervisorId !=0)?LM.employee_code:"-"
                };



                // if none of that is true, create the group, else just return quitely.
            }
        }


        public static dynamic getGroup(int groupId)
        {
            dynamic obj = new
            {
                success = false,
                group_id = "",
                group_name = "",
                follows_general_calendar = false,
                line_manager_id = "",
                line_manager_code = "",
                group_employees = new[] {
                    new {id = "id" , text = "val"}, 
                    new {id = "id" , text = "val"}, 
                },
                group_shifts = new[] {
                    new {id = "id" , text = "val"}, 
                    new {id = "id" , text = "val"}, 
                }
            };


            using (var db = new Context())
            {

                Group requestedGroup = db.group.Where(m => m.active && m.GroupId == groupId).FirstOrDefault();

                if (requestedGroup != null)
                {
                    //1
                    //var groupID = groupId;
                    //2
                    dynamic lineManager;
                    if(requestedGroup.supervisor_id == 0) {
                        lineManager = new {EmployeeId=0,employee_code="none"};
                    } else {
                        lineManager = db.employee.Find(requestedGroup.supervisor_id);
                    }

                    //3
                    Employee[] employees = requestedGroup.Employees.Where(m=>m.active && m.EmployeeId != lineManager.EmployeeId).ToArray();
                    List<dynamic> groupEmployees = new List<dynamic>();
                    for (int i = 0; i < employees.Length; i++)
                    {
                        groupEmployees.Add(new
                        {
                            id = employees[i].EmployeeId + "",
                            text=employees[i].employee_code
                        });
                    }

                    //4
                    Shift[] shifts = requestedGroup.Shifts.ToArray();
                    List<dynamic> groupShifts = new List<dynamic>();
                    for (int i = 0; i < shifts.Length; i++)
                    {
                        groupShifts.Add(new
                        {
                            id = shifts[i].ShiftId + "",
                            text = shifts[i].name + " " + shifts[i].start_time + "-" + shifts[i].shift_end
                        });
                    }


                    obj = new
                    {
                        success = true,
                        group_id = groupId,
                        group_name = "", // unused
                        follows_general_calendar = requestedGroup.follows_general_calendar,
                        line_manager_id = lineManager.EmployeeId,
                        line_manager_code = lineManager.employee_code,
                        group_employees = groupEmployees,
                        group_shifts = groupShifts,
                        
                    };
                        


                }

            }
            

            return obj;
        }

        public static ViewModels.Group deleteGroup(string id)
        {
            // return if the string contains an invalid
            // number.
            int group_id;
            if (!int.TryParse(id, out group_id))
                return null;

            using (var db = new Context())
            {
                // fetch the group from the db
                Group grp = db.group.Find(group_id);

                if (grp == null)
                    return null;

                // set active to false,
                // soft delete.
                grp.active = false;

                // release employees
                if (grp.Employees != null)
                {
                    for (int i = 0; i < grp.Employees.Count;i++ )
                    {
                        grp.Employees[i].Group.active = false;
                        grp.Employees.Remove(grp.Employees[i]);
                    }
                }

                // release shifts
                if (grp.Shifts != null)
                {
                    for (int i = 0; i < grp.Shifts.Count; i++)
                    {
                        grp.Shifts.Remove(grp.Shifts[i]);
                    }
                }


                // release line manager
                if (grp.supervisor_id != 0 && grp.supervisor_id != -1)
                {
                    var lm = db.employee.Find(grp.supervisor_id);

                    if (lm != null)
                    {
                        lm.Group = null;
                    }

                    grp.supervisor_id = 0;
                }
                // set the line manager group id to null,
                // remove employees from the group.
                db.SaveChanges();
                ViewModels.Group group = new ViewModels.Group()
                {
                    group_description = grp.group_description,
                    group_name = grp.group_name,
                    id = grp.GroupId
                };

                return group;

               

            }


        }
        /*
        // returns a list of view models for use with data tables.
        public static List<ViewModels.GroupsTableView> getAllGroupsForTable()
        {
            using (Context db = new Context())
            {
                List<DLL.Models.Group> groups = null;
                try
                {
                    groups = db.group.Where(m => m.active).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    groups = new List<DLL.Models.Group>();
                }



                List<ViewModels.GroupsTableView> toReturn = new List<ViewModels.GroupsTableView>();

                for (int i = 0; i < groups.Count(); i++)
                {
                    ViewModels.GroupsTableView vm = new ViewModels.GroupsTableView();
                    DLL.Models.Group dm = groups[i];

                    vm.id = dm.GroupId;

                    vm.group_name = dm.group_name;


                    Employee lineManager = db.employee.Where(m => m.active && m.EmployeeId.Equals(dm.supervisor_id)).FirstOrDefault();



                    vm.line_manager_code = (lineManager == null)?"-":lineManager.employee_code;

                    vm.line_manager_name = (lineManager == null) ?"-" : lineManager.first_name + " " + lineManager.last_name;


                    // The employee table will show the edit/delete column.
                    vm.action =
                            @"<div data-id='" + vm.id + @"'>
                            <a href='javascript:void(editGroup(" + vm.id + @"));'>Edit</a>
                            <span>/</span>
                            <a href='javascript:void(deleteGroup(" + vm.id + @"));'>Delete</a>
                        </div>";


                    toReturn.Add(vm);
                }

                return toReturn;
            }
        }
        */

        public static int getAllGroupsMatching(string search, string sortOrder, int start, int length, out List<ViewModels.GroupsTableView> toReturn)
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
                        count = db.group.Where(p => p.active &&

                                p.GroupId.ToString().ToLower().Contains(search.ToLower()) ||
                                
                                p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().first_name.Contains(search.ToLower()) ||

                                p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().last_name.Contains(search.ToLower())  ||

                                p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().employee_code.Contains(search.ToLower())
                            ).Count();

                        
                        // load actual content.
                        toReturn = db.group.Where(p => p.active &&

                                p.GroupId.ToString().ToLower().Contains(search.ToLower()) ||

                                p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().first_name.Contains(search.ToLower()) ||

                                p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().last_name.Contains(search.ToLower()) ||

                                p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().employee_code.Contains(search.ToLower())
                            )
                        .Select(p =>
                            new ViewModels.GroupsTableView() {
                                id = p.GroupId,
                                line_manager_code = p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().employee_code,
                                line_manager_name = 
                                    p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().first_name + " " +
                                    p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().last_name,
                                action =
                                @"<div data-id='" + p.GroupId + @"'>
                                    <a href='javascript:void(editGroup(" + p.GroupId + @"));'>Edit</a>
                                    <span>/</span>
                                    <a href='javascript:void(deleteGroup(" + p.GroupId + @"));'>Delete</a>
                                </div>"
                            })
                        .SortBy(sortOrder).Skip(start).Take(length).ToList();

                        // id, 

                        

                    }
                    else
                    {
                        // query the db for count
                        count = db.group.Where(m => m.active).Count();

                        // load actual data.
                        toReturn = db.group.Where(m => m.active)
                            .Select(p =>
                            new ViewModels.GroupsTableView()
                            {
                                id = p.GroupId,
                                line_manager_code = p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().employee_code,
                                line_manager_name =
                                    p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().first_name + " " +
                                    p.Employees.Where(e => e.EmployeeId.Equals(p.supervisor_id)).FirstOrDefault().last_name,
                                action =
                                @"<div data-id='" + p.GroupId + @"'>
                                    <a href='javascript:void(editGroup(" + p.GroupId + @"));'>Edit</a>
                                    <span>/</span>
                                    <a href='javascript:void(deleteGroup(" + p.GroupId + @"));'>Delete</a>
                                </div>"
                            })
                        .SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.GroupsTableView>();
                }


               

                return count;
            }
        }
        
        /*
        public class GroupsTableResultSet
        {
            public static List<ViewModels.GroupsTableView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.GroupsTableView> dtResult)
            {
                return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
            }

            public static int Count(string search, List<ViewModels.GroupsTableView> dtResult)
            {
                return FilterResult(search, dtResult).Count();
            }

            private static IQueryable<ViewModels.GroupsTableView> FilterResult(string search, List<ViewModels.GroupsTableView> dtResult)
            {
                IQueryable<ViewModels.GroupsTableView> results = dtResult.AsQueryable();

                results = results.Where(p =>
                    (
                        search == null ||
                        (
                                p.id.ToString().ToLower().Contains(search.ToLower())
                            || (p.line_manager_code != null && p.line_manager_code.ToLower().Contains(search.ToLower()))
                            || (p.line_manager_name != null && p.line_manager_name.ToLower().Contains(search.ToLower()))
                            || (p.group_name != null && p.group_name.ToLower().Contains(search.ToLower()))
                        )
                    )
                    //&& (columnFilters[0] == null || (p.first_name != null && p.first_name.ToLower().Contains(columnFilters[0].ToLower())))
                    //&& (columnFilters[1] == null || (p.last_name != null && p.last_name.ToLower().Contains(columnFilters[1].ToLower())))
                    //&& (columnFilters[2] == null || (p.employee_code != null && p.employee_code.ToLower().Contains(columnFilters[2].ToLower())))
                    //&& (columnFilters[3] == null || (p.email != null && p.email.ToLower().Contains(columnFilters[3].ToLower())))
                    //&& (columnFilters[4] == null || (p.function_name != null && p.function_name.ToLower().Contains(columnFilters[4].ToLower())))
                    //&& (columnFilters[5] == null || (p.department_name != null && p.department_name.ToLower().Contains(columnFilters[5].ToLower())))
                    //&& (columnFilters[6] == null || (p.designation_name != null && p.designation_name.ToLower().Contains(columnFilters[6].ToLower())))
                    //&& (columnFilters[7] == null || (p.access_group_name != null && p.access_group_name.ToLower().Contains(columnFilters[7].ToLower())))
                    //&& (columnFilters[8] == null || (p.group_name != null && p.group_name.ToLower().Contains(columnFilters[8].ToLower())))
                    //&& (columnFilters[9] == null || (p.grade_name != null && p.grade_name.ToLower().Contains(columnFilters[9].ToLower())))
                    //&& (columnFilters[10] == null || (p.region_name != null && p.region_name.ToLower().Contains(columnFilters[10].ToLower())))
                    //&& (columnFilters[11] == null || (p.location_name != null && p.location_name.ToLower().Contains(columnFilters[11].ToLower())))
                    //&& (columnFilters[13] == null || (p.type_of_employment_name != null && p.type_of_employment_name.ToLower().Contains(columnFilters[13].ToLower())))
                    );

                return results;
            }
        }
        */
        #endregion


    }
}