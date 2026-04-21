using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class AccountController : Controller
    {
        #region Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToRoute(new
            {
                Area = "",
                Controller = "Home",
                Action = "Index"
            });
        }
        #endregion


        #region ChangePassword

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
        public ActionResult Manage(BLL.ViewModels.LocalPasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");

            bool changePasswordSucceeded;

            try
            {
                changePasswordSucceeded = Membership.Provider.ChangePassword(
                    User.Identity.Name,
                    model.OldPassword,
                    model.NewPassword
                );
            }
            catch
            {
                changePasswordSucceeded = false;
            }

            if (changePasswordSucceeded)
            {
                return RedirectToAction("Manage", new
                {
                    Message = ManageMessageId.ChangePasswordSuccess
                });
            }

            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            return View(model);
        }

        #endregion


        #region 🌍 Language Change (FIXED)

        [HttpPost]
        public ActionResult Languagechange(string ddl_language)
        {
            // ✅ Default fallback
            string lang = string.IsNullOrEmpty(ddl_language) ? "En" : ddl_language;

            // ✅ Save in DB (optional)
            TimeTune.AuditTrail.updatelanguage(User.Identity.Name, lang);

            // 🔥 MOST IMPORTANT (PDF + whole app ke liye)
            Session["lang"] = lang;

            // ⚠️ Optional (agar system already use kar raha hai)
            MvcApplication1.ViewModel.GlobalVariables.GV_Langauge = lang;

            return RedirectToAction("Dashboard", "HR");
        }

        #endregion


        #region Helpers

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        #endregion
    }
}