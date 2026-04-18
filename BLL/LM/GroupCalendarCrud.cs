using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using DLL.Models;
using System.Globalization;
using DLL;

namespace TimeTune
{
    public class GroupCalendarCrud
    {
        int currentGroup;

        public GroupCalendarCrud()
        {
            string str = HttpContext.Current.User.Identity.Name;
            
            using( var db = new Context()) {

                
                try
                {
                    Employee lineManager = db.employee.Where(m => m.active && m.employee_code.Equals(str)).FirstOrDefault();
                    Group group = db.group.Single(m => m.supervisor_id.Equals(lineManager.EmployeeId));
                    this.currentGroup = group.GroupId;

                } catch(System.NullReferenceException ex) {
                    this.currentGroup = -1;
                }
                catch
                {
                    this.currentGroup = -1;
                }
                
            }
            

        }
        //Parametrized Group Calendar
        public GroupCalendarCrud(int group_id)
        {
            string str = HttpContext.Current.User.Identity.Name;

            using (var db = new Context())
            {


                try
                {
                    //Employee lineManager = db.employee.Where(m => m.active && m.employee_code.Equals(str)).FirstOrDefault();
                    Group group = db.group.Single(m => m.GroupId==group_id);
                    this.currentGroup = group.GroupId;

                }
                catch (System.NullReferenceException ex)
                {
                    this.currentGroup = -1;
                }
                catch
                {
                    this.currentGroup = -1;
                }

            }


        }

        //End here
        public ViewModels.GroupCalendar convert(GroupCalendar model)
        {
            if (model == null)
            {

                return new ViewModels.GroupCalendar()
                {
                    isGeneralCalendar = false,
                    id = 1,
                    year = 1,
                    Shift = null,
                    Shift1 = null,
                    Shift2 = null,
                    Shift3 = null,
                    Shift4 = null,
                    Shift5 = null,
                    Shift6 = null,
                    generalShift = null,
                    generalOverrides = new ViewModels.GroupCalendarOverride[0]
                };

            }

            // pick only the active logs.
            GroupCalendarOverride[] generalOverrides = model.calendarOverrides.Where(m => m.active).ToArray();

            ViewModels.GroupCalendarOverride[] generalOverridesViewModel = new ViewModels.GroupCalendarOverride[generalOverrides.Length];

            for (int i = 0; i < generalOverridesViewModel.Length; i++)
            {
                string _date = generalOverrides[i].date.Value.ToString("MM/dd/yyyy");
                string _reason = generalOverrides[i].reason;
                string _shift = (generalOverrides[i].Shift == null ||(generalOverrides[i].Shift.shift_end == 0 && generalOverrides[i].Shift.early_time == 0)) ? "-1" : generalOverrides[i].Shift.ShiftId + "";
                string _type = (generalOverrides[i].Shift == null || (generalOverrides[i].Shift.shift_end == 0 && generalOverrides[i].Shift.early_time == 0)) ? (generalOverrides[i].isGazettedHoliday) ? "gazetted" : "holiday" : "shift";

                generalOverridesViewModel[i] = new ViewModels.GroupCalendarOverride()
                {
                    date = _date,
                    reason = _reason,
                    shift = _shift,
                    type = _type
                };

            }




            return new ViewModels.GroupCalendar()
            {
                isGeneralCalendar = false,
                id = model.GroupCalendarId,
                year = model.year,
                Shift = EmployeeShift.convert(model.sunday),
                Shift1 = EmployeeShift.convert(model.monday),
                Shift2 = EmployeeShift.convert(model.tuesday),
                Shift3 = EmployeeShift.convert(model.wednesday),
                Shift4 = EmployeeShift.convert(model.thursday),
                Shift5 = EmployeeShift.convert(model.friday),
                Shift6 = EmployeeShift.convert(model.saturday),
                generalShift = EmployeeShift.convert(model.generalShift),
                generalOverrides = generalOverridesViewModel
            };
        }

        public ViewModels.GroupCalendar convert(GeneralCalendar model)
        {
            if (model == null)
            {

                return new ViewModels.GroupCalendar()
                {
                    isGeneralCalendar = true,
                    id = 1,
                    year = 1,
                    Shift = null,
                    Shift1 = null,
                    Shift2 = null,
                    Shift3 = null,
                    Shift4 = null,
                    Shift5 = null,
                    Shift6 = null,
                    generalShift = null,
                    generalOverrides = new ViewModels.GroupCalendarOverride[0]
                };

            }

            // pick only the active logs.
            GeneralCalendarOverride[] generalOverrides = model.calendarOverrides.Where(m => m.active).ToArray();

            ViewModels.GroupCalendarOverride[] generalOverridesViewModel = new ViewModels.GroupCalendarOverride[generalOverrides.Length];

            for (int i = 0; i < generalOverridesViewModel.Length; i++)
            {
                string _date = generalOverrides[i].date.Value.ToString("MM/dd/yyyy");
                string _reason = generalOverrides[i].reason;
                string _shift = (generalOverrides[i].Shift == null || (generalOverrides[i].Shift.shift_end == 0 && generalOverrides[i].Shift.early_time == 0)) ? "-1" : generalOverrides[i].Shift.ShiftId + "";
                string _type = (generalOverrides[i].Shift == null || (generalOverrides[i].Shift.shift_end == 0 && generalOverrides[i].Shift.early_time == 0)) ? (generalOverrides[i].isGazettedHoliday) ? "gazetted" : "holiday" : "shift";

                generalOverridesViewModel[i] = new ViewModels.GroupCalendarOverride()
                {
                    date = _date,
                    reason = _reason,
                    shift = _shift,
                    type = _type
                };
            }




            return new ViewModels.GroupCalendar()
            {
                isGeneralCalendar = true,
                id = model.GeneralCalendarId,
                year = model.year,
                Shift = EmployeeShift.convert(model.Shift),
                Shift1 = EmployeeShift.convert(model.Shift1),
                Shift2 = EmployeeShift.convert(model.Shift2),
                Shift3 = EmployeeShift.convert(model.Shift3),
                Shift4 = EmployeeShift.convert(model.Shift4),
                Shift5 = EmployeeShift.convert(model.Shift5),
                Shift6 = EmployeeShift.convert(model.Shift6),
                generalShift = EmployeeShift.convert(model.generalShift),
                generalOverrides = generalOverridesViewModel
            };
        }


        public Shift getShift(dynamic id, Context db)
        {
            if (this.currentGroup == -1)
                return null;

            // It is important for us to pass the
            // Context object, because if we use a
            // new context in here, and the objects that we
            // return are used for assignments to 'public virtual Shift'
            // fields, then the context used for doing that
            // will treat these objects as new and insert them
            // into the database as new shifts.


            Shift toReturn = (id == null) ? null : // return null if null

                    (id == -1) ? // if -1 return holiday

                    // HOLIDAY is a special shift with 
                // active = false, star_time as 13:00
                // and all of the other fields are 0
                // which is not allowed for any shifts
                // except a holiday shift. Also note that
                // the id for the holiday shift is not
                // -1, infact -1 is simply a flag that the
                // front end uses to tell us that a holiday needs
                // to be assigned over here.
                    db.shift.Where(m =>
                        m.active.Equals(false) &&
                        m.early_time.Equals(0) &&
                        m.day_end.Equals(0)).FirstOrDefault():

                    db.group.Find(currentGroup).Shifts.Where(m=>m.active && m.ShiftId.Equals(id)).FirstOrDefault(); // else return shift

            return toReturn;
        }

        public List<ViewModels.Shift> getAllShifts()
        {
            if(this.currentGroup == -1)
                return new List<ViewModels.Shift>();

            using (var db = new Context())
            {
                Group grp = db.group.Find(this.currentGroup);
                List<ViewModels.Shift> toReturn = null;

                if (grp.follows_general_calendar)
                {
                    List<Shift> shifts = db.shift.Where(m=>m.active).ToList();

                    toReturn = new List<ViewModels.Shift>();

                    foreach (Shift shift in shifts)
                    {
                        toReturn.Add(EmployeeShift.convert(shift));
                    }
                }
                else
                {
                    List<Shift> shifts = grp.Shifts.ToList();

                    toReturn = new List<ViewModels.Shift>();

                    foreach (Shift shift in shifts)
                    {
                        toReturn.Add(EmployeeShift.convert(shift));
                    }
                }


                

                return toReturn;
            }
        }



        public List<ViewModels.Shift> getEmployeeShifts()
        {
            if (this.currentGroup == -1)
                return new List<ViewModels.Shift>();

            using (var db = new Context())
            {
                Group grp = db.group.Find(this.currentGroup);
                List<ViewModels.Shift> toReturn = null;

                if (grp.follows_general_calendar)
                {
                    List<Shift> shifts = db.shift.Where(m => m.name=="Regular" || m.name=="Friday").ToList();

                    toReturn = new List<ViewModels.Shift>();

                    foreach (Shift shift in shifts)
                    {
                        toReturn.Add(EmployeeShift.convert(shift));
                    }
                }
                else
                {
                    List<Shift> shifts = grp.Shifts.ToList();

                    toReturn = new List<ViewModels.Shift>();

                    foreach (Shift shift in shifts)
                    {
                        toReturn.Add(EmployeeShift.convert(shift));
                    }
                }




                return toReturn;
            }
        }
        public ViewModels.GroupCalendar getCalendarForYear(int year)
        {
            if (this.currentGroup == -1)
                return null;

            using (Context db = new Context())
            {
                Group grp = db.group.Find(this.currentGroup);

                if (grp.follows_general_calendar)
                {
                    GeneralCalendar gc = db.general_calender.Where(m => m.active && m.year.Equals(year)).FirstOrDefault();
                    return convert(gc);
                }
                else
                {
                    GroupCalendar gc =
                    db.group_calendar.Where(m => m.active && m.year.Equals(year) && m.Group.GroupId.Equals(currentGroup)).FirstOrDefault();
                    return convert(gc);
                }

                
                    //db.general_calender.Where(m => m.active && m.year.Equals(year)).SingleOrDefault();

                

            }
        }

        public bool addOrUpdateCalendar(dynamic data)
        {
            if (this.currentGroup == -1)
                return false;

            using (Context db = new Context())
            {
                

                int year = data.year;
                //int general_shift = data.general_shift;
                DLL.Models.GroupCalendar gc = 
                    db.group_calendar.Where(m => m.active && m.year.Equals(year) && m.Group.GroupId.Equals(currentGroup)).FirstOrDefault();
                    //db.general_calender.Where(m => m.active && m.year.Equals(year)).SingleOrDefault();


                /*
                 * data.shift_days[0-6] week days.
                 * for holiday values are -1 (string)
                 * null for none,
                 * and a number for shifts.
                 */
                if (gc == null)
                {

                    DLL.Models.GroupCalendar cal = new DLL.Models.GroupCalendar()
                    {
                        year = year,
                        sunday = getShift(data.shift_days[0], db),
                        monday = getShift(data.shift_days[1], db),
                        tuesday = getShift(data.shift_days[2], db),
                        wednesday = getShift(data.shift_days[3], db),
                        thursday = getShift(data.shift_days[4], db),
                        friday = getShift(data.shift_days[5], db),
                        saturday = getShift(data.shift_days[6], db),
                        generalShift = getShift(data.general_shift, db),
                        Group = db.group.Find(this.currentGroup),
                        active = true,
                        calendarOverrides = new List<DLL.Models.GroupCalendarOverride>()
                    };








                    foreach (var item in data.general_overrides)
                    {


                        string date = item.date;
                        string type = item.type;


                        // ignore type nones
                        if (type.Equals("none"))
                            continue;

                        string reason = item.reason;
                        int shift = item.shift;


                        cal.calendarOverrides.Add(new GroupCalendarOverride()
                        {
                            active = true,
                            isGazettedHoliday = (type.Equals("gazetted")),
                            date = DateTime.Parse(date).Date,
                            reason = reason,
                            Shift = getShift(shift, db)
                        });



                    }


                    db.group_calendar.Add(cal);



                    db.SaveChanges();
                    return true;
                }
                else
                {
                    // Because our shift attributes are specified
                    // using the 'pulic virtual' specifier their models are loaded
                    // only when requested. So we will request 
                    // for all of the shift attributes first,
                    // and then assign values to them. Else
                    // directly assigning does not have an effect
                    var forceLoad = gc.sunday;
                    forceLoad = gc.monday;
                    forceLoad = gc.tuesday;
                    forceLoad = gc.wednesday;
                    forceLoad = gc.thursday;
                    forceLoad = gc.friday;
                    forceLoad = gc.saturday;
                    forceLoad = gc.generalShift;
                    var forceLoad2 = gc.calendarOverrides;

                    // assign values now.
                    gc.year = year;
                    gc.sunday = getShift(data.shift_days[0], db);
                    gc.monday = getShift(data.shift_days[1], db);
                    gc.tuesday = getShift(data.shift_days[2], db);
                    gc.wednesday = getShift(data.shift_days[3], db);
                    gc.thursday = getShift(data.shift_days[4], db);
                    gc.friday = getShift(data.shift_days[5], db);
                    gc.saturday = getShift(data.shift_days[6], db);
                    gc.generalShift = getShift(data.general_shift, db);




                    foreach (var item in data.general_overrides)
                    {


                        string date = item.date;
                        string type = item.type;
                        string reason = item.reason;
                        int shift = int.Parse(item.shift + "");




                        // Search for an already existing override in the gc
                        // for this date.

                        var calendarOverride = gc.calendarOverrides.Where(m =>

                            m.date.Value.Date.Equals(
                                DateTime.Parse(date).Date
                            )


                            ).FirstOrDefault();


                        // The override already exists
                        if (calendarOverride != null)
                        {

                            calendarOverride.active = (!type.Equals("none"));
                            calendarOverride.isGazettedHoliday = (type.Equals("gazetted"));

                            calendarOverride.reason = reason;
                            calendarOverride.Shift = getShift(shift, db);


                        }
                        else // The override does not exist.
                        {
                            // ignore nones.
                            if (type.Equals("none"))
                                continue;


                            bool isGazetted = (type.Equals("gazetted"));
                            DateTime dateToAssign = DateTime.Parse(date).Date;
                            Shift shiftToAssign = getShift(shift, db);

                            gc.calendarOverrides.Add(new GroupCalendarOverride()
                            {
                                active = true,
                                isGazettedHoliday = isGazetted,
                                date = dateToAssign,
                                reason = reason,
                                Shift = shiftToAssign
                            });



                        }



                    }



                    db.SaveChanges();

                    return true;
                }


            }
        }
    }
}