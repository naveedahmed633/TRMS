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
//using Oracle.DataAccess.Client;
using System.Data.OracleClient;

namespace CSV_CourseTest_PostJob
{
    class Program
    {
        public static bool isStaticTTLogAllowed = false;

        static void Main(string[] args)
        {
            int zero = 0, iLogSave = 0, cutOff = 0;
            int count1 = 0, count2 = 0, count3 = 0;

            bool isTTLogAllowed = false;
            string iCSVorDBImport = "0", strORCCMSConnString = "", strOracleSQLQuery = "";
            string strFilePath = "", strFileName = "", strUNISRegionID = "2", strLTitleText = "", strStep = "";
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
                    #region 'StudentTest' table - Read CMS-Oracle-DB and Post Data to TRMS-DB

                    //STEP Schedule 1/3: Get CMS Schedule Table Data
                    strStep = "1";
                    Console.WriteLine("TRMS-Test: Class-Test - Get Test Data Only");

                    List<CMSStudentEnrollment> enrCMSList = new List<CMSStudentEnrollment>();
                    enrCMSList = CMS_GetTestData(strORCCMSConnString, strOracleSQLQuery);
                    if (enrCMSList != null && enrCMSList.Count > 0)
                    {
                        foreach (var enrCMS in enrCMSList)
                        {
                            string str01 = enrCMS.CampusCode;
                            string str02 = enrCMS.ProgramCode;
                            string str03 = enrCMS.CourseCode;
                            string str04 = enrCMS.EnrProgramType;
                            Console.WriteLine("TRMS-Test: Data01=" + str01 + " - " + str02 + " - " + str03 + " - " + str04);
                        }
                    }
                    else
                    {
                        Console.WriteLine("TRMS-Test: NO Data Found");
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
                            LogTitle = "TRMS-Test: Completed",
                            LogDateTime = DateTime.Now,
                            LogMessageText = "TRMS-Test: All Steps Completed"
                        });
                        db3.SaveChanges();
                    }
                }

                Console.WriteLine("TRMS-Test: ALL Steps Completed!");
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
                            LogTitle = "TRMS-Test: Exception " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "TRMS-Test: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText
                        });

                        db.SaveChanges();
                        Console.WriteLine("TRMS-Test: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText);
                    }
                }
            }

            Console.WriteLine("\n\n\nTRMS-Test: EXIT Now...");
            Console.ReadKey();
        }

        ///////////////////// Oracle using Methods //////////////////////

        public static List<CMSStudentEnrollment> CMS_GetTestData(string conString, string strOracleSQLQuery)
        {
            List<CMSStudentEnrollment> enrCMSList = new List<CMSStudentEnrollment>();

            // Get a connection to the db
            // context connection is used in a stored procedure
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString; // "context connection=true";
            con.Open();

            // Create command and parameter objects
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = strOracleSQLQuery; // "select * from intr_cms_student_enrollments order by id";
            //cmd.Parameters.Add(":1", OracleDbType.Int32, 1, ParameterDirection.Input);
            //cmd.Parameters.Add(":1", OracleDbType.Int32, ca_id, ParameterDirection.Input);

            // get a data reader
            OracleDataReader _SqlDataReader = cmd.ExecuteReader();

            // get the country name from the data reader
            if (_SqlDataReader != null && _SqlDataReader.HasRows)
            {

                //COURSE    CLASS_NBR   SUBJECT     ACAD    DESCR               CATALOG_NB  ACAD    STRM       CAMPU   INSTI   MTG_START       MTG_END         M   T   W   T   F   S   S   START_DAT   END_DATE
                //002188    1016        HEMNGTSC    UGRD    Information Systems 202-BBA     IHM	   1808     DMC     DUHSP   09:00:00 AM     10:30:00 AM     N   Y   N   N   N   N   N   13-SEP-18   06-JAN-19

                int count = 0;
                // We have data in SqlDataReader, lets loop through and display the data
                while (_SqlDataReader.Read())
                {
                    CMSStudentEnrollment enrCMS = new CMSStudentEnrollment();

                    Console.Write("COURSE=" + _SqlDataReader[0].ToString() ?? "X"); Console.WriteLine("\t\t\t\tCLASS_NBR=" + _SqlDataReader[1].ToString() ?? "X");
                    Console.Write("SUBJECT=" + _SqlDataReader[2].ToString() ?? "X"); Console.WriteLine("\t\t\tACAD=" + _SqlDataReader[3].ToString() ?? "X");
                    Console.Write("DESCR=" + _SqlDataReader[4].ToString() ?? "X"); Console.WriteLine("\t\tCATALOG_NB=" + _SqlDataReader[5].ToString() ?? "X");
                    Console.Write("ACAD_=" + _SqlDataReader[6].ToString() ?? "X"); Console.WriteLine("\t\t\t\tSTRM=" + _SqlDataReader[7].ToString() ?? "X");
                    Console.Write("CAMPUS=" + _SqlDataReader[8].ToString() ?? "X"); Console.WriteLine("\t\t\t\tINSTI=" + _SqlDataReader[9].ToString() ?? "X");
                    Console.Write("MTG_START=" + _SqlDataReader[10].ToString() ?? "X"); Console.WriteLine("\t\t\tMTG_END=" + _SqlDataReader[11].ToString() ?? "X");
                    Console.Write("M=" + _SqlDataReader[12].ToString() ?? "X"); Console.WriteLine("\t\t\t\t\tT=" + _SqlDataReader[13].ToString() ?? "X");
                    Console.Write("W=" + _SqlDataReader[14].ToString() ?? "X"); Console.WriteLine("\t\t\t\t\tT=" + _SqlDataReader[15].ToString() ?? "X");
                    Console.Write("F=" + _SqlDataReader[16].ToString() ?? "X"); Console.WriteLine("\t\t\t\t\tS=" + _SqlDataReader[17].ToString() ?? "X");
                    Console.WriteLine("S=" + _SqlDataReader[18].ToString() ?? "X");
                    Console.Write("START_DATE=" + _SqlDataReader[19].ToString() ?? "X"); Console.WriteLine("\t\tEND_DATE=" + _SqlDataReader[20].ToString() ?? "X");

                    enrCMS.CampusCode = _SqlDataReader[0].ToString() ?? "";
                    enrCMS.ProgramCode = _SqlDataReader[1].ToString() ?? "";
                    enrCMS.StudentCode = _SqlDataReader[2].ToString() ?? "";
                    enrCMS.EnrProgramType = _SqlDataReader[3].ToString() ?? "";

                    enrCMSList.Add(enrCMS);

                    Console.WriteLine("\n-----------------------------------------------------\n");

                    if (count >= 4)
                    {
                        break;
                    }

                    count++;
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

        ///////////////////// Logging Methid ////////////////////////////
        public static void WriteToConsoleAudiLog(string strLTitleText)
        {
            if (isStaticTTLogAllowed)
            {
                using (var dbTT = new DLL.Models.Context())
                {
                    dbTT.log_message.Add(new DLL.Models.LogMessage
                    {
                        LogTitle = "TRMS-Test:",
                        LogDateTime = DateTime.Now,
                        LogMessageText = "Message: " + strLTitleText
                    });
                    dbTT.SaveChanges();

                    Console.WriteLine("TRMS-Test: Message: " + strLTitleText);
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
