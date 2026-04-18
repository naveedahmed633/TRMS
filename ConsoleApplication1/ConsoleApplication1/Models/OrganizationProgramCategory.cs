using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationProgramCategory
    {
        /*
        [Id]
      ,[ProgramCategoryName]
         */
        public int Id { get; set; }
        public string ProgramCategoryName { get; set; }
    }
}
