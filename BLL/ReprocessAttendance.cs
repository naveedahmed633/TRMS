using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Globalization;

namespace BLL
{
    public class MarkPreviousAttendance
    {
        public static void createOrUpdateAttendance(string from,string to,int empCode)
        {
            
            DateTime fromDate = DateTime.Parse(from);

            DateTime toDate= DateTime.Parse(to);

             using (var db = new Context())
            {
                 var haTransit=new List<HaTransit>();
                if (empCode != null)
                {
                    var getEmployee = db.employee.Where(m =>
                           m.active &&
                           m.EmployeeId == empCode
                           ).FirstOrDefault();
                    // get all the entries in ha transit which are active
                    // - they have not been processed.
                    haTransit = db.ha_transit.Where(m => m.C_Date.Value >= fromDate && m.C_Date <= toDate && m.C_Unique.Equals(getEmployee.employee_code))
                        .OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToList();
                }
                else
                {
                     haTransit = db.ha_transit.Where(m => m.C_Date.Value >= fromDate && m.C_Date <= toDate)
                      .OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToList();
                }

                foreach (HaTransit value in haTransit)
                {




                    // 1.5)
                    // pick the employee for the current ha transit entry

                    var employee = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(empCode)
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

                }

        }
        public static void overrideConsolidate(DateTime date, Employee employee, Context db)
        {
            DateTime? newDate = date.Date;
            // find if an entry for this date already exists in the consolidated logs.
            var toUpdate = db.consolidated_attendance.Where(m => m.date.Value.Equals(newDate.Value) &&
                m.employee.employee_code.Equals(employee.employee_code)).FirstOrDefault();


            //The object of persistent log is created to update the consolidated Attendance log
            //this is not a db persistent log this is just to use persistance log attributes
            PersistentAttendanceLog persistentLog = new PersistentAttendanceLog();
            var shift = Utils.getShift(employee, date, db);
            // if there's such an entry.
            if (toUpdate != null)
            {

                if (shift == null)
                {
                    Console.WriteLine("Compiler: No Shift For Employee" + employee.employee_code + "on date " + date.Date);
                }
                else
                {
                    // get the start time for the next date.

                    persistentLog.start_time = date.Date.Add(shift.start_time.TimeOfDay);
                    persistentLog.time_start = persistentLog.start_time.Value.AddSeconds(shift.early_time);
                    persistentLog.time_end = persistentLog.start_time.Value.AddSeconds(shift.day_end);
                    persistentLog.half_day = persistentLog.start_time.Value.AddSeconds(shift.half_day);
                    persistentLog.late_time = persistentLog.start_time.Value.AddSeconds(shift.late_time);
                    persistentLog.shift_end = persistentLog.start_time.Value.AddSeconds(shift.shift_end);
                    persistentLog.time_in = toUpdate.time_in;
                    if (persistentLog.time_start < date && date <= persistentLog.time_end)
                    {
                        if (toUpdate.time_in == null || toUpdate.time_in > date)
                        {
                            toUpdate.time_in = date;
                            persistentLog.time_in = date;
                        }
                        else if (toUpdate.time_in < date)
                        {
                            toUpdate.time_out = date;
                            persistentLog.time_out = date;
                        }

                        toUpdate.status_in = Utils.getTimeInRemarks(persistentLog);
                        toUpdate.status_out = Utils.getTimeOutRemarks(persistentLog);
                        Utils.setFinalRemarks(toUpdate);
                    }
                }
            }
            else
            {
                toUpdate = new ConsolidatedAttendance();
                if (shift == null)
                {
                    Console.WriteLine("Compiler: No Shift For Employee" + employee.employee_code + "on date " + date.Date);
                }
                else
                {
                    // get the start time for the next date.

                    persistentLog.start_time = date.Date.Add(shift.start_time.TimeOfDay);
                    persistentLog.time_start = persistentLog.start_time.Value.AddSeconds(shift.early_time);
                    persistentLog.time_end = persistentLog.start_time.Value.AddSeconds(shift.day_end);
                    persistentLog.half_day = persistentLog.start_time.Value.AddSeconds(shift.half_day);
                    persistentLog.late_time = persistentLog.start_time.Value.AddSeconds(shift.late_time);
                    persistentLog.shift_end = persistentLog.start_time.Value.AddSeconds(shift.shift_end);

                    toUpdate.date = date.Date;
                    toUpdate.employee = employee;
                    if (persistentLog.time_start < date && date <= persistentLog.time_end)
                    {
                        if (toUpdate.time_in == null)
                        {
                            toUpdate.time_in = date;
                            persistentLog.time_in = date;
                        }
                        else
                        {
                            toUpdate.time_out = date;
                            persistentLog.time_out = date;
                        }

                        toUpdate.status_in = Utils.getTimeInRemarks(persistentLog);
                        toUpdate.status_out = Utils.getTimeOutRemarks(persistentLog);
                        Utils.setFinalRemarks(toUpdate);
                        toUpdate.active = true;
                        db.consolidated_attendance.Add(toUpdate);
                        
                    }
                }
            }
        }
    }
    public class Utils
    {

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
            // if the employee group is null, it means that the employee lies in the general group.
            if (employee.Group == null || employee.Group.follows_general_calendar)
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

                    var generalCalendarOverride = calendar.calendarOverrides.Where(m => m.date.Value.Equals(attendanceDate.Value.Date)).FirstOrDefault();

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
                    Console.WriteLine("Cannot find shift for employee " + employee.employee_code + " on date." + attendanceDate);

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
                            Console.WriteLine("Calendar for group " + employee.Group.GroupId + ", for year " + attendanceDate.Value.Year + " does not exist.");
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
                    Console.WriteLine("Cannot find shift for employee " + employee.employee_code + ", of group " + employee.Group.GroupId + ", on date." + attendanceDate);

                }

                return toReturn;
            }
        }

    }
}
