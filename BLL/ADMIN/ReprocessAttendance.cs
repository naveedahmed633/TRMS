using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Globalization;
using System.Configuration;

namespace BLL
{
    public class MarkPreviousAttendance
    {
        public static HaTransit[] getHaTransitLogs(DateTime from, DateTime to, Employee emp, Context db)
        {
            // to = to.AddDays(1);

            // Get Ha transit logs.
            List<HaTransit> haTransitLogs = new List<HaTransit>();

            // If the employee was specified, get the sorted ha transit logs
            // for that employee only. Else if there was no employee specified
            // get all the ha transit logs between the specified dates.
            if (emp != null)
            {
                haTransitLogs = db.ha_transit.Where(m =>
                    m.C_Unique.Equals(emp.employee_code) &&
                    m.C_Date.Value >= from &&
                    m.C_Date.Value < to).ToList();
            }
            else
            {
                haTransitLogs = db.ha_transit.Where(m =>
                    m.C_Date.Value >= from &&
                    m.C_Date.Value < to).ToList();
            }

            haTransitLogs.Add(new HaTransit()
            {
                active = true,
                C_Date = from,
                C_Time = "00:00:00",
            });

            haTransitLogs.Add(new HaTransit()
            {
                active = true,
                C_Date = to,
                C_Time = "00:00:00",
            });

            haTransitLogs = haTransitLogs.OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToList();

            return haTransitLogs.ToArray();
        }

        private static PersistentAttendanceLog getPersistantLog(Employee emp, DateTime date, Context db)
        {
            // get the shift for employee 'emp' on date
            // 'date'.
            var shift = Utils.getShift(emp, date, db);

            // No shift has been assigned to the employee on this date.
            if (shift == null)
            {
                return null;
            }
            else
            {
                // get the start time for the date.
                DateTime startDateTime =
                    date.Date.Add(shift.start_time.TimeOfDay);

                PersistentAttendanceLog log = new PersistentAttendanceLog();
                log.Employee = emp;
                log.employee_code = emp.employee_code;
                log.time_start = startDateTime.AddSeconds(shift.early_time);
                log.half_day = startDateTime.AddSeconds(shift.half_day);
                log.late_time = startDateTime.AddSeconds(shift.late_time);
                log.shift_end = startDateTime.AddSeconds(shift.shift_end);
                log.time_end = startDateTime.AddSeconds(shift.day_end);
                log.time_in = null;
                log.time_out = null;
                log.dirtyBit = false;
                log.terminal_in = "";
                log.terminal_out = "";
                return log;
            }
        }

        public static PersistentAttendanceLog[] getPersistantAttendanceLogs(Employee emp, DateTime from, Context db)
        {
            // If there was no employee specified, we need to get persistant attendance logs for everybody
            // who uses time tune.
            if (emp == null)
            {
                Employee[] employees = db.employee.Where(m =>
                    m.active &&
                    m.timetune_active).ToArray();

                List<PersistentAttendanceLog> tempList = new List<PersistentAttendanceLog>();

                for (int i = 0; i < employees.Length; i++)
                {
                    PersistentAttendanceLog log = getPersistantLog(employees[i], from, db);
                    if (log != null)
                    {
                        tempList.Add(log);
                    }

                }

                return tempList.ToArray();
            }

            // Else we will only get the persistant log for the employee
            // who was specified.
            List<PersistentAttendanceLog> tempLis = new List<PersistentAttendanceLog>();

            PersistentAttendanceLog lg = getPersistantLog(emp, from, db);
            if (lg != null)
            {
                tempLis.Add(lg);
                return tempLis.ToArray();
            }
            else
            {
                return null;
            }



        }

        public static List<ConsolidatedAttendance> synchAndCompile(DateTime from, DateTime to, Employee emp)
        {
            // This is the list of consolidated logs and it will keep growing with 
            // every iteration. We will ultimately return it.
            List<ConsolidatedAttendance> consLogs = new List<ConsolidatedAttendance>();

            using (var db = new Context())
            {
                Employee employee = null;

                if (emp != null)
                {
                    employee = db.employee.Where(m =>
                    m.EmployeeId.Equals(emp.EmployeeId)).FirstOrDefault();
                }

                // Get ha transit entries between from and to date
                var haTransitLogs = getHaTransitLogs(from, to, employee, db);

                // Get fake persistant attendance logs for employee
                // 'emp' starting at 'from' date
                var persistantLogs = getPersistantAttendanceLogs(employee, from, db);

                if (persistantLogs == null)
                    return consLogs;




                foreach (HaTransit value in haTransitLogs)
                {
                    PersistentAttendanceLog[] dirtyList = Syncher.getDirtyList(persistantLogs, value);

                    if (dirtyList != null && dirtyList.Length > 0)
                    {
                        Compiler.cleanPersistantLogs(dirtyList, value, consLogs, db);
                    }


                    // get the persistant log for the employee that
                    // we are concerned with, only if the time_end is not
                    // null - which would mean that the employee is a freshie
                    // and no shift has been assigned to him.
                    PersistentAttendanceLog paLog = persistantLogs.Where(m =>
                        m.employee_code.Equals(value.C_Unique) &&
                        m.time_end != null).FirstOrDefault();

                    if (paLog == null)
                        continue;


                    // 2)
                    var log = paLog;


                    // 3) check if the log is in dirty list
                    if (log.dirtyBit)
                        continue;


                    // 4)
                    if (Syncher.isEntryValid(value, log))
                    {
                        DateTime entryDateTime = DateTime.ParseExact(
                            value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time,
                            "dd/MM/yyyy HH:mm:ss",
                            CultureInfo.InvariantCulture);


                        if (log.time_in == null)
                        {

                            log.time_in = entryDateTime;
                            log.terminal_in = Utils.getTerminalName(value, db);
                        }
                        else if (entryDateTime < log.time_in.Value)
                        {
                            log.time_in = entryDateTime;

                            log.terminal_in = Utils.getTerminalName(value, db);
                        }
                        else
                        {
                            log.time_out = entryDateTime;

                            log.terminal_out = Utils.getTerminalName(value, db);
                        }
                    }

                    // 5)
                    //value.active = false;
                }
            }


            return consLogs;


        }

        public static string GetAdminWorkingHoursInDay()
        {
            string strWorkingHours = "7";

            using (Context db = new Context())
            {
                strWorkingHours = db.access_code_value.Where(a => a.AccessCode.ToUpper() == "ADMIN_WHOURS_IN_DAY").Select(m => m.AccessValue).FirstOrDefault();
            }

            return strWorkingHours;
        }

        public static string GetClinicalWorkingHoursInDay()
        {
            string strWorkingHours = "8";

            using (Context db = new Context())
            {
                strWorkingHours = db.access_code_value.Where(a => a.AccessCode.ToUpper() == "CLINICAL_WHOURS_IN_DAY").Select(m => m.AccessValue).FirstOrDefault();
            }

            return strWorkingHours;
        }

