using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNIS_UNIS_FetchJob
{
    class Program
    {
        static void Main(string[] args)
        {
            int zero = 0, iLogSave = 0, cutOff = 0;
            int count1 = 0, count2 = 0, count3 = 0;

            bool isTTLogAllowed = false;
            string strUNISRegionID = "2", strLTitleText = "", strStep = "";
            List<int> insertedUserIDs = new List<int>();
            List<int> insertedEmployeIDs = new List<int>();
            List<string> insertedUserCardIDs = new List<string>();
            List<int> insertedUserFingerIDs = new List<int>();

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

                if (ConfigurationManager.AppSettings["UNISRegionID"] != null && ConfigurationManager.AppSettings["UNISRegionID"].ToString() != "")
                {
                    strUNISRegionID = ConfigurationManager.AppSettings["UNISRegionID"].ToString();
                }

                if (ConfigurationManager.AppSettings["IsTTLogAllowed"] != null && ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() != "")
                {
                    isTTLogAllowed = ConfigurationManager.AppSettings["IsTTLogAllowed"].ToString() == "1" ? true : false;
                }


                //use ACTIVE bit columns given in DB tables

                #region UNISDataLog Master 'tUser' table - UNIS Region: Data Transfer

                //STEP tUser 1/3: Get UNIS tUser Table Data
                strStep = "1";
                Console.WriteLine("U-U: tUser - Get UNISMaster DB Active Data Only");

                tUsers[] allActivetUsers = null;
                using (var db = new ContextUNISDataLogMaster())
                {
                    if (strUNISRegionID == "2")
                        allActivetUsers = db.UNIS_tUsers.Where(m => m.U_Active2).ToArray();
                    else if (strUNISRegionID == "3")
                        allActivetUsers = db.UNIS_tUsers.Where(m => m.U_Active3).ToArray();
                    else
                        allActivetUsers = db.UNIS_tUsers.Where(m => m.U_Active).ToArray();

                    if (allActivetUsers.Length > 0)
                    {
                        count1 = allActivetUsers.Length;
                    }
                }


                //STEP tUser 2/3: TimeTune Context from referenced DLL
                strStep = "2";
                Console.WriteLine("U-U: tUser - INSERT INTO 'UNIS Region' DB");

                if (allActivetUsers != null && allActivetUsers.Length > 0)
                {
                    using (var db = new ContextUNISRegion())
                    {
                        foreach (var entity in allActivetUsers)
                        {
                            //IR Added Code for Existing Data by PK
                            var dbUNIStUser = db.UNIS_tUser.Where(u => u.L_ID == entity.L_ID).FirstOrDefault();
                            if (dbUNIStUser != null)
                            {
                                int u_id = dbUNIStUser.L_ID;

                                db.UNIS_tUser.Remove(dbUNIStUser);
                                db.SaveChanges();

                                //IR Delete - If already Exists
                                using (var dbTT = new DLL.Models.Context())
                                {
                                    dbTT.log_message.Add(new DLL.Models.LogMessage
                                    {
                                        LogTitle = "U-U: tUser - DELETED " + strLTitleText + "",
                                        LogDateTime = DateTime.Now,
                                        LogMessageText = "U-U: tUser - DELETED (Id=" + u_id.ToString() + ")"
                                    });
                                    dbTT.SaveChanges();

                                    Console.WriteLine("U-U: tUser - DELETED (Id=" + u_id.ToString() + ")");
                                }
                            }

                            if (entity.L_ID > 0)
                            {
                                db.UNIS_tUser.Add(new tUser()
                                {
                                    L_ID = entity.L_ID,
                                    C_Name = entity.C_Name,
                                    C_Unique = entity.C_Unique,
                                    L_Type = entity.L_Type,
                                    C_RegDate = entity.C_RegDate,
                                    L_OptDateLimit = entity.L_OptDateLimit,
                                    C_DateLimit = entity.C_DateLimit,
                                    L_AccessType = entity.L_AccessType,
                                    C_Password = entity.C_Password,
                                    L_Identify = entity.L_Identify,
                                    L_VerifyLevel = entity.L_VerifyLevel,
                                    C_AccessGroup = entity.C_AccessGroup,
                                    C_PassbackStatus = entity.C_PassbackStatus,
                                    L_VOIPUsed = entity.L_VOIPUsed,
                                    L_DoorOpen = entity.L_DoorOpen,
                                    L_AutoAnswer = entity.L_AutoAnswer,
                                    L_EnableMeta1 = entity.L_EnableMeta1,
                                    L_RingCount1 = entity.L_RingCount1,
                                    C_LoginID1 = entity.C_LoginID1,
                                    C_SipAddr1 = entity.C_SipAddr1,
                                    L_EnableMeta2 = entity.L_EnableMeta2,
                                    L_RingCount2 = entity.L_RingCount2,
                                    C_LoginID2 = entity.C_LoginID2,
                                    C_SipAddr2 = entity.C_SipAddr2,
                                    C_UserMessage = entity.C_UserMessage,
                                    L_Blacklist = entity.L_Blacklist,
                                    L_IsNotice = entity.L_IsNotice,
                                    C_Notice = entity.C_Notice,
                                    C_PassbackTime = entity.C_PassbackTime,
                                    L_ExceptPassback = entity.L_ExceptPassback,
                                    L_DataCheck = entity.L_DataCheck,
                                    L_Partition = entity.L_Partition,
                                    L_FaceIdentify = entity.L_FaceIdentify,
                                    B_DuressFinger = entity.B_DuressFinger,
                                    C_RemotePW = entity.C_RemotePW,
                                    L_WrongCount = entity.L_WrongCount,
                                    L_LogonLocked = entity.L_LogonLocked,
                                    C_LogonDateTime = entity.C_LogonDateTime,
                                    C_UdatePassword = entity.C_UdatePassword,
                                    C_MustChgPwd = entity.C_MustChgPwd,
                                    L_AuthValue = entity.L_AuthValue,
                                    L_RegServer = entity.L_RegServer
                                });

                                insertedUserIDs.Add(entity.L_ID);
                                cutOff++; count2++;
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

                //STEP tUser 3/3: Again Inactive the UNISDataLog Master data
                strStep = "3"; cutOff = 0;
                Console.WriteLine("U-U: tUser - Inactive Master Data");

                if (allActivetUsers != null && allActivetUsers.Length > 0)
                {
                    using (var db = new ContextUNISDataLogMaster())
                    {
                        foreach (var entity in insertedUserIDs)
                        {
                            var record = db.UNIS_tUsers.Where(m => m.L_ID.Equals(entity)).FirstOrDefault();
                            if (record != null)
                            {
                                if (strUNISRegionID == "2")
                                    record.U_Active2 = false;
                                else if (strUNISRegionID == "3")
                                    record.U_Active3 = false;
                                else
                                    record.U_Active = false;

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

                //STEP tUser: log the message
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "U-U: tUser - Sync " + strLTitleText + "",
                            LogDateTime = DateTime.Now,
                            LogMessageText = "U-U: tUser - Sync " + strLTitleText + " - Succeeded! (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + ")"
                        });

                        db.SaveChanges();
                        Console.WriteLine("U-U: tUser - Sync " + strLTitleText + " - Message Logged with Success!");
                    }
                }

                #endregion

                zero = 0; iLogSave = 0; cutOff = 0;
                count1 = 0; count2 = 0; count3 = 0;

                #region UNISDataLog Master 'tEmploye' table - UNIS Region: Data Transfer

                //STEP tEmploye 1/3: Get UNIS tUser Table Data
                strStep = "1";
                Console.WriteLine("U-U: tEmploye - Get UNISMaster DB Active Data Only");

                tEmployes[] allActivetEmployees = null;
                using (var db = new ContextUNISDataLogMaster())
                {
                    if (strUNISRegionID == "2")
                        allActivetEmployees = db.UNIS_tEmployes.Where(m => m.E_Active2).ToArray();
                    else if (strUNISRegionID == "3")
                        allActivetEmployees = db.UNIS_tEmployes.Where(m => m.E_Active3).ToArray();
                    else
                        allActivetEmployees = db.UNIS_tEmployes.Where(m => m.E_Active).ToArray();

                    if (allActivetEmployees.Length > 0)
                    {
                        count1 = allActivetEmployees.Length;
                    }
                }


                //STEP tEmploye 2/3: TimeTune Context from referenced DLL
                strStep = "2";
                Console.WriteLine("U-U: tEmploye - INSERT INTO 'UNIS Region' DB");

                if (allActivetEmployees != null && allActivetEmployees.Length > 0)
                {
                    using (var db = new ContextUNISRegion())
                    {
                        foreach (var entity in allActivetEmployees)
                        {
                            //IR Added Code for Existing Data by PK
                            var dbUNIStEmployee = db.UNIS_tEmploye.Where(u => u.L_UID == entity.L_UID).FirstOrDefault();
                            if (dbUNIStEmployee != null)
                            {
                                int u_id = dbUNIStEmployee.L_UID;

                                db.UNIS_tEmploye.Remove(dbUNIStEmployee);
                                db.SaveChanges();

                                //IR Delete - If already Exists
                                using (var dbTT = new DLL.Models.Context())
                                {
                                    dbTT.log_message.Add(new DLL.Models.LogMessage
                                    {
                                        LogTitle = "U-U: tEmployee - DELETED " + strLTitleText + "",
                                        LogDateTime = DateTime.Now,
                                        LogMessageText = "U-U: tEmployee - DELETED (Id=" + u_id.ToString() + ")"
                                    });
                                    dbTT.SaveChanges();

                                    Console.WriteLine("U-U: tEmployee - DELETED (Id=" + u_id.ToString() + ")");
                                }
                            }

                            if (entity.L_UID > 0)
                            {
                                db.UNIS_tEmploye.Add(new tEmploye()
                                {
                                    L_UID = entity.L_UID,
                                    C_IncludeDate = entity.C_IncludeDate,
                                    C_RetiredDate = entity.C_RetiredDate,
                                    C_Office = entity.C_Office,
                                    C_Post = entity.C_Post,
                                    C_Staff = entity.C_Staff,
                                    C_Authority = entity.C_Authority,
                                    C_Work = entity.C_Work,
                                    C_Money = entity.C_Money,
                                    C_Meal = entity.C_Meal,
                                    C_Phone = entity.C_Phone,
                                    C_Email = entity.C_Email,
                                    C_Address = entity.C_Address,
                                    C_Remark = entity.C_Remark
                                });

                                insertedEmployeIDs.Add(entity.L_UID);
                                cutOff++; count2++;
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

                //STEP tEmploye 3/3: Again Inactive the UNISDataLog Master data
                strStep = "3"; cutOff = 0;
                Console.WriteLine("U-U: tEmploye - Inactive Master Data");

                if (allActivetEmployees != null && allActivetEmployees.Length > 0)
                {
                    using (var db = new ContextUNISDataLogMaster())
                    {
                        foreach (var entity in insertedEmployeIDs)
                        {
                            var record = db.UNIS_tEmployes.Where(m => m.L_UID.Equals(entity)).FirstOrDefault();
                            if (record != null)
                            {
                                if (strUNISRegionID == "2")
                                    record.E_Active2 = false;
                                else if (strUNISRegionID == "3")
                                    record.E_Active3 = false;
                                else
                                    record.E_Active = false;

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

                //STEP tEmploye: log the message
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "U-U: tEmploye - Sync " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "U-U: tEmploye - Sync " + strLTitleText + " - Succeeded! (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + ")"
                        });

                        db.SaveChanges();
                        Console.WriteLine("U-U: tEmploye - Sync " + strLTitleText + " - Message Logged with Success!");
                    }
                }

                #endregion

                zero = 0; iLogSave = 0; cutOff = 0;
                count1 = 0; count2 = 0; count3 = 0;

                #region UNISDataLog Master 'iUserCard' table - UNIS Region: Data Transfer

                //STEP iUserCard 1/3: Get UNIS tUser Table Data
                strStep = "1";
                Console.WriteLine("U-U: iUserCard - Get UNISMaster DB Active Data Only");

                iUserCards[] allActiveiUserCards = null;
                using (var db = new ContextUNISDataLogMaster())
                {
                    if (strUNISRegionID == "2")
                        allActiveiUserCards = db.UNIS_iUserCards.Where(m => m.C_Active2).ToArray();
                    else if (strUNISRegionID == "3")
                        allActiveiUserCards = db.UNIS_iUserCards.Where(m => m.C_Active3).ToArray();
                    else
                        allActiveiUserCards = db.UNIS_iUserCards.Where(m => m.C_Active).ToArray();

                    if (allActiveiUserCards.Length > 0)
                    {
                        count1 = allActiveiUserCards.Length;
                    }
                }


                //STEP iUserCard 2/3: TimeTune Context from referenced DLL
                strStep = "2";
                Console.WriteLine("U-U: iUserCard - INSERT INTO 'UNIS Region' DB");

                if (allActiveiUserCards != null && allActiveiUserCards.Length > 0)
                {
                    using (var db = new ContextUNISRegion())
                    {
                        foreach (var entity in allActiveiUserCards)
                        {
                            //IR Added Code for Existing Data by PK
                            var dbUNISiUserCard = db.UNIS_iUserCard.Where(u => u.L_UID == entity.L_UID).FirstOrDefault();
                            if (dbUNISiUserCard != null)
                            {
                                int u_id = dbUNISiUserCard.L_UID ?? 0;

                                db.UNIS_iUserCard.Remove(dbUNISiUserCard);
                                db.SaveChanges();

                                //IR Delete - If already Exists
                                using (var dbTT = new DLL.Models.Context())
                                {
                                    dbTT.log_message.Add(new DLL.Models.LogMessage
                                    {
                                        LogTitle = "U-U: iUserCard - DELETED " + strLTitleText + "",
                                        LogDateTime = DateTime.Now,
                                        LogMessageText = "U-U: iUserCard - DELETED (Id=" + u_id.ToString() + ")"
                                    });
                                    dbTT.SaveChanges();

                                    Console.WriteLine("U-U: iUserCard - DELETED (Id=" + u_id.ToString() + ")");
                                }
                            }

                            if (entity.L_UID > 0)
                            {
                                db.UNIS_iUserCard.Add(new iUserCard()
                                {
                                    L_UID = entity.L_UID,
                                    C_CardNum = entity.C_CardNum,
                                    L_DataCheck = entity.L_DataCheck
                                });

                                insertedUserCardIDs.Add(entity.C_CardNum);
                                cutOff++; count2++;
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

                //STEP iUserCard 3/3: Again Inactive the UNISDataLog Master data
                strStep = "3"; cutOff = 0;
                Console.WriteLine("U-U: iUserCard - Inactive Master Data");

                if (allActiveiUserCards != null && allActiveiUserCards.Length > 0)
                {
                    using (var db = new ContextUNISDataLogMaster())
                    {
                        foreach (var entity in insertedUserCardIDs)
                        {
                            var record = db.UNIS_iUserCards.Where(m => m.C_CardNum.Equals(entity)).FirstOrDefault();
                            if (record != null)
                            {
                                if (strUNISRegionID == "2")
                                    record.C_Active2 = false;
                                else if (strUNISRegionID == "3")
                                    record.C_Active3 = false;
                                else
                                    record.C_Active = false;

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

                //STEP iUserCard: log the message
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "U-U: iUserCard Sync " + strLTitleText + "",
                            LogDateTime = DateTime.Now,
                            LogMessageText = "U-U: iUserCard Sync " + strLTitleText + " - Succeeded! (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + ")"
                        });

                        db.SaveChanges();
                        Console.WriteLine("U-U: iUserCard Sync " + strLTitleText + " - Message Logged with Success!");
                    }
                }

                #endregion

                zero = 0; iLogSave = 0; cutOff = 0;
                count1 = 0; count2 = 0; count3 = 0;

                #region UNISDataLog Master 'iUserFinger' table - UNIS Region: Data Transfer

                //STEP iUserFinger 1/3: Get UNIS iUserFinger Table Data
                strStep = "1";
                Console.WriteLine("U-U: iUserFinger - Get UNISMaster DB Active Data Only");

                iUserFingers[] allActiveiUserFingers = null;
                using (var db = new ContextUNISDataLogMaster())
                {
                    if (strUNISRegionID == "2")
                        allActiveiUserFingers = db.UNIS_iUserFingers.Where(m => m.F_Active2).ToArray();
                    else if (strUNISRegionID == "3")
                        allActiveiUserFingers = db.UNIS_iUserFingers.Where(m => m.F_Active3).ToArray();
                    else
                        allActiveiUserFingers = db.UNIS_iUserFingers.Where(m => m.F_Active).ToArray();

                    if (allActiveiUserFingers.Length > 0)
                    {
                        count1 = allActiveiUserFingers.Length;
                    }
                }

                //--ki735082602

                //STEP iUserFinger 2/3: TimeTune Context from referenced DLL
                strStep = "2";
                Console.WriteLine("U-U: iUserFinger - INSERT INTO 'UNIS Region' DB");

                if (allActiveiUserFingers != null && allActiveiUserFingers.Length > 0)
                {
                    using (var db = new ContextUNISRegion())
                    {
                        foreach (var entity in allActiveiUserFingers)
                        {
                            //IR Added Code for Existing Data by PK
                            var dbUNISiUserFinger = db.UNIS_iUserFinger.Where(u => u.L_UID == entity.L_UID).FirstOrDefault();
                            if (dbUNISiUserFinger != null)
                            {
                                int u_id = dbUNISiUserFinger.L_UID;

                                db.UNIS_iUserFinger.Remove(dbUNISiUserFinger);
                                db.SaveChanges();

                                //IR Delete - If already Exists
                                using (var dbTT = new DLL.Models.Context())
                                {
                                    dbTT.log_message.Add(new DLL.Models.LogMessage
                                    {
                                        LogTitle = "U-U: iUserFinger - DELETED " + strLTitleText + "",
                                        LogDateTime = DateTime.Now,
                                        LogMessageText = "U-U: iUserFinger - DELETED (Id=" + u_id.ToString() + ")"
                                    });
                                    dbTT.SaveChanges();

                                    Console.WriteLine("U-U: iUserFinger - DELETED (Id=" + u_id.ToString() + ")");
                                }
                            }

                            if (entity.L_UID > 0)
                            {
                                db.UNIS_iUserFinger.Add(new iUserFinger()
                                {
                                    L_UID = entity.L_UID,
                                    L_IsWideChar = entity.L_IsWideChar,
                                    B_TextFIR = entity.B_TextFIR
                                });

                                insertedUserFingerIDs.Add(entity.L_UID);
                                cutOff++; count2++;
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

                //STEP iUserFinger 3/3: Again Inactive the UNISDataLog Master data
                strStep = "3"; cutOff = 0;
                Console.WriteLine("U-U: iUserFinger - Inactive Master Data");

                if (allActiveiUserFingers != null && allActiveiUserFingers.Length > 0)
                {
                    using (var db = new ContextUNISDataLogMaster())
                    {
                        foreach (var entity in insertedUserFingerIDs)
                        {
                            var record = db.UNIS_iUserFingers.Where(m => m.L_UID.Equals(entity)).FirstOrDefault();
                            if (record != null)
                            {
                                if (strUNISRegionID == "2")
                                    record.F_Active2 = false;
                                else if (strUNISRegionID == "3")
                                    record.F_Active3 = false;
                                else
                                    record.F_Active = false;


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

                //STEP iUserFinger: log the message
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "U-U: iUserFinger - Sync " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "U-U: iUserFinger - Sync " + strLTitleText + " - Succeeded! (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + ")"
                        });

                        db.SaveChanges();
                        Console.WriteLine("U-U: iUserFinger - Sync " + strLTitleText + " - Message Logged with Success!");
                    }
                }

                #endregion

                //log message to process completed
                if (isTTLogAllowed)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        db.log_message.Add(new DLL.Models.LogMessage
                        {
                            LogTitle = "U-U: Completed",
                            LogDateTime = DateTime.Now,
                            LogMessageText = "U-U: All Steps Completed"
                        });
                        db.SaveChanges();
                    }
                }

                Console.WriteLine("U-U: ALL Steps Completed!");
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
                            LogTitle = "U-U: Exception " + strLTitleText,
                            LogDateTime = DateTime.Now,
                            LogMessageText = "U-U: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText
                        });

                        db.SaveChanges();
                        Console.WriteLine("U-U: Exception " + strLTitleText + " - At STEP: " + strStep + " (Get=" + count1 + ", Ins=" + count2 + ", Set=" + count3 + "), StackTrace: " + stkText + ", Message: " + msgText + ", Inner: " + innText);
                    }
                }
            }

            Console.WriteLine("\n\n\nU-U: EXIT Now...");
            //Console.ReadKey();
        }
    }
}
