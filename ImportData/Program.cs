// Target framework: .NET Framework 4.8
// NuGet/Refs: System.Net.Http (comes with .NET 4.8), System.Xml.Linq, System.Data

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace ImportData
{
    public class Employee
    {
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }     // allow nulls at runtime
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string CNICNo { get; set; }
        public int GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
        public int? GradeId { get; set; }
        public int? RegionId { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public bool ImportedEmployee { get; set; }
    }

    internal class Program
    {
        // .NET Framework: no async Main. Use a sync entrypoint that waits.
        [STAThread]
        private static void Main(string[] args)
        {
            // Ensure modern TLS for outbound HTTPS
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            try
            {
                RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Fatal error: " + ex);
            }
        }

        private static async Task RunAsync()
        {
            Console.OutputEncoding = Encoding.UTF8;
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

            using (var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            using (var httpClient = new HttpClient(handler))
            {
                // Match Postman-style headers
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.49.0");
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(soapRequest, new UTF8Encoding(false), "text/xml");

                // Optional logging
                Console.WriteLine("\n--- REQUEST HEADERS ---");
                foreach (var header in httpClient.DefaultRequestHeaders)
                    Console.WriteLine(header.Key + ": " + string.Join(",", header.Value));
                Console.WriteLine("\n--- REQUEST BODY ---");
                Console.WriteLine(soapRequest);

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                string responseXml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                Console.WriteLine("\n--- RESPONSE STATUS ---\n" + response.StatusCode);
                Console.WriteLine("\n--- RESPONSE BODY ---");
                Console.WriteLine(responseXml);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ SOAP request failed: " + response.StatusCode);
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
                    var cols = row.Elements(ns3 + "Column").ToList();

                    // local helper is fine in C# 7.x
                    string GetValue(string name)
                    {
                        var col = cols.FirstOrDefault(c =>
                            (c.Attribute("name") != null ? c.Attribute("name").Value : null) == name);
                        return col != null ? (col.Value ?? string.Empty).Trim() : null;
                    }

                    DateTime tmpDate;
                    int tmpInt;

                    var emp = new Employee
                    {
                        EmployeeCode = GetValue("EMPLOYEE_NUMBER"),
                        FirstName = GetValue("FIRST_NAME_EN"),
                        LastName = GetValue("LAST_NAME_EN"),
                        FatherName = GetValue("SECOND_NAME_EN"),
                        Email = GetValue("EMAIL_ADDRESS"),
                        MobileNo = GetValue("MOBILE_NUM"),
                        CNICNo = GetValue("NATIONAL_IDENTIFIER"),
                        GenderId = string.Equals(GetValue("GENDER_CODE"), "F", StringComparison.OrdinalIgnoreCase) ? 2 : 1,
                        DateOfBirth = DateTime.TryParse(GetValue("DATE_OF_BIRTH"), out tmpDate) ? (DateTime?)tmpDate : null,
                        DateOfJoining = DateTime.TryParse(GetValue("JOIN_DATE"), out tmpDate) ? (DateTime?)tmpDate : null,
                        DateOfLeaving = DateTime.TryParse(GetValue("TERMINATION_DATE"), out tmpDate) ? (DateTime?)tmpDate : null,
                        DesignationId = int.TryParse(GetValue("DESIGNATION_ID"), out tmpInt) ? (int?)tmpInt : null,
                        DepartmentId = int.TryParse(GetValue("ORGANIZATION_ID"), out tmpInt) ? (int?)tmpInt : null,
                        GradeId = int.TryParse(GetValue("GRADE_ID"), out tmpInt) ? (int?)tmpInt : null,
                        RegionId = int.TryParse(GetValue("WORK_LOCATION_ID"), out tmpInt) ? (int?)tmpInt : null,
                        Active = string.Equals(GetValue("STATUS"), "A", StringComparison.OrdinalIgnoreCase),
                        Description = GetValue("JOB_TITLE_EN"),
                        ImportedEmployee = true
                    };

                    employees.Add(emp);
                }

                Console.WriteLine("📦 Total employees parsed: " + employees.Count);
                await SaveToDatabaseAsync(employees).ConfigureAwait(false);
                Console.WriteLine("🎉 All employees imported successfully into SQL Server.");
            }
        }

        private static async Task<int?> EnsureDepartmentExistsAsync(SqlConnection conn, int? externalDeptId)
        {
            if (externalDeptId == null) return null;

            const string checkQuery = "SELECT DepartmentId FROM Departments WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalDeptId.Value);
                var result = await checkCmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (result != null && result != DBNull.Value) return Convert.ToInt32(result);
            }

            const string insertQuery = @"
INSERT INTO Departments (dbID, [name], [description], [active])
OUTPUT INSERTED.DepartmentId
VALUES (@dbID, @name, @desc, 1)";
            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalDeptId.Value);
                insertCmd.Parameters.AddWithValue("@name", "FAHR Dept " + externalDeptId);
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");
                var newId = await insertCmd.ExecuteScalarAsync().ConfigureAwait(false);
                return Convert.ToInt32(newId);
            }
        }

        private static async Task<int?> EnsureDesignationExistsAsync(SqlConnection conn, int? externalDesigId)
        {
            if (externalDesigId == null) return null;

            const string checkQuery = "SELECT DesignationId FROM Designations WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalDesigId.Value);
                var result = await checkCmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (result != null && result != DBNull.Value) return Convert.ToInt32(result);
            }

            const string insertQuery = @"
