using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Globalization;
using System.Timers;

namespace AttendancePipeLine
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer1 = null; 
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = 60000*5;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.start_pipeLine);
            timer1.Enabled = true;
        }

        private void start_pipeLine(object sender, ElapsedEventArgs e)
        {
            pipeline();
        }


        public static bool synchAndCompile()
        {
            Logs.writeLogs("* Syncher started! ");
            Logs.writeLogs("* Processing HA transit entries... ");

            using (var db = new Context())
            {

                List<PersistentAttendanceLog> dirtyList = null;
                var haTransit = db.ha_transit.Where(m => m.active).OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToList();
                foreach (var value in haTransit)
                {


                    // 1)

                    dirtyList = Syncher.MarkDirty(db, value);

                    Logs.writeLogs(value.C_Date.Value.Date + ", " + value.C_Time + ", " + dirtyList.Count);

                    // 1.5)
                    // pick the employee for the current ha transit entry
                    // only if the time end in its persistent attendance log is not
                    // null.

                    var employee = db.employee.Where(m => m.employee_code.Equals(value.C_Unique) && m.persistent_attendance_log.time_end != null).FirstOrDefault();
                    if (employee == null)
                    {
                       // Logs.writeLogs("EMployee " + employee.employee_code + " Time End is " + employee.persistent_attendance_log.time_end);
                        continue;
                    }



                    // 2)
                    var log = db.persistent_attendance_log.Where(m => m.PersistentAttendanceLogId == employee.persistent_attendance_log.PersistentAttendanceLogId).FirstOrDefault();

                    var val = dirtyList.Where(m => m.PersistentAttendanceLogId == log.PersistentAttendanceLogId).FirstOrDefault();
                    // 3) check if the log is in dirty list
                    if (val != null)
                    {
                        continue;
                    }


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

                        }
                        else if (entryDateTime < log.time_in.Value)
                        {
                            log.time_in = entryDateTime;
                        }
                        else
                        {
                            log.time_out = entryDateTime;
                        }
                    }

                    // 5)
                    value.active = false;

                }

                Logs.writeLogs("Done!");
                Logs.writeLogs("* Syncher complete!");

                Logs.writeLogs("* Compiler Initiated... ");
                Logs.writeLogs("* Compiling old persistent log entries... ");


                bool toReturn = (dirtyList.Count == 0);

                // Compiler 
                if (dirtyList != null)
                {

                    foreach (var dirtylog in dirtyList)
                    {
                        Compiler.Consolidate(dirtylog, db);
                        Compiler.updateLog(dirtylog, db);
                    }
                }

                Logs.writeLogs("Done!");

                Logs.writeLogs("* Commiting changes to database... ");
                db.SaveChanges();
                Logs.writeLogs("Done!");

                return toReturn;
            }
        }

        public static void pipeline()
        {
            Logs.writeLogs("*****************************\nTime Tune pipeline initiated\n*****************************");


            Logs.writeLogs("* Checking for new employees... ");
            ////syncher Check For Freshies
            Syncher.checkForFreshies();
            Logs.writeLogs("Done!");
            int difference = 0;
            using (Context db = new Context())
            {

                var maxDate = db.ha_transit.Where(m => m.active).OrderByDescending(m => m.C_Date).FirstOrDefault();
                var minDate = db.ha_transit.Where(m => m.active).OrderBy(m => m.C_Date).FirstOrDefault();
                if (maxDate != null || minDate != null)
                {
                    Logs.writeLogs("* Processing attendance from " + minDate.C_Date.Value + " to " + maxDate.C_Date.Value);
                    difference = (int)(maxDate.C_Date.Value - minDate.C_Date.Value).TotalDays;


                    for (int i = 0; i < 6; i++)
                    {
                        synchAndCompile();
                    }


                }
                Logs.writeLogs("No Record to Process in HATransit ");

            }

            Logs.writeLogs("* Pipeline flushed... ");

            Logs.writeLogs("* press any key to continue... ");




        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
        }
    }

    public class Syncher
    {
        // Checks is the ha transit entry is within the bounds defined
        // by the time_start and time_end attributes in persistent attendance log
        // haLog: the ha transit log entry.
        // pasLog: the persistant attendance log entry
        public static bool isEntryValid(HaTransit haLog, PersistentAttendanceLog paLog)
        {

            DateTime startTime = paLog.time_start.Value;
            DateTime endTime = paLog.time_end.Value;

            DateTime entryDateTime = DateTime.ParseExact(

                haLog.C_Date.Value.ToString("dd/MM/yyyy") + " " + haLog.C_Time

                , "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            return (entryDateTime >= startTime && entryDateTime <= endTime);

        }

        public static void checkForFreshies()
        {
            using (var db = new Context())
            {
                var freshies = db.persistent_attendance_log.Where(m => m.time_end == null).ToList();
                if (freshies != null)
                {

                    foreach (var entity in freshies)
                    {
                        var employee = db.employee.Where(m => m.persistent_attendance_log.PersistentAttendanceLogId.Equals(entity.PersistentAttendanceLogId)).FirstOrDefault();
                        if (employee == null)
                        {
                            Logs.writeLogs("Persistance Log is not Created For Employee" + employee.employee_code);
                            continue;
                        }
                        var shift = Utils.markShift(employee, entity, db);
                        if (shift == null)
                        {
                            Logs.writeLogs("No Shift For Employee" + employee.employee_code);
                            continue;
                        }
                        DateTime? dateOfJoining = employee.date_of_joining;
                        if (dateOfJoining == null)
                        {
                            Logs.writeLogs("Date Of Joining for Employee" + employee.employee_code + "is null");
                            continue;
                        }
                        DateTime startDateTime = dateOfJoining.Value.Add(
                            shift.start_time - DateTime.ParseExact("01/01/1990 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                            //DateTime.ParseExact(shift.start_time.ToString("HH:mm:ss"),"HH:mm:ss",CultureInfo.InvariantCulture).Ticks       )
                            );
                        DateTime earlyIn = startDateTime.AddSeconds(shift.early_time);

                        entity.time_start = earlyIn;
                        entity.half_day = startDateTime.AddSeconds(shift.half_day);
                        entity.late_time = startDateTime.AddSeconds(shift.late_time);
                        entity.shift_end = startDateTime.AddSeconds(shift.shift_end);
                        entity.time_end = startDateTime.AddSeconds(shift.day_end);
                    }
                }
                else
                {
                    Logs.writeLogs("No Employee Registered");
                }


                db.SaveChanges();
            }
        }

        public static List<PersistentAttendanceLog> MarkDirty(Context db, HaTransit value)
        {
            List<PersistentAttendanceLog> dirtyLogsList = new List<PersistentAttendanceLog>();

            // get all employees
            var employee = db.employee.Where(m => m.active && m.persistent_attendance_log.time_end != null).ToList();


            foreach (var attendance in employee)
            {
                var attendanceLog = db.persistent_attendance_log.Where(m => m.PersistentAttendanceLogId == attendance.persistent_attendance_log.PersistentAttendanceLogId).FirstOrDefault();

                // the date from persistent log.
                DateTime pLogDT = attendanceLog.time_end.Value;

                // the date from ha transit log.

                DateTime haLogDT = DateTime.ParseExact(value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                if (haLogDT > pLogDT)
                {
                    attendanceLog.dirtyBit = true;
                    dirtyLogsList.Add(attendanceLog);
                }
            }
            Logs.writeLogs("Dirty List Count" + dirtyLogsList.Count);
            return dirtyLogsList;

        }


        //public static void getPersistentLog(Context db, HaTransit value)
        //{
        //    DateTime time = DateTime.ParseExact(value.C_Date.Value.ToString("dd/MM/yyyy")+" "+ value.C_Time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);



        //    var employee = db.employee.Where(m => m.employee_code.Equals(value.C_Unique)).FirstOrDefault();

        //    var employeeEntry = employee.persistent_attendance_log;

        //    DateTime? date = value.C_Date;

        //    if (employeeEntry.time_in == null)
        //    {
        //        string remarks = Utils.timeInRemarks(employeeEntry, time);
        //        if(remarks==null)
        //        {

        //        }
        //        else
        //        {
        //            employeeEntry.time_in = time;
        //        }

        //    }
        //    else
        //    {
        //        string remarks = Utils.timeOutRemarks(employeeEntry, time);
        //        if(remarks==null)
        //        {

        //        }
        //        else
        //        {
        //            employeeEntry.time_out = time;
        //        }

        //    }

        //    // db.SaveChanges();
        //}


    }

    public class Compiler
    {
        public static string getTimeInRemarks(PersistentAttendanceLog log, DateTime? date)
        {
            // check time in status
            if (log.time_in == null)
            {
                return "MissPunch";
            }
            else
            {
                long span = log.late_time.Value.Ticks - log.time_start.Value.Ticks;
                long timeIn = log.time_in.Value.Ticks - log.time_start.Value.Ticks;
                if (timeIn <= span)
                {
                    return "OnTime";
                }
                else
                {
                    return "Late";
                }
            }



        }
        public static string getTimeOutRemarks(PersistentAttendanceLog log, DateTime? date)
        {
            // check time in status
            if (log.time_out == null)
            {
                return "MissPunch";
            }
            else
            {
                long span = log.time_end.Value.Ticks - log.shift_end.Value.Ticks;
                long timeOut = log.time_out.Value.Ticks - log.shift_end.Value.Ticks;
                if (timeOut < 0)
                {
                    return "EarlyGone";
                }
                if (timeOut <= span)
                {
                    return "OnTime";
                }
                else
                {
                    return null;
                }
            }



        }

        public static void Consolidate(PersistentAttendanceLog log, Context db)
        {
            ConsolidatedAttendance attendance = new ConsolidatedAttendance();
            DateTime? date = log.time_start.Value.Date;
            var employee = db.employee.Where(m => m.persistent_attendance_log.PersistentAttendanceLogId == log.PersistentAttendanceLogId).FirstOrDefault();
            var toUpdate = db.consolidated_attendance.Where(m => m.date == date && m.employee.employee_code.Equals(employee.employee_code)).FirstOrDefault();
            if (toUpdate != null)
            {
                toUpdate.time_in = log.time_in;
                toUpdate.time_out = log.time_out;
                toUpdate.employee = employee;
                toUpdate.status_in = Compiler.getTimeInRemarks(log, log.start_time);
                toUpdate.status_out = Compiler.getTimeOutRemarks(log, log.start_time);
                if (toUpdate.status_in.Equals("MissPunch") && toUpdate.status_out.Equals("MissPunch"))
                {
                    toUpdate.final_remarks = "Absent";
                }
                else
                {
                    toUpdate.final_remarks = toUpdate.status_in + " " + toUpdate.status_out;
                }
                toUpdate.date = log.time_start.Value.Date;
                toUpdate.active = true;
            }
            else
            {
                attendance.active = true;
                attendance.time_in = log.time_in;
                attendance.time_out = log.time_out;
                attendance.employee = employee;

                attendance.status_in = Compiler.getTimeInRemarks(log, log.start_time);

                attendance.status_out = Compiler.getTimeOutRemarks(log, log.start_time);
                if (attendance.status_in.Equals("MissPunch") && attendance.status_out.Equals("MissPunch"))
                {
                    attendance.final_remarks = "Absent";
                }
                else
                {
                    attendance.final_remarks = attendance.status_in + " " + attendance.status_out;
                }
                attendance.date = log.time_start.Value.Date;
                db.consolidated_attendance.Add(attendance);
            }
        }

        public static void updateLog(PersistentAttendanceLog log, Context db)
        {
            // add one day to the current start date.
            DateTime? date = log.time_start.Value.AddDays(1);

            // get the next shift for the employee.
            var emp = db.employee.Where(m => m.persistent_attendance_log.PersistentAttendanceLogId == log.PersistentAttendanceLogId).FirstOrDefault();
            var shift = Utils.getShift(emp, date, db);

            if (shift == null)
            {
                Logs.writeLogs("No Shift For Employee" + emp.employee_code);
            }
            else
            {
                DateTime startDateTime =
                    DateTime.ParseExact(date.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    .Add(shift.start_time.TimeOfDay);
                // day from 'date'
                // time from 'time'
                ;

                //date.Value.Add(new System.TimeSpan(shift.start_time.Ticks));
                DateTime earlyIn = startDateTime.AddSeconds(shift.early_time);

                log.time_start = earlyIn;
                log.half_day = startDateTime.AddSeconds(shift.half_day);
                log.late_time = startDateTime.AddSeconds(shift.late_time);
                log.shift_end = startDateTime.AddSeconds(shift.shift_end);
                log.time_end = startDateTime.AddSeconds(shift.day_end);
                log.time_in = null;
                log.time_out = null;
                log.dirtyBit = false;
            }
        }
    }

    public class Utils
    {
        public static Shift markShift(Employee employee, PersistentAttendanceLog entity, Context db)
        {
            if (employee.Group == null)
            {


                int year = employee.date_of_joining.Value.Year;
                var calendar = db.general_calender.Where(m => m.year.Equals(year)).FirstOrDefault();
                if (calendar == null)
                {
                    Logs.writeLogs("\nCalendar for the year " + year + " has not been generated.");
                    Logs.writeLogs("press any key to exit... ");
                    
                    return null;
                }
                else
                {
                    var genralCalenderOveride = calendar.calendarOverrides.Where(m => m.date == employee.date_of_joining.Value.Date).FirstOrDefault();
                    if (genralCalenderOveride == null)
                    {
                        DateTime? date1 = employee.date_of_joining;
                        string day = date1.Value.DayOfWeek.ToString();
                        switch (day)
                        {
                            case "Monday":
                                return (calendar.Shift1 != null) ? calendar.Shift1 : calendar.generalShift;
                            case "Tuesday":
                                return (calendar.Shift2 != null) ? calendar.Shift2 : calendar.generalShift;
                            case "Wednesday":
                                return (calendar.Shift3 != null) ? calendar.Shift3 : calendar.generalShift;
                            case "Thursday":
                                return (calendar.Shift4 != null) ? calendar.Shift4 : calendar.generalShift;
                            case "Friday":
                                return (calendar.Shift5 != null) ? calendar.Shift5 : calendar.generalShift;
                            case "Saturday":
                                return (calendar.Shift6 != null) ? calendar.Shift6 : calendar.generalShift;
                            case "Sunday":
                                return (calendar.Shift != null) ? calendar.Shift : calendar.generalShift;
                        }
                    }
                    else
                    {
                        return genralCalenderOveride.Shift;
                    }
                }
            }

            else
            {
                DateTime? date = employee.date_of_joining.Value.Date;
                var manualShiftAssingned = db.manual_group_shift_assigned.Where(m => m.date == date &&
                    m.Employee.employee_code.Equals(employee.employee_code) &&
                    m.Group.GroupId == employee.Group.GroupId).FirstOrDefault();
                if (manualShiftAssingned != null)
                {
                    return manualShiftAssingned.Shift;
                }
                else
                {
                    var groupOverride = db.group_calendar_overrides.Where(m => m.active && m.date == date
                        && m.GroupCalendar.Group.GroupId == employee.Group.GroupId).FirstOrDefault();
                    if (groupOverride != null)
                    {
                        return groupOverride.Shift;
                    }
                    else
                    {
                        string day = employee.date_of_joining.Value.DayOfWeek.ToString();
                        var groupGeneralcalendar = db.group_calendar.Where(m => m.active && m.year.Equals(employee.date_of_joining.Value.Year) && m.Group.GroupId == employee.Group.GroupId).FirstOrDefault();
                        if (groupGeneralcalendar == null)
                        {
                            Logs.writeLogs(" Group General Calendar is Empty \n");
                            return null;
                        }
                        switch (day)
                        {
                            case "Monday":
                                return (groupGeneralcalendar.monday != null) ? groupGeneralcalendar.monday : groupGeneralcalendar.generalShift;
                            case "Tuesday":
                                return (groupGeneralcalendar.tuesday != null) ? groupGeneralcalendar.tuesday : groupGeneralcalendar.generalShift;
                            case "Wednesday":
                                return (groupGeneralcalendar.wednesday != null) ? groupGeneralcalendar.wednesday : groupGeneralcalendar.generalShift;
                            case "Thursday":
                                return (groupGeneralcalendar.thursday != null) ? groupGeneralcalendar.thursday : groupGeneralcalendar.generalShift;
                            case "Friday":
                                return (groupGeneralcalendar.friday != null) ? groupGeneralcalendar.friday : groupGeneralcalendar.generalShift;
                            case "Saturday":
                                return (groupGeneralcalendar.saturday != null) ? groupGeneralcalendar.saturday : groupGeneralcalendar.generalShift;
                            case "Sunday":
                                return (groupGeneralcalendar.sunday != null) ? groupGeneralcalendar.sunday : groupGeneralcalendar.generalShift;
                        }
                    }

                }
            }
            return null;
        }

        public static Shift getShift(Employee employee, DateTime? attendanceDate, Context db)
        {
            {
                if (employee.Group == null)
                {



                    int year = attendanceDate.Value.Year;
                    var calendar = db.general_calender.Where(m => m.year.Equals(year)).FirstOrDefault();

                    if (calendar == null)
                    {
                        Logs.writeLogs("Error at compiler getShift(): General calendar for year " + year + " does not exist.");
                        return null;
                    }

                    else
                    {
                        var genralCalenderOveride = calendar.calendarOverrides.Where(m => m.date == attendanceDate.Value.Date).FirstOrDefault();
                        if (genralCalenderOveride == null)
                        {
                            string day = attendanceDate.Value.DayOfWeek.ToString();
                            switch (day)
                            {
                                case "Monday":
                                    return (calendar.Shift1 != null) ? calendar.Shift1 : calendar.generalShift;
                                case "Tuesday":
                                    return (calendar.Shift2 != null) ? calendar.Shift2 : calendar.generalShift;
                                case "Wednesday":
                                    return (calendar.Shift3 != null) ? calendar.Shift3 : calendar.generalShift;
                                case "Thursday":
                                    return (calendar.Shift4 != null) ? calendar.Shift4 : calendar.generalShift;
                                case "Friday":
                                    return (calendar.Shift5 != null) ? calendar.Shift5 : calendar.generalShift;
                                case "Saturday":
                                    return (calendar.Shift6 != null) ? calendar.Shift6 : calendar.generalShift;
                                case "Sunday":
                                    return (calendar.Shift != null) ? calendar.Shift : calendar.generalShift;
                            }

                        }
                        else
                        {
                            return genralCalenderOveride.Shift;
                        }

                    }


                }
                else
                {

                    DateTime? date = attendanceDate.Value.Date;
                    var manualShiftAssingned = db.manual_group_shift_assigned.Where(m => m.date == date &&
                        m.Employee.employee_code.Equals(employee.employee_code) &&
                        m.Group.GroupId == employee.Group.GroupId).FirstOrDefault();
                    if (manualShiftAssingned != null)
                    {
                        return manualShiftAssingned.Shift;
                    }
                    else
                    {
                        var groupOverride = db.group_calendar_overrides.Where(m => m.active && m.date == date
                            && m.GroupCalendar.Group.GroupId == employee.Group.GroupId).FirstOrDefault();
                        if (groupOverride != null)
                        {
                            return groupOverride.Shift;
                        }
                        else
                        {
                            string day = attendanceDate.Value.DayOfWeek.ToString();
                            var groupGeneralcalendar = db.group_calendar.Where(m => m.active && m.year.Equals(attendanceDate.Value.Year) && m.Group.GroupId == employee.Group.GroupId).FirstOrDefault();
                            if (groupGeneralcalendar == null)
                            {
                                Logs.writeLogs("Group Calendar in null please assign shift to general Calendar");
                            }
                            switch (day)
                            {
                                case "Monday":
                                    return (groupGeneralcalendar.monday != null) ? groupGeneralcalendar.monday : groupGeneralcalendar.generalShift;
                                case "Tuesday":
                                    return (groupGeneralcalendar.tuesday != null) ? groupGeneralcalendar.tuesday : groupGeneralcalendar.generalShift;
                                case "Wednesday":
                                    return (groupGeneralcalendar.wednesday != null) ? groupGeneralcalendar.wednesday : groupGeneralcalendar.generalShift;
                                case "Thursday":
                                    return (groupGeneralcalendar.thursday != null) ? groupGeneralcalendar.thursday : groupGeneralcalendar.generalShift;
                                case "Friday":
                                    return (groupGeneralcalendar.friday != null) ? groupGeneralcalendar.friday : groupGeneralcalendar.generalShift;
                                case "Saturday":
                                    return (groupGeneralcalendar.saturday != null) ? groupGeneralcalendar.saturday : groupGeneralcalendar.generalShift;
                                case "Sunday":
                                    return (groupGeneralcalendar.sunday != null) ? groupGeneralcalendar.sunday : groupGeneralcalendar.generalShift;
                            }
                        }

                    }
                }
                return null;
            }
        }

        public static string timeInRemarks(DLL.Models.PersistentAttendanceLog log, DateTime? date)
        {
            DateTime? start = log.time_start;
            DateTime? end = log.time_end;
            int current = (int)(date - start).Value.TotalSeconds;
            int limit = (int)(end - start).Value.TotalSeconds;
            if (current >= 0 && current <= limit)
            {
                return "On time";
            }

            if (current >= 0 && current > limit)
            {
                return "Late";
            }

            return null;

        }

        public static string timeOutRemarks(DLL.Models.PersistentAttendanceLog log, DateTime? date)
        {

            DateTime? start = log.time_start;
            DateTime? end = log.time_end;

            int current = (int)(date - start).Value.TotalSeconds;
            int limit = (int)(end - start).Value.TotalSeconds;


            if (current >= 0 && current <= limit)
            {
                return "On time";
            }

            if (current >= 0 && current > limit)
            {
                return null;
            }

            return "Early Gone";

        }
    }
}
