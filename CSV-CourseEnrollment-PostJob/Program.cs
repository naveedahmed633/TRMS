using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Data.OracleClient;

namespace CSV_CourseEnrollment_PostJob
{
    class Program
    {
        public static bool isStaticTTLogAllowed = false;

        static void Main(string[] args)
        {
            int zero = 0, iLogSave = 0, cutOff = 0;
            int count1 = 0, count2 = 0, count3 = 0;

            bool isTTLogAllowed = false;
            string iCSVorDBImport = "0", strORCCMSConnString = "", strFilePath = "", strFileName = "", strUNISRegionID = "2", strLTitleText = "", strStep = "";
            List<int> insertedUserIDs = new List<int>();
            List<int> insertedEmployeIDs = new List<int>();
            List<string> insertedUserCardIDs = new List<string>();
            List<int> insertedUserFingerIDs = new List<int>();

            try
            {
                if (ConfigurationManager.AppSettings["CSVorDBImport"] != null && ConfigurationManager.AppSettings["CSVorDBImport"].ToString() != "")
                {
                    iCSVorDBImport = ConfigurationManager.AppSettings["CSVorDBImport"].ToString() ?? "0";
                }

                if (ConfigurationManager.AppSettings["DefaultFilePath"] != null && ConfigurationManager.AppSettings["DefaultFilePath"].ToString() != "")
                {
                    strFilePath = ConfigurationManager.AppSettings["DefaultFilePath"].ToString() ?? "";
                }

                if (ConfigurationManager.AppSettings["DefaultFileName"] != null && ConfigurationManager.AppSettings["DefaultFileName"].ToString() != "")
                {
                    strFileName = ConfigurationManager.AppSettings["DefaultFileName"].ToString() ?? "";
                }

                if (ConfigurationManager.ConnectionStrings["ORC_CMS"] != null && ConfigurationManager.ConnectionStrings["ORC_CMS"].ConnectionString != "")
                {
                    strORCCMSConnString = ConfigurationManager.ConnectionStrings["ORC_CMS"].ConnectionString ?? "";
                }

                ////GetContactByID(strORCCMSConnString, 1);

                if (ConfigurationManager.AppSettings["LogTitleText"] != null && ConfigurationManager.AppSettings["LogTitleText"].ToString() != "")
                {
                    strLTitleText = ConfigurationManager.AppSettings["LogTitleText"].ToString();
                }

                if (ConfigurationManager.AppSettings["LogSave"] != null && ConfigurationManager.AppSettings["LogSave"].ToString() != "")
                {
                    iLogSave = int.Parse(ConfigurationManager.AppSettings["LogSave"].ToString());
                }
                else
                {
                    iLogSave = 999;
                }

                if (ConfigurationManager.AppSettings["IsTTLogAllowed"] != null && ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() != "")
                {
                    isTTLogAllowed = ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() == "1" ? true : false;
                    isStaticTTLogAllowed = ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() == "1" ? true : false;
                }

                if (iCSVorDBImport == "0") //Reading CSV
                {
                    //nothing
                }
                else if (iCSVorDBImport == "1") //reading SQL
                {
                    //nothing
                }
                else //reading Oracle
                {
                    #region 'StudentEnrollment' table - Read CMS-Oracle-DB and Post Data to TRMS-DB

                    //STEP Schedule 1/3: Get CMS Schedule Table Data
                    strStep = "1";
                    Console.WriteLine("TRMS-Import: Class-Enrollment - Get ENR Unsent Data Only");

                    int uResponse = 0, CMS_ID = 0;
                    string TRMS_IDs = "";
                    OrganizationStudentEnrollment enrTRMS = new OrganizationStudentEnrollment();

                    List<DLL.Models.GeneralCalendar> dbGCalendars = new List<DLL.Models.GeneralCalendar>();
                    List<DLL.Models.Employee> dbStudents = new List<DLL.Models.Employee>();
                    List<DLL.Models.OrganizationCampus> dbCampuses = new List<DLL.Models.OrganizationCampus>();
                    List<DLL.Models.OrganizationProgramCourse> dbCourses = new List<DLL.Models.OrganizationProgramCourse>();
                    List<DLL.Models.OrganizationProgram> dbPrograms = new List<DLL.Models.OrganizationProgram>();
                    List<DLL.Models.OrganizationProgramType> dbPTypes = new List<DLL.Models.OrganizationProgramType>();

                    using (var db3 = new DLL.Models.Context())
                    {
                        dbGCalendars = db3.general_calender.ToList();
                        dbStudents = db3.employee.ToList();
                        dbCampuses = db3.organization_campus.ToList();
                        dbCourses = db3.organization_program_course.ToList();
                        dbPrograms = db3.organization_program.ToList();
                        dbPTypes = db3.organization_program_type.ToList();
                    }

                    List<CMSStudentEnrollment> enrCMSList = new List<CMSStudentEnrollment>();

                    enrCMSList = CMS_GetEnrollmentRowUnsyncedYet(strORCCMSConnString);
                    if (enrCMSList != null && enrCMSList.Count > 0)
                    {
                        foreach (var enrCMS in enrCMSList)
                        {
                            DateTime dtCreateDateEnr = DateTime.ParseExact(enrCMS.CreateDateEnr, "MM/dd/yyyy", CultureInfo.CurrentCulture);

                            CMS_ID = 0; TRMS_IDs = ""; uResponse = 0;
                            DLL.Models.OrganizationProgramCourseEnrollment enr = new DLL.Models.OrganizationProgramCourseEnrollment();

                            //get gcalendar id using CMS code
                            var dbGCalendar = dbGCalendars.Where(c => c.year.ToString() == enrCMS.CalendarYear).FirstOrDefault();
                            int iGCalendarID = dbGCalendar != null ? dbGCalendar.GeneralCalendarId : 0;

                            //get campus id using CMS code
                            var dbCampus = dbCampuses.Where(c => c.CampusCode.ToLower() == enrCMS.CampusCode.ToLower()).FirstOrDefault();
                            int iCampusID = dbCampus != null ? dbCampus.Id : 0;

                            //get program id using CMS code
                            var dbProgram = dbPrograms.Where(c => c.ProgramCode.ToLower() == enrCMS.ProgramCode.ToLower()).FirstOrDefault();
                            int iProgram = dbProgram != null ? dbProgram.Id : 0;

                            //get student id using CMS code
                            var dbStudent = dbStudents.Where(c => c.employee_code.ToLower() == enrCMS.StudentCode.ToLower()).FirstOrDefault();
                            int iStudentID = dbStudent != null ? dbStudent.EmployeeId : 0;

                            //get course id using CMS code
                            var dbCourse = dbCourses.Where(c => c.CourseCode.ToLower() == enrCMS.CourseCode.ToLower()).FirstOrDefault();
                            int iCourseID = dbCourse != null ? dbCourse.Id : 0;

                            //get program type id using CMS code
                            var dbPType = dbPTypes.Where(c => c.ProgramTypeName.ToLower() == enrCMS.EnrProgramType.ToLower()).FirstOrDefault();
                            int iPTypeID = dbPType != null ? dbPType.Id : 0;

                            CMS_ID = enrCMS.Id;
                            enr.GeneralCalendarId = iGCalendarID;
                            enr.CampusId = iCampusID;
                            enr.ProgramId = iProgram;
                            enr.EmployeeStudentId = iStudentID;
                            enr.ProgramCourseId = iCourseID;
                            enr.EnrollmentTitle = enrCMS.EnrTitle;
                            enr.EnrolledProgramTypeId = iPTypeID;
                            enr.EnrolledProgramTypeNumber = enrCMS.EnrProgramTypeNumber;
                            enr.IsCourseFailed = enrCMS.IsCourseFailed == "Y" ? true : false;
                            enr.CreateDateEnr = dtCreateDateEnr;

                            using (var db2 = new DLL.Models.Context())
                            {
                                db2.organization_program_course_enrollment.Add(enr);

                                db2.SaveChanges();
                                TRMS_IDs += enr.Id;
                            }

                            //update status
                            uResponse = CMS_UpdateEnrollmentRowSyncedNow(strORCCMSConnString, CMS_ID, TRMS_IDs);
                            if (uResponse > 0)
                            {
                                Console.WriteLine("TRMS-Import: Student-Enrollment - CMS IDs and Sync UPDATED");
                            }

                        }//for ended             
                    }
                    else
                    {
                        Console.WriteLine("TRMS-Import: NO Data Found");
                    }

                    #endregion
                }

                //log message to process completed
                if (isTTLogAllowed)
                {
                    using (var db3 = new DLL.Models.Context())
                    {
                        db3.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "TRMS-Import: Completed",
                            LogDateTime = DateTime.Now,
                            LogMessageText = "TRMS-Import: All Steps Completed"
                        });
                        db3.SaveChanges();
                    }
                }

