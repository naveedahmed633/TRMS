using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationCampusBuilding
    {
        /*
      [Id]
      ,[CampusId]
      ,[BuildingCode]
      ,[BuildingTitle]
      ,[CreateDateBld]
         */
        public int Id { get; set; }
        public int CampusId { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingTitle { get; set; }
        public DateTime CreateDateBld { get; set; }
    }
}
