using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Web.Helpers;
using System.Web.Routing;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using MvcApplication1.ViewModel;
using System.Configuration;
using System.Collections;

namespace MvcApplication1.Controllers
{
    public class HandleAntiForgeryError : ActionFilterAttribute, IExceptionFilter
    {
        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception as HttpAntiForgeryException;
            if (exception != null)
            {
                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Account";
                routeValues["action"] = "Login";
                filterContext.Result = new RedirectToRouteResult(routeValues);
                filterContext.ExceptionHandled = true;
            }
        }

        #endregion
    }
    [Authorize]
    public class AccountController : Controller
    {
        # region Login

        [AllowAnonymous]
        [HandleAntiForgeryError]
        public ActionResult Login(string returnUrl)
        {

            
            //hide license key form assuming existing license key is ok/valid
            ViewBag.LoginFormDisplay = "block";
            ViewBag.LicenseFormDisplay = "none";


            // If the user is already logged in redirect him to the
            // home page.
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            string url = Request.Url.AbsoluteUri.ToString().ToLower();
            int access = TimeTune.LeadsValidation.checkAccessValidation(url);
            if (access > 0)
            {
                if ((access % int.Parse(GlobalVariables.GV_AccessValidationCode)) == 0)
                {
                    //hide license key form as existing license key is ok/valid
                    ViewBag.LoginFormDisplay = "block";
                    ViewBag.LicenseFormDisplay = "none";
                }
                else
                {
                    //show license key form as existing license key is expired/invalid
                    ViewBag.LoginFormDisplay = "none";
                    ViewBag.LicenseFormDisplay = "block";
                }
            }
            //else
            //{
            //    //show license key form as existing license key is expired/invalid
            //    ViewBag.LoginFormDisplay = "none";
            //    ViewBag.LicenseFormDisplay = "block";
            //}

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult changelanguage(string value)
        {

            ViewModel.GlobalVariables.GV_Langauge = value;
            return Json(new { success = true });


        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HandleAntiForgeryError]
        public ActionResult Login(BLL.ViewModels.LoginModel model, string returnUrl)
        {
            int emp_id = 0; int leaves_counter = 0, leaves_counter_lm = 0, missing_info_counter_hr = 0;
            string language = "En";
            if (ModelState.IsValid)
            {
                //validate-license-key - exemted for SUPER_ADMIN, Sudo, RESCO_ADMIN 
                string url = Request.Url.AbsoluteUri.ToString().ToLower();
                int access = TimeTune.LeadsValidation.checkAccessValidation(url);
                if (model.UserName.ToUpper() == "SUPER_ADMIN" || model.UserName.ToUpper() == "Sudo" || model.UserName.ToUpper() == "RESCO_ADMIN")
                {
                    #region SUPER_ADMIN___Sudo___RESCO_ADMIN

                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        emp_id = BLL.ADMIN.Utility.GetEmployeeIdByEmployeeCode(model.UserName);
                        language = BLL.ADMIN.Utility.GetEmployeeLanguage(model.UserName);
                        if (emp_id > 0)
                        {
                            GlobalVariables.GV_EmployeeCode = model.UserName.ToString();
                            GlobalVariables.GV_EmployeeId = emp_id.ToString();
                            GlobalVariables.GV_Langauge = language.ToString();
                            GlobalVariables.GV_EmployeeName = BLL.ADMIN.Utility.GetEmployeeNameByEmployeeCode(model.UserName);
                            GlobalVariables.GV_EmployeeAccessGroupID = BLL.ADMIN.Utility.GetEmployeeAccessGroupIDByEmployeeCode(model.UserName).ToString();

                            //check for HR
                            leaves_counter = BLL.ADMIN.Utility.GetPendingLeavesApprovalForHR(emp_id);
                            if (leaves_counter > 0)
                            {
                                GlobalVariables.GV_PendingLeavesCountHR = leaves_counter.ToString();
                            }
                            else
                            {
                                GlobalVariables.GV_PendingLeavesCountHR = "0";
                            }

                            //check for LM
                            if (GlobalVariables.GV_EmployeeAccessGroupID == "2")
                            {
                                leaves_counter_lm = BLL.ADMIN.Utility.GetPendingLeavesApprovalForLM(emp_id);
                                if (leaves_counter_lm > 0)
                                {
                                    GlobalVariables.GV_PendingLeavesCountLM = leaves_counter_lm.ToString();
                                }
                                else
                                {
                                    GlobalVariables.GV_PendingLeavesCountLM = "0";
                                }
                            }

                            //employees missing info - count
                            if (GlobalVariables.GV_EmployeeAccessGroupID == "1")
                            {
                                missing_info_counter_hr = BLL.ADMIN.Utility.GetEmployeesMissingInfoCountForHR();
                                if (missing_info_counter_hr > 0)
                                {
                                    GlobalVariables.GV_EmployeesMissingInfoCountHR = missing_info_counter_hr.ToString();
                                }
                                else
                                {
                                    GlobalVariables.GV_EmployeesMissingInfoCountHR = "0";
                                }
                                //sum for HR
                                GlobalVariables.GV_BadgeSumForHR = (int.Parse(GlobalVariables.GV_PendingLeavesCountHR) + int.Parse(GlobalVariables.GV_EmployeesMissingInfoCountHR)).ToString();

                            }

                        }


                        FormsAuthentication.RedirectFromLoginPage(model.UserName, model.RememberMe);
                    }

                    #endregion

                    //incorrect user/pass
                    ViewBag.LoginFormDisplay = "block";
                    ViewBag.LicenseFormDisplay = "none";

                    ModelState.AddModelError("", "Incorrect Username or Password entered");

                }
                else if (access > 0 && (access % int.Parse(GlobalVariables.GV_AccessValidationCode)) == 0)
                {
                    #region Other-Users-Than-Super-Admin--Sudo--RESCO-ADMIN
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        emp_id = BLL.ADMIN.Utility.GetEmployeeIdByEmployeeCode(model.UserName);
                        language = BLL.ADMIN.Utility.GetEmployeeLanguage(model.UserName);
                        if (emp_id > 0)
                        {
                            GlobalVariables.GV_EmployeeCode = model.UserName.ToString();
                            GlobalVariables.GV_EmployeeId = emp_id.ToString();
                            GlobalVariables.GV_Langauge = language.ToString();
                            GlobalVariables.GV_EmployeeName = BLL.ADMIN.Utility.GetEmployeeNameByEmployeeCode(model.UserName);
                            GlobalVariables.GV_EmployeePhoto = BLL.ADMIN.Utility.GetEmployeeNameByEmployeePhoto(model.UserName);
                            GlobalVariables.GV_EmployeeAccessGroupID = BLL.ADMIN.Utility.GetEmployeeAccessGroupIDByEmployeeCode(model.UserName).ToString();
                            GlobalVariables.GV_EmployeeAccessGroupCode = BLL.ADMIN.Utility.GetEmployeeAccessGroupCodeByEmployeeCode(model.UserName).ToString();
                            GlobalVariables.GV_RoleIsSuperHR = BLL.ADMIN.Utility.GetEmployeeSuperHRAccessByEmployeeCode(model.UserName);
                            GlobalVariables.GV_EmployeeCampusID = BLL.ADMIN.Utility.GetEmployeeCampusIDByEmployeeCode(model.UserName).ToString();
                            GlobalVariables.GV_EmployeeCampusCode = BLL.ADMIN.Utility.GetEmployeeCampusCodeByEmployeeCode(model.UserName).ToString();
                            GlobalVariables.GV_EmployeeFunctionSLSID = BLL.ADMIN.Utility.GetEmployeeFunctionSLSByEmployeeCode(model.UserName).ToString();
                            GlobalVariables.GV_StudentShiftMEID = BLL.ADMIN.Utility.GetStudentShiftMEByEmployeeCode(model.UserName).ToString();
                            GlobalVariables.GV_StudentShiftGroupABID = BLL.ADMIN.Utility.GetStudentShiftGroupByEmployeeCode(model.UserName).ToString();
                            GlobalVariables.GV_IsRoasterAllowed = BLL.ADMIN.Utility.GetRoasterAllowedStatusByEmployeeCode(model.UserName);
                            GlobalVariables.GV_SiteTitle = BLL.ADMIN.Utility.GetEmployeeSiteTitleByEmployeeCode(model.UserName);

                            //check for HR
                            leaves_counter = BLL.ADMIN.Utility.GetPendingLeavesApprovalForHR(emp_id);
                            if (leaves_counter > 0)
                            {
                                GlobalVariables.GV_PendingLeavesCountHR = leaves_counter.ToString();
                            }
                            else
                            {
                                GlobalVariables.GV_PendingLeavesCountHR = "0";
                            }

                            //check for LM
                            if (GlobalVariables.GV_EmployeeAccessGroupCode == BLL.TimeTuneRoles.ROLE_LM)
                            {
                                leaves_counter_lm = BLL.ADMIN.Utility.GetPendingLeavesApprovalForLM(emp_id);
                                if (leaves_counter_lm > 0)
                                {
                                    GlobalVariables.GV_PendingLeavesCountLM = leaves_counter_lm.ToString();
                                }
                                else
                                {
                                    GlobalVariables.GV_PendingLeavesCountLM = "0";
                                }
                            }

                            //employees missing info - count
                            if (GlobalVariables.GV_EmployeeAccessGroupCode == BLL.TimeTuneRoles.ROLE_HR)
                            {
                                missing_info_counter_hr = BLL.ADMIN.Utility.GetEmployeesMissingInfoCountForHR();
                                if (missing_info_counter_hr > 0)
                                {
                                    GlobalVariables.GV_EmployeesMissingInfoCountHR = missing_info_counter_hr.ToString();
                                }
                                else
                                {
                                    GlobalVariables.GV_EmployeesMissingInfoCountHR = "0";
                                }
                                //sum for HR
                                GlobalVariables.GV_BadgeSumForHR = (int.Parse(GlobalVariables.GV_PendingLeavesCountHR) + int.Parse(GlobalVariables.GV_EmployeesMissingInfoCountHR)).ToString();
                            }

                            //org-section-access-validation
                            if (ConfigurationManager.AppSettings["Organization_Access_Denied"] != null && ConfigurationManager.AppSettings["Organization_Access_Denied"].ToString() != "" && ConfigurationManager.AppSettings["Organization_Access_Denied"].ToString() == "111")
                                GlobalVariables.GV_AccessDeniedToOrganization = true;
                            else
                                GlobalVariables.GV_AccessDeniedToOrganization = false;
                        }

                        ManageSessionInfoForUser(model.UserName);

                        FormsAuthentication.RedirectFromLoginPage(model.UserName, model.RememberMe);
                    }
                    #endregion

                    //hide license key form as existing license key is ok/valid
                    ViewBag.LoginFormDisplay = "block";
                    ViewBag.LicenseFormDisplay = "none";

                    ModelState.AddModelError("", "Incorrect Username or Password entered");
                }
                else
                {

                    //hide license key form as existing license key is ok/valid
                    ViewBag.LoginFormDisplay = "none";
                    ViewBag.LicenseFormDisplay = "block";

                    ModelState.AddModelError("", "Invalid License");
                }
            }

            return View(model);
        }

        public void ManageSessionInfoForUser(string strEmpCode)
        {
            #region Leaves-Variables-Badge-Count-Info
            /*
            int emp_id = 0; int leaves_counter = 0, leaves_counter_lm = 0, missing_info_counter_hr = 0;

            emp_id = BLL.ADMIN.Utility.GetEmployeeIdByEmployeeCode(strEmpCode);
            if (emp_id > 0)
            {
                GlobalVariables.GV_EmployeeCode = strEmpCode;
                GlobalVariables.GV_EmployeeId = emp_id.ToString();
                GlobalVariables.GV_EmployeeName = BLL.ADMIN.Utility.GetEmployeeNameByEmployeeCode(strEmpCode);
                GlobalVariables.GV_EmployeeAccessGroupID = BLL.ADMIN.Utility.GetEmployeeAccessGroupIDByEmployeeCode(strEmpCode).ToString();

                ////////////////////////////////////check for HR
                leaves_counter = BLL.ADMIN.Utility.GetPendingLeavesApprovalForHR(emp_id);
                if (leaves_counter > 0)
                {
                    GlobalVariables.GV_PendingLeavesCountHR = leaves_counter.ToString();
                }
                else
                {
                    GlobalVariables.GV_PendingLeavesCountHR = "0";
                }

                ////////////////////////////////check for LM
                if (GlobalVariables.GV_EmployeeAccessGroupID == "2")
                {
                    leaves_counter_lm = BLL.ADMIN.Utility.GetPendingLeavesApprovalForLM(emp_id);
                    if (leaves_counter_lm > 0)
                    {
                        GlobalVariables.GV_PendingLeavesCountLM = leaves_counter_lm.ToString();
                    }
                    else
                    {
                        GlobalVariables.GV_PendingLeavesCountLM = "0";
                    }
                }

                ////////////////////////////////check for SLM
                //if (GlobalVariables.GV_EmployeeAccessGroupID == "4")
                //{
                //    leaves_counter_lm = BLL.ADMIN.Utility.GetPendingLeavesApprovalForSLM(emp_id);
                //    if (leaves_counter_lm > 0)
                //    {
                //        GlobalVariables.GV_PendingLeavesCountSLM = leaves_counter_lm.ToString();
                //    }
                //    else
                //    {
                //        GlobalVariables.GV_PendingLeavesCountSLM = "0";
                //    }
                //}

                ///////////////////////////////////employees missing info - count
                if (GlobalVariables.GV_EmployeeAccessGroupID == "1")
                {
                    missing_info_counter_hr = BLL.ADMIN.Utility.GetEmployeesMissingInfoCountForHR();
                    if (missing_info_counter_hr > 0)
                    {
                        GlobalVariables.GV_EmployeesMissingInfoCountHR = missing_info_counter_hr.ToString();
                    }
                    else
                    {
                        GlobalVariables.GV_EmployeesMissingInfoCountHR = "0";
                    }

                    //sum for HR
                    GlobalVariables.GV_BadgeSumForHR = (int.Parse(GlobalVariables.GV_PendingLeavesCountHR) + int.Parse(GlobalVariables.GV_EmployeesMissingInfoCountHR)).ToString();
                }

            }
            */
            #endregion

            ///////////////////////////////////////start
            if (ConfigurationManager.AppSettings["SessionStartDay"] != null && ConfigurationManager.AppSettings["SessionStartDay"].ToString() != "")
            {
                GlobalVariables.GV_SessionStartDay = ConfigurationManager.AppSettings["SessionStartDay"].ToString();
                Session["GV_SessionStartDay"] = ConfigurationManager.AppSettings["SessionStartDay"].ToString();
            }
            else
            {
                GlobalVariables.GV_SessionStartDay = "01";
                Session["GV_SessionStartDay"] = "01";
            }
            if (ConfigurationManager.AppSettings["SessionStartMonth"] != null && ConfigurationManager.AppSettings["SessionStartMonth"].ToString() != "")
            {
                GlobalVariables.GV_SessionStartMonth = ConfigurationManager.AppSettings["SessionStartMonth"].ToString();
                Session["GV_SessionStartMonth"] = ConfigurationManager.AppSettings["SessionStartMonth"].ToString();
            }
            else
            {
                GlobalVariables.GV_SessionStartMonth = "01";
                Session["GV_SessionStartMonth"] = "01";
            }

            //////////////////////////////////end
            if (ConfigurationManager.AppSettings["SessionEndDay"] != null && ConfigurationManager.AppSettings["SessionEndDay"].ToString() != "")
            {
                GlobalVariables.GV_SessionEndDay = ConfigurationManager.AppSettings["SessionEndDay"].ToString();
                Session["GV_SessionEndDay"] = ConfigurationManager.AppSettings["SessionEndDay"].ToString();
            }
            else
            {
                GlobalVariables.GV_SessionEndDay = "31";
                Session["GV_SessionEndDay"] = "31";
            }
            if (ConfigurationManager.AppSettings["SessionEndMonth"] != null && ConfigurationManager.AppSettings["SessionEndMonth"].ToString() != "")
            {
                GlobalVariables.GV_SessionEndMonth = ConfigurationManager.AppSettings["SessionEndMonth"].ToString();
                Session["GV_SessionEndMonth"] = ConfigurationManager.AppSettings["SessionEndMonth"].ToString();
            }
            else
            {
                GlobalVariables.GV_SessionEndMonth = "12";
                Session["GV_SessionEndMonth"] = "12";
            }

            //////////////////////////////sat & sun leaves to count or not
            if (ConfigurationManager.AppSettings["CountSaturdayLeave"] != null && ConfigurationManager.AppSettings["CountSaturdayLeave"].ToString() != "")
            {
                Session["GV_CountSaturdayLeave"] = ConfigurationManager.AppSettings["CountSaturdayLeave"].ToString();
            }
            else
            {
                Session["GV_CountSaturdayLeave"] = "1";
            }
            if (ConfigurationManager.AppSettings["CountSundayLeave"] != null && ConfigurationManager.AppSettings["CountSundayLeave"].ToString() != "")
            {
                Session["GV_CountSundayLeave"] = ConfigurationManager.AppSettings["CountSundayLeave"].ToString();
            }
            else
            {
                Session["GV_CountSundayLeave"] = "1";
            }

            /////////////////////////// max leaves limit
            //if (ConfigurationManager.AppSettings["MaxSickLeaves"] != null && ConfigurationManager.AppSettings["MaxSickLeaves"].ToString() != "")
            //{
            //    Session["GV_MaxSickLeaves"] = ConfigurationManager.AppSettings["MaxSickLeaves"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxSickLeaves"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxCasualLeaves"] != null && ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString() != "")
            //{
            //    Session["GV_MaxCasualLeaves"] = ConfigurationManager.AppSettings["MaxCasualLeaves"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxCasualLeaves"] = "15";
            //}

            //if (ConfigurationManager.AppSettings["MaxAnnualLeaves"] != null && ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString() != "")
            //{
            //    Session["GV_MaxAnnualLeaves"] = ConfigurationManager.AppSettings["MaxAnnualLeaves"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxAnnualLeaves"] = "20";
            //}

            //if (ConfigurationManager.AppSettings["MaxOtherLeaves"] != null && ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString() != "")
            //{
            //    Session["GV_MaxOtherLeaves"] = ConfigurationManager.AppSettings["MaxOtherLeaves"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxOtherLeaves"] = "99";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType01"] != null && ConfigurationManager.AppSettings["MaxLeaveType01"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType01"] = ConfigurationManager.AppSettings["MaxLeaveType01"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType01"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType02"] != null && ConfigurationManager.AppSettings["MaxLeaveType02"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType02"] = ConfigurationManager.AppSettings["MaxLeaveType02"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType02"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType03"] != null && ConfigurationManager.AppSettings["MaxLeaveType03"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType03"] = ConfigurationManager.AppSettings["MaxLeaveType03"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType03"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType04"] != null && ConfigurationManager.AppSettings["MaxLeaveType04"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType04"] = ConfigurationManager.AppSettings["MaxLeaveType04"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType04"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType05"] != null && ConfigurationManager.AppSettings["MaxLeaveType05"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType05"] = ConfigurationManager.AppSettings["MaxLeaveType05"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType05"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType06"] != null && ConfigurationManager.AppSettings["MaxLeaveType06"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType06"] = ConfigurationManager.AppSettings["MaxLeaveType06"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType06"] = "10";
            //}


            //if (ConfigurationManager.AppSettings["MaxLeaveType07"] != null && ConfigurationManager.AppSettings["MaxLeaveType07"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType07"] = ConfigurationManager.AppSettings["MaxLeaveType07"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType07"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType08"] != null && ConfigurationManager.AppSettings["MaxLeaveType08"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType08"] = ConfigurationManager.AppSettings["MaxLeaveType08"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType08"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType09"] != null && ConfigurationManager.AppSettings["MaxLeaveType09"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType09"] = ConfigurationManager.AppSettings["MaxLeaveType09"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType09"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType10"] != null && ConfigurationManager.AppSettings["MaxLeaveType10"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType10"] = ConfigurationManager.AppSettings["MaxLeaveType10"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType10"] = "10";
            //}

            //if (ConfigurationManager.AppSettings["MaxLeaveType11"] != null && ConfigurationManager.AppSettings["MaxLeaveType11"].ToString() != "")
            //{
            //    Session["GV_MaxLeaveType11"] = ConfigurationManager.AppSettings["MaxLeaveType11"].ToString();
            //}
            //else
            //{
            //    Session["GV_MaxLeaveType11"] = "10";
            //}

            //LeaveTypesCountStatus vm = new LeaveTypesCountStatus();
            int[] leaves = new int[45] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            leaves = TimeTune.LeaveSessionCrud.GetDefaultMaxLeavesCountStatus();
            Session["GV_DefaultSickLeaves"] = leaves[0];
            Session["GV_DefaultCasualLeaves"] = leaves[3];
            Session["GV_DefaultAnnualLeaves"] = leaves[6];
            Session["GV_DefaultOtherLeaves"] = leaves[9];
            Session["GV_DefaultLeaveType01"] = leaves[12];
            Session["GV_DefaultLeaveType02"] = leaves[15];
            Session["GV_DefaultLeaveType03"] = leaves[18];
            Session["GV_DefaultLeaveType04"] = leaves[21];
            Session["GV_DefaultLeaveType05"] = leaves[24];
            Session["GV_DefaultLeaveType06"] = leaves[27];
            Session["GV_DefaultLeaveType07"] = leaves[30];
            Session["GV_DefaultLeaveType08"] = leaves[33];
            Session["GV_DefaultLeaveType09"] = leaves[36];
            Session["GV_DefaultLeaveType10"] = leaves[39];
            Session["GV_DefaultLeaveType11"] = leaves[42];

            Session["GV_MaxSickLeaves"] = leaves[1];
            Session["GV_MaxCasualLeaves"] = leaves[4];
            Session["GV_MaxAnnualLeaves"] = leaves[7];
            Session["GV_MaxOtherLeaves"] = leaves[10];
            Session["GV_MaxLeaveType01"] = leaves[13];
            Session["GV_MaxLeaveType02"] = leaves[16];
            Session["GV_MaxLeaveType03"] = leaves[19];
            Session["GV_MaxLeaveType04"] = leaves[22];
            Session["GV_MaxLeaveType05"] = leaves[25];
            Session["GV_MaxLeaveType06"] = leaves[28];
            Session["GV_MaxLeaveType07"] = leaves[31];
            Session["GV_MaxLeaveType08"] = leaves[34];
            Session["GV_MaxLeaveType09"] = leaves[37];
            Session["GV_MaxLeaveType10"] = leaves[40];
            Session["GV_MaxLeaveType11"] = leaves[43];

            Session["GV_StatusSickLeaves"] = leaves[2];
            Session["GV_StatusCasualLeaves"] = leaves[5];
            Session["GV_StatusAnnualLeaves"] = leaves[8];
            Session["GV_StatusOtherLeaves"] = leaves[11];
            Session["GV_StatusLeaveType01"] = leaves[14];
            Session["GV_StatusLeaveType02"] = leaves[17];
            Session["GV_StatusLeaveType03"] = leaves[20];
            Session["GV_StatusLeaveType04"] = leaves[23];
            Session["GV_StatusLeaveType05"] = leaves[26];
            Session["GV_StatusLeaveType06"] = leaves[29];
            Session["GV_StatusLeaveType07"] = leaves[32];
            Session["GV_StatusLeaveType08"] = leaves[35];
            Session["GV_StatusLeaveType09"] = leaves[38];
            Session["GV_StatusLeaveType10"] = leaves[41];
            Session["GV_StatusLeaveType11"] = leaves[44];

            //////////////////////leaves counter - get all employee view models /////////////////////////////////////////
            int session_year = DateTime.Now.Year;
            var data = new List<ViewModels.LeavesCountReportLog>();
            bool isRpt01Perm = false, isRpt02Perm = false, isRpt03Perm = false, isRpt04Perm = false;

            if (ViewModel.GlobalVariables.GV_EmployeeId != null && ViewModel.GlobalVariables.GV_EmployeeId != "")
            {
                isRpt01Perm = TimeTune.ReportPermissionAccess.GetReportAccessByCode(ViewModel.GlobalVariables.GV_EmployeeCode, 1);
                ViewModel.GlobalVariables.GV_Rpt01Perm = isRpt01Perm;
                isRpt02Perm = TimeTune.ReportPermissionAccess.GetReportAccessByCode(ViewModel.GlobalVariables.GV_EmployeeCode, 2);
                ViewModel.GlobalVariables.GV_Rpt02Perm = isRpt02Perm;
                isRpt03Perm = TimeTune.ReportPermissionAccess.GetReportAccessByCode(ViewModel.GlobalVariables.GV_EmployeeCode, 3);
                ViewModel.GlobalVariables.GV_Rpt03Perm = isRpt03Perm;
                isRpt04Perm = TimeTune.ReportPermissionAccess.GetReportAccessByCode(ViewModel.GlobalVariables.GV_EmployeeCode, 4);
                ViewModel.GlobalVariables.GV_Rpt04Perm = isRpt04Perm;

                ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(int.Parse(ViewModel.GlobalVariables.GV_EmployeeId));
                session_year = int.Parse(yearList[0].ToString());
                TimeTune.Attendance.getLeavesCountReportByEmpCode(strEmpCode, session_year.ToString(), out data);

                if (data != null && data.Count > 0)
                {
                    foreach (ViewModels.LeavesCountReportLog item in data)
                    {
                        Session["GV_AllocatedSickLeaves"] = item.AllocatedSickLeaves;
                        Session["GV_AllocatedCasualLeaves"] = item.AllocatedCasualLeaves;
                        Session["GV_AllocatedAnnualLeaves"] = item.AllocatedAnnualLeaves;
                        Session["GV_AllocatedOtherLeaves"] = item.AllocatedOtherLeaves;
                        Session["GV_AllocatedLeaveType01"] = item.AllocatedLeaveType01;
                        Session["GV_AllocatedLeaveType02"] = item.AllocatedLeaveType02;
                        Session["GV_AllocatedLeaveType03"] = item.AllocatedLeaveType03;
                        Session["GV_AllocatedLeaveType04"] = item.AllocatedLeaveType04;
                        Session["GV_AllocatedLeaveType05"] = item.AllocatedLeaveType05;
                        Session["GV_AllocatedLeaveType06"] = item.AllocatedLeaveType06;
                        Session["GV_AllocatedLeaveType07"] = item.AllocatedLeaveType07;
                        Session["GV_AllocatedLeaveType08"] = item.AllocatedLeaveType08;
                        Session["GV_AllocatedLeaveType09"] = item.AllocatedLeaveType09;
                        Session["GV_AllocatedLeaveType10"] = item.AllocatedLeaveType10;
                        Session["GV_AllocatedLeaveType11"] = item.AllocatedLeaveType11;

                        Session["GV_AvailedSickLeaves"] = item.AvailedSickLeaves;
                        Session["GV_AvailedCasualLeaves"] = item.AvailedCasualLeaves;
                        Session["GV_AvailedAnnualLeaves"] = item.AvailedAnnualLeaves;
                        Session["GV_AvailedOtherLeaves"] = item.AvailedOtherLeaves;
                        Session["GV_AvailedLeaveType01"] = item.AvailedLeaveType01;
                        Session["GV_AvailedLeaveType02"] = item.AvailedLeaveType02;
                        Session["GV_AvailedLeaveType03"] = item.AvailedLeaveType03;
                        Session["GV_AvailedLeaveType04"] = item.AvailedLeaveType04;
                        Session["GV_AvailedLeaveType05"] = item.AvailedLeaveType05;
                        Session["GV_AvailedLeaveType06"] = item.AvailedLeaveType06;
                        Session["GV_AvailedLeaveType07"] = item.AvailedLeaveType07;
                        Session["GV_AvailedLeaveType08"] = item.AvailedLeaveType08;
                        Session["GV_AvailedLeaveType09"] = item.AvailedLeaveType09;
                        Session["GV_AvailedLeaveType10"] = item.AvailedLeaveType10;
                        Session["GV_AvailedLeaveType11"] = item.AvailedLeaveType11;
                    }
                }
                else
                {
                    Session["GV_AllocatedSickLeaves"] = 0;
                    Session["GV_AllocatedCasualLeaves"] = 0;
                    Session["GV_AllocatedAnnualLeaves"] = 0;
                    Session["GV_AllocatedOtherLeaves"] = 0;
                    Session["GV_AllocatedLeaveType01"] = 0;
                    Session["GV_AllocatedLeaveType02"] = 0;
                    Session["GV_AllocatedLeaveType03"] = 0;
                    Session["GV_AllocatedLeaveType04"] = 0;
                    Session["GV_AllocatedLeaveType05"] = 0;
                    Session["GV_AllocatedLeaveType06"] = 0;
                    Session["GV_AllocatedLeaveType07"] = 0;
                    Session["GV_AllocatedLeaveType08"] = 0;
                    Session["GV_AllocatedLeaveType09"] = 0;
                    Session["GV_AllocatedLeaveType10"] = 0;
                    Session["GV_AllocatedLeaveType11"] = 0;

                    Session["GV_AvailedSickLeaves"] = 0;
                    Session["GV_AvailedCasualLeaves"] = 0;
                    Session["GV_AvailedAnnualLeaves"] = 0;
                    Session["GV_AvailedOtherLeaves"] = 0;
                    Session["GV_AvailedLeaveType01"] = 0;
                    Session["GV_AvailedLeaveType02"] = 0;
                    Session["GV_AvailedLeaveType03"] = 0;
                    Session["GV_AvailedLeaveType04"] = 0;
                    Session["GV_AvailedLeaveType05"] = 0;
                    Session["GV_AvailedLeaveType06"] = 0;
                    Session["GV_AvailedLeaveType07"] = 0;
                    Session["GV_AvailedLeaveType08"] = 0;
                    Session["GV_AvailedLeaveType09"] = 0;
                    Session["GV_AvailedLeaveType10"] = 0;
                    Session["GV_AvailedLeaveType11"] = 0;
                }
            }
        }

        #endregion

        # region ForgotPassword

        [AllowAnonymous]
        [HandleAntiForgeryError]
        public ActionResult ForgotPassword()
        {
            bool isInternetOn = false; bool accessAllowed = false;

            isInternetOn = CheckForInternetConnection();
            if (!isInternetOn)
            {
                //due to already added condition to hide form, so using this message given below
                ViewBag.Message = "<span class='text-danger'>Internet connection is not available.<br>Please enable internet and refresh this page again.</span>";

                return View("ForgotPassword");
            }

            accessAllowed = BLL.ADMIN.ForgotPassword.verifyAccessAllowed();
            if (accessAllowed)
            {
                return View("ForgotPassword");
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HandleAntiForgeryError]
        public async Task<ActionResult> ForgotPassword(BLL.ViewModels.ForgotPassword forgotPassword)
        {
            string strMessage = string.Empty, strEmpEmail = string.Empty;
            bool empCode = false, empRequest = false;
            //bool emailSentStatus = false;

            //verify employee code
            if (forgotPassword.EmployeeCode != "")// && forgotPassword.EmployeeEmail != "")
            {
                strEmpEmail = BLL.ADMIN.ForgotPassword.verifyEmployeeCodeANDEmail(forgotPassword.EmployeeCode);//, forgotPassword.EmployeeEmail);

                if (strEmpEmail != "")
                {
                    //validate alreade generated password request
                    empRequest = BLL.ADMIN.ForgotPassword.validiateAlreadyExistingPasswordRequest(forgotPassword.EmployeeCode); //forgotPassword.EmployeeCode);
                    if (empRequest)
                    {
                        //already exists
                        ViewBag.Message = "<span class='text-danger'>A request is already generated a while back.<br>Please check email in your inbox OR retry later.</span>";
                    }
                    else
                    {
                        //add entry for Forgot Password table
                        string url_access = "", guid_code = "";

                        guid_code = BLL.ADMIN.ForgotPassword.GetGuidCode();
                        if (guid_code != "")
                        {
                            //send email with password reset link
                            //sample url http://localhost/Account/ForgotPassword

                            url_access = Request.Url.ToString().ToLower();
                            //url_access = "http://trms.duhs.edu.pk/Account/ForgotPasswordReset";

                            if (url_access != null && url_access.Contains("forgotpassword") && url_access.Split('/').Count() >= 3)
                            {
                                strMessage = await BLL.ADMIN.EmailHelper.SendEmailWithPasswordResetLink(url_access.Split('/')[2].ToString(), forgotPassword.EmployeeCode, strEmpEmail, guid_code);//forgotPassword.EmployeeEmail
                                if (strMessage == "success")
                                {
                                    BLL.ADMIN.ForgotPassword.AddNEWForgotPasswordRequest(forgotPassword.EmployeeCode, guid_code);

                                    ViewBag.Message = "<span title='" + strMessage + "' class='text-success'>Your password-reset request has been submitted. An email with instructions will be sent shortly.<br>Please contact the System Administrator, if you do not receive an email within the next 10-15 minutes.<br>Back to <a href='/Account/Login'>LOGIN</a> page.</span>";
                                }
                                else
                                {
                                    ViewBag.Message = "<span title='" + strMessage + "' class='text-danger'>An error occurred while sending the email. Please try again later.</span>";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.Message = "<span class='text-danger'>An error occurred. Please try again.</span>";
                        }
                    }
                }
                else
                {
                    //invalid code entered
                    ViewBag.Message = "<span class='text-danger'>No record found OR No email is assiciated with this Username.</span>";
                }
            }
            //verify employee email
            else
            {
                //empty field values
                ViewBag.Message = "<span class='text-danger'>Please enter a valid Employee Code.</span>";//Please enter both field values (Employee Code and Email) correctly.
            }

            return View();
        }

        ////////////////////////////////////////////// FORGOT PASSWORD - RESET ///////////////////////////////////////////////////////////

        [AllowAnonymous]
        [HandleAntiForgeryError]
        public ActionResult ForgotPasswordReset()
        {
            //get GUID Code from Query String
            string guid_code = string.Empty; string out_emp_code = string.Empty;
            int gResponse = 0;

            if (Request.QueryString["gcode"] != null && Request.QueryString["gcode"].ToString() != "")
            {
                guid_code = Request.QueryString["gcode"].ToString();
                ViewBag.GCode = guid_code;

                //validate alreade generated password request
                gResponse = BLL.ADMIN.ForgotPassword.validiateGuidCode(guid_code, out out_emp_code);

                ViewBag.EmployeeCode = out_emp_code;

                if (gResponse == 0)
                {
                    ViewBag.EmployeeCode = ""; //hide form

                    //no record
                    ViewBag.Message = "<span class='text-danger'>No such request received.</span>";
                }
                else if (gResponse == -1)
                {
                    ViewBag.EmployeeCode = ""; //hide form

                    //already exists
                    ViewBag.Message = "<span class='text-danger'>This link has been expired. Please perform a new request at <a href='/Account/ForgotPassword'>Forgot Password</a> page again.</span>";
                }
                else if (gResponse == -2)
                {
                    ViewBag.EmployeeCode = ""; //hide form

                    //already exists
                    ViewBag.Message = "<span class='text-danger'>This link has been utilized. Please try a new one at <a href='/Account/ForgotPassword'>Forgot Password</a> page.</span>";
                }
                else //if (gResponse == 1)
                {
                    //already exists
                    ViewBag.Message = "<span class='text-success'>.</span>";//Please proceed...
                }
            }
            else
            {
                ViewBag.EmployeeCode = ""; //hide form

                ViewBag.Message = "<span class='text-danger'>Invalid Request</span>";
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HandleAntiForgeryError]
        public ActionResult ForgotPasswordReset(BLL.ViewModels.ResetPassword resetPassword)
        {
            //get GUID Code from Query String
            string guid_code = string.Empty;
            int gResponse = 0; bool uResponse = false;

            if (resetPassword.NewPassword == resetPassword.NewPasswordConfirm)
            {
                guid_code = resetPassword.GuidCode;//Request.QueryString["gcode"].ToString();

                //validate alreade generated password request
                gResponse = BLL.ADMIN.ForgotPassword.validiateGuidCodeWithPasswordUpdateInDatabase(guid_code, resetPassword.NewPassword);
                if (gResponse == 0)
                {
                    //no record
                    ViewBag.Message = "<span class='text-danger'>No record found</span>";
                }
                else if (gResponse == -1)
                {
                    //already exists
                    ViewBag.Message = "<span class='text-danger'>This link has been expired</span>";
                }
                else if (gResponse == -2)
                {
                    //already exists
                    ViewBag.Message = "<span class='text-danger'>This link has been utilized</span>";
                }
                else if (gResponse == 1)
                {
                    //validate alreade generated password request
                    uResponse = BLL.ADMIN.ForgotPassword.UpdateDateByGuid(guid_code);
                    if (uResponse)
                    {
                        ViewBag.EmployeeCode = "";

                        //success
                        ViewBag.Message = "<span class='text-success'>The password has been reset successfully; use the new password to login to the system <a href='/Account/Login'>here</a>.</span>";
                    }
                    else
                    {
                        //failed
                        ViewBag.Message = "<span class='text-danger'>No record found</span>";
                    }
                }
            }
            else
            {
                ViewBag.Message = "<span class='text-danger'>Both passswords must be same.</span>";
            }

            return View();
        }

        public static bool CheckForInternetConnection()
        {
            bool response = false, iResponse = false;

            try
            {
                //if allowed in config to check internet then do : VALIDATE_INTERNET_CONNECTION
                iResponse = BLL.ADMIN.ForgotPassword.validateInternetConnetionChecking();
                if (!iResponse)
                {
                    return true;//no need to validate internet connection
                }

                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://www.google.com/"))       //http://clients3.google.com/generate_204
                    {
                        response = true;
                    }
                }
            }
            catch
            {
                response = false;
            }

            return response;
        }


        #endregion


        #region Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleAntiForgeryError]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
        #endregion


        #region ChangePassword
        // Change password pages
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";


            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.LoggedInAs = Roles.GetRolesForUser(User.Identity.Name)[0];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleAntiForgeryError]
        public ActionResult Manage(BLL.ViewModels.LocalPasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");

            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
            bool changePasswordSucceeded;
            try
            {
                changePasswordSucceeded = Membership.Provider.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            }
            catch (Exception)
            {
                changePasswordSucceeded = false;
            }

            if (changePasswordSucceeded)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            else
            {
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        # region LicenseKey

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HandleAntiForgeryError]
        public ActionResult LicenseKey(string LicenseKey)
        {
            int response = 0;

            if (ModelState.IsValid)
            {
                if (LicenseKey != "")
                {
                    if (LicenseKey.Contains("-"))
                    {
                        if (LicenseKey.Length == 35)
                        {
                            response = TimeTune.LeadsValidation.updateLicenseKey(LicenseKey);
                            if (response > 0)
                            {
                                //validate-license-key - exemted for SUPER_ADMIN, Sudo, RESCO_ADMIN 
                                string url = Request.Url.AbsoluteUri.ToString().ToLower();
                                int access = TimeTune.LeadsValidation.checkAccessValidation(url);
                                if (access > 0)
                                {
                                    if ((access % int.Parse(GlobalVariables.GV_AccessValidationCode)) == 0)
                                    {
                                        ModelState.AddModelError("", "Done! Refresh this Page");

                                        return RedirectPermanent("~/Account/Login");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid License Key");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "An error occurred");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "License key is wrong");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "License key is fake");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "License key is required");
                }
            }

            return View("Login");
        }

        #endregion


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

    }
}
