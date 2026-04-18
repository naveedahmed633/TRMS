using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTune
{
    public class ReprocessAttendanceImportExport
    {
        public class ReprocessCsv
        {
            public string employee_code { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        private static bool convertStringToBool(string value)
        {
            return (value == "True" || value == "true" || value == "TRUE");
        }
        public static string reprocessAttendance(List<string> csvContents, string user_code)
        {
            try
            {
                using (var db = new Context())
                {
                    foreach (string line in csvContents)
                    {
                        // split the line
                        string[] values = line.Split(',');

                        // check if the split resulted in at least 3 values (columns).

                        if (values == null || values.Length < 3)
                            continue;

                        // Remove unwanted " characters.
                        string employeeCode = values[0].Replace("\"", "");
                        values[1] = values[1].Replace("\"", "");
                        values[2] = values[2].Replace("\"", "");


                        // continue if its the first row (CSV Header line).
                        if (employeeCode == "employeeCode" || employeeCode == "")
                        {
                            continue;
                        }
                        var emp = db.employee.Where(c => c.employee_code == employeeCode && c.active==true).FirstOrDefault();
                        //for (int i = 0; i < 18; i++)
                        //{
                        //Update Employee
                        if (emp != null)
                        {
                            BLL.MarkPreviousAttendance.createOrUpdateAttendance(values[1], values[2], emp.EmployeeId, user_code);
                        }
                        else
                        {
                            continue;
                        }

                    }



                    return "Successfull";
                }


            }

            catch (Exception)
            {
                return "failed";
            }
        }
    }
}
