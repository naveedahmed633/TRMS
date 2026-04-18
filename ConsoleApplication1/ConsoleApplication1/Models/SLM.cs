namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class SLM
    {
        public int SLMId { get; set; }

        public virtual Employee superLineManager { get; set; }

        public virtual Employee taggedEmployee { get; set; }

        public bool active { get; set; }


    }
}
