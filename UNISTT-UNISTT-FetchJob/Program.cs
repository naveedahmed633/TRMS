using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNISTT_UNISTT_FetchJob
{
    class Program
    {
        static void Main(string[] args)
        {
            int zero = 0, iLogSave = 0, cutOff = 0;
            int count1 = 0, count2 = 0, count3 = 0;

            bool isTTLogAllowed = false;
            string strLTitleText = "", strStep = "1";
            List<int> insertedIDs = new List<int>();

            try
            {
                if (ConfigurationManager.AppSettings["LogTitleText"] != null && ConfigurationManager.AppSettings["LogTitleText"].ToString() != "")
                {
                    strLTitleText = ConfigurationManager.AppSettings["LogTitleText"].ToString();
                }

                if (ConfigurationManager.AppSettings["LogSave"] != null && ConfigurationManager.AppSettings["LogSave"].ToString() != "")
                {
                    iLogSave = int.Parse(ConfigurationManager.AppSettings["LogSave"].ToString());
                }
                else
                {
                    iLogSave = 999;
                }

                if (ConfigurationManager.AppSettings["IsTTLogAllowed"] != null && ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() != "")
                {
                    isTTLogAllowed = ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() == "1" ? true : false;
                }

                //STEP 1/3: UNISTimeTune Context
                strStep = "1";
                Console.WriteLine("UT-UT: Get Active Data Only");

                UNISTTHaTransit[] allActive = null;
                using (var db = new Context())
                {
                    allActive = db.UNISTT_ha_transit.Where(m => m.active).ToArray();
                    if (allActive.Length > 0)
                    {
                        count1 = allActive.Length;
                    }
                }
                
                //STEP 2/3: TimeTune Context from referenced DLL
                strStep = "2";
                Console.WriteLine("UT-UT: INSERT Data");
                if (allActive != null && allActive.Length > 0)
                {
                    using (var db = new ContextMaster())
                    {
                        foreach (var entity in allActive)
                        {
                            DateTime? date = null;
                            string time = null;

                            if (entity.C_Date.HasValue)
                            {
                                insertedIDs.Add(entity.UTHaTransitId);
                                date = entity.C_Date.Value.Date;
                                time = entity.C_Time;

                                if (entity.C_Unique != null && !entity.C_Unique.Equals(""))
                                {
                                    count2++;

                                    // id employee code is not numeric
                                    // move on to another record.
                                    int temp;
                                    if (!int.TryParse(entity.C_Unique, out temp))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                db.UNISTT_ha_transit.Add(new UNISTTHaTransit()
                                {
                                    C_Date = date,
                                    C_Time = time,
                                    C_Name = entity.C_Name,
                                    C_Unique = entity.C_Unique,
                                    L_UID = entity.L_UID,
                                    //C_Unique = ((count2 % 2) == 0) ? "614624" : "524542",
                                    //L_UID = ((count2 % 2) == 0) ? 614624 : 524542,
                                    L_TID = entity.L_TID,
                                    active = true,
                                    Region_ID = entity.Region_ID
                                });

                                cutOff++;
                            }

                            //save to db
                            if (cutOff == iLogSave)
                            {
                                cutOff = 0;
                                db.SaveChanges();
                            }
                        }

                        db.SaveChanges();
                    }                  
                }

                //STEP 3/3: Again UNISTimeTune Context
                strStep = "3"; cutOff = 0;
                Console.WriteLine("UT-UT: Inactive INSERTED Data");
                if (allActive != null && allActive.Length > 0)
                {
                    using (var db = new Context())
                    {
                        foreach (var entity in insertedIDs)
                        {
                            var record = db.UNISTT_ha_transit.Where(m => m.UTHaTransitId.Equals(entity)).FirstOrDefault();
                            if (record != null)
                            {
                                record.active = false;

                                count3++;
                                cutOff++;

                                //generate exception to log its testing
                                //if (length3 == 5)
                                //{
                                //    count3 = 5 / zero;
                                //}

                                //save to db
                                if (cutOff == iLogSave)
                                {
                                    cutOff = 0;
                                    db.SaveChanges();
                                }
                            }
                        }

                        db.SaveChanges();
                    }
                }

                //log the message
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "UT-UT: ALL Steps Done " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "UT-UT: ALL Steps Done " + strLTitleText + " - Succeeded! (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + ")"
                        });

                        db.SaveChanges();
                        Console.WriteLine("UT-UT: ALL Steps Done " + strLTitleText + " - Message Logged with Success!");
                    }
                }

                Console.WriteLine("UT-UT: ALL Steps Completed!");
            }
            catch (Exception ex)
            {
                string stkText = "na", msgText = "na", innText = "na";

                if (ex.StackTrace != null && ex.StackTrace.Length > 0)
                {
                    stkText = (ex.StackTrace.Length > 301) ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
                }

                if (ex.Message != null && ex.Message.Length > 0)
                {
                    msgText = (ex.Message.Length > 301) ? ex.Message.Substring(0, 300) : ex.Message;
                }

                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message != null && ex.InnerException.Message.Length > 0)
                    {
                        innText = (ex.InnerException.Message.Length > 501) ? ex.InnerException.Message.Substring(0, 500) : ex.InnerException.Message;
                    }
                }

                //log the message execption
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "UT-UT: Exception " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "UT-UT: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText
                        });

                        db.SaveChanges();
                        Console.WriteLine("UT-UT: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText);
                    }
                }
            }

            Console.WriteLine("\n\n\nUT-UT: EXIT Now...");
            ////Console.ReadKey();
        }
    }
}
