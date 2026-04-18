using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FahrEmployeeImport;

// Constants for XML field names
internal static class FahrXmlFields
{
    public const string EmployeeNumber = "EMPLOYEE_NUMBER";
    public const string FirstNameEn = "FIRST_NAME_EN";
    public const string LastNameEn = "LAST_NAME_EN";
    public const string SecondNameEn = "SECOND_NAME_EN";
    public const string EmailAddress = "EMAIL_ADDRESS";
    public const string MobileNum = "MOBILE_NUM";
    public const string NationalIdentifier = "NATIONAL_IDENTIFIER";
    public const string GenderCode = "GENDER_CODE";
    public const string DateOfBirth = "DATE_OF_BIRTH";
    public const string JoinDate = "JOIN_DATE";
    public const string TerminationDate = "TERMINATION_DATE";
    public const string DesignationId = "DESIGNATION_ID";
    public const string OrganizationId = "ORGANIZATION_ID";
    public const string GradeId = "GRADE_ID";
    public const string WorkLocationId = "WORK_LOCATION_ID";
    public const string Status = "STATUS";
    public const string JobTitleEn = "JOB_TITLE_EN";
    public const string DepartmentNameEn = "DEPARTMENT_NAME_EN";
    public const string DepartmentNameAr = "DEPARTMENT_NAME_AR";
    public const string JobTitleAr = "JOB_TITLE_AR";
    
    // Leave fields
    public const string SickLeaves = "SICK_LEAVES";
    public const string CasualLeaves = "CASUAL_LEAVES";
    public const string AnnualLeaves = "ANNUAL_LEAVES";
    public const string OtherLeaves = "OTHER_LEAVES";
    public const string LeaveType01 = "LEAVE_TYPE_01";
    public const string LeaveType02 = "LEAVE_TYPE_02";
    public const string LeaveType03 = "LEAVE_TYPE_03";
    public const string LeaveType04 = "LEAVE_TYPE_04";
    public const string LeaveType05 = "LEAVE_TYPE_05";
    public const string LeaveType06 = "LEAVE_TYPE_06";
    public const string LeaveType07 = "LEAVE_TYPE_07";
    public const string LeaveType08 = "LEAVE_TYPE_08";
    public const string LeaveType09 = "LEAVE_TYPE_09";
    public const string LeaveType10 = "LEAVE_TYPE_10";
    public const string LeaveType11 = "LEAVE_TYPE_11";
}

// Constants for gender mapping
internal static class GenderMapping
{
    public const int Male = 1;
    public const int Female = 2;
}

// Request Type Constants
internal static class FahrRequestTypes
{
    public const string EmployeeInfo = "EMPLOYEE_INFO";
    public const string EmployeeInfoActive = "EMPLOYEE_INFO_ACTIVE";
    public const string EmployeeLeaves = "EMPLOYEE_LEAVES";
    public const string Designation = "DESIGNATION";
    public const string Religion = "RELIGION";
    public const string Grade = "GRADE";
    public const string LeaveType = "LEAVE_TYPE";
    public const string PermissionType = "PERMISSION_TYPE";
    public const string OrganizationType = "ORGANIZATION_TYPE";
    public const string WorkLocation = "WORK_LOCATION";
    public const string Nationality = "NATIONALITY";
}

// Generic Lookup Data Model
public record LookupData
{
    public required int ExternalId { get; init; }
    public string? Name { get; init; }
    public string? NameEn { get; init; }
    public string? NameAr { get; init; }
    public string? Description { get; init; }
    public string? Code { get; init; }
    public bool Active { get; init; } = true;
}

// Employee model
public record Employee
{
    public required string EmployeeCode { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? FatherName { get; init; }
    public string? Email { get; init; }
    public string? MobileNo { get; init; }
    public string? CNICNo { get; init; }
    public int GenderId { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public DateTime? DateOfJoining { get; init; }
    public DateTime? DateOfLeaving { get; init; }
    public int? DesignationId { get; init; }
    public string? DesignationNameAr { get; init; }
    public string? DesignationNameEn { get; init; }
    public int? DepartmentId { get; init; }
    public string? DepartmentNameAr { get; init; }
    public string? DepartmentNameEn { get; init; }
    public int? GradeId { get; init; }
    public int? RegionId { get; init; }
    public bool Active { get; init; }
    public string? Description { get; init; }
    public bool ImportedEmployee { get; init; } = true;
    public string? Password { get; init; }
    public string? Salt { get; init; }
    public string? SelectLanguage { get; init; }
    public int? SickLeaves { get; init; }
    public int? CasualLeaves { get; init; }
    public int? AnnualLeaves { get; init; }
    public int? OtherLeaves { get; init; }
    public int? LeaveType01 { get; init; }
    public int? LeaveType02 { get; init; }
    public int? LeaveType03 { get; init; }
    public int? LeaveType04 { get; init; }
    public int? LeaveType05 { get; init; }
    public int? LeaveType06 { get; init; }
    public int? LeaveType07 { get; init; }
    public int? LeaveType08 { get; init; }
    public int? LeaveType09 { get; init; }
    public int? LeaveType10 { get; init; }
    public int? LeaveType11 { get; init; }
}

// Configuration model
internal sealed class FahrServiceConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string ExternalAuthorityCode { get; set; } = string.Empty;
    public string TransactionRefNo { get; set; } = string.Empty;
    public string TransactionSubtype { get; set; } = string.Empty;
    public string EntityCode { get; set; } = string.Empty;
    public string IdentificationKey { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}

internal sealed class DatabaseConfiguration
{
    public int DefaultCampusId { get; set; } = 1;
    public bool DefaultTimetuneActive { get; set; } = true;
}

// FAHR SOAP Service Client
internal sealed class FahrSoapService
{
    private readonly FahrServiceConfiguration _config;
    private static readonly HttpClient _httpClient = CreateHttpClient();

    public FahrSoapService(FahrServiceConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        client.DefaultRequestHeaders.Add("User-Agent", "FAHR-Employee-Import/1.0");
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

        return client;
    }

    public async Task<string> GetEmployeeDetailsAsync(CancellationToken cancellationToken = default)
    {
        var soapRequest = BuildSoapRequest();
        var request = new HttpRequestMessage(HttpMethod.Post, _config.BaseUrl)
        {
            Content = new StringContent(soapRequest, new UTF8Encoding(false), "text/xml")
        };

        try
        {
            Console.WriteLine("🔹 Sending SOAP request to FAHR service...");
            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseXml = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"SOAP request failed with status {response.StatusCode}. Response: {responseXml[..Math.Min(500, responseXml.Length)]}");
            }

            Console.WriteLine($"✅ SOAP response received (Status: {response.StatusCode})");
            return responseXml;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException("Request was cancelled", cancellationToken);
        }
    }

