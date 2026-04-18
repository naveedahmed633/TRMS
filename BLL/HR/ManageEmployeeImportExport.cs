using DLL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTune
{
    public class ManageEmployeeImportExport
    {
        public class ManageEmployeeCSV
        {
            public string employeeCode { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobileNo { get; set; }
            public string dateOfJoining { get; set; }
            public string dateOfLeaving { get; set; }
            public string accessGroup { get; set; }
            public string departmentID { get; set; }
            public string designationID { get; set; }
            public string functionID { get; set; }
            public string gradeID { get; set; }
            //public string groupID { get; set; }
            public string locationID { get; set; }
            public string regionID { get; set; }
            public string typeOfEmploymentID { get; set; }
            public string active { get; set; }
            public int sick_leaves { get; set; }
            public int casual_leaves { get; set; }
            public int annual_leaves { get; set; }
            public int other_leaves { get; set; }

            public int leave_type01 { get; set; }
            public int leave_type02 { get; set; }
            public int leave_type03 { get; set; }
            public int leave_type04 { get; set; }
            public int leave_type05 { get; set; }
            public int leave_type06 { get; set; }
            public int leave_type07 { get; set; }
            public int leave_type08 { get; set; }
            public int leave_type09 { get; set; }
            public int leave_type10 { get; set; }
            public int leave_type11 { get; set; }

            public string photograph { get; set; }
            public string file_01 { get; set; }
            public string file_02 { get; set; }
            public string file_03 { get; set; }
            public string file_04 { get; set; }
            public string file_05 { get; set; }
            public string emergency_contact_01 { get; set; }
            public string emergency_contact_02 { get; set; }
            public string description { get; set; }
            public string date_of_birth { get; set; }
            public string father_name { get; set; }
            public string gender { get; set; }
            public string campus_code { get; set; }
            public string program_shift { get; set; }
        }

        public class EmpExportTable
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string employee_code { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobile_no { get; set; }
            public DateTime? date_of_joining { get; set; }
            public DateTime? date_of_leaving { get; set; }
            public string access_group { get; set; }
            public int department_DepartmentId { get; set; }
            public int designation_DesignationId { get; set; }
            public int function_FunctionId { get; set; }
            public int grade_GradeId { get; set; }
            //public int Group_GroupId { get; set; }
            public int location_LocationId { get; set; }
            public int region_RegionId { get; set; }
            public int type_of_employment_TypeOfEmploymentId { get; set; }
            public bool active { get; set; }
        }
        public static List<ManageEmployeeCSV> getManageEmployeeCSV()
        {
            List<ManageEmployeeCSV> toReturn;
            using (var db = new Context())
            {
                bool aLT01 = false, aLT02 = false, aLT03 = false, aLT04 = false, aLT05 = false, aLT06 = false, aLT07 = false, aLT08 = false, aLT09 = false, aLT10 = false, aLT11 = false, aLT12 = false, aLT13 = false, aLT14 = false, aLT15 = false;
                var dbLTypes = db.leave_type.ToList();
                if (dbLTypes != null && dbLTypes.Count > 0)
                {
                    foreach (var l in dbLTypes)
                    {
                        if (l.Id == 1)
                            aLT01 = l.IsActive ? true : false;
                        else if (l.Id == 2)
                            aLT02 = l.IsActive ? true : false;
                        else if (l.Id == 3)
                            aLT03 = l.IsActive ? true : false;
                        else if (l.Id == 4)
                            aLT04 = l.IsActive ? true : false;
                        else if (l.Id == 5)
                            aLT05 = l.IsActive ? true : false;
                        else if (l.Id == 6)
                            aLT06 = l.IsActive ? true : false;
                        else if (l.Id == 7)
                            aLT07 = l.IsActive ? true : false;
                        else if (l.Id == 8)
                            aLT08 = l.IsActive ? true : false;
                        else if (l.Id == 9)
                            aLT09 = l.IsActive ? true : false;
                        else if (l.Id == 10)
                            aLT10 = l.IsActive ? true : false;
                        else if (l.Id == 11)
                            aLT11 = l.IsActive ? true : false;
                        else if (l.Id == 12)
                            aLT12 = l.IsActive ? true : false;
                        else if (l.Id == 13)
                            aLT13 = l.IsActive ? true : false;
                        else if (l.Id == 14)
                            aLT14 = l.IsActive ? true : false;
                        else if (l.Id == 15)
                            aLT15 = l.IsActive ? true : false;
                    }
                }


                try
                {
                    /*
                        employeeCode,
                        firstName,
                        lastName,
                        email,
                        address,
                        mobileNo,
                        dateOfJoining,
                        dateOfLeaving,
                        accessGroup,
                        departmentID,
                        designationID,
                        functionID,
                        gradeID,
                        locationID,
                        regionID,
                        typeOfEmploymentID,
                        active,
                        sick_leaves,
                        casual_leaves,
                        annual_leaves,
                        photograph,
                        file_01,file_02,file_03,file_04,file_05,
                        emergency_contact_01,
                        emergency_contact_02,
                        description,
                        date_of_birth,
                        father_name,
                        gender,
                        campus_code,
                        program_shift
                     */
                    toReturn =
                        db.employee.Where(m => m.active).Select(
                        p => new ManageEmployeeCSV()
                        {
                            employeeCode = "<" + p.employee_code + ">",
                            firstName = (p.first_name == null) ? "<->" : p.first_name,
                            lastName = (p.last_name == null) ? "<->" : p.last_name,
                            email = (p.email == null) ? "<->" : p.email,
                            address = (p.address == null) ? "<->" : p.address,
                            mobileNo = (p.mobile_no == null) ? "<->" : p.mobile_no,
                            dateOfJoining = p.date_of_joining.ToString(),
                            dateOfLeaving = p.date_of_leaving.ToString(),
                            //accessGroupID=p.access_group_AccessGroupId,
                            accessGroup = p.access_group.name,
                            departmentID = (p.department == null) ? "<NOT ASSIGNED>" : p.department.name.ToUpper(),
                            designationID = (p.designation == null) ? "<NOT ASSIGNED>" : p.designation.name.ToUpper(),
                            functionID = (p.function == null) ? "<NOT ASSIGNED>" : p.function.name.ToUpper(),
                            gradeID = (p.grade == null) ? "<NOT ASSIGNED>" : p.grade.name.ToUpper(),
                            //groupID=p.Group_GroupId,
                            locationID = (p.location == null) ? "<NOT ASSIGNED>" : p.location.name.ToUpper(),
                            regionID = (p.region == null) ? "<NOT ASSIGNED>" : p.region.name.ToUpper(),
                            typeOfEmploymentID = (p.type_of_employment == null) ? "1" : p.type_of_employment.dbID.ToString(),
                            active = (p.active) ? "TRUE" : "FALSE",
                            sick_leaves = p.sick_leaves,
                            casual_leaves = p.casual_leaves,
                            annual_leaves = p.annual_leaves,
                            other_leaves = p.other_leaves,
                            leave_type01 = p.leave_type01,
                            leave_type02 = p.leave_type02,
                            leave_type03 = p.leave_type03,
                            leave_type04 = p.leave_type04,
                            leave_type05 = p.leave_type05,
                            leave_type06 = p.leave_type06,
                            leave_type07 = p.leave_type07,
                            leave_type08 = p.leave_type08,
                            leave_type09 = p.leave_type09,
                            leave_type10 = p.leave_type10,
                            leave_type11 = p.leave_type11,
                            photograph = p.photograph,
                            file_01 = p.file_01,
                            file_02 = p.file_02,
                            file_03 = p.file_03,
                            file_04 = p.file_04,
                            file_05 = p.file_05,
                            emergency_contact_01 = p.emergency_contact_01,
                            emergency_contact_02 = p.emergency_contact_02,
                            description = p.description,
                            date_of_birth = p.date_of_birth.ToString(),
                            father_name = p.father_name,
                            gender = p.gender_id.ToString(),
                            campus_code = db.organization_campus.Where(k => k.Id == p.campus_id).FirstOrDefault().CampusCode ?? "DMC",
                            program_shift = p.program_shift_id == 1 ? "Morning" : "Evening"
                        }).ToList();


                    // convert dates to a more processable form.
                    foreach (var value in toReturn)
                    {
                        if (value.firstName == null)
                        {
                            value.firstName = "<.>";
                        }

                        if (value.lastName == null)
                        {
                            value.lastName = "<.>";
                        }

                        if (value.email == null)
                        {
                            value.email = "<->";
                        }

                        if (value.address == null)
                        {
                            value.address = "<->";
                        }

                        if (value.mobileNo == null)
                        {
                            value.mobileNo = "<->";
                        }

                        if (value.dateOfJoining != null && value.dateOfJoining != "")
                        {
                            DateTime dateOfJoining = DateTime.Parse(value.dateOfJoining);
                            value.dateOfJoining = dateOfJoining.ToString("dd/MM/yyyy");
                        }

                        if (value.dateOfLeaving != null && value.dateOfLeaving != "")
                        {
                            DateTime dateOfLeaving = DateTime.Parse(value.dateOfLeaving);
                            value.dateOfLeaving = dateOfLeaving.ToString("dd/MM/yyyy");
                        }


                        if (value.accessGroup != null && value.accessGroup != "")
                        {
                            if (value.accessGroup == "TimeTuneEMP")
                            {
                                value.accessGroup = "EMP";
                            }
                            else if (value.accessGroup == "TimeTuneLM")
                            {
                                value.accessGroup = "LM";
                            }
                            else if (value.accessGroup == "TimeTuneHR")
                            {
                                value.accessGroup = "<HR>";
                            }
                            else
                            {
                                value.accessGroup = "<EMP>";
                            }
                        }
                        else
                        {
                            value.accessGroup = "EMP";
                        }

                        if (value.date_of_birth != null && value.date_of_birth != "")
                        {
                            DateTime dateOfBirth = DateTime.Parse(value.date_of_birth);
                            value.date_of_birth = dateOfBirth.ToString("dd/MM/yyyy");
                        }

                        if (value.photograph == null)
                        {
                            value.photograph = "demo-user.png";
                        }

                        if (value.file_01 == null)
                        {
                            value.file_01 = "demo-01.png";
                        }

                        if (value.file_02 == null)
                        {
                            value.file_02 = "demo-02.png";
                        }

                        if (value.file_03 == null)
                        {
                            value.file_03 = "demo-01.png";
                        }

                        if (value.file_04 == null)
                        {
                            value.file_04 = "demo-02.png";
                        }

                        if (value.file_05 == null)
                        {
                            value.file_05 = "demo-01.png";
                        }




                        if (value.emergency_contact_01 == null)
                        {
                            value.emergency_contact_01 = "<->";
                        }

                        if (value.emergency_contact_02 == null)
                        {
                            value.emergency_contact_02 = "<->";
                        }


                        if (value.description == null)
                        {
                            value.description = "<->";
                        }

                        if (value.father_name == null)
                        {
                            value.father_name = "<.>";
                        }

                        if (value.gender != null && value.gender != "")
                        {
                            if (value.gender == "1")
                            {
                                value.gender = "male";
                            }
                            else if (value.gender == "2")
                            {
                                value.gender = "female";
                            }
                            else
                            {
                                value.gender = "<->";
                            }
                        }
                        else
                        {
                            value.gender = "<->";
                        }


                    }
                    /*string sql =
                         "select\n" +
                          "[first_name]\n" +
                          ",[last_name]\n" +
                          ",[employee_code]\n" +
                          ",[email]\n" +
                          ",[address]\n" +
                          ",[mobile_no]\n" +
                          ",[date_of_joining]\n" +
                          ",[date_of_leaving]\n" +
                          ",COALESCE([access_group_AccessGroupId], 0) as [access_group_AccessGroupId]\n" +
                          ",COALESCE([department_DepartmentId], 0) as [department_DepartmentId]\n" +
                          ",COALESCE([designation_DesignationId], 0) as [designation_DesignationId]\n" +
                          ",COALESCE([function_FunctionId], 0) as [function_FunctionId]\n" +
                          ",COALESCE([grade_GradeId], 0) as [grade_GradeId]\n" +
                          ",COALESCE([Group_GroupId], 0) as [Group_GroupId]\n" +
                          ",COALESCE([location_LocationId], 0) as [location_LocationId]\n" +
                          ",COALESCE([region_RegionId], 0) as [region_RegionId]\n" +
                          ",COALESCE([type_of_employment_TypeOfEmploymentId], 0) as [type_of_employment_TypeOfEmploymentId]\n" +
                           ",[timetune_active]\n" +
                        "from\n" +
                        "Employees where [active]=1 \n";*/

                    //var results = db.Database.SqlQuery<EmpExportTable>(sql).ToList();


                    /*

                    int count = results.Count();

                    toReturn = results.Select(
                        p => new ManageEmployeeCSV()
                        {
                            employeeCode=p.employee_code,
                            firstName = (p.first_name == null) ? "" : p.first_name,
                            lastName=(p.last_name == null) ? "" : p.last_name,
                            email=(p.email == null) ? "" : p.email,
                            address=(p.address==null)?"":p.address,
                            mobileNo = (p.mobile_no == null) ? "" : p.mobile_no,
                            dateOfJoining=p.date_of_joining.ToString(),
                            dateOfLeaving=p.date_of_leaving.ToString(),
                            //accessGroupID=p.access_group_AccessGroupId,
                           departmentID=p.department_DepartmentId,
                           designationID=p.designation_DesignationId,
                           functionID=p.function_FunctionId,
                           gradeID=p.grade_GradeId,
                           //groupID=p.Group_GroupId,
                           locationID=p.location_LocationId,
                           regionID=p.region_RegionId,
                           typeOfEmploymentID=p.type_of_employment_TypeOfEmploymentId,
                           active=p.active
                        }).ToList();
                     * 
                     * */
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ManageEmployeeCSV>();
                }

            }

            return toReturn;
        }
        public class ManageContractualStaffCSV
        {
            public string employeeCode { get; set; }
            public string employeeName { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string mobileNo { get; set; }
            public string dateOfJoining { get; set; }
            public string dateOfLeaving { get; set; }
            public string company { get; set; }
            public string department { get; set; }
            public string designation { get; set; }
            public string function { get; set; }
            public string grade { get; set; }
            //public int groupID { get; set; }
            public string location { get; set; }
            public string region { get; set; }
            public bool active { get; set; }
        }
        private static bool convertStringToBool(string value)
        {
            return (value == "True" || value == "true" || value == "TRUE");
        }
        public static string setEmployees(List<string> csvContents, string user_code)
        {

            DateTime? date_of_joining = DateTime.Now; // (fromForm.date_of_joining != null) ? (DateTime?)DateTime.ParseExact(fromForm.date_of_joining, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null;
            DateTime? date_of_leaving = DateTime.Now; //(fromForm.date_of_leaving != null) ? (DateTime?)DateTime.ParseExact(fromForm.date_of_leaving, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null;

            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 47 values (columns).

                        if (values == null || values.Length < 47)
                            continue;

                        // Remove unwanted " characters.
                        string employeeCode = values[0].Replace("\"", "");
                        values[1] = values[1].Replace("\"", "");
                        values[2] = values[2].Replace("\"", "");
                        values[3] = values[3].Replace("\"", "");
                        values[4] = values[4].Replace("\"", "");
                        values[5] = values[5].Replace("\"", "");
                        values[6] = values[6].ToLower().Replace("\"", "");
                        values[7] = values[7].ToLower().Replace("\"", "");
                        values[8] = values[8].ToLower().Replace("\"", ""); string strValue8 = values[8];
                        values[9] = values[9].ToLower().Replace("\"", ""); string strValue9 = values[9];
                        values[10] = values[10].ToLower().Replace("\"", ""); string strValue10 = values[10];
                        values[11] = values[11].ToLower().Replace("\"", ""); string strValue11 = values[11];
                        values[12] = values[12].ToLower().Replace("\"", ""); string strValue12 = values[12];
                        values[13] = values[13].ToLower().Replace("\"", ""); string strValue13 = values[13];
                        values[14] = values[14].ToLower().Replace("\"", ""); string strValue14 = values[14];
                        values[15] = values[15].Replace("\"", "");
                        values[16] = values[16].Replace("\"", "");

                        values[17] = values[17].Replace("\"", "");//sick_leaves
                        values[18] = values[18].Replace("\"", "");
                        values[19] = values[19].Replace("\"", "");
                        values[20] = values[20].Replace("\"", "");

                        values[21] = values[21].Replace("\"", "");//leave type 01
                        values[22] = values[22].Replace("\"", "");
                        values[23] = values[23].Replace("\"", "");
                        values[24] = values[24].Replace("\"", "");
                        values[25] = values[25].Replace("\"", "");//leave type 05
                        values[26] = values[26].Replace("\"", "");
                        values[27] = values[27].Replace("\"", "");
                        values[28] = values[28].Replace("\"", "");
                        values[29] = values[29].Replace("\"", "");//leave type 09
                        values[30] = values[30].Replace("\"", "");
                        values[31] = values[31].Replace("\"", "");

                        values[32] = values[32].Replace("\"", "");//photo//20
                        values[33] = values[33].Replace("\"", "");//file01//21
                        values[34] = values[34].Replace("\"", "");//22
                        values[35] = values[35].Replace("\"", "");//23
                        values[36] = values[36].Replace("\"", "");//24
                        values[37] = values[37].Replace("\"", "");//Driving License
                        values[38] = values[38].Replace("\"", "");//emergency_contact
                        values[39] = values[39].Replace("\"", "");//emergency_contact
                        values[40] = values[40].Replace("\"", "");//description
                        values[41] = values[41].ToLower().Replace("\"", "");//date of birth
                        values[42] = values[42].ToLower().Replace("\"", "");//father name
                        values[43] = values[43].ToLower().Replace("\"", ""); string strValue43 = values[43];//gender
                        values[44] = values[44].ToLower().Replace("\"", ""); string strValue44 = values[44];//campus code
                        values[45] = values[45].ToLower().Replace("\"", ""); string strValue45 = values[45];//campus code//p-shift
                        values[46] = values[46].ToLower().Replace("\"", ""); string strValue46 = values[46];//doc. no. cnic/smart card

                        // continue if its the first row (CSV Header line).
                        if (employeeCode == "employeeCode" || values[6] == "dateOfJoining" || values[17] == "sick_leaves" || values[34] == "date_of_birth" || values[38] == "program_shift")
                        {
                            continue;
                        }
                        // Get the employee 
                        Employee employee = db.employee.Where(m =>
                            m.employee_code.Equals(employeeCode)).FirstOrDefault();

                        //try
                        //{
                        // If the line manager and the employee both exist.
                        if (employee != null)
                        {
                            //for (int i = 0; i < 18; i++)
                            //{
                            //Update Employee
                            employee.first_name = values[1];
                            employee.last_name = values[2];
                            employee.email = values[3];
                            employee.address = values[4];
                            employee.mobile_no = values[5];

                            //string strJDate = values[6].Replace("\"", "");//date
                            //string strProperJoinDate = DateTime.ParseExact(strJDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            //if (strProperJoinDate != "")
                            //{
                            //    strJDate = strProperJoinDate;
                            //}


                            employee.date_of_joining = (values[6] == null || values[6] == "") ? null : (DateTime?)DateTime.ParseExact(values[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            employee.date_of_leaving = (values[7] == null || values[7] == "") ? null : (DateTime?)DateTime.ParseExact(values[7], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                            //IR added on 19 Dec 2020
                            date_of_joining = employee.date_of_joining;
                            date_of_leaving = employee.date_of_leaving;

                            // Set access group, as per the rules defined in 'edit employee'.
                            ////string accessGroupName = values[8];
                            ////AccessGroup accessGroup = db.access_group.Where(m => m.name.Equals(accessGroupName)).FirstOrDefault();

                            ////// access group cannot be null.
                            ////if (accessGroup != null)
                            ////{
                            ////    TimeTune.EmployeeCrud.changeEmployeeAccessGroup(accessGroup.AccessGroupId, db, employee);
                            ////}
                            ////else
                            ////{
                            ////    continue;
                            ////}

                            employee.access_group = (values[8] == "" || values[8] == "0") ? null : db.access_group.Where(a => a.name.ToLower().Contains(strValue8)).FirstOrDefault();//.Find(int.Parse(values[8]));
                            employee.department = (values[9] == "" || values[9] == "0") ? null : db.department.Where(d => d.name.ToLower() == strValue9).FirstOrDefault(); //.Find(int.Parse(values[9]));
                            employee.designation = (values[10] == "" || values[10] == "0") ? null : db.designation.Where(d => d.name.ToLower() == strValue10).FirstOrDefault(); //.Find(int.Parse(values[10]));
                            employee.function = (values[11] == "" || values[11] == "0") ? null : db.function.Where(f => f.name.ToLower() == strValue11).FirstOrDefault();
                            employee.grade = (values[12] == "" || values[12] == "0") ? null : db.grade.Where(g => g.name.ToLower() == strValue12).FirstOrDefault(); //.Find(int.Parse(values[12]));
                            //employee.Group = (int.Parse(values[13]) == 0) ? null : db.group.Find(int.Parse(values[13]));
                            employee.location = (values[13] == "" || values[13] == "0") ? null : db.location.Where(l => l.name.ToLower() == strValue13).FirstOrDefault(); //.Find(int.Parse(values[13]));
                            employee.region = (values[14] == "" || values[14] == "0") ? null : db.region.Where(r => r.name.ToLower() == strValue14).FirstOrDefault(); //.Find(int.Parse(values[14]));

                            employee.position_id = 1;
                            employee.site_id = 1;

                            employee.type_of_employment = (values[15] == "" || int.Parse(values[15]) == 0) ? null : db.type_of_employment.Find(int.Parse(values[15]));
                            employee.timetune_active = convertStringToBool(values[16]);

                            employee.active = true;

                            int sLeaves = 0; int.TryParse(values[17].ToString(), out sLeaves);
                            int cLeaves = 0; int.TryParse(values[18].ToString(), out cLeaves);
                            int aLeaves = 0; int.TryParse(values[19].ToString(), out aLeaves);
                            int oLeaves = 0; int.TryParse(values[20].ToString(), out oLeaves);

                            employee.sick_leaves = sLeaves;
                            employee.casual_leaves = cLeaves;
                            employee.annual_leaves = aLeaves;
                            employee.other_leaves = oLeaves;

                            int iLeaves01 = 0; int.TryParse(values[21].ToString(), out iLeaves01);
                            int iLeaves02 = 0; int.TryParse(values[22].ToString(), out iLeaves02);
                            int iLeaves03 = 0; int.TryParse(values[23].ToString(), out iLeaves03);
                            int iLeaves04 = 0; int.TryParse(values[24].ToString(), out iLeaves04);
                            int iLeaves05 = 0; int.TryParse(values[25].ToString(), out iLeaves05);
                            int iLeaves06 = 0; int.TryParse(values[26].ToString(), out iLeaves06);
                            int iLeaves07 = 0; int.TryParse(values[27].ToString(), out iLeaves07);
                            int iLeaves08 = 0; int.TryParse(values[28].ToString(), out iLeaves08);
                            int iLeaves09 = 0; int.TryParse(values[29].ToString(), out iLeaves09);
                            int iLeaves10 = 0; int.TryParse(values[30].ToString(), out iLeaves10);
                            int iLeaves11 = 0; int.TryParse(values[31].ToString(), out iLeaves11);

                            employee.leave_type01 = iLeaves01;
                            employee.leave_type02 = iLeaves02;
                            employee.leave_type03 = iLeaves03;
                            employee.leave_type04 = iLeaves04;
                            employee.leave_type05 = iLeaves05;
                            employee.leave_type06 = iLeaves06;
                            employee.leave_type07 = iLeaves07;
                            employee.leave_type08 = iLeaves08;
                            employee.leave_type09 = iLeaves09;
                            employee.leave_type10 = iLeaves10;
                            employee.leave_type11 = iLeaves11;

                            employee.photograph = values[32];//25
                            employee.file_01 = values[33];//26
                            employee.file_02 = values[34];//27
                            employee.file_03 = values[35];//28
                            employee.file_04 = values[36];//29
                            employee.file_05 = values[37];//30
                            employee.emergency_contact_01 = values[38];//31
                            employee.emergency_contact_02 = values[39];//32
                            employee.description = values[40];//33
                            employee.date_of_birth = (values[41] == null || values[41] == "") ? null : (DateTime?)DateTime.ParseExact(values[41], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);//34

                            employee.father_name = (values[42] == "" || values[42] == "0") ? null : values[42];//35
                            employee.gender_id = (values[43] != null && values[43].ToString() != "") ? db.gender.Where(c => c.GenderName.ToLower() == strValue43).FirstOrDefault().Id : 0;//36
                            employee.campus_id = (values[44] != null && values[44].ToString() != "") ? db.organization_campus.Where(c => c.CampusCode.ToLower() == strValue44).FirstOrDefault().Id : 0;//37
                            employee.program_shift_id = (values[45] != null && values[45].ToString() != "") ? db.organization_program_shift.Where(c => c.ProgramShiftName.ToLower() == strValue45).FirstOrDefault().Id : 0;//38
                            employee.cnic_no = strValue46;//cnic. no. - position 47 by 0 starting

                            db.SaveChanges();

                            //IR UpdateEmp
                            int resp = 0;
                            resp = TimeTune.EmployeeCrud.resetPermissionsByEmpID(employee.EmployeeId);

                            TimeTune.AuditTrail.update("{\"*id\" : \"" + employee.EmployeeId.ToString() + "\"}", "Employees", user_code);

                            //}
                            ////db.SaveChanges();
                            //return "Successfull";
                        }
                        else //new employee
                        {
                            string aGroupName = (values[8] == "" || values[8] == "0") ? "EMP" : values[8]; //set for emp default
                            AccessGroup accessGroup = db.access_group.Where(m => m.name.ToLower().Contains(aGroupName)).FirstOrDefault();

                            Department dept = (values[9] == "" || values[9] == "0") ? null : db.department.Where(d => d.name.ToLower() == strValue9).FirstOrDefault(); //.Find(int.Parse(values[9]));
                            Designation desig = (values[10] == "" || values[10] == "0") ? null : db.designation.Where(d => d.name.ToLower() == strValue10).FirstOrDefault(); //.Find(int.Parse(values[10]));
                            Function func = (values[11] == "" || values[11] == "0") ? null : db.function.Where(f => f.name.ToLower() == strValue11).FirstOrDefault(); //.Find(int.Parse(values[11]));
                            Grade grd = (values[12] == "" || values[12] == "0") ? null : db.grade.Where(g => g.name.ToLower() == strValue12).FirstOrDefault(); //.Find(int.Parse(values[12]));
                            Location loc = (values[13] == "" || values[13] == "0") ? null : db.location.Where(l => l.name.ToLower() == strValue13).FirstOrDefault(); //.Find(int.Parse(values[13]));
                            Region reg = (values[14] == "" || values[14] == "0") ? null : db.region.Where(r => r.name.ToLower() == strValue14).FirstOrDefault(); //.Find(int.Parse(values[14]));
                            TypeOfEmployment toe = (values[15] == "" || int.Parse(values[15]) == 0) ? null : db.type_of_employment.Find(int.Parse(values[15]));

                            ////string accessGroupName = values[8];
                            ////AccessGroup accessGroup = db.access_group.Where(m => m.name.Equals(accessGroupName)).FirstOrDefault();

                            int sLeaves = 0; int.TryParse(values[17].ToString(), out sLeaves);
                            int cLeaves = 0; int.TryParse(values[18].ToString(), out cLeaves);
                            int aLeaves = 0; int.TryParse(values[19].ToString(), out aLeaves);
                            int oLeaves = 0; int.TryParse(values[20].ToString(), out oLeaves);
                            int iLeaves01 = 0; int.TryParse(values[21].ToString(), out iLeaves01);
                            int iLeaves02 = 0; int.TryParse(values[22].ToString(), out iLeaves02);
                            int iLeaves03 = 0; int.TryParse(values[23].ToString(), out iLeaves03);
                            int iLeaves04 = 0; int.TryParse(values[24].ToString(), out iLeaves04);
                            int iLeaves05 = 0; int.TryParse(values[25].ToString(), out iLeaves05);
                            int iLeaves06 = 0; int.TryParse(values[26].ToString(), out iLeaves06);
                            int iLeaves07 = 0; int.TryParse(values[27].ToString(), out iLeaves07);
                            int iLeaves08 = 0; int.TryParse(values[28].ToString(), out iLeaves08);
                            int iLeaves09 = 0; int.TryParse(values[29].ToString(), out iLeaves09);
                            int iLeaves10 = 0; int.TryParse(values[30].ToString(), out iLeaves10);
                            int iLeaves11 = 0; int.TryParse(values[31].ToString(), out iLeaves11);

                            int iGenderId = (values[43] != null && values[43].ToString() != "") ? db.gender.Where(c => c.GenderName.ToLower() == strValue43).FirstOrDefault().Id : 0;
                            int iCampusId = (values[44] != null && values[44].ToString() != "") ? db.organization_campus.Where(c => c.CampusCode.ToLower() == strValue44).FirstOrDefault().Id : 0;
                            int iProgramShiftId = (values[45] != null && values[45].ToString() != "") ? db.organization_program_shift.Where(c => c.ProgramShiftName.ToLower() == strValue45).FirstOrDefault().Id : 0;

                            //IR added on 19 Dec 2020
                            date_of_joining = (values[6] != null || values[6] == "") ? (DateTime?)DateTime.ParseExact(values[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null;
                            date_of_leaving = (values[7] == null || values[7] == "") ? null : (DateTime?)DateTime.ParseExact(values[7], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                            string str_cnic_no = ".";
                            str_cnic_no = (values[46] == null || values[46] == "") ? "." : values[46].ToString();

                            Employee emp = new Employee()
                            {
                                first_name = values[1],
                                last_name = values[2],
                                employee_code = employeeCode,
                                cnic_no = str_cnic_no,
                                email = values[3],
                                address = values[4],
                                mobile_no = values[5],
                                date_of_joining = (values[6] != null || values[6] == "") ? (DateTime?)DateTime.ParseExact(values[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null,
                                date_of_leaving = (values[7] == null || values[7] == "") ? null : (DateTime?)DateTime.ParseExact(values[7], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                access_group = accessGroup,
                                function = func,
                                designation = desig,
                                department = dept,
                                type_of_employment = toe,
                                grade = grd,
                                //Group = (int.Parse(values[13]) == 0) ? null : db.group.Find(int.Parse(values[13])),
                                region = reg,
                                location = loc,
                                position_id = 1,
                                site_id = 1,
                                timetune_active = convertStringToBool(values[16]),
                                active = true,
                                sick_leaves = sLeaves,
                                casual_leaves = cLeaves,
                                annual_leaves = aLeaves,
                                other_leaves = oLeaves,
                                leave_type01 = iLeaves01,
                                leave_type02 = iLeaves02,
                                leave_type03 = iLeaves03,
                                leave_type04 = iLeaves04,
                                leave_type05 = iLeaves05,
                                leave_type06 = iLeaves06,
                                leave_type07 = iLeaves07,
                                leave_type08 = iLeaves08,
                                leave_type09 = iLeaves09,
                                leave_type10 = iLeaves10,
                                leave_type11 = iLeaves11,
                                photograph = values[32],//25
                                file_01 = values[33],//26
                                file_02 = values[34],
                                file_03 = values[35],
                                file_04 = values[36],
                                file_05 = values[37],
                                emergency_contact_01 = values[38],
                                emergency_contact_02 = values[39],
                                description = values[40],//33
                                date_of_birth = (values[41] != null || values[41] == "") ? (DateTime?)DateTime.ParseExact(values[41], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null,
                                father_name = (values[42] == "" || values[42] == "0") ? null : values[42],
                                gender_id = iGenderId,
                                campus_id = iCampusId,
                                program_shift_id = iProgramShiftId,
                                skills_set = "1"
                            };

                            // On employee create the default password
                            // is set to the employee code.
                            // the employee needs to change this 
                            // after the first log in.
                            DLL.Commons.Passwords.setPassword(emp, employeeCode);

                            //OR Password Set
                            //string[] pwd_salt = DLL.Commons.Passwords.generatePasswordAndSalt(employeeCode);
                            //emp.password = pwd_salt[0];
                            //emp.salt = pwd_salt[1];

                            db.employee.Add(emp);
                            db.SaveChanges();

                            //IR - User Persistant entry
                            PersistentAttendanceLog pl = new PersistentAttendanceLog()
                            {
                                Employee = emp,
                                active = true,
                                employee_code = emp.employee_code
                            };
                            db.persistent_attendance_log.Add(pl);
                            db.SaveChanges();

                            TimeTune.AuditTrail.insert("{\"*id\" : \"" + emp.EmployeeId.ToString() + "\"}", "Employees", user_code);

                            //IR - User Payroll entry
                            int sumAmount = 0;
                            Payroll pEmployee = new Payroll();

                            pEmployee.EmployeeId = emp.EmployeeId;

                            //IR ADDEmp
                            int resp = 0;
                            resp = TimeTune.EmployeeCrud.resetPermissionsByEmpID(emp.EmployeeId);

                            //////////////////////////////////////////////////////////////////////////////////////


                            string strFuncName = "";
                            strFuncName = func.name.ToLower() ?? "";
                            //set salary amounts using designation and grade of employee
                            if (strFuncName == "student")
                            {
                                //do nothing
                            }
                            else
                            {
                                //set salary amounts using designation and grade of employee
                                if (desig != null && desig.DesignationId > 0 && grd != null && grd.GradeId > 0)
                                {
                                    ViewModels.PayrollAmountInfo pAmount = ViewModels.PayrollResultSet.IsPayrollAmountExists(desig.DesignationId, grd.GradeId);
                                    if (pAmount != null)
                                    {
                                        pEmployee.BasicPay = pAmount.BasicPay;
                                        pEmployee.Increment = pAmount.Increment;
                                        pEmployee.Transport = pAmount.Transport;
                                        pEmployee.Mobile = pAmount.Mobile;
                                        pEmployee.Medical = pAmount.Medical;
                                        pEmployee.CashAllowance = pAmount.CashAllowance;
                                        pEmployee.Commission = pAmount.Commission;
                                        pEmployee.Food = pAmount.Food;
                                        pEmployee.Night = pAmount.Night;
                                        pEmployee.Rent = pAmount.Rent;
                                        pEmployee.GroupAllowance = pAmount.GroupAllowance;
                                        sumAmount = pEmployee.BasicPay + pEmployee.Increment + pEmployee.Transport + pEmployee.Mobile + pEmployee.Medical + pEmployee.CashAllowance + pEmployee.Commission + pEmployee.Food + pEmployee.Night + pEmployee.Rent + pEmployee.GroupAllowance;
                                    }
                                    else
                                    {
                                        pEmployee.BasicPay = 0;
                                        pEmployee.Increment = 0;
                                        pEmployee.Transport = 0;
                                        pEmployee.Mobile = 0;
                                        pEmployee.Medical = 0;
                                        pEmployee.CashAllowance = 0;
                                        pEmployee.Commission = 0;
                                        pEmployee.Food = 0;
                                        pEmployee.Night = 0;
                                        pEmployee.Rent = 0;
                                        pEmployee.GroupAllowance = 0;
                                    }
                                }
                                else
                                {
                                    pEmployee.BasicPay = 0;
                                    pEmployee.Increment = 0;
                                    pEmployee.Transport = 0;
                                    pEmployee.Mobile = 0;
                                    pEmployee.Medical = 0;
                                    pEmployee.CashAllowance = 0;
                                    pEmployee.Commission = 0;
                                    pEmployee.Food = 0;
                                    pEmployee.Night = 0;
                                    pEmployee.Rent = 0;
                                    pEmployee.GroupAllowance = 0;
                                }

                                pEmployee.GrossSalary = sumAmount;
                                pEmployee.NetSalary = sumAmount;

                                pEmployee.PaymentModeId = 1;
                                pEmployee.BankNameId = 1;
                                pEmployee.AckStatusId = 1;
                                pEmployee.PayStatusId = 1;
                                pEmployee.IsFirstMonthText = "YES";
                                pEmployee.SalaryMonthYear = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));
                                pEmployee.PaymentDateTime = Convert.ToDateTime(DateTime.Now.AddDays(5).AddMonths(1).ToString("yyyy-MM-dd 00:00:00.000"));
                                pEmployee.CreateDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));
                                pEmployee.UpdateDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

                                db.payroll.Add(pEmployee);
                                db.SaveChanges();
                            }
                            //////////////////////////////////////////////////////////////////////////////////////

                            //IR - User Session - Leave or Salary

                            if (toe.TypeOfEmploymentId != null && toe.TypeOfEmploymentId == 1)//1-permanent
                            {
                                ManageLeaveSessionForUsers(emp.employee_code, sLeaves, cLeaves, aLeaves, oLeaves, iLeaves01, iLeaves02, iLeaves03, iLeaves04, iLeaves05, iLeaves06, iLeaves07, iLeaves08, iLeaves09, iLeaves10, iLeaves11);
                            }
                            else if (toe.TypeOfEmploymentId != null && toe.TypeOfEmploymentId == 2)//2-probation
                            {
                                ManageLeaveSessionForUsers(emp.employee_code, sLeaves, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                            }
                            else//3-contract - all 0s
                            {
                                if (date_of_joining != null && date_of_leaving != null)
                                {
                                    //double days_diff = (date_of_leaving - date_of_joining).Value.TotalDays;
                                    //double mon_diff = (date_of_leaving - date_of_joining).Value.TotalDays;

                                    DateTime compareTo = date_of_joining.Value;
                                    DateTime compareWith = date_of_leaving.Value;
                                    var dateSpan = DateTimeSpan.CompareDates(compareTo, compareWith);
                                    Console.WriteLine("Days: " + dateSpan.Days);
                                    Console.WriteLine("Months: " + dateSpan.Months);
                                    Console.WriteLine("Years: " + dateSpan.Years);

                                    int total_days = 0, total_months = 0, total_year = 0;
                                    total_days = dateSpan.Days; total_months = dateSpan.Months; total_year = dateSpan.Years;

                                    if (total_months >= 0 && total_months <= 11)//6 months
                                    {
                                        if (total_months < 6)//6 months
                                        {
                                            int round_leaves = 0;
                                            round_leaves = int.Parse(Math.Ceiling(total_months * 2.5).ToString());

                                            //15 leaves added as Casual
                                            ManageLeaveSessionForUsers(emp.employee_code, 0, round_leaves, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                                        }
                                        else
                                        {
                                            //15 leaves added as Casual
                                            ManageLeaveSessionForUsers(emp.employee_code, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                                        }
                                    }
                                    else if (total_year == 1)//12 months = 1 year
                                    {
                                        ManageLeaveSessionForUsers(emp.employee_code, 15, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                                    }
                                    else if (total_year == 2)//2 years
                                    {
                                        ManageLeaveSessionForUsers(emp.employee_code, 30, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                                    }
                                    else
                                    {
                                        ManageLeaveSessionForUsers(emp.employee_code, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                                    }
                                }
                                else
                                {
                                    ManageLeaveSessionForUsers(emp.employee_code, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                                }
                            }

                            //{
                            //ManageLeaveSessionForUsers(emp.employee_code, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                            //}






                            ////ManageLeaveSessionForUsers(emp.EmployeeId, sLeaves, cLeaves, aLeaves);
                        }
                        //return "Successfull";
                        //}
                        //catch (Exception)
                        //{
                        //    return "Failed";
                        //}
                    }

                    return "Successfull";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        public static string setLeaveSession(List<string> csvContents, string user_code)
        {

            DateTime? date_of_joining = DateTime.Now; // (fromForm.date_of_joining != null) ? (DateTime?)DateTime.ParseExact(fromForm.date_of_joining, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null;
            DateTime? date_of_leaving = DateTime.Now; //(fromForm.date_of_leaving != null) ? (DateTime?)DateTime.ParseExact(fromForm.date_of_leaving, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null;

            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 47 values (columns).

                        if (values == null || values.Length < 18)
                            continue;

                        // Remove unwanted " characters.
                        string employeeCode = values[0].Replace("\"", "");
                        values[1] = values[1].Replace("\"", "");
                        values[2] = values[2].Replace("\"", "");

                        values[3] = values[3].Replace("\"", "");//sick_leaves
                        values[4] = values[4].Replace("\"", "");
                        values[5] = values[5].Replace("\"", "");
                        values[6] = values[6].Replace("\"", "");

                        values[7] = values[7].Replace("\"", "");//leave type 01
                        values[8] = values[8].Replace("\"", "");
                        values[9] = values[9].Replace("\"", "");
                        values[10] = values[10].Replace("\"", "");
                        values[11] = values[11].Replace("\"", "");//leave type 05
                        values[12] = values[12].Replace("\"", "");
                        values[13] = values[13].Replace("\"", "");
                        values[14] = values[14].Replace("\"", "");
                        values[15] = values[15].Replace("\"", "");//leave type 09
                        values[16] = values[16].Replace("\"", "");
                        values[17] = values[17].Replace("\"", "");

                        // continue if its the first row (CSV Header line).
                        if (employeeCode == "employeeCode" || values[1] == "dateOfStart" || values[3] == "sick_leaves")
                        {
                            continue;
                        }
                        // Get the employee 
                        Employee employee = db.employee.Where(m =>
                            m.employee_code.Equals(employeeCode)).FirstOrDefault();

                        //try
                        //{
                        // If the line manager and the employee both exist.
                        if (employee != null)
                        {
                            //DateTime dtSessionStartDate = DateTime.ParseExact(values[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            DateTime dtSessionEndDate = DateTime.ParseExact(values[2], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                            LeaveSession leaveSession = db.leave_session.Where(m =>
                                    m.EmployeeId == employee.EmployeeId && m.YearId == dtSessionEndDate.Year).FirstOrDefault();

                            if (leaveSession != null)
                            {
                                leaveSession.EmployeeId = employee.EmployeeId;

                                leaveSession.SessionStartDate = DateTime.ParseExact(values[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                leaveSession.SessionEndDate = DateTime.ParseExact(values[2], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                                leaveSession.YearId = leaveSession.SessionEndDate.Year;

                                int sLeaves = 0; int.TryParse(values[3].ToString(), out sLeaves);
                                int cLeaves = 0; int.TryParse(values[4].ToString(), out cLeaves);
                                int aLeaves = 0; int.TryParse(values[5].ToString(), out aLeaves);
                                int oLeaves = 0; int.TryParse(values[6].ToString(), out oLeaves);

                                leaveSession.SickLeaves = sLeaves;
                                leaveSession.CasualLeaves = cLeaves;
                                leaveSession.AnnualLeaves = aLeaves;
                                leaveSession.OtherLeaves = oLeaves;

                                int iLeaves01 = 0; int.TryParse(values[7].ToString(), out iLeaves01);
                                int iLeaves02 = 0; int.TryParse(values[8].ToString(), out iLeaves02);
                                int iLeaves03 = 0; int.TryParse(values[9].ToString(), out iLeaves03);
                                int iLeaves04 = 0; int.TryParse(values[10].ToString(), out iLeaves04);
                                int iLeaves05 = 0; int.TryParse(values[11].ToString(), out iLeaves05);
                                int iLeaves06 = 0; int.TryParse(values[12].ToString(), out iLeaves06);
                                int iLeaves07 = 0; int.TryParse(values[13].ToString(), out iLeaves07);
                                int iLeaves08 = 0; int.TryParse(values[14].ToString(), out iLeaves08);
                                int iLeaves09 = 0; int.TryParse(values[15].ToString(), out iLeaves09);
                                int iLeaves10 = 0; int.TryParse(values[16].ToString(), out iLeaves10);
                                int iLeaves11 = 0; int.TryParse(values[17].ToString(), out iLeaves11);

                                leaveSession.LeaveType01 = iLeaves01;
                                leaveSession.LeaveType02 = iLeaves02;
                                leaveSession.LeaveType03 = iLeaves03;
                                leaveSession.LeaveType04 = iLeaves04;
                                leaveSession.LeaveType05 = iLeaves05;
                                leaveSession.LeaveType06 = iLeaves06;
                                leaveSession.LeaveType07 = iLeaves07;
                                leaveSession.LeaveType08 = iLeaves08;
                                leaveSession.LeaveType09 = iLeaves09;
                                leaveSession.LeaveType10 = iLeaves10;
                                leaveSession.LeaveType11 = iLeaves11;

                                db.SaveChanges();
                            }
                            else
                            {
                                //new session
                                LeaveSession leaveSessionNew = new LeaveSession();

                                leaveSessionNew.EmployeeId = employee.EmployeeId;

                                leaveSessionNew.SessionStartDate = DateTime.ParseExact(values[1], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                leaveSessionNew.SessionEndDate = DateTime.ParseExact(values[2], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                                leaveSessionNew.YearId = leaveSessionNew.SessionEndDate.Year;

                                int sLeaves = 0; int.TryParse(values[3].ToString(), out sLeaves);
                                int cLeaves = 0; int.TryParse(values[4].ToString(), out cLeaves);
                                int aLeaves = 0; int.TryParse(values[5].ToString(), out aLeaves);
                                int oLeaves = 0; int.TryParse(values[6].ToString(), out oLeaves);

                                leaveSessionNew.SickLeaves = sLeaves;
                                leaveSessionNew.CasualLeaves = cLeaves;
                                leaveSessionNew.AnnualLeaves = aLeaves;
                                leaveSessionNew.OtherLeaves = oLeaves;

                                int iLeaves01 = 0; int.TryParse(values[7].ToString(), out iLeaves01);
                                int iLeaves02 = 0; int.TryParse(values[8].ToString(), out iLeaves02);
                                int iLeaves03 = 0; int.TryParse(values[9].ToString(), out iLeaves03);
                                int iLeaves04 = 0; int.TryParse(values[10].ToString(), out iLeaves04);
                                int iLeaves05 = 0; int.TryParse(values[11].ToString(), out iLeaves05);
                                int iLeaves06 = 0; int.TryParse(values[12].ToString(), out iLeaves06);
                                int iLeaves07 = 0; int.TryParse(values[13].ToString(), out iLeaves07);
                                int iLeaves08 = 0; int.TryParse(values[14].ToString(), out iLeaves08);
                                int iLeaves09 = 0; int.TryParse(values[15].ToString(), out iLeaves09);
                                int iLeaves10 = 0; int.TryParse(values[16].ToString(), out iLeaves10);
                                int iLeaves11 = 0; int.TryParse(values[17].ToString(), out iLeaves11);

                                leaveSessionNew.LeaveType01 = iLeaves01;
                                leaveSessionNew.LeaveType02 = iLeaves02;
                                leaveSessionNew.LeaveType03 = iLeaves03;
                                leaveSessionNew.LeaveType04 = iLeaves04;
                                leaveSessionNew.LeaveType05 = iLeaves05;
                                leaveSessionNew.LeaveType06 = iLeaves06;
                                leaveSessionNew.LeaveType07 = iLeaves07;
                                leaveSessionNew.LeaveType08 = iLeaves08;
                                leaveSessionNew.LeaveType09 = iLeaves09;
                                leaveSessionNew.LeaveType10 = iLeaves10;
                                leaveSessionNew.LeaveType11 = iLeaves11;

                                db.leave_session.Add(leaveSessionNew);
                                db.SaveChanges();
                            }
                        }


                        //return "Successfull";
                        //}
                        //catch (Exception)
                        //{
                        //    return "Failed";
                        //}
                    }

                    return "Successfull";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        public static string setEmployeesCalendar00(List<string> csvContents, string user_code)
        {

            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 39 values (columns).

                        if (values == null || values.Length < 46)
                            continue;

                        // Remove unwanted " characters.
                        string employeeCode = values[0].Replace("\"", "");
                        values[1] = values[1].Replace("\"", "");
                        values[2] = values[2].Replace("\"", "");
                        values[3] = values[3].Replace("\"", "");
                        values[4] = values[4].Replace("\"", "");
                        values[5] = values[5].Replace("\"", "");
                        values[6] = values[6].ToLower().Replace("\"", "");
                        values[7] = values[7].ToLower().Replace("\"", "");
                        values[8] = values[8].ToLower().Replace("\"", ""); string strValue8 = values[8];
                        values[9] = values[9].ToLower().Replace("\"", ""); string strValue9 = values[9];
                        values[10] = values[10].ToLower().Replace("\"", ""); string strValue10 = values[10];
                        values[11] = values[11].ToLower().Replace("\"", ""); string strValue11 = values[11];
                        values[12] = values[12].ToLower().Replace("\"", ""); string strValue12 = values[12];
                        values[13] = values[13].ToLower().Replace("\"", ""); string strValue13 = values[13];
                        values[14] = values[14].ToLower().Replace("\"", ""); string strValue14 = values[14];
                        values[15] = values[15].Replace("\"", "");
                        values[16] = values[16].Replace("\"", "");

                        values[17] = values[17].Replace("\"", "");//sick_leaves
                        values[18] = values[18].Replace("\"", "");
                        values[19] = values[19].Replace("\"", "");
                        values[20] = values[20].Replace("\"", "");

                        values[21] = values[21].Replace("\"", "");//leave type 01
                        values[22] = values[22].Replace("\"", "");
                        values[23] = values[23].Replace("\"", "");
                        values[24] = values[24].Replace("\"", "");
                        values[25] = values[25].Replace("\"", "");//leave type 05
                        values[26] = values[26].Replace("\"", "");
                        values[27] = values[27].Replace("\"", "");
                        values[28] = values[28].Replace("\"", "");
                        values[29] = values[29].Replace("\"", "");//leave type 09
                        values[30] = values[30].Replace("\"", "");
                        values[31] = values[31].Replace("\"", "");

                        values[32] = values[32].Replace("\"", "");//photo//20
                        values[33] = values[33].Replace("\"", "");//file01//21
                        values[34] = values[34].Replace("\"", "");//22
                        values[35] = values[35].Replace("\"", "");//23
                        values[36] = values[36].Replace("\"", "");//24
                        values[37] = values[37].Replace("\"", "");//Driving License
                        values[38] = values[38].Replace("\"", "");//emergency_contact
                        values[39] = values[39].Replace("\"", "");//emergency_contact
                        values[40] = values[40].Replace("\"", "");//description
                        values[41] = values[41].ToLower().Replace("\"", "");//date of birth
                        values[42] = values[42].ToLower().Replace("\"", "");//father name
                        values[43] = values[43].ToLower().Replace("\"", ""); string strValue43 = values[43];//gender
                        values[44] = values[44].ToLower().Replace("\"", ""); string strValue44 = values[44];//campus code
                        values[45] = values[45].ToLower().Replace("\"", ""); string strValue45 = values[45];//campus code//p-shift

                        // continue if its the first row (CSV Header line).
                        if (employeeCode == "employeeCode" || values[6] == "dateOfJoining" || values[17] == "sick_leaves" || values[34] == "date_of_birth" || values[38] == "program_shift")
                        {
                            continue;
                        }
                        // Get the employee 
                        Employee employee = db.employee.Where(m =>
                            m.employee_code.Equals(employeeCode)).FirstOrDefault();

                        //try
                        //{
                        // If the line manager and the employee both exist.
                        if (employee != null)
                        {
                            //for (int i = 0; i < 18; i++)
                            //{
                            //Update Employee
                            employee.first_name = values[1];
                            employee.last_name = values[2];
                            employee.email = values[3];
                            employee.address = values[4];
                            employee.mobile_no = values[5];

                            //string strJDate = values[6].Replace("\"", "");//date
                            //string strProperJoinDate = DateTime.ParseExact(strJDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            //if (strProperJoinDate != "")
                            //{
                            //    strJDate = strProperJoinDate;
                            //}


                            employee.date_of_joining = (values[6] == null || values[6] == "") ? null : (DateTime?)DateTime.ParseExact(values[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            employee.date_of_leaving = (values[7] == null || values[7] == "") ? null : (DateTime?)DateTime.ParseExact(values[7], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                            // Set access group, as per the rules defined in 'edit employee'.
                            ////string accessGroupName = values[8];
                            ////AccessGroup accessGroup = db.access_group.Where(m => m.name.Equals(accessGroupName)).FirstOrDefault();

                            ////// access group cannot be null.
                            ////if (accessGroup != null)
                            ////{
                            ////    TimeTune.EmployeeCrud.changeEmployeeAccessGroup(accessGroup.AccessGroupId, db, employee);
                            ////}
                            ////else
                            ////{
                            ////    continue;
                            ////}

                            employee.access_group = (values[8] == "" || values[8] == "0") ? null : db.access_group.Where(a => a.name.ToLower().Contains(strValue8)).FirstOrDefault();//.Find(int.Parse(values[8]));
                            employee.department = (values[9] == "" || values[9] == "0") ? null : db.department.Where(d => d.name.ToLower() == strValue9).FirstOrDefault(); //.Find(int.Parse(values[9]));
                            employee.designation = (values[10] == "" || values[10] == "0") ? null : db.designation.Where(d => d.name.ToLower() == strValue10).FirstOrDefault(); //.Find(int.Parse(values[10]));
                            employee.function = (values[11] == "" || values[11] == "0") ? null : db.function.Where(f => f.name.ToLower() == strValue11).FirstOrDefault();
                            employee.grade = (values[12] == "" || values[12] == "0") ? null : db.grade.Where(g => g.name.ToLower() == strValue12).FirstOrDefault(); //.Find(int.Parse(values[12]));
                            //employee.Group = (int.Parse(values[13]) == 0) ? null : db.group.Find(int.Parse(values[13]));
                            employee.location = (values[13] == "" || values[13] == "0") ? null : db.location.Where(l => l.name.ToLower() == strValue13).FirstOrDefault(); //.Find(int.Parse(values[13]));
                            employee.region = (values[14] == "" || values[14] == "0") ? null : db.region.Where(r => r.name.ToLower() == strValue14).FirstOrDefault(); //.Find(int.Parse(values[14]));

                            employee.type_of_employment = (values[15] == "" || int.Parse(values[15]) == 0) ? null : db.type_of_employment.Find(int.Parse(values[15]));
                            employee.timetune_active = convertStringToBool(values[16]);

                            employee.active = true;

                            int sLeaves = 0; int.TryParse(values[17].ToString(), out sLeaves);
                            int cLeaves = 0; int.TryParse(values[18].ToString(), out cLeaves);
                            int aLeaves = 0; int.TryParse(values[19].ToString(), out aLeaves);
                            int oLeaves = 0; int.TryParse(values[20].ToString(), out oLeaves);

                            employee.sick_leaves = sLeaves;
                            employee.casual_leaves = cLeaves;
                            employee.annual_leaves = aLeaves;
                            employee.other_leaves = oLeaves;

                            int iLeaves01 = 0; int.TryParse(values[21].ToString(), out iLeaves01);
                            int iLeaves02 = 0; int.TryParse(values[22].ToString(), out iLeaves02);
                            int iLeaves03 = 0; int.TryParse(values[23].ToString(), out iLeaves03);
                            int iLeaves04 = 0; int.TryParse(values[24].ToString(), out iLeaves04);
                            int iLeaves05 = 0; int.TryParse(values[25].ToString(), out iLeaves05);
                            int iLeaves06 = 0; int.TryParse(values[26].ToString(), out iLeaves06);
                            int iLeaves07 = 0; int.TryParse(values[27].ToString(), out iLeaves07);
                            int iLeaves08 = 0; int.TryParse(values[28].ToString(), out iLeaves08);
                            int iLeaves09 = 0; int.TryParse(values[29].ToString(), out iLeaves09);
                            int iLeaves10 = 0; int.TryParse(values[30].ToString(), out iLeaves10);
                            int iLeaves11 = 0; int.TryParse(values[31].ToString(), out iLeaves11);

                            employee.leave_type01 = iLeaves01;
                            employee.leave_type02 = iLeaves02;
                            employee.leave_type03 = iLeaves03;
                            employee.leave_type04 = iLeaves04;
                            employee.leave_type05 = iLeaves05;
                            employee.leave_type06 = iLeaves06;
                            employee.leave_type07 = iLeaves07;
                            employee.leave_type08 = iLeaves08;
                            employee.leave_type09 = iLeaves09;
                            employee.leave_type10 = iLeaves10;
                            employee.leave_type11 = iLeaves11;

                            employee.photograph = values[32];//25
                            employee.file_01 = values[33];//26
                            employee.file_02 = values[34];//27
                            employee.file_03 = values[35];//28
                            employee.file_04 = values[36];//29
                            employee.file_05 = values[37];//30
                            employee.emergency_contact_01 = values[38];//31
                            employee.emergency_contact_02 = values[39];//32
                            employee.description = values[40];//33
                            employee.date_of_birth = (values[41] == null || values[41] == "") ? null : (DateTime?)DateTime.ParseExact(values[41], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);//34

                            employee.father_name = (values[42] == "" || values[42] == "0") ? null : values[42];//35
                            employee.gender_id = (values[43] != null && values[43].ToString() != "") ? db.gender.Where(c => c.GenderName.ToLower() == strValue43).FirstOrDefault().Id : 0;//36
                            employee.campus_id = (values[44] != null && values[44].ToString() != "") ? db.organization_campus.Where(c => c.CampusCode.ToLower() == strValue44).FirstOrDefault().Id : 0;//37
                            employee.program_shift_id = (values[45] != null && values[45].ToString() != "") ? db.organization_program_shift.Where(c => c.ProgramShiftName.ToLower() == strValue45).FirstOrDefault().Id : 0;//38

                            db.SaveChanges();

                            TimeTune.AuditTrail.update("{\"*id\" : \"" + employee.EmployeeId.ToString() + "\"}", "Employees", user_code);

                            //}
                            ////db.SaveChanges();
                            //return "Successfull";
                        }
                        else //new employee
                        {
                            string aGroupName = (values[8] == "" || values[8] == "0") ? "EMP" : values[8]; //set for emp default
                            AccessGroup accessGroup = db.access_group.Where(m => m.name.ToLower().Contains(aGroupName)).FirstOrDefault();

                            Department dept = (values[9] == "" || values[9] == "0") ? null : db.department.Where(d => d.name.ToLower() == strValue9).FirstOrDefault(); //.Find(int.Parse(values[9]));
                            Designation desig = (values[10] == "" || values[10] == "0") ? null : db.designation.Where(d => d.name.ToLower() == strValue10).FirstOrDefault(); //.Find(int.Parse(values[10]));
                            Function func = (values[11] == "" || values[11] == "0") ? null : db.function.Where(f => f.name.ToLower() == strValue11).FirstOrDefault(); //.Find(int.Parse(values[11]));
                            Grade grd = (values[12] == "" || values[12] == "0") ? null : db.grade.Where(g => g.name.ToLower() == strValue12).FirstOrDefault(); //.Find(int.Parse(values[12]));
                            Location loc = (values[13] == "" || values[13] == "0") ? null : db.location.Where(l => l.name.ToLower() == strValue13).FirstOrDefault(); //.Find(int.Parse(values[13]));
                            Region reg = (values[14] == "" || values[14] == "0") ? null : db.region.Where(r => r.name.ToLower() == strValue14).FirstOrDefault(); //.Find(int.Parse(values[14]));
                            TypeOfEmployment toe = (values[15] == "" || int.Parse(values[15]) == 0) ? null : db.type_of_employment.Find(int.Parse(values[15]));

                            ////string accessGroupName = values[8];
                            ////AccessGroup accessGroup = db.access_group.Where(m => m.name.Equals(accessGroupName)).FirstOrDefault();

                            int sLeaves = 0; int.TryParse(values[17].ToString(), out sLeaves);
                            int cLeaves = 0; int.TryParse(values[18].ToString(), out cLeaves);
                            int aLeaves = 0; int.TryParse(values[19].ToString(), out aLeaves);
                            int oLeaves = 0; int.TryParse(values[20].ToString(), out oLeaves);
                            int iLeaves01 = 0; int.TryParse(values[21].ToString(), out iLeaves01);
                            int iLeaves02 = 0; int.TryParse(values[22].ToString(), out iLeaves02);
                            int iLeaves03 = 0; int.TryParse(values[23].ToString(), out iLeaves03);
                            int iLeaves04 = 0; int.TryParse(values[24].ToString(), out iLeaves04);
                            int iLeaves05 = 0; int.TryParse(values[25].ToString(), out iLeaves05);
                            int iLeaves06 = 0; int.TryParse(values[26].ToString(), out iLeaves06);
                            int iLeaves07 = 0; int.TryParse(values[27].ToString(), out iLeaves07);
                            int iLeaves08 = 0; int.TryParse(values[28].ToString(), out iLeaves08);
                            int iLeaves09 = 0; int.TryParse(values[29].ToString(), out iLeaves09);
                            int iLeaves10 = 0; int.TryParse(values[30].ToString(), out iLeaves10);
                            int iLeaves11 = 0; int.TryParse(values[31].ToString(), out iLeaves11);

                            int iGenderId = (values[43] != null && values[43].ToString() != "") ? db.gender.Where(c => c.GenderName.ToLower() == strValue43).FirstOrDefault().Id : 0;
                            int iCampusId = (values[44] != null && values[44].ToString() != "") ? db.organization_campus.Where(c => c.CampusCode.ToLower() == strValue44).FirstOrDefault().Id : 0;
                            int iProgramShiftId = (values[45] != null && values[45].ToString() != "") ? db.organization_program_shift.Where(c => c.ProgramShiftName.ToLower() == strValue45).FirstOrDefault().Id : 0;

                            Employee emp = new Employee()
                            {
                                first_name = values[1],
                                last_name = values[2],
                                employee_code = employeeCode,
                                email = values[3],
                                address = values[4],
                                mobile_no = values[5],
                                date_of_joining = (values[6] != null || values[6] == "") ? (DateTime?)DateTime.ParseExact(values[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null,
                                date_of_leaving = (values[7] == null || values[7] == "") ? null : (DateTime?)DateTime.ParseExact(values[7], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                access_group = accessGroup,
                                function = func,
                                designation = desig,
                                department = dept,
                                type_of_employment = toe,
                                grade = grd,
                                //Group = (int.Parse(values[13]) == 0) ? null : db.group.Find(int.Parse(values[13])),
                                region = reg,
                                location = loc,
                                timetune_active = convertStringToBool(values[16]),
                                active = true,
                                sick_leaves = sLeaves,
                                casual_leaves = cLeaves,
                                annual_leaves = aLeaves,
                                other_leaves = oLeaves,
                                leave_type01 = iLeaves01,
                                leave_type02 = iLeaves02,
                                leave_type03 = iLeaves03,
                                leave_type04 = iLeaves04,
                                leave_type05 = iLeaves05,
                                leave_type06 = iLeaves06,
                                leave_type07 = iLeaves07,
                                leave_type08 = iLeaves08,
                                leave_type09 = iLeaves09,
                                leave_type10 = iLeaves10,
                                leave_type11 = iLeaves11,
                                photograph = values[32],//25
                                file_01 = values[33],//26
                                file_02 = values[34],
                                file_03 = values[35],
                                file_04 = values[36],
                                file_05 = values[37],
                                emergency_contact_01 = values[38],
                                emergency_contact_02 = values[39],
                                description = values[40],//33
                                date_of_birth = (values[41] != null || values[41] == "") ? (DateTime?)DateTime.ParseExact(values[41], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null,
                                father_name = (values[42] == "" || values[42] == "0") ? null : values[42],
                                gender_id = iGenderId,
                                campus_id = iCampusId,
                                program_shift_id = iProgramShiftId,
                                skills_set = "1"
                            };

                            // On employee create the default password
                            // is set to the employee code.
                            // the employee needs to change this 
                            // after the first log in.
                            DLL.Commons.Passwords.setPassword(emp, employeeCode);

                            //OR Password Set
                            //string[] pwd_salt = DLL.Commons.Passwords.generatePasswordAndSalt(employeeCode);
                            //emp.password = pwd_salt[0];
                            //emp.salt = pwd_salt[1];

                            db.employee.Add(emp);
                            db.SaveChanges();

                            //IR - User Persistant entry
                            PersistentAttendanceLog pl = new PersistentAttendanceLog()
                            {
                                Employee = emp,
                                active = true,
                                employee_code = emp.employee_code
                            };
                            db.persistent_attendance_log.Add(pl);
                            db.SaveChanges();

                            TimeTune.AuditTrail.insert("{\"*id\" : \"" + emp.EmployeeId.ToString() + "\"}", "Employees", user_code);

                            //IR - User Payroll entry
                            int sumAmount = 0;
                            Payroll pEmployee = new Payroll();

                            pEmployee.EmployeeId = emp.EmployeeId;
                            //////////////////////////////////////////////////////////////////////////////////////


                            string strFuncName = "";
                            strFuncName = func.name.ToLower() ?? "";
                            //set salary amounts using designation and grade of employee
                            if (strFuncName == "student")
                            {
                                //do nothing
                            }
                            else
                            {
                                //set salary amounts using designation and grade of employee
                                if (desig != null && desig.DesignationId > 0 && grd != null && grd.GradeId > 0)
                                {
                                    ViewModels.PayrollAmountInfo pAmount = ViewModels.PayrollResultSet.IsPayrollAmountExists(desig.DesignationId, grd.GradeId);
                                    if (pAmount != null)
                                    {
                                        pEmployee.BasicPay = pAmount.BasicPay;
                                        pEmployee.Increment = pAmount.Increment;
                                        pEmployee.Transport = pAmount.Transport;
                                        pEmployee.Mobile = pAmount.Mobile;
                                        pEmployee.Medical = pAmount.Medical;
                                        pEmployee.CashAllowance = pAmount.CashAllowance;
                                        pEmployee.Commission = pAmount.Commission;
                                        pEmployee.Food = pAmount.Food;
                                        pEmployee.Night = pAmount.Night;
                                        pEmployee.Rent = pAmount.Rent;
                                        pEmployee.GroupAllowance = pAmount.GroupAllowance;
                                        sumAmount = pEmployee.BasicPay + pEmployee.Increment + pEmployee.Transport + pEmployee.Mobile + pEmployee.Medical + pEmployee.CashAllowance + pEmployee.Commission + pEmployee.Food + pEmployee.Night + pEmployee.Rent + pEmployee.GroupAllowance;
                                    }
                                    else
                                    {
                                        pEmployee.BasicPay = 0;
                                        pEmployee.Increment = 0;
                                        pEmployee.Transport = 0;
                                        pEmployee.Mobile = 0;
                                        pEmployee.Medical = 0;
                                        pEmployee.CashAllowance = 0;
                                        pEmployee.Commission = 0;
                                        pEmployee.Food = 0;
                                        pEmployee.Night = 0;
                                        pEmployee.Rent = 0;
                                        pEmployee.GroupAllowance = 0;
                                    }
                                }
                                else
                                {
                                    pEmployee.BasicPay = 0;
                                    pEmployee.Increment = 0;
                                    pEmployee.Transport = 0;
                                    pEmployee.Mobile = 0;
                                    pEmployee.Medical = 0;
                                    pEmployee.CashAllowance = 0;
                                    pEmployee.Commission = 0;
                                    pEmployee.Food = 0;
                                    pEmployee.Night = 0;
                                    pEmployee.Rent = 0;
                                    pEmployee.GroupAllowance = 0;
                                }

                                pEmployee.GrossSalary = sumAmount;
                                pEmployee.NetSalary = sumAmount;

                                pEmployee.PaymentModeId = 1;
                                pEmployee.BankNameId = 1;
                                pEmployee.AckStatusId = 1;
                                pEmployee.PayStatusId = 1;
                                pEmployee.IsFirstMonthText = "YES";
                                pEmployee.SalaryMonthYear = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));
                                pEmployee.PaymentDateTime = Convert.ToDateTime(DateTime.Now.AddDays(5).AddMonths(1).ToString("yyyy-MM-dd 00:00:00.000"));
                                pEmployee.CreateDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));
                                pEmployee.UpdateDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt"));

                                db.payroll.Add(pEmployee);
                                db.SaveChanges();
                            }
                            //////////////////////////////////////////////////////////////////////////////////////

                            //IR - User Session - Leave or Salary

                            if (toe.TypeOfEmploymentId != null && toe.TypeOfEmploymentId == 1)//1-permanent
                            {
                                ManageLeaveSessionForUsers(emp.employee_code, sLeaves, cLeaves, aLeaves, oLeaves, iLeaves01, iLeaves02, iLeaves03, iLeaves04, iLeaves05, iLeaves06, iLeaves07, iLeaves08, iLeaves09, iLeaves10, iLeaves11);
                            }
                            else if (toe.TypeOfEmploymentId != null && toe.TypeOfEmploymentId == 2)//2-probation
                            {
                                ManageLeaveSessionForUsers(emp.employee_code, sLeaves, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                            }
                            else//3-contract - all 0s
                            {
                                ManageLeaveSessionForUsers(emp.employee_code, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                            }
                            ////ManageLeaveSessionForUsers(emp.EmployeeId, sLeaves, cLeaves, aLeaves);
                        }
                        //return "Successfull";
                        //}
                        //catch (Exception)
                        //{
                        //    return "Failed";
                        //}
                    }

                    return "Successfull";
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        public static void ManageLeaveSessionForUsers(string employee_code, int sick_leaves, int casual_leaves, int annual_leaves, int other_leaves, int leave_type01, int leave_type02, int leave_type03, int leave_type04, int leave_type05, int leave_type06, int leave_type07, int leave_type08, int leave_type09, int leave_type10, int leave_type11) //actually Staff
        {
            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.employee_code == employee_code).FirstOrDefault();//e.active && 
                if (data_user != null)
                {
                    var data_leave_session = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();// l.SessionEndDate >= DateTime.Now
                    if (data_leave_session == null)
                    {
                        DateTime dtStart = new DateTime(); DateTime dtEnd = new DateTime();

                        if (data_user.date_of_joining != null)
                        {
                            /*///DateTime dtJoiningDate = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                            DateTime dtJoiningDate = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            if (dtJoiningDate == new DateTime(DateTime.Now.Year, 6, 30) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 1) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 2))
                            {
                                dtStart = new DateTime(DateTime.Now.Year, 7, 1);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }
                            else if (dtJoiningDate < new DateTime(DateTime.Now.Year, 6, 30)) //like month less than June month - Feb 
                            {
                                dtStart = new DateTime((DateTime.Now.Year - 1), data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            }
                            else //if (data_user.date_of_joining > new DateTime(DateTime.Now.Year, 6, 30)) //like month greater than June month - Aug 
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }
                            */

                            int stMonth = 0, stDay = 0, enMonth = 0, enDay = 0;
                            stMonth = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionStartMonth"].ToString()); stDay = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionStartDay"].ToString());
                            enMonth = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionEndMonth"].ToString()); enDay = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionEndDay"].ToString());

                            ////DateTime dtJoiningDate = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                            DateTime dtJoiningDate = new DateTime(data_user.date_of_joining.Value.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);

                            if (dtJoiningDate.Year == DateTime.Now.Year)
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, enMonth, enDay);
                            }
                            else if (dtJoiningDate.Year < DateTime.Now.Year)
                            {
                                dtStart = new DateTime(DateTime.Now.Year - 1, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, enMonth, enDay);
                            }
                            else
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, enMonth, enDay);

                            }
                            DLL.Models.LeaveSession lsession = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtEnd.Year,
                                SessionStartDate = dtStart,
                                SessionEndDate = dtEnd,
                                SickLeaves = sick_leaves,
                                CasualLeaves = casual_leaves,
                                AnnualLeaves = annual_leaves,
                                OtherLeaves = other_leaves,
                                LeaveType01 = leave_type01,
                                LeaveType02 = leave_type02,
                                LeaveType03 = leave_type03,
                                LeaveType04 = leave_type04,
                                LeaveType05 = leave_type05,
                                LeaveType06 = leave_type06,
                                LeaveType07 = leave_type07,
                                LeaveType08 = leave_type08,
                                LeaveType09 = leave_type09,
                                LeaveType10 = leave_type10,
                                LeaveType11 = leave_type11
                            };

                            db.leave_session.Add(lsession);
                            db.SaveChanges();
                        }
                    }

                    //when new session started and manage carry forward leaves as well
                    DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000");
                    var data_leave_session_next = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId && l.SessionEndDate >= dtToday).OrderByDescending(o => o.id).FirstOrDefault();
                    if (data_leave_session_next == null)
                    {
                        //int sick_leaves = data_user.sick_leaves;
                        //int casual_leaves = data_user.casual_leaves;
                        //int annual_leaves = data_user.annual_leaves;

                        ///////////////// Carry Forward - Last Session remaining Leaves //////////////////////

                        DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

                        int[] leaves = new int[30] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        int last_sick_leaves = 0, last_casual_leaves = 0, last_annual_leaves = 0, last_other_leaves = 0;
                        int last_leave_type01 = 0, last_leave_type02 = 0, last_leave_type03 = 0, last_leave_type04 = 0, last_leave_type05 = 0, last_leave_type06 = 0, last_leave_type07 = 0, last_leave_type08 = 0, last_leave_type09 = 0, last_leave_type10 = 0, last_leave_type11 = 0;
                        int iAvailableSickLeaves = 0, iAvailableCasualLeaves = 0, iAvailableAnnualLeaves = 0, iAvailableOtherLeaves = 0;
                        int iAvailableLeaveType01 = 0, iAvailableLeaveType02 = 0, iAvailableLeaveType03 = 0, iAvailableLeaveType04 = 0, iAvailableLeaveType05 = 0, iAvailableLeaveType06 = 0, iAvailableLeaveType07 = 0, iAvailableLeaveType08 = 0, iAvailableLeaveType09 = 0, iAvailableLeaveType10 = 0, iAvailableLeaveType11 = 0;
                        int iAvailedSickLeaves = 0, iAvailedCasualLeaves = 0, iAvailedAnnualLeaves = 0, iAvailedOtherLeaves = 0;
                        int iAvailedLeaveType01 = 0, iAvailedLeaveType02 = 0, iAvailedLeaveType03 = 0, iAvailedLeaveType04 = 0, iAvailedLeaveType05 = 0, iAvailedLeaveType06 = 0, iAvailedLeaveType07 = 0, iAvailedLeaveType08 = 0, iAvailedLeaveType09 = 0, iAvailedLeaveType10 = 0, iAvailedLeaveType11 = 0;

                        leaves = ViewModels.LeaveApplicationResultSet.getUserLeavesByUserCode(data_user.employee_code);
                        iAvailableSickLeaves = leaves[0];
                        iAvailableCasualLeaves = leaves[1];
                        iAvailableAnnualLeaves = leaves[2];
                        iAvailableOtherLeaves = leaves[6];
                        iAvailableLeaveType01 = leaves[8];
                        iAvailableLeaveType02 = leaves[9];
                        iAvailableLeaveType03 = leaves[10];
                        iAvailableLeaveType04 = leaves[11];
                        iAvailableLeaveType05 = 0; // leaves[8];
                        iAvailableLeaveType06 = 0; // leaves[9];
                        iAvailableLeaveType07 = 0; //leaves[10];
                        iAvailableLeaveType08 = 0; //leaves[11];
                        iAvailableLeaveType09 = 0; // leaves[8];
                        iAvailableLeaveType10 = 0; // leaves[9];
                        iAvailableLeaveType11 = 0; //leaves[10];

                        iAvailedSickLeaves = leaves[3];
                        iAvailedCasualLeaves = leaves[4];
                        iAvailedAnnualLeaves = leaves[5];
                        iAvailedOtherLeaves = leaves[7];
                        iAvailedLeaveType01 = leaves[12];
                        iAvailedLeaveType02 = leaves[13];
                        iAvailedLeaveType03 = leaves[14];
                        iAvailedLeaveType04 = leaves[15];
                        iAvailedLeaveType05 = 0; //leaves[12];
                        iAvailedLeaveType06 = 0; //leaves[13];
                        iAvailedLeaveType07 = 0; //leaves[14];
                        iAvailedLeaveType08 = 0; //leaves[15];
                        iAvailedLeaveType09 = 0; //leaves[12];
                        iAvailedLeaveType10 = 0; //leaves[13];
                        iAvailedLeaveType11 = 0; //leaves[14];

                        last_sick_leaves = iAvailableSickLeaves - iAvailedSickLeaves;
                        last_casual_leaves = iAvailableCasualLeaves - iAvailedCasualLeaves;
                        last_annual_leaves = iAvailableAnnualLeaves - iAvailedAnnualLeaves;
                        last_other_leaves = iAvailableOtherLeaves - iAvailedOtherLeaves;

                        last_leave_type01 = iAvailableLeaveType01 - iAvailedLeaveType01;
                        last_leave_type02 = iAvailableLeaveType02 - iAvailedLeaveType02;
                        last_leave_type03 = iAvailableLeaveType03 - iAvailedLeaveType03;
                        last_leave_type04 = iAvailableLeaveType04 - iAvailedLeaveType04;
                        last_leave_type05 = iAvailableLeaveType05 - iAvailedLeaveType05;
                        last_leave_type06 = iAvailableLeaveType06 - iAvailedLeaveType06;
                        last_leave_type07 = iAvailableLeaveType07 - iAvailedLeaveType07;
                        last_leave_type08 = iAvailableLeaveType08 - iAvailedLeaveType08;
                        last_leave_type09 = iAvailableLeaveType09 - iAvailedLeaveType09;
                        last_leave_type10 = iAvailableLeaveType10 - iAvailedLeaveType10;
                        last_leave_type11 = iAvailableLeaveType11 - iAvailedLeaveType11;

                        if (last_sick_leaves > 0)
                            sick_leaves = sick_leaves + int.Parse(last_sick_leaves.ToString());

                        if (last_casual_leaves > 0)
                            casual_leaves = casual_leaves + int.Parse(last_casual_leaves.ToString());

                        if (last_annual_leaves > 0)
                            annual_leaves = annual_leaves + int.Parse(last_annual_leaves.ToString());

                        if (last_other_leaves > 0)
                            other_leaves = other_leaves + int.Parse(last_other_leaves.ToString());

                        if (last_leave_type01 > 0)
                            leave_type01 = leave_type01 + int.Parse(last_leave_type01.ToString());

                        if (last_leave_type02 > 0)
                            leave_type02 = leave_type02 + int.Parse(last_leave_type02.ToString());

                        if (last_leave_type03 > 0)
                            leave_type03 = leave_type03 + int.Parse(last_leave_type03.ToString());

                        if (last_leave_type04 > 0)
                            leave_type04 = leave_type04 + int.Parse(last_leave_type04.ToString());

                        if (last_leave_type05 > 0)
                            leave_type05 = leave_type05 + int.Parse(last_leave_type05.ToString());

                        if (last_leave_type06 > 0)
                            leave_type06 = leave_type06 + int.Parse(last_leave_type06.ToString());

                        if (last_leave_type07 > 0)
                            leave_type07 = leave_type07 + int.Parse(last_leave_type07.ToString());

                        if (last_leave_type08 > 0)
                            leave_type08 = leave_type08 + int.Parse(last_leave_type08.ToString());

                        if (last_leave_type09 > 0)
                            leave_type09 = leave_type09 + int.Parse(last_leave_type09.ToString());

                        if (last_leave_type10 > 0)
                            leave_type10 = leave_type10 + int.Parse(last_leave_type10.ToString());

                        if (last_leave_type11 > 0)
                            leave_type11 = leave_type11 + int.Parse(last_leave_type11.ToString());

                        //////////////////////////////////////////////////////////////////////////////////////

                        if (data_leave_session != null)
                        {
                            DateTime dtSessionEndDate = data_leave_session.SessionEndDate == null ? DateTime.Now : data_leave_session.SessionEndDate;

                            DLL.Models.LeaveSession lsession_next = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(-1).AddYears(1).Year : DateTime.Now.AddDays(-1).AddYears(1).Year,
                                SessionStartDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(1) : DateTime.Now.AddDays(1),
                                SessionEndDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddYears(1) : DateTime.Now.AddYears(1),
                                SickLeaves = sick_leaves,
                                CasualLeaves = casual_leaves,
                                AnnualLeaves = annual_leaves,
                                OtherLeaves = other_leaves,
                                LeaveType01 = leave_type01,
                                LeaveType02 = leave_type02,
                                LeaveType03 = leave_type03,
                                LeaveType04 = leave_type04,
                                LeaveType05 = leave_type05,
                                LeaveType06 = leave_type06,
                                LeaveType07 = leave_type07,
                                LeaveType08 = leave_type08,
                                LeaveType09 = leave_type09,
                                LeaveType10 = leave_type10,
                                LeaveType11 = leave_type11
                            };

                            db.leave_session.Add(lsession_next);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }


        public static void ManageLeaveSessionForUsers_BK2(string User_Code, int sick_leaves, int casual_leaves, int annual_leaves)//, int other_leaves) //actually Staff
        {
            string gvStartDay = "01", gvStartMonth = "01", gvEndDay = "31", gvEndMonth = "12";

            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.employee_code == User_Code).FirstOrDefault();
                if (data_user != null)
                {
                    var data_leave_session = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();// l.SessionEndDate >= DateTime.Now
                    if (data_leave_session == null)
                    {
                        DateTime dtStart = new DateTime(); DateTime dtEnd = new DateTime();

                        if (data_user.date_of_joining != null)
                        {
                            /*//DateTime dtJoiningDate = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                            DateTime dtJoiningDate = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            if (dtJoiningDate == new DateTime(DateTime.Now.Year, 6, 30) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 1) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 2))
                            {
                                dtStart = new DateTime(DateTime.Now.Year, 7, 1);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }
                            else if (dtJoiningDate < new DateTime(DateTime.Now.Year, 6, 30)) //like month less than June month - Feb 
                            {
                                dtStart = new DateTime((DateTime.Now.Year - 1), data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            }
                            else //if (data_user.date_of_joining > new DateTime(DateTime.Now.Year, 6, 30)) //like month greater than June month - Aug 
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }
                            */

                            if (System.Web.HttpContext.Current.Session["GV_SessionStartDay"] != null && System.Web.HttpContext.Current.Session["GV_SessionStartDay"].ToString() != "")
                                gvStartDay = System.Web.HttpContext.Current.Session["GV_SessionStartDay"].ToString();
                            else
                                gvStartDay = "01";

                            if (System.Web.HttpContext.Current.Session["GV_SessionStartMonth"] != null && System.Web.HttpContext.Current.Session["GV_SessionStartMonth"].ToString() != "")
                                gvStartMonth = System.Web.HttpContext.Current.Session["GV_SessionStartMonth"].ToString();
                            else
                                gvStartMonth = "01";

                            if (System.Web.HttpContext.Current.Session["GV_SessionEndDay"] != null && System.Web.HttpContext.Current.Session["GV_SessionEndDay"].ToString() != "")
                                gvEndDay = System.Web.HttpContext.Current.Session["GV_SessionEndDay"].ToString();
                            else
                                gvEndDay = "31";

                            if (System.Web.HttpContext.Current.Session["GV_SessionEndMonth"] != null && System.Web.HttpContext.Current.Session["GV_SessionEndMonth"].ToString() != "")
                                gvEndMonth = System.Web.HttpContext.Current.Session["GV_SessionEndMonth"].ToString();
                            else
                                gvEndMonth = "12";


                            ////DateTime dtJoiningDate = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                            DateTime dtJoiningDate = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            if (dtJoiningDate == new DateTime(DateTime.Now.Year, int.Parse(gvEndMonth), int.Parse(gvEndDay)) || dtJoiningDate == new DateTime(DateTime.Now.Year, int.Parse(gvStartMonth), int.Parse(gvStartDay)) || dtJoiningDate == new DateTime(DateTime.Now.Year, int.Parse(gvStartMonth), 2))
                            {
                                dtStart = new DateTime(DateTime.Now.Year, int.Parse(gvStartMonth), int.Parse(gvStartDay));
                                dtEnd = new DateTime((DateTime.Now.Year + 1), int.Parse(gvEndMonth), int.Parse(gvEndDay));
                            }
                            else if (dtJoiningDate < new DateTime(DateTime.Now.Year, int.Parse(gvEndMonth), int.Parse(gvEndDay))) //like month less than June month - Feb 
                            {
                                dtStart = new DateTime((DateTime.Now.Year - 1), data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, int.Parse(gvEndMonth), int.Parse(gvEndDay));
                            }
                            else //if (data_user.date_of_joining > new DateTime(DateTime.Now.Year, int.Parse(gvEndMonth), int.Parse(gvEndDay))) //like month greater than June month - Aug 
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), int.Parse(gvEndMonth), int.Parse(gvEndDay));
                            }

                            DLL.Models.LeaveSession lsession = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtEnd.Year,
                                SessionStartDate = dtStart,
                                SessionEndDate = dtEnd,
                                SickLeaves = data_user.sick_leaves,
                                CasualLeaves = data_user.casual_leaves,
                                AnnualLeaves = data_user.annual_leaves
                                //,OtherLeaves = data_user.other_leaves
                            };

                            db.leave_session.Add(lsession);
                            db.SaveChanges();


                        }
                    }

                    //when new session started and manage carry forward leaves as well
                    DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000");
                    var data_leave_session_next = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId && l.SessionEndDate >= dtToday).OrderByDescending(o => o.id).FirstOrDefault();
                    if (data_leave_session_next == null)
                    {
                        //int sick_leaves = data_user.sick_leaves;
                        //int casual_leaves = data_user.casual_leaves;
                        //int annual_leaves = data_user.annual_leaves;

                        ///////////////// Carry Forward - Last Session remaining Leaves //////////////////////

                        DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

                        int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                        int last_sick_leaves = 0, last_casual_leaves = 0, last_annual_leaves = 0, last_other_leaves = 0;
                        int iAvailableSickLeaves = 0, iAvailableCasualLeaves = 0, iAvailableAnnualLeaves = 0, iAvailableOtherLeaves = 0;
                        int iAvailedSickLeaves = 0, iAvailedCasualLeaves = 0, iAvailedAnnualLeaves = 0, iAvailedOtherLeaves = 0;

                        leaves = ViewModels.LeaveApplicationResultSet.getUserLeavesByUserCode(User_Code);
                        iAvailableSickLeaves = leaves[0];
                        iAvailableCasualLeaves = leaves[1];
                        iAvailableAnnualLeaves = leaves[2];
                        //iAvailableOtherLeaves = decimal.Parse(leaves[6]);

                        iAvailedSickLeaves = leaves[3];
                        iAvailedCasualLeaves = leaves[4];
                        iAvailedAnnualLeaves = leaves[5];
                        //iAvailedOtherLeaves = decimal.Parse(leaves[7];

                        last_sick_leaves = iAvailableSickLeaves - iAvailedSickLeaves;
                        last_casual_leaves = iAvailableCasualLeaves - iAvailedCasualLeaves;
                        last_annual_leaves = iAvailableAnnualLeaves - iAvailedAnnualLeaves;
                        //last_other_leaves = iAvailableOtherLeaves - iAvailedOtherLeaves;

                        if (last_sick_leaves > 0)
                            sick_leaves = sick_leaves + int.Parse(last_sick_leaves.ToString());

                        if (last_casual_leaves > 0)
                            casual_leaves = casual_leaves + int.Parse(last_casual_leaves.ToString());

                        if (last_annual_leaves > 0)
                            annual_leaves = annual_leaves + int.Parse(last_annual_leaves.ToString());

                        //if (last_other_leaves > 0)
                        //     other_leaves = other_leaves + int.Parse(last_other_leaves.ToString());

                        //////////////////////////////////////////////////////////////////////////////////////

                        if (data_leave_session != null)
                        {
                            DateTime dtSessionEndDate = data_leave_session.SessionEndDate == null ? DateTime.Now : data_leave_session.SessionEndDate;

                            DLL.Models.LeaveSession lsession_next = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(-1).AddYears(1).Year : DateTime.Now.AddDays(-1).AddYears(1).Year,
                                SessionStartDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(1) : DateTime.Now.AddDays(1),
                                SessionEndDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddYears(1) : DateTime.Now.AddYears(1),
                                SickLeaves = sick_leaves,
                                CasualLeaves = casual_leaves,
                                AnnualLeaves = annual_leaves
                                //,OtherLeaves = other_leaves
                            };

                            db.leave_session.Add(lsession_next);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public static void ManageLeaveSessionForUsers_BK(int employee_id, int sick_leaves, int casual_leaves, int annual_leaves) //actually Staff
        {
            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.EmployeeId == employee_id).FirstOrDefault();
                if (data_user != null)
                {
                    var data_leave_session = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();// l.SessionEndDate >= DateTime.Now
                    if (data_leave_session == null)
                    {
                        DateTime dtStart = new DateTime(); DateTime dtEnd = new DateTime();

                        if (data_user.date_of_joining != null)
                        {
                            ////DateTime dtJoiningDate = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                            DateTime dtJoiningDate = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            if (dtJoiningDate == new DateTime(DateTime.Now.Year, 6, 30) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 1) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 2))
                            {
                                dtStart = new DateTime(DateTime.Now.Year, 7, 1);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }
                            else if (dtJoiningDate < new DateTime(DateTime.Now.Year, 6, 30)) //like month less than June month - Feb 
                            {
                                dtStart = new DateTime((DateTime.Now.Year - 1), data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            }
                            else //if (data_user.date_of_joining > new DateTime(DateTime.Now.Year, 6, 30)) //like month greater than June month - Aug 
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }

                            DLL.Models.LeaveSession lsession = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtEnd.Year,
                                SessionStartDate = dtStart,
                                SessionEndDate = dtEnd,
                                SickLeaves = sick_leaves,
                                CasualLeaves = casual_leaves,
                                AnnualLeaves = annual_leaves
                            };

                            db.leave_session.Add(lsession);
                            db.SaveChanges();
                        }
                    }

                    //when new session started and manage carry forward leaves as well
                    DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000");
                    var data_leave_session_next = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId && l.SessionEndDate >= dtToday).OrderByDescending(o => o.id).FirstOrDefault();
                    if (data_leave_session_next == null)
                    {
                        //int sick_leaves = data_user.sick_leaves;
                        //int casual_leaves = data_user.casual_leaves;
                        //int annual_leaves = data_user.annual_leaves;

                        ///////////////// Carry Forward - Last Session remaining Leaves //////////////////////

                        DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

                        int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                        int last_sick_leaves = 0, last_casual_leaves = 0, last_annual_leaves = 0;
                        int iAvailableSickLeaves = 0, iAvailableCasualLeaves = 0, iAvailableAnnualLeaves = 0;
                        int iAvailedSickLeaves = 0, iAvailedCasualLeaves = 0, iAvailedAnnualLeaves = 0;

                        leaves = ViewModels.LeaveApplicationResultSet.getUserLeavesByUserCode(data_user.employee_code);
                        iAvailableSickLeaves = leaves[0];
                        iAvailableCasualLeaves = leaves[1];
                        iAvailableAnnualLeaves = leaves[2];
                        iAvailedSickLeaves = leaves[3];
                        iAvailedCasualLeaves = leaves[4];
                        iAvailedAnnualLeaves = leaves[5];

                        last_sick_leaves = iAvailableSickLeaves - iAvailedSickLeaves;
                        last_casual_leaves = iAvailableCasualLeaves - iAvailedCasualLeaves;
                        last_annual_leaves = iAvailableAnnualLeaves - iAvailedAnnualLeaves;

                        if (last_sick_leaves > 0)
                            sick_leaves = sick_leaves + last_sick_leaves;

                        if (last_casual_leaves > 0)
                            casual_leaves = casual_leaves + last_casual_leaves;

                        if (last_annual_leaves > 0)
                            annual_leaves = annual_leaves + last_annual_leaves;

                        //////////////////////////////////////////////////////////////////////////////////////

                        if (data_leave_session != null)
                        {
                            DateTime dtSessionEndDate = data_leave_session.SessionEndDate == null ? DateTime.Now : data_leave_session.SessionEndDate;

                            DLL.Models.LeaveSession lsession_next = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(-1).AddYears(1).Year : DateTime.Now.AddDays(-1).AddYears(1).Year,
                                SessionStartDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(1) : DateTime.Now.AddDays(1),
                                SessionEndDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddYears(1) : DateTime.Now.AddYears(1),
                                SickLeaves = sick_leaves,
                                CasualLeaves = casual_leaves,
                                AnnualLeaves = annual_leaves
                            };

                            db.leave_session.Add(lsession_next);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public static void ManageLeaveSessionForUsers_BACKUP(int employee_id, int sick_leaves, int casual_leaves, int annual_leaves) //actually Staff
        {
            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.EmployeeId == employee_id).FirstOrDefault();
                if (data_user != null)
                {
                    var data_leave_session = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();// l.SessionEndDate >= DateTime.Now
                    if (data_leave_session == null)
                    {
                        DateTime dtStart = new DateTime(); DateTime dtEnd = new DateTime();

                        if (data_user.date_of_joining != null)
                        {
                            data_user.date_of_joining = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);

                            if (data_user.date_of_joining == new DateTime(DateTime.Now.Year, 6, 30) || data_user.date_of_joining == new DateTime(DateTime.Now.Year, 7, 1) || data_user.date_of_joining == new DateTime(DateTime.Now.Year, 7, 2))
                            {
                                dtStart = new DateTime(DateTime.Now.Year, 7, 1);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }
                            else if (data_user.date_of_joining < new DateTime(DateTime.Now.Year, 6, 30)) //like month less than June month - Feb 
                            {
                                dtStart = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            }
                            else if (data_user.date_of_joining > new DateTime(DateTime.Now.Year, 6, 30)) //like month greater than June month - Aug 
                            {
                                dtStart = Convert.ToDateTime((DateTime.Now.Year - 1) + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            }

                            DLL.Models.LeaveSession lsession = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtEnd.Year,
                                SessionStartDate = dtStart,
                                SessionEndDate = dtEnd,
                                SickLeaves = sick_leaves,
                                CasualLeaves = casual_leaves,
                                AnnualLeaves = annual_leaves
                            };

                            db.leave_session.Add(lsession);
                            db.SaveChanges();
                        }
                    }

                    //when new session started and manage carry forward leaves as well
                    DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000");
                    var data_leave_session_next = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId && l.SessionEndDate >= dtToday).OrderByDescending(o => o.id).FirstOrDefault();
                    if (data_leave_session_next == null)
                    {
                        //int sick_leaves = data_user.sick_leaves;
                        //int casual_leaves = data_user.casual_leaves;
                        //int annual_leaves = data_user.annual_leaves;

                        ///////////////// Carry Forward - Last Session remaining Leaves //////////////////////

                        DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

                        int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                        int last_sick_leaves = 0, last_casual_leaves = 0, last_annual_leaves = 0;
                        int iAvailableSickLeaves = 0, iAvailableCasualLeaves = 0, iAvailableAnnualLeaves = 0;
                        int iAvailedSickLeaves = 0, iAvailedCasualLeaves = 0, iAvailedAnnualLeaves = 0;

                        leaves = ViewModels.LeaveApplicationResultSet.getUserLeavesByUserCode(data_user.employee_code);
                        iAvailableSickLeaves = leaves[0];
                        iAvailableCasualLeaves = leaves[1];
                        iAvailableAnnualLeaves = leaves[2];
                        iAvailedSickLeaves = leaves[3];
                        iAvailedCasualLeaves = leaves[4];
                        iAvailedAnnualLeaves = leaves[5];

                        last_sick_leaves = iAvailableSickLeaves - iAvailedSickLeaves;
                        last_casual_leaves = iAvailableCasualLeaves - iAvailedCasualLeaves;
                        last_annual_leaves = iAvailableAnnualLeaves - iAvailedAnnualLeaves;

                        if (last_sick_leaves > 0)
                            sick_leaves = sick_leaves + last_sick_leaves;

                        if (last_casual_leaves > 0)
                            casual_leaves = casual_leaves + last_casual_leaves;

                        if (last_annual_leaves > 0)
                            annual_leaves = annual_leaves + last_annual_leaves;

                        //////////////////////////////////////////////////////////////////////////////////////

                        DLL.Models.LeaveSession lsession_next = new DLL.Models.LeaveSession()
                        {
                            EmployeeId = data_user.EmployeeId,
                            YearId = data_leave_session.SessionEndDate != null ? Convert.ToDateTime(data_leave_session.SessionEndDate).AddDays(-1).AddYears(1).Year : DateTime.Now.AddDays(-1).AddYears(1).Year,
                            SessionStartDate = data_leave_session.SessionEndDate != null ? Convert.ToDateTime(data_leave_session.SessionEndDate).AddDays(1) : DateTime.Now.AddDays(1),
                            SessionEndDate = data_leave_session.SessionEndDate != null ? Convert.ToDateTime(data_leave_session.SessionEndDate).AddYears(1) : DateTime.Now.AddYears(1),
                            SickLeaves = sick_leaves,
                            CasualLeaves = casual_leaves,
                            AnnualLeaves = annual_leaves
                        };

                        db.leave_session.Add(lsession_next);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static bool validateCampusCode(string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strCampusCode = strCampusCode.ToLower();

                    var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                    if (dbCampus != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public static bool validateCampusCodeAllowed(int iCampusID, string strCampusCode)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    if (iCampusID == -1)
                    {
                        isValid = true;
                    }
                    else
                    {
                        strCampusCode = strCampusCode.ToLower();

                        var dbCampus = db.organization_campus.Where(c => c.CampusCode == strCampusCode).FirstOrDefault();
                        var dbEmployee = db.employee.Where(c => c.campus_id == iCampusID).FirstOrDefault();

                        if (dbCampus != null && dbEmployee != null)
                        {
                            if (dbCampus.Id == dbEmployee.campus_id)
                                isValid = true;
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }


        public static bool validateShiftName(string strShiftName)
        {
            bool isValid = false;

            using (var db = new Context())
            {
                try
                {
                    strShiftName = strShiftName.ToLower();

                    var dbShift = db.shift.Where(s => s.name.ToLower() == strShiftName).FirstOrDefault();
                    if (dbShift != null)
                    {
                        isValid = true;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        #region Contractual Staff
        public static string setContractualStaff(List<string> csvContents)
        {
            string toReturn = "";
            bool check = false;
            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 17 values (columns).

                        if (values == null || values.Length < 15)
                        {
                            toReturn = "Unable to upload file ,Pease check file format or contact HR";
                            continue;
                        }


                        // Remove unwanted " characters.
                        string employeeCode = values[0].Replace("\"", "");
                        values[1] = values[1].Replace("\"", "");
                        values[2] = values[2].Replace("\"", "");
                        values[3] = values[3].Replace("\"", "");
                        values[4] = values[4].Replace("\"", "");
                        values[5] = values[5].Replace("\"", "");
                        values[6] = values[6].Replace("\"", "");
                        values[7] = values[7].Replace("\"", "");
                        values[8] = values[8].Replace("\"", "");
                        values[9] = values[9].Replace("\"", "");
                        values[10] = values[10].Replace("\"", "");
                        values[11] = values[11].Replace("\"", "");
                        values[12] = values[12].Replace("\"", "");
                        values[13] = values[13].Replace("\"", "");
                        values[14] = values[14].Replace("\"", "");

                        // continue if its the first row (CSV Header line).
                        if (employeeCode == "employeeCode" || employeeCode == "" || values[5] == "")
                        {
                            continue;
                        }
                        // Get the employee 
                        ContractualStaff employee = db.contractual_staff.Where(m =>
                            m.employee_code.Equals(employeeCode)).FirstOrDefault();

                        //try
                        //{
                        // If the line manager and the employee both exist.
                        if (employee != null)
                        {
                            //for (int i = 0; i < 18; i++)
                            //{
                            //Update Employee
                            employee.employee_name = values[1];
                            employee.email = values[2];
                            employee.address = values[3];
                            employee.mobile_no = values[4];
                            employee.date_of_joining = (values[5] == null || values[5].Equals("")) ? null : (DateTime?)DateTime.Parse(values[5], System.Globalization.CultureInfo.InvariantCulture);
                            employee.date_of_leaving = (values[6] == null || values[6] == "") ? null : (DateTime?)DateTime.Parse(values[6], System.Globalization.CultureInfo.InvariantCulture);

                            employee.company = values[7];
                            employee.function = values[10];
                            employee.designation = values[9];
                            employee.department = values[8];
                            employee.active = convertStringToBool(values[14]);

                            employee.grade = values[11];
                            //employee.Group = (int.Parse(values[13]) == 0) ? null : db.group.Find(int.Parse(values[13]));
                            employee.region = values[13];
                            employee.location = values[12];
                            //}
                            if (db.SaveChanges() > 0)
                            {
                                toReturn = "Successful";
                            }
                            else
                            {
                                toReturn = "Unable to upload file ,Please check file format or contact HR";
                            }
                            //return "Successfull";
                        }

                        else
                        {


                            ContractualStaff emp = new ContractualStaff();
                            emp.employee_name = values[1];
                            emp.employee_code = employeeCode;
                            emp.email = values[2];
                            emp.address = values[3];
                            emp.mobile_no = values[4];
                            emp.date_of_joining = (values[5] == null || values[5] == "") ? null : (DateTime?)DateTime.Parse(values[5], System.Globalization.CultureInfo.InvariantCulture);
                            emp.date_of_leaving = (values[6] == null || values[6] == "") ? null : (DateTime?)DateTime.Parse(values[6], System.Globalization.CultureInfo.InvariantCulture);
                            emp.company = values[7];
                            emp.function = values[10];
                            emp.designation = values[9];
                            emp.department = values[8];
                            emp.grade = values[11];
                            emp.region = values[13];
                            emp.location = values[12];
                            emp.active = convertStringToBool(values[14]);

                            db.contractual_staff.Add(emp);
                            CS_AttendanceLog pl = new CS_AttendanceLog()
                            {
                                ContractualStaff = emp,
                                active = true,
                                employee_code = emp.employee_code
                            };
                            db.cs_persistent_log.Add(pl);
                            if (db.SaveChanges() > 0)
                            {
                                toReturn = "Successful";
                            }
                            else
                            {
                                return "Unable to upload file ,Pease check file format or contact HR";
                            }

                        }
                        //return "Successfull";
                        //}
                        //catch (Exception)
                        //{
                        //    return "Failed";
                        //}

                    }
                    return toReturn;
                }


            }

            catch (Exception)
            {
                return "File unable be uploaded, Please check the file format and contact HR";
            }

        }
        public static List<ManageContractualStaffCSV> getContractualStaff()
        {
            List<ManageContractualStaffCSV> toReturn;
            using (var db = new Context())
            {

                try
                {


                    toReturn =
                        db.contractual_staff.Where(m => m.active).Select(
                        p => new ManageContractualStaffCSV()
                        {
                            employeeCode = p.employee_code,
                            employeeName = (p.employee_name == null) ? "" : p.employee_name,
                            email = (p.email == null) ? "" : p.email,
                            address = (p.address == null) ? "" : p.address,
                            mobileNo = (p.mobile_no == null) ? "" : p.mobile_no,
                            dateOfJoining = p.date_of_joining.ToString(),
                            dateOfLeaving = p.date_of_leaving.ToString(),
                            //accessGroupID=p.access_group_AccessGroupId,
                            company = (p.company == null) ? "" : p.company,
                            department = (p.department == null) ? "" : p.department,
                            designation = (p.designation == null) ? "" : p.designation,
                            function = (p.function == null) ? "" : p.function,
                            grade = (p.grade == null) ? "" : p.grade,
                            //groupID=p.Group_GroupId,
                            location = (p.location == null) ? "" : p.location,
                            region = (p.region == null) ? "" : p.region,
                            active = p.active
                        }).ToList();


                    // convert dates to a more processable form.
                    foreach (var value in toReturn)
                    {
                        if (value.dateOfJoining != null && value.dateOfJoining != "")
                        {
                            DateTime dateOfJoining = DateTime.Parse(value.dateOfJoining);
                            value.dateOfJoining = dateOfJoining.ToString("yyyy-MM-dd");
                        }

                        if (value.dateOfLeaving != null && value.dateOfLeaving != "")
                        {
                            DateTime dateOfLeaving = DateTime.Parse(value.dateOfLeaving);
                            value.dateOfLeaving = dateOfLeaving.ToString("yyyy-MM-dd");
                        }

                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ManageContractualStaffCSV>();
                }

            }

            return toReturn;
        }
        #endregion
    }
}
