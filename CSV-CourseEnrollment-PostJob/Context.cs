using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_CourseEnrollment_PostJob
{
    public class ContextTRMSDataLogMaster : DbContext
    {
        public static string connectionStringTRMSDataLogMaster = ConfigurationManager.ConnectionStrings["TimeTune"].ConnectionString;
        public static string connectionStringORC_CMS = ConfigurationManager.ConnectionStrings["ORC_CMS"].ConnectionString;

        public ContextTRMSDataLogMaster()
            : base(connectionStringTRMSDataLogMaster)
        {
            // : base("Server=10.200.65.194; Database=TimeTune; User Id=sa; Password=resco123!!")
            //public Context():base("Server=192.168.0.229; Database=TimeTune; User Id=sa; Password=mngr")
            //this.Configuration.LazyLoadingEnabled = true;

            //if (ConfigurationManager.AppSettings["DBTimeOut"] != null && ConfigurationManager.AppSettings["DBTimeOut"].ToString() != "")
            //{
            //    this.Database.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["DBTimeOut"].ToString()); //600 = 10 * 60 = 10 min
            //}
        }

        public DbSet<OrganizationStudentEnrollment> organization_student_enrollment { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }


    #region trms_data_log

    public class CMSStudentEnrollment
    {
        public int Id { get; set; }       
        public string CalendarYear { get; set; }
        public string CampusCode { get; set; }
        public string ProgramCode { get; set; }
        public string StudentCode { get; set; }
        public string CourseCode { get; set; }
        public string EnrTitle { get; set; }
        public string EnrProgramType { get; set; }
        public int EnrProgramTypeNumber { get; set; }
        public string IsCourseFailed { get; set; }
        public string CreateDateEnr { get; set; }
        public string TRMS_IDs { get; set; }
        public string TRMS_Synced { get; set; }
    }

    public class OrganizationStudentEnrollment
    {
        public int Id { get; set; }
        public int GeneralCalendarId { get; set; }
        public int CampusId { get; set; }
        public int ProgramId { get; set; }
        public int EmployeeStudentId { get; set; }
        public int ProgramCourseId { get; set; }
        public string EnrollmentTitle { get; set; }
        public int EnrolledProgramTypeId { get; set; }
        public int EnrolledProgramTypeNumber { get; set; }
        public bool IsCourseFailed { get; set; }
        public DateTime CreateDateEnr { get; set; }
    }

    #endregion

}
