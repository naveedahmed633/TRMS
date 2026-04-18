using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationProgramType
    {
        /*
        [Id]
      ,[ProgramTypeName]
         */
        public int Id { get; set; }
        public string ProgramTypeName { get; set; }
    }
}
