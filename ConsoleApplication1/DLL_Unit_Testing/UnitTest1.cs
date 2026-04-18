using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1.Models;
using System.Linq;

namespace DLL_Unit_Testing
{
    [TestClass]
    public class UnitTest1
    {
        #region testMethods

        //Add Group
        [TestMethod]
        public void AddGroup()
        {
            var entity = new AccessGroup()
            {
                description = "Nothing",
                name = "Group4"
            };
            bool check = addAccessGroup(entity);
        }
        //Get Group
        [TestMethod]
        public void GetGroup()
        {
            string name = "Group4";
            var entity = getAccessGroup(name);
           
        }

        //Add Region
        [TestMethod]
        public void AddRegion()
        {
            var entity = new Region()
            {
                description = "Nothing",
                name = "Region4"
            };
            bool check = addRegion(entity);
        }


        //Add Function
        [TestMethod]
        public void AddFunction()
        {
            var entity = new Function
            {
                description = "Nothing",
                name = "Function4"
            };
            bool check = addFunction(entity);
        }
        //Add Designation
        [TestMethod]
        public void AddDesignation()
        {
            var entity = new Designation
            {
                description = "Nothing",
                name = "Designation4"
            };
            bool check = addDesignation(entity);
        }

        //Add Department
        [TestMethod]
        public void AddDepartment()
        {
            var entity = new Department
            {
                description = "Nothing",
                name = "Department4"
            };
            bool check = addDepartment(entity);
        }

        //Add Grade
        [TestMethod]
        public void AddGrade()
        {
            var entity = new Grade
            {
                description = "Nothing",
                name = "Grade4"
            };
            bool check = addGrade(entity);
        }
        [TestMethod]
        public void AddTypeOfEmployeement()
        {
            var entity = new TypeOfEmployment
            {
                description = "Nothing",
                name = "Permenent"
            };
            bool check = addTypeOfEmployment(entity);
        }

        [TestMethod]
        public void AddShift()
        {
            var entity = new Shift
            {
                start_time=DateTime.Now,
                end_time=DateTime.Now,
                late_time=DateTime.Now,
                early_time=DateTime.Now,
                name="Shift4"
            };
            bool check = addShift(entity);
        }

       
        //Get Region
        [TestMethod]
        public void GetRegion()
        {
            string name = "Region4";
            var entity = getRegion(name);
        }
        //Get Function
        [TestMethod]
        public void GetFunction()
        {
            string name = "Function4";
            var entity = getFunction(name);
        }

        //Get Designation
        [TestMethod]
        public void GetDesignation()
        {
            string name = "Designation4";
            var entity = getDesignation(name);
        }

      

        // Get Grade
        [TestMethod]
        public void GetGrade()
        {
            string name = "Grade4";
            var entity = getGrade(name);
        }

        //Get Department
        [TestMethod]
        public void GetDepartment()
        {
            string name = "Department4";
            var entity = getDepartment(name);
        }

        [TestMethod]
        public void GetTypeOfEmployment()
        {
            string name = "Permenent";
            var entity = getTypeOfEmployment(name);
        }

        [TestMethod]
        public void GetShift()
        {
            string name = "Shift4";
            var entity = getShift(name);
        }

       
       //Add Employee
        [TestMethod]
        public void AddEmployee()
        { 
            
            var entity = new Employee()
            {
               first_name="Abdul",
               last_name="Muqeet",
               employee_code="1234",
               email="abc@123.com",
               address="A-933/12",
               mobile_no="02123213213",
               date_of_joining=DateTime.Now,
               date_of_leaving=DateTime.Parse("2016-06-19"),
               function=getFunction("Function3"),
               region=getRegion("Region3"),
               designation=getDesignation("Designation3"),
               department=getDepartment("Department3"),
               type_of_employment=getTypeOfEmployment("Permenent"),
               grade=getGrade("Grade3"),
               acces_group=getAccessGroup("Group1"),
               shift=getShift("Shift3"),
               group=getGroup("group1")
            };
            bool check = addEmployee(entity);
        }

        

        #endregion


        /// <summary>
        /// 
        /// </summary>
        



        #region Methods
        public bool addAccessGroup(AccessGroup ac)
        {
            using (Context db = new Context())
            {
                
                db.Access_Group.Add(ac);
                if (db.SaveChanges() == 1)
                {
                    return true;
                }
                else
                   return false;
            }
        }
        private AccessGroup getAccessGroup(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Access_Group.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private bool addRegion(Region entity)
        {
           using(Context db=new Context())
           {
               db.Region.Add(entity);
               if(db.SaveChanges()==1)
               {
                   return true;
               }
               else
                   return false;
           }
        }
        private bool addFunction(Function entity)
        {
            using (Context db = new Context())
            {
                db.Function.Add(entity);
                if (db.SaveChanges() == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private bool addDesignation(Designation entity)
        {
            using (Context db = new Context())
            {
                db.Designation.Add(entity);
                if (db.SaveChanges() == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private bool addDepartment(Department entity)
        {
            using (Context db = new Context())
            {
                db.Department.Add(entity);
                if (db.SaveChanges() == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private bool addGrade(Grade entity)
        {
            using (Context db = new Context())
            {
                db.Grade.Add(entity);
                if (db.SaveChanges() == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private bool addTypeOfEmployment(TypeOfEmployment entity)
        {
            using (Context db = new Context())
            {
                db.Type_Of_Employment.Add(entity);
                if (db.SaveChanges() == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private bool addShift(Shift entity)
        {
            using (Context db = new Context())
            {
                db.Shift.Add(entity);
                if (db.SaveChanges() == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private Region getRegion(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Region.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private Function getFunction(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Function.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private Designation getDesignation(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Designation.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private Department getDepartment(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Department.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }

        private Grade getGrade(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Grade.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private TypeOfEmployment getTypeOfEmployment(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Type_Of_Employment.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private Shift getShift(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Shift.Where(m => m.name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private Group getGroup(string name)
        {
            using (var db = new Context())
            {
                var entity = db.Group.Where(m => m.group_name.Equals(name)).SingleOrDefault();
                return entity;
            }
        }
        private bool addEmployee(Employee entity)
        {
            using (var db = new Context())
            {
                db.Employee.Add(entity);
                if(db.SaveChanges()==1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion
        
       
    }
}
