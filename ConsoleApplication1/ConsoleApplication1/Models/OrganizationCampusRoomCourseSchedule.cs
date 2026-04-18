using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DLL.Models
{
    public class OrganizationCampusRoomCourseSchedule
    {
        /*
      [Id]
      ,[RoomId]
      ,[CourseId]
      ,[EmployeeTeacherId]
      ,[StudyTitle]
      ,[StartTime]
      ,[EndTime]
      ,[CreateDateSch]
         */
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int CourseId { get; set; }
        public int LectureGroupId { get; set; }
        public int ShiftId { get; set; }
        public string StudyTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EmployeeTeacherId { get; set; }
        public int ProgramId { get; set; }
        public int CampusId { get; set; }
        public DateTime CreateDateSch { get; set; }
    }
}
