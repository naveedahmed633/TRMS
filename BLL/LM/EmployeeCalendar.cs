using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Web;
using System.Globalization;

namespace TimeTune
{
    public class EmployeeCalendar
    {
        int currentGroup;
        public EmployeeCalendar()
        {
            string str = HttpContext.Current.User.Identity.Name;

            using (var db = new Context())
            {

                try
                {
                    // get the employee entity for the currently logged in user
                    Employee lineManager = db.employee.Where(m => m.active && m.employee_code.Equals(str)).FirstOrDefault();


                    Group group = db.group.Single(m => m.supervisor_id.Equals(lineManager.EmployeeId));
                    this.currentGroup = group.GroupId;
                }
                catch (System.NullReferenceException)
                {
                    this.currentGroup = -1;
                }
                catch
                {
                    this.currentGroup = -1;
                }
            }
        }

        /**
         * Returns a list of employees for the currently logged in line manager.
         * 
         */
        public List<ViewModels.Employee> getAllGroupEmployees()
        {
            List<ViewModels.Employee> toReturn = new List<ViewModels.Employee>();

            using (var db = new Context())
            {


                Group grp = db.group.Find(this.currentGroup);

                if (grp != null)
                {
                    // get all employees, even the line manager.
                    List<Employee> employees = grp.Employees.ToList();

                    // convert each model to view model and add to the result list.
                    foreach (Employee emp in employees)
                        toReturn.Add(TimeTune.EmployeeCrud.convert(emp));

                }

            }

            return toReturn;
        }

        public List<ViewModels.Employee> getAllGroupEmployees(int employee_id)
        {
            List<ViewModels.Employee> toReturn = new List<ViewModels.Employee>();
            using (var db = new Context())
            {
                var grp = db.group.Where(c => c.supervisor_id == employee_id).FirstOrDefault();
                if (grp != null)
                {
                    // get all employees, even the line manager.
                    List<Employee> employees = grp.Employees.ToList();

                    // convert each model to view model and add to the result list.
                    foreach (Employee emp in employees)
                        toReturn.Add(TimeTune.EmployeeCrud.convert(emp));
                }

            }
            return toReturn;
        }




        public Shift getShift(dynamic id, Context db)
        {
            // It is important for us to pass the
            // Context object, because if we use a
            // new context in here, and the objects that we
            // return are used for assignments to 'public virtual Shift'
            // fields, then the context used for doing that
            // will treat these objects as new and insert them
            // into the database as new shifts.

            // an exception of the type null pointer
            // might be thrown when finding the shift from the group shifts.
            // return null if that happens.
            try
            {
                Group grp = db.group.Where(m => m.GroupId.Equals(this.currentGroup) && m.active).FirstOrDefault();
                if (grp == null)
                    return null;


                Shift shift = grp.Shifts.Where(m => m.ShiftId.Equals(id) && m.active).FirstOrDefault();


                Shift toReturn = (id == null) ? null : // return null if null

                        (id == -1) ? // if -1 return holiday

                        // HOLIDAY is a special shift with 
                        // active = false, star_time as 13:00
                        // and all of the other fields are 0
                        // which is not allowed for any shifts
                        // except a holiday shift. Also note that
                        // the id for the holiday shift is not
                        // -1, infact -1 is simply a flag that the
                        // front end uses to tell us that holiday needs
                        // to be assigned over here.
                        db.shift.Where(m =>
                            m.active.Equals(false) &&
                            m.early_time.Equals(0) &&
                            m.day_end.Equals(0)).FirstOrDefault() :

                        shift; // else return shift

                return toReturn;
            }
            catch
            {
                return null;
            }
        }