    private string BuildSoapRequest()
    {
        // ICD v2.0: GetEmployeeDetailsService uses Service Code 45.
        // This importer only calls GetEmployeeDetails operation.
        const string getEmployeeDetailsServiceId = "45";

        return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:fah=""http://esb.bayanati.gov.ae/services/FAHRAttendanceService/"">
   <soapenv:Header>
      <wsse:Security soapenv:mustUnderstand=""1""
         xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""
         xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
         <wsse:UsernameToken wsu:Id=""UsernameToken-4A14FC3D2847494704158727497458528"">
            <wsse:Username>{_config.Username}</wsse:Username>
            <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">{_config.Password}</wsse:Password>
         </wsse:UsernameToken>
      </wsse:Security>
   </soapenv:Header>
   <soapenv:Body>
      <fah:GetEmployeeDetailsRequest>
         <EAIHeader versionID=""1.0"">
            <ServiceId>{getEmployeeDetailsServiceId}</ServiceId>
            <ExternalAuthorityCode>{_config.ExternalAuthorityCode}</ExternalAuthorityCode>
            <TransactionRefNo>{_config.TransactionRefNo}</TransactionRefNo>
            <TransactionSubtype>{_config.TransactionSubtype}</TransactionSubtype>
            <Notes></Notes>
         </EAIHeader>
         <EAIBody>
            <EntityCode>{_config.EntityCode}</EntityCode>
            <IdentificationKey>{_config.IdentificationKey}</IdentificationKey>
            <RequestType>{_config.RequestType}</RequestType>
         </EAIBody>
      </fah:GetEmployeeDetailsRequest>
   </soapenv:Body>
</soapenv:Envelope>";
    }
}

// XML Parser
internal sealed class FahrXmlParser
{
    private static readonly XNamespace SoapNamespace = 
        "http://xmlns.oracle.com/apps/hxt/soaprovider/plsql/xxfahr_time_attendance_ws_pkg/getemployeedetails/";

    public List<Employee> ParseEmployees(string xmlResponse)
    {
        if (string.IsNullOrWhiteSpace(xmlResponse))
        {
            throw new ArgumentException("XML response cannot be null or empty", nameof(xmlResponse));
        }

        try
        {
            var doc = XDocument.Parse(xmlResponse);
            var rows = doc.Descendants(SoapNamespace + "Row").ToList();

            var employees = new List<Employee>(rows.Count);

            foreach (var row in rows)
            {
                var employee = ParseEmployeeRow(row);
                if (employee != null && !string.IsNullOrWhiteSpace(employee.EmployeeCode))
                {
                    employees.Add(employee);
                }
            }

            return employees;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse XML response", ex);
        }
    }

    private static Employee? ParseEmployeeRow(XElement row)
    {
        var cols = row.Elements(SoapNamespace + "Column").ToList();
        string? GetValue(string name) =>
            cols.FirstOrDefault(c => c.Attribute("name")?.Value == name)?.Value?.Trim();

        var employeeCode = GetValue(FahrXmlFields.EmployeeNumber);
        if (string.IsNullOrWhiteSpace(employeeCode))
        {
            return null; // Skip employees without employee code
        }

        return new Employee
        {
            EmployeeCode = employeeCode,
            FirstName = GetValue(FahrXmlFields.FirstNameEn),
            LastName = GetValue(FahrXmlFields.LastNameEn),
            FatherName = GetValue(FahrXmlFields.SecondNameEn),
            Email = GetValue(FahrXmlFields.EmailAddress),
            MobileNo = GetValue(FahrXmlFields.MobileNum),
            CNICNo = GetValue(FahrXmlFields.NationalIdentifier),
            GenderId = GetValue(FahrXmlFields.GenderCode)?.ToUpperInvariant() == "F" 
                ? GenderMapping.Female 
                : GenderMapping.Male,
            DateOfBirth = ParseDateTime(GetValue(FahrXmlFields.DateOfBirth)),
            DateOfJoining = ParseDateTime(GetValue(FahrXmlFields.JoinDate)),
            DateOfLeaving = ParseDateTime(GetValue(FahrXmlFields.TerminationDate)),
            DesignationId = ParseInt(GetValue(FahrXmlFields.DesignationId)),
            DesignationNameAr = GetValue(FahrXmlFields.JobTitleAr),
            DesignationNameEn = GetValue(FahrXmlFields.JobTitleEn),
            DepartmentId = ParseInt(GetValue(FahrXmlFields.OrganizationId)),
            DepartmentNameAr = CleanDepartmentName(GetValue(FahrXmlFields.DepartmentNameAr)),
            DepartmentNameEn = CleanDepartmentName(GetValue(FahrXmlFields.DepartmentNameEn)),
            GradeId = ParseInt(GetValue(FahrXmlFields.GradeId)),
            RegionId = ParseInt(GetValue(FahrXmlFields.WorkLocationId)),
            Active = GetValue(FahrXmlFields.Status)?.ToUpperInvariant() == "A",
            Description = GetValue(FahrXmlFields.JobTitleEn),
            ImportedEmployee = true,
            // Parse leave data
            SickLeaves = ParseInt(GetValue(FahrXmlFields.SickLeaves)) ?? 
                        ParseInt(GetValue("SICK_LEAVE")) ?? 
                        ParseInt(GetValue("SICK_LEAVE_BALANCE")),
            CasualLeaves = ParseInt(GetValue(FahrXmlFields.CasualLeaves)) ?? 
                          ParseInt(GetValue("CASUAL_LEAVE")) ?? 
                          ParseInt(GetValue("CASUAL_LEAVE_BALANCE")),
            AnnualLeaves = ParseInt(GetValue(FahrXmlFields.AnnualLeaves)) ?? 
                          ParseInt(GetValue("ANNUAL_LEAVE")) ?? 
                          ParseInt(GetValue("ANNUAL_LEAVE_BALANCE")),
            OtherLeaves = ParseInt(GetValue(FahrXmlFields.OtherLeaves)) ?? 
                         ParseInt(GetValue("OTHER_LEAVE")) ?? 
                         ParseInt(GetValue("OTHER_LEAVE_BALANCE")),
            LeaveType01 = ParseInt(GetValue(FahrXmlFields.LeaveType01)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE01")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_01")),
            LeaveType02 = ParseInt(GetValue(FahrXmlFields.LeaveType02)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE02")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_02")),
            LeaveType03 = ParseInt(GetValue(FahrXmlFields.LeaveType03)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE03")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_03")),
            LeaveType04 = ParseInt(GetValue(FahrXmlFields.LeaveType04)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE04")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_04")),
            LeaveType05 = ParseInt(GetValue(FahrXmlFields.LeaveType05)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE05")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_05")),
            LeaveType06 = ParseInt(GetValue(FahrXmlFields.LeaveType06)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE06")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_06")),
            LeaveType07 = ParseInt(GetValue(FahrXmlFields.LeaveType07)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE07")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_07")),
            LeaveType08 = ParseInt(GetValue(FahrXmlFields.LeaveType08)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE08")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_08")),
            LeaveType09 = ParseInt(GetValue(FahrXmlFields.LeaveType09)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE09")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_09")),
            LeaveType10 = ParseInt(GetValue(FahrXmlFields.LeaveType10)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE10")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_10")),
            LeaveType11 = ParseInt(GetValue(FahrXmlFields.LeaveType11)) ?? 
                         ParseInt(GetValue("LEAVE_TYPE11")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_11"))
        };
    }

    private static DateTime? ParseDateTime(string? value)
    {
        return DateTime.TryParse(value, out var result) ? result : null;
    }

    private static int? ParseInt(string? value)
    {
        return int.TryParse(value, out var result) ? result : null;
    }

    private static string? CleanDepartmentName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        // Remove "02/" prefix if present
        if (name.StartsWith("02/", StringComparison.OrdinalIgnoreCase))
        {
            return name.Substring(3).Trim();
        }

        return name.Trim();
    }
}

