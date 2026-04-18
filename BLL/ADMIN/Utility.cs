using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ADMIN
{
    public static class Utility
    {
        public static int GetEmployeeIdByEmployeeCode(string emp_code)
        {
            int employee_id = 0;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        employee_id = data_emp.EmployeeId;
                    }
                }
                catch (Exception)
                {
                    employee_id = -1;
                }
            }

            return employee_id;
        }

        public static int GetEmployeeAccessGroupIDByEmployeeCode(string emp_code)
        {
            int employee_ag = 0;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        employee_ag = data_emp.access_group.AccessGroupId;
                    }
                }
                catch (Exception)
                {
                    employee_ag = -1;
                }
            }

            return employee_ag;
        }

        public static string GetEmployeeAccessGroupCodeByEmployeeCode(string emp_code)
        {
            string empAGCode = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        empAGCode = data_emp.access_group.name;
                    }
                }
                catch (Exception)
                {
                    empAGCode = "TimeTuneEMP";
                }
            }

            return empAGCode;
        }

        public static bool GetEmployeeSuperHRAccessByEmployeeCode(string emp_code)
        {
            bool employee_shr = false;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && e.employee_code == emp_code && e.is_super_hr).FirstOrDefault();
                    if (data_emp != null)
                    {
                        employee_shr = data_emp.is_super_hr;
                    }
                }
                catch (Exception)
                {
                    employee_shr = false;
                }
            }

            return employee_shr;
        }

        public static string GetEmployeeNameByEmployeeCode(string emp_code)
        {
            string strEmpName = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        strEmpName = data_emp.first_name + " " + data_emp.last_name;
                    }
                }
                catch (Exception)
                {

                }
            }

            return strEmpName;
        }

        public static string GetEmployeeSiteTitleByEmployeeCode(string emp_code)
        {
            string strEmpSite = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var dbSiteTitle = db.site_status.Where(p => p.Id == data_emp.site_id).FirstOrDefault();
                        if (dbSiteTitle != null)
                        {
                            strEmpSite = dbSiteTitle.SiteText;
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return strEmpSite;
        }

        public static int GetEmployeeSiteIDByEmployeeCode(string emp_code)
        {
            int iSiteID = 0;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var dbSiteID = db.site_status.Where(p => p.Id == data_emp.site_id).FirstOrDefault();
                        if (dbSiteID != null)
                        {
                            iSiteID = dbSiteID.Id;
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return iSiteID;
        }


        public static string GetEmployeeNameByEmployeePhoto(string emp_code)
        {
            string strEmpPhoto = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        strEmpPhoto = data_emp.photograph ?? "user.png";
                    }
                }
                catch (Exception)
                {

                }
            }

            return strEmpPhoto;
        }

        public static bool GetRoasterAllowedStatusByEmployeeCode(string emp_code)
        {
            bool bRstrAllowed = false;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.Group != null && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        bRstrAllowed = true;
                    }
                }
                catch (Exception)
                {
                    bRstrAllowed = false;
                }
            }

            return bRstrAllowed;
        }

        public static string GetEmployeeCampusIDByEmployeeCode(string emp_code)
        {
            string strEmpCampusId = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        strEmpCampusId = data_emp.campus_id.ToString();
                    }
                }
                catch (Exception)
                {

                }
            }

            return strEmpCampusId;
        }

        public static string GetEmployeeCampusCodeByEmployeeCode(string emp_code)
        {
            string strEmpCampusCode = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var data_cmp = db.organization_campus.Where(e => e.Id == data_emp.campus_id).FirstOrDefault();
                        if (data_cmp != null)
                        {
                            strEmpCampusCode = data_cmp.CampusCode.ToString();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return strEmpCampusCode;
        }

        public static string GetEmployeeFunctionSLSByEmployeeCode(string emp_code)
        {
            string strEmpFunct = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {


                        strEmpFunct = data_emp.function.FunctionId + ":" + data_emp.function.name.ToString().ToLower();
                    }
                }
                catch (Exception)
                {

                }
            }

            return strEmpFunct;
        }

        public static string GetStudentShiftMEByEmployeeCode(string emp_code)
        {
            string strStdShift = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        strStdShift = data_emp.program_shift_id.ToString() + "-";
                        var dbShift = db.organization_program_shift.Where(s => s.Id == data_emp.program_shift_id).FirstOrDefault();
                        if (dbShift != null)
                        {
                            strStdShift = data_emp.program_shift_id + ":" + dbShift.ProgramShiftName;
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            return strStdShift;
        }


        public static string GetStudentShiftGroupByEmployeeCode(string emp_code)
        {
            string strStdSGroup = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        strStdSGroup = data_emp.region.RegionId + ":" + data_emp.region.name;
                    }
                }
                catch (Exception)
                {

                }
            }

            return strStdSGroup;
        }

        public static int GetPendingLeavesApprovalForHR(int emp_id)
        {
            int leaves_pending = 0;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    //find pending leaves count
                    var data_leaves_app = db.leave_application.Where(l => l.IsActive && l.LeaveStatusId == 1).ToList();
                    if (data_leaves_app != null)
                    {
                        leaves_pending = data_leaves_app.Count;
                    }
                }
                catch (Exception)
                {
                    leaves_pending = 0;
                }
            }

            return leaves_pending;
        }

        public static int GetPendingLeavesApprovalForLM(int apprId)
        {
            int leaves_pending = 0;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    //find pending leaves count
                    var data_leaves_app = db.leave_application.Where(l => l.IsActive && l.ApproverId == apprId && l.LeaveStatusId == 1).ToList();
                    if (data_leaves_app != null)
                    {
                        leaves_pending = data_leaves_app.Count;
                    }
                }
                catch (Exception)
                {
                    leaves_pending = 0;
                }
            }

            return leaves_pending;
        }

        public static int GetEmployeesMissingInfoCountForHR()
        {
            int missing_info_count = 0;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    //find pending leaves count
                    var data_missing_info = db.employee.Where(m => m.employee_code != "000000" && (m.function == null || m.region == null || m.location == null || m.department == null || m.designation == null)).ToList();
                    if (data_missing_info != null && data_missing_info.Count > 0)
                    {
                        missing_info_count = data_missing_info.Count;
                    }
                }
                catch (Exception)
                {
                    missing_info_count = 0;
                }
            }

            return missing_info_count;
        }

        public static string GetEmployeeLanguage(string emp_code)
        {
         
            string emp_lan = "";

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                     
                        emp_lan = data_emp.select_langauge;
                    }
                }
                catch (Exception)
                {
                    emp_lan = "En";
                }
            }

            return emp_lan;
        }




    }
}
