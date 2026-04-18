using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTune
{
    public class EmployeeManagementHelper
    {
        #region ChosenAjaxDataHandlerForLM and SLM
        public static ViewModels.Employee[] getAllEmployeesMatchingLM(string subString, string employeeCode)
        {
            using (var db = new Context())
            {
                Employee superior = db.employee.Where(m => m.employee_code.Equals(employeeCode)).FirstOrDefault();
                // get employee with supervisor of employeeID


                if (superior.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM))
                {
                    List<Employee> emps = db.employee.Where(m =>
                    m.active &&
                    m.Group.supervisor_id.Equals(superior.EmployeeId) &&
                    m.employee_code.ToLower().Contains(subString)).ToList();

                    // add line manager himself.
                    emps.Add(superior);

                    Employee[] employees = emps.ToArray();


                    ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

                    for (int i = 0; i < employees.Length; i++)
                    {
                        toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                    }

                    return toReturn;
                }
                else if (superior.access_group.name.Equals(DLL.Commons.Roles.ROLE_SLM))
                {

                    List<ViewModels.Employee> toReturn =
                        db.super_line_manager_tagging.Where(m =>
                        m.superLineManager.EmployeeId.Equals(superior.EmployeeId) &&
                        m.taggedEmployee.active).Select(m =>
                            new ViewModels.Employee()
                            {
                                id = m.taggedEmployee.EmployeeId,
                                employee_code = m.taggedEmployee.employee_code,
                                first_name = m.taggedEmployee.first_name,
                                last_name = m.taggedEmployee.last_name
                            }).ToList();

                    toReturn.Sort();

                    //toReturn.Add(new ViewModels.Employee()
                    //{
                    //    id = superior.EmployeeId,
                    //    employee_code = superior.employee_code,
                    //    first_name = superior.first_name,
                    //    last_name = superior.last_name
                    //});

                    return toReturn.ToArray();

                }
                else
                {
                    return new ViewModels.Employee[] { };
                }



            }
        }


        #endregion


        #region ChosenAjaxDataHandler

        public static ViewModels.Employee[] getAllEmployeesMatchingExceptSuperHRs(string subString)
        {
            using (var db = new Context())
            {
                subString = subString.ToLower();

                // get all employees with no groups
                Employee[] employees = db.employee.Where(m => !m.employee_code.Equals("000000") && !m.is_super_hr && (m.active && m.timetune_active) && (m.employee_code.ToLower().Contains(subString) || (m.first_name != null && m.first_name.ToLower().Contains(subString)) || (m.last_name != null && m.last_name.ToLower().Contains(subString)))).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                }

                return toReturn;
            }
        }

        public static ViewModels.Employee[] getAllEmployeesMatching(string subString)
        {
            using (var db = new Context())
            {
                subString = subString.ToLower();

                // get all employees with no groups
                Employee[] employees = db.employee.Where(m => (m.active && m.timetune_active) && (m.employee_code.ToLower().Contains(subString) || (m.first_name != null && m.first_name.ToLower().Contains(subString)) || (m.last_name != null && m.last_name.ToLower().Contains(subString)))).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                }

                return toReturn;
            }
        }

        public static ViewModels.Employee[] getSearchEmployeesMatching(string subString)
        {
            using (var db = new Context())
            {
                subString = subString.ToLower();

                // get all employees with no groups
                //                Employee[] employees = db.employee.Where(m => (m.active && m.timetune_active) && (m.employee_code + "" + m.first_name + "" + m.last_name).ToLower().Contains(subString)).ToArray();
                Employee[] employees = db.employee.Where(m => (m.active && m.timetune_active) && (m.employee_code.ToLower().Contains(subString) || (m.first_name != null && m.first_name.ToLower().Contains(subString)) || (m.last_name != null && m.last_name.ToLower().Contains(subString)))).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                }

                return toReturn;
            }
        }


        public static ViewModels.Employee[] getAllEmployeesWithLeaves(string self_code, string subString)
        {
            using (var db = new Context())
            {
                subString = subString.ToLower();

                // get all employees with no groups
                Employee[] employees = db.employee.Where(m => (m.active && m.timetune_active) && m.employee_code.ToLower() != self_code && (m.employee_code.ToLower().Contains(subString) || (m.first_name != null && m.first_name.ToLower().Contains(subString)) || (m.last_name != null && m.last_name.ToLower().Contains(subString)))).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convertWithLeaves(employees[i]);
                }

                return toReturn;
            }
        }

        public static BLL.ViewModels.VM_ContractualStaff[] getContractualStaffMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                ContractualStaff[] employees = db.contractual_staff.Where(m => m.active && m.employee_code.ToLower().Contains(subString)).ToArray();

                BLL.ViewModels.VM_ContractualStaff[] toReturn = new BLL.ViewModels.VM_ContractualStaff[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    BLL.ViewModels.VM_ContractualStaff staff = new BLL.ViewModels.VM_ContractualStaff();
                    staff.ContractualStaffId = employees[i].ContractualStaffId;
                    staff.active = employees[i].active;
                    staff.company = employees[i].company;
                    staff.date_of_joining = (!employees[i].date_of_joining.HasValue) ? null : employees[i].date_of_joining.ToString();
                    staff.date_of_leaving = (!employees[i].date_of_leaving.HasValue) ? null : employees[i].date_of_leaving.ToString();
                    staff.department = employees[i].department;
                    staff.designation = employees[i].designation;
                    staff.email = employees[i].email;
                    staff.employee_code = employees[i].employee_code;
                    staff.function = employees[i].function;
                    staff.grade = employees[i].grade;
                    staff.location = employees[i].location;
                    staff.mobile_no = employees[i].mobile_no;
                    staff.region = employees[i].region;
                    staff.Group = employees[i].Group;
                    staff.address = employees[i].address;
                    toReturn[i] = staff;
                }

                return toReturn;
            }
        }



        #endregion


        #region terminal
        public static BLL.ViewModels.Terminals[] getllTerminalMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                Terminals[] terminal = db.termainal.Where(m => m.L_ID.ToString().Contains(subString)).ToArray();

                BLL.ViewModels.Terminals[] toReturn = new BLL.ViewModels.Terminals[terminal.Length];

                for (int i = 0; i < terminal.Length; i++)
                {
                    toReturn[i] = new BLL.ViewModels.Terminals()
                    {
                        id = terminal[i].L_ID,
                        name = terminal[i].C_Name,
                        terminal_id = terminal[i].L_ID.ToString()
                    };
                }

                return toReturn;
            }
        }
        #endregion

        #region EmployeeFields ChosenAjaxDataHandlers


        // campuses
        public static ViewModels.OrganizationCampusView[] getAllCampusesMatching(string subString, bool bGVIsSuperHRRole, int iGVCampusID)
        {
            OrganizationCampus[] campuses;
            ViewModels.OrganizationCampusView[] toReturn = new ViewModels.OrganizationCampusView[1];

            using (var db = new Context())
            {
                // get all campuses
                if (bGVIsSuperHRRole) //alowed to manage all campuses
                {
                    campuses = db.organization_campus.ToArray();
                }
                else
                {
                    campuses = db.organization_campus.Where(c => c.Id == iGVCampusID && c.CampusCode.ToLower().Contains(subString)).ToArray();
                }
                //OrganizationCampus[] campuses = db.organization_campus.Where(m => m.CampusCode.ToLower().Contains(subString)).ToArray();

                if (campuses.Length > 0)
                {
                    toReturn = new ViewModels.OrganizationCampusView[campuses.Length];

                    for (int i = 0; i < campuses.Length; i++)
                    {
                        toReturn[i] = new ViewModels.OrganizationCampusView()
                        {
                            Id = campuses[i].Id,
                            CampusCode = campuses[i].CampusCode.Replace("-", "- "),
                            CampusTitle = campuses[i].CampusTitle
                        };
                    }

                    //return toReturn;
                }
            }

            return toReturn;
        }

        // genders
        public static ViewModels.GenderView[] getAllGendersMatching(string subString)
        {
            Gender[] genders;
            ViewModels.GenderView[] toReturn = new ViewModels.GenderView[1];

            using (var db = new Context())
            {
                genders = db.gender.ToArray();
                if (genders.Length > 0)
                {
                    toReturn = new ViewModels.GenderView[genders.Length];

                    for (int i = 0; i < genders.Length; i++)
                    {
                        toReturn[i] = new ViewModels.GenderView()
                        {
                            Id = genders[i].Id,
                            GenderName = genders[i].GenderName.Replace("-", "- ")
                        };
                    }

                    //return toReturn;
                }
            }

            return toReturn;
        }


        // shifts
        public static ViewModels.OrganizationProgramShiftView[] getAllPShiftsMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all campuses
                OrganizationProgramShift[] pshifts = db.organization_program_shift.Where(m => m.ProgramShiftName.ToLower().Contains(subString)).ToArray();

                ViewModels.OrganizationProgramShiftView[] toReturn = new ViewModels.OrganizationProgramShiftView[pshifts.Length];

                for (int i = 0; i < pshifts.Length; i++)
                {
                    toReturn[i] = new ViewModels.OrganizationProgramShiftView()
                    {
                        Id = pshifts[i].Id,
                        ProgramShiftName = pshifts[i].ProgramShiftName.Replace("-", "- ")
                    };
                }

                return toReturn;
            }
        }


        // functions
        public static ViewModels.Function[] getAllFunctionsMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                Function[] functions = db.function.Where(m => m.active && m.name.ToLower().Contains(subString)).ToArray();

                ViewModels.Function[] toReturn = new ViewModels.Function[functions.Length];

                for (int i = 0; i < functions.Length; i++)
                {
                    toReturn[i] = new ViewModels.Function()
                    {
                        id = functions[i].FunctionId,
                        name = functions[i].name.Replace("-", "- "),
                        description = functions[i].description
                    };
                }

                return toReturn;
            }
        }

        // designations
        public static ViewModels.Designation[] getAllDesignationsMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                Designation[] designations = db.designation.Where(m => m.active && m.name.ToLower().Contains(subString)).ToArray();

                ViewModels.Designation[] toReturn = new ViewModels.Designation[designations.Length];

                for (int i = 0; i < designations.Length; i++)
                {
                    toReturn[i] = new ViewModels.Designation()
                    {
                        id = designations[i].DesignationId,
                        name = designations[i].name.Replace("-", "- "),
                        description = designations[i].description
                    };
                }

                return toReturn;
            }
        }

        // departments
        public static ViewModels.Department[] getAllDepartmentsMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                Department[] departments = db.department.Where(m => m.active && m.name.ToLower().Contains(subString)).ToArray();

                ViewModels.Department[] toReturn = new ViewModels.Department[departments.Length];

                for (int i = 0; i < departments.Length; i++)
                {
                    toReturn[i] = new ViewModels.Department()
                    {
                        id = departments[i].DepartmentId,
                        name = departments[i].name.Replace("-", "- "),
                        description = departments[i].description
                    };
                }

                return toReturn;
            }
        }

        // type of employments
        public static ViewModels.TypeOfEmployment[] getAllTypeOfEmploymentsMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                TypeOfEmployment[] departments = db.type_of_employment.Where(m => m.active && m.name.ToLower().Contains(subString)).ToArray();

                ViewModels.TypeOfEmployment[] toReturn = new ViewModels.TypeOfEmployment[departments.Length];

                for (int i = 0; i < departments.Length; i++)
                {
                    toReturn[i] = new ViewModels.TypeOfEmployment()
                    {
                        id = departments[i].TypeOfEmploymentId,
                        name = departments[i].name,
                        description = departments[i].description
                    };
                }

                return toReturn;
            }
        }

        // regions
        public static ViewModels.Region[] getAllRegionsMatching(string subString)
        {
            using (var db = new Context())
            {
                // get all employees with no groups
                Region[] regions = db.region.Where(m => m.active && m.name.ToLower().Contains(subString)).ToArray();

                ViewModels.Region[] toReturn = new ViewModels.Region[regions.Length];

                for (int i = 0; i < regions.Length; i++)
                {
                    toReturn[i] = new ViewModels.Region()
                    {
                        id = regions[i].RegionId,
                        name = regions[i].name.Replace("-", "- "),
                        description = regions[i].description
                    };
                }

                return toReturn;
            }
        }

        // grades
        public static ViewModels.Grades[] getAllGradesMatching(string subString)
        {
            using (var db = new Context())
            {
                Grade[] grades = db.grade.Where(m => m.active && m.name.ToLower().Contains(subString)).ToArray();

                ViewModels.Grades[] toReturn = new ViewModels.Grades[grades.Length];

                for (int i = 0; i < grades.Length; i++)
                {
                    toReturn[i] = new ViewModels.Grades()
                    {
                        id = grades[i].GradeId,
                        name = grades[i].name.Replace("-", "- "),
                        description = grades[i].description
                    };
                }

                return toReturn;
            }
        }

        // locations
        public static ViewModels.Location[] getAllLocationsMatching(string subString)
        {
            using (var db = new Context())
            {
                Location[] locations = db.location.Where(m => m.active && m.name.ToLower().Contains(subString)).ToArray();

                ViewModels.Location[] toReturn = new ViewModels.Location[locations.Length];

                for (int i = 0; i < locations.Length; i++)
                {
                    toReturn[i] = new ViewModels.Location()
                    {
                        id = locations[i].LocationId,
                        name = locations[i].name.Replace("-", "- "),
                        description = locations[i].description
                    };
                }

                return toReturn;
            }
        }

        // positions
        public static ViewModels.Position_Status[] getAllPositionsMatching(string subString)
        {
            using (var db = new Context())
            {
                PositionStatus[] positions = db.position_status.Where(m => m.PositionText.ToLower().Contains(subString)).ToArray();

                ViewModels.Position_Status[] toReturn = new ViewModels.Position_Status[positions.Length];

                for (int i = 0; i < positions.Length; i++)
                {
                    toReturn[i] = new ViewModels.Position_Status()
                    {
                        id = positions[i].Id,
                        position_text = positions[i].PositionText.Replace("-", "- ")
                    };
                }

                return toReturn;
            }
        }


        // sites
        public static ViewModels.Site_Status[] getAllSitesMatching(string subString)
        {
            using (var db = new Context())
            {


                //StringResource [] str= db.string_resource.Select(x =>x.Id = 1)
                SiteStatus[] sites = db.site_status.Where(m => m.SiteText.ToLower().Contains(subString)).ToArray();

                ViewModels.Site_Status[] toReturn = new ViewModels.Site_Status[sites.Length];

                for (int i = 0; i < sites.Length; i++)
                {
                    toReturn[i] = new ViewModels.Site_Status()
                    {
                        id = sites[i].Id,
                        site_text = sites[i].SiteText.Replace("-", "- ")
                    };
                }

                return toReturn;
            }
        }



        #endregion
    }
}