// Employee Leave Data Model
public record EmployeeLeaveData
{
    public required string EmployeeCode { get; init; }
    public int? SickLeaves { get; init; }
    public int? CasualLeaves { get; init; }
    public int? AnnualLeaves { get; init; }
    public int? OtherLeaves { get; init; }
    public int? LeaveType01 { get; init; }
    public int? LeaveType02 { get; init; }
    public int? LeaveType03 { get; init; }
    public int? LeaveType04 { get; init; }
    public int? LeaveType05 { get; init; }
    public int? LeaveType06 { get; init; }
    public int? LeaveType07 { get; init; }
    public int? LeaveType08 { get; init; }
    public int? LeaveType09 { get; init; }
    public int? LeaveType10 { get; init; }
    public int? LeaveType11 { get; init; }
}

// Employee Leaves Parser
internal sealed class FahrEmployeeLeavesParser
{
    private static readonly XNamespace SoapNamespace = 
        "http://xmlns.oracle.com/apps/hxt/soaprovider/plsql/xxfahr_time_attendance_ws_pkg/getemployeedetails/";
    private static bool _loggedColumns = false;

    public List<EmployeeLeaveData> ParseEmployeeLeaves(string xmlResponse)
    {
        if (string.IsNullOrWhiteSpace(xmlResponse))
        {
            throw new ArgumentException("XML response cannot be null or empty", nameof(xmlResponse));
        }

        try
        {
            var doc = XDocument.Parse(xmlResponse);
            var rows = doc.Descendants(SoapNamespace + "Row").ToList();

            var leaveData = new List<EmployeeLeaveData>(rows.Count);

            foreach (var row in rows)
            {
                var data = ParseLeaveRow(row);
                if (data != null && !string.IsNullOrWhiteSpace(data.EmployeeCode))
                {
                    leaveData.Add(data);
                }
            }

            return leaveData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse employee leaves XML response", ex);
        }
    }

    private static EmployeeLeaveData? ParseLeaveRow(XElement row)
    {
        var cols = row.Elements(SoapNamespace + "Column").ToList();
        string? GetValue(string name) =>
            cols.FirstOrDefault(c => c.Attribute("name")?.Value == name)?.Value?.Trim();

        // Log available columns for debugging (first row only)
        if (!_loggedColumns && cols.Count > 0)
        {
            var columnNames = cols.Select(c => c.Attribute("name")?.Value ?? "NULL").ToList();
            Console.WriteLine($"📋 Available columns in EMPLOYEE_LEAVES response: {string.Join(", ", columnNames)}");
            _loggedColumns = true;
        }

        var employeeCode = GetValue("EMPLOYEE_NUMBER") ?? GetValue("EMPLOYEE_CODE") ?? GetValue("EMP_NUMBER");
        if (string.IsNullOrWhiteSpace(employeeCode))
        {
            return null;
        }

        // Try multiple field name variations based on common patterns
        // The actual field names may vary, so we try multiple possibilities
        return new EmployeeLeaveData
        {
            EmployeeCode = employeeCode,
            // Try various field name patterns for sick leaves
            SickLeaves = ParseInt(GetValue("SICK_LEAVES")) ?? 
                        ParseInt(GetValue("SICK_LEAVE")) ?? 
                        ParseInt(GetValue("SICK_LEAVE_BALANCE")) ??
                        ParseInt(GetValue("SICK_LEAVE_BAL")) ??
                        ParseInt(GetValue("SICK")),
            // Try various field name patterns for casual leaves
            CasualLeaves = ParseInt(GetValue("CASUAL_LEAVES")) ?? 
                          ParseInt(GetValue("CASUAL_LEAVE")) ?? 
                          ParseInt(GetValue("CASUAL_LEAVE_BALANCE")) ??
                          ParseInt(GetValue("CASUAL_LEAVE_BAL")) ??
                          ParseInt(GetValue("CASUAL")),
            // Try various field name patterns for annual leaves
            AnnualLeaves = ParseInt(GetValue("ANNUAL_LEAVES")) ?? 
                          ParseInt(GetValue("ANNUAL_LEAVE")) ?? 
                          ParseInt(GetValue("ANNUAL_LEAVE_BALANCE")) ??
                          ParseInt(GetValue("ANNUAL_LEAVE_BAL")) ??
                          ParseInt(GetValue("ANNUAL")),
            // Try various field name patterns for other leaves
            OtherLeaves = ParseInt(GetValue("OTHER_LEAVES")) ?? 
                         ParseInt(GetValue("OTHER_LEAVE")) ?? 
                         ParseInt(GetValue("OTHER_LEAVE_BALANCE")) ??
                         ParseInt(GetValue("OTHER_LEAVE_BAL")) ??
                         ParseInt(GetValue("OTHER")),
            // Try various field name patterns for leave types
            LeaveType01 = ParseInt(GetValue("LEAVE_TYPE_01")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE01")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_01")) ??
                         ParseInt(GetValue("LEAVE_01")) ??
                         ParseInt(GetValue("LT01")),
            LeaveType02 = ParseInt(GetValue("LEAVE_TYPE_02")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE02")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_02")) ??
                         ParseInt(GetValue("LEAVE_02")) ??
                         ParseInt(GetValue("LT02")),
            LeaveType03 = ParseInt(GetValue("LEAVE_TYPE_03")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE03")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_03")) ??
                         ParseInt(GetValue("LEAVE_03")) ??
                         ParseInt(GetValue("LT03")),
            LeaveType04 = ParseInt(GetValue("LEAVE_TYPE_04")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE04")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_04")) ??
                         ParseInt(GetValue("LEAVE_04")) ??
                         ParseInt(GetValue("LT04")),
            LeaveType05 = ParseInt(GetValue("LEAVE_TYPE_05")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE05")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_05")) ??
                         ParseInt(GetValue("LEAVE_05")) ??
                         ParseInt(GetValue("LT05")),
            LeaveType06 = ParseInt(GetValue("LEAVE_TYPE_06")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE06")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_06")) ??
                         ParseInt(GetValue("LEAVE_06")) ??
                         ParseInt(GetValue("LT06")),
            LeaveType07 = ParseInt(GetValue("LEAVE_TYPE_07")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE07")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_07")) ??
                         ParseInt(GetValue("LEAVE_07")) ??
                         ParseInt(GetValue("LT07")),
            LeaveType08 = ParseInt(GetValue("LEAVE_TYPE_08")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE08")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_08")) ??
                         ParseInt(GetValue("LEAVE_08")) ??
                         ParseInt(GetValue("LT08")),
            LeaveType09 = ParseInt(GetValue("LEAVE_TYPE_09")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE09")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_09")) ??
                         ParseInt(GetValue("LEAVE_09")) ??
                         ParseInt(GetValue("LT09")),
            LeaveType10 = ParseInt(GetValue("LEAVE_TYPE_10")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE10")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_10")) ??
                         ParseInt(GetValue("LEAVE_10")) ??
                         ParseInt(GetValue("LT10")),
            LeaveType11 = ParseInt(GetValue("LEAVE_TYPE_11")) ?? 
                         ParseInt(GetValue("LEAVE_TYPE11")) ?? 
                         ParseInt(GetValue("LEAVE_BALANCE_11")) ??
                         ParseInt(GetValue("LEAVE_11")) ??
                         ParseInt(GetValue("LT11"))
        };
    }

    private static int? ParseInt(string? value)
    {
        return int.TryParse(value, out var result) ? result : null;
    }
}

