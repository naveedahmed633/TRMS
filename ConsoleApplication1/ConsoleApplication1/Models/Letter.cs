namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Letter
    {
        public int Id { get; set; }
        public string LetterName { get; set; }
        public string formate { get; set; }
       


       

    }
}
