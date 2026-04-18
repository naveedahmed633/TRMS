using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Configuration;
using System.Data.Entity;

namespace CoursesAttendance
{
    class Program
    {
        static void Main(string[] args)
        {
            string strMessage = "";
            DateTime dtStarted = DateTime.Now, dtFinished = DateTime.Now;

            try
            {
                //STARTING step
                strMessage = "::::: STARTED @ " + dtStarted.ToString("dd MMM yyyy hh:mm:ss tt") + " :::::";
                Program.WriteToLog(strMessage);

                strMessage = "* Courses Attendance - Process Started! ";
                Program.WriteToLog(strMessage);

                using (var db = new Context())
                {
                    ProcessCoursesAttendance(db);
                }

                strMessage = "* Courses Attendance - Process Completed! ";
                Program.WriteToLog(strMessage);

                //FINAL step
                dtFinished = DateTime.Now;

                strMessage = "::::: ENDED @ " + dtFinished.ToString("dd MMM yyyy hh:mm:ss tt") + " :::::";
                Program.WriteToLog(strMessage);


                TimeSpan tSpan = (dtFinished - dtStarted);
                strMessage = "::::: TOTAL TIME - " + tSpan.ToString(@"hh\:mm\:ss") + " (hh:mm:ss) :::::";

                Program.WriteToLog(strMessage);
            }
            catch (Exception ex)
            {
                string stkText = "na", msgText = "na", innText = "na";

                if (ex.StackTrace != null && ex.StackTrace.Length > 0)
                {
                    stkText = (ex.StackTrace.Length > 301) ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
                }

                if (ex.Message != null && ex.Message.Length > 0)
                {
                    msgText = (ex.Message.Length > 301) ? ex.Message.Substring(0, 300) : ex.Message;
                }

                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message != null && ex.InnerException.Message.Length > 0)
                    {
                        innText = (ex.InnerException.Message.Length > 501) ? ex.InnerException.Message.Substring(0, 500) : ex.InnerException.Message;
                    }
                }

                Program.WriteToLog("********** TimeTune Pipeline - EXCEPTION - StackTrace: " + stkText + "\n\nMessage: " + msgText + ", \n\nInner: " + innText);
            }

