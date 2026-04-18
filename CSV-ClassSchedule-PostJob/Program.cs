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
using DLL.Models;
//using Oracle.DataAccess.Client;

namespace CSV_ClassSchedule_PostJob
{
    class Program
    {
        public static bool isStaticTTLogAllowed = false;

        static void Main(string[] args)
        {
            int zero = 0, iLogSave = 0, cutOff = 0;
            int count1 = 0, count2 = 0, count3 = 0;

            bool isTTLogAllowed = false;
            string strOracleSQLQuery = "", iCSVorDBImport = "0", strORCCMSConnString = "", strFilePath = "", strFileName = "", strUNISRegionID = "2", strLTitleText = "", strStep = "";
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

                if (ConfigurationManager.AppSettings["OracleSQLQuery"] != null && ConfigurationManager.AppSettings["OracleSQLQuery"].ToString() != "")
                {
                    strOracleSQLQuery = ConfigurationManager.AppSettings["OracleSQLQuery"].ToString() ?? "";
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

                //Console.WriteLine("Step 111"); Console.ReadKey();

                if (iCSVorDBImport == "0") //Reading CSV
                {
                    #region 'ClassSchedule' table - Read CMS-CSV-Files (placed in Local Folder) and Post Data to TRMS-DB

                    //STEP Schedule 1/3: Get UNIS Schedule Table Data
                    strStep = "1";
                    Console.WriteLine("TRMS-Import: Class-Schedules - Get SCH Unsent Data Only");

                    string result = "";

                    HttpPostedFileBase file = null;

                    if (file != null && file.ContentLength > 0)
                    {
                        string FileName = Path.GetFileName(file.FileName);
                        string FileExtension = Path.GetExtension(file.FileName).ToLower();

                        if (FileExtension != ".csv")
                        {
                            //return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "Invalid-File-Format" });
                        }
                        else
                        {
                            //try
                            //{
                            string path = Path.Combine(strFilePath, DateTime.Now.ToString("yyyyMMddHHmmss") + "_schedule.csv");
                            file.SaveAs(path);

                            int counter = 0;
                            List<string> content = new List<string>();
                            string strReadLine = ""; bool invalidDate = false, invalidTime = false, invalidCampusCode = false, invalidCampusCodeAllowed = false, invalidRoomCode = false, invalidProgramCode = false, invalidCourseCode = false, invalidLecturerCode = false, invalidCols = false;

                            //////////////// Check if 2nd Row is THERE or NOT with data? /////////////////////
                            bool isDataRowFound = false; int a = 0;
                            using (StreamReader sr = new StreamReader(path))
                            {
                                while (sr.Peek() >= 0)
                                {
                                    strReadLine = sr.ReadLine();
                                    a++;

                                    if (a == 2)
                                    {
                                        isDataRowFound = true;
                                        break;
                                    }
                                }
                            }

                            if (!isDataRowFound)
                            {
                                //return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "No Data Found in the Sheet" });
                            }
                            /////////////////////////////////////////////////////////////////////////

                            using (StreamReader sr = new StreamReader(path))
                            {
                                while (sr.Peek() >= 0)
                                {
                                    strReadLine = sr.ReadLine();
                                    strReadLine = strReadLine.TrimEnd(',');
                                    strReadLine = strReadLine.Replace("<", "").Replace(">", "");//remove <> from Employee Code
                                    strReadLine = strReadLine.Replace("\"", "");
                                    strReadLine = strReadLine.TrimEnd(',');

                                    string new_code = "";
                                    string[] ecode_dt = strReadLine.Split(',');
                                    if (ecode_dt.Length == 11)
                                    {
                                        counter++;

                                        if (ecode_dt[0].ToLower().Contains("date") || ecode_dt[1].ToLower().Contains("st_time"))
                                        {
                                            continue;
                                        }

                                        //if (ecode_dt[0].Length == 6)
                                        //{
                                        //    new_code = ecode_dt[0];
                                        //}
                                        //else
                                        //{
                                        //    if (ecode_dt[0].Length == 1)
                                        //        new_code = "00000" + ecode_dt[0];
                                        //    else if (ecode_dt[0].Length == 2)
                                        //        new_code = "0000" + ecode_dt[0];
                                        //    else if (ecode_dt[0].Length == 3)
                                        //        new_code = "000" + ecode_dt[0];
                                        //    else if (ecode_dt[0].Length == 4)
                                        //        new_code = "00" + ecode_dt[0];
                                        //    else if (ecode_dt[0].Length == 5)
                                        //        new_code = "0" + ecode_dt[0];
                                        //    else
                                        //        new_code = "";
                                        //}

                                        ////validate employee code
                                        //if (!ValidateEmployeeCode(new_code))
                                        //{
                                        //    invalidEcode = true;
                                        //    result = "Invalid User Code Found at Row-" + counter;
                                        //    break;
                                        //}

                                        ////validate First Name
                                        //if (!ValidateName(ecode_dt[1]))
                                        //{
                                        //    invalidFName = true;
                                        //    result = "Invalid First Name Found at Row-" + counter;
                                        //    break;
                                        //}

                                        ////validate Last Name
                                        //if (!ValidateName(ecode_dt[2]))
                                        //{
                                        //    invalidLName = true;
                                        //    result = "Invalid Last Name Found at Row-" + counter;
                                        //    break;
                                        //}

                                        //validate Date
                                        if (!ValidateDate(ecode_dt[0]))
                                        {
                                            invalidDate = true;
                                            result = "Invalid Date Found at Row-" + counter;
                                            break;
                                        }

                                        if (!ValidateNOTPastDate(ecode_dt[0]))
                                        {
                                            invalidDate = true;
                                            result = "Past Date NOT Allowed - Found at Row-" + counter;
                                            break;
                                        }

                                        if (!ValidateTime(ecode_dt[0], ecode_dt[1]))
                                        {
                                            invalidTime = true;
                                            result = "Invalid Time Start Found at Row-" + counter;
                                            break;
                                        }

                                        if (!ValidateTime(ecode_dt[0], ecode_dt[2]))
                                        {
                                            invalidTime = true;
                                            result = "Invalid Time End Found at Row-" + counter;
                                            break;
                                        }

                                        if (!ValidateCampusCode(ecode_dt[3]))
                                        {
                                            invalidCampusCode = true;
                                            result = "Invalid Campus-Code Found at Row-" + counter;
                                            break;
                                        }

                                        //bool bGVIsSuperHRRole = false; bGVIsSuperHRRole = ViewModel.GlobalVariables.GV_RoleIsSuperHR;
                                        //int iGVCampusID = 0; iGVCampusID = int.Parse(ViewModel.GlobalVariables.GV_EmployeeCampusID);
                                        //if (!ValidateCampusCodeAllowed(bGVIsSuperHRRole, iGVCampusID, ecode_dt[3]))
                                        //{
                                        //    invalidCampusCodeAllowed = true;
                                        //    result = "NOT Allowed Campus-Code Found at Row-" + counter;
                                        //    break;
                                        //}

                                        if (!ValidateRoomCode(ecode_dt[3], ecode_dt[4]))
                                        {
                                            invalidRoomCode = true;
                                            result = "Invalid Room-Code Found at Row-" + counter;
                                            break;
                                        }

                                        if (!ValidateProgramCode(ecode_dt[5]))
                                        {
                                            invalidProgramCode = true;
                                            result = "Invalid Program-Code Found at Row-" + counter;
                                            break;
                                        }

                                        if (!ValidateCourseCode(ecode_dt[8]))
                                        {
                                            invalidCourseCode = true;
                                            result = "Invalid Course-Code Found at Row-" + counter;
                                            break;
                                        }

                                        //if (!ValidateLecturerCode(ecode_dt[3], ecode_dt[10]))
                                        //{
                                        //    invalidLecturerCode = true;
                                        //    result = "Invalid Lecturer-Code Found at Row-" + counter;
                                        //    break;
                                        //}
                                    }
                                    else
                                    {
                                        invalidCols = true;
                                        result = "Invalid Col(s) Found";
                                        break;
                                    }

                                    //iterate to replace EmployeeCode only having 0 as prefix
                                    //for (int i = 0; i < ecode_dt.Length; i++)
                                    //{
                                    //    if (i == 0)
                                    //    {
                                    //        strReadLine = new_code + ",";
                                    //    }
                                    //    else
                                    //    {
                                    //        strReadLine += ecode_dt[i] + ",";
                                    //    }
                                    //}

                                    //strReadLine = strReadLine.TrimEnd(',');
                                    content.Add(strReadLine);

                                    //restrict to upload if 1000+ rows are found
                                    /*if (counter > 1000)
                                    {
                                        invalidRowsCount = true;
                                        result = "Max 1000 records are allowed be uploaded";
                                        break;
                                    }*/
                                }
                            }

                            if (invalidDate || invalidTime || invalidCampusCode || invalidCampusCodeAllowed || invalidRoomCode || invalidProgramCode || invalidCourseCode || invalidLecturerCode || invalidCols)
                            {
                                //return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = result });
                            }
                            else
                            {
                                result = setSchedue(content, "AUTO");
                            }

                        }

                        if (result == "failed")
                        {
                            //return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "Failed to Update due to Invalid info" });
                        }

                        //return RedirectToAction("ManageOrganizationSchedule", "OrganizationsManagement", new { result = "Successful" });

                        //return JavaScript("displayToastrSuccessfull()");
                        //}
                        //catch (Exception ex)
                        //{
                        //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                        //}
                    }


                    //STEP Schedule 2/3: TimeTune Context from referenced DLL
                    strStep = "2";
                    Console.WriteLine("TRMS-Import: CourseAttendance - INSERT INTO 'DB'");
                    List<string> ca_list = new List<string>();

                    if (ca_list != null && ca_list.Count > 0)
                    {
                        ////////////////////// PROCESSING DATA FOR FILE ////////////////////////////////////////
                        WriteToConsoleAudiLog("TRMS-Import: Data Manipulation - In Progress...");
                        string strCALogs = "";



                        //sw.Close();

                        WriteToConsoleAudiLog("TRMS-Import: Data Manipulation - Completed");


                        ////////////////////////// COPYING FILE ////////////////////////////////////////////
                        string sourcePath = strFilePath + strFileName;
                        string targetPath = "", fileName = "";

                        //orc24 file processing
                        if (strCALogs.Length > 0)
                        {
                            WriteToConsoleAudiLog("TRMS-Import: Writing Data to File - Starting...");

                            using (StreamWriter sw = new StreamWriter(sourcePath))
                            {
                                sw.WriteLine(strCALogs);
                                //sw.WriteLine(string.Join(",", strCALogs));
                            }

                            WriteToConsoleAudiLog("TRMS-Import: Writing Data to File - Completed");

                            //f3Response = 1;
                        }

                        //copy file from this to other location




                        WriteToConsoleAudiLog("TRMS-Import: Copying File...");

                        targetPath = strFilePath;
                        fileName = "" + DateTime.Now.ToString("dd-MMM-yyyy") + ".csv";
                        string destFile = System.IO.Path.Combine(targetPath, fileName);
                        System.IO.File.Copy(sourcePath, destFile, true);

                        //c3Response = 1;
                    }

                    //STEP Schedule: log the message
                    if (isTTLogAllowed)
                    {
                        using (var db = new Context())
                        {
                            db.log_message.Add(new LogMessage
                            {
                                LogTitle = "TRMS-Import: Schedule - Sync " + strLTitleText + "",
                                LogDateTime = DateTime.Now,
                                LogMessageText = "TRMS-Import: Schedule - Sync " + strLTitleText + " - Succeeded! (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + ")"
                            });

                            db.SaveChanges();
                            Console.WriteLine("TRMS-Import: Schedule - Sync " + strLTitleText + " - Message Logged with Success!");
                        }
                    }

                    #endregion
                }
                else if (iCSVorDBImport == "1") //reading SQL
                {
                    //nothing
                }
                else //reading Oracle
                {
                    #region 'ClassSchedule' table - Read CMS-Oracle-DB and Post Data to TRMS-DB

                    //Console.WriteLine("Step 222"); Console.ReadKey();

                    //STEP Schedule 1/3: Get CMS Schedule Table Data
                    strStep = "1";
                    Console.WriteLine("TRMS-Import: Class-Schedules - Get SCH Unsent Data Only");

                    int uResponse = 0, CMS_ID = 0;
                    string TRMS_IDs = "";
                    //OrganizationClassSchedule schTRMS = new OrganizationClassSchedule();

                    List<OrganizationCampus> dbCampuses = new List<OrganizationCampus>();
                    List<OrganizationProgramCourse> dbCourses = new List<OrganizationProgramCourse>();
                    List<OrganizationProgram> dbPrograms = new List<OrganizationProgram>();
                    List<OrganizationCampusBuildingRoom> dbRooms = new List<OrganizationCampusBuildingRoom>();
                    List<OrganizationProgramShift> dbShifts = new List<OrganizationProgramShift>();

                    using (var db3 = new Context())
                    {
                        dbCampuses = db3.organization_campus.ToList();
                        dbCourses = db3.organization_program_course.ToList();
                        dbPrograms = db3.organization_program.ToList();
                        dbRooms = db3.organization_campus_building_room.ToList();
                        dbShifts = db3.organization_program_shift.ToList();
                    }

                    //Console.WriteLine("Step 333"); Console.ReadKey();

                    //get TRMS last-date-entry already recorded
                    var dtLastSch = GetLastRowDateFromTRMS();
                    if (dtLastSch != null)
                    {
                        //get CMS data latest by last-date-entry recorded in TRMS
                        List<CMSClassSchedule> enrCMSList = new List<CMSClassSchedule>();
                        enrCMSList = CMS_GetScheduleRowUnsyncedYet(strORCCMSConnString, strOracleSQLQuery, dtLastSch.Value);
                        if (enrCMSList != null && enrCMSList.Count > 0)
                        {
                            using (var db4 = new Context())
                            {
                                foreach (var enrCMS in enrCMSList)
                                {
                                    DateTime dtSchedule = DateTime.ParseExact(enrCMS.strCreateDateTimeSch, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);

                                    var c = new CMS_ClassSchedule();
                                    c.Course_Id = enrCMS.Course_Id;
                                    c.Class_Nbr = enrCMS.Class_Nbr;
                                    c.Subject = enrCMS.Subject;
                                    c.Career = enrCMS.Career;
                                    c.Descr = enrCMS.Descr;
                                    c.Catalog = enrCMS.Catalog;
                                    c.Acad_Group = enrCMS.Acad_Group;
                                    c.Term = enrCMS.Term;
                                    c.Campus = enrCMS.Campus;
                                    c.Institution = enrCMS.Institution;
                                    c.Mtg_Start = enrCMS.Mtg_Start;
                                    c.Mtg_End = enrCMS.Mtg_End;
                                    c.Mon = enrCMS.Mon;
                                    c.Tues = enrCMS.Tues;
                                    c.Wed = enrCMS.Wed;
                                    c.Thurs = enrCMS.Thurs;
                                    c.Fri = enrCMS.Fri;
                                    c.Sat = enrCMS.Sat;
                                    c.Sun = enrCMS.Sun;
                                    c.Start_Date = enrCMS.Start_Date;
                                    c.End_Date = enrCMS.End_Date;
                                    c.TRMS_IDs = "0";
                                    c.TRMS_Synced = false;
                                    c.CreateDateTimeSch = dtSchedule;

                                    db4.cms_class_schedule.Add(c);
                                    db4.SaveChanges();

                                }
                            }
                        }
                    }

                    //Console.WriteLine("Step 666"); Console.ReadKey();

                    //get latest date data from TRMS table
                    List<CMS_ClassSchedule> enrTRMSList = new List<CMS_ClassSchedule>();
                    using (var db3 = new Context())
                    {
                        enrTRMSList = db3.cms_class_schedule.Where(s => s.TRMS_Synced == false).ToList();// && s.CreateDateTimeSch > dtLastSch
                    }

                    //Console.WriteLine("Step 777"); Console.ReadKey();

                    if (enrTRMSList != null && enrTRMSList.Count > 0)
                    {
                        foreach (var enrTRMS in enrTRMSList)
                        {
                            #region
                            DateTime dtStartLoop = DateTime.ParseExact(enrTRMS.Start_Date, "dd-MMM-yy", CultureInfo.CurrentCulture);
                            DateTime dtEndLoop = DateTime.ParseExact(enrTRMS.End_Date, "dd-MMM-yy", CultureInfo.CurrentCulture);

                            //Console.WriteLine("Step 888"); //Console.ReadKey();

                            int days_count = 0, weeks_count = 0, current_day = -1;
                            days_count = int.Parse((dtEndLoop.Date - dtStartLoop.Date).TotalDays.ToString());
                            weeks_count = int.Parse((Convert.ToDecimal(days_count / 7) + 1).ToString());
                            CMS_ID = 0; TRMS_IDs = ""; uResponse = 0;
                            if (weeks_count > 0)
                            {
                                for (int i = 0; i < weeks_count; i++)
                                {
                                    #region
                                    OrganizationCampusRoomCourseSchedule sch = new OrganizationCampusRoomCourseSchedule();

                                    DateTime dtStartTime = DateTime.ParseExact(dtStartLoop.ToString("MM/dd/yyyy") + " " + enrTRMS.Mtg_Start, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);
                                    DateTime dtEndTime = DateTime.ParseExact(dtStartLoop.ToString("MM/dd/yyyy") + " " + enrTRMS.Mtg_End, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);

                                    //Console.WriteLine("Step 999"); //Console.ReadKey();

                                    //get campus id using CMS code
                                    var dbCampus = dbCampuses.Where(c => c.CampusCode.ToLower() == enrTRMS.Campus.ToLower()).FirstOrDefault();
                                    int iCampusID = dbCampus != null ? dbCampus.Id : 0;

                                    //get program id using CMS code
                                    var dbProgram = dbPrograms.Where(c => c.ProgramCode.ToLower() == enrTRMS.Catalog.ToLower()).FirstOrDefault();
                                    int iProgram = dbProgram != null ? dbProgram.Id : 0;

                                    //get course id using CMS code
                                    var dbCourse = dbCourses.Where(c => c.ProgramId == iProgram && c.CourseCode.ToLower().Contains(enrTRMS.Course_Id.ToLower())).FirstOrDefault();
                                    int iCourseID = dbCourse != null ? dbCourse.Id : 0;

                                    //get room id using CMS code
                                    var dbRoom = dbRooms.Where(c => c.TerminalId.ToString() == enrTRMS.Term.ToLower()).FirstOrDefault();
                                    int iRoom = dbRoom != null ? dbRoom.Id : 0;

                                    //get shift id using CMS code
                                    int iShift = 1; // dbShift.Where(c => c.ProgramShiftName.ToLower() == enrCMS.Career).FirstOrDefault().Id;

                                    CMS_ID = enrTRMS.Id;
                                    sch.CampusId = iCampusID;
                                    sch.CourseId = iCourseID;
                                    sch.ProgramId = iProgram;
                                    sch.RoomId = iRoom;
                                    sch.ShiftId = iShift;
                                    sch.StudyTitle = enrTRMS.Subject;
                                    sch.LectureGroupId = 0;
                                    sch.EmployeeTeacherId = 0;
                                    sch.CreateDateSch = DateTime.Now;

                                    using (var db2 = new Context())
                                    {
                                        current_day++;
                                        if (enrTRMS.Mon.ToUpper() == "Y")
                                        {
                                            if (dtStartTime.AddDays(current_day).DayOfWeek.ToString().ToUpper() == "MONDAY")
                                            {
                                                sch.StartTime = dtStartTime.AddDays(current_day);
                                                sch.EndTime = dtEndTime.AddDays(current_day);//dtStartTime.AddDays(current_day);//actually same day date

                                                //check already exists then delete
                                                CheckAlreadyExistsThenDelete(iCampusID, iCourseID, iProgram, sch.StartTime);

                                                db2.organization_campus_room_course_schedule.Add(sch);

                                                db2.SaveChanges();
                                                TRMS_IDs += sch.Id + ",";
                                            }
                                        }

                                        current_day++;
                                        if (enrTRMS.Tues.ToUpper() == "Y")
                                        {
                                            if (dtStartTime.AddDays(current_day).DayOfWeek.ToString().ToUpper() == "TUESDAY")
                                            {
                                                sch.StartTime = dtStartTime.AddDays(current_day);
                                                sch.EndTime = dtEndTime.AddDays(current_day);//actually same day date

                                                //check already exists then delete
                                                CheckAlreadyExistsThenDelete(iCampusID, iCourseID, iProgram, sch.StartTime);//StartTime is just date to check

                                                db2.organization_campus_room_course_schedule.Add(sch);

                                                db2.SaveChanges();
                                                TRMS_IDs += sch.Id + ",";
                                            }
                                        }

                                        current_day++;
                                        if (enrTRMS.Wed.ToUpper() == "Y")
                                        {
                                            if (dtStartTime.AddDays(current_day).DayOfWeek.ToString().ToUpper() == "WEDNESDAY")
                                            {
                                                sch.StartTime = dtStartTime.AddDays(current_day);
                                                sch.EndTime = dtEndTime.AddDays(current_day);//actually same day date

                                                //check already exists then delete
                                                CheckAlreadyExistsThenDelete(iCampusID, iCourseID, iProgram, sch.StartTime);

                                                db2.organization_campus_room_course_schedule.Add(sch);

                                                db2.SaveChanges();
                                                TRMS_IDs += sch.Id + ",";
                                            }
                                        }

                                        current_day++;
                                        if (enrTRMS.Thurs.ToUpper() == "Y")
                                        {
                                            if (dtStartTime.AddDays(current_day).DayOfWeek.ToString().ToUpper() == "THURSDAY")
                                            {
                                                sch.StartTime = dtStartTime.AddDays(current_day);
                                                sch.EndTime = dtEndTime.AddDays(current_day);//actually same day date

                                                //check already exists then delete
                                                CheckAlreadyExistsThenDelete(iCampusID, iCourseID, iProgram, sch.StartTime);

                                                db2.organization_campus_room_course_schedule.Add(sch);

                                                db2.SaveChanges();
                                                TRMS_IDs += sch.Id + ",";
                                            }
                                        }

                                        current_day++;
                                        if (enrTRMS.Fri.ToUpper() == "Y")
                                        {
                                            if (dtStartTime.AddDays(current_day).DayOfWeek.ToString().ToUpper() == "FRIDAY")
                                            {
                                                sch.StartTime = dtStartTime.AddDays(current_day);
                                                sch.EndTime = dtEndTime.AddDays(current_day);//actually same day date

                                                //check already exists then delete
                                                CheckAlreadyExistsThenDelete(iCampusID, iCourseID, iProgram, sch.StartTime);

                                                db2.organization_campus_room_course_schedule.Add(sch);

                                                db2.SaveChanges();
                                                TRMS_IDs += sch.Id + ",";
                                            }
                                        }

                                        current_day++;
                                        if (enrTRMS.Sat.ToUpper() == "Y")
                                        {
                                            if (dtStartTime.AddDays(current_day).DayOfWeek.ToString().ToUpper() == "SATURDAY")
                                            {
                                                sch.StartTime = dtStartTime.AddDays(current_day);
                                                sch.EndTime = dtEndTime.AddDays(current_day);//actually same day date

                                                //check already exists then delete
                                                CheckAlreadyExistsThenDelete(iCampusID, iCourseID, iProgram, sch.StartTime);

                                                db2.organization_campus_room_course_schedule.Add(sch);

                                                db2.SaveChanges();
                                                TRMS_IDs += sch.Id + ",";
                                            }
                                        }

                                        current_day++;
                                        if (enrTRMS.Sun.ToUpper() == "Y")
                                        {
                                            if (dtStartTime.AddDays(current_day).DayOfWeek.ToString().ToUpper() == "SUNDAY")
                                            {
                                                sch.StartTime = dtStartTime.AddDays(current_day);
                                                sch.EndTime = dtEndTime.AddDays(current_day);//actually same day date

                                                //check already exists then delete
                                                CheckAlreadyExistsThenDelete(iCampusID, iCourseID, iProgram, sch.StartTime.Date);

                                                db2.organization_campus_room_course_schedule.Add(sch);

                                                db2.SaveChanges();
                                                TRMS_IDs += sch.Id + ",";
                                            }
                                        }
                                    }

                                    #endregion

                                    //Console.WriteLine("Step 1010"); //Console.ReadKey();

                                }//inner-for ended   

                                TRMS_IDs = TRMS_IDs.TrimEnd(',');
                                using (var db3 = new Context())
                                {
                                    var dbSync = db3.cms_class_schedule.Where(c => c.Id == enrTRMS.Id).FirstOrDefault();
                                    if (dbSync != null)
                                    {
                                        dbSync.TRMS_IDs = TRMS_IDs;
                                        dbSync.TRMS_Synced = true;

                                        db3.SaveChanges();
                                    }
                                }

                                //Console.WriteLine("Step 1111"); //Console.ReadKey();

                                //update CMS status
                                //TRMS_IDs = TRMS_IDs.TrimEnd(',');
                                //uResponse = CMS_UpdateScheduleRowSyncedNow(strORCCMSConnString, CMS_ID, TRMS_IDs);
                                //if (uResponse > 0)
                                //{
                                //    Console.WriteLine("TRMS-Import: Class-Schedules - CMS IDs and Sync UPDATED");
                                //}
                            }

                            #endregion
                        }
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
                    using (var db3 = new Context())
                    {
                        db3.log_message.Add(new LogMessage
                        {
                            LogTitle = "TRMS-Import: Completed",
                            LogDateTime = DateTime.Now,
                            LogMessageText = "TRMS-Import: All Steps Completed"
                        });
                        db3.SaveChanges();
                    }
                }

                //Console.WriteLine("Step 1212"); Console.ReadKey();

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
                    using (var db = new Context())
                    {
                        db.log_message.Add(new LogMessage
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
            Console.ReadKey();
        }

        ///////////////////// Oracle using Methods //////////////////////

        public static void CheckAlreadyExistsThenDelete(int cam_id, int crs_id, int prg_id, DateTime OnlyDate)
        {
            using (var db = new Context())
            {
                var dbOrgCourseSchedule = db.organization_campus_room_course_schedule.Where(s => s.CampusId == cam_id && s.CourseId == crs_id && s.ProgramId == prg_id && (s.StartTime.Day == OnlyDate.Day && s.StartTime.Month == OnlyDate.Month && s.StartTime.Year == OnlyDate.Year)).FirstOrDefault();
                if (dbOrgCourseSchedule != null)
                {
                    db.organization_campus_room_course_schedule.Remove(dbOrgCourseSchedule);

                    db.SaveChanges();
                }
            }
        }

        public static DateTime? GetLastRowDateFromTRMS()
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            using (var db = new Context())
            {
                var dbOrgCourseSchedule = db.cms_class_schedule.OrderByDescending(c => c.CreateDateTimeSch).FirstOrDefault();
                if (dbOrgCourseSchedule != null)
                {
                    dt = dbOrgCourseSchedule.CreateDateTimeSch;
                }
                else
                {
                    dt = DateTime.Now.AddSeconds(-10);
                }
            }

            return dt;
        }

        public static List<CMSClassSchedule> CMS_GetScheduleRowUnsyncedYet(string conString, string strOracleSQLQuery, DateTime dtLastSch)
        {
            List<CMSClassSchedule> enrCMSList = new List<CMSClassSchedule>();

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString; // "context connection=true";
            con.Open();

            //Console.WriteLine("Step 444"); Console.ReadKey();

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = strOracleSQLQuery; // "select * from intr_cms_class_schedules order by id";
                                                 //cmd.Parameters.Add(":1", OracleDbType.Int32, 1, ParameterDirection.Input);
                                                 //cmd.Parameters.Add(":1", OracleDbType.Int32, ca_id, ParameterDirection.Input);

            // get a data reader
            int counter = 1;
            OracleDataReader _SqlDataReader = cmd.ExecuteReader();

            // get the country name from the data reader
            if (_SqlDataReader != null && _SqlDataReader.HasRows)
            {
                // We have data in SqlDataReader, lets loop through and display the data
                while (_SqlDataReader.Read())
                {
                    //Console.WriteLine(_SqlDataReader["ID"].ToString()); Console.WriteLine(_SqlDataReader["COURSE_ID"].ToString()); Console.WriteLine(_SqlDataReader["CLASS_NBR"].ToString());
                    //Console.WriteLine(_SqlDataReader["SUBJECT"].ToString()); Console.WriteLine(_SqlDataReader["CAREER"].ToString()); Console.WriteLine(_SqlDataReader["DESCR"].ToString());
                    CMSClassSchedule enrCMS = new CMSClassSchedule();

                    //  0       1       2       3       4   5           6       7           8   9       10          11          12-18       19      20
                    //COURSE CLASS_NBR SUBJECT ACAD DESCR CATALOG_NB ACAD_      STRM    CAMPU   INSTI MTG_START   MTG_END M T W T F S S START_DAT END_DATE

                    /*
                    Name Null?    Type
                    ---------------------------------------- - ------------------------------------
                    COURSE_ID                                 NOT NULL VARCHAR2(6 CHAR)
                    CLASS_NBR NOT NULL NUMBER(38)
                    SUBJECT                                   NOT NULL VARCHAR2(8 CHAR)
                    ACAD_CAREER                               NOT NULL VARCHAR2(4 CHAR)
                    DESCR                                     NOT NULL VARCHAR2(30 CHAR)
                    CATALOG_NBR                               NOT NULL VARCHAR2(10 CHAR)
                    ACAD_GROUP                                NOT NULL VARCHAR2(5 CHAR)
                    STRM                                      NOT NULL VARCHAR2(4 CHAR)
                    CAMPUS                                    NOT NULL VARCHAR2(5 CHAR)
                    INSTITUTION                               NOT NULL VARCHAR2(5 CHAR)
                    MTG_START                                          VARCHAR2(11)
                    MTG_END                                            VARCHAR2(11)
                    MON                                       NOT NULL VARCHAR2(1 CHAR)
                    TUES                                      NOT NULL VARCHAR2(1 CHAR)
                    WED                                       NOT NULL VARCHAR2(1 CHAR)
                    THURS                                     NOT NULL VARCHAR2(1 CHAR)
                    FRI                                       NOT NULL VARCHAR2(1 CHAR)
                    SAT                                       NOT NULL VARCHAR2(1 CHAR)
                    SUN                                       NOT NULL VARCHAR2(1 CHAR)
                    START_DATE                                         DATE
                    END_DATE                                           DATE
                    */

                    enrCMS.Id = counter; //int.Parse(_SqlDataReader["ID"].ToString());
                    enrCMS.Course_Id = _SqlDataReader["COURSE_ID"].ToString() ?? "";
                    enrCMS.Class_Nbr = _SqlDataReader["CLASS_NBR"].ToString() ?? "";
                    enrCMS.Subject = _SqlDataReader["SUBJECT"].ToString() ?? "";
                    enrCMS.Career = _SqlDataReader["ACAD_CAREER"].ToString() ?? "";
                    enrCMS.Descr = _SqlDataReader["DESCR"].ToString() ?? "";
                    enrCMS.Catalog = _SqlDataReader["CATALOG_NBR"].ToString() ?? "";
                    enrCMS.Acad_Group = _SqlDataReader["ACAD_GROUP"].ToString() ?? "";
                    enrCMS.Term = _SqlDataReader["STRM"].ToString() ?? "";
                    enrCMS.Campus = _SqlDataReader["CAMPUS"].ToString() ?? "";
                    enrCMS.Institution = _SqlDataReader["INSTITUTION"].ToString() ?? "";
                    enrCMS.Mtg_Start = _SqlDataReader["MTG_START"].ToString() ?? "";
                    enrCMS.Mtg_End = _SqlDataReader["MTG_END"].ToString() ?? "";
                    enrCMS.Mon = _SqlDataReader["MON"].ToString() ?? "";
                    enrCMS.Tues = _SqlDataReader["TUES"].ToString() ?? "";
                    enrCMS.Wed = _SqlDataReader["WED"].ToString() ?? "";
                    enrCMS.Thurs = _SqlDataReader["THURS"].ToString() ?? "";
                    enrCMS.Fri = _SqlDataReader["FRI"].ToString() ?? "";
                    enrCMS.Sat = _SqlDataReader["SAT"].ToString() ?? "";
                    enrCMS.Sun = _SqlDataReader["SUN"].ToString() ?? "";
                    enrCMS.Start_Date = _SqlDataReader["START_DATE"].ToString() ?? ""; //"29-JUN-20";
                    enrCMS.End_Date = _SqlDataReader["END_DATE"].ToString() ?? ""; //"29-JUN-20";
                    //enrCMS.strCreateDateTimeSch = _SqlDataReader["CreateDateTimeSch"].ToString() ?? "";
                    //enrCMS.CreateDateTimeSch = DateTime.ParseExact(_SqlDataReader["CreateDateTimeSch"].ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);
                    enrCMS.strCreateDateTimeSch = DateTime.Now.AddMinutes(1).ToString("MM/dd/yyyy hh:mm:00 tt");  //_SqlDataReader["CreateDateTimeSch"].ToString() ?? "";//_SqlDataReader["CreateDateTimeSch"].ToString() ?? "";
                    enrCMS.CreateDateTimeSch = DateTime.ParseExact(enrCMS.strCreateDateTimeSch, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);

                    //  0       1       2       3       4   5           6       7           8   9       10          11          12-18       19      20
                    //COURSE CLASS_NBR SUBJECT ACAD DESCR CATALOG_NB ACAD_      STRM    CAMPU   INSTI MTG_START   MTG_END M T W T F S S START_DAT END_DATE

                    //enrCMS.Id = 0; //int.Parse(_SqlDataReader["ID"].ToString());
                    //enrCMS.Course_Id = _SqlDataReader[0].ToString() ?? ""; //_SqlDataReader["COURSE"].ToString() ?? "";
                    //enrCMS.Class_Nbr = _SqlDataReader[1].ToString() ?? ""; //_SqlDataReader["CLASS_NBR"].ToString() ?? "";
                    //enrCMS.Subject = _SqlDataReader[2].ToString() ?? ""; //_SqlDataReader["SUBJECT"].ToString() ?? "";
                    //enrCMS.Career = _SqlDataReader[3].ToString() ?? ""; //_SqlDataReader["CAREER"].ToString() ?? "";
                    //enrCMS.Descr = _SqlDataReader[4].ToString() ?? ""; // _SqlDataReader["DESCR"].ToString() ?? "";
                    //enrCMS.Catalog = _SqlDataReader[5].ToString() ?? ""; // _SqlDataReader["CATALOG_NB"].ToString() ?? "";
                    //enrCMS.Acad_Group = _SqlDataReader[6].ToString() ?? ""; //_SqlDataReader["ACAD"].ToString() ?? "";
                    //enrCMS.Term = _SqlDataReader[7].ToString() ?? ""; //_SqlDataReader["STRM"].ToString() ?? "";
                    //enrCMS.Campus = _SqlDataReader[8].ToString() ?? ""; //_SqlDataReader["CAMPU"].ToString() ?? "";
                    //enrCMS.Institution = _SqlDataReader[9].ToString() ?? ""; //_SqlDataReader["INSTI"].ToString() ?? "";
                    //enrCMS.Mtg_Start = _SqlDataReader[10].ToString() ?? ""; //_SqlDataReader["MTG_START"].ToString() ?? "";
                    //enrCMS.Mtg_End = _SqlDataReader[11].ToString() ?? ""; // _SqlDataReader["MTG_END"].ToString() ?? "";
                    //enrCMS.Mon = _SqlDataReader[12].ToString() ?? ""; //_SqlDataReader["MON"].ToString() ?? "";
                    //enrCMS.Tues = _SqlDataReader[13].ToString() ?? ""; //_SqlDataReader["TUES"].ToString() ?? "";
                    //enrCMS.Wed = _SqlDataReader[14].ToString() ?? "";// _SqlDataReader["WED"].ToString() ?? "";
                    //enrCMS.Thurs = _SqlDataReader[15].ToString() ?? "";//_SqlDataReader["THURS"].ToString() ?? "";
                    //enrCMS.Fri = _SqlDataReader[16].ToString() ?? "";// _SqlDataReader["FRI"].ToString() ?? "";
                    //enrCMS.Sat = _SqlDataReader[17].ToString() ?? "";// _SqlDataReader["SAT"].ToString() ?? "";
                    //enrCMS.Sun = _SqlDataReader[18].ToString() ?? "";// _SqlDataReader["SUN"].ToString() ?? "";
                    //enrCMS.Start_Date = _SqlDataReader[19].ToString() ?? "";// _SqlDataReader["START_DATE"].ToString() ?? "";"29-JUN-20";
                    //enrCMS.End_Date = _SqlDataReader[20].ToString() ?? ""; //_SqlDataReader["END_DATE"].ToString() ?? "";"29-AUG-20";
                    //enrCMS.strCreateDateTimeSch = DateTime.Now.AddMinutes(1).ToString("MM/dd/yyyy hh:mm:00 tt");  //_SqlDataReader["CreateDateTimeSch"].ToString() ?? "";//_SqlDataReader["CreateDateTimeSch"].ToString() ?? "";
                    //enrCMS.CreateDateTimeSch = DateTime.ParseExact(enrCMS.strCreateDateTimeSch, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CurrentCulture);

                    Console.WriteLine("Dates - ST=" + _SqlDataReader["START_DATE"].ToString() + " and END=" + _SqlDataReader["END_DATE"].ToString());

                    enrCMSList.Add(enrCMS);

                    counter++;
                }
            }

            // clean up objects
            _SqlDataReader.Close();

            // clean up objects
            cmd.Dispose();
            con.Dispose();

            //Console.WriteLine("Step 555"); Console.ReadKey();

            // Return the counter
            return enrCMSList.Where(l => l.CreateDateTimeSch > dtLastSch).ToList();
        }

        public static int CMS_UpdateScheduleRowSyncedNow(string conString, int cms_id, string trms_ids)
        {
            int iResponse = 0;

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString;
            con.Open();

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "update intr_cms_class_schedules set trms_ids=:1, trms_synced=1 where id = :2";

            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1"; p01.Value = trms_ids; p01.OracleType = OracleType.VarChar;//cmd.Parameters.Add(":1", OracleDbType.Varchar2, trms_ids, ParameterDirection.Input);
            cmd.Parameters.Add(p01);

            OracleParameter p02 = new OracleParameter();
            p02.ParameterName = ":2"; p02.Value = cms_id; p02.OracleType = OracleType.Int32;//cmd.Parameters.Add(":2", OracleDbType.Int32, cms_id, ParameterDirection.Input);
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
            cmd.CommandText = "select * from intr_cms_class_schedules where id = :1";

            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1"; p01.Value = a; p01.OracleType = OracleType.Int32;//cmd.Parameters.Add(":1", OracleDbType.Int32, a, ParameterDirection.Input);
            cmd.Parameters.Add(p01);

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
                using (var db = new Context())
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
                        OrganizationCampusRoomCourseSchedule schedule = db.organization_campus_room_course_schedule
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
                            OrganizationCampusRoomCourseSchedule sch = new OrganizationCampusRoomCourseSchedule()
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

            using (var db = new Context())
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

            using (var db = new Context())
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

            using (var db = new Context())
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

            using (var db = new Context())
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

            using (var db = new Context())
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
                using (var dbTT = new Context())
                {
                    dbTT.log_message.Add(new LogMessage
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
 * 
 * https://www.oracle.com/webfolder/technetwork/tutorials/obe/db/12c/r1/Windows_DB_Install_OBE/Installing_Oracle_Db12c_Windows.html
 * 
 * 
 * 
 pdborcl


orcl
Oracle_1



Enter user-name: sys as sysdba
Enter password: Oracle_1

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

SQL> GRANT CONNECT, RESOURCE, DBA TO OT;//pwd=Ot1234

Grant succeeded.

SQL> CONNECT ot@pdborcl
Enter password:
Connected.
Note that OT user only exists in the PDBORCL pluggable database, therefore, you must explicitly specify the username as ot@pdborcl in the CONNECT command.


https://www.oracletutorial.com/getting-started/oracle-sample-database/


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
