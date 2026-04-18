using MvcApplication1.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcApplication1
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            #region Initializing-Global-Variables

            //HttpContext.Current.Session["GV_EmployeeId"] = "";
            //GlobalVariables.GV_PendingLeavesCountHR = "0";
            //GlobalVariables.GV_PendingLeavesCountLM = "0";
            //HttpContext.Current.Session["GV_EmployeeCode"] = "";
            //HttpContext.Current.Session["GV_EmployeeName"] = "";
            //HttpContext.Current.Session["GV_EmployeePhoto"] = "user.png";
            //HttpContext.Current.Session["GV_EmployeeAccessGroupID"] = "";
            //HttpContext.Current.Session["GV_EmployeeCampusID"] = "";
            //HttpContext.Current.Session["GV_EmployeeCampusCode"] = "";
            //GlobalVariables.GV_EmployeeFunctionSLSID = "";
            //GlobalVariables.GV_StudentShiftMEID = "";
            //GlobalVariables.GV_StudentShiftGroupABID = "";
            //GlobalVariables.GV_RoleIsSuperHR = false;

            GlobalVariables.GV_Rpt01Perm = false;
            GlobalVariables.GV_Rpt02Perm = false;
            GlobalVariables.GV_Rpt03Perm = false;
            GlobalVariables.GV_Rpt04Perm = false;
 

            //GlobalVariables.GV_EmployeeId = "";
            //GlobalVariables.GV_PendingLeavesCountHR = "0";
            //GlobalVariables.GV_PendingLeavesCountLM = "0";
            //GlobalVariables.GV_EmployeeCode = "";
            //GlobalVariables.GV_EmployeeName = "";
            //GlobalVariables.GV_EmployeePhoto = "user.png";
            //GlobalVariables.GV_EmployeeAccessGroupID = "";
            //GlobalVariables.GV_EmployeeCampusID = "";
            //GlobalVariables.GV_EmployeeCampusCode = "";
            //GlobalVariables.GV_EmployeeFunctionSLSID = "";
            //GlobalVariables.GV_StudentShiftMEID = "";
            //GlobalVariables.GV_StudentShiftGroupABID = "";
            //GlobalVariables.GV_RoleIsSuperHR = false;

            //GlobalVariables.GV_Rpt01Perm = false;
            //GlobalVariables.GV_Rpt02Perm = false;
            //GlobalVariables.GV_Rpt03Perm = false;
            //GlobalVariables.GV_Rpt04Perm = false;

            #endregion

            // EPPlus 5+ requires explicit license context before any ExcelPackage use.
            // Use LicenseContext.Commercial if your organization has a commercial EPPlus license.
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Session_Start()
        {
            HttpContext.Current.Session["GV_AccessValidationCode"] = "";
            HttpContext.Current.Session["GV_AccessDeniedToOrganization"] = "false";
            HttpContext.Current.Session["GV_IsRoasterAllowed"] = "false";
            HttpContext.Current.Session["GV_SiteTitle"] = "";

            HttpContext.Current.Session["GV_EmployeeCode"] = "";
            HttpContext.Current.Session["GV_EmployeeId"] = "";

            HttpContext.Current.Session["GV_EmployeeName"] = "";
            HttpContext.Current.Session["GV_EmployeePhoto"] = "user.png";
            HttpContext.Current.Session["GV_EmployeeAccessGroupID"] = "";
            HttpContext.Current.Session["GV_EmployeeAccessGroupCode"] = "";
            HttpContext.Current.Session["GV_EmployeeCampusID"] = "";
            HttpContext.Current.Session["GV_EmployeeCampusCode"] = "";

            HttpContext.Current.Session["GV_EmployeeFunctionSLSID"] = "";
            HttpContext.Current.Session["GV_StudentShiftMEID"] = "";
            HttpContext.Current.Session["GV_StudentShiftGroupABID"] = "";

            HttpContext.Current.Session["GV_RoleIsSuperHR"] = "false";
            HttpContext.Current.Session["GV_PendingLeavesCountHR"] = "0";
            HttpContext.Current.Session["GV_PendingLeavesCountLM"] = "0";

            HttpContext.Current.Session["GV_TotalRegisteredActiveEmployees"] = "0";
            HttpContext.Current.Session["GV_EmployeesMissingInfoCountHR"] = "0";
            HttpContext.Current.Session["GV_BadgeSumForHR"] = "0";
        }

        protected void Application_BeginRequest()
        {
            #region Leaves-Counter

            //int emp_id = 0, missing_info_counter_hr = 0;

            //get pending leaves count for current user
            //if (GlobalVariables.GV_EmployeeId != null && GlobalVariables.GV_EmployeeId.ToString() != "" && GlobalVariables.GV_EmployeeAccessGroupID != null && GlobalVariables.GV_EmployeeAccessGroupID.ToString() != "")
            //{
            //    emp_id = int.Parse(GlobalVariables.GV_EmployeeId);

            //    //check for HR
            //    if (GlobalVariables.GV_EmployeeAccessGroupID == "1") //check for HR
            //    {
            //        int leaves_counter_hr = BLL.ADMIN.Utility.GetPendingLeavesApprovalForHR(emp_id);
            //        if (leaves_counter_hr > 0)
            //        {
            //            GlobalVariables.GV_PendingLeavesCountHR = leaves_counter_hr.ToString();
            //        }
            //        else
            //        {
            //            GlobalVariables.GV_PendingLeavesCountHR = "0";
            //        }
            //        //sum for HR
            //        GlobalVariables.GV_BadgeSumForHR = (int.Parse(GlobalVariables.GV_PendingLeavesCountHR) + int.Parse(GlobalVariables.GV_EmployeesMissingInfoCountHR)).ToString();


            //        //employees - missing info counter
            //        missing_info_counter_hr = BLL.ADMIN.Utility.GetEmployeesMissingInfoCountForHR();
            //        if (missing_info_counter_hr > 0)
            //        {
            //            GlobalVariables.GV_EmployeesMissingInfoCountHR = missing_info_counter_hr.ToString();
            //        }
            //        else
            //        {
            //            GlobalVariables.GV_EmployeesMissingInfoCountHR = "0";
            //        }

            //    }
            //    else if (GlobalVariables.GV_EmployeeAccessGroupID == "2") //check for LM
            //    {
            //        int leaves_counter_lm = BLL.ADMIN.Utility.GetPendingLeavesApprovalForLM(emp_id);
            //        if (leaves_counter_lm > 0)
            //        {
            //            GlobalVariables.GV_PendingLeavesCountLM = leaves_counter_lm.ToString();
            //        }
            //        else
            //        {
            //            GlobalVariables.GV_PendingLeavesCountLM = "0";
            //        }
            //    }

            //}

            #endregion


            #region Access-Code

            //DateTime dtAccessCode = DateTime.Now;
            //GlobalVariables.GV_AccessValidationCode = (dtAccessCode.Year - 1111 - (dtAccessCode.Month + dtAccessCode.Day)).ToString();

            #endregion

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}