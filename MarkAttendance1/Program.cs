using System;
using DLL.Models;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Configuration;

// for calling sleep.
using System.Threading;
using System.IO;
using System.Text;

namespace MarkAttendance1
{
    public class Syncher
    {
        // Checks if the ha transit entry is within the bounds defined
        // by the time_start and time_end attributes in persistent attendance log
        // haLog: the ha transit log entry.
        // paLog: the persistant attendance log entry
        
        public static bool isEntryValid(HaTransit haLog, PersistentAttendanceLog paLog, Employee employee, Context db)
        {

            DateTime startTime = paLog.time_start.Value;
            DateTime endTime = paLog.time_end.Value;

            DateTime entryDateTime = DateTime.ParseExact(

                haLog.C_Date.Value.ToString("dd/MM/yyyy") + " " + haLog.C_Time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);



            // TODO:
            // There can be a situation where, devices connect after a long period
            // and send in their logs, these logs can be such that they do not,
            // fit into the current time_start and the time_end values of the 
            // persistant attendance log. For such entries we need to reprocess
            // the attendance and bypass the current persistant logs, using 
            // an intermediate data structure which will not reside in the
            // database tables.

            /*
            if (entryDateTime < startTime)
            {
                overrideConsolidate(entryDateTime,employee,db);
            }
             */
            // return true if the time of the log entry lies between
            // startTime and endTime of the persistant attendance log.

            return (startTime <= entryDateTime && entryDateTime <= endTime);

        }

