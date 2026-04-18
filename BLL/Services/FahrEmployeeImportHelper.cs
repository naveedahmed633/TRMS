using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BLL.Services
{
    /// <summary>
    /// Helper class to run FAHR Employee Import in the background
    /// </summary>
    public static class FahrEmployeeImportHelper
    {
        private static bool _isRunning = false;
        private static DateTime? _lastRunTime = null;
        private static object _lockObject = new object();

        /// <summary>
        /// Runs the FAHR employee import process in the background
        /// </summary>
        public static void RunImportInBackground()
        {
            lock (_lockObject)
            {
                if (_isRunning)
                {
                    return; // Already running, skip
                }

                _isRunning = true;
            }

            Task.Run(() =>
            {
                try
                {
                    // Get the base path of the running web application
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    //LogPathCheck($"BaseDirectory = {basePath}");

                    try
                    {
                        if (HttpContext.Current != null && HttpContext.Current.Server != null)
                        {
                            string mapped = HttpContext.Current.Server.MapPath("~/");
                            //LogPathCheck($"Server.MapPath = {mapped}");
                            basePath = mapped;
                        }
                    }
                    catch (Exception ex)
                    {
                       // LogPathCheck("MapPath error: " + ex.Message);
                    }

                    // Search paths:
                    // 1) web root/bin for deployment scenarios
                    // 2) sibling project output for local development builds
                    var searchRoots = new List<string>
                    {
                        basePath,
                        Path.Combine(basePath, "bin"),
                        Path.GetFullPath(Path.Combine(basePath, "..")),
                        Path.GetFullPath(Path.Combine(basePath, "..", ".."))
                    };

                    var possiblePaths = searchRoots
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .SelectMany(root => new[]
                        {
                            Path.Combine(root, "ImportFahrEmployeeData.exe"),
                            Path.Combine(root, "ImportFahrEmployeeData.dll"),
                            Path.Combine(root, "ImportFahrEmployeeData", "bin", "Debug", "net8.0", "ImportFahrEmployeeData.exe"),
                            Path.Combine(root, "ImportFahrEmployeeData", "bin", "Release", "net8.0", "ImportFahrEmployeeData.exe"),
                            Path.Combine(root, "ImportFahrEmployeeData", "bin", "Debug", "net8.0", "ImportFahrEmployeeData.dll"),
                            Path.Combine(root, "ImportFahrEmployeeData", "bin", "Release", "net8.0", "ImportFahrEmployeeData.dll")
                        })
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    string importAppPath = null;

                    foreach (var path in possiblePaths)
                    {
                        bool exists = File.Exists(path);
                        //LogPathCheck($"Checking path: {path} | Exists: {exists}");

                        if (exists)
                        {
                            importAppPath = path;
                          //  LogPathCheck($"FOUND Import App: {importAppPath}");
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(importAppPath))
                    {
                        //LogPathCheck("ERROR: ImportFahrEmployeeData app NOT FOUND in any expected location!");
                        throw new FileNotFoundException(
                            "ImportFahrEmployeeData (.exe/.dll) not found.",
                            string.Join(" | ", possiblePaths)
                        );
                    }

                    if (!string.IsNullOrEmpty(importAppPath) && File.Exists(importAppPath))
                    {
                        string workingDirectory = Path.GetDirectoryName(importAppPath);
                        if (string.IsNullOrEmpty(workingDirectory))
                        {
                            workingDirectory = basePath; // Fallback to base path if directory name is null
                        }
                        
                        // Also check for appsettings.json in the same directory
                        string appSettingsPath = Path.Combine(workingDirectory, "appsettings.json");
                        if (!File.Exists(appSettingsPath))
                        {
                            // Try parent directory
                            string parentDir = Path.GetDirectoryName(workingDirectory);
                            if (!string.IsNullOrEmpty(parentDir))
                            {
                                string parentAppSettings = Path.Combine(parentDir, "appsettings.json");
                                if (File.Exists(parentAppSettings))
                                {
                                    workingDirectory = parentDir;
                                }
                            }
                        }
                        
                        bool isDll = string.Equals(Path.GetExtension(importAppPath), ".dll", StringComparison.OrdinalIgnoreCase);

                        ProcessStartInfo startInfo;
                        if (isDll)
                        {
                            startInfo = new ProcessStartInfo
                            {
                                FileName = "dotnet",
                                Arguments = $"\"{importAppPath}\"",
                                WorkingDirectory = workingDirectory,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            };
                        }
                        else
                        {
                            startInfo = new ProcessStartInfo
                            {
                                FileName = importAppPath,
                                WorkingDirectory = workingDirectory,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            };
                        }

                        Process process = Process.Start(startInfo);
                        if (process != null)
                        {
                            int processId = process.Id; // Store process ID early before it might be disposed
                            System.Diagnostics.Debug.WriteLine($"FAHR Import started successfully. Process ID: {processId}, App: {importAppPath}, Working Directory: {workingDirectory}");
                            
                            // Don't wait for exit - let it run in background
                            process.EnableRaisingEvents = true;
                            process.Exited += (sender, e) =>
                            {
                                _lastRunTime = DateTime.Now;
                                System.Diagnostics.Debug.WriteLine($"FAHR Import process exited. Process ID: {processId}");
                                try
                                {
                                    process?.Dispose();
                                }
                                catch { }
                            };
                            
                            // Set a timeout to avoid hanging
                            Task.Delay(300000).ContinueWith(t => // 5 minutes timeout
                            {
                                try
                                {
                                    // Try to kill using the original process object first
                                    bool killed = false;
                                    try
                                    {
                                        if (process != null && !process.HasExited)
                                        {
                                            process.Kill();
                                            killed = true;
                                            System.Diagnostics.Debug.WriteLine($"FAHR Import process timeout reached. Killed process ID: {processId}");
                                        }
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        // Process has been disposed, try to get it by ID
                                        try
                                        {
                                            using (var proc = Process.GetProcessById(processId))
                                            {
                                                if (proc != null && !proc.HasExited)
                                                {
                                                    proc.Kill();
                                                    killed = true;
                                                    System.Diagnostics.Debug.WriteLine($"FAHR Import process timeout reached. Killed process ID: {processId} (retrieved by ID)");
                                                }
                                            }
                                        }
                                        catch (ArgumentException)
                                        {
                                            // Process doesn't exist (already exited) - nothing to do
                                        }
                                        catch (InvalidOperationException)
                                        {
                                            // Process already exited - nothing to do
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"FAHR Import timeout handler error: {ex.Message}");
                                }
                            });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"FAHR Import process failed to start. App: {importAppPath}");
                        }
                    }
                    else
                    {
                        // Log error - executable not found
                        string errorMessage = $"FAHR Import executable not found. Base path: {basePath}, Searched paths: {string.Join("; ", possiblePaths)}";
                        System.Diagnostics.Debug.WriteLine(errorMessage);
                        
                        // Also try to log to a file if possible (for production debugging)
                        try
                        {
                            string logPath = Path.Combine(basePath, "FahrImportError.log");
                            File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {errorMessage}\r\n");
                        }
                        catch
                        {
                            // Ignore file logging errors
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FAHR Import Error: {ex.Message}");
                }
                finally
                {
                    lock (_lockObject)
                    {
                        _isRunning = false;
                    }
                }
            });
        }

        private static void LogPathCheck(string message)
        {
            try
            {
                string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImportFahrEmployeeData_pathlog.txt");
                File.AppendAllText(logFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            }
            catch
            {
                // ignore logging errors
            }
        }

        /// <summary>
        /// Gets the status of the import process
        /// </summary>
        public static ImportStatus GetStatus()
        {
            lock (_lockObject)
            {
                return new ImportStatus
                {
                    IsRunning = _isRunning,
                    LastRunTime = _lastRunTime
                };
            }
        }
    }

    /// <summary>
    /// Status information about the import process
    /// </summary>
    public class ImportStatus
    {
        public bool IsRunning { get; set; }
        public DateTime? LastRunTime { get; set; }
    }
}