                Console.WriteLine("TRMS-Import: ALL Steps Completed!");
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

                //log the message execption
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "TRMS-Import: Exception " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "TRMS-Import: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText
                        });

                        db.SaveChanges();
                        Console.WriteLine("TRMS-Import: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText);
                    }
                }
            }

            Console.WriteLine("\n\n\nTRMS-Import: EXIT Now...");
            //Console.ReadKey();
        }

        ///////////////////// Oracle using Methods //////////////////////

        public static List<CMSStudentEnrollment> CMS_GetEnrollmentRowUnsyncedYet(string conString)
        {
            List<CMSStudentEnrollment> enrCMSList = new List<CMSStudentEnrollment>();

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString; // "context connection=true";
            con.Open();

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "select * from intr_cms_student_enrollments order by id";
            //cmd.Parameters.Add(":1", OracleDbType.Int32, 1, ParameterDirection.Input);
            //cmd.Parameters.Add(":1", OracleDbType.Int32, ca_id, ParameterDirection.Input);

            // get a data reader
            OracleDataReader _SqlDataReader = cmd.ExecuteReader();

            // get the country name from the data reader
            if (_SqlDataReader != null && _SqlDataReader.HasRows)
            {
                // We have data in SqlDataReader, lets loop through and display the data
                while (_SqlDataReader.Read())
                {
                    CMSStudentEnrollment enrCMS = new CMSStudentEnrollment();

                    enrCMS.Id = int.Parse(_SqlDataReader["ID"].ToString());
                    enrCMS.CalendarYear = _SqlDataReader["CalendarYear"].ToString() ?? "";
                    enrCMS.CampusCode = _SqlDataReader["CampusCode"].ToString() ?? "";
                    enrCMS.ProgramCode = _SqlDataReader["ProgramCode"].ToString() ?? "";
                    enrCMS.StudentCode = _SqlDataReader["StudentCode"].ToString() ?? "";
                    enrCMS.CourseCode = _SqlDataReader["CourseCode"].ToString() ?? "";
                    enrCMS.EnrTitle = _SqlDataReader["EnrTitle"].ToString() ?? "";
                    enrCMS.EnrProgramType = _SqlDataReader["EnrProgramType"].ToString() ?? "";
                    enrCMS.EnrProgramTypeNumber = int.Parse(_SqlDataReader["EnrProgramTypeNumber"].ToString());
                    enrCMS.IsCourseFailed = _SqlDataReader["IsCourseFailed"].ToString() ?? "";
                    enrCMS.CreateDateEnr = _SqlDataReader["CreateDateEnr"].ToString() ?? "";
                    enrCMS.TRMS_IDs = _SqlDataReader["TRMS_IDS"].ToString() ?? "";
                    enrCMS.TRMS_Synced = _SqlDataReader["TRMS_SYNCED"].ToString() ?? "";

                    enrCMSList.Add(enrCMS);
                }
            }

            // clean up objects
            _SqlDataReader.Close();

            // clean up objects
            cmd.Dispose();
            con.Dispose();

            // Return the counter
            return enrCMSList;
        }

        public static int CMS_UpdateEnrollmentRowSyncedNow(string conString, int cms_id, string trms_ids)
        {
            int iResponse = 0;

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString;
            con.Open();

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "update intr_cms_student_enrollments set trms_ids=:1, trms_synced=1 where id = :2";

            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1"; p01.Value = trms_ids; p01.OracleType = OracleType.VarChar;//cmd.Parameters.Add(":1", OracleType.VarChar, trms_ids);
            cmd.Parameters.Add(p01);

            OracleParameter p02 = new OracleParameter();
            p02.ParameterName = ":2"; p02.Value = cms_id; p02.OracleType = OracleType.Int32;//cmd.Parameters.Add(":2", OracleType.Int32, cms_id);
            cmd.Parameters.Add(p02);

            // get a non query
            iResponse = cmd.ExecuteNonQuery();

            // clean up objects
            cmd.Dispose();
            con.Dispose();

            // Return the counter
            return iResponse;
        }

        //TESTING Methods: Code found from site: https://developer.oracle.com/dotnet/williams-sps.html
        public static string GetContactByID(string conString, int a)
        {
            // used to return the country name
            string CountryName = "";

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString; // "context connection=true";
            con.Open();

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            //cmd.CommandText = "select * from contacts where contact_id = :1";
            cmd.CommandText = "select * from intr_cms_class_schedules where id = 1";
            //cmd.Parameters.Add(":1", OracleType.Int32);

            // get a data reader
            OracleDataReader rdr = cmd.ExecuteReader();

            // get the country name from the data reader
            if (rdr.Read())
            {
                CountryName = rdr.GetString(0);
            }

            // clean up objects
            rdr.Close();
            cmd.Dispose();

            // Return the country name
            return CountryName;
        }

        ////////////////////////////////////////////////////////////////////////////////////

        public static string setSchedue(List<string> csvContents, string user_code)
        {
            try
            {
                using (var db = new DLL.Models.Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 11 values (columns).

                        if (values == null || values.Length < 11)
                            continue;

                        // Remove unwanted " characters.
                        string strDate = values[0].Replace("\"", "");//date
                        string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        if (strProperDate != "")
                        {
                            strDate = strProperDate;
                        }

                        string strStTime = values[1].Replace("\"", "");//time_in
                        string strEnTime = values[2].Replace("\"", "");//time_en
                        string strCampusCode = values[3].Replace("\"", "");//campus
                        string strRoomCode = values[4].Replace("\"", "");//room
                        string strProgramCode = values[5].Replace("\"", "");//program
                        string strPShift = values[6].Replace("\"", "");//shift
                        string strLGroup = values[7].Replace("\"", "");//group
                        string strCourseCode = values[8].Replace("\"", "");//course
                        string strStudyTitle = values[9].Replace("\"", "");//study
                        string strLecturerCode = values[10].Replace("\"", "");//lecturer

                        DateTime dtStartDateTime = Convert.ToDateTime(Convert.ToDateTime(strDate + " " + strStTime).ToString("yyyy-MM-dd HH:mm:00.000"));
                        DateTime dtEndDateTime = Convert.ToDateTime(Convert.ToDateTime(strDate + " " + strEnTime).ToString("yyyy-MM-dd HH:mm:00.000"));

                        //-----------------------------------------------
                        int iCampusId = 0; strCampusCode = strCampusCode.ToLower();
                        var dbCampus = db.organization_campus.Where(r => r.CampusCode.ToLower() == strCampusCode).FirstOrDefault();
                        iCampusId = dbCampus != null ? dbCampus.Id : 0;

                        //-----------------------------------------------
                        int iRoomId = 0; strRoomCode = strRoomCode.ToLower();
                        var dbRoom = db.organization_campus_building_room.Where(r => r.RoomCode != null && r.RoomCode.ToLower() == strRoomCode).FirstOrDefault();
                        iRoomId = dbRoom != null ? dbRoom.Id : 0;

                        //-----------------------------------------------
                        int iProgramId = 0; strProgramCode = strProgramCode.ToLower();
                        if (strProgramCode == "0")
                        {
                            iProgramId = 0;
                        }
                        else
                        {
                            var dbProgram = db.organization_program.Where(r => r.ProgramCode.ToLower() == strProgramCode).FirstOrDefault();
                            iProgramId = dbProgram != null ? dbProgram.Id : 0;
                        }

                        //-----------------------------------------------
                        int iPShiftId = 0; strPShift = strPShift.ToLower();
                        var dbProgramShift = db.organization_program_shift.Where(r => r.ProgramShiftName.ToLower() == strPShift).FirstOrDefault();
                        iPShiftId = dbProgramShift != null ? dbProgramShift.Id : 1;


                        //-----------------------------------------------
                        int iLGroupId = 0; strLGroup = strLGroup.ToLower();
                        if (strLGroup == "0")
                        {
                            iLGroupId = 0;
                        }
                        else
                        {
                            var dbLectureGroup = db.region.Where(r => r.name != null && r.name.ToLower() == strLGroup).FirstOrDefault();
                            iLGroupId = dbLectureGroup != null ? dbLectureGroup.RegionId : 0;
                        }

                        //-----------------------------------------------
                        int iCourseId = 0; strCourseCode = strCourseCode.ToLower();
                        if (strCourseCode == "self study")
                        {
                            iCourseId = -3;
                        }
                        else if (strCourseCode == "seminar")
                        {
                            iCourseId = -2;
                        }
                        else if (strCourseCode == "break")
                        {
                            iCourseId = -1;
                        }
                        else if (strCourseCode == "off")
                        {
                            iCourseId = 0;
                        }
                        else
                        {
                            var dbCourse = db.organization_program_course.Where(r => r.CourseCode != null && r.CourseCode.ToLower() == strCourseCode).FirstOrDefault();
                            iCourseId = dbCourse != null ? dbCourse.Id : 0;
                        }


                        //-----------------------------------------------
                        if (strStudyTitle == "0")
                        {
                            strStudyTitle = "";
                        }

                        //-----------------------------------------------
                        int iEmployeeTeacherId = 0;
                        if (strLecturerCode == "0")
                        {
                            iEmployeeTeacherId = 0;
                        }
                        else
                        {
                            var dbEmployeeTeacher = db.employee.Where(r => r.employee_code == strLecturerCode).FirstOrDefault();
                            iEmployeeTeacherId = dbEmployeeTeacher != null ? dbEmployeeTeacher.EmployeeId : 0;
                        }


                        // continue if its the first row (CSV Header line).
                        if (strDate == "date" || strStTime == "st_time")
                        {
                            continue;
                        }

                        // Get the existing schedule by room and date 
                        DLL.Models.OrganizationCampusRoomCourseSchedule schedule = db.organization_campus_room_course_schedule
                            .Where(m => m.CampusId == iCampusId && m.RoomId == iRoomId && m.ProgramId == iProgramId && m.ShiftId == iPShiftId &&
                                            m.LectureGroupId == iLGroupId && m.CourseId == iCourseId && m.EmployeeTeacherId == iEmployeeTeacherId
                                                    && m.StartTime.Equals(dtStartDateTime) && m.EndTime.Equals(dtEndDateTime)
                                ).FirstOrDefault();

                        if (schedule != null)//existing schedule
                        {
                            schedule.StartTime = dtStartDateTime;
                            schedule.EndTime = dtEndDateTime;
                            schedule.CampusId = iCampusId;
                            schedule.RoomId = iRoomId;
                            schedule.ProgramId = iProgramId;
                            schedule.ShiftId = iPShiftId;
                            schedule.LectureGroupId = iLGroupId;
                            schedule.CourseId = iCourseId;
                            schedule.StudyTitle = strStudyTitle;
                            schedule.EmployeeTeacherId = iEmployeeTeacherId;
                            schedule.CreateDateSch = DateTime.Now;

                            db.SaveChanges();

                            //TimeTune.AuditTrail.update("{\"*id\" : \"" + schedule.Id.ToString() + "\"}", "OrganizationCampusRoomCourseSchedule", user_code);
                        }
                        else //new schedule
                        {
                            DLL.Models.OrganizationCampusRoomCourseSchedule sch = new DLL.Models.OrganizationCampusRoomCourseSchedule()
                            {
                                StartTime = dtStartDateTime,
                                EndTime = dtEndDateTime,
                                CampusId = iCampusId,
                                RoomId = iRoomId,
                                ProgramId = iProgramId,
                                ShiftId = iPShiftId,
                                LectureGroupId = iLGroupId,
                                CourseId = iCourseId,
                                StudyTitle = strStudyTitle,
                                EmployeeTeacherId = iEmployeeTeacherId,
                                CreateDateSch = DateTime.Now
                            };

                            db.organization_campus_room_course_schedule.Add(sch);
                            db.SaveChanges();

                            //TimeTune.AuditTrail.insert("{\"*id\" : \"" + sch.Id.ToString() + "\"}", "OrganizationCampusRoomCourseSchedule", user_code);
                        }
                    }

                    return "Successful";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }


        private static bool ValidateDate(string strDate)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (!DateTime.TryParse(strProperDate, out dtTest))
            {
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateNOTPastDate(string strDate)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (dtTest.Date < DateTime.Now.Date)
            {
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateTime(string strDate, string strTime)
        {
            bool isValid = true;
            DateTime dtTest = DateTime.Now;

            string strProperDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (!DateTime.TryParse(strProperDate + " " + strTime, out dtTest))
            {
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateCampusCode(string strCampusCode)
        {
            bool isValid = false;

            isValid = validateCampusCode(strCampusCode);

            return isValid;
        }

        private static bool ValidateCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            isValid = validateCampusCodeAllowed(bGVIsSuperHRRole, iCampusID, strCampusCode);

            return isValid;
        }

        private static bool ValidateRoomCode(string strCampusCode, string strRoomCode)
        {
            bool isValid = false;

            isValid = validateRoomCode(strCampusCode, strRoomCode);

            return isValid;
        }

        private static bool ValidateProgramCode(string strProgramCode)
        {
            bool isValid = false;

            isValid = validateProgramCode(strProgramCode);

            return isValid;
        }

        private static bool ValidateCourseCode(string strCourseCode)
        {
            bool isValid = false;

            strCourseCode = strCourseCode.ToLower();

            if (strCourseCode == "self study")
            {
                isValid = true;
            }
            else if (strCourseCode == "seminar")
            {
                isValid = true;
            }
            else if (strCourseCode == "break")
            {
                isValid = true;
            }
            else if (strCourseCode == "off")
            {
                isValid = true;
            }
            else
            {
                isValid = validateCourseCode(strCourseCode);
            }

            return isValid;
        }



        public static bool validateCampusCode(string strCampusCode)
        {
            bool isValid = false;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCampusCodeAllowed(bool bGVIsSuperHRRole, int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    if (bGVIsSuperHRRole)
                    {
                        isValid = true;
                    }
                    else
                    {
                        strCampusCode = strCampusCode.ToLower();

                        var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                        var dbEmployee = db.employee.Where(c => c.campus_id == iCampusID).FirstOrDefault();

                        if (dbCampus != null && dbEmployee != null)
                        {
                            if (dbCampus.Id == dbEmployee.campus_id)
                                isValid = true;
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateRoomCode(string strCampusCode, string strRoomCode)
        {
            bool isValid = false;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();
                    strRoomCode = strRoomCode.ToLower();

                    var dbCampusRoom = (from r in db.organization_campus_building_room
                                        join b in db.organization_campus_building on r.BuildingId equals b.Id
                                        join c in db.organization_campus on b.CampusId equals c.Id
                                        where c.CampusCode.ToLower() == strCampusCode && r.RoomCode.ToLower() == strRoomCode
                                        select new { cr_code = c.CampusCode + "-" + r.RoomCode }).FirstOrDefault();

                    //var dbCampusRoom = db.organization_campus_building_room.Where(c => c.RoomCode == strRoomCode).FirstOrDefault();
                    if (dbCampusRoom != null)
                    {
                        if (dbCampusRoom.cr_code != null && dbCampusRoom.cr_code != "")
                            isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }


        public static bool validateProgramCode(string strProgramCode)
        {
            bool isValid = false;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    strProgramCode = strProgramCode.ToLower();

                    var dbProgram = db.organization_program.Where(c => c.ProgramCode == strProgramCode).FirstOrDefault();
                    if (dbProgram != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }



        public static bool validateCourseCode(string strCourseCode)
        {
            bool isValid = false;

            using (var db = new DLL.Models.Context())
            {
                try
                {
                    strCourseCode = strCourseCode.ToLower();

                    var dbCourse = db.organization_program_course.Where(c => c.CourseCode == strCourseCode).FirstOrDefault();
                    if (dbCourse != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }



        ///////////////////// Logging Methid ////////////////////////////
        public static void WriteToConsoleAudiLog(string strLTitleText)
        {
            if (isStaticTTLogAllowed)
            {
                using (var dbTT = new DLL.Models.Context())
                {
                    dbTT.log_message.Add(new DLL.Models.LogMessage
                    {
                        LogTitle = "TRMS-Import:",
                        LogDateTime = DateTime.Now,
                        LogMessageText = "Message: " + strLTitleText
                    });
                    dbTT.SaveChanges();

                    Console.WriteLine("TRMS-Import: Message: " + strLTitleText);
                }
            }
        }

    }
}


/*
 pdborcl


orcl
Resco1234



Enter user-name: sys as sysdba
Enter password:

SQL> SHOW con_name;

CON_NAME
------------------------------
CDB$ROOT

SQL> ALTER SESSION SET CONTAINER = pdborcl;

Session altered.

SQL> SHOW con_name;

CON_NAME
------------------------------
PDBORCL


SQL> ALTER DATABASE OPEN;

Database altered.


SQL> CREATE USER OT IDENTIFIED BY Orcl1234;

User created.

SQL> GRANT CONNECT, RESOURCE, DBA TO OT;

Grant succeeded.

SQL> CONNECT ot@pdborcl
Enter password:
Connected.
Note that OT user only exists in the PDBORCL pluggable database, therefore, you must explicitly specify the username as ot@pdborcl in the CONNECT command.

Creating database tables

SQL>@c:\dbsample\ot_schema.sql
Once the statement completes, you can verify whether the tables were created successfully or not by listing the tables owned by the OT user. The following is the statement to do so.

SQL> SELECT table_name FROM user_tables ORDER BY table_name;

TABLE_NAME
--------------------------------------------------------------------------------
CONTACTS
COUNTRIES
CUSTOMERS
EMPLOYEES
INVENTORIES
LOCATIONS
ORDERS
ORDER_ITEMS
PRODUCTS
PRODUCT_CATEGORIES
REGIONS
WAREHOUSES

12 rows selected.

SQL>@c:\dbsample\ot_data.sql


SQL> SELECT COUNT(*) FROM contacts;

  COUNT(*)
----------
       319

////////////////////////////////////////////////////////////////////////
CREATE TABLE INTR_TRMS_Course_Attendances
  (
    id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
    ca_id NUMBER(12,0) NOT NULL,
    schedule_date_dt    TIMESTAMP(2) NULL,
    schedule_date       VARCHAR2( 50 ) NULL,
    student_id          NUMBER(12,0) NULL,
    student_code        VARCHAR2( 20 ),
    student_name        VARCHAR2( 50 ),
    student_group_id    NUMBER(12,0) NULL,
    student_group       VARCHAR2( 50 ),
    student_pshift_id   NUMBER(12,0) NULL,
    student_pshift      VARCHAR2( 50 ) NULL,
    course_id           NUMBER(12,0) NULL, 
    course_code         VARCHAR2( 50 ) NULL,
    course_title        VARCHAR2( 255 ) NULL,
    time_in_dt          TIMESTAMP(2) NULL,
    time_in             VARCHAR2( 50 ) NULL,
    status_in           VARCHAR2( 20 ) NULL,
    time_out_dt         TIMESTAMP(2) NULL,
    time_out            VARCHAR2( 50 ) NULL, 
    status_out          VARCHAR2( 20 ) NULL,
    final_remarks       VARCHAR2( 20 ) NULL,
    terminal_in_id      NUMBER(12,0) NULL,  
    terminal_in         VARCHAR2( 255 ) NULL,
    terminal_out_id     NUMBER(12,0) NULL, 
    terminal_out        VARCHAR2( 255 ) NULL,
    course_time_start_dt      TIMESTAMP(2) NULL,
    course_time_start   VARCHAR2( 20 ) NULL,
    course_time_end_dt  TIMESTAMP(2) NULL,
    course_time_end     VARCHAR2( 20 ) NULL,
    is_synced           NUMBER(4,0) NULL
  );
  



/////////////////////////////////////////////////////////////////

create or replace 
procedure sp_manage_course_attendance
(
    p_ca_id in number default 0 
  , p_schedule_date_dt in varchar2  
  , p_schedule_date in varchar2
  , p_student_id in number default 0
  , p_student_code in varchar2  
  , p_student_name in varchar2  
  , p_student_group_id in number default 0 
  , p_student_group in varchar2  
  , p_student_pshift_id in number default 0 
  , p_student_pshift in varchar2  
  , p_course_id in number  
  , p_course_code in varchar2  
  , p_course_title in varchar2  
  , p_time_in_dt in varchar2  
  , p_time_in in varchar2  
  , p_status_in in varchar2  
  , p_time_out_dt in varchar2  
  , p_time_out in varchar2  
  , p_status_out in varchar2  
  , p_final_remarks in varchar2  
  , p_terminal_in_id in number default 0 
  , p_terminal_in in varchar2  
  , p_terminal_out_id in number default 0 
  , p_terminal_out in varchar2  
  , p_course_time_start_dt in varchar2  
  , p_course_time_start in varchar2  
  , p_course_time_end_dt in varchar2  
  , p_course_time_end in varchar2 
  , p_is_synced in number default 0 
) as 
BEGIN

  DECLARE  
    n_schedule_dt Timestamp(2) NULL := CURRENT_TIMEStamp;
    n_time_in_dt Timestamp(2) NULL := CURRENT_TIMEStamp;
    n_time_out_dt Timestamp(2) NULL := CURRENT_TIMEStamp;
    n_course_time_start_dt Timestamp(2) NULL := CURRENT_TIMEStamp;
    n_course_time_end_dt Timestamp(2) NULL := CURRENT_TIMEStamp;
    
  BEGIN
    n_schedule_dt := to_timestamp(p_schedule_date_dt,'DD-Mon-RR HH24:MI:SS.FF');
    n_time_in_dt := to_timestamp(p_time_in_dt,'DD-Mon-RR HH24:MI:SS.FF');
    n_time_out_dt := to_timestamp(p_time_out_dt,'DD-Mon-RR HH24:MI:SS.FF');
    n_course_time_start_dt := to_timestamp(p_course_time_start_dt,'DD-Mon-RR HH24:MI:SS.FF');
    n_course_time_end_dt := to_timestamp(p_course_time_end_dt,'DD-Mon-RR HH24:MI:SS.FF');
    
    INSERT INTO INTR_TRMS_COURSE_ATTENDANCES 
      (ca_id,schedule_date_dt,schedule_date,student_id,student_code,student_name,student_group_id,student_group,student_pshift_id,student_pshift,course_id,course_code,course_title,
      time_in_dt,time_in,time_out_dt,time_out,
      status_in,status_out,final_remarks,terminal_in_id,terminal_in,terminal_out_id,terminal_out,
      course_time_start_dt,course_time_start,course_time_end_dt,course_time_end,is_synced)
    VALUES
      (p_ca_id,n_schedule_dt,p_schedule_date,p_student_id,p_student_code,p_student_name,p_student_group_id,p_student_group,p_student_pshift_id,p_student_pshift,p_course_id,p_course_code,p_course_title,
      n_time_in_dt,p_time_in,n_time_out_dt,p_time_out,
      p_status_in,p_status_out,p_final_remarks,p_terminal_in_id,p_terminal_in,p_terminal_out_id,p_terminal_out,
      n_course_time_start_dt,p_course_time_start,n_course_time_end_dt,p_course_time_end,p_is_synced);
      
    END;
    
END sp_manage_course_attendance;

/////////////////////////////////////////////////////////////////






/////////////////////////////////////////////////////////////////

CREATE TABLE INTR_CMS_Class_Schedules
(
  Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
  Course_Id 	  VARCHAR2( 50 ) NULL,
  Class_Nbr	    VARCHAR2( 50 ) NULL,
  Subject		    VARCHAR2( 255 ) NULL,
  Career		    VARCHAR2( 50 ) NULL,
  Descr		      VARCHAR2( 255 ) NULL,
  Catalog		    VARCHAR2( 50 ) NULL,
  Acad_Group	  VARCHAR2( 50 ) NULL,
  Term 		      VARCHAR2( 50 ) NULL,
  Campus		    VARCHAR2( 50 ) NULL,
  Institution		VARCHAR2( 100 ) NULL,
  Mtg_Start		  VARCHAR2( 50 ) NULL,
  Mtg_End		    VARCHAR2( 50 ) NULL,
  Mon		        VARCHAR2( 1 ) NULL,			--Y/N
  Tues		      VARCHAR2( 1 ) NULL,			--Y/N
  Wed		        VARCHAR2( 1 ) NULL,			--Y/N
  Thurs		      VARCHAR2( 1 ) NULL,			--Y/N
  Fri		        VARCHAR2( 1 ) NULL,			--Y/N
  Sat		        VARCHAR2( 1 ) NULL,			--Y/N
  Sun		        VARCHAR2( 1 ) NULL,			--Y/N
  Start_Date	  VARCHAR2( 50 ) NULL,
  End_Date		  VARCHAR2( 50 ) NULL,
  TRMS_Ids		  VARCHAR2( 255 ) NULL,
  TRMS_Synced     NUMBER(4,0) NULL				--0/1
);
  



/////////////////////////////////////////////////////////////////

INSERT INTO INTR_CMS_Class_Schedules
(
  --Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
  Course_Id,
  Class_Nbr,
  Subject,
  Career,
  Descr,
  Catalog,
  Acad_Group,
  Term ,
  Campus,
  Institution,
  Mtg_Start	,
  Mtg_End,
  Mon,			--Y/N
  Tues,			--Y/N
  Wed,			--Y/N
  Thurs,			--Y/N
  Fri,			--Y/N
  Sat,			--Y/N
  Sun,			--Y/N
  Start_Date,
  End_Date,
TRMS_Ids,
  TRMS_Synced				--0/1
)
VALUES
(
  --Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
  '00201',
  '1016',
  'HEMNGTSC',
  'UGRD',
  'INFORMATIONS SYSTEM',
  'BDA',
  'ihm',
  '1808',
  'DMC',
  'DUHSP',
  '09:00:00 AM',
  '10:30:00 AM',
  'Y',			                        --Y/N
  'Y',			                        --Y/N
  'N',			                        --Y/N
  'Y',			                        --Y/N
  'Y',			                        --Y/N
  'N',			                        --Y/N
  'N',			                        --Y/N
  '08/03/2020',
  '08/08/2020',
 '0',	
  0
);

-----

INSERT INTO INTR_CMS_Class_Schedules
(
  --Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
  Course_Id,
  Class_Nbr,
  Subject,
  Career,
  Descr,
  Catalog,
  Acad_Group,
  Term ,
  Campus,
  Institution,
  Mtg_Start	,
  Mtg_End,
  Mon,			--Y/N
  Tues,			--Y/N
  Wed,			--Y/N
  Thurs,			--Y/N
  Fri,			--Y/N
  Sat,			--Y/N
  Sun,			--Y/N
  Start_Date,
  End_Date,
  TRMS_Ids,
  TRMS_Synced				--0/1
)
VALUES
(
  --Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
  '00202',
  '1019',
  'HEMNGTSC',
  'UGRD',
  'INFORMATIONS SYSTEM',
  'MBBS',
  'ihm',
  '1809',
  'DMC',
  'DUHSP',
  '09:00:00 AM',
  '10:30:00 AM',
  'Y',			                        --Y/N
  'N',			                        --Y/N
  'Y',			                        --Y/N
  'N',			                        --Y/N
  'Y',			                        --Y/N
  'N',			                        --Y/N
  'N',			                        --Y/N
  '08/03/2020',
  '08/08/2020',
   '0',	
  0
);

----


INSERT INTO INTR_CMS_Class_Schedules
(
  --Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
  Course_Id,
  Class_Nbr,
  Subject,
  Career,
  Descr,
  Catalog,
  Acad_Group,
  Term ,
  Campus,
  Institution,
  Mtg_Start	,
  Mtg_End,
  Mon,			--Y/N
  Tues,			--Y/N
  Wed,			--Y/N
  Thurs,			--Y/N
  Fri,			--Y/N
  Sat,			--Y/N
  Sun,			--Y/N
  Start_Date,
  End_Date,
TRMS_Ids,
 TRMS_Synced				--0/1
)
VALUES
(
  --Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
  '00203',
  '1020',
  'HEMNGTSC',
  'UGRD',
  'INFORMATIONS SYSTEM',
  'BDA',
  'ihm',
  '1810',
  'DMC',
  'DUHSP',
  '10:00:00 AM',
  '11:30:00 AM',
  'N',			                        --Y/N
  'Y',			                        --Y/N
  'Y',			                        --Y/N
  'Y',			                        --Y/N
  'N',			                        --Y/N
  'N',			                        --Y/N
  'N',			                        --Y/N
  '08/03/2020',
  '08/08/2020',
0,	
  0
);

/////////////////////////////////////////////////////////////////




/////////////////////////////////////////////////////////////////


CREATE TABLE INTR_CMS_Student_Enrollments
  (
          Id NUMBER GENERATED BY DEFAULT AS IDENTITY START WITH 1 PRIMARY KEY,
	CalendarYear 	        VARCHAR2( 4 ) NULL,
        	CampusCode 	        VARCHAR2( 50 ) NULL,
        	ProgramCode	        VARCHAR2( 50 ) NULL,
        	StudentCode   		  VARCHAR2( 50 ) NULL,
        	CourseCode		      VARCHAR2( 50 ) NULL,
        	EnrTitle		        VARCHAR2( 100 ) NULL,
          EnrProgramType	    VARCHAR2( 20 ) NULL,       --Count of Year/Semester/Month
        	EnrProgramTypeNumber	NUMBER(4,0) NULL,  --Count of Year/Semester/Month
        	IsCourseFailed		  VARCHAR2( 1 ) NULL,       --Y/N
        	CreateDateEnr		    VARCHAR2( 50 ) NULL,
	TRMS_Ids		  VARCHAR2( 255 ) NULL,
          TRMS_Synced           NUMBER(4,0) NULL        --0/1
  );



/////////////////////////////////////////////////////////////////


INSERT INTO INTR_CMS_Student_Enrollments
  (
          CalendarYear,
        	CampusCode,
        	ProgramCode,
        	StudentCode,
        	CourseCode,
        	EnrTitle,
          EnrProgramType,
        	EnrProgramTypeNumber,
        	IsCourseFailed,
        	CreateDateEnr,
          TRMS_Ids,
          TRMS_Synced
  )
  VALUES
    (          '2020',
        	'DMC',
        	'MBBS',
        	'003118',
        	'00201',
        	'MBBS',
          'Year',       --S/Y/M
        	1,  --Count of S/Y/M
        	'N',       --Y/N
        	'08/04/2020',
          '0',
          '0'        --0/1
  );


//////////////////////////////////////////////////////////////////////////




INSERT INTO INTR_CMS_Student_Enrollments
  (
          CalendarYear,
        	CampusCode,
        	ProgramCode,
        	StudentCode,
        	CourseCode,
        	EnrTitle,
          EnrProgramType,
        	EnrProgramTypeNumber,
        	IsCourseFailed,
        	CreateDateEnr,
          TRMS_Ids,
          TRMS_Synced
  )
  VALUES
    (          '2020',
        	'DDC',
        	'BDS',
        	'003118',
        	'00201',
        	'BDS',
          'Semester',       --S/Y/M
        	2,  --Count of S/Y/M
        	'N',       --Y/N
        	'08/07/2020',
          '0',
          '0'        --0/1
  );









 */
