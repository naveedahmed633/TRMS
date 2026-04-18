using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class CMS_ClassSchedule
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
        public bool TRMS_Synced { get; set; }
        public DateTime CreateDateTimeSch { get; set; }
    }
}