        public static void createOrUpdateAttendance(string from, string to, int empId, string user_code)
        {
            double _overTime = 0; double _lateTime = 0; int _overTime_Status = 1;
            List<PersistentAttendanceLog> pLog = null;

            DateTime fromDate = DateTime.ParseExact(from, "dd-MM-yyyy",//yyyy-MM-dd => older
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None);
            DateTime toDate = DateTime.ParseExact(to, "dd-MM-yyyy",//yyyy-MM-dd => older
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None).AddDays(1);
            Employee emp = null;

            using (var db = new Context())
            {
                emp = db.employee.Where(m =>
                    m.active &&
                    m.EmployeeId.Equals(empId)).FirstOrDefault();

                pLog = db.persistent_attendance_log.ToList();
            }

            //deactivate those employees who left
            //DeactivateEmployeesLeft();

            string strAdminWorkingHours = GetAdminWorkingHoursInDay();
            string strClinicalWorkingHours = GetClinicalWorkingHoursInDay();

            int iAdminWorkingHours = int.Parse(strAdminWorkingHours);
            int iClinicalWorkingHours = int.Parse(strClinicalWorkingHours);

            List<ConsolidatedAttendance> reprocessedAttendance = synchAndCompile(fromDate, toDate, emp);
            if (reprocessedAttendance != null && reprocessedAttendance.Count > 0)
            {
                using (var db = new Context())
                {
                    foreach (var reprocessedLog in reprocessedAttendance)
                    {
                        int employeeID = reprocessedLog.employee.EmployeeId;

                        // get the corresponding log for the reprocessed log.
                        ConsolidatedAttendance thisEMPca = db.consolidated_attendance.Where(m =>
                            m.date.Value.Equals(reprocessedLog.date.Value) &&
                            m.employee.EmployeeId.Equals(employeeID)).FirstOrDefault();

                        ////_overTime = 0; _lateTime = 0;

                        if (thisEMPca == null)
                        {
                            ///////// START: CALC Overtime and SET Status ///////////////////////////////////
                            var thisEmployee = db.employee.Where(m => m.EmployeeId == employeeID).FirstOrDefault();
                            if (thisEmployee != null)
                            {
                                var data_plog = pLog.Where(p => p.employee_code == thisEmployee.employee_code).FirstOrDefault();
                                if (reprocessedLog.time_in != null && data_plog.late_time != null)
                                {
                                    ////OVERTime Calc - Version 01
                                    //if (reprocessedLog.time_in != null && reprocessedLog.time_out != null) ////4if (reprocessedLog.time_out != null && data_plog.shift_end != null)
                                    //{
                                    //    ////4_overTime = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - data_plog.shift_end.Value.TimeOfDay.TotalSeconds;

                                    //    double seconds_spent_in_office = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds;
                                    //    if (seconds_spent_in_office > (8 * 60 * 60))
                                    //    {
                                    //        _overTime = seconds_spent_in_office - (8 * 60 * 60);
                                    //        _overTimeStatus = 1;
                                    //    }
                                    //    else
                                    //    {
                                    //        _overTime = 0;
                                    //        _overTimeStatus = 0;
                                    //    }
                                    //}

                                    ////LATETime Calc - Version 01
                                    //if (reprocessedLog.status_in == "Late" && reprocessedLog.time_in != null && data_plog.late_time != null) //if (log.time_out != null && log.shift_end != null)
                                    //{
                                    //    //_overTime = log.time_out.Value.TimeOfDay.TotalSeconds - log.shift_end.Value.TimeOfDay.TotalSeconds;

                                    //    double seconds_late_to_office = reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds - data_plog.late_time.Value.TimeOfDay.TotalSeconds;
                                    //    if (seconds_late_to_office > 0)
                                    //    {
                                    //        _lateTime = seconds_late_to_office;
                                    //    }
                                    //    else
                                    //    {
                                    //        _lateTime = 0;
                                    //    }
                                    //}

                                    DateTime dtLate = DateTime.Now;

                                    try
                                    {
                                        dtLate = new DateTime(reprocessedLog.time_in.Value.Year, reprocessedLog.time_in.Value.Month, reprocessedLog.time_in.Value.Day,
                                         data_plog.late_time.Value.Hour, data_plog.late_time.Value.Minute, data_plog.late_time.Value.Second);
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }
                                  
                                    //OVERTime Calc - Version 02
                                    _overTime = 0; _overTime_Status = 0;
                                    if (reprocessedLog.time_in != null && reprocessedLog.time_out != null)
                                    {
                                        double seconds_spent_in_office = 0.0; double seconds_early = 0.0;
                                        //seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                                        seconds_early = (dtLate.TimeOfDay.TotalSeconds - 60.0) - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds;

                                        if (seconds_early >= 0)
                                        {
                                            seconds_spent_in_office = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds - seconds_early;
                                        }
                                        else
                                        {
                                            seconds_spent_in_office = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds;
                                        }

                                        int site_id = 0;
                                        site_id = reprocessedLog.employee.site_id;

                                        if (site_id >= 0)
                                        {
                                            if (site_id == 3)//Clinical=8hrs
                                            {
                                                if (seconds_spent_in_office >= (iClinicalWorkingHours * 60 * 60))//after 8 hours
                                                {
                                                    //add one hour + over sec
                                                    _overTime = (1 * 60 * 60) + (seconds_spent_in_office - (iClinicalWorkingHours * 60 * 60));
                                                    _overTime_Status = 1;
                                                }
                                                else
                                                {
                                                    _overTime = 0;
                                                    _overTime_Status = 0;
                                                }
                                            }
                                            else//Admin and Other=7hrs
                                            {
                                                if (seconds_spent_in_office >= (iAdminWorkingHours * 60 * 60))//after 7 hours
                                                {
                                                    //add one hour + over sec
                                                    _overTime = (1 * 60 * 60) + (seconds_spent_in_office - (iAdminWorkingHours * 60 * 60));
                                                    _overTime_Status = 1;
                                                }
                                                else
                                                {
                                                    _overTime = 0;
                                                    _overTime_Status = 0;
                                                }
                                            }

                                        }
                                    }//if grt 1


                                    ///////////////////////////// Late Deduction //////////////////////////////////////

                                    //LATETime Calc - Version 02
                                    _lateTime = 0;
                                    if (reprocessedLog.status_in == "Late" && reprocessedLog.time_in != null) //if (log.time_out != null && log.shift_end != null)
                                    {
                                        //_overTime = log.time_out.Value.TimeOfDay.TotalSeconds - log.shift_end.Value.TimeOfDay.TotalSeconds;

                                        double late_difference = 0.0, seconds_late_to_office = 0.0;

                                        int SecIn30Min = 30 * 60;
                                        int SecIn01Hour = (1 * 60) * 60, SecIn1_5Hour = (1 * 60 + 30) * 60;//1.5 hours to sec
                                        int SecIn02Hour = (2 * 60) * 60, SecIn2_5Hour = (2 * 60 + 30) * 60;//2.5 hours to sec
                                        int SecIn03Hour = (3 * 60) * 60, SecIn3_5Hour = (3 * 60 + 30) * 60;//3.5 hours to sec
                                        int SecIn04Hour = (4 * 60) * 60, SecIn4_5Hour = (4 * 60 + 30) * 60;//4.5 hours to sec
                                        int SecIn05Hour = (5 * 60) * 60, SecIn5_5Hour = (5 * 60 + 30) * 60;//5.5 hours to sec
                                        int SecIn06Hour = (6 * 60) * 60, SecIn6_5Hour = (6 * 60 + 30) * 60;//6.5 hours to sec
                                        int SecIn07Hour = (7 * 60) * 60, SecIn7_5Hour = (7 * 60 + 30) * 60;//7.5 hours to sec
                                        int SecIn08Hour = (8 * 60) * 60, SecIn8_5Hour = (8 * 60 + 30) * 60;//8.5 hours to sec

                                        //double seconds_late_to_office = log.time_in.Value.TimeOfDay.TotalSeconds - log.late_time.Value.TimeOfDay.TotalSeconds;
                                        //seconds_late_to_office = log.time_in.Value.TimeOfDay.TotalSeconds - (log.late_time.Value.TimeOfDay.TotalSeconds + 60.0);
                                        late_difference = reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds - dtLate.TimeOfDay.TotalSeconds;

                                        //set as late-time to reach office
                                        seconds_late_to_office = late_difference;

                                        //check to fix late-hours as per late-time-range
                                        if (seconds_late_to_office >= 0 && seconds_late_to_office < SecIn30Min)//if one minute passed then deduct 1 hour
                                        {
                                            _lateTime = 60 * 60 * 1.0;//1 hour deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn30Min && seconds_late_to_office < SecIn01Hour)//if 30 minute passed then deduct 1.5 hours
                                        {
                                            _lateTime = 60 * 60 * 1.5;//1.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn01Hour && seconds_late_to_office < SecIn1_5Hour)//if one hour passed then deduct 2 hours
                                        {
                                            _lateTime = 60 * 60 * 2.0;//2 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn1_5Hour && seconds_late_to_office < SecIn02Hour)//if 1.5 hours passed then deduct 2.5 hours
                                        {
                                            _lateTime = 60 * 60 * 2.5;//2.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn02Hour && seconds_late_to_office < SecIn2_5Hour)//if 2 hours passed then deduct 3 hours
                                        {
                                            _lateTime = 60 * 60 * 3.0;//3 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn2_5Hour && seconds_late_to_office < SecIn03Hour)//if 2.5 hours passed then deduct 3.5 hours
                                        {
                                            _lateTime = 60 * 60 * 3.5;//3.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn03Hour && seconds_late_to_office < SecIn3_5Hour)//if 3 hours minute passed then deduct 4 hours
                                        {
                                            _lateTime = 60 * 60 * 4.0;//4 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn3_5Hour && seconds_late_to_office < SecIn04Hour)//if 3.5 hours passed then deduct 4.5 hours
                                        {
                                            _lateTime = 60 * 60 * 4.5;//4.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn04Hour && seconds_late_to_office < SecIn4_5Hour)//if 4 hours passed then deduct 5 hours
                                        {
                                            _lateTime = 60 * 60 * 5.0;//5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn4_5Hour && seconds_late_to_office < SecIn05Hour)//if 4.5 hours passed then deduct 5.5 hours
                                        {
                                            _lateTime = 60 * 60 * 5.5;//5.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn05Hour && seconds_late_to_office < SecIn5_5Hour)//if 5 hours passed then deduct 6 hours
                                        {
                                            _lateTime = 60 * 60 * 6.0;//6 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn5_5Hour && seconds_late_to_office < SecIn06Hour)//if 5.5 hours passed then deduct 6.5 hours
                                        {
                                            _lateTime = 60 * 60 * 6.5;//6.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn06Hour && seconds_late_to_office < SecIn6_5Hour)//if 6 hours passed then deduct 7 hours
                                        {
                                            _lateTime = 60 * 60 * 7.0;//7 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn6_5Hour && seconds_late_to_office < SecIn07Hour)//if 6.5 hours passed then deduct 7.5 hours
                                        {
                                            _lateTime = 60 * 60 * 7.5;//7.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn07Hour && seconds_late_to_office < SecIn7_5Hour)//if 7 hours passed then deduct 8 hours
                                        {
                                            _lateTime = 60 * 60 * 8.0;//8 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn7_5Hour && seconds_late_to_office <= SecIn08Hour)//if 7.5 hours passed then deduct 8.5 hours
                                        {
                                            _lateTime = 60 * 60 * 8.5;//8.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn08Hour && seconds_late_to_office <= SecIn8_5Hour)//if 7.5 hours passed then deduct 8.5 hours
                                        {
                                            _lateTime = 60 * 60 * 9.0;//9 hours deduction
                                        }
                                        else
                                        {
                                            _lateTime = 0;
                                        }
                                    }
                                }
                            }

                            ////4if (_overTime <= 0)
                            ////{
                            ////    _overTimeStatus = 3;//set it discart
                            ////}
                            ////else
                            ////{
                            ////    _overTimeStatus = 1;//set it unapproved
                            ////}
                            ///////// END: CALC Overtime and SET Status ///////////////////////////////////

                            // if the consolidated attendance for the employee
                            // does not exist in the database, then create a new one.
                            db.consolidated_attendance.Add(new ConsolidatedAttendance()
                            {
                                active = true,
                                employee = db.employee.Where(m =>
                                    m.active &&
                                    m.EmployeeId.Equals(employeeID)).FirstOrDefault(),
                                date = reprocessedLog.date,
                                final_remarks = reprocessedLog.final_remarks,
                                description=reprocessedLog.description,
                                manualAttendances = new List<ManualAttendance>(),
                                status_in = reprocessedLog.status_in,
                                status_out = reprocessedLog.status_out,
                                time_in = reprocessedLog.time_in,
                                time_out = reprocessedLog.time_out,
                                terminal_out = reprocessedLog.terminal_out,
                                terminal_in = reprocessedLog.terminal_in,
                                overtime = Convert.ToInt32(_overTime),
                                overtime_status = _overTime_Status,
                                latetime = Convert.ToInt32(_lateTime)
                            });

                            TimeTune.AuditTrail.insert("{\"*id\" : \"" + employeeID + "\"}", "ConsAttn", user_code);
                        }
                        else
                        {
                            ///////// START: CALC Overtime and SET Status ///////////////////////////////////
                            var thisEmployee = db.employee.Where(m => m.EmployeeId == employeeID).FirstOrDefault();
                            if (thisEmployee != null)
                            {
                                var data_plog = pLog.Where(p => p.employee_code == thisEmployee.employee_code).FirstOrDefault();
                                if (reprocessedLog.time_in != null && data_plog.late_time != null)
                                {
                                    //OVERTime Calc - Version 02
                                    //if (reprocessedLog.time_in != null && reprocessedLog.time_out != null) ////5if (reprocessedLog.time_out != null && data_plog.shift_end != null)
                                    //{
                                    //    ////5_overTime = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - data_plog.shift_end.Value.TimeOfDay.TotalSeconds;

                                    //    double seconds_spent_in_office = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds;
                                    //    if (seconds_spent_in_office > (8 * 60 * 60))
                                    //    {
                                    //        _overTime = seconds_spent_in_office - (8 * 60 * 60);
                                    //        _overTime_Status = 1;
                                    //    }
                                    //    else
                                    //    {
                                    //        _overTime = 0;
                                    //        _overTime_Status = 0;
                                    //    }
                                    //}

                                    //LATETime Calc - Version 01
                                    //if (reprocessedLog.status_in == "Late" && reprocessedLog.time_in != null && data_plog.late_time != null) //if (log.time_out != null && log.shift_end != null)
                                    //{
                                    //    //_overTime = log.time_out.Value.TimeOfDay.TotalSeconds - log.shift_end.Value.TimeOfDay.TotalSeconds;

                                    //    double seconds_late_to_office = reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds - data_plog.late_time.Value.TimeOfDay.TotalSeconds;
                                    //    if (seconds_late_to_office > 0)
                                    //    {
                                    //        _lateTime = seconds_late_to_office;
                                    //    }
                                    //    else
                                    //    {
                                    //        _lateTime = 0;
                                    //    }
                                    //}

                                    DateTime dtLate = DateTime.Now;

                                    try
                                    {
                                        dtLate = new DateTime(reprocessedLog.time_in.Value.Year, reprocessedLog.time_in.Value.Month, reprocessedLog.time_in.Value.Day,
                                         data_plog.late_time.Value.Hour, data_plog.late_time.Value.Minute, data_plog.late_time.Value.Second);
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }

                                    //OVERTime Calc - Version 02
                                    _overTime = 0; _overTime_Status = 0;
                                    if (reprocessedLog.time_in != null && reprocessedLog.time_out != null)
                                    {
                                        //DateTime? date = log.time_start.Value.Date.AddDays(1);
                                        //// get the start time for the next date.
                                        //DateTime startDateTime =
                                        //    date.Value.Date.Add(shift.start_time.TimeOfDay);

                                        //log.time_start = startDateTime.AddSeconds(shift.early_time);
                                        //log.half_day = startDateTime.AddSeconds(shift.half_day);
                                        //log.late_time = startDateTime.AddSeconds(shift.late_time);
                                                                               

                                        //DateTime dtLate = new DateTime(reprocessedLog.time_in.Value.Year, reprocessedLog.time_in.Value.Month, reprocessedLog.time_in.Value.Day,
                                        //    data_plog.late_time.Value.Hour, data_plog.late_time.Value.Minute, data_plog.late_time.Value.Second);


                                        double seconds_spent_in_office = 0.0; double seconds_early = 0.0;
                                        //seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                                        seconds_early = (dtLate.TimeOfDay.TotalSeconds - 60.0) - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds;

                                        if (seconds_early >= 0)
                                        {
                                            seconds_spent_in_office = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds - seconds_early;
                                        }
                                        else
                                        {
                                            seconds_spent_in_office = reprocessedLog.time_out.Value.TimeOfDay.TotalSeconds - reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds;
                                        }

                                        int site_id = 0;
                                        site_id = reprocessedLog.employee.site_id;

                                        if (site_id >= 0)
                                        {
                                            if (site_id == 3)//Clinical=8hrs
                                            {
                                                if (seconds_spent_in_office >= (iClinicalWorkingHours * 60 * 60))//after 8 hours
                                                {
                                                    //add one hour + over sec
                                                    _overTime = (1 * 60 * 60) + (seconds_spent_in_office - (iClinicalWorkingHours * 60 * 60));
                                                    _overTime_Status = 1;
                                                }
                                                else
                                                {
                                                    _overTime = 0;
                                                    _overTime_Status = 0;
                                                }
                                            }
                                            else//Admin and Other=7hrs
                                            {
                                                if (seconds_spent_in_office >= (iAdminWorkingHours * 60 * 60))//after 7 hours
                                                {
                                                    //add one hour + over sec
                                                    _overTime = (1 * 60 * 60) + (seconds_spent_in_office - (iAdminWorkingHours * 60 * 60));
                                                    _overTime_Status = 1;
                                                }
                                                else
                                                {
                                                    _overTime = 0;
                                                    _overTime_Status = 0;
                                                }
                                            }

                                        }
                                    }//if grt 1


                                    ///////////////////////////// Late Deduction //////////////////////////////////////

                                    //LATETime Calc - Version 02
                                    _lateTime = 0;
                                    if (reprocessedLog.status_in == "Late" && reprocessedLog.time_in != null) //if (log.time_out != null && log.shift_end != null)
                                    {
                                        //_overTime = log.time_out.Value.TimeOfDay.TotalSeconds - log.shift_end.Value.TimeOfDay.TotalSeconds;

                                        double late_difference = 0.0, seconds_late_to_office = 0.0;

                                        int SecIn30Min = 30 * 60;
                                        int SecIn01Hour = (1 * 60) * 60, SecIn1_5Hour = (1 * 60 + 30) * 60;//1.5 hours to sec
                                        int SecIn02Hour = (2 * 60) * 60, SecIn2_5Hour = (2 * 60 + 30) * 60;//2.5 hours to sec
                                        int SecIn03Hour = (3 * 60) * 60, SecIn3_5Hour = (3 * 60 + 30) * 60;//3.5 hours to sec
                                        int SecIn04Hour = (4 * 60) * 60, SecIn4_5Hour = (4 * 60 + 30) * 60;//4.5 hours to sec
                                        int SecIn05Hour = (5 * 60) * 60, SecIn5_5Hour = (5 * 60 + 30) * 60;//5.5 hours to sec
                                        int SecIn06Hour = (6 * 60) * 60, SecIn6_5Hour = (6 * 60 + 30) * 60;//6.5 hours to sec
                                        int SecIn07Hour = (7 * 60) * 60, SecIn7_5Hour = (7 * 60 + 30) * 60;//7.5 hours to sec
                                        int SecIn08Hour = (8 * 60) * 60, SecIn8_5Hour = (8 * 60 + 30) * 60;//8.5 hours to sec


                                        //DateTime? date = log.time_start.Value.Date.AddDays(1);
                                        //// get the start time for the next date.
                                        //DateTime startDateTime =
                                        //    date.Value.Date.Add(shift.start_time.TimeOfDay);

                                        //log.time_start = startDateTime.AddSeconds(shift.early_time);
                                        //log.half_day = startDateTime.AddSeconds(shift.half_day);
                                        //log.late_time = startDateTime.AddSeconds(shift.late_time);

                                        //DateTime dtLate = new DateTime(reprocessedLog.time_in.Value.Year, reprocessedLog.time_in.Value.Month, reprocessedLog.time_in.Value.Day,
                                        //    data_plog.late_time.Value.Hour, data_plog.late_time.Value.Minute, data_plog.late_time.Value.Second);

                                        //double seconds_late_to_office = log.time_in.Value.TimeOfDay.TotalSeconds - log.late_time.Value.TimeOfDay.TotalSeconds;
                                        //seconds_late_to_office = log.time_in.Value.TimeOfDay.TotalSeconds - (log.late_time.Value.TimeOfDay.TotalSeconds + 60.0);
                                        late_difference = reprocessedLog.time_in.Value.TimeOfDay.TotalSeconds - dtLate.TimeOfDay.TotalSeconds;

                                        //set as late-time to reach office
                                        seconds_late_to_office = late_difference;

                                        //check to fix late-hours as per late-time-range
                                        if (seconds_late_to_office >= 0 && seconds_late_to_office < SecIn30Min)//if one minute passed then deduct 1 hour
                                        {
                                            _lateTime = 60 * 60 * 1.0;//1 hour deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn30Min && seconds_late_to_office < SecIn01Hour)//if 30 minute passed then deduct 1.5 hours
                                        {
                                            _lateTime = 60 * 60 * 1.5;//1.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn01Hour && seconds_late_to_office < SecIn1_5Hour)//if one hour passed then deduct 2 hours
                                        {
                                            _lateTime = 60 * 60 * 2.0;//2 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn1_5Hour && seconds_late_to_office < SecIn02Hour)//if 1.5 hours passed then deduct 2.5 hours
                                        {
                                            _lateTime = 60 * 60 * 2.5;//2.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn02Hour && seconds_late_to_office < SecIn2_5Hour)//if 2 hours passed then deduct 3 hours
                                        {
                                            _lateTime = 60 * 60 * 3.0;//3 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn2_5Hour && seconds_late_to_office < SecIn03Hour)//if 2.5 hours passed then deduct 3.5 hours
                                        {
                                            _lateTime = 60 * 60 * 3.5;//3.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn03Hour && seconds_late_to_office < SecIn3_5Hour)//if 3 hours minute passed then deduct 4 hours
                                        {
                                            _lateTime = 60 * 60 * 4.0;//4 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn3_5Hour && seconds_late_to_office < SecIn04Hour)//if 3.5 hours passed then deduct 4.5 hours
                                        {
                                            _lateTime = 60 * 60 * 4.5;//4.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn04Hour && seconds_late_to_office < SecIn4_5Hour)//if 4 hours passed then deduct 5 hours
                                        {
                                            _lateTime = 60 * 60 * 5.0;//5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn4_5Hour && seconds_late_to_office < SecIn05Hour)//if 4.5 hours passed then deduct 5.5 hours
                                        {
                                            _lateTime = 60 * 60 * 5.5;//5.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn05Hour && seconds_late_to_office < SecIn5_5Hour)//if 5 hours passed then deduct 6 hours
                                        {
                                            _lateTime = 60 * 60 * 6.0;//6 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn5_5Hour && seconds_late_to_office < SecIn06Hour)//if 5.5 hours passed then deduct 6.5 hours
                                        {
                                            _lateTime = 60 * 60 * 6.5;//6.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn06Hour && seconds_late_to_office < SecIn6_5Hour)//if 6 hours passed then deduct 7 hours
                                        {
                                            _lateTime = 60 * 60 * 7.0;//7 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn6_5Hour && seconds_late_to_office < SecIn07Hour)//if 6.5 hours passed then deduct 7.5 hours
                                        {
                                            _lateTime = 60 * 60 * 7.5;//7.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn07Hour && seconds_late_to_office < SecIn7_5Hour)//if 7 hours passed then deduct 8 hours
                                        {
                                            _lateTime = 60 * 60 * 8.0;//8 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn7_5Hour && seconds_late_to_office <= SecIn08Hour)//if 7.5 hours passed then deduct 8.5 hours
                                        {
                                            _lateTime = 60 * 60 * 8.5;//8.5 hours deduction
                                        }
                                        else if (seconds_late_to_office >= SecIn08Hour && seconds_late_to_office <= SecIn8_5Hour)//if 7.5 hours passed then deduct 8.5 hours
                                        {
                                            _lateTime = 60 * 60 * 9.0;//9 hours deduction
                                        }
                                        else
                                        {
                                            _lateTime = 0;
                                        }
                                    }

                                }
                            }

                            ////if (_overTime <= 0)
                            ////{
                            ////    _overTimeStatus = 3;//set it discart
                            ////}
                            ////else
                            ////{
                            ////    _overTimeStatus = 1;//set it unapproved
                            ////}
                            ///////// END: CALC Overtime and SET Status ///////////////////////////////////////////////

                            ManualAttendance[] manualAttendances = thisEMPca.manualAttendances.ToArray();

                            if (manualAttendances != null && manualAttendances.Length > 0)
                                db.manual_attendance.RemoveRange(manualAttendances);

                            /*IR:111 =>
                            ManualAttendance[] manualAttendances = thisEMPca.manualAttendances.ToArray();
                            ManualAttendance[] manualAttendancesLeaves = thisEMPca.manualAttendances.Where(m => m.remarks == DLL.Commons.FinalRemarks.LV).ToArray();

                            if (manualAttendances != null && manualAttendances.Length > 0)
                            {
                                if (manualAttendancesLeaves != null && manualAttendancesLeaves.Length > 0)
                                {
                                    //do nothing
                                }
                                else
                                {
                                    db.manual_attendance.RemoveRange(manualAttendances);
                                }
                            }
                            */
                            ////////////////////////////////////////////////////////////////////////////////

                            thisEMPca.active = true;

                            //fromDB.final_remarks = reprocessedLog.final_remarks;
                            //IR:111 => thisEMPca.final_remarks = manualAttendancesLeaves.Length > 0 ? DLL.Commons.FinalRemarks.LV : reprocessedLog.final_remarks;
                            if (thisEMPca.final_remarks != DLL.Commons.FinalRemarks.LV)
                                thisEMPca.final_remarks = reprocessedLog.final_remarks;
                            thisEMPca.description = reprocessedLog.description;
                            thisEMPca.manualAttendances = new List<ManualAttendance>();
                            thisEMPca.status_in = reprocessedLog.status_in;
                            thisEMPca.status_out = reprocessedLog.status_out;
                            thisEMPca.time_in = reprocessedLog.time_in;
                            thisEMPca.time_out = reprocessedLog.time_out;
                            thisEMPca.terminal_in = reprocessedLog.terminal_in;//IR added
                            thisEMPca.terminal_out = reprocessedLog.terminal_out;//IR added
                            thisEMPca.overtime = Convert.ToInt32(_overTime);//IR added
                            thisEMPca.overtime_status = _overTime_Status;//IR added
                            thisEMPca.latetime = Convert.ToInt32(_lateTime);//IR added

                            TimeTune.AuditTrail.update("{\"*id\" : \"" + thisEMPca.ConsolidatedAttendanceId + "\"}", "ConsAttn", user_code);
                        }
                    }

                    db.SaveChanges();
                }

            }

            #region
            /*
            toDate = toDate.AddDays(1);

             using (var db = new Context())
            {
                 var haTransit=new List<HaTransit>();
                if (empCode != -1)
                {
                    var getEmployee = db.employee.Where(m =>
                           m.active &&
                           m.EmployeeId == empCode
                           ).FirstOrDefault();
                    // get all the entries in ha transit which are active
                    // - they have not been processed.
                    haTransit = db.ha_transit.Where(m => m.C_Date.Value >= fromDate && m.C_Date.Value < toDate && m.C_Unique.Equals(getEmployee.employee_code))
                        .OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToList();
                }
                else
                {
                     haTransit = db.ha_transit.Where(m => m.C_Date.Value >= fromDate && m.C_Date.Value < toDate)
                      .OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToList();
                }

                foreach (HaTransit value in haTransit)
                {




                    // 1.5)
                    // pick the employee for the current ha transit entry

                    var employee = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(value.C_Unique)
                        ).FirstOrDefault();
                    if (employee == null)
                    {
                        continue;
                    }

                    DateTime entryDateTime = DateTime.ParseExact(

               value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time

               , "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    overrideConsolidate(entryDateTime, employee, db);
                    
                    // 5)
                    value.active = false;
                    db.SaveChanges();
                }

                }*/
            #endregion
        }

        public static void DeactivateEmployeesLeft()
        {
            Console.WriteLine("* Deactivating - Employees who Left..."); //Employee Record & Persistant Log

            using (var db = new Context())
            {
                DateTime dtNow = DateTime.Now;
                DateTime dtMinValue = Convert.ToDateTime("1900-01-01 00:00:00.000");

                List<Employee> employees_left = new List<Employee>();

                employees_left = db.employee.Where(m => m.date_of_leaving != dtMinValue && m.date_of_leaving <= dtNow && m.active == true).ToList();
                if (employees_left != null && employees_left.Count > 0)
                {
                    foreach (var e in employees_left)
                    {
                        var emp_pers_log = db.persistent_attendance_log.Where(m => m.employee_code == e.employee_code && m.active == true).OrderByDescending(o => o.PersistentAttendanceLogId).FirstOrDefault();
                        if (emp_pers_log != null)
                        {
                            //inactive the Persistant Log
                            emp_pers_log.active = false;
                        }

                        //inactive the employee and delete it
                        e.active = false;
                        e.timetune_active = false;

                        db.SaveChanges();
                    }
                }
            }
        }

    }





    public class Syncher
    {
        // Checks if the ha transit entry is within the bounds defined
        // by the time_start and time_end attributes in persistent attendance log
        // haLog: the ha transit log entry.
        // paLog: the persistant attendance log entry

        public static bool isEntryValid(HaTransit haLog, PersistentAttendanceLog paLog)
        {

            DateTime startTime = paLog.time_start.Value;
            DateTime endTime = paLog.time_end.Value;

            DateTime entryDateTime = DateTime.ParseExact(

                haLog.C_Date.Value.ToString("dd/MM/yyyy") + " " + haLog.C_Time

                , "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            return (startTime <= entryDateTime && entryDateTime <= endTime);

        }

        public static void overrideConsolidate(DateTime date, Employee employee, Context db)
        {
            DateTime? newDate = date;
            // find if an entry for this date already exists in the consolidated logs.
            var toUpdate = db.consolidated_attendance.Where(m => m.date.Value.Equals(newDate.Value.Date) &&
                m.employee.employee_code.Equals(employee.employee_code)).FirstOrDefault();

            //The object of persistent log is created to update the consolidated Attendance log
            //this is not a db persistent log this is just to use persistance log attributes
            PersistentAttendanceLog persistentLog = new PersistentAttendanceLog();
            // if there's such an entry.
            if (toUpdate != null)
            {
                var shift = Utils.getShift(employee, date, db);
                if (shift == null)
                {
                    Console.WriteLine("Compiler: No Shift For Employee" + employee.employee_code + "on date " + date.Date);
                }
                else
                {

                    persistentLog.start_time = date.Date.Add(shift.start_time.TimeOfDay);
                    persistentLog.time_start = persistentLog.start_time.Value.AddSeconds(shift.early_time);
                    persistentLog.time_end = persistentLog.start_time.Value.AddSeconds(shift.day_end);
                    persistentLog.half_day = persistentLog.start_time.Value.AddSeconds(shift.half_day);
                    persistentLog.late_time = persistentLog.start_time.Value.AddSeconds(shift.late_time);
                    persistentLog.shift_end = persistentLog.start_time.Value.AddSeconds(shift.shift_end);
                    if (persistentLog.time_start <= date && date <= persistentLog.time_end)
                    {
                        if (toUpdate.time_in == null || toUpdate.time_in > date)
                        {
                            toUpdate.time_in = date;
                        }
                        else
                        {
                            toUpdate.time_out = date;
                        }


                        toUpdate.status_in = Utils.getTimeInRemarks(persistentLog);
                        toUpdate.status_out = Utils.getTimeOutRemarks(persistentLog);
                        toUpdate.final_remarks = Utils.getFinalRemarks(toUpdate.status_in, toUpdate.status_out);
                        //Utils.setFinalRemarks(toUpdate);
                    }
                }
                // Get Terminal Name

            }

        }



        // return all the entries earlier than the ha transit entry 'value'.
        public static PersistentAttendanceLog[] getDirtyList(PersistentAttendanceLog[] logs, HaTransit value)
        {


            DateTime haLogDT = DateTime.ParseExact(value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            var dirtyPersistentLogs =
                logs.Where(m =>
                    m.time_end != null &&
                    m.time_start.Value < haLogDT
            ).ToArray();



            //Console.WriteLine(": "+value.HaTransitId + " "+value.C_Date.Value.ToString("yyyy-MM-dd") +",Dirty List Count " + dirtyPersistentLogs.Length);
            return dirtyPersistentLogs;

        }

    }

    public class Compiler
    {
        // clean the persistant attendance logs, and update them upto the date time
        // value.C_Date and value.C_Time
        public static void cleanPersistantLogs(PersistentAttendanceLog[] logs, HaTransit value, List<ConsolidatedAttendance> toSaveIn, Context db)
        {
            // limit is the date of the ha transit value on which this function is called,
            // so we need to bring the persistant logs upto this date.
            DateTime limit = value.C_Date.Value.Date;


            PersistentAttendanceLog[] dirtyLogs;
            do
            {
                // get the logs that need update.
                dirtyLogs = logs.Where(m => m.time_start < limit).ToArray();

                // consolidate them
                consolidatePersistantLogs(dirtyLogs, toSaveIn);

                // update them and break if none of them were updated.
            } while (updatePersistantLogs(dirtyLogs, db) != 0);

        }

        // consolidate the dirty persistant attendance logs.
        public static void consolidatePersistantLogs(PersistentAttendanceLog[] dirtyLogs, List<ConsolidatedAttendance> toSaveIn)
        {
            foreach (PersistentAttendanceLog log in dirtyLogs)
            {
                // if this is a log which was previously consolidated
                // but has not moved on because its shift does not exist
                // for the next day. do nothing!
                string reason = "";
                using(Context db=new Context())
                {
                    DateTime dd = Convert.ToDateTime(log.time_start.Value.Date);
                    string x = dd.ToString("yyyy-MM-dd");
                    DateTime dateTime12 = Convert.ToDateTime(x);
                    var desc = db.general_calender_override.Where(d => d.active && d.date == dd).FirstOrDefault();
                    if (desc != null)
                    {reason = desc.reason;}
                }
                if (log.dirtyBit)
                    continue;

                string statusIn = Utils.getTimeInRemarks(log);
                string statusOut = Utils.getTimeOutRemarks(log);
                string finalRemarks = Utils.getFinalRemarks(statusIn, statusOut);

                toSaveIn.Add(new ConsolidatedAttendance()
                {
                    active = true,
                    employee = log.Employee,
                    time_in = log.time_in,
                    time_out = log.time_out,
                    status_in = statusIn,
                    status_out = statusOut,
                    date = log.time_start.Value.Date,
                    final_remarks = finalRemarks,
                    description=reason,
                    terminal_in = log.terminal_in,
                    terminal_out = log.terminal_out
                });

                log.dirtyBit = true;


            }
        }


        public static int updatePersistantLogs(PersistentAttendanceLog[] dirtyLogs, Context db)
        {
            int logsUpdated = 0;
            foreach (PersistentAttendanceLog log in dirtyLogs)
            {
                Employee emp = log.Employee;


                //PersistentAttendanceLog paLog = emp.persistent_attendance_log;

                // add one day to the current start date.
                // this will give us the next day's shift.
                DateTime? date = log.time_start.Value.Date.AddDays(1);

                // get the shift for employee 'emp' on date
                // 'date'.
                var shift = Utils.getShift(emp, date, db);

                // if the shift is not available print and return
                // without making any changes to the log. This log will 
                // remain dirty until a shift is assigned to the employee
                // on the date.
                if (shift == null)
                {
                    continue;
                    //Console.WriteLine("Compiler: No Shift For Employee" + emp.employee_code + "on date " + date.Value.Date);
                    //log.dirtyBit = true;
                }
                else
                {
                    // get the start time for the next date.
                    DateTime startDateTime =
                        date.Value.Date.Add(shift.start_time.TimeOfDay);

                    log.time_start = startDateTime.AddSeconds(shift.early_time);
                    log.half_day = startDateTime.AddSeconds(shift.half_day);
                    log.late_time = startDateTime.AddSeconds(shift.late_time);
                    log.shift_end = startDateTime.AddSeconds(shift.shift_end);
                    log.time_end = startDateTime.AddSeconds(shift.day_end);
                    log.time_in = null;
                    log.time_out = null;
                    log.dirtyBit = false;

                    logsUpdated++;
                }


            }

            return logsUpdated;
        }


        public static ConsolidatedAttendance[] ConsolidateAll(PersistentAttendanceLog[] dirtyLogs, Context db)
        {
            List<ConsolidatedAttendance> toReturn = new List<ConsolidatedAttendance>();

            foreach (PersistentAttendanceLog log in dirtyLogs)
            {
                // if this is a log which was previously consolidated
                // but has not moved on because its shift does not exist
                // for the next day. do nothing!
                if (log.dirtyBit)
                    continue;

                string statusIn = Utils.getTimeInRemarks(log);
                string statusOut = Utils.getTimeOutRemarks(log);
                //string finalRemarks = Utils.getFinalRemarks(statusIn, statusOut);
                string emp_code = log.employee_code;
                DateTime futureDate = log.shift_end.Value.Date;
                string finalRemarks = "";
                var futureManualAttendance = db.futureManualAttendance.Where(c => c.employee_code.Equals(emp_code) && futureDate >= c.from_date && futureDate <= c.to_date).FirstOrDefault();
                if (futureManualAttendance != null)
                {
                    finalRemarks = futureManualAttendance.remarks;
                }
                else
                {
                    finalRemarks = Utils.getFinalRemarks(statusIn, statusOut);
                }
                toReturn.Add(new ConsolidatedAttendance()
                {
                    active = true,
                    employee = db.employee.Where(m => m.active && m.persistent_attendance_log.PersistentAttendanceLogId.Equals(log.PersistentAttendanceLogId)).FirstOrDefault(),
                    time_in = log.time_in,
                    time_out = log.time_out,
                    status_in = statusIn,
                    status_out = statusOut,
                    date = log.time_start.Value.Date,
                    final_remarks = finalRemarks,
                    terminal_in = log.terminal_in,
                    terminal_out = log.terminal_out
                });


            }

            ConsolidatedAttendance[] arr = toReturn.ToArray();
            db.consolidated_attendance.AddRange(arr);
            //db.SaveChanges();
            return arr;
        }

        // returns the number of persistant logs updated.
        public static int UpdateAll(ConsolidatedAttendance[] toUpdate, Context db)
        {

            int logsUpdated = 0;
            foreach (ConsolidatedAttendance caLog in toUpdate)
            {
                Employee emp = caLog.employee;

                PersistentAttendanceLog paLog = emp.persistent_attendance_log;

                // add one day to the current start date.
                // this will give us the next day's shift.
                DateTime? date = paLog.time_start.Value.Date.AddDays(1);

                // get the shift for employee 'emp' on date
                // 'date'.
                var shift = Utils.getShift(emp, date, db);

                // if the shift is not available print and return
                // without making any changes to the log. This log will 
                // remain dirty until a shift is assigned to the employee
                // on the date.
                if (shift == null)
                {
                    Console.WriteLine("Compiler: No Shift For Employee" + emp.employee_code + "on date " + date.Value.Date);
                    paLog.dirtyBit = true;
                }
                else
                {
                    // get the start time for the next date.
                    DateTime startDateTime =
                        date.Value.Date.Add(shift.start_time.TimeOfDay);

                    paLog.time_start = startDateTime.AddSeconds(shift.early_time);
                    paLog.half_day = startDateTime.AddSeconds(shift.half_day);
                    paLog.late_time = startDateTime.AddSeconds(shift.late_time);
                    paLog.shift_end = startDateTime.AddSeconds(shift.shift_end);
                    paLog.time_end = startDateTime.AddSeconds(shift.day_end);
                    paLog.time_in = null;
                    paLog.time_out = null;
                    paLog.dirtyBit = false;
                    paLog.terminal_in = "";
                    paLog.terminal_out = "";
                    logsUpdated++;
                }


            }

            db.SaveChanges();
            return logsUpdated;
        }

    }

    public class Utils
    {
        public static string getTerminalName(HaTransit value, Context db)
        {
            string terminalName = " ";
            if (value.L_TID != 0)
            {
                var terminal = db.termainal.Where(c => c.L_ID == value.L_TID).FirstOrDefault();
                if (terminal != null)
                {
                    terminalName = terminal.C_Name;
                }
                else
                {
                    terminalName = "";
                }
            }
            return terminalName;
        }
        public static string getTimeInRemarks(PersistentAttendanceLog log)
        {
            // check for a holiday shift
            if (log.time_start.HasValue &&
                log.time_end.HasValue &&
                log.time_start.Value.Equals(log.time_end.Value))
            {

                return DLL.Commons.TimeOutRemarks.OFF;

            }

            // check time in status
            if (log.time_in == null)
            {
                return DLL.Commons.TimeInRemarks.MISS_PUNCH;
            }
            else
            {
                long span = log.late_time.Value.Ticks - log.time_start.Value.Ticks;
                long timeIn = log.time_in.Value.Ticks - log.time_start.Value.Ticks;
                if (timeIn <= span)
                {
                    return DLL.Commons.TimeInRemarks.ON_TIME;
                }
                else
                {
                    return DLL.Commons.TimeInRemarks.LATE;
                }
            }



        }

        public static string getTimeOutRemarks(PersistentAttendanceLog log)
        {
            // check for a holiday shift
            if (log.time_start.HasValue &&
                log.time_end.HasValue &&
                log.time_start.Value.Equals(log.time_end.Value))
            {

                return DLL.Commons.TimeOutRemarks.OFF;

            }
            // check time in status
            if (log.time_out == null)
            {
                return DLL.Commons.TimeOutRemarks.MISS_PUNCH;
            }
            else
            {
                long span = log.time_end.Value.Ticks - log.shift_end.Value.Ticks;
                long timeOut = log.time_out.Value.Ticks - log.shift_end.Value.Ticks;
                if (timeOut < 0)
                {
                    return DLL.Commons.TimeOutRemarks.EARLY_GONE;
                }
                if (timeOut <= span)
                {
                    return DLL.Commons.TimeOutRemarks.ON_TIME;
                }
                else
                {
                    // This is just so that all code paths return a value.
                    // The code must never end up executing this statement.
                    return DLL.Commons.TimeOutRemarks.MISS_PUNCH;
                }
            }



        }

        public static string getFinalRemarks(string status_in, string status_out)
        {
            if (status_in.Equals(DLL.Commons.TimeInRemarks.OFF) &&
                status_out.Equals(DLL.Commons.TimeOutRemarks.OFF))
            {

                return DLL.Commons.FinalRemarks.OFF;

            }
            else if (status_in.Equals(DLL.Commons.TimeInRemarks.ON_TIME) &&
                status_out.Equals(DLL.Commons.TimeOutRemarks.ON_TIME))
            {

                return DLL.Commons.FinalRemarks.PRESENT;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.ON_TIME) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.EARLY_GONE))
            {

                return DLL.Commons.FinalRemarks.POE;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.ON_TIME) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.MISS_PUNCH))
            {

                return DLL.Commons.FinalRemarks.POM;

                // as per HBL
                //return DLL.Commons.FinalRemarks.POE;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.LATE) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.ON_TIME))
            {

                return DLL.Commons.FinalRemarks.PLO;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.LATE) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.EARLY_GONE))
            {

                return DLL.Commons.FinalRemarks.PLE;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.LATE) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.MISS_PUNCH))
            {
                return DLL.Commons.FinalRemarks.PLM;

                // as per HBL
                //return DLL.Commons.FinalRemarks.PLM;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.MISS_PUNCH) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.ON_TIME))
            {

                return DLL.Commons.FinalRemarks.PMO;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.MISS_PUNCH) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.EARLY_GONE))
            {

                return DLL.Commons.FinalRemarks.PME;

            }
            else if (
              status_in.Equals(DLL.Commons.TimeInRemarks.MISS_PUNCH) &&
              status_out.Equals(DLL.Commons.TimeOutRemarks.MISS_PUNCH))
            {

                return DLL.Commons.FinalRemarks.ABSENT;

            }
            else
            {
                return ""; // should never happen
            }


        }


        public static void setFinalRemarks(ConsolidatedAttendance log)
        {

            if (log.status_in.Equals(DLL.Commons.TimeInRemarks.ON_TIME) &&
                log.status_out.Equals(DLL.Commons.TimeOutRemarks.ON_TIME))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.PRESENT;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.ON_TIME) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.EARLY_GONE))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.POE;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.ON_TIME) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.MISS_PUNCH))
            {
                log.final_remarks = DLL.Commons.FinalRemarks.POM;

                // as per HBL
                //log.final_remarks = DLL.Commons.FinalRemarks.POM;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.LATE) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.ON_TIME))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.PLO;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.LATE) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.EARLY_GONE))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.PLE;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.LATE) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.MISS_PUNCH))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.PLM;

                // as per HBL
                //log.final_remarks = DLL.Commons.FinalRemarks.POM;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.MISS_PUNCH) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.ON_TIME))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.PMO;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.MISS_PUNCH) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.EARLY_GONE))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.PME;

            }
            else if (
              log.status_in.Equals(DLL.Commons.TimeInRemarks.MISS_PUNCH) &&
              log.status_out.Equals(DLL.Commons.TimeOutRemarks.MISS_PUNCH))
            {

                log.final_remarks = DLL.Commons.FinalRemarks.ABSENT;

            }


        }

        // used in compiler:
        // gets the shift for the 'employee' on the date 'attendanctDate'.
        public static Shift getShift(Employee emp, DateTime? attendanceDate, Context db)
        {
            // if the employee group is null, it means that the employee lies in the general group.
            if (emp.Group == null || emp.Group.follows_general_calendar)
            {
                Shift toReturn = null;

                int year = attendanceDate.Value.Year;
                var calendar = db.general_calender.Where(m => m.year.Equals(year)).FirstOrDefault();

                if (calendar == null)
                {
                    Console.WriteLine("General calendar for year " + year + " does not exist.");

                    return null;
                }

                else
                {

                    if (emp.Group != null)
                    {
                        DateTime? date = attendanceDate;
                        // 1)
                        var manualShiftAssingned = db.manual_group_shift_assigned.Where(m =>
                            m.date.Value.Equals(date.Value) &&
                            m.Employee.EmployeeId.Equals(emp.EmployeeId) &&
                            m.Group.GroupId.Equals(emp.Group.GroupId))
                            .FirstOrDefault();

                        if (manualShiftAssingned != null)
                        {
                            toReturn = manualShiftAssingned.Shift;
                        }
                    }
                    // FOR GENERAL SHIFT EMPLOYEES:
                    // 1) first check for a general calendar override.
                    // 2) then check for day shift
                    // 3) then check for general shift.

                    var generalCalendarOverride = calendar.calendarOverrides.Where(m => m.active && m.date.Value.Equals(attendanceDate.Value.Date)).FirstOrDefault();

                    // 1)
                    if (generalCalendarOverride != null)
                    {
                        Shift forceLoad = generalCalendarOverride.Shift;

                        toReturn = generalCalendarOverride.Shift;
                    }
                    else
                    {
                        // 2) && 3)
                        string day = attendanceDate.Value.DayOfWeek.ToString();
                        switch (day)
                        {
                            case "Monday":
                                toReturn = (calendar.Shift1 != null) ? calendar.Shift1 : calendar.generalShift;
                                break;
                            case "Tuesday":
                                toReturn = (calendar.Shift2 != null) ? calendar.Shift2 : calendar.generalShift;
                                break;
                            case "Wednesday":
                                toReturn = (calendar.Shift3 != null) ? calendar.Shift3 : calendar.generalShift;
                                break;
                            case "Thursday":
                                toReturn = (calendar.Shift4 != null) ? calendar.Shift4 : calendar.generalShift;
                                break;
                            case "Friday":
                                toReturn = (calendar.Shift5 != null) ? calendar.Shift5 : calendar.generalShift;
                                break;
                            case "Saturday":
                                toReturn = (calendar.Shift6 != null) ? calendar.Shift6 : calendar.generalShift;
                                break;
                            case "Sunday":
                                toReturn = (calendar.Shift != null) ? calendar.Shift : calendar.generalShift;
                                break;
                        }
                    }

                }

                if (toReturn == null)
                {
                    Console.WriteLine("Cannot find shift for employee " + emp.employee_code + " on date." + attendanceDate);

                }

                return toReturn;
            }
            else
            {
                Shift toReturn = null;

                DateTime? date = attendanceDate;

                // FOR GROUP EMPLOYEES:
                // 1) first check for manual shift assigned
                // 2) then check for group calendar overrides
                // 3) then check for group calendar day shift
                // 4) then check for group calendar general shift


                // 1)
                var manualShiftAssingned = db.manual_group_shift_assigned.Where(m =>
                    m.date.Value.Equals(date.Value) &&
                    m.Employee.EmployeeId.Equals(emp.EmployeeId) &&
                    m.Group.GroupId.Equals(emp.Group.GroupId))
                    .FirstOrDefault();

                if (manualShiftAssingned != null)
                {
                    toReturn = manualShiftAssingned.Shift;
                }
                else
                {
                    // 2) 
                    var groupOverride = db.group_calendar_overrides.Where(m =>
                        m.active &&
                        m.date.Value.Equals(date.Value) &&
                        m.GroupCalendar.Group.GroupId.Equals(emp.Group.GroupId))
                        .FirstOrDefault();

                    if (groupOverride != null)
                    {
                        toReturn = groupOverride.Shift;
                    }
                    else
                    {
                        // 3) & 4)
                        string day = attendanceDate.Value.DayOfWeek.ToString();

                        var groupGeneralcalendar = db.group_calendar.Where(m =>
                            m.active &&
                            m.year.Equals(attendanceDate.Value.Year) &&
                            m.Group.GroupId.Equals(emp.Group.GroupId))
                            .FirstOrDefault();

                        if (groupGeneralcalendar == null)
                        {
                            Console.WriteLine("Calendar for group " + emp.Group.GroupId + ", for year " + attendanceDate.Value.Year + " does not exist.");
                            return null;
                        }
                        switch (day)
                        {
                            case "Monday":
                                toReturn = (groupGeneralcalendar.monday != null) ? groupGeneralcalendar.monday : groupGeneralcalendar.generalShift;
                                break;
                            case "Tuesday":
                                toReturn = (groupGeneralcalendar.tuesday != null) ? groupGeneralcalendar.tuesday : groupGeneralcalendar.generalShift;
                                break;
                            case "Wednesday":
                                toReturn = (groupGeneralcalendar.wednesday != null) ? groupGeneralcalendar.wednesday : groupGeneralcalendar.generalShift;
                                break;
                            case "Thursday":
                                toReturn = (groupGeneralcalendar.thursday != null) ? groupGeneralcalendar.thursday : groupGeneralcalendar.generalShift;
                                break;
                            case "Friday":
                                toReturn = (groupGeneralcalendar.friday != null) ? groupGeneralcalendar.friday : groupGeneralcalendar.generalShift;
                                break;
                            case "Saturday":
                                toReturn = (groupGeneralcalendar.saturday != null) ? groupGeneralcalendar.saturday : groupGeneralcalendar.generalShift;
                                break;
                            case "Sunday":
                                toReturn = (groupGeneralcalendar.sunday != null) ? groupGeneralcalendar.sunday : groupGeneralcalendar.generalShift;
                                break;
                        }
                    }

                }

                if (toReturn == null)
                {
                    Console.WriteLine("Cannot find shift for employee " + emp.employee_code + ", of group " + emp.Group.GroupId + ", on date." + attendanceDate);

                }

                return toReturn;
            }
        }

    }
}
