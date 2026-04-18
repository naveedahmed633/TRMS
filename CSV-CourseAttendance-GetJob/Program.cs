using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Globalization;
using System.IO;
using System.Data.Odbc;
//using Oracle.DataAccess.Client;

namespace CSV_CourseAttendance_GetJob
{
    class Program
    {
        public static bool isStaticTTLogAllowed = false;

        static void Main(string[] args)
        {
            int zero = 0, iLogSave = 0, cutOff = 0;
            int count1 = 0, count2 = 0, count3 = 0;
            int iDBTimeOut = 0;

            bool isTTLogAllowed = false;
            string iCSVorDBImport = "0", strORCCMSConnString = "", strFilePath = "", strFileName = "", strLTitleText = "", strStep = "";

            List<int> insertedCAIds = new List<int>();

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

                if (ConfigurationManager.ConnectionStrings["ORC_CMS"] != null && ConfigurationManager.ConnectionStrings["ORC_CMS"].ConnectionString != "")
                {
                    strORCCMSConnString = ConfigurationManager.ConnectionStrings["ORC_CMS"].ConnectionString ?? "";
                }

                //GetCountryName(strORCCMSConnString, "AU");

                //InsertALLRegionName(strORCCMSConnString, "Australia", "Canada", "Asia");

                //InsertRegionName(strORCCMSConnString, "AAA", "BBB", "CCC");

                //DeleteRegionName(strORCCMSConnString, 31, 32, 33);

                if (ConfigurationManager.AppSettings["DefaultFileName"] != null && ConfigurationManager.AppSettings["DefaultFileName"].ToString() != "")
                {
                    strFileName = ConfigurationManager.AppSettings["DefaultFileName"].ToString() ?? "";
                }

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

                if (ConfigurationManager.AppSettings["DBTimeOut"] != null && ConfigurationManager.AppSettings["DBTimeOut"].ToString() != "")
                {
                    iDBTimeOut = int.Parse(ConfigurationManager.AppSettings["DBTimeOut"].ToString() ?? "999");
                }

                if (ConfigurationManager.AppSettings["IsTTLogAllowed"] != null && ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() != "")
                {
                    isTTLogAllowed = ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() == "1" ? true : false;
                    isStaticTTLogAllowed = ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() == "1" ? true : false;
                }

                //STEP CourseAttendances 1/3: Get TRMS CourseAttendances Table Data
                #region step_01
                strStep = "1";
                Console.WriteLine("TRMS-Export: STEP 01 - Getting Unsynced Data Only");

                //List<OrganizationCourseAttendance> allUnsentData = new List<OrganizationCourseAttendance>();
                List<CA_Data> ca_raw_list = new List<CA_Data>();
                List<CA_Data> ca_list = new List<CA_Data>();

                using (var db = new DLL.Models.Context())
                {
                    ca_raw_list = (from ca in db.organization_course_attendance
                                       //join e in db.employee on ca.employee_student_id equals e.EmployeeId
                                       //join sg in db.region on ca.schedule_group_id equals sg.RegionId
                                       //join ps in db.organization_program_shift on ca.student_pshift_id equals ps.Id
                                       //join cr in db.organization_program_course on ca.course_id equals cr.Id
                                       ////join tr in db.termainal on ca.terminal_in_id equals tr.Id
                                   where ca.is_sent == false
                                   select new CA_Data
                                   {
                                       _id = ca.id,
                                       _schedule_date_dt = ca.schedule_date_time,
                                       _schedule_date = "",
                                       _student_id = ca.employee_student_id,
                                       _student_code = "",
                                       _student_name = "",
                                       _student_group_id = ca.student_group_id,
                                       _student_group = "",
                                       _student_pshift_id = ca.student_pshift_id,
                                       _student_pshift = "",
                                       _course_id = ca.course_id,
                                       _course_code = "",
                                       _course_title = "",
                                       _time_in_dt = ca.student_time_in.HasValue ? ca.student_time_in.Value : new DateTime(2001, 01, 01),
                                       _time_in = "",
                                       _status_in = ca.status_in,
                                       _time_out_dt = ca.student_time_out.HasValue ? ca.student_time_out.Value : new DateTime(2001, 01, 01),
                                       _time_out = "",
                                       _status_out = ca.status_out,
                                       _final_remarks = ca.final_remarks,
                                       _terminal_in_id = ca.terminal_in_id,
                                       _terminal_in = "",
                                       _terminal_out_id = ca.terminal_out_id,
                                       _terminal_out = "",
                                       _course_time_start_dt = ca.course_time_start ?? new DateTime(2001, 01, 01),
                                       _course_time_start = "",
                                       _course_time_end_dt = ca.course_time_end ?? new DateTime(2001, 01, 01),
                                       _course_time_end = ""
                                   }).ToList();

                    if (ca_raw_list != null && ca_raw_list.Count > 0)
                    {
                        //db data
                        var dbStudents = db.employee.ToList();
                        var dbStdGroups = db.region.ToList();
                        var dbPShifts = db.organization_program_shift.ToList();
                        var dbCourses = db.organization_program_course.ToList();
                        var dbTerminals = db.termainal.ToList();

                        foreach (var r in ca_raw_list)
                        {
                            //employees data
                            var dbStudent = dbStudents.Where(t => t.EmployeeId == r._student_id).FirstOrDefault();
                            int iStdID = dbStudent != null ? dbStudent.EmployeeId : 0;
                            string strStdCode = dbStudent != null ? dbStudent.employee_code : "";
                            string strStdName = dbStudent != null ? (dbStudent.first_name + " " + dbStudent.last_name) : "";

                            //student group - region
                            var dbGroup = dbStdGroups.Where(t => t.RegionId == r._student_group_id).FirstOrDefault();
                            int iGrpID = dbGroup != null ? dbGroup.RegionId : 0;
                            string strGrpName = dbGroup != null ? dbGroup.name : "";

                            //program shift
                            var dbShift = dbPShifts.Where(t => t.Id == r._student_pshift_id).FirstOrDefault();
                            int iShfID = dbShift != null ? dbShift.Id : 0;
                            string strShfName = dbShift != null ? dbShift.ProgramShiftName : "";

                            //course data
                            var dbCourse = dbCourses.Where(t => t.Id == r._course_id).FirstOrDefault();
                            int iCrsID = dbCourse != null ? dbCourse.Id : 0;
                            string strCrsCode = dbCourse != null ? dbCourse.CourseCode : "";
                            string strCrsTitle = dbCourse != null ? dbCourse.CourseTitle : "";

                            //terminals
                            var dbTerminalIn = dbTerminals.Where(t => t.L_ID == r._terminal_in_id).FirstOrDefault();
                            string strTermIn = dbTerminalIn != null ? dbTerminalIn.C_Name : "";
                            var dbTerminalOut = dbTerminals.Where(t => t.L_ID == r._terminal_in_id).FirstOrDefault();
                            string strTermOut = dbTerminalOut != null ? dbTerminalOut.C_Name : "";

                            var ca = new CA_Data()
                            {
                                _id = r._id,
                                _schedule_date_dt = r._schedule_date_dt,
                                _schedule_date = r._schedule_date_dt.ToString("dd-MM-yyyy"),
                                _student_id = iStdID,
                                _student_code = strStdCode ?? "",
                                _student_name = strStdName ?? "",
                                _student_group_id = iGrpID,
                                _student_group = strGrpName ?? "",
                                _student_pshift_id = iShfID,
                                _student_pshift = strShfName ?? "",
                                _course_id = iCrsID,
                                _course_code = strCrsCode ?? "",
                                _course_title = strCrsTitle ?? "",
                                _time_in_dt = r._time_in_dt.Value,
                                _time_in = r._time_in_dt.HasValue ? r._time_in_dt.Value.ToString("dd-MM-yyyy HH:mm tt") : "01-01-01 00:00:00",
                                _status_in = r._status_in ?? "",
                                _time_out_dt = r._time_out_dt.Value,
                                _time_out = r._time_out_dt.HasValue ? r._time_out_dt.Value.ToString("dd-MM-yyyy HH:mm tt") : "01-01-01 00:00:00",
                                _status_out = r._status_out ?? "",
                                _final_remarks = r._final_remarks ?? "",
                                _terminal_in_id = r._terminal_in_id,
                                _terminal_in = strTermIn ?? "",
                                _terminal_out_id = r._terminal_out_id,
                                _terminal_out = strTermOut ?? "",
                                _course_time_start_dt = r._course_time_start_dt,
                                _course_time_start = r._course_time_start_dt.HasValue ? r._course_time_start_dt.Value.ToString("dd-MM-yyyy HH:mm") : "01-01-01 00:00:00",
                                _course_time_end_dt = r._course_time_end_dt,
                                _course_time_end = r._course_time_end_dt.HasValue ? r._course_time_end_dt.Value.ToString("dd-MM-yyyy HH:mm") : "01-01-01 00:00:00",
                            };

                            ca_list.Add(ca);
                        }
                    }
                }
                #endregion

                //STEP CourseAttendances 2/3: Generate CSV / SQL Import / Oracle Import
                #region step_02
                strStep = "2";
                Console.WriteLine("TRMS-Export: STEP 02: Generate CSV / Insert Process");

                if (ca_list != null && ca_list.Count > 0)
                {
                    if (iCSVorDBImport == "0") //Generating CSV
                    {
                        #region 'CoursesAttendance' TRMS table: Generate CSV

                        ////////////////////// PROCESSING DATA FOR CSV FILE ////////////////////////////////////////
                        WriteToConsoleAudiLog("TRMS-Export: CSV Generation Process Started");

                        string strCALogs = "";
                        strCALogs += "\"ca_id\",\"schedule_date\",\"student_code\",\"student_name\",\"student_group\",\"student_group\",\"course_code\",\"course_title\",\"time_in\",\"status_in\",\"time_out\",\"status_out\",\"final_remarks\",\"terminal_in\",\"terminal_out\"" + Environment.NewLine;

                        foreach (var c in ca_list)
                        {
                            strCALogs += "\"" + c._id + "\",\"" + c._schedule_date + "\",\"" + c._student_code + "\",\"" + c._student_name + "\",\"" + c._student_group + "\",\"" + c._course_code + "\",\"" + c._course_title + "\",\"" + c._time_in + "\",\"" + c._status_in + "\",\"" + c._time_out + "\",\"" + c._status_out + "\",\"" + c._final_remarks + "\",\"" + c._terminal_in + "\",\"" + c._terminal_out + "\"" + Environment.NewLine;

                            count2++;
                        }

                        strCALogs = strCALogs.TrimEnd('\r', '\n');
                        strCALogs = strCALogs.TrimEnd('\r', '\n');

                        //sw.Close();

                        WriteToConsoleAudiLog("TRMS-Export: Data Manipulation - Completed");

                        ////////////////////////// COPYING FILE ////////////////////////////////////////////
                        string sourcePath = strFilePath + strFileName;
                        string targetPath = "", fileName = "";

                        //orc24 file processing
                        if (strCALogs.Length > 0)
                        {
                            WriteToConsoleAudiLog("TRMS-Export: Writing Data to File - Starting...");

                            using (StreamWriter sw = new StreamWriter(sourcePath))
                            {
                                sw.WriteLine(strCALogs);
                                //sw.WriteLine(string.Join(",", strCALogs));
                            }

                            WriteToConsoleAudiLog("TRMS-Export: Writing Data to File - Completed");
                        }

                        //copy file from this to other location
                        WriteToConsoleAudiLog("TRMS-Export: Copying File...");

                        targetPath = strFilePath;
                        fileName = "" + DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss") + ".csv";
                        string destFile = System.IO.Path.Combine(targetPath, fileName);
                        System.IO.File.Copy(sourcePath, destFile, true);

                        #endregion
                    }
                    else if (iCSVorDBImport == "1") //Posting data to SQL DB 
                    {
                        #region 'CoursesAttendance' TRMS table: Post to SQL Server
                        WriteToConsoleAudiLog("TRMS-Export: SQL Insertion Process Started");

                        using (var db = new ContextTRMS_CMS())
                        {
                            foreach (var e in ca_list)
                            {
                                if (e._id > 0)
                                {
                                    db.trms_organization_course_attendances.Add(new TRMS_OrganizationCourseAttendances()
                                    {
                                        id = e._id,
                                        schedule_date_dt = e._schedule_date_dt,
                                        schedule_date = e._schedule_date_dt.ToString("dd-MM-yyyy"),
                                        student_id = e._student_id,
                                        student_code = e._student_code,
                                        student_name = e._student_name,
                                        student_group_id = e._student_group_id,
                                        student_group = e._student_group,
                                        student_pshift_id = e._student_pshift_id,
                                        student_pshift = e._student_pshift,
                                        course_id = e._course_id,
                                        course_code = e._course_code,
                                        course_title = e._course_title,
                                        time_in_dt = e._time_in_dt,
                                        time_in = e._time_in,
                                        status_in = e._status_in,
                                        time_out_dt = e._time_out_dt,
                                        time_out = e._time_out,
                                        status_out = e._status_out,
                                        final_remarks = e._final_remarks,
                                        terminal_in_id = e._terminal_in_id,
                                        terminal_in = e._terminal_in,
                                        terminal_out_id = e._terminal_out_id,
                                        terminal_out = e._terminal_out,
                                        course_time_start_dt = e._course_time_start_dt,
                                        course_time_start = e._course_time_start,
                                        course_time_end_dt = e._course_time_end_dt,
                                        course_time_end = e._course_time_end
                                    });

                                    insertedCAIds.Add(e._id);
                                    cutOff++; count2++;
                                }

                                //save to db
                                if (cutOff == iLogSave)
                                {
                                    cutOff = 0;
                                    db.SaveChanges();
                                }
                            }

                            db.SaveChanges();
                        }

                        #endregion
                    }
                    else //Posting data to ORACLE DB 
                    {
                        #region 'CoursesAttendance' TRMS table: Post to Oracle Server
                        WriteToConsoleAudiLog("TRMS-Export: Oracle Insertion Process Started");

                        int addedCount = 1;

                        using (OracleConnection orcCon = new OracleConnection(strORCCMSConnString))
                        {
                            int pCount = 0;
                            foreach (var e in ca_list)
                            {
                                pCount = 0;
                                if (e._id > 0)
                                {
                                    using (OracleCommand orcCmd = new OracleCommand())
                                    {
                                        orcCmd.Connection = orcCon;
                                        orcCmd.CommandTimeout = iDBTimeOut;
                                        orcCmd.CommandType = CommandType.StoredProcedure;
                                        orcCmd.CommandText = "sp_manage_course_attendance";

                                        //////////////////////////////////////////////////

                                        OracleParameter[] prm = new OracleParameter[1];

                                        //0
                                        OracleParameter p00 = new OracleParameter();
                                        p00.ParameterName = "p_ca_id";
                                        p00.Value = e._id;
                                        p00.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p00);

                                        pCount++;//1
                                        OracleParameter p01 = new OracleParameter();
                                        p01.ParameterName = "p_schedule_date_dt";
                                        p01.Value = e._schedule_date_dt.ToString("dd-MMM-yy HH:mm:ss.000000");
                                        p01.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p01);

                                        pCount++;//2
                                        OracleParameter p02 = new OracleParameter();
                                        p02.ParameterName = "p_schedule_date";
                                        p02.Value = e._schedule_date_dt.ToString("dd-MM-yyyy");
                                        p02.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p02);

                                        pCount++;//3
                                        OracleParameter p03 = new OracleParameter();
                                        p03.ParameterName = "p_student_id";
                                        p03.Value = e._student_id;
                                        p03.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p03);

                                        pCount++;//4
                                        OracleParameter p04 = new OracleParameter();
                                        p04.ParameterName = "p_student_code";
                                        p04.Value = e._student_code;
                                        p04.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p04);

                                        pCount++;//5
                                        OracleParameter p05 = new OracleParameter();
                                        p05.ParameterName = "p_student_name";
                                        p05.Value = e._student_name;
                                        p05.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p05);

                                        pCount++;//6
                                        OracleParameter p06 = new OracleParameter();
                                        p06.ParameterName = "p_student_group_id";
                                        p06.Value = e._student_group_id;
                                        p06.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p06);

