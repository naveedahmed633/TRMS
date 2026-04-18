using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationProgramCourse
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
        public int PassingMarks { get; set; }
        public int CreditHours { get; set; }
        public int TotalMarks { get; set; }
        public bool IsActiveCourse { get; set; }
        public DateTime CreateDateCrs { get; set; }
    }
}
