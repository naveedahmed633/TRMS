using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Globalization;
using System.Configuration;
using System.IO;

namespace MarkContractualStaffAttendance
{

    public class Syncher
    {
        public static bool isEntryValid(HaTransit haLog, CS_AttendanceLog paLog, ContractualStaff employee, Context db)
        {

            DateTime startDate = paLog.date.Value.Date;

            DateTime entryDateTime = DateTime.ParseExact(

                haLog.C_Date.Value.ToString("dd/MM/yyyy") + " " + haLog.C_Time

                , "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
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

            return (startDate.Date.Equals(entryDateTime.Date));

        }

        public static void checkForFreshies()
        {
            if (!Utils.effectiveDateTime.HasValue)
            {
                Console.WriteLine("Invlaid attendance effective date.");
                Environment.Exit(0);
            }
            using (var db = new Context())
            {
                // get all the persistant logs where
                // time_start and time_end are null.
                var freshies = db.cs_persistent_log.Where(m =>
                    m.active &&
                    m.date.Equals(null)
                    ).ToList();

                if (freshies == null)
                {
                    Console.WriteLine("No new employees.");
                    return;
                }



                foreach (var entity in freshies)
                {
                    // get the employee
                    var employee = db.contractual_staff.Where(m =>
                        m.active &&
                        m.persistent_log.CS_AttendanceLogId.Equals(entity.CS_AttendanceLogId)
                        ).FirstOrDefault();
                    /*
                    if (employee.employee_code.Equals("578821"))
                    {
                        Console.WriteLine("Stopping..");
                    }*/
                    if (employee == null)
                    {
                        // Console.WriteLine("Persistant attendance log for Employee " + employee.employee_code+" not found.");
                        continue;
                    }

                    DateTime? dateOfJoining = employee.date_of_joining;

                    if (!dateOfJoining.HasValue)
                    {
                        Console.WriteLine("employee " + employee.employee_code + " does not have a date of joining.");
                        continue;
                    }

                    // if the date of joining is earlier than the date after which
                    // attendance is considered. chose the latter.
                    DateTime firstDay =
                        ((dateOfJoining.Value.Date > Utils.effectiveDateTime.Value.Date)) ? dateOfJoining.Value.Date : Utils.effectiveDateTime.Value;



                    // get the shift for the freshie.
                    // on the date of joining.

                    // DateTime startDateTime = firstDay.Add(shift.start_time.TimeOfDay);
                    entity.date = firstDay;
                }



                db.SaveChanges();
            }
        }
        // return all the entries earlier than the ha transit entry 'value'.
        public static CS_AttendanceLog[] getDirtyList(Context db, HaTransit value)
        {


            DateTime haLogDT = DateTime.ParseExact(

                value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time

                , "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var dirtyPersistentLogs = db.contractual_staff.Where(c => c.date_of_leaving > haLogDT && c.persistent_log.date < haLogDT.Date
                && c.active).Select(c => c.persistent_log).ToArray();
            /* var dirtyPersistentLogs =
                 db.cs_persistent_log.Where(m =>
                     m.active &&
                     m.date<haLogDT.Date 
             ).ToArray();
             * */
            //Console.WriteLine(": "+value.HaTransitId + " "+value.C_Date.Value.ToString("yyyy-MM-dd") +",Dirty List Count " + dirtyPersistentLogs.Length);
            return dirtyPersistentLogs.ToArray();

        }
    }

    public class Compiler
    {

        // clean the persistant attendance logs, and update them upto the date time
        // value.C_Date and value.C_Time

        public static void cleanPersistantLogs(CS_AttendanceLog[] logs, HaTransit value, Context db)
        {
            DateTime limit =
                DateTime.ParseExact(value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);


            CS_AttendanceLog[] dirtyLogs;
            do
            {
                // get the logs that need update.
                DateTime date = limit.Date;
                dirtyLogs = logs.Where(m => m.date < date).ToArray();

                // consolidate them
                consolidatePersistantLogs(dirtyLogs, db);

                // update them and break if none of them were updated.
            } while (updatePersistantLogs(dirtyLogs, db) != 0);


            db.SaveChanges();

        }

        // consolidate the dirty persistant attendance logs.
        //terminal addded in consolidated attendance logs

        public static void consolidatePersistantLogs(CS_AttendanceLog[] dirtyLogs, Context db)
        {
            List<CS_PersistentLog> consolidatedLogs = new List<CS_PersistentLog>();
            foreach (CS_AttendanceLog log in dirtyLogs)
            {
                // if this is a log which was previously consolidated
                // but has not moved on because its shift does not exist
                // for the next day. do nothing!
                if (log.dirtyBit)
                    continue;

                string remarks = Utils.getRemarks(log);

                consolidatedLogs.Add(new CS_PersistentLog()
                {
                    active = true,
                    employee = log.ContractualStaff,
                    time_in = log.time_in,
                    time_out = log.time_out,
                    date = log.date,
                    terminal_in = log.terminal_in,
                    terminal_out = log.terminal_out,
                    remarks = remarks
                });

                log.dirtyBit = true;


            }

            db.cs_consolidated_log.AddRange(consolidatedLogs.ToArray());
        }