// Generic Lookup Data Parser
internal sealed class FahrLookupDataParser
{
    private static readonly XNamespace SoapNamespace = 
        "http://xmlns.oracle.com/apps/hxt/soaprovider/plsql/xxfahr_time_attendance_ws_pkg/getemployeedetails/";

    public List<LookupData> ParseLookupData(string xmlResponse, string requestType)
    {
        if (string.IsNullOrWhiteSpace(xmlResponse))
        {
            throw new ArgumentException("XML response cannot be null or empty", nameof(xmlResponse));
        }

        try
        {
            var doc = XDocument.Parse(xmlResponse);
            var rows = doc.Descendants(SoapNamespace + "Row").ToList();

            var lookupData = new List<LookupData>(rows.Count);

            foreach (var row in rows)
            {
                var data = ParseLookupRow(row, requestType);
                if (data != null)
                {
                    lookupData.Add(data);
                }
            }

            return lookupData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse XML response for {requestType}", ex);
        }
    }

    private static LookupData? ParseLookupRow(XElement row, string requestType)
    {
        var cols = row.Elements(SoapNamespace + "Column").ToList();
        string? GetValue(string name) =>
            cols.FirstOrDefault(c => c.Attribute("name")?.Value == name)?.Value?.Trim();

        // Common field names that might be used across different request types
        var idValue = GetValue("ID") ?? GetValue("DESIGNATION_ID") ?? GetValue("GRADE_ID") ?? 
                     GetValue("RELIGION_ID") ?? GetValue("LEAVE_TYPE_ID") ?? GetValue("NATIONALITY_ID") ??
                     GetValue("WORK_LOCATION_ID") ?? GetValue("ORGANIZATION_TYPE_ID") ?? GetValue("PERMISSION_TYPE_ID");

        if (string.IsNullOrWhiteSpace(idValue) || !int.TryParse(idValue, out var externalId))
        {
            return null;
        }

        // For GRADE and some other types, the field names are different:
        // - NAME = English name
        // - ARABIC_NAME = Arabic name
        var nameEn = GetValue("NAME_EN") ?? GetValue("DESIGNATION_NAME_EN") ?? GetValue("GRADE_NAME_EN") ??
                    GetValue("RELIGION_NAME_EN") ?? GetValue("LEAVE_TYPE_NAME_EN") ?? GetValue("NATIONALITY_NAME_EN") ??
                    GetValue("WORK_LOCATION_NAME_EN") ?? GetValue("NAME"); // NAME is English for GRADE
        
        var nameAr = GetValue("NAME_AR") ?? GetValue("DESIGNATION_NAME_AR") ?? GetValue("GRADE_NAME_AR") ??
                    GetValue("RELIGION_NAME_AR") ?? GetValue("LEAVE_TYPE_NAME_AR") ?? GetValue("NATIONALITY_NAME_AR") ??
                    GetValue("WORK_LOCATION_NAME_AR") ?? GetValue("ARABIC_NAME"); // ARABIC_NAME for GRADE

        var description = GetValue("DESCRIPTION") ?? GetValue("DESIGNATION_DESCRIPTION") ?? 
                         GetValue("GRADE_DESCRIPTION") ?? GetValue("MEANING");
        
        var code = GetValue("CODE");
        var status = GetValue("STATUS") ?? GetValue("ACTIVE_FLAG");
        var isActive = status?.ToUpperInvariant() == "Y" || status?.ToUpperInvariant() == "A" || 
                      status?.ToUpperInvariant() == "ACTIVE" || string.IsNullOrWhiteSpace(status);

        return new LookupData
        {
            ExternalId = externalId,
            Name = nameEn ?? nameAr,
            NameEn = nameEn,
            NameAr = nameAr,
            Description = description,
            Code = code,
            Active = isActive
        };
    }
}

// Database Service
internal sealed class EmployeeDatabaseService
{
    private readonly string _connectionString;
    private readonly DatabaseConfiguration _dbConfig;
    private const string DefaultPassword = "112233";

