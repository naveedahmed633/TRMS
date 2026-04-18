using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /**
     * This is the view model for an employee, it contains all fields
     * that might be used when moving data in between the server and client( browser ).
     */
    public class Employee
    {
        [Key]
        public int id { get; set; }
        public int campus_id { get; set; }
        public int pshift_id { get; set; }
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string father_name { get; set; }
        public int gender_id { get; set; }

        public string employee_code { get; set; }
        public string cnic_no { get; set; }

        public string email { get; set; }


        public string address { get; set; }

        public string mobile_no { get; set; }

        public int position_id { get; set; }
        public string position_name { get; set; }

        public int site_id { get; set; }
        public string site_name { get; set; }

        public string line_manager_code { get; set; }
        public string superline_manager_code { get; set; }

        // NOTICE: I have changed the data type of the
        // dates to string, so now we are heavily reliant
        // upon the frontend datepickers to do the job for us
        // we must also keep a validation in the backend for this.
        //public DateTime? date_of_joining { get; set; }
        public string date_of_joining { get; set; }


        //public DateTime? date_of_leaving { get; set; }
        public string date_of_leaving { get; set; }

        public int function_id { get; set; }
        public string function_name { get; set; }


        public int designation_id { get; set; }
        public string designation_name { get; set; }


        public int department_id { get; set; }
        public string department_name { get; set; }


        public int type_of_employment_id { get; set; }
        public string type_of_employment_name { get; set; }

        public int cards_count { get; set; }
        public string cards_list { get; set; }

        public string is_finger_registered { get; set; }


        public int grade_id { get; set; }
        public string grade_name { get; set; }


        public int access_group_id { get; set; }
        public string access_group_name { get; set; }


        public int region_id { get; set; }
        public string region_name { get; set; }

        // These two field are not meant to be shown 
        // they are for keeping track of the group to
        // which an employee is assigned.
        public int group_id { get; set; }
        public string group_name { get; set; }



        public int location_id { get; set; }
        public string location_name { get; set; }


        //This field active or deactive the time tune status 
        public string time_tune_status { get; set; }

        public string active { get; set; }

        // This little variable will hold the string
        // for the javascript code for edit/delete.
        // we have to do this because, well the
        // data is coming from the server side,
        // and we are not generating it with RAZOR 
        // or anything.
        public string actions;

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

        public int avld_lt01 { get; set; }
        public int avld_lt02 { get; set; }
        public int avld_lt03 { get; set; }
        public int avld_lt04 { get; set; }
        public int avld_lt05 { get; set; }
        public int avld_lt06 { get; set; }
        public int avld_lt07 { get; set; }
        public int avld_lt08 { get; set; }
        public int avld_lt09 { get; set; }
        public int avld_lt10 { get; set; }
        public int avld_lt11 { get; set; }
        public int avld_lt12 { get; set; }
        public int avld_lt13 { get; set; }
        public int avld_lt14 { get; set; }
        public int avld_lt15 { get; set; }
        
        public string photograph { get; set; }
        public string file_01 { get; set; }
        public string file_02 { get; set; }
        public string file_03 { get; set; }
        public string file_04 { get; set; }
        public string file_05 { get; set; }
        public string emergency_contact_01 { get; set; }
        public string emergency_contact_02 { get; set; }
        public string description { get; set; }

        public string view_images { get; set; }
        public string view_profile { get; set; }
        public string view_payroll { get; set; }


        public string date_of_birth { get; set; }
        public DateTime? birth_date { get; set; }
        public string[] skills_set { get; set; }
        public string g_skills_set { get; set; }
        public string[] modal_skills_set { get; set; }

        public string campus_code { get; set; }
        public string campus_name { get; set; }
        public string pshift_name { get; set; }

        public bool is_super_hr { get; set; }
        public string is_super_hr_str { get; set; }

        public string gender_name { get; set; }

        public string imported_employee { get; set; }
    }
}