                                        pCount++;//7
                                        OracleParameter p07 = new OracleParameter();
                                        p07.ParameterName = "p_student_group";
                                        p07.Value = e._student_group;
                                        p07.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p07);

                                        pCount++;//8
                                        OracleParameter p08 = new OracleParameter();
                                        p08.ParameterName = "p_student_pshift_id";
                                        p08.Value = e._student_pshift_id;
                                        p08.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p08);

                                        pCount++;//9
                                        OracleParameter p09 = new OracleParameter();
                                        p09.ParameterName = "p_student_pshift";
                                        p09.Value = e._student_pshift;
                                        p09.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p09);

                                        pCount++;//10
                                        OracleParameter p10 = new OracleParameter();
                                        p10.ParameterName = "p_course_id";
                                        p10.Value = e._course_id;
                                        p10.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p10);

                                        pCount++;//11
                                        OracleParameter p11 = new OracleParameter();
                                        p11.ParameterName = "p_course_code";
                                        p11.Value = e._course_code;
                                        p11.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p11);

                                        pCount++;//12
                                        OracleParameter p12 = new OracleParameter();
                                        p12.ParameterName = "p_course_title";
                                        p12.Value = e._course_title;
                                        p12.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p12);

                                        pCount++;//13
                                        OracleParameter p13 = new OracleParameter();
                                        p13.ParameterName = "p_time_in_dt";
                                        p13.Value = e._time_in_dt.HasValue ? e._time_in_dt.Value.ToString("dd-MMM-yy HH:mm:ss.000000") : "01-Jan-00 00:00:00.000000";
                                        p13.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p13);

                                        pCount++;//14
                                        OracleParameter p14 = new OracleParameter();
                                        p14.ParameterName = "p_time_in";
                                        p14.Value = e._time_in_dt.HasValue ? (e._time_in_dt.Value.ToString("dd-MM-yy HH:mm:ss") == "01-01-01 00:00:00" ? null : e._time_in_dt.Value.ToString("dd-MM-yyyy HH:mm:ss")) : "01-01-01 00:00:00";
                                        p14.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p14);

                                        pCount++;//15
                                        OracleParameter p15 = new OracleParameter();
                                        p15.ParameterName = "p_status_in";
                                        p15.Value = e._status_in;
                                        p15.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p15);

                                        pCount++;//16
                                        OracleParameter p16 = new OracleParameter();
                                        p16.ParameterName = "p_time_out_dt";
                                        p16.Value = e._time_out_dt.HasValue ? e._time_out_dt.Value.ToString("dd-MMM-yy HH:mm:ss.000000") : "01-Jan-00 00:00:00.000000";
                                        p16.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p16);

                                        pCount++;//17
                                        OracleParameter p17 = new OracleParameter();
                                        p17.ParameterName = "p_time_out";
                                        p17.Value = e._time_out_dt.HasValue ? (e._time_out_dt.Value.ToString("dd-MM-yy HH:mm:ss") == "01-01-01 00:00:00" ? null : e._time_out_dt.Value.ToString("dd-MM-yyyy HH:mm:ss")) : "01-01-01 00:00:00";
                                        p17.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p17);

                                        pCount++;//18
                                        OracleParameter p18 = new OracleParameter();
                                        p18.ParameterName = "p_status_out";
                                        p18.Value = e._status_out;
                                        p18.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p18);

                                        pCount++;//19
                                        OracleParameter p19 = new OracleParameter();
                                        p19.ParameterName = "p_final_remarks";
                                        p19.Value = e._final_remarks;
                                        p19.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p19);

                                        pCount++;//20
                                        OracleParameter p20 = new OracleParameter();
                                        p20.ParameterName = "p_terminal_in_id";
                                        p20.Value = e._terminal_in_id;
                                        p20.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p20);

                                        pCount++;//21
                                        OracleParameter p21 = new OracleParameter();
                                        p21.ParameterName = "p_terminal_in";
                                        p21.Value = e._terminal_in;
                                        p21.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p21);

                                        pCount++;//22
                                        OracleParameter p22 = new OracleParameter();
                                        p22.ParameterName = "p_terminal_out_id";
                                        p22.Value = e._terminal_out_id;
                                        p22.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p22);

                                        pCount++;//23
                                        OracleParameter p23 = new OracleParameter();
                                        p23.ParameterName = "p_terminal_out";
                                        p23.Value = e._terminal_out;
                                        p23.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p23);

                                        pCount++;//24
                                        OracleParameter p24 = new OracleParameter();
                                        p24.ParameterName = "p_course_time_start_dt";
                                        p24.Value = e._course_time_start_dt.HasValue ? e._course_time_start_dt.Value.ToString("dd-MMM-yy HH:mm:ss.000000") : "01-Jan-00 00:00:00.000000";
                                        p24.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p24);

                                        pCount++;//25
                                        OracleParameter p25 = new OracleParameter();
                                        p25.ParameterName = "p_course_time_start";
                                        p25.Value = e._course_time_start_dt.HasValue ? (e._course_time_start_dt.Value.ToString("dd-MM-yy HH:mm:ss") == "01-01-01 00:00:00" ? null : e._course_time_start_dt.Value.ToString("dd-MM-yyyy HH:mm:ss")) : "01-01-01 00:00:00";
                                        p25.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p25);

                                        pCount++;//26
                                        OracleParameter p26 = new OracleParameter();
                                        p26.ParameterName = "p_course_time_end_dt";
                                        p26.Value = e._course_time_end_dt.HasValue ? e._course_time_end_dt.Value.ToString("dd-MMM-yy HH:mm:ss.000000") : "01-Jan-00 00:00:00.000000";
                                        p26.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p26);

                                        pCount++;//27
                                        OracleParameter p27 = new OracleParameter();
                                        p27.ParameterName = "p_course_time_end";
                                        p27.Value = e._course_time_end_dt.HasValue ? (e._course_time_end_dt.Value.ToString("dd-MM-yy HH:mm:ss") == "01-01-01 00:00:00" ? null : e._course_time_end_dt.Value.ToString("dd-MM-yyyy HH:mm:ss")) : "01-01-01 00:00:00";
                                        p27.OracleType = OracleType.VarChar;
                                        orcCmd.Parameters.Add(p27);

                                        pCount++;//28
                                        OracleParameter p28 = new OracleParameter();
                                        p28.ParameterName = "p_is_synced";
                                        p28.Value = 0;
                                        p28.OracleType = OracleType.Number;
                                        orcCmd.Parameters.Add(p28);

                                        //for (int i = 0; i < prm.Length; i++)
                                        //{
                                        //    //orcCmd.Parameters.Add(prm[i]);
                                        //}

                                        int counter = ValidateCourseAttendanceExists(strORCCMSConnString, e._id);
                                        if (counter > 0)
                                        {
                                            Console.WriteLine("TRMS-Export: CourseAttendance - Yes EXISTS");

                                            int deleted = DeleteExistedCourseAttendance(strORCCMSConnString, e._id);
                                            if (deleted == 1)
                                            {
                                                Console.WriteLine("TRMS-Export: CourseAttendance - DELETED");
                                            }
                                        }

                                        orcCon.Open();
                                        int response = orcCmd.ExecuteNonQuery();
                                        if (response > 0)
                                        {
                                            addedCount++;
                                            insertedCAIds.Add(e._id);
                                        }
                                        orcCon.Close();

                                        Console.WriteLine("TRMS-Export: CourseAttendance - ADDED");
                                    }
                                }

                            }//for ended

                            orcCon.Dispose();
                        }

                        #endregion
                    }
                }
                else
                {
                    Console.WriteLine("TRMS-Export: CourseAttendance - NO Data Found");
                }
                #endregion

                //STEP CourseAttendances 3/3: Update Status in DB table
                #region step_03
                strStep = "3"; cutOff = 0;               

                if (insertedCAIds != null && insertedCAIds.Count > 0)
                {
                    Console.WriteLine("TRMS-Export: STEP 03 - Setting Sync Flag OK");

                    using (var db = new DLL.Models.Context())
                    {
                        foreach (var entity in insertedCAIds)
                        {
                            var record = db.organization_course_attendance.Where(m => m.id.Equals(entity)).FirstOrDefault();
                            if (record != null)
                            {
                                record.is_sent = true;

                                count3++;
                                cutOff++;

                                //save to db
                                if (cutOff == iLogSave)
                                {
                                    cutOff = 0;
                                    db.SaveChanges();
                                }
                            }
                        }

                        db.SaveChanges();
                    }
                }

                #endregion

                //log message to process completed
                if (isTTLogAllowed)
                {
                    using (var db3 = new DLL.Models.Context())
                    {
                        db3.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "TRMS-Export: Completed",
                            LogDateTime = DateTime.Now,
                            LogMessageText = "TRMS-Export: All Steps Completed"
                        });
                        db3.SaveChanges();
                    }
                }

                Console.WriteLine("TRMS-Export: ALL Steps Completed!");
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
                            LogTitle = "TRMS-Export: Exception " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "TRMS-Export: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText
                        });

                        db.SaveChanges();
                        Console.WriteLine("TRMS-Export: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText);
                    }
                }
            }

            Console.WriteLine("\n\n\nTRMS-Export: EXIT Now...");
            Console.ReadKey();
        }

        public static int ValidateCourseAttendanceExists(string conString, int ca_id)
        {
            // used to return the country name
            int iCounter = 0;
            string strCounter = "";

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString; // "context connection=true";

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "select count(*) Counter from INTR_TRMS_COURSE_ATTENDANCES where ca_id=:1";
            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1";
            p01.Value = ca_id;
            p01.OracleType = OracleType.Number;
            cmd.Parameters.Add(p01);
            //cmd.Parameters.Add(":1", OracleDbType.Int32, ca_id, ParameterDirection.Input);

            // get a scalar value
            con.Open();
            strCounter = cmd.ExecuteScalar().ToString();
            iCounter = int.Parse(strCounter);
            con.Close();

            // clean up objects
            cmd.Dispose();
            con.Dispose();

            // Return the counter
            return iCounter;
        }

        public static int DeleteExistedCourseAttendance(string conString, int ca_id)
        {
            // used to return the country name
            int iDeleted = 0;

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString;

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM INTR_TRMS_COURSE_ATTENDANCES WHERE ca_id=:1";
            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1";
            p01.Value = ca_id;
            p01.OracleType = OracleType.Number;
            cmd.Parameters.Add(p01);
            //cmd.Parameters.Add(":1", OracleDbType.Int32, ca_id, ParameterDirection.Input);

            // get execute non query
            con.Open();
            iDeleted = cmd.ExecuteNonQuery();
            con.Close();

            // get the country name from the data reade
            cmd.Dispose();
            con.Dispose();

            // Return the status
            return iDeleted;
        }


        //TESTING Methods: Code found from site: https://developer.oracle.com/dotnet/williams-sps.html
        public static string GetCountryName(string conString, string CountryCode)
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
            cmd.CommandText = "select country_name from countries where country_id = :1";
            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1";
            p01.Value = CountryCode;
            p01.OracleType = OracleType.VarChar;
            cmd.Parameters.Add(p01);
            //cmd.Parameters.Add(":1", OracleDbType.Varchar2, CountryCode, ParameterDirection.Input);

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

        public static string InsertRegionName(string conString, string Region_Name_01, string Region_Name_02, string Region_Name_03)
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
            //cmd.CommandText = "select region_name from regions where region_id = :1";

            cmd.CommandText = "INSERT INTO Regions (region_name) VALUES (:1)";
            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1";
            p01.Value = Region_Name_01;
            p01.OracleType = OracleType.VarChar;
            cmd.Parameters.Add(p01);
            //cmd.Parameters.Add(":1", OracleDbType.Varchar2, Region_Name_01, ParameterDirection.Input);
            ////cmd.Parameters.Add(":2", OracleDbType.Varchar2, Region_Name_02, ParameterDirection.Input);
            ////cmd.Parameters.Add(":3", OracleDbType.Varchar2, Region_Name_03, ParameterDirection.Input);

            // get a data reader
            int rdr = cmd.ExecuteNonQuery();

            // get the country name from the data reade
            cmd.Dispose();

            // Return the country name
            return CountryName;
        }

        public static string InsertALLRegionName(string conString, string Region_Name_01, string Region_Name_02, string Region_Name_03)
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
            //cmd.CommandText = "select region_name from regions where region_id = :1";

            cmd.CommandText = "INSERT ALL INTO Regions (region_name) VALUES (:1) INTO Regions (region_name) VALUES (:2) INTO Regions (region_name) VALUES (:3) SELECT * FROM dual";
            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1";
            p01.Value = Region_Name_01;
            p01.OracleType = OracleType.VarChar;
            cmd.Parameters.Add(p01);
            //cmd.Parameters.Add(":1", OracleDbType.Varchar2, Region_Name_01, ParameterDirection.Input);

            OracleParameter p02 = new OracleParameter();
            p02.ParameterName = ":1";
            p02.Value = Region_Name_02;
            p02.OracleType = OracleType.VarChar;
            cmd.Parameters.Add(p02);
            //cmd.Parameters.Add(":2", OracleDbType.Varchar2, Region_Name_02, ParameterDirection.Input);

            OracleParameter p03 = new OracleParameter();
            p03.ParameterName = ":1";
            p03.Value = Region_Name_03;
            p03.OracleType = OracleType.VarChar;
            cmd.Parameters.Add(p03);
            //cmd.Parameters.Add(":3", OracleDbType.Varchar2, Region_Name_03, ParameterDirection.Input);

            // get a data reader
            int rdr = cmd.ExecuteNonQuery();

            // get the country name from the data reade
            cmd.Dispose();

            // Return the country name
            return CountryName;
        }

        public static string DeleteRegionName(string conString, int Region_ID_01, int Region_ID_02, int Region_ID_03)
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
            //cmd.CommandText = "select region_name from regions where region_id = :1";

            cmd.CommandText = "DELETE FROM Regions WHERE Region_ID=:1";
            OracleParameter p01 = new OracleParameter();
            p01.ParameterName = ":1";
            p01.Value = Region_ID_01;
            p01.OracleType = OracleType.Int32;
            cmd.Parameters.Add(p01);
            //cmd.Parameters.Add(":1", OracleDbType.Int64, Region_ID_01, ParameterDirection.Input);
            ////cmd.Parameters.Add(":2", OracleDbType.Int32, Region_ID_02, ParameterDirection.Input);
            ////cmd.Parameters.Add(":3", OracleDbType.Int32, Region_ID_03, ParameterDirection.Input);

            // get a data reader
            int rdr = cmd.ExecuteNonQuery();

            // get the country name from the data reade
            cmd.Dispose();

            // Return the country name
            return CountryName;
        }

        public static void WriteToConsoleAudiLog(string strLTitleText)
        {
            if (isStaticTTLogAllowed)
            {
                using (var dbTT = new DLL.Models.Context())
                {
                    dbTT.log_message.Add(new DLL.Models.LogMessage
                    {
                        LogTitle = "TRMS-Export:",
                        LogDateTime = DateTime.Now,
                        LogMessageText = "Message: " + strLTitleText
                    });
                    dbTT.SaveChanges();

                    Console.WriteLine("TRMS-Export: Message: " + strLTitleText);
                }
            }
        }

        public class CA_Data
        {
            public int _id { get; set; }
            public DateTime _schedule_date_dt { get; set; }
            public string _schedule_date { get; set; }
            public int _student_id { get; set; }
            public string _student_code { get; set; }
            public string _student_name { get; set; }
            public int _student_group_id { get; set; }
            public string _student_group { get; set; }
            public int _student_pshift_id { get; set; }
            public string _student_pshift { get; set; }
            public int _course_id { get; set; }
            public string _course_code { get; set; }
            public string _course_title { get; set; }

            public DateTime? _time_in_dt { get; set; }
            public string _time_in { get; set; }
            public string _status_in { get; set; }
            public DateTime? _time_out_dt { get; set; }
            public string _time_out { get; set; }
            public string _status_out { get; set; }
            public string _final_remarks { get; set; }

            public int _terminal_in_id { get; set; }
            public string _terminal_in { get; set; }
            public int _terminal_out_id { get; set; }
            public string _terminal_out { get; set; }

            public DateTime? _course_time_start_dt { get; set; }
            public string _course_time_start { get; set; }
            public DateTime? _course_time_end_dt { get; set; }
            public string _course_time_end { get; set; }
        }

    }
}



