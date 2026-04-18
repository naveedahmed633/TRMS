namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Group
    {
        public int GroupId { get; set; }

        //public int supervisor_id { get; set; }

        public string group_name { get; set; }

        public string group_description { get; set; }

        public bool follows_general_calendar { get; set; }

        public bool active { get; set; }

        public int supervisor_id { get; set; }

        public virtual List<Employee> Employees { get; set; }

        public virtual List<Shift> Shifts { get; set; }

        public virtual List<GroupCalendar> GroupCalendars { get; set; }

    }



}
