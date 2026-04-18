namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class PermissionLM
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public bool LMExportGridData { get; set; }
        public bool LMDownloadExcel { get; set; }
        public bool LMExportPDF { get; set; }
       
     
        public bool LMLeaves { get; set; }
        public bool LMOvertime { get; set; }
        public bool LMRoaster { get; set; }
        public bool LMAttendance { get; set; }
        public bool LMReport { get; set; }
        public bool LMManual { get; set; }
    }
}
