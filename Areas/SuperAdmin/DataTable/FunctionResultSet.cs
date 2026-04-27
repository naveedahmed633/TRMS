using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ViewModels;

namespace MvcApplication1.Areas.SuperAdmin.DataTable
{
    //public class HR_UNISEnterResultSet
    //{
    //    public static List<BLL_UNIS.ViewModels.EnterArchive> GetResult(string search, string sortOrder, int start, int length, List<BLL_UNIS.ViewModels.EnterArchive> dtResult)
    //    {
    //        return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
    //    }

    //    public static int Count(string search, List<BLL_UNIS.ViewModels.EnterArchive> dtResult)
    //    {
    //        return FilterResult(search, dtResult).Count();
    //    }

    //    private static IQueryable<BLL_UNIS.ViewModels.EnterArchive> FilterResult(string search, List<BLL_UNIS.ViewModels.EnterArchive> dtResult)
    //    {
    //        IQueryable<BLL_UNIS.ViewModels.EnterArchive> results = dtResult.AsQueryable();


    //        results = results.Where(p =>
    //            (
    //                search == null ||
    //                (
    //                     (p.C_Unique != null && p.C_Unique.ToString().Contains(search.ToLower())) ||
    //                     //(p.L_UID != null && p.L_UID.ToString().Contains(search.ToLower())) ||
    //                     (p.C_DateTime != null && p.C_DateTime.ToString("yyyy-MM-dd").Contains(search)) ||
    //                     (p.C_Name != null && p.C_Name.ToString().ToLower().Contains(search.ToLower()))
    //                //p.C_IPAddr.Contains(search)||
    //                //p.C_Office.Contains(search)||
    //                //p.C_Place.Contains(search)

    //                )
    //            ));

    //        return results;
    //    }
    //}

    //public class HR_UNISTerminalStateLogResultSet
    //{
    //    public static List<BLL_UNIS.ViewModels.TerminalStateLogArchive> GetResult(string search, string sortOrder, int start, int length, List<BLL_UNIS.ViewModels.TerminalStateLogArchive> dtResult)
    //    {
    //        return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
    //    }

    //    public static int Count(string search, List<BLL_UNIS.ViewModels.TerminalStateLogArchive> dtResult)
    //    {
    //        return FilterResult(search, dtResult).Count();
    //    }

    //    private static IQueryable<BLL_UNIS.ViewModels.TerminalStateLogArchive> FilterResult(string search, List<BLL_UNIS.ViewModels.TerminalStateLogArchive> dtResult)
    //    {
    //        IQueryable<BLL_UNIS.ViewModels.TerminalStateLogArchive> results = dtResult.AsQueryable();


    //        results = results.Where(p =>
    //            (
    //                search == null ||
    //                (
    //                     (p.L_TID != null && p.L_TID.ToString().Contains(search.ToLower())) ||
    //                     //(p.L_UID != null && p.L_UID.ToString().Contains(search.ToLower())) ||
    //                     (p.C_EventDateTime != null && p.C_EventDateTime.ToString("yyyy-MM-dd").Contains(search)) ||
    //                     (p.L_Detail != null && p.L_Detail.ToString().ToLower().Contains(search.ToLower()))
    //                //p.C_IPAddr.Contains(search)||
    //                //p.C_Office.Contains(search)||
    //                //p.C_Place.Contains(search)

    //                )
    //            ));

    //        return results;
    //    }
    //}

    public class HR_MarkSheetResultSet
    {
        public static List<BLL_UNIS.ViewModels.DeviceStatusReport> GetResult(string search, string sortOrder, int start, int length, List<BLL_UNIS.ViewModels.DeviceStatusReport> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<BLL_UNIS.ViewModels.DeviceStatusReport> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<BLL_UNIS.ViewModels.DeviceStatusReport> FilterResult(string search, List<BLL_UNIS.ViewModels.DeviceStatusReport> dtResult)
        {
            IQueryable<BLL_UNIS.ViewModels.DeviceStatusReport> results = dtResult.AsQueryable();


            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.L_ID != null && p.L_ID.ToString().ToLower().Contains(search.ToLower()) ||
                         p.C_EventTime.Contains(search) ||
                         p.L_Detail.ToLower().Contains(search.ToLower())
                    //p.C_IPAddr.Contains(search)||
                    //p.C_Office.Contains(search)||
                    //p.C_Place.Contains(search)

                    )
                ));

            return results;
        }
    }

    public class HR_DevicesStatusRegionResultSet
    {
        public static List<BLL_UNIS.ViewModels.DeviceStatusReport> GetResult(string search, string sortOrder, int start, int length, List<BLL_UNIS.ViewModels.DeviceStatusReport> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<BLL_UNIS.ViewModels.DeviceStatusReport> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<BLL_UNIS.ViewModels.DeviceStatusReport> FilterResult(string search, List<BLL_UNIS.ViewModels.DeviceStatusReport> dtResult)
        {
            IQueryable<BLL_UNIS.ViewModels.DeviceStatusReport> results = dtResult.AsQueryable();


            results = results.Where(p =>
                (
                    search == null ||
                    (
                         p.L_ID != null && p.L_ID.ToString().ToLower().Contains(search.ToLower()) ||
                         p.C_EventTime.Contains(search) ||
                         p.L_Detail.ToLower().Contains(search.ToLower())
                    //p.C_IPAddr.Contains(search)||
                    //p.C_Office.Contains(search)||
                    //p.C_Place.Contains(search)

                    )
                ));

            return results;
        }
    }

}