using System;
using DLL.Models;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Configuration;

namespace Leaves_Sandwich_Rule_Implication
{
    public class Program
    {
        static void Main(string[] args)
        {
            string isStartupAllowed = "";
            Program p = new Program();
            DateTime dtJobStartTime = DateTime.Now; DateTime dtJobEndTime = DateTime.Now;
            DateTime dtFromDate = DateTime.Now; DateTime dtToDate = DateTime.Now;

            try
            {
                //Console.WriteLine("Job is started...");
                p.WriteToLog("Job is started...");

                isStartupAllowed = p.ValidateAppConfigValue("StartupAllowed");
                if (isStartupAllowed == "1")
                {
                    string strStartDayNumber = "", strEndDayNumber = "", strStartMonthNumber = "", strEndMonthNumber = "";
                    int iStartDayNumber = 0, iEndDayNumber = 0, iStartMonthNumber = -1, iEndMonthNumber = -1;

                    //get day number
                    strStartDayNumber = p.ValidateAppConfigValue("StartDayNumber");
                    iStartDayNumber = int.Parse(strStartDayNumber);

                    strEndDayNumber = p.ValidateAppConfigValue("EndDayNumber");
                    iEndDayNumber = int.Parse(strEndDayNumber);
                    if (iEndDayNumber == -1)
                    {
                        iEndDayNumber = DateTime.DaysInMonth(DateTime.Now.AddMonths(iEndDayNumber).Year, DateTime.Now.AddMonths(iEndDayNumber).Month);
                    }

                    //get month number
                    strStartMonthNumber = p.ValidateAppConfigValue("StartMonthNumber");
                    iStartMonthNumber = int.Parse(strStartMonthNumber);

                    strEndMonthNumber = p.ValidateAppConfigValue("EndMonthNumber");
                    iEndMonthNumber = int.Parse(strEndMonthNumber);

                    /*
                       SELECT * FROM [TRMS_DUHS].[dbo].[ConsolidatedAttendances]
                          where (date>='2020-02-01 00:00:00.000' and  date<='2020-02-29 00:00:00.000')
                          and (final_remarks = 'AB' or  final_remarks like '%L%')
                     */
                    //iStartMonth = -1; iEndMonth = -1;
                    //dtFromDate = new DateTime(DateTime.Now.AddMonths(iStartMonth).Year, DateTime.Now.AddMonths(iStartMonth).Month, iStartNumber);
                    //dtToDate = new DateTime(DateTime.Now.AddMonths(iEndMonth).Year, DateTime.Now.AddMonths(iEndMonth).Month, iEndNumber);

                    //iStartMonth = -1; iEndMonth = 0; iStartNumber = 2; iEndNumber = 1;
                    //dtFromDate = new DateTime(DateTime.Now.AddMonths(iStartMonth).Year, DateTime.Now.AddMonths(iStartMonth).Month, iStartNumber);
                    //dtToDate = new DateTime(DateTime.Now.AddMonths(iEndMonth).Year, DateTime.Now.AddMonths(iEndMonth).Month, iEndNumber);

                    //iStartDayNumber = 0; iEndDayNumber = 1; iStartMonthNumber = 1; iEndMonthNumber = 1;
                    dtFromDate = new DateTime(DateTime.Now.AddMonths(iStartMonthNumber).Year, DateTime.Now.AddMonths(iStartMonthNumber).Month, iStartDayNumber);
                    dtToDate = new DateTime(DateTime.Now.AddMonths(iEndMonthNumber).Year, DateTime.Now.AddMonths(iEndMonthNumber).Month, iEndDayNumber);

                    p.WriteToLog("[DATED] Start Date: " + dtFromDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                    p.WriteToLog("[DATED] End Date:" + dtToDate.ToString("dd-MMM-yyyy HH:mm:ss"));

                    //Check Holidays before FIRST Date - if Sunday
                    using (var db = new Context())
                    {
                        DateTime dtItterate = dtFromDate;
                        if (dtItterate.ToString("dddd").ToUpper() == "SUNDAY")
                        {
                            DateTime dtBack03 = dtFromDate.AddDays(-3);
                            DateTime dtBack02 = dtFromDate.AddDays(-2);
                            DateTime dtBack01 = dtFromDate.AddDays(-1);
                            GeneralCalendarOverride dbGeneralCalendarOverride03 = db.general_calender_override.Where(gco => gco.active && gco.date.HasValue && gco.date.Value == dtBack03).FirstOrDefault();
                            GeneralCalendarOverride dbGeneralCalendarOverride02 = db.general_calender_override.Where(gco => gco.active && gco.date.HasValue && gco.date.Value == dtBack02).FirstOrDefault();
                            GeneralCalendarOverride dbGeneralCalendarOverride01 = db.general_calender_override.Where(gco => gco.active && gco.date.HasValue && gco.date.Value == dtBack01).FirstOrDefault();
                            if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 == null)
                            {
                                //NO Off - 1 DAY before Sunday
                            }
                            else if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 != null)
                            {
                                dtFromDate = dtBack01;//OK
                            }
                            else if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 == null)
                            {
                                //
                            }
                            else if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 != null)
                            {
                                dtFromDate = dtBack02;//OK
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 == null)
                            {
                                //
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 != null)
                            {
                                dtFromDate = dtBack01;//OK
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 == null)
                            {
                                //
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 != null)
                            {
                                dtFromDate = dtBack03;//OK
                            }
                        }
                    }

                    //Check Holidays after LAST Date - if Sunday
                    using (var db = new Context())
                    {
                        DateTime dtItterate = dtToDate;
                        if (dtItterate.ToString("dddd").ToUpper() == "SUNDAY")
                        {
                            DateTime dtBack03 = dtToDate.AddDays(-3);
                            DateTime dtBack02 = dtToDate.AddDays(-2);
                            DateTime dtBack01 = dtToDate.AddDays(-1);
                            GeneralCalendarOverride dbGeneralCalendarOverride03 = db.general_calender_override.Where(gco => gco.active && gco.date.HasValue && gco.date.Value == dtBack03).FirstOrDefault();
                            GeneralCalendarOverride dbGeneralCalendarOverride02 = db.general_calender_override.Where(gco => gco.active && gco.date.HasValue && gco.date.Value == dtBack02).FirstOrDefault();
                            GeneralCalendarOverride dbGeneralCalendarOverride01 = db.general_calender_override.Where(gco => gco.active && gco.date.HasValue && gco.date.Value == dtBack01).FirstOrDefault();
                            if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 == null)
                            {
                                //NO Off - 1 DAY before Sunday
                            }
                            else if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 != null)
                            {
                                dtToDate = dtBack02;//OK//OK
                            }
                            else if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 == null)
                            {
                                //dtToDate = dtBack02;//OK
                            }
                            else if (dbGeneralCalendarOverride03 == null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 != null)
                            {
                                dtToDate = dtBack03;//OK
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 == null)
                            {
                                //dtToDate = dtBack03;//OK
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 == null && dbGeneralCalendarOverride01 != null)
                            {
                                dtToDate = dtBack02;//OK
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 == null)
                            {
                                //dtToDate = dtBack03;//OK
                            }
                            else if (dbGeneralCalendarOverride03 != null && dbGeneralCalendarOverride02 != null && dbGeneralCalendarOverride01 != null)
                            {
                                dtToDate = dtBack03.AddDays(-1);//OK//OK
                            }
                        }
                    }

                    p.RunSandwichRuleProcessing(dtFromDate, dtToDate);

                    ////////////////////////////////////

                    dtJobEndTime = DateTime.Now;

                    TimeSpan ts = dtJobEndTime - dtJobStartTime;
                    //Console.WriteLine("\n\nJob is Completed Successfully!!!" + ts.Minutes + " min " + ts.Seconds + " sec");
                    p.WriteToLog("Job is ended successfully in " + ts.Minutes + " min " + ts.Seconds + " sec");
                }
                else
                {
                    //Console.WriteLine("\n\nJob execution is disabled");
                    p.WriteToLog("Job execution is disabled");
                }

                //************ COMMENT ON PROD ********************/
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int RunSandwichRuleProcessing(DateTime dtFromDate, DateTime dtToDate)
        {
            int response = 0, count = 0;
            List<SunHoliDays> sundays_holidays = new List<SunHoliDays>();

            try
            {
                //get Sundays list
                DateTime dtItterate = dtFromDate;
                int daysCount = (dtToDate - dtFromDate).Days + 1;
                for (int i = 0; i < daysCount; i++)
                {
                    if (dtItterate.ToString("dddd").ToUpper() == "SUNDAY")
                    {
                        SunHoliDays obj = new SunHoliDays(0, dtItterate);
                        sundays_holidays.Add(obj);
                    }

                    dtItterate = dtItterate.AddDays(1);
                }

                //TESTING
                foreach (var sh in sundays_holidays)//////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx
                {
                    //apply for his/her leave
                    Console.WriteLine("1: Id=" + sh.id + " - Date: [" + sh.shDate.ToString("dd-MM-yyyy]"));
                    WriteToLog("1: Id=" + sh.id + " - Date: [" + sh.shDate.ToString("dd-MM-yyyy]"));
                }

                //get Holidays list
                using (var db = new Context())
                {
                    List<GeneralCalendarOverride> dbGeneralCalendarOverride = db.general_calender_override.Where(gco => gco.active && gco.date.HasValue && (gco.date.Value >= dtFromDate && gco.date.Value <= dtToDate)).ToList();
                    if (dbGeneralCalendarOverride != null && dbGeneralCalendarOverride.Count > 0)
                    {
                        foreach (var h in dbGeneralCalendarOverride)
                        {
                            if (h.date.HasValue && h.date.Value != null)
                            {
                                SunHoliDays obj = new SunHoliDays(count, h.date.Value);
                                sundays_holidays.Add(obj);

                                count++;
                            }
                        }
                    }
                }

                //TESTING
                //SunHoliDays obj5 = new SunHoliDays(0, Convert.ToDateTime("2020-02-19"));
                //sundays_holidays.Add(obj5);

                //SunHoliDays obj6 = new SunHoliDays(0, Convert.ToDateTime("2020-02-20"));
                //sundays_holidays.Add(obj6);

                //SunHoliDays obj4 = new SunHoliDays(0, Convert.ToDateTime("2020-02-24"));
                //sundays_holidays.Add(obj4);

                //SunHoliDays obj2 = new SunHoliDays(0, Convert.ToDateTime("2020-02-10"));
                //sundays_holidays.Add(obj2);

                //SunHoliDays obj3 = new SunHoliDays(0, Convert.ToDateTime("2020-02-11"));
                //sundays_holidays.Add(obj3);

                //TESTING
                //count++;
                //SunHoliDays obj3 = new SunHoliDays(count, Convert.ToDateTime("2020-02-11"));
                //sundays_holidays.Add(obj3);

                sundays_holidays = sundays_holidays.OrderBy(o => o.shDate).ToList();

                //resequence
                Console.WriteLine("\n");
                count = 1;
                foreach (var sh in sundays_holidays)//////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx
                {
                    sh.id = count;
                    count++;

                    //apply for his/her leave
                    //Console.WriteLine("2: Id=" + sh.id + "- Date: [" + sh.shDate.ToString("dd-MM-yyyy]"));
                    WriteToLog("2: Id=" + sh.id + "- Date: [" + sh.shDate.ToString("dd-MM-yyyy]"));
                }

                count = 1;
                for (int i = 0; i <= (sundays_holidays.Count - 1); i++)
                {
                    bool status = false;

                    if (i == (sundays_holidays.Count - 1))
                    {
                        if (status)
                        {
                            count = count - 1;
                        }

                        //count = count + 1;
                        sundays_holidays[i].id = count;
                    }
                    else
                    {
                        sundays_holidays[i].id = count;
                        status = AssignSequenceOrder(sundays_holidays[i + 1].shDate, sundays_holidays[i].shDate, sundays_holidays[i].id);

                        if (status)
                        {
                            count = count + 1;
                        }
                    }

                    ////if ((i + 1) < sundays_holidays.Count)
                    //{
                    //    if ((sundays_holidays[i + 1].shDate - sundays_holidays[i].shDate).Days == 1)
                    //    {
                    //        sundays_holidays[i].id = 0;
                    //        sundays_holidays[i + 1].id = 1;
                    //    }
                    //    else
                    //    {
                    //        if (sundays_holidays[i].id != 1)
                    //            sundays_holidays[i].id = 0;

                    //        //sundays_holidays[i + 1].id = 0;
                    //    }
                    //}
                }

                //TESTING
                Console.WriteLine("\n");
                foreach (var sh in sundays_holidays)//////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx
                {
                    //Console.WriteLine("3: Id=" + sh.id + " - Date: [" + sh.shDate.ToString("dd-MM-yyyy]"));
                    WriteToLog("3: Id=" + sh.id + " - Date: [" + sh.shDate.ToString("dd-MM-yyyy]"));
                }

                List<OFFDays> listOFFDays = new List<OFFDays>();
                if (sundays_holidays != null && sundays_holidays.Count > 0)
                {
                    //configure the Before and After Days beside OFF Days with their Count in a TABLE
                    for (int i = 0; i <= (sundays_holidays.Count - 1); i++)
                    {
                        if (sundays_holidays[0].shDate.ToString("dddd") == "SUNDAY")
                        {
                            //////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                        }
                        else if (sundays_holidays[sundays_holidays.Count - 1].shDate.ToString("dddd") == "SUNDAY")
                        {
                            //////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                        }
                        else
                        {
                            DateTime dtMin = GetMinMaxSequenceByID(sundays_holidays, sundays_holidays[i].id, 0);
                            DateTime dtMax = GetMinMaxSequenceByID(sundays_holidays, sundays_holidays[i].id, 1);

                            int difference = (dtMax - dtMin).Days + 1;

                            OFFDays obj = new OFFDays(sundays_holidays[i].id, dtMin.AddDays(-1), dtMin, dtMax, dtMax.AddDays(1), difference);
                            listOFFDays.Add(obj);
                        }
                    }

                    //////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    //if (sundays_holidays[sundays_holidays.Count - 1].id == 0)
                    //{
                    //    OFFDays objLast = new OFFDays(1, sundays_holidays[sundays_holidays.Count - 1].shDate.AddDays(-1), sundays_holidays[sundays_holidays.Count - 1].shDate, sundays_holidays[sundays_holidays.Count - 1].shDate, sundays_holidays[sundays_holidays.Count - 1].shDate.AddDays(1), 0);
                    //    listOFFDays.Add(objLast);
                    //}
                }

                //TESTING
                Console.WriteLine("\n");
                foreach (var off in listOFFDays)//////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                {
                    //Console.WriteLine("4: Id=" + off.id + " - Date: [" + off.Before.ToString("dd-MM-yyyy") + " : " + off.Later.ToString("dd-MM-yyyy]"));
                    WriteToLog("4: Id=" + off.id + " - Date: [" + off.Before.ToString("dd-MM-yyyy") + " : " + off.Later.ToString("dd-MM-yyyy]"));
                }

                //skip duplicates
                Console.WriteLine("\n");
                int seq_previous = 1, seq_current = 1;
                List<OFFDays> listOFFDaysFinal = new List<OFFDays>();
                count = 1;
                foreach (var off in listOFFDays)//////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                {
                    if (count == 1)
                    {
                        OFFDays obj = new OFFDays(off.id, off.Before, off.Holiday01, off.Holiday02, off.Later, off.count);
                        listOFFDaysFinal.Add(obj);

                        seq_previous = off.id;
                    }
                    else
                    {
                        seq_current = off.id;
                        if (seq_current != seq_previous)
                        {
                            OFFDays obj = new OFFDays(off.id, off.Before, off.Holiday01, off.Holiday02, off.Later, off.count);
                            listOFFDaysFinal.Add(obj);
                        }

                        seq_previous = seq_current;
                    }

                    count++;
                }

                //TESTING
                Console.WriteLine("\n");
                foreach (var off in listOFFDaysFinal)//////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                {
                    //Console.WriteLine("5: Id=" + off.id + " - Date: [" + off.Before.ToString("dd-MM-yyyy") + " : " + off.Later.ToString("dd-MM-yyyy]"));
                    WriteToLog("5: Id=" + off.id + " - Date: [" + off.Before.ToString("dd-MM-yyyy") + " : " + off.Later.ToString("dd-MM-yyyy]"));
                }

                //Nested Loop to apply for Leaves
                using (var db = new Context())
                {
                    var cnAbsentsList = db.consolidated_attendance.Where(c => (c.final_remarks == "AB" || c.final_remarks == "PLO" || c.final_remarks == "PLE" || c.final_remarks == "PLM") && (c.date >= dtFromDate && c.date <= dtToDate)).Select(c => new { aemp_id = c.employee.EmployeeId, aemp_code = c.employee.employee_code, a_date = c.date, a_rem = c.final_remarks }).ToList();

                    var cnAbsenteesEmployeesList = GetSandwichAbsenmteesListByDateRange(dtFromDate.ToString("yyyy-MM-dd"), dtToDate.ToString("yyyy-MM-dd"), dtToDate.Year);
                    if (cnAbsenteesEmployeesList != null && cnAbsenteesEmployeesList.Count > 0)
                    {
                        int e = 0, o = 1;
                        foreach (var empCode in cnAbsenteesEmployeesList)
                        {
                            count = 1;
                            foreach (var off in listOFFDaysFinal)
                            {
                                bool checkBeforeDate = cnAbsentsList.Find(a => a.aemp_code == empCode.e_code && a.a_date == off.Before) != null ? true : false;
                                if (checkBeforeDate)
                                {
                                    bool checkLaterDate = cnAbsentsList.Find(a => a.aemp_code == empCode.e_code && a.a_date == off.Later) != null ? true : false;
                                    if (checkLaterDate)
                                    {
                                        var dbLeaves = db.leave_application.Where(l => l.EmployeeId == empCode.e_id && l.FromDate == off.Before && l.ToDate == off.Later).FirstOrDefault();
                                        if (dbLeaves == null)
                                        {
                                            int days_count = 0, leave_type_id = 0;
                                            days_count = (off.Later - off.Before).Days + 1;

                                            //check others
                                            if (empCode.AllocatedOtherLeaves > 0 && (empCode.AvailedOtherLeaves + days_count) <= empCode.AllocatedOtherLeaves)
                                            {
                                                leave_type_id = 4;
                                            }

                                            //check sick
                                            if (empCode.AllocatedSickLeaves > 0 && empCode.AvailedSickLeaves + days_count <= empCode.AllocatedSickLeaves)
                                            {
                                                leave_type_id = 1;
                                            }

                                            //check annual
                                            if (empCode.AllocatedAnnualLeaves > 0 && empCode.AvailedAnnualLeaves + days_count <= empCode.AllocatedAnnualLeaves)
                                            {
                                                leave_type_id = 3;
                                            }

                                            //check casual
                                            if (empCode.AllocatedCasualLeaves > 0 && empCode.AvailedCasualLeaves + days_count <= empCode.AllocatedCasualLeaves)
                                            {
                                                leave_type_id = 2;
                                            }

                                            var appliedLeave = new LeaveApplication();
                                            appliedLeave.EmployeeId = empCode.e_id;
                                            appliedLeave.LeaveTypeId = leave_type_id;
                                            appliedLeave.FromDate = off.Before;
                                            appliedLeave.ToDate = off.Later;
                                            appliedLeave.DaysCount = days_count;
                                            appliedLeave.ApproverId = 1;
                                            appliedLeave.LeaveStatusId = 2;
                                            appliedLeave.LeaveReasonId = 1;
                                            appliedLeave.CreateDateTime = DateTime.Now;
                                            appliedLeave.UpdateDateTime = DateTime.Now;

                                            db.leave_application.Add(appliedLeave);
                                            //db.SaveChanges(); //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                                            if (count == 1)
                                            {
                                                e = e + 1;
                                            }

                                            count++;

                                            WriteToLog(e + "." + o + ") ECode: " + empCode.e_code + " [" + off.Before.ToString("dd-MM-yyyy") + " : " + off.Later.ToString("dd-MM-yyyy") + "] = " + days_count);
                                            //Console.WriteLine(e + "." + o + ") ECode: " + empCode.e_code + " [" + off.Before.ToString("dd-MM-yyyy") + " : " + off.Later.ToString("dd-MM-yyyy") + "] = " + days_count);
                                        }
                                    }
                                }
                                o++;
                            }

                            o = 1;

                            //Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        public DateTime GetMinMaxSequenceByID(List<SunHoliDays> listSequence, int id, int min_max)
        {
            DateTime dtReturn = DateTime.Now;

            if (min_max == 0)
            {
                dtReturn = listSequence.Where(l => l.id == id).Min(m => m.shDate);

                //(from i in listSequence
                //            let minId = listSequence.Where(l => l.id == id).Min(m => m.shDate)
                //            where i.id == id
                //            select i.shDate).FirstOrDefault();
            }
            else
            {
                dtReturn = listSequence.Where(l => l.id == id).Max(m => m.shDate);

                //dtReturn = (from i in listSequence
                //            let maxId = listSequence.Max(m => m.shDate)
                //            where i.id == id
                //            select i.shDate).FirstOrDefault();
            }

            return dtReturn;
        }

        public bool AssignSequenceOrder(DateTime dtLater, DateTime dtBefore, int id)
        {
            bool allowed_inc = false;

            if ((dtLater - dtBefore).Days == 1)
            {
                allowed_inc = false;
            }
            else
            {
                allowed_inc = true;
            }

            return allowed_inc;
        }

        public string ValidateAppConfigValue(string fName)
        {
            string strReturn = "";

            if (ConfigurationManager.AppSettings[fName] != null && ConfigurationManager.AppSettings[fName].ToString() != "")
            {
                strReturn = ConfigurationManager.AppSettings[fName];
            }

            return strReturn;
        }

        public void WriteToLog(string msg)
        {
            bool bLogStatus = false, bDLogStatus = false;

            //console log
            Console.WriteLine(msg);

            //IR999 - 3/3
            bLogStatus = bool.Parse(ValidateAppConfigValue("SNW_LOG_STATUS")); //int.Parse(ConfigurationManager.AppSettings["ma-log-status"] ?? "0");
            if (bLogStatus == true)
            {
                //db detailed-log
                bDLogStatus = bool.Parse(ValidateAppConfigValue("SNW_LOG_STATUS_DETAILED")); //int.Parse(ConfigurationManager.AppSettings["ma-log-status-detailed"] ?? "0");

                //log to database
                using (var db = new Context())
                {
                    if (bDLogStatus)
                    {
                        //log all type of messages
                        db.log_message.Add(new LogMessage()
                        {
                            LogTitle = "Sandwich Job",
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
                                LogTitle = "Sandwich Job",
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

        public static List<EmpIDCodeLeaves> GetSandwichAbsenmteesListByDateRange(string strFromDate, string strToDate, int iYearId)
        {
            List<EmpIDCodeLeaves> list_employee_codes = new List<EmpIDCodeLeaves>();

            using (Context db = new Context())
            {
                list_employee_codes = db.Database.SqlQuery<EmpIDCodeLeaves>(string.Format("[SP_Sandwich_Absentees_List] '{0}','{1}',{2}", strFromDate, strToDate, iYearId)).ToList();
            }

            return list_employee_codes;
        }
    }

    public class EmpIDCodeLeaves
    {
        public int e_id { get; set; }
        public string e_code { get; set; }

        public int AvailedSickLeaves { get; set; }
        public int AvailedCasualLeaves { get; set; }
        public int AvailedAnnualLeaves { get; set; }
        public int AvailedOtherLeaves { get; set; }

        public int AllocatedSickLeaves { get; set; }
        public int AllocatedCasualLeaves { get; set; }
        public int AllocatedAnnualLeaves { get; set; }
        public int AllocatedOtherLeaves { get; set; }
    }

    public class SunHoliDays
    {
        public int id { get; set; }
        public DateTime shDate { get; set; }

        public SunHoliDays(int _id, DateTime _shDate)
        {
            id = _id;
            shDate = _shDate;
        }
    }

    public class OFFDays
    {
        public int id { get; set; }
        public DateTime Before { get; set; }
        public DateTime Holiday01 { get; set; }
        public DateTime Holiday02 { get; set; }
        public DateTime Later { get; set; }
        public int count { get; set; }

        public OFFDays(int _id, DateTime _before, DateTime _holi01, DateTime _holi02, DateTime _later, int _count)
        {
            id = _id;

            Before = _before;
            Holiday01 = _holi01;
            Holiday02 = _holi02;
            Later = _later;

            count = _count;
        }
    }
}