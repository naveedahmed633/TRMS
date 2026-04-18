using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FahrEmployeeImport
{
    /// <summary>
    /// Service class to run FAHR employee import in the background
    /// </summary>
    public static class FahrEmployeeImportService
    {
        private static bool _isRunning = false;
        private static DateTime? _lastRunTime = null;
        private static string? _lastError = null;

        /// <summary>
        /// Runs the import process in the background (fire and forget)
        /// </summary>
        public static void RunImportInBackground(string configPath = null)
        {
            if (_isRunning)
            {
                return; // Already running, skip
            }

            Task.Run(async () =>
            {
                try
                {
                    _isRunning = true;
                    _lastError = null;
                    await RunImportAsync(configPath);
                    _lastRunTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    _lastError = ex.Message;
                    System.Diagnostics.Debug.WriteLine($"FAHR Import Error: {ex.Message}");
                }
                finally
                {
                    _isRunning = false;
                }
            });
        }

        /// <summary>
        /// Runs the import process synchronously
        /// </summary>
        public static async Task RunImportAsync(string configPath = null)
        {
            // Load configuration
            var configurationBuilder = new ConfigurationBuilder();
            
            if (string.IsNullOrEmpty(configPath))
            {
                configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            }

            if (System.IO.File.Exists(configPath))
            {
                configurationBuilder.SetBasePath(System.IO.Path.GetDirectoryName(configPath));
                configurationBuilder.AddJsonFile(System.IO.Path.GetFileName(configPath), optional: false, reloadOnChange: true);
            }
            else
            {
                // Fallback to default location
                var defaultPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (System.IO.File.Exists(defaultPath))
                {
                    configurationBuilder.SetBasePath(System.IO.Path.GetDirectoryName(defaultPath));
                    configurationBuilder.AddJsonFile(System.IO.Path.GetFileName(defaultPath), optional: false, reloadOnChange: true);
                }
            }

            configurationBuilder.AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var fahrConfig = configuration.GetSection("FahrService").Get<FahrServiceConfiguration>()
                ?? throw new InvalidOperationException("FahrService configuration not found.");

            var dbConfig = configuration.GetSection("Database").Get<DatabaseConfiguration>()
                ?? new DatabaseConfiguration();

            // Initialize services
            var soapService = new FahrSoapService(fahrConfig);
            var responseXml = await soapService.GetEmployeeDetailsAsync();

            // Check if this is an employee import, employee leaves import, or lookup data import
            var isEmployeeRequest = fahrConfig.RequestType.Contains("EMPLOYEE_INFO", StringComparison.OrdinalIgnoreCase);
            var isEmployeeLeavesRequest = fahrConfig.RequestType.Equals(FahrRequestTypes.EmployeeLeaves, StringComparison.OrdinalIgnoreCase);

            if (isEmployeeLeavesRequest)
            {
                var leavesParser = new FahrEmployeeLeavesParser();
                var dbService = new EmployeeDatabaseService(connectionString, dbConfig);
                var employeeLeaves = leavesParser.ParseEmployeeLeaves(responseXml);

                if (employeeLeaves.Count > 0)
                {
                    await dbService.UpdateEmployeeLeavesAsync(employeeLeaves);
                }
            }
            else if (isEmployeeRequest)
            {
                // Handle employee import
                var xmlParser = new FahrXmlParser();
                var dbService = new EmployeeDatabaseService(connectionString, dbConfig);

                var employees = xmlParser.ParseEmployees(responseXml);

                if (employees.Count > 0)
                {
                    await dbService.SaveEmployeesAsync(employees);
                }

                // Keep leave balances in sync after employee master import.
                await SyncEmployeeLeavesAsync(fahrConfig, dbService);
            }
            else
            {
                // Handle lookup data import (Designation, Grade, Religion, etc.)
                var lookupParser = new FahrLookupDataParser();
                var lookupDbService = new LookupDataDatabaseService(connectionString);

                var lookupData = lookupParser.ParseLookupData(responseXml, fahrConfig.RequestType);

                if (lookupData.Count > 0)
                {
                    await lookupDbService.ImportLookupDataAsync(lookupData, fahrConfig.RequestType);
                }
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

                var leavesSoapService = new FahrSoapService(leavesConfig);
                var leavesResponseXml = await leavesSoapService.GetEmployeeDetailsAsync();
                var leavesParser = new FahrEmployeeLeavesParser();
                var employeeLeaves = leavesParser.ParseEmployeeLeaves(leavesResponseXml);

                if (employeeLeaves.Count > 0)
                {
                    await dbService.UpdateEmployeeLeavesAsync(employeeLeaves);
                }
            }
            catch
            {
                // Do not fail overall import when leaves sync fails.
            }
        }

        /// <summary>
        /// Gets the current status of the import process
        /// </summary>
        public static ImportStatus GetStatus()
        {
            return new ImportStatus
            {
                IsRunning = _isRunning,
                LastRunTime = _lastRunTime,
                LastError = _lastError
            };
        }
    }

    /// <summary>
    /// Status information about the import process
    /// </summary>
    public class ImportStatus
    {
        public bool IsRunning { get; set; }
        public DateTime? LastRunTime { get; set; }
        public string? LastError { get; set; }
    }
}

