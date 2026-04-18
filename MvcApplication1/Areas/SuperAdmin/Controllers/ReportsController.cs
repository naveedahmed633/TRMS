using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;
using BLL.ViewModels;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data;
using System.Reflection;
using OfficeOpenXml;
using System.Configuration;
using BLL_UNIS.ViewModels;

namespace MvcApplication1.Areas.SuperAdmin.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SUPER_USER)]
    public class ReportsController : Controller
    {
        #region Jobs-Logs


        public ActionResult JobsLogs()
        {
            return View();
        }


        public class JobsLogsTable : DTParameters
        {
            //public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public string log_title { get; set; }
        }



        [HttpPost]
        public JsonResult ServicesLogsDataHandler(JobsLogsTable param)
        {
            try
            {
                var data = new List<JobsLog>();

                // get all employee view models
                int count = TimeTune.Reports.getJobsLogs(param.from_date, param.to_date, param.log_title, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<JobsLog> result = new DTResult<JobsLog>
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




        #endregion



        #region Devices Status

        public ActionResult DevicesStatusReport()
        {
            //List<BLL_UNIS.ViewModels.DevicesStatus> vList = new List<DevicesStatus>();

            //int count = BLL_UNIS.UNIS_Reports.getDevicesList(out vList);

            return View();
        }

        [HttpPost]
        public JsonResult DevicesStatusReportDataHandler(DeviceStatusReportTable param)
        {
            try
            {
                List<BLL_UNIS.ViewModels.DevicesStatus> data = new List<DevicesStatus>();

                data = BLL_UNIS.UNIS_Reports.getDevicesStatusList(param.Search.Value, param.SortOrder, param.Start, param.Length);

                DTResult<BLL_UNIS.ViewModels.DevicesStatus> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatus>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = data.Count,
                    recordsTotal = data.Count
                };
                return Json(result);
                //List<BLL_UNIS.ViewModels.DevicesStatus> data = new List<DevicesStatus>();

                //int count = BLL_UNIS.UNIS_Reports.getDevicesList(param.device_number, param.from_date, param.to_date, param.device_status_type, param.Search.Value, "L_TID", param.Start, param.Length, out data);
                ////var data = new List<MonthlyTimesheetAttendanceLog>();
                ////// get all employee view models
                ////int count2 = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<BLL_UNIS.ViewModels.DevicesStatus> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatus>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};
                //return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        /*
        [HttpPost]
        public ActionResult DevicesStatusReportExcelDownload(DeviceStatusReportTable param)
        {
            List<DevicesStatusExcelDownload> ddata = new List<DevicesStatusExcelDownload>();
            string handle = Guid.NewGuid().ToString();

            try
            {
                int count = BLL_UNIS.UNIS_Reports.getDevicesListExceldownload(param.device_number, param.from_date, param.to_date, param.device_status_type, param.Start, param.Length, out ddata);
                if (ddata.Count() > 0)
                {
                    var products = ToDataTable<DevicesStatusExcelDownload>(ddata);

                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("DevicesStatusReport");
                    var totalCols = products.Columns.Count;
                    var totalRows = products.Rows.Count;

                    for (var col = 1; col <= totalCols; col++)
                    {
                        workSheet.Cells[1, col].Value = products.Columns[col - 1].ColumnName.Replace("_", " ");
                    }

                    for (var row = 1; row <= totalRows; row++)
                    {
                        for (var col = 0; col < totalCols; col++)
                        {
                            workSheet.Cells[row + 1, col + 1].Value = products.Rows[row - 1][col];
                        }
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        excel.SaveAs(memoryStream);

                        memoryStream.Position = 0;
                        TempData[handle] = memoryStream.ToArray();
                    }
                }
                else
                {
                    return new JsonResult()
                    {
                        Data = new { FileGuid = "", FileName = "" }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = "Devices-Status-Report.xlsx" }
            };
        }
        */


        public class DeviceStatusReportTable : DTParameters
        {
            public string device_number { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public string device_status_type { get; set; }
        }

        [HttpPost]
        public JsonResult ChangeDeviceStatusNumberDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            BLL_UNIS.ViewModels.Terminal[] terminals = BLL_UNIS.UNIS_Reports.getAllDeviceNumbersMatching(q);

            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[terminals.Length];
            for (int i = 0; i < terminals.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = terminals[i].L_ID.ToString();
                toSend[i].text = terminals[i].C_Name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        public ActionResult DevicesUnregistered()
        {

            return View();
        }

        [HttpPost]
        public JsonResult DevicesUnregistered(DeviceStatusReportTable param)
        {
            try
            {
                List<BLL_UNIS.ViewModels.DevicesUnregister> data = new List<DevicesUnregister>();

                data = BLL_UNIS.UNIS_Reports.getUnRegisterDevicesStatusList();

                DTResult<BLL_UNIS.ViewModels.DevicesUnregister> result = new DTResult<BLL_UNIS.ViewModels.DevicesUnregister>
                {
                    draw = param.Draw,
                    data = data

                };
                return Json(result);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        #endregion


        #region Devices Status By Region

        public class DeviceStatusByRegionReportTable : DTParameters
        {
            public int device_region_id { get; set; }
        }

        public ActionResult DevicesStatusByRegionReport()
        {
            //List<BLL_UNIS.ViewModels.DevicesStatus> vList = new List<DevicesStatus>();

            //int count = BLL_UNIS.UNIS_Reports.getDevicesList(out vList);

            return View();
        }

        [HttpPost]
        public JsonResult DevicesStatusByRegionReportDataHandler(DeviceStatusByRegionReportTable param)
        {
            List<BLL_UNIS.ViewModels.DeviceStatusReport> data = new List<BLL_UNIS.ViewModels.DeviceStatusReport>();

            try
            {
                if (param.device_region_id == 0)
                {
                    DTResult<BLL_UNIS.ViewModels.DeviceStatusReport> resultNULL = new DTResult<BLL_UNIS.ViewModels.DeviceStatusReport>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = 0,
                        recordsTotal = 0
                    };

                    return Json(resultNULL);
                }



                data = BLL_UNIS.UNIS_Reports.getDevicesStatusByRegionIDList(param.device_region_id, param.Search.Value, param.SortOrder, param.Start, param.Length);
                if (data == null)
                {
                    return Json("No Data to Show");
                }


                List<BLL_UNIS.ViewModels.DeviceStatusReport> data2 = MvcApplication1.Areas.SuperAdmin.DataTable.HR_DevicesStatusRegionResultSet.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, data);
                //data = data.OrderByDescending(c => c.id).ToList();

                int count = MvcApplication1.Areas.SuperAdmin.DataTable.HR_DevicesStatusRegionResultSet.Count(param.Search.Value, data);



                DTResult<BLL_UNIS.ViewModels.DeviceStatusReport> result = new DTResult<BLL_UNIS.ViewModels.DeviceStatusReport>
                {
                    draw = param.Draw,
                    data = data2,
                    recordsFiltered = count,
                    recordsTotal = count
                };


                return Json(result);
                //List<BLL_UNIS.ViewModels.DevicesStatus> data = new List<DevicesStatus>();

                //int count = BLL_UNIS.UNIS_Reports.getDevicesList(param.device_number, param.from_date, param.to_date, param.device_status_type, param.Search.Value, "L_TID", param.Start, param.Length, out data);
                ////var data = new List<MonthlyTimesheetAttendanceLog>();
                ////// get all employee view models
                ////int count2 = TimeTune.Reports.getMonthlyTimesheetReportByEmployeeId(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                //DTResult<BLL_UNIS.ViewModels.DevicesStatus> result = new DTResult<BLL_UNIS.ViewModels.DevicesStatus>
                //{
                //    draw = param.Draw,
                //    data = data,
                //    recordsFiltered = count,
                //    recordsTotal = count
                //};
                //return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        #endregion

        public ActionResult ConsolidateAttendanceArchive()
        {
            return View();
        }


        public class ConsolidatedReportTable : DTParameters
        {
            public string employee_id { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }


        [HttpPost]
        public JsonResult ConsolidatedAtDataHandler(ConsolidatedReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceArchiveLog>();

                // get all employee view models

                int count = TimeTune.Reports.getAllConsolidateAttendanceMatchingArchive(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ConsolidatedAttendanceArchiveLog> result = new DTResult<ConsolidatedAttendanceArchiveLog>
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

    }
}
