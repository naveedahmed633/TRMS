using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViewModels;
using System.Web.UI.WebControls;

namespace MvcApplication1
{
    public class MonthlyReportResultSet
    {
        public static List<MonthlyReport> GetResult(string search, string sortOrder, int start, int length, List<MonthlyReport> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<MonthlyReport> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<MonthlyReport> FilterResult(string search, List<MonthlyReport> dtResult)
        {
            IQueryable<MonthlyReport> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.employeeCode != null && p.employeeCode.Contains(search)
                        || (p.present != null && p.present.ToString().Contains(search))
                        || (p.ontime.ToString().Contains(search))
                        || (p.late != null && p.late.ToString().Contains(search))
                        || (p.earlyGone != null && p.earlyGone.ToString().Contains(search))
                        || (p.absent != null && p.absent.ToString().Contains(search))

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