using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync_BAT_File_Text_Change_Job
{
    class Program
    {
        static void Main(string[] args)
        {
            string isUpdateAllowed = "";

            try
            {
                isUpdateAllowed = GetAppConfigValue("UpdateAllowed");
                if (isUpdateAllowed == "1")
                {
                    updateConfigFile();
                }

                System.Threading.Thread.Sleep(1000);
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void updateConfigFile()
        {
            string check_folder_file_name = "";

            check_folder_file_name = GetAppConfigValue("UpdateFolderFilePath");
            if (check_folder_file_name != "")
            {
                UpdateAFile(check_folder_file_name);
            }
        }


        public static void UpdateAFile(string strFolderFilePath)
        {
            string config_file_text = "";
            string strSource = "", strDestination = "";

            /*
                 {
                    "host" : "10.200.65.128",
                    "port" : "22",
                    "username" : "oebs",
                    "password" : "EBS@ora1",
                    "files":
                        [
                            "C:/TimeTuneOUT/RescoMachineData.csv",
                        ],
                    "output_directory" : "/IN"
                }
            */

            config_file_text = "@echo off\r\n" +
                                    "\t rem nothing\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(-2).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(-2).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(0).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(0).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(1).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(1).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(2).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(2).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(3).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(3).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(4).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(4).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(5).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(5).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(6).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(6).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(7).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(7).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(8).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(8).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(9).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(9).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(10).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(10).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(11).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(11).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(12).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(12).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(13).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(13).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(14).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(14).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(15).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(15).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(16).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(16).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(17).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(17).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(18).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(18).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(19).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(19).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t xcopy \"AAAA\\" + DateTime.Now.AddDays(20).ToString("yyyyMMdd") + "\\*.jpg\" \"BBBB\\" + DateTime.Now.AddDays(20).ToString("yyyyMMdd") + "\\\" /C /D /Y\r\n" +
                                    "\t rem nothing\r\n" +
                                "exit";

            strSource = GetAppConfigValue("SourceServerPath");
            config_file_text = config_file_text.Replace("AAAA", strSource);

            strDestination = GetAppConfigValue("DestinationServerPath");
            config_file_text = config_file_text.Replace("BBBB", strDestination);

            try
            {
                System.IO.File.WriteAllText(strFolderFilePath, config_file_text);

                Console.WriteLine("File-" + " (" + strFolderFilePath + ") is updated successfully!!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("File- " + " (" + strFolderFilePath + ") has error cccurred. Message: " + ex.Message);
            }
        }

        public static string GetAppConfigValue(string strAppSetting)
        {
            string strReturn = "";

            if (ConfigurationManager.AppSettings[strAppSetting] != null && ConfigurationManager.AppSettings[strAppSetting].ToString() != "")
            {
                strReturn = ConfigurationManager.AppSettings[strAppSetting];
            }

            return strReturn;
        }
    }
}