        //Terminal added in update
        public static int updatePersistantLogs(CS_AttendanceLog[] dirtyLogs, Context db)
        {
            int logsUpdated = 0;
            foreach (CS_AttendanceLog log in dirtyLogs)
            {
                ContractualStaff emp = log.ContractualStaff;
                //PersistentAttendanceLog paLog = emp.persistent_attendance_log;

                // add one day to the current start date.
                // this will give us the next day's shift.
                DateTime? date = log.date.Value.Date.AddDays(1);


                log.date = date;
                log.time_in = null;
                log.time_out = null;
                log.dirtyBit = false;
                log.terminal_in = " ";
                log.terminal_out = " ";
                logsUpdated++;
            }

            return logsUpdated;
        }

        //Terminal Added in Consolidated all
        public static CS_PersistentLog[] ConsolidateAll(CS_AttendanceLog[] dirtyLogs, Context db)
        {
            List<CS_PersistentLog> toReturn = new List<CS_PersistentLog>();

            foreach (CS_AttendanceLog log in dirtyLogs)
            {
                // if this is a log which was previously consolidated
                // but has not moved on because its shift does not exist
                // for the next day. do nothing!
                if (log.dirtyBit)
                    continue;

                string remarks = Utils.getRemarks(log);

                toReturn.Add(new CS_PersistentLog()
                {
                    active = true,
                    employee = db.contractual_staff.Where(m => m.active && m.persistent_log.CS_AttendanceLogId.Equals(log.CS_AttendanceLogId)).FirstOrDefault(),
                    time_in = log.time_in,
                    time_out = log.time_out,
                    date = log.date.Value.Date,
                    terminal_in = log.terminal_in,
                    terminal_out = log.terminal_out,
                    remarks = remarks
                });
            }

            CS_PersistentLog[] arr = toReturn.ToArray();
            db.cs_consolidated_log.AddRange(arr);
            //db.SaveChanges();
            return arr;
        }

        // returns the number of persistant logs updated.
        public static int UpdateAll(CS_PersistentLog[] toUpdate, Context db)
        {

            int logsUpdated = 0;
            foreach (CS_PersistentLog caLog in toUpdate)
            {
                ContractualStaff emp = caLog.employee;

                CS_AttendanceLog paLog = emp.persistent_log;

                // add one day to the current start date.
                // this will give us the next day's shift.
                DateTime? date = paLog.date.Value.Date.AddDays(1);





                paLog.date = date;
                paLog.time_in = null;
                paLog.time_out = null;
                paLog.dirtyBit = false;
                paLog.terminal_in = " ";
                paLog.terminal_out = " ";
                logsUpdated++;



            }

            db.SaveChanges();
            return logsUpdated;
        }

    }

    public class Utils
    {

        //DateTime entryDateTime = DateTime.ParseExact(
        //                    value.C_Date.Value.ToString("dd/MM/yyyy") + " " + value.C_Time,
        //                    "dd/MM/yyyy HH:mm:ss",
        //                    CultureInfo.InvariantCulture);

        public static DateTime? effectiveDateTime =
            DateTime.ParseExact
            (ConfigurationManager.AppSettings["attendance-effective-date"], "dd-MM-yyyy", CultureInfo.InvariantCulture);

        public static string offDay1 = (ConfigurationManager.AppSettings["attendance-off-Day"] == null) ? " " : ConfigurationManager.AppSettings["attendance-off-Day"];
        public static string offDay2 = (ConfigurationManager.AppSettings["attendance-off-Day2"] == null) ? " " : ConfigurationManager.AppSettings["attendance-off-Day2"];
        public static string getTerminalName(HaTransit value, Context db)
        {
            string terminalName = " ";
            if (value.L_TID != 0)
            {
                var terminal = db.termainal.Where(c => c.L_ID.Equals(value.L_TID)).FirstOrDefault();
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
        public static string getRemarks(CS_AttendanceLog logs)
        {


            if (logs.time_in != null || logs.time_out != null)
            {
                if (logs.date.Value.DayOfWeek.ToString().Equals(offDay1) || logs.date.Value.Day.ToString().Equals(offDay2))
                {
                    return "OFF";

                }
                else
                {
                    return "Present";
                }

            }
            else if (logs.date.Value.DayOfWeek.ToString().Equals(offDay1) || logs.date.Value.DayOfWeek.ToString().Equals(offDay2))
            {
                return "OFF";
            }
            else
            {
                return "Absent";
            }
        }

    }

}