/*
                //TESTING CODE - Orcale Database Connection
                 <add name="ORC_CMS" connectionString="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=pdborcl))); User ID=ot; Password=Orcl1234;" />

                using (OracleConnection orcCon = new OracleConnection(strORCCMSConnString))
                {
                    WriteToConsoleAudiLog("Table - Entry Logs ID Inserting...");

                    DateTime dtNow = DateTime.Now;
                    string strSQLQueryCMS1 = "INSERT INTO TBL_Names (" +
                            "ID," +
                            "NAME," +
                            "CREATEDATETIME" +
                    ") VALUES (" +
                            "4," +
                            "'Ali Rehman'," +
                            "TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd") + "'))" +
                            ";"; // "; SELECT TOP 1 * ORDER BY DESC;";
                                 //INSERT INTO TBL_Names (ID,NAME,CREATEDATETIME) VALUES (1,'Inayat Rehman',TO_DATE('2020-07-29','YYYY-MM-DD'));

                    string strSQLQueryCMS2 = "INSERT INTO TBL_NAMES (" +
                          "ID," +
                          "NAME" +
                          "" +
                  ") VALUES (" +
                          "2," +
                          "'Ali Rehman')" +
                          // "TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd") + "'))" +
                          ""; // "; SELECT TOP 1 * ORDER BY DESC;";
                              //INSERT INTO TBL_Names (ID,NAME,CREATEDATETIME) VALUES (1,'Inayat Rehman',TO_DATE('2020-07-29','YYYY-MM-DD'));

                    string strSQLQueryCMS = "SELECT FIRST_NAME FROM CONTACTS ORDER BY CONTACT_ID";

                    //using System.Data.Odbc;

                    //String connectionString = "Dsn=HP5ODBC;uid=system;pwd=manager";
                    //OdbcConnection coon = new OdbcConnection();
                    //coon.ConnectionString = connectionString;
                    //coon.Open();
                    //MessageBox.Show(coon.State.ToString());



                    ////Now create an OdbcConnection object and connect it to our sample data source.
                    //OdbcConnection DbConnection = new OdbcConnection("DSN=oracle_odbc");
                    ////This example uses a DSN connection, but a DSN free connection can be used by using the DRIVER = { } format.Consult your ODBC driver documentation for more details.
                    ////Now open that OdbcConnection object:
                    //DbConnection.Open();
                    ////Now using the OdbcConnection, we will create an OdbcCommand:
                    //OdbcCommand DbCommand = DbConnection.CreateCommand();
                    ////Using that command, we can issue a Query and create an OdbcDataReader:
                    ////DbCommand.CommandText = "SELECT * FROM TBLNAMES";
                    ////OdbcDataReader DbReader = DbCommand.ExecuteReader();

                    //DbCommand.CommandText = "SELECT ID FROM TBLNAMES";
                    //string strReader = DbCommand.ExecuteScalar().ToString();

                    using (OracleCommand orcCmd = new OracleCommand(strSQLQueryCMS, orcCon))
                    {
                        orcCmd.CommandTimeout = 600;
                        orcCon.Open();
                        //OracleString dbReturnID = "";
                        //orcCmd.ExecuteOracleNonQuery(out dbReturnID);

                        string dbReturnID = orcCmd.ExecuteOracleScalar().ToString();
                        orcCon.Close();
                    }

                }

    
                                strSQLQueryCMS += " SELECT " +
                                       "TO_DATE('" + e._schedule_date_dt.ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI','NLS_DATE_LANGUAGE=AMERICAN')," +//TO_DATE('2020-07-29','YYYY-MM-DD')
                                       "'" + e._schedule_date_dt.ToString("dd-MM-yyyy") + "'," +
                                       "'" + e._student_id + "'," +
                                       "'" + e._student_code + "'," +
                                       "'" + e._student_name + "'," +
                                       "'" + e._student_group_id + "'," +
                                       "'" + e._student_group + "'," +
                                       "'" + e._student_pshift_id + "'," +
                                       "'" + e._student_pshift + "'," +
                                       "'" + e._course_id + "'," +
                                       "'" + e._course_code + "'," +
                                       "'" + e._course_title + "'," +
                                       "TO_DATE('" + e._time_in_dt.Value.ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI','NLS_DATE_LANGUAGE=AMERICAN')," +
                                       "'" + e._time_in_dt.Value.ToString("dd-MM-yyyy") + "'," +
                                       "'" + e._status_in + "'," +
                                       "TO_DATE('" + e._time_out_dt.Value.ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI','NLS_DATE_LANGUAGE=AMERICAN')," +
                                       "'" + e._time_out_dt.Value.ToString("dd-MM-yyyy") + "'," +
                                       "'" + e._status_out + "'," +
                                       "'" + e._final_remarks + "'," +
                                       "'" + e._terminal_in_id + "'," +
                                       "'" + e._terminal_in + "'," +
                                       "'" + e._terminal_out_id + "'," +
                                       "'" + e._terminal_out + "'," +
                                       "TO_DATE('" + e._course_time_start_dt.Value.ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI','NLS_DATE_LANGUAGE=AMERICAN')," +
                                       "'" + e._course_time_start_dt.Value.ToString("dd-MM-yyyy") + "'," +
                                       "TO_DATE('" + e._course_time_end_dt.Value.ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI','NLS_DATE_LANGUAGE=AMERICAN')," +
                                       "'" + e._course_time_end_dt.Value.ToString("dd-MM-yyyy") + "'" +
                                       " FROM dual";
									   
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
										CREATE TABLE TRMS_OrgCourseAttendances
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
											
											INSERT INTO ot.trms_orgcourseattendances 
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






										/////////////////////////////////////////////////////////////////






										/////////////////////////////////////////////////////////////////
*/
