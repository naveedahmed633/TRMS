using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationCourseAttendance
    {
        /*
       [id]
      ,[schedule_date_time]
      ,[employee_student_id]
      ,[course_id]
      ,[student_time_in]
      ,[status_in]
      ,[student_time_out]
      ,[status_out]
      ,[final_remarks]
      ,[terminal_in]
      ,[terminal_out]
      ,[course_time_start]
      ,[course_time_end]
      ,[active]
         */
        public int id { get; set; }
        public DateTime schedule_date_time { get; set; }
        public int employee_student_id { get; set; }
        public int student_group_id { get; set; }
        public int schedule_group_id { get; set; }
        public int student_pshift_id { get; set; }
        public int schedule_pshift_id { get; set; }        
        public int course_id { get; set; }
        public DateTime? student_time_in { get; set; }
        public string status_in { get; set; }
        public DateTime? student_time_out { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public int terminal_in_id { get; set; }
        public int terminal_out_id { get; set; }
        public DateTime? course_time_start { get; set; }
        public DateTime? course_time_end { get; set; }
        public bool active { get; set; }

        public int process_count { get; set; }
        public string process_code { get; set; }

        public bool is_sent { get; set; }
    }
}
