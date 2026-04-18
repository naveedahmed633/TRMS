using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationCampus
    {
        /*
        [Id]
      ,[OrganizationId]
      ,[CampusTypeId]
      ,[CampusCode]
      ,[CampusTitle]
      ,[Address]
      ,[City]
      ,[StateProvice]
      ,[ZipCode]
      ,[EmailAddress]
      ,[Phone01]
      ,[Phone02]
      ,[FaxNumber]
      ,[CreateDateCmp]
         */
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CampusTypeId { get; set; }
        public string CampusCode { get; set; }
        public string CampusTitle { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public string ZipCode { get; set; }
        public string Phone01 { get; set; }
        public string Phone02 { get; set; }
        public string FaxNumber { get; set; }
        public DateTime CreateDateCmp { get; set; }
    }
}