            //Console.ReadKey();
        }

        public static void WriteToLog(string msg)
        {
            bool bLogStatus = false, bDLogStatus = false;

            //console log
            Console.WriteLine(msg);

            //IR999 - 3/3
            bLogStatus = Utils.GetLogStatus("CA_LOG_STATUS"); //int.Parse(ConfigurationManager.AppSettings["ma-log-status"] ?? "0");
            if (bLogStatus)
            {
                //db detailed-log
                bDLogStatus = Utils.GetLogStatus("CA_LOG_STATUS_DETAILED"); //int.Parse(ConfigurationManager.AppSettings["ma-log-status-detailed"] ?? "0");

                //log to database
                using (var db = new Context())
                {
                    if (bDLogStatus)
                    {
                        //log all type of messages
                        db.log_message.Add(new LogMessage()
                        {
                            LogTitle = "CA Job",
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
                                LogTitle = "CA Job",
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

        private static void ProcessCoursesAttendance(Context db)
        {
            int response = 0, counter = 1, total = 0;
            string strMessage = "";

            List<ScheduledDateTerminals> listUnprocessedDateTerminals = new List<ScheduledDateTerminals>();

            listUnprocessedDateTerminals = getUnprocessedDateTerminalsList();

            if (listUnprocessedDateTerminals != null && listUnprocessedDateTerminals.Count > 0)
            {
                total = listUnprocessedDateTerminals.Count;

                foreach (ScheduledDateTerminals sTerm in listUnprocessedDateTerminals)
                {
                    var dbStudentSchedule = (from s in db.organization_campus_room_course_schedule
                                             join r in db.organization_program_course_enrollment on s.CourseId equals r.ProgramCourseId
                                             join e in db.employee on r.EmployeeStudentId equals e.EmployeeId
                                             join m in db.organization_campus_building_room on s.RoomId equals m.Id
                                             where s.CampusId == r.CampusId && 
                                             e.active && e.function.name.ToLower() == "student" && //e.timetune_active &&
                                             //DbFunctions.TruncateTime(s.StartTime) == sTerm.UnProcessedDate &&
                                             EntityFunctions.TruncateTime(s.StartTime) == sTerm.UnProcessedDate &&
                                             sTerm.ScheduledTerminalID == m.Id
                                             // db.organization_campus_building_room.Where(m => m.Id == s.RoomId).FirstOrDefault().TerminalId
                                             select new
                                             {
                                                 std_id = r.EmployeeStudentId,
                                                 std_code = db.employee.Where(c => c.EmployeeId == r.EmployeeStudentId).FirstOrDefault().employee_code,
                                                 std_group_id = db.employee.Where(c => c.EmployeeId == r.EmployeeStudentId).FirstOrDefault() == null ? 0 : db.employee.Where(c => c.EmployeeId == r.EmployeeStudentId).FirstOrDefault().region.RegionId,
                                                 sch_group_id = s.LectureGroupId,
                                                 std_pshift_id = db.employee.Where(c => c.EmployeeId == r.EmployeeStudentId).FirstOrDefault() == null ? 1 : db.employee.Where(c => c.EmployeeId == r.EmployeeStudentId).FirstOrDefault().program_shift_id,
                                                 sch_pshift_id = s.ShiftId,
                                                 course_id = s.CourseId,
                                                 course_code = db.organization_program_course.Where(c => c.Id == s.CourseId).FirstOrDefault().CourseCode,
                                                 course_start_time = s.StartTime,
                                                 course_end_time = s.EndTime,
                                                 term_id = db.organization_campus_building_room.Where(k => k.Id == s.RoomId).FirstOrDefault().TerminalId,
                                                 //term_out_id = db.organization_campus_building_room.Where(r => r.Id == s.RoomId).FirstOrDefault().TerminalId,
                                             }).ToList();

                    if (dbStudentSchedule != null && dbStudentSchedule.Count > 0)
                    {
                        HaTranistInOut hInOut = new HaTranistInOut();
                        RemarksStatus rmStatus = new RemarksStatus();

                        List<HaTransitView> haListView = new List<HaTransitView>();

                        var haList = db.ha_transit.Where(h => h.course_active && h.C_Date == sTerm.UnProcessedDate).ToList();
                        if (haList != null && haList.Count > 0)
                        {
                            foreach (var h in haList)
                            {
                                haListView.Add(new HaTransitView()
                                {
                                    U_Name = h.C_Name,
                                    U_Code = h.C_Unique,
                                    L_TID = h.L_TID,
                                    L_UID = h.L_UID,
                                    U_Date_Time = Convert.ToDateTime(h.C_Date.Value.ToString("yyyy-MM-dd") + " " + h.C_Time),
                                    course_active = h.course_active
                                });
                            }
                        }

                        if (haListView != null && haListView.Count > 0)
                        {
                            int iMinFMinutes = 0, iMaxFMinutes = 0;
                            iMinFMinutes = Utils.GetMinFlexiMinutes(); iMaxFMinutes = Utils.GetMaxFlexiMinutes();

                            foreach (var ss in dbStudentSchedule)
                            {
                                //int ii = 0, jj = 0;
                                //if (ss.std_code == "000002" && ss.term_id.ToString() == "1" && ss.course_id.ToString() == "10")
                                //{
                                //    ii = 1;
                                //}


                                //if (ss.std_code == "000005" && ss.term_id.ToString() == "1" && ss.course_id.ToString() == "1")
                                //{
                                //    jj = 2;
                                //}


                                hInOut = getHaTransitLogsByDate(haListView, sTerm.UnProcessedDate, ss.std_code, iMaxFMinutes, ss.term_id, ss.course_start_time, ss.course_end_time);

                                rmStatus.in_remarks = Utils.getLogTimeInRemarks(hInOut.time_in, hInOut.time_out, iMinFMinutes, ss.course_start_time, ss.course_end_time);
                                rmStatus.out_remarks = Utils.getLogTimeOutRemarks(hInOut.time_in, hInOut.time_out, iMinFMinutes, ss.course_start_time, ss.course_end_time);
                                rmStatus.final_remarks = Utils.getFinalRemarks(rmStatus.in_remarks, rmStatus.out_remarks);

                                ConsolidateCourseAttendance(db, sTerm.UnProcessedDate, sTerm.ScheduledTerminalID, ss.std_id, ss.std_group_id, ss.sch_group_id, ss.std_pshift_id, ss.sch_pshift_id, ss.course_id, hInOut.time_in, rmStatus.in_remarks, hInOut.time_out, rmStatus.out_remarks, rmStatus.final_remarks, ss.term_id, ss.term_id, ss.course_start_time, ss.course_end_time);
                            }
                        }
                    }

                    //update HaTransit logs back-1006452=1573 records
                    response = updateHaTransitLogs(sTerm.AHaID, sTerm.ZHaID, sTerm.ScheduledTerminalID);
                    if (response == 1)
                    {
                        strMessage = string.Format("Succeed {0}/{1}: Update HaID: {2}-{3}={4} having Tid: {5}", counter, total, sTerm.AHaID, sTerm.ZHaID, (sTerm.ZHaID - sTerm.AHaID), sTerm.ScheduledTerminalID);
                        Program.WriteToLog(strMessage);
                    }
                    else
                    {
                        strMessage = string.Format("Error {0}/{1}: Update HaID: {2}-{3}={4} having Tid: {5}", counter, total, sTerm.AHaID, sTerm.ZHaID, (sTerm.ZHaID - sTerm.AHaID), sTerm.ScheduledTerminalID);
                        Program.WriteToLog(strMessage);
                    }

                    counter++;
                }
            }
        }

        private static void ConsolidateCourseAttendance(Context db, DateTime dtUnProcessedDate, int iScheduledTerminalID, int iEStudentID, int iStdGroupID, int iSchGroupID,
                                int iStdPShiftID, int iSchPShiftID, int iCourseID, DateTime? dtStdTimeIn, string strRemarksIn, DateTime? dtStdTimeOut, string strRemarksOut, string strRemarksFinal,
                                int iTermIn, int iTermOut, DateTime strCStartTime, DateTime strCEndTime)
        {

            var cAttendance = db.organization_course_attendance
                .Where(a => a.employee_student_id == iEStudentID && a.course_id == iCourseID && a.schedule_date_time == dtUnProcessedDate &&
                                        a.terminal_in_id == iTermIn && a.terminal_out_id == iTermOut)
                                            .FirstOrDefault();
            if (cAttendance != null)//already
            {
                cAttendance.schedule_date_time = dtUnProcessedDate;
                cAttendance.employee_student_id = iEStudentID;
                cAttendance.student_group_id = iStdGroupID;
                cAttendance.schedule_group_id = iSchGroupID;
                cAttendance.student_pshift_id = iStdPShiftID;
                cAttendance.schedule_pshift_id = iSchPShiftID;
                cAttendance.course_id = iCourseID;

                cAttendance.student_time_in = (dtStdTimeIn != null && dtStdTimeIn.HasValue) ? dtStdTimeIn.Value : (DateTime?)null;//getFROMHaTransit()
                cAttendance.status_in = strRemarksIn; // Utils.getTimeInRemarks(cAttendance.time_in);
                cAttendance.student_time_out = (dtStdTimeOut != null && dtStdTimeOut.HasValue) ? dtStdTimeOut.Value : (DateTime?)null;//getFROMHaTransit()
                cAttendance.status_out = strRemarksOut; // Utils.getTimeOutRemarks(cAttendance.time_out);
                cAttendance.final_remarks = strRemarksFinal; // Utils.setFinalRemarks(temp);

                cAttendance.terminal_in_id = iTermIn; // Utils.getTerminalName(iTermIn, db);
                cAttendance.terminal_out_id = iTermOut; // Utils.getTerminalName(iTermOut, db);
                cAttendance.course_time_start = strCStartTime;//getFROMHaTransit()
                cAttendance.course_time_end = strCEndTime;//getFROMHaTransit()
                cAttendance.active = true;

                //cAttendance.process_count = 1;
                //cAttendance.process_code = null;

                                cAttendance.is_sent = false;

                db.SaveChanges();
            }
            else//new
            {
                OrganizationCourseAttendance crsAtn = new OrganizationCourseAttendance();
                crsAtn.schedule_date_time = dtUnProcessedDate;
                crsAtn.employee_student_id = iEStudentID;
                crsAtn.student_group_id = iStdGroupID;
                crsAtn.schedule_group_id = iSchGroupID;
                crsAtn.student_pshift_id = iStdPShiftID;
                crsAtn.schedule_pshift_id = iSchPShiftID;
                crsAtn.course_id = iCourseID;

                crsAtn.student_time_in = (dtStdTimeIn != null && dtStdTimeIn.HasValue) ? dtStdTimeIn.Value : (DateTime?)null;//getFROMHaTransit()
                crsAtn.status_in = strRemarksIn; // Utils.getTimeInRemarks(cAttendance.time_in);
                crsAtn.student_time_out = (dtStdTimeOut != null && dtStdTimeOut.HasValue) ? dtStdTimeOut.Value : (DateTime?)null;//getFROMHaTransit()
                crsAtn.status_out = strRemarksOut; // Utils.getTimeOutRemarks(cAttendance.time_out);
                crsAtn.final_remarks = strRemarksFinal; // Utils.setFinalRemarks(temp);

                crsAtn.terminal_in_id = iTermIn; //Utils.getTerminalName(iTermIn, db);
                crsAtn.terminal_out_id = iTermOut; //Utils.getTerminalName(iTermOut, db);
                crsAtn.course_time_start = strCStartTime;//getFROMHaTransit()
                crsAtn.course_time_end = strCEndTime;//getFROMHaTransit()

                cAttendance.process_count = 1;
                cAttendance.process_code = null;

                crsAtn.active = true;
                crsAtn.is_sent = false;

                db.organization_course_attendance.Add(crsAtn);
                db.SaveChanges();
            }

            ////////////////////////// Check OFF in scheduler - Sunday or Holiday ////////////////////////////////////////////////////

            //Todays
            int iOFFHoliday = 0;
            iOFFHoliday = Utils.GetAppSettingsValuByCode("off-day-01") * -1;
            DateTime dtOFFHoliday = Convert.ToDateTime(DateTime.Now.AddDays(iOFFHoliday).ToString("yyyy-MM-dd"));
            //DateTime dtOFFHoliday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            var dbStudentCampus = db.employee.Where(e => e.EmployeeId == iEStudentID).FirstOrDefault();
            if (dbStudentCampus != null)
            {
                int iStudentCampusID = 0;
                iStudentCampusID = dbStudentCampus.campus_id;
                if (iStudentCampusID > 0)
                {
                    var dbSchedulerOFFHoliday = db.organization_campus_room_course_schedule.Where(s => s.CampusId == iStudentCampusID && EntityFunctions.TruncateTime(s.StartTime) == dtOFFHoliday && s.CourseId == 0).FirstOrDefault();
                    if (dbSchedulerOFFHoliday != null)
                    {
                        var oAttendance = db.organization_course_attendance
                                                .Where(a => a.employee_student_id == iEStudentID && a.course_id == 0
                                                        && a.schedule_date_time == dtOFFHoliday && a.terminal_in_id == 0 && a.terminal_out_id == 0)
                                                            .FirstOrDefault();
                        if (oAttendance == null)
                        {
                            OrganizationCourseAttendance offAtn = new OrganizationCourseAttendance();
                            offAtn.schedule_date_time = dtOFFHoliday;
                            offAtn.employee_student_id = iEStudentID;
                            offAtn.course_id = 0;

                            offAtn.student_time_in = dtOFFHoliday;
                            offAtn.status_in = "Off";
                            offAtn.student_time_out = dtOFFHoliday;
                            offAtn.status_out = "Off";
                            offAtn.final_remarks = "OFF";

                            offAtn.terminal_in_id = 0;
                            offAtn.terminal_out_id = 0;
                            offAtn.course_time_start = dtOFFHoliday;
                            offAtn.course_time_end = dtOFFHoliday;

                            offAtn.active = true;
                            offAtn.is_sent = false;

                            db.organization_course_attendance.Add(offAtn);
                            db.SaveChanges();
                        }
                    }
                }
            }

            //Last Day
            int iOFFHoliday1 = 0;
            iOFFHoliday1 = Utils.GetAppSettingsValuByCode("off-day-02") * -1;
            DateTime dtOFFHoliday1 = Convert.ToDateTime(DateTime.Now.AddDays(iOFFHoliday1).ToString("yyyy-MM-dd"));
            var dbStudentCampus1 = db.employee.Where(e => e.EmployeeId == iEStudentID).FirstOrDefault();
            if (dbStudentCampus1 != null)
            {
                int iStudentCampusID = 0;
                iStudentCampusID = dbStudentCampus1.campus_id;
                if (iStudentCampusID > 0)
                {
                    var dbSchedulerOFFHoliday = db.organization_campus_room_course_schedule.Where(s => s.CampusId == iStudentCampusID && EntityFunctions.TruncateTime(s.StartTime) == dtOFFHoliday1 && s.CourseId == 0).FirstOrDefault();
                    if (dbSchedulerOFFHoliday != null)
                    {
                        var oAttendance = db.organization_course_attendance
                                                .Where(a => a.employee_student_id == iEStudentID && a.course_id == 0
                                                        && a.schedule_date_time == dtOFFHoliday1 && a.terminal_in_id == 0 && a.terminal_out_id == 0)
                                                            .FirstOrDefault();
                        if (oAttendance == null)
                        {
                            OrganizationCourseAttendance offAtn = new OrganizationCourseAttendance();
                            offAtn.schedule_date_time = dtOFFHoliday1;
                            offAtn.employee_student_id = iEStudentID;
                            offAtn.course_id = 0;

                            offAtn.student_time_in = dtOFFHoliday1;
                            offAtn.status_in = "Off";
                            offAtn.student_time_out = dtOFFHoliday1;
                            offAtn.status_out = "Off";
                            offAtn.final_remarks = "OFF";

                            offAtn.terminal_in_id = 0;
                            offAtn.terminal_out_id = 0;
                            offAtn.course_time_start = dtOFFHoliday1;
                            offAtn.course_time_end = dtOFFHoliday1;

                            offAtn.active = true;
                            offAtn.is_sent = false;

                            db.organization_course_attendance.Add(offAtn);
                            db.SaveChanges();

                        }
                    }
                }
            }
            ////////////////////// OFF or Holiday marking /////////////////////////////

        }

        public static List<ScheduledDateTerminals> getUnprocessedDateTerminalsList()
        {
            List<ScheduledDateTerminals> listDateTerminals = new List<ScheduledDateTerminals>();

            using (var db = new Context())
            {
                db.Database.CommandTimeout = 600;

                listDateTerminals = db.Database.SqlQuery<ScheduledDateTerminals>("SP_GetCourseAttendanceHaDatesTerminals").ToList();
            }

            return listDateTerminals;
        }

        public static int updateHaTransitLogs(int iAHaID, int iZHaID, int iTermID)
        {
            int response = 0;

            using (var db = new Context())
            {
                db.Database.CommandTimeout = 600;

                response = db.Database.SqlQuery<int>(string.Format("SP_UpdateCourseAttendanceHaDatesTerminals {0},{1},{2}", iAHaID, iZHaID, iTermID)).FirstOrDefault();
            }

            return response;
        }

        public static HaTranistInOut getHaTransitLogsByDate(List<HaTransitView> haListView, DateTime dtSchedule, string strEmpCode, int iFlexiMin, int iTermId, DateTime dtCourseStartTime, DateTime dtCourseEndTime)
        {
            HaTranistInOut haEntry = new HaTranistInOut();

            DateTime dtCourseStartMinTime = dtCourseStartTime.AddMinutes(-1 * iFlexiMin);
            DateTime dtCourseStartMaxTime = dtCourseStartTime.AddMinutes(iFlexiMin);

            DateTime dtCourseEndMinTime = dtCourseEndTime.AddMinutes(-1 * iFlexiMin);
            DateTime dtCourseEndMaxTime = dtCourseEndTime.AddMinutes(iFlexiMin);

            if (haListView != null && haListView.Count > 0)
            {
                var haLogIn = haListView.Where(h => h.U_Code == strEmpCode && h.L_TID == iTermId &&
                                                    (h.U_Date_Time >= dtCourseStartMinTime && h.U_Date_Time <= dtCourseStartMaxTime))
                                        .OrderBy(o => o.U_Date_Time).FirstOrDefault();
                if (haLogIn != null)
                {
                    haEntry.time_in = haLogIn.U_Date_Time; // Convert.ToDateTime(dtSchedule.ToString("yyyy-MM-dd") + " " + haLogIn.C_Time);
                }
                else
                {
                    haEntry.time_in = null;
                }

                var haLogOut = haListView.Where(h => h.U_Code == strEmpCode && h.L_TID == iTermId &&
                                                    (h.U_Date_Time >= dtCourseEndMinTime && h.U_Date_Time <= dtCourseEndMaxTime))
                                            .OrderByDescending(o => o.U_Date_Time).FirstOrDefault();
                if (haLogOut != null)
                {
                    haEntry.time_out = haLogOut.U_Date_Time; // Convert.ToDateTime(dtSchedule.ToString("yyyy-MM-dd") + " " + haLogOut.C_Time);

                    var haLogInAgain = haListView.Where(h => h.U_Code == strEmpCode && h.L_TID == iTermId &&
                                                    (h.U_Date_Time >= dtCourseStartMinTime && h.U_Date_Time <= dtCourseStartMaxTime))
                                        .OrderBy(o => o.U_Date_Time).FirstOrDefault();
                    if (haLogInAgain != null)
                    {
                        haEntry.time_in = haLogInAgain.U_Date_Time; // Convert.ToDateTime(dtSchedule.ToString("yyyy-MM-dd") + " " + haLogInAgain.C_Time);
                    }
                    else
                    {
                        haEntry.time_in = null;
                    }

                    if (haEntry.time_out == haEntry.time_in)
                    {
                        haEntry.time_out = null;
                    }
                    else
                    {
                        haEntry.time_out = haLogOut.U_Date_Time; // Convert.ToDateTime(dtSchedule.ToString("yyyy-MM-dd") + " " + haLogOut.C_Time);
                    }
                }
                else
                {
                    haEntry.time_out = null;
                }
            }

            return haEntry;
        }



        public class Utils
        {
            //public static DateTime? effectiveDateTime = DateTime.ParseExact(ConfigurationManager.AppSettings["attendance-effective-date"], "dd-MM-yyyy", CultureInfo.InvariantCulture);

            public static string strDate = GetEffectiveDate();
            public static DateTime? effectiveDateTime = DateTime.ParseExact(strDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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

            public static string getLogTimeInRemarks(DateTime? dtStdTimeIn, DateTime? dtStdTimeOut, int iFlexiMin, DateTime dtCrsTimeStart, DateTime dtCrsTimeEnd)
            {
                //RemarksStatus rs = new RemarksStatus();

                DateTime dtCrMinStTime = dtCrsTimeStart.AddMinutes(-1 * iFlexiMin);
                DateTime dtCrMaxStTime = dtCrsTimeStart.AddMinutes(1 * iFlexiMin);

                if (dtStdTimeIn == null && dtStdTimeOut == null)
                {
                    return DLL.Commons.TimeInRemarks.AB;
                }

                if (dtStdTimeIn == null && dtStdTimeOut != null)
                {
                    return DLL.Commons.TimeInRemarks.MISS_PUNCH;
                }

                if (dtStdTimeIn == null)
                {
                    return DLL.Commons.TimeInRemarks.MISS_PUNCH;
                }
                else
                {
                    /*
                     *  long span = log.late_time.Value.Ticks - log.time_start.Value.Ticks;
                    long timeIn = log.time_in.Value.Ticks - log.time_start.Value.Ticks;
                    if (timeIn <= span)
                    {
                        return DLL.Commons.TimeInRemarks.ON_TIME;
                    }
                    else
                    {
                        return DLL.Commons.TimeInRemarks.LATE;
                    }

                    ////////////////////////////

                     long span = dtCrMaxEnTime.Ticks - dtCrMinEnTime.Ticks;
                    long timeIn = dtStdTimeIn.Value.Ticks - dtCrMinEnTime.Ticks;
                    if (timeIn <= span)
                    {
                        return DLL.Commons.TimeInRemarks.ON_TIME;
                    }
                    else
                    {
                        return DLL.Commons.TimeInRemarks.LATE;
                    }
                    */

                    if (dtStdTimeIn.Value <= dtCrMaxStTime)
                    {
                        return DLL.Commons.TimeInRemarks.ON_TIME;
                    }
                    else
                    {
                        return DLL.Commons.TimeInRemarks.LATE;
                    }

                }

            }

            public static string getLogTimeOutRemarks(DateTime? dtStdTimeIn, DateTime? dtStdTimeOut, int iFlexiMin, DateTime dtCrsTimeStart, DateTime dtCrsTimeEnd)
            {
                //RemarksStatus rs = new RemarksStatus();

                DateTime dtCrMinStTime = dtCrsTimeStart.AddMinutes(-1 * iFlexiMin);
                DateTime dtCrMaxStTime = dtCrsTimeStart.AddMinutes(1 * iFlexiMin);

                // check for a holiday shift
                if (dtStdTimeIn == null && dtStdTimeOut == null)
                {
                    return DLL.Commons.TimeOutRemarks.AB;

                }

                if (dtStdTimeIn != null && dtStdTimeOut == null)
                {
                    return DLL.Commons.TimeInRemarks.MISS_PUNCH;
                }

                // check time in status
                if (dtStdTimeOut == null)
                {
                    return DLL.Commons.TimeOutRemarks.MISS_PUNCH;
                }
                else
                {
                    long span = dtCrsTimeEnd.Ticks - dtCrsTimeStart.Ticks;
                    long timeOut = dtStdTimeOut.Value.Ticks - dtCrsTimeEnd.Ticks;
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
                else if (
                 status_in.Equals(DLL.Commons.TimeInRemarks.AB) &&
                 status_out.Equals(DLL.Commons.TimeOutRemarks.AB))
                {

                    return DLL.Commons.FinalRemarks.ABSENT;

                }
                else
                {
                    return ""; // should never happen
                }


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

            public static RemarksStatus getAllRemarks(DateTime? dtStdTimeIn, DateTime? dtStdTimeOut, int iFlexiMin, DateTime strCrsTimeStart, DateTime strCrsTimeEnd)
            {
                RemarksStatus rs = new RemarksStatus();

                DateTime dtCrMinStTime = strCrsTimeStart.AddMinutes(-1 * iFlexiMin);
                DateTime dtCrMaxStTime = strCrsTimeStart.AddMinutes(1 * iFlexiMin);

                if (dtStdTimeIn >= dtCrMinStTime && dtStdTimeIn <= dtCrMaxStTime)
                {
                    rs.in_remarks = DLL.Commons.TimeInRemarks.ON_TIME;
                }
                else
                {
                    rs.in_remarks = DLL.Commons.TimeInRemarks.LATE;
                }

                DateTime dtCrMinEnTime = strCrsTimeEnd.AddMinutes(-1 * iFlexiMin);
                DateTime dtCrMaxEnTime = strCrsTimeEnd.AddMinutes(1 * iFlexiMin);

                if (dtStdTimeOut >= dtCrMinEnTime && dtStdTimeOut <= dtCrMaxEnTime)
                {
                    rs.out_remarks = DLL.Commons.TimeInRemarks.ON_TIME;
                }
                else
                {
                    rs.out_remarks = DLL.Commons.TimeInRemarks.LATE;
                }

                if (rs.in_remarks == DLL.Commons.TimeInRemarks.ON_TIME && rs.out_remarks == DLL.Commons.TimeInRemarks.ON_TIME)
                {
                    rs.final_remarks = DLL.Commons.FinalRemarks.PRESENT;
                }
                else
                {
                    rs.final_remarks = DLL.Commons.FinalRemarks.PLO;
                }

                return rs;

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

            // Get Terminal Name
            public static string getTerminalName(int value, Context db)
            {
                string terminalName = "";
                if (value != 0)
                {
                    Terminals terminal = null;

                    terminal = db.termainal.Where(c => c.L_ID.Equals(value)).FirstOrDefault();
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

            public static int GetMinFlexiMinutes()
            {
                int iMinFlexiMinutes = 15;

                using (Context db = new Context())
                {
                    var dbMinFlexiMinutes = db.access_code_value.Where(a => a.AccessCode.ToUpper() == "MIN_FLEXI_MINUTES").FirstOrDefault();
                    if (dbMinFlexiMinutes != null)
                    {
                        iMinFlexiMinutes = int.Parse(dbMinFlexiMinutes.AccessValue);
                    }
                }

                return iMinFlexiMinutes;
            }

            public static int GetMaxFlexiMinutes()
            {
                int iMaxFlexiMinutes = 22;

                using (Context db = new Context())
                {
                    var dbMaxFlexiMinutes = db.access_code_value.Where(a => a.AccessCode.ToUpper() == "MAX_FLEXI_MINUTES").FirstOrDefault();
                    if (dbMaxFlexiMinutes != null)
                    {
                        iMaxFlexiMinutes = int.Parse(dbMaxFlexiMinutes.AccessValue);
                    }
                }

                return iMaxFlexiMinutes;
            }

            public static int GetAppSettingsValuByCode(string strCode)
            {
                int iValue = 0;

                if (ConfigurationManager.AppSettings[strCode] != null && ConfigurationManager.AppSettings[strCode].ToString() != "")
                    iValue = int.Parse(ConfigurationManager.AppSettings[strCode].ToString());
                else
                    iValue = 0;

                return iValue;
            }
        }

        public class ScheduledDateTerminals
        {
            public int AHaID { get; set; }//min
            public int ZHaID { get; set; }//max
            public int DHaID { get; set; }//difference
            public DateTime UnProcessedDate { get; set; }
            public int ScheduledTerminalID { get; set; }
        }

        public class HaTranistInOut
        {
            public DateTime? time_in { get; set; }
            public DateTime? time_out { get; set; }
        }

        public class HaTransitView
        {
            public string U_Name { get; set; }
            public string U_Code { get; set; }
            public int L_UID { get; set; }
            public int L_TID { get; set; }
            public bool course_active { get; set; }
            public DateTime U_Date_Time { get; set; }
        }

        public class RemarksStatus
        {
            public string in_remarks { get; set; }
            public string out_remarks { get; set; }
            public string final_remarks { get; set; }
        }
    }


}

/*
  (from s in db.organization_campus_room_course_schedule
                                             join r in db.organization_program_course_enrollment on s.CourseId equals r.ProgramCourseId
                                             join m in db.organization_campus_building_room on s.RoomId equals m.Id
                                             join t in db.termainal on m.TerminalId equals t.L_ID  
                                             where (EntityFunctions.TruncateTime(s.StartTime) == sTerm.UnProcessedDate) //&& sTerm.ScheduledTerminalID == db.termainal.Where(x => x.L_ID == m.TerminalId).FirstOrDefault().L_ID
                                             select new
                                             {
                                                 std_id = r.EmployeeStudentId,
                                                 std_code = db.employee.Where(c => c.EmployeeId == r.EmployeeStudentId).FirstOrDefault().employee_code,
                                                 course_id = s.CourseId,
                                                 course_code = db.organization_program_course.Where(c => c.Id == s.CourseId).FirstOrDefault().CourseCode,
                                                 course_start_time = s.StartTime,
                                                 course_end_time = s.EndTime,
                                                 term_id = db.organization_campus_building_room.Where(r => r.Id == s.RoomId).FirstOrDefault().TerminalId,
                                                 //term_out_id = db.organization_campus_building_room.Where(r => r.Id == s.RoomId).FirstOrDefault().TerminalId,
                                             }).ToList();
*/
