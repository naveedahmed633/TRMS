using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SkillsSet
    {
        [Key]
        public int id { get; set; }
        public string skillname { get; set; }
        public string actions { get; set; }
    }
}
