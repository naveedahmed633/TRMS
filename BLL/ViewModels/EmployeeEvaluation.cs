using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EmployeeEvaluation
    {
        [Key]
        public int id { get; set; }
        public string empCode { get; set; }
        public string reviewPeriod { get; set; }
        public string postion { get; set; }
        public string project { get; set; }
        public string primaryResponsibilities { get; set; }
        public string secondaryResponsibilities { get; set; }
        public string careerPath { get; set; }

        public int personality { get; set; }
        public int communicationSkills { get; set; }
        public int attendancePromptness { get; set; }
        public int imitative { get; set; }
        public int organizationAwareness { get; set; }
        public int selfControl { get; set; }
        public int proficiency { get; set; }
        public int projectManagement { get; set; }
        public int attentionDetail { get; set; }
        public int clientInteraction { get; set; }
        public int creativity { get; set; }
        public int businessSkill { get; set; }
        public int achievement { get; set; }


        public string majorStrength { get; set; }
        public string areaImprovement { get; set; }
        public string otherComment { get; set; }
        public string goal { get; set; }


        public string EmpName { get; set; }

        public float total { get; set; }

        public string result { get; set; }


    }
}
