using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TimeTune
{
    public class ManualAttendance
    {
        // this method will return null, if manual attendance has been marked successfully,
        // else it will return a string containing the error message.
        public static string addManualAttendance(ViewModels.ManualAttendance fromForm, string empCode)
        {
            using (var db = new DLL.Models.Context())
            {
                // return error if any of the dates is empty.
                if (!fromForm.time_in_from.HasValue || !fromForm.time_in_to.HasValue)
                {
                    return "Dates cannot be empty.";
                }


                int emp_id;
                if (!int.TryParse(fromForm.employee_code, out emp_id))
                {
                    return "Invalid employee Id.";
                }


                // RIGHT: this will include the last day
                var endDateExclusive = fromForm.time_in_to.Value.Date.AddDays(1);

                // get all the logs from the consolidated attendance
                // which fall between the dates specified by the manual attendance
                // view model

                DateTime start = fromForm.time_in_from.Value.Date;


                List<DLL.Models.ConsolidatedAttendance> allLogsForEmployee =
                    db.consolidated_attendance.Where(m =>
                        m.employee.EmployeeId.Equals(emp_id) &&

                        m.date >= start && m.date < endDateExclusive.Date

                        //fromForm.time_in_from.Value <= m.date.Value  &&
                        //m.date.Value <= fromForm.time_in_to.Value
                        ).ToList();


                string str = db.consolidated_attendance.Where(m =>
                        m.employee.EmployeeId.Equals(emp_id) &&

                        m.date >= fromForm.time_in_from && m.date < endDateExclusive

                        //fromForm.time_in_from.Value <= m.date.Value  &&
                        //m.date.Value <= fromForm.time_in_to.Value
                        ).ToString();
                Console.WriteLine(str);

                if (allLogsForEmployee == null || allLogsForEmployee.Count == 0)
                {
                    return "There are no entries for the employee on the selected dates.";
                }

                // if the start date or the end date in the view model is greater
                // than the maximum date in the consolidated result set, return an error, 
                // with the largest date
                DateTime max = allLogsForEmployee.Max(m => m.date).Value;
                DateTime min = allLogsForEmployee.Min(m => m.date).Value;

                if (fromForm.time_in_from.Value > max)
                {
                    return "No entries exists for date " + fromForm.time_in_from.Value.Date.ToString("dd-MMMM-yyyy");
                }
                if (fromForm.time_in_to.Value > max)
                {
                    return "No entries exists for date " + fromForm.time_in_to.Value.Date.ToString("dd-MMMM-yyyy");
                }


                // if any of the logs have three manual attendance already then
                // return an error stating the date which has three manual attendances.
                for (int i = 0; i < allLogsForEmployee.Count; i++)
                {
                    if (allLogsForEmployee[i].manualAttendances.Count >= 3)
                    {
                        return "Limit for manual changes for this employee on date " + allLogsForEmployee[i].date.Value.Date + " has been reached.";
                    }
                }


                int strideDate = 5;

                // checking if the stride date is not violated.
                //Read from Text File
                ////strideDate = new BLL.DataFiles().getManualAttendanceStride();

                //Read from DB
                if (int.TryParse(new BLL.DataFiles().getAccessCodeValue(), out strideDate))
                {
                    //IR coded logic
                }

                ConsolidatedAttendance[] invalidEntries;


                DateTime todaysDate = DateTime.Now;
                DateTime firstDayOfThisMonth = new DateTime(todaysDate.Year, todaysDate.Month, 1);


                if (DateTime.Now.Day <= strideDate)
                {
                    invalidEntries = allLogsForEmployee.Where(l =>
                    l.date.Value < firstDayOfThisMonth.AddMonths(-1) ||
                    l.date.Value > todaysDate).ToArray();
                }
                else
                {
                    invalidEntries = allLogsForEmployee.Where(l =>
                        l.date.Value < firstDayOfThisMonth ||
                        l.date.Value > todaysDate).ToArray();
                }

                //IR Commented as per the Requert
                //if (invalidEntries.Count() > 0)
                //    return "Unable to change attendance for " + invalidEntries[0].date.Value.ToString("dd-MM-yyyy") + " as CUT-OFF date passed.";

                // assign manual attendance, if you get here.
                for (int i = 0; i < allLogsForEmployee.Count; i++)
                {
                    string remark = (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PLO)) ?
                                        DLL.Commons.FinalRemarks.PLO :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.ABSENT)) ?
                                        DLL.Commons.FinalRemarks.ABSENT :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PRESENT)) ?
                                        DLL.Commons.FinalRemarks.PRESENT :

                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OTV)) ?
                                        DLL.Commons.FinalRemarks.OTV :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OM)) ?
                                        DLL.Commons.FinalRemarks.OM :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OT)) ?
                                        DLL.Commons.FinalRemarks.OT :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OD)) ?
                                        DLL.Commons.FinalRemarks.OD :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OV)) ?
                                        DLL.Commons.FinalRemarks.OV :
                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.LV)) ?
                                        DLL.Commons.FinalRemarks.LV :

                                        (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.OFF)) ?
                                        DLL.Commons.FinalRemarks.OFF :
                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.POE)) ?
                                        DLL.Commons.FinalRemarks.POE :
                                         (fromForm.remarks.Equals(DLL.Commons.FinalRemarks.PLE)) ?
                                        DLL.Commons.FinalRemarks.PLE : null;
                    if (remark == null)
                    {
                        return "Invalid remarks";
                    }

                    ///////////////////////////////////////////////////////////////////////////////

                    var mcons = new DLL.Models.ManualAttendance()
                    {
                        active = true,

                        // current user.
                        employee = db.employee.Where(m => m.employee_code.Equals(HttpContext.Current.User.Identity.Name)).FirstOrDefault(),
                        ManualAttendanceId = allLogsForEmployee[i].ConsolidatedAttendanceId,
                        remarks = remark,
                        WhoMark = empCode
                    };

                    if (allLogsForEmployee[i].final_remarks.ToLower() != "off")
                    {
                        allLogsForEmployee[i].manualAttendances.Add(mcons);
                        allLogsForEmployee[i].final_remarks = remark;
                    }

                    //allLogsForEmployee[i].manualAttendances.Add(new DLL.Models.ManualAttendance()
                    //{
                    //    active = true,

                    //    // current user.
                    //    employee = db.employee.Where(m => m.employee_code.Equals(HttpContext.Current.User.Identity.Name)).FirstOrDefault(),
                    //    ManualAttendanceId = allLogsForEmployee[i].ConsolidatedAttendanceId,


                    //    remarks = remark
                    //});

                    //allLogsForEmployee[i].final_remarks = remark;

                    ///////////////////////////////////////////////////////////////////////////////
                }

                db.SaveChanges();
                return "Successfull";  //return null;
            }
        }

        //upload Manual Attendance
        public static string uploadManualAttendance(List<string> csvContents, string user_code)
        {
            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 3 values (columns).

                        if (values == null || values.Length < 4)
                            continue;

                        // Remove unwanted " characters.
                        string employeeCode = values[0].Replace("\"", "");
                        values[1] = values[1].Replace("\"", "");
                        values[2] = values[2].Replace("\"", "");
                        values[3] = values[3].Replace("\"", "");


                        // continue if its the first row (CSV Header line).
                        if (employeeCode == "employeeCode" || employeeCode == "")
                        {
                            continue;
                        }

                        string employeeId = "";
                        var dbEmployeeId = db.employee.Where(c => c.employee_code == employeeCode).FirstOrDefault();
                        if (dbEmployeeId != null)
                        {
                            employeeId = dbEmployeeId.EmployeeId.ToString();
                        }
                        else
                        {
                            continue;
                        }

                        ViewModels.ManualAttendance attendance = new ViewModels.ManualAttendance
                        {
                            employee_code = employeeId,
                            time_in_from = DateTime.Parse(values[1]),
                            time_in_to = DateTime.Parse(values[2]),
                            remarks = values[3]
                        };

                        TimeTune.ManualAttendance.addManualAttendance(attendance, "00000");

                        TimeTune.AuditTrail.insert("{\"*id\" : \"" + employeeId.ToString() + "\",\"from_date\" : \"" + values[1].ToString() + "\",\"to_date\" : \"" + values[2].ToString() + "\",\"remarks\" : \"" + values[3].ToString() + "\"}", "ManualAttendance", user_code);
                    }

                    return "Successfull";
                }


            }

            catch (Exception)
            {
                return "failed";
            }
        }
    }
}
