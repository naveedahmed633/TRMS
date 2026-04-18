using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class RosterManagementController : Controller
    {
        //
        // GET: /RosterManagement/

        public ActionResult ManageGroup(string error)
        {
            if(error != null && !error.Equals(""))
                ModelState.AddModelError("",error);

            return View();
        }

        [HttpPost]
        public ActionResult AddGroup(ViewModels.CreateGroup fromForm)
        {
            fromForm.group_name = "grp"; //Group Name was removed from the front-end
            string error = TimeTune.RosterManagementHelper.createGroup(fromForm);
            var json = JsonConvert.SerializeObject(fromForm);
            TimeTune.AuditTrail.insert(json, "Group", int.Parse(User.Identity.Name));
            return RedirectToAction("ManageGroup", new { error = error});
        }

        
        #region ChosenAjaxDatahandlers
        /* 
         * checkimg chosen ajaxification list.
         */

        
        [HttpPost]
        public JsonResult lineManagersDataHandler()
        {
            string q = Request.Form["data[q]"];
            
            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.RosterManagementHelper.getAllLineManagersMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for(int i=0;i<employees.Length;i++) {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].id.ToString();
                toSend[i].text = employees[i].employee_code;
            }

            var toReturn = Json(new { 
                q=q,
                results = toSend
            });

            return toReturn;
        }


        // get all the employees, that do not have a group.
        [HttpPost]
        public JsonResult employeesDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.RosterManagementHelper.getAllEmployeesWhereGroupIsNull(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].id + "";
                toSend[i].text = employees[i].employee_code;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // get all the shifts
        [HttpPost]
        public JsonResult shiftsDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the
            // pattern 'q'
            ViewModels.Shift[] shifts = TimeTune.RosterManagementHelper.getAllActiveShifts(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[shifts.Length];
            for (int i = 0; i < shifts.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = shifts[i].id + "";
                toSend[i].text = shifts[i].name+" "+shifts[i].start_time+"-"+shifts[i].shift_end;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        #endregion


        // Do not be confused by the name this is a data table handler
        // for displaying groups.
        [HttpPost]
        public JsonResult groupEditDeleteDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ViewModels.GroupsTableView>();
                // get all employee view models
                int count = TimeTune.RosterManagementHelper.getAllGroupsMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                
                DTResult<ViewModels.GroupsTableView> result = new DTResult<ViewModels.GroupsTableView>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }



        [HttpPost]
        public JsonResult RawAttendanceDataHandler(DTParameters param)
        {
            try
            {
                // ReportRawAttendanceLog <=>  ViewModels.GroupsTableView
                // TimeTune.Reports <=> TimeTune.GroupsTable
                // RawAttendanceResultSet <=> GroupsTableResultSet

                var dtsource = new List<ViewModels.GroupsTableView>();

//                dtsource = TimeTune

                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }
                /*List<ReportRawAttendanceLog> data = RawAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = RawAttendanceResultSet.Count(param.Search.Value, dtsource);*/

                /*
                DTResult<ReportRawAttendanceLog> result = new DTResult<ReportRawAttendanceLog>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);*/

                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpPost]
        public JsonResult groupDelete()
        {
            string id = Request.Form["id"];


            var group=TimeTune.RosterManagementHelper.deleteGroup(id);
            var json = JsonConvert.SerializeObject(group);
            TimeTune.AuditTrail.delete(json, "Group", int.Parse(User.Identity.Name));

            return Json(new { success="true"});
        }

        [HttpPost]
        public JsonResult getGroupData()
        {
            int groupID =0;
            
            if(!int.TryParse(Request.Form["id"],out groupID)) {
                return Json(new { success=false});
            }

            var obj = TimeTune.RosterManagementHelper.getGroup(groupID);


            return Json(obj);
        }

        [HttpPost]
        public JsonResult editGroup(ViewModels.CreateGroup fromForm)
        {
            var from = fromForm;

            dynamic jsonToSend = TimeTune.RosterManagementHelper.editGroup(fromForm);
            var json = JsonConvert.SerializeObject(fromForm);
            TimeTune.AuditTrail.update(json, "Group", int.Parse(User.Identity.Name));
            return Json(jsonToSend);
        }
        

        public ActionResult ManageRoster()
        {
            return View();
        }
        public ActionResult EmployeeWOShift()
        {
            return View();
        }
        public ActionResult ViewSelfRoster()
        {
            return View();
        }
    }
}