INSERT INTO Designations (dbID, [name], [description], [active])
OUTPUT INSERTED.DesignationId
VALUES (@dbID, @name, @desc, 1)";
            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalDesigId.Value);
                insertCmd.Parameters.AddWithValue("@name", "FAHR Designation " + externalDesigId);
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");
                var newId = await insertCmd.ExecuteScalarAsync().ConfigureAwait(false);
                return Convert.ToInt32(newId);
            }
        }

        private static async Task<int?> EnsureGradeExistsAsync(SqlConnection conn, int? externalGradeId)
        {
            if (externalGradeId == null) return null;

            const string checkQuery = "SELECT GradeId FROM Grades WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalGradeId.Value);
                var result = await checkCmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (result != null && result != DBNull.Value) return Convert.ToInt32(result);
            }

            const string insertQuery = @"
INSERT INTO Grades (dbID, [name], [description], [active])
OUTPUT INSERTED.GradeId
VALUES (@dbID, @name, @desc, 1)";
            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalGradeId.Value);
                insertCmd.Parameters.AddWithValue("@name", "FAHR Grade " + externalGradeId);
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");
                var newId = await insertCmd.ExecuteScalarAsync().ConfigureAwait(false);
                return Convert.ToInt32(newId);
            }
        }

        private static async Task<int?> EnsureRegionExistsAsync(SqlConnection conn, int? externalRegionId)
        {
            if (externalRegionId == null) return null;

            const string checkQuery = "SELECT RegionId FROM Regions WHERE dbID = @dbID";
            using (var checkCmd = new SqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@dbID", externalRegionId.Value);
                var result = await checkCmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (result != null && result != DBNull.Value) return Convert.ToInt32(result);
            }

            const string insertQuery = @"
INSERT INTO Regions (dbID, [name], [description], [active])
OUTPUT INSERTED.RegionId
VALUES (@dbID, @name, @desc, 1)";
            using (var insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@dbID", externalRegionId.Value);
                insertCmd.Parameters.AddWithValue("@name", "FAHR Region " + externalRegionId);
                insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");
                var newId = await insertCmd.ExecuteScalarAsync().ConfigureAwait(false);
                return Convert.ToInt32(newId);
            }
        }

        private static async Task SaveToDatabaseAsync(List<Employee> employees)
        {
            // Adjust for your environment
            string connString = @"Data Source=YASEEN-JARI\SQL2019DEV;Initial Catalog=TRMS;User Id=sa;Password=ur#1sQlok?;Integrated Security=False;TrustServerCertificate=True;";

            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                foreach (var emp in employees)
                {
                    const string query = @"
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

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@first_name", (object)(emp.FirstName ?? (string)null) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@last_name", (object)(emp.LastName ?? (string)null) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@father_name", (object)(emp.FatherName ?? (string)null) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@employee_code", (object)(emp.EmployeeCode ?? (string)null) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", (object)(emp.Email ?? (string)null) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@mobile_no", (object)(emp.MobileNo ?? (string)null) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@cnic_no", (object)(emp.CNICNo ?? (string)null) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@gender_id", emp.GenderId);
                        cmd.Parameters.AddWithValue("@date_of_birth", (object)emp.DateOfBirth ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@date_of_joining", (object)emp.DateOfJoining ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@date_of_leaving", (object)emp.DateOfLeaving ?? DBNull.Value);

                        int? validDeptId = await EnsureDepartmentExistsAsync(conn, emp.DepartmentId).ConfigureAwait(false);
                        int? validDesigId = await EnsureDesignationExistsAsync(conn, emp.DesignationId).ConfigureAwait(false);
                        int? validGradeId = await EnsureGradeExistsAsync(conn, emp.GradeId).ConfigureAwait(false);
                        int? validRegionId = await EnsureRegionExistsAsync(conn, emp.RegionId).ConfigureAwait(false);

                        cmd.Parameters.AddWithValue("@department_DepartmentId", (object)validDeptId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@designation_DesignationId", (object)validDesigId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@grade_GradeId", (object)validGradeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@region_RegionId", (object)validRegionId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@active", emp.Active);
                        cmd.Parameters.AddWithValue("@description", (object)(emp.Description ?? (string)null) ?? DBNull.Value);

                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
