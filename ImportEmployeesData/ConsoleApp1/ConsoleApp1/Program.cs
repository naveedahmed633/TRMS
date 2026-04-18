using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace FahrEmployeeImport
{
    public class Employee
    {
        public string EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? CNICNo { get; set; }
        public int GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
        public int? GradeId { get; set; }
        public int? RegionId { get; set; }
        public bool Active { get; set; }
        public string? Description { get; set; }
        public bool ImportedEmployee { get; set; }
    }

    internal class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("🔹 Fetching employee data from FAHR web service...");

            string url = "https://esbdev.fahr.gov.ae/services/FAHRAttendanceservice";

            string soapRequest = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:fah=""http://esb.bayanati.gov.ae/services/FAHRAttendanceService/"">
   <soapenv:Header>
      <wsse:Security soapenv:mustUnderstand=""1""
         xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""
         xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
         <wsse:UsernameToken wsu:Id=""UsernameToken-4A14FC3D2847494704158727497458528"">
            <wsse:Username>testing.fahresb.MOEI@fahr.gov.ae</wsse:Username>
            <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">testing.fahresb.MOEI@fahr.gov.ae</wsse:Password>
         </wsse:UsernameToken>
      </wsse:Security>
   </soapenv:Header>
   <soapenv:Body>
      <fah:GetEmployeeDetailsRequest>
         <EAIHeader versionID=""1.0"">
            <ServiceId>44</ServiceId>
            <ExternalAuthorityCode>116</ExternalAuthorityCode>
            <TransactionRefNo>REQ1538</TransactionRefNo>
            <TransactionSubtype>I</TransactionSubtype>
            <Notes></Notes>
         </EAIHeader>
         <EAIBody>
            <EntityCode>02</EntityCode>
            <IdentificationKey>ad$121@b02</IdentificationKey>
            <RequestType>EMPLOYEE_INFO_ACTIVE</RequestType>
         </EAIBody>
      </fah:GetEmployeeDetailsRequest>
   </soapenv:Body>
</soapenv:Envelope>";

            try
            {
                using var handler = new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                };

                using var httpClient = new HttpClient(handler);

                // Match Postman headers exactly
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.49.0");
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                // If your Postman request includes an API key or Cookie header, uncomment & paste here:
                // httpClient.DefaultRequestHeaders.Add("api-key", "YOUR_API_KEY_HERE");
                // httpClient.DefaultRequestHeaders.Add("Cookie", "TS01dbaa04=01f3d7...");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(soapRequest, new UTF8Encoding(false), "text/xml");

                // Optional: log what’s being sent
                Console.WriteLine("\n--- REQUEST HEADERS ---");
                foreach (var header in httpClient.DefaultRequestHeaders)
                    Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
                Console.WriteLine("\n--- REQUEST BODY ---");
                Console.WriteLine(soapRequest);

                var response = await httpClient.SendAsync(request);
                string responseXml = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"\n--- RESPONSE STATUS ---\n{response.StatusCode}");
                Console.WriteLine("\n--- RESPONSE BODY ---");
                Console.WriteLine(responseXml);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ SOAP request failed: {response.StatusCode}");
                    return;
                }

                Console.WriteLine("✅ SOAP response received. Parsing data...");

                // Parse the XML
                XDocument doc = XDocument.Parse(responseXml);
                XNamespace ns3 = "http://xmlns.oracle.com/apps/hxt/soaprovider/plsql/xxfahr_time_attendance_ws_pkg/getemployeedetails/";
                var rows = doc.Descendants(ns3 + "Row").ToList();

                var employees = new List<Employee>();

                foreach (var row in rows)
                {
                    var cols = row.Elements(ns3 + "Column");
                    string GetValue(string name) =>
                        cols.FirstOrDefault(c => c.Attribute("name")?.Value == name)?.Value?.Trim();

                    var emp = new Employee
                    {
                        EmployeeCode = GetValue("EMPLOYEE_NUMBER"),
                        FirstName = GetValue("FIRST_NAME_EN"),
                        LastName = GetValue("LAST_NAME_EN"),
                        FatherName = GetValue("SECOND_NAME_EN"),
                        Email = GetValue("EMAIL_ADDRESS"),
                        MobileNo = GetValue("MOBILE_NUM"),
                        CNICNo = GetValue("NATIONAL_IDENTIFIER"),
                        GenderId = GetValue("GENDER_CODE")?.ToUpper() == "F" ? 2 : 1,
                        DateOfBirth = DateTime.TryParse(GetValue("DATE_OF_BIRTH"), out var dob) ? dob : null,
                        DateOfJoining = DateTime.TryParse(GetValue("JOIN_DATE"), out var doj) ? doj : null,
                        DateOfLeaving = DateTime.TryParse(GetValue("TERMINATION_DATE"), out var dol) ? dol : null,
                        DesignationId = int.TryParse(GetValue("DESIGNATION_ID"), out var des) ? des : null,
                        DepartmentId = int.TryParse(GetValue("ORGANIZATION_ID"), out var dept) ? dept : null,
                        GradeId = int.TryParse(GetValue("GRADE_ID"), out var grd) ? grd : null,
                        RegionId = int.TryParse(GetValue("WORK_LOCATION_ID"), out var reg) ? reg : null,
                        Active = GetValue("STATUS")?.ToUpper() == "A",
                        Description = GetValue("JOB_TITLE_EN"),
                        ImportedEmployee = true
                    };

                    employees.Add(emp);
                }

                Console.WriteLine($"📦 Total employees parsed: {employees.Count}");
                await SaveToDatabaseAsync(employees);
                Console.WriteLine("🎉 All employees imported successfully into SQL Server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex}");
            }
        }

        private static async Task<int?> EnsureDepartmentExistsAsync(SqlConnection conn, int? externalDeptId)
        {
            if (externalDeptId == null)
                return null;

            // Check if a department with this FAHR external ID already exists
            string checkQuery = "SELECT DepartmentId FROM Departments WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalDeptId.Value);
                var result = await checkCmd.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            // If not found — create it
            string insertQuery = @"
        INSERT INTO Departments (dbID, [name], [description], [active])
        OUTPUT INSERTED.DepartmentId
        VALUES (@dbID, @name, @desc, 1)";

            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalDeptId.Value);
                insertCmd.Parameters.AddWithValue("@name", $"FAHR Dept {externalDeptId}");
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");

                var newId = await insertCmd.ExecuteScalarAsync();
                return Convert.ToInt32(newId);
            }
        }

        private static async Task<int?> EnsureDesignationExistsAsync(SqlConnection conn, int? externalDesigId)
        {
            if (externalDesigId == null)
                return null;

            // Check if this FAHR designation already exists
            string checkQuery = "SELECT DesignationId FROM Designations WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalDesigId.Value);
                var result = await checkCmd.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            // If not found, create a new one
            string insertQuery = @"
        INSERT INTO Designations (dbID, [name], [description], [active])
        OUTPUT INSERTED.DesignationId
        VALUES (@dbID, @name, @desc, 1)";

            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalDesigId.Value);
                insertCmd.Parameters.AddWithValue("@name", $"FAHR Designation {externalDesigId}");
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");

                var newId = await insertCmd.ExecuteScalarAsync();
                return Convert.ToInt32(newId);
            }
        }


        private static async Task<int?> EnsureGradeExistsAsync(SqlConnection conn, int? externalGradeId)
        {
            if (externalGradeId == null)
                return null;

            // Check if grade already exists
            string checkQuery = "SELECT GradeId FROM Grades WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalGradeId.Value);
                var result = await checkCmd.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            // If not found, create it
            string insertQuery = @"
        INSERT INTO Grades (dbID, [name], [description], [active])
        OUTPUT INSERTED.GradeId
        VALUES (@dbID, @name, @desc, 1)";

            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalGradeId.Value);
                insertCmd.Parameters.AddWithValue("@name", $"FAHR Grade {externalGradeId}");
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");

                var newId = await insertCmd.ExecuteScalarAsync();
                return Convert.ToInt32(newId);
            }
        }
        private static async Task<int?> EnsureRegionExistsAsync(SqlConnection conn, int? externalRegionId)
        {
            if (externalRegionId == null)
                return null;

            // Check if region already exists
            string checkQuery = "SELECT RegionId FROM Regions WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalRegionId.Value);
                var result = await checkCmd.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            // If not found, create it
            string insertQuery = @"
        INSERT INTO Regions (dbID, [name], [description], [active])
        OUTPUT INSERTED.RegionId
        VALUES (@dbID, @name, @desc, 1)";

            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalRegionId.Value);
                insertCmd.Parameters.AddWithValue("@name", $"FAHR Region {externalRegionId}");
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");

                var newId = await insertCmd.ExecuteScalarAsync();
                return Convert.ToInt32(newId);
            }
        }


        private static async Task SaveToDatabaseAsync(List<Employee> employees)
        {
            string connString = "Data Source=.;Initial Catalog=TRMS;User Id=sa;Password=1234;Integrated Security=False;TrustServerCertificate=True;";

            using var conn = new SqlConnection(connString);
            await conn.OpenAsync();

            foreach (var emp in employees)
            {
                string query = @"
IF EXISTS (SELECT 1 FROM Employees WHERE employee_code = @employee_code)
    UPDATE Employees SET 
        first_name = @first_name,
        last_name = @last_name,
        father_name = @father_name,
        email = @email,
        mobile_no = @mobile_no,
        cnic_no = @cnic_no,
        gender_id = @gender_id,
        date_of_birth = @date_of_birth,
        date_of_joining = @date_of_joining,
        date_of_leaving = @date_of_leaving,
        designation_DesignationId = @designation_DesignationId,
        department_DepartmentId = @department_DepartmentId,
        grade_GradeId = @grade_GradeId,
        region_RegionId = @region_RegionId,
        active = @active,
        description = @description,
        imported_employee = 1
    WHERE employee_code = @employee_code;
ELSE
    INSERT INTO Employees 
    (first_name, last_name, father_name, employee_code, email, mobile_no, cnic_no, gender_id, 
     date_of_birth, date_of_joining, date_of_leaving, designation_DesignationId, department_DepartmentId, 
     grade_GradeId, region_RegionId, active, description, imported_employee, campus_id, timetune_active)
    VALUES 
    (@first_name, @last_name, @father_name, @employee_code, @email, @mobile_no, @cnic_no, @gender_id,
     @date_of_birth, @date_of_joining, @date_of_leaving, @designation_DesignationId, @department_DepartmentId,
     @grade_GradeId, @region_RegionId, @active, @description, 1, 1, 1);";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@first_name", emp.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@last_name", emp.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@father_name", emp.FatherName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@employee_code", emp.EmployeeCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", emp.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@mobile_no", emp.MobileNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cnic_no", emp.CNICNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@gender_id", emp.GenderId);
                cmd.Parameters.AddWithValue("@date_of_birth", emp.DateOfBirth ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@date_of_joining", emp.DateOfJoining ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@date_of_leaving", emp.DateOfLeaving ?? (object)DBNull.Value);
               
                int? validDeptId = await EnsureDepartmentExistsAsync(conn, emp.DepartmentId);
                int? validDesigId = await EnsureDesignationExistsAsync(conn, emp.DesignationId);
                int? validGradeId = await EnsureGradeExistsAsync(conn, emp.GradeId);
                int? validRegionId = await EnsureRegionExistsAsync(conn, emp.RegionId);

                cmd.Parameters.AddWithValue("@department_DepartmentId", validDeptId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@designation_DesignationId", validDesigId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@grade_GradeId", validGradeId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@region_RegionId", validRegionId ?? (object)DBNull.Value);


                cmd.Parameters.AddWithValue("@active", emp.Active);
                cmd.Parameters.AddWithValue("@description", emp.Description ?? (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
