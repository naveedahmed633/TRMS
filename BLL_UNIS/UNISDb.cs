using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_UNIS.Models;
using BLL_UNIS.ViewModels;
using System.Globalization;

namespace BLL_UNIS
{
    public class UNISQueryRun
    {
        public static string run(string query)
        {
            using (var db = new DLL_UNIS.Models.UNISContext())
            {
                try
                {
                    var result = db.Database.SqlQuery<DLL_UNIS.Models.tTerminal>(query);
                    string toReturn = result.Select(p => p.C_Name).FirstOrDefault();
                    return "Successfull";
                }
                catch (Exception ex)
                {
                    return "Failed";
                }
            }
        }

        public class RegCardCount
        {
            public long L_UID { get; set; }
            public int CardCount { get; set; }
        }

        public class RegCardList
        {
            public long L_UID { get; set; }
            public string CardNumber { get; set; }
        }

        public class RegFingerCount
        {
            public long L_UID { get; set; }
            public int FingerCount { get; set; }
        }

        public static string getALLCardsCount()
        {
            int icount = 0;
            string total_count = "0";
            string query = "select user_id as L_UID, count(*) as CardCount from user_rfcards group by user_id;";
            /*
                select u.L_ID,(select count(*) from iUserCard c where u.L_ID=c.L_UID)  from tUser u
                where (select count(*) from iUserCard c where u.L_ID=c.L_UID) = 1
            */

            using (var db = new DLL_UNIS.Models.UNISContext())
            {
                try
                {
                    List<RegCardCount> result = db.Database.SqlQuery<RegCardCount>(query).ToList();
                    foreach (var item in result)
                    {
                        icount += item.CardCount;
                    }

                    total_count = icount.ToString();
                }
                catch (Exception ex)
                {
                    total_count = "-1";
                }
            }

            return total_count;
        }

        public static string getALLFingersCount()
        {
            int icount = 0;
            string total_count = "0";
            string query = "select user_id as L_UID, count(*) as FingerCount from user_fps group by user_id;";
            /*
                select u.L_ID,(select count(*) from iUserCard c where u.L_ID=c.L_UID)  from tUser u
                where (select count(*) from iUserCard c where u.L_ID=c.L_UID) = 1
            */

            using (var db = new DLL_UNIS.Models.UNISContext())
            {
                try
                {
                    List<RegFingerCount> result = db.Database.SqlQuery<RegFingerCount>(query).ToList();
                    foreach (var item in result)
                    {
                        icount += item.FingerCount;
                    }

                    total_count = icount.ToString();
                }
                catch (Exception ex)
                {
                    total_count = "-1";
                }
            }

            return total_count;
        }

        public static string getCardsListByLUID(string emp_code)
        {
            string card_num = "0";
            string query = "select user_id as L_UID, card_num as CardNumber from user_rfcards where user_id=(select top 1 user_id from users u WHERE u.unique_id='" + emp_code + "')";
            /*
                select u.L_ID,(select count(*) from iUserCard c where u.L_ID=c.L_UID)  from tUser u
                where (select count(*) from iUserCard c where u.L_ID=c.L_UID) = 1
            */
            List<RegCardList> result = new List<RegCardList>();
            using (var db = new DLL_UNIS.Models.UNISContext())
            {
                try
                {
                    result = db.Database.SqlQuery<RegCardList>(query).ToList();
                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            card_num += item.CardNumber + "~";
                        }

                        if (card_num.Contains("~"))
                        {
                            card_num = card_num.TrimEnd('~');
                            card_num = card_num.Replace("~", "<br>");
                        }
                    }
                }
                catch (Exception ex)
                {
                    card_num = "-1";
                }
            }

