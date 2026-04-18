using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationProgramCourseEnrollment
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
    }
}
