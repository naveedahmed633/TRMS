namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Employee
    {
        public int EmployeeId { get; set; }

        public int campus_id { get; set; }

        public int program_shift_id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]

        public string employee_code { get; set; }
        public string cnic_no { get; set; }
        public string father_name { get; set; }

        public int gender_id { get; set; }

        public string email { get; set; }

        public string address { get; set; }

        public string mobile_no { get; set; }

        public int position_id { get; set; }

        public int site_id { get; set; }

        public DateTime? date_of_joining { get; set; }

        public DateTime? date_of_leaving { get; set; }

        public bool active { get; set; }

        public bool timetune_active { get; set; }

        public virtual AccessGroup access_group { get; set; }

        public virtual Department department { get; set; }

        public virtual Designation designation { get; set; }

        public virtual Function function { get; set; }

        public virtual Grade grade { get; set; }

        public virtual Group Group { get; set; }

        public virtual Location location { get; set; }

        //public virtual PositionStatus position { get; set; }

        //public virtual SiteStatus site { get; set; }

        public virtual Region region { get; set; }

        public virtual TypeOfEmployment type_of_employment { get; set; }
        
        public virtual PersistentAttendanceLog persistent_attendance_log { get; set; }

        public DateTime? date_of_birth { get; set; }

        public string skills_set { get; set; }
        public string password { get; set; }

        public string salt { get; set; }

        public int sick_leaves { get; set; }
        public int casual_leaves { get; set; }
        public int annual_leaves { get; set; }
        public int other_leaves { get; set; }

        public int leave_type01 { get; set; }
        public int leave_type02 { get; set; }
        public int leave_type03 { get; set; }
        public int leave_type04 { get; set; }
        public int leave_type05 { get; set; }
        public int leave_type06 { get; set; }
        public int leave_type07 { get; set; }
        public int leave_type08 { get; set; }
        public int leave_type09 { get; set; }
        public int leave_type10 { get; set; }
        public int leave_type11 { get; set; }

        public string photograph { get; set; }
        public string file_01 { get; set; }
        public string file_02 { get; set; }
        public string file_03 { get; set; }
        public string file_04 { get; set; }
        public string file_05 { get; set; }
        public string emergency_contact_01 { get; set; }
        public string emergency_contact_02 { get; set; }
        public string description { get; set; }

        public bool is_super_hr { get; set; }

        public string select_langauge { get; set; }

        public bool imported_employee { get; set; }
    }
}
