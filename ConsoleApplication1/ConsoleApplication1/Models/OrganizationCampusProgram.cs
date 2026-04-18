using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationCampusProgram
    {
        /*
      [Id]
      ,[CampusId]
      ,[ProgramId]
      ,[IsActiveProgram]
         */
        public int Id { get; set; }
        public int CampusId { get; set; }
        public int ProgramId { get; set; }
        public bool IsActiveProgram { get; set; }
    }
}
