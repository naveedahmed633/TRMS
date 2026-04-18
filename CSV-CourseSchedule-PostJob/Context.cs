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

namespace CSV_CourseSchedule_PostJob
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

        public DbSet<OrganizationClassSchedule> organization_class_schedule { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }


    #region trms_data_log

    public class CMSClassSchedule
    {
        public int Id { get; set; }       
        public string Course_Id { get; set; }
        public string Class_Nbr { get; set; }
        public string Subject { get; set; }
        public string Career { get; set; }
        public string Descr { get; set; }
        public string Catalog { get; set; }
        public string Acad_Group { get; set; }
        public string Term { get; set; }
        public string Campus { get; set; }
        public string Institution { get; set; }
        public string Mtg_Start { get; set; }
        public string Mtg_End { get; set; }
        public string Mon { get; set; }
        public string Tues { get; set; }
        public string Wed { get; set; }
        public string Thurs { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public string TRMS_IDs { get; set; }
        public string TRMS_Synced { get; set; }
    }

    public class OrganizationClassSchedule
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int CourseId { get; set; }
        public int LectureGroupId { get; set; }
        public int ShiftId { get; set; }
        public string StudyTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EmployeeTeacherId { get; set; }
        public int ProgramId { get; set; }
        public int CampusId { get; set; }
        public DateTime CreateDateSch { get; set; }
    }

    #endregion

}
