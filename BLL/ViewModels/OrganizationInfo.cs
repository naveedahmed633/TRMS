using DLL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTune;
using System.Web;
using System.Web.UI.WebControls;
using ViewModels;
using System.Collections;

namespace ViewModels
{

    #region Organization

    public class OrganizationResultSet
    {

        public static OrganizationView GetOrganizationData()
        {
            OrganizationView oView = new OrganizationView();

            using (Context db = new Context())
            {
                //Organization org = null; // new Organization();

                try
                {
                    Organization org = db.organization.FirstOrDefault();
                    if (org != null)
                    {
                        oView.Id = org.Id;
                        oView.OrganizationTitle = org.OrganizationTitle;
                        oView.EstablishedDateText = org.EstablishedDate.ToString("dd-MM-yyyy");
                        oView.CampusLimit = org.CampusLimit;
                        oView.Logo = org.Logo;
                        oView.WebsiteURL = org.WebsiteURL;
                        oView.Description = org.Description;
                    }
                    else
                    {
                        oView.Id = 0;
                        oView.OrganizationTitle = "DUHS - DOW University of Health Sciences";
                        oView.EstablishedDateText = DateTime.Now.ToString("dd-MM-yyyy");
                        oView.CampusLimit = 1;
                        oView.Logo = "/Content/Logos/DUHSLogo.png";
                        oView.WebsiteURL = "https://www.duhs.edu.pk/";
                        oView.Description = "";
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    Organization org = new Organization();
                }
            }

            return oView;
        }

        public static int UpdateOrganization(OrganizationView toUpdate)
        {
            int response = 0;
            Organization orgModel = null;

            try
            {
                using (Context db = new Context())
                {
                    if (toUpdate.Id > 0)
                    {
                        orgModel = db.organization.Find(toUpdate.Id);

                        orgModel.OrganizationTitle = toUpdate.OrganizationTitle;
                        orgModel.EstablishedDate = toUpdate.EstablishedDate;
                        orgModel.CampusLimit = toUpdate.CampusLimit;
                        orgModel.Logo = toUpdate.Logo;
                        orgModel.WebsiteURL = toUpdate.WebsiteURL;
                        orgModel.Description = toUpdate.Description;

                        db.SaveChanges();
                        response = 0;
                    }
                    else
                    {
                        orgModel.OrganizationTitle = toUpdate.OrganizationTitle;
                        orgModel.EstablishedDate = toUpdate.EstablishedDate;
                        orgModel.CampusLimit = toUpdate.CampusLimit;
                        orgModel.Logo = toUpdate.Logo;
                        orgModel.WebsiteURL = toUpdate.WebsiteURL;
                        orgModel.Description = toUpdate.Description;
                        orgModel.CreateDateOrg = DateTime.Now;

                        db.organization.Add(orgModel);
                        db.SaveChanges();

                        response = orgModel.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }
    }

    public class OrganizationListReportResultSet
    {
        public static List<ViewModels.LeavesListReportLog> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeavesListReportLog> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.LeavesListReportLog> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.LeavesListReportLog> FilterResult(string search, List<ViewModels.LeavesListReportLog> dtResult)
        {
            IQueryable<ViewModels.LeavesListReportLog> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower()) ||
                         p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower())
                        ||
                         p.AllocatedSickLeaves != null && p.AllocatedSickLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AllocatedCasualLeaves != null && p.AllocatedCasualLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AvailedCasualLeaves != null && p.AvailedCasualLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AvailedAnnualLeaves != null && p.AvailedAnnualLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AllocatedAnnualLeaves != null && p.AllocatedAnnualLeaves.ToString().ToLower().Contains(search.ToLower())
                          ||
                         p.AvailedSickLeaves != null && p.AvailedSickLeaves.ToString().ToLower().Contains(search.ToLower())
                        || (p.Last_Name != null && p.Last_Name.ToLower().Contains(search.ToLower()))


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

    #endregion

    #region Organization-Campus

    public class OrganizationCampusResultSet
    {
        public static List<OrganizationView> GetOrganizationsList()
        {
            OrganizationView oView = new OrganizationView();
            List<OrganizationView> lView = new List<OrganizationView>();

            using (Context db = new Context())
            {
                try
                {
                    Organization org = db.organization.FirstOrDefault();
                    if (org != null)
                    {
                        oView.Id = org.Id;
                        oView.OrganizationTitle = org.OrganizationTitle;
                        oView.EstablishedDateText = org.EstablishedDate.ToString("dd-MM-yyyy");
                        oView.CampusLimit = org.CampusLimit;
                        oView.Logo = org.Logo;
                        oView.WebsiteURL = org.WebsiteURL;
                        oView.Description = org.Description;
                    }

                    lView.Add(oView);
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    Organization org = new Organization();
                }
            }

            return lView;
        }

        public static List<OrganizationCampusTypeView> GetOrganizationCampusTypesList()
        {
            List<DLL.Models.OrganizationCampusType> mView = null;
            List<OrganizationCampusTypeView> lView = new List<OrganizationCampusTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_campus_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusTypeView() { Id = item.Id, CampusTypeName = item.CampusTypeName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampusType>();
                }
            }

            return lView;
        }


        public static List<LocationView> GetLocationsList()
        {
            List<DLL.Models.Location> mView = null;
            List<LocationView> lView = new List<LocationView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.location.Where(l => l.active).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new LocationView() { Id = item.LocationId, Name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Location>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramTypeView> GetProgramTypesList()
        {
            List<OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramTypeView() { Id = item.Id, ProgramTypeName = item.ProgramTypeName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<OrganizationProgramType>();
                }
            }

            return lView;
        }

        public static List<StateProvinceView> GetStatesProvincesList()
        {
            List<StateProvince> mView = null;
            List<StateProvinceView> lView = new List<StateProvinceView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.state_province.Where(s => s.active).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new StateProvinceView() { Id = item.StateProvinceId, Name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<StateProvince>();
                }
            }

            return lView;
        }

        public static string GetOrganizationCampusTypeString()
        {
            string type_list = "";
            List<DLL.Models.OrganizationCampusType> mView = null;
            List<OrganizationCampusTypeView> lView = new List<OrganizationCampusTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_campus_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            type_list += c.Id + ":" + c.CampusTypeName.Replace(",", "") + ",";
                        }

                        type_list = type_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampusType>();
                }
            }

            return type_list.Replace("\r\n", "");
        }

