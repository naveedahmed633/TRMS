using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace ViewModels
{
    #region Organization-Forms


    public class OrganizationForm
    {
        public int id { get; set; }
        public string organization_title { get; set; }
        public string established_date { get; set; }
        public int campus_limit { get; set; }
        public string logo_file_path { get; set; }
        public string website_url { get; set; }
        public string description { get; set; }
    }

    public class OrganizationCampusForm
    {
        public int id { get; set; }
        public int organization_id { get; set; }
        public int campus_type_id { get; set; }
        public string campus_code { get; set; }
        public string campus_title { get; set; }
        public string email_address { get; set; }
        public string address { get; set; }
        public int city_id { get; set; }
        public int state_province_id { get; set; }
        public string zip_code { get; set; }
        public string phone_01 { get; set; }
        public string phone_02 { get; set; }
        public string fax_number { get; set; }
    }

    public class OrganizationCampusBuildingForm
    {
        /*
      [Id]
      ,[CampusId]
      ,[BuildingCode]
      ,[BuildingTitle]
      ,[CreateDateBld]
         */
        public int id { get; set; }
        public int campus_id { get; set; }
        public string building_code { get; set; }
        public string building_title { get; set; }
    }


    public class OrganizationCampusBuildingRoomForm
    {
        public int id { get; set; }
        public int building_id { get; set; }
        public string room_code { get; set; }
        public string room_title { get; set; }
        public int terminal_id { get; set; }
        public int floor_number { get; set; }
    }

    public class OrganizationCampusRoomCourseScheduleForm
    {
        public int id { get; set; }
        public int room_id { get; set; }
        public int course_id { get; set; }
        public int lecture_group_id { get; set; }
        public string study_title { get; set; }
        public string start_date { get; set; }
        public string start_hr { get; set; }
        public string start_mn { get; set; }
        public string start_ap { get; set; }
        public string end_date { get; set; }
        public string end_hr { get; set; }
        public string end_mn { get; set; }
        public string end_ap { get; set; }

        public string from_date { get; set; }

        public string to_date { get; set; }

        public int employee_teacher_id { get; set; }
        public int shift_id { get; set; }
        public int program_id { get; set; }
        public int campus_id { get; set; }
    }

    public class OrganizationProgramForm
    {
        /*
        [Id]
      ,[CategoryId]
      ,[ProgramCode]
      ,[ProgramTitle]
      ,[DisciplineName]
      ,[CreditHours]
      ,[WholeProgramTypeId]
      ,[WholeProgramTypeNumber]
      ,[CreateDatePrg]
         */
        public int id { get; set; }
        public int category_id { get; set; }
        public string program_code { get; set; }
        public string program_title { get; set; }
        public string discipline_name { get; set; }
        public int credit_hours { get; set; }
        public int whole_program_type_id { get; set; }
        public int whole_program_type_number { get; set; }
    }


    public class OrganizationCampusProgramForm
    {
        public int id { get; set; }
        public int campus_id { get; set; }
        public int program_id { get; set; }
        public bool is_active_program { get; set; }
    }

    public class OrganizationProgramCourseForm
    {
        public int id { get; set; }
        public int program_id { get; set; }
        public string course_code { get; set; }
        public string course_title { get; set; }
        public string book_name { get; set; }
        public string book_author { get; set; }
        public int default_program_type_id { get; set; }
        public int default_program_type_number { get; set; }
        public int credit_hours { get; set; }
        public int passing_marks { get; set; }
        public int total_marks { get; set; }
        public bool is_active_course { get; set; }
    }


    public class OrganizationProgramCourseEnrollmentForm
    {
        public int id { get; set; }
        public int general_calendar_id { get; set; }
        public int campus_id { get; set; }
        public int program_id { get; set; }
        public int employee_student_id { get; set; }
        public int program_course_id { get; set; }
        public string enrollment_title { get; set; }
        public int enrolled_program_type_id { get; set; }
        public int enrolled_program_type_number { get; set; }
        public bool is_course_failed { get; set; }


        public int year_id { get; set; }
    }



    public class OrganizationProgramShiftForm
    {
        public int id { get; set; }
        public string program_shift_name { get; set; }

        public int fn_id { get; set; }
        public string name { get; set; }
    }


    public class OrganizationCourseAttendanceStudentForm
    {
        public int id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int student_id { get; set; }
        public int course_id { get; set; }
        public string schedule_date { get; set; }
        public string student_code { get; set; }
        public string course_code { get; set; }
        public string ddl_remarks { get; set; }
    }



    #endregion



    #region Organization-Views


    public class OrganizationView
    {
        public int Id { get; set; }
        public string OrganizationTitle { get; set; }
        public DateTime EstablishedDate { get; set; }
        public int CampusLimit { get; set; }
        public string Logo { get; set; }
        public string WebsiteURL { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateOrg { get; set; }

        public string EstablishedDateText { get; set; }
        public string CreateDateOrgText { get; set; }

        public string actions { get; set; }
    }

    public class OrganizationStudentsView
    {
        public int YearId { get; set; }
        public int CampusId { get; set; }
        public int ProgramId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int LectureGroupId { get; set; }

        public string YearCode { get; set; }
        public string CampusCode { get; set; }
        public string ProgramCode { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string FatherName { get; set; }
        public string CourseCodes { get; set; }
        public string CourseTitles { get; set; }
        public string PGroupName { get; set; }
        public string PShiftName { get; set; }
        public string BirthDateText { get; set; }
        public string Career { get; set; }
        public string GenderText { get; set; }
        public string CreateDateText { get; set; }

        public List<GeneralCalendarView> list_years;

        public List<OrganizationCampusView> list_campuses;

        public List<OrganizationProgramView> list_programs;

        public List<OrganizationProgramTypeView> list_program_types;

        public List<OrganizationProgramShiftView> list_program_shifts;

        public List<LGRegionView> list_program_groups;

        //public List<EmployeeView> list_students;
        //public string str_students { get; set; }
    }

    public class OrganizationEmployeesByTypeView
    {
        public List<TypeOfEmployment> list_emp_types;

    }

    public class OrganizationEmployeesView
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string CampusCode { get; set; }
        public string DeptName { get; set; }
        public string DesgName { get; set; }
        public string BirthDateText { get; set; }
        public string GenderText { get; set; }
        public string FatherName { get; set; }
        public string PhotoPath { get; set; }
        public string CreateDateText { get; set; }

        public List<OrganizationCampusView> list_campuses;

        public List<Department> list_departments;

        public List<Designation> list_designations;

        public List<Location> list_locations;

        //public List<EmployeeView> list_students;
        //public string str_students { get; set; }
    }

    public class OrganizationCampusView
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CampusTypeId { get; set; }
        public string CampusCode { get; set; }
        public string CampusTitle { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public string ZipCode { get; set; }
        public string Phone01 { get; set; }
        public string Phone02 { get; set; }
        public string FaxNumber { get; set; }
        public DateTime CreateDateCmp { get; set; }

        public string OrganizationName { get; set; }
        public string CampusTypeName { get; set; }
        public string CityName { get; set; }
        public string StateProvinceName { get; set; }
        public string CreateDateCmpText { get; set; }

        public List<OrganizationView> list_organization;

        public List<OrganizationCampusTypeView> list_campus_type;

        public List<OrganizationProgramTypeView> list_organization_program_type;

        public List<LocationView> list_location;

        public List<StateProvinceView> list_state_province;

        public string str_campus_type { get; set; }

        public string actions { get; set; }
    }

    public class OrganizationCampusTypeView
    {
        public int Id { get; set; }
        public string CampusTypeName { get; set; }
    }


    public class LocationView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StateProvinceView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string active { get; set; }
    }


    public class LGRegionView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EmployeeView
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
    }

    public class GeneralCalendarView
    {
        public int Id { get; set; }
        public string YearName { get; set; }
    }


    public class GenderView
    {
        public int Id { get; set; }
        public string GenderName { get; set; }
    }


    public class OrganizationProgramView
    {
        /*
        [Id]
      ,[CategoryId]
      ,[ProgramCode]
      ,[ProgramTitle]
      ,[DisciplineName]
      ,[CreditHours]
      ,[WholeProgramTypeId]
      ,[WholeProgramTypeNumber]
      ,[CreateDatePrg]
         */
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramTitle { get; set; }
        public string DisciplineName { get; set; }
        public int CreditHours { get; set; }
        public int WholeProgramTypeId { get; set; }
        public int WholeProgramTypeNumber { get; set; }
        public DateTime CreateDatePrg { get; set; }

        public string CategoryName { get; set; }
        public string WholeTypeName { get; set; }
        public string CreateDatePrgText { get; set; }

        public List<OrganizationProgramCategoryView> list_category;

        public List<OrganizationProgramTypeView> list_whole_type;

        public string str_category { get; set; }
        public string str_whole_type { get; set; }

        public string actions { get; set; }
    }


    public class OrganizationProgramTypeView
    {
        /*
        [Id]
      ,[ProgramTypeName]
         */
        public int Id { get; set; }
        public string ProgramTypeName { get; set; }
    }


    public class OrganizationCampusProgramView
    {
        public int Id { get; set; }
        public int CampusId { get; set; }
        public int ProgramId { get; set; }
        public bool IsActiveProgram { get; set; }


        public string CampusName { get; set; }
        public string ProgramName { get; set; }
        public string IsActiveProgramName { get; set; }


        public List<OrganizationCampusView> list_campus;

        public List<OrganizationProgramView> list_program;

        public string str_campus { get; set; }
        public string str_program { get; set; }


        public string actions { get; set; }
    }


    public class OrganizationProgramCourseView
    {
        /*
        [Id]
     ,[ProgramId]
      ,[CourseCode]
      ,[CourseTitle]
      ,[BookName]
      ,[BookAuthor]
      ,[DefaultProgramTypeId]
      ,[DefaultProgramTypeNumber]
      ,[PassingMarks]
      ,[TotalMarks]
      ,[IsActiveCourse]
      ,[CreateDateCrs]
         */
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string BookName { get; set; }
        public string BookAuthor { get; set; }
        public int DefaultProgramTypeId { get; set; }
        public int DefaultProgramTypeNumber { get; set; }
        public int CreditHours { get; set; }
        public int PassingMarks { get; set; }
        public int TotalMarks { get; set; }
        public bool IsActiveCourse { get; set; }
        public DateTime CreateDateCrs { get; set; }


        public string ProgramTitle { get; set; }
        public string DefaultTypeName { get; set; }
        public string IsActiveCourseName { get; set; }
        public string CreateDateCrsText { get; set; }

        public List<OrganizationProgramView> list_program;

        public List<OrganizationProgramTypeView> list_default_type;

        public string str_program { get; set; }
        public string str_default_type { get; set; }

        public string actions { get; set; }
    }


    public class OrganizationProgramCourseEnrollmentView
    {
        /*
        [Id]
        ,[GeneralCalendarId]
      ,[EmployeeStudentId]
      ,[ProgramCourseId]
      ,[EnrollmentTitle]
      ,[EnrolledProgramTypeId]
      ,[EnrolledProgramTypeNumber]
      ,[IsCoursePassed]
      ,[CreateDateEnr]
         */
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

        public string GeneralCalendarYear { get; set; }
        public string CampusCode { get; set; }
        public string ProgramCode { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string ProgramCourseTitle { get; set; }
        public string ProgramTitle { get; set; }

        public string EnrolledTypeName { get; set; }
        public string IsCourseFailedName { get; set; }
        public string CreateDateEnrText { get; set; }

        public List<GeneralCalendarView> list_general_calendar;

        public List<EmployeeView> list_employee;

        public List<OrganizationProgramCourseView> list_program_course;

        public List<OrganizationProgramTypeView> list_enrolled_type;

        public List<GeneralCalendarView> list_year;

        public List<OrganizationCampusView> list_campus;

        public List<OrganizationProgramView> list_program;

        public string str_general_calnedar { get; set; }
        public string str_campus { get; set; }
        public string str_program { get; set; }
        public string str_employee { get; set; }
        public string str_program_course { get; set; }
        public string str_enrolled_type { get; set; }

        public string actions { get; set; }
    }


    public class OrganizationProgramCategoryView
    {
        /*
        [Id]
      ,[ProgramCategoryName]
         */
        public int Id { get; set; }
        public string ProgramCategoryName { get; set; }
    }


    public class OrganizationProgramShiftView
    {
        public int Id { get; set; }
        public string ProgramShiftName { get; set; }

        public string actions { get; set; }
    }


    public class OrganizationCampusBuildingView
    {
        /*
      [Id]
      ,[CampusId]
      ,[BuildingCode]
      ,[BuildingTitle]
      ,[CreateDateBld]
         */
        public int Id { get; set; }
        public int CampusId { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingTitle { get; set; }
        public DateTime CreateDateBld { get; set; }

        public string CampusTitle { get; set; }
        public string CreateDateBldText { get; set; }

        public List<OrganizationCampusView> list_campus;
        public string str_campus { get; set; }

        public string actions { get; set; }
    }


    public class TerminalView
    {
        public int Id { get; set; }
        public int L_Id { get; set; }
        public string C_Name { get; set; }
        public int Terminal_id { get; set; }
        public string Branch_Code { get; set; }
        public string Branch_Name { get; set; }
        public string Terminal_Branch { get; set; }

        public string type { get; set; }
    }

    public class OrganizationCampusBuildingRoomView
    {
        /*
      [Id]
      ,[BuildingId]
      ,[RoomCode]
      ,[RoomTitle]
      ,[TerminalId]
      ,[FloorNumber]
      ,[CreateDateRm]
         */
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string RoomCode { get; set; }
        public string RoomTitle { get; set; }
        public int TerminalId { get; set; }
        public int FloorNumber { get; set; }
        public DateTime CreateDateRm { get; set; }

        public string BuildingTitle { get; set; }
        public string TerminalTitle { get; set; }
        public string CreateDateRmText { get; set; }

        public List<OrganizationCampusBuildingView> list_building;

        public List<TerminalView> list_terminal;

        public string str_building { get; set; }
        public string str_terminal { get; set; }

        public string actions { get; set; }
    }


    public class OrganizationCampusRoomCourseScheduleView
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

        public string RoomCode { get; set; }
        public string ShiftName { get; set; }
        public string RoomTitle { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string LectureGroupName { get; set; }
        public string DateText { get; set; }
        public string StartTimeText { get; set; }
        public string EndTimeText { get; set; }
        public string EmployeeTeacherName { get; set; }
        public string ProgramCode { get; set; }
        public string CampusCode { get; set; }
        public string CreateDateSchText { get; set; }


        public List<OrganizationCampusView> list_campus;

        public List<OrganizationProgramView> list_program;

        public List<OrganizationCampusBuildingRoomView> list_room;

        public List<OrganizationProgramCourseView> list_course;

        public List<LGRegionView> list_lecture_group;

        public List<EmployeeView> list_teacher;

        public List<OrganizationProgramShiftView> list_shift;


        public string str_campus { get; set; }
        public string str_program { get; set; }
        public string str_shift { get; set; }
        public string str_rooms { get; set; }
        public string str_courses { get; set; }
        public string str_lecture_groups { get; set; }
        public string str_teachers { get; set; }

        public string actions { get; set; }
    }


    public class OrganizationCourseAttendanceStudentView
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public DateTime SchDate { get; set; }
        public string strSchDate { get; set; }
        public string strCourseCode { get; set; }
        public string strRemarks { get; set; }

        public string actions { get; set; }

        public string strProcessBy { get; set; }
        public int iProcessCount { get; set; }

    }

    #endregion



    #region Course-Attendance

    public class OrganizationCourseAttendanceView
    {
        public int Id { get; set; }

        public DateTime ScheduleDate { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int LectureGroupId { get; set; }
        public string TimeInRemarks { get; set; }
        public string TimeOutRemarks { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EmployeeTeacherId { get; set; }

        public DateTime CourseTimeStart { get; set; }
        public DateTime CourseTimeEnd { get; set; }

        public int TerminalInId { get; set; }
        public int TerminalOutId { get; set; }

        public DateTime CreateDateSch { get; set; }

        public string StudentCode { get; set; }
        public string ShiftName { get; set; }
        public string RoomTitle { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string LectureGroupName { get; set; }
        public string DateText { get; set; }
        public string StartTimeText { get; set; }
        public string EndTimeText { get; set; }
        public string EmployeeTeacherName { get; set; }
        public string ProgramCode { get; set; }
        public string CampusCode { get; set; }
        public string CreateDateSchText { get; set; }


        public List<OrganizationCampusView> list_campuses;

        public List<OrganizationProgramView> list_programs;

        public List<OrganizationProgramTypeView> list_program_types;

        public List<OrganizationProgramShiftView> list_program_shifts;

        public List<LGRegionView> list_program_groups;

        public List<EmployeeView> list_students;
        public string str_students { get; set; }
    }

    #endregion





}
