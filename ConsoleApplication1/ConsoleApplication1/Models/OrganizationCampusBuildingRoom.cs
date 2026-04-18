using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationCampusBuildingRoom
    {
        /*
      [Id]
      ,[BuildingId]
      ,[RoomCode]
      ,[RoomTitle]
      ,[TerminalId]
      ,[FloorNumber]
      ,[CreateDateRm]
         */
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string RoomCode { get; set; }
        public string RoomTitle { get; set; }
        public int TerminalId { get; set; }
        public int FloorNumber { get; set; }
        public DateTime CreateDateRm { get; set; }
    }
}
