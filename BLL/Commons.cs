using DLL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    public class DataFiles
    {
        private string ManualAttendanceStride;

        public string getAccessCodeValue()
        {
            string strValue = "5";

            DLL.Models.AccessCodeValue fun = new DLL.Models.AccessCodeValue();
            using (Context db = new Context())
            {
                fun = db.access_code_value.Where(c => c.AccessCode == "MANUAL_ATTENDANCE_STRIDE").FirstOrDefault();
                if (fun != null)
                {
                    strValue = fun.AccessValue;
                }
            }

            return strValue;
        }

        public string getAccessCodeDate()
        {
            string strValue = DateTime.Now.AddDays(-35).ToString("dd-MM-yyyy");

            DLL.Models.AccessCodeValue fun = new DLL.Models.AccessCodeValue();
            using (Context db = new Context())
            {
                fun = db.access_code_value.Where(c => c.AccessCode == "EFFECTIVE_DATE").FirstOrDefault();
                if (fun != null)
                {
                    strValue = fun.AccessValue;
                }
            }

            return strValue;
        }

        public bool updateAccessCodeValue(string value)
        {
            bool response = false;

            //DLL.Models.AccessCodeValue fun2 = new DLL.Models.AccessCodeValue();
            //AccessCodeValue ss = new AccessCodeValue();
            int temp;
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out temp))
            {
                if (temp >= 1 && temp <= 28)
                {
                    using (Context db = new Context())
                    {
                        var ss = db.access_code_value.Where(x => x.AccessCode == "MANUAL_ATTENDANCE_STRIDE").FirstOrDefault();
                        if (ss != null)
                        {
                            ss.AccessValue = value;
                            db.SaveChanges();
                        }
                    }

                    response = true;
                }
                else
                {
                    response = false;
                }
            }
            else
            {
                response = false;
            }

            return response;
        }

        public bool updateAccessCodeDate(string value)
        {
            bool response = false;

            //DLL.Models.AccessCodeValue fun2 = new DLL.Models.AccessCodeValue();
            //AccessCodeValue ss = new AccessCodeValue();
            if (value.Length == 10)
            {
                using (Context db = new Context())
                {
                    var ss = db.access_code_value.Where(x => x.AccessCode == "EFFECTIVE_DATE").FirstOrDefault();
                    if (ss != null)
                    {
                        ss.AccessValue = value;
                        db.SaveChanges();
                    }
                }

                response = true;
            }
            else
            {
                response = false;
            }

            return response;
        }


        public DataFiles()
        {
            ManualAttendanceStride = HttpContext.Current.Server.MapPath("~/ManualAttendance.dat");
        }

        // this method will never write a non integer to the file
        public void updateManualAttendanceStride(string value)
        {
            int temp;

            // if the string is not empty and is a number fit for the purpose.
            // only then write it down, else quitely pass.
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out temp))
            {
                if (temp >= 1 && temp <= 31)
                    System.IO.File.WriteAllText(ManualAttendanceStride, value);



            }
        }

        public int getManualAttendanceStride()
        {
            // if the file does not exist, simply write a "5"
            // to a new file and exit. "5" is the default value for
            // the stride this means.
            if (!System.IO.File.Exists(ManualAttendanceStride))
            {
                System.IO.File.WriteAllText(ManualAttendanceStride, "5");
            }

            try
            {
                // read from the file
                string value = System.IO.File.ReadAllText(ManualAttendanceStride);

                return int.Parse(value);
            }
            catch
            {
                // For whatever reason if the program fails to return a proper integer

                // :p
                return 5;
            }

        }

        public string getAccessCodeInfoByIdentity(string Identity)
        {
            string strValue = string.Empty;
            DLL.Models.AccessCodeValue access_code = new DLL.Models.AccessCodeValue();

            using (Context db = new Context())
            {
                access_code = db.access_code_value.Where(c => c.AccessCode.ToUpper() == Identity).FirstOrDefault();
                if (access_code != null)
                {
                    strValue = access_code.AccessValue;
                }
            }

            return strValue;
        }


        public string getLeaveTypeTitleById(int id)
        {
            string strValue = string.Empty;
            DLL.Models.LeaveType leave_type = new DLL.Models.LeaveType();

            using (Context db = new Context())
            {
                leave_type = db.leave_type.Where(c => c.Id == id).FirstOrDefault();
                if (leave_type != null)
                {
                    strValue = leave_type.LeaveTypeText;
                }
            }

            return strValue;
        }


        public bool updateSmtpDetails(string value, string access_code)
        {
            bool response = false;

            //DLL.Models.AccessCodeValue fun2 = new DLL.Models.AccessCodeValue();
            //AccessCodeValue ss = new AccessCodeValue();
            if (value.Length != 0)
            {
                using (Context db = new Context())
                {
                    var ss = db.access_code_value.Where(x => x.AccessCode == access_code.ToUpper()).FirstOrDefault();
                    if (ss != null)
                    {
                        ss.AccessValue = value;
                        db.SaveChanges();
                    }
                }
                response = true;
            }
            else
            {
                response = false;
            }
            return response;
        }

        public bool updateLeaveTitleById(int id, string title)
        {
            bool response = false;

            if (id > 0)
            {
                using (Context db = new Context())
                {
                    var lt = db.leave_type.Where(x => x.Id == id).FirstOrDefault();
                    if (lt != null)
                    {
                        lt.LeaveTypeText = title;
                        db.SaveChanges();
                    }
                }

                response = true;
            }
            else
            {
                response = false;
            }

            return response;
        }

        public class ConsolidatedCounter
        {
            public long CCount { get; set; }
        }

        public long ArchiveTablesByDateRange(string fromdate, string todate)
        {
            long counter = 0;

            if (fromdate.Length != 0 && todate.Length != 0)
            {
                try
                {
                    using (Context db = new Context())
                    {
                        var Fromdate = new SqlParameter("@FromDate", fromdate);
                        var Todate = new SqlParameter("@ToDate", todate);

                        List<ConsolidatedCounter> data = new List<ConsolidatedCounter>();
                        data = db.Database.SqlQuery<ConsolidatedCounter>("SP_ArchiveTables @FromDate, @ToDate", Fromdate, Todate).ToList();
                        counter = data[0].CCount;

                        //db.Database.ExecuteSqlCommand("SP_ArchiveTables @FromDate, @ToDate", Fromdate, Todate);
                    }
                }
                catch (Exception ex)
                {
                    counter = -1;
                }
            }
            else
            {
                counter = -2;
            }

            return counter;
        }

    }
}