        public static void overrideConsolidate(DateTime date, Employee employee, Context db)
        {
            string reason = "";
            DateTime? newDate = date;

            DateTime dd = Convert.ToDateTime(date.Date);
            var desc = db.general_calender_override.Where(d => d.active && d.date == dd).FirstOrDefault();
            if (desc != null)
            { reason = desc.reason; }

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
                    string strMessage = "Compiler: No Shift For Employee " + employee.employee_code + " on date " + date.Date;
                    Program.WriteToLog(strMessage);
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
                            toUpdate.time_out = date;

                        toUpdate.status_in = Utils.getTimeInRemarks(persistentLog);
                        toUpdate.status_out = Utils.getTimeOutRemarks(persistentLog);
                        toUpdate.final_remarks = Utils.getFinalRemarks(toUpdate.status_in, toUpdate.status_out);
                        toUpdate.description = reason;
                        //Utils.setFinalRemarks(toUpdate);
                    }
                }
            }
        }

        public static void checkForFreshies()
        {
            if (!Utils.effectiveDateTime.HasValue)
            {
                string strMessage = "Invlaid attendance effective date.";
                Program.WriteToLog(strMessage);

                Environment.Exit(0);
            }

            using (var db = new Context())
            {
                // get all the persistant logs where
                // time_start and time_end are null.
                var freshies = db.persistent_attendance_log.Where(m =>
                    m.active &&
                    m.time_start.Equals(null) &&
                    m.time_end.Equals(null)
                    ).ToList();

                if (freshies == null)
                {
                    string strMessage = "No new employees.";
                    Program.WriteToLog(strMessage);

                    return;
                }



                foreach (var entity in freshies)
                {
                    // get the employee
                    var employee = db.employee.Where(m =>
                        m.active &&
                        m.persistent_attendance_log.PersistentAttendanceLogId.Equals(entity.PersistentAttendanceLogId)
                        ).FirstOrDefault();
                    /*
                    if (employee.employee_code.Equals("578821"))
                    {
                        Program.WriteToLog("Stopping..");
                    }*/
                    if (employee == null)
                    {
                        // Program.WriteToLog("Persistant attendance log for Employee " + employee.employee_code+" not found.");
                        continue;
                    }

                    DateTime? dateOfJoining = employee.date_of_joining;

                    if (!dateOfJoining.HasValue)
                    {
                        string strMessage = "employee " + employee.employee_code + " does not have a date of joining.";
                        Program.WriteToLog(strMessage);

                        continue;
                    }

                    // if the date of joining is earlier than the date after which
                    // attendance is considered. chose the latter.
                    DateTime firstDay =
                        ((dateOfJoining.Value.Date > Utils.effectiveDateTime.Value.Date)) ? dateOfJoining.Value.Date : Utils.effectiveDateTime.Value;



                    // get the shift for the freshie.
                    // on the date of joining.
                    var shift = Utils.getShift(employee, firstDay, db);


                    if (shift == null)
                    {
                        //Program.WriteToLog("No Shift For Employee" + employee.employee_code);
                        continue;
                    }


                    DateTime startDateTime = firstDay.Add(shift.start_time.TimeOfDay);

                    entity.time_start = startDateTime.AddSeconds(shift.early_time);
                    entity.half_day = startDateTime.AddSeconds(shift.half_day);
                    entity.late_time = startDateTime.AddSeconds(shift.late_time);
                    entity.shift_end = startDateTime.AddSeconds(shift.shift_end);
                    entity.time_end = startDateTime.AddSeconds(shift.day_end);
                }



                db.SaveChanges();
            }
        }


        // return all the entries earlier than the ha transit entry 'value'.
        public static PersistentAttendanceLog[] getDirtyList(Context db, HaTransit value)
        {


            DateTime haLogDT = DateTime.ParseExact(value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            var dirtyPersistentLogs =
                db.persistent_attendance_log.Where(m =>
                    m.active &&
                    m.time_end != null &&
                    m.time_end.Value < haLogDT
            ).ToArray();



            //Program.WriteToLog(": "+value.HaTransitId + " "+value.C_Date.Value.ToString("yyyy-MM-dd") +",Dirty List Count " + dirtyPersistentLogs.Length);
            return dirtyPersistentLogs.ToArray();

        }

    }

    public class Compiler
    {
        public static string strAdminWorkingHours = GetAdminWorkingHoursInDay();
        public static string strClinicalWorkingHours = GetClinicalWorkingHoursInDay();

        public static int iAdminWorkingHours = int.Parse(strAdminWorkingHours);
        public static int iClinicalWorkingHours = int.Parse(strClinicalWorkingHours);

        // clean the persistant attendance logs, and update them upto the date time
        // value.C_Date and value.C_Time
        public static void cleanPersistantLogs(PersistentAttendanceLog[] logs, HaTransit value, Context db)
        {
            int counter = 0;
            DateTime limit =
                DateTime.ParseExact(value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);


            PersistentAttendanceLog[] dirtyLogs;
            do
            {
                // get the logs that need update.
                dirtyLogs = logs.Where(m => m.time_end < limit).ToArray();

                // consolidate them
                consolidatePersistantLogs(dirtyLogs, db);

                counter++;

                string strMessage = "\tHaTransit Date: " + limit + " & Dirty Logs: " + dirtyLogs.Count() + " & Counter: " + counter;
                Program.WriteToLog(strMessage);
                //Program.WriteToLog("\tHaTransit Date: " + limit + " & Dirty Logs: " + dirtyLogs.Count() + " & Counter: " + counter);

                if (counter == 25)
                {
                    counter = 0;

                    db.SaveChanges();
                }

                // update them and break if none of them were updated.
            } while (updatePersistantLogs(dirtyLogs, db) != 0);

            db.SaveChanges();
        }

        // consolidate the dirty persistant attendance logs.
        //terminal addded in consolidated attendance logs

        public static void consolidatePersistantLogs(PersistentAttendanceLog[] dirtyLogs, Context db)
        {
            string reason = "";
            List<ConsolidatedAttendance> consolidatedLogs = new List<ConsolidatedAttendance>();
            foreach (PersistentAttendanceLog log in dirtyLogs)
            {
                DateTime dd = Convert.ToDateTime(log.time_start.Value.Date);
                var desc = db.general_calender_override.Where(d => d.active && d.date == dd).FirstOrDefault();
                if (desc != null)
                { reason = desc.reason; }
                // if this is a log which was previously consolidated
                // but has not moved on because its shift does not exist
                // for the next day. do nothing!
                if (log.dirtyBit)
                    continue;

                string statusIn = Utils.getTimeInRemarks(log);
                string statusOut = Utils.getTimeOutRemarks(log);
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

                /////////////////////////// Find Unique Records /////////////////////////////////

                DateTime dtValue = DateTime.Now;
                dtValue = log.time_start.Value.Date;

                //if (log.Employee.EmployeeId == 28805)
                //{
                //    if (log.time_in == new DateTime(2018, 05, 09, 8, 45, 52) || log.time_in == new DateTime(2018, 05, 10, 8, 43, 43) || log.time_in == new DateTime(2018, 05, 11, 8, 49, 02) || log.time_in == new DateTime(2018, 05, 12, 8, 49, 21))
                //    {
                //        int a = 0;
                //        a = a + 1;
                //    }
                //}

                ////OVERTime Calc - Version 01
                //double _overTime = 0; int _overTime_Status = 0;
                //if (log.time_in != null && log.time_out != null) //if (log.time_out != null && log.shift_end != null)
                //{
                //    //_overTime = log.time_out.Value.TimeOfDay.TotalSeconds - log.shift_end.Value.TimeOfDay.TotalSeconds;

                //    double seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                //    if (seconds_spent_in_office > (iWorkingHours * 60 * 60))
                //    {
                //        _overTime = seconds_spent_in_office - (iWorkingHours * 60 * 60);
                //        _overTime_Status = 1;
                //    }
                //    else
                //    {
                //        _overTime = 0;
                //        _overTime_Status = 0;
                //    }
                //}

                ////LATETime Calc - Version 01
                //double _lateTime = 0;
                //if (statusIn == "Late" && log.time_in != null && log.late_time != null) //if (log.time_out != null && log.shift_end != null)
                //{
                //    //_overTime = log.time_out.Value.TimeOfDay.TotalSeconds - log.shift_end.Value.TimeOfDay.TotalSeconds;

                //    double seconds_late_to_office = log.time_in.Value.TimeOfDay.TotalSeconds - log.late_time.Value.TimeOfDay.TotalSeconds;
                //    if (seconds_late_to_office > 0)
                //    {
                //        _lateTime = seconds_late_to_office;
                //    }
                //    else
                //    {
                //        _lateTime = 0;
                //    }
                //}

                //OVERTime Calc - Version 02
                double _overTime = 0; int _overTime_Status = 0;
                if (log.time_in != null && log.time_out != null)
                {
                    double seconds_spent_in_office = 0.0; double seconds_early = 0.0;
                    //seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                    seconds_early = (log.late_time.Value.TimeOfDay.TotalSeconds - 60.0) - log.time_in.Value.TimeOfDay.TotalSeconds;

                    if (seconds_early >= 0)
                    {
                        seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds - seconds_early;
                    }
                    else
                    {
                        seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                    }

                    int site_id = 0;
                    site_id = log.Employee.site_id;

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
                double _lateTime = 0;
                if (statusIn == "Late" && log.time_in != null) //if (log.time_out != null && log.shift_end != null)
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
                    late_difference = log.time_in.Value.TimeOfDay.TotalSeconds - log.late_time.Value.TimeOfDay.TotalSeconds;

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

                //var data_ca = db.consolidated_attendance.Where(c => c.employee.EmployeeId == log.Employee.EmployeeId && c.date == dtValue &&
                //    c.time_in == log.time_in && c.time_out == log.time_out).FirstOrDefault();

                var data_ca = db.consolidated_attendance.Where(c => c.employee.EmployeeId == log.Employee.EmployeeId && c.date == dtValue).FirstOrDefault();
                if (data_ca == null)
                {
                    consolidatedLogs.Add(new ConsolidatedAttendance()
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
                        terminal_out = log.terminal_out,
                        overtime = Convert.ToInt32(_overTime),
                        overtime_status = _overTime_Status, //1,
                        latetime = Convert.ToInt32(_lateTime),
                        is_payroll_synced = false
                    });
                }

                log.dirtyBit = true;

                ////////////////////////////////////////////////////////////////////////////////
            }

            db.consolidated_attendance.AddRange(consolidatedLogs.ToArray());
        }

        //Terminal added in update
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

                Program.WriteToLog("\t\tPerst time_st+1day: " + date + " & EmployeeCode: " + emp.employee_code);


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
                    //Program.WriteToLog("Compiler: No Shift For Employee" + emp.employee_code + "on date " + date.Value.Date);
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
                    log.terminal_in = " ";
                    log.terminal_out = " ";
                    logsUpdated++;
                }


            }

            string strMessage = "\t\tCount Left: " + logsUpdated;
            Program.WriteToLog(strMessage);

            return logsUpdated;
        }

        //Terminal Added in Consolidated all
        public static ConsolidatedAttendance[] ConsolidateAll(PersistentAttendanceLog[] dirtyLogs, Context db)
        {
            string reason = "";
            List<ConsolidatedAttendance> toReturn = new List<ConsolidatedAttendance>();

            foreach (PersistentAttendanceLog log in dirtyLogs)
            {
                DateTime dd = Convert.ToDateTime(log.time_start.Value.Date);
                var desc = db.general_calender_override.Where(d => d.active && d.date == dd).FirstOrDefault();
                if (desc != null)
                { reason = desc.reason; }
                // if this is a log which was previously consolidated
                // but has not moved on because its shift does not exist
                // for the next day. do nothing!
                if (log.dirtyBit)
                    continue;

                string statusIn = Utils.getTimeInRemarks(log);
                string statusOut = Utils.getTimeOutRemarks(log);
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
                    description=reason,
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
                    string strMessage = "Compiler: No Shift For Employee" + emp.employee_code + "on date " + date.Value.Date;
                    Program.WriteToLog(strMessage);
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
                    paLog.terminal_in = " ";
                    paLog.terminal_out = " ";
                    logsUpdated++;
                }


            }

            db.SaveChanges();
            return logsUpdated;
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

    }

    public class Utils
    {
        //public static DateTime? effectiveDateTime = DateTime.ParseExact(ConfigurationManager.AppSettings["attendance-effective-date"], "dd-MM-yyyy", CultureInfo.InvariantCulture);

        public static string strDate = GetEffectiveDate();

        public static DateTime? effectiveDateTime = DateTime.ParseExact(strDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);


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

        public static string getTimeInRemarks_FlexiMin_New(PersistentAttendanceLog log)
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
                int iFlexiMinutes = 0;//call from database Access Code value

                long timeIn = 0;
                timeIn = log.time_in.Value.Ticks - log.time_start.Value.Ticks;

                long span = 0;
                if (iFlexiMinutes == 0)
                    span = log.late_time.Value.Ticks - log.time_start.Value.Ticks;
                else
                    span = log.late_time.Value.AddMinutes(iFlexiMinutes).Ticks - log.time_start.Value.Ticks;

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

        public static string getTimeOutRemarks_FlexiMin_New(PersistentAttendanceLog log)
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
                int iFlexiMinutes = 0;//call from database Access Code value

                long timeOut = 0;
                if (iFlexiMinutes == 0)
                    timeOut = log.time_out.Value.Ticks - log.shift_end.Value.Ticks;
                else
                    timeOut = log.shift_end.Value.AddMinutes(iFlexiMinutes).Ticks - log.time_out.Value.Ticks;

                long span = 0;
                if (iFlexiMinutes == 0)
                    span = log.time_end.Value.Ticks - log.shift_end.Value.Ticks;
                else
                    span = log.time_end.Value.Ticks - log.shift_end.Value.AddMinutes(iFlexiMinutes).Ticks;

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
                //IR enabled
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
                //IR enabled
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

        public static Shift getShift(Employee employee, DateTime? attendanceDate, Context db)
        {
            int year = DateTime.Now.Year; //IR added line

            // if the employee group is null, it means that the employee lies in the general group.
            if (employee.Group == null || employee.Group.follows_general_calendar)
            {
                Shift toReturn = null;

                year = attendanceDate.Value.Year; //IR removed - int

                var calendar = db.general_calender.Where(m => m.year.Equals(year)).FirstOrDefault();

                if (calendar == null)
                {
                    string strMessage = "General calendar for year " + year + " does not exist.";
                    Program.WriteToLog(strMessage);

                    return null;
                }

                else
                {

                    if (employee.Group != null)
                    {
                        DateTime? date = attendanceDate;
                        // 1)
                        var manualShiftAssingned = db.manual_group_shift_assigned.Where(m =>
                            m.date.Value.Equals(date.Value) &&
                            m.Employee.EmployeeId.Equals(employee.EmployeeId) &&
                            m.Group.GroupId.Equals(employee.Group.GroupId))
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

                    //IR added 'active' condition
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
                    string strMessage = "Cannot find shift for employee " + employee.employee_code + " on date." + attendanceDate;
                    Program.WriteToLog(strMessage);
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
                    m.Employee.EmployeeId.Equals(employee.EmployeeId) &&
                    m.Group.GroupId.Equals(employee.Group.GroupId))
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
                        m.GroupCalendar.Group.GroupId.Equals(employee.Group.GroupId))
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
                            m.Group.GroupId.Equals(employee.Group.GroupId))
                            .FirstOrDefault();

                        if (groupGeneralcalendar == null)
                        {
                            string strMessage = "Calendar for group " + employee.Group.GroupId + ", for year " + attendanceDate.Value.Year + " does not exist.";
                            Program.WriteToLog(strMessage);

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
                    Program.WriteToLog("Cannot find shift for employee " + employee.employee_code + ", of group " + employee.Group.GroupId + ", on date." + attendanceDate);

                    //IR: change made on 10th Nov 2019
                    var shiftGeneralCalendar = db.general_calender.Where(m => m.year.Equals(year)).FirstOrDefault();
                    toReturn = shiftGeneralCalendar.generalShift;
                }

                return toReturn;
            }
        }

        // Get Terminal Name
        public static string getTerminalName(HaTransit value, Context db)
        {
            string terminalName = " ";
            if (value.L_TID != 0)
            {
                Terminals terminal = null;

                if (value.L_TID == null)
                    terminal = db.termainal.Where(c => c.L_ID.Equals(value.L_TID)).FirstOrDefault();
                else
                    terminal = db.termainal.Where(c => c.L_ID == value.L_TID).FirstOrDefault();


                //var terminal = db.termainal.Where(c => c.L_ID.Equals(value.L_TID)).FirstOrDefault();
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

        public static string GetEffectiveDate()
        {
            string strEffectiveDate = string.Empty;

            using (Context db = new Context())
            {
                strEffectiveDate = db.access_code_value.Where(a => a.AccessCode.ToUpper() == "EFFECTIVE_DATE").Select(m => m.AccessValue).FirstOrDefault();
            }

            return strEffectiveDate;
        }

        public static bool GetLogStatus(string code)
        {
            bool bStatus = false;

            using (Context db = new Context())
            {
                var data_log = db.access_code_value.Where(a => a.AccessCode.ToUpper() == code.ToUpper()).FirstOrDefault();
                if (data_log != null)
                {
                    if (data_log.AccessValue == "1")
                    {
                        bStatus = true;
                    }
                }
            }

            return bStatus;
        }
    }

    public class LeaveWaveInfo
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public decimal DaysCount { get; set; }
        public int LeaveRemainder { get; set; }
    }

    public class ConsLeaveInfo
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public int EmployeeTypeID { get; set; }
        public int LateCount { get; set; }

        public int LeaveDCountBy3 { get; set; }
    }
    class Program
    {
        public static string strWorkingHours = GetWorkingHoursInDay();
        public static int iWorkingHours = int.Parse(strWorkingHours);

        public static void AddHaTransitDefaultEntry()
        {
            using (var db = new Context())
            {
                HaTransit defEntry = new HaTransit();
                defEntry.C_Name = "DEFAULT";
                defEntry.C_Unique = "000000";
                defEntry.C_Date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                defEntry.C_Time = DateTime.Now.ToString("HH:mm:ss");
                defEntry.L_UID = 1;
                defEntry.active = true;
                defEntry.course_active = false;
                defEntry.L_TID = 1;

                db.ha_transit.Add(defEntry);
                db.SaveChanges();
            }

            Program.WriteToLog("* Default HaTransit - Entry Added");
        }

        public static void DeactivateEmployeesLeft()
        {
            Program.WriteToLog("* Deactivating - Employees who Left..."); //Employee Record & Persistant Log

            using (var db = new Context())
            {
                DateTime dtNow = DateTime.Now;
                DateTime dtMinValue = Convert.ToDateTime("1900-01-01 00:00:00.000");

                int counter = 1;
                List<Employee> employees_left = new List<Employee>();

                employees_left = db.employee.Where(m => m.active && m.date_of_leaving != dtMinValue && m.date_of_leaving < dtNow).ToList();
                if (employees_left != null && employees_left.Count > 0)
                {
                    foreach (var e in employees_left)
                    {
                        var emp_pers_log = db.persistent_attendance_log.Where(m => m.active && m.employee_code == e.employee_code).FirstOrDefault(); //.OrderByDescending(o => o.PersistentAttendanceLogId).FirstOrDefault();
                        if (emp_pers_log != null)
                        {
                            ////inactive the Persistant Log
                            emp_pers_log.active = false;
                            Program.WriteToLog("\tDeactive Pers - EmpCode=" + e.employee_code);

                            //if (emp_pers_log.time_in == null && emp_pers_log.time_out == null && emp_pers_log.time_start == null && emp_pers_log.time_end == null &&
                            //    emp_pers_log.start_time == null && emp_pers_log.late_time == null && emp_pers_log.half_day == null && emp_pers_log.shift_end == null)
                            //{
                            //    db.persistent_attendance_log.Remove(emp_pers_log);
                            //    Program.WriteToLog("\t\tDelete Pers - EmpCode=" + e.employee_code);
                            //}
                        }

                        //inactive the employee and delete it
                        e.active = false;
                        e.timetune_active = false;

                        db.SaveChanges();

                        Program.WriteToLog("\tDeactive Emp #" + counter + ", LeftDate=" + e.date_of_leaving + " & EmpCode=" + e.employee_code);
                        counter++;
                    }
                }
            }
        }

        public static void synchAndCompile()
        {
            Program.WriteToLog("* Syncher started! ");
            Program.WriteToLog("* Processing HA transit entries... ");

            using (var db = new Context())
            {
                // get all the entries in ha transit which are active
                // - they have not been processed.
                var haTransit = db.ha_transit.Where(m => m.active && m.C_Date.Value >= Utils.effectiveDateTime.Value)
                    .OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToArray();


                foreach (HaTransit value in haTransit)
                {

                    PersistentAttendanceLog[] dirtyList = Syncher.getDirtyList(db, value);


                    //Program.WriteToLog(value.C_Date.Value.ToString("dd-MM-yyyy"));


                    if (dirtyList != null && dirtyList.Length > 0)
                    {
                        Console.Write("\nCompiling: " + value.C_Date.Value.ToString("dd-MM-yyyy"));
                        Compiler.cleanPersistantLogs(dirtyList, value, db);
                        Program.WriteToLog("Done!");
                    }

                    // 1.5)
                    // pick the employee for the current ha transit entry
                    // only if the time end in its persistent attendance log is not
                    // null. That is, if the employee is a freshie and its shift is missing.
                    string transitEmployeeCode = (value.C_Unique ?? string.Empty).Trim();
                    var employee = db.employee.Where(m =>
                        m.active &&
                        m.employee_code != null &&
                        m.employee_code.Trim() == transitEmployeeCode &&
                        m.persistent_attendance_log.time_end != null
                        ).FirstOrDefault();
                    if (employee == null)
                    {
                        continue;
                    }



                    // 2)
                    var log = employee.persistent_attendance_log;

                    // 3) check if the log is in dirty list
                    if (log.dirtyBit)
                        continue;


                    // 4)
                    if (Syncher.isEntryValid(value, log, employee, db))
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
                    value.active = false;

                }

                // Final pass ensures logs that have ended by "now" get consolidated
                // even when no newer device punch exists yet.
                HaTransit finalFlushTransit = new HaTransit
                {
                    C_Date = DateTime.Now.Date,
                    C_Time = DateTime.Now.ToString("HH:mm:ss")
                };
                PersistentAttendanceLog[] pendingDirtyLogs = Syncher.getDirtyList(db, finalFlushTransit);
                if (pendingDirtyLogs != null && pendingDirtyLogs.Length > 0)
                {
                    Program.WriteToLog("* Final consolidation flush for pending logs...");
                    Compiler.cleanPersistantLogs(pendingDirtyLogs, finalFlushTransit, db);
                }


                Console.Write("* Commiting changes to database... ");
                db.SaveChanges();
                Program.WriteToLog("Done!");
            }
        }

        public static void ExemptedEmployeesMarking()
        {
            Program.WriteToLog("* Exempting - Employees marked in the list..."); //Employee coded added to Exempted List

            using (var db = new Context())
            {
                List<ExemptedEmployee> employees_exempted = new List<ExemptedEmployee>();

                employees_exempted = db.exempted_employee.Where(m => m.EmployeeId > 0).ToList();
                if (employees_exempted != null && employees_exempted.Count > 0)
                {
                    foreach (var x in employees_exempted)
                    {
                        //var emp_cons_logs = db.consolidated_attendance.Where(m => m.active && m.employee.EmployeeId == x.EmployeeId && m.final_remarks != "EXM").ToList();
                        var emp_cons_logs = db.consolidated_attendance.Where(m => m.active && m.employee.EmployeeId == x.EmployeeId && m.final_remarks != "EXM").ToList();
                        if (emp_cons_logs != null && emp_cons_logs.Count > 0)
                        {
                            foreach (var c in emp_cons_logs)
                            {
                                if (c != null)
                                {
                                    if (c.final_remarks == "OFF")
                                    {
                                      //do nothing
                                    }
                                    else if (c.final_remarks == "AB")
                                    {
                                        c.status_in = "ab";
                                        c.status_out = "ab";
                                        c.time_in = null;
                                        c.time_out = null;
                                        c.final_remarks = "AB";
                                        c.terminal_in = "";
                                        c.terminal_out = "";

                                    }
                                    else
                                    {
                                        c.status_in = "on time";
                                        c.status_out = "on time";
                                        //c.time_in = null;
                                        //c.time_out = null;
                                        c.final_remarks = "PO";
                                        c.terminal_in = "";
                                        c.terminal_out = "";
                                    }

                                    db.SaveChanges();

                                    Program.WriteToLog("\tExmpt Cons - EmpCode=" + c.employee.employee_code);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void LateByHoursManagement()
        {
            Program.WriteToLog("* Late Management - Employees in the list..."); //Employee who are marked Late but worked 8 hours

            using (var db = new Context())
            {
                var emp_cons_logs = db.consolidated_attendance.Where(m => m.overtime >= (iWorkingHours * 60 * 60) && (m.final_remarks == "PLO" || m.final_remarks == "PLE" || m.final_remarks == "PLM")).ToList();
                if (emp_cons_logs != null && emp_cons_logs.Count > 0)
                {
                    foreach (var c in emp_cons_logs)
                    {
                        c.final_remarks = "PO";
                        db.SaveChanges();

                        Program.WriteToLog("\tLate Cons to Mark PO - EmpCode=" + c.employee.employee_code);
                    }
                }
            }

            ///////////// Mark Absent who are Late

            using (var db = new Context())
            {
                var emp_cons_logs = db.consolidated_attendance.Where(m => m.overtime < (iWorkingHours * 60 * 60) && (m.final_remarks == "PLO" || m.final_remarks == "PLE" || m.final_remarks == "PLM")).ToList();
                if (emp_cons_logs != null && emp_cons_logs.Count > 0)
                {
                    foreach (var c in emp_cons_logs)
                    {
                        c.final_remarks = "AB";

                        Program.WriteToLog("\tLate Cons to Mark AB - EmpCode=" + c.employee.employee_code);

                        //comment vice versa the following 2 lines
                        break;
                        ////db.SaveChanges();//Waiting for DOW approval to enable it
                    }
                }
            }
        }

        public static string GetWorkingHoursInDay()
        {
            string strWrokingHours = "8";

            using (Context db = new Context())
            {
                strWrokingHours = db.access_code_value.Where(a => a.AccessCode.ToUpper() == "WORKING_HOURS_IN_DAY").Select(m => m.AccessValue).FirstOrDefault();
            }

            return strWrokingHours;
        }


        public static void ShortLeaveWaveProcess()
        {
            DateTime dtLastMonthStart = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1, 0, 0, 0);
            DateTime dtLastMonthEnd = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month), 23, 59, 0);

            List<LeaveWaveInfo> leave_app = new List<LeaveWaveInfo>();
            List<LeaveWaveInfo> leave_odd_count = new List<LeaveWaveInfo>();

            /*
              SELECT [EmployeeId]
                 ,sum(dayscount) as DaysCount
                 ,(count(*)%4) as RemainderBy4
              FROM [TimeTune_Samsons].[dbo].[LeaveApplications]
              where IsActive=1 and LeaveCodeId=2 and FromDate>='2019-02-01 00:00:00.000' AND ToDate<='2019-02-28 23:59:00.000'
              group by [EmployeeId]
            */

            using (var db = new Context())
            {
                ////&& l.LeaveCodeId == 2
                leave_app = db.leave_application.Where(l => l.IsActive && (l.FromDate >= dtLastMonthStart && l.ToDate <= dtLastMonthEnd))
                .GroupBy(g => g.EmployeeId)
                .Select(s => new LeaveWaveInfo
                {
                    EmployeeId = s.FirstOrDefault().EmployeeId,
                    DaysCount = s.Sum(c => c.DaysCount),
                    LeaveRemainder = (s.Count() % 4)
                }).ToList();

                if (leave_app != null && leave_app.Count > 0)
                {
                    string strEmployeeCode = "";
                    foreach (var l in leave_app)
                    {
                        strEmployeeCode = db.employee.Where(e => e.EmployeeId == l.EmployeeId).FirstOrDefault().employee_code;
                        if (l.LeaveRemainder > 0)
                        {
                            leave_odd_count.Add(new LeaveWaveInfo()
                            {
                                EmployeeId = l.EmployeeId,
                                EmployeeCode = strEmployeeCode,
                                DaysCount = l.DaysCount,
                                LeaveRemainder = l.LeaveRemainder
                            });
                        }
                        strEmployeeCode = "";
                    }

                    if (leave_odd_count != null && leave_odd_count.Count > 0)
                    {
                        foreach (var l in leave_odd_count)
                        {
                            ////a.LeaveCodeId == 2 &&
                            var leave_waves = db.leave_application.Where(a => a.IsActive && a.EmployeeId == l.EmployeeId && (a.FromDate >= dtLastMonthStart && a.ToDate <= dtLastMonthEnd)).OrderByDescending(o => o.FromDate).Take(l.LeaveRemainder).ToList();
                            if (leave_waves != null && leave_waves.Count > 0)
                            {
                                foreach (var i in leave_waves)
                                {
                                    i.DaysCount = 0; //set -1 for testing
                                }
                            }

                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public static void LeavesDeductionOnLate(int maLateStart, int maLateEnd)
        {
            int iAvailableLeaveType = 0;
            decimal iTotalSickCount = 0, iTotalCasualCount = 0, iTotalAnnualCount = 0, iTotalOtherCount = 0;
            //DateTime dtLastMonthStart = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1, 0, 0, 0);
            //DateTime dtLastMonthEnd = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month), 23, 59, 0);
            //DateTime dtLastDateOfMonth = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month), 0, 0, 0);

            DateTime dtLastMonthStart = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, maLateStart, 0, 0, 0);
            DateTime dtThisMonthEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, maLateEnd, 23, 59, 0);
            DateTime dtLastDateOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, maLateEnd, 0, 0, 0); //new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month), 0, 0, 0);

            List<ConsLeaveInfo> consolidated_late = new List<ConsLeaveInfo>();
            List<ConsLeaveInfo> leave_emp_count = new List<ConsLeaveInfo>();

            /*
            --int output
            SELECT employee_EmployeeId
            ,count(*) as LateTotal
            ,(count(*)%4) as LateRemainderBy4
            ,(count(*)/4) as DeductLeaveBy4
            ,(count(*)%5) as LateRemainderBy5
            ,(count(*)/5) as DeductLeaveBy5
            FROM [TimeTune_Samsons].[dbo].ConsolidatedAttendances
            where final_remarks in('PLO','PLE','PLM') and [Date]>='2019-03-01 00:00:00.000' AND [Date]<='2019-03-31 23:59:00.000'
            group by employee_EmployeeId

            --decimal output
            SELECT employee_EmployeeId
            ,count(*) as LateTotal
            ,cast((count(*)%4) as decimal(5,2)) as LateRemainderBy4
            ,cast((CONVERT(decimal(5,2),count(*))/4) as decimal(5,2)) as DeductLeaveBy4
            ,cast((count(*)%5) as decimal(5,2)) as LateRemainderBy5
            ,cast((CONVERT(decimal(5,2),count(*))/5) as decimal(5,2)) as DeductLeaveBy5
            FROM [TimeTune_Samsons].[dbo].ConsolidatedAttendances
            where final_remarks in('PLO','PLE','PLM') and ([Date]>='2019-03-01 00:00:00.000' AND [Date]<='2019-03-31 23:59:00.000')
            group by employee_EmployeeId
			having cast((CONVERT(decimal(5,2),count(*))/5) as decimal(5,2))>=1
            
			delete from LeaveApplications where FromDate='2019-03-31 00:00:00.000' and ToDate='2019-03-31 00:00:00.000' and LeaveReasonId=1
			and EmployeeId in(18,21,39,46,56,63,71,78,130)

            */

            using (var db = new Context())
            {
                consolidated_late = db.consolidated_attendance.Where(l => (l.final_remarks.Equals("PLO") || l.final_remarks.Equals("PLE") || l.final_remarks.Equals("PLM")) && (l.date >= dtLastMonthStart && l.date <= dtThisMonthEnd))
                .GroupBy(g => g.employee.EmployeeId)
                .Select(s => new ConsLeaveInfo
                {
                    EmployeeId = s.FirstOrDefault().employee.EmployeeId,
                    LateCount = s.Count(),
                    LeaveDCountBy3 = (s.Count() / 3)
                }).ToList();

                if (consolidated_late != null && consolidated_late.Count > 0)
                {
                    int iEmployeeTypeID = 0; string strEmployeeCode = "";
                    foreach (var l in consolidated_late)
                    {
                        strEmployeeCode = db.employee.Where(e => e.EmployeeId == l.EmployeeId).FirstOrDefault().employee_code;
                        iEmployeeTypeID = db.employee.Where(e => e.EmployeeId == l.EmployeeId).FirstOrDefault().type_of_employment.TypeOfEmploymentId;
                        if (l.LeaveDCountBy3 >= 1 && iEmployeeTypeID > 0)//leave count must be greater than equal to 1
                        {
                            leave_emp_count.Add(new ConsLeaveInfo()
                            {
                                EmployeeId = l.EmployeeId,
                                EmployeeCode = strEmployeeCode,
                                EmployeeTypeID = iEmployeeTypeID,
                                LateCount = l.LateCount,
                                LeaveDCountBy3 = l.LeaveDCountBy3
                            });
                        }

                        strEmployeeCode = "";
                        iEmployeeTypeID = 0;
                    }

                    if (leave_emp_count != null && leave_emp_count.Count > 0)
                    {
                        foreach (var l in leave_emp_count)
                        {
                            iAvailableLeaveType = 0;

                            //////////////////////// Validate Number of Leaves with Remainings ///////////////////////////////////
                            int alcSick = 0, alcCasual = 0, alcAnnual = 0, alcOther = 0;
                            decimal avdSick = 0, avdCasual = 0, avdAnnual = 0, avdOther = 0;
                            //List<ViewModels.LeaveTypeInfo> toReturn = new List<ViewModels.LeaveTypeInfo>();

                            string[] leaves = new string[8] { "0", "0", "0", "0", "0", "0", "0", "0" };
                            leaves = GetUserLeavesByUserCode(l.EmployeeCode);
                            alcSick = int.Parse(leaves[0]); alcCasual = int.Parse(leaves[1]); alcAnnual = int.Parse(leaves[2]); alcOther = int.Parse(leaves[6]);
                            avdSick = decimal.Parse(leaves[3]); avdCasual = decimal.Parse(leaves[4]); avdAnnual = decimal.Parse(leaves[5]); avdOther = decimal.Parse(leaves[7]);

                            //for testing
                            ////avlSick = 10; avlCasual = 9; avlAnnual = 21; avlOther = 0;

                            //IR Commented
                            //if (Math.Ceiling(avdSick + l.LeaveDCountBy3) <= alcSick)
                            //{
                            //    iTotalSickCount = Math.Ceiling(avdSick + l.LeaveDCountBy3);
                            //}

                            if (Math.Ceiling(avdCasual + l.LeaveDCountBy3) <= alcCasual)
                            {
                                iTotalCasualCount = Math.Ceiling(avdCasual + l.LeaveDCountBy3);
                            }

                            //IR Commented
                            //if (Math.Ceiling(avdAnnual + l.LeaveDCountBy3) <= alcAnnual)
                            //{
                            //    iTotalAnnualCount = Math.Ceiling(avdAnnual + l.LeaveDCountBy3);
                            //}

                            //if (Math.Ceiling(avdOther + l.LeaveDCountBy3) <= alcOther)
                            //{
                            //    iTotalOtherCount = Math.Ceiling(avdOther + l.LeaveDCountBy3);
                            //}

                            //check employee-type to find out leave-type
                            if (l.EmployeeTypeID == 1)//permanent
                            {
                                if (iTotalCasualCount >= l.LeaveDCountBy3)//casual
                                {
                                    iAvailableLeaveType = 2;
                                }
                                //else if (iTotalAnnualCount >= l.LeaveDCountBy3)//annual
                                //{
                                //    iAvailableLeaveType = 3;
                                //}
                                //else if (iTotalSickCount >= l.LeaveDCountBy3)//sick
                                //{
                                //    iAvailableLeaveType = 1;
                                //}
                                else
                                {
                                    iAvailableLeaveType = 5;
                                }
                            }
                            else
                            {
                                //if (iTotalSickCount >= l.LeaveDCountBy3)//sick
                                //{
                                //    iAvailableLeaveType = 1;
                                //}
                                if (iTotalCasualCount >= l.LeaveDCountBy3)//casual
                                {
                                    iAvailableLeaveType = 2;
                                }
                                //else if (iTotalAnnualCount >= l.LeaveDCountBy3)//annual
                                //{
                                //    iAvailableLeaveType = 3;
                                //}
                                else
                                {
                                    iAvailableLeaveType = 5;
                                }
                            }

                            ///////////////////////////////////////////////////////////

                            var leave_exists = db.leave_application.Where(e => e.IsActive && e.EmployeeId == l.EmployeeId && (e.FromDate == dtLastDateOfMonth && e.ToDate == dtLastDateOfMonth) && e.LeaveReasonId == 1).FirstOrDefault();
                            if (leave_exists == null)
                            {
                                //NO record found with ReasonId=1
                                var leaveApp = new LeaveApplication();
                                leaveApp.EmployeeId = l.EmployeeId;
                                leaveApp.LeaveTypeId = iAvailableLeaveType;

                                ////leaveApp.LeaveCodeId = 1;//normal
                                leaveApp.LeaveStatusId = 2;//approved
                                leaveApp.DaysCount = l.LeaveDCountBy3;

                                leaveApp.FromDate = dtLastDateOfMonth;
                                leaveApp.ToDate = dtLastDateOfMonth;

                                leaveApp.ApproverId = 1;
                                leaveApp.IsActive = true;

                                leaveApp.LeaveValidityId = 1;
                                leaveApp.LeaveValidityRemarks = "Auto marked by System";

                                leaveApp.LeaveReasonId = 1;//late
                                leaveApp.ReasonDetail = "Auto marked by System";

                                leaveApp.CreateDateTime = DateTime.Now;
                                leaveApp.UpdateDateTime = DateTime.Now;

                                db.leave_application.Add(leaveApp);
                                db.SaveChanges();

                                //add another blank record in case days-count > 2
                                if (l.LeaveDCountBy3 > 2)
                                {
                                    leaveApp.DaysCount = 0; ////0.00M;

                                    db.leave_application.Add(leaveApp);
                                    db.SaveChanges();
                                }
                            }

                            iTotalSickCount = 0; iTotalCasualCount = 0; iTotalAnnualCount = 0; iTotalOtherCount = 0;
                        }
                    }
                }
            }
        }


        public static string[] GetUserLeavesByUserCode(string user_code)
        {
            string[] leaves = new string[8] { "0", "0", "0", "0", "0", "0", "0", "0" };
            using (Context db = new Context())
            {
                int access_group_id = 0, user_id = 0;
                decimal iAvailedSL = 0, iAvailedCL = 0, iAvailedAL = 0, iAvailedOL = 0;

                var data_user = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                if (data_user != null)
                {
                    //find access group id
                    access_group_id = data_user.access_group.AccessGroupId;

                    //find user id by using user code
                    user_id = data_user.EmployeeId;

                    //get leave allocated to this user
                    var dbLeavesData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                    if (dbLeavesData != null)
                    {
                        leaves[0] = dbLeavesData.SickLeaves.ToString();
                        leaves[1] = dbLeavesData.CasualLeaves.ToString();
                        leaves[2] = dbLeavesData.AnnualLeaves.ToString();
                        leaves[6] = dbLeavesData.OtherLeaves.ToString();

                        //get available and availed leaves count
                        //var dbSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                        //if (dbSessionData != null)
                        //{
                        //get approved leaves falling during the session of particular user
                        var AvailedLeaves = db.leave_application.Where(l => l.EmployeeId == user_id && l.IsActive && l.LeaveStatusId == 2 && (l.FromDate >= dbLeavesData.SessionStartDate && l.ToDate <= dbLeavesData.SessionEndDate)).ToList();
                        if (AvailedLeaves != null && AvailedLeaves.Count > 0)
                        {
                            foreach (var item in AvailedLeaves)
                            {
                                if (item.LeaveTypeId == 1) //for sick leaves
                                {
                                    iAvailedSL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 2)// for casual leaves
                                {
                                    iAvailedCL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 3)// for annual leaves
                                {
                                    iAvailedAL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 4)// for other leaves
                                {
                                    iAvailedOL += item.DaysCount;
                                }
                            }

                            //set counter to array
                            leaves[3] = iAvailedSL.ToString();
                            leaves[4] = iAvailedCL.ToString();
                            leaves[5] = iAvailedAL.ToString();
                            leaves[7] = iAvailedOL.ToString();
                        }
                        //}
                    }
                }
            }

            return leaves;
        }


        // This method reads all the consolidated logs, and updates them according
        // to the current calendars.

        public static void correctConsolidatedLogs(int month, int year)
        {
            // pick all the consolidated logs
            // foreach log
            //      get the employee
            //      get the shift for the employee on that date
            //      get the time in rem
            //      get the time out rem
            //      get the final remarks
            //      update the consaolidated log
            //      save changes to db

            using (var db = new Context())
            {

                DateTime startTime = DateTime.Now;

                // starting day
                DateTime startOfMonth = new DateTime(year, month, 1);

                // ending day
                DateTime endOfMonth = new DateTime(year, month + 1, 1);
                endOfMonth = endOfMonth.AddDays(-1);

                ConsolidatedAttendance[] logs = db.consolidated_attendance.Where(m => m.active &&
                    m.date.Value >= startOfMonth && m.date.Value <= endOfMonth).ToArray();

                Console.Write("Updating consolidated logs for " + year + ":\nStart Date: " + startOfMonth.ToString("dd-MM-yyyy") + "\nEnd Date: " + endOfMonth.ToString("dd-MM-yyyy"));
                foreach (ConsolidatedAttendance log in logs)
                {
                    if (log.manualAttendances.Count > 0)
                        continue;

                    Employee emp = log.employee;
                    if (emp == null)
                    {
                        Program.WriteToLog("Employee not found... " + log.ConsolidatedAttendanceId);
                        continue;
                    }


                    Shift shift = Utils.getShift(emp, log.date, db);
                    if (shift == null)
                    {
                        continue;
                    }


                    // create a temporary persistant attendance log,
                    // set it as it would have been when the entry was compiled,
                    // but using the shift acquired above.

                    PersistentAttendanceLog temp = new PersistentAttendanceLog();
                    DateTime startDateTime = log.date.Value.Add(shift.start_time.TimeOfDay);
                    temp.time_start = startDateTime.AddSeconds(shift.early_time);
                    temp.half_day = startDateTime.AddSeconds(shift.half_day);
                    temp.late_time = startDateTime.AddSeconds(shift.late_time);
                    temp.shift_end = startDateTime.AddSeconds(shift.shift_end);
                    temp.time_end = startDateTime.AddSeconds(shift.day_end);
                    temp.time_in = log.time_in;
                    temp.time_out = log.time_out;
                    temp.terminal_in = (log.terminal_in != null) ? log.terminal_in : "";
                    temp.terminal_out = (log.terminal_out != null) ? log.terminal_out : "";
                    log.status_in = Utils.getTimeInRemarks(temp);
                    log.status_out = Utils.getTimeOutRemarks(temp);
                    //Future Manual attendance Final remarks comes here 
                    string employeeCode = log.employee.employee_code;
                    DateTime futureDate = log.date.Value;
                    var futureManualAttendance = db.futureManualAttendance.Where(c => c.employee_code.Equals(employeeCode) && futureDate >= c.from_date && futureDate <= c.to_date).FirstOrDefault();
                    if (futureManualAttendance != null)
                    {
                        log.final_remarks = futureManualAttendance.remarks;
                    }
                    else
                    {
                        log.final_remarks = Utils.getFinalRemarks(log.status_in, log.status_out);
                    }

                }

                Program.WriteToLog(" Done!");
                Program.WriteToLog("Saving changes to db....");
                db.SaveChanges();
                Program.WriteToLog("done!");
                Program.WriteToLog("Started: " + startTime.ToString("dd-MM-yyyy HH:mm:ss"));
                Program.WriteToLog("Done: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            }

        }

        public static void pipeline()
        {
            string strMessage = "";
            DateTime dtStarted = DateTime.Now, dtFinished = DateTime.Now;

            strMessage = "::::: STARTED @ " + dtStarted.ToString("dd MMM yyyy hh:mm:ss tt") + " :::::";
            Program.WriteToLog(strMessage);

            strMessage = "* Adding HaTransit DEFAULT Entry...";
            Program.WriteToLog(strMessage);
            AddHaTransitDefaultEntry();

            strMessage = "* Checking for NEW Employees...";
            Program.WriteToLog(strMessage);

            //syncher Check For Freshies
            Syncher.checkForFreshies();

            Program.WriteToLog("-- Next --");

            //added by IR
            DeactivateEmployeesLeft();

            Program.WriteToLog("-- Next --");

            synchAndCompile();

            //added by IR
            ExemptedEmployeesMarking();

            //late by hours management
            LateByHoursManagement();

            //Leave Deduction on Late Calculation
            int maLateStart = 0, maLateEnd = 0;
            if (ConfigurationManager.AppSettings["ma-late-start"] != null && ConfigurationManager.AppSettings["ma-late-start"].ToString() != "")
            {
                maLateStart = int.Parse(ConfigurationManager.AppSettings["ma-late-start"].ToString());
            }
            else
            {
                maLateStart = 16;
            }

            if (ConfigurationManager.AppSettings["ma-late-end"] != null && ConfigurationManager.AppSettings["ma-late-end"].ToString() != "")
            {
                maLateEnd = int.Parse(ConfigurationManager.AppSettings["ma-late-end"].ToString());
            }
            else
            {
                maLateEnd = 15;
            }

            //Testing of Date
            DateTime dtLastMonthStart = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, maLateStart, 0, 0, 0);
            strMessage = ">> Leave Start Range: " + dtLastMonthStart.ToString("dd-MMM-yyyy");
            Program.WriteToLog(strMessage);

            DateTime dtThisMonthEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, maLateEnd, 23, 59, 0);
            strMessage = ">> Leave End Range: " + dtThisMonthEnd.ToString("dd-MMM-yyyy");
            Program.WriteToLog(strMessage);

            if (DateTime.Now.Day >= maLateStart && DateTime.Now.Day <= (maLateStart + 3))
            {
                //short leave waving by IR
                ////ShortLeaveWaveProcess();

                //leave deduct late
                LeavesDeductionOnLate(maLateStart, maLateEnd);
            }

            //FINAL step
            dtFinished = DateTime.Now;

            strMessage = "::::: ENDED @ " + dtFinished.ToString("dd MMM yyyy hh:mm:ss tt") + " :::::";
            Program.WriteToLog(strMessage);


            TimeSpan tSpan = (dtFinished - dtStarted);
            strMessage = "::::: TOTAL TIME - " + tSpan.ToString(@"hh\:mm\:ss") + " (hh:mm:ss) :::::";

            Program.WriteToLog(strMessage);

            //Console.ReadKey();
        }

        public static void reTryCatch()
        {
            for (int i = 7; i <= 8; i++)
            {
                try
                {
                    // (month,year)
                    correctConsolidatedLogs(i, 2016);

                }
                catch (System.OutOfMemoryException)
                {
                    Program.WriteToLog("\n\nRan out of memory... running explicit garbage collection before a re-attempt!!!");
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    i--;
                }

            }

        }

        static void decodeBase64AndWriteToFile(string toDecode)
        {
            try
            {
                byte[] data = Convert.FromBase64String(toDecode);
                string decodedString = Encoding.UTF8.GetString(data);

                File.WriteAllText("newFile.csv", decodedString);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in base64Encode" + exception.Message);
            }



        }

        public static void Main(string[] args)
        {

            //Console.SetOut(File.CreateText("output.txt"));

            //reTryCatch();

            //TestTimeCalclations();


            pipeline();


        }

        public static void WriteToLog(string msg)
        {
            bool bLogStatus = false, bDLogStatus = false;

            //console log
            Console.WriteLine(msg);

            //IR999 - 3/3
            bLogStatus = Utils.GetLogStatus("MA_LOG_STATUS"); //int.Parse(ConfigurationManager.AppSettings["ma-log-status"] ?? "0");
            if (bLogStatus)
            {
                //db detailed-log
                bDLogStatus = Utils.GetLogStatus("MA_LOG_STATUS_DETAILED"); //int.Parse(ConfigurationManager.AppSettings["ma-log-status-detailed"] ?? "0");

                //log to database
                using (var db = new Context())
                {
                    if (bDLogStatus)
                    {
                        //log all type of messages
                        db.log_message.Add(new LogMessage()
                        {
                            LogTitle = "MA Job",
                            LogDateTime = DateTime.Now,
                            LogMessageText = msg
                        });

                        db.SaveChanges();
                    }
                    else
                    {
                        if (msg.Contains("\t"))
                        {
                            //don't log '\t' log-messages
                        }
                        else
                        {
                            db.log_message.Add(new LogMessage()
                            {
                                LogTitle = "MA Job",
                                LogDateTime = DateTime.Now,
                                LogMessageText = msg
                            });

                            db.SaveChanges();
                        }
                    }
                }
            }

            ////LoggingProject.Logger.Write(msg, LoggingProject.LoggerType.InfoLogger, LoggingProject.LoggerFor.Service);
        }

        public static void TestTimeCalclations()
        {
            int iClinicalWorkingHours = 7, iAdminWorkingHours = 8;
            myLog log = new myLog();

            //Console.SetOut(File.CreateText("output.txt"));

            //reTryCatch();

            double _overTime = 0; int _overTime_Status = 0;
            if (log.time_in != null && log.time_out != null)
            {
                log.start_time = new DateTime(2020, 12, 23, 08, 30, 00);
                log.time_in = new DateTime(2020, 12, 23, 09, 01, 15);
                log.time_out = new DateTime(2020, 12, 23, 18, 02, 00);
                log.late_time = new DateTime(2020, 12, 23, 09, 01, 00);

                double late_time = 0.0;
                late_time = log.time_in.Value.TimeOfDay.TotalSeconds - log.late_time.Value.TimeOfDay.TotalSeconds;

                Console.WriteLine("111: Late Sec: " + late_time);

                Console.WriteLine("111: Time In:  " + log.time_in.Value.TimeOfDay.TotalSeconds);
                Console.WriteLine("111: Start Time: " + (log.late_time.Value.TimeOfDay.TotalSeconds - 60));
                Console.WriteLine("111: Out Time: " + log.time_out.Value.TimeOfDay.TotalSeconds);

                double seconds_spent_in_office = 0.0; double seconds_early = 0.0;
                //seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                seconds_early = log.start_time.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                Console.WriteLine("111: Compare by Late Time: " + seconds_early);

                if (seconds_early >= 0)
                {
                    seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds - seconds_early;
                    Console.WriteLine("111: Time Spent in office: " + seconds_spent_in_office);
                }
                else
                {
                    seconds_spent_in_office = log.time_out.Value.TimeOfDay.TotalSeconds - log.time_in.Value.TimeOfDay.TotalSeconds;
                    Console.WriteLine("111: Time Spent in office (after removing early time): " + seconds_spent_in_office);
                }

                ////////////////////////////////////////////////////////////////
                int site_id = 0;
                site_id = 2;

                if (site_id >= 0)
                {
                    if (site_id == 3)//Clinical=8hrs
                    {
                        if (seconds_spent_in_office >= (iClinicalWorkingHours * 60 * 60))//after 8 hours
                        {
                            //add one hour + over sec
                            _overTime = (1 * 60 * 60) + (seconds_spent_in_office - (iClinicalWorkingHours * 60 * 60));
                            _overTime_Status = 1;
                            Console.WriteLine("111: C-Overtime: " + _overTime);
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
                            Console.WriteLine("111: A-Overtime: " + _overTime);
                        }
                        else
                        {
                            _overTime = 0;
                            _overTime_Status = 0;
                        }
                    }

                }

                //////// Case 2 - Came Late /////////////////

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Case 222: Came Late");
                Console.WriteLine("");
                Console.WriteLine("");



                log.time_in = new DateTime(2020, 12, 23, 9, 01, 01);
                log.time_out = new DateTime(2020, 12, 23, 18, 00, 00);
                log.late_time = new DateTime(2020, 12, 23, 09, 01, 00);

                seconds_spent_in_office = 0.0;

                Console.WriteLine("222: Time In:  " + log.time_in.Value.TimeOfDay.TotalSeconds);
                Console.WriteLine("222: Late Time: " + (log.late_time.Value.TimeOfDay.TotalSeconds + 60.0));

                double iLateSec = 0.0;
                iLateSec = log.time_in.Value.TimeOfDay.TotalSeconds - log.late_time.Value.TimeOfDay.TotalSeconds;
                Console.WriteLine("222: Calc Late Time: " + iLateSec);

                //iLateSec = log.time_in.Value.TimeOfDay.TotalSeconds - (log.late_time.Value.TimeOfDay.TotalSeconds + 60.0);
                //Console.WriteLine("333: Calc Late Time + 1min: " + iLateSec);

                //set as late-time to reach office
                double seconds_late_to_office = iLateSec, _lateTime = 0;

                int SecIn30Min = 30 * 60;
                int SecIn01Hour = (1 * 60) * 60, SecIn1_5Hour = (1 * 60 + 30) * 60;//1.5 hours to sec
                int SecIn02Hour = (2 * 60) * 60, SecIn2_5Hour = (2 * 60 + 30) * 60;//2.5 hours to sec
                int SecIn03Hour = (3 * 60) * 60, SecIn3_5Hour = (3 * 60 + 30) * 60;//3.5 hours to sec
                int SecIn04Hour = (4 * 60) * 60, SecIn4_5Hour = (4 * 60 + 30) * 60;//4.5 hours to sec
                int SecIn05Hour = (5 * 60) * 60, SecIn5_5Hour = (5 * 60 + 30) * 60;//5.5 hours to sec
                int SecIn06Hour = (6 * 60) * 60, SecIn6_5Hour = (6 * 60 + 30) * 60;//6.5 hours to sec
                int SecIn07Hour = (7 * 60) * 60, SecIn7_5Hour = (7 * 60 + 30) * 60;//7.5 hours to sec
                int SecIn08Hour = (8 * 60) * 60, SecIn8_5Hour = (8 * 60 + 30) * 60;//8.5 hours to sec

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

                Console.WriteLine("444: Final Late Time: " + _lateTime);

                Console.ReadKey();

            }
        }//if grt 1

    }

    public class myLog
    {
        public DateTime? start_time { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public DateTime? late_time { get; set; }

        public myLog()
        {
            this.start_time = new DateTime(2020, 12, 23, 8, 30, 00);
            this.time_in = new DateTime(2020, 12, 23, 8, 55, 00);
            this.time_out = new DateTime(2020, 12, 23, 18, 00, 00);
            this.late_time = new DateTime(2020, 12, 23, 09, 01, 00);
        }
    }




}
