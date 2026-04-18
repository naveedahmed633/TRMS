using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class Organization
    {
        /*
         [Id]
      ,[OrganizationTitle]
      ,[EstablishedDate]
      ,[CampusLimit]
      ,[Logo]
      ,[WebsiteURL]
      ,[Description]
      ,[CreateDateOrg]
             */
        public int Id { get; set; }
        public string OrganizationTitle { get; set; }
        public DateTime EstablishedDate { get; set; }
        public int CampusLimit { get; set; }
        public string Logo { get; set; }
        public string WebsiteURL { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateOrg { get; set; }
    }
}
