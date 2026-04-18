using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.IO;
using System.Globalization;

namespace MarkContractualStaffAttendance
{
    public class Program
    {
        public static void synchAndCompile()
        {
            Console.WriteLine("* Syncher started! ");
            Console.WriteLine("* Processing HA transit entries... ");

            using (var db = new Context())
            {
                // get all the entries in ha transit which are active
                // - they have not been processed.
                var haTransit = db.ha_transit.Where(m => m.active && m.C_Date.Value >= Utils.effectiveDateTime.Value)
                    .OrderBy(m => m.C_Date).ThenBy(m => m.C_Time).ToArray();


                foreach (HaTransit value in haTransit)
                {

                    CS_AttendanceLog[] dirtyList = Syncher.getDirtyList(db, value);


                    //Console.WriteLine(value.C_Date.Value.ToString("dd-MM-yyyy"));


                    if (dirtyList != null && dirtyList.Count() > 0)
                    {
                        Console.Write("\nCompiling: " + value.C_Date.Value.ToString("dd-MM-yyyy"));
                        Compiler.cleanPersistantLogs(dirtyList, value, db);
                        Console.WriteLine("Done!");
                    }

                    // 1.5)
                    // pick the employee for the current ha transit entry
                    // only if the time end in its persistent attendance log is not
                    // null. That is, if the employee is a freshie and its shift is missing.
                    var employee = db.contractual_staff.Where(m =>
                        m.active &&
                        m.employee_code.Equals(value.C_Unique) &&
                        m.persistent_log.date != null
                        ).FirstOrDefault();
                    if (employee == null)
                    {
                        continue;
                    }

                    // 2)
                    var log = employee.persistent_log;

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
                            log.terminal_out = Utils.getTerminalName(value, db);
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


                Console.Write("* Commiting changes to database... ");
                db.SaveChanges();
                Console.WriteLine("Done!");
            }
        }

        // This method reads all the consolidated logs, and updates them according
        // to the current calendars.

        /*   public static void correctConsolidatedLogs(int month, int year)
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
                        Console.WriteLine("Employee not found... " + log.ConsolidatedAttendanceId);
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
                    temp.terminal_in = log.terminal_in;
                    temp.terminal_out = log.terminal_out;


                    log.status_in = Utils.getTimeInRemarks(temp);
                    log.status_out = Utils.getTimeOutRemarks(temp);
                    log.final_remarks = Utils.getFinalRemarks(log.status_in, log.status_out);
                }
                Console.WriteLine(" Done!");
                Console.Write("Saving changes to db....");
                db.SaveChanges();
                Console.WriteLine("done!");
                Console.WriteLine("Started: " + startTime.ToString("dd-MM-yyyy HH:mm:ss"));
                Console.WriteLine("Done: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            }

        }
        */

        public static void pipeline()
        {
            Console.WriteLine("*****************************\nTime Tune pipeline initiated\n*****************************");

            Console.Write("* Checking for new employees... ");
            //syncher Check For Freshies
            Syncher.checkForFreshies();
            Console.WriteLine("Done!");

            synchAndCompile();

            Console.WriteLine("Pipeline finished! ");

        }

        public static void reTryCatch()
        {
            for (int i = 7; i <= 8; i++)
            {
                try
                {
                    // (month,year)
                  //  correctConsolidatedLogs(i, 2016);

                }
                catch (System.OutOfMemoryException)
                {
                    Console.WriteLine("\n\nRan out of memory... running explicit garbage collection before a re-attempt!!!");
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
            pipeline();
        }

    }
}
