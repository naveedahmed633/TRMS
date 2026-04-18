using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Letter
    {
        [Key]
        public int Id { get; set; }
        public string LetterName { get; set; }
        public string Formate { get; set; }

    }
}