        public static int CreateOrganizationCampus(OrganizationCampusView toCreate)
        {
            int response = 0;
            OrganizationCampus cmpModel = new OrganizationCampus();

            try
            {
                using (Context db = new Context())
                {
                    var already_campus = db.organization_campus.Where(c => c.CampusCode != null && c.CampusCode.ToLower() == toCreate.CampusCode.ToLower()).FirstOrDefault();
                    if (already_campus == null)
                    {
                        cmpModel.OrganizationId = toCreate.OrganizationId;
                        cmpModel.CampusTypeId = toCreate.CampusTypeId;
                        cmpModel.CampusCode = toCreate.CampusCode;
                        cmpModel.CampusTitle = toCreate.CampusTitle;
                        cmpModel.EmailAddress = toCreate.EmailAddress;
                        cmpModel.Address = toCreate.Address;
                        cmpModel.CityId = toCreate.CityId;
                        cmpModel.StateProvinceId = toCreate.StateProvinceId;
                        cmpModel.ZipCode = toCreate.ZipCode;
                        cmpModel.Phone01 = toCreate.Phone01;
                        cmpModel.Phone02 = toCreate.Phone02;
                        cmpModel.FaxNumber = toCreate.FaxNumber;
                        cmpModel.CreateDateCmp = toCreate.CreateDateCmp;

                        db.organization_campus.Add(cmpModel);
                        db.SaveChanges();

                        response = cmpModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationCampus(OrganizationCampusView toUpdate)
        {
            int response = 0;
            OrganizationCampus cmpModel = null;

            try
            {
                using (Context db = new Context())
                {
                    var already_exists = db.organization_campus.Where(c => c.Id != toUpdate.Id && (c.CampusCode != null && c.CampusCode.ToLower() == toUpdate.CampusCode.ToLower())).FirstOrDefault();
                    if (already_exists != null)
                    {
                        //another same campus-code already there in table
                        response = 0;
                    }
                    else
                    {
                        cmpModel = db.organization_campus.Find(toUpdate.Id);

                        //cmpModel.OrganizationId = toUpdate.OrganizationId;
                        cmpModel.CampusTypeId = toUpdate.CampusTypeId;
                        cmpModel.CampusCode = toUpdate.CampusCode;
                        cmpModel.CampusTitle = toUpdate.CampusTitle;
                        cmpModel.EmailAddress = toUpdate.EmailAddress;
                        cmpModel.Address = toUpdate.Address;
                        ////cmpModel.CityId = toUpdate.CityId;
                        ////cmpModel.StateProvinceId = toUpdate.StateProvinceId;
                        cmpModel.ZipCode = toUpdate.ZipCode;
                        cmpModel.Phone01 = toUpdate.Phone01;
                        cmpModel.Phone02 = toUpdate.Phone02;
                        ////cmpModel.FaxNumber = toUpdate.FaxNumber;
                        //cmpModel.CreateDateCmp = toUpdate.CreateDateCmp;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationCampusView RemoveOrganizationCampus(ViewModels.OrganizationCampusView toRemove)
        {

            using (Context db = new Context())
            {
                OrganizationCampus toRemoveModel = db.organization_campus.Find(toRemove.Id);

                //db.leave_application.Remove(toRemoveModel);

                //toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove;
            }

        }


        public static List<ViewModels.OrganizationCampusView> getOrganizationCampusByUserCode(string user_code)
        {
            string strAttPath = string.Empty;
            List<ViewModels.OrganizationCampusView> toReturn = new List<ViewModels.OrganizationCampusView>();

            using (Context db = new Context())
            {
                List<OrganizationCampus> oCampus = new List<OrganizationCampus>();
                try
                {
                    int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    //MISSING org_id using employee record
                    int org_id = 1;

                    oCampus = db.organization_campus.Where(l => l.OrganizationId == org_id).ToList();

                    if (oCampus != null && oCampus.Count > 0)
                    {
                        for (int i = 0; i < oCampus.Count(); i++)
                        {
                            int ct_id = oCampus[i].CampusTypeId;
                            var tb_campus_type = db.organization_campus_type.Where(e => e.Id == ct_id).FirstOrDefault();
                            string ctype_name = tb_campus_type != null ? tb_campus_type.CampusTypeName : "";

                            int cy_id = oCampus[i].CityId;
                            var tb_city = db.location.Where(e => e.LocationId == cy_id).FirstOrDefault();
                            string city_name = tb_city != null ? tb_city.name : "";

                            int st_id = oCampus[i].StateProvinceId;
                            var tb_state_province = db.state_province.Where(e => e.StateProvinceId == st_id).FirstOrDefault();
                            string state_name = tb_state_province != null ? tb_state_province.name : "";

                            toReturn.Add(new ViewModels.OrganizationCampusView()
                            {
                                Id = oCampus[i].Id,
                                OrganizationId = oCampus[i].OrganizationId,
                                CampusTypeId = oCampus[i].CampusTypeId,
                                CampusTypeName = ctype_name,
                                CampusCode = oCampus[i].CampusCode,
                                CampusTitle = oCampus[i].CampusTitle, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                EmailAddress = oCampus[i].EmailAddress,
                                Address = oCampus[i].Address,
                                CityId = oCampus[i].CityId,
                                CityName = city_name,
                                StateProvinceId = oCampus[i].StateProvinceId,
                                StateProvinceName = state_name,
                                ZipCode = oCampus[i].ZipCode,
                                Phone01 = oCampus[i].Phone01,
                                Phone02 = oCampus[i].Phone02,
                                FaxNumber = oCampus[i].FaxNumber,
                                CreateDateCmpText = oCampus[i].CreateDateCmp.ToString("dd-MMM-yyyy"),
                                actions =
                                    "<span data-row='" + oCampus[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationCampus(" + oCampus[i].Id + "," + oCampus[i].OrganizationId + "," + oCampus[i].CampusTypeId + ",'" + oCampus[i].CampusCode + "','" + oCampus[i].CampusTitle + "','" + oCampus[i].EmailAddress + "','" + oCampus[i].Address + "','" + oCampus[i].ZipCode + "','" + oCampus[i].Phone01 + "','" + oCampus[i].Phone02 + "'));\">Edit</a>" +
                                    //"<span> / </span>" +
                                    //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oCampus = new List<OrganizationCampus>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCampusView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCampusView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCampusView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCampusView> FilterResult(string search, List<ViewModels.OrganizationCampusView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCampusView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.OrganizationName != null && p.OrganizationName.ToLower().Contains(search.ToLower()) ||
                         p.OrganizationName != null && p.OrganizationName.ToLower().Contains(search.ToLower()) ||
                         p.CampusCode != null && p.CampusCode.ToString().ToLower().Contains(search.ToLower()) ||
                         p.CampusTitle != null && p.CampusTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.CampusTypeName != null && p.CampusTypeName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.EmailAddress != null && p.EmailAddress.ToString().ToLower().Contains(search.ToLower()) ||
                         p.Address != null && p.Address.ToString().ToLower().Contains(search.ToLower()) ||
                         p.CityName != null && p.CityName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.StateProvinceName != null && p.StateProvinceName.ToLower().Contains(search.ToLower())
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

    #endregion


    #region Organization-Campus-Building

    public class OrganizationCampusBuildingResultSet
    {

        public static List<OrganizationCampusView> GetOrganizationCampusList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusView() { Id = item.Id, CampusCode = item.CampusCode + "-" + item.CampusTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return lView;
        }

        public static string GetOrganizationCampusString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string campus_list = "";
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            campus_list += c.Id + ":" + c.CampusCode + "-" + c.CampusTitle.Replace(",", "") + ",";
                        }

                        campus_list = campus_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return campus_list.Replace("\r\n", "");
        }

        public static int CreateOrganizationBuilding(OrganizationCampusBuildingView toCreate)
        {
            int response = 0;
            OrganizationCampusBuilding bldModel = new OrganizationCampusBuilding();

            try
            {
                using (Context db = new Context())
                {
                    var already_building = db.organization_campus_building.Where(c => c.CampusId == toCreate.CampusId && (c.BuildingCode != null && c.BuildingCode.ToLower() == toCreate.BuildingCode.ToLower())).FirstOrDefault();
                    if (already_building == null)
                    {
                        bldModel.CampusId = toCreate.CampusId;
                        bldModel.BuildingCode = toCreate.BuildingCode;
                        bldModel.BuildingTitle = toCreate.BuildingTitle;
                        bldModel.CreateDateBld = toCreate.CreateDateBld;

                        db.organization_campus_building.Add(bldModel);
                        db.SaveChanges();

                        response = bldModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationBuilding(OrganizationCampusBuildingView toUpdate)
        {
            int response = 0;
            OrganizationCampusBuilding bldModel = null;

            try
            {
                using (Context db = new Context())
                {
                    var already_exists = db.organization_campus_building.Where(c => c.Id != toUpdate.Id && c.CampusId == toUpdate.CampusId && (c.BuildingCode != null && c.BuildingCode.ToLower() == toUpdate.BuildingCode.ToLower())).FirstOrDefault();
                    if (already_exists != null)
                    {
                        //another same campus-code already there in table
                        response = 0;
                    }
                    else
                    {
                        bldModel = db.organization_campus_building.Find(toUpdate.Id);

                        //bldModel.OrganizationId = toUpdate.OrganizationId;
                        bldModel.CampusId = toUpdate.CampusId;
                        bldModel.BuildingCode = toUpdate.BuildingCode;
                        bldModel.BuildingTitle = toUpdate.BuildingTitle;
                        //bldModel.CreateDateBld = toUpdate.CreateDateCmp;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationCampusBuildingView RemoveOrganizationBuilding(ViewModels.OrganizationCampusBuildingView toRemove)
        {

            using (Context db = new Context())
            {
                OrganizationCampusBuilding toRemoveModel = db.organization_campus_building.Find(toRemove.Id);

                //db.leave_application.Remove(toRemoveModel);

                //toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove;
            }

        }


        public static List<ViewModels.OrganizationCampusBuildingView> getOrganizationCampusBuildingsByUserCode(string user_code)
        {
            string strAttPath = string.Empty;
            List<ViewModels.OrganizationCampusBuildingView> toReturn = new List<ViewModels.OrganizationCampusBuildingView>();

            using (Context db = new Context())
            {
                List<OrganizationCampus> oCampus = new List<OrganizationCampus>();
                List<OrganizationCampusBuilding> oBuilding = new List<OrganizationCampusBuilding>();

                try
                {
                    //string campuses_list = "";
                    //int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    //oCampus = db.organization_campus.ToList();
                    //foreach (var c in oCampus)
                    //{
                    //    campuses_list += c.Id + ":" + c.CampusCode + "-" + c.CampusTitle + ",";
                    //}
                    //campuses_list = campuses_list.TrimEnd(',');

                    //MISSING org_id using employee record
                    //int cmp_id = 1;

                    oBuilding = db.organization_campus_building.ToList(); //db.organization_campus_building.Where(l => l.CampusId == cmp_id).ToList();

                    if (oBuilding != null && oBuilding.Count > 0)
                    {
                        for (int i = 0; i < oBuilding.Count(); i++)
                        {
                            int ct_id = oBuilding[i].CampusId;
                            var tb_campus = db.organization_campus.Where(e => e.Id == ct_id).FirstOrDefault();
                            string cam_title = tb_campus != null ? tb_campus.CampusTitle : "";

                            toReturn.Add(new ViewModels.OrganizationCampusBuildingView()
                            {
                                Id = oBuilding[i].Id,
                                CampusId = oBuilding[i].CampusId,
                                CampusTitle = cam_title,
                                BuildingCode = oBuilding[i].BuildingCode,
                                BuildingTitle = oBuilding[i].BuildingTitle, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                CreateDateBldText = oBuilding[i].CreateDateBld.ToString("dd-MMM-yyyy"),
                                actions =
                                    "<span data-row='" + oBuilding[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationBuilding(" + oBuilding[i].Id + "," + oBuilding[i].CampusId + ",'" + oBuilding[i].BuildingCode + "','" + oBuilding[i].BuildingTitle + "'));\">Edit</a>" +
                                    //"<span> / </span>" +
                                    //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oBuilding = new List<OrganizationCampusBuilding>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCampusBuildingView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCampusBuildingView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCampusBuildingView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCampusBuildingView> FilterResult(string search, List<ViewModels.OrganizationCampusBuildingView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCampusBuildingView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CampusTitle != null && p.CampusTitle.ToLower().Contains(search.ToLower()) ||
                         p.BuildingCode != null && p.BuildingCode.ToLower().Contains(search.ToLower()) ||
                         p.BuildingTitle != null && p.BuildingTitle.ToString().ToLower().Contains(search.ToLower())
                    )
                )
           );

            return results;
        }

    }

    #endregion


    #region Organization-Campus-Building-Room

    public class OrganizationCampusBuildingRoomResultSet
    {

        public static List<OrganizationCampusBuildingView> GetOrganizationBuildingList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampusBuilding> mView = null;
            List<OrganizationCampusBuildingView> lView = new List<OrganizationCampusBuildingView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus_building.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus_building.Where(b => b.CampusId == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusBuildingView() { Id = item.Id, BuildingCode = item.BuildingCode + "-" + item.BuildingTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampusBuilding>();
                }
            }

            return lView;
        }

        public static List<TerminalView> GetOrganizationTerminalList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.Terminals> mView = null;
            List<TerminalView> lView = new List<TerminalView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.termainal.ToList();
                        if (mView != null && mView.Count > 0)
                        {
                            foreach (var item in mView)
                            {
                                lView.Add(new TerminalView() { Id = item.L_ID, C_Name = item.C_Name });
                            }
                        }
                    }
                    else
                    {
                        lView = (from t in db.termainal
                                 join r in db.organization_campus_building_room on t.L_ID equals r.TerminalId
                                 join b in db.organization_campus_building on r.BuildingId equals b.Id
                                 where b.CampusId == iGVCampusID
                                 select new TerminalView { Id = t.L_ID, C_Name = t.C_Name }).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Terminals>();
                }
            }

            return lView;
        }

        public static string GetOrganizationCampusBuildingString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string building_list = "";
            List<DLL.Models.OrganizationCampusBuilding> mView = null;
            List<OrganizationCampusBuildingView> lView = new List<OrganizationCampusBuildingView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus_building.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus_building.Where(b => b.CampusId == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            building_list += c.Id + ":" + c.BuildingCode + "-" + c.BuildingTitle.Replace(",", "") + ",";
                        }

                        building_list = building_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampusBuilding>();
                }
            }

            return building_list.Replace("\r\n", "");
        }

        public static string GetOrganizationTerminalsString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string terminal_list = "";
            List<DLL.Models.Terminals> mView = null;
            List<TerminalView> lView = new List<TerminalView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.termainal.ToList();
                        if (mView != null && mView.Count > 0)
                        {
                            foreach (var c in mView)
                            {
                                terminal_list += c.L_ID + ":" + c.C_Name.Replace(",", "") + ",";
                            }

                            terminal_list = terminal_list.TrimEnd(',');
                        }
                    }
                    else
                    {
                        lView = (from t in db.termainal
                                 join r in db.organization_campus_building_room on t.L_ID equals r.TerminalId
                                 join b in db.organization_campus_building on r.BuildingId equals b.Id
                                 where b.CampusId == iGVCampusID
                                 select new TerminalView { Id = t.L_ID, C_Name = t.C_Name }).ToList();

                        if (lView != null && lView.Count > 0)
                        {
                            foreach (var c in lView)
                            {
                                terminal_list += c.Id + ":" + c.C_Name.Replace(",", "") + ",";
                            }

                            terminal_list = terminal_list.TrimEnd(',');
                        }
                    }


                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Terminals>();
                }
            }

            return terminal_list.Replace("\r\n", "");
        }

        public static int CreateOrganizationRoom(OrganizationCampusBuildingRoomView toCreate)
        {
            int response = 0;
            OrganizationCampusBuildingRoom rmModel = new OrganizationCampusBuildingRoom();

            try
            {
                using (Context db = new Context())
                {
                    var already_building = db.organization_campus_building_room.Where(c => c.BuildingId == toCreate.BuildingId && (c.RoomCode != null && c.RoomCode.ToLower() == toCreate.RoomCode.ToLower())).FirstOrDefault();
                    if (already_building == null)
                    {
                        rmModel.BuildingId = toCreate.BuildingId;
                        rmModel.RoomCode = toCreate.RoomCode;
                        rmModel.RoomTitle = toCreate.RoomTitle;
                        rmModel.TerminalId = toCreate.TerminalId;
                        rmModel.FloorNumber = toCreate.FloorNumber;
                        rmModel.CreateDateRm = toCreate.CreateDateRm;

                        db.organization_campus_building_room.Add(rmModel);
                        db.SaveChanges();

                        response = rmModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationRoom(OrganizationCampusBuildingRoomView toUpdate)
        {
            int response = 0;
            OrganizationCampusBuildingRoom rmModel = null;

            try
            {
                using (Context db = new Context())
                {
                    var already_exists = db.organization_campus_building_room.Where(c => c.Id != toUpdate.Id && c.BuildingId == toUpdate.BuildingId && (c.RoomCode != null && c.RoomCode.ToLower() == toUpdate.RoomCode.ToLower())).FirstOrDefault();
                    if (already_exists != null)
                    {
                        //another same campus-code already there in table

                        response = 0;
                    }
                    else
                    {
                        rmModel = db.organization_campus_building_room.Find(toUpdate.Id);

                        //rmModel.OrganizationId = toUpdate.OrganizationId;
                        rmModel.BuildingId = toUpdate.BuildingId;
                        rmModel.RoomCode = toUpdate.RoomCode;
                        rmModel.RoomTitle = toUpdate.RoomTitle;
                        rmModel.TerminalId = toUpdate.TerminalId;
                        rmModel.FloorNumber = toUpdate.FloorNumber;
                        //rmModel.CreateDateBld = toUpdate.CreateDateCmp;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationCampusBuildingRoomView RemoveOrganizationRoom(ViewModels.OrganizationCampusBuildingRoomView toRemove)
        {

            using (Context db = new Context())
            {
                OrganizationCampusBuildingRoom toRemoveModel = db.organization_campus_building_room.Find(toRemove.Id);

                //db.leave_application.Remove(toRemoveModel);

                //toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove;
            }

        }


        public static List<ViewModels.OrganizationCampusBuildingRoomView> getOrganizationCampusBuildingsRoomByUserCode(string user_code)
        {
            List<ViewModels.OrganizationCampusBuildingRoomView> toReturn = new List<ViewModels.OrganizationCampusBuildingRoomView>();

            using (Context db = new Context())
            {
                List<OrganizationCampusBuilding> oBuilding = new List<OrganizationCampusBuilding>();
                List<Terminals> oTerminal = new List<Terminals>();
                List<OrganizationCampusBuildingRoom> oRoom = new List<OrganizationCampusBuildingRoom>();

                try
                {
                    string building_list = "", terminal_list = "";
                    int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    oBuilding = db.organization_campus_building.ToList();
                    foreach (var c in oBuilding)
                    {
                        building_list += c.Id + ":" + c.BuildingCode + "-" + c.BuildingTitle + ",";
                    }
                    building_list = building_list.TrimEnd(',');

                    oTerminal = db.termainal.ToList();
                    foreach (var t in oTerminal)
                    {
                        terminal_list += t.L_ID + ":" + t.C_Name + ",";
                    }
                    terminal_list = terminal_list.TrimEnd(',');

                    //MISSING org_id using employee record
                    int bld_id = 1;

                    oRoom = db.organization_campus_building_room.Where(l => l.BuildingId == bld_id).ToList();

                    if (oRoom != null && oRoom.Count > 0)
                    {
                        for (int i = 0; i < oRoom.Count(); i++)
                        {
                            int b_id = oRoom[i].BuildingId;
                            var tb_building = db.organization_campus_building.Where(e => e.Id == b_id).FirstOrDefault();
                            string bld_title = tb_building != null ? tb_building.BuildingTitle : "";

                            int t_id = oRoom[i].TerminalId;
                            var tb_terminal = db.termainal.Where(e => e.L_ID == t_id).FirstOrDefault();
                            string term_title = tb_terminal != null ? tb_terminal.C_Name : "";

                            toReturn.Add(new ViewModels.OrganizationCampusBuildingRoomView()
                            {
                                Id = oRoom[i].Id,
                                BuildingId = oRoom[i].BuildingId,
                                BuildingTitle = bld_title,
                                RoomCode = oRoom[i].RoomCode ?? "",
                                RoomTitle = oRoom[i].RoomTitle ?? "",
                                TerminalId = oRoom[i].TerminalId,
                                TerminalTitle = term_title,
                                FloorNumber = oRoom[i].FloorNumber,
                                CreateDateRmText = oRoom[i].CreateDateRm.ToString("dd-MMM-yyyy"),
                                actions =
                                    "<span data-row='" + oRoom[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationRoom(" + oRoom[i].Id + "," + oRoom[i].BuildingId + ",'" + oRoom[i].RoomCode + "','" + oRoom[i].RoomTitle + "'," + oRoom[i].TerminalId + "," + oRoom[i].FloorNumber + "));\">Edit</a>" +
                                    //"<span> / </span>" +
                                    //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oRoom = new List<OrganizationCampusBuildingRoom>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCampusBuildingRoomView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCampusBuildingRoomView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCampusBuildingRoomView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCampusBuildingRoomView> FilterResult(string search, List<ViewModels.OrganizationCampusBuildingRoomView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCampusBuildingRoomView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.BuildingTitle != null && p.BuildingTitle.ToLower().Contains(search.ToLower()) ||
                         p.RoomCode != null && p.RoomCode.ToLower().Contains(search.ToLower()) ||
                         p.RoomTitle != null && p.RoomTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.FloorNumber != null && p.FloorNumber.ToString().ToLower().Contains(search.ToLower()) ||
                         p.TerminalTitle != null && p.TerminalTitle.ToString().ToLower().Contains(search.ToLower())
                    )
                )
           );

            return results;
        }

    }

    #endregion


    #region Organization-Campus-Building-Room-Schedule

    public class OrganizationCampusRoomCourseScheduleResultSet
    {
        public static List<OrganizationCampusView> GetOrganizationCampusList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusView() { Id = item.Id, CampusCode = item.CampusCode + "-" + item.CampusTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramView> GetOrganizationProgramList()
        {
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramView() { Id = item.Id, ProgramCode = item.ProgramCode + "-" + item.ProgramTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return lView;
        }


        public static List<OrganizationCampusBuildingRoomView> GetOrganizationRoomList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampusBuildingRoom> mView = null;
            List<OrganizationCampusBuildingRoomView> lView = new List<OrganizationCampusBuildingRoomView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus_building_room.ToList();

                        if (mView != null && mView.Count > 0)
                        {
                            foreach (var item in mView)
                            {
                                lView.Add(new OrganizationCampusBuildingRoomView() { Id = item.Id, RoomCode = item.RoomCode + "-" + item.RoomTitle });
                            }
                        }
                    }
                    else
                    {
                        lView = (from r in db.organization_campus_building_room
                                 join b in db.organization_campus_building on r.BuildingId equals b.Id
                                 join c in db.organization_campus on b.CampusId equals c.Id
                                 where c.Id == iGVCampusID
                                 select new OrganizationCampusBuildingRoomView { Id = r.Id, RoomCode = r.RoomCode + "-" + r.RoomTitle }).ToList();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampusBuildingRoom>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramCourseView> GetOrganizationCourseList()
        {
            List<DLL.Models.OrganizationProgramCourse> mView = null;
            List<OrganizationProgramCourseView> lView = new List<OrganizationProgramCourseView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_course.Where(c => c.IsActiveCourse).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramCourseView() { Id = item.Id, CourseCode = item.CourseCode + "-" + item.CourseTitle });
                        }

                        lView.Add(new OrganizationProgramCourseView() { Id = 0, CourseCode = "OFF" });
                        lView.Add(new OrganizationProgramCourseView() { Id = -1, CourseCode = "Break" });
                        //lView.Add(new OrganizationProgramCourseView() { Id = -2, CourseCode = "Seminar" });
                        //lView.Add(new OrganizationProgramCourseView() { Id = -3, CourseCode = "Self Study" });
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramCourse>();
                }
            }

            return lView;
        }

        public static List<LGRegionView> GetOrganizationLectureGroupList()
        {
            List<DLL.Models.Region> mView = null;
            List<LGRegionView> lView = new List<LGRegionView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.region.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new LGRegionView() { Id = item.RegionId, Name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Region>();
                }
            }

            return lView;
        }

        public static List<EmployeeView> GetOrganizationEmployeeTeacherList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "lecturer").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.active && e.campus_id == iGVCampusID && e.function.name.ToLower() == "lecturer").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new EmployeeView() { Id = item.EmployeeId, EmployeeCode = item.employee_code + "-" + item.first_name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramShiftView> GetOrganizationShiftList()
        {
            List<DLL.Models.OrganizationProgramShift> mView = null;
            List<OrganizationProgramShiftView> lView = new List<OrganizationProgramShiftView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_shift.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramShiftView() { Id = item.Id, ProgramShiftName = item.ProgramShiftName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramShift>();
                }
            }

            return lView;
        }


        public static string GetOrganizationCampusString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string campus_list = "";
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            campus_list += c.Id + ":" + c.CampusCode + "-" + c.CampusTitle.Replace(",", "") + ",";
                        }

                        campus_list = campus_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return campus_list.Replace("\r\n", "");
        }

        public static string GetOrganizationProgramString()
        {
            string program_list = "";
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            program_list += c.Id + ":" + c.ProgramCode + "-" + c.ProgramTitle.Replace(",", "") + ",";
                        }

                        program_list = program_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return program_list.Replace("\r\n", "");
        }


        public static string GetOrganizationShiftString()
        {
            string program_list = "";
            List<DLL.Models.OrganizationProgramShift> mView = null;
            List<OrganizationProgramShiftView> lView = new List<OrganizationProgramShiftView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_shift.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            program_list += c.Id + ":" + c.ProgramShiftName.Replace(",", "") + ",";
                        }

                        program_list = program_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramShift>();
                }
            }

            return program_list.Replace("\r\n", "");
        }

        public static string GetOrganizationRoomString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string room_list = "";
            List<DLL.Models.OrganizationCampusBuildingRoom> mView = null;
            List<OrganizationCampusBuildingRoomView> lView = new List<OrganizationCampusBuildingRoomView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus_building_room.ToList();

                        if (mView != null && mView.Count > 0)
                        {
                            foreach (var item in mView)
                            {
                                lView.Add(new OrganizationCampusBuildingRoomView() { Id = item.Id, RoomCode = item.RoomCode, RoomTitle = item.RoomTitle });
                            }
                        }
                    }
                    else
                    {
                        lView = (from r in db.organization_campus_building_room
                                 join b in db.organization_campus_building on r.BuildingId equals b.Id
                                 join c in db.organization_campus on b.CampusId equals c.Id
                                 where c.Id == iGVCampusID
                                 select new OrganizationCampusBuildingRoomView { Id = r.Id, RoomCode = r.RoomCode, RoomTitle = r.RoomTitle }).ToList();
                    }

                    if (lView != null && lView.Count > 0)
                    {
                        foreach (var r in lView)
                        {
                            room_list += r.Id + ":" + r.RoomCode + "-" + r.RoomTitle.Replace(",", "") + ",";
                        }

                        room_list = room_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampusBuildingRoom>();
                }
            }

            return room_list.Replace("\r\n", "");
        }

        public static string GetOrganizationCourseString()
        {
            string course_list = "";
            List<DLL.Models.OrganizationProgramCourse> mView = null;
            List<OrganizationProgramCourseView> lView = new List<OrganizationProgramCourseView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_course.Where(c => c.IsActiveCourse).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            course_list += c.Id + ":" + c.CourseCode + "-" + c.CourseTitle.Replace(",", "") + ",";
                        }

                        course_list += "0" + ":" + "OFF,";
                        course_list += "-1" + ":" + "Break,";
                        //course_list += "-2" + ":" + "Seminar,";
                        //course_list += "-3" + ":" + "Self Study,";
                        course_list = course_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramCourse>();
                }
            }

            return course_list.Replace("\r\n", "");
        }

        public static string GetOrganizationLectureGroupString()
        {
            string lg_list = "";
            List<DLL.Models.Region> mView = null;
            List<LGRegionView> lView = new List<LGRegionView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.region.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            lg_list += c.RegionId + ":" + c.name.Replace(",", "") + ",";
                        }

                        lg_list = lg_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Region>();
                }
            }

            return lg_list.Replace("\r\n", "");
        }

        public static string GetOrganizationEmployeeTeacherString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string teacher_list = "";
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "lecturer").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.active && e.campus_id == iGVCampusID && e.function.name.ToLower() == "lecturer").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            teacher_list += c.EmployeeId + ":" + c.employee_code.Replace(",", "") + "-" + c.first_name + ",";
                        }

                        teacher_list = teacher_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return teacher_list.Replace("\r\n", "");
        }

        public static int CreateOrganizationSchedule(OrganizationCampusRoomCourseScheduleView toCreate)
        {
            int response = 0;
            OrganizationCampusRoomCourseSchedule shModel = new OrganizationCampusRoomCourseSchedule();

            try
            {
                using (Context db = new Context())
                {
                    var already_schedule = db.organization_campus_room_course_schedule.Where(c => c.RoomId == toCreate.RoomId && (c.StartTime == toCreate.StartTime && c.EndTime == toCreate.EndTime)).FirstOrDefault();
                    if (already_schedule == null)
                    {
                        shModel.CampusId = toCreate.CampusId;
                        shModel.RoomId = toCreate.RoomId;
                        shModel.ProgramId = toCreate.ProgramId;
                        shModel.ShiftId = toCreate.ShiftId;
                        shModel.LectureGroupId = toCreate.LectureGroupId;
                        shModel.CourseId = toCreate.CourseId;
                        shModel.StudyTitle = toCreate.StudyTitle;
                        shModel.StartTime = toCreate.StartTime;
                        shModel.EndTime = toCreate.EndTime;
                        shModel.EmployeeTeacherId = toCreate.EmployeeTeacherId;
                        shModel.CreateDateSch = toCreate.CreateDateSch;

                        db.organization_campus_room_course_schedule.Add(shModel);
                        db.SaveChanges();

                        response = shModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationSchedule(OrganizationCampusRoomCourseScheduleView toUpdate)
        {
            int response = 0;
            OrganizationCampusRoomCourseSchedule shModel = null;

            try
            {
                using (Context db = new Context())
                {
                    if (toUpdate.Id > 0)
                    {
                        shModel = db.organization_campus_room_course_schedule.Find(toUpdate.Id);

                        shModel.CampusId = toUpdate.CampusId;
                        shModel.RoomId = toUpdate.RoomId;
                        shModel.ProgramId = toUpdate.ProgramId;
                        shModel.ShiftId = toUpdate.ShiftId;
                        shModel.LectureGroupId = toUpdate.LectureGroupId;
                        shModel.CourseId = toUpdate.CourseId;
                        shModel.StartTime = toUpdate.StartTime;
                        shModel.EndTime = toUpdate.EndTime;
                        shModel.StudyTitle = toUpdate.StudyTitle;
                        shModel.EmployeeTeacherId = toUpdate.EmployeeTeacherId;
                        //shModel.CreateDateSch = toUpdate.CreateDateSch;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationCampusRoomCourseScheduleView RemoveOrganizationSchedule(ViewModels.OrganizationCampusRoomCourseScheduleView toRemove)
        {
            using (Context db = new Context())
            {
                OrganizationCampusRoomCourseSchedule toRemoveModel = db.organization_campus_room_course_schedule.Find(toRemove.Id);

                db.organization_campus_room_course_schedule.Remove(toRemoveModel);
                //toRemoveModel.IsActive = false;

                db.SaveChanges();

                return toRemove;
            }
        }

        public static List<ViewModels.OrganizationCampusRoomCourseScheduleView> getOrganizationCampusRoomCourseScheduleByUserCode(string user_code)
        {
            List<ViewModels.OrganizationCampusRoomCourseScheduleView> toReturn = new List<ViewModels.OrganizationCampusRoomCourseScheduleView>();

            using (Context db = new Context())
            {
                List<OrganizationCampusBuilding> oBuilding = new List<OrganizationCampusBuilding>();
                List<Terminals> oTerminal = new List<Terminals>();
                List<OrganizationCampusRoomCourseSchedule> oSchedule = new List<OrganizationCampusRoomCourseSchedule>();

                try
                {
                    //string building_list = "", terminal_list = "";
                    //int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;



                    ////MISSING org_id using employee record
                    //int sch_id = 1;

                    oSchedule = db.organization_campus_room_course_schedule.ToList();

                    if (oSchedule != null && oSchedule.Count > 0)
                    {
                        for (int i = 0; i < oSchedule.Count(); i++)
                        {
                            int cm_id = oSchedule[i].CampusId;
                            var tb_cmps = db.organization_campus.Where(e => e.Id == cm_id).FirstOrDefault();
                            string cm_name = tb_cmps != null ? tb_cmps.CampusCode : "";

                            int rm_id = oSchedule[i].RoomId;
                            var tb_room = db.organization_campus_building_room.Where(e => e.Id == rm_id).FirstOrDefault();
                            string rm_code = tb_room != null ? tb_room.RoomCode : "";
                            string rm_title = tb_room != null ? tb_room.RoomTitle : "";

                            int pr_id = oSchedule[i].ProgramId;
                            var tb_prog = db.organization_program.Where(e => e.Id == pr_id).FirstOrDefault();
                            string pr_name = tb_prog != null ? tb_prog.ProgramCode : "";

                            int sh_id = oSchedule[i].ShiftId;
                            var tb_shift = db.organization_program_shift.Where(e => e.Id == sh_id).FirstOrDefault();
                            string sh_name = tb_shift != null ? tb_shift.ProgramShiftName : "";

                            int lg_id = oSchedule[i].LectureGroupId;
                            var tb_lg = db.region.Where(e => e.RegionId == lg_id).FirstOrDefault();
                            string lg_name = tb_lg != null ? tb_lg.name : "";

                            int cr_id = oSchedule[i].CourseId;
                            var tb_course = db.organization_program_course.Where(e => e.Id == cr_id).FirstOrDefault();
                            string cr_code = tb_course != null ? tb_course.CourseCode : "";
                            string cr_title = tb_course != null ? tb_course.CourseTitle : "";

                            int et_id = oSchedule[i].EmployeeTeacherId;
                            var tb_teacher = db.employee.Where(e => e.EmployeeId == et_id).FirstOrDefault();
                            string et_name = tb_teacher != null ? tb_teacher.employee_code : "";

                            toReturn.Add(new ViewModels.OrganizationCampusRoomCourseScheduleView()
                            {
                                Id = oSchedule[i].Id,
                                RoomId = oSchedule[i].RoomId,
                                RoomCode = rm_code,
                                ShiftName = sh_name,
                                RoomTitle = rm_title,
                                CourseCode = cr_code,
                                CourseTitle = cr_title,
                                LectureGroupId = oSchedule[i].LectureGroupId,
                                LectureGroupName = lg_name,
                                StudyTitle = oSchedule[i].StudyTitle,
                                StartTime = oSchedule[i].StartTime,
                                StartTimeText = oSchedule[i].StartTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                EndTime = oSchedule[i].EndTime,
                                EndTimeText = oSchedule[i].EndTime.ToString("dd-MMM-yyyy hh:mm tt"),
                                EmployeeTeacherId = oSchedule[i].EmployeeTeacherId,
                                ProgramId = oSchedule[i].ProgramId,
                                CampusId = oSchedule[i].CampusId,
                                EmployeeTeacherName = et_name,
                                ProgramCode = pr_name,
                                CampusCode = cm_name,
                                CreateDateSchText = oSchedule[i].CreateDateSch.ToString("dd-MMM-yyyy"),
                                actions =
                                    "<span data-row='" + oSchedule[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationSchedule(" + oSchedule[i].Id + "," + oSchedule[i].CampusId + "," + oSchedule[i].RoomId + "," + oSchedule[i].ProgramId + "," + oSchedule[i].ShiftId + "," + oSchedule[i].LectureGroupId + "," + oSchedule[i].CourseId + ",'" + oSchedule[i].StudyTitle + "','" + oSchedule[i].StartTime + "','" + oSchedule[i].EndTime + "'," + oSchedule[i].EmployeeTeacherId + "));\">Edit</a>" +
                                    "<span> / </span>" +
                                    "<a href=\"javascript:void(deleteOrganizationSchedule(" + oSchedule[i].Id + "));\">Delete</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oSchedule = new List<OrganizationCampusRoomCourseSchedule>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCampusRoomCourseScheduleView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCampusRoomCourseScheduleView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCampusRoomCourseScheduleView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCampusRoomCourseScheduleView> FilterResult(string search, List<ViewModels.OrganizationCampusRoomCourseScheduleView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCampusRoomCourseScheduleView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CampusCode != null && p.CampusCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCode != null && p.ProgramCode.ToLower().Contains(search.ToLower()) ||
                         p.RoomTitle != null && p.RoomTitle.ToLower().Contains(search.ToLower()) ||
                         p.RoomCode != null && p.RoomCode.ToLower().Contains(search.ToLower()) ||
                         p.CourseCode != null && p.CourseCode.ToString().ToLower().Contains(search.ToLower()) ||
                         p.CourseTitle != null && p.CourseTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.LectureGroupName != null && p.LectureGroupName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.StudyTitle != null && p.StudyTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.ShiftName != null && p.ShiftName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.StartTimeText != null && p.StartTimeText.ToString().ToLower().Contains(search.ToLower()) ||
                         p.EndTimeText != null && p.EndTimeText.ToString().ToLower().Contains(search.ToLower())
                    )
                )
           );

            return results;
        }

    }

    public class ManageStudentsScheduleImportExport
    {
        public class ManageEmployeeCSV
        {
            public string employeeCode { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobileNo { get; set; }
            public string dateOfJoining { get; set; }
            public string dateOfLeaving { get; set; }
            public string accessGroup { get; set; }
            public int departmentID { get; set; }
            public int designationID { get; set; }
            public int functionID { get; set; }
            public int gradeID { get; set; }
            //public int groupID { get; set; }
            public int locationID { get; set; }
            public int regionID { get; set; }
            public int typeOfEmploymentID { get; set; }
            public bool active { get; set; }
        }
        public class EmpExportTable
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string employee_code { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobile_no { get; set; }
            public DateTime? date_of_joining { get; set; }
            public DateTime? date_of_leaving { get; set; }
            public string access_group { get; set; }
            public int department_DepartmentId { get; set; }
            public int designation_DesignationId { get; set; }
            public int function_FunctionId { get; set; }
            public int grade_GradeId { get; set; }
            //public int Group_GroupId { get; set; }
            public int location_LocationId { get; set; }
            public int region_RegionId { get; set; }
            public int type_of_employment_TypeOfEmploymentId { get; set; }
            public bool active { get; set; }
        }

        public static bool validateCampusCode(string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole)
                    {
                        isValid = true;
                    }
                    else
                    {
                        strCampusCode = strCampusCode.ToLower();

                        var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                        var dbEmployee = db.employee.Where(c => c.campus_id == iCampusID).FirstOrDefault();

                        if (dbCampus != null && dbEmployee != null)
                        {
                            if (dbCampus.Id == dbEmployee.campus_id)
                                isValid = true;
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateRoomCode(string strCampusCode, string strRoomCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();
                    strRoomCode = strRoomCode.ToLower();

                    var dbCampusRoom = (from r in db.organization_campus_building_room
                                        join b in db.organization_campus_building on r.BuildingId equals b.Id
                                        join c in db.organization_campus on b.CampusId equals c.Id
                                        where c.CampusCode.ToLower() == strCampusCode && r.RoomCode.ToLower() == strRoomCode
                                        select new { cr_code = c.CampusCode + "-" + r.RoomCode }).FirstOrDefault();

                    //var dbCampusRoom = db.organization_campus_building_room.Where(c => c.RoomCode == strRoomCode).FirstOrDefault();
                    if (dbCampusRoom != null)
                    {
                        if (dbCampusRoom.cr_code != null && dbCampusRoom.cr_code != "")
                            isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }


        public static bool validateProgramCode(string strProgramCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strProgramCode = strProgramCode.ToLower();

                    var dbProgram = db.organization_program.Where(c => c.ProgramCode == strProgramCode).FirstOrDefault();
                    if (dbProgram != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }



        public static bool validateCourseCode(string strCourseCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCourseCode = strCourseCode.ToLower();

                    var dbCourse = db.organization_program_course.Where(c => c.CourseCode == strCourseCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateLecturerCode(string strCampusCode, string strLecturerCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();
                    strLecturerCode = strLecturerCode.ToLower();

                    var dbCampusEmp = (from e in db.employee
                                       join f in db.function on e.function.FunctionId equals f.FunctionId
                                       join c in db.organization_campus on e.campus_id equals c.Id
                                       where e.employee_code.ToLower() == strLecturerCode && f.name.ToLower() == "lecturer" && c.CampusCode.ToLower() == strCampusCode
                                       select new { lr_code = e.employee_code }).FirstOrDefault();

                    //var dbCourse = db.employee.Where(c => c.function.name.ToLower() == "lecturer" && c.employee_code == strLecturerCode).FirstOrDefault();
                    if (dbCampusEmp != null)
                    {
                        if (dbCampusEmp.lr_code != null && dbCampusEmp.lr_code != "")
                            isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }



        public static List<ManageEmployeeCSV> getManageStudentsScheduleCSVDownload()
        {
            List<ManageEmployeeCSV> toReturn;
            using (var db = new Context())
            {

                try
                {


                    toReturn =
                        db.employee.Where(m => m.active).Select(
                        p => new ManageEmployeeCSV()
                        {
                            employeeCode = p.employee_code,
                            firstName = (p.first_name == null) ? "" : p.first_name,
                            lastName = (p.last_name == null) ? "" : p.last_name,
                            email = (p.email == null) ? "" : p.email,
                            address = (p.address == null) ? "" : p.address,
                            mobileNo = (p.mobile_no == null) ? "" : p.mobile_no,
                            dateOfJoining = p.date_of_joining.ToString(),
                            dateOfLeaving = p.date_of_leaving.ToString(),
                            //accessGroupID=p.access_group_AccessGroupId,
                            accessGroup = p.access_group.name,
                            departmentID = (p.department == null) ? 0 : p.department.DepartmentId,
                            designationID = (p.designation == null) ? 0 : p.designation.DesignationId,
                            functionID = (p.function == null) ? 0 : p.function.FunctionId,
                            gradeID = (p.grade == null) ? 0 : p.grade.GradeId,
                            //groupID=p.Group_GroupId,
                            locationID = (p.location == null) ? 0 : p.location.LocationId,
                            regionID = (p.region == null) ? 0 : p.region.RegionId,
                            typeOfEmploymentID = (p.type_of_employment == null) ? 0 : p.type_of_employment.TypeOfEmploymentId,
                            active = p.timetune_active
                        }).ToList();


                    // convert dates to a more processable form.
                    foreach (var value in toReturn)
                    {
                        if (value.dateOfJoining != null && value.dateOfJoining != "")
                        {
                            DateTime dateOfJoining = DateTime.Parse(value.dateOfJoining);
                            value.dateOfJoining = dateOfJoining.ToString("yyyy-MM-dd");
                        }

                        if (value.dateOfLeaving != null && value.dateOfLeaving != "")
                        {
                            DateTime dateOfLeaving = DateTime.Parse(value.dateOfLeaving);
                            value.dateOfLeaving = dateOfLeaving.ToString("yyyy-MM-dd");
                        }

                    }
                    /*string sql =
                         "select\n" +
                          "[first_name]\n" +
                          ",[last_name]\n" +
                          ",[employee_code]\n" +
                          ",[email]\n" +
                          ",[address]\n" +
                          ",[mobile_no]\n" +
                          ",[date_of_joining]\n" +
                          ",[date_of_leaving]\n" +
                          ",COALESCE([access_group_AccessGroupId], 0) as [access_group_AccessGroupId]\n" +
                          ",COALESCE([department_DepartmentId], 0) as [department_DepartmentId]\n" +
                          ",COALESCE([designation_DesignationId], 0) as [designation_DesignationId]\n" +
                          ",COALESCE([function_FunctionId], 0) as [function_FunctionId]\n" +
                          ",COALESCE([grade_GradeId], 0) as [grade_GradeId]\n" +
                          ",COALESCE([Group_GroupId], 0) as [Group_GroupId]\n" +
                          ",COALESCE([location_LocationId], 0) as [location_LocationId]\n" +
                          ",COALESCE([region_RegionId], 0) as [region_RegionId]\n" +
                          ",COALESCE([type_of_employment_TypeOfEmploymentId], 0) as [type_of_employment_TypeOfEmploymentId]\n" +
                           ",[timetune_active]\n" +
                        "from\n" +
                        "Employees where [active]=1 \n";*/

                    //var results = db.Database.SqlQuery<EmpExportTable>(sql).ToList();


                    /*

                    int count = results.Count();

                    toReturn = results.Select(
                        p => new ManageEmployeeCSV()
                        {
                            employeeCode=p.employee_code,
                            firstName = (p.first_name == null) ? "" : p.first_name,
                            lastName=(p.last_name == null) ? "" : p.last_name,
                            email=(p.email == null) ? "" : p.email,
                            address=(p.address==null)?"":p.address,
                            mobileNo = (p.mobile_no == null) ? "" : p.mobile_no,
                            dateOfJoining=p.date_of_joining.ToString(),
                            dateOfLeaving=p.date_of_leaving.ToString(),
                            //accessGroupID=p.access_group_AccessGroupId,
                           departmentID=p.department_DepartmentId,
                           designationID=p.designation_DesignationId,
                           functionID=p.function_FunctionId,
                           gradeID=p.grade_GradeId,
                           //groupID=p.Group_GroupId,
                           locationID=p.location_LocationId,
                           regionID=p.region_RegionId,
                           typeOfEmploymentID=p.type_of_employment_TypeOfEmploymentId,
                           active=p.active
                        }).ToList();
                     * 
                     * */
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ManageEmployeeCSV>();
                }

            }

            return toReturn;
        }
        public class ManageContractualStaffCSV
        {
            public string employeeCode { get; set; }
            public string employeeName { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobileNo { get; set; }
            public string dateOfJoining { get; set; }
            public string dateOfLeaving { get; set; }
            public string company { get; set; }
            public string department { get; set; }
            public string designation { get; set; }
            public string function { get; set; }
            public string grade { get; set; }
            //public int groupID { get; set; }
            public string location { get; set; }
            public string region { get; set; }
            public bool active { get; set; }
        }

        private static bool convertStringToBool(string value)
        {
            return (value == "True" || value == "true" || value == "TRUE");
        }

        public static string setSchedue(List<string> csvContents, string user_code)
        {
            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 11 values (columns).

                        if (values == null || values.Length < 11)
                            continue;

                        // Remove unwanted " characters.
                        string strDate = values[0].Replace("\"", "");//date
                        string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        if (strProperDate != "")
                        {
                            strDate = strProperDate;
                        }

                        string strStTime = values[1].Replace("\"", "");//time_in
                        string strEnTime = values[2].Replace("\"", "");//time_en
                        string strCampusCode = values[3].Replace("\"", "");//campus
                        string strRoomCode = values[4].Replace("\"", "");//room
                        string strProgramCode = values[5].Replace("\"", "");//program
                        string strPShift = values[6].Replace("\"", "");//shift
                        string strLGroup = values[7].Replace("\"", "");//group
                        string strCourseCode = values[8].Replace("\"", "");//course
                        string strStudyTitle = values[9].Replace("\"", "");//study
                        string strLecturerCode = values[10].Replace("\"", "");//lecturer

                        DateTime dtStartDateTime = Convert.ToDateTime(Convert.ToDateTime(strDate + " " + strStTime).ToString("yyyy-MM-dd HH:mm:00.000"));
                        DateTime dtEndDateTime = Convert.ToDateTime(Convert.ToDateTime(strDate + " " + strEnTime).ToString("yyyy-MM-dd HH:mm:00.000"));

                        //-----------------------------------------------
                        int iCampusId = 0; strCampusCode = strCampusCode.ToLower();
                        var dbCampus = db.organization_campus.Where(r => r.CampusCode.ToLower() == strCampusCode).FirstOrDefault();
                        iCampusId = dbCampus != null ? dbCampus.Id : 0;

                        //-----------------------------------------------
                        int iRoomId = 0; strRoomCode = strRoomCode.ToLower();
                        var dbRoom = db.organization_campus_building_room.Where(r => r.RoomCode != null && r.RoomCode.ToLower() == strRoomCode).FirstOrDefault();
                        iRoomId = dbRoom != null ? dbRoom.Id : 0;

                        //-----------------------------------------------
                        int iProgramId = 0; strProgramCode = strProgramCode.ToLower();
                        if (strProgramCode == "0")
                        {
                            iProgramId = 0;
                        }
                        else
                        {
                            var dbProgram = db.organization_program.Where(r => r.ProgramCode.ToLower() == strProgramCode).FirstOrDefault();
                            iProgramId = dbProgram != null ? dbProgram.Id : 0;
                        }

                        //-----------------------------------------------
                        int iPShiftId = 0; strPShift = strPShift.ToLower();
                        var dbProgramShift = db.organization_program_shift.Where(r => r.ProgramShiftName.ToLower() == strPShift).FirstOrDefault();
                        iPShiftId = dbProgramShift != null ? dbProgramShift.Id : 1;


                        //-----------------------------------------------
                        int iLGroupId = 0; strLGroup = strLGroup.ToLower();
                        if (strLGroup == "0")
                        {
                            iLGroupId = 0;
                        }
                        else
                        {
                            var dbLectureGroup = db.region.Where(r => r.name != null && r.name.ToLower() == strLGroup).FirstOrDefault();
                            iLGroupId = dbLectureGroup != null ? dbLectureGroup.RegionId : 0;
                        }

                        //-----------------------------------------------
                        int iCourseId = 0; strCourseCode = strCourseCode.ToLower();
                        if (strCourseCode == "self study")
                        {
                            iCourseId = -3;
                        }
                        else if (strCourseCode == "seminar")
                        {
                            iCourseId = -2;
                        }
                        else if (strCourseCode == "break")
                        {
                            iCourseId = -1;
                        }
                        else if (strCourseCode == "off")
                        {
                            iCourseId = 0;
                        }
                        else
                        {
                            var dbCourse = db.organization_program_course.Where(r => r.CourseCode != null && r.CourseCode.ToLower() == strCourseCode).FirstOrDefault();
                            iCourseId = dbCourse != null ? dbCourse.Id : 0;
                        }


                        //-----------------------------------------------
                        if (strStudyTitle == "0")
                        {
                            strStudyTitle = "";
                        }

                        //-----------------------------------------------
                        int iEmployeeTeacherId = 0;
                        if (strLecturerCode == "0")
                        {
                            iEmployeeTeacherId = 0;
                        }
                        else
                        {
                            var dbEmployeeTeacher = db.employee.Where(r => r.employee_code == strLecturerCode).FirstOrDefault();
                            iEmployeeTeacherId = dbEmployeeTeacher != null ? dbEmployeeTeacher.EmployeeId : 0;
                        }


                        // continue if its the first row (CSV Header line).
                        if (strDate == "date" || strStTime == "st_time")
                        {
                            continue;
                        }

                        // Get the existing schedule by room and date 
                        DLL.Models.OrganizationCampusRoomCourseSchedule schedule = db.organization_campus_room_course_schedule
                            .Where(m => m.CampusId == iCampusId && m.RoomId == iRoomId && m.ProgramId == iProgramId && m.ShiftId == iPShiftId &&
                                            m.LectureGroupId == iLGroupId && m.CourseId == iCourseId && m.EmployeeTeacherId == iEmployeeTeacherId
                                                    && m.StartTime.Equals(dtStartDateTime) && m.EndTime.Equals(dtEndDateTime)
                                ).FirstOrDefault();

                        if (schedule != null)//existing schedule
                        {
                            schedule.StartTime = dtStartDateTime;
                            schedule.EndTime = dtEndDateTime;
                            schedule.CampusId = iCampusId;
                            schedule.RoomId = iRoomId;
                            schedule.ProgramId = iProgramId;
                            schedule.ShiftId = iPShiftId;
                            schedule.LectureGroupId = iLGroupId;
                            schedule.CourseId = iCourseId;
                            schedule.StudyTitle = strStudyTitle;
                            schedule.EmployeeTeacherId = iEmployeeTeacherId;
                            schedule.CreateDateSch = DateTime.Now;

                            db.SaveChanges();

                            TimeTune.AuditTrail.update("{\"*id\" : \"" + schedule.Id.ToString() + "\"}", "OrganizationCampusRoomCourseSchedule", user_code);
                        }
                        else //new schedule
                        {
                            DLL.Models.OrganizationCampusRoomCourseSchedule sch = new DLL.Models.OrganizationCampusRoomCourseSchedule()
                            {
                                StartTime = dtStartDateTime,
                                EndTime = dtEndDateTime,
                                CampusId = iCampusId,
                                RoomId = iRoomId,
                                ProgramId = iProgramId,
                                ShiftId = iPShiftId,
                                LectureGroupId = iLGroupId,
                                CourseId = iCourseId,
                                StudyTitle = strStudyTitle,
                                EmployeeTeacherId = iEmployeeTeacherId,
                                CreateDateSch = DateTime.Now
                            };

                            db.organization_campus_room_course_schedule.Add(sch);
                            db.SaveChanges();

                            TimeTune.AuditTrail.insert("{\"*id\" : \"" + sch.Id.ToString() + "\"}", "OrganizationCampusRoomCourseSchedule", user_code);
                        }
                    }

                    return "Successful";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

    }


    #region Schedule-Report

    public class ScheduleReportLog
    {
        public int id_row { get; set; }

        public int id_col { get; set; }

        public string str_data { get; set; }

    }



    public class ScheduleReportData
    {
        public string org_title { get; set; }
        public string org_logo_path { get; set; }
        public string org_web_url { get; set; }
        public string campus_code { get; set; }
        public string program_code { get; set; }
        public string building_code { get; set; }
        public string room_code { get; set; }
        public string program_shift { get; set; }
        public string date_range { get; set; }

        public List<ScheduleReportLog> logs { get; set; }
        public int rows_count { get; set; }
        public int cols_count { get; set; }
    }

    public class ScheduleRawData
    {
        public DateTime act_date { get; set; }
        public DateTime act_st_time { get; set; }
        public DateTime act_end_time { get; set; }
        public string str_date { get; set; }
        public string str_time { get; set; }
        public string str_course { get; set; }

        public string str_stitle { get; set; }

        public int id_shift { get; set; }
    }


    public class ScheduleReport
    {
        public ScheduleReportData getScheduleData(int iCampusId, int iProgId, int iShiftId, int iRoomId, DateTime dtStart, DateTime dtEnd)
        {
            List<DLL.Models.OrganizationCampusRoomCourseSchedule> dbSchedule = null;
            List<ScheduleRawData> schedule_raw = new List<ScheduleRawData>();
            ScheduleReportData toReturn = null;

            using (var db = new Context())
            {
                dbSchedule = db.organization_campus_room_course_schedule.Where(s => s.CampusId == iCampusId && s.ProgramId == iProgId && s.ShiftId == iShiftId && s.RoomId == iRoomId && (s.StartTime >= dtStart && s.EndTime <= dtEnd)).OrderBy(o => o.StartTime).ToList();
                if (dbSchedule != null && dbSchedule.Count > 0)
                {
                    toReturn = new ScheduleReportData();
                    List<ScheduleReportLog> tempLogs = new List<ScheduleReportLog>();

                    //////////////////// Org Detail ////////////////////////////
                    /*
                    SELECT c.Id FROM[OrganizationCampusBuildingRooms] r
                    JOIN OrganizationCampusBuildings b ON r.BuildingId = b.id
                    JOIN OrganizationCampus c ON b.CampusId = c.id
                    JOIN OrganizationCampusPrograms cp ON c.Id = cp.CampusId
                    JOIN OrganizationPrograms p ON cp.ProgramId = p.Id
                    JOIN Organizations o ON c.OrganizationId = o.Id
                    where r.Id = 1 and p.Id = 2 and c.Id = 1

                    SELECT c.Id FROM Organizations o
                    JOIN OrganizationCampus c ON o.Id=c.OrganizationId
                    JOIN OrganizationCampusBuildings b ON b.CampusId=c.id
                    JOIN [OrganizationCampusBuildingRooms] r  ON r.BuildingId=b.Id
                    JOIN OrganizationCampusPrograms cp ON c.Id=cp.CampusId
                    JOIN OrganizationPrograms p ON cp.ProgramId=p.Id
                    where r.Id=1 and p.Id=2 and c.Id=3
                     */

                    var dbOrgCampus = (from r in db.organization_campus_building_room
                                       join b in db.organization_campus_building on r.BuildingId equals b.Id
                                       join c in db.organization_campus on b.CampusId equals c.Id
                                       join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                       join p in db.organization_program on cp.ProgramId equals p.Id
                                       join o in db.organization on c.OrganizationId equals o.Id
                                       where r.Id == iRoomId && c.Id == iCampusId && p.Id == iProgId && cp.IsActiveProgram
                                       select new
                                       {
                                           org_name = o.OrganizationTitle,
                                           org_logo = o.Logo,
                                           org_url = o.WebsiteURL,
                                           campus_name = c.CampusCode + "-" + c.CampusTitle,
                                           program_name = p.ProgramCode + "-" + p.ProgramTitle,
                                           building_name = b.BuildingCode + "-" + b.BuildingTitle,
                                           room_name = r.RoomCode + "-" + r.RoomTitle
                                       }).FirstOrDefault();

                    if (dbOrgCampus != null)
                    {
                        toReturn.org_title = dbOrgCampus.org_name;
                        toReturn.org_logo_path = dbOrgCampus.org_logo ?? "/Content/Logos/logo-default.png";
                        toReturn.org_web_url = dbOrgCampus.org_url ?? "";
                        toReturn.campus_code = dbOrgCampus.campus_name;
                        toReturn.program_code = dbOrgCampus.program_name;
                        toReturn.building_code = dbOrgCampus.building_name;
                        toReturn.room_code = dbOrgCampus.room_name;
                    }

                    var dbProgramShift = (from s in db.organization_program_shift
                                          where s.Id == iShiftId
                                          select new
                                          {
                                              shift_id = s.Id,
                                              shift_name = s.ProgramShiftName
                                          }).FirstOrDefault();

                    if (dbProgramShift != null)
                    {
                        toReturn.program_shift = dbProgramShift.shift_name;
                    }

                    toReturn.date_range = dtStart.ToString("ddd, dd-MMM-yyyy") + " TO " + dtEnd.ToString("ddd, dd-MMM-yyyy");

                    //////////////////// Logs Data ////////////////////////////

                    foreach (var s in dbSchedule)
                    {
                        string course_title = "";
                        if (s.CourseId == 0)
                        {
                            course_title = "OFF";
                        }
                        else if (s.CourseId == -1)
                        {
                            course_title = "BREAK";
                        }
                        else if (s.CourseId == -2)
                        {
                            course_title = "SEMINAR";
                        }
                        else if (s.CourseId == -3)
                        {
                            course_title = "SELF STUDY";
                        }
                        else
                        {
                            if (s.LectureGroupId == 0)
                            {
                                var db_course = db.organization_program_course.Where(l => l.Id == s.CourseId).FirstOrDefault();
                                if (db_course != null)
                                {
                                    course_title = db_course.CourseCode + " [" + s.StudyTitle + "]";
                                }
                            }
                            else
                            {
                                var db_lec_group = db.region.ToList();
                                if (db_lec_group != null && db_lec_group.Count > 0)
                                {
                                    foreach (var lg in db_lec_group)
                                    {
                                        course_title += getCourseANDGroupString(iCampusId, iProgId, iShiftId, lg.RegionId, s.StartTime, s.EndTime, dbSchedule, db);
                                    }
                                }
                            }
                        }

                        schedule_raw.Add(new ScheduleRawData
                        {
                            act_st_time = s.StartTime,
                            act_end_time = s.EndTime,
                            str_date = s.StartTime.ToString("dddd\r\ndd-MMM-yyyy"),
                            str_time = s.StartTime.ToString("hh:mm tt") + " - " + s.EndTime.ToString("hh:mm tt"),
                            str_course = course_title == "" ? "X" : course_title,
                            str_stitle = s.StudyTitle,
                            id_shift = s.ShiftId
                        });
                    }


                    if (schedule_raw != null && schedule_raw.Count > 0)
                    {
                        var titleRows = schedule_raw.GroupBy(g => g.str_date).ToArray();
                        var titleCols = schedule_raw.GroupBy(g => g.str_time).ToArray();

                        toReturn.rows_count = (titleRows.Length + 1);
                        toReturn.cols_count = titleCols.Length;

                        if (titleRows != null && titleRows.Length > 0 && titleCols != null && titleCols.Length > 0)
                        {
                            for (int r = 0; r <= titleRows.Length; r++)
                            {
                                for (int c = 0; c < titleCols.Length; c++)
                                {
                                    ScheduleReportLog tmp = new ScheduleReportLog();

                                    if (r == 0 && c == 0)
                                    {
                                        tmp.id_row = r;
                                        tmp.id_col = c;

                                        tmp.str_data = "Day/Time";
                                    }
                                    else if (r == 0)
                                    {
                                        tmp.id_row = r;
                                        tmp.id_col = c;

                                        int d = c - 1;
                                        tmp.str_data = titleCols[d].Key.ToString();
                                    }
                                    else if (c == 0)
                                    {
                                        tmp.id_row = r;
                                        tmp.id_col = c;

                                        int b = r - 1;
                                        tmp.str_data = titleRows[b].Key.ToString();
                                    }
                                    else
                                    {
                                        tmp.id_row = r;
                                        tmp.id_col = c;

                                        string strDate = titleRows[r - 1].Key.ToString();
                                        string strTime = titleCols[c - 1].Key.ToString();

                                        tmp.str_data = getScheduleLog(strDate, strTime, schedule_raw);
                                    }

                                    tempLogs.Add(tmp);
                                }
                            }
                        }
                    }

                    toReturn.logs = tempLogs;
                }
            }

            return toReturn;
        }

        private string getCourseANDGroupString(int iCampusId, int iProgId, int iShiftId, int iGroupId, DateTime stDate, DateTime enDate, List<DLL.Models.OrganizationCampusRoomCourseSchedule> dbScheduleList, Context db)
        {
            int course_id = 0;
            string strReturn = "", course_title = "", group_name = "";

            var dbScheduleRow = dbScheduleList.Find(l => l.CampusId == iCampusId && l.ProgramId == iProgId && l.ShiftId == iShiftId && l.LectureGroupId == iGroupId && l.StartTime == stDate && l.EndTime == enDate);
            if (dbScheduleRow != null)
            {
                course_id = dbScheduleRow.CourseId;
                if (course_id > 0)
                {
                    var dbCourse = db.organization_program_course.Where(l => l.Id == course_id).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        course_title = dbCourse.CourseCode;
                    }

                    ///////////////////////////////////////

                    var dbLGroup = db.region.Where(l => l.RegionId == iGroupId).FirstOrDefault();
                    if (dbLGroup != null)
                    {
                        group_name = dbLGroup.name;
                    }

                    if (group_name != "")
                    {
                        strReturn = (course_title + " (Gp. " + group_name + ")" + " [" + dbScheduleRow.StudyTitle + "]\r\n");
                    }
                    else
                    {
                        strReturn = course_title + " [" + dbScheduleRow.StudyTitle + "]\r\n";
                    }
                }
            }

            ////////////////////////////////////

            return strReturn;
        }

        private string getScheduleLog(string rDate, string cTime, List<ScheduleRawData> schedule_raw)
        {
            string strReturn = "";

            var f = schedule_raw.Find(s => s.str_date == rDate && s.str_time == cTime);
            if (f != null)
            {
                strReturn = f.str_course;
            }

            return strReturn;
        }

        public ScheduleReportData getScheduleDataForStudent(int iCampusId, int iStudentId, DateTime dtStart, DateTime dtEnd)
        {
            List<DLL.Models.OrganizationCampusRoomCourseSchedule> dbSchedule = null;
            List<ScheduleRawData> schedule_raw = new List<ScheduleRawData>();
            ScheduleReportData toReturn = null;

            using (var db = new Context())
            {
                //find general-calenar-year using date
                int iStYear = dtStart.Year, iProgId = 0;
                var dbGCYear = db.general_calender.Where(g => g.year == iStYear).FirstOrDefault();
                if (dbGCYear != null)
                {
                    var dbEnrollment = db.organization_program_course_enrollment.Where(e => e.GeneralCalendarId == dbGCYear.GeneralCalendarId &&
                                                    e.EmployeeStudentId == iStudentId).FirstOrDefault();
                    if (dbEnrollment != null)
                    {
                        iProgId = dbEnrollment.ProgramId;

                        if (iProgId > 0)
                        {
                            dbSchedule = db.organization_campus_room_course_schedule.Where(s => s.CampusId == iCampusId && s.ProgramId == iProgId && (s.StartTime >= dtStart && s.EndTime <= dtEnd)).OrderBy(o => o.StartTime).ToList();
                            if (dbSchedule != null && dbSchedule.Count > 0)
                            {
                                toReturn = new ScheduleReportData();
                                List<ScheduleReportLog> tempLogs = new List<ScheduleReportLog>();

                                //////////////////// Org Detail ////////////////////////////
                                /*
                                SELECT c.Id FROM[OrganizationCampusBuildingRooms] r
                                JOIN OrganizationCampusBuildings b ON r.BuildingId = b.id
                                JOIN OrganizationCampus c ON b.CampusId = c.id
                                JOIN OrganizationCampusPrograms cp ON c.Id = cp.CampusId
                                JOIN OrganizationPrograms p ON cp.ProgramId = p.Id
                                JOIN Organizations o ON c.OrganizationId = o.Id
                                where r.Id = 1 and p.Id = 2 and c.Id = 1

                                SELECT c.Id FROM Organizations o
                                JOIN OrganizationCampus c ON o.Id=c.OrganizationId
                                JOIN OrganizationCampusBuildings b ON b.CampusId=c.id
                                JOIN [OrganizationCampusBuildingRooms] r  ON r.BuildingId=b.Id
                                JOIN OrganizationCampusPrograms cp ON c.Id=cp.CampusId
                                JOIN OrganizationPrograms p ON cp.ProgramId=p.Id
                                where r.Id=1 and p.Id=2 and c.Id=3
                                 */

                                var dbOrgCampus = (from r in db.organization_campus_building_room
                                                   join b in db.organization_campus_building on r.BuildingId equals b.Id
                                                   join c in db.organization_campus on b.CampusId equals c.Id
                                                   join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                                   join p in db.organization_program on cp.ProgramId equals p.Id
                                                   join o in db.organization on c.OrganizationId equals o.Id
                                                   where c.Id == iCampusId && p.Id == iProgId && cp.IsActiveProgram
                                                   select new
                                                   {
                                                       org_name = o.OrganizationTitle,
                                                       org_logo = o.Logo,
                                                       org_url = o.WebsiteURL,
                                                       campus_name = c.CampusCode + "-" + c.CampusTitle,
                                                       program_name = p.ProgramCode + "-" + p.ProgramTitle,
                                                       building_name = b.BuildingCode + "-" + b.BuildingTitle,
                                                       room_name = r.RoomCode + "-" + r.RoomTitle
                                                   }).FirstOrDefault();

                                if (dbOrgCampus != null)
                                {
                                    toReturn.org_title = dbOrgCampus.org_name;
                                    toReturn.org_logo_path = dbOrgCampus.org_logo ?? "/Content/Logos/logo-default.png";
                                    toReturn.org_web_url = dbOrgCampus.org_url ?? "";
                                    toReturn.campus_code = dbOrgCampus.campus_name;
                                    toReturn.program_code = dbOrgCampus.program_name;
                                    toReturn.building_code = dbOrgCampus.building_name;
                                    toReturn.room_code = dbOrgCampus.room_name;
                                }

                                //var dbProgramShift = (from s in db.organization_program_shift
                                //                      where s.Id == iShiftId
                                //                      select new
                                //                      {
                                //                          shift_id = s.Id,
                                //                          shift_name = s.ProgramShiftName
                                //                      }).FirstOrDefault();

                                //if (dbProgramShift != null)
                                //{
                                //    toReturn.program_shift = dbProgramShift.shift_name;
                                //}

                                toReturn.date_range = dtStart.ToString("ddd, dd-MMM-yyyy") + " TO " + dtEnd.ToString("ddd, dd-MMM-yyyy");

                                //////////////////// Logs Data ////////////////////////////

                                foreach (var s in dbSchedule)
                                {
                                    string course_title = "";
                                    if (s.CourseId == 0)
                                    {
                                        course_title = "OFF";
                                    }
                                    else if (s.CourseId == -1)
                                    {
                                        course_title = "BREAK";
                                    }
                                    else if (s.CourseId == -2)
                                    {
                                        course_title = "SEMINAR";
                                    }
                                    else if (s.CourseId == -3)
                                    {
                                        course_title = "SELF STUDY";
                                    }
                                    else
                                    {
                                        if (s.LectureGroupId == 0)
                                        {
                                            var db_course = db.organization_program_course.Where(l => l.Id == s.CourseId).FirstOrDefault();
                                            if (db_course != null)
                                            {
                                                course_title = db_course.CourseCode + " [" + s.StudyTitle + "]";
                                            }
                                        }
                                        else
                                        {
                                            var db_lec_group = db.region.ToList();
                                            if (db_lec_group != null && db_lec_group.Count > 0)
                                            {
                                                foreach (var lg in db_lec_group)
                                                {
                                                    course_title += getCourseANDGroupStringForStudent(iCampusId, iProgId, lg.RegionId, s.StartTime, s.EndTime, dbSchedule, db);
                                                }
                                            }
                                        }
                                    }

                                    schedule_raw.Add(new ScheduleRawData
                                    {
                                        act_st_time = s.StartTime,
                                        act_end_time = s.EndTime,
                                        str_date = s.StartTime.ToString("dddd\r\ndd-MMM-yyyy"),
                                        str_time = s.StartTime.ToString("hh:mm tt") + " - " + s.EndTime.ToString("hh:mm tt"),
                                        str_course = course_title == "" ? "X" : course_title,
                                        str_stitle = s.StudyTitle,
                                        id_shift = s.ShiftId
                                    });
                                }


                                if (schedule_raw != null && schedule_raw.Count > 0)
                                {
                                    var titleRows = schedule_raw.GroupBy(g => g.str_date).ToArray();
                                    var titleCols = schedule_raw.GroupBy(g => g.str_time).ToArray();

                                    toReturn.rows_count = (titleRows.Length + 1);
                                    toReturn.cols_count = titleCols.Length;

                                    if (titleRows != null && titleRows.Length > 0 && titleCols != null && titleCols.Length > 0)
                                    {
                                        for (int r = 0; r <= titleRows.Length; r++)
                                        {
                                            for (int c = 0; c < titleCols.Length; c++)
                                            {
                                                ScheduleReportLog tmp = new ScheduleReportLog();

                                                if (r == 0 && c == 0)
                                                {
                                                    tmp.id_row = r;
                                                    tmp.id_col = c;

                                                    tmp.str_data = "Day/Time";
                                                }
                                                else if (r == 0)
                                                {
                                                    tmp.id_row = r;
                                                    tmp.id_col = c;

                                                    int d = c - 1;
                                                    tmp.str_data = titleCols[d].Key.ToString();
                                                }
                                                else if (c == 0)
                                                {
                                                    tmp.id_row = r;
                                                    tmp.id_col = c;

                                                    int b = r - 1;
                                                    tmp.str_data = titleRows[b].Key.ToString();
                                                }
                                                else
                                                {
                                                    tmp.id_row = r;
                                                    tmp.id_col = c;

                                                    string strDate = titleRows[r - 1].Key.ToString();
                                                    string strTime = titleCols[c - 1].Key.ToString();

                                                    tmp.str_data = getScheduleLogForStudent(strDate, strTime, schedule_raw);
                                                }

                                                tempLogs.Add(tmp);
                                            }
                                        }
                                    }
                                }

                                toReturn.logs = tempLogs;
                            }
                        }
                    }
                }
            }

            return toReturn;
        }

        private string getCourseANDGroupStringForStudent(int iCampusId, int iProgId, int iGroupId, DateTime stDate, DateTime enDate, List<DLL.Models.OrganizationCampusRoomCourseSchedule> dbScheduleList, Context db)
        {
            int course_id = 0;
            string strReturn = "", course_title = "", group_name = "";

            var dbScheduleRow = dbScheduleList.Find(l => l.CampusId == iCampusId && l.ProgramId == iProgId && l.LectureGroupId == iGroupId && l.StartTime == stDate && l.EndTime == enDate);
            if (dbScheduleRow != null)
            {
                course_id = dbScheduleRow.CourseId;
                if (course_id > 0)
                {
                    var dbCourse = db.organization_program_course.Where(l => l.Id == course_id).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        course_title = dbCourse.CourseCode;
                    }

                    ///////////////////////////////////////

                    var dbLGroup = db.region.Where(l => l.RegionId == iGroupId).FirstOrDefault();
                    if (dbLGroup != null)
                    {
                        group_name = dbLGroup.name;
                    }

                    if (group_name != "")
                    {
                        strReturn = (course_title + " (Gp. " + group_name + ")" + " [" + dbScheduleRow.StudyTitle + "]\r\n");
                    }
                    else
                    {
                        strReturn = course_title + " [" + dbScheduleRow.StudyTitle + "]\r\n";
                    }
                }
            }

            ////////////////////////////////////

            return strReturn;
        }

        private string getScheduleLogForStudent(string rDate, string cTime, List<ScheduleRawData> schedule_raw)
        {
            string strReturn = "";

            var f = schedule_raw.Find(s => s.str_date == rDate && s.str_time == cTime);
            if (f != null)
            {
                strReturn = f.str_course;
            }

            return strReturn;
        }




    }

    #endregion

    #endregion


    #region Organization-Programs

    public class OrganizationProgramResultSet
    {
        public static List<OrganizationProgramCategoryView> GetOrganizationProgramCategoryList()
        {
            List<DLL.Models.OrganizationProgramCategory> mView = null;
            List<OrganizationProgramCategoryView> lView = new List<OrganizationProgramCategoryView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_category.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramCategoryView() { Id = item.Id, ProgramCategoryName = item.ProgramCategoryName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramCategory>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramTypeView> GetOrganizationProgramTypeList()
        {
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramTypeView() { Id = item.Id, ProgramTypeName = item.ProgramTypeName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return lView;
        }


        public static string GetOrganizationProgramCategoryString()
        {
            string category_list = "";
            List<DLL.Models.OrganizationProgramCategory> mView = null;
            List<OrganizationProgramCategoryView> lView = new List<OrganizationProgramCategoryView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_category.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            category_list += c.Id + ":" + c.ProgramCategoryName.Replace(",", "") + ",";
                        }

                        category_list = category_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramCategory>();
                }
            }

            return category_list.Replace("\r\n", "");
        }

        public static string GetOrganizationProgramTypeString()
        {
            string type_list = "";
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            type_list += c.Id + ":" + c.ProgramTypeName.Replace(",", "") + ",";
                        }

                        type_list = type_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return type_list.Replace("\r\n", "");
        }


        public static int CreateOrganizationProgram(OrganizationProgramView toCreate)
        {
            int response = 0;
            OrganizationProgram pgModel = new OrganizationProgram();

            try
            {
                using (Context db = new Context())
                {
                    var already_program = db.organization_program.Where(c => c.ProgramCode != null && c.ProgramCode.ToLower() == toCreate.ProgramCode.ToLower()).FirstOrDefault();
                    if (already_program == null)
                    {
                        pgModel.CategoryId = toCreate.CategoryId;
                        pgModel.ProgramCode = toCreate.ProgramCode;
                        pgModel.ProgramTitle = toCreate.ProgramTitle;
                        pgModel.DisciplineName = toCreate.DisciplineName;
                        pgModel.CreditHours = toCreate.CreditHours;
                        pgModel.WholeProgramTypeId = toCreate.WholeProgramTypeId;
                        pgModel.WholeProgramTypeNumber = toCreate.WholeProgramTypeNumber;
                        pgModel.CreateDatePrg = toCreate.CreateDatePrg;

                        db.organization_program.Add(pgModel);
                        db.SaveChanges();

                        response = pgModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationProgram(OrganizationProgramView toUpdate)
        {
            int response = 0;
            OrganizationProgram pgModel = null;

            try
            {
                using (Context db = new Context())
                {
                    var already_exists = db.organization_program.Where(c => c.Id != toUpdate.Id && (c.ProgramCode != null && c.ProgramCode.ToLower() == toUpdate.ProgramCode.ToLower())).FirstOrDefault();
                    if (already_exists != null)
                    {
                        //another same campus-code already there in table
                        response = 0;
                    }
                    else
                    {
                        pgModel = db.organization_program.Find(toUpdate.Id);

                        //pgModel.OrganizationId = toUpdate.OrganizationId;
                        pgModel.CategoryId = toUpdate.CategoryId;
                        pgModel.ProgramCode = toUpdate.ProgramCode;
                        pgModel.ProgramTitle = toUpdate.ProgramTitle;
                        pgModel.DisciplineName = toUpdate.DisciplineName;
                        pgModel.CreditHours = toUpdate.CreditHours;
                        pgModel.WholeProgramTypeId = toUpdate.WholeProgramTypeId;
                        pgModel.WholeProgramTypeNumber = toUpdate.WholeProgramTypeNumber;
                        //pgModel.CreateDatePrg = toUpdate.CreateDatePrg;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationProgramView RemoveOrganizationProgram(ViewModels.OrganizationProgramView toRemove)
        {
            using (Context db = new Context())
            {
                OrganizationProgram toRemoveModel = db.organization_program.Find(toRemove.Id);

                //db.organization_program.Remove(toRemoveModel);
                //toRemoveModel.IsActive = false;

                db.SaveChanges();

                return toRemove;
            }
        }


        public static List<ViewModels.OrganizationProgramView> getOrganizationProgramByUserCode(string user_code)
        {
            List<ViewModels.OrganizationProgramView> toReturn = new List<ViewModels.OrganizationProgramView>();

            using (Context db = new Context())
            {

                List<OrganizationProgram> oProgram = new List<OrganizationProgram>();

                try
                {
                    int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;



                    //MISSING org_id using employee record
                    int sch_id = 1;

                    oProgram = db.organization_program.Where(l => l.Id > 0).ToList();

                    if (oProgram != null && oProgram.Count > 0)
                    {
                        for (int i = 0; i < oProgram.Count(); i++)
                        {
                            int ct_id = oProgram[i].CategoryId;
                            var tb_ctg = db.organization_program_category.Where(e => e.Id == ct_id).FirstOrDefault();
                            string ct_name = tb_ctg != null ? tb_ctg.ProgramCategoryName : "";

                            int ty_id = oProgram[i].WholeProgramTypeId;
                            var tb_ptype = db.organization_program_type.Where(e => e.Id == ty_id).FirstOrDefault();
                            string ty_name = tb_ptype != null ? tb_ptype.ProgramTypeName : "";

                            toReturn.Add(new ViewModels.OrganizationProgramView()
                            {
                                Id = oProgram[i].Id,
                                CategoryId = oProgram[i].CategoryId,
                                CategoryName = ct_name,
                                ProgramCode = oProgram[i].ProgramCode,
                                ProgramTitle = oProgram[i].ProgramTitle,
                                DisciplineName = oProgram[i].DisciplineName,
                                CreditHours = oProgram[i].CreditHours,
                                WholeProgramTypeId = oProgram[i].WholeProgramTypeId,
                                WholeProgramTypeNumber = oProgram[i].WholeProgramTypeNumber,
                                WholeTypeName = ty_name,
                                CreateDatePrgText = oProgram[i].CreateDatePrg.ToString("dd-MMM-yyyy"),
                                actions =
                                    "<span data-row='" + oProgram[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationProgram(" + oProgram[i].Id + "," + oProgram[i].CategoryId + ",'" + oProgram[i].ProgramCode + "','" + oProgram[i].ProgramTitle + "','" + oProgram[i].DisciplineName + "'," + oProgram[i].CreditHours + "," + oProgram[i].WholeProgramTypeId + "," + oProgram[i].WholeProgramTypeNumber + "));\">Edit</a>" +
                                    // "<span> / </span>" +
                                    // "<a href=\"javascript:void(deleteOrganizationSchedule(" + oProgram[i].Id + "));\">Delete</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oProgram = new List<OrganizationProgram>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationProgramView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationProgramView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationProgramView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationProgramView> FilterResult(string search, List<ViewModels.OrganizationProgramView> dtResult)
        {
            IQueryable<ViewModels.OrganizationProgramView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CategoryName != null && p.CategoryName.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCode != null && p.ProgramCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramTitle != null && p.ProgramTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.WholeTypeName != null && p.WholeTypeName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.DisciplineName != null && p.DisciplineName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.CreditHours != null && p.CreditHours.ToString().ToLower().Contains(search.ToLower())
                    )
                )
           );

            return results;
        }

    }

    #endregion




    #region Organization-Campuses-Programs

    public class OrganizationCampusProgramResultSet
    {

        public static List<OrganizationCampusView> GetOrganizationCampusList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusView() { Id = item.Id, CampusCode = item.CampusCode + "-" + item.CampusTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return lView;
        }


        public static List<OrganizationProgramView> GetOrganizationProgramList()
        {
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramView() { Id = item.Id, ProgramCode = item.ProgramCode + "-" + item.ProgramTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return lView;
        }


        public static string GetOrganizationCampusString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string cm_list = "";
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            cm_list += c.Id + ":" + c.CampusCode + "-" + c.CampusTitle.Replace(",", "") + ",";
                        }

                        cm_list = cm_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return cm_list.Replace("\r\n", "");
        }

        public static string GetOrganizationProgramString()
        {
            string prg_list = "";
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            prg_list += c.Id + ":" + c.ProgramCode + "-" + c.ProgramTitle + ",";
                        }

                        prg_list = prg_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return prg_list.Replace("\r\n", "");
        }


        public static int CreateOrganizationCampusProgram(OrganizationCampusProgramView toCreate)
        {
            int response = 0;
            OrganizationCampusProgram cpModel = new OrganizationCampusProgram();

            try
            {
                using (Context db = new Context())
                {
                    var already_enrollment = db.organization_campus_program.Where(c => c.CampusId == toCreate.CampusId && c.ProgramId == toCreate.ProgramId).FirstOrDefault();
                    if (already_enrollment == null)
                    {
                        cpModel.CampusId = toCreate.CampusId;
                        cpModel.ProgramId = toCreate.ProgramId;
                        cpModel.IsActiveProgram = toCreate.IsActiveProgram;

                        db.organization_campus_program.Add(cpModel);
                        db.SaveChanges();

                        response = cpModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationCampusProgram(OrganizationCampusProgramView toUpdate)
        {
            int response = 0;
            OrganizationCampusProgram cpModel = null;

            try
            {
                using (Context db = new Context())
                {
                    if (toUpdate.Id > 0)
                    {
                        cpModel = db.organization_campus_program.Find(toUpdate.Id);

                        //cpModel.OrganizationId = toUpdate.OrganizationId;
                        cpModel.CampusId = toUpdate.CampusId;
                        cpModel.ProgramId = toUpdate.ProgramId;
                        cpModel.IsActiveProgram = toUpdate.IsActiveProgram;
                        //cpModel.CreateDateEnr = toUpdate.CreateDateEnr;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationCampusProgramView RemoveOrganizationCampusProgram(ViewModels.OrganizationCampusProgramView toRemove)
        {
            using (Context db = new Context())
            {
                OrganizationCampusProgram toRemoveModel = db.organization_campus_program.Find(toRemove.Id);

                //db.organization_program.Remove(toRemoveModel);
                toRemoveModel.IsActiveProgram = false;

                db.SaveChanges();

                return toRemove;
            }
        }


        public static List<ViewModels.OrganizationCampusProgramView> getOrganizationCampusProgramByUserCode(string user_code)
        {
            List<ViewModels.OrganizationCampusProgramView> toReturn = new List<ViewModels.OrganizationCampusProgramView>();

            using (Context db = new Context())
            {
                List<OrganizationCampusProgram> oCampusProgram = new List<OrganizationCampusProgram>();

                try
                {
                    int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    //MISSING org_id using employee record
                    int sch_id = 1;

                    oCampusProgram = db.organization_campus_program.Where(l => l.Id > 0).ToList();

                    if (oCampusProgram != null && oCampusProgram.Count > 0)
                    {
                        for (int i = 0; i < oCampusProgram.Count(); i++)
                        {
                            int cm_id = oCampusProgram[i].CampusId;
                            var tb_cm = db.organization_campus.Where(e => e.Id == cm_id).FirstOrDefault();
                            string cm_code = tb_cm != null ? tb_cm.CampusCode.ToString() : "";

                            int pg_id = oCampusProgram[i].ProgramId;
                            var tb_pg = db.organization_program.Where(e => e.Id == pg_id).FirstOrDefault();
                            string pg_code = tb_pg != null ? tb_pg.ProgramCode : "";

                            string strIsActive = oCampusProgram[i].IsActiveProgram ? "true" : "false";

                            toReturn.Add(new ViewModels.OrganizationCampusProgramView()
                            {
                                Id = oCampusProgram[i].Id,
                                CampusId = oCampusProgram[i].CampusId,
                                ProgramId = oCampusProgram[i].ProgramId,
                                CampusName = cm_code,
                                ProgramName = pg_code,
                                IsActiveProgram = oCampusProgram[i].IsActiveProgram,
                                IsActiveProgramName = strIsActive,
                                actions =
                                    "<span data-row='" + oCampusProgram[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationCampusProgram(" + oCampusProgram[i].Id + "," + oCampusProgram[i].CampusId + "," + oCampusProgram[i].ProgramId + ",'" + strIsActive + "'));\">Edit</a>" +
                                     "<span> / </span>" +
                                     "<a href=\"javascript:void(deleteOrganizationCampusProgram(" + oCampusProgram[i].Id + "));\">Deactive</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oCampusProgram = new List<OrganizationCampusProgram>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCampusProgramView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCampusProgramView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCampusProgramView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCampusProgramView> FilterResult(string search, List<ViewModels.OrganizationCampusProgramView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCampusProgramView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CampusName != null && p.CampusName.ToLower().Contains(search.ToLower()) ||
                         p.ProgramName != null && p.ProgramName.ToLower().Contains(search.ToLower()) ||
                         p.IsActiveProgramName != null && p.IsActiveProgramName.ToString().ToLower().Contains(search.ToLower())

                    )
                )
           );

            return results;
        }

    }

    #endregion




    #region Organization-Courses

    public class OrganizationCourseResultSet
    {
        public static List<OrganizationProgramView> GetOrganizationProgramList()
        {
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramView() { Id = item.Id, ProgramCode = item.ProgramCode + "-" + item.ProgramTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramTypeView> GetOrganizationProgramTypeList()
        {
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramTypeView() { Id = item.Id, ProgramTypeName = item.ProgramTypeName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return lView;
        }


        public static string GetOrganizationProgramString()
        {
            string program_list = "";
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            program_list += c.Id + ":" + c.ProgramCode + "-" + c.ProgramTitle.Replace(",", "") + ",";
                        }

                        program_list = program_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return program_list.Replace("\r\n", "");
        }

        public static string GetOrganizationProgramTypeString()
        {
            string type_list = "";
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            type_list += c.Id + ":" + c.ProgramTypeName.Replace(",", "") + ",";
                        }

                        type_list = type_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return type_list.Replace("\r\n", "");
        }


        public static int CreateOrganizationCourse(OrganizationProgramCourseView toCreate)
        {
            int response = 0;
            OrganizationProgramCourse pcModel = new OrganizationProgramCourse();

            try
            {
                using (Context db = new Context())
                {
                    var already_program_course = db.organization_program_course.Where(c => c.ProgramId == toCreate.ProgramId && c.CourseCode != null && c.CourseCode.ToLower() == toCreate.CourseCode.ToLower()).FirstOrDefault();
                    if (already_program_course == null)
                    {
                        pcModel.ProgramId = toCreate.ProgramId;
                        pcModel.CourseCode = toCreate.CourseCode;
                        pcModel.CourseTitle = toCreate.CourseTitle;
                        pcModel.BookName = toCreate.BookName;
                        pcModel.BookAuthor = toCreate.BookAuthor;
                        pcModel.DefaultProgramTypeId = toCreate.DefaultProgramTypeId;
                        pcModel.DefaultProgramTypeNumber = toCreate.DefaultProgramTypeNumber;
                        pcModel.CreditHours = toCreate.CreditHours;
                        pcModel.PassingMarks = toCreate.PassingMarks;
                        pcModel.TotalMarks = toCreate.TotalMarks;
                        pcModel.IsActiveCourse = toCreate.IsActiveCourse;
                        pcModel.CreateDateCrs = toCreate.CreateDateCrs;

                        db.organization_program_course.Add(pcModel);
                        db.SaveChanges();

                        response = pcModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationCourse(OrganizationProgramCourseView toUpdate)
        {
            int response = 0;
            OrganizationProgramCourse pcModel = null;

            try
            {
                using (Context db = new Context())
                {
                    var already_exists = db.organization_program_course.Where(c => c.Id != toUpdate.Id && (c.CourseCode != null && c.CourseCode.ToLower() == toUpdate.CourseCode.ToLower())).FirstOrDefault();
                    if (already_exists != null)
                    {
                        //another same campus-code already there in table
                        response = 0;
                    }
                    else
                    {
                        pcModel = db.organization_program_course.Find(toUpdate.Id);

                        //pcModel.OrganizationId = toUpdate.OrganizationId;
                        pcModel.ProgramId = toUpdate.ProgramId;
                        pcModel.CourseCode = toUpdate.CourseCode;
                        pcModel.CourseTitle = toUpdate.CourseTitle;
                        pcModel.BookName = toUpdate.BookName;
                        pcModel.BookAuthor = toUpdate.BookAuthor;
                        pcModel.DefaultProgramTypeId = toUpdate.DefaultProgramTypeId;
                        pcModel.DefaultProgramTypeNumber = toUpdate.DefaultProgramTypeNumber;
                        pcModel.CreditHours = toUpdate.CreditHours;
                        pcModel.PassingMarks = toUpdate.PassingMarks;
                        pcModel.TotalMarks = toUpdate.TotalMarks;
                        pcModel.IsActiveCourse = toUpdate.IsActiveCourse;
                        //pcModel.CreateDatePrg = toUpdate.CreateDatePrg;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationProgramCourseView RemoveOrganizationCourse(ViewModels.OrganizationProgramCourseView toRemove)
        {
            using (Context db = new Context())
            {
                OrganizationProgramCourse toRemoveModel = db.organization_program_course.Find(toRemove.Id);

                //db.organization_program.Remove(toRemoveModel);
                toRemoveModel.IsActiveCourse = false;

                db.SaveChanges();

                return toRemove;
            }
        }


        public static List<ViewModels.OrganizationProgramCourseView> getOrganizationCourseByUserCode(string user_code)
        {
            List<ViewModels.OrganizationProgramCourseView> toReturn = new List<ViewModels.OrganizationProgramCourseView>();

            using (Context db = new Context())
            {
                List<OrganizationProgramCourse> oProgramCourse = new List<OrganizationProgramCourse>();

                try
                {
                    int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    //MISSING org_id using employee record
                    int sch_id = 1;

                    oProgramCourse = db.organization_program_course.Where(l => l.Id > 0).ToList();

                    if (oProgramCourse != null && oProgramCourse.Count > 0)
                    {
                        for (int i = 0; i < oProgramCourse.Count(); i++)
                        {
                            int pr_id = oProgramCourse[i].ProgramId;
                            var tb_prg = db.organization_program.Where(e => e.Id == pr_id).FirstOrDefault();
                            string pr_name = tb_prg != null ? tb_prg.ProgramTitle : "";

                            int ty_id = oProgramCourse[i].DefaultProgramTypeId;
                            var tb_ptype = db.organization_program_type.Where(e => e.Id == ty_id).FirstOrDefault();
                            string ty_name = tb_ptype != null ? tb_ptype.ProgramTypeName : "";

                            string strIsActive = oProgramCourse[i].IsActiveCourse ? "true" : "false";

                            toReturn.Add(new ViewModels.OrganizationProgramCourseView()
                            {
                                Id = oProgramCourse[i].Id,
                                ProgramId = oProgramCourse[i].ProgramId,
                                ProgramTitle = pr_name,
                                CourseCode = oProgramCourse[i].CourseCode,
                                CourseTitle = oProgramCourse[i].CourseTitle,
                                BookName = oProgramCourse[i].BookName,
                                BookAuthor = oProgramCourse[i].BookAuthor,
                                DefaultTypeName = ty_name,
                                DefaultProgramTypeId = oProgramCourse[i].DefaultProgramTypeId,
                                DefaultProgramTypeNumber = oProgramCourse[i].DefaultProgramTypeNumber,
                                CreditHours = oProgramCourse[i].CreditHours,
                                PassingMarks = oProgramCourse[i].PassingMarks,
                                TotalMarks = oProgramCourse[i].TotalMarks,
                                IsActiveCourse = oProgramCourse[i].IsActiveCourse,
                                IsActiveCourseName = strIsActive,
                                CreateDateCrsText = oProgramCourse[i].CreateDateCrs.ToString("dd-MMM-yyyy"),
                                actions =
                                    "<span data-row='" + oProgramCourse[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationCourse(" + oProgramCourse[i].Id + "," + oProgramCourse[i].ProgramId + ",'" + oProgramCourse[i].CourseCode + "','" + oProgramCourse[i].CourseTitle + "','" + oProgramCourse[i].BookName + "','" + oProgramCourse[i].BookAuthor + "'," + oProgramCourse[i].DefaultProgramTypeId + "," + oProgramCourse[i].DefaultProgramTypeNumber + "," + oProgramCourse[i].CreditHours + "," + oProgramCourse[i].PassingMarks + "," + oProgramCourse[i].TotalMarks + ",'" + strIsActive + "'));\">Edit</a>" +
                                     "<span> / </span>" +
                                     "<a href=\"javascript:void(deleteOrganizationCourse(" + oProgramCourse[i].Id + "));\">Deactive</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oProgramCourse = new List<OrganizationProgramCourse>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationProgramCourseView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationProgramCourseView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationProgramCourseView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationProgramCourseView> FilterResult(string search, List<ViewModels.OrganizationProgramCourseView> dtResult)
        {
            IQueryable<ViewModels.OrganizationProgramCourseView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.ProgramTitle != null && p.ProgramTitle.ToLower().Contains(search.ToLower()) ||
                         p.CourseCode != null && p.CourseCode.ToLower().Contains(search.ToLower()) ||
                         p.CourseTitle != null && p.CourseTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.BookName != null && p.BookName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.BookAuthor != null && p.BookAuthor.ToString().ToLower().Contains(search.ToLower()) ||
                         p.DefaultTypeName != null && p.DefaultTypeName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.DefaultProgramTypeNumber != null && p.DefaultProgramTypeNumber.ToString().ToLower().Contains(search.ToLower()) ||
                         p.PassingMarks != null && p.PassingMarks.ToString().ToLower().Contains(search.ToLower()) ||
                         p.TotalMarks != null && p.TotalMarks.ToString().ToLower().Contains(search.ToLower())
                    )
                )
           );

            return results;
        }

    }



    public class ManageCoursesImportExport
    {
        public class ManageCoursesCSV
        {
            public string employeeCode { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobileNo { get; set; }
            public string dateOfJoining { get; set; }
            public string dateOfLeaving { get; set; }
            public string accessGroup { get; set; }
            public int departmentID { get; set; }
            public int designationID { get; set; }
            public int functionID { get; set; }
            public int gradeID { get; set; }
            //public int groupID { get; set; }
            public int locationID { get; set; }
            public int regionID { get; set; }
            public int typeOfEmploymentID { get; set; }
            public bool active { get; set; }
        }
        public class CoursesExportTable
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string employee_code { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobile_no { get; set; }
            public DateTime? date_of_joining { get; set; }
            public DateTime? date_of_leaving { get; set; }
            public string access_group { get; set; }
            public int department_DepartmentId { get; set; }
            public int designation_DesignationId { get; set; }
            public int function_FunctionId { get; set; }
            public int grade_GradeId { get; set; }
            //public int Group_GroupId { get; set; }
            public int location_LocationId { get; set; }
            public int region_RegionId { get; set; }
            public int type_of_employment_TypeOfEmploymentId { get; set; }
            public bool active { get; set; }
        }
        public static List<ManageCoursesCSV> getManageCoursesCSVDownload()
        {
            List<ManageCoursesCSV> toReturn;
            using (var db = new Context())
            {

                try
                {


                    toReturn =
                        db.employee.Where(m => m.active).Select(
                        p => new ManageCoursesCSV()
                        {
                            employeeCode = p.employee_code,
                            firstName = (p.first_name == null) ? "" : p.first_name,
                            lastName = (p.last_name == null) ? "" : p.last_name,
                            email = (p.email == null) ? "" : p.email,
                            address = (p.address == null) ? "" : p.address,
                            mobileNo = (p.mobile_no == null) ? "" : p.mobile_no,
                            dateOfJoining = p.date_of_joining.ToString(),
                            dateOfLeaving = p.date_of_leaving.ToString(),
                            //accessGroupID=p.access_group_AccessGroupId,
                            accessGroup = p.access_group.name,
                            departmentID = (p.department == null) ? 0 : p.department.DepartmentId,
                            designationID = (p.designation == null) ? 0 : p.designation.DesignationId,
                            functionID = (p.function == null) ? 0 : p.function.FunctionId,
                            gradeID = (p.grade == null) ? 0 : p.grade.GradeId,
                            //groupID=p.Group_GroupId,
                            locationID = (p.location == null) ? 0 : p.location.LocationId,
                            regionID = (p.region == null) ? 0 : p.region.RegionId,
                            typeOfEmploymentID = (p.type_of_employment == null) ? 0 : p.type_of_employment.TypeOfEmploymentId,
                            active = p.timetune_active
                        }).ToList();


                    // convert dates to a more processable form.
                    foreach (var value in toReturn)
                    {
                        if (value.dateOfJoining != null && value.dateOfJoining != "")
                        {
                            DateTime dateOfJoining = DateTime.Parse(value.dateOfJoining);
                            value.dateOfJoining = dateOfJoining.ToString("yyyy-MM-dd");
                        }

                        if (value.dateOfLeaving != null && value.dateOfLeaving != "")
                        {
                            DateTime dateOfLeaving = DateTime.Parse(value.dateOfLeaving);
                            value.dateOfLeaving = dateOfLeaving.ToString("yyyy-MM-dd");
                        }

                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ManageCoursesCSV>();
                }

            }

            return toReturn;
        }


        private static bool convertStringToBool(string value)
        {
            return (value == "True" || value == "true" || value == "TRUE");
        }

        public static string setCourses(List<string> csvContents, string user_code)
        {
            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 11 values (columns).

                        if (values == null || values.Length < 11)
                            continue;

                        // Remove unwanted " characters.
                        string strProgramCode = values[0].Replace("\"", "");
                        string strCourseCode = values[1].Replace("\"", "");
                        string strCourseTitle = values[2].Replace("\"", "");
                        string strBookName = values[3].Replace("\"", ""); strBookName = strBookName ?? "0";
                        string strBookAuthor = values[4].Replace("\"", ""); strBookAuthor = strBookAuthor ?? "0";
                        string strPType = values[5].Replace("\"", "");
                        string strPTypeNumber = values[6].Replace("\"", "");
                        string strCreditHours = values[7].Replace("\"", "");
                        string strPassingMarks = values[8].Replace("\"", "");
                        string strTotalMarks = values[9].Replace("\"", "");
                        string strIsActive = values[10].Replace("\"", "");

                        //-----------------------------------------------
                        int iProgramId = 0; strProgramCode = strProgramCode.ToLower();
                        if (strProgramCode == "0")
                        {
                            iProgramId = 0;
                        }
                        else
                        {
                            var dbProgram = db.organization_program.Where(r => r.ProgramCode.ToLower() == strProgramCode).FirstOrDefault();
                            iProgramId = dbProgram != null ? dbProgram.Id : 0;
                        }

                        //-----------------------------------------------
                        if (strBookName == "0")
                        {
                            strBookName = "";
                        }

                        //-----------------------------------------------
                        if (strBookAuthor == "0")
                        {
                            strBookAuthor = "";
                        }


                        //-----------------------------------------------
                        int iProgTypeId = 0; strPType = strPType.ToLower();
                        var dbProgramType = db.organization_program_type.Where(r => r.ProgramTypeName.ToLower() == strPType).FirstOrDefault();
                        iProgTypeId = dbProgramType != null ? dbProgramType.Id : 1;

                        //-----------------------------------------------
                        int iProgTypeNumber = 0;
                        if (strPTypeNumber == "")
                        {
                            iProgTypeNumber = 1;
                        }
                        else
                        {
                            iProgTypeNumber = int.Parse(strPTypeNumber);
                        }


                        //-----------------------------------------------
                        int iCreditHours = 0;
                        if (strCreditHours == "0")
                        {
                            iCreditHours = 0;
                        }
                        else
                        {
                            iCreditHours = int.Parse(strCreditHours);
                        }

                        //-----------------------------------------------
                        int iPassMarks = 0;
                        if (strPassingMarks == "0")
                        {
                            iPassMarks = 0;
                        }
                        else
                        {
                            iPassMarks = int.Parse(strPassingMarks);
                        }

                        //-----------------------------------------------
                        int iTotalMarks = 0;
                        if (strTotalMarks == "0")
                        {
                            iTotalMarks = 0;
                        }
                        else
                        {
                            iTotalMarks = int.Parse(strTotalMarks);
                        }

                        //-----------------------------------------------
                        bool isActive = false;
                        if (strIsActive == "yes")
                        {
                            isActive = true;
                        }
                        else
                        {
                            isActive = false;
                        }

                        // continue if its the first row (CSV Header line).
                        if (strProgramCode == "program_code" || strCourseCode == "course_code")
                        {
                            continue;
                        }

                        // Get the existing course by program and type 
                        DLL.Models.OrganizationProgramCourse course = db.organization_program_course
                            .Where(m => m.ProgramId == iProgramId && m.CourseCode == strCourseCode &&
                                                    m.DefaultProgramTypeId == iProgTypeId && m.DefaultProgramTypeNumber == iProgTypeNumber).FirstOrDefault();

                        if (course != null)//existing course
                        {
                            course.ProgramId = iProgramId;
                            course.CourseCode = strCourseCode;
                            course.CourseTitle = strCourseTitle;
                            course.BookName = strBookName;
                            course.BookAuthor = strBookAuthor;
                            course.DefaultProgramTypeId = iProgTypeId;
                            course.DefaultProgramTypeNumber = iProgTypeNumber;
                            course.CreditHours = iCreditHours;
                            course.PassingMarks = iPassMarks;
                            course.TotalMarks = iTotalMarks;
                            course.IsActiveCourse = isActive;
                            course.CreateDateCrs = DateTime.Now;

                            db.SaveChanges();

                            TimeTune.AuditTrail.update("{\"*id\" : \"" + course.Id.ToString() + "\"}", "OrganizationProgramCourse", user_code);
                        }
                        else //new course
                        {
                            DLL.Models.OrganizationProgramCourse crs = new DLL.Models.OrganizationProgramCourse()
                            {
                                ProgramId = iProgramId,
                                CourseCode = strCourseCode,
                                CourseTitle = strCourseTitle,
                                BookName = strBookName,
                                BookAuthor = strBookAuthor,
                                DefaultProgramTypeId = iProgTypeId,
                                DefaultProgramTypeNumber = iProgTypeNumber,
                                CreditHours = iCreditHours,
                                PassingMarks = iPassMarks,
                                TotalMarks = iTotalMarks,
                                IsActiveCourse = isActive,
                                CreateDateCrs = DateTime.Now
                            };

                            db.organization_program_course.Add(crs);
                            db.SaveChanges();

                            TimeTune.AuditTrail.insert("{\"*id\" : \"" + crs.Id.ToString() + "\"}", "OrganizationProgramCourse", user_code);
                        }
                    }

                    return "Successful";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        public static bool validateEnrGCYear(string strGCYear)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    var dbGCYear = db.general_calender.Where(c => c.year.ToString() == strGCYear).FirstOrDefault();
                    if (dbGCYear != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrCampusCode(string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole)
                    {
                        isValid = true;
                    }
                    else
                    {
                        strCampusCode = strCampusCode.ToLower();

                        var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                        var dbEmployee = db.employee.Where(c => c.campus_id == iCampusID).FirstOrDefault();

                        if (dbCampus != null && dbEmployee != null)
                        {
                            if (dbCampus.Id == dbEmployee.campus_id)
                                isValid = true;
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrProgramCode(string strProgramCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strProgramCode = strProgramCode.ToLower();

                    var dbProgram = db.organization_program.Where(c => c.ProgramCode == strProgramCode).FirstOrDefault();
                    if (dbProgram != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }



        public static bool validateEnrProgramType(string strPType)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strPType = strPType.ToLower();

                    var dbCourse = db.organization_program_type.Where(c => c.ProgramTypeName.ToLower() == strPType).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrProgramCourseCode(string strCourseCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCourseCode = strCourseCode.ToLower();

                    var dbCourse = db.organization_program_course.Where(c => c.CourseCode == strCourseCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrStudentCode(string strStudentCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strStudentCode = strStudentCode.ToLower();

                    var dbCourse = db.employee.Where(c => c.function.name.ToLower() == "student" && c.employee_code == strStudentCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrStudentCodeCampus(string strCampusCode, string strStudentCode)
        {
            int iCampusID = 0;
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strStudentCode = strStudentCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        iCampusID = dbCampus.Id;
                    }

                    var dbCourse = db.employee.Where(c => c.campus_id == iCampusID && c.function.name.ToLower() == "student" && c.employee_code == strStudentCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

    }


    #region Courses-Report

    public class CoursesReportData
    {
        public string org_title { get; set; }
        public string org_logo_path { get; set; }
        public string campus_code { get; set; }
        public string program_code { get; set; }

        public string default_type_code { get; set; }

        public List<CoursesLog> crs_logs { get; set; }
    }

    public class CoursesLog
    {
        public int course_id { get; set; }
        public string course_code { get; set; }
        public string course_title { get; set; }
        public string credit_hours { get; set; }
    }


    public class CoursesReport
    {
        public CoursesReportData getCoursesData(int iProgId, int iDPTypeID, int iDPNumber)
        {
            List<DLL.Models.OrganizationProgramCourse> dbCourse = null;
            List<Employee> employees_list = new List<Employee>();
            CoursesReportData toReturn = null;

            using (var db = new Context())
            {
                toReturn = new CoursesReportData();
                List<CoursesLog> courses_log = new List<CoursesLog>();

                var dbOrgCampus = (from c in db.organization_campus
                                   join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                   join p in db.organization_program on cp.ProgramId equals p.Id
                                   join o in db.organization on c.OrganizationId equals o.Id
                                   where p.Id == iProgId && cp.IsActiveProgram
                                   select new
                                   {
                                       org_name = o.OrganizationTitle,
                                       org_logo = o.Logo,
                                       org_url = o.WebsiteURL,
                                       campus_name = c.CampusCode + "-" + c.CampusTitle,
                                       program_name = p.ProgramCode + "-" + p.ProgramTitle
                                   }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.org_title = dbOrgCampus.org_name;
                    toReturn.org_logo_path = dbOrgCampus.org_logo;
                    toReturn.campus_code = dbOrgCampus.campus_name;
                    toReturn.program_code = dbOrgCampus.program_name;
                }

                //string strProgram = "";
                //var dbProgram = db.organization_program.Where(p => p.Id == iProgId).FirstOrDefault();
                //if (dbProgram != null)
                //{
                //    strProgram = dbProgram.ProgramCode + "-" + dbProgram.ProgramTitle;
                //}

                string strDPType = "";
                var dbDPType = db.organization_program_type.Where(p => p.Id == iDPTypeID).FirstOrDefault();
                if (dbDPType != null)
                {
                    strDPType = dbDPType.ProgramTypeName;
                }

                toReturn.default_type_code = strDPType + " - " + iDPNumber;

                dbCourse = db.organization_program_course.Where(s => s.ProgramId == iProgId && s.DefaultProgramTypeId == iDPTypeID && s.DefaultProgramTypeNumber == iDPNumber).ToList();
                if (dbCourse != null && dbCourse.Count > 0)
                {
                    foreach (var c in dbCourse)
                    {
                        CoursesLog cLog = new CoursesLog()
                        {
                            course_id = c.Id,
                            course_code = c.CourseCode,
                            course_title = c.CourseTitle,
                            credit_hours = c.CreditHours.ToString()
                        };

                        courses_log.Add(cLog);
                    }

                    toReturn.crs_logs = courses_log;
                }
            }

            return toReturn;
        }


        public EnrollmentReportData getEnrollmentData_BACKUP(int iGCYearID, int iCampusId, int iProgId)
        {
            List<DLL.Models.OrganizationProgramCourseEnrollment> dbEnrollment = null;
            List<Employee> employees_list = new List<Employee>();
            EnrollmentReportData toReturn = null;

            using (var db = new Context())
            {
                toReturn = new EnrollmentReportData();
                List<EnrollmentStudentLog> enrolled_student_log = new List<EnrollmentStudentLog>();

                var dbOrgCampus = (from c in db.organization_campus
                                   join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                   join p in db.organization_program on cp.ProgramId equals p.Id
                                   join o in db.organization on c.OrganizationId equals o.Id
                                   where c.Id == iCampusId && p.Id == iProgId && cp.IsActiveProgram
                                   select new
                                   {
                                       org_name = o.OrganizationTitle,
                                       org_logo = o.Logo,
                                       org_url = o.WebsiteURL,
                                       campus_name = c.CampusCode + "-" + c.CampusTitle,
                                       program_name = p.ProgramCode + "-" + p.ProgramTitle
                                   }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.year_code = db.general_calender.Where(x => x.GeneralCalendarId == iGCYearID).FirstOrDefault().year.ToString();
                    toReturn.org_title = dbOrgCampus.org_name;
                    toReturn.org_logo_path = dbOrgCampus.org_logo;
                    toReturn.campus_code = dbOrgCampus.campus_name;
                    toReturn.program_code = dbOrgCampus.program_name;
                }

                int xCampusID = 0, xStudentID = 0;
                var dbStudentCampus = db.employee.Where(x => x.campus_id == iCampusId && x.function.name.ToLower() == "student").ToList();
                if (dbStudentCampus != null && dbStudentCampus.Count > 0)
                {
                    foreach (var std in dbStudentCampus)
                    {
                        xCampusID = std.campus_id;
                        xStudentID = std.EmployeeId;

                        dbEnrollment = db.organization_program_course_enrollment.Where(s => s.GeneralCalendarId == iGCYearID && s.EmployeeStudentId == xStudentID).ToList();
                        if (dbEnrollment != null && dbEnrollment.Count > 0)
                        {
                            foreach (var e in dbEnrollment)
                            {
                                var dbProgram = db.organization_program_course.Where(x => x.Id == e.ProgramCourseId && x.ProgramId == iProgId).FirstOrDefault();
                                if (dbProgram != null)
                                {
                                    int iCampusID = 0, iLGroupID = 0; string strStudentCode = "", strStudentName = "", strLGroupName = "";
                                    var dbStudentInfo = db.employee.Where(p => p.EmployeeId == e.EmployeeStudentId).FirstOrDefault();
                                    if (dbStudentInfo != null)
                                    {
                                        iCampusID = dbStudentInfo.campus_id;
                                        iLGroupID = dbStudentInfo.region.RegionId;
                                        strLGroupName = dbStudentInfo.region.name;
                                        strStudentCode = dbStudentInfo.employee_code;
                                        strStudentName = dbStudentInfo.first_name + " " + dbStudentInfo.last_name;
                                    }

                                    int program_id = 0;
                                    var dbProgCourse = db.organization_program_course.Where(p => p.ProgramId == e.ProgramCourseId).FirstOrDefault();
                                    if (dbProgCourse != null)
                                    {
                                        program_id = dbProgCourse.ProgramId;

                                        var dbProg = db.organization_program.Where(p => p.Id == program_id).FirstOrDefault();
                                        if (dbProg != null)
                                        {
                                            //toReturn.program_code = dbProg.ProgramCode + "-" + dbProg.ProgramTitle;
                                        }
                                    }

                                    string strCoursesList = "";
                                    var dbStdEnr = db.organization_program_course_enrollment.Where(x => x.GeneralCalendarId == iGCYearID && x.EmployeeStudentId == e.EmployeeStudentId).FirstOrDefault();
                                    if (dbStdEnr != null)
                                    {
                                        var dbCourses = db.organization_program_course.Where(p => p.Id == dbStdEnr.ProgramCourseId).ToList();
                                        if (dbCourses != null)
                                        {
                                            foreach (var cc in dbCourses)
                                            {
                                                strCoursesList += cc.CourseCode + ",";
                                            }

                                        }
                                    }

                                    string strProgType = "";
                                    var dbProgType = db.organization_program_type.Where(p => p.Id == e.EnrolledProgramTypeId).FirstOrDefault();
                                    if (dbProgType != null)
                                    {
                                        strProgType = dbProgType.ProgramTypeName;
                                    }

                                    EnrollmentStudentLog eLog = new EnrollmentStudentLog()
                                    {
                                        campus_id = iCampusID,
                                        program_id = program_id,
                                        lgroup_id = iLGroupID,//region
                                        student_code = strStudentCode,
                                        student_name = strStudentName,
                                        lgroup_name = strLGroupName,//region
                                        enrollment_title = e.EnrollmentTitle,
                                        crs_list = strCoursesList ?? "",
                                        program_type_id = e.EnrolledProgramTypeId,
                                        program_type_name = strProgType,
                                        program_type_number = e.EnrolledProgramTypeNumber,
                                        is_course_failed = e.IsCourseFailed,
                                        is_course_failed_name = e.IsCourseFailed == false ? "No" : "Yes"
                                    };

                                    enrolled_student_log.Add(eLog);
                                }
                            }
                        }
                    }

                    toReturn.std_logs = enrolled_student_log;
                }
            }

            return toReturn;
        }




    }

    #endregion


    #endregion




    #region Organization-Enrollments

    public class OrganizationEnrollmentResultSet
    {
        public static List<GeneralCalendarView> GetOrganizationGeneralCalendarList()
        {
            List<DLL.Models.GeneralCalendar> mView = null;
            List<GeneralCalendarView> lView = new List<GeneralCalendarView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.general_calender.OrderByDescending(o => o.GeneralCalendarId).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new GeneralCalendarView() { Id = item.GeneralCalendarId, YearName = item.year.ToString() });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.GeneralCalendar>();
                }
            }

            return lView;
        }

        public static List<EmployeeView> GetOrganizationEmployeeStudentList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.function.name.ToLower() == "student").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.campus_id == iGVCampusID && e.function.name.ToLower() == "student").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new EmployeeView() { Id = item.EmployeeId, EmployeeCode = item.employee_code + "-" + item.first_name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramCourseView> GetOrganizationProgramCourseList()
        {
            List<DLL.Models.OrganizationProgramCourse> mView = null;
            List<OrganizationProgramCourseView> lView = new List<OrganizationProgramCourseView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_course.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramCourseView() { Id = item.Id, CourseCode = item.CourseCode });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramCourse>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramTypeView> GetOrganizationProgramTypeList()
        {
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramTypeView() { Id = item.Id, ProgramTypeName = item.ProgramTypeName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return lView;
        }

        public static List<GeneralCalendarView> GetOrganizationYearList()
        {
            List<DLL.Models.GeneralCalendar> mView = null;
            List<GeneralCalendarView> lView = new List<GeneralCalendarView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.general_calender.OrderByDescending(g => g.year).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new GeneralCalendarView() { Id = item.GeneralCalendarId, YearName = item.year.ToString() });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.GeneralCalendar>();
                }
            }

            return lView;
        }

        public static List<OrganizationCampusView> GetOrganizationCampusList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusView() { Id = item.Id, CampusCode = item.CampusCode + "-" + item.CampusTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return lView;
        }


        public static List<OrganizationProgramView> GetOrganizationProgramList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_program.ToList();
                        if (mView != null && mView.Count > 0)
                        {
                            foreach (var item in mView)
                            {
                                lView.Add(new OrganizationProgramView()
                                {
                                    Id = item.Id,
                                    ProgramCode = item.ProgramCode + "-" + item.ProgramTitle + " (" + item.DisciplineName + ")"
                                });
                            }
                        }
                    }
                    else
                    {
                        lView = (from c in db.organization_campus
                                 join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                 join p in db.organization_program on cp.CampusId equals p.Id
                                 where c.Id == iGVCampusID
                                 select new OrganizationProgramView()
                                 {
                                     Id = p.Id,
                                     ProgramCode = p.ProgramCode + "-" + p.ProgramTitle + " (" + p.DisciplineName + ")"
                                 }).ToList();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return lView;
        }

        public static string GetOrganizationGeneralCalendarString()
        {
            string gc_list = "";
            List<DLL.Models.GeneralCalendar> mView = null;
            List<GeneralCalendarView> lView = new List<GeneralCalendarView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.general_calender.OrderByDescending(o => o.GeneralCalendarId).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            gc_list += c.GeneralCalendarId + ":" + c.year.ToString().Replace(",", "") + ",";
                        }

                        gc_list = gc_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.GeneralCalendar>();
                }
            }

            return gc_list.Replace("\r\n", "");
        }

        public static string GetOrganizationCampusString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string cmp_list = "";
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            cmp_list += c.Id + ":" + c.CampusCode + "-" + c.CampusTitle + ",";
                        }

                        cmp_list = cmp_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return cmp_list.Replace("\r\n", "");
        }

        public static string GetOrganizationProgramString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string pc_list = "";
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            pc_list += c.Id + ":" + c.ProgramCode + "-" + c.ProgramTitle.Replace(",", "") + ",";
                        }

                        pc_list = pc_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return pc_list.Replace("\r\n", "");
        }


        public static string GetOrganizationEmployeeStudentString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string emp_list = "";
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "student").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.active && e.campus_id == iGVCampusID && e.function.name.ToLower() == "student").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            emp_list += c.EmployeeId + ":" + c.employee_code + "-" + c.first_name + ",";
                        }

                        emp_list = emp_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return emp_list.Replace("\r\n", "");
        }

        public static string GetOrganizationProgramCourseString()
        {
            string pc_list = "";
            List<DLL.Models.OrganizationProgramCourse> mView = null;
            List<OrganizationProgramCourseView> lView = new List<OrganizationProgramCourseView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_course.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            pc_list += c.Id + ":" + c.CourseCode + "-" + c.CourseTitle.Replace(",", "") + ",";
                        }

                        pc_list = pc_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramCourse>();
                }
            }

            return pc_list.Replace("\r\n", "");
        }

        public static string GetOrganizationProgramTypeString()
        {
            string type_list = "";
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            type_list += c.Id + ":" + c.ProgramTypeName.Replace(",", "") + ",";
                        }

                        type_list = type_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return type_list.Replace("\r\n", "");
        }

        public static int CreateOrganizationEnrollment(OrganizationProgramCourseEnrollmentView toCreate)
        {
            int response = 0;
            OrganizationProgramCourseEnrollment enModel = new OrganizationProgramCourseEnrollment();

            try
            {
                using (Context db = new Context())
                {
                    var already_enrollment = db.organization_program_course_enrollment.Where(c => c.GeneralCalendarId == toCreate.GeneralCalendarId &&
                                                    c.CampusId == toCreate.CampusId && c.ProgramId == toCreate.ProgramId && c.EmployeeStudentId == toCreate.EmployeeStudentId &&
                                                    c.EnrolledProgramTypeId == toCreate.EnrolledProgramTypeId && c.EnrolledProgramTypeNumber == toCreate.EnrolledProgramTypeNumber &&
                                                    c.EnrollmentTitle != null && c.EnrollmentTitle == toCreate.EnrollmentTitle).FirstOrDefault();
                    if (already_enrollment == null)
                    {
                        enModel.GeneralCalendarId = toCreate.GeneralCalendarId;
                        enModel.CampusId = toCreate.CampusId;
                        enModel.ProgramId = toCreate.ProgramId;
                        enModel.EmployeeStudentId = toCreate.EmployeeStudentId;
                        enModel.ProgramCourseId = toCreate.ProgramCourseId;
                        enModel.EnrollmentTitle = toCreate.EnrollmentTitle;
                        enModel.EnrolledProgramTypeId = toCreate.EnrolledProgramTypeId;
                        enModel.EnrolledProgramTypeNumber = toCreate.EnrolledProgramTypeNumber;
                        enModel.IsCourseFailed = toCreate.IsCourseFailed;
                        enModel.CreateDateEnr = toCreate.CreateDateEnr;

                        db.organization_program_course_enrollment.Add(enModel);
                        db.SaveChanges();

                        response = enModel.Id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationEnrollment(OrganizationProgramCourseEnrollmentView toUpdate)
        {
            int response = 0;
            OrganizationProgramCourseEnrollment enModel = null;

            try
            {
                using (Context db = new Context())
                {
                    if (toUpdate.Id > 0)
                    {
                        enModel = db.organization_program_course_enrollment.Find(toUpdate.Id);

                        //enModel.OrganizationId = toUpdate.OrganizationId;
                        enModel.GeneralCalendarId = toUpdate.GeneralCalendarId;
                        enModel.CampusId = toUpdate.CampusId;
                        enModel.ProgramId = toUpdate.ProgramId;
                        enModel.EmployeeStudentId = toUpdate.EmployeeStudentId;
                        enModel.ProgramCourseId = toUpdate.ProgramCourseId;
                        enModel.EnrollmentTitle = toUpdate.EnrollmentTitle;
                        enModel.EnrolledProgramTypeId = toUpdate.EnrolledProgramTypeId;
                        enModel.EnrolledProgramTypeNumber = toUpdate.EnrolledProgramTypeNumber;
                        enModel.IsCourseFailed = toUpdate.IsCourseFailed;
                        //enModel.CreateDateEnr = toUpdate.CreateDateEnr;

                        db.SaveChanges();

                        response = toUpdate.Id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static ViewModels.OrganizationProgramCourseEnrollmentView RemoveOrganizationEnrollment(ViewModels.OrganizationProgramCourseEnrollmentView toRemove)
        {
            using (Context db = new Context())
            {
                OrganizationProgramCourseEnrollment toRemoveModel = db.organization_program_course_enrollment.Find(toRemove.Id);

                db.organization_program_course_enrollment.Remove(toRemoveModel);
                //toRemoveModel.IsCourseFailed = false;

                db.SaveChanges();

                return toRemove;
            }
        }


        public static List<ViewModels.OrganizationProgramCourseEnrollmentView> getOrganizationEnrollmentByUserCode(string user_code)
        {
            List<ViewModels.OrganizationProgramCourseEnrollmentView> toReturn = new List<ViewModels.OrganizationProgramCourseEnrollmentView>();

            using (Context db = new Context())
            {
                List<OrganizationProgramCourseEnrollment> oEnrollment = new List<OrganizationProgramCourseEnrollment>();

                try
                {
                    int user_id = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    //MISSING org_id using employee record
                    int sch_id = 1;

                    oEnrollment = db.organization_program_course_enrollment.Where(l => l.Id > 0).ToList();

                    if (oEnrollment != null && oEnrollment.Count > 0)
                    {
                        for (int i = 0; i < oEnrollment.Count(); i++)
                        {
                            int gc_id = oEnrollment[i].GeneralCalendarId;
                            var tb_gc = db.general_calender.Where(e => e.GeneralCalendarId == gc_id).FirstOrDefault();
                            string gc_year = tb_gc != null ? tb_gc.year.ToString() : "";

                            int cm_id = oEnrollment[i].CampusId;
                            var tb_cm = db.organization_campus.Where(e => e.Id == cm_id).FirstOrDefault();
                            string cm_name = tb_cm != null ? tb_cm.CampusCode.ToString() : "";

                            int pg_id = oEnrollment[i].ProgramId;
                            var tb_pg = db.organization_program.Where(e => e.Id == pg_id).FirstOrDefault();
                            string pg_name = tb_pg != null ? tb_pg.ProgramCode.ToString() : "";

                            int pc_id = oEnrollment[i].ProgramCourseId;
                            var tb_pc = db.organization_program_course.Where(e => e.Id == pc_id).FirstOrDefault();
                            string pc_name = tb_pc != null ? tb_pc.CourseCode : "";

                            int p_id = oEnrollment[i].ProgramCourseId;
                            var tb_p = db.organization_program.Where(e => e.Id == p_id).FirstOrDefault();
                            string p_name = ""; // tb_p != null ? tb_p.CourseCode : "";

                            int emp_id = oEnrollment[i].EmployeeStudentId;
                            var tb_emp = db.employee.Where(e => e.EmployeeId == emp_id).FirstOrDefault();
                            string st_code = tb_emp != null ? tb_emp.employee_code : "";
                            string st_name = tb_emp != null ? tb_emp.first_name : "";

                            int ty_id = oEnrollment[i].EnrolledProgramTypeId;
                            var tb_ptype = db.organization_program_type.Where(e => e.Id == ty_id).FirstOrDefault();
                            string ty_name = tb_ptype != null ? tb_ptype.ProgramTypeName : "";

                            string strIsFailed = oEnrollment[i].IsCourseFailed ? "true" : "false";

                            toReturn.Add(new ViewModels.OrganizationProgramCourseEnrollmentView()
                            {
                                Id = oEnrollment[i].Id,
                                GeneralCalendarId = oEnrollment[i].GeneralCalendarId,
                                GeneralCalendarYear = gc_year,
                                CampusId = oEnrollment[i].CampusId,
                                ProgramId = oEnrollment[i].ProgramId,
                                EmployeeStudentId = oEnrollment[i].EmployeeStudentId,
                                ProgramCourseTitle = pc_name,
                                ProgramTitle = p_name,
                                StudentCode = st_code,
                                CampusCode = cm_name,
                                ProgramCode = pg_name,
                                StudentName = st_name,
                                EnrollmentTitle = oEnrollment[i].EnrollmentTitle,
                                EnrolledTypeName = ty_name,
                                EnrolledProgramTypeId = oEnrollment[i].EnrolledProgramTypeId,
                                EnrolledProgramTypeNumber = oEnrollment[i].EnrolledProgramTypeNumber,
                                IsCourseFailed = oEnrollment[i].IsCourseFailed,
                                IsCourseFailedName = strIsFailed,
                                CreateDateEnrText = oEnrollment[i].CreateDateEnr.ToString("dd-MMM-yyyy"),
                                actions =
                                    "<span data-row='" + oEnrollment[i].Id + "'>" +
                                        "<a href=\"javascript:void(editOrganizationEnrollment(" + oEnrollment[i].Id + "," + oEnrollment[i].GeneralCalendarId + "," + oEnrollment[i].CampusId + "," + oEnrollment[i].ProgramId + "," + oEnrollment[i].EmployeeStudentId + "," + oEnrollment[i].ProgramCourseId + ",'" + oEnrollment[i].EnrollmentTitle + "'," + oEnrollment[i].EnrolledProgramTypeId + "," + oEnrollment[i].EnrolledProgramTypeNumber + ",'" + strIsFailed + "'));\">Edit</a>" +
                                    "<span> / </span>" +
                                    "<a href=\"javascript:void(deleteOrganizationEnrollment(" + oEnrollment[i].Id + "));\">Delete</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    oEnrollment = new List<OrganizationProgramCourseEnrollment>();
                }
            }

            return toReturn;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationProgramCourseEnrollmentView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationProgramCourseEnrollmentView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationProgramCourseEnrollmentView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationProgramCourseEnrollmentView> FilterResult(string search, List<ViewModels.OrganizationProgramCourseEnrollmentView> dtResult)
        {
            IQueryable<ViewModels.OrganizationProgramCourseEnrollmentView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.GeneralCalendarYear != null && p.GeneralCalendarYear.ToLower().Contains(search.ToLower()) ||
                         p.CampusCode != null && p.CampusCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCode != null && p.ProgramCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCourseTitle != null && p.ProgramCourseTitle.ToLower().Contains(search.ToLower()) ||
                         p.ProgramTitle != null && p.ProgramTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.StudentCode != null && p.StudentCode.ToString().ToLower().Contains(search.ToLower()) ||
                         p.StudentName != null && p.StudentName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.EnrollmentTitle != null && p.EnrollmentTitle.ToString().ToLower().Contains(search.ToLower()) ||
                         p.EnrolledTypeName != null && p.EnrolledTypeName.ToString().ToLower().Contains(search.ToLower()) ||
                         p.EnrolledProgramTypeNumber != null && p.EnrolledProgramTypeNumber.ToString().ToLower().Contains(search.ToLower()) ||
                         p.IsCourseFailed != null && p.IsCourseFailed.ToString().ToLower().Contains(search.ToLower())

                    )
                )
           );

            return results;
        }

    }




    public class ManageStudentsEnrollmentImportExport
    {
        public class ManageEnrollmentCSV
        {
            public string gcYear { get; set; }
            public string campusCode { get; set; }
            public string programCode { get; set; }
            public string studentCode { get; set; }
            public string courseCode { get; set; }
            public string enrollmentTitle { get; set; }
            public string pType { get; set; }
            public string pTypeNumber { get; set; }
            public string isFailed { get; set; }
        }
        public class EnrExportTable
        {
            public string gcYear { get; set; }
            public string campusCode { get; set; }
            public string programCode { get; set; }
            public string studentCode { get; set; }
            public string courseCode { get; set; }
            public string enrollmentTitle { get; set; }
            public string pType { get; set; }
            public string pTypeNumber { get; set; }
            public string isFailed { get; set; }
        }

        public static List<ManageEnrollmentCSV> getManageStudentsEnrollmentCSVDownload()
        {
            List<ManageEnrollmentCSV> toReturn;
            using (var db = new Context())
            {
                try
                {
                    toReturn =
                        db.organization_program_course_enrollment.Where(m => m.Id > 0).Select(
                        p => new ManageEnrollmentCSV()
                        {
                            gcYear = db.general_calender.Where(c => c.GeneralCalendarId == p.GeneralCalendarId).FirstOrDefault().year.ToString(),
                            campusCode = db.organization_campus.Where(c => c.Id == p.CampusId).FirstOrDefault().CampusCode,
                            programCode = db.organization_program.Where(c => c.Id == p.ProgramId).FirstOrDefault().ProgramCode,
                            studentCode = db.employee.Where(c => c.EmployeeId == p.EmployeeStudentId).FirstOrDefault().employee_code,
                            courseCode = db.organization_program_course.Where(c => c.Id == p.ProgramCourseId).FirstOrDefault().CourseCode,
                            enrollmentTitle = p.EnrollmentTitle,
                            pType = db.organization_program_type.Where(c => c.Id == p.EnrolledProgramTypeId).FirstOrDefault().ProgramTypeName,
                            pTypeNumber = p.EnrolledProgramTypeNumber.ToString(),
                            isFailed = p.IsCourseFailed ? "yes" : "no"
                        }).ToList();

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ManageEnrollmentCSV>();
                }
            }

            return toReturn;
        }

        public class ManageContractualStaffCSV
        {
            public string employeeCode { get; set; }
            public string employeeName { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobileNo { get; set; }
            public string dateOfJoining { get; set; }
            public string dateOfLeaving { get; set; }
            public string company { get; set; }
            public string department { get; set; }
            public string designation { get; set; }
            public string function { get; set; }
            public string grade { get; set; }
            //public int groupID { get; set; }
            public string location { get; set; }
            public string region { get; set; }
            public bool active { get; set; }
        }

        private static bool convertStringToBool(string value)
        {
            return (value == "True" || value == "true" || value == "TRUE");
        }

        public static string setEnrollment(List<string> csvContents, string user_code)
        {
            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 8 values (columns).

                        if (values == null || values.Length < 9)
                            continue;

                        // Remove unwanted " characters.
                        string strGCYear = values[0].Replace("\"", "");
                        string strCampusCode = values[1].Replace("\"", "");
                        string strProgramCode = values[2].Replace("\"", "");
                        string strStudentCode = values[3].Replace("\"", "");
                        string strCourseCode = values[4].Replace("\"", "");
                        string strEnrTitle = values[5].Replace("\"", "");
                        string strEProgType = values[6].Replace("\"", "");
                        string strEProgTypeNumber = values[7].Replace("\"", "");
                        string strIsFailed = values[8].Replace("\"", "");

                        //-----------------------------------------------
                        int iGCId = 0; int gcYear = int.Parse(strGCYear);
                        var dbGCYear = db.general_calender.Where(r => r.year == gcYear).FirstOrDefault();
                        iGCId = dbGCYear != null ? dbGCYear.GeneralCalendarId : 0;

                        //-----------------------------------------------
                        int iCampusId = 0; strCampusCode = strCampusCode.ToLower();
                        var dbCampus = db.organization_campus.Where(r => r.CampusCode.ToLower() == strCampusCode).FirstOrDefault();
                        iCampusId = dbCampus != null ? dbCampus.Id : 0;

                        //-----------------------------------------------
                        int iProgramId = 0; strProgramCode = strProgramCode.ToLower();
                        if (strProgramCode == "0")
                        {
                            iProgramId = 0;
                        }
                        else
                        {
                            var dbProgram = db.organization_program.Where(r => r.ProgramCode.ToLower() == strProgramCode).FirstOrDefault();
                            iProgramId = dbProgram != null ? dbProgram.Id : 0;
                        }

                        //-----------------------------------------------
                        int iEmployeeStudentId = 0;
                        if (strStudentCode == "0")
                        {
                            iEmployeeStudentId = 0;
                        }
                        else
                        {
                            var dbEmployeeStudent = db.employee.Where(r => r.employee_code == strStudentCode).FirstOrDefault();
                            iEmployeeStudentId = dbEmployeeStudent != null ? dbEmployeeStudent.EmployeeId : 0;
                        }

                        //-----------------------------------------------
                        int iCourseId = 0; strCourseCode = strCourseCode.ToLower();
                        if (strCourseCode == "0")
                        {
                            iCourseId = 0;
                        }
                        else
                        {
                            var dbCourse = db.organization_program_course.Where(r => r.CourseCode != null && r.CourseCode.ToLower() == strCourseCode).FirstOrDefault();
                            iCourseId = dbCourse != null ? dbCourse.Id : 0;
                        }

                        //-----------------------------------------------
                        if (strEnrTitle == "0")
                        {
                            strEnrTitle = "";
                        }

                        //-----------------------------------------------
                        int iProgTypeId = 0; strEProgType = strEProgType.ToLower();
                        var dbProgramType = db.organization_program_type.Where(r => r.ProgramTypeName.ToLower() == strEProgType).FirstOrDefault();
                        iProgTypeId = dbProgramType != null ? dbProgramType.Id : 1;

                        //-----------------------------------------------
                        int iProgTypeNumber = 0;
                        if (strEProgTypeNumber == "")
                        {
                            iProgTypeNumber = 0;
                        }
                        else
                        {
                            iProgTypeNumber = int.Parse(strEProgTypeNumber);
                        }

                        //-----------------------------------------------
                        bool isFailed = false;
                        if (strIsFailed == "yes")
                        {
                            isFailed = true;
                        }
                        else
                        {
                            isFailed = false;
                        }


                        // continue if its the first row (CSV Header line).
                        if (strGCYear == "year" || strCampusCode == "campus_code" || strIsFailed == "is_course_failed")
                        {
                            continue;
                        }

                        // Get the existing schedule by room and date 
                        DLL.Models.OrganizationProgramCourseEnrollment enrollment = db.organization_program_course_enrollment
                            .Where(m => m.GeneralCalendarId == iGCId && m.CampusId == iCampusId && m.ProgramId == iProgramId &&
                            m.EmployeeStudentId == iEmployeeStudentId && m.EnrolledProgramTypeId == iProgTypeId && m.EnrolledProgramTypeNumber == iProgTypeNumber &&
                            m.ProgramCourseId == iCourseId
                            ).FirstOrDefault();

                        if (enrollment != null)//existing enrollment
                        {
                            enrollment.GeneralCalendarId = iGCId;
                            enrollment.CampusId = iCampusId;
                            enrollment.ProgramId = iProgramId;
                            enrollment.EmployeeStudentId = iEmployeeStudentId;
                            enrollment.ProgramCourseId = iCourseId;
                            enrollment.EnrollmentTitle = strEnrTitle;
                            enrollment.EnrolledProgramTypeId = iProgTypeId;
                            enrollment.EnrolledProgramTypeNumber = iProgTypeNumber;
                            enrollment.IsCourseFailed = isFailed;
                            enrollment.CreateDateEnr = DateTime.Now;
                            db.SaveChanges();

                            TimeTune.AuditTrail.update("{\"*id\" : \"" + enrollment.Id.ToString() + "\"}", "OrganizationProgramCourseEnrollment", user_code);
                        }
                        else //new enrollment
                        {
                            DLL.Models.OrganizationProgramCourseEnrollment enr = new DLL.Models.OrganizationProgramCourseEnrollment()
                            {
                                GeneralCalendarId = iGCId,
                                CampusId = iCampusId,
                                ProgramId = iProgramId,
                                EmployeeStudentId = iEmployeeStudentId,
                                ProgramCourseId = iCourseId,
                                EnrollmentTitle = strEnrTitle,
                                EnrolledProgramTypeId = iProgTypeId,
                                EnrolledProgramTypeNumber = iProgTypeNumber,
                                IsCourseFailed = isFailed,
                                CreateDateEnr = DateTime.Now
                            };

                            db.organization_program_course_enrollment.Add(enr);
                            db.SaveChanges();

                            TimeTune.AuditTrail.insert("{\"*id\" : \"" + enr.Id.ToString() + "\"}", "OrganizationProgramCourseEnrollment", user_code);
                        }
                    }

                    return "Successful";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        public static bool validateEnrGCYear(string strGCYear)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    var dbGCYear = db.general_calender.Where(c => c.year.ToString() == strGCYear).FirstOrDefault();
                    if (dbGCYear != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrCampusCode(string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole)
                    {
                        isValid = true;
                    }
                    else
                    {
                        strCampusCode = strCampusCode.ToLower();

                        var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                        var dbEmployee = db.employee.Where(c => c.campus_id == iCampusID).FirstOrDefault();

                        if (dbCampus != null && dbEmployee != null)
                        {
                            if (dbCampus.Id == dbEmployee.campus_id)
                                isValid = true;
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrProgramCode(string strProgramCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strProgramCode = strProgramCode.ToLower();

                    var dbProgram = db.organization_program.Where(c => c.ProgramCode == strProgramCode).FirstOrDefault();
                    if (dbProgram != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }



        public static bool validateEnrProgramType(string strPType)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strPType = strPType.ToLower();

                    var dbCourse = db.organization_program_type.Where(c => c.ProgramTypeName.ToLower() == strPType).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrProgramCourseCode(string strCourseCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCourseCode = strCourseCode.ToLower();

                    var dbCourse = db.organization_program_course.Where(c => c.CourseCode == strCourseCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrStudentCode(string strStudentCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strStudentCode = strStudentCode.ToLower();

                    var dbCourse = db.employee.Where(c => c.function.name.ToLower() == "student" && c.employee_code == strStudentCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateEnrStudentCodeCampus(string strCampusCode, string strStudentCode)
        {
            int iCampusID = 0;
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strStudentCode = strStudentCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        iCampusID = dbCampus.Id;
                    }

                    var dbCourse = db.employee.Where(c => c.campus_id == iCampusID && c.function.name.ToLower() == "student" && c.employee_code == strStudentCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

    }


    #region Enrollment-Report

    public class EnrollmentReportData
    {
        public string org_title { get; set; }
        public string org_logo_path { get; set; }
        public string year_code { get; set; }
        public string campus_code { get; set; }
        public string program_code { get; set; }
        public string date_range { get; set; }

        public List<EnrollmentStudentLog> std_logs { get; set; }
    }

    public class EnrollmentStudentLog
    {
        public int year_id { get; set; }
        public int campus_id { get; set; }
        public int lgroup_id { get; set; }
        public int program_id { get; set; }

        public string student_code { get; set; }
        public string student_name { get; set; }
        public string lgroup_name { get; set; }
        public string enrollment_title { get; set; }

        public string crs_list { get; set; }

        public int program_type_id { get; set; }
        public string program_type_name { get; set; }
        public int program_type_number { get; set; }
        public bool is_course_failed { get; set; }
        public string is_course_failed_name { get; set; }

    }


    public class EnrollmentReport
    {
        public EnrollmentReportData getEnrollmentData(int iGCYearID, int iCampusId, int iProgId)
        {
            List<DLL.Models.OrganizationProgramCourseEnrollment> dbEnrollment = null;
            List<Employee> employees_list = new List<Employee>();
            EnrollmentReportData toReturn = null;

            using (var db = new Context())
            {
                toReturn = new EnrollmentReportData();
                List<EnrollmentStudentLog> enrolled_student_log = new List<EnrollmentStudentLog>();

                var dbOrgCampus = (from c in db.organization_campus
                                   join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                   join p in db.organization_program on cp.ProgramId equals p.Id
                                   join o in db.organization on c.OrganizationId equals o.Id
                                   where c.Id == iCampusId && p.Id == iProgId && cp.IsActiveProgram
                                   select new
                                   {
                                       org_name = o.OrganizationTitle,
                                       org_logo = o.Logo,
                                       org_url = o.WebsiteURL,
                                       campus_name = c.CampusCode + "-" + c.CampusTitle,
                                       program_name = p.ProgramCode + "-" + p.ProgramTitle
                                   }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.year_code = db.general_calender.Where(x => x.GeneralCalendarId == iGCYearID).FirstOrDefault().year.ToString();
                    toReturn.org_title = dbOrgCampus.org_name;
                    toReturn.org_logo_path = dbOrgCampus.org_logo;
                    toReturn.campus_code = dbOrgCampus.campus_name;
                    toReturn.program_code = dbOrgCampus.program_name;
                }

                dbEnrollment = db.organization_program_course_enrollment.Where(s => s.GeneralCalendarId == iGCYearID && s.CampusId == iCampusId && s.ProgramId == iProgId).ToList();
                if (dbEnrollment != null && dbEnrollment.Count > 0)
                {
                    foreach (var e in dbEnrollment)
                    {
                        int iCampusID = 0, iLGroupID = 0; string strStudentCode = "", strStudentName = "", strLGroupName = "";
                        var dbStudentInfo = db.employee.Where(p => p.EmployeeId == e.EmployeeStudentId).FirstOrDefault();
                        if (dbStudentInfo != null)
                        {
                            iCampusID = dbStudentInfo.campus_id;
                            iLGroupID = dbStudentInfo.region.RegionId;
                            strLGroupName = dbStudentInfo.region.name;
                            strStudentCode = dbStudentInfo.employee_code;
                            strStudentName = dbStudentInfo.first_name + " " + dbStudentInfo.last_name;
                        }

                        //int program_id = 0;
                        //var dbProgCourse = db.organization_program_course.Where(p => p.ProgramId == e.ProgramCourseId).FirstOrDefault();
                        //if (dbProgCourse != null)
                        //{
                        //    program_id = dbProgCourse.ProgramId;

                        //    var dbProg = db.organization_program.Where(p => p.Id == program_id).FirstOrDefault();
                        //    if (dbProg != null)
                        //    {
                        //        //toReturn.program_code = dbProg.ProgramCode + "-" + dbProg.ProgramTitle;
                        //    }
                        //}

                        //string strCoursesList = "";
                        //var dbStdEnr = db.organization_program_course_enrollment.Where(x => x.GeneralCalendarId == iGCYearID &&
                        //                    x.CampusId == iCampusId && x.ProgramId == iProgId && x.EmployeeStudentId == e.EmployeeStudentId).FirstOrDefault();
                        //if (dbStdEnr != null)
                        //{
                        //    var dbCourses = db.organization_program_course.Where(p => p.Id == dbStdEnr.ProgramCourseId).ToList();
                        //    if (dbCourses != null)
                        //    {
                        //        foreach (var cc in dbCourses)
                        //        {
                        //            strCoursesList += cc.CourseCode + ",";
                        //        }

                        //    }
                        //}

                        string strCourse = "";
                        var dbCourse = db.organization_program_course.Where(p => p.Id == e.ProgramCourseId).FirstOrDefault();
                        if (dbCourse != null)
                        {
                            strCourse = dbCourse.CourseCode + "-" + dbCourse.CourseTitle;
                        }


                        string strProgType = "";
                        var dbProgType = db.organization_program_type.Where(p => p.Id == e.EnrolledProgramTypeId).FirstOrDefault();
                        if (dbProgType != null)
                        {
                            strProgType = dbProgType.ProgramTypeName;
                        }

                        EnrollmentStudentLog eLog = new EnrollmentStudentLog()
                        {
                            campus_id = iCampusId,
                            program_id = iProgId,
                            lgroup_id = iLGroupID,//region
                            student_code = strStudentCode,
                            student_name = strStudentName,
                            lgroup_name = strLGroupName,//region
                            enrollment_title = e.EnrollmentTitle,
                            crs_list = strCourse ?? "",
                            program_type_id = e.EnrolledProgramTypeId,
                            program_type_name = strProgType,
                            program_type_number = e.EnrolledProgramTypeNumber,
                            is_course_failed = e.IsCourseFailed,
                            is_course_failed_name = e.IsCourseFailed == false ? "No" : "Yes"
                        };

                        enrolled_student_log.Add(eLog);
                    }

                    toReturn.std_logs = enrolled_student_log;
                }
            }

            return toReturn;
        }


        public EnrollmentReportData getEnrollmentData_BACKUP(int iGCYearID, int iCampusId, int iProgId)
        {
            List<DLL.Models.OrganizationProgramCourseEnrollment> dbEnrollment = null;
            List<Employee> employees_list = new List<Employee>();
            EnrollmentReportData toReturn = null;

            using (var db = new Context())
            {
                toReturn = new EnrollmentReportData();
                List<EnrollmentStudentLog> enrolled_student_log = new List<EnrollmentStudentLog>();

                var dbOrgCampus = (from c in db.organization_campus
                                   join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                   join p in db.organization_program on cp.ProgramId equals p.Id
                                   join o in db.organization on c.OrganizationId equals o.Id
                                   where c.Id == iCampusId && p.Id == iProgId && cp.IsActiveProgram
                                   select new
                                   {
                                       org_name = o.OrganizationTitle,
                                       org_logo = o.Logo,
                                       org_url = o.WebsiteURL,
                                       campus_name = c.CampusCode + "-" + c.CampusTitle,
                                       program_name = p.ProgramCode + "-" + p.ProgramTitle
                                   }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.year_code = db.general_calender.Where(x => x.GeneralCalendarId == iGCYearID).FirstOrDefault().year.ToString();
                    toReturn.org_title = dbOrgCampus.org_name;
                    toReturn.org_logo_path = dbOrgCampus.org_logo;
                    toReturn.campus_code = dbOrgCampus.campus_name;
                    toReturn.program_code = dbOrgCampus.program_name;
                }

                int xCampusID = 0, xStudentID = 0;
                var dbStudentCampus = db.employee.Where(x => x.campus_id == iCampusId && x.function.name.ToLower() == "student").ToList();
                if (dbStudentCampus != null && dbStudentCampus.Count > 0)
                {
                    foreach (var std in dbStudentCampus)
                    {
                        xCampusID = std.campus_id;
                        xStudentID = std.EmployeeId;

                        dbEnrollment = db.organization_program_course_enrollment.Where(s => s.GeneralCalendarId == iGCYearID && s.EmployeeStudentId == xStudentID).ToList();
                        if (dbEnrollment != null && dbEnrollment.Count > 0)
                        {
                            foreach (var e in dbEnrollment)
                            {
                                var dbProgram = db.organization_program_course.Where(x => x.Id == e.ProgramCourseId && x.ProgramId == iProgId).FirstOrDefault();
                                if (dbProgram != null)
                                {
                                    int iCampusID = 0, iLGroupID = 0; string strStudentCode = "", strStudentName = "", strLGroupName = "";
                                    var dbStudentInfo = db.employee.Where(p => p.EmployeeId == e.EmployeeStudentId).FirstOrDefault();
                                    if (dbStudentInfo != null)
                                    {
                                        iCampusID = dbStudentInfo.campus_id;
                                        iLGroupID = dbStudentInfo.region.RegionId;
                                        strLGroupName = dbStudentInfo.region.name;
                                        strStudentCode = dbStudentInfo.employee_code;
                                        strStudentName = dbStudentInfo.first_name + " " + dbStudentInfo.last_name;
                                    }

                                    int program_id = 0;
                                    var dbProgCourse = db.organization_program_course.Where(p => p.ProgramId == e.ProgramCourseId).FirstOrDefault();
                                    if (dbProgCourse != null)
                                    {
                                        program_id = dbProgCourse.ProgramId;

                                        var dbProg = db.organization_program.Where(p => p.Id == program_id).FirstOrDefault();
                                        if (dbProg != null)
                                        {
                                            //toReturn.program_code = dbProg.ProgramCode + "-" + dbProg.ProgramTitle;
                                        }
                                    }

                                    string strCoursesList = "";
                                    var dbStdEnr = db.organization_program_course_enrollment.Where(x => x.GeneralCalendarId == iGCYearID && x.EmployeeStudentId == e.EmployeeStudentId).FirstOrDefault();
                                    if (dbStdEnr != null)
                                    {
                                        var dbCourses = db.organization_program_course.Where(p => p.Id == dbStdEnr.ProgramCourseId).ToList();
                                        if (dbCourses != null)
                                        {
                                            foreach (var cc in dbCourses)
                                            {
                                                strCoursesList += cc.CourseCode + ",";
                                            }

                                        }
                                    }

                                    string strProgType = "";
                                    var dbProgType = db.organization_program_type.Where(p => p.Id == e.EnrolledProgramTypeId).FirstOrDefault();
                                    if (dbProgType != null)
                                    {
                                        strProgType = dbProgType.ProgramTypeName;
                                    }

                                    EnrollmentStudentLog eLog = new EnrollmentStudentLog()
                                    {
                                        campus_id = iCampusID,
                                        program_id = program_id,
                                        lgroup_id = iLGroupID,//region
                                        student_code = strStudentCode,
                                        student_name = strStudentName,
                                        lgroup_name = strLGroupName,//region
                                        enrollment_title = e.EnrollmentTitle,
                                        crs_list = strCoursesList ?? "",
                                        program_type_id = e.EnrolledProgramTypeId,
                                        program_type_name = strProgType,
                                        program_type_number = e.EnrolledProgramTypeNumber,
                                        is_course_failed = e.IsCourseFailed,
                                        is_course_failed_name = e.IsCourseFailed == false ? "No" : "Yes"
                                    };

                                    enrolled_student_log.Add(eLog);
                                }
                            }
                        }
                    }

                    toReturn.std_logs = enrolled_student_log;
                }
            }

            return toReturn;
        }




    }

    #endregion




    #endregion



    #region Course-Attendance-Report

    public class CourseAttendanceReportData
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }

        public string orgName { get; set; }
        public string orgLogo { get; set; }
        public string orgWebUrl { get; set; }
        public string campusName { get; set; }
        public string progCode { get; set; }
        public string progTitle { get; set; }

        public string employeeName { get; set; }
        public string employeeCode { get; set; }
        public string employeeFatherName { get; set; }
        public int employeeRGroupID { get; set; }
        public string employeeRGroupName { get; set; }

        public string dateRange { get; set; }

        public string enrolledProgramText { get; set; }
        public string enrolledProgramNumber { get; set; }

        public CourseAttendanceLog[] logs { get; set; }
        public CourseAttendanceCountLog[] counts { get; set; }

        public string totalPresent { get; set; }
        public string totalAbsent { get; set; }
        public string totalOFF { get; set; }
        public string totalLate { get; set; }
        public string totalEarlyOut { get; set; }
        public string totalLeave { get; set; }
        public string totalDays { get; set; }
    }

    public class CourseAttendanceLog
    {
        //      year_code schedule_date_time  campus_id campus_code program_id program_code    student_id student_code    course_id course_code student_time_in status_in   
        //      student_time_out status_out  final_remarks course_time_start   course_time_end terminal_in_id  terminal_in_name terminal_out_id terminal_out_name active

        public int year_code { get; set; }
        public DateTime schedule_date_time { get; set; }
        public string schedule_date_time_text { get; set; }
        public int campus_id { get; set; }
        public string campus_code { get; set; }
        public int program_id { get; set; }
        public string program_code { get; set; }
        public string program_title { get; set; }

        public int enrolled_program_id { get; set; }
        public string enrolled_program_name { get; set; }
        public int enrolled_program_number { get; set; }

        public int student_id { get; set; }
        public int student_lgroup_id { get; set; }
        public int schedule_lgroup_id { get; set; }
        public int student_pshift_id { get; set; }
        public int schedule_pshift_id { get; set; }
        public string student_code { get; set; }
        public string student_name { get; set; }
        public string student_father_name { get; set; }
        public int course_id { get; set; }
        public string course_code { get; set; }
        public string course_title { get; set; }
        public DateTime? student_time_in { get; set; }
        public string student_time_in_text { get; set; }
        public string status_in { get; set; }
        public DateTime? student_time_out { get; set; }
        public string student_time_out_text { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public DateTime course_time_start { get; set; }
        public string course_time_start_text { get; set; }
        public DateTime course_time_end { get; set; }
        public string course_time_end_text { get; set; }

        public int terminal_in_id { get; set; }
        public string terminal_in_code { get; set; }
        public int terminal_out_id { get; set; }
        public string terminal_out_code { get; set; }

        public bool active { get; set; }
        public string active_text { get; set; }

    }

    public class CourseAttendanceCountLog
    {
        public string student_code { get; set; }
        public string student_info { get; set; }
        public string course_code { get; set; }
        public string course_code_title { get; set; }

        public int total_count { get; set; }
        public int present_count { get; set; }
        public int absent_count { get; set; }
        public int off_count { get; set; }
        public int late_count { get; set; }
        public int early_count { get; set; }

        public decimal per_present { get; set; }
        public decimal per_absent { get; set; }

    }


    public class CourseAttendanceReport
    {
        public CourseAttendanceReportData getCourseAttendanceStudentReport(string from_date, string to_date, int studentID)
        {
            CourseAttendanceReportData toReturn = null;

            using (var db = new Context())
            {
                toReturn = new CourseAttendanceReportData();

                DLL.Models.Employee emp = db.employee.Find(studentID);

                if (emp == null)
                    return null;

                var dbOrgCampus = (from e in db.employee
                                   join c in db.organization_campus on e.campus_id equals c.Id
                                   join o in db.organization on c.OrganizationId equals o.Id
                                   where e.EmployeeId == studentID
                                   select new
                                   {
                                       org_name = o.OrganizationTitle,
                                       org_logo = o.Logo,
                                       org_url = o.WebsiteURL,
                                       campus_name = c.CampusCode + "-" + c.CampusTitle
                                   }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.orgName = dbOrgCampus.org_name;
                    toReturn.orgLogo = dbOrgCampus.org_logo ?? "/Content/Logos/logo-default.png";
                    toReturn.orgWebUrl = dbOrgCampus.org_url ?? "";
                    toReturn.campusName = dbOrgCampus.campus_name;
                    //toReturn.progName = dbOrgCampus.program_name;
                }

                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;
                toReturn.employeeFatherName = emp.father_name;
                toReturn.employeeRGroupID = emp.region.RegionId;
                toReturn.employeeRGroupName = emp.region.name;

                DateTime dtFromDate = DateTime.ParseExact(from_date + " 00:00:00.000", "dd-MM-yyyy HH:mm:00.000", CultureInfo.InvariantCulture);
                DateTime dtToDate = DateTime.ParseExact(to_date + " 23:59:00.000", "dd-MM-yyyy HH:mm:00.000", CultureInfo.InvariantCulture);

                toReturn.dateRange = dtFromDate.ToString("ddd, dd-MMM-yyyy") + " TO " + dtToDate.ToString("ddd, dd-MMM-yyyy");

                //OrganizationCourseAttendance[] attendanceLogs = db.organization_course_attendance.Where(m =>
                //     m.employee_student_id.Equals(emp.EmployeeId) &&
                //     (m.schedule_date_time >= dtFromDate && m.schedule_date_time <= dtToDate)).ToArray();

                List<CourseAttendanceLog> caRawLogs = new List<CourseAttendanceLog>();
                caRawLogs = CourseAttendanceReport.getCourseAttendanceStudentList(dtFromDate, dtToDate, studentID);

                if (caRawLogs != null && caRawLogs.Count > 0)
                {
                    List<CourseAttendanceLog> tempLogs = new List<CourseAttendanceLog>();
                    List<CourseAttendanceCountLog> countLogs = new List<CourseAttendanceCountLog>();

                    foreach (CourseAttendanceLog log in caRawLogs)
                    {
                        toReturn.progCode = log.program_code;
                        toReturn.progTitle = log.program_title;
                        toReturn.enrolledProgramText = log.enrolled_program_name;
                        toReturn.enrolledProgramNumber = log.enrolled_program_number.ToString();

                        ////if (log.schedule_lgroup_id == 0 || log.schedule_lgroup_id == log.student_lgroup_id)
                        {
                            tempLogs.Add(new CourseAttendanceLog()
                            {
                                schedule_date_time_text = (log.schedule_date_time != null && log.schedule_date_time.ToString() != "") ? log.schedule_date_time.ToString("dd-MMM-yyyy") : "",
                                course_code = log.course_code,
                                course_title = log.course_title,
                                student_time_in_text = log.final_remarks == "OFF" ? "" : ((log.student_time_in.HasValue && log.student_time_in.Value.ToString() != "") ? log.student_time_in.Value.ToString("hh:mm tt") : "") + " [" + log.course_time_start.ToString("hh:mm tt") + "]",
                                status_in = log.status_in,
                                student_time_out_text = log.final_remarks == "OFF" ? "" : ((log.student_time_out.HasValue && log.student_time_out.Value.ToString() != "") ? log.student_time_out.Value.ToString("hh:mm tt") : "") + " [" + log.course_time_end.ToString("hh:mm tt") + "]",
                                status_out = log.status_out,
                                final_remarks = log.final_remarks,
                                terminal_in_id = log.terminal_in_id,
                                terminal_in_code = log.terminal_in_code,
                                terminal_out_id = log.terminal_out_id,
                                terminal_out_code = log.terminal_out_code
                            });
                        }
                    }

                    //Count Summary
                    var lstStudents = caRawLogs.GroupBy(s => s.student_code).OrderBy(o => o.Key).ToList();
                    var lstCourses = caRawLogs.Where(s => (s.schedule_lgroup_id == 0 || s.schedule_lgroup_id == toReturn.employeeRGroupID)).GroupBy(s => s.course_code).OrderBy(o => o.Key).ToList();
                    var lstALLInfo = caRawLogs.Select(s => new { s_code = s.student_code, s_name = s.student_name, s_fname = s.student_father_name, c_code = s.course_code, c_title = s.course_title, std_lgroup_id = s.student_lgroup_id, sch_lgroup_id = s.schedule_lgroup_id, std_pshift_id = s.student_pshift_id, sch_pshift_id = s.schedule_pshift_id }).OrderBy(o => o.s_code).ToList();
                    //var lstCourseInfo = caRawLogs.GroupBy(s => s.course_code + "-" + s.course_title).OrderBy(o => o.Key).ToList();

                    List<CourseAttendanceLog> stdLogs = new List<CourseAttendanceLog>();

                    if (lstStudents != null && lstStudents.Count > 0 && lstCourses != null && lstCourses.Count > 0)
                    {
                        int sumTotalClasses = 0, sumPresent = 0, sumAbsent = 0, sumLate = 0, sumEarlyOut = 0, sumOff = 0;
                        decimal perPresent = 0, perAbsent = 0;
                        int tCount = 0, pCount = 0, aCount = 0, lCount = 0, eCount = 0, oCount = 0;

                        foreach (var s in lstStudents)
                        {
                            sumTotalClasses = 0; sumPresent = 0; sumAbsent = 0; sumLate = 0; sumEarlyOut = 0; sumOff = 0;
                            perPresent = 0; perAbsent = 0;

                            stdLogs = caRawLogs.Where(c => c.student_code == s.Key).ToList();

                            foreach (var c in lstCourses)
                            {
                                ////if (lstALLInfo.Where(i => i.s_code == s.Key).FirstOrDefault().sch_lgroup_id == 0 || lstALLInfo.Where(i => i.s_code == s.Key).FirstOrDefault().sch_lgroup_id == lstALLInfo.Where(i => i.s_code == s.Key).FirstOrDefault().std_lgroup_id)
                                {
                                    tCount = 0; pCount = 0; aCount = 0; lCount = 0; eCount = 0; oCount = 0;

                                    pCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 0);
                                    aCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 1);
                                    tCount = pCount + aCount;

                                    oCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 2);
                                    lCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 3);
                                    eCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 4);

                                    perPresent = (tCount == 0) ? 0.00M : (decimal.Parse(pCount.ToString()) / decimal.Parse(tCount.ToString()));
                                    perAbsent = (tCount == 0) ? 0.00M : (decimal.Parse(aCount.ToString()) / decimal.Parse(tCount.ToString()));

                                    countLogs.Add(new CourseAttendanceCountLog()
                                    {
                                        student_code = s.Key,
                                        course_code = c.Key,
                                        course_code_title = lstALLInfo.Where(i => i.c_code == c.Key).FirstOrDefault().c_code + " - " + lstALLInfo.Where(i => i.c_code == c.Key).FirstOrDefault().c_title, //lstCourseInfo.Find(f => f.Key.ToString().Contains(c.Key)).Key,
                                        total_count = tCount,
                                        present_count = pCount,
                                        absent_count = aCount,
                                        off_count = oCount,
                                        late_count = lCount,
                                        early_count = eCount,
                                        per_present = perPresent,
                                        per_absent = perAbsent
                                    });

                                    sumTotalClasses += tCount; sumPresent += pCount; sumAbsent += aCount;
                                    sumLate += lCount; sumEarlyOut += eCount; sumOff += oCount;
                                }
                            }

                            //last row showing SUM only
                            perPresent = (sumTotalClasses == 0.00M) ? 0 : (decimal.Parse(sumPresent.ToString()) / decimal.Parse(sumTotalClasses.ToString()));
                            perAbsent = (sumTotalClasses == 0.00M) ? 0 : (decimal.Parse(sumAbsent.ToString()) / decimal.Parse(sumTotalClasses.ToString()));

                            countLogs.Add(new CourseAttendanceCountLog()
                            {
                                student_code = "SUM",
                                course_code = "SUM + Per%",
                                course_code_title = "SUM",
                                total_count = sumTotalClasses,
                                present_count = sumPresent,
                                absent_count = sumAbsent,
                                off_count = sumOff,
                                late_count = sumLate,
                                early_count = sumEarlyOut,
                                per_present = perPresent,
                                per_absent = perAbsent
                            });
                            /////////////////////////////////////////////

                        }
                    }


                    toReturn.logs = tempLogs.AsQueryable().ToArray();
                    toReturn.counts = countLogs.AsQueryable().ToArray();

                    //Present
                    toReturn.totalPresent = tempLogs.Where(m =>
                        !m.final_remarks.Equals(DLL.Commons.FinalRemarks.ABSENT) && !m.final_remarks.Equals(DLL.Commons.FinalRemarks.LV) && !m.final_remarks.Equals(DLL.Commons.FinalRemarks.OFF)
                        ).Count() + "";

                    //Absent
                    toReturn.totalAbsent = tempLogs.Where(m => m.final_remarks.Equals(DLL.Commons.FinalRemarks.ABSENT)).Count() + "";

                    //OFF
                    toReturn.totalOFF = tempLogs.Where(m => m.final_remarks.Equals(DLL.Commons.FinalRemarks.OFF)).Count() + "";

                    //Leave
                    toReturn.totalLeave = tempLogs.Where(m => m.final_remarks.Equals(DLL.Commons.FinalRemarks.LV)).Count() + "";

                    //Late
                    toReturn.totalLate = tempLogs.Where(m =>
                        m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                        m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                        m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLO)
                        ).Count() + "";

                    //Early Out
                    toReturn.totalEarlyOut = tempLogs.Where(m =>
                        m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                        //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                        //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PME) ||
                        m.final_remarks.Equals(DLL.Commons.FinalRemarks.POE)
                        ).Count() + "";

                    //Total Days
                    toReturn.totalDays = tempLogs.Where(m =>
                        !m.final_remarks.Equals(DLL.Commons.FinalRemarks.OFF)
                        ).Count() + "";
                }
            }

            return toReturn;

        }

        public int getCourseAttendanceCount(List<CourseAttendanceLog> listData, string std_code, string course_code, int rem_type)
        {
            int retCount = 0;

            if (rem_type == 0) // present
            {
                retCount = listData.Where(l => l.student_code == std_code && l.course_code == course_code && l.final_remarks.ToUpper().Contains("P")).Count();
            }
            else if (rem_type == 1) // absent
            {
                retCount = listData.Where(l => l.student_code == std_code && l.course_code == course_code && l.final_remarks.ToUpper().Contains("AB")).Count();
            }
            else if (rem_type == 2) // off
            {
                retCount = listData.Where(l => l.student_code == std_code && l.course_code == course_code && l.final_remarks.ToUpper().Contains("OFF")).Count();
            }
            else if (rem_type == 3) // late
            {
                retCount = listData.Where(l => l.student_code == std_code && l.course_code == course_code && l.final_remarks.ToUpper().Contains("L")).Count();
            }
            else if (rem_type == 4) // early
            {
                retCount = listData.Where(l => l.student_code == std_code && l.course_code == course_code && l.final_remarks.ToUpper().Contains("E")).Count();
            }
            else
            {
                retCount = 0;
            }

            return retCount;
        }

        public CourseAttendanceReportData getCourseAttendanceCampusReport(string from_date, string to_date, int campusID, int programID, int programTypeID, int programTypeNumber, int programShiftID, int programGroupID)
        {
            CourseAttendanceReportData toReturn = null;

            using (var db = new Context())
            {
                OrganizationView dbOrgCampus = new OrganizationView();
                toReturn = new CourseAttendanceReportData();

                dbOrgCampus = (from c in db.organization_campus
                               join o in db.organization on c.OrganizationId equals o.Id
                               where c.Id == campusID
                               select new OrganizationView()
                               {
                                   OrganizationTitle = o.OrganizationTitle,
                                   Logo = o.Logo,
                                   WebsiteURL = o.WebsiteURL,
                                   Description = c.CampusCode + "-" + c.CampusTitle
                               }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.orgName = dbOrgCampus.OrganizationTitle;
                    toReturn.orgLogo = dbOrgCampus.Logo ?? "/Content/Logos/logo-default.png";
                    toReturn.orgWebUrl = dbOrgCampus.WebsiteURL ?? "";
                    toReturn.campusName = dbOrgCampus.Description;
                    //toReturn.progName = dbOrgCampus.program_name;
                }

                DateTime dtFromDate = DateTime.ParseExact(from_date + " 00:00:00.000", "dd-MM-yyyy HH:mm:00.000", CultureInfo.InvariantCulture);
                DateTime dtToDate = DateTime.ParseExact(to_date + " 23:59:00.000", "dd-MM-yyyy HH:mm:00.000", CultureInfo.InvariantCulture);

                toReturn.dateRange = dtFromDate.ToString("ddd, dd-MMM-yyyy") + " TO " + dtToDate.ToString("ddd, dd-MMM-yyyy");

                //OrganizationCourseAttendance[] attendanceLogs = db.organization_course_attendance.Where(m =>
                //     m.employee_student_id.Equals(emp.EmployeeId) &&
                //     (m.schedule_date_time >= dtFromDate && m.schedule_date_time <= dtToDate)).ToArray();


                List<CourseAttendanceLog> caRawLogs = new List<CourseAttendanceLog>();
                caRawLogs = CourseAttendanceReport.getCourseAttendanceCampusList(dtFromDate, dtToDate, campusID, programID, programTypeID, programTypeNumber, programShiftID, programGroupID);
                if (caRawLogs != null && caRawLogs.Count > 0)
                {
                    List<CourseAttendanceLog> tempLogs = new List<CourseAttendanceLog>();
                    List<CourseAttendanceCountLog> countLogs = new List<CourseAttendanceCountLog>();

                    foreach (CourseAttendanceLog log in caRawLogs)
                    {
                        toReturn.progCode = log.program_code;
                        toReturn.progTitle = log.program_title;
                        toReturn.enrolledProgramText = log.enrolled_program_name;
                        toReturn.enrolledProgramNumber = log.enrolled_program_number.ToString();

                        //if (log.lecture_group_id == 0 || log.lecture_group_id == toReturn.employeeRGroupID)
                        //{
                        //tempLogs.Add(new CourseAttendanceLog()
                        //{
                        //    schedule_date_time_text = (log.schedule_date_time != null && log.schedule_date_time.ToString() != "") ? log.schedule_date_time.ToString("dd-MMM-yyyy") : "",
                        //    course_code = log.course_code,
                        //    course_title = log.course_title,
                        //    student_time_in_text = log.final_remarks == "OFF" ? "" : ((log.student_time_in.HasValue && log.student_time_in.Value.ToString() != "") ? log.student_time_in.Value.ToString("hh:mm tt") : "") + "\r\n[" + log.course_time_start.ToString("hh:mm tt") + "]",
                        //    status_in = log.status_in,
                        //    student_time_out_text = log.final_remarks == "OFF" ? "" : ((log.student_time_out.HasValue && log.student_time_out.Value.ToString() != "") ? log.student_time_out.Value.ToString("hh:mm tt") : "") + "\r\n[" + log.course_time_end.ToString("hh:mm tt") + "]",
                        //    status_out = log.status_out,
                        //    final_remarks = log.final_remarks,
                        //    terminal_in_id = log.terminal_in_id,
                        //    terminal_in_code = log.terminal_in_code
                        //});
                        //}
                        break;
                    }

                    //Count Summary
                    //var lstStudents = caRawLogs.GroupBy(s => s.student_code).OrderBy(o => o.Key).ToList();
                    //var lstStudentInfo = caRawLogs.GroupBy(s => s.student_code + ", Name: " + s.student_name + ", Father's Name: " + s.student_father_name).OrderBy(o => o.Key).ToList();
                    //var lstCourses = caRawLogs.GroupBy(s => s.course_code).OrderBy(o => o.Key).ToList();
                    //var lstCourseInfo = caRawLogs.GroupBy(s => s.course_code + "-" + s.course_title).OrderBy(o => o.Key).ToList();
                    var lstStudents = caRawLogs.GroupBy(s => s.student_code).OrderBy(o => o.Key).ToList();
                    var lstCourses = caRawLogs.GroupBy(s => s.course_code).OrderBy(o => o.Key).ToList();
                    var lstALLInfo = caRawLogs.Select(s => new { s_code = s.student_code, s_name = s.student_name, s_fname = s.student_father_name, c_code = s.course_code, c_title = s.course_title, std_lgroup_id = s.student_lgroup_id, sch_lgroup_id = s.schedule_lgroup_id, std_pshift_id = s.student_pshift_id, sch_pshift_id = s.schedule_pshift_id }).OrderBy(o => o.s_code).ToList();

                    List<CourseAttendanceLog> stdLogs = new List<CourseAttendanceLog>();

                    if (lstStudents != null && lstStudents.Count > 0 && lstCourses != null && lstCourses.Count > 0)
                    {
                        int sumTotalClasses = 0, sumPresent = 0, sumAbsent = 0, sumLate = 0, sumEarlyOut = 0, sumOff = 0;
                        decimal perPresent = 0, perAbsent = 0;
                        int tCount = 0, pCount = 0, aCount = 0, lCount = 0, eCount = 0, oCount = 0;

                        foreach (var s in lstStudents)
                        {
                            sumTotalClasses = 0; sumPresent = 0; sumAbsent = 0; sumLate = 0; sumEarlyOut = 0; sumOff = 0;
                            perPresent = 0; perAbsent = 0;

                            stdLogs = caRawLogs.Where(c => c.student_code == s.Key).ToList();

                            foreach (var c in lstCourses)
                            {
                                tCount = 0; pCount = 0; aCount = 0; lCount = 0; eCount = 0; oCount = 0;

                                pCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 0);
                                aCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 1);
                                tCount = pCount + aCount;

                                oCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 2);
                                lCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 3);
                                eCount = getCourseAttendanceCount(stdLogs, s.Key, c.Key, 4);

                                perPresent = (tCount == 0) ? 0.00M : (decimal.Parse(pCount.ToString()) / decimal.Parse(tCount.ToString()));
                                perAbsent = (tCount == 0) ? 0.00M : (decimal.Parse(aCount.ToString()) / decimal.Parse(tCount.ToString()));

                                string strStdInfo = "", strCrsInfo = ""; int iStdLecGroupID = 0, iSchLecGroupID = 0;
                                var stdStudentData = lstALLInfo.Where(f => f.s_code == s.Key).FirstOrDefault();
                                if (stdStudentData != null)
                                {
                                    strStdInfo = stdStudentData.s_code + ", Name: " + stdStudentData.s_name + ", Father Name: " + stdStudentData.s_fname;
                                    iStdLecGroupID = stdStudentData.std_lgroup_id;
                                    iSchLecGroupID = stdStudentData.sch_lgroup_id;
                                }

                                var stdCourseData = lstALLInfo.Where(f => f.c_code == c.Key).FirstOrDefault();
                                if (stdCourseData != null)
                                {
                                    strCrsInfo = stdCourseData.c_code + "-" + stdCourseData.c_title;
                                }

                                // if (lstALLInfo.Where(i => i.s_code == s.Key).FirstOrDefault().sch_lgroup_id == 0 || lstALLInfo.Where(i => i.s_code == s.Key).FirstOrDefault().sch_lgroup_id == lstALLInfo.Where(i => i.s_code == s.Key).FirstOrDefault().std_lgroup_id)
                                ////if (iSchLecGroupID == 0 || iSchLecGroupID == iStdLecGroupID)
                                {
                                    countLogs.Add(new CourseAttendanceCountLog()
                                    {
                                        student_code = s.Key,
                                        student_info = strStdInfo, //lstALLInfo.Find(f => f.Key.ToString().Contains(s.Key)).Key,
                                        //student_info
                                        course_code = c.Key,
                                        course_code_title = strCrsInfo,//lstCourseInfo.Find(f => f.Key.ToString().Contains(c.Key)).Key,
                                        //lstALLInfo.Where(i => i.c_code == c.Key).FirstOrDefault().c_code + " - " + lstALLInfo.Where(i => i.c_code == c.Key).FirstOrDefault().c_title,
                                        total_count = tCount,
                                        present_count = pCount,
                                        absent_count = aCount,
                                        off_count = oCount,
                                        late_count = lCount,
                                        early_count = eCount,
                                        per_present = perPresent,
                                        per_absent = perAbsent
                                    });

                                    sumTotalClasses += tCount; sumPresent += pCount; sumAbsent += aCount;
                                    sumLate += lCount; sumEarlyOut += eCount; sumOff += oCount;
                                }
                            }

                            //last row showing SUM only
                            perPresent = (sumTotalClasses == 0) ? 0.00M : (decimal.Parse(sumPresent.ToString()) / decimal.Parse(sumTotalClasses.ToString()));
                            perAbsent = (sumTotalClasses == 0) ? 0.00M : (decimal.Parse(sumAbsent.ToString()) / decimal.Parse(sumTotalClasses.ToString()));

                            countLogs.Add(new CourseAttendanceCountLog()
                            {
                                student_code = "",
                                student_info = "SUM",
                                course_code = "",
                                course_code_title = "SUM",
                                total_count = sumTotalClasses,
                                present_count = sumPresent,
                                absent_count = sumAbsent,
                                off_count = sumOff,
                                late_count = sumLate,
                                early_count = sumEarlyOut,
                                per_present = perPresent,
                                per_absent = perAbsent
                            });
                            /////////////////////////////////////////////

                        }
                    }

                    ////toReturn.logs = tempLogs.AsQueryable().ToArray();
                    toReturn.counts = countLogs.AsQueryable().ToArray();

                    ////Present
                    //toReturn.totalPresent = tempLogs.Where(m =>
                    //    !m.final_remarks.Equals(DLL.Commons.FinalRemarks.ABSENT) && !m.final_remarks.Equals(DLL.Commons.FinalRemarks.LV) && !m.final_remarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    //    ).Count() + "";

                    ////Absent
                    //toReturn.totalAbsent = tempLogs.Where(m => m.final_remarks.Equals(DLL.Commons.FinalRemarks.ABSENT)).Count() + "";

                    ////OFF
                    //toReturn.totalOFF = tempLogs.Where(m => m.final_remarks.Equals(DLL.Commons.FinalRemarks.OFF)).Count() + "";

                    ////Leave
                    //toReturn.totalLeave = tempLogs.Where(m => m.final_remarks.Equals(DLL.Commons.FinalRemarks.LV)).Count() + "";

                    ////Late
                    //toReturn.totalLate = tempLogs.Where(m =>
                    //    m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                    //    m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                    //    m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLO)
                    //    ).Count() + "";

                    ////Early Out
                    //toReturn.totalEarlyOut = tempLogs.Where(m =>
                    //    m.final_remarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                    //    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                    //    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PME) ||
                    //    m.final_remarks.Equals(DLL.Commons.FinalRemarks.POE)
                    //    ).Count() + "";

                    ////Total Days
                    //toReturn.totalDays = tempLogs.Where(m =>
                    //    !m.final_remarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    //    ).Count() + "";
                }
            }

            return toReturn;

        }

        public static List<CourseAttendanceLog> getCourseAttendanceStudentList(DateTime dtFromDate, DateTime dtToDate, int iStudentID)
        {
            List<CourseAttendanceLog> listCALogs = new List<CourseAttendanceLog>();

            using (var db = new Context())
            {
                db.Database.CommandTimeout = 600;

                listCALogs = db.Database.SqlQuery<CourseAttendanceLog>(string.Format("SP_GetCourseAttendanceStudentReport '{0}','{1}',{2}", dtFromDate, dtToDate, iStudentID)).ToList();
            }

            return listCALogs;
        }

        public static List<CourseAttendanceLog> getCourseAttendanceCampusList(DateTime dtFromDate, DateTime dtToDate, int iCampusID, int iProgramID, int iProgramTypeID, int iProgramTypeNumber, int iProgramShiftID, int iProgramGroupID)
        {
            List<CourseAttendanceLog> listCALogs = new List<CourseAttendanceLog>();

            using (var db = new Context())
            {
                db.Database.CommandTimeout = 600;

                listCALogs = db.Database.SqlQuery<CourseAttendanceLog>(string.Format("SP_GetCourseAttendanceCampusReport '{0}','{1}',{2},{3},{4},{5},{6},{7}", dtFromDate, dtToDate, iCampusID, iProgramID, iProgramTypeID, iProgramTypeNumber, iProgramShiftID, iProgramGroupID)).ToList();
            }

            return listCALogs;
        }
    }

    public class OrganizationCourseAttendanceResultSet
    {
        public static List<OrganizationCampusView> GetOrganizationCampusList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusView() { Id = item.Id, CampusCode = item.CampusCode + "-" + item.CampusTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return lView;
        }


        public static List<OrganizationProgramView> GetOrganizationProgramList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_program.ToList();
                        if (mView != null && mView.Count > 0)
                        {
                            foreach (var item in mView)
                            {
                                lView.Add(new OrganizationProgramView() { Id = item.Id, ProgramCode = item.ProgramCode + "-" + item.ProgramTitle + " (" + item.DisciplineName + ")" });
                            }
                        }
                    }
                    else
                    {
                        lView = (from c in db.organization_campus
                                 join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                 join p in db.organization_program on cp.CampusId equals p.Id
                                 where c.Id == iGVCampusID
                                 select new OrganizationProgramView() { Id = p.Id, ProgramCode = p.ProgramCode + "-" + p.ProgramTitle + " (" + p.DisciplineName + ")" }).ToList();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramTypeView> GetOrganizationProgramTypeList()
        {
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramTypeView() { Id = item.Id, ProgramTypeName = item.ProgramTypeName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return lView;
        }


        public static List<OrganizationProgramShiftView> GetOrganizationProgramShiftList()
        {
            List<DLL.Models.OrganizationProgramShift> mView = null;
            List<OrganizationProgramShiftView> lView = new List<OrganizationProgramShiftView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_shift.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramShiftView() { Id = item.Id, ProgramShiftName = item.ProgramShiftName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramShift>();
                }
            }

            return lView;
        }

        public static List<LGRegionView> GetOrganizationProgramGroupList()
        {
            List<DLL.Models.Region> mView = null;
            List<LGRegionView> lView = new List<LGRegionView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.region.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new LGRegionView() { Id = item.RegionId, Name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Region>();
                }
            }

            return lView;
        }



        public static ViewModels.Employee[] getAllStudentsMatching(string subString)
        {
            using (var db = new Context())
            {
                subString = subString.ToLower();

                // get all employees with no groups
                DLL.Models.Employee[] employees = db.employee.Where(m => m.function.name.ToLower() == "student" && (m.active && m.timetune_active) && (m.employee_code.ToLower().Contains(subString) || (m.first_name != null && m.first_name.ToLower().Contains(subString)) || (m.last_name != null && m.last_name.ToLower().Contains(subString)))).ToArray();

                ViewModels.Employee[] toReturn = new ViewModels.Employee[employees.Length];

                for (int i = 0; i < employees.Length; i++)
                {
                    toReturn[i] = TimeTune.EmployeeCrud.convert(employees[i]);
                }

                return toReturn;
            }
        }



        public static List<OrganizationCourseAttendanceStudentView> getCourseAttendanceStudentData()
        {
            List<OrganizationCourseAttendance> list_course_attendance = null;
            List<OrganizationCourseAttendanceStudentView> toReturn = new List<OrganizationCourseAttendanceStudentView>();

            using (Context db = new Context())
            {
                try
                {
                    list_course_attendance = db.organization_course_attendance.ToList();
                    if (list_course_attendance != null && list_course_attendance.Count > 0)
                    {
                        for (int i = 0; i < list_course_attendance.Count(); i++)
                        {
                            int id_user = list_course_attendance[i].employee_student_id;
                            var data_employee = db.employee.Where(p => p.active && p.EmployeeId == id_user).FirstOrDefault();
                            if (data_employee != null)
                            {
                                string user_name = data_employee.first_name;
                                string user_lstname = data_employee.last_name;
                                string empcode = data_employee.employee_code;

                                toReturn.Add(new ViewModels.OrganizationCourseAttendanceStudentView()
                                {
                                    Id = list_course_attendance[i].id,
                                    StudentId = Convert.ToInt32(empcode),
                                    StudentCode = empcode,
                                    StudentName = user_name + " " + user_lstname,
                                    strSchDate = list_course_attendance[i].schedule_date_time.ToString("dd-MMM-yyyy"),
                                    CourseId = list_course_attendance[i].course_id,
                                    strCourseCode = "",
                                    actions =
                                       "<span data-row='" + list_course_attendance[i].id + "'>" +
                                       //"<a href=\"javascript:void(editLeaveSession(" + list_leave_session[i].id + "," + list_leave_session[i].EmployeeId + ",'" + user_name + " " + user_lstname + "','" + list_leave_session[i].SessionStartDate.ToString("dd-MM-yyyy") + "','" + list_leave_session[i].SessionEndDate.ToString("dd-MM-yyyy") + "'));\">Edit</a>" +
                                       "<a href=\"javascript:void(editCourseAttendanceStudent(" + list_course_attendance[i].id + "," + list_course_attendance[i].employee_student_id + ",'" + list_course_attendance[i].schedule_date_time.ToString("dd-MM-yyyy") + "'," + list_course_attendance[i].course_id + "));\">Edit</a>" +
                                           "<span> / </span>" +
                                       "<a href=\"javascript:void(deleteCourseAttendanceStudent(" + list_course_attendance[i].id + "));\">Delete</a>" +
                                       "</span>"
                                });
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    list_course_attendance = new List<OrganizationCourseAttendance>();
                }
            }

            return toReturn;
        }


        public static List<OrganizationCourseAttendanceStudentView> getCourseAttendanceStudentByDateRange(int iStudentID, DateTime dtStartDate, DateTime dtEndDate)
        {
            List<OrganizationCourseAttendance> list_course_attendance = null;
            List<OrganizationCourseAttendanceStudentView> toReturn = new List<OrganizationCourseAttendanceStudentView>();

            using (Context db = new Context())
            {
                try
                {
                    list_course_attendance = db.organization_course_attendance.Where(c => c.employee_student_id == iStudentID && (c.schedule_date_time >= dtStartDate && c.schedule_date_time <= dtEndDate)).ToList();
                    if (list_course_attendance != null && list_course_attendance.Count > 0)
                    {
                        for (int i = 0; i < list_course_attendance.Count(); i++)
                        {
                            int id_user = list_course_attendance[i].employee_student_id;
                            var data_employee = db.employee.Where(p => p.active && p.EmployeeId == id_user).FirstOrDefault();
                            if (data_employee != null)
                            {
                                string user_name = data_employee.first_name;
                                string user_lstname = data_employee.last_name;
                                string empcode = data_employee.employee_code;

                                string crs_code = "";
                                int id_crs = list_course_attendance[i].course_id;
                                var dbCourse = db.organization_program_course.Where(c => c.Id == id_crs).FirstOrDefault();
                                if (dbCourse != null)
                                {
                                    crs_code = dbCourse.CourseCode;
                                }

                                toReturn.Add(new ViewModels.OrganizationCourseAttendanceStudentView()
                                {
                                    Id = list_course_attendance[i].id,
                                    StudentId = id_user,
                                    StudentCode = empcode,
                                    StudentName = user_name + " " + user_lstname,
                                    strSchDate = list_course_attendance[i].schedule_date_time.ToString("dd-MM-yyyy"),
                                    CourseId = list_course_attendance[i].course_id,
                                    strCourseCode = crs_code,
                                    strRemarks = (list_course_attendance[i].final_remarks + (list_course_attendance[i].process_count > 1 ? "*" : "")) ?? "",
                                    strProcessBy = list_course_attendance[i].process_code ?? "-",
                                    iProcessCount = list_course_attendance[i].process_count,
                                    actions =
                                       "<span data-row='" + list_course_attendance[i].id + "'>" +
                                       //"<a href=\"javascript:void(editLeaveSession(" + list_leave_session[i].id + "," + list_leave_session[i].EmployeeId + ",'" + user_name + " " + user_lstname + "','" + list_leave_session[i].SessionStartDate.ToString("dd-MM-yyyy") + "','" + list_leave_session[i].SessionEndDate.ToString("dd-MM-yyyy") + "'));\">Edit</a>" +
                                       "<a href=\"javascript:void(editCourseAttendanceStudent(" + list_course_attendance[i].id + "," + list_course_attendance[i].employee_student_id + "," + list_course_attendance[i].course_id + ",'" + list_course_attendance[i].schedule_date_time.ToString("dd-MM-yyyy") + "','" + empcode + "','" + crs_code + "','" + list_course_attendance[i].final_remarks + "'));\">Edit</a>" +
                                       //"<span> / </span>" +
                                       //"<a href=\"javascript:void(deleteCourseAttendanceStudent(" + list_course_attendance[i].id + "));\">Delete</a>" +
                                       "</span>"
                                });
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    list_course_attendance = new List<OrganizationCourseAttendance>();
                }
            }

            return toReturn;
        }

        public static int CreateOrganizationCourseAttendanceStudent(OrganizationCourseAttendanceStudentForm toCreate)
        {
            int response = 0;
            OrganizationCourseAttendance crsModel = new OrganizationCourseAttendance();

            try
            {
                DateTime dtSchDate = DateTime.ParseExact(toCreate.start_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);


                using (Context db = new Context())
                {
                    var already_course_attendance = db.organization_course_attendance.Where(c => c.employee_student_id == toCreate.student_id && c.course_id == toCreate.course_id && c.schedule_date_time == dtSchDate).FirstOrDefault();
                    if (already_course_attendance == null)
                    {
                        crsModel.employee_student_id = toCreate.student_id;
                        crsModel.schedule_date_time = dtSchDate;
                        //crsModel.course_id = toCreate.StudentCode;

                        db.organization_course_attendance.Add(crsModel);
                        db.SaveChanges();

                        response = crsModel.id;
                    }
                    else
                    {
                        response = 0;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }

        public static int UpdateOrganizationCourseAttendanceStudent(OrganizationCourseAttendanceStudentForm toUpdate, string user_code)
        {
            int response = 0;
            OrganizationCourseAttendance crsModel = null;

            try
            {
                DateTime dtSchDate = DateTime.ParseExact(toUpdate.schedule_date.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                using (Context db = new Context())
                {
                    var already_exists = db.organization_course_attendance.Where(c => c.id != toUpdate.id && c.employee_student_id == toUpdate.student_id && c.course_id == toUpdate.course_id && c.schedule_date_time == dtSchDate).FirstOrDefault();
                    if (already_exists != null)
                    {
                        //another same campus-code already there in table
                        response = 0;
                    }
                    else
                    {
                        crsModel = db.organization_course_attendance.Find(toUpdate.id);

                        //crsModel.OrganizationId = toUpdate.OrganizationId;
                        //crsModel.employee_student_id = toUpdate.student_id;
                        //crsModel.schedule_date_time = dtSchDate;
                        //crsModel.CreateDateBld = toUpdate.CreateDateCmp;

                        crsModel.final_remarks = toUpdate.ddl_remarks;
                        crsModel.process_count = crsModel.process_count + 1;
                        crsModel.process_code = user_code;

                        db.SaveChanges();

                        response = toUpdate.id;
                    }
                }
            }
            catch (Exception)
            {
                response = -1;
            }

            return response;
        }


        public static int RemoveOrganizationCourseAttendanceStudent(OrganizationCourseAttendanceStudentForm toRemove)
        {

            using (Context db = new Context())
            {
                OrganizationCourseAttendance toRemoveModel = db.organization_course_attendance.Find(toRemove.id);

                //db.leave_application.Remove(toRemoveModel);

                //toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove.id;
            }

        }




        public static List<EmployeeView> GetOrganizationEmployeeStudentList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "student").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.campus_id == iGVCampusID && e.function.name.ToLower() == "student").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new EmployeeView() { Id = item.EmployeeId, EmployeeCode = item.employee_code + "-" + item.first_name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return lView;
        }

        public static string GetOrganizationEmployeeStudentString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string emp_list = "";
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "student").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.active && e.campus_id == iGVCampusID && e.function.name.ToLower() == "student").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            emp_list += c.EmployeeId + ":" + c.employee_code + "-" + c.first_name + ",";
                        }

                        emp_list = emp_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return emp_list.Replace("\r\n", "");
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCourseAttendanceView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCourseAttendanceView> FilterResult(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCourseAttendanceView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CampusCode != null && p.CampusCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCode != null && p.ProgramCode.ToLower().Contains(search.ToLower()) ||
                         p.StudentCode != null && p.StudentCode.ToString().ToLower().Contains(search.ToLower())

                    )
                )
           );

            return results;
        }


        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCourseAttendanceStudentView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCourseAttendanceStudentView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCourseAttendanceStudentView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCourseAttendanceStudentView> FilterResult(string search, List<ViewModels.OrganizationCourseAttendanceStudentView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCourseAttendanceStudentView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.StudentName != null && p.StudentName.ToLower().Contains(search.ToLower()) ||
                         p.strCourseCode != null && p.strCourseCode.ToLower().Contains(search.ToLower()) ||
                         p.strSchDate != null && p.strSchDate.ToString().ToLower().Contains(search.ToLower())

                    )
                )
           );

            return results;
        }


    }

    #endregion



    #region Organization-Students-Report

    public class StudentsReportData
    {
        public string orgName { get; set; }
        public string orgLogo { get; set; }
        public string orgWebUrl { get; set; }
        public string campusName { get; set; }
        public string progCode { get; set; }
        public string progTitle { get; set; }

        public string year_code { get; set; }
        public string campus_code { get; set; }
        public string program_code { get; set; }
        public string category_code { get; set; }
        public string enrolled_code { get; set; }
        public string shift_code { get; set; }
        public string group_code { get; set; }

        public StudentsLog[] logs { get; set; }
        public StudentCoursesLog[] courses { get; set; }

    }

    public class StudentsLog
    {
        //      year_code schedule_date_time  campus_id campus_code program_id program_code    student_id student_code    course_id course_code student_time_in status_in   
        //      student_time_out status_out  final_remarks course_time_start   course_time_end terminal_in_id  terminal_in_name terminal_out_id terminal_out_name active

        public int year_code { get; set; }
        public string student_code { get; set; }
        public string student_name { get; set; }
        public string father_name { get; set; }
        public string gender_name { get; set; }
        public string campus_code { get; set; }
        public string program_code { get; set; }

        public string program_type_name { get; set; }
        public int program_type_number { get; set; }
        public string program_category_name { get; set; }
        public string program_shift_name { get; set; }
        public string program_group_name { get; set; }

        public List<StudentCoursesLog> list_courses { get; set; }
    }

    public class StudentCoursesLog
    {
        public string course_code { get; set; }
        public string course_title { get; set; }
        public string is_failed { get; set; }
    }


    public class OrganizationStudentsReport
    {
        public StudentsReportData getOrganizationStudentsReport(int gcYearID, int campusID, int programID, int programTypeID, int programTypeNumber, int programShiftID, int programGroupID)
        {
            StudentsReportData toReturn = null;

            using (var db = new Context())
            {
                OrganizationView dbOrgCampus = new OrganizationView();
                toReturn = new StudentsReportData();

                dbOrgCampus = (from c in db.organization_campus
                               join o in db.organization on c.OrganizationId equals o.Id
                               where c.Id == campusID
                               select new OrganizationView()
                               {
                                   OrganizationTitle = o.OrganizationTitle,
                                   Logo = o.Logo,
                                   WebsiteURL = o.WebsiteURL,
                                   Description = c.CampusCode + "-" + c.CampusTitle
                               }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.orgName = dbOrgCampus.OrganizationTitle;
                    toReturn.orgLogo = dbOrgCampus.Logo ?? "/Content/Logos/logo-default.png";
                    toReturn.orgWebUrl = dbOrgCampus.WebsiteURL ?? "";
                    toReturn.campusName = dbOrgCampus.Description;
                    //toReturn.progName = dbOrgCampus.program_name;
                }

                //DateTime dtFromDate = DateTime.ParseExact(from_date + " 00:00:00.000", "dd-MM-yyyy HH:mm:00.000", CultureInfo.InvariantCulture);
                //DateTime dtToDate = DateTime.ParseExact(to_date + " 23:59:00.000", "dd-MM-yyyy HH:mm:00.000", CultureInfo.InvariantCulture);

                //toReturn.dateRange = dtFromDate.ToString("ddd, dd-MMM-yyyy") + " TO " + dtToDate.ToString("ddd, dd-MMM-yyyy");

                //OrganizationCourseAttendance[] attendanceLogs = db.organization_course_attendance.Where(m =>
                //     m.employee_student_id.Equals(emp.EmployeeId) &&
                //     (m.schedule_date_time >= dtFromDate && m.schedule_date_time <= dtToDate)).ToArray();


                List<StudentsLog> caRawLogs = new List<StudentsLog>();
                caRawLogs = OrganizationStudentsReport.getOrganizationStudentsList(gcYearID, campusID, programID, programTypeID, programTypeNumber, programShiftID, programGroupID);
                if (caRawLogs != null && caRawLogs.Count > 0)
                {
                    List<StudentsLog> tempLogs = new List<StudentsLog>();
                    List<StudentCoursesLog> countLogs = new List<StudentCoursesLog>();

                    foreach (StudentsLog log in caRawLogs)
                    {
                        toReturn.year_code = log.year_code.ToString();
                        toReturn.program_code = log.program_code;
                        toReturn.category_code = log.program_category_name;
                        toReturn.shift_code = log.program_shift_name.ToString();
                        toReturn.group_code = log.program_group_name.ToString();

                        tempLogs.Add(new StudentsLog()
                        {
                            student_code = log.student_code,
                            student_name = log.student_name,
                            father_name = log.father_name,
                            gender_name = log.gender_name
                        });
                    }

                    toReturn.logs = tempLogs.AsQueryable().ToArray();
                    ////toReturn.courses = countLogs.AsQueryable().ToArray();
                }
            }

            return toReturn;

        }

        public static List<StudentsLog> getOrganizationStudentsList(int iGCYearID, int iCampusID, int iProgramID, int iProgramTypeID, int iProgramTypeNumber, int iProgramShiftID, int iProgramGroupID)
        {
            List<StudentsLog> listSLogs = new List<StudentsLog>();

            using (var db = new Context())
            {
                db.Database.CommandTimeout = 600;

                listSLogs = db.Database.SqlQuery<StudentsLog>(string.Format("SP_GetOrganizationStudentsReport {0},{1},{2},{3},{4},{5},{6}", iGCYearID, iCampusID, iProgramID, iProgramTypeID, iProgramTypeNumber, iProgramShiftID, iProgramGroupID)).ToList();
            }

            return listSLogs;
        }
    }

    public class OrganizationStudentsResultSet
    {
        public static List<GeneralCalendarView> GetOrganizationGeneralCalendarList()
        {
            List<DLL.Models.GeneralCalendar> mView = null;
            List<GeneralCalendarView> lView = new List<GeneralCalendarView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.general_calender.OrderByDescending(o => o.GeneralCalendarId).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new GeneralCalendarView() { Id = item.GeneralCalendarId, YearName = item.year.ToString() });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.GeneralCalendar>();
                }
            }

            return lView;
        }


        public static List<OrganizationCampusView> GetOrganizationCampusList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.ToList();
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusView() { Id = item.Id, CampusCode = item.CampusCode + "-" + item.CampusTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return lView;
        }


        public static List<OrganizationProgramView> GetOrganizationProgramList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationProgram> mView = null;
            List<OrganizationProgramView> lView = new List<OrganizationProgramView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_program.ToList();
                        if (mView != null && mView.Count > 0)
                        {
                            foreach (var item in mView)
                            {
                                lView.Add(new OrganizationProgramView() { Id = item.Id, ProgramCode = item.ProgramCode + "-" + item.ProgramTitle + " (" + item.DisciplineName + ")" });
                            }
                        }
                    }
                    else
                    {
                        lView = (from c in db.organization_campus
                                 join cp in db.organization_campus_program on c.Id equals cp.CampusId
                                 join p in db.organization_program on cp.CampusId equals p.Id
                                 where c.Id == iGVCampusID
                                 select new OrganizationProgramView() { Id = p.Id, ProgramCode = p.ProgramCode + "-" + p.ProgramTitle + " (" + p.DisciplineName + ")" }).ToList();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgram>();
                }
            }

            return lView;
        }

        public static List<OrganizationProgramTypeView> GetOrganizationProgramTypeList()
        {
            List<DLL.Models.OrganizationProgramType> mView = null;
            List<OrganizationProgramTypeView> lView = new List<OrganizationProgramTypeView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_type.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramTypeView() { Id = item.Id, ProgramTypeName = item.ProgramTypeName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramType>();
                }
            }

            return lView;
        }


        public static List<OrganizationProgramShiftView> GetOrganizationProgramShiftList()
        {
            List<DLL.Models.OrganizationProgramShift> mView = null;
            List<OrganizationProgramShiftView> lView = new List<OrganizationProgramShiftView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.organization_program_shift.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationProgramShiftView() { Id = item.Id, ProgramShiftName = item.ProgramShiftName });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationProgramShift>();
                }
            }

            return lView;
        }

        public static List<LGRegionView> GetOrganizationProgramGroupList()
        {
            List<DLL.Models.Region> mView = null;
            List<LGRegionView> lView = new List<LGRegionView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.region.ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new LGRegionView() { Id = item.RegionId, Name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Region>();
                }
            }

            return lView;
        }


        public static List<EmployeeView> GetOrganizationEmployeeStudentList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "student").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.campus_id == iGVCampusID && e.function.name.ToLower() == "student").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new EmployeeView() { Id = item.EmployeeId, EmployeeCode = item.employee_code + "-" + item.first_name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return lView;
        }

        public static string GetOrganizationEmployeeStudentString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string emp_list = "";
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "student").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.active && e.campus_id == iGVCampusID && e.function.name.ToLower() == "student").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            emp_list += c.EmployeeId + ":" + c.employee_code + "-" + c.first_name + ",";
                        }

                        emp_list = emp_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return emp_list.Replace("\r\n", "");
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCourseAttendanceView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCourseAttendanceView> FilterResult(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCourseAttendanceView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CampusCode != null && p.CampusCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCode != null && p.ProgramCode.ToLower().Contains(search.ToLower()) ||
                         p.StudentCode != null && p.StudentCode.ToString().ToLower().Contains(search.ToLower())

                    )
                )
           );

            return results;
        }

    }

    #endregion



    #region Organization-Employees-Report

    public class EmployeesReportData
    {
        public string orgName { get; set; }
        public string orgLogo { get; set; }
        public string orgWebUrl { get; set; }
        public string campusName { get; set; }

        public string campus_code { get; set; }
        public string dept_name { get; set; }
        public string desg_name { get; set; }
        public string loct_name { get; set; }

        public EmployeesLog[] logs { get; set; }
    }

    public class EmployeesLog
    {
        //      year_code schedule_date_time  campus_id campus_code program_id program_code    student_id student_code    course_id course_code student_time_in status_in   
        //      student_time_out status_out  final_remarks course_time_start   course_time_end terminal_in_id  terminal_in_name terminal_out_id terminal_out_name active

        public string employee_code { get; set; }
        public string employee_name { get; set; }
        public string gender_name { get; set; }
        public string campus_code { get; set; }
        public string dept_name { get; set; }
        public string desg_name { get; set; }
        public string loct_name { get; set; }
        public string photograph { get; set; }
        public string father_name { get; set; }
        public string date_of_joining { get; set; }
    }

    public class OrganizationEmployeesReport
    {
        public EmployeesReportData getOrganizationEmployeesReport(int campusID, int deptID, int desgID, int loctID)
        {
            EmployeesReportData toReturn = null;

            using (var db = new Context())
            {
                OrganizationView dbOrgCampus = new OrganizationView();
                toReturn = new EmployeesReportData();

                //var dbOrgCampus = (from c in db.organization_campus
                //               join o in db.organization on c.OrganizationId equals o.Id
                //               where c.Id == campusID
                //               select new OrganizationView()
                //               {
                //                   OrganizationTitle = o.OrganizationTitle,
                //                   Logo = o.Logo,
                //                   WebsiteURL = o.WebsiteURL,
                //                   Description = c.CampusCode + "-" + c.CampusTitle
                //               }).FirstOrDefault();

                dbOrgCampus = (from o in db.organization
                               select new OrganizationView()
                               {
                                   OrganizationTitle = o.OrganizationTitle,
                                   Logo = o.Logo,
                                   WebsiteURL = o.WebsiteURL,
                                   Description = ""
                               }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.orgName = dbOrgCampus.OrganizationTitle;
                    toReturn.orgLogo = dbOrgCampus.Logo ?? "/Content/Logos/logo-default.png";
                    toReturn.orgWebUrl = dbOrgCampus.WebsiteURL ?? "";
                    toReturn.campusName = dbOrgCampus.Description;
                    //toReturn.progName = dbOrgCampus.program_name;
                }

                List<EmployeesLog> caRawLogs = new List<EmployeesLog>();
                caRawLogs = OrganizationEmployeesReport.getOrganizationEmployeesList(campusID, deptID, desgID, loctID);
                if (caRawLogs != null && caRawLogs.Count > 0)
                {
                    List<EmployeesLog> tempLogs = new List<EmployeesLog>();
                    //List<StudentCoursesLog> countLogs = new List<StudentCoursesLog>();

                    foreach (EmployeesLog log in caRawLogs)
                    {
                        toReturn.campus_code = log.campus_code;
                        toReturn.dept_name = log.dept_name;
                        toReturn.desg_name = log.desg_name;
                        toReturn.loct_name = log.loct_name;

                        tempLogs.Add(new EmployeesLog()
                        {
                            employee_code = log.employee_code,
                            employee_name = log.employee_name,
                            dept_name = log.dept_name,
                            desg_name = log.desg_name,
                            loct_name = log.loct_name,
                            father_name = log.father_name,
                            gender_name = log.gender_name
                        });
                    }

                    toReturn.logs = tempLogs.AsQueryable().ToArray();
                    ////toReturn.courses = countLogs.AsQueryable().ToArray();
                }
            }

            return toReturn;

        }

        public static List<EmployeesLog> getOrganizationEmployeesList(int iCampusID, int iDeptID, int iDesgID, int iLoctID)
        {
            List<EmployeesLog> listELogs = new List<EmployeesLog>();

            using (var db = new Context())
            {
                db.Database.CommandTimeout = 600;

                listELogs = db.Database.SqlQuery<EmployeesLog>(string.Format("SP_GetOrganizationEmployeesReport {0},{1},{2},{3}", iCampusID, iDeptID, iDesgID, iLoctID)).ToList();
            }

            return listELogs;
        }
    }

    public class OrganizationEmployeesByTypeReport
    {
        public EmployeesReportData getOrganizationEmployeesByTypeReport(int type_of_employeement)
        {
            EmployeesReportData toReturn = null;

            using (var db = new Context())
            {
                OrganizationView dbOrgCampus = new OrganizationView();
                toReturn = new EmployeesReportData();

                //var dbOrgCampus = (from c in db.organization_campus
                //               join o in db.organization on c.OrganizationId equals o.Id
                //               where c.Id == campusID
                //               select new OrganizationView()
                //               {
                //                   OrganizationTitle = o.OrganizationTitle,
                //                   Logo = o.Logo,
                //                   WebsiteURL = o.WebsiteURL,
                //                   Description = c.CampusCode + "-" + c.CampusTitle
                //               }).FirstOrDefault();

                dbOrgCampus = (from o in db.organization
                               select new OrganizationView()
                               {
                                   OrganizationTitle = o.OrganizationTitle,
                                   Logo = o.Logo,
                                   WebsiteURL = o.WebsiteURL,
                                   Description = ""
                               }).FirstOrDefault();

                if (dbOrgCampus != null)
                {
                    toReturn.orgName = dbOrgCampus.OrganizationTitle;
                    toReturn.orgLogo = dbOrgCampus.Logo ?? "/Content/Logos/logo-default.png";
                    toReturn.orgWebUrl = dbOrgCampus.WebsiteURL ?? "";
                    toReturn.campusName = dbOrgCampus.Description;
                    //toReturn.progName = dbOrgCampus.program_name;
                }

                List<EmployeesLog> caRawLogs = new List<EmployeesLog>();
                caRawLogs = OrganizationEmployeesByTypeReport.getOrganizationEmployeesByTypeList(type_of_employeement);
                if (caRawLogs != null && caRawLogs.Count > 0)
                {
                    List<EmployeesLog> tempLogs = new List<EmployeesLog>();
                    //List<StudentCoursesLog> countLogs = new List<StudentCoursesLog>();

                    foreach (EmployeesLog log in caRawLogs)
                    {
                        toReturn.campus_code = log.campus_code;
                        toReturn.dept_name = log.dept_name;
                        toReturn.desg_name = log.desg_name;
                        toReturn.loct_name = log.loct_name;

                        tempLogs.Add(new EmployeesLog()
                        {
                            employee_code = log.employee_code,
                            employee_name = log.employee_name,
                            dept_name = log.dept_name,
                            desg_name = log.desg_name,
                            loct_name = log.loct_name,
                            father_name = log.father_name,
                            gender_name = log.gender_name
                        });
                    }

                    toReturn.logs = tempLogs.AsQueryable().ToArray();
                    ////toReturn.courses = countLogs.AsQueryable().ToArray();
                }
            }

            return toReturn;

        }

        public static List<EmployeesLog> getOrganizationEmployeesByTypeList(int iTypeEmployeement)
        {
            List<EmployeesLog> listELogs = new List<EmployeesLog>();

            using (var db = new Context())
            {
                db.Database.CommandTimeout = 600;

                listELogs = db.Database.SqlQuery<EmployeesLog>(string.Format("SP_GetOrganizationEmployeesByTypeReport {0}", iTypeEmployeement)).ToList();
            }

            return listELogs;
        }
    }

    public class OrganizationEmployeesResultSet
    {
        public static List<GeneralCalendarView> GetOrganizationGeneralCalendarList()
        {
            List<DLL.Models.GeneralCalendar> mView = null;
            List<GeneralCalendarView> lView = new List<GeneralCalendarView>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.general_calender.OrderByDescending(o => o.GeneralCalendarId).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new GeneralCalendarView() { Id = item.GeneralCalendarId, YearName = item.year.ToString() });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.GeneralCalendar>();
                }
            }

            return lView;
        }


        public static List<OrganizationCampusView> GetOrganizationCampusList(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            List<DLL.Models.OrganizationCampus> mView = null;
            List<OrganizationCampusView> lView = new List<OrganizationCampusView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.organization_campus.Where(c => c.FaxNumber == "1").OrderBy(o => o.CampusCode).ToList();//Fax Number used for Active/Inactive Status
                    }
                    else
                    {
                        mView = db.organization_campus.Where(c => c.Id == iGVCampusID).ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new OrganizationCampusView() { Id = item.Id, CampusCode = item.CampusCode + "-" + item.CampusTitle });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.OrganizationCampus>();
                }
            }

            return lView;
        }

        public static List<Department> GetOrganizationDepartmentList()
        {
            List<DLL.Models.Department> mView = null;
            List<Department> lView = new List<Department>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.department.Where(o => o.active).OrderBy(o => o.name).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new Department() { id = item.DepartmentId, name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Department>();
                }
            }

            return lView;
        }

        public static List<Designation> GetOrganizationDesignationList()
        {
            List<DLL.Models.Designation> mView = null;
            List<Designation> lView = new List<Designation>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.designation.Where(o => o.active).OrderBy(o => o.name).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new Designation() { id = item.DesignationId, name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Designation>();
                }
            }

            return lView;
        }

        public static List<Location> GetOrganizationLocationList()
        {
            List<DLL.Models.Location> mView = null;
            List<Location> lView = new List<Location>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.location.Where(o => o.active).OrderBy(o => o.name).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new Location() { id = item.LocationId, name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Location>();
                }
            }

            return lView;
        }

        public static string GetOrganizationEmployeeString(bool bGVIsSuperHRRole, int iGVCampusID)
        {
            string emp_list = "";
            List<DLL.Models.Employee> mView = null;
            List<EmployeeView> lView = new List<EmployeeView>();

            using (Context db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole) //alowed to manage all campuses
                    {
                        mView = db.employee.Where(e => e.active && e.function.name.ToLower() == "student").ToList();
                    }
                    else
                    {
                        mView = db.employee.Where(e => e.active && e.campus_id == iGVCampusID && e.function.name.ToLower() == "student").ToList();
                    }

                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var c in mView)
                        {
                            emp_list += c.EmployeeId + ":" + c.employee_code + "-" + c.first_name + ",";
                        }

                        emp_list = emp_list.TrimEnd(',');
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.Employee>();
                }
            }

            return emp_list.Replace("\r\n", "");
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCourseAttendanceView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCourseAttendanceView> FilterResult(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCourseAttendanceView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CampusCode != null && p.CampusCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCode != null && p.ProgramCode.ToLower().Contains(search.ToLower()) ||
                         p.StudentCode != null && p.StudentCode.ToString().ToLower().Contains(search.ToLower())

                    )
                )
           );

            return results;
        }

    }

    public class OrganizationEmployeesByTypeResultSet
    {
        public static List<TypeOfEmployment> GetOrganizationEmployeesByTypeList()
        {
            List<DLL.Models.TypeOfEmployment> mView = null;
            List<TypeOfEmployment> lView = new List<TypeOfEmployment>();

            using (Context db = new Context())
            {
                try
                {
                    mView = db.type_of_employment.Where(o => o.active).OrderBy(o => o.name).ToList();
                    if (mView != null && mView.Count > 0)
                    {
                        foreach (var item in mView)
                        {
                            lView.Add(new TypeOfEmployment() { id = item.TypeOfEmploymentId, name = item.name });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    mView = new List<DLL.Models.TypeOfEmployment>();
                }
            }

            return lView;
        }

        //////////////////////////////////////////////////

        public static List<ViewModels.OrganizationCourseAttendanceView> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.OrganizationCourseAttendanceView> FilterResult(string search, List<ViewModels.OrganizationCourseAttendanceView> dtResult)
        {
            IQueryable<ViewModels.OrganizationCourseAttendanceView> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.CampusCode != null && p.CampusCode.ToLower().Contains(search.ToLower()) ||
                         p.ProgramCode != null && p.ProgramCode.ToLower().Contains(search.ToLower()) ||
                         p.StudentCode != null && p.StudentCode.ToString().ToLower().Contains(search.ToLower())

                    )
                )
           );

            return results;
        }

    }



    #endregion


    #region Course-Attendance-Management


    public class ManageCourseAttendanceStudentImportExport
    {
        public class ManageCourseAttendanceCSV
        {
            public string schDate { get; set; }
            public string stdCode { get; set; }
            public string crsCode { get; set; }
            public string finRemarks { get; set; }
        }
        public class CrsAttExportTable
        {
            public string schDate { get; set; }
            public string stdCode { get; set; }
            public string crsCode { get; set; }
            public string finRemarks { get; set; }
        }

        public static List<ManageCourseAttendanceCSV> getManageCourseAttendanceStudentsCSVDownload()
        {
            List<ManageCourseAttendanceCSV> toReturn;
            using (var db = new Context())
            {
                try
                {
                    toReturn =
                        db.organization_program_course_enrollment.Where(m => m.Id > 0).Select(
                        p => new ManageCourseAttendanceCSV()
                        {
                            schDate = db.organization_course_attendance.Where(c => c.id == p.Id).FirstOrDefault().schedule_date_time.ToString("dd/MM/yyyy"),
                            stdCode = db.employee.Where(c => c.EmployeeId == p.EmployeeStudentId).FirstOrDefault().employee_code,
                            crsCode = db.organization_program_course.Where(c => c.Id == p.ProgramCourseId).FirstOrDefault().CourseCode,
                            finRemarks = db.organization_course_attendance.Where(c => c.id == p.Id).FirstOrDefault().final_remarks
                        }).ToList();

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ManageCourseAttendanceCSV>();
                }
            }

            return toReturn;
        }


        public static string setCourseAttendanceEnrollment(List<string> csvContents, string user_code)
        {
            string strLastRemarks = "";

            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 8 values (columns).

                        if (values == null || values.Length < 4)
                            continue;

                        // Remove unwanted " characters.
                        string strScheduleDate = values[0].Replace("\"", "");
                        string strStudentCode = values[1].Replace("\"", "");
                        string strCourseCode = values[2].Replace("\"", "");
                        string strNewRemarks = values[3].Replace("\"", "");

                        //-----------------------------------------------
                        DateTime dtSchDate = DateTime.ParseExact(strScheduleDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        //-----------------------------------------------
                        int iStudentId = 0;
                        if (strStudentCode == "0")
                        {
                            iStudentId = 0;
                        }
                        else
                        {
                            var dbEmployeeStudent = db.employee.Where(r => r.employee_code == strStudentCode).FirstOrDefault();
                            iStudentId = dbEmployeeStudent != null ? dbEmployeeStudent.EmployeeId : 0;
                        }

                        //-----------------------------------------------
                        int iCourseId = 0; strCourseCode = strCourseCode.ToLower();
                        if (strCourseCode == "0")
                        {
                            iCourseId = 0;
                        }
                        else
                        {
                            var dbCourse = db.organization_program_course.Where(r => r.CourseCode != null && r.CourseCode.ToLower() == strCourseCode).FirstOrDefault();
                            iCourseId = dbCourse != null ? dbCourse.Id : 0;
                        }

                        // continue if its the first row (CSV Header line).
                        if (strScheduleDate == "schedule_date" || strStudentCode == "student_code" || strCourseCode == "course_code")
                        {
                            continue;
                        }

                        // Get the existing schedule by room and date 
                        DLL.Models.OrganizationCourseAttendance cAttendance = db.organization_course_attendance
                            .Where(m => m.schedule_date_time == dtSchDate && m.employee_student_id == iStudentId && m.course_id == iCourseId &&
                                        m.final_remarks != "LV").FirstOrDefault();

                        if (cAttendance != null)//existing enrollment
                        {
                            strLastRemarks = cAttendance.final_remarks;

                            cAttendance.final_remarks = strNewRemarks;
                            cAttendance.process_count = cAttendance.process_count + 1;
                            cAttendance.process_code = user_code;

                            db.SaveChanges();

                            //var json = JsonConvert.SerializeObject(cAttendance);
                            //AuditTrail.update(json, "ManageCourseAttendanceStudent", User.Identity.Name);

                            TimeTune.AuditTrail.update("{\"*id\" : \"" + cAttendance.id.ToString() + "\", \"*sch_date\" : \"" + strScheduleDate + "\", \"*std_code\" : \"" + strStudentCode.ToString() + "\", \"*crs_code\" : \"" + strCourseCode + "\", \"*last_remarks\" : \"" + strLastRemarks + "\", \"*new_remarks\" : \"" + strNewRemarks + "\"}", "OrganizationCourseAttendance", user_code);
                        }
                        else //new enrollment
                        {
                            //DLL.Models.OrganizationCourseAttendance crs = new DLL.Models.OrganizationCourseAttendance()
                            //{
                            //    schedule_date_time = dtSchDate,
                            //    employee_student_id = iStudentId,
                            //    course_id = iCourseId,
                            //    final_remarks = strFinalRemarks,
                            //    process_count = 1,
                            //    process_code = user_code
                            //};

                            //db.organization_course_attendance.Add(crs);
                            //db.SaveChanges();

                            //TimeTune.AuditTrail.insert("{\"*id\" : \"" + crs.id.ToString() + "\"}", "OrganizationProgramCourseEnrollment", user_code);
                        }
                    }

                    return "Successful";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        public static bool validateCrsAttGCYear(string strGCYear)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    var dbGCYear = db.general_calender.Where(c => c.year.ToString() == strGCYear).FirstOrDefault();
                    if (dbGCYear != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCrsAttCampusCode(string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCrsAttCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    if (bGVIsSuperHRRole)
                    {
                        isValid = true;
                    }
                    else
                    {
                        strCampusCode = strCampusCode.ToLower();

                        var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                        var dbEmployee = db.employee.Where(c => c.campus_id == iCampusID).FirstOrDefault();

                        if (dbCampus != null && dbEmployee != null)
                        {
                            if (dbCampus.Id == dbEmployee.campus_id)
                                isValid = true;
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCrsAttProgramCode(string strProgramCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strProgramCode = strProgramCode.ToLower();

                    var dbProgram = db.organization_program.Where(c => c.ProgramCode == strProgramCode).FirstOrDefault();
                    if (dbProgram != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }



        public static bool validateCrsAttProgramType(string strPType)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strPType = strPType.ToLower();

                    var dbCourse = db.organization_program_type.Where(c => c.ProgramTypeName.ToLower() == strPType).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCrsAttProgramCourseCode(string strCourseCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCourseCode = strCourseCode.ToLower();

                    var dbCourse = db.organization_program_course.Where(c => c.CourseCode == strCourseCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCrsAttStudentCode(string strStudentCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strStudentCode = strStudentCode.ToLower();

                    var dbCourse = db.employee.Where(c => c.function.name.ToLower() == "student" && c.employee_code == strStudentCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCrsAttStudentCodeCampus(string strCampusCode, string strStudentCode)
        {
            int iCampusID = 0;
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strStudentCode = strStudentCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        iCampusID = dbCampus.Id;
                    }

                    var dbCourse = db.employee.Where(c => c.campus_id == iCampusID && c.function.name.ToLower() == "student" && c.employee_code == strStudentCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

    }



    #endregion

}
