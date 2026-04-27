using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using TimeTune;
using ViewModels;

namespace MvcApplication1
{
    public class RawAttendanceResultSet
    {
        public static List<ReportRawAttendanceLog> GetResult(string search, string sortOrder, int start, int length, List<ReportRawAttendanceLog> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ReportRawAttendanceLog> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ReportRawAttendanceLog> FilterResult(string search, List<ReportRawAttendanceLog> dtResult)
        {
            IQueryable<ReportRawAttendanceLog> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                           (p.employee_code != null && p.employee_code.ToLower().Contains(search.ToLower()))
                        || (p.employee_first_name != null && p.employee_first_name.ToLower().Contains(search.ToLower()))
                        || (p.employee_last_name != null && p.employee_last_name.ToLower().Contains(search.ToLower()))
                        || (p.employee_code.ToLower().Contains(search.ToLower()))
                        || p.date != null && p.date.ToLower().Contains(search.ToLower())
                        || (p.time != null && p.time.ToLower().Contains(search.ToLower()))

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
}