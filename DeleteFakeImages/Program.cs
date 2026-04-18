using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;

namespace DeleteFakeImages
{
    class Program
    {
        static void Main(string[] args)
        {
            string isDeleteAllowed = "";

            try
            {
                isDeleteAllowed = ValidateAppConfigValue("DeleteAllowed");
                if (isDeleteAllowed == "1")
                {
                    deleteImagesAtPath();
                }

                System.Threading.Thread.Sleep(100);

                Console.WriteLine("\n\nJob Completed Successfully!!!");

                //************ COMMENT ON PROD ********************/
                ////Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void deleteImagesAtPath()
        {
            string folder_path = "";

            folder_path = ValidateAppConfigValue("DeleteFolderPath");
            if (folder_path != "")
            {
                DeleteAllImages(folder_path);
            }
        }

        public static void DeleteAllImages(string strImagesPath)
        {
            string dirRegion = "";
            try
            {
                DateTime dtNow = DateTime.Now; DateTime dtFolder = DateTime.Now;

                //File.Delete(strFilePath);
                for (int i = 1; i <= dtNow.Day; i++)
                {
                    dtFolder = new DateTime(dtNow.Year, dtNow.Month, i);

                    /* FOR SINGLE FOLDER DELETION
                    int i =0;
                    dtFolder = dtNow.AddDays(i);
                    */

                    //R1
                    dirRegion = strImagesPath.Replace("RRR", "R1") + "\\" + dtFolder.ToString("yyyyMMdd");
                    var dirPathR1 = new DirectoryInfo(dirRegion);
                    if (dirPathR1.Exists)
                    {
                        List<FileInfo> listImagesR1 = dirPathR1.GetFiles("*_-0000001.jpg").ToList(); //dirPathR1.EnumerateFiles("*-00000001.jpg").ToList();
                        if (listImagesR1 != null && listImagesR1.Count > 0)
                        {
                            foreach (var file in listImagesR1)
                            {
                                try
                                {
                                    file.Attributes = FileAttributes.Normal;
                                    file.Delete();
                                    Console.WriteLine("\tR1 Folder - " + file.Name + " Image Deleted!");

                                    //break;
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("\tR1 Folder - Exception to DELETE");
                                }
                            }
                        }
                    }

                    dirRegion = "";
                    Console.WriteLine("R1 Folder - images at /" + dtFolder.ToString("yyyyMMdd") + "/ Scan Completed!");

                    //R2
                    dirRegion = strImagesPath.Replace("RRR", "R2") + "\\" + dtFolder.ToString("yyyyMMdd");
                    var dirPathR2 = new DirectoryInfo(dirRegion);
                    if (dirPathR2.Exists)
                    {
                        List<FileInfo> listImagesR2 = dirPathR2.GetFiles("*_-0000001.jpg").ToList(); //List<FileInfo> listImagesR2 = dirPathR2.EnumerateFiles("*-00000001.jpg").ToList();
                        if (listImagesR2 != null && listImagesR2.Count > 0)
                        {
                            foreach (var file in listImagesR2)
                            {
                                try
                                {
                                    file.Attributes = FileAttributes.Normal;
                                    file.Delete();
                                    Console.WriteLine("\tR2 Folder - " + file.Name + " Image Deleted!");
                                    //break;
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("\tR2 Folder - Exception to DELETE");
                                }
                            }
                        }
                    }

                    Console.WriteLine("R2 Folder - images at /" + dtFolder.ToString("yyyyMMdd") + "/ Scan Completed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("File - (" + strImagesPath + ") has error cccurred. Message: " + ex.Message);
            }
        }

        public static string ValidateAppConfigValue(string fName)
        {
            string strReturn = "";

            if (ConfigurationManager.AppSettings[fName] != null && ConfigurationManager.AppSettings[fName].ToString() != "")
            {
                strReturn = ConfigurationManager.AppSettings[fName];
            }

            return strReturn;
        }


    }
}
