using System.Configuration;
using DLL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DLL.Models
{
    public class Context : DbContext
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["TimeTune"].ConnectionString;
        public Context()
            : base(connectionString)
        {
            this.Database.CommandTimeout = 600; //10 min
            Database.SetInitializer<Context>(null);
            // : base("Server=10.200.65.194; Database=TimeTune; User Id=sa; Password=resco123!!")
            //public Context():base("Server=192.168.0.229; Database=TimeTune; User Id=sa; Password=mngr")
            //this.Configuration.LazyLoadingEnabled = true;
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    //base.OnModelCreating(modelBuilder);
        //}

        public DbSet<AccessCodeValue> access_code_value { get; set; }
        public DbSet<AccessGroup> access_group { get; set; }
        public DbSet<ConsolidatedAttendance> consolidated_attendance { get; set; }
        public DbSet<ConsolidatedAttendancesArchive> consolidated_attendance_su { get; set; }
        public DbSet<AttendanceRemarksRegister> attendance_remarks_register { get; set; }
        public DbSet<GroupCalendar> group_calendar { get; set; }
        public DbSet<Department> department { get; set; }
        public DbSet<Designation> designation { get; set; }
        public DbSet<Employee> employee { get; set; }
        public DbSet<EmployeeForgotPassword> employee_forgotpassword { get; set; }
        //public DbSet<Employee_SSO> employee_sso { get; set; }
        public DbSet<PersistentAttendanceLog> persistent_attendance_log { get; set; }
        public DbSet<Function> function { get; set; }
        public DbSet<Grade> grade { get; set; }
        public DbSet<Group> group { get; set; }
        public DbSet<ManualAttendance> manual_attendance { get; set; }
        public DbSet<Region> region { get; set; }
        public DbSet<Remark> remarks { get; set; }
        public DbSet<Shift> shift { get; set; }
        public DbSet<DLL.Models.GroupCalendarOverride> group_calendar_overrides { get; set; }
        public DbSet<GeneralCalendar> general_calender { get; set; }
        public DbSet<GeneralCalendarOverride> general_calender_override { get; set; }
        public DbSet<TypeOfEmployment> type_of_employment { get; set; }
        public DbSet<Location> location { get; set; }
        public DbSet<AuditTrail> audit_trail { get; set; }
        public DbSet<HaTransit> ha_transit { get; set; }
        public DbSet<ShiftToGroupRegister> shift_to_group_register { get; set; }
        public DbSet<ManualGroupShiftAssigned> manual_group_shift_assigned { get; set; }
        public DbSet<Terminals> termainal { get; set; }
        public DbSet<SLM> super_line_manager_tagging { get; set; }
        public DbSet<ContractualStaff> contractual_staff { get; set; }
        public DbSet<CS_AttendanceLog> cs_persistent_log { get; set; }
        public DbSet<CS_PersistentLog> cs_consolidated_log { get; set; }
        public DbSet<FutureManualAttendance> futureManualAttendance { get; set; }
        public DbSet<PermissionUser> permissionUser { get; set; }
        public DbSet<PermissionHR> permissionHR { get; set; }
        public DbSet<PermissionLM> permissionLM { get; set; }
        public DbSet<UserTracking> user_Tracking { get; set; }
        public DbSet<ManualAttLog> manAttLog { get; set; }
        public DbSet<HaTransitsUnique> ha_transitUnique { get; set; }
        public DbSet<PermissionSuperAdmin> per_superAdmin { get; set; }
        public DbSet<OrgInformation> orgInfo { get; set; }

        //HR
        public DbSet<SkillsSets> skills_set { get; set; }
        public DbSet<EmployeeEvaluation> emp_evaluation { get; set; }
        public DbSet<Letter> emp_letter { get; set; }

        //IR Added NEW Tables - Leaves Section
        public DbSet<LeaveApplication> leave_application { get; set; }
        public DbSet<LeaveReason> leave_reason { get; set; }
        public DbSet<LeaveType> leave_type { get; set; }
        public DbSet<LeaveStatus> leave_status { get; set; }
        public DbSet<LeaveSession> leave_session { get; set; }

        //IR Added NEW Tables - Payrolls Section
        public DbSet<Payroll> payroll { get; set; }
        public DbSet<PayrollAmount> payroll_amount { get; set; }
        public DbSet<BankName> bank_name { get; set; }
        public DbSet<PaymentStatusType> payment_status { get; set; }
        public DbSet<PaymentModeType> payment_mode { get; set; }

        //IR - Loan Section
        public DbSet<Loan> loan { get; set; }
        public DbSet<LoanType> loan_type { get; set; }
        public DbSet<LoanStatusType> loan_status_type { get; set; }


        //IR - Organization Section
        public DbSet<Organization> organization { get; set; }
        public DbSet<OrganizationCampus> organization_campus { get; set; }
        public DbSet<OrganizationCampusType> organization_campus_type { get; set; }
        public DbSet<OrganizationCampusBuilding> organization_campus_building { get; set; }
        public DbSet<OrganizationCampusBuildingRoom> organization_campus_building_room { get; set; }
        public DbSet<OrganizationCampusRoomCourseSchedule> organization_campus_room_course_schedule { get; set; }

        public DbSet<OrganizationCampusProgram> organization_campus_program { get; set; }
        public DbSet<OrganizationProgramCategory> organization_program_category { get; set; }
        public DbSet<OrganizationProgramType> organization_program_type { get; set; }
        public DbSet<OrganizationProgramShift> organization_program_shift { get; set; }
        public DbSet<StateProvince> state_province { get; set; }
        //public DbSet<Region> organization_lecture_group { get; set; }

        public DbSet<OrganizationProgram> organization_program { get; set; }
        public DbSet<OrganizationProgramCourse> organization_program_course { get; set; }
        public DbSet<OrganizationProgramCourseEnrollment> organization_program_course_enrollment { get; set; }

        public DbSet<OrganizationCourseAttendance> organization_course_attendance { get; set; }
        //public DbSet<OrganizationCourseAttendanceLog> organization_course_attendance_log { get; set; }

        public DbSet<LogMessage> log_message { get; set; }

        public DbSet<Gender> gender { get; set; }

        public DbSet<GeoPhencingTerminal> geo_phencing_terminal { get; set; }

        public DbSet<ExemptedEmployee> exempted_employee { get; set; }
        public DbSet<CMS_ClassSchedule> cms_class_schedule { get; set; }

        public DbSet<PermissionReport> permission_report { get; set; }

        public DbSet<LeaveValidity> leave_validity { get; set; }

        public DbSet<PositionStatus> position_status { get; set; }

        public DbSet<SiteStatus> site_status { get; set; }

        public DbSet<StringResource> string_resource { get; set; }
    }



}
