using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationProgram
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramTitle { get; set; }
        public string DisciplineName { get; set; }
        public int CreditHours { get; set; }
        public int WholeProgramTypeId { get; set; }
        public int WholeProgramTypeNumber { get; set; }
        public DateTime CreateDatePrg { get; set; }
    }
}