        public void replication_emptoemp(int empfrom, int empto, DateTime start, DateTime end)
        {
            using (var db = new Context())
            {
                var shiftfromemp = db.manual_group_shift_assigned.Where(m => m.active &&
                  m.Group.GroupId.Equals(this.currentGroup) &&
                  m.Employee.EmployeeId.Equals(empfrom) &&
                  m.date >= start && m.date <= end).ToList();
                var entry = db.manual_group_shift_assigned.Where(

                        m =>
                            m.Group.GroupId.Equals(this.currentGroup) &&
                            m.Employee.EmployeeId.Equals(empto) &&
                           m.date >= start && m.date <= end &&
                            m.active
                        ).ToList();
                if (entry.Count > 0)
                {
                    for (int i = 0; i < entry.Count; i++)
                    {
                        int aa = entry[i].ManualGroupShiftAssignedId;
                        var dltentry = db.manual_group_shift_assigned.Where(a => a.ManualGroupShiftAssignedId == aa).Single();
                        db.manual_group_shift_assigned.Remove(dltentry);
                        db.SaveChanges();
                    }
                }
                for (int i = 0; i < shiftfromemp.Count; i++)
                {            
                        db.manual_group_shift_assigned.Add(new ManualGroupShiftAssigned()
                        {

                            Group = shiftfromemp[i].Group,
                            Employee = db.employee.Find(empto),
                            date = shiftfromemp[i].date,
                            active = true,

                            Shift = shiftfromemp[i].Shift,
                            reason = shiftfromemp[i].reason

                        });
                    db.SaveChanges();
                }
            }
        }
        public void addOrUpdateCalendar(ViewModels.EmployeeCalendar toUpdate)
        {

            using (var db = new Context())
            {
                int empId;
                if (toUpdate.overrides != null &&
                    toUpdate.employee_id != null &&
                    !toUpdate.employee_id.Equals("") &&
                    int.TryParse(toUpdate.employee_id, out empId))
                {

                    foreach (ViewModels.GeneralCalendarOverride ovr in toUpdate.overrides)
                    {


                        // get date
                        DateTime date = DateTime.ParseExact(ovr.date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        // don't process if something goes wrong.
                        if (date == null)
                            continue;

                        // get shift
                        int shiftId;
                        if (!int.TryParse(ovr.shift, out shiftId))
                            continue;
                        Shift shift = getShift(shiftId, db);
                        if (shift == null)
                            continue;








                        ManualGroupShiftAssigned entry = db.manual_group_shift_assigned.Where(

                            m =>
                                m.Group.GroupId.Equals(this.currentGroup) &&
                                m.Employee.EmployeeId.Equals(empId) &&
                                m.date.Value.Equals(date) &&
                                m.active
                            ).FirstOrDefault();

                        // if no such entry found, create a new one.
                        // else modify existing one
                        if (entry == null)
                        {
                            // none means no override
                            if (ovr.type.Equals("none"))
                            {
                                continue;
                            }

                            db.manual_group_shift_assigned.Add(new ManualGroupShiftAssigned()
                            {

                                Group = db.group.Find(this.currentGroup),
                                Employee = db.employee.Find(empId),
                                date = date,
                                active = true,

                                Shift = shift,
                                reason = ovr.reason

                            });
                        }
                        else
                        {
                            // none means no override
                            if (ovr.type.Equals("none"))
                            {
                                entry.active = false;
                                continue;
                            }



                            entry.active = true;
                            entry.Shift = shift;
                            entry.reason = ovr.reason;

                        }



                    }

                    db.SaveChanges();


                }

            }

        }


        public dynamic getEmployeeCalendar(string employee_id, string year)
        {
            dynamic obj = new
            {
                success = false,
                data = new[] {new{}
                    /*
                    new {date = "03/09/2016" , reason = "There has...", shift="-1",type="holiday"}, 
                    new {date = "03/10/2016" , reason = "", shift="3",type="shift"},
                    new {date = "03/11/2017" , reason = "", shift="3",type="shift"},
                    */
                }
            };

            int yer;
            if (!int.TryParse(year, out yer))
            {
                return obj;
            }

            int id;
            if (!int.TryParse(employee_id, out id))
            {
                return obj;
            }



            using (var db = new Context())
            {
                ManualGroupShiftAssigned[] manualAssignedShifts =
                    db.manual_group_shift_assigned.Where(m =>
                        m.active &&
                        m.Employee.EmployeeId.Equals(id) &&
                        m.Group.GroupId.Equals(this.currentGroup) &&
                        m.date.Value.Year.Equals(yer)).ToArray();

                List<dynamic> data = new List<dynamic>();

                foreach (ManualGroupShiftAssigned t in manualAssignedShifts)
                {

                    string sh = (
                        t.Shift.half_day.Equals(0) &&
                        t.Shift.late_time.Equals(0) &&
                        t.Shift.shift_end.Equals(0) &&
                        t.Shift.day_end.Equals(0)) ? "-1" : t.Shift.ShiftId + "";

                    string typ = "shift";
                    if (sh.Equals("-1"))
                    {
                        typ = "holiday";
                    }

                    data.Add(new
                    {
                        date = t.date.Value.Date.ToString("MM/dd/yyyy"),
                        reason = t.reason,
                        shift = sh,
                        type = typ
                    });
                }


                obj = new
                {
                    success = true,
                    data = data.ToArray()
                };

                return obj;



            }

        }
    }
}
