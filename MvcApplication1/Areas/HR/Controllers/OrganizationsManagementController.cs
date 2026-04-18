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
using ViewModels;
using System.IO;
using System.Globalization;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text;
using TimeTune;
using BLL.ViewModels;
using MvcApplication1.ViewModel;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class OrganizationsManagementController : Controller
    {

        #region Organization

        [HttpGet]
        public ActionResult ManageOrganization()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            OrganizationView vm = new OrganizationView();
            vm = OrganizationResultSet.GetOrganizationData();

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganization(OrganizationForm oform, HttpPostedFileBase logo_file_path)
        {
            string FileName = string.Empty, FileExtension = string.Empty;

            try
            {
                if (logo_file_path != null)
                {
                    FileName = Path.GetFileName(logo_file_path.FileName);
                    FileExtension = Path.GetExtension(logo_file_path.FileName).ToLower();

                    if (FileExtension == ".jpg" || FileExtension == ".png" || FileExtension == ".gif")
                    {
                        var guid = Guid.NewGuid().ToString();
                        string filename_guid = FileName; // guid + "_" + FileName;
                        var path = Path.Combine(Server.MapPath("~/Content/Logos"), filename_guid);
                        logo_file_path.SaveAs(path);

                        oform.logo_file_path = "/Content/Logos/" + filename_guid;
                    }
                    else
                    {
                        oform.logo_file_path = "/Content/Logos/logo-default.png";
                    }
                }
                else
                {
                    oform.logo_file_path = "/Content/Logos/logo-default.png";
                }

                OrganizationView orgView = new OrganizationView();

                orgView.Id = oform.id;
                orgView.OrganizationTitle = oform.organization_title;
                orgView.EstablishedDate = DateTime.ParseExact(oform.established_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                orgView.CampusLimit = oform.campus_limit;
                orgView.Logo = oform.logo_file_path;
                orgView.WebsiteURL = oform.website_url;
                orgView.Description = oform.description;
                //orgView.CreateDateOrg = DateTime.Now;

                int updated = OrganizationResultSet.UpdateOrganization(orgView);

                if (updated == 0)
                {
                    //success
                    ViewBag.Message = "The organization is updated successfully!";

                    var json = JsonConvert.SerializeObject(oform);
                    AuditTrail.update(json, "Organization", User.Identity.Name);
                }
                else if (updated > 0)
                {
                    //success
                    ViewBag.Message = "The organization is added successfully!";

                    var json = JsonConvert.SerializeObject(oform);
                    AuditTrail.insert(json, "Organization", User.Identity.Name);
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            OrganizationView vm = new OrganizationView();
            vm = OrganizationResultSet.GetOrganizationData();

            return View(vm);
        }


        #endregion


        #region Organization-Campuses

        [HttpGet]
        public ActionResult ManageOrganizationCampus()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            OrganizationCampusView vm = new OrganizationCampusView();

            vm.list_organization = OrganizationCampusResultSet.GetOrganizationsList();
            vm.list_campus_type = OrganizationCampusResultSet.GetOrganizationCampusTypesList();
            vm.list_location = OrganizationCampusResultSet.GetLocationsList();
            vm.list_organization_program_type = OrganizationCampusResultSet.GetProgramTypesList();
            vm.list_state_province = OrganizationCampusResultSet.GetStatesProvincesList();

            vm.str_campus_type = OrganizationCampusResultSet.GetOrganizationCampusTypeString();

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationCampus(OrganizationCampusForm cform)
        {
            int idCreated = 0;

            try
            {

                OrganizationCampusView cmpView = new OrganizationCampusView();

                //cmpView.Id = cform.id;
                cmpView.OrganizationId = cform.organization_id;
                cmpView.CampusTypeId = cform.campus_type_id;
                cmpView.CampusCode = cform.campus_code;
                cmpView.CampusTitle = cform.campus_title;
                cmpView.EmailAddress = cform.email_address;
                cmpView.Address = cform.address;
                cmpView.CityId = cform.city_id;
                cmpView.StateProvinceId = cform.state_province_id;
                cmpView.ZipCode = cform.zip_code;
                cmpView.Phone01 = cform.phone_01;
                cmpView.Phone02 = cform.phone_02;
                cmpView.FaxNumber = cform.fax_number;
                cmpView.CreateDateCmp = DateTime.Now;

                idCreated = OrganizationCampusResultSet.CreateOrganizationCampus(cmpView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The campus is added successfully!";

                    var json = JsonConvert.SerializeObject(cform);
                    AuditTrail.insert(json, "OrganizationCampus", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A campus with same code already exists";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            OrganizationCampusView vm = new OrganizationCampusView();

            vm.list_organization = OrganizationCampusResultSet.GetOrganizationsList();
            vm.list_campus_type = OrganizationCampusResultSet.GetOrganizationCampusTypesList();
            vm.list_location = OrganizationCampusResultSet.GetLocationsList();
            vm.list_organization_program_type = OrganizationCampusResultSet.GetProgramTypesList();
            vm.list_state_province = OrganizationCampusResultSet.GetStatesProvincesList();

            vm.str_campus_type = OrganizationCampusResultSet.GetOrganizationCampusTypeString();

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationCampusDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationCampusView>();
                dtSource = OrganizationCampusResultSet.getOrganizationCampusByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationCampusView> data = OrganizationCampusResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationCampusResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationCampusView> result = new DTResult<OrganizationCampusView>
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

            //return null;
        }


        [HttpPost]
        public ActionResult UpdateOrganizationCampus(ViewModels.OrganizationCampusView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationCampusResultSet.UpdateOrganizationCampus(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationCampus", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationCampus(ViewModels.OrganizationCampusView toRemove)
        {
            var entity = OrganizationCampusResultSet.RemoveOrganizationCampus(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationCampus", User.Identity.Name);
            return Json(new { status = "success" });
        }

        #endregion


        #region Organization-Buildings

        [HttpGet]
        public ActionResult ManageOrganizationBuilding()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCampusBuildingView vm = new OrganizationCampusBuildingView();
            vm.list_campus = OrganizationCampusBuildingResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);

            vm.str_campus = OrganizationCampusBuildingResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationBuilding(OrganizationCampusBuildingForm bform)
        {
            int idCreated = 0;
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            try
            {
                OrganizationCampusBuildingView bldView = new OrganizationCampusBuildingView();

                //bldView.Id = bform.id;
                bldView.CampusId = bform.campus_id;
                bldView.BuildingCode = bform.building_code;
                bldView.BuildingTitle = bform.building_title;
                bldView.CreateDateBld = DateTime.Now;

                idCreated = OrganizationCampusBuildingResultSet.CreateOrganizationBuilding(bldView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The building is added successfully!";

                    var json = JsonConvert.SerializeObject(bform);
                    AuditTrail.insert(json, "OrganizationCampusBuilding", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A building with same code (under provided campus) already exists";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            OrganizationCampusBuildingView vm = new OrganizationCampusBuildingView();
            vm.list_campus = OrganizationCampusBuildingResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);

            vm.str_campus = OrganizationCampusBuildingResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationBuildingDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationCampusBuildingView>();
                dtSource = OrganizationCampusBuildingResultSet.getOrganizationCampusBuildingsByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationCampusBuildingView> data = OrganizationCampusBuildingResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationCampusBuildingResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationCampusBuildingView> result = new DTResult<OrganizationCampusBuildingView>
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

            //return null;
        }


        [HttpPost]
        public ActionResult UpdateOrganizationBuilding(ViewModels.OrganizationCampusBuildingView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationCampusBuildingResultSet.UpdateOrganizationBuilding(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationCampusBuilding", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationBuilding(ViewModels.OrganizationCampusBuildingView toRemove)
        {
            var entity = OrganizationCampusBuildingResultSet.RemoveOrganizationBuilding(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationCampusBuilding", User.Identity.Name);
            return Json(new { status = "success" });
        }

        #endregion


        #region Organization-Buildings-Rooms

        [HttpGet]
        public ActionResult ManageOrganizationRoom()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCampusBuildingRoomView vm = new OrganizationCampusBuildingRoomView();
            vm.list_building = OrganizationCampusBuildingRoomResultSet.GetOrganizationBuildingList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_terminal = OrganizationCampusBuildingRoomResultSet.GetOrganizationTerminalList(bGVIsSuperHRRole, iGVCampusID);

            vm.str_building = OrganizationCampusBuildingRoomResultSet.GetOrganizationCampusBuildingString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_terminal = OrganizationCampusBuildingRoomResultSet.GetOrganizationTerminalsString(bGVIsSuperHRRole, iGVCampusID);

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationRoom(OrganizationCampusBuildingRoomForm rform)
        {
            int idCreated = 0;
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            try
            {
                OrganizationCampusBuildingRoomView rmView = new OrganizationCampusBuildingRoomView();

                //rmView.Id = rform.id;
                rmView.BuildingId = rform.building_id;
                rmView.RoomCode = rform.room_code;
                rmView.RoomTitle = rform.room_title;
                rmView.TerminalId = rform.terminal_id;
                rmView.FloorNumber = rform.floor_number;
                rmView.CreateDateRm = DateTime.Now;

                idCreated = OrganizationCampusBuildingRoomResultSet.CreateOrganizationRoom(rmView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The room is added successfully!";

                    var json = JsonConvert.SerializeObject(rform);
                    AuditTrail.insert(json, "OrganizationCampusBuildingRoom", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A room with same code already exists";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            OrganizationCampusBuildingRoomView vm = new OrganizationCampusBuildingRoomView();
            vm.list_building = OrganizationCampusBuildingRoomResultSet.GetOrganizationBuildingList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_terminal = OrganizationCampusBuildingRoomResultSet.GetOrganizationTerminalList(bGVIsSuperHRRole, iGVCampusID);

            vm.str_building = OrganizationCampusBuildingRoomResultSet.GetOrganizationCampusBuildingString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_terminal = OrganizationCampusBuildingRoomResultSet.GetOrganizationTerminalsString(bGVIsSuperHRRole, iGVCampusID);

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationRoomDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationCampusBuildingRoomView>();
                dtSource = OrganizationCampusBuildingRoomResultSet.getOrganizationCampusBuildingsRoomByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationCampusBuildingRoomView> data = OrganizationCampusBuildingRoomResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationCampusBuildingRoomResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationCampusBuildingRoomView> result = new DTResult<OrganizationCampusBuildingRoomView>
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

            //return null;
        }


        [HttpPost]
        public ActionResult UpdateOrganizationRoom(ViewModels.OrganizationCampusBuildingRoomView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationCampusBuildingRoomResultSet.UpdateOrganizationRoom(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationCampusBuildingRoom", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationRoom(ViewModels.OrganizationCampusBuildingRoomView toRemove)
        {
            var entity = OrganizationCampusBuildingRoomResultSet.RemoveOrganizationRoom(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationCampusBuildingRoom", User.Identity.Name);
            return Json(new { status = "success" });
        }

        #endregion


        #region Organization-Buildings-Rooms-Schedules

        [HttpGet]
        public ActionResult ManageOrganizationSchedule()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCampusRoomCourseScheduleView vm = new OrganizationCampusRoomCourseScheduleView();
            vm.list_campus = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationProgramList();
            vm.list_shift = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationShiftList();
            vm.list_room = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationRoomList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_course = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCourseList();
            vm.list_lecture_group = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationLectureGroupList();
            vm.list_teacher = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationEmployeeTeacherList(bGVIsSuperHRRole, iGVCampusID);

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_campus = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationProgramString();
            vm.str_shift = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationShiftString();
            vm.str_rooms = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationRoomString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_courses = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCourseString();
            vm.str_lecture_groups = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationLectureGroupString();
            vm.str_teachers = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationEmployeeTeacherString(bGVIsSuperHRRole, iGVCampusID);

            if (Request.QueryString["result"] != null && Request.QueryString["result"].ToString() != "")
                ViewBag.UpMessage = Request.QueryString["result"].ToString();
            else
                ViewBag.UpMessage = "";

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationSchedule(OrganizationCampusRoomCourseScheduleForm sform)
        {
            int idCreated = 0;
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            try
            {
                string strStartTime = sform.start_date.ToString() + " " + sform.start_hr.ToString() + ":" + sform.start_mn.ToString() + ":00 " + sform.start_ap.ToString();
                string strEndTime = sform.end_date.ToString() + " " + sform.end_hr.ToString() + ":" + sform.end_mn.ToString() + ":00 " + sform.end_ap.ToString();


                OrganizationCampusRoomCourseScheduleView shView = new OrganizationCampusRoomCourseScheduleView();

                //shView.Id = rform.id;
                shView.CampusId = sform.campus_id;
                shView.RoomId = sform.room_id;
                shView.ProgramId = sform.program_id;
                shView.ShiftId = sform.shift_id;
                shView.LectureGroupId = sform.lecture_group_id;
                shView.CourseId = sform.course_id;
                shView.StudyTitle = sform.study_title;
                shView.StartTime = DateTime.ParseExact(strStartTime, "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                shView.EndTime = DateTime.ParseExact(strEndTime, "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                shView.EmployeeTeacherId = sform.employee_teacher_id;
                shView.CreateDateSch = DateTime.Now;

                idCreated = OrganizationCampusRoomCourseScheduleResultSet.CreateOrganizationSchedule(shView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The schedule is added successfully!";

                    var json = JsonConvert.SerializeObject(sform);
                    AuditTrail.insert(json, "OrganizationCampusRoomCourseSchedule", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A schedule with same title (under provided room) already exists";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            OrganizationCampusRoomCourseScheduleView vm = new OrganizationCampusRoomCourseScheduleView();
            vm.list_campus = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationProgramList();
            vm.list_shift = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationShiftList();
            vm.list_room = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationRoomList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_course = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCourseList();
            vm.list_lecture_group = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationLectureGroupList();
            vm.list_teacher = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationEmployeeTeacherList(bGVIsSuperHRRole, iGVCampusID);

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_campus = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationProgramString();
            vm.str_shift = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationShiftString();
            vm.str_rooms = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationRoomString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_courses = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCourseString();
            vm.str_lecture_groups = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationLectureGroupString();
            vm.str_teachers = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationEmployeeTeacherString(bGVIsSuperHRRole, iGVCampusID);

            if (Request.QueryString["result"] != null && Request.QueryString["result"].ToString() != "")
                ViewBag.UpMessage = Request.QueryString["result"].ToString();
            else
                ViewBag.UpMessage = "";

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationScheduleDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationCampusRoomCourseScheduleView>();
                dtSource = OrganizationCampusRoomCourseScheduleResultSet.getOrganizationCampusRoomCourseScheduleByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationCampusRoomCourseScheduleView> data = OrganizationCampusRoomCourseScheduleResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationCampusRoomCourseScheduleResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationCampusRoomCourseScheduleView> result = new DTResult<OrganizationCampusRoomCourseScheduleView>
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

            //return null;
        }

        [HttpPost]
        public ActionResult UpdateOrganizationSchedule(ViewModels.OrganizationCampusRoomCourseScheduleView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationCampusRoomCourseScheduleResultSet.UpdateOrganizationSchedule(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationCampusRoomCourseSchedule", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationSchedule(ViewModels.OrganizationCampusRoomCourseScheduleView toRemove)
        {
            var entity = OrganizationCampusRoomCourseScheduleResultSet.RemoveOrganizationSchedule(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationCampusRoomCourseSchedule", User.Identity.Name);
            return Json(new { status = "success" });
        }


        #region Schedule-PDF-Report

        [HttpPost]
        public JsonResult ScheduleReportDataHandler(OrganizationCampusRoomCourseScheduleForm param)
        {
            try
            {
                //int employeeID;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("MonthlyTimeSheet");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("MonthlyTimeSheet");


                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};

                return null; // Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("GenerateScheduleReport")]
        public ActionResult GenerateScheduleReport_Get()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCampusRoomCourseScheduleView vm = new OrganizationCampusRoomCourseScheduleView();
            vm.list_campus = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationProgramList();
            vm.list_room = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationRoomList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_shift = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationShiftList();

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateScheduleReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateScheduleReport_Post()
        {
            int shiftID = 0, roomID = 0, progID = 0, campID = 0, found = 0;
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            ScheduleReport reportMaker = new ScheduleReport();

            if (!int.TryParse(Request.Form["room_id"], out roomID))
                return RedirectToAction("GenerateScheduleReport");

            if (!int.TryParse(Request.Form["shift_id"], out shiftID))
                return RedirectToAction("GenerateScheduleReport");

            if (!int.TryParse(Request.Form["program_id"], out progID))
                return RedirectToAction("GenerateScheduleReport");

            if (!int.TryParse(Request.Form["campus_id"], out campID))
                return RedirectToAction("GenerateScheduleReport");

            dtStart = DateTime.ParseExact(Request.Form["from_date"] + " 00:00:00", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture); //DateTime.ParseExact(Request.Form["from_date"], "dd-MM-yyyy",);
            dtEnd = DateTime.ParseExact(Request.Form["to_date"] + " 23:59:59", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture); //DateTime.ParseExact(Request.Form["to_date"], "dd-MM-yyyy", "");

            ScheduleReportData toRender = reportMaker.getScheduleData(campID, progID, shiftID, roomID, dtStart, dtEnd);
            if (toRender == null)
                return RedirectToAction("GenerateScheduleReport");

            found = DownloadScheduleReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationCampusRoomCourseScheduleView vm = new OrganizationCampusRoomCourseScheduleView();
            vm.list_campus = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationProgramList();
            vm.list_room = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationRoomList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_shift = OrganizationCampusRoomCourseScheduleResultSet.GetOrganizationShiftList();

            return View(vm);
        }


        private int DownloadScheduleReportPDF(ScheduleReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to download PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBoldUnderline16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 5f, 5f);

                    //// To download PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.org_logo_path); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 100.0f;
                    //logo.Height = 80.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 880.0f, 120.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.org_title + "\n" + sdata.campus_code + "\n" + sdata.program_code + " (" + sdata.program_shift + ")", fBold16));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 4.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellEName = new PdfPCell(new Phrase("Effective FROM " + sdata.date_range, fBold12));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase(sdata.room_code, fBold12));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //Paragraph p1 = new Paragraph("Name: " + sdata.employeeName, fBold9);
                    ////p1.SpacingBefore = 10;
                    //document.Add(p1);

                    //Paragraph p2 = new Paragraph("Employee Number: " + sdata.employeeCode, fBold9);
                    //document.Add(p2);

                    //Paragraph p3 = new Paragraph("Month: " + sdata.month, fBold9);
                    //document.Add(p3);

                    //Paragraph p4 = new Paragraph("Year: " + sdata.year, fBold9);
                    //document.Add(p4);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    //PdfPTable tableMid = new PdfPTable(new[] { 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f, 15.0f });
                    PdfPTable tableSchedule = new PdfPTable(sdata.cols_count);

                    tableSchedule.WidthPercentage = 100;
                    tableSchedule.HeaderRows = 1;
                    tableSchedule.SpacingBefore = 3;
                    tableSchedule.SpacingAfter = 1;

                    //PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold8));
                    //cell1.BackgroundColor = Color.LIGHT_GRAY;
                    //cell1.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell1);

                    //PdfPCell cell2 = new PdfPCell(new Phrase("Time In", fBold8));
                    //cell2.BackgroundColor = Color.LIGHT_GRAY;
                    //cell2.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell2);

                    //PdfPCell cell3 = new PdfPCell(new Phrase("Remarks In", fBold8));
                    //cell3.BackgroundColor = Color.LIGHT_GRAY;
                    //cell3.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell3);

                    //PdfPCell cell4 = new PdfPCell(new Phrase("Time Out", fBold8));
                    //cell4.BackgroundColor = Color.LIGHT_GRAY;
                    //cell4.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell4);

                    //PdfPCell cell5 = new PdfPCell(new Phrase("Remarks Out", fBold8));
                    //cell5.BackgroundColor = Color.LIGHT_GRAY;
                    //cell5.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell5);

                    //PdfPCell cell6 = new PdfPCell(new Phrase("Final Remarks", fBold8));
                    //cell6.BackgroundColor = Color.LIGHT_GRAY;
                    //cell6.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell6);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("Device In", fBold8));
                    //cell7.BackgroundColor = Color.LIGHT_GRAY;
                    //cell7.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell7);

                    //PdfPCell cell8 = new PdfPCell(new Phrase("Device Out", fBold8));
                    //cell8.BackgroundColor = Color.LIGHT_GRAY;
                    //cell8.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell8);

                    for (int i = 0; i < sdata.rows_count; i++)
                    {
                        for (int j = 0; j < sdata.cols_count; j++)
                        {
                            string strData = sdata.logs.Find(l => l.id_row == i && l.id_col == j).str_data;

                            PdfPCell cellData1 = new PdfPCell(new Phrase(strData, fNormal8));
                            cellData1.HorizontalAlignment = 1;
                            tableSchedule.AddCell(cellData1);
                        }
                    }


                    //foreach (ScheduleReportLog log in sdata.logs)
                    //{
                    //    //{
                    //    //    log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                    //    //}

                    //    //PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, FontFactory.GetFont("Arial", 11, Font.NORMAL, Color.BLACK)));
                    //    //cellData1.HorizontalAlignment = 0; // 0 for left, 1 for Center - 2 for Right
                    //    //tableMid.AddCell(log.date);

                    //    //tableMid.AddCell(log.date);
                    //    //tableMid.AddCell(log.timeIn);
                    //    //tableMid.AddCell(log.remarksIn);
                    //    //tableMid.AddCell(log.timeOut);
                    //    //tableMid.AddCell(log.remarksOut);
                    //    //tableMid.AddCell(log.finalRemarks);
                    //    //tableMid.AddCell(log.terminalIn);
                    //    //tableMid.AddCell(log.terminalOut);

                    //    PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, fNormal8));
                    //    //cellData1.HorizontalAlignment = 0;
                    //    tableMid.AddCell(cellData1);

                    //    PdfPCell cellData2 = new PdfPCell(new Phrase(log.timeIn, fNormal8));
                    //    cellData2.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData2);

                    //    PdfPCell cellData3 = new PdfPCell(new Phrase(log.remarksIn, fNormal8));
                    //    //cellData3.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData3);

                    //    PdfPCell cellData4 = new PdfPCell(new Phrase(log.timeOut, fNormal8));
                    //    cellData4.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData4);

                    //    PdfPCell cellData5 = new PdfPCell(new Phrase(log.remarksOut, fNormal8));
                    //    //cellData5.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData5);

                    //    PdfPCell cellData6 = new PdfPCell(new Phrase(log.finalRemarks, fNormal8));
                    //    //cellData6.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData6);

                    //    PdfPCell cellData7 = new PdfPCell(new Phrase(log.terminalIn, fNormal7));
                    //    //cellData7.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData7);

                    //    PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminalOut, fNormal7));
                    //    //PdfPCell cellData8 = new PdfPCell(new Phrase("Second Floor Terminal 1234 678976 6543", fNormal7));
                    //    //cellData8.HorizontalAlignment = 1;
                    //    tableMid.AddCell(cellData8);
                    //}

                    if (sdata.logs != null && sdata.logs.Count > 0)
                    {
                        document.Add(tableSchedule);


                        // Summary heading
                        Paragraph p_summary = new Paragraph("Summary", fBold10);
                        ////document.Add(p_summary);

                        // ---------- Last Table ---------------------
                        PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        tableEnd.WidthPercentage = 100;
                        tableEnd.HeaderRows = 0;
                        tableEnd.SpacingBefore = 3;
                        tableEnd.SpacingAfter = 3;

                        PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        tableEnd.AddCell(lt_cell_11);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Late:", fBold9));
                        tableEnd.AddCell(lt_cell_21);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Absent:", fBold9));
                        tableEnd.AddCell(lt_cell_31);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        tableEnd.AddCell(lt_cell_41);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        tableEnd.AddCell(lt_cell_51);
                        tableEnd.AddCell(new Phrase(" " + sdata.rows_count, fNormal8));

                        ////document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, LV-Leave, PLO-Present Late & left On Time, PLE-Present Late & Early Out, POE-Present On Time & Early Out, PLM-Present Late & Miss Punch, PME-Present Miss Punch & Early Out, POM-Present On Time & Miss Punch, OV-Official Visit, OT-Official Travel, OM-Official Meeting, TR-Traning, *-Manually Updated", fNormal7);
                        p_abrv.SpacingBefore = 1;
                        //p_nsig.Alignment = 2;
                        ////document.Add(p_abrv);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Schedule-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion


        #region Schedule-Upload-Download-Sheets

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadScheduleCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<ManageStudentsScheduleImportExport.ManageEmployeeCSV>
                    (
                        ManageStudentsScheduleImportExport.getManageStudentsScheduleCSVDownload(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-Schedule.csv"
                    );


            return toReturn;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadScheduleCSVFile(HttpPostedFileBase file)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = Path.GetFileName(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName).ToLower();

                if (FileExtension != ".csv")
                {
                    return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_schedule.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidDate = false, invalidTime = false, invalidCampusCode = false, invalidCampusCodeAllowed = false, invalidRoomCode = false, invalidProgramCode = false, invalidCourseCode = false, invalidLecturerCode = false, invalidCols = false;

                    //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                    bool isDataRowFound = false; int a = 0;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            a++;

                            if (a == 2)
                            {
                                isDataRowFound = true;
                                break;
                            }
                        }
                    }

                    if (!isDataRowFound)
                    {
                        return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////

                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            strReadLine = strReadLine.TrimEnd(',');
                            strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                            strReadLine = strReadLine.Replace("\"", "");
                            strReadLine = strReadLine.TrimEnd(',');

                            string new_code = "";
                            string[] ecode_dt = strReadLine.Split(',');
                            if (ecode_dt.Length == 11)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("date") || ecode_dt[1].ToLower().Contains("st_time"))
                                {
                                    continue;
                                }

                                //if (ecode_dt[0].Length == 6)
                                //{
                                //    new_code = ecode_dt[0];
                                //}
                                //else
                                //{
                                //    if (ecode_dt[0].Length == 1)
                                //        new_code = "00000" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 2)
                                //        new_code = "0000" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 3)
                                //        new_code = "000" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 4)
                                //        new_code = "00" + ecode_dt[0];
                                //    else if (ecode_dt[0].Length == 5)
                                //        new_code = "0" + ecode_dt[0];
                                //    else
                                //        new_code = "";
                                //}

                                ////validate employee code
                                //if (!ValidateEmployeeCode(new_code))
                                //{
                                //    invalidEcode = true;
                                //    result = "Invalid User Code Found at Row-" + counter;
                                //    break;
                                //}

                                ////validate First Name
                                //if (!ValidateName(ecode_dt[1]))
                                //{
                                //    invalidFName = true;
                                //    result = "Invalid First Name Found at Row-" + counter;
                                //    break;
                                //}

                                ////validate Last Name
                                //if (!ValidateName(ecode_dt[2]))
                                //{
                                //    invalidLName = true;
                                //    result = "Invalid Last Name Found at Row-" + counter;
                                //    break;
                                //}

                                //validate Date
                                if (!ValidateDate(ecode_dt[0]))
                                {
                                    invalidDate = true;
                                    result = "Invalid Date Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateNOTPastDate(ecode_dt[0]))
                                {
                                    invalidDate = true;
                                    result = "Past Date NOT Allowed - Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateTime(ecode_dt[0], ecode_dt[1]))
                                {
                                    invalidTime = true;
                                    result = "Invalid Time Start Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateTime(ecode_dt[0], ecode_dt[2]))
                                {
                                    invalidTime = true;
                                    result = "Invalid Time End Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateCampusCode(ecode_dt[3]))
                                {
                                    invalidCampusCode = true;
                                    result = "Invalid Campus-Code Found at Row-" + counter;
                                    break;
                                }

                                bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
                                int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);
                                if (!ValidateCampusCodeAllowed(bGVIsSuperHRRole, iGVCampusID, ecode_dt[3]))
                                {
                                    invalidCampusCodeAllowed = true;
                                    result = "NOT Allowed Campus-Code Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateRoomCode(ecode_dt[3], ecode_dt[4]))
                                {
                                    invalidRoomCode = true;
                                    result = "Invalid Room-Code Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateProgramCode(ecode_dt[5]))
                                {
                                    invalidProgramCode = true;
                                    result = "Invalid Program-Code Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateCourseCode(ecode_dt[8]))
                                {
                                    invalidCourseCode = true;
                                    result = "Invalid Course-Code Found at Row-" + counter;
                                    break;
                                }

                                //if (!ValidateLecturerCode(ecode_dt[3], ecode_dt[10]))
                                //{
                                //    invalidLecturerCode = true;
                                //    result = "Invalid Lecturer-Code Found at Row-" + counter;
                                //    break;
                                //}
                            }
                            else
                            {
                                invalidCols = true;
                                result = "Invalid Col(s) Found";
                                break;
                            }

                            //iterate to replace EmployeeCode only having 0 as prefix
                            //for (int i = 0; i < ecode_dt.Length; i++)
                            //{
                            //    if (i == 0)
                            //    {
                            //        strReadLine = new_code + ",";
                            //    }
                            //    else
                            //    {
                            //        strReadLine += ecode_dt[i] + ",";
                            //    }
                            //}

                            //strReadLine = strReadLine.TrimEnd(',');
                            content.Add(strReadLine);

                            //restrict to upload if 1000+ rows are found
                            /*if (counter > 1000)
                            {
                                invalidRowsCount = true;
                                result = "Max 1000 records are allowed be uploaded";
                                break;
                            }*/
                        }
                    }

                    if (invalidDate || invalidTime || invalidCampusCode || invalidCampusCodeAllowed || invalidRoomCode || invalidProgramCode || invalidCourseCode || invalidLecturerCode || invalidCols)
                    {
                        return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = result });
                    }
                    else
                    {
                        result = ManageStudentsScheduleImportExport.setSchedue(content, User.Identity.Name);
                    }

                }

                if (result == "failed")
                {
                    return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "Successful" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "Select File first" });
        }

        private bool ValidateEmployeeCode(string strEmployeeCode)
        {
            bool isValid = true;

            Regex smallAlphaPattern = new Regex("[a-z]");
            Regex capsAlphaPattern = new Regex("[A-Z]");
            //Regex numeralsPattern = new Regex("[0-9]");
            Regex specialPattern = new Regex("[!@#$%^&*()_,.<>?;':-]");

            if (strEmployeeCode.Length < 6)
            {
                isValid = false;
            }
            else if (smallAlphaPattern.IsMatch(strEmployeeCode))
            {
                isValid = false;
            }
            else if (capsAlphaPattern.IsMatch(strEmployeeCode))
            {
                isValid = false;
            }
            else if (specialPattern.IsMatch(strEmployeeCode))
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateName(string strName)
        {
            bool isValid = true;

            //Regex smallAlphaPattern = new Regex("[a-z]");
            //Regex capsAlphaPattern = new Regex("[A-Z]");
            Regex numeralsPattern = new Regex("[0-9]");
            Regex specialPattern = new Regex("[!@#$%^&*()_,<>?;':<>]");

            if (strName.Length == 0)
            {
                isValid = false;
            }
            else if (numeralsPattern.IsMatch(strName))
            {
                isValid = false;
            }
            else if (specialPattern.IsMatch(strName))
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateDate(string strDate)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (!DateTime.TryParse(strProperDate, out dtTest))
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateNOTPastDate(string strDate)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (dtTest.Date < DateTime.Now.Date)
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateTime(string strDate, string strTime)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (!DateTime.TryParse(strProperDate + " " + strTime, out dtTest))
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateCampusCode(string strCampusCode)
        {
            bool isValid = false;

            isValid = ManageStudentsScheduleImportExport.validateCampusCode(strCampusCode);

            return isValid;
        }

        private bool ValidateCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            isValid = ManageStudentsScheduleImportExport.validateCampusCodeAllowed(bGVIsSuperHRRole, iCampusID, strCampusCode);

            return isValid;
        }

        private bool ValidateRoomCode(string strCampusCode, string strRoomCode)
        {
            bool isValid = false;

            isValid = ManageStudentsScheduleImportExport.validateRoomCode(strCampusCode, strRoomCode);

            return isValid;
        }

        private bool ValidateProgramCode(string strProgramCode)
        {
            bool isValid = false;

            isValid = ManageStudentsScheduleImportExport.validateProgramCode(strProgramCode);

            return isValid;
        }

        private bool ValidateCourseCode(string strCourseCode)
        {
            bool isValid = false;

            strCourseCode = strCourseCode.ToLower();

            if (strCourseCode == "self study")
            {
                isValid = true;
            }
            else if (strCourseCode == "seminar")
            {
                isValid = true;
            }
            else if (strCourseCode == "break")
            {
                isValid = true;
            }
            else if (strCourseCode == "off")
            {
                isValid = true;
            }
            else
            {
                isValid = ManageStudentsScheduleImportExport.validateCourseCode(strCourseCode);
            }

            return isValid;
        }

        private bool ValidateLecturerCode(string strCampusCode, string strLecturerCode)
        {
            bool isValid = false;

            if (strLecturerCode == "0")
            {
                isValid = true;
            }
            else
            {
                isValid = ManageStudentsScheduleImportExport.validateLecturerCode(strCampusCode, strLecturerCode);
            }

            return isValid;
        }



        #endregion


        #endregion


        #region Organization-Programs

        [HttpGet]
        public ActionResult ManageOrganizationProgram()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            OrganizationProgramView vm = new OrganizationProgramView();
            vm.list_category = OrganizationProgramResultSet.GetOrganizationProgramCategoryList();
            vm.list_whole_type = OrganizationProgramResultSet.GetOrganizationProgramTypeList();

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_category = OrganizationProgramResultSet.GetOrganizationProgramCategoryString();
            vm.str_whole_type = OrganizationProgramResultSet.GetOrganizationProgramTypeString();

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationProgram(OrganizationProgramForm pform)
        {
            int idCreated = 0;

            try
            {
                OrganizationProgramView pgView = new OrganizationProgramView();

                //pgView.Id = rform.id;
                pgView.CategoryId = pform.category_id;
                pgView.ProgramCode = pform.program_code;
                pgView.ProgramTitle = pform.program_title;
                pgView.DisciplineName = pform.discipline_name;
                pgView.CreditHours = pform.credit_hours;
                pgView.WholeProgramTypeId = pform.whole_program_type_id;
                pgView.WholeProgramTypeNumber = pform.whole_program_type_number;
                pgView.CreateDatePrg = DateTime.Now;

                idCreated = OrganizationProgramResultSet.CreateOrganizationProgram(pgView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The program is added successfully!";

                    var json = JsonConvert.SerializeObject(pform);
                    AuditTrail.insert(json, "OrganizationProgram", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A program with same code already exists";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            OrganizationProgramView vm = new OrganizationProgramView();
            vm.list_category = OrganizationProgramResultSet.GetOrganizationProgramCategoryList();
            vm.list_whole_type = OrganizationProgramResultSet.GetOrganizationProgramTypeList();

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_category = OrganizationProgramResultSet.GetOrganizationProgramCategoryString();
            vm.str_whole_type = OrganizationProgramResultSet.GetOrganizationProgramTypeString();

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationProgramDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationProgramView>();
                dtSource = OrganizationProgramResultSet.getOrganizationProgramByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationProgramView> data = OrganizationProgramResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationProgramResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationProgramView> result = new DTResult<OrganizationProgramView>
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

            //return null;
        }


        [HttpPost]
        public ActionResult UpdateOrganizationProgram(ViewModels.OrganizationProgramView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationProgramResultSet.UpdateOrganizationProgram(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationProgram", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationProgram(ViewModels.OrganizationProgramView toRemove)
        {
            var entity = OrganizationProgramResultSet.RemoveOrganizationProgram(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationProgram", User.Identity.Name);
            return Json(new { status = "success" });
        }

        #endregion





        #region Organization-Campus-Program

        [HttpGet]
        public ActionResult ManageOrganizationCampusProgram()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCampusProgramView vm = new OrganizationCampusProgramView();
            vm.list_campus = OrganizationCampusProgramResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationCampusProgramResultSet.GetOrganizationProgramList();

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_campus = OrganizationCampusProgramResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program = OrganizationCampusProgramResultSet.GetOrganizationProgramString();

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationCampusProgram(OrganizationCampusProgramForm cpform)
        {
            int idCreated = 0;

            try
            {
                OrganizationCampusProgramView eView = new OrganizationCampusProgramView();

                //eView.Id = rform.id;
                eView.CampusId = cpform.campus_id;
                eView.ProgramId = cpform.program_id;
                eView.IsActiveProgram = cpform.is_active_program;

                idCreated = OrganizationCampusProgramResultSet.CreateOrganizationCampusProgram(eView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The program is added to campus successfully!";

                    var json = JsonConvert.SerializeObject(cpform);
                    AuditTrail.insert(json, "OrganizationCampusProgram", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A program already added to this campus";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCampusProgramView vm = new OrganizationCampusProgramView();
            vm.list_campus = OrganizationCampusProgramResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationCampusProgramResultSet.GetOrganizationProgramList();

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_campus = OrganizationCampusProgramResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program = OrganizationCampusProgramResultSet.GetOrganizationProgramString();

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationCampusProgramDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationCampusProgramView>();
                dtSource = OrganizationCampusProgramResultSet.getOrganizationCampusProgramByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationCampusProgramView> data = OrganizationCampusProgramResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationCampusProgramResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationCampusProgramView> result = new DTResult<OrganizationCampusProgramView>
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

            //return null;
        }


        [HttpPost]
        public ActionResult UpdateOrganizationCampusProgram(ViewModels.OrganizationCampusProgramView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationCampusProgramResultSet.UpdateOrganizationCampusProgram(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationCampusProgram", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationCampusProgram(ViewModels.OrganizationCampusProgramView toRemove)
        {
            var entity = OrganizationCampusProgramResultSet.RemoveOrganizationCampusProgram(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationCampusProgram", User.Identity.Name);
            return Json(new { status = "success" });
        }

        #endregion





        #region Organization-Courses

        [HttpGet]
        public ActionResult ManageOrganizationCourse()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            OrganizationProgramCourseView vm = new OrganizationProgramCourseView();
            vm.list_program = OrganizationCourseResultSet.GetOrganizationProgramList();
            vm.list_default_type = OrganizationCourseResultSet.GetOrganizationProgramTypeList();

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_program = OrganizationCourseResultSet.GetOrganizationProgramString();
            vm.str_default_type = OrganizationCourseResultSet.GetOrganizationProgramTypeString();

            if (Request.QueryString["result"] != null && Request.QueryString["result"].ToString() != "")
                ViewBag.UpMessage = Request.QueryString["result"].ToString();
            else
                ViewBag.UpMessage = "";

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationCourse(OrganizationProgramCourseForm cform)
        {
            int idCreated = 0;

            try
            {
                OrganizationProgramCourseView pcView = new OrganizationProgramCourseView();

                //pcView.Id = rform.id;
                pcView.ProgramId = cform.program_id;
                pcView.CourseCode = cform.course_code;
                pcView.CourseTitle = cform.course_title;
                pcView.BookName = cform.book_name;
                pcView.BookAuthor = cform.book_author;
                pcView.DefaultProgramTypeId = cform.default_program_type_id;
                pcView.DefaultProgramTypeNumber = cform.default_program_type_number;
                pcView.CreditHours = cform.credit_hours;
                pcView.PassingMarks = cform.passing_marks;
                pcView.TotalMarks = cform.total_marks;
                pcView.IsActiveCourse = cform.is_active_course;
                pcView.CreateDateCrs = DateTime.Now;

                idCreated = OrganizationCourseResultSet.CreateOrganizationCourse(pcView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The course is added successfully!";

                    var json = JsonConvert.SerializeObject(cform);
                    AuditTrail.insert(json, "OrganizationProgramCourse", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A course with same code already exists";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            OrganizationProgramCourseView vm = new OrganizationProgramCourseView();
            vm.list_program = OrganizationCourseResultSet.GetOrganizationProgramList();
            vm.list_default_type = OrganizationCourseResultSet.GetOrganizationProgramTypeList();

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_program = OrganizationCourseResultSet.GetOrganizationProgramString();
            vm.str_default_type = OrganizationCourseResultSet.GetOrganizationProgramTypeString();

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationCourseDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationProgramCourseView>();
                dtSource = OrganizationCourseResultSet.getOrganizationCourseByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationProgramCourseView> data = OrganizationCourseResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationCourseResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationProgramCourseView> result = new DTResult<OrganizationProgramCourseView>
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

            //return null;
        }


        [HttpPost]
        public ActionResult UpdateOrganizationCourse(ViewModels.OrganizationProgramCourseView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationCourseResultSet.UpdateOrganizationCourse(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationProgramCourse", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationCourse(ViewModels.OrganizationProgramCourseView toRemove)
        {
            var entity = OrganizationCourseResultSet.RemoveOrganizationCourse(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationProgramCourse", User.Identity.Name);
            return Json(new { status = "success" });
        }


        #region Courses-PDF-Report

        [HttpPost]
        public JsonResult CoursesReportDataHandler(OrganizationProgramCourseForm param)
        {
            try
            {
                //int employeeID;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("MonthlyTimeSheet");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("MonthlyTimeSheet");


                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};

                return null; // Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("GenerateCoursesReport")]
        public ActionResult GenerateCoursesReport_Get()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationProgramCourseView vm = new OrganizationProgramCourseView();
            vm.list_program = OrganizationCourseResultSet.GetOrganizationProgramList();
            vm.list_default_type = OrganizationCourseResultSet.GetOrganizationProgramTypeList();

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateCoursesReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateCoursesReport_Post()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            int progID = 0, dTypeID = 0, dTypeNumber = 0, found = 0;
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            CoursesReport reportMaker = new CoursesReport();

            if (!int.TryParse(Request.Form["program_id"], out progID))
                return RedirectToAction("GenerateCoursesReport");

            if (!int.TryParse(Request.Form["default_type_id"], out dTypeID))
                return RedirectToAction("GenerateCoursesReport");

            if (!int.TryParse(Request.Form["default_type_number"], out dTypeNumber))
                return RedirectToAction("GenerateCoursesReport");

            CoursesReportData toRender = reportMaker.getCoursesData(progID, dTypeID, dTypeNumber);
            if (toRender == null)
                return RedirectToAction("GenerateCoursesReport");

            found = DownloadCoursesReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationProgramCourseView vm = new OrganizationProgramCourseView();
            vm.list_program = OrganizationCourseResultSet.GetOrganizationProgramList();
            vm.list_default_type = OrganizationCourseResultSet.GetOrganizationProgramTypeList();

            return View(vm);
        }


        private int DownloadCoursesReportPDF(CoursesReportData cdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to download PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBoldUnderline16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10.0f, 10.0f, 10.0f, 10.0f);

                    //// To download PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + cdata.org_logo_path); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 100.0f;
                    //logo.Height = 80.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 880.0f, 120.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    tableHeader.SpacingBefore = 5;
                    tableHeader.SpacingAfter = 5;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(cdata.org_title + "\n" + cdata.campus_code + "\n" + cdata.program_code, fBold16));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 4.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellETitle = new PdfPCell(new Phrase("Default Type: " + cdata.default_type_code, fBold12));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 0;

                    PdfPCell cellDPTitle = new PdfPCell(new Phrase("Courses List", fBold12));
                    cellDPTitle.Border = 0;
                    cellDPTitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(cellETitle);
                    tableEmployee.AddCell(cellDPTitle);

                    document.Add(tableEmployee);

                    //Paragraph p1 = new Paragraph("Name: " + sdata.employeeName, fBold9);
                    ////p1.SpacingBefore = 10;
                    //document.Add(p1);

                    //Paragraph p2 = new Paragraph("Employee Number: " + sdata.employeeCode, fBold9);
                    //document.Add(p2);

                    //Paragraph p3 = new Paragraph("Month: " + sdata.month, fBold9);
                    //document.Add(p3);

                    //Paragraph p4 = new Paragraph("Year: " + sdata.year, fBold9);
                    //document.Add(p4);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableCourses = new PdfPTable(new[] { 25.0f, 50.0f, 25.0f });
                    //PdfPTable tableSchedule = new PdfPTable(5);

                    tableCourses.WidthPercentage = 100;
                    tableCourses.HeaderRows = 1;
                    tableCourses.SpacingBefore = 3;
                    tableCourses.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Course Code", fBold12));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableCourses.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Title", fBold12));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableCourses.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Credit Hours", fBold12));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableCourses.AddCell(cell3);


                    if (cdata.crs_logs != null && cdata.crs_logs.Count > 0)
                    {
                        foreach (CoursesLog log in cdata.crs_logs)
                        {
                            PdfPCell cellData1 = new PdfPCell(new Phrase(log.course_code, fNormal8));
                            cellData1.HorizontalAlignment = 1;
                            tableCourses.AddCell(cellData1);

                            PdfPCell cellData2 = new PdfPCell(new Phrase(log.course_title, fNormal8));
                            //cellData2.HorizontalAlignment = 1;
                            tableCourses.AddCell(cellData2);

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.credit_hours, fNormal8));
                            cellData3.HorizontalAlignment = 1;
                            tableCourses.AddCell(cellData3);
                        }

                        document.Add(tableCourses);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Courses-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion


        #region Courses-Upload-Download-Sheets

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadCoursesCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<ManageStudentsEnrollmentImportExport.ManageEnrollmentCSV>
                    (
                        ManageStudentsEnrollmentImportExport.getManageStudentsEnrollmentCSVDownload(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-Courses.csv"
                    );


            return toReturn;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCoursesCSVFile(HttpPostedFileBase file)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = Path.GetFileName(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName).ToLower();

                if (FileExtension != ".csv")
                {
                    return RedirectToAction("ManageOrganizationCourse", "OrganizationsManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_enrollment.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidProgCode = false, invalidCrsCode = false, invalidCrsTitle = false, invalidBookName = false, invalidBookAuthor = false, invalidPType = false, invalidActive = false, invalidCols = false;

                    //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                    bool isDataRowFound = false; int a = 0;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            a++;

                            if (a == 2)
                            {
                                isDataRowFound = true;
                                break;
                            }
                        }
                    }

                    if (!isDataRowFound)
                    {
                        return RedirectToAction("ManageOrganizationCourse", "OrganizationsManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////

                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            strReadLine = strReadLine.TrimEnd(',');
                            strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                            strReadLine = strReadLine.Replace("\"", "");
                            strReadLine = strReadLine.TrimEnd(',');

                            string new_code = "";
                            string[] ecode_dt = strReadLine.Split(',');
                            if (ecode_dt.Length == 11)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("program_code") || ecode_dt[1].ToLower().Contains("course_code") || ecode_dt[10].ToLower().Contains("is_active_course"))
                                {
                                    continue;
                                }

                                //validate gc-year

                                //bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
                                //int iGVCampusID = 0; iGVCampusID = int.Parse(ViewModel.GlobalVariables.GV_EmployeeCampusID);
                                //if (!ValidateEnrCampusCodeAllowed(bGVIsSuperHRRole, iGVCampusID, ecode_dt[1]))
                                //{
                                //    invalidCampCode = true;
                                //    result = "NOT Allowed Campus-Code Found at Row-" + counter;
                                //    break;
                                //}

                                //validate program-code
                                if (!ValidateCrsProgramCode(ecode_dt[0]))
                                {
                                    invalidProgCode = true;
                                    result = "Invalid Program-Code Found at Row-" + counter;
                                    break;
                                }

                                //validate course-code
                                if (!ValidateCrsCourseCode(ecode_dt[1]))
                                {
                                    invalidCrsCode = true;
                                    result = "Invalid Course-Code Found at Row-" + counter;
                                    break;
                                }

                                //validate couese-title
                                if (!ValidateCrsCourseTitle(ecode_dt[2]))
                                {
                                    invalidCrsTitle = true;
                                    result = "Invalid Course-Title Found at Row-" + counter;
                                    break;
                                }

                                //validate book-name
                                if (!ValidateCrsBook(ecode_dt[3]))
                                {
                                    invalidBookName = true;
                                    result = "Invalid Book-Name Found at Row-" + counter;
                                    break;
                                }

                                //validate book-author
                                if (!ValidateCrsBook(ecode_dt[4]))
                                {
                                    invalidBookAuthor = true;
                                    result = "Invalid Book-Author Found at Row-" + counter;
                                    break;
                                }

                                //validate course-code
                                if (!ValidateCrsPType(ecode_dt[5]))
                                {
                                    invalidPType = true;
                                    result = "Invalid Program-Type Found at Row-" + counter;
                                    break;
                                }

                                //validate course-code
                                if (!ValidateCrsActive(ecode_dt[10]))
                                {
                                    invalidActive = true;
                                    result = "Invalid Active-Status Found at Row-" + counter;
                                    break;
                                }


                            }
                            else
                            {
                                invalidCols = true;
                                result = "Invalid Col(s) Found";
                                break;
                            }

                            //iterate to replace EmployeeCode only having 0 as prefix
                            //for (int i = 0; i < ecode_dt.Length; i++)
                            //{
                            //    if (i == 0)
                            //    {
                            //        strReadLine = new_code + ",";
                            //    }
                            //    else
                            //    {
                            //        strReadLine += ecode_dt[i] + ",";
                            //    }
                            //}

                            //strReadLine = strReadLine.TrimEnd(',');
                            content.Add(strReadLine);

                            //restrict to upload if 1000+ rows are found
                            /*if (counter > 1000)
                            {
                                invalidRowsCount = true;
                                result = "Max 1000 records are allowed be uploaded";
                                break;
                            }*/
                        }
                    }

                    if (invalidProgCode || invalidCrsCode || invalidCrsCode || invalidBookName || invalidBookAuthor || invalidPType || invalidActive || invalidCols)
                    {
                        return RedirectToAction("ManageOrganizationCourse", "OrganizationsManagement", new { result = result });
                    }
                    else
                    {
                        result = ManageCoursesImportExport.setCourses(content, User.Identity.Name);
                    }

                }

                if (result == "failed")
                {
                    return RedirectToAction("ManageOrganizationCourse", "OrganizationsManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("ManageOrganizationCourse", "OrganizationsManagement", new { result = "Successful" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("ManageOrganizationCourse", "OrganizationsManagement", new { result = "Select File first" });
        }

        private bool ValidateCrsCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrCampusCodeAllowed(bGVIsSuperHRRole, iCampusID, strCampusCode);

            return isValid;
        }

        private bool ValidateCrsProgramCode(string strProgramCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrProgramCode(strProgramCode);

            return isValid;
        }

        private bool ValidateCrsCourseCode(string strCourseCode)
        {
            bool isValid = false;

            isValid = (strCourseCode != null && strCourseCode != "") ? true : false;

            return isValid;
        }

        private bool ValidateCrsCourseTitle(string strCourseTitle)
        {
            bool isValid = false;

            isValid = (strCourseTitle != null && strCourseTitle != "") ? true : false;

            return isValid;
        }

        private bool ValidateCrsBook(string strCourseBook)
        {
            bool isValid = false;

            isValid = (strCourseBook != null && strCourseBook != "") ? true : false;

            return isValid;
        }

        private bool ValidateCrsPType(string strPType)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrProgramType(strPType);

            return isValid;
        }


        private bool ValidateCrsActive(string strActive)
        {
            bool isValid = false;

            strActive = strActive.ToLower();

            isValid = (strActive != null && strActive != "" && (strActive == "yes" || strActive == "no")) ? true : false;

            return isValid;
        }


        #endregion


        #endregion




        #region Organization-Enrollments

        [HttpGet]
        public ActionResult ManageOrganizationEnrollment()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationProgramCourseEnrollmentView vm = new OrganizationProgramCourseEnrollmentView();
            vm.list_general_calendar = OrganizationEnrollmentResultSet.GetOrganizationGeneralCalendarList();
            vm.list_program_course = OrganizationEnrollmentResultSet.GetOrganizationProgramCourseList();
            vm.list_enrolled_type = OrganizationEnrollmentResultSet.GetOrganizationProgramTypeList();
            vm.list_year = OrganizationEnrollmentResultSet.GetOrganizationYearList();
            vm.list_campus = OrganizationEnrollmentResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationEnrollmentResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_employee = OrganizationEnrollmentResultSet.GetOrganizationEmployeeStudentList(bGVIsSuperHRRole, iGVCampusID);

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_general_calnedar = OrganizationEnrollmentResultSet.GetOrganizationGeneralCalendarString();
            vm.str_campus = OrganizationEnrollmentResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program = OrganizationEnrollmentResultSet.GetOrganizationProgramString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_employee = OrganizationEnrollmentResultSet.GetOrganizationEmployeeStudentString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program_course = OrganizationEnrollmentResultSet.GetOrganizationProgramCourseString();
            vm.str_enrolled_type = OrganizationEnrollmentResultSet.GetOrganizationProgramTypeString();

            if (Request.QueryString["result"] != null && Request.QueryString["result"].ToString() != "")
                ViewBag.UpMessage = Request.QueryString["result"].ToString();
            else
                ViewBag.UpMessage = "";

            return View(vm);
        }

        [HttpPost]
        public ActionResult ManageOrganizationEnrollment(OrganizationProgramCourseEnrollmentForm eform)
        {
            int idCreated = 0;
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            try
            {
                OrganizationProgramCourseEnrollmentView eView = new OrganizationProgramCourseEnrollmentView();

                //eView.Id = rform.id;
                eView.GeneralCalendarId = eform.general_calendar_id;
                eView.CampusId = eform.campus_id;
                eView.ProgramId = eform.program_id;
                eView.EmployeeStudentId = eform.employee_student_id;
                eView.ProgramCourseId = eform.program_course_id;
                eView.EnrollmentTitle = eform.enrollment_title;
                eView.EnrolledProgramTypeId = eform.enrolled_program_type_id;
                eView.EnrolledProgramTypeNumber = eform.enrolled_program_type_number;
                eView.IsCourseFailed = eform.is_course_failed;
                eView.CreateDateEnr = DateTime.Now;

                idCreated = OrganizationEnrollmentResultSet.CreateOrganizationEnrollment(eView);
                if (idCreated > 0)
                {
                    //success
                    ViewBag.Message = "The enrollment is done successfully!";

                    var json = JsonConvert.SerializeObject(eform);
                    AuditTrail.insert(json, "OrganizationProgramCourseEnrollment", User.Identity.Name);
                }
                else if (idCreated == 0)
                {
                    //success
                    ViewBag.Message = "A enrollment with same student already exists";
                }
                else
                {
                    //exception
                    ViewBag.Message = "An exception occurred.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            ////////////////////////////////////////////////////////////////

            OrganizationProgramCourseEnrollmentView vm = new OrganizationProgramCourseEnrollmentView();
            vm.list_general_calendar = OrganizationEnrollmentResultSet.GetOrganizationGeneralCalendarList();
            vm.list_program_course = OrganizationEnrollmentResultSet.GetOrganizationProgramCourseList();
            vm.list_enrolled_type = OrganizationEnrollmentResultSet.GetOrganizationProgramTypeList();
            vm.list_year = OrganizationEnrollmentResultSet.GetOrganizationYearList();
            vm.list_campus = OrganizationEnrollmentResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationEnrollmentResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_employee = OrganizationEnrollmentResultSet.GetOrganizationEmployeeStudentList(bGVIsSuperHRRole, iGVCampusID);

            //'@Model.list_rooms', '@Model.list_courses', '@Model.list_lecture_groups', '@Model.list_lecturers'
            vm.str_general_calnedar = OrganizationEnrollmentResultSet.GetOrganizationGeneralCalendarString();
            vm.str_campus = OrganizationEnrollmentResultSet.GetOrganizationCampusString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program = OrganizationEnrollmentResultSet.GetOrganizationProgramString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_employee = OrganizationEnrollmentResultSet.GetOrganizationEmployeeStudentString(bGVIsSuperHRRole, iGVCampusID);
            vm.str_program_course = OrganizationEnrollmentResultSet.GetOrganizationProgramCourseString();
            vm.str_enrolled_type = OrganizationEnrollmentResultSet.GetOrganizationProgramTypeString();

            if (Request.QueryString["result"] != null && Request.QueryString["result"].ToString() != "")
                ViewBag.UpMessage = Request.QueryString["result"].ToString();
            else
                ViewBag.UpMessage = "";

            return View(vm);
        }

        [HttpPost]
        public JsonResult OrganizationEnrollmentDataHandler(DTParameters param)
        {
            try
            {
                var dtSource = new List<OrganizationProgramCourseEnrollmentView>();
                dtSource = OrganizationEnrollmentResultSet.getOrganizationEnrollmentByUserCode(User.Identity.Name);

                if (dtSource == null)
                {
                    return Json("No data found");
                }

                // get all employee view models
                //TimeTune.Attendance.getConsolidatedLogForEmp(param.Search.Value, param.from_date.ToString(), param.to_date.ToString(), User.Identity.Name, out data);

                List<OrganizationProgramCourseEnrollmentView> data = OrganizationEnrollmentResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtSource);
                data = data.OrderByDescending(o => o.Id).ToList();
                int count = OrganizationEnrollmentResultSet.Count(param.Search.Value, dtSource);

                //data = LeaveApplicationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);

                DTResult<OrganizationProgramCourseEnrollmentView> result = new DTResult<OrganizationProgramCourseEnrollmentView>
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

            //return null;
        }


        [HttpPost]
        public ActionResult UpdateOrganizationEnrollment(ViewModels.OrganizationProgramCourseEnrollmentView toUpdate)
        {
            int respose = 0;
            string strStatus = "";

            var json = JsonConvert.SerializeObject(toUpdate);
            respose = OrganizationEnrollmentResultSet.UpdateOrganizationEnrollment(toUpdate);

            if (respose == 0)
            {
                strStatus = "already";
            }
            else if (respose > 0)
            {
                strStatus = "success";
                AuditTrail.update(json, "OrganizationProgramCourseEnrollment", User.Identity.Name);
            }
            else
            {
                strStatus = "error";
            }

            return Json(new { status = strStatus });
        }

        [HttpPost]
        public ActionResult RemoveOrganizationEnrollment(ViewModels.OrganizationProgramCourseEnrollmentView toRemove)
        {
            var entity = OrganizationEnrollmentResultSet.RemoveOrganizationEnrollment(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "OrganizationProgramCourseEnrollment", User.Identity.Name);
            return Json(new { status = "success" });
        }


        #region Enrollment-PDF-Report

        [HttpPost]
        public JsonResult EnrollmentReportDataHandler(OrganizationCampusRoomCourseScheduleForm param)
        {
            try
            {
                //int employeeID;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("MonthlyTimeSheet");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("MonthlyTimeSheet");


                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};

                return null; // Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("GenerateEnrollmentReport")]
        public ActionResult GenerateEnrollmentReport_Get()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationProgramCourseEnrollmentView vm = new OrganizationProgramCourseEnrollmentView();
            vm.list_year = OrganizationEnrollmentResultSet.GetOrganizationYearList();
            vm.list_campus = OrganizationEnrollmentResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationEnrollmentResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program_course = OrganizationEnrollmentResultSet.GetOrganizationProgramCourseList();

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateEnrollmentReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateEnrollmentReport_Post()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            int yearID = 0, campusID = 0, progID = 0, found = 0;
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            EnrollmentReport reportMaker = new EnrollmentReport();

            if (!int.TryParse(Request.Form["year_id"], out yearID))
                return RedirectToAction("GenerateEnrollmentReport");

            if (!int.TryParse(Request.Form["campus_id"], out campusID))
                return RedirectToAction("GenerateEnrollmentReport");

            if (!int.TryParse(Request.Form["program_id"], out progID))
                return RedirectToAction("GenerateEnrollmentReport");

            EnrollmentReportData toRender = reportMaker.getEnrollmentData(yearID, campusID, progID);
            if (toRender == null)
                return RedirectToAction("GenerateEnrollmentReport");

            found = DownloadEnrollmentReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationProgramCourseEnrollmentView vm = new OrganizationProgramCourseEnrollmentView();
            vm.list_year = OrganizationEnrollmentResultSet.GetOrganizationYearList();
            vm.list_campus = OrganizationEnrollmentResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program = OrganizationEnrollmentResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program_course = OrganizationEnrollmentResultSet.GetOrganizationProgramCourseList();

            return View(vm);
        }


        private int DownloadEnrollmentReportPDF(EnrollmentReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to download PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBoldUnderline16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4.Rotate(), 5.0f, 5.0f, 5.0f, 5.0f);

                    //// To download PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.org_logo_path); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 100.0f;
                    //logo.Height = 80.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 880.0f, 120.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    tableHeader.SpacingBefore = 5;
                    tableHeader.SpacingAfter = 5;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.org_title + "\n" + sdata.campus_code + "\n" + "List of Students Enrolled in Courses", fBold16));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 4.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellEName = new PdfPCell(new Phrase("Year: " + sdata.year_code, fBold12));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase(sdata.program_code, fBold12));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //Paragraph p1 = new Paragraph("Name: " + sdata.employeeName, fBold9);
                    ////p1.SpacingBefore = 10;
                    //document.Add(p1);

                    //Paragraph p2 = new Paragraph("Employee Number: " + sdata.employeeCode, fBold9);
                    //document.Add(p2);

                    //Paragraph p3 = new Paragraph("Month: " + sdata.month, fBold9);
                    //document.Add(p3);

                    //Paragraph p4 = new Paragraph("Year: " + sdata.year, fBold9);
                    //document.Add(p4);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableEnrollment = new PdfPTable(new[] { 7.0f, 20.0f, 8.0f, 15.0f, 27.0f, 8.0f, 7.0f, 8.0f });
                    //PdfPTable tableSchedule = new PdfPTable(5);

                    tableEnrollment.WidthPercentage = 100;
                    tableEnrollment.HeaderRows = 1;
                    tableEnrollment.SpacingBefore = 3;
                    tableEnrollment.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("Student Code", fBold8));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Name", fBold8));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell2);

                    PdfPCell cell21 = new PdfPCell(new Phrase("LGroup", fBold8));
                    cell21.BackgroundColor = Color.LIGHT_GRAY;
                    cell21.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell21);

                    PdfPCell cell22 = new PdfPCell(new Phrase("Title", fBold8));
                    cell22.BackgroundColor = Color.LIGHT_GRAY;
                    cell22.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell22);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Course", fBold8));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Prog Type", fBold8));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Prog No.", fBold8));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Is Failed", fBold8));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableEnrollment.AddCell(cell6);

                    if (sdata.std_logs != null && sdata.std_logs.Count > 0)
                    {
                        foreach (EnrollmentStudentLog log in sdata.std_logs)
                        {
                            //{
                            //    log.finalRemarks = log.finalRemarks + ((log.hasManualAttendance) ? "*" : "");
                            //}

                            //PdfPCell cellData1 = new PdfPCell(new Phrase(log.date, FontFactory.GetFont("Arial", 11, Font.NORMAL, Color.BLACK)));
                            //cellData1.HorizontalAlignment = 0; // 0 for left, 1 for Center - 2 for Right
                            //tableMid.AddCell(log.date);

                            PdfPCell cellData1 = new PdfPCell(new Phrase(log.student_code, fNormal8));
                            //cellData1.HorizontalAlignment = 0;
                            tableEnrollment.AddCell(cellData1);

                            PdfPCell cellData2 = new PdfPCell(new Phrase(log.student_name, fNormal8));
                            cellData2.HorizontalAlignment = 1;
                            tableEnrollment.AddCell(cellData2);

                            PdfPCell cellData21 = new PdfPCell(new Phrase(log.lgroup_name, fNormal8));
                            cellData21.HorizontalAlignment = 1;
                            tableEnrollment.AddCell(cellData21);

                            PdfPCell cellData22 = new PdfPCell(new Phrase(log.enrollment_title, fNormal8));
                            cellData22.HorizontalAlignment = 1;
                            tableEnrollment.AddCell(cellData22);

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.crs_list, fNormal8));
                            //cellData3.HorizontalAlignment = 1;
                            tableEnrollment.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.program_type_name, fNormal8));
                            cellData4.HorizontalAlignment = 1;
                            tableEnrollment.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.program_type_number.ToString(), fNormal8));
                            //cellData5.HorizontalAlignment = 1;
                            tableEnrollment.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.is_course_failed_name, fNormal8));
                            //cellData6.HorizontalAlignment = 1;
                            tableEnrollment.AddCell(cellData6);
                        }

                        document.Add(tableEnrollment);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically

                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=Enrollment-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion


        #region Enrollment-Upload-Download-Sheets

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadEnrollmentCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<ManageStudentsEnrollmentImportExport.ManageEnrollmentCSV>
                    (
                        ManageStudentsEnrollmentImportExport.getManageStudentsEnrollmentCSVDownload(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-Enrollment.csv"
                    );


            return toReturn;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadEnrollmentCSVFile(HttpPostedFileBase file)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = Path.GetFileName(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName).ToLower();

                if (FileExtension != ".csv")
                {
                    return RedirectToAction("ManageOrganizationEnrollment", "OrganizationsManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_enrollment.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidCampCode = false, invalidProgCode = false, invalidSCode = false, invalidGCYear = false, invalidCCode = false, invalidPType = false, invalidActive = false, invalidCols = false;

                    //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                    bool isDataRowFound = false; int a = 0;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            a++;

                            if (a == 2)
                            {
                                isDataRowFound = true;
                                break;
                            }
                        }
                    }

                    if (!isDataRowFound)
                    {
                        return RedirectToAction("ManageOrganizationEnrollment", "OrganizationsManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////

                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            strReadLine = strReadLine.TrimEnd(',');
                            strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                            strReadLine = strReadLine.Replace("\"", "");
                            strReadLine = strReadLine.TrimEnd(',');

                            string new_code = "";
                            string[] ecode_dt = strReadLine.Split(',');
                            if (ecode_dt.Length == 9)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("year") || ecode_dt[1].ToLower().Contains("campus_code") || ecode_dt[8].ToLower().Contains("is_course_failed"))
                                {
                                    continue;
                                }

                                //validate gc-year
                                if (!ValidateEnrGeneralCalendarYear(ecode_dt[0]))
                                {
                                    invalidGCYear = true;
                                    result = "Invalid General Calendar Year Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateEnrNOTPastYear(ecode_dt[0]))
                                {
                                    invalidGCYear = true;
                                    result = "Invalid Past Year Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateEnrCampusCode(ecode_dt[1]))
                                {
                                    invalidCampCode = true;
                                    result = "Invalid Campus Code Found at Row-" + counter;
                                    break;
                                }

                                bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
                                int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);
                                if (!ValidateEnrCampusCodeAllowed(bGVIsSuperHRRole, iGVCampusID, ecode_dt[1]))
                                {
                                    invalidCampCode = true;
                                    result = "NOT Allowed Campus-Code Found at Row-" + counter;
                                    break;
                                }

                                //validate program-code
                                if (!ValidateEnrProgramCode(ecode_dt[2]))
                                {
                                    invalidProgCode = true;
                                    result = "Invalid Program Found at Row-" + counter;
                                    break;
                                }

                                //validate student-code in that campus
                                if (!ValidateEnrStudentCode(ecode_dt[3]))
                                {
                                    invalidSCode = true;
                                    result = "Invalid Student Code Found at Row-" + counter;
                                    break;
                                }

                                //validate student-code in that campus
                                if (!ValidateEnrStudentCodeCampus(ecode_dt[1], ecode_dt[3]))
                                {
                                    invalidSCode = true;
                                    result = "Invalid Student Code assigned to Campus Found at Row-" + counter;
                                    break;
                                }

                                //validate course-code
                                if (!ValidateEnrPCourseCode(ecode_dt[4]))
                                {
                                    invalidCCode = true;
                                    result = "Invalid Course Found at Row-" + counter;
                                    break;
                                }

                                //validate course-code
                                if (!ValidateEnrPType(ecode_dt[6]))
                                {
                                    invalidPType = true;
                                    result = "Invalid Program-Type Found at Row-" + counter;
                                    break;
                                }

                                //validate is-active
                                if (!ValidateEnrActive(ecode_dt[8]))
                                {
                                    invalidActive = true;
                                    result = "Invalid Active-Status Found at Row-" + counter;
                                    break;
                                }


                            }
                            else
                            {
                                invalidCols = true;
                                result = "Invalid Col(s) Found";
                                break;
                            }

                            //iterate to replace EmployeeCode only having 0 as prefix
                            //for (int i = 0; i < ecode_dt.Length; i++)
                            //{
                            //    if (i == 0)
                            //    {
                            //        strReadLine = new_code + ",";
                            //    }
                            //    else
                            //    {
                            //        strReadLine += ecode_dt[i] + ",";
                            //    }
                            //}

                            //strReadLine = strReadLine.TrimEnd(',');
                            content.Add(strReadLine);

                            //restrict to upload if 1000+ rows are found
                            /*if (counter > 1000)
                            {
                                invalidRowsCount = true;
                                result = "Max 1000 records are allowed be uploaded";
                                break;
                            }*/
                        }
                    }

                    if (invalidGCYear || invalidCampCode || invalidProgCode || invalidSCode || invalidCCode || invalidPType || invalidActive || invalidCols)
                    {
                        return RedirectToAction("ManageOrganizationEnrollment", "OrganizationsManagement", new { result = result });
                    }
                    else
                    {
                        result = ManageStudentsEnrollmentImportExport.setEnrollment(content, User.Identity.Name);
                    }

                }

                if (result == "failed")
                {
                    return RedirectToAction("ManageOrganizationEnrollment", "OrganizationsManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("ManageOrganizationEnrollment", "OrganizationsManagement", new { result = "Successful" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("ManageOrganizationEnrollment", "OrganizationsManagement", new { result = "Select File first" });
        }

        private bool ValidateEnrGeneralCalendarYear(string strGCYear)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrGCYear(strGCYear);

            return isValid;
        }

        private bool ValidateEnrNOTPastYear(string strGCYear)
        {
            bool isValid = true;

            if (int.Parse(strGCYear) < DateTime.Now.Year)
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateEnrCampusCode(string strCampusCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrCampusCode(strCampusCode);

            return isValid;
        }


        private bool ValidateEnrCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrCampusCodeAllowed(bGVIsSuperHRRole, iCampusID, strCampusCode);

            return isValid;
        }

        private bool ValidateEnrProgramCode(string strProgramCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrProgramCode(strProgramCode);

            return isValid;
        }


        private bool ValidateEnrStudentCode(string strStudentCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrStudentCode(strStudentCode);

            return isValid;
        }

        private bool ValidateEnrStudentCodeCampus(string strCampusCode, string strStudentCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrStudentCodeCampus(strCampusCode, strStudentCode);

            return isValid;
        }

        private bool ValidateEnrPCourseCode(string strPCourseCode)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrProgramCourseCode(strPCourseCode);

            return isValid;
        }


        private bool ValidateEnrPType(string strPType)
        {
            bool isValid = false;

            isValid = ManageStudentsEnrollmentImportExport.validateEnrProgramType(strPType);

            return isValid;
        }


        private bool ValidateEnrActive(string strActive)
        {
            bool isValid = false;

            strActive = strActive.ToLower();

            isValid = (strActive != null && strActive != "" && (strActive == "yes" || strActive == "no")) ? true : false;

            return isValid;
        }


        #endregion



        #endregion


        #region Course-Attendance-Students-Campus-Reports

        #region Course-Attendance-Student-PDF-Report

        [HttpPost]
        public JsonResult CourseAttendanceStudentReportDataHandler(OrganizationCampusRoomCourseScheduleView param)
        {
            try
            {
                //int employeeID;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("MonthlyTimeSheet");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("MonthlyTimeSheet");


                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};

                return null; // Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("GenerateCourseAttendanceStudentReport")]
        public ActionResult GenerateCourseAttendanceStudentReport_Get()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCourseAttendanceView vm = new OrganizationCourseAttendanceView();
            vm.list_students = OrganizationCourseAttendanceResultSet.GetOrganizationEmployeeStudentList(bGVIsSuperHRRole, iGVCampusID);
            vm.str_students = OrganizationCourseAttendanceResultSet.GetOrganizationEmployeeStudentString(bGVIsSuperHRRole, iGVCampusID);

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateCourseAttendanceStudentReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateCourseAttendanceStudentReport_Post()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            int found = 0, iStudentID = 0; string strFromDate = "", strToDate = "";
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            CourseAttendanceReport reportMaker = new CourseAttendanceReport();

            if (!int.TryParse(Request.Form["student_id"], out iStudentID))
                return RedirectToAction("GenerateCourseAttendanceStudentReport");

            if (Request.Form["from_date"] != null && Request.Form["from_date"].ToString() != "")
                strFromDate = Request.Form["from_date"];
            else
                return RedirectToAction("GenerateCourseAttendanceStudentReport");

            if (Request.Form["to_date"] != null && Request.Form["to_date"].ToString() != "")
                strToDate = Request.Form["to_date"];
            else
                return RedirectToAction("GenerateCourseAttendanceStudentReport");

            CourseAttendanceReportData toRender = reportMaker.getCourseAttendanceStudentReport(strFromDate, strToDate, iStudentID);
            if (toRender == null)
                return RedirectToAction("GenerateCourseAttendanceStudentReport");

            found = DownloadCourseAttendanceStudentReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationCourseAttendanceView vm = new OrganizationCourseAttendanceView();
            vm.list_students = OrganizationCourseAttendanceResultSet.GetOrganizationEmployeeStudentList(bGVIsSuperHRRole, iGVCampusID);
            vm.str_students = OrganizationCourseAttendanceResultSet.GetOrganizationEmployeeStudentString(bGVIsSuperHRRole, iGVCampusID);

            return View(vm);
        }

        private int DownloadCourseAttendanceStudentReportPDF(CourseAttendanceReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);

                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.orgLogo); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.orgName + "\n" + sdata.campusName + "\n" + sdata.progCode + "-" + sdata.progTitle, fBold14));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellEName = new PdfPCell(new Phrase("Student Code & Name: " + sdata.employeeCode + " - " + sdata.employeeName, fBold9));
                    cellEName.Border = 0;
                    tableEInfo.AddCell(cellEName);

                    PdfPCell cellECode = new PdfPCell(new Phrase("Father's Name: " + sdata.employeeFatherName, fBold9));
                    cellECode.Border = 0;
                    tableEInfo.AddCell(cellECode);

                    PdfPCell cellRCode = new PdfPCell(new Phrase("Enrolled In: " + sdata.enrolledProgramText + "-" + sdata.enrolledProgramNumber, fBold9));
                    cellRCode.Border = 0;
                    tableEInfo.AddCell(cellRCode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase("Attendance Date Range: " + sdata.dateRange, fBold9));
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);

                    //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.toDate, fBold9));
                    //cellEYear.Border = 0;
                    //tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    //Paragraph p1 = new Paragraph("Name: " + sdata.employeeName, fBold9);
                    ////p1.SpacingBefore = 10;
                    //document.Add(p1);

                    //Paragraph p2 = new Paragraph("Employee Code: " + sdata.employeeCode, fBold9);
                    //document.Add(p2);

                    //Paragraph p3 = new Paragraph("Month: " + sdata.month, fBold9);
                    //document.Add(p3);

                    //Paragraph p4 = new Paragraph("Year: " + sdata.year, fBold9);
                    //document.Add(p4);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 18.0f, 18.0f, 10.0f, 18.0f, 10.0f, 10.0f, 26.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    //PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold8));
                    //cell1.BackgroundColor = Color.LIGHT_GRAY;
                    //cell1.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Course Code", fBold9));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Time In [Actual]", fBold9));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Remarks In", fBold9));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Time Out [Actual]", fBold9));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Remarks Out", fBold9));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Final Remarks", fBold9));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Device In/Out", fBold9));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase("Device Out", fBold9));
                    //cell9.BackgroundColor = Color.LIGHT_GRAY;
                    //cell9.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell9);

                    if (sdata.logs.Length > 0)
                    {
                        int j = 1, m = 0;
                        bool isNewHeading = false;
                        string strCurrDate = "", strPrevDate = "";

                        foreach (CourseAttendanceLog log in sdata.logs)
                        {
                            if (j == 1)
                            {
                                isNewHeading = true;
                                strCurrDate = log.schedule_date_time_text.ToString();
                                strPrevDate = log.schedule_date_time_text.ToString();

                                m = 1;
                            }
                            else
                            {
                                if (strCurrDate == log.schedule_date_time_text.ToString())
                                {
                                    strCurrDate = strPrevDate;
                                    isNewHeading = false;

                                    m++;
                                }
                                else
                                {
                                    strCurrDate = log.schedule_date_time_text.ToString();
                                    strPrevDate = log.schedule_date_time_text.ToString();
                                    isNewHeading = true;

                                    m = 1;
                                }
                            }

                            if (isNewHeading)
                            {
                                tableMid.AddCell(new PdfPCell(new Phrase("DATE: " + strCurrDate, fBold9)) { Colspan = 7 });
                            }

                            //PdfPCell cellData1 = new PdfPCell(new Phrase(log.schedule_date_time_text, fNormal8));
                            ////cellData1.HorizontalAlignment = 0;
                            //tableMid.AddCell(cellData1);

                            if (log.course_code != null && log.course_code == "-")
                            {
                                PdfPCell cellData2 = new PdfPCell(new Phrase("OFF", fNormal8));
                                cellData2.HorizontalAlignment = 1;
                                tableMid.AddCell(cellData2);
                            }
                            else
                            {
                                PdfPCell cellData2 = new PdfPCell(new Phrase(log.course_code, fNormal8));
                                cellData2.HorizontalAlignment = 1;
                                tableMid.AddCell(cellData2);
                            }

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.student_time_in_text, fNormal8));
                            cellData3.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.status_in, fNormal8));
                            //cellData4.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.student_time_out_text, fNormal8));
                            cellData5.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.status_out, fNormal8));
                            //cellData6.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData6);

                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.final_remarks, fNormal8));
                            //cellData7.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData7);

                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminal_in_code + " /\n" + log.terminal_out_code, fNormal7));
                            //cellData8.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData8);

                            //PdfPCell cellData9 = new PdfPCell(new Phrase(log.terminal_out_code, fNormal7));
                            ////cellData9.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData9);

                            j++;
                        }

                        document.Add(tableMid);
                    }

                    //Summary Count table
                    if (sdata.counts.Length > 0)
                    {
                        // Summary heading
                        Paragraph p_summary = new Paragraph("Summary", fBold10);
                        document.Add(p_summary);

                        // ---------- Count Table ---------------------
                        //set table with 595 pixels width - subtract 10x2 from either sides border
                        PdfPTable tableCount = new PdfPTable(new[] { 40.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f });
                        tableCount.WidthPercentage = 100;
                        tableCount.HeaderRows = 1;
                        tableCount.SpacingBefore = 3;
                        tableCount.SpacingAfter = 3;

                        PdfPCell cellc1 = new PdfPCell(new Phrase("Course Code", fBold8));
                        cellc1.BackgroundColor = Color.LIGHT_GRAY;
                        cellc1.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc1);

                        PdfPCell cellc2 = new PdfPCell(new Phrase("Present [%]", fBold8));
                        cellc2.BackgroundColor = Color.LIGHT_GRAY;
                        cellc2.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc2);

                        PdfPCell cellc3 = new PdfPCell(new Phrase("Absent [%]", fBold8));
                        cellc3.BackgroundColor = Color.LIGHT_GRAY;
                        cellc3.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc3);

                        PdfPCell cellc4 = new PdfPCell(new Phrase("Total Classes", fBold8));
                        cellc4.BackgroundColor = Color.LIGHT_GRAY;
                        cellc4.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc4);

                        PdfPCell cellc5 = new PdfPCell(new Phrase("Late", fBold8));
                        cellc5.BackgroundColor = Color.LIGHT_GRAY;
                        cellc5.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc5);

                        PdfPCell cellc6 = new PdfPCell(new Phrase("Early Out", fBold8));
                        cellc6.BackgroundColor = Color.LIGHT_GRAY;
                        cellc6.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc6);

                        PdfPCell cellc7 = new PdfPCell(new Phrase("Off", fBold8));
                        cellc7.BackgroundColor = Color.LIGHT_GRAY;
                        cellc7.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc7);

                        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                        foreach (CourseAttendanceCountLog log in sdata.counts)
                        {
                            PdfPCell cellData1 = new PdfPCell(new Phrase(log.course_code_title, fNormal8));
                            //cellData1.HorizontalAlignment = 0;
                            tableCount.AddCell(cellData1);

                            PdfPCell cellData2 = new PdfPCell(new Phrase(log.present_count.ToString() + " [" + log.per_present.ToString("P2", nfi) + "]", fNormal8));
                            cellData2.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData2);

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.absent_count.ToString() + " [" + log.per_absent.ToString("P2", nfi) + "]", fNormal8));
                            cellData3.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.total_count.ToString(), fNormal8));
                            cellData4.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.late_count.ToString(), fNormal8));
                            cellData5.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.early_count.ToString(), fNormal8));
                            cellData6.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData6);

                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.off_count.ToString(), fNormal8));
                            cellData7.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData7);
                        }

                        document.Add(tableCount);
                    }

                    if (sdata.logs.Length > 0)
                    {
                        //// Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        //// ---------- Last Table ---------------------
                        //PdfPTable tableEnd = new PdfPTable(new[] { 25.0f, 75.0f });
                        //tableEnd.WidthPercentage = 100;
                        //tableEnd.HeaderRows = 0;
                        //tableEnd.SpacingBefore = 3;
                        //tableEnd.SpacingAfter = 3;

                        //PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        //lt_cell_11.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_11);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        //PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Absent:", fBold9));
                        //lt_cell_21.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_21);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        ////PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Off:", fBold9));
                        ////lt_cell_31.BackgroundColor = Color.LIGHT_GRAY;
                        ////tableEnd.AddCell(lt_cell_31);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalOff, fNormal8));

                        //PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Late:", fBold9));
                        //lt_cell_41.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_41);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        ////PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Leave:", fBold9));
                        ////lt_cell_51.BackgroundColor = Color.LIGHT_GRAY;
                        ////tableEnd.AddCell(lt_cell_51);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        //PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        //lt_cell_61.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_61);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        //PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        //lt_cell_71.BackgroundColor = Color.LIGHT_GRAY;
                        //tableEnd.AddCell(lt_cell_71);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        //document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, OFF-Off, PLO-Present Late, PLE-Present Late Early Out, POE-On Time Early Out, POM-On Time Miss Punch, PMO-Miss Punch & Left On Time, PLM-Late Miss Punch, *-Manually Updated", fNormal7);
                        p_abrv.SpacingBefore = 1;
                        //p_nsig.Alignment = 2;
                        document.Add(p_abrv);

                        Paragraph p_late = new Paragraph("NOTE: After 15 min of actual class time, LATE will be marked.", fNormal7);
                        p_late.SpacingBefore = 2;
                        //p_late.Alignment = 2;
                        document.Add(p_late);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=CourseAttendanceStudent-" + sdata.employeeCode + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion

        #region Course-Attendance-Campus-PDF-Report

        [HttpPost]
        public JsonResult CourseAttendanceCampusReportDataHandler(OrganizationCampusRoomCourseScheduleView param)
        {
            try
            {
                //int employeeID;

                //if (!int.TryParse(param.employee_id, out employeeID))
                //    return RedirectToAction("MonthlyTimeSheet");

                //string month = param.month;

                //BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

                //BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

                //if (toRender == null)
                //    return RedirectToAction("MonthlyTimeSheet");


                var data = new List<MonthlyTimesheetAttendanceLog>();

                // get all employee view models

                //int count = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<MonthlyTimesheetAttendanceLog> result = new DTResult<MonthlyTimesheetAttendanceLog>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};

                return null; // Json(result);
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpGet]
        [ActionName("GenerateCourseAttendanceCampusReport")]
        public ActionResult GenerateCourseAttendanceCampusReport_Get()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationCourseAttendanceView vm = new OrganizationCourseAttendanceView();
            vm.list_campuses = OrganizationCourseAttendanceResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_programs = OrganizationCourseAttendanceResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program_types = OrganizationCourseAttendanceResultSet.GetOrganizationProgramTypeList();
            vm.list_program_shifts = OrganizationCourseAttendanceResultSet.GetOrganizationProgramShiftList();
            vm.list_program_groups = OrganizationCourseAttendanceResultSet.GetOrganizationProgramGroupList();

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateCourseAttendanceCampusReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateCourseAttendanceCampusReport_Post()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            int found = 0, iCampusID = 0, iProgramID = 0, iProgramTypeID = 0, iProgramTypeNumber = 0, iProgramShiftID = 0, iProgramGroupID = 0; string strFromDate = "", strToDate = "";
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            CourseAttendanceReport reportMaker = new CourseAttendanceReport();

            if (Request.Form["from_date"] != null && Request.Form["from_date"].ToString() != "")
                strFromDate = Request.Form["from_date"];
            else
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            if (Request.Form["to_date"] != null && Request.Form["to_date"].ToString() != "")
                strToDate = Request.Form["to_date"];
            else
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            if (!int.TryParse(Request.Form["campus_id"], out iCampusID))
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            if (!int.TryParse(Request.Form["program_id"], out iProgramID))
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            if (!int.TryParse(Request.Form["program_type_id"], out iProgramTypeID))
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            if (!int.TryParse(Request.Form["program_type_number"], out iProgramTypeNumber))
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            if (!int.TryParse(Request.Form["program_shift_id"], out iProgramShiftID))
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            if (!int.TryParse(Request.Form["program_group_id"], out iProgramGroupID))
                return RedirectToAction("GenerateCourseAttendanceCampusReport");


            CourseAttendanceReportData toRender = reportMaker.getCourseAttendanceCampusReport(strFromDate, strToDate, iCampusID, iProgramID, iProgramTypeID, iProgramTypeNumber, iProgramShiftID, iProgramGroupID);
            if (toRender == null)
                return RedirectToAction("GenerateCourseAttendanceCampusReport");

            found = DownloadCourseAttendanceCampusReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationCourseAttendanceView vm = new OrganizationCourseAttendanceView();
            vm.list_campuses = OrganizationCourseAttendanceResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_programs = OrganizationCourseAttendanceResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program_types = OrganizationCourseAttendanceResultSet.GetOrganizationProgramTypeList();
            vm.list_program_shifts = OrganizationCourseAttendanceResultSet.GetOrganizationProgramShiftList();
            vm.list_program_groups = OrganizationCourseAttendanceResultSet.GetOrganizationProgramGroupList();

            return View(vm);
        }

        private int DownloadCourseAttendanceCampusReportPDF(CourseAttendanceReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.orgLogo); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.orgName + "\n" + sdata.campusName + "\n" + sdata.progCode + "-" + sdata.progTitle, fBold12));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellRCode = new PdfPCell(new Phrase("Enrolled In: " + sdata.enrolledProgramText + "-" + sdata.enrolledProgramNumber, fBold9));
                    cellRCode.Border = 0;
                    tableEInfo.AddCell(cellRCode);

                    PdfPCell cellEMonth = new PdfPCell(new Phrase("Attendance Date Range: " + sdata.dateRange, fBold9));
                    cellEMonth.Border = 0;
                    tableEInfo.AddCell(cellEMonth);

                    //PdfPCell cellEYear = new PdfPCell(new Phrase("Year: " + sdata.toDate, fBold9));
                    //cellEYear.Border = 0;
                    //tableEInfo.AddCell(cellEYear);

                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);


                    /*
                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 10.0f, 22.0f, 10.0f, 22.0f, 10.0f, 10.0f, 26.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    //PdfPCell cell1 = new PdfPCell(new Phrase("Date", fBold8));
                    //cell1.BackgroundColor = Color.LIGHT_GRAY;
                    //cell1.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Course Code", fBold9));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Time In [Actual]", fBold9));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Remarks In", fBold9));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Time Out [Actual]", fBold9));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Remarks Out", fBold9));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Final Remarks", fBold9));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    PdfPCell cell8 = new PdfPCell(new Phrase("Device In/Out", fBold9));
                    cell8.BackgroundColor = Color.LIGHT_GRAY;
                    cell8.HorizontalAlignment = 1;
                    tableMid.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase("Device Out", fBold9));
                    //cell9.BackgroundColor = Color.LIGHT_GRAY;
                    //cell9.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell9);

                    if (sdata.logs.Length > 0)
                    {
                        int j = 1, m = 0;
                        bool isNewHeading = false;
                        string strCurrDate = "", strPrevDate = "";

                        foreach (CourseAttendanceLog log in sdata.logs)
                        {
                            if (j == 1)
                            {
                                isNewHeading = true;
                                strCurrDate = log.schedule_date_time_text.ToString();
                                strPrevDate = log.schedule_date_time_text.ToString();

                                m = 1;
                            }
                            else
                            {
                                if (strCurrDate == log.schedule_date_time_text.ToString())
                                {
                                    strCurrDate = strPrevDate;
                                    isNewHeading = false;

                                    m++;
                                }
                                else
                                {
                                    strCurrDate = log.schedule_date_time_text.ToString();
                                    strPrevDate = log.schedule_date_time_text.ToString();
                                    isNewHeading = true;

                                    m = 1;
                                }
                            }

                            if (isNewHeading)
                            {
                                tableMid.AddCell(new PdfPCell(new Phrase("DATE: " + strCurrDate, fBold9)) { Colspan = 7 });
                            }

                            //PdfPCell cellData1 = new PdfPCell(new Phrase(log.schedule_date_time_text, fNormal8));
                            ////cellData1.HorizontalAlignment = 0;
                            //tableMid.AddCell(cellData1);

                            PdfPCell cellData2 = new PdfPCell(new Phrase(log.course_code, fNormal8));
                            cellData2.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData2);

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.student_time_in_text, fNormal8));
                            cellData3.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.status_in, fNormal8));
                            //cellData4.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.student_time_out_text, fNormal8));
                            cellData5.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.status_out, fNormal8));
                            //cellData6.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData6);

                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.final_remarks, fNormal8));
                            //cellData7.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData7);

                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.terminal_in_code, fNormal7));
                            //cellData8.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData8);

                            //PdfPCell cellData9 = new PdfPCell(new Phrase(log.terminal_out_code, fNormal7));
                            ////cellData9.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData9);

                            j++;
                        }

                        document.Add(tableMid);
                    }
                    */


                    //Summary Count table
                    if (sdata.counts.Length > 0)
                    {
                        int j = 1, m = 0;
                        bool isNewHeading = false;
                        string strCurrStdCode = "", strPrevStdCode = "";

                        // Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        // ---------- Count Table ---------------------
                        //set table with 595 pixels width - subtract 10x2 from either sides border
                        PdfPTable tableCount = new PdfPTable(new[] { 40.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f });
                        tableCount.WidthPercentage = 100;
                        tableCount.HeaderRows = 1;
                        tableCount.SpacingBefore = 3;
                        tableCount.SpacingAfter = 3;

                        PdfPCell cellc1_1 = new PdfPCell(new Phrase("Course Code", fBold8));
                        cellc1_1.BackgroundColor = Color.LIGHT_GRAY;
                        cellc1_1.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc1_1);

                        PdfPCell cellc2 = new PdfPCell(new Phrase("Present [%]", fBold8));
                        cellc2.BackgroundColor = Color.LIGHT_GRAY;
                        cellc2.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc2);

                        PdfPCell cellc3 = new PdfPCell(new Phrase("Absent [%]", fBold8));
                        cellc3.BackgroundColor = Color.LIGHT_GRAY;
                        cellc3.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc3);

                        PdfPCell cellc4 = new PdfPCell(new Phrase("Total Classes", fBold8));
                        cellc4.BackgroundColor = Color.LIGHT_GRAY;
                        cellc4.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc4);

                        PdfPCell cellc5 = new PdfPCell(new Phrase("Late", fBold8));
                        cellc5.BackgroundColor = Color.LIGHT_GRAY;
                        cellc5.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc5);

                        PdfPCell cellc6 = new PdfPCell(new Phrase("Early Out", fBold8));
                        cellc6.BackgroundColor = Color.LIGHT_GRAY;
                        cellc6.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc6);

                        PdfPCell cellc7 = new PdfPCell(new Phrase("Off", fBold8));
                        cellc7.BackgroundColor = Color.LIGHT_GRAY;
                        cellc7.HorizontalAlignment = 1;
                        tableCount.AddCell(cellc7);

                        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                        foreach (CourseAttendanceCountLog log in sdata.counts)
                        {
                            if (j == 1)
                            {
                                isNewHeading = true;
                                strCurrStdCode = log.student_code.ToString();
                                strPrevStdCode = log.student_code.ToString();

                                m = 1;
                            }
                            else
                            {
                                if (strCurrStdCode == log.student_code.ToString())
                                {
                                    strCurrStdCode = strPrevStdCode;
                                    isNewHeading = false;

                                    m++;
                                }
                                else
                                {
                                    strCurrStdCode = log.student_code.ToString();
                                    strPrevStdCode = log.student_code.ToString();
                                    isNewHeading = true;

                                    m = 1;
                                }
                            }

                            if (isNewHeading && log.student_info != "SUM")
                            {
                                tableCount.AddCell(new PdfPCell(new Phrase("Student Code: " + log.student_info, fBold9)) { Colspan = 7 });
                            }

                            if (log.course_code_title.ToString() == "-")
                            {
                                PdfPCell cellData1 = new PdfPCell(new Phrase("OFF", fNormal8));
                                //cellData1.HorizontalAlignment = 0;
                                tableCount.AddCell(cellData1);
                            }
                            else
                            {
                                PdfPCell cellData1 = new PdfPCell(new Phrase(log.course_code_title, fNormal8));
                                //cellData1.HorizontalAlignment = 0;
                                tableCount.AddCell(cellData1);
                            }

                            PdfPCell cellData2 = new PdfPCell(new Phrase(log.present_count.ToString() + " [" + log.per_present.ToString("P2", nfi) + "]", fNormal8));
                            cellData2.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData2);

                            PdfPCell cellData3 = new PdfPCell(new Phrase(log.absent_count.ToString() + " [" + log.per_absent.ToString("P2", nfi) + "]", fNormal8));
                            cellData3.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.total_count.ToString(), fNormal8));
                            cellData4.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.late_count.ToString(), fNormal8));
                            cellData5.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.early_count.ToString(), fNormal8));
                            cellData6.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData6);

                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.off_count.ToString(), fNormal8));
                            cellData7.HorizontalAlignment = 1;
                            tableCount.AddCell(cellData7);

                            j++;
                        }

                        document.Add(tableCount);
                    }

                    if (sdata.counts.Length > 0)
                    {
                        // Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        //// ---------- Last Table ---------------------
                        //PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        //tableEnd.WidthPercentage = 100;
                        //tableEnd.HeaderRows = 0;
                        //tableEnd.SpacingBefore = 3;
                        //tableEnd.SpacingAfter = 3;

                        //PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        //tableEnd.AddCell(lt_cell_11);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        //PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Absent:", fBold9));
                        //tableEnd.AddCell(lt_cell_21);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        ////PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Off:", fBold9));
                        ////tableEnd.AddCell(lt_cell_31);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalOff, fNormal8));

                        //PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Late:", fBold9));
                        //tableEnd.AddCell(lt_cell_41);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        ////PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Leave:", fBold9));
                        ////tableEnd.AddCell(lt_cell_51);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        //PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        //tableEnd.AddCell(lt_cell_61);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        //PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        //tableEnd.AddCell(lt_cell_71);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        //document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        //Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, OFF-Off, PLO-Present Late, PLE-Present Late Early Out, POE-On Time Early Out, POM-On Time Miss Punch, PMO-Miss Punch & Left On Time, PLM-Late Miss Punch, *-Manually Updated", fNormal7);
                        //p_abrv.SpacingBefore = 1;
                        ////p_nsig.Alignment = 2;
                        //document.Add(p_abrv);

                        //Paragraph p_late = new Paragraph("NOTE: After 15 min of actual class time, LATE will be marked.", fNormal7);
                        //p_late.SpacingBefore = 2;
                        ////p_late.Alignment = 2;
                        //document.Add(p_late);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=CourseAttendanceCampus-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion

        #endregion


        #region Organization-Students-Report

        [HttpGet]
        [ActionName("GenerateStudentsReport")]
        public ActionResult GenerateStudentsReport_Get()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationStudentsView vm = new OrganizationStudentsView();
            vm.list_years = OrganizationStudentsResultSet.GetOrganizationGeneralCalendarList();
            vm.list_campuses = OrganizationStudentsResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_programs = OrganizationStudentsResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program_types = OrganizationStudentsResultSet.GetOrganizationProgramTypeList();
            vm.list_program_shifts = OrganizationStudentsResultSet.GetOrganizationProgramShiftList();
            vm.list_program_groups = OrganizationStudentsResultSet.GetOrganizationProgramGroupList();

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateStudentsReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateStudentsReport_Post()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            int found = 0, iGCYearID = 0, iCampusID = 0, iProgramID = 0, iProgramTypeID = 0, iProgramTypeNumber = 0, iProgramShiftID = 0, iProgramGroupID = 0;
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            OrganizationStudentsReport reportMaker = new OrganizationStudentsReport();

            if (!int.TryParse(Request.Form["gc_year_id"], out iGCYearID))
                return RedirectToAction("GenerateOrganizationStudentsReport");

            if (!int.TryParse(Request.Form["campus_id"], out iCampusID))
                return RedirectToAction("GenerateOrganizationStudentsReport");

            if (!int.TryParse(Request.Form["program_id"], out iProgramID))
                return RedirectToAction("GenerateOrganizationStudentsReport");

            if (!int.TryParse(Request.Form["program_type_id"], out iProgramTypeID))
                return RedirectToAction("GenerateOrganizationStudentsReport");

            if (!int.TryParse(Request.Form["program_type_number"], out iProgramTypeNumber))
                return RedirectToAction("GenerateOrganizationStudentsReport");

            if (!int.TryParse(Request.Form["program_shift_id"], out iProgramShiftID))
                return RedirectToAction("GenerateOrganizationStudentsReport");

            if (!int.TryParse(Request.Form["program_group_id"], out iProgramGroupID))
                return RedirectToAction("GenerateOrganizationStudentsReport");

            StudentsReportData toRender = reportMaker.getOrganizationStudentsReport(iGCYearID, iCampusID, iProgramID, iProgramTypeID, iProgramTypeNumber, iProgramShiftID, iProgramGroupID);
            if (toRender == null)
                return RedirectToAction("GenerateOrganizationStudentsReport");

            found = DownloadStudentsReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationStudentsView vm = new OrganizationStudentsView();
            vm.list_years = OrganizationStudentsResultSet.GetOrganizationGeneralCalendarList();
            vm.list_campuses = OrganizationStudentsResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_programs = OrganizationStudentsResultSet.GetOrganizationProgramList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_program_types = OrganizationStudentsResultSet.GetOrganizationProgramTypeList();
            vm.list_program_shifts = OrganizationStudentsResultSet.GetOrganizationProgramShiftList();
            vm.list_program_groups = OrganizationStudentsResultSet.GetOrganizationProgramGroupList();

            return View(vm);
        }

        private int DownloadStudentsReportPDF(StudentsReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);
                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.orgLogo); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.orgName + "\n" + sdata.campusName + "\n" + sdata.program_code + "-" + sdata.progTitle, fBold14));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell cellCode01 = new PdfPCell(new Phrase("Enrolled Year: " + sdata.year_code, fBold9));
                    cellCode01.Border = 0;
                    tableEInfo.AddCell(cellCode01);

                    //PdfPCell cellCode02 = new PdfPCell(new Phrase("Campus Code: " + sdata.campus_code, fBold9));
                    //cellCode02.Border = 0;
                    //tableEInfo.AddCell(cellCode02);

                    //PdfPCell cellCode03 = new PdfPCell(new Phrase("Program Code: " + sdata.program_code, fBold9));
                    //cellCode03.Border = 0;
                    //tableEInfo.AddCell(cellCode03);

                    PdfPCell cellCode04 = new PdfPCell(new Phrase("Shift Code: " + sdata.shift_code, fBold9));
                    cellCode04.Border = 0;
                    tableEInfo.AddCell(cellCode04);

                    PdfPCell cellCode05 = new PdfPCell(new Phrase("Group Code: " + sdata.group_code, fBold9));
                    cellCode05.Border = 0;
                    tableEInfo.AddCell(cellCode05);


                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 10.0f, 20.0f, 30.0f, 30.0f, 20.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    PdfPCell cell1 = new PdfPCell(new Phrase("S#", fBold12));
                    cell1.BackgroundColor = Color.LIGHT_GRAY;
                    cell1.HorizontalAlignment = 1;
                    tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("Student Code", fBold12));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Student Name", fBold12));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Father Name", fBold12));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Gender", fBold12));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    //PdfPCell cell6 = new PdfPCell(new Phrase("Remarks Out", fBold9));
                    //cell6.BackgroundColor = Color.LIGHT_GRAY;
                    //cell6.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell6);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("Final Remarks", fBold9));
                    //cell7.BackgroundColor = Color.LIGHT_GRAY;
                    //cell7.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell7);

                    //PdfPCell cell8 = new PdfPCell(new Phrase("Device In/Out", fBold9));
                    //cell8.BackgroundColor = Color.LIGHT_GRAY;
                    //cell8.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase("Device Out", fBold9));
                    //cell9.BackgroundColor = Color.LIGHT_GRAY;
                    //cell9.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell9);

                    if (sdata.logs.Length > 0)
                    {
                        int y = 1;

                        foreach (StudentsLog log in sdata.logs)
                        {
                            PdfPCell cellData1 = new PdfPCell(new Phrase(y.ToString(), fNormal8));
                            //cellData1.HorizontalAlignment = 0;
                            tableMid.AddCell(cellData1);

                            //PdfPCell cellData2 = new PdfPCell(new Phrase(log.year_code.ToString(), fNormal8));
                            //cellData2.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData2);

                            //PdfPCell cellData3 = new PdfPCell(new Phrase(log.campus_code, fNormal8));
                            //cellData3.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.student_code, fNormal8));
                            cellData4.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.student_name, fNormal8));
                            //cellData5.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.father_name, fNormal8));
                            cellData6.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData6);

                            //PdfPCell cellData7 = new PdfPCell(new Phrase(log.program_code, fNormal8));
                            ////cellData7.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData7);

                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.gender_name, fNormal8));
                            //cellData8.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData8);

                            //PdfPCell cellData9 = new PdfPCell(new Phrase(log.terminal_out_code, fNormal7));
                            ////cellData9.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData9);

                            y++;
                        }

                        document.Add(tableMid);
                    }


                    if (sdata.logs.Length > 0)
                    {
                        // Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        //// ---------- Last Table ---------------------
                        //PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        //tableEnd.WidthPercentage = 100;
                        //tableEnd.HeaderRows = 0;
                        //tableEnd.SpacingBefore = 3;
                        //tableEnd.SpacingAfter = 3;

                        //PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        //tableEnd.AddCell(lt_cell_11);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        //PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Absent:", fBold9));
                        //tableEnd.AddCell(lt_cell_21);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        ////PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Off:", fBold9));
                        ////tableEnd.AddCell(lt_cell_31);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalOff, fNormal8));

                        //PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Late:", fBold9));
                        //tableEnd.AddCell(lt_cell_41);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        ////PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Leave:", fBold9));
                        ////tableEnd.AddCell(lt_cell_51);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        //PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        //tableEnd.AddCell(lt_cell_61);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        //PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        //tableEnd.AddCell(lt_cell_71);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        //document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        //Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, OFF-Off, PLO-Present Late, PLE-Present Late Early Out, POE-On Time Early Out, POM-On Time Miss Punch, PMO-Miss Punch & Left On Time, PLM-Late Miss Punch, *-Manually Updated", fNormal7);
                        //p_abrv.SpacingBefore = 1;
                        ////p_nsig.Alignment = 2;
                        //document.Add(p_abrv);

                        //Paragraph p_late = new Paragraph("NOTE: After 15 min of actual class time, LATE will be marked.", fNormal7);
                        //p_late.SpacingBefore = 2;
                        ////p_late.Alignment = 2;
                        //document.Add(p_late);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=StudentsReport-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion

        #region Organization-Employees-Report

        [HttpGet]
        [ActionName("GenerateEmployeesReport")]
        public ActionResult GenerateEmployeesReport_Get()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationEmployeesView vm = new OrganizationEmployeesView();
            vm.list_campuses = OrganizationEmployeesResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_departments = OrganizationEmployeesResultSet.GetOrganizationDepartmentList();
            vm.list_designations = OrganizationEmployeesResultSet.GetOrganizationDesignationList();
            vm.list_locations = OrganizationEmployeesResultSet.GetOrganizationLocationList();

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateEmployeesReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateEmployeesReport_Post()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            int found = 0, iCampusID = -1, iDeptID = -1, iDesgID = -1, iLoctID = -1;
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            OrganizationEmployeesReport reportMaker = new OrganizationEmployeesReport();

            //if (!int.TryParse(Request.Form["campus_id"], out iCampusID))
            //    return RedirectToAction("GenerateOrganizationEmployeesReport");

            if (!int.TryParse(Request.Form["dept_id"], out iDeptID))
                return RedirectToAction("GenerateOrganizationEmployeesReport");

            if (!int.TryParse(Request.Form["desg_id"], out iDesgID))
                return RedirectToAction("GenerateOrganizationEmployeesReport");

            if (!int.TryParse(Request.Form["loct_id"], out iLoctID))
                return RedirectToAction("GenerateOrganizationEmployeesReport");

            EmployeesReportData toRender = reportMaker.getOrganizationEmployeesReport(iCampusID, iDeptID, iDesgID, iLoctID);
            if (toRender == null)
                return RedirectToAction("GenerateOrganizationEmployeesReport");

            found = DownloadEmployeesReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationEmployeesView vm = new OrganizationEmployeesView();
            vm.list_campuses = OrganizationEmployeesResultSet.GetOrganizationCampusList(bGVIsSuperHRRole, iGVCampusID);
            vm.list_departments = OrganizationEmployeesResultSet.GetOrganizationDepartmentList();
            vm.list_designations = OrganizationEmployeesResultSet.GetOrganizationDesignationList();
            vm.list_locations = OrganizationEmployeesResultSet.GetOrganizationLocationList();

            return View(vm);
        }

        private int DownloadEmployeesReportPDF(EmployeesReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);
                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.orgLogo); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.orgName + "\n" + sdata.campusName, fBold14));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellCode01 = new PdfPCell(new Phrase("Campus Code: " + sdata.campus_code, fBold9));
                    //cellCode01.Border = 0;
                    //tableEInfo.AddCell(cellCode01);

                    ////PdfPCell cellCode02 = new PdfPCell(new Phrase("Campus Code: " + sdata.campus_code, fBold9));
                    ////cellCode02.Border = 0;
                    ////tableEInfo.AddCell(cellCode02);

                    ////PdfPCell cellCode03 = new PdfPCell(new Phrase("Program Code: " + sdata.program_code, fBold9));
                    ////cellCode03.Border = 0;
                    ////tableEInfo.AddCell(cellCode03);

                    //PdfPCell cellCode04 = new PdfPCell(new Phrase("Department: " + sdata.dept_name, fBold9));
                    //cellCode04.Border = 0;
                    //tableEInfo.AddCell(cellCode04);

                    //PdfPCell cellCode05 = new PdfPCell(new Phrase("Designation: " + sdata.desg_name, fBold9));
                    //cellCode05.Border = 0;
                    //tableEInfo.AddCell(cellCode05);


                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 8.0f, 21.0f, 21.0f, 21.0f, 21.0f, 8.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    //PdfPCell cell1 = new PdfPCell(new Phrase("S#", fBold10));
                    //cell1.BackgroundColor = Color.LIGHT_GRAY;
                    //cell1.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("ECode", fBold10));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Name", fBold10));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Department", fBold10));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Designation", fBold10));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Location", fBold10));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Gender", fBold10));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("Final Remarks", fBold9));
                    //cell7.BackgroundColor = Color.LIGHT_GRAY;
                    //cell7.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell7);

                    //PdfPCell cell8 = new PdfPCell(new Phrase("Device In/Out", fBold9));
                    //cell8.BackgroundColor = Color.LIGHT_GRAY;
                    //cell8.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase("Device Out", fBold9));
                    //cell9.BackgroundColor = Color.LIGHT_GRAY;
                    //cell9.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell9);

                    if (sdata.logs.Length > 0)
                    {
                        int y = 1;

                        foreach (EmployeesLog log in sdata.logs)
                        {
                            ////PdfPCell cellData1 = new PdfPCell(new Phrase(y.ToString(), fNormal8));
                            //////cellData1.HorizontalAlignment = 0;
                            ////tableMid.AddCell(cellData1);

                            //PdfPCell cellData2 = new PdfPCell(new Phrase(log.year_code.ToString(), fNormal8));
                            //cellData2.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData2);

                            //PdfPCell cellData3 = new PdfPCell(new Phrase(log.campus_code, fNormal8));
                            //cellData3.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.employee_code, fNormal8));
                            cellData4.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.employee_name, fNormal8));
                            //cellData5.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.dept_name, fNormal8));
                            //cellData6.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData6);

                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.desg_name, fNormal8));
                            //cellData7.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData7);

                            PdfPCell cellData71 = new PdfPCell(new Phrase(log.loct_name, fNormal8));
                            //cellData71.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData71);

                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.gender_name, fNormal8));
                            cellData8.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData8);

                            //PdfPCell cellData9 = new PdfPCell(new Phrase(log.terminal_out_code, fNormal7));
                            ////cellData9.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData9);

                            y++;
                        }

                        document.Add(tableMid);
                    }


                    if (sdata.logs.Length > 0)
                    {
                        // Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        //// ---------- Last Table ---------------------
                        //PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        //tableEnd.WidthPercentage = 100;
                        //tableEnd.HeaderRows = 0;
                        //tableEnd.SpacingBefore = 3;
                        //tableEnd.SpacingAfter = 3;

                        //PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        //tableEnd.AddCell(lt_cell_11);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        //PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Absent:", fBold9));
                        //tableEnd.AddCell(lt_cell_21);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        ////PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Off:", fBold9));
                        ////tableEnd.AddCell(lt_cell_31);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalOff, fNormal8));

                        //PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Late:", fBold9));
                        //tableEnd.AddCell(lt_cell_41);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        ////PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Leave:", fBold9));
                        ////tableEnd.AddCell(lt_cell_51);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        //PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        //tableEnd.AddCell(lt_cell_61);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        //PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        //tableEnd.AddCell(lt_cell_71);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        //document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        //Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, OFF-Off, PLO-Present Late, PLE-Present Late Early Out, POE-On Time Early Out, POM-On Time Miss Punch, PMO-Miss Punch & Left On Time, PLM-Late Miss Punch, *-Manually Updated", fNormal7);
                        //p_abrv.SpacingBefore = 1;
                        ////p_nsig.Alignment = 2;
                        //document.Add(p_abrv);

                        //Paragraph p_late = new Paragraph("NOTE: After 15 min of actual class time, LATE will be marked.", fNormal7);
                        //p_late.SpacingBefore = 2;
                        ////p_late.Alignment = 2;
                        //document.Add(p_late);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=EmployeesReport-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion

        #region Organization-Employees-By-Type-Report

        [HttpGet]
        [ActionName("GenerateEmployeesByTypeReport")]
        public ActionResult GenerateEmployeesByTypeReport_Get()
        {
            if (ViewModel.GlobalVariables.GV_AccessDeniedToOrganization)
            {
                return RedirectToAction("AccessDenied", "Unauthorized");
            }

            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            OrganizationEmployeesByTypeView vm = new OrganizationEmployeesByTypeView();
            vm.list_emp_types = OrganizationEmployeesByTypeResultSet.GetOrganizationEmployeesByTypeList();

            return View(vm);
        }


        [HttpPost]
        [ActionName("GenerateEmployeesByTypeReport")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateEmployeesByTypeReport_Post()
        {
            bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
            int iGVCampusID = 0; int.TryParse(ViewModel.GlobalVariables.GV_EmployeeCampusID, out iGVCampusID);

            int found = 0, iEmpTypeID = -1;
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;
            ViewData["PDFNoDataFound"] = "";

            OrganizationEmployeesByTypeReport reportMaker = new OrganizationEmployeesByTypeReport();

            if (!int.TryParse(Request.Form["emp_type_id"], out iEmpTypeID))
                return RedirectToAction("GenerateOrganizationEmployeesByTypeReport");

            EmployeesReportData toRender = reportMaker.getOrganizationEmployeesByTypeReport(iEmpTypeID);
            if (toRender == null)
                return RedirectToAction("GenerateOrganizationEmployeesByTypeReport");

            found = DownloadEmployeesByTypeReportPDF(toRender);
            if (found == 1)
            {
                ViewData["PDFNoDataFound"] = "";
            }
            else
            {
                ViewData["PDFNoDataFound"] = "No Data Found";
            }

            OrganizationEmployeesByTypeView vm = new OrganizationEmployeesByTypeView();
            vm.list_emp_types = OrganizationEmployeesByTypeResultSet.GetOrganizationEmployeesByTypeList();

            return View(vm);
        }

        private int DownloadEmployeesByTypeReportPDF(EmployeesReportData sdata)
        {
            int reponse = 0;

            try
            {

                ////here MemoryStream is used to Export PDF file instead of saving the PDF file in a specific folder
                using (MemoryStream ms = new MemoryStream())
                {
                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesNormal = new Font(bfTimesNormal, 11, Font.NORMAL, Color.BLACK);

                    //// set a FONT properties as required and here for BLACK color
                    //BaseFont bfTimesBold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                    //Font timesBold = new Font(bfTimesBold, 12, Font.BOLD, Color.BLACK);

                    Font fNormal7 = FontFactory.GetFont("HELVETICA", 7, Font.NORMAL, Color.BLACK);

                    Font fNormal8 = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, Color.BLACK);
                    Font fBold8 = FontFactory.GetFont("HELVETICA", 8, Font.BOLD, Color.BLACK);

                    Font fNormal9 = FontFactory.GetFont("HELVETICA", 9, Font.NORMAL, Color.BLACK);
                    Font fBold9 = FontFactory.GetFont("HELVETICA", 9, Font.BOLD, Color.BLACK);

                    Font fNormal10 = FontFactory.GetFont("HELVETICA", 10, Font.NORMAL, Color.BLACK);
                    Font fBold10 = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, Color.BLACK);

                    Font fBold12 = FontFactory.GetFont("HELVETICA", 12, Font.BOLD, Color.BLACK);
                    Font fBold14 = FontFactory.GetFont("HELVETICA", 14, Font.BOLD, Color.BLACK);

                    Font fBold14Red = FontFactory.GetFont("HELVETICA", 14, Font.BOLD | Font.UNDERLINE, Color.RED);
                    Font fBold16 = FontFactory.GetFont("HELVETICA", 16, Font.BOLD | Font.UNDERLINE, Color.BLACK);

                    //// Initialize Document Page for PDF
                    Document document = new Document(PageSize.A4, 10f, 10f, 10f, 20f);

                    //// To Export PDF file automatically then write data to memory stream
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    writer.RunDirection = PdfLayoutHelper.RunDirection;
                    writer.PageEvent = new PageHeaderFooter();

                    //// To save file in a specific folder of project, also remove MemoryStream code above and Response code lines below
                    //string path = Server.MapPath("~/Content");
                    //PdfWriter.GetInstance(document, new FileStream(path + "/Report-" + sdata.employeeCode + "-" + sdata.month + "-" + sdata.year + ".pdf", FileMode.CreateNew));

                    document.Open();

                    // ----------- Line Separator -------------------
                    iTextSharp.text.pdf.draw.LineSeparator lineSeparator = new iTextSharp.text.pdf.draw.LineSeparator(0.8F, 99.0F, Color.BLACK, Element.ALIGN_LEFT, 1);

                    // ---------- Header Table ---------------------
                    string imageURL = Server.MapPath("~/" + sdata.orgLogo); //Server.MapPath("~/images/hbl-logo.png");
                    //string imageURL = Request.PhysicalApplicationPath + "/Content/hbl-logo.png";

                    Image logo = Image.GetInstance(imageURL);
                    //logo.Width = 140.0f;
                    //logo.Alignment = Element.ALIGN_LEFT;
                    //logo.ScaleToFit(140f, 20f);
                    //logo.ScaleAbsolute(140f, 20f);
                    //logo.SpacingBefore = 5f;
                    //logo.SpacingAfter = 5f;

                    PdfPTable tableHeader = new PdfPTable(new[] { 100.0f, 860.0f, 140.0f });//total 595 - 10 x 2 due to Left and Right side margin
                    tableHeader.WidthPercentage = 100;
                    tableHeader.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableHeader.SpacingAfter = 3;
                    tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                    tableHeader.AddCell(logo);

                    PdfPCell cellTitle = new PdfPCell(new Phrase(sdata.orgName + "\n" + sdata.campusName, fBold14));
                    cellTitle.HorizontalAlignment = 1;
                    cellTitle.Border = 0;
                    tableHeader.AddCell(cellTitle);

                    PdfPCell cellDateTime = new PdfPCell(new Phrase("Date:\n" + DateTime.Now.ToString("dd-MMM-yyyy") + "\n\nTime:\n" + DateTime.Now.ToString("hh:mm tt"), fNormal10));
                    //cellDateTime.HorizontalAlignment = 2;
                    cellDateTime.PaddingTop = 2.0f;
                    cellDateTime.Border = 0;
                    tableHeader.AddCell(cellDateTime);

                    //tableHeader.AddCell("Date: " + DateTime.Now.ToShortDateString() + "\nTime: " +DateTime.Now.ToString("hh:mm tt"));

                    document.Add(tableHeader);

                    //separator
                    document.Add(lineSeparator);

                    // ---------- Top Data -------------------------
                    PdfPTable tableEmployee = new PdfPTable(2);//total 595 - 10 x 2 due to Left and Right side margin
                    tableEmployee.WidthPercentage = 100;
                    tableEmployee.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    //tableEmployee.SpacingAfter = 3;
                    tableEmployee.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPTable tableEInfo = new PdfPTable(1);
                    tableEInfo.WidthPercentage = 100;
                    tableEInfo.HeaderRows = 0;
                    //tableHeader.SpacingBefore = 50;
                    tableEInfo.SpacingAfter = 3;
                    tableEInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                    //PdfPCell cellCode01 = new PdfPCell(new Phrase("Campus Code: " + sdata.campus_code, fBold9));
                    //cellCode01.Border = 0;
                    //tableEInfo.AddCell(cellCode01);

                    ////PdfPCell cellCode02 = new PdfPCell(new Phrase("Campus Code: " + sdata.campus_code, fBold9));
                    ////cellCode02.Border = 0;
                    ////tableEInfo.AddCell(cellCode02);

                    ////PdfPCell cellCode03 = new PdfPCell(new Phrase("Program Code: " + sdata.program_code, fBold9));
                    ////cellCode03.Border = 0;
                    ////tableEInfo.AddCell(cellCode03);

                    //PdfPCell cellCode04 = new PdfPCell(new Phrase("Department: " + sdata.dept_name, fBold9));
                    //cellCode04.Border = 0;
                    //tableEInfo.AddCell(cellCode04);

                    //PdfPCell cellCode05 = new PdfPCell(new Phrase("Designation: " + sdata.desg_name, fBold9));
                    //cellCode05.Border = 0;
                    //tableEInfo.AddCell(cellCode05);


                    //Paragraph p_title = new Paragraph("Monthly Time Sheet", fBold16);
                    //p_title.SpacingBefore = 50f;
                    //p_title.SpacingAfter = 10f;
                    ////document.Add(p_title);

                    PdfPCell cellETitle = new PdfPCell(new Phrase("", fBold16));
                    cellETitle.Border = 0;
                    cellETitle.HorizontalAlignment = 2;

                    tableEmployee.AddCell(tableEInfo);
                    tableEmployee.AddCell(cellETitle);

                    document.Add(tableEmployee);

                    // ---------- Middle Table ---------------------
                    //set table with 595 pixels width - subtract 10x2 from either sides border
                    PdfPTable tableMid = new PdfPTable(new[] { 8.0f, 21.0f, 21.0f, 21.0f, 21.0f, 8.0f });

                    tableMid.WidthPercentage = 100;
                    tableMid.HeaderRows = 1;
                    tableMid.SpacingBefore = 3;
                    tableMid.SpacingAfter = 1;

                    //PdfPCell cell1 = new PdfPCell(new Phrase("S#", fBold10));
                    //cell1.BackgroundColor = Color.LIGHT_GRAY;
                    //cell1.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(new Phrase("ECode", fBold10));
                    cell2.BackgroundColor = Color.LIGHT_GRAY;
                    cell2.HorizontalAlignment = 1;
                    tableMid.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(new Phrase("Name", fBold10));
                    cell3.BackgroundColor = Color.LIGHT_GRAY;
                    cell3.HorizontalAlignment = 1;
                    tableMid.AddCell(cell3);

                    PdfPCell cell4 = new PdfPCell(new Phrase("Department", fBold10));
                    cell4.BackgroundColor = Color.LIGHT_GRAY;
                    cell4.HorizontalAlignment = 1;
                    tableMid.AddCell(cell4);

                    PdfPCell cell5 = new PdfPCell(new Phrase("Designation", fBold10));
                    cell5.BackgroundColor = Color.LIGHT_GRAY;
                    cell5.HorizontalAlignment = 1;
                    tableMid.AddCell(cell5);

                    PdfPCell cell6 = new PdfPCell(new Phrase("Location", fBold10));
                    cell6.BackgroundColor = Color.LIGHT_GRAY;
                    cell6.HorizontalAlignment = 1;
                    tableMid.AddCell(cell6);

                    PdfPCell cell7 = new PdfPCell(new Phrase("Gender", fBold10));
                    cell7.BackgroundColor = Color.LIGHT_GRAY;
                    cell7.HorizontalAlignment = 1;
                    tableMid.AddCell(cell7);

                    //PdfPCell cell7 = new PdfPCell(new Phrase("Final Remarks", fBold9));
                    //cell7.BackgroundColor = Color.LIGHT_GRAY;
                    //cell7.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell7);

                    //PdfPCell cell8 = new PdfPCell(new Phrase("Device In/Out", fBold9));
                    //cell8.BackgroundColor = Color.LIGHT_GRAY;
                    //cell8.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell8);

                    //PdfPCell cell9 = new PdfPCell(new Phrase("Device Out", fBold9));
                    //cell9.BackgroundColor = Color.LIGHT_GRAY;
                    //cell9.HorizontalAlignment = 1;
                    //tableMid.AddCell(cell9);

                    if (sdata.logs.Length > 0)
                    {
                        int y = 1;

                        foreach (EmployeesLog log in sdata.logs)
                        {
                            ////PdfPCell cellData1 = new PdfPCell(new Phrase(y.ToString(), fNormal8));
                            //////cellData1.HorizontalAlignment = 0;
                            ////tableMid.AddCell(cellData1);

                            //PdfPCell cellData2 = new PdfPCell(new Phrase(log.year_code.ToString(), fNormal8));
                            //cellData2.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData2);

                            //PdfPCell cellData3 = new PdfPCell(new Phrase(log.campus_code, fNormal8));
                            //cellData3.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData3);

                            PdfPCell cellData4 = new PdfPCell(new Phrase(log.employee_code, fNormal8));
                            cellData4.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData4);

                            PdfPCell cellData5 = new PdfPCell(new Phrase(log.employee_name, fNormal8));
                            //cellData5.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData5);

                            PdfPCell cellData6 = new PdfPCell(new Phrase(log.dept_name, fNormal8));
                            //cellData6.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData6);

                            PdfPCell cellData7 = new PdfPCell(new Phrase(log.desg_name, fNormal8));
                            //cellData7.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData7);

                            PdfPCell cellData71 = new PdfPCell(new Phrase(log.loct_name, fNormal8));
                            //cellData71.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData71);

                            PdfPCell cellData8 = new PdfPCell(new Phrase(log.gender_name, fNormal8));
                            cellData8.HorizontalAlignment = 1;
                            tableMid.AddCell(cellData8);

                            //PdfPCell cellData9 = new PdfPCell(new Phrase(log.terminal_out_code, fNormal7));
                            ////cellData9.HorizontalAlignment = 1;
                            //tableMid.AddCell(cellData9);

                            y++;
                        }

                        document.Add(tableMid);
                    }


                    if (sdata.logs.Length > 0)
                    {
                        // Summary heading
                        //Paragraph p_summary = new Paragraph("Summary", fBold10);
                        //document.Add(p_summary);

                        //// ---------- Last Table ---------------------
                        //PdfPTable tableEnd = new PdfPTable(new[] { 75.0f, 25.0f });
                        //tableEnd.WidthPercentage = 100;
                        //tableEnd.HeaderRows = 0;
                        //tableEnd.SpacingBefore = 3;
                        //tableEnd.SpacingAfter = 3;

                        //PdfPCell lt_cell_11 = new PdfPCell(new Phrase("Present:", fBold9));
                        //tableEnd.AddCell(lt_cell_11);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalPresent, fNormal8));

                        //PdfPCell lt_cell_21 = new PdfPCell(new Phrase("Absent:", fBold9));
                        //tableEnd.AddCell(lt_cell_21);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalAbsent, fNormal8));

                        ////PdfPCell lt_cell_31 = new PdfPCell(new Phrase("Off:", fBold9));
                        ////tableEnd.AddCell(lt_cell_31);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalOff, fNormal8));

                        //PdfPCell lt_cell_41 = new PdfPCell(new Phrase("Late:", fBold9));
                        //tableEnd.AddCell(lt_cell_41);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalLate, fNormal8));

                        ////PdfPCell lt_cell_51 = new PdfPCell(new Phrase("Leave:", fBold9));
                        ////tableEnd.AddCell(lt_cell_51);
                        ////tableEnd.AddCell(new Phrase(" " + sdata.totalLeave, fNormal8));

                        //PdfPCell lt_cell_61 = new PdfPCell(new Phrase("Early Out:", fBold9));
                        //tableEnd.AddCell(lt_cell_61);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalEarlyOut, fNormal8));

                        //PdfPCell lt_cell_71 = new PdfPCell(new Phrase("Total Days:", fBold9));
                        //tableEnd.AddCell(lt_cell_71);
                        //tableEnd.AddCell(new Phrase(" " + sdata.totalDays, fNormal8));

                        //document.Add(tableEnd);

                        // legends message
                        // AB-Absent, PLO-Present Late, PO-Present On Time, PLE-Present Late Early Out, POE-Present On Time Early Out, OFF-Off, *-Manually Updated
                        //Paragraph p_abrv = new Paragraph("Legends: PO-Present On Time, AB-Absent, OFF-Off, PLO-Present Late, PLE-Present Late Early Out, POE-On Time Early Out, POM-On Time Miss Punch, PMO-Miss Punch & Left On Time, PLM-Late Miss Punch, *-Manually Updated", fNormal7);
                        //p_abrv.SpacingBefore = 1;
                        ////p_nsig.Alignment = 2;
                        //document.Add(p_abrv);

                        //Paragraph p_late = new Paragraph("NOTE: After 15 min of actual class time, LATE will be marked.", fNormal7);
                        //p_late.SpacingBefore = 2;
                        ////p_late.Alignment = 2;
                        //document.Add(p_late);

                        Paragraph p_nsig = new Paragraph("This is a system generated report and does not require any signature.", fNormal7);
                        p_nsig.SpacingBefore = 1;
                        //p_nsig.SpacingAfter = 3;
                        document.Add(p_nsig);

                        // ------------- close PDF Document and download it automatically



                        document.Close();
                        writer.Close();
                        Response.ContentType = "pdf/application";
                        Response.AddHeader("content-disposition", "attachment;filename=EmployeesReport-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".pdf");
                        Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                        Response.Flush();
                        Response.End();

                        reponse = 1;
                    }
                    else
                    {
                        Paragraph p_no_data = new Paragraph("No Data Found.", fBold14Red);
                        p_no_data.SpacingBefore = 20;
                        p_no_data.SpacingAfter = 20;
                        document.Add(p_no_data);

                        reponse = 0;
                    }
                }
            }
            catch (Exception)
            {
                //handle exception
            }

            return reponse;
        }

        #endregion


        #region Organization-Student-Courses-Attendance-Rare-Cases-ONLY

        public ActionResult ManageCourseAttendanceStudent()
        {
            if (Request.QueryString["result"] != null && Request.QueryString["result"].ToString() != "")
                ViewBag.UpMessage = Request.QueryString["result"].ToString();
            else
                ViewBag.UpMessage = "";

            return View();
        }

        [HttpPost]
        public ActionResult ManageCourseAttendanceStudent(DTParameters param)
        {
            string strStudentID = "0", strStartDate = "", strEndDate = "";

            try
            {
                strStudentID = Request.Form["student_id"] ?? "0";
                strStartDate = Request.Form["start_date"] ?? DateTime.Now.ToString("dd-MM-yyyy");
                strEndDate = Request.Form["end_date"] ?? DateTime.Now.ToString("dd-MM-yyyy");

                DateTime dtStDate = DateTime.ParseExact(strStartDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime dtEnDate = DateTime.ParseExact(strEndDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);


                var dtsource = new List<ViewModels.OrganizationCourseAttendanceStudentView>();

                // get all employee view models
                dtsource = OrganizationCourseAttendanceResultSet.getCourseAttendanceStudentByDateRange(int.Parse(strStudentID), dtStDate, dtEnDate);
                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }

                List<ViewModels.OrganizationCourseAttendanceStudentView> data = OrganizationCourseAttendanceResultSet.GetResult("", "", 0, 100, dtsource);
                data = data.OrderByDescending(o => o.Id).ToList();

                int count = OrganizationCourseAttendanceResultSet.Count("", dtsource);

                DTResult<ViewModels.OrganizationCourseAttendanceStudentView> result = new DTResult<ViewModels.OrganizationCourseAttendanceStudentView>
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

            //int id = OrganizationCourseAttendanceResultSet.CreateOrganizationCourseAttendanceStudent(toCreate);

            //if (id == -1)
            //{
            //    return RedirectToAction("ManageCourseAttendanceStudent", new { Message = "already" });

            //}

            //var json = JsonConvert.SerializeObject(toCreate);
            //AuditTrail.insert(json, "ManageCourseAttendanceStudent", User.Identity.Name);

            //return RedirectToAction("ManageCourseAttendanceStudent", new { Message = "success" });

            ////return View();
        }

        public class CourseAttandenceTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        [HttpPost]
        public JsonResult ManageCourseAttendanceStudentDataHandler(CourseAttandenceTable param)
        {
            string strStudentID = "0", strStartDate = "", strEndDate = "";

            try
            {
                strStudentID = param.employee_id ?? "0";// Request.Form["student_id"] ?? "0";
                strStartDate = param.from_date ?? DateTime.Now.ToString("dd-MM-yyyy"); // Request.Form["start_date"] ?? DateTime.Now.ToString("dd-MM-yyyy");
                strEndDate = param.to_date ?? DateTime.Now.ToString("dd-MM-yyyy"); //Request.Form["end_date"] ?? DateTime.Now.ToString("dd-MM-yyyy");

                DateTime dtStDate = DateTime.ParseExact(strStartDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime dtEnDate = DateTime.ParseExact(strEndDate.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);


                var dtsource = new List<ViewModels.OrganizationCourseAttendanceStudentView>();

                // get all employee view models
                dtsource = OrganizationCourseAttendanceResultSet.getCourseAttendanceStudentByDateRange(int.Parse(strStudentID), dtStDate, dtEnDate);
                if (dtsource == null)
                {
                    return Json("No Data to Show");
                }

                List<ViewModels.OrganizationCourseAttendanceStudentView> data = OrganizationCourseAttendanceResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);
                data = data.OrderByDescending(o => o.Id).ToList();

                int count = OrganizationCourseAttendanceResultSet.Count(param.Search.Value, dtsource);

                DTResult<ViewModels.OrganizationCourseAttendanceStudentView> result = new DTResult<ViewModels.OrganizationCourseAttendanceStudentView>
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
        public ActionResult UpdateCourseAttendanceStudent(OrganizationCourseAttendanceStudentForm toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            int response = OrganizationCourseAttendanceResultSet.UpdateOrganizationCourseAttendanceStudent(toUpdate, User.Identity.Name);

            if (response == 0)
            {
                return Json(new { status = "already" });
            }

            AuditTrail.update(json, "ManageCourseAttendanceStudent", User.Identity.Name);

            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveCourseAttendanceStudent(OrganizationCourseAttendanceStudentForm toRemove)
        {
            var entity = OrganizationCourseAttendanceResultSet.RemoveOrganizationCourseAttendanceStudent(toRemove);

            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "ManageCourseAttendanceStudent", User.Identity.Name);

            return Json(new { status = "success" });
        }




        #region File-Upload-Course-Attendance-Upload-Download-Sheets

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadCourseAttendanceCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<ManageCourseAttendanceStudentImportExport.ManageCourseAttendanceCSV>
                    (
                        ManageCourseAttendanceStudentImportExport.getManageCourseAttendanceStudentsCSVDownload(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-CourseAttendance.csv"
                    );


            return toReturn;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCourseAttendanceCSVFile(HttpPostedFileBase file)
        {
            string result = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = Path.GetFileName(file.FileName);
                string FileExtension = Path.GetExtension(file.FileName).ToLower();

                if (FileExtension != ".csv")
                {
                    return RedirectToAction("ManageCourseAttendanceStudent", "OrganizationsManagement", new { result = "Invalid-File-Format" });
                }
                else
                {
                    //try
                    //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_course_attendace.csv");
                    file.SaveAs(path);

                    int counter = 0;
                    List<string> content = new List<string>();
                    string strReadLine = ""; bool invalidDate = false, invalidPastDate = false, invalidSCode = false, invalidCCode = false, invalidRem = false, invalidCols = false;

                    //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                    bool isDataRowFound = false; int a = 0;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            a++;

                            if (a == 2)
                            {
                                isDataRowFound = true;
                                break;
                            }
                        }
                    }

                    if (!isDataRowFound)
                    {
                        return RedirectToAction("ManageCourseAttendanceStudent", "OrganizationsManagement", new { result = "No Data Found in the Sheet" });
                    }
                    /////////////////////////////////////////////////////////////////////////

                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            strReadLine = sr.ReadLine();
                            strReadLine = strReadLine.TrimEnd(',');
                            strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                            strReadLine = strReadLine.Replace("\"", "");
                            strReadLine = strReadLine.TrimEnd(',');

                            string new_code = "";
                            string[] ecode_dt = strReadLine.Split(',');
                            if (ecode_dt.Length == 4)
                            {
                                counter++;

                                if (ecode_dt[0].ToLower().Contains("schedule_date") || ecode_dt[1].ToLower().Contains("student_code") || ecode_dt[2].ToLower().Contains("course_code"))
                                {
                                    continue;
                                }

                                //validate gc-year
                                if (!ValidateCrsAttDate(ecode_dt[0]))
                                {
                                    invalidDate = true;
                                    result = "Invalid Date Found at Row-" + counter;
                                    break;
                                }

                                if (!ValidateCrsAttPastDate(ecode_dt[0]))
                                {
                                    invalidPastDate = true;
                                    result = "Invalid Past Date at Row-" + counter;
                                    break;
                                }

                                //validate student-code in that campus
                                if (!ValidateCrsAttStudentCode(ecode_dt[1]))
                                {
                                    invalidSCode = true;
                                    result = "Invalid Student Code Found at Row-" + counter;
                                    break;
                                }

                                //validate course-code
                                if (!ValidateCrsAttPCourseCode(ecode_dt[2]))
                                {
                                    invalidCCode = true;
                                    result = "Invalid Course Code Found at Row-" + counter;
                                    break;
                                }


                                //validate remarks
                                if (!ValidateCrsAttRemarks(ecode_dt[3]))
                                {
                                    invalidRem = true;
                                    result = "Invalid Remarks Found at Row-" + counter;
                                    break;
                                }


                            }
                            else
                            {
                                invalidCols = true;
                                result = "Invalid Col(s) Found";
                                break;
                            }

                            //iterate to replace EmployeeCode only having 0 as prefix
                            //for (int i = 0; i < ecode_dt.Length; i++)
                            //{
                            //    if (i == 0)
                            //    {
                            //        strReadLine = new_code + ",";
                            //    }
                            //    else
                            //    {
                            //        strReadLine += ecode_dt[i] + ",";
                            //    }
                            //}

                            //strReadLine = strReadLine.TrimEnd(',');
                            content.Add(strReadLine);

                            //restrict to upload if 1000+ rows are found
                            /*if (counter > 1000)
                            {
                                invalidRowsCount = true;
                                result = "Max 1000 records are allowed be uploaded";
                                break;
                            }*/
                        }
                    }

                    if (invalidDate || invalidPastDate || invalidSCode || invalidCCode || invalidRem || invalidCols)
                    {
                        return RedirectToAction("ManageCourseAttendanceStudent", "OrganizationsManagement", new { result = result });
                    }
                    else
                    {
                        result = ManageCourseAttendanceStudentImportExport.setCourseAttendanceEnrollment(content, User.Identity.Name);
                    }

                }

                if (result == "failed")
                {
                    return RedirectToAction("ManageCourseAttendanceStudent", "OrganizationsManagement", new { result = "Failed to Update due to Invalid info" });
                }

                return RedirectToAction("ManageCourseAttendanceStudent", "OrganizationsManagement", new { result = "Successful" });

                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
            }

            return RedirectToAction("ManageCourseAttendanceStudent", "OrganizationsManagement", new { result = "Select File first" });
        }

        private bool ValidateCrsAttDate(string strDate)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (!DateTime.TryParse(strProperDate, out dtTest))
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateCrsAttPastDate(string strDate)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now.AddMonths(-1);

            DateTime dtProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (dtProperDate.Date < dtTest.Date)
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateCrsAttStudentCode(string strStudentCode)
        {
            bool isValid = false;

            isValid = ManageCourseAttendanceStudentImportExport.validateCrsAttStudentCode(strStudentCode);

            return isValid;
        }

        private bool ValidateCrsAttPCourseCode(string strPCourseCode)
        {
            bool isValid = false;

            isValid = ManageCourseAttendanceStudentImportExport.validateCrsAttProgramCourseCode(strPCourseCode);

            return isValid;
        }

        private bool ValidateCrsAttRemarks(string strRemarks)
        {
            bool isValid = true;
            string[] strRemarksList = new string[8] { "PO", "PLO", "PLE", "PLM", "POE", "POM", "AB", "OFF" };

            if (!strRemarksList.Contains(strRemarks))
            {
                isValid = false;
            }

            return isValid;
        }



        #endregion

        #endregion


        #region Searchable-Students-DropDown

        //GetUsers
        [HttpPost]
        public JsonResult Students_Dropdown()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = OrganizationCourseAttendanceResultSet.getAllStudentsMatching(q);

            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];

            if (employees.Length > 0)
            {
                for (int i = 0; i < employees.Length; i++)
                {
                    toSend[i] = new ChosenAutoCompleteResults();

                    toSend[i].id = employees[i].id.ToString();
                    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name;

                    //if (employees[i].access_group_id == 3)
                    //{
                    //    toSend[i].id = employees[i].id.ToString();
                    //    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name + "*";
                    //}
                    //else
                    //{
                    //    toSend[i].id = employees[i].id.ToString();
                    //    toSend[i].text = employees[i].employee_code + " " + employees[i].first_name + " " + employees[i].last_name;
                    //}

                }


            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        #endregion

        public class PageHeaderFooter : PdfPageEventHelper
        {
            private readonly Font _pageNumberFont = new Font(Font.HELVETICA, 8f, Font.NORMAL, Color.BLACK);

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                AddPageNumber(writer, document);

                //////////////////////////////////////////

                ////https://stackoverflow.com/questions/2321526/pdfptable-as-a-header-in-itextsharp



                ////PdfPTable table = new PdfPTable(1);
                //////table.WidthPercentage = 100; //PdfPTable.writeselectedrows below didn't like this
                ////table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin; //this centers [table]
                ////PdfPTable table2 = new PdfPTable(2);

                //////logo
                ////PdfPCell cell2 = new PdfPCell(); //Image.GetInstance(@"C:\path\to\file.gif")
                ////cell2.Colspan = 2;
                ////table2.AddCell(cell2);

                //////title
                ////cell2 = new PdfPCell(new Phrase("\nTITLE", new Font(Font.HELVETICA, 16, Font.BOLD | Font.UNDERLINE)));
                ////cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                ////cell2.Colspan = 2;
                ////table2.AddCell(cell2);

                ////PdfPCell cell = new PdfPCell(table2);
                ////table.AddCell(cell);

                ////table.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 36, writer.DirectContent);


            }

            private void AddPageNumber(PdfWriter writer, Document document)
            {
                var text = writer.PageNumber.ToString();

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                //float len = bf.GetWidthPoint(text, 10);

                //iTextSharp.text.Rectangle pageSize = document.PageSize;

                var numberTable = new PdfPTable(1);
                numberTable.DefaultCell.Border = Rectangle.NO_BORDER;
                var numberCell = new PdfPCell(new Phrase(text, _pageNumberFont)) { HorizontalAlignment = Element.ALIGN_RIGHT };
                numberCell.Border = 0;

                numberTable.AddCell(numberCell);

                numberTable.TotalWidth = 20;
                numberTable.WriteSelectedRows(0, -1, document.Right - 20, document.Bottom + 5, writer.DirectContent);
            }
        }
    }
}
