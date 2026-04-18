using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTune
{
    public class DeleteHaTransit
    {
        public static void deleteRawAttendance(string employee_code, string date)
        {
            using (var db = new Context())
            {
                Employee emp = db.employee.Where(m =>
                    m.employee_code.Equals(employee_code)).FirstOrDefault();

                if (emp == null)
                {
                    return;
                }

                DateTime dt;
                try
                {
                    dt = DateTime.Parse(date);
                }
                catch
                {
                    return;
                }


                HaTransit[] logsToDelete = db.ha_transit.Where(m =>
                    m.C_Unique.Equals(emp.employee_code) &&
                    m.C_Date.Value.Equals(dt)).ToArray();

                db.ha_transit.RemoveRange(logsToDelete);

                db.SaveChanges();
            }
        }

        public static void deleteConsolidate(string employee_code, string date)
        {
            using (var db = new Context())
            {
                Employee emp = db.employee.Where(m =>
                    m.employee_code.Equals(employee_code)).FirstOrDefault();

                if (emp == null)
                {
                    return;
                }

                DateTime dt;
                try
                {
                    dt = DateTime.Parse(date);
                }
                catch
                {
                    return;
                }


                ConsolidatedAttendance[] logsToDelete = db.consolidated_attendance.Where(m =>
                    m.employee.employee_code.Equals(emp.employee_code) &&
                    m.date.Value.Equals(dt)).ToArray();

                db.consolidated_attendance.RemoveRange(logsToDelete);

                db.SaveChanges();
            }
        }
    }
}