            return card_num;
        }

        public static string getCardsCountByLUID(long l_uid)
        {
            string total_count = "0";
            string query = "select count(*) from user_rfcards where user_id = " + l_uid.ToString();

            using (var db = new DLL_UNIS.Models.UNISContext())
            {
                try
                {
                    int n = db.Database.SqlQuery<int>(query).FirstOrDefault();
                    total_count = n.ToString();
                }
                catch (Exception ex)
                {
                    total_count = "-1";
                }
            }

            return total_count;
        }

        public static string getFingerStatusByLUID(string emp_code)
        {
            string finger_status = "No";
            string query = "select count(*) from user_fps where user_id=(select top 1 user_id from users u WHERE u.unique_id='" + emp_code + "')";

            using (var db = new DLL_UNIS.Models.UNISContext())
            {
                try
                {
                    int cnt = db.Database.SqlQuery<int>(query).FirstOrDefault();
                    if (cnt > 0)
                    {
                        finger_status = "Yes";
                    }
                    else
                    {
                        finger_status = "No";
                    }
                }
                catch (Exception ex)
                {
                    finger_status = "-1";
                }
            }

            return finger_status;
        }


    }



    public class UNIS_Reports
    {
        #region device status report

        public static List<C_office> getCOfficeList()
        {
            List<C_office> result = new List<C_office>();

            try
            {
                using (UNISContext db = new UNISContext())
                {
                    db.Database.CommandTimeout = 840;
                    result = db.Database.SqlQuery<C_office>("[SP_GetCOfficeList]").ToList();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }


        public static List<DevicesUnregister> getUnRegisterDevicesStatusList()
        {
            List<DevicesUnregister> result = new List<DevicesUnregister>();

            try
            {
                using (UNISContext db = new UNISContext())
                {
                    db.Database.CommandTimeout = 840;
                    result = db.Database.SqlQuery<DevicesUnregister>("GetUnregisterDevices").ToList();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static List<BLL_UNIS.ViewModels.DeviceStatusReport> getDevicesStatusByRegionIDList(int region_id, string strSearch, string strSortOrder, int iStart, int iLength)
        {
            string date = "";
            string status = "";
            List<DevicesStatus> result = new List<DevicesStatus>();
            List<BLL_UNIS.ViewModels.DeviceStatusReport> finalResult = new List<BLL_UNIS.ViewModels.DeviceStatusReport>();

            if (region_id == 2)
            {
                using (UNISContextR2 db = new UNISContextR2())
                {
                    db.Database.CommandTimeout = 840;

                    strSearch = null;
                    iStart = iStart + 1;

                    var result2 = db.Database.SqlQuery<BLL_UNIS.ViewModels.DeviceStatusReport>(string.Format("SP_GetDevicesStatusByRegionID @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();

                    for (int i = 0; i < result2.Count; i++)
                    {
                        date = DateTime.Parse(result2[i].C_EventTime).ToString("dd-MMM-yyyy");
                        if (result2[i].L_Detail == "Connected")
                        {
                            status = "Active";
                        }
                        else { status = "Inactive"; }

                        finalResult.Add(new BLL_UNIS.ViewModels.DeviceStatusReport()
                        {
                            L_ID = result2[i].L_ID,
                            C_EventTime = date,
                            C_Name = result2[i].C_Name,
                            C_Office = result2[i].C_Office,
                            C_Place = result2[i].C_Place,
                            C_IPAddr = result2[i].C_IPAddr,
                            L_Detail = status

                        });
                    }
                }
            }
            //else if (region_id == 3)
            //{
            //    using (UNISContextR3 db = new UNISContextR3())
            //    {
            //        db.Database.CommandTimeout = 840;

            //        strSearch = null;
            //        iStart = iStart + 1;

            //        var result2 = db.Database.SqlQuery<BLL_UNIS.ViewModels.DeviceStatusReport>(string.Format("SP_GetDevicesStatusByRegionID @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();

            //        for (int i = 0; i < result2.Count; i++)
            //        {
            //            date = DateTime.Parse(result2[i].C_EventTime).ToString("dd-MMM-yyyy");
            //            if (result2[i].L_Detail == "Connected")
            //            {
            //                status = "Active";
            //            }
            //            else { status = "Inactive"; }

            //            finalResult.Add(new BLL_UNIS.ViewModels.DeviceStatusReport()
            //            {
            //                L_ID = result2[i].L_ID,
            //                C_EventTime = date,
            //                C_Name = result2[i].C_Name,
            //                C_Office = result2[i].C_Office,
            //                C_Place = result2[i].C_Place,
            //                C_IPAddr = result2[i].C_IPAddr,
            //                L_Detail = status

            //            });

            //        }
            //    }
            //}
            else
            {
                using (UNISContext db = new UNISContext())
                {
                    db.Database.CommandTimeout = 840;

                    strSearch = null;
                    iStart = iStart + 1;

                    var result2 = db.Database.SqlQuery<BLL_UNIS.ViewModels.DeviceStatusReport>(string.Format("SP_GetDevicesStatusByRegionID @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();

                    for (int i = 0; i < result2.Count; i++)
                    {
                        date = DateTime.Parse(result2[i].C_EventTime).ToString("dd-MMM-yyyy");
                        if (result2[i].L_Detail == "Connected")
                        {
                            status = "Active";
                        }
                        else { status = "Inactive"; }

                        finalResult.Add(new BLL_UNIS.ViewModels.DeviceStatusReport()
                        {
                            L_ID = result2[i].L_ID,
                            C_EventTime = date,
                            C_Name = result2[i].C_Name,
                            C_Office = result2[i].C_Office,
                            C_Place = result2[i].C_Place,
                            C_IPAddr = result2[i].C_IPAddr,
                            L_Detail = status

                        });

                    }
                }
            }


            return finalResult;
        }


        public static List<DevicesStatus> getDevicesStatusRegionList(int region_id, string strSearch, string strSortOrder, int iStart, int iLength)
        {
            List<DevicesStatus> result = new List<DevicesStatus>();
            List<DevicesStatus> resultR1 = new List<DevicesStatus>();
            List<DevicesStatus> resultR2 = new List<DevicesStatus>();

            try
            {
                if (region_id == 1)
                {
                    //UNIS Region1 - Master Server Data
                    using (UNISContext db = new UNISContext())
                    {
                        db.Database.CommandTimeout = 840;

                        strSearch = strSearch ?? "";
                        iStart = iStart + 1;

                        resultR1 = db.Database.SqlQuery<DevicesStatus>(string.Format("SP_GetDeviceLatestStatusRegion @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();
                    }

                    if (resultR1 != null && resultR1.Count > 0)
                    {
                        result = resultR1.ToList();
                    }
                }
                else if (region_id == 2)
                {
                    //UNIS Region2 Server Data
                    using (UNISContextR2 dbR2 = new UNISContextR2())
                    {
                        dbR2.Database.CommandTimeout = 840;

                        strSearch = strSearch ?? "";
                        iStart = iStart + 1;

                        resultR2 = dbR2.Database.SqlQuery<DevicesStatus>(string.Format("SP_GetDeviceLatestStatusRegion @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();
                    }

                    if (resultR2 != null && resultR2.Count > 0)
                    {
                        result = resultR2.ToList();
                    }
                }
                else if (region_id == -1)//all 2 regions
                {
                    //UNIS both Servers Data
                    using (UNISContext db = new UNISContext())
                    {
                        db.Database.CommandTimeout = 840;

                        strSearch = strSearch ?? "";
                        iStart = iStart + 1;

                        resultR1 = db.Database.SqlQuery<DevicesStatus>(string.Format("SP_GetDeviceLatestStatusRegion @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();
                    }

                    //UNIS Region2 Server Data
                    using (UNISContextR2 dbR2 = new UNISContextR2())
                    {
                        dbR2.Database.CommandTimeout = 840;

                        strSearch = strSearch ?? "";
                        iStart = iStart + 1;

                        resultR2 = dbR2.Database.SqlQuery<DevicesStatus>(string.Format("SP_GetDeviceLatestStatusRegion @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();
                    }

                    if (resultR2 != null && resultR2.Count > 0)
                    {
                        result = resultR1.ToList();

                        foreach (var r2 in resultR2)
                        {
                            if (result.Where(r => r.L_ID == r2.L_ID).FirstOrDefault() == null)//if (result.Where(r => r.L_ID == r2.L_ID && r.C_Name.ToLower() == r2.C_Name.ToLower()).FirstOrDefault() == null)//
                            {
                                result.Add(r2);
                            }
                        }

                        //result = resultR1.Concat(resultR2).u.ToList();
                    }
                }
                else
                {

                }

            }
            catch (Exception)
            {
                if (resultR1 != null && resultR1.Count > 0)
                {
                    result = resultR1.ToList();
                }
            }

            return result;
        }

        public static List<DevicesStatusCount> getDevicesStatusCountList(int region_id, string strSearch, string strSortOrder, int iStart, int iLength)
        {
            List<DevicesStatusCount> result = new List<DevicesStatusCount>();
            List<DevicesStatusCount> resultR1 = new List<DevicesStatusCount>();

            try
            {
                if (region_id > -2)
                {
                    //UNIS Region1 - Master Server Data
                    using (UNISContext db = new UNISContext())
                    {
                        db.Database.CommandTimeout = 840;

                        strSearch = strSearch ?? "";
                        iStart = iStart + 1;

                        resultR1 = db.Database.SqlQuery<DevicesStatusCount>(string.Format("[SP_GetDeviceStatusCount] @RegionID={0}", region_id)).ToList();
                    }

                    if (resultR1 != null && resultR1.Count > 0)
                    {
                        result = resultR1.ToList();
                    }
                }
            }
            catch (Exception)
            {
                if (resultR1 != null && resultR1.Count > 0)
                {
                    result = resultR1.ToList();
                }
            }

            return result;
        }

        public static string getDevicesStatusCount(int region_id)
        {
            List<DevicesStatusCount> result = new List<DevicesStatusCount>();
            List<DevicesStatusCount> resultR1 = new List<DevicesStatusCount>();

            string strReturn = "";
            int connCount = 0, disCount = 0, totalCount = 0;

            try
            {
                if (region_id > -2)
                {
                    //UNIS Region1 - Master Server Data
                    using (UNISContext db = new UNISContext())
                    {
                        db.Database.CommandTimeout = 840;

                        resultR1 = db.Database.SqlQuery<DevicesStatusCount>(string.Format("[SP_GetDeviceStatusCount] @RegionID={0}", region_id)).ToList();
                    }

                    if (resultR1 != null && resultR1.Count > 0)
                    {
                        foreach (var item in resultR1)
                        {
                            connCount = item.ConnCount;
                            disCount = item.DisConnCount;
                            totalCount = item.TotalCount;

                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (resultR1 != null && resultR1.Count > 0)
                {
                    result = resultR1.ToList();
                }
            }

            strReturn = connCount + ":" + disCount + ":" + totalCount;

            return strReturn;
        }


        public static List<DevicesStatus> getDevicesStatusList(string strSearch, string strSortOrder, int iStart, int iLength)
        {
            List<DevicesStatus> result = new List<DevicesStatus>();
            List<DevicesStatus> resultR1 = new List<DevicesStatus>();
            List<DevicesStatus> resultR2 = new List<DevicesStatus>();

            try
            {
                //UNIS Master Server Data
                using (UNISContext db = new UNISContext())
                {
                    db.Database.CommandTimeout = 840;

                    strSearch = strSearch ?? "";
                    iStart = iStart + 1;

                    resultR1 = db.Database.SqlQuery<DevicesStatus>(string.Format("SP_GetDeviceLatestStatus @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();
                }

                //UNIS Region2 Server Data
                using (UNISContextR2 dbR2 = new UNISContextR2())
                {
                    dbR2.Database.CommandTimeout = 840;

                    strSearch = strSearch ?? "";
                    iStart = iStart + 1;

                    resultR2 = dbR2.Database.SqlQuery<DevicesStatus>(string.Format("SP_GetDeviceLatestStatus @Search='{0}', @Page={1}, @RecsPerPage={2}", strSearch, iStart, iLength)).ToList();
                }

                if (resultR2 != null && resultR2.Count > 0)
                {
                    result = resultR1.ToList();

                    foreach (var r2 in resultR2)
                    {
                        if (result.Where(r => r.L_ID == r2.L_ID).FirstOrDefault() == null)//if (result.Where(r => r.L_ID == r2.L_ID && r.C_Name.ToLower() == r2.C_Name.ToLower()).FirstOrDefault() == null)//
                        {
                            result.Add(r2);
                        }
                    }

                    //result = resultR1.Concat(resultR2).u.ToList();
                }
                else
                {
                    if (resultR1 != null && resultR1.Count > 0)
                    {
                        result = resultR1.ToList();
                    }
                }
            }
            catch (Exception)
            {
                if (resultR1 != null && resultR1.Count > 0)
                {
                    result = resultR1.ToList();
                }
            }

            return result;
        }

        public static int getOnlineDevicesCount()
        {
            int Count = 0;
            List<DevicesStatus> result = new List<DevicesStatus>();

            try
            {
                using (UNISContext db = new UNISContext())
                {
                    db.Database.CommandTimeout = 840;

                    result = db.Database.SqlQuery<DevicesStatus>(string.Format("SP_GetDeviceLatestStatus @Search='{0}', @Page={1}, @RecsPerPage={2}", "", 0, 1000)).ToList();
                    if (result != null && result.Count > 0)
                    {
                        Count = result.Where(c => c.L_Detail == "Connected").Count();
                    }
                }
            }
            catch (Exception)
            {
            }

            return Count;
        }

        /*
        public static int getDevicesList(string device_number, string from, string to, string dcStatus, string searchValue, string sortOrder, int start, int length, out List<DevicesStatus> dList)
        {
            int count = 0, d_status = 11, c_status = 12;
            var dbList = new List<DevicesStatus>();
            dList = new List<DevicesStatus>();
            var temp = new List<DevicesStatus>();

            int dNumber = 0;

            if (!int.TryParse(device_number, out dNumber))
            {
                dNumber = -1;
            }

            decimal firstDay = 0;
            decimal lastDay = 0;

            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    string[] splitFromDate = from.Split('/');
                    firstDay = decimal.Parse(splitFromDate[2] + splitFromDate[0] + splitFromDate[1] + "000000");

                    string[] splitToDate = to.Split('/');
                    lastDay = decimal.Parse(splitToDate[2] + splitToDate[0] + splitToDate[1] + "235959");
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                //Added by Inayat 7th Dec 2017
                if (device_number == null && from == null && to == null)
                {
                    firstDay = decimal.Parse(DateTime.Now.ToString("yyyyMMdd000000"));
                    lastDay = decimal.Parse(DateTime.Now.AddDays(1).ToString("yyyyMMdd235959"));
                }
            }

            try
            {
                using (UNISContext db = new UNISContext())
                {
                    //var dbList1 = db.tTerminals
                    //                .Join(db.tTerminalStateLogs,
                    //                      t => t.L_ID,
                    //                      s => s.L_TID,
                    //                      (t, s) => new DevicesStatus
                    //                      {
                    //                          L_ID = t.L_ID,
                    //                          L_TID = s.L_TID ?? 0,
                    //                          C_Name = t.C_Name,
                    //                          C_EventTime = s.C_EventTime.ToString(),
                    //                          D_EventTime = s.C_EventTime,
                    //                          L_Class = s.L_Class.ToString(),
                    //                          L_Detail = s.L_Detail.ToString()
                    //                      })
                    //    //.Select(p => new DevicesStatus
                    //    //{
                    //    //    L_ID = p.L_ID,
                    //    //    L_TID = p.L_TID,
                    //    //    C_Name = p.C_Name,
                    //    //    C_EventTime = p.C_EventTime,
                    //    //    L_Class = p.L_Class.ToString(),
                    //    //    L_Detail = p.L_Detail.ToString()
                    //    //})
                    //                      .Where(w => w.L_Class == "1" && w.L_ID == dNumber && (w.D_EventTime >= firstDay && w.D_EventTime <= lastDay))
                    //                      .OrderBy(o => sortOrder).Skip(start).Take(length).ToList();

                    //sortOrder = "s." + sortOrder.Replace("DESC", "descending");
                    if (dcStatus == null)
                    {
                        d_status = 11;
                        c_status = 12;
                    }
                    else
                    {
                        if (dcStatus == "11")//inactive or disconnected
                        {
                            d_status = 11;
                            c_status = 11;
                        }
                        else if (dcStatus == "12")//active or connected
                        {
                            d_status = 12;
                            c_status = 12;
                        }
                        else
                        {
                            d_status = 11;
                            c_status = 12;
                        }
                    }

                    if (from != null && to != null)
                    {
                        if (device_number == null)
                        {

                            //List<tTerminalStateLog> tt = db.tTerminalStateLogs
                            //   .Where(s => s.L_Class == 1 && (s.L_Detail == d_status || s.L_Detail == c_status) && (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay))
                            //   .GroupBy(l => l.L_TID)
                            //   .Select(g => g.OrderByDescending(c => c.C_EventTime).FirstOrDefault()).ToList();



                            //count = (from t in db.tTerminals
                            //         join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                            //         //let eTime = decimalParse(s.C_EventTime)
                            //         where
                            //             //s.L_TID == dNumber && 
                            //        s.L_Class == 1 &&
                            //        (s.L_Detail == d_status || s.L_Detail == c_status) &&
                            //        (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                            //         orderby s.L_TID descending, s.C_EventTime descending
                            //         select new DevicesStatus
                            //         {
                            //             L_ID = t.L_ID,
                            //             L_TID = s.L_TID ?? 0,
                            //             C_Name = t.C_Name,
                            //             C_EventTime = s.C_EventTime.ToString(),
                            //             C_Office = t.C_Office ?? "",
                            //             C_Place = t.C_Place ?? "",
                            //             //D_EventTime = (s.C_EventTime * 1),
                            //             L_Class = s.L_Class.ToString(),
                            //             L_Detail = s.L_Detail.ToString()
                            //         }).Count();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)

                            count = (from t in db.tTerminals
                                     join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                                     //let eTime = decimalParse(s.C_EventTime)
                                     where
                                         //s.L_TID == dNumber && 
                                    s.L_Class == 1 &&
                                    (s.L_Detail == d_status || s.L_Detail == c_status) &&
                                    (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                                     //orderby s.L_TID //, s.C_EventTime descending
                                     select new DevicesStatus
                                     {
                                         L_ID = t.L_ID,
                                         L_TID = s.L_TID ?? 0,
                                         C_Name = t.C_Name,
                                         C_EventTime = s.C_EventTime.ToString(),
                                         C_Office = t.C_Office ?? "",
                                         C_Place = t.C_Place ?? "",
                                         //D_EventTime = (s.C_EventTime * 1),
                                         L_Class = s.L_Class.ToString(),
                                         L_Detail = s.L_Detail.ToString()
                                     }).Count();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)


                            dbList = (from t in db.tTerminals
                                      join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                                      //let eTime = decimalParse(s.C_EventTime)
                                      where
                                          //s.L_TID == dNumber && 
                                     s.L_Class == 1 &&
                                     (s.L_Detail == d_status || s.L_Detail == c_status) &&
                                     (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                                      orderby s.L_TID descending, s.C_EventTime descending
                                      select new DevicesStatus
                                      {
                                          L_ID = t.L_ID,
                                          L_TID = s.L_TID ?? 0,
                                          C_Name = t.C_Name,
                                          C_EventTime = s.C_EventTime.ToString(),
                                          C_Office = t.C_Office ?? "",
                                          C_Place = t.C_Place ?? "",
                                          //D_EventTime = (s.C_EventTime * 1),
                                          L_Class = s.L_Class.ToString(),
                                          L_Detail = s.L_Detail.ToString()
                                      }).Skip(start).Take(length).ToList();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)
                        }
                        else
                        {
                            //count = (from t in db.tTerminals
                            //         join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                            //         //let eTime = decimalParse(s.C_EventTime)
                            //         where
                            //           s.L_TID == dNumber &&
                            //           s.L_Class == 1 &&
                            //           (s.L_Detail == d_status || s.L_Detail == c_status) &&
                            //           (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                            //         orderby s.L_TID descending, s.C_EventTime descending
                            //         select new DevicesStatus
                            //         {
                            //             L_ID = t.L_ID,
                            //             L_TID = s.L_TID ?? 0,
                            //             C_Name = t.C_Name,
                            //             C_EventTime = s.C_EventTime.ToString(),
                            //             C_Office = t.C_Office ?? "",
                            //             C_Place = t.C_Place ?? "",
                            //             //D_EventTime = (s.C_EventTime * 1),
                            //             L_Class = s.L_Class.ToString(),
                            //             L_Detail = s.L_Detail.ToString()
                            //         }).Count();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)

                            count = 1;

                            dbList = (from t in db.tTerminals
                                      join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                                      //let eTime = decimalParse(s.C_EventTime)
                                      where
                                        s.L_TID == dNumber &&
                                        s.L_Class == 1 &&
                                        (s.L_Detail == d_status || s.L_Detail == c_status) &&
                                        (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                                      orderby s.L_TID descending, s.C_EventTime descending
                                      select new DevicesStatus
                                      {
                                          L_ID = t.L_ID,
                                          L_TID = s.L_TID ?? 0,
                                          C_Name = t.C_Name,
                                          C_EventTime = s.C_EventTime.ToString(),
                                          C_Office = t.C_Office ?? "",
                                          C_Place = t.C_Place ?? "",
                                          //D_EventTime = (s.C_EventTime * 1),
                                          L_Class = s.L_Class.ToString(),
                                          L_Detail = s.L_Detail.ToString()
                                      }).ToList();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)
                        }
                        //dbList.Sort();
                    }

                    string strDateTime = "";
                    int iCurrDevice = 0, iNextDevice = 0;
                    bool dAdd = true;

                    if (dbList.Count > 0)
                    {
                        for (int i = 0; i < dbList.Count; i++)
                        {
                            strDateTime = ApplyDateTimeFormat(dbList[i].C_EventTime);
                            iCurrDevice = dbList[i].L_ID;

                            if (i <= (dbList.Count - 2))
                                iNextDevice = dbList[i + 1].L_ID;

                            if (dAdd)
                            {
                                dList.Add(new DevicesStatus()
                                {
                                    L_ID = dbList[i].L_ID,
                                    L_TID = dbList[i].L_TID,
                                    C_Name = dbList[i].C_Name,
                                    C_EventTime = strDateTime,
                                    C_Office = dbList[i].C_Office,
                                    C_Place = dbList[i].C_Place,
                                    //D_EventTime = ApplyDateTimeFormat(item.C_EventTime),
                                    L_Class = dbList[i].L_Class,
                                    L_Detail = dbList[i].L_Detail //((item.L_Detail == "11") ? "Disconnected" : ((item.L_Detail == "12") ? "Connected" : "-"))
                                });

                                if (device_number != null)
                                    break;
                            }

                            if (iCurrDevice == iNextDevice)
                            {
                                dAdd = false;
                            }
                            else
                            {
                                dAdd = true;
                            }
                        }


                        //for (int i = 0; i < dbList.Count; i++)
                        //{
                        //    strDateTime = ApplyDateTimeFormat(dbList[i].C_EventTime);
                        //    strCurrDate = strDateTime;

                        //    if (i <= (dbList.Count - 2))
                        //        strNextDate = ApplyDateTimeFormat(dbList[i + 1].C_EventTime);

                        //    if (dAdd)
                        //    {
                        //        dList.Add(new DevicesStatus()
                        //        {
                        //            L_ID = dbList[i].L_ID,
                        //            L_TID = dbList[i].L_TID,
                        //            C_Name = dbList[i].C_Name,
                        //            C_EventTime = strDateTime,
                        //            //D_EventTime = ApplyDateTimeFormat(item.C_EventTime),
                        //            L_Class = dbList[i].L_Class,
                        //            L_Detail = dbList[i].L_Detail //((item.L_Detail == "11") ? "Disconnected" : ((item.L_Detail == "12") ? "Connected" : "-"))
                        //        });                                
                        //    }

                        //    if (strCurrDate.Split(' ')[0] == strNextDate.Split(' ')[0])
                        //    {
                        //        dAdd = false;
                        //    }
                        //    else
                        //    {
                        //        dAdd = true;
                        //    }
                        //}

                    }

                    //dList.Reverse();

                    //count = count / dList.Count();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }
        */

        public static string ApplyDateTimeFormat(string dtime)
        {
            string[] dtArray = new string[6];
            string strDateTime = string.Empty;
            DateTime date_time = DateTime.Now;

            if (dtime.Length == 14)
            {
                dtArray[0] = dtime.Substring(0, 4);
                dtArray[1] = dtime.Substring(4, 2);
                dtArray[2] = dtime.Substring(6, 2);
                dtArray[3] = dtime.Substring(8, 2);
                dtArray[4] = dtime.Substring(10, 2);
                dtArray[5] = dtime.Substring(12, 2);

                strDateTime = dtArray[0] + "-" + dtArray[1] + "-" + dtArray[2] + " " + dtArray[3] + ":" + dtArray[4] + ":" + dtArray[5] + ".000";
                date_time = Convert.ToDateTime(strDateTime);
                strDateTime = date_time.ToString("dd-MM-yyyy hh:mm tt");

                //10-10-2017 12:22:09
            }
            else
            {
                strDateTime = "-";
            }

            //2016-10-10-12-22-09

            return strDateTime;
        }

        /*
        public static int getDevicesListExceldownload(string device_number, string from, string to, string dcStatus, int start, int length, out List<DevicesStatusExcelDownload> dList)
        {
            int count = 0, d_status = 11, c_status = 12;
            var dbList = new List<DevicesStatus>();
            dList = new List<DevicesStatusExcelDownload>();
            var temp = new List<DevicesStatus>();

            int dNumber = 0;

            if (!int.TryParse(device_number, out dNumber))
            {
                dNumber = -1;
            }

            decimal firstDay = 0;
            decimal lastDay = 0;

            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    string[] splitFromDate = from.Split('/');
                    firstDay = decimal.Parse(splitFromDate[2] + splitFromDate[0] + splitFromDate[1] + "000000");

                    string[] splitToDate = to.Split('/');
                    lastDay = decimal.Parse(splitToDate[2] + splitToDate[0] + splitToDate[1] + "235959");
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                //Added by Inayat 7th Dec 2017
                if (device_number == null && from == null && to == null)
                {
                    firstDay = decimal.Parse(DateTime.Now.ToString("yyyyMMdd000000"));
                    lastDay = decimal.Parse(DateTime.Now.AddDays(1).ToString("yyyyMMdd235959"));
                }
            }

            try
            {
                using (UNISContext db = new UNISContext())
                {
                    //var dbList1 = db.tTerminals
                    //                .Join(db.tTerminalStateLogs,
                    //                      t => t.L_ID,
                    //                      s => s.L_TID,
                    //                      (t, s) => new DevicesStatus
                    //                      {
                    //                          L_ID = t.L_ID,
                    //                          L_TID = s.L_TID ?? 0,
                    //                          C_Name = t.C_Name,
                    //                          C_EventTime = s.C_EventTime.ToString(),
                    //                          D_EventTime = s.C_EventTime,
                    //                          L_Class = s.L_Class.ToString(),
                    //                          L_Detail = s.L_Detail.ToString()
                    //                      })
                    //    //.Select(p => new DevicesStatus
                    //    //{
                    //    //    L_ID = p.L_ID,
                    //    //    L_TID = p.L_TID,
                    //    //    C_Name = p.C_Name,
                    //    //    C_EventTime = p.C_EventTime,
                    //    //    L_Class = p.L_Class.ToString(),
                    //    //    L_Detail = p.L_Detail.ToString()
                    //    //})
                    //                      .Where(w => w.L_Class == "1" && w.L_ID == dNumber && (w.D_EventTime >= firstDay && w.D_EventTime <= lastDay))
                    //                      .OrderBy(o => sortOrder).Skip(start).Take(length).ToList();

                    //sortOrder = "s." + sortOrder.Replace("DESC", "descending");
                    if (dcStatus == null)
                    {
                        d_status = 11;
                        c_status = 12;
                    }
                    else
                    {
                        if (dcStatus == "11")//inactive or disconnected
                        {
                            d_status = 11;
                            c_status = 11;
                        }
                        else if (dcStatus == "12")//active or connected
                        {
                            d_status = 12;
                            c_status = 12;
                        }
                        else
                        {
                            d_status = 11;
                            c_status = 12;
                        }
                    }

                    if (from != null && to != null)
                    {
                        if (device_number == null)
                        {

                            //List<tTerminalStateLog> tt = db.tTerminalStateLogs
                            //   .Where(s => s.L_Class == 1 && (s.L_Detail == d_status || s.L_Detail == c_status) && (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay))
                            //   .GroupBy(l => l.L_TID)
                            //   .Select(g => g.OrderByDescending(c => c.C_EventTime).FirstOrDefault()).ToList();



                            //count = (from t in db.tTerminals
                            //         join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                            //         //let eTime = decimalParse(s.C_EventTime)
                            //         where
                            //             //s.L_TID == dNumber && 
                            //        s.L_Class == 1 &&
                            //        (s.L_Detail == d_status || s.L_Detail == c_status) &&
                            //        (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                            //         orderby s.L_TID descending, s.C_EventTime descending
                            //         select new DevicesStatus
                            //         {
                            //             L_ID = t.L_ID,
                            //             L_TID = s.L_TID ?? 0,
                            //             C_Name = t.C_Name,
                            //             C_EventTime = s.C_EventTime.ToString(),
                            //             C_Office = t.C_Office ?? "",
                            //             C_Place = t.C_Place ?? "",
                            //             //D_EventTime = (s.C_EventTime * 1),
                            //             L_Class = s.L_Class.ToString(),
                            //             L_Detail = s.L_Detail.ToString()
                            //         }).Count();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)

                            count = (from t in db.tTerminals
                                     join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                                     //let eTime = decimalParse(s.C_EventTime)
                                     where
                                         //s.L_TID == dNumber && 
                                    s.L_Class == 1 &&
                                    (s.L_Detail == d_status || s.L_Detail == c_status) &&
                                    (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                                     //orderby s.L_TID //, s.C_EventTime descending
                                     select new DevicesStatus
                                     {
                                         L_ID = t.L_ID,
                                         L_TID = s.L_TID ?? 0,
                                         C_Name = t.C_Name,
                                         C_EventTime = s.C_EventTime.ToString(),
                                         C_Office = t.C_Office ?? "",
                                         C_Place = t.C_Place ?? "",
                                         //D_EventTime = (s.C_EventTime * 1),
                                         L_Class = s.L_Class.ToString(),
                                         L_Detail = s.L_Detail.ToString()
                                     }).Count();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)

                            dbList = (from t in db.tTerminals
                                      join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                                      //let eTime = decimalParse(s.C_EventTime)
                                      where
                                          //s.L_TID == dNumber && 
                                     s.L_Class == 1 &&
                                     (s.L_Detail == d_status || s.L_Detail == c_status) &&
                                     (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                                      orderby s.L_TID descending, s.C_EventTime descending
                                      select new DevicesStatus
                                      {
                                          L_ID = t.L_ID,
                                          L_TID = s.L_TID ?? 0,
                                          C_Name = t.C_Name,
                                          C_EventTime = s.C_EventTime.ToString(),
                                          C_Office = t.C_Office ?? "",
                                          C_Place = t.C_Place ?? "",
                                          //D_EventTime = (s.C_EventTime * 1),
                                          L_Class = s.L_Class.ToString(),
                                          L_Detail = s.L_Detail.ToString()
                                      }).Skip(start).Take(length).ToList();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)
                        }
                        else
                        {
                            //count = (from t in db.tTerminals
                            //         join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                            //         //let eTime = decimalParse(s.C_EventTime)
                            //         where
                            //           s.L_TID == dNumber &&
                            //           s.L_Class == 1 &&
                            //           (s.L_Detail == d_status || s.L_Detail == c_status) &&
                            //           (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                            //         orderby s.L_TID descending, s.C_EventTime descending
                            //         select new DevicesStatus
                            //         {
                            //             L_ID = t.L_ID,
                            //             L_TID = s.L_TID ?? 0,
                            //             C_Name = t.C_Name,
                            //             C_EventTime = s.C_EventTime.ToString(),
                            //             C_Office = t.C_Office ?? "",
                            //             C_Place = t.C_Place ?? "",
                            //             //D_EventTime = (s.C_EventTime * 1),
                            //             L_Class = s.L_Class.ToString(),
                            //             L_Detail = s.L_Detail.ToString()
                            //         }).Count();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)

                            count = 1;

                            dbList = (from t in db.tTerminals
                                      join s in db.tTerminalStateLogs on t.L_ID equals s.L_TID
                                      //let eTime = decimalParse(s.C_EventTime)
                                      where
                                        s.L_TID == dNumber &&
                                        s.L_Class == 1 &&
                                        (s.L_Detail == d_status || s.L_Detail == c_status) &&
                                        (s.C_EventTime >= firstDay && s.C_EventTime <= lastDay)
                                      orderby s.L_TID descending, s.C_EventTime descending
                                      select new DevicesStatus
                                      {
                                          L_ID = t.L_ID,
                                          L_TID = s.L_TID ?? 0,
                                          C_Name = t.C_Name,
                                          C_EventTime = s.C_EventTime.ToString(),
                                          C_Office = t.C_Office ?? "",
                                          C_Place = t.C_Place ?? "",
                                          //D_EventTime = (s.C_EventTime * 1),
                                          L_Class = s.L_Class.ToString(),
                                          L_Detail = s.L_Detail.ToString()
                                      }).ToList();//.OrderByDescending(o => o.L_TID).OrderByDescending(o => o.C_EventTime)
                        }
                        //dbList.Sort();
                    }

                    string strDateTime = "";
                    int iCurrDevice = 0, iNextDevice = 0;
                    bool dAdd = true;

                    if (dbList.Count > 0)
                    {
                        for (int i = 0; i < dbList.Count; i++)
                        {
                            strDateTime = ApplyDateTimeFormat(dbList[i].C_EventTime);
                            iCurrDevice = dbList[i].L_ID;

                            if (i <= (dbList.Count - 2))
                                iNextDevice = dbList[i + 1].L_ID;

                            if (dAdd)
                            {
                                dList.Add(new DevicesStatusExcelDownload()
                                {
                                    Device_Id = dbList[i].L_ID.ToString(),
                                    Date_Time = ApplyDateTimeFormat(dbList[i].C_EventTime),
                                    Device_Name = dbList[i].C_Name,
                                    Branch = dbList[i].C_Office,
                                    Location = dbList[i].C_Place,
                                    Device_Status = ((dbList[i].L_Detail == "11") ? "Inactive" : ((dbList[i].L_Detail == "12") ? "Active" : "-"))
                                });

                                if (device_number != null)
                                    break;
                            }

                            if (iCurrDevice == iNextDevice)
                            {
                                dAdd = false;
                            }
                            else
                            {
                                dAdd = true;
                            }
                        }


                        //for (int i = 0; i < dbList.Count; i++)
                        //{
                        //    strDateTime = ApplyDateTimeFormat(dbList[i].C_EventTime);
                        //    strCurrDate = strDateTime;

                        //    if (i <= (dbList.Count - 2))
                        //        strNextDate = ApplyDateTimeFormat(dbList[i + 1].C_EventTime);

                        //    if (dAdd)
                        //    {
                        //        dList.Add(new DevicesStatus()
                        //        {
                        //            L_ID = dbList[i].L_ID,
                        //            L_TID = dbList[i].L_TID,
                        //            C_Name = dbList[i].C_Name,
                        //            C_EventTime = strDateTime,
                        //            //D_EventTime = ApplyDateTimeFormat(item.C_EventTime),
                        //            L_Class = dbList[i].L_Class,
                        //            L_Detail = dbList[i].L_Detail //((item.L_Detail == "11") ? "Disconnected" : ((item.L_Detail == "12") ? "Connected" : "-"))
                        //        });                                
                        //    }

                        //    if (strCurrDate.Split(' ')[0] == strNextDate.Split(' ')[0])
                        //    {
                        //        dAdd = false;
                        //    }
                        //    else
                        //    {
                        //        dAdd = true;
                        //    }
                        //}

                    }

                    //dList.Reverse();

                    //count = count / dList.Count();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return count;
        }
        */

        public static ViewModels.Terminal[] getAllDeviceNumbersMatching(string device_number)
        {
            using (var db = new UNISContext())
            {
                // get all employees with no groups
                tTerminal[] terminals = db.tTerminals.Where(m => m.C_Name.ToLower().Contains(device_number)).ToArray();

                ViewModels.Terminal[] toReturn = new ViewModels.Terminal[terminals.Length];

                for (int i = 0; i < terminals.Length; i++)
                {
                    var t = new Terminal()
                    {
                        L_ID = terminals[i].L_ID,
                        C_Name = terminals[i].C_Name
                    };

                    toReturn[i] = t;//TimeTune.EmployeeCrud.convert(terminals[i]);
                }

                return toReturn;
            }
        }

        #endregion
    }
}