    public EmployeeDatabaseService(string connectionString, DatabaseConfiguration dbConfig)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _dbConfig = dbConfig ?? throw new ArgumentNullException(nameof(dbConfig));
    }

    private static (string hashedPassword, string salt) GeneratePasswordHash(string password)
    {
        // Generate a random salt
        var saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        var salt = Convert.ToBase64String(saltBytes);

        // Hash the password with the salt
        var passwordBytes = Encoding.UTF8.GetBytes(password + salt);
        var hashBytes = SHA256.HashData(passwordBytes);
        var hashedPassword = Convert.ToBase64String(hashBytes);

        return (hashedPassword, salt);
    }

    public async Task SaveEmployeesAsync(List<Employee> employees, CancellationToken cancellationToken = default)
    {
        if (employees == null || employees.Count == 0)
        {
            Console.WriteLine("⚠️ No employees to save.");
            return;
        }

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(cancellationToken);

        await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync(cancellationToken);

        try
        {
            var successCount = 0;
            var errorCount = 0;

            foreach (var emp in employees)
            {
                try
                {
                    // Generate password hash and salt if not already set
                    var (hashedPassword, salt) = string.IsNullOrEmpty(emp.Password) 
                        ? GeneratePasswordHash(DefaultPassword) 
                        : (emp.Password, emp.Salt ?? string.Empty);

                    // Create employee with password and salt
                    var employeeWithPassword = emp with { Password = hashedPassword, Salt = salt };

                    // Ensure related entities exist
                    var validDeptId = await EnsureDepartmentExistsAsync(conn, transaction, employeeWithPassword.DepartmentId, employeeWithPassword.DepartmentNameAr, employeeWithPassword.DepartmentNameEn, cancellationToken);
                    var validDesigId = await EnsureDesignationExistsAsync(conn, transaction, employeeWithPassword.DesignationId, employeeWithPassword.DesignationNameAr, employeeWithPassword.DesignationNameEn, cancellationToken);
                    var validGradeId = await EnsureGradeExistsAsync(conn, transaction, employeeWithPassword.GradeId, cancellationToken);
                    var validRegionId = await EnsureRegionExistsAsync(conn, transaction, employeeWithPassword.RegionId, cancellationToken);

                    // Save or update employee
                    await SaveOrUpdateEmployeeAsync(conn, transaction, employeeWithPassword, validDeptId, validDesigId, validGradeId, validRegionId, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Console.WriteLine($"❌ Error saving employee {emp.EmployeeCode}: {ex.Message}");
                    // Continue with next employee
                }
            }

            await transaction.CommitAsync(cancellationToken);
            Console.WriteLine($"✅ Successfully saved {successCount} employees. Errors: {errorCount}");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task<int?> EnsureDepartmentExistsAsync(
        SqlConnection conn, 
        SqlTransaction transaction, 
        int? externalDeptId,
        string? departmentNameAr,
        string? departmentNameEn,
        CancellationToken cancellationToken)
    {
        if (externalDeptId == null) return null;

        const string checkQuery = "SELECT DepartmentId FROM Departments WHERE dbID = @dbID";
        await using var checkCmd = new SqlCommand(checkQuery, conn, transaction);
        checkCmd.Parameters.AddWithValue("@dbID", externalDeptId.Value);
        
        var result = await checkCmd.ExecuteScalarAsync(cancellationToken);
        if (result != null && result != DBNull.Value)
        {
            // Update the name if it exists and we have a better name (and current name is generic)
            if (!string.IsNullOrWhiteSpace(departmentNameAr) || !string.IsNullOrWhiteSpace(departmentNameEn))
            {
                const string updateQuery = @"
                    UPDATE Departments SET [name] = @name, [description] = @desc
                    WHERE dbID = @dbID AND ([name] LIKE 'FAHR Dept%' OR [name] IS NULL OR [name] = '')";
                await using var updateCmd = new SqlCommand(updateQuery, conn, transaction);
                updateCmd.Parameters.AddWithValue("@dbID", externalDeptId.Value);
                updateCmd.Parameters.AddWithValue("@name", departmentNameAr ?? departmentNameEn ?? $"FAHR Dept {externalDeptId}");
                updateCmd.Parameters.AddWithValue("@desc", departmentNameEn ?? departmentNameAr ?? "Imported from FAHR");
                await updateCmd.ExecuteNonQueryAsync(cancellationToken);
            }
            return Convert.ToInt32(result);
        }

        // If not found — create it with actual name if available
        const string insertQuery = @"
            INSERT INTO Departments (dbID, [name], [description], [active])
            OUTPUT INSERTED.DepartmentId
            VALUES (@dbID, @name, @desc, 1)";

        await using var insertCmd = new SqlCommand(insertQuery, conn, transaction);
        insertCmd.Parameters.AddWithValue("@dbID", externalDeptId.Value);
        insertCmd.Parameters.AddWithValue("@name", departmentNameAr ?? departmentNameEn ?? $"FAHR Dept {externalDeptId}");
        insertCmd.Parameters.AddWithValue("@desc", departmentNameEn ?? departmentNameAr ?? "Imported from FAHR");

        var newId = await insertCmd.ExecuteScalarAsync(cancellationToken);
        return newId != null ? Convert.ToInt32(newId) : null;
    }

    private static async Task<int?> EnsureDesignationExistsAsync(
        SqlConnection conn, 
        SqlTransaction transaction, 
        int? externalDesigId,
        string? designationNameAr,
        string? designationNameEn,
        CancellationToken cancellationToken)
    {
        if (externalDesigId == null) return null;

        const string checkQuery = "SELECT DesignationId FROM Designations WHERE dbID = @dbID";
        await using var checkCmd = new SqlCommand(checkQuery, conn, transaction);
        checkCmd.Parameters.AddWithValue("@dbID", externalDesigId.Value);
        
        var result = await checkCmd.ExecuteScalarAsync(cancellationToken);
        if (result != null && result != DBNull.Value)
        {
            // Update the name if it exists and we have a better name (and current name is generic)
            if (!string.IsNullOrWhiteSpace(designationNameAr) || !string.IsNullOrWhiteSpace(designationNameEn))
            {
                const string updateQuery = @"
                    UPDATE Designations SET [name] = @name, [description] = @desc
                    WHERE dbID = @dbID AND ([name] LIKE 'FAHR Designation%' OR [name] IS NULL OR [name] = '')";
                await using var updateCmd = new SqlCommand(updateQuery, conn, transaction);
                updateCmd.Parameters.AddWithValue("@dbID", externalDesigId.Value);
                updateCmd.Parameters.AddWithValue("@name", designationNameAr ?? designationNameEn ?? $"FAHR Designation {externalDesigId}");
                updateCmd.Parameters.AddWithValue("@desc", designationNameEn ?? designationNameAr ?? "Imported from FAHR");
                await updateCmd.ExecuteNonQueryAsync(cancellationToken);
            }
            return Convert.ToInt32(result);
        }

        // If not found, create a new one with actual name if available
        const string insertQuery = @"
            INSERT INTO Designations (dbID, [name], [description], [active])
            OUTPUT INSERTED.DesignationId
            VALUES (@dbID, @name, @desc, 1)";

        await using var insertCmd = new SqlCommand(insertQuery, conn, transaction);
        insertCmd.Parameters.AddWithValue("@dbID", externalDesigId.Value);
        insertCmd.Parameters.AddWithValue("@name", designationNameAr ?? designationNameEn ?? $"FAHR Designation {externalDesigId}");
        insertCmd.Parameters.AddWithValue("@desc", designationNameEn ?? designationNameAr ?? "Imported from FAHR");

        var newId = await insertCmd.ExecuteScalarAsync(cancellationToken);
        return newId != null ? Convert.ToInt32(newId) : null;
    }

    private static async Task<int?> EnsureGradeExistsAsync(
        SqlConnection conn, 
        SqlTransaction transaction, 
        int? externalGradeId, 
        CancellationToken cancellationToken)
    {
        if (externalGradeId == null) return null;

        const string checkQuery = "SELECT GradeId FROM Grades WHERE dbID = @dbID";
        await using var checkCmd = new SqlCommand(checkQuery, conn, transaction);
        checkCmd.Parameters.AddWithValue("@dbID", externalGradeId.Value);
        
        var result = await checkCmd.ExecuteScalarAsync(cancellationToken);
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32(result);
        }

        const string insertQuery = @"
            INSERT INTO Grades (dbID, [name], [description], [active])
            OUTPUT INSERTED.GradeId
            VALUES (@dbID, @name, @desc, 1)";

        await using var insertCmd = new SqlCommand(insertQuery, conn, transaction);
        insertCmd.Parameters.AddWithValue("@dbID", externalGradeId.Value);
        insertCmd.Parameters.AddWithValue("@name", $"FAHR Grade {externalGradeId}");
        insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");

        var newId = await insertCmd.ExecuteScalarAsync(cancellationToken);
        return newId != null ? Convert.ToInt32(newId) : null;
    }

    private static async Task<int?> EnsureRegionExistsAsync(
        SqlConnection conn, 
        SqlTransaction transaction, 
        int? externalRegionId, 
        CancellationToken cancellationToken)
    {
        if (externalRegionId == null) return 0; // Default to 0 if not provided

        const string checkQuery = "SELECT RegionId FROM Regions WHERE dbID = @dbID";
        await using var checkCmd = new SqlCommand(checkQuery, conn, transaction);
        checkCmd.Parameters.AddWithValue("@dbID", externalRegionId.Value);
        
        var result = await checkCmd.ExecuteScalarAsync(cancellationToken);
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32(result);
        }

        const string insertQuery = @"
            INSERT INTO Regions (dbID, [name], [description], [active])
            OUTPUT INSERTED.RegionId
            VALUES (@dbID, @name, @desc, 1)";

        await using var insertCmd = new SqlCommand(insertQuery, conn, transaction);
        insertCmd.Parameters.AddWithValue("@dbID", externalRegionId.Value);
        insertCmd.Parameters.AddWithValue("@name", $"FAHR Region {externalRegionId}");
        insertCmd.Parameters.AddWithValue("@desc", "Auto-created from FAHR import");

        var newId = await insertCmd.ExecuteScalarAsync(cancellationToken);
        return newId != null ? Convert.ToInt32(newId) : null;
    }

    private async Task SaveOrUpdateEmployeeAsync(
        SqlConnection conn,
        SqlTransaction transaction,
        Employee emp,
        int? validDeptId,
        int? validDesigId,
        int? validGradeId,
        int? validRegionId,
        CancellationToken cancellationToken)
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
                    imported_employee = 1,
                    password = @password,
                    salt = @salt,
                    select_langauge = @select_language,
                    sick_leaves = COALESCE(@sick_leaves, sick_leaves),
                    casual_leaves = COALESCE(@casual_leaves, casual_leaves),
                    annual_leaves = COALESCE(@annual_leaves, annual_leaves),
                    other_leaves = COALESCE(@other_leaves, other_leaves),
                    leave_type01 = COALESCE(@leave_type01, leave_type01),
                    leave_type02 = COALESCE(@leave_type02, leave_type02),
                    leave_type03 = COALESCE(@leave_type03, leave_type03),
                    leave_type04 = COALESCE(@leave_type04, leave_type04),
                    leave_type05 = COALESCE(@leave_type05, leave_type05),
                    leave_type06 = COALESCE(@leave_type06, leave_type06),
                    leave_type07 = COALESCE(@leave_type07, leave_type07),
                    leave_type08 = COALESCE(@leave_type08, leave_type08),
                    leave_type09 = COALESCE(@leave_type09, leave_type09),
                    leave_type10 = COALESCE(@leave_type10, leave_type10),
                    leave_type11 = COALESCE(@leave_type11, leave_type11)
                WHERE employee_code = @employee_code;
            ELSE
                INSERT INTO Employees 
                (first_name, last_name, father_name, employee_code, email, mobile_no, cnic_no, gender_id, 
                 date_of_birth, date_of_joining, date_of_leaving, designation_DesignationId, department_DepartmentId, 
                 grade_GradeId, region_RegionId, active, description, imported_employee, campus_id, timetune_active, password, salt, select_langauge,
                 sick_leaves, casual_leaves, annual_leaves, other_leaves, leave_type01, leave_type02, leave_type03, leave_type04, leave_type05,
                 leave_type06, leave_type07, leave_type08, leave_type09, leave_type10, leave_type11)
                VALUES 
                (@first_name, @last_name, @father_name, @employee_code, @email, @mobile_no, @cnic_no, @gender_id,
                 @date_of_birth, @date_of_joining, @date_of_leaving, @designation_DesignationId, @department_DepartmentId,
                 @grade_GradeId, @region_RegionId, @active, @description, 1, @campus_id, @timetune_active, @password, @salt, @select_language,
                 @sick_leaves, @casual_leaves, @annual_leaves, @other_leaves, @leave_type01, @leave_type02, @leave_type03, @leave_type04, @leave_type05,
                 @leave_type06, @leave_type07, @leave_type08, @leave_type09, @leave_type10, @leave_type11);";

        await using var cmd = new SqlCommand(query, conn, transaction);
        
        cmd.Parameters.AddWithValue("@first_name", emp.FirstName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@last_name", emp.LastName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@father_name", emp.FatherName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@employee_code", emp.EmployeeCode);
        cmd.Parameters.AddWithValue("@email", emp.Email ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@mobile_no", emp.MobileNo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@cnic_no", emp.CNICNo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@gender_id", emp.GenderId);
        cmd.Parameters.AddWithValue("@date_of_birth", emp.DateOfBirth ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@date_of_joining", emp.DateOfJoining ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@date_of_leaving", emp.DateOfLeaving ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@department_DepartmentId", validDeptId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@designation_DesignationId", validDesigId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@grade_GradeId", validGradeId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@region_RegionId", validRegionId ?? 0); // Default to 0 if not provided
        cmd.Parameters.AddWithValue("@active", emp.Active);
        cmd.Parameters.AddWithValue("@description", emp.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@campus_id", _dbConfig.DefaultCampusId);
        cmd.Parameters.AddWithValue("@timetune_active", _dbConfig.DefaultTimetuneActive);
        cmd.Parameters.AddWithValue("@password", emp.Password ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@salt", emp.Salt ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@select_language", emp.SelectLanguage ?? "En");
        cmd.Parameters.AddWithValue("@sick_leaves", (object?)emp.SickLeaves ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@casual_leaves", (object?)emp.CasualLeaves ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@annual_leaves", (object?)emp.AnnualLeaves ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@other_leaves", (object?)emp.OtherLeaves ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type01", (object?)emp.LeaveType01 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type02", (object?)emp.LeaveType02 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type03", (object?)emp.LeaveType03 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type04", (object?)emp.LeaveType04 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type05", (object?)emp.LeaveType05 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type06", (object?)emp.LeaveType06 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type07", (object?)emp.LeaveType07 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type08", (object?)emp.LeaveType08 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type09", (object?)emp.LeaveType09 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type10", (object?)emp.LeaveType10 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@leave_type11", (object?)emp.LeaveType11 ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateEmployeeLeavesAsync(List<EmployeeLeaveData> leaveData, CancellationToken cancellationToken = default)
    {
        if (leaveData == null || leaveData.Count == 0)
        {
            Console.WriteLine("⚠️ No employee leave data to update.");
            return;
        }

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(cancellationToken);

        await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync(cancellationToken);

        try
        {
            var successCount = 0;
            var errorCount = 0;

            foreach (var leave in leaveData)
            {
                try
                {
                    const string updateQuery = @"
                        UPDATE Employees SET 
                            sick_leaves = COALESCE(@sick_leaves, sick_leaves),
                            casual_leaves = COALESCE(@casual_leaves, casual_leaves),
                            annual_leaves = COALESCE(@annual_leaves, annual_leaves),
                            other_leaves = COALESCE(@other_leaves, other_leaves),
                            leave_type01 = COALESCE(@leave_type01, leave_type01),
                            leave_type02 = COALESCE(@leave_type02, leave_type02),
                            leave_type03 = COALESCE(@leave_type03, leave_type03),
                            leave_type04 = COALESCE(@leave_type04, leave_type04),
                            leave_type05 = COALESCE(@leave_type05, leave_type05),
                            leave_type06 = COALESCE(@leave_type06, leave_type06),
                            leave_type07 = COALESCE(@leave_type07, leave_type07),
                            leave_type08 = COALESCE(@leave_type08, leave_type08),
                            leave_type09 = COALESCE(@leave_type09, leave_type09),
                            leave_type10 = COALESCE(@leave_type10, leave_type10),
                            leave_type11 = COALESCE(@leave_type11, leave_type11)
                        WHERE employee_code = @employee_code";

                    await using var cmd = new SqlCommand(updateQuery, conn, transaction);
                    cmd.Parameters.AddWithValue("@employee_code", leave.EmployeeCode);
                    cmd.Parameters.AddWithValue("@sick_leaves", (object?)leave.SickLeaves ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@casual_leaves", (object?)leave.CasualLeaves ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@annual_leaves", (object?)leave.AnnualLeaves ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@other_leaves", (object?)leave.OtherLeaves ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type01", (object?)leave.LeaveType01 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type02", (object?)leave.LeaveType02 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type03", (object?)leave.LeaveType03 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type04", (object?)leave.LeaveType04 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type05", (object?)leave.LeaveType05 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type06", (object?)leave.LeaveType06 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type07", (object?)leave.LeaveType07 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type08", (object?)leave.LeaveType08 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type09", (object?)leave.LeaveType09 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type10", (object?)leave.LeaveType10 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@leave_type11", (object?)leave.LeaveType11 ?? DBNull.Value);

                    var rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);
                    if (rowsAffected > 0)
                    {
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Console.WriteLine($"❌ Error updating leaves for employee {leave.EmployeeCode}: {ex.Message}");
                }
            }

            await transaction.CommitAsync(cancellationToken);
            Console.WriteLine($"✅ Successfully updated leaves for {successCount} employees. Errors: {errorCount}");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

// Lookup Data Database Service
internal sealed class LookupDataDatabaseService
{
    private readonly string _connectionString;

    public LookupDataDatabaseService(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task ImportLookupDataAsync(List<LookupData> lookupData, string requestType, CancellationToken cancellationToken = default)
    {
        if (lookupData == null || lookupData.Count == 0)
        {
            Console.WriteLine($"⚠️ No {requestType} data to import.");
            return;
        }

        var tableName = GetTableName(requestType);
        if (string.IsNullOrEmpty(tableName))
        {
            Console.WriteLine($"⚠️ Unknown request type: {requestType}. Skipping import.");
            return;
        }

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(cancellationToken);

        await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync(cancellationToken);

        try
        {
            var successCount = 0;
            var errorCount = 0;

            foreach (var data in lookupData)
            {
                try
                {
                    await SaveOrUpdateLookupDataAsync(conn, transaction, data, tableName, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Console.WriteLine($"❌ Error saving {requestType} data (ID: {data.ExternalId}): {ex.Message}");
                }
            }

            await transaction.CommitAsync(cancellationToken);
            Console.WriteLine($"✅ Successfully imported {successCount} {requestType} records. Errors: {errorCount}");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static string GetTableName(string requestType)
    {
        return requestType.ToUpperInvariant() switch
        {
            FahrRequestTypes.Designation => "Designations",
            FahrRequestTypes.Grade => "Grades",
            FahrRequestTypes.Religion => "Religions",
            FahrRequestTypes.LeaveType => "LeaveTypes",
            FahrRequestTypes.WorkLocation => "Regions",
            FahrRequestTypes.OrganizationType => "Departments",
            FahrRequestTypes.Nationality => "Nationalities",
            FahrRequestTypes.PermissionType => "PermissionTypes",
            _ => string.Empty
        };
    }

    private static async Task SaveOrUpdateLookupDataAsync(
        SqlConnection conn,
        SqlTransaction transaction,
        LookupData data,
        string tableName,
        CancellationToken cancellationToken)
    {
        // Check if record exists by dbID
        var checkQuery = $"SELECT COUNT(1) FROM {tableName} WHERE dbID = @dbID";
        await using var checkCmd = new SqlCommand(checkQuery, conn, transaction);
        checkCmd.Parameters.AddWithValue("@dbID", data.ExternalId);
        
        var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellationToken)) > 0;

        string query;
        if (exists)
        {
            // Update existing record
            query = $@"
                UPDATE {tableName} SET 
                    [name] = @name,
                    [description] = @description,
                    [active] = @active
                WHERE dbID = @dbID";
        }
        else
        {
            // Insert new record
            query = $@"
                INSERT INTO {tableName} (dbID, [name], [description], [active])
                VALUES (@dbID, @name, @description, @active)";
        }

        await using var cmd = new SqlCommand(query, conn, transaction);
        cmd.Parameters.AddWithValue("@dbID", data.ExternalId);
        
        // Store Arabic in name column and English in description column (same pattern as Departments/Designations)
        // If Arabic is not available, fall back to English for name
        var nameValue = data.NameAr ?? data.NameEn ?? $"FAHR {data.ExternalId}";
        var descriptionValue = data.NameEn ?? data.Description ?? string.Empty;
        
        // For existing records (especially Grades), update if they have generic names
        if (exists)
        {
            // Check if current name is generic, if so update it
            var checkNameQuery = $"SELECT [name] FROM {tableName} WHERE dbID = @dbID";
            await using var checkNameCmd = new SqlCommand(checkNameQuery, conn, transaction);
            checkNameCmd.Parameters.AddWithValue("@dbID", data.ExternalId);
            var currentName = await checkNameCmd.ExecuteScalarAsync(cancellationToken) as string;
            
            // Only update name if current name is generic (starts with "FAHR ") or empty
            // Always update description with English name
            if (string.IsNullOrWhiteSpace(currentName) || currentName.StartsWith("FAHR ", StringComparison.OrdinalIgnoreCase))
            {
                cmd.Parameters.AddWithValue("@name", nameValue);
            }
            else
            {
                // Keep existing name if it's not generic
                cmd.Parameters.AddWithValue("@name", currentName);
            }
            cmd.Parameters.AddWithValue("@description", descriptionValue);
        }
        else
        {
            // For new records, use Arabic for name and English for description
            cmd.Parameters.AddWithValue("@name", nameValue);
            cmd.Parameters.AddWithValue("@description", descriptionValue);
        }
        
        cmd.Parameters.AddWithValue("@active", data.Active);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}

// Main Program
internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var fahrConfig = configuration.GetSection("FahrService").Get<FahrServiceConfiguration>()
                ?? throw new InvalidOperationException("FahrService configuration not found.");

            var dbConfig = configuration.GetSection("Database").Get<DatabaseConfiguration>()
                ?? new DatabaseConfiguration();

            Console.WriteLine("🔹 Starting FAHR Data Import Process...");
            Console.WriteLine($"   Service URL: {fahrConfig.BaseUrl}");
            Console.WriteLine($"   Request Type: {fahrConfig.RequestType}\n");

            // Initialize services
            var soapService = new FahrSoapService(fahrConfig);
            var responseXml = await soapService.GetEmployeeDetailsAsync();

            // Check if this is an employee import, employee leaves, or lookup data import
            var isEmployeeRequest = fahrConfig.RequestType.Contains("EMPLOYEE_INFO", StringComparison.OrdinalIgnoreCase);
            var isEmployeeLeavesRequest = fahrConfig.RequestType.Equals(FahrRequestTypes.EmployeeLeaves, StringComparison.OrdinalIgnoreCase);

            if (isEmployeeLeavesRequest)
            {
                // Handle employee leaves import - update leave balances for employees
                var leavesParser = new FahrEmployeeLeavesParser();
                var dbService = new EmployeeDatabaseService(connectionString, dbConfig);

                var employeeLeaves = leavesParser.ParseEmployeeLeaves(responseXml);
                Console.WriteLine($"📦 Total employee leave records parsed: {employeeLeaves.Count}");

                if (employeeLeaves.Count == 0)
                {
                    Console.WriteLine("⚠️ No employee leave data found to import.");
                    return;
                }

                await dbService.UpdateEmployeeLeavesAsync(employeeLeaves);
                Console.WriteLine("\n🎉 Employee leaves import process completed successfully!");
            }
            else if (isEmployeeRequest)
            {
                // Handle employee import
                var xmlParser = new FahrXmlParser();
                var dbService = new EmployeeDatabaseService(connectionString, dbConfig);

                var employees = xmlParser.ParseEmployees(responseXml);
                Console.WriteLine($"📦 Total employees parsed: {employees.Count}");

                if (employees.Count == 0)
                {
                    Console.WriteLine("⚠️ No employees found to import.");
                    return;
                }

                await dbService.SaveEmployeesAsync(employees);

                // After employee master data import, always sync leave balances.
                // EMPLOYEE_INFO responses may not include leave fields.
                await SyncEmployeeLeavesAsync(fahrConfig, dbService);
                Console.WriteLine("\n🎉 Employee import process completed successfully!");
            }
            else
            {
                // Handle lookup data import (Designation, Grade, Religion, etc.)
                var lookupParser = new FahrLookupDataParser();
                var lookupDbService = new LookupDataDatabaseService(connectionString);

                var lookupData = lookupParser.ParseLookupData(responseXml, fahrConfig.RequestType);
                Console.WriteLine($"📦 Total {fahrConfig.RequestType} records parsed: {lookupData.Count}");

                if (lookupData.Count == 0)
                {
                    Console.WriteLine($"⚠️ No {fahrConfig.RequestType} data found to import.");
                    return;
                }

                await lookupDbService.ImportLookupDataAsync(lookupData, fahrConfig.RequestType);
                Console.WriteLine($"\n🎉 {fahrConfig.RequestType} import process completed successfully!");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ HTTP Error: {ex.Message}");
            Environment.ExitCode = 1;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"❌ Configuration Error: {ex.Message}");
            Environment.ExitCode = 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            Console.WriteLine($"   Stack Trace: {ex.StackTrace}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task SyncEmployeeLeavesAsync(FahrServiceConfiguration fahrConfig, EmployeeDatabaseService dbService)
    {
        try
        {
            var leavesConfig = new FahrServiceConfiguration
            {
                BaseUrl = fahrConfig.BaseUrl,
                Username = fahrConfig.Username,
                Password = fahrConfig.Password,
                ServiceId = fahrConfig.ServiceId,
                ExternalAuthorityCode = fahrConfig.ExternalAuthorityCode,
                TransactionRefNo = fahrConfig.TransactionRefNo,
                TransactionSubtype = fahrConfig.TransactionSubtype,
                EntityCode = fahrConfig.EntityCode,
                IdentificationKey = fahrConfig.IdentificationKey,
                RequestType = FahrRequestTypes.EmployeeLeaves,
                TimeoutSeconds = fahrConfig.TimeoutSeconds
            };

            Console.WriteLine("🔹 Syncing employee leaves using EMPLOYEE_LEAVES request...");
            var leavesSoapService = new FahrSoapService(leavesConfig);
            var leavesResponseXml = await leavesSoapService.GetEmployeeDetailsAsync();

            var leavesParser = new FahrEmployeeLeavesParser();
            var employeeLeaves = leavesParser.ParseEmployeeLeaves(leavesResponseXml);
            Console.WriteLine($"📦 Total employee leave records parsed: {employeeLeaves.Count}");

            if (employeeLeaves.Count > 0)
            {
                await dbService.UpdateEmployeeLeavesAsync(employeeLeaves);
            }
            else
            {
                Console.WriteLine("⚠️ EMPLOYEE_LEAVES returned no records.");
            }
        }
        catch (Exception ex)
        {
            // Keep employee import successful even if leaves sync fails.
            Console.WriteLine($"⚠️ Employee leaves sync skipped due to error: {ex.Message}");
        }
    }
}
