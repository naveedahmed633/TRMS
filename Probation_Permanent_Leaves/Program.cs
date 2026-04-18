using System;
using DLL.Models;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Configuration;

namespace Probation_Permanent_Leaves
{
    public class Program
    {
        static void Main(string[] args)
        {
            int response = 0, back_months = 0, back_days = 0;
            string isStartupAllowed = "";           
            Program p = new Program();
            DateTime dtJobStartTime = DateTime.Now; DateTime dtJobEndTime = DateTime.Now;
            DateTime dtJoinDate = DateTime.Now;

            try
            {
                //Console.WriteLine("Job is started...");
                p.WriteToLog("Job is started...");

                isStartupAllowed = p.ValidateAppConfigValue("StartupAllowed");
                if (isStartupAllowed == "1")
                {
                    back_months = int.Parse(p.ValidateAppConfigValue("ProbationMonthsBack"));
                    back_days = int.Parse(p.ValidateAppConfigValue("RunJobDaysBack"));

                    /////////////////////////////////////

                    for (int i = back_days; i >= 0; i--)
                    {
                        dtJoinDate = new DateTime(DateTime.Now.AddMonths(back_months).Year, DateTime.Now.AddMonths(back_months).Month, DateTime.Now.AddDays(-1 * i).Day);

                        p.WriteToLog("[STARTED] - JOIN Date: " + dtJoinDate.ToString("dd-MMM-yyyy"));

                        response = p.SetProbationaryEmployeesToPermanent(dtJoinDate);

                        if (response >= 0)
                        {
                            p.WriteToLog("[ENDED] - JOIN Date: " + dtJoinDate.ToString("dd-MMM-yyyy") + " - Updated: " + response.ToString());
                        }                        
                    }

                    ////////////////////////////////////

                    dtJobEndTime = DateTime.Now;

                    TimeSpan ts = dtJobEndTime - dtJobStartTime;
                    p.WriteToLog("Job is ended successfully in " + ts.Minutes + " min " + ts.Seconds + " sec");
                }
                else
                {
                    p.WriteToLog("Job execution is disabled");
                }

                //************ COMMENT ON PROD ********************/
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SetProbationaryEmployeesToPermanent(DateTime dtJoiningDate)
        {
            int response_count = 0;

            try
            {
                using (Context db = new Context())
                {
                    //get Probationary Employees joined xx Months back
                    var dbProbEmployees = db.employee.Where(e => e.active && e.type_of_employment.TypeOfEmploymentId == 2 && e.date_of_joining == dtJoiningDate).ToList();
                    if (dbProbEmployees != null && dbProbEmployees.Count > 0)
                    {
                        foreach (var emp in dbProbEmployees)
                        {
                            var dbLeavesSession = db.leave_session.Where(l => l.EmployeeId == emp.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();
                            if (dbLeavesSession != null)
                            {
                                //main four types
                                if (dbLeavesSession.SickLeaves == 0 && emp.sick_leaves > 0)
                                {
                                    dbLeavesSession.SickLeaves = emp.sick_leaves;
                                }

                                if (dbLeavesSession.CasualLeaves == 0 && emp.casual_leaves > 0)
                                {
                                    dbLeavesSession.CasualLeaves = emp.casual_leaves;
                                }

                                if (dbLeavesSession.AnnualLeaves == 0 && emp.annual_leaves > 0)
                                {
                                    dbLeavesSession.AnnualLeaves = emp.annual_leaves;
                                }

                                if (dbLeavesSession.OtherLeaves == 0 && emp.other_leaves > 0)
                                {
                                    dbLeavesSession.OtherLeaves = emp.other_leaves;
                                }

                                //types 01 - 04
                                if (dbLeavesSession.LeaveType01 == 0 && emp.leave_type01 > 0)
                                {
                                    dbLeavesSession.LeaveType01 = emp.leave_type01;
                                }

                                if (dbLeavesSession.LeaveType02 == 0 && emp.leave_type02 > 0)
                                {
                                    dbLeavesSession.LeaveType02 = emp.leave_type02;
                                }

                                if (dbLeavesSession.LeaveType03 == 0 && emp.leave_type03 > 0)
                                {
                                    dbLeavesSession.LeaveType03 = emp.leave_type03;
                                }

                                if (dbLeavesSession.LeaveType04 == 0 && emp.leave_type04 > 0)
                                {
                                    dbLeavesSession.LeaveType04 = emp.leave_type04;
                                }

                                //db.SaveChanges();

                                response_count++;
                            }

                            emp.type_of_employment.TypeOfEmploymentId = 1;//Permanent
                            //db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response_count;
        }

        public string ValidateAppConfigValue(string fName)
        {
            string strReturn = "";

            if (ConfigurationManager.AppSettings[fName] != null && ConfigurationManager.AppSettings[fName].ToString() != "")
            {
                strReturn = ConfigurationManager.AppSettings[fName];
            }

            return strReturn;
        }

        public void WriteToLog(string msg)
        {
            bool bLogStatus = false, bDLogStatus = false;

            //console log
            Console.WriteLine(msg);

            //IR999 - 3/3
            bLogStatus = bool.Parse(ValidateAppConfigValue("PPL_LOG_STATUS")); //int.Parse(ConfigurationManager.AppSettings["ma-log-status"] ?? "0");
            if (bLogStatus == true)
            {
                //db detailed-log
                bDLogStatus = bool.Parse(ValidateAppConfigValue("PPL_LOG_STATUS_DETAILED")); //int.Parse(ConfigurationManager.AppSettings["ma-log-status-detailed"] ?? "0");

                //log to database
                using (Context db = new Context())
                {
                    if (bDLogStatus)
                    {
                        //log all type of messages
                        db.log_message.Add(new LogMessage()
                        {
                            LogTitle = "PP Leaves Job",
                            LogDateTime = DateTime.Now,
                            LogMessageText = msg
                        });

                        db.SaveChanges();
                    }
                    else
                    {
                        if (msg.Contains("\t"))
                        {
                            //don't log '\t' log-messages
                        }
                        else
                        {
                            db.log_message.Add(new LogMessage()
                            {
                                LogTitle = "PP Leaves Job",
                                LogDateTime = DateTime.Now,
                                LogMessageText = msg
                            });

                            db.SaveChanges();
                        }
                    }
                }

            }
            ////LoggingProject.Logger.Write(msg, LoggingProject.LoggerType.InfoLogger, LoggingProject.LoggerFor.Service);
        }
    }

}



