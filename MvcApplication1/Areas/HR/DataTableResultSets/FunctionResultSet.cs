using BLL.PdfReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ViewModels;

namespace MvcApplication1
{
    public class FunctionResultSet
    {
        public static List<ViewModels.Function> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.Function> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.Function> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.Function> FilterResult(string search, List<ViewModels.Function> dtResult)
        {
            IQueryable<ViewModels.Function> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.name != null && p.name.ToLower().Contains(search.ToLower())
                        || (p.description != null && p.description.ToLower().Contains(search.ToLower()))


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

    public class ProgramShiftResultSet
    {
        public static List<ViewModels.OrganizationProgramShiftView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationProgramShiftView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationProgramShiftView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationProgramShiftView> FilterResult(string search, List<ViewModels.OrganizationProgramShiftView> dtResult)
        {
            IQueryable<ViewModels.OrganizationProgramShiftView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.ProgramShiftName != null && p.ProgramShiftName.ToLower().Contains(search.ToLower())
                    )
                )
                );

            return results;
        }
    }

    public class SkillResultSet
    {
        public static List<ViewModels.SkillsSet> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.SkillsSet> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.SkillsSet> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.SkillsSet> FilterResult(string search, List<ViewModels.SkillsSet> dtResult)
        {
            IQueryable<ViewModels.SkillsSet> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.skillname != null && p.skillname.ToLower().Contains(search.ToLower())

                    )
                )
               );

            return results;
        }
    }

    public class ExmptEmployeeResultSet
    {
        public static List<ViewModels.ExmptEmployee> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.ExmptEmployee> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.ExmptEmployee> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.ExmptEmployee> FilterResult(string search, List<ViewModels.ExmptEmployee> dtResult)
        {
            IQueryable<ViewModels.ExmptEmployee> results = dtResult.AsQueryable();


            results = results.Where(p =>
                (
                    search == null ||
                    (

                     p.EmployeeCode != null && p.EmployeeCode.ToLower().Contains(search.ToLower())
                                || (p.FullName != null && p.FullName.ToLower().Contains(search.ToLower()))
                                || (p.EmployeeId != null && p.EmployeeId.ToString().Contains(search.ToLower()))



                         //(p.EmployeeId != null && p.EmployeeId == Convert.ToInt32(search))
                    //(p. != null && p.fullname.ToLower().Contains(search.ToLower()))

                    )
                ));

            return results;
        }
    }

    public class LeaveSessionResultSet
    {
        public static List<ViewModels.LeaveSession> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeaveSession> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.LeaveSession> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.LeaveSession> FilterResult(string search, List<ViewModels.LeaveSession> dtResult)
        {
            IQueryable<ViewModels.LeaveSession> results = dtResult.AsQueryable();


            results = results.Where(p =>
                (
                    search == null ||
                    (

                     p.EmployeeCode != null && p.EmployeeCode.ToLower().Contains(search.ToLower())
                                || (p.fullname != null && p.fullname.ToLower().Contains(search.ToLower()))
                                 || (p.str_EmpTypeName != null && p.str_EmpTypeName.ToLower().Contains(search.ToLower()))
                                || (p.EmployeeId != null && p.EmployeeId.ToString().Contains(search.ToLower()))



                    //(p.EmployeeId != null && p.EmployeeId == Convert.ToInt32(search))
                    //(p. != null && p.fullname.ToLower().Contains(search.ToLower()))

                    )
                ));

            return results;
        }
    }

    public class LeaveTypeResultSet
    {
        public static List<ViewModels.LeaveType> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeaveType> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.LeaveType> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.LeaveType> FilterResult(string search, List<ViewModels.LeaveType> dtResult)
        {
            IQueryable<ViewModels.LeaveType> results = dtResult.AsQueryable();


            results = results.Where(p =>
                (
                    search == null ||
                    (

                     p.LeaveTypeText != null && p.LeaveTypeText.ToLower().Contains(search.ToLower())
                                || (p.LeaveDefaultCount != null && p.LeaveDefaultCount.ToString().Contains(search.ToLower()))
                                || (p.LeaveMaxCount != null && p.LeaveMaxCount.ToString().Contains(search.ToLower()))



                    //(p.EmployeeId != null && p.EmployeeId == Convert.ToInt32(search))
                    //(p. != null && p.fullname.ToLower().Contains(search.ToLower()))

                    )
                ));

            return results;
        }
    }



    public class GeoPhencingResultSet
    {
        public static List<ViewModels.GeoPhencingTerminal> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.GeoPhencingTerminal> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.GeoPhencingTerminal> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.GeoPhencingTerminal> FilterResult(string search, List<ViewModels.GeoPhencingTerminal> dtResult)
        {
            IQueryable<ViewModels.GeoPhencingTerminal> results = dtResult.AsQueryable();


            results = results.Where(p =>
                (
                    search == null ||
                    (
                         (p.EmployeeId != null && p.EmployeeId == Convert.ToInt32(search))
                    || (p.EmployeeCode != null && p.EmployeeCode.ToLower().Contains(search.ToLower()))
                    || (p.status_in != null && p.status_in.ToLower().Contains(search.ToLower()))
                    || (p.status_out != null && p.status_out.ToLower().Contains(search.ToLower()))
                    || (p.final_remarks != null && p.final_remarks.ToLower().Contains(search.ToLower()))
                    || (p.terminal_in != null && p.terminal_in.ToLower().Contains(search.ToLower()))
                    || (p.terminal_out != null && p.terminal_out.ToLower().Contains(search.ToLower()))

                    )
                ));

            return results;
        }
    }

    public class LeaveReasonResultSet
    {
        public static List<ViewModels.LeaveReason> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeaveReason> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.LeaveReason> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.LeaveReason> FilterResult(string search, List<ViewModels.LeaveReason> dtResult)
        {
            IQueryable<ViewModels.LeaveReason> results = dtResult.AsQueryable();


            results = results.Where(p =>
                (
                    search == null ||
                    (


                         (p.LeaveReasonText != null && p.LeaveReasonText.ToLower().Contains(search.ToLower()))


                    )
                ));

            return results;
        }
    }


    public class DeptPerReportSet
    {
        public static List<ViewModels.Dept_Per_Rept> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.Dept_Per_Rept> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.Dept_Per_Rept> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.Dept_Per_Rept> FilterResult(string search, List<ViewModels.Dept_Per_Rept> dtResult)
        {
            IQueryable<ViewModels.Dept_Per_Rept> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.DepartmentName != null && p.DepartmentName.ToLower().Contains(search.ToLower())

                    )
                )
               );

            return results;
        }
    }

    public class DepartmentAttendanceResultSet
    {
        public static List<ViewModels.DepartmentAttendanceCountReport> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.DepartmentAttendanceCountReport> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.DepartmentAttendanceCountReport> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.DepartmentAttendanceCountReport> FilterResult(string search, List<ViewModels.DepartmentAttendanceCountReport> dtResult)
        {
            IQueryable<ViewModels.DepartmentAttendanceCountReport> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.EmployeeCode != null && p.EmployeeCode.ToLower().Contains(search.ToLower())
                                || (p.FirstName != null && p.FirstName.ToLower().Contains(search.ToLower()))
                                || (p.LastName != null && p.LastName.ToLower().Contains(search.ToLower()))
                    )
                )
                );

            return results;
        }
    }


    public class BankResultSet
    {
        public static List<ViewModels.BankNameInfo> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.BankNameInfo> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.BankNameInfo> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.BankNameInfo> FilterResult(string search, List<ViewModels.BankNameInfo> dtResult)
        {
            IQueryable<ViewModels.BankNameInfo> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.BankNameText != null && p.BankNameText.ToLower().Contains(search.ToLower())

                    )
                )
               );

            return results;
        }
    }


    public class DepartmentalTimesSheetResultSet
    {
        public static List<MonthlyDepartmentalTimeSheetData> GetResult(string search, string sortOrder, int start, int length, List<MonthlyDepartmentalTimeSheetData> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<MonthlyDepartmentalTimeSheetData> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<MonthlyDepartmentalTimeSheetData> FilterResult(string search, List<MonthlyDepartmentalTimeSheetData> dtResult)
        {
            IQueryable<MonthlyDepartmentalTimeSheetData> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.designationName != null && p.designationName.ToLower().Contains(search.ToLower())
                        || (p.locationName != null && p.locationName.ToLower().Contains(search.ToLower()))


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


    public class MissPunchResultSet
    {
        public static List<ViewModels.ConsolidatedAttendanceDepartmentWise> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.ConsolidatedAttendanceDepartmentWise> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.ConsolidatedAttendanceDepartmentWise> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.ConsolidatedAttendanceDepartmentWise> FilterResult(string search, List<ViewModels.ConsolidatedAttendanceDepartmentWise> dtResult)
        {
            IQueryable<ViewModels.ConsolidatedAttendanceDepartmentWise> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.department != null && p.department.ToLower().Contains(search.ToLower())
                            ||
                              p.employee_code != null && p.employee_code.ToLower().Contains(search.ToLower())
                              ||
                                p.employee_first_name != null && p.employee_first_name.ToLower().Contains(search.ToLower())
                                                              ||
                                p.employee_last_name != null && p.employee_last_name.ToLower().Contains(search.ToLower())
                                 ||
                                p.final_remarks != null && p.final_remarks.ToLower().Contains(search.ToLower())
                                                     ||
                                p.function != null && p.function.ToLower().Contains(search.ToLower())

                                || (p.designation != null && p.designation.ToLower().Contains(search.ToLower()))


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

    public class ManualPunchResultSet
    {
        public static List<ViewModels.ConsolidatedAttendanceDepartmentWise> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.ConsolidatedAttendanceDepartmentWise> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.ConsolidatedAttendanceDepartmentWise> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.ConsolidatedAttendanceDepartmentWise> FilterResult(string search, List<ViewModels.ConsolidatedAttendanceDepartmentWise> dtResult)
        {
            IQueryable<ViewModels.ConsolidatedAttendanceDepartmentWise> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.department != null && p.department.ToLower().Contains(search.ToLower())
                            ||
                              p.employee_code != null && p.employee_code.ToLower().Contains(search.ToLower())
                              ||
                                p.employee_first_name != null && p.employee_first_name.ToLower().Contains(search.ToLower())
                                                              ||
                                p.employee_last_name != null && p.employee_last_name.ToLower().Contains(search.ToLower())
                                 ||
                                p.final_remarks != null && p.final_remarks.ToLower().Contains(search.ToLower())
                                                     ||
                                p.function != null && p.function.ToLower().Contains(search.ToLower())

                                || (p.designation != null && p.designation.ToLower().Contains(search.ToLower()))


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

    public class Employee_Status_ResultSet
    {
        public static List<ViewModels.Employee> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.Employee> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.Employee> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.Employee> FilterResult(string search, List<ViewModels.Employee> dtResult)
        {
            IQueryable<ViewModels.Employee> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.time_tune_status != null && p.time_tune_status.ToLower().Contains(search.ToLower())
                            ||
                              p.employee_code != null && p.employee_code.ToLower().Contains(search.ToLower())
                              ||
                                p.first_name != null && p.first_name.ToLower().Contains(search.ToLower())
                                                              ||
                                p.last_name != null && p.last_name.ToLower().Contains(search.ToLower())
                                 ||
                                p.employee_code != null && p.employee_code.ToLower().Contains(search.ToLower())
                                                     ||
                                p.department_name != null && p.department_name.ToLower().Contains(search.ToLower())
                                 ||
                                p.grade_name != null && p.grade_name.ToLower().Contains(search.ToLower())

                                || (p.department_name != null && p.department_name.ToLower().Contains(search.ToLower()))


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

    public class Devices_Status_ResultSet
    {
        public static List<BLL_UNIS.ViewModels.DevicesStatus> GetResult(string search, string sortOrder, int start, int length, List<BLL_UNIS.ViewModels.DevicesStatus> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<BLL_UNIS.ViewModels.DevicesStatus> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<BLL_UNIS.ViewModels.DevicesStatus> FilterResult(string search, List<BLL_UNIS.ViewModels.DevicesStatus> dtResult)
        {
            IQueryable<BLL_UNIS.ViewModels.DevicesStatus> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.L_TID != null && p.L_TID.ToString().ToLower().Contains(search.ToLower())
                            ||
                              p.C_EventTime != null && p.C_EventTime.ToLower().Contains(search.ToLower())
                              ||
                                p.C_Name != null && p.C_Name.ToLower().Contains(search.ToLower())
                                                              ||
                                p.C_Office != null && p.C_Office.ToLower().Contains(search.ToLower())
                                 ||
                                p.C_Place != null && p.C_Place.ToLower().Contains(search.ToLower())

                                || (p.C_IPAddr != null && p.C_IPAddr.ToLower().Contains(search.ToLower()))


                    )
                )

                );

            return results;
        }
    }

    public class Devices_StatusCount_ResultSet
    {
        public static List<BLL_UNIS.ViewModels.DevicesStatusCount> GetResult(string search, string sortOrder, int start, int length, List<BLL_UNIS.ViewModels.DevicesStatusCount> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<BLL_UNIS.ViewModels.DevicesStatusCount> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<BLL_UNIS.ViewModels.DevicesStatusCount> FilterResult(string search, List<BLL_UNIS.ViewModels.DevicesStatusCount> dtResult)
        {
            IQueryable<BLL_UNIS.ViewModels.DevicesStatusCount> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.L_TID != null && p.L_TID.ToString().ToLower().Contains(search.ToLower())
                            ||
                              p.C_EventTime != null && p.C_EventTime.ToLower().Contains(search.ToLower())
                              ||
                                p.C_Name != null && p.C_Name.ToLower().Contains(search.ToLower())
                                                              ||
                                p.C_Office != null && p.C_Office.ToLower().Contains(search.ToLower())
                                 ||
                                p.C_Place != null && p.C_Place.ToLower().Contains(search.ToLower())

                                || (p.C_IPAddr != null && p.C_IPAddr.ToLower().Contains(search.ToLower()))


                    )
                )

                );

            return results;
        }
    }


    public class UserTrackingResultSet
    {
        public static List<ViewModels.UserTracking> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.UserTracking> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.UserTracking> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.UserTracking> FilterResult(string search, List<ViewModels.UserTracking> dtResult)
        {
            IQueryable<ViewModels.UserTracking> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            (p.C_Date != null && p.C_Date.ToLower().Contains(search.ToLower()))
                        || (p.C_Unique != null && p.C_Unique.ToLower().Contains(search.ToLower()))
                        || (p.C_Name != null && p.C_Name.ToLower().Contains(search.ToLower()))
                        || (p.TerminalName != null && p.TerminalName.ToLower().Contains(search.ToLower()))
                        || (p.C_Time != null && p.C_Time.ToLower().Contains(search.ToLower()))


                    )
                )

                );

            return results;
        }
    }

    public class DeviceTrackingResultSet
    {
        public static List<ViewModels.DevicesTracking> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.DevicesTracking> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.DevicesTracking> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.DevicesTracking> FilterResult(string search, List<ViewModels.DevicesTracking> dtResult)
        {
            IQueryable<ViewModels.DevicesTracking> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.Name != null && p.Name.ToLower().Contains(search.ToLower())
                        || (p.C_Unique != null && p.C_Unique.ToLower().Contains(search.ToLower()))
                        || (p.branchCode != null && p.branchCode.ToString().ToLower().Contains(search.ToLower()))
                        || (p.branchName != null && p.branchName.ToLower().Contains(search.ToLower()))
                        || (p.Status != null && p.Status.ToLower().Contains(search.ToLower()))
                        || (p.L_TID != null && p.L_TID.HasValue && p.L_TID.Value.ToString().ToLower().Contains(search.ToLower()))

                    )
                )



                );

            return results;
        }

    }

}