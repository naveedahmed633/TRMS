using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.ViewModel
{

    public static class GlobalVariables
    {
        //public static string GV_AccessValidationCode { get; set; }
        public static string GV_AccessValidationCode
        {
            get
            {
                if (HttpContext.Current.Session["GV_AccessValidationCode"] != null && HttpContext.Current.Session["GV_AccessValidationCode"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_AccessValidationCode"].ToString();
                }
                else
                {
                    DateTime dtAccessCode = DateTime.Now;
                    return (dtAccessCode.Year - 1111 - (dtAccessCode.Month + dtAccessCode.Day)).ToString();
                }
            }
            set { HttpContext.Current.Session["GV_AccessValidationCode"] = value; }
        }


        //public static bool GV_AccessDeniedToOrganization { get; set; }
        public static bool GV_AccessDeniedToOrganization
        {
            get
            {
                if (HttpContext.Current.Session["GV_AccessDeniedToOrganization"] != null && HttpContext.Current.Session["GV_AccessDeniedToOrganization"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_AccessDeniedToOrganization"].ToString().ToLower() == "true" ? true : false;
                }
                else
                {
                    HttpContext.Current.Session["GV_AccessDeniedToOrganization"] = "false";
                    return HttpContext.Current.Session["GV_AccessDeniedToOrganization"].ToString().ToLower() == "true" ? true : false; ;
                }
            }
            set { HttpContext.Current.Session["GV_AccessDeniedToOrganization"] = value; }
        }


        //public static bool GV_IsRoasterAllowed { get; set; }
        public static bool GV_IsRoasterAllowed
        {
            get
            {
                if (HttpContext.Current.Session["GV_IsRoasterAllowed"] != null && HttpContext.Current.Session["GV_IsRoasterAllowed"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_IsRoasterAllowed"].ToString().ToLower() == "true" ? true : false;
                }
                else
                {
                    HttpContext.Current.Session["GV_IsRoasterAllowed"] = "false";
                    return HttpContext.Current.Session["GV_IsRoasterAllowed"].ToString().ToLower() == "true" ? true : false; ;
                }
            }
            set { HttpContext.Current.Session["GV_IsRoasterAllowed"] = value; }
        }


        //public static string GV_SiteTitle { get; set; }
        public static string GV_SiteTitle
        {
            get
            {
                if (HttpContext.Current.Session["GV_SiteTitle"] != null && HttpContext.Current.Session["GV_SiteTitle"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_SiteTitle"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_SiteTitle"] = "";
                    return HttpContext.Current.Session["GV_SiteTitle"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_SiteTitle"] = value; }
        }

        public static string GV_SessionStartDay { get; set; }
        public static string GV_SessionStartMonth { get; set; }
        public static string GV_SessionEndDay { get; set; }
        public static string GV_SessionEndMonth { get; set; }

        //public static string GV_EmployeeCode { get; set; }
        public static string GV_EmployeeCode
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeCode"] != null && HttpContext.Current.Session["GV_EmployeeCode"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeCode"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeCode"] = "";
                    return HttpContext.Current.Session["GV_EmployeeCode"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeCode"] = value; }
        }

        //public static string GV_EmployeeId { get; set; }
        public static string GV_EmployeeId
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeId"] != null && HttpContext.Current.Session["GV_EmployeeId"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeId"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeId"] = "1";
                    return HttpContext.Current.Session["GV_EmployeeId"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeId"] = value; }
        }

        //public static string GV_EmployeeName { get; set; }
        public static string GV_EmployeeName
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeName"] != null && HttpContext.Current.Session["GV_EmployeeName"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeName"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeName"] = "";
                    return HttpContext.Current.Session["GV_EmployeeName"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeName"] = value; }
        }

        //public static string GV_EmployeePhoto { get; set; }
        public static string GV_EmployeePhoto
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeePhoto"] != null && HttpContext.Current.Session["GV_EmployeePhoto"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeePhoto"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeePhoto"] = "user.png";
                    return HttpContext.Current.Session["GV_EmployeePhoto"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeePhoto"] = value; }
        }

        //public static string GV_EmployeeAccessGroupID { get; set; }
        public static string GV_EmployeeAccessGroupID
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeAccessGroupID"] != null && HttpContext.Current.Session["GV_EmployeeAccessGroupID"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeAccessGroupID"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeAccessGroupID"] = "3";
                    return HttpContext.Current.Session["GV_EmployeeAccessGroupID"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeAccessGroupID"] = value; }
        }

        //public static string GV_EmployeeAccessGroupCode { get; set; }
        public static string GV_EmployeeAccessGroupCode
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeAccessGroupCode"] != null && HttpContext.Current.Session["GV_EmployeeAccessGroupCode"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeAccessGroupCode"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeAccessGroupCode"] = "";
                    return HttpContext.Current.Session["GV_EmployeeAccessGroupCode"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeAccessGroupCode"] = value; }
        }


        public static bool GV_Rpt01Perm { get; set; }
        public static bool GV_Rpt02Perm { get; set; }
        public static bool GV_Rpt03Perm { get; set; }
        public static bool GV_Rpt04Perm { get; set; }

        //public static string GV_EmployeeCampusID { get; set; }
        public static string GV_EmployeeCampusID
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeCampusID"] != null && HttpContext.Current.Session["GV_EmployeeCampusID"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeCampusID"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeCampusID"] = "";
                    return HttpContext.Current.Session["GV_EmployeeCampusID"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeCampusID"] = value; }
        }

        //public static string GV_EmployeeCampusCode { get; set; }
        public static string GV_EmployeeCampusCode
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeCampusCode"] != null && HttpContext.Current.Session["GV_EmployeeCampusCode"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeCampusCode"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeCampusCode"] = "";
                    return HttpContext.Current.Session["GV_EmployeeCampusCode"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeCampusCode"] = value; }
        }

        //public static string GV_EmployeeFunctionSLSID { get; set; }
        public static string GV_EmployeeFunctionSLSID
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeeFunctionSLSID"] != null && HttpContext.Current.Session["GV_EmployeeFunctionSLSID"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeeFunctionSLSID"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeeFunctionSLSID"] = "";
                    return HttpContext.Current.Session["GV_EmployeeFunctionSLSID"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeeFunctionSLSID"] = value; }
        }

        //public static string GV_StudentShiftMEID { get; set; }
        public static string GV_StudentShiftMEID
        {
            get
            {
                if (HttpContext.Current.Session["GV_StudentShiftMEID"] != null && HttpContext.Current.Session["GV_StudentShiftMEID"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_StudentShiftMEID"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_StudentShiftMEID"] = "";
                    return HttpContext.Current.Session["GV_StudentShiftMEID"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_StudentShiftMEID"] = value; }
        }

        //public static string GV_StudentShiftGroupABID { get; set; }
        public static string GV_StudentShiftGroupABID
        {
            get
            {
                if (HttpContext.Current.Session["GV_StudentShiftGroupABID"] != null && HttpContext.Current.Session["GV_StudentShiftGroupABID"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_StudentShiftGroupABID"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_StudentShiftGroupABID"] = "";
                    return HttpContext.Current.Session["GV_StudentShiftGroupABID"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_StudentShiftGroupABID"] = value; }
        }

        //public static bool GV_RoleIsSuperHR { get; set; }
        public static bool GV_RoleIsSuperHR
        {
            get
            {
                if (HttpContext.Current.Session["GV_RoleIsSuperHR"] != null && HttpContext.Current.Session["GV_RoleIsSuperHR"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_RoleIsSuperHR"].ToString().ToLower() == "true" ? true : false;
                }
                else
                {
                    HttpContext.Current.Session["GV_RoleIsSuperHR"] = "false";
                    return HttpContext.Current.Session["GV_RoleIsSuperHR"].ToString().ToLower() == "true" ? true : false; ;
                }
            }
            set { HttpContext.Current.Session["GV_RoleIsSuperHR"] = value; }
        }


        //public static string GV_PendingLeavesCountHR { get; set; }
        public static string GV_PendingLeavesCountHR
        {
            get
            {
                if (HttpContext.Current.Session["GV_PendingLeavesCountHR"] != null && HttpContext.Current.Session["GV_PendingLeavesCountHR"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_PendingLeavesCountHR"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_PendingLeavesCountHR"] = "0";
                    return HttpContext.Current.Session["GV_PendingLeavesCountHR"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_PendingLeavesCountHR"] = value; }
        }

        //public static string GV_PendingLeavesCountLM { get; set; }
        public static string GV_PendingLeavesCountLM
        {
            get
            {
                if (HttpContext.Current.Session["GV_PendingLeavesCountLM"] != null && HttpContext.Current.Session["GV_PendingLeavesCountLM"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_PendingLeavesCountLM"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_PendingLeavesCountLM"] = "0";
                    return HttpContext.Current.Session["GV_PendingLeavesCountLM"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_PendingLeavesCountLM"] = value; }
        }

        public static string GV_DepartmentPerformanceTill { get; set; }


        //public static int GV_TotalRegisteredActiveEmployees { get; set; }
        public static int GV_TotalRegisteredActiveEmployees
        {
            get
            {
                if (HttpContext.Current.Session["GV_TotalRegisteredActiveEmployees"] != null && HttpContext.Current.Session["GV_TotalRegisteredActiveEmployees"].ToString().Length > 0)
                {
                    return int.Parse(HttpContext.Current.Session["GV_TotalRegisteredActiveEmployees"].ToString());
                }
                else
                {
                    HttpContext.Current.Session["GV_TotalRegisteredActiveEmployees"] = "0";
                    return int.Parse(HttpContext.Current.Session["GV_TotalRegisteredActiveEmployees"].ToString());
                }
            }
            set { HttpContext.Current.Session["GV_TotalRegisteredActiveEmployees"] = value; }
        }

        public static string GV_DepartmentPerformanceCount { get; set; }
        public static string GV_DepartmentPerformancePercent { get; set; }
        public static string GV_DepartmentAttendancePercent { get; set; }
        public static string GV_DepartmentAttendanceNames { get; set; }

        public static string GV_DepartmentTodaysCount { get; set; }
        public static string GV_DepartmentTodaysPercent { get; set; }


        //public static string GV_EmployeesMissingInfoCountHR { get; set; }
        public static string GV_EmployeesMissingInfoCountHR
        {
            get
            {
                if (HttpContext.Current.Session["GV_EmployeesMissingInfoCountHR"] != null && HttpContext.Current.Session["GV_EmployeesMissingInfoCountHR"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_EmployeesMissingInfoCountHR"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_EmployeesMissingInfoCountHR"] = "0";
                    return HttpContext.Current.Session["GV_EmployeesMissingInfoCountHR"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_EmployeesMissingInfoCountHR"] = value; }
        }


        ///
        public static string GV_Langauge
        {
            get
            {
                if (HttpContext.Current.Session["GV_Langauge"] != null && HttpContext.Current.Session["GV_Langauge"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_Langauge"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_Langauge"] = "En";
                    return HttpContext.Current.Session["GV_Langauge"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_Langauge"] = value; }
        }



        //public static string GV_BadgeSumForHR { get; set; }
        public static string GV_BadgeSumForHR
        {
            get
            {
                if (HttpContext.Current.Session["GV_BadgeSumForHR"] != null && HttpContext.Current.Session["GV_BadgeSumForHR"].ToString().Length > 0)
                {
                    return HttpContext.Current.Session["GV_BadgeSumForHR"].ToString();
                }
                else
                {
                    HttpContext.Current.Session["GV_BadgeSumForHR"] = "0";
                    return HttpContext.Current.Session["GV_BadgeSumForHR"].ToString();
                }
            }
            set { HttpContext.Current.Session["GV_BadgeSumForHR"] = value; }
        }

        public static string CheckNULLValidation(string value)
        {
            string strReturn = "";

            if (value != null && value.ToString() != "")
            {
                strReturn = value;
            }
            else
            {
                strReturn = "";
            }

            return strReturn;
        }

        public static string CheckCampusCodeValidation(string value)
        {
            string strReturn = "";

            if (value != null && value.ToString() != "")
            {
                strReturn = value;
            }
            else
            {
                strReturn = "-";
            }

            return strReturn;
        }

        public static string CheckSuperHRValidation(bool value)
        {
            string strReturn = "";

            if (value)
            {
                strReturn = "(Super HR)";
            }
            else
            {
                strReturn = "";
            }

            return strReturn;
        }

        private static string GetFallbackStringResource(string valueId, string language)
        {
            bool isArabic = string.Equals(language, "Ar", StringComparison.OrdinalIgnoreCase);
            switch (valueId)
            {
                case "report.fromdate":
                    return isArabic ? "من التاريخ" : "From Date";
                case "report.todate":
                    return isArabic ? "إلى التاريخ" : "To Date";
                case "report.terminalin":
                    return isArabic ? "جهاز الدخول" : "Terminal In";
                case "report.statusin":
                    return isArabic ? "حالة الدخول" : "Status In";
                case "report.terminalout":
                    return isArabic ? "جهاز الخروج" : "Terminal Out";
                case "report.statusout":
                    return isArabic ? "حالة الخروج" : "Status Out";
                default:
                    return valueId;
            }
        }

        public static string GetStringResource(string Value)
        {

            string stringtext = Value;
            using (Context db = new Context())
            {
                try
                {
                    var Getvalue = (db.string_resource.Where(m => m.ValueID == Value)).FirstOrDefault();
                    if (Getvalue != null)
                    {
                        if (GV_Langauge == "En")
                        {
                            stringtext = Getvalue.ValueTextEn;
                        }
                        else if (GV_Langauge == "Ar")
                        {
                            stringtext = Getvalue.ValueTextAr;
                        }

                    }
                }
                catch (Exception ex)
                {
                    stringtext = ex.Message;
                }
            }

            // Fallback for known report labels when DB value is missing/empty.
            if (string.IsNullOrWhiteSpace(stringtext) || stringtext == Value)
            {
                stringtext = GetFallbackStringResource(Value, GV_Langauge);
            }

            return stringtext;
        }


    }

    public static class GlobalVariablesPast
    {
        public static string GV_AccessValidationCode { get; set; }
        public static bool GV_AccessDeniedToOrganization { get; set; }

        public static bool GV_IsRoasterAllowed { get; set; }

        public static string GV_SiteTitle { get; set; }

        public static string GV_SessionStartDay { get; set; }
        public static string GV_SessionStartMonth { get; set; }
        public static string GV_SessionEndDay { get; set; }
        public static string GV_SessionEndMonth { get; set; }

        public static string GV_EmployeeCode { get; set; }
        public static string GV_EmployeeId { get; set; }
        public static string GV_EmployeeName { get; set; }
        public static string GV_EmployeePhoto { get; set; }
        public static string GV_EmployeeAccessGroupID { get; set; }
        public static string GV_EmployeeAccessGroupCode { get; set; }

        public static bool GV_Rpt01Perm { get; set; }
        public static bool GV_Rpt02Perm { get; set; }
        public static bool GV_Rpt03Perm { get; set; }
        public static bool GV_Rpt04Perm { get; set; }

        public static string GV_EmployeeCampusID { get; set; }
        public static string GV_EmployeeCampusCode { get; set; }
        public static string GV_EmployeeFunctionSLSID { get; set; }
        public static string GV_StudentShiftMEID { get; set; }
        public static string GV_StudentShiftGroupABID { get; set; }
        public static bool GV_RoleIsSuperHR { get; set; }


        public static string GV_PendingLeavesCountHR { get; set; }
        public static string GV_PendingLeavesCountLM { get; set; }

        public static string GV_DepartmentPerformanceTill { get; set; }
        public static int GV_TotalRegisteredActiveEmployees { get; set; }
        public static string GV_DepartmentPerformanceCount { get; set; }
        public static string GV_DepartmentPerformancePercent { get; set; }
        public static string GV_DepartmentAttendancePercent { get; set; }
        public static string GV_DepartmentAttendanceNames { get; set; }

        public static string GV_DepartmentTodaysCount { get; set; }
        public static string GV_DepartmentTodaysPercent { get; set; }

        public static string CheckNULLValidation(string value)
        {
            string strReturn = "";

            if (value != null && value.ToString() != "")
            {
                strReturn = value;
            }
            else
            {
                strReturn = "";
            }

            return strReturn;
        }

        public static string CheckCampusCodeValidation(string value)
        {
            string strReturn = "";

            if (value != null && value.ToString() != "")
            {
                strReturn = value;
            }
            else
            {
                strReturn = "-";
            }

            return strReturn;
        }

        public static string CheckSuperHRValidation(bool value)
        {
            string strReturn = "";

            if (value)
            {
                strReturn = "(Super HR)";
            }
            else
            {
                strReturn = "";
            }

            return strReturn;
        }

        public static string GV_EmployeesMissingInfoCountHR { get; set; }

        public static string GV_BadgeSumForHR { get; set; }




    }
}