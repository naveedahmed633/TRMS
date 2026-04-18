using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    // This class is just a Big View Model for sending multiple models to the
    // employee CRUD view.
    public class CreateEmployee
    {
        public List<OrganizationCampusView> campuses;
        public List<OrganizationProgramShiftView> pshifts;
        public List<Function> functions;
        public List<Designation> designations;
        public List<Department> departments;
        public List<TypeOfEmployment> typeOfEmployments;
        public List<Group> groups;
        public List<AccessGroup> accessGroups;
        public List<Position_Status> positionStatus;
        public List<Site_Status> siteStatus;
        public List<Region> regions;
        public List<Shift> shifts;
        public List<Grades> grades;
        public List<Location> locations;
        public List<SkillsSet> skillSets;
        public List<PersistentAttendanceLog> persistentAttendanceLog;
       
    }
}
