using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TimeTune;
using ViewModels;
using MvcApplication1;
using MvcApplication1.Areas.HR.DataTableResultSets;
using System.Globalization;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class EmployeeManagementController : Controller
    {

        #region Function

        public ActionResult Function()
        {

            return View();
        }

        [HttpPost]
        public JsonResult FunctionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Function>();
                // get all employee view models
                dtsource = TimeTune.EmployeeFunction.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Function> data = FunctionResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = FunctionResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Function> result = new DTResult<ViewModels.Function>
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
        public ActionResult AddFunction(ViewModels.Function fromForm)
        {
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeFunction.add(fromForm);
            AuditTrail.insert(json, "Function", id);
            return RedirectToAction("Function");
        }

        [HttpPost]
        public ActionResult UpdateFunction(ViewModels.Function toUpdate)
        {
            var json = JsonConvert.SerializeObject(toUpdate);
            TimeTune.EmployeeFunction.update(toUpdate);
            AuditTrail.update(json, "Function", toUpdate.id);
            return Json(new { status= "success"});
        }

        [HttpPost]
        public ActionResult RemoveFunction(ViewModels.Function toRemove)
        {
            var entity=TimeTune.EmployeeFunction.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Function", toRemove.id);
            return Json(new { status = "success" });
        }
        #endregion
        
        #region Region
        public ActionResult Region()
        {
           

            return View();
        }
        [HttpPost]
        public JsonResult RegionDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Region>();
                // get all employee view models
                dtsource = TimeTune.EmployeeRegion.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Region> data = RegionResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = RegionResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Region> result = new DTResult<ViewModels.Region>
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
        public ActionResult AddRegion(ViewModels.Region fromForm)
        {

            int id=TimeTune.EmployeeRegion.add(fromForm);
            var json = JsonConvert.SerializeObject(fromForm);
            AuditTrail.insert(json,"Region", id);
            return RedirectToAction("Region");
        }

        [HttpPost]
        public ActionResult UpdateRegion(ViewModels.Region toUpdate)
        {
            TimeTune.EmployeeRegion.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Region", toUpdate.id);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveRegion(ViewModels.Region toRemove)
        {
            var entity=TimeTune.EmployeeRegion.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Region", entity.id);
            return Json(new { status = "success" });
        }
        #endregion
        
        #region Department
        public ActionResult Department()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DepartmentDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Department>();
                // get all employee view models
                dtsource = TimeTune.EmployeeDepartment.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Department> data = DepartmentResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = DepartmentResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Department> result = new DTResult<ViewModels.Department>
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
        public ActionResult AddDepartment(ViewModels.Department fromForm)
        {
           
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeDepartment.add(fromForm);
            AuditTrail.insert(json, "Department", id);
            return RedirectToAction("Department");
        }

        [HttpPost]
        public ActionResult UpdateDepartment(ViewModels.Department toUpdate)
        {
            TimeTune.EmployeeDepartment.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Department", toUpdate.id);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveDepartment(ViewModels.Department toRemove)
        {
            var entity=TimeTune.EmployeeDepartment.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Department", entity.id);
            return Json(new { status = "success" });
        }
        #endregion

        #region Designation
        public ActionResult Designation()
        {
            
            return View();
        }
        [HttpPost]
        public JsonResult DesignationDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Designation>();
                // get all employee view models
                dtsource = TimeTune.EmployeeDesignation.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Designation> data = DesignationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = DesignationResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Designation> result = new DTResult<ViewModels.Designation>
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
        public ActionResult AddDesignation(ViewModels.Designation fromForm)
        {
            
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeDesignation.add(fromForm);
            AuditTrail.insert(json, "Designation", id);
            return RedirectToAction("Designation");
        }

        [HttpPost]
        public ActionResult UpdateDesignation(ViewModels.Designation toUpdate)
        {
            TimeTune.EmployeeDesignation.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Designation", toUpdate.id);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveDesignation(ViewModels.Designation toRemove)
        {
            var entity=TimeTune.EmployeeDesignation.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Designation", entity.id);
            return Json(new { status = "success" });
        }
        #endregion

        #region Grade
        public ActionResult Grade()
        {
            List<ViewModels.Grades> gradeList = TimeTune.EmployeeGrade.getAll();

            return View("Grade", gradeList);
        }

        [HttpPost]
        public ActionResult AddGrade(ViewModels.Grades fromForm)
        {
           
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeGrade.add(fromForm);
            AuditTrail.insert(json, "Grade", id);
            return RedirectToAction("Grade");
        }

        [HttpPost]
        public ActionResult UpdateGrade(ViewModels.Grades toUpdate)
        {
            TimeTune.EmployeeGrade.update(toUpdate);
           
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Grade", toUpdate.id);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveGrade(ViewModels.Grades toRemove)
        {
            var entity=TimeTune.EmployeeGrade.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Grade", entity.id);
            return Json(new { status = "success" });
        }
        #endregion

        #region Location
        public ActionResult Location()
        {
            return View();
        }
        [HttpPost]
        public JsonResult LocationDataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<ViewModels.Location>();
                // get all employee view models
                dtsource = TimeTune.EmployeeLocation.getAll();
                if (dtsource == null)
                {

                    return Json("No Data to Show");

                }

                List<ViewModels.Location> data = LocationResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource);

                int count = LocationResultSet.Count(param.Search.Value, dtsource);



                DTResult<ViewModels.Location> result = new DTResult<ViewModels.Location>
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
        public ActionResult AddLocation(ViewModels.Location fromForm)
        {
           
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeLocation.add(fromForm);
            AuditTrail.insert(json, "Location", id);
            return RedirectToAction("Location");
        }

        [HttpPost]
        public ActionResult UpdateLocation(ViewModels.Location toUpdate)
        {
            TimeTune.EmployeeLocation.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Location", toUpdate.id);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveLocation(ViewModels.Location toRemove)
        {
            var entity=TimeTune.EmployeeLocation.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Location", entity.id);
            return Json(new { status = "success" });
        }

        #endregion

        #region TypeOfEmployment


        public ActionResult TypeOfEmployment()
        {
            List<ViewModels.TypeOfEmployment> typeofemploymentList = TimeTune.EmployeeTypeOfEmployee.getAll();

            return View("TypeOfEmployment", typeofemploymentList);
        }

        [HttpPost]
        public ActionResult AddTypeOfEmployment(ViewModels.TypeOfEmployment fromForm)
        {
            
            var json = JsonConvert.SerializeObject(fromForm);
            int id = TimeTune.EmployeeTypeOfEmployee.add(fromForm);
            AuditTrail.insert(json, "Type Of Employement",id);
            return RedirectToAction("TypeOfEmployment");
        }

        [HttpPost]
        public ActionResult UpdateTypeOfEmployment(ViewModels.TypeOfEmployment toUpdate)
        {
            TimeTune.EmployeeTypeOfEmployee.update(toUpdate);
            var json = JsonConvert.SerializeObject(toUpdate);
            AuditTrail.update(json, "Location", toUpdate.id);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public ActionResult RemoveTypeOfEmployment(ViewModels.TypeOfEmployment toRemove)
        {
            var entity=TimeTune.EmployeeTypeOfEmployee.remove(toRemove);
            var json = JsonConvert.SerializeObject(entity);
            AuditTrail.delete(json, "Type Of Employement", entity.id);
            return Json(new { status = "success" });
        }
        #endregion

        #region ManageEmployees
        public ActionResult ManageEmployee(string result)
        {
            
            string value = result;
            ViewBag.Message = result;
            // only access groups are to be sent without ajax.
            CreateEmployee createEmployeeViewModel = new CreateEmployee();
            createEmployeeViewModel.accessGroups = EmployeeAccessGroup.getAllButHr();
          


            return View(createEmployeeViewModel);
        }

        [HttpPost]
        public ActionResult AddEmployee(Employee toCreate)
        {
            int id=TimeTune.EmployeeCrud.add(toCreate);
            var json = JsonConvert.SerializeObject(toCreate);
            AuditTrail.insert(json, "Employee", id);
            return RedirectToAction("ManageEmployee");
        }

        // This method is called by the delete modal.
        [HttpPost]
        public JsonResult DeleteEmployee(Employee toRemove)
        {
            int id=TimeTune.EmployeeCrud.remove(toRemove);
            var json = JsonConvert.SerializeObject(toRemove);
            AuditTrail.delete(json, "Employee", id);
            return Json(new { status = "success" });
        }

        // This method is called by the delete modal.
        [HttpPost]
        public JsonResult EditEmployee(Employee toEdit)
        {
            TimeTune.EmployeeCrud.update(toEdit);
            var json = JsonConvert.SerializeObject(toEdit);
            AuditTrail.update(json, "Employee", toEdit.id);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public JsonResult DataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<Employee>();

                // get all employee view models
                int count = EmployeeCrud.searchEmployees(param.Search.Value, param.SortOrder, param.Start, param.Length,out dtsource);
                
                //List<Employee> data = ResultSet.GetResult(param.SortOrder, param.Start, param.Length, dtsource);

                //int count = ResultSet.Count(param.Search.Value, dtsource);

                
                
                
                DTResult<Employee> result = new DTResult<Employee>
                {
                    draw = param.Draw,
                    data = dtsource,
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
        public JsonResult GetEmployee()
        {
            string employee_id = Request.Form["employee_id"];

            int employeeID;

            if (employee_id != null && !employee_id.Equals("") && int.TryParse(employee_id, out employeeID))
            {
                Employee emp = EmployeeCrud.get(employeeID);

                return Json(new
                {
                    success=true,
                    id = emp.id,
                    first_name= emp.first_name,
                    last_name= emp.last_name,
                    employee_code= emp.employee_code,
                    email= emp.email,
                    address= emp.address,
                    mobile_no= emp.mobile_no,
                    date_of_joining=  emp.date_of_joining,//(emp.date_of_joining != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_joining, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null,
                    date_of_leaving = emp.date_of_leaving,//(emp.date_of_leaving != null) ? (DateTime?)DateTime.ParseExact(emp.date_of_leaving, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null,
                    
                    function_id= (emp.function_id !=-1 )? emp.function_id:0,
                    function_name = (emp.function_name!=null)?emp.function_name:"",

                    designation_id = (emp.designation_id != -1) ? emp.department_id : 0,
                    designation_name = (emp.designation_name!=null)?emp.department_name:"",

                    department_id = (emp.department_id != -1) ? emp.department_id : 0,
                    department_name = (emp.department_name!=null)?emp.department_name:"",

                    type_of_employment_id = (emp.type_of_employment_id != -1) ? emp.type_of_employment_id : 0,
                    type_of_employment_name = (emp.type_of_employment_name!=null)?emp.type_of_employment_name:"",

                    access_group_id = (emp.access_group_id != -1) ? emp.access_group_id : 0,
                    access_group_name = (emp.access_group_name!=null)?emp.access_group_name:"",
                    
                    region_id = (emp.region_id != -1) ? emp.region_id : 0,
                    region_name = (emp.region_name!=null)?emp.region_name:"",

                    grade_id = (emp.grade_id != -1) ? emp.grade_id : 0,
                    grade_name = (emp.grade_name!=null)?emp.grade_name:"",

                    time_tune_status_id=(emp.time_tune_status =="Active") ? "1" : "0",
                    time_tune_status_val=(emp.time_tune_status=="Active")? "Active":"Deactive",
                    location_id = (emp.location_id != -1) ? emp.location_id : 0,
                    location_name = (emp.location_name!=null)?emp.location_name:""
                });
            }
            else 
            {
                return Json(new { success=false });
            }

            /*{
                    id = ''
                    first_name= ''
                    last_name= ''
                    employee_code= ''
                    email= ''
                    address= ''
                    mobile_no= ''
                    date_of_joining= ''
                    date_of_leaving= ''
                    function_id= ''
                    designation_id= ''
                    department_id= ''
                    type_of_employment_id= ''
                    access_group_id= ''
                    region_id= ''
                    grade_id= ''
                    location_id= ''
                }
             */

            
        }

        #region ChosenAjaxDatahandlers
        /* 
         * checkimg chosen ajaxification list.
         */



        // functions
        [HttpPost]
        public JsonResult functions()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Function[] functions = TimeTune.EmployeeManagementHelper.getAllFunctionsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[functions.Length];
            for (int i = 0; i < functions.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = functions[i].id.ToString();
                toSend[i].text = functions[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // designations
        [HttpPost]
        public JsonResult designations()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Designation[] designations = TimeTune.EmployeeManagementHelper.getAllDesignationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[designations.Length];
            for (int i = 0; i < designations.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = designations[i].id.ToString();
                toSend[i].text = designations[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // departments
        [HttpPost]
        public JsonResult departments()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Department[] departments = TimeTune.EmployeeManagementHelper.getAllDepartmentsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[departments.Length];
            for (int i = 0; i < departments.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = departments[i].id.ToString();
                toSend[i].text = departments[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // type of employments
        [HttpPost]
        public JsonResult typeOfEmployments()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.TypeOfEmployment[] typeOEmployments = TimeTune.EmployeeManagementHelper.getAllTypeOfEmploymentsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[typeOEmployments.Length];
            for (int i = 0; i < typeOEmployments.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = typeOEmployments[i].id.ToString();
                toSend[i].text = typeOEmployments[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // regions
        [HttpPost]
        public JsonResult regions()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Region[] regions = TimeTune.EmployeeManagementHelper.getAllRegionsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = regions[i].id.ToString();
                toSend[i].text = regions[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        
        // grades
        [HttpPost]
        public JsonResult grades()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Grades[] grades = TimeTune.EmployeeManagementHelper.getAllGradesMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[grades.Length];
            for (int i = 0; i < grades.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = grades[i].id.ToString();
                toSend[i].text = grades[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        // locations
        [HttpPost]
        public JsonResult locations()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Location[] locations = TimeTune.EmployeeManagementHelper.getAllLocationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[locations.Length];
            for (int i = 0; i < locations.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = locations[i].id.ToString();
                toSend[i].text = locations[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        
        


        #endregion
        
        #endregion

        #region ChangeEmployeePassword

        public ActionResult ChangeEmployeePassword(string error)
        {
            if (error != null && !error.Equals(""))
                ModelState.AddModelError("",error);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmployeePassword()
        {
            int employeeID;

            if (!int.TryParse(Request.Form["change_password_employee"], out employeeID))
                return RedirectToAction("ChangeEmployeePassword", new { error="Invalid emplyoee id."});

            Employee emp = TimeTune.EmployeeCrud.get(employeeID);
            if(emp == null)
                return RedirectToAction("ChangeEmployeePassword", new { error = "Invalid emplyoee id." });


            string newPassword = Request.Form["employee_password"];
            if(newPassword == null || newPassword.Equals(""))
                return RedirectToAction("ChangeEmployeePassword", new { error = "Password is required." });

            int id=TimeTune.EmployeeCrud.setPassword(employeeID,newPassword);

            //Aduit Log
            var json=JsonConvert.SerializeObject(emp);
            AuditTrail.update(json,"EmployeePasswordChange", int.Parse(User.Identity.Name));

            return RedirectToAction("ChangeEmployeePassword");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPasswordFile (HttpPostedFileBase file)
        {
            string result = null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                string path = Path.Combine(Server.MapPath("~/Uploads"),
                                   "ChangePassword.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                result = TimeTune.EmployeeCrud.bulkSetPassword(content);
                return RedirectToAction("ChangeEmployeePassword", new { result=result});
                

            }
            return RedirectToAction("ChangeEmployeePassword", "EmployeeManagement", new { result = "Select File first" });
        }



        [HttpPost]
        public JsonResult ChangeEmployeePasswordDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].id.ToString();
                toSend[i].text = employees[i].employee_code;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }


        #endregion


        #region Upload Download Employee
              [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadCSVFile() {

            var toReturn = 
                
                new MvcApplication1.Utils.CSVWriter<TimeTune.ManageEmployeeImportExport.ManageEmployeeCSV>
                    (
                        TimeTune.ManageEmployeeImportExport.getManageEmployeeCSV(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS")+"-EMPLOYEE.csv"
                    );


            return toReturn;
        }


        // Handle file upload and read/write etc.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string result=null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                    string path = Path.Combine(Server.MapPath("~/Uploads"),
                                       "employee.csv");
                    file.SaveAs(path);


                    List<string> content = new List<string>();


                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            content.Add(sr.ReadLine());
                        }
                    }

                    result=TimeTune.ManageEmployeeImportExport.setEmployees(content);
                    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Successful" });
                    //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}
               
            }
            return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Select File first" });
        }
        
        #endregion


        public ActionResult EmployeeHierarchy()
        {
            return View();
        }

        public ActionResult ManualUploads()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ManualUploads(HttpPostedFileBase file)
        {
            if (Request.Files.Count > 0)
            {

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine("D:\\TimeTune", fileName);
                    string line;
                    string[] words;
                    if (fileName.Equals("function.csv"))
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string buff;

                        while ((line = reader.ReadLine()) != null)
                        {
                            words = line.Split(',');
                            
                        }

                    }

                }
            }
            return View();
        }

        public ActionResult HierarchyTransfer()
        {
            return View();
        }
    }


}
