using DLL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTune;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ViewModels;

namespace ViewModels
{
    /*
     * This View is used to load the finalized present
     * and absent report
     */

    public class LeaveApplicationInfo
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string UsersName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeText { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateText { get; set; }
        public DateTime ToDate { get; set; }
        public string ToDateText { get; set; }
        public int DaysCount { get; set; }
        public int LeaveReasonId { get; set; }
        public string LeaveReasonText { get; set; }
        public string ReasonDetail { get; set; }
        public int ApproverId { get; set; }
        public string ApproverCode { get; set; }
        public string ApproverName { get; set; }
        public string ApproverDetail { get; set; }
        public int LeaveStatusId { get; set; }
        public string LeaveStatusText { get; set; }



        public int LeaveStautsHODId { get; set; }
        public string LeaveStatusHODText { get; set; }
        public int LeaveStautsPRNId { get; set; }
        public string LeaveStatusPRNText { get; set; }
        public int LeaveStautsHRId { get; set; }
        public string LeaveStatusHRText { get; set; }
        public int LeaveStautsVCId { get; set; }
        public string LeaveStatusVCText { get; set; }



        public string AttachmentFilePath { get; set; }
        public bool IsActive { get; set; }

        public int LeaveValidityId { get; set; }
        public string LeaveValidityRemarks { get; set; }

        public string IsHODApproved { get; set; }
        public string IsPRNApproved { get; set; }
        public string IsHRApproved { get; set; }
        public string IsVCApproved { get; set; }


        public DateTime CreateDateTime { get; set; }
        public string CreateDateText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateText { get; set; }

        public string actions { get; set; }
    }

    public class LeaveApplicationResultSet
    {
        public static int GetLeaveIdByName(string leave_name)
        {
            int leave_id = 0;
            leave_name = leave_name.ToLower();

            using (Context db = new Context())
            {
                leave_id = db.leave_type.Where(e => e.LeaveTypeText.ToLower() == leave_name).FirstOrDefault().Id;
            }

            return leave_id;
        }

        public static List<ViewModels.LeaveApplicationInfo> getAllApproveApplicationsForSA(string user_code)
        {
            int userID = 0, isHRUser = 0;
            string strLeaveCodeText = string.Empty; string strAttPath = string.Empty;
            List<ViewModels.LeaveApplicationInfo> toReturn = new List<ViewModels.LeaveApplicationInfo>();
            List<LeaveApplication> lApplication = new List<LeaveApplication>();

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        userID = data_emp.EmployeeId;

                        DateTime dtFrom = DateTime.Now.AddMonths(-12);
                        DateTime dtTo = DateTime.Now.AddMonths(12);

                        lApplication = db.leave_application.Where(l => (l.FromDate >= dtFrom && l.ToDate <= dtTo)).OrderByDescending(l => l.Id).ToList();
                        if (lApplication != null && lApplication.Count > 0)
                        {
                            for (int i = 0; i < lApplication.Count(); i++)
                            {
                                int appr_id = lApplication[i].ApproverId;
                                int user_id = lApplication[i].EmployeeId;
                                var approver_name = db.employee.Where(e => e.EmployeeId == appr_id).FirstOrDefault();
                                var users_name = db.employee.Where(e => e.EmployeeId == user_id).FirstOrDefault();
                                isHRUser = 1;

                                if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                                {
                                    if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                    {
                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                            "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                        "</span>";
                                    }
                                    else
                                    {
                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                            "</span>";
                                    }
                                }
                                else
                                {
                                    strAttPath = "-";
                                }

                                //if (lApplication[i].LeaveCodeId == 2)
                                //{
                                //    strLeaveCodeText = "Short";
                                //}
                                //else if (lApplication[i].LeaveCodeId == 3)
                                //{
                                //    strLeaveCodeText = "Half";
                                //}
                                //else
                                //{
                                //    strLeaveCodeText = "Norm";
                                //}

                                DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };
                                dt = LeaveApplicationResultSet.getUserSessionDatesByUserCode(users_name.employee_code);

                                int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                                leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(users_name.employee_code);

                                int ltype = 0; string leaveTypeText = "";
                                ltype = lApplication[i].LeaveTypeId;
                                leaveTypeText = db.leave_type.Where(t => t.Id == ltype).FirstOrDefault().LeaveTypeText;

                                int lreason = 0; string leaveReasonText = "";
                                lreason = lApplication[i].LeaveReasonId;
                                leaveReasonText = db.leave_reason.Where(r => r.Id == lreason).FirstOrDefault().LeaveReasonText;

                                toReturn.Add(new ViewModels.LeaveApplicationInfo()
                                {
                                    Id = lApplication[i].Id,
                                    LeaveTypeId = lApplication[i].LeaveTypeId,
                                    LeaveTypeText = leaveTypeText,
                                    FromDateText = lApplication[i].FromDate.ToString("dd-MMM-yyyy"),
                                    ToDateText = lApplication[i].ToDate.ToString("dd-MMM-yyyy"),
                                    //LeaveCodeText = strLeaveCodeText,
                                    DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                    LeaveReasonText = leaveReasonText, //lApplication[i].LeaveReason.LeaveReasonText,
                                    ReasonDetail = lApplication[i].ReasonDetail,
                                    EmployeeId = lApplication[i].EmployeeId,
                                    EmployeeCode = users_name.employee_code,
                                    UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                    ApproverId = lApplication[i].ApproverId,
                                    ApproverCode = approver_name.employee_code,
                                    ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                    LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                                    ApproverDetail = lApplication[i].ApproverDetail,
                                    //HRComments = lApplication[i].HRComments,
                                    IsActive = lApplication[i].IsActive,
                                    IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                                    IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                                    IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                                    IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                                    CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MMM-yyyy"),
                                    UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MMM-yyyy"),
                                    AttachmentFilePath = strAttPath,
                                    actions =
                                        "<span data-row='" + lApplication[i].Id + "'>" +
                                        "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + users_name.access_group.AccessGroupId + "," + users_name.grade.GradeId + "," + users_name.site_id + "," + lApplication[i].LeaveTypeId + "," + lApplication[i].LeaveStatusHODId + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                          //"<span> / </span>" +
                                          (lApplication[i].LeaveStatusId == 1 ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") +
                                        "</span>"
                                });

                                strLeaveCodeText = ""; strAttPath = ""; isHRUser = 0;
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lApplication = new List<LeaveApplication>();
                }
            }

            return toReturn;
        }

        public static List<ViewModels.LeaveApplicationInfo> getAll()
        {
            using (Context db = new Context())
            {
                List<LeaveApplication> lApplication = null;
                try
                {
                    lApplication = db.leave_application.Where(m => m.IsActive).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lApplication = new List<LeaveApplication>();
                }

                List<ViewModels.LeaveApplicationInfo> toReturn = new List<ViewModels.LeaveApplicationInfo>();

                if (lApplication != null && lApplication.Count() > 0)
                {
                    for (int i = 0; i < lApplication.Count(); i++)
                    {
                        int user_id = lApplication[i].EmployeeId;
                        var users_name = db.employee.Where(e => e.active && e.EmployeeId == user_id).FirstOrDefault();

                        int appr_id = lApplication[i].ApproverId;
                        var approver_name = db.employee.Where(e => e.active && e.EmployeeId == appr_id).FirstOrDefault();

                        toReturn.Add(new ViewModels.LeaveApplicationInfo()
                        {
                            Id = lApplication[i].Id,
                            LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                            FromDateText = lApplication[i].FromDate.ToString("yyyy-MM-dd"),
                            ToDateText = lApplication[i].ToDate.ToString("yyyy-MM-dd"),
                            DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                            LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                            ReasonDetail = lApplication[i].ReasonDetail,
                            EmployeeId = lApplication[i].EmployeeId,
                            EmployeeCode = users_name.employee_code,
                            ApproverId = lApplication[i].ApproverId,
                            ApproverCode = approver_name.employee_code,
                            ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                            LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                            ApproverDetail = lApplication[i].ApproverDetail,
                            IsActive = lApplication[i].IsActive,
                            IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                            IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                            IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                            IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                            CreateDateText = lApplication[i].CreateDateTime.ToString("yyyy-MM-dd"),
                            UpdateDateText = lApplication[i].UpdateDateTime.ToString("yyyy-MM-dd"),
                            AttachmentFilePath = (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "") ? "<span data-row='" + lApplication[i].Id + "'>" +
                                    "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                "</span>" : "--",
                            actions =
                                "<span data-row='" + lApplication[i].Id + "'>" +
                                    "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + users_name.access_group.AccessGroupId + "," + users_name.grade.GradeId + "," + users_name.site_id + "," + lApplication[i].LeaveTypeId + "," + lApplication[i].LeaveStatusHODId + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                    "<span> / </span>" +
                                    "<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                "</span>"
                        });
                    }
                }
                return toReturn;
            }
        }

        public static List<ViewModels.LeaveApplicationInfo> getLeavesApplicationsByUserCode(string user_code)
        {
            int user_id_emp = 0;

            using (Context db = new Context())
            {
                List<LeaveApplication> lApplication = new List<LeaveApplication>();
                try
                {
                    user_id_emp = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    var dbSessionData = db.leave_session.Where(l => l.EmployeeId == user_id_emp).OrderByDescending(o => o.id).FirstOrDefault();
                    if (dbSessionData != null)
                    {
                        DateTime dtEnd = dbSessionData.SessionEndDate.AddHours(23).AddMinutes(59);
                        lApplication = db.leave_application.Where(l => l.EmployeeId == user_id_emp && l.IsActive/* && (l.FromDate >= dbSessionData.SessionStartDate && l.ToDate <= dtEnd)*/).OrderByDescending(l => l.Id).ToList();
                        //lApplication = db.leave_application.Where(m => m.UserId == user_id).ToList();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lApplication = new List<LeaveApplication>();
                }

                string strAttPath = string.Empty;
                List<ViewModels.LeaveApplicationInfo> toReturn = new List<ViewModels.LeaveApplicationInfo>();

                if (lApplication != null && lApplication.Count() > 0)
                {
                    for (int i = 0; i < lApplication.Count(); i++)
                    {
                        int appr_id = lApplication[i].ApproverId;
                        int user_id = lApplication[i].EmployeeId;
                        var approver_name = db.employee.Where(e => e.active && e.EmployeeId == appr_id).FirstOrDefault();
                        var users_name = db.employee.Where(e => e.active && e.EmployeeId == user_id).FirstOrDefault();

                        if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                        {
                            if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                            {
                                strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                    "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                "</span>";
                            }
                            else
                            {
                                strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                        "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                    "</span>";
                            }
                        }
                        else
                        {
                            strAttPath = "-";
                        }

                        toReturn.Add(new ViewModels.LeaveApplicationInfo()
                        {
                            Id = lApplication[i].Id,
                            LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                            FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                            ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                            DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                            LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                            ReasonDetail = lApplication[i].ReasonDetail,
                            EmployeeId = lApplication[i].EmployeeId,
                            EmployeeCode = users_name.employee_code,
                            UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                            ApproverId = lApplication[i].ApproverId,
                            //ApproverCode = users_name.employee_code,
                            //ApproverName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                            ApproverCode = approver_name.employee_code,
                            ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                            LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                            ApproverDetail = lApplication[i].ApproverDetail,
                            IsActive = lApplication[i].IsActive,
                            IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                            IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                            IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                            IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                            CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                            UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                            AttachmentFilePath = strAttPath,
                            actions =

                            "<span data-row='" + lApplication[i].Id + "'>" +
                               // "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                            //"<span> / </span>" +
                            (lApplication[i].LeaveStatusId == 1 ? "<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "-") +
                            "</span>"
                            //actions =
                            //    "<span data-row='" + lApplication[i].Id + "'>" +
                            //        "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                            //    //"<span> / </span>" +
                            //    //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                            //    "</span>"
                        });
                    }
                }

                return toReturn;
            }
        }

        public static List<ViewModels.LeaveApplicationInfo> getApproveApplicationsByUserCodeForLM(string user_code)
        {
            int emp_id = 0;
            DLL.Models.Employee emp_data = null;

            using (Context db = new Context())
            {
                List<LeaveApplication> lApplication = new List<LeaveApplication>();

                DateTime dtFrom = new DateTime(DateTime.Now.AddMonths(-2).Year, DateTime.Now.AddMonths(-2).Month, 1);
                DateTime dtTo = new DateTime(DateTime.Now.AddMonths(3).Year, DateTime.Now.AddMonths(3).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(3).Year, DateTime.Now.AddMonths(3).Month));

                try
                {
                    emp_data = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                    emp_id = emp_data.EmployeeId;

                    //var dbAcademicYear = db.academic_year.Where(l => l.IsCurrentSession).OrderByDescending(o => o.id).FirstOrDefault();
                    //if (dbAcademicYear != null)
                    {
                        lApplication = db.leave_application.Where(l => l.IsActive /*&& (l.FromDate >= dtFrom && l.ToDate <= dtTo)*/ && l.ApproverId == emp_id).OrderByDescending(l => l.Id).ToList();//// && (l.FromDate >= dbAcademicYear.SessionStartDate && l.ToDate <= dbAcademicYear.SessionEndDate)).OrderByDescending(l => l.Id).ToList();
                        //lApplication = db.leave_application.Where(m => m.UserId == user_id).ToList();
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lApplication = new List<LeaveApplication>();
                }

                string strAttPath = string.Empty;
                List<ViewModels.LeaveApplicationInfo> toReturn = new List<ViewModels.LeaveApplicationInfo>();

                for (int i = 0; i < lApplication.Count(); i++)
                {
                    int appr_id = lApplication[i].ApproverId;
                    int user_id = lApplication[i].EmployeeId;
                    var approver_name = db.employee.Where(e => e.active && e.EmployeeId == appr_id).FirstOrDefault();
                    var users_name = db.employee.Where(e => e.active && e.EmployeeId == user_id).FirstOrDefault();


                    if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                    {
                        if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                        {
                            strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                            "</span>";
                        }
                        else
                        {
                            strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                    "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                "</span>";
                        }
                    }
                    else
                    {
                        strAttPath = "-";
                    }

                    toReturn.Add(new ViewModels.LeaveApplicationInfo()
                    {
                        Id = lApplication[i].Id,
                        LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                        FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                        ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                        DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                        LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                        ReasonDetail = lApplication[i].ReasonDetail,
                        EmployeeId = lApplication[i].EmployeeId,
                        EmployeeCode = users_name.employee_code,
                        UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                        ApproverId = lApplication[i].ApproverId,
                        ApproverCode = approver_name.employee_code,
                        ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                        LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                        ApproverDetail = lApplication[i].ApproverDetail,
                        IsActive = lApplication[i].IsActive,
                        IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                        IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                        IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                        IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                        CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                        UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                        AttachmentFilePath = strAttPath,
                        actions =
                            "<span data-row='" + lApplication[i].Id + "'>" +//DONE LM 02
                                "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + emp_data.access_group.AccessGroupId + ","+users_name.site_id+"," + users_name.grade.GradeId + "," +emp_data.site_id/* users_name.site_id*/ + "," + lApplication[i].LeaveTypeId + "," + lApplication[i].LeaveStatusHODId + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                            //"<span> / </span>" +
                            (lApplication[i].LeaveStatusId == 1 ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") + //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                            "</span>"
                    });
                }

                return toReturn;
            }
        }

        public static List<ViewModels.LeaveApplicationInfo> getApproveApplicationsByUserCodeForHR(string user_code, int iGVEmpID, bool bGVIsSuperHRRole, int iGVCampusID)

        {
            int userID = 0; int current_user = 0;
            string strAttPath = string.Empty;
            List<ViewModels.LeaveApplicationInfo> toReturn = new List<ViewModels.LeaveApplicationInfo>();
            List<LeaveApplication> lApplication = new List<LeaveApplication>();

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                   // var a = data_emp.department.DepartmentId;
                    if (data_emp != null)
                    {
                        userID = data_emp.EmployeeId;

                        //var dbSessionData = db.leave_session.Where(l => l.UserId == 0 && l.YearId == DateTime.Now.Year).OrderByDescending(o => o.Id).FirstOrDefault();
                        //if (dbSessionData != null)
                        //{
                        DateTime dtFrom = new DateTime(DateTime.Now.Year, 1, 1);
                        DateTime dtTo = new DateTime(DateTime.Now.AddMonths(3).Year, DateTime.Now.AddMonths(3).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(3).Year, DateTime.Now.AddMonths(3).Month));

                        if (bGVIsSuperHRRole)
                        {
                            lApplication = db.leave_application.Where(l => l.IsActive /*&& (l.FromDate >= dtFrom && l.ToDate <= dtTo)*/).OrderByDescending(l => l.Id).ToList();
                        }
                        else
                        {
                            lApplication = (from l in db.leave_application
                                            join e in db.employee on l.EmployeeId equals e.EmployeeId
                                            where //(e.campus_id == iGVCampusID || l.ApproverId == iGVEmpID) &&
                                                    (l.FromDate >= dtFrom && l.ToDate <= dtTo) && l.IsActive
                                            select l).ToList();

                            //db.leave_application.Where(l => l.IsActive && (l.FromDate >= dtFrom && l.ToDate <= dtTo)).OrderByDescending(l => l.Id).ToList();
                        }

                        if (lApplication != null && lApplication.Count > 0)
                        {
                            for (int i = 0; i < lApplication.Count(); i++)
                            {
                                int appr_id = lApplication[i].ApproverId;
                                int user_id = lApplication[i].EmployeeId;

                                var approver_name = db.employee.Where(e => e.EmployeeId == appr_id).FirstOrDefault();
                                var users_name = db.employee.Where(e => e.EmployeeId == user_id).FirstOrDefault();
                                if (data_emp.site_id == 5)
                                {
                                    if (data_emp.campus_id == users_name.campus_id)
                                    {
                                        if (users_name.site_id == 4)
                                        {
                                            if (approver_name != null && users_name != null)
                                            {
                                                if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                                                {
                                                    if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                                    {
                                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                            "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                                        "</span>";
                                                    }
                                                    else
                                                    {
                                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                                "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                                            "</span>";
                                                    }
                                                }
                                                else
                                                {
                                                    strAttPath = "-";
                                                }



                                                toReturn.Add(new ViewModels.LeaveApplicationInfo()
                                                {
                                                    Id = lApplication[i].Id,
                                                    LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                                                    FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                                                    ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                                                    DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                                    LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                                                    ReasonDetail = lApplication[i].ReasonDetail,
                                                    EmployeeId = lApplication[i].EmployeeId,
                                                    EmployeeCode = users_name.employee_code,
                                                    UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                                    ApproverId = lApplication[i].ApproverId,
                                                    ApproverCode = approver_name.employee_code,
                                                    ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                                    LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                                                    ApproverDetail = lApplication[i].ApproverDetail,
                                                    IsActive = lApplication[i].IsActive,
                                                    IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                                                    IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                                                    IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                                                    IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                                                    CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                                                    UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                                                    AttachmentFilePath = strAttPath,
                                                    actions =
                                                        "<span data-row='" + lApplication[i].Id + "'>" +//DONE 01 - HR
                                                            "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + data_emp.access_group.AccessGroupId + "," + users_name.site_id + "," + users_name.grade.GradeId + "," + data_emp.site_id /*HttpContext.Current.Session["GV_SiteID"].ToString()*/ + "," + lApplication[i].LeaveTypeId + "," + /*lApplication[i].LeaveStatusHODId*/ 2 + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                                            //"<span> / </span>" +
                                                            ((lApplication[i].LeaveStatusId > 0 && lApplication[i].ToDate.Year == DateTime.Now.Year) ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") + //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                                        "</span>"
                                                });

                                            }
                                        }
                                      
                                        else if (users_name.site_id == 1)
                                        {
                                            if (lApplication[i].LeaveTypeId == 2 || (lApplication[i].LeaveTypeId >= 5/* && lApplication[i].LeaveTypeId <= 15*/))
                                            {
                                                //do nothing
                                            }
                                            else
                                            {


                                                if (approver_name != null && users_name != null)
                                                {
                                                    if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                                                    {
                                                        if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                                        {
                                                            strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                                "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                                            "</span>";
                                                        }
                                                        else
                                                        {
                                                            strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                                    "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                                                "</span>";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        strAttPath = "-";
                                                    }



                                                    toReturn.Add(new ViewModels.LeaveApplicationInfo()
                                                    {
                                                        Id = lApplication[i].Id,
                                                        LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                                                        FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                                                        ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                                                        DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                                        LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                                                        ReasonDetail = lApplication[i].ReasonDetail,
                                                        EmployeeId = lApplication[i].EmployeeId,
                                                        EmployeeCode = users_name.employee_code,
                                                        UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                                        ApproverId = lApplication[i].ApproverId,
                                                        ApproverCode = approver_name.employee_code,
                                                        ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                                        LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                                                        ApproverDetail = lApplication[i].ApproverDetail,
                                                        IsActive = lApplication[i].IsActive,
                                                        IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                                                        IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                                                        IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                                                        IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                                                        CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                                                        UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                                                        AttachmentFilePath = strAttPath,
                                                        actions =
                                                            "<span data-row='" + lApplication[i].Id + "'>" +//DONE 01 - HR
                                                                "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + data_emp.access_group.AccessGroupId + "," + users_name.site_id + "," + users_name.grade.GradeId + "," + data_emp.site_id /*HttpContext.Current.Session["GV_SiteID"].ToString()*/ + "," + lApplication[i].LeaveTypeId + "," + lApplication[i].LeaveStatusHODId + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                                                //"<span> / </span>" +
                                                                ((lApplication[i].LeaveStatusId > 0 && lApplication[i].ToDate.Year == DateTime.Now.Year) ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") + //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                                            "</span>"
                                                    });

                                                }
                                            }
                                        }
                                    }
                                }
                                else if (data_emp.site_id == 2 || data_emp.site_id == 6)
                                {
                                    if (users_name.site_id == 4)
                                    {
                                        if (lApplication[i].LeaveTypeId == 2 || (lApplication[i].LeaveTypeId >= 5))
                                        { /*nothing*/}
                                        else
                                        {
                                            if (approver_name != null && users_name != null)
                                            {
                                                if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                                                {
                                                    if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                                    {
                                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                            "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                                        "</span>";
                                                    }
                                                    else
                                                    {
                                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                                "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                                            "</span>";
                                                    }
                                                }
                                                else
                                                {
                                                    strAttPath = "-";
                                                }



                                                toReturn.Add(new ViewModels.LeaveApplicationInfo()
                                                {
                                                    Id = lApplication[i].Id,
                                                    LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                                                    FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                                                    ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                                                    DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                                    LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                                                    ReasonDetail = lApplication[i].ReasonDetail,
                                                    EmployeeId = lApplication[i].EmployeeId,
                                                    EmployeeCode = users_name.employee_code,
                                                    UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                                    ApproverId = lApplication[i].ApproverId,
                                                    ApproverCode = approver_name.employee_code,
                                                    ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                                    LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                                                    ApproverDetail = lApplication[i].ApproverDetail,
                                                    IsActive = lApplication[i].IsActive,
                                                    IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                                                    IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                                                    IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                                                    IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                                                    CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                                                    UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                                                    AttachmentFilePath = strAttPath,
                                                    actions =
                                                        "<span data-row='" + lApplication[i].Id + "'>" +//DONE 01 - HR
                                                            "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + data_emp.access_group.AccessGroupId + "," + users_name.site_id + "," + users_name.grade.GradeId + "," + data_emp.site_id /*HttpContext.Current.Session["GV_SiteID"].ToString()*/ + "," + lApplication[i].LeaveTypeId + "," + /*lApplication[i].LeaveStatusHODId*/ 2 + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                                            //"<span> / </span>" +
                                                            ((lApplication[i].LeaveStatusId > 0 && lApplication[i].ToDate.Year == DateTime.Now.Year) ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") + //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                                        "</span>"
                                                });

                                            }
                                        }
                                    }
                                    else if (users_name.site_id==5)
                                    {
                                        if (approver_name != null && users_name != null)
                                        {
                                            if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                                            {
                                                if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                                {
                                                    strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                        "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                                    "</span>";
                                                }
                                                else
                                                {
                                                    strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                            "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                                        "</span>";
                                                }
                                            }
                                            else
                                            {
                                                strAttPath = "-";
                                            }



                                            toReturn.Add(new ViewModels.LeaveApplicationInfo()
                                            {
                                                Id = lApplication[i].Id,
                                                LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                                                FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                                                ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                                                DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                                LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                                                ReasonDetail = lApplication[i].ReasonDetail,
                                                EmployeeId = lApplication[i].EmployeeId,
                                                EmployeeCode = users_name.employee_code,
                                                UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                                ApproverId = lApplication[i].ApproverId,
                                                ApproverCode = approver_name.employee_code,
                                                ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                                LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                                                ApproverDetail = lApplication[i].ApproverDetail,
                                                IsActive = lApplication[i].IsActive,
                                                IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                                                IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                                                IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                                                IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                                                CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                                                UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                                                AttachmentFilePath = strAttPath,
                                                actions =
                                                    "<span data-row='" + lApplication[i].Id + "'>" +//DONE 01 - HR
                                                        "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + data_emp.access_group.AccessGroupId + "," + users_name.site_id + "," + users_name.grade.GradeId + "," + data_emp.site_id /*HttpContext.Current.Session["GV_SiteID"].ToString()*/ + "," + lApplication[i].LeaveTypeId + "," + /*lApplication[i].LeaveStatusHODId*/ 2 + "," + /*lApplication[i].LeaveStatusPRNId*/2 + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                                        //"<span> / </span>" +
                                                        ((lApplication[i].LeaveStatusId > 0 && lApplication[i].ToDate.Year == DateTime.Now.Year) ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") + //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                                    "</span>"
                                            });

                                        }
                                    }
                                    else if (users_name.site_id == 1)
                                    {
                                        if (lApplication[i].LeaveTypeId == 2 || (lApplication[i].LeaveTypeId >= 5 ))
                                        {
                                            //do nothing
                                        }
                                        else
                                        {


                                            if (approver_name != null && users_name != null)
                                            {
                                                if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                                                {
                                                    if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                                    {
                                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                            "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                                        "</span>";
                                                    }
                                                    else
                                                    {
                                                        strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                                "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                                            "</span>";
                                                    }
                                                }
                                                else
                                                {
                                                    strAttPath = "-";
                                                }



                                                toReturn.Add(new ViewModels.LeaveApplicationInfo()
                                                {
                                                    Id = lApplication[i].Id,
                                                    LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                                                    FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                                                    ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                                                    DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                                    LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                                                    ReasonDetail = lApplication[i].ReasonDetail,
                                                    EmployeeId = lApplication[i].EmployeeId,
                                                    EmployeeCode = users_name.employee_code,
                                                    UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                                    ApproverId = lApplication[i].ApproverId,
                                                    ApproverCode = approver_name.employee_code,
                                                    ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                                    LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                                                    ApproverDetail = lApplication[i].ApproverDetail,
                                                    IsActive = lApplication[i].IsActive,
                                                    IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                                                    IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                                                    IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                                                    IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                                                    CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                                                    UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                                                    AttachmentFilePath = strAttPath,
                                                    actions =
                                                        "<span data-row='" + lApplication[i].Id + "'>" +//DONE 01 - HR
                                                            "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + data_emp.access_group.AccessGroupId + "," + users_name.site_id + "," + users_name.grade.GradeId + "," + data_emp.site_id /*HttpContext.Current.Session["GV_SiteID"].ToString()*/ + "," + lApplication[i].LeaveTypeId + "," + lApplication[i].LeaveStatusHODId + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                                            //"<span> / </span>" +
                                                            ((lApplication[i].LeaveStatusId > 0 && lApplication[i].ToDate.Year == DateTime.Now.Year) ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") + //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                                        "</span>"
                                                });

                                            }
                                        }
                                    }
                                }
                                else if (data_emp.site_id == 1 && data_emp.access_group.AccessGroupId == 1)
                                {
                                    if (approver_name != null && users_name != null)
                                    {
                                        if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                                        {
                                            if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                            {
                                                strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                    "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                                "</span>";
                                            }
                                            else
                                            {
                                                strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                                        "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                                    "</span>";
                                            }
                                        }
                                        else
                                        {
                                            strAttPath = "-";
                                        }



                                        toReturn.Add(new ViewModels.LeaveApplicationInfo()
                                        {
                                            Id = lApplication[i].Id,
                                            LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                                            FromDateText = lApplication[i].FromDate.ToString("dd-MM-yyyy"),
                                            ToDateText = lApplication[i].ToDate.ToString("dd-MM-yyyy"),
                                            DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                            LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                                            ReasonDetail = lApplication[i].ReasonDetail,
                                            EmployeeId = lApplication[i].EmployeeId,
                                            EmployeeCode = users_name.employee_code,
                                            UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                            ApproverId = lApplication[i].ApproverId,
                                            ApproverCode = approver_name.employee_code,
                                            ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                            LeaveStatusText = lApplication[i].LeaveStatus != null ? lApplication[i].LeaveStatus.LeaveStatusText : "Pending",
                                            ApproverDetail = lApplication[i].ApproverDetail,
                                            IsActive = lApplication[i].IsActive,
                                            IsHODApproved = lApplication[i].LeaveStatusHODId == 2 ? "Yes" : "No",
                                            IsPRNApproved = lApplication[i].LeaveStatusPRNId == 2 ? "Yes" : "No",
                                            IsHRApproved = lApplication[i].LeaveStatusHRId == 2 ? "Yes" : "No",
                                            IsVCApproved = lApplication[i].LeaveStatusVCId == 2 ? "Yes" : "No",
                                            CreateDateText = lApplication[i].CreateDateTime.ToString("dd-MM-yyyy"),
                                            UpdateDateText = lApplication[i].UpdateDateTime.ToString("dd-MM-yyyy"),
                                            AttachmentFilePath = strAttPath,
                                            actions =
                                                "<span data-row='" + lApplication[i].Id + "'>" +//DONE 01 - HR
                                                    "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + data_emp.access_group.AccessGroupId + "," + users_name.site_id + "," + users_name.grade.GradeId + "," + data_emp.site_id /*HttpContext.Current.Session["GV_SiteID"].ToString()*/ + "," + lApplication[i].LeaveTypeId + "," + lApplication[i].LeaveStatusHODId + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                                    //"<span> / </span>" +
                                                    ((lApplication[i].LeaveStatusId > 0 && lApplication[i].ToDate.Year == DateTime.Now.Year) ? " / <a style=\"line-height: 18px;\" href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" : "") + //"<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                                "</span>"
                                        });

                                    }
                                }
                                else
                                { //nothing
                                }
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lApplication = new List<LeaveApplication>();
                }
            }

            return toReturn;
        }



        public static int AddNewLeaveApplication(LeaveApplicationInfo linfo)
        {
            int response = 0;
            int iMaxLeaveApplyDaysCount = 0;

            try
            {
                //////////////////////// Validate Number of Leaves with Remainings ///////////////////////////////////

                //bool isSickAllowed = false, isCasualAllowed = false, isAnnualAllowed = false, isOtherAllowed = false;
                int alcSick = 0, alcCasual = 0, alcAnnual = 0, alcOther = 0;
                int alcLeaveType01 = 0, alcLeaveType02 = 0, alcLeaveType03 = 0, alcLeaveType04 = 0, alcLeaveType05 = 0, alcLeaveType06 = 0, alcLeaveType07 = 0, alcLeaveType08 = 0;
                int alcLeaveType09 = 0, alcLeaveType10 = 0, alcLeaveType11 = 0;

                decimal avlSick = 0, avlCasual = 0, avlAnnual = 0, avlOther = 0;
                decimal avlLT01 = 0, avlLT02 = 0, avlLT03 = 0, avlLT04 = 0, avlLT05 = 0, avlLT06 = 0, avlLT07 = 0, avlLT08 = 0;
                decimal avlLT09 = 0, avlLT10 = 0, avlLT11 = 0;

                List<ViewModels.LeaveTypeInfo> toReturn = new List<ViewModels.LeaveTypeInfo>();

                //total
                if (HttpContext.Current.Session["GV_AllocatedSickLeaves"] != null && HttpContext.Current.Session["GV_AllocatedSickLeaves"].ToString() != "")
                    alcSick = int.Parse(HttpContext.Current.Session["GV_AllocatedSickLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedCasualLeaves"] != null && HttpContext.Current.Session["GV_AllocatedCasualLeaves"].ToString() != "")
                    alcCasual = int.Parse(HttpContext.Current.Session["GV_AllocatedCasualLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedAnnualLeaves"] != null && HttpContext.Current.Session["GV_AllocatedAnnualLeaves"].ToString() != "")
                    alcAnnual = int.Parse(HttpContext.Current.Session["GV_AllocatedAnnualLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedOtherLeaves"] != null && HttpContext.Current.Session["GV_AllocatedOtherLeaves"].ToString() != "")
                    alcOther = int.Parse(HttpContext.Current.Session["GV_AllocatedOtherLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType01"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType01"].ToString() != "")
                    alcLeaveType01 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType01"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType02"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType02"].ToString() != "")
                    alcLeaveType02 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType02"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType03"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType03"].ToString() != "")
                    alcLeaveType03 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType03"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType04"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType04"].ToString() != "")
                    alcLeaveType04 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType04"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType05"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType05"].ToString() != "")
                    alcLeaveType05 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType05"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType06"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType06"].ToString() != "")
                    alcLeaveType06 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType06"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType07"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType07"].ToString() != "")
                    alcLeaveType07 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType07"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType08"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType08"].ToString() != "")
                    alcLeaveType08 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType08"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType09"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType09"].ToString() != "")
                    alcLeaveType09 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType09"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType10"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType10"].ToString() != "")
                    alcLeaveType10 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType10"].ToString());

                if (HttpContext.Current.Session["GV_AllocatedLeaveType11"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType11"].ToString() != "")
                    alcLeaveType11 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType11"].ToString());

                //utilized
                if (HttpContext.Current.Session["GV_AvailedSickLeaves"] != null && HttpContext.Current.Session["GV_AvailedSickLeaves"].ToString() != "")
                    avlSick = decimal.Parse(HttpContext.Current.Session["GV_AvailedSickLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AvailedCasualLeaves"] != null && HttpContext.Current.Session["GV_AvailedCasualLeaves"].ToString() != "")
                    avlCasual = decimal.Parse(HttpContext.Current.Session["GV_AvailedCasualLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AvailedAnnualLeaves"] != null && HttpContext.Current.Session["GV_AvailedAnnualLeaves"].ToString() != "")
                    avlAnnual = decimal.Parse(HttpContext.Current.Session["GV_AvailedAnnualLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AvailedOtherLeaves"] != null && HttpContext.Current.Session["GV_AvailedOtherLeaves"].ToString() != "")
                    avlOther = decimal.Parse(HttpContext.Current.Session["GV_AvailedOtherLeaves"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType01"] != null && HttpContext.Current.Session["GV_AvailedLeaveType01"].ToString() != "")
                    avlLT01 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType01"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType02"] != null && HttpContext.Current.Session["GV_AvailedLeaveType02"].ToString() != "")
                    avlLT02 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType02"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType03"] != null && HttpContext.Current.Session["GV_AvailedLeaveType03"].ToString() != "")
                    avlLT03 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType03"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType04"] != null && HttpContext.Current.Session["GV_AvailedLeaveType04"].ToString() != "")
                    avlLT04 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType04"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType05"] != null && HttpContext.Current.Session["GV_AvailedLeaveType05"].ToString() != "")
                    avlLT05 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType05"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType06"] != null && HttpContext.Current.Session["GV_AvailedLeaveType06"].ToString() != "")
                    avlLT06 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType06"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType07"] != null && HttpContext.Current.Session["GV_AvailedLeaveType07"].ToString() != "")
                    avlLT07 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType07"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType08"] != null && HttpContext.Current.Session["GV_AvailedLeaveType08"].ToString() != "")
                    avlLT08 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType08"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType09"] != null && HttpContext.Current.Session["GV_AvailedLeaveType09"].ToString() != "")
                    avlLT09 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType09"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType10"] != null && HttpContext.Current.Session["GV_AvailedLeaveType10"].ToString() != "")
                    avlLT10 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType10"].ToString());

                if (HttpContext.Current.Session["GV_AvailedLeaveType11"] != null && HttpContext.Current.Session["GV_AvailedLeaveType11"].ToString() != "")
                    avlLT11 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType11"].ToString());

                if (ConfigurationManager.AppSettings["MaxLeaveApplyDaysCount"] != null && ConfigurationManager.AppSettings["MaxLeaveApplyDaysCount"].ToString() != "")
                {
                    iMaxLeaveApplyDaysCount = int.Parse(ConfigurationManager.AppSettings["MaxLeaveApplyDaysCount"].ToString());
                }
                else
                {
                    iMaxLeaveApplyDaysCount = 61;
                }

                //for testing
                ////avlSick = 10; avlCasual = 9; avlAnnual = 21; avlOther = 0;

                //compare FROM and TO dates
                if (linfo.FromDate > linfo.ToDate)
                {
                    return -3;
                }

                ////apply within same month range
                if (linfo.FromDate.Month == linfo.ToDate.Month)
                {
                    //ok
                }
                else
                {
                    return -4;
                }

                //apply for leave max. 20 days before
                //if (Math.Abs((linfo.FromDate - DateTime.Now).Days) > iMaxLeaveApplyDaysCount)
                //{
                //    return -5;
                //}

                if (linfo.LeaveTypeId == 1)
                {
                    if (alcSick > 0 && Math.Ceiling(avlSick + linfo.DaysCount) <= alcSick)
                    {
                        //isSickAllowed = true;
                    }
                    else
                    {
                        return -6;
                    }
                }

                if (linfo.LeaveTypeId == 2)
                {
                    if (alcCasual > 0 && Math.Ceiling(avlCasual + linfo.DaysCount) <= alcCasual)
                    {
                        //isCasualAllowed = true;
                    }
                    else
                    {
                        return -7;
                    }
                }

                if (linfo.LeaveTypeId == 3)
                {
                    if (alcAnnual > 0 && Math.Ceiling(avlAnnual + linfo.DaysCount) <= alcAnnual)
                    {
                        //isAnnualAllowed = true;
                    }
                    else
                    {
                        return -8;
                    }
                }

                if (linfo.LeaveTypeId == 4)
                {
                    if (alcOther > 0 && Math.Ceiling(avlOther + linfo.DaysCount) <= alcOther)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -9;
                    }
                }

                //Leave Type 01
                if (linfo.LeaveTypeId == 5)
                {
                    if (alcLeaveType01 > 0 && Math.Ceiling(avlLT01 + linfo.DaysCount) <= alcLeaveType01)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -10;
                    }
                }

                //Cancel Leave Type
                if (linfo.LeaveTypeId == 6)
                {
                    if (alcLeaveType02 > 0 && Math.Ceiling(avlLT02 + linfo.DaysCount) <= alcLeaveType02)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -11;
                    }
                }

                if (linfo.LeaveTypeId == 7)
                {
                    if (alcLeaveType03 > 0 && Math.Ceiling(avlLT03 + linfo.DaysCount) <= alcLeaveType03)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -12;
                    }
                }

                if (linfo.LeaveTypeId == 8)
                {
                    if (alcLeaveType04 > 0 && Math.Ceiling(avlLT04 + linfo.DaysCount) <= alcLeaveType04)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -13;
                    }
                }

                if (linfo.LeaveTypeId == 9)
                {
                    if (alcLeaveType05 > 0 && Math.Ceiling(avlLT05 + linfo.DaysCount) <= alcLeaveType05)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -14;
                    }
                }

                if (linfo.LeaveTypeId == 10)
                {
                    if (alcLeaveType06 > 0 && Math.Ceiling(avlLT06 + linfo.DaysCount) <= alcLeaveType06)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -15;
                    }
                }


                if (linfo.LeaveTypeId == 11)
                {
                    if (alcLeaveType07 > 0 && Math.Ceiling(avlLT07 + linfo.DaysCount) <= alcLeaveType07)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -16;
                    }
                }

                if (linfo.LeaveTypeId == 12)
                {
                    if (alcLeaveType08 > 0 && Math.Ceiling(avlLT08 + linfo.DaysCount) <= alcLeaveType08)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -17;
                    }
                }


                if (linfo.LeaveTypeId == 13)
                {
                    if (alcLeaveType09 > 0 && Math.Ceiling(avlLT09 + linfo.DaysCount) <= alcLeaveType09)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -18;
                    }
                }


                if (linfo.LeaveTypeId == 14)
                {
                    if (alcLeaveType10 > 0 && Math.Ceiling(avlLT10 + linfo.DaysCount) <= alcLeaveType10)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -19;
                    }
                }


                if (linfo.LeaveTypeId == 15)
                {
                    if (alcLeaveType11 > 0 && Math.Ceiling(avlLT11 + linfo.DaysCount) <= alcLeaveType11)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -20;
                    }
                }

                using (Context db = new Context())
                {
                    //////compare FROM and TO dates
                    ////if (linfo.FromDate > linfo.ToDate)
                    ////{
                    ////    return -3;
                    ////}

                    //////apply within same month range
                    ////if (linfo.FromDate.Month == DateTime.Now.Month && linfo.ToDate.Month != DateTime.Now.Month)
                    ////{
                    ////    return -4;
                    ////}

                    ////if (Math.Abs((linfo.FromDate - DateTime.Now).Days) > 3)
                    ////{
                    ////    return -5;
                    ////}

                    //////find if a leave already exists
                    ////var existingLeaves = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && d.FromDate == linfo.FromDate && d.ToDate == linfo.ToDate);

                    ////if (existingLeaves != null && existingLeaves.Count() > 0)
                    ////{
                    ////    return -2;
                    ////}

                    //find if a leave with same dates exists dont allow - except Cancel leave
                    //if (linfo.LeaveTypeId != 6)
                    {
                        var existingLeavesEXACT = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && d.FromDate == linfo.FromDate && d.ToDate == linfo.ToDate).FirstOrDefault();
                        if (existingLeavesEXACT != null)
                        {
                            return -2;
                        }

                        var existingLeavesFROMResides = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && ((linfo.FromDate >= d.FromDate && linfo.FromDate <= d.ToDate) || (linfo.ToDate >= d.FromDate && linfo.ToDate <= d.ToDate))).FirstOrDefault();
                        if (existingLeavesFROMResides != null)
                        {
                            return -21;
                        }
                    }

                    //if CANCEL Leave
                    ////if (linfo.LeaveTypeId == 6)
                    ////{
                    ////    var existingLeavesSameLeavePending = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.LeaveStatusId == 1 && d.IsActive && d.FromDate == linfo.FromDate && d.ToDate == linfo.ToDate).FirstOrDefault();
                    ////    if (existingLeavesSameLeavePending == null)
                    ////    {
                    ////        //allowed                         
                    ////    }
                    ////    else
                    ////    {
                    ////        return -25;
                    ////    }

                    ////    var existingLeavesSameLeaveMustApp = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.LeaveStatusId == 2 && d.IsActive && d.FromDate == linfo.FromDate && d.ToDate == linfo.ToDate).FirstOrDefault();
                    ////    if (existingLeavesSameLeaveMustApp != null)
                    ////    {
                    ////        //allowed
                    ////    }
                    ////    else
                    ////    {
                    ////        return -26;
                    ////    }

                    ////    var existingLeavesCancel = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && linfo.LeaveTypeId == 6 && d.IsActive && (d.FromDate == linfo.FromDate && d.ToDate == linfo.ToDate)).FirstOrDefault();
                    ////    if (existingLeavesCancel != null)
                    ////    {
                    ////        //allowed
                    ////    }
                    ////    else
                    ////    {
                    ////        //already cancel leave applied 
                    ////        return -27;
                    ////    }


                    ////}

                    ////var existingLeavesFROMResides = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && ((DateTime.Compare(linfo.FromDate, d.FromDate) == 1) && (DateTime.Compare(linfo.FromDate, d.ToDate) == 1)));
                    ////if (existingLeavesFROMResides != null && existingLeavesFROMResides.Count() > 0)
                    ////{
                    ////    return -21;
                    ////}

                    ////var existingLeavesTOResides = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && ((DateTime.Compare(linfo.ToDate, d.FromDate) == 1) && (DateTime.Compare(linfo.ToDate, d.ToDate) == 1)));
                    ////if (existingLeavesTOResides != null && existingLeavesTOResides.Count() > 0)
                    ////{
                    ////    return -22;
                    ////}



                    //add new leave record
                    LeaveApplication leave = new LeaveApplication();

                    if (linfo.EmployeeId != 0)
                        leave.EmployeeId = linfo.EmployeeId;

                    leave.LeaveTypeId = linfo.LeaveTypeId;

                    leave.FromDate = linfo.FromDate;
                    leave.ToDate = linfo.ToDate;

                    leave.DaysCount = linfo.DaysCount;

                    leave.LeaveReasonId = linfo.LeaveReasonId;

                    leave.ReasonDetail = linfo.ReasonDetail;
                    leave.ApproverId = linfo.ApproverId;
                    leave.ApproverDetail = "";

                    leave.LeaveStatusId = linfo.LeaveStatusId;

                    leave.LeaveStatusHODId = 1;
                    leave.LeaveStatusPRNId = 1;
                    leave.LeaveStatusHRId = 1;
                    leave.LeaveStatusVCId = 1;

                    leave.AttachmentFilePath = linfo.AttachmentFilePath;

                    leave.IsActive = linfo.IsActive;

                    leave.LeaveValidityId = 1;
                    leave.LeaveValidityRemarks = "--";

                    leave.CreateDateTime = linfo.CreateDateTime;
                    leave.UpdateDateTime = linfo.UpdateDateTime;

                    db.leave_application.Add(leave);
                    db.SaveChanges();

                    response = leave.Id;
                }
            }
            catch (Exception ex)
            {
                response = -1;
                //throw ex;
            }

            return response;
        }

        public static int AddNewLeaveApplicationOnBehalf(LeaveApplicationInfo linfo)
        {
            int response = 0;
            int iMaxLeaveApplyDaysCount = 0;

            try
            {
                //////////////////////// Validate Number of Leaves with Remainings ///////////////////////////////////

                //bool isSickAllowed = false, isCasualAllowed = false, isAnnualAllowed = false, isOtherAllowed = false;
                int alcSick = 0, alcCasual = 0, alcAnnual = 0, alcOther = 0;
                int alcLeaveType01 = 0, alcLeaveType02 = 0, alcLeaveType03 = 0, alcLeaveType04 = 0;

                decimal avlSick = 0, avlCasual = 0, avlAnnual = 0, avlOther = 0;
                decimal avlLT01 = 0, avlLT02 = 0, avlLT03 = 0, avlLT04 = 0;

                List<ViewModels.LeaveTypeInfo> toReturn = new List<ViewModels.LeaveTypeInfo>();

                //total
                if (HttpContext.Current.Session["OnBe_AllocatedSickLeaves"] != null && HttpContext.Current.Session["OnBe_AllocatedSickLeaves"].ToString() != "")
                    alcSick = int.Parse(HttpContext.Current.Session["OnBe_AllocatedSickLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AllocatedCasualLeaves"] != null && HttpContext.Current.Session["OnBe_AllocatedCasualLeaves"].ToString() != "")
                    alcCasual = int.Parse(HttpContext.Current.Session["OnBe_AllocatedCasualLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AllocatedAnnualLeaves"] != null && HttpContext.Current.Session["OnBe_AllocatedAnnualLeaves"].ToString() != "")
                    alcAnnual = int.Parse(HttpContext.Current.Session["OnBe_AllocatedAnnualLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AllocatedOtherLeaves"] != null && HttpContext.Current.Session["OnBe_AllocatedOtherLeaves"].ToString() != "")
                    alcOther = int.Parse(HttpContext.Current.Session["OnBe_AllocatedOtherLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AllocatedLeaveType01"] != null && HttpContext.Current.Session["OnBe_AllocatedLeaveType01"].ToString() != "")
                    alcLeaveType01 = int.Parse(HttpContext.Current.Session["OnBe_AllocatedLeaveType01"].ToString());

                if (HttpContext.Current.Session["OnBe_AllocatedLeaveType02"] != null && HttpContext.Current.Session["OnBe_AllocatedLeaveType02"].ToString() != "")
                    alcLeaveType02 = int.Parse(HttpContext.Current.Session["OnBe_AllocatedLeaveType02"].ToString());

                if (HttpContext.Current.Session["OnBe_AllocatedLeaveType03"] != null && HttpContext.Current.Session["OnBe_AllocatedLeaveType03"].ToString() != "")
                    alcLeaveType03 = int.Parse(HttpContext.Current.Session["OnBe_AllocatedLeaveType03"].ToString());

                if (HttpContext.Current.Session["OnBe_AllocatedLeaveType04"] != null && HttpContext.Current.Session["OnBe_AllocatedLeaveType04"].ToString() != "")
                    alcLeaveType04 = int.Parse(HttpContext.Current.Session["OnBe_AllocatedLeaveType04"].ToString());

                //utilized
                if (HttpContext.Current.Session["OnBe_AvailedSickLeaves"] != null && HttpContext.Current.Session["OnBe_AvailedSickLeaves"].ToString() != "")
                    avlSick = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedSickLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AvailedCasualLeaves"] != null && HttpContext.Current.Session["OnBe_AvailedCasualLeaves"].ToString() != "")
                    avlCasual = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedCasualLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AvailedAnnualLeaves"] != null && HttpContext.Current.Session["OnBe_AvailedAnnualLeaves"].ToString() != "")
                    avlAnnual = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedAnnualLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AvailedOtherLeaves"] != null && HttpContext.Current.Session["OnBe_AvailedOtherLeaves"].ToString() != "")
                    avlOther = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedOtherLeaves"].ToString());

                if (HttpContext.Current.Session["OnBe_AvailedLeaveType01"] != null && HttpContext.Current.Session["OnBe_AvailedLeaveType01"].ToString() != "")
                    avlLT01 = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedLeaveType01"].ToString());

                if (HttpContext.Current.Session["OnBe_AvailedLeaveType02"] != null && HttpContext.Current.Session["OnBe_AvailedLeaveType02"].ToString() != "")
                    avlLT02 = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedLeaveType02"].ToString());

                if (HttpContext.Current.Session["OnBe_AvailedLeaveType03"] != null && HttpContext.Current.Session["OnBe_AvailedLeaveType03"].ToString() != "")
                    avlLT03 = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedLeaveType03"].ToString());

                if (HttpContext.Current.Session["OnBe_AvailedLeaveType04"] != null && HttpContext.Current.Session["OnBe_AvailedLeaveType04"].ToString() != "")
                    avlLT04 = decimal.Parse(HttpContext.Current.Session["OnBe_AvailedLeaveType04"].ToString());

                if (ConfigurationManager.AppSettings["MaxLeaveApplyDaysCount"] != null && ConfigurationManager.AppSettings["MaxLeaveApplyDaysCount"].ToString() != "")
                {
                    iMaxLeaveApplyDaysCount = int.Parse(ConfigurationManager.AppSettings["MaxLeaveApplyDaysCount"].ToString());
                }
                else
                {
                    iMaxLeaveApplyDaysCount = 61;
                }

                //for testing
                ////avlSick = 10; avlCasual = 9; avlAnnual = 21; avlOther = 0;

                //compare FROM and TO dates
                if (linfo.FromDate > linfo.ToDate)
                {
                    return -3;
                }

                ////apply within same month range
                if (linfo.FromDate.Month == linfo.ToDate.Month)
                {
                    //ok
                }
                else
                {
                    return -4;
                }

                //apply for leave max. 20 days before
                //if (Math.Abs((linfo.FromDate - DateTime.Now).Days) > iMaxLeaveApplyDaysCount)
                //{
                //    return -5;
                //}

                if (linfo.LeaveTypeId == 1)
                {
                    if (alcSick > 0 && Math.Ceiling(avlSick + linfo.DaysCount) <= alcSick)
                    {
                        //isSickAllowed = true;
                    }
                    else
                    {
                        return -6;
                    }
                }

                if (linfo.LeaveTypeId == 2)
                {
                    if (alcCasual > 0 && Math.Ceiling(avlCasual + linfo.DaysCount) <= alcCasual)
                    {
                        //isCasualAllowed = true;
                    }
                    else
                    {
                        return -7;
                    }
                }

                if (linfo.LeaveTypeId == 3)
                {
                    if (alcAnnual > 0 && Math.Ceiling(avlAnnual + linfo.DaysCount) <= alcAnnual)
                    {
                        //isAnnualAllowed = true;
                    }
                    else
                    {
                        return -8;
                    }
                }

                if (linfo.LeaveTypeId == 4)
                {
                    if (alcOther > 0 && Math.Ceiling(avlOther + linfo.DaysCount) <= alcOther)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -9;
                    }
                }

                //Leave Type 01
                if (linfo.LeaveTypeId == 5)
                {
                    if (alcLeaveType01 > 0 && Math.Ceiling(avlLT01 + linfo.DaysCount) <= alcLeaveType01)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -10;
                    }
                }

                if (linfo.LeaveTypeId == 6)
                {
                    if (alcLeaveType02 > 0 && Math.Ceiling(avlLT02 + linfo.DaysCount) <= alcLeaveType02)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -11;
                    }
                }

                if (linfo.LeaveTypeId == 7)
                {
                    if (alcLeaveType03 > 0 && Math.Ceiling(avlLT03 + linfo.DaysCount) <= alcLeaveType03)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -12;
                    }
                }

                if (linfo.LeaveTypeId == 8)
                {
                    if (alcLeaveType04 > 0 && Math.Ceiling(avlLT04 + linfo.DaysCount) <= alcLeaveType04)
                    {
                        //isOtherAllowed = true;
                    }
                    else
                    {
                        return -13;
                    }
                }

                using (Context db = new Context())
                {
                    //////compare FROM and TO dates
                    ////if (linfo.FromDate > linfo.ToDate)
                    ////{
                    ////    return -3;
                    ////}

                    //////apply within same month range
                    ////if (linfo.FromDate.Month == DateTime.Now.Month && linfo.ToDate.Month != DateTime.Now.Month)
                    ////{
                    ////    return -4;
                    ////}

                    ////if (Math.Abs((linfo.FromDate - DateTime.Now).Days) > 3)
                    ////{
                    ////    return -5;
                    ////}

                    //////find if a leave already exists
                    ////var existingLeaves = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && d.FromDate == linfo.FromDate && d.ToDate == linfo.ToDate);

                    ////if (existingLeaves != null && existingLeaves.Count() > 0)
                    ////{
                    ////    return -2;
                    ////}

                    //find if a leave with same dates exists
                    var existingLeavesEXACT = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && d.FromDate == linfo.FromDate && d.ToDate == linfo.ToDate);
                    if (existingLeavesEXACT != null && existingLeavesEXACT.Count() > 0)
                    {
                        return -2;
                    }




                    var existingLeavesFROMResides = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && ((linfo.FromDate >= d.FromDate && linfo.FromDate <= d.ToDate) || (linfo.ToDate >= d.FromDate && linfo.ToDate <= d.ToDate)));
                    if (existingLeavesFROMResides != null && existingLeavesFROMResides.Count() > 0)
                    {
                        return -21;
                    }


                    //var existingLeavesFROMResides = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && ((DateTime.Compare(linfo.FromDate, d.FromDate) == 1) && (DateTime.Compare(linfo.FromDate, d.ToDate) == 1)));
                    //if (existingLeavesFROMResides != null && existingLeavesFROMResides.Count() > 0)
                    //{
                    //    return -21;
                    //}

                    //var existingLeavesTOResides = db.leave_application.Where(d => d.EmployeeId == linfo.EmployeeId && d.IsActive && ((DateTime.Compare(linfo.ToDate, d.FromDate) == 1) && (DateTime.Compare(linfo.ToDate, d.ToDate) == 1)));
                    //if (existingLeavesTOResides != null && existingLeavesTOResides.Count() > 0)
                    //{
                    //    return -22;
                    //}



                    //add new leave record
                    LeaveApplication leave = new LeaveApplication();

                    if (linfo.EmployeeId != 0)
                        leave.EmployeeId = linfo.EmployeeId;

                    leave.LeaveTypeId = linfo.LeaveTypeId;

                    leave.FromDate = linfo.FromDate;
                    leave.ToDate = linfo.ToDate;

                    leave.DaysCount = linfo.DaysCount;
                    leave.LeaveReasonId = linfo.LeaveReasonId;

                    leave.ReasonDetail = linfo.ReasonDetail;
                    leave.ApproverId = linfo.ApproverId;
                    leave.ApproverDetail = "";

                    leave.LeaveStatusId = linfo.LeaveStatusId;

                    leave.AttachmentFilePath = linfo.AttachmentFilePath;

                    leave.IsActive = linfo.IsActive;

                    leave.CreateDateTime = linfo.CreateDateTime;
                    leave.UpdateDateTime = linfo.UpdateDateTime;

                    db.leave_application.Add(leave);
                    db.SaveChanges();

                    response = leave.Id;
                }
            }
            catch (Exception ex)
            {
                response = -1;
                //throw ex;
            }

            return response;
        }


        public static int GetUserId(string user_employee_code)
        {
            int user_id = 0;

            using (Context db = new Context())
            {
                user_id = db.employee.Where(e => e.active && e.employee_code == user_employee_code).FirstOrDefault().EmployeeId;
            }

            return user_id;
        }

        public static int GetApproverUserID(string user_employee_code)
        {
            int group_id = 0, supervisor_id = 0, approver_id = 0;

            using (Context db = new Context())
            {
                if (db.employee.Where(e => e.active && e.employee_code == user_employee_code).FirstOrDefault().Group != null)
                {
                    group_id = db.employee.Where(e => e.active && e.employee_code == user_employee_code).FirstOrDefault().Group.GroupId;

                    if (group_id != 0)
                    {
                        supervisor_id = db.group.Where(g => g.GroupId == group_id).FirstOrDefault().supervisor_id;
                    }

                    //approver_id = db.employee.Where(e => e.EmployeeId == supervisor_id).FirstOrDefault().EmployeeId;
                }
                else
                {
                    supervisor_id = 1;
                }
            }

            return supervisor_id;
        }

        public static List<ViewModels.LeaveApplicationInfo> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeaveApplicationInfo> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<LeaveApplicationInfo> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<LeaveApplicationInfo> FilterResult(string search, List<LeaveApplicationInfo> dtResult)
        {
            IQueryable<LeaveApplicationInfo> results = dtResult.AsQueryable();

            //results = results.Where(p =>
            //search == null ||
            //(
            //p.FromDate != null));

            results = results.Where(p => p.IsActive &&
                    (
                        search == null || search.Equals("") ||
                        (
                                p.Id.ToString().ToLower().Contains(search.ToLower())
                            || (p.EmployeeCode != null && p.EmployeeCode.ToLower().Contains(search.ToLower()))
                            || (p.FromDateText != null && p.FromDateText.ToLower().Contains(search.ToLower()))
                            || (p.ToDateText != null && p.ToDateText.ToLower().Contains(search.ToLower()))
                            || (p.LeaveTypeText != null && p.LeaveTypeText.ToLower().Contains(search.ToLower()))
                            || (p.ApproverCode != null && p.ApproverCode.ToLower().Contains(search.ToLower()))
                            || (p.UsersName != null && p.UsersName.ToLower().Contains(search.ToLower()))
                            || (p.ApproverName != null && p.ApproverName.ToLower().Contains(search.ToLower()))
                            || (p.DaysCount != null && p.DaysCount.ToString().Contains(search.ToLower()))
                            || (p.LeaveStatusText != null && p.LeaveStatusText.ToLower().Contains(search.ToLower()))
                            || (p.AttachmentFilePath != null && p.AttachmentFilePath.ToLower().Contains(search.ToLower()))
                            || (p.ReasonDetail != null && p.ReasonDetail.ToLower().Contains(search.ToLower()))
                            || (p.ApproverDetail != null && p.ApproverDetail.ToLower().Contains(search.ToLower()))
                        // || EntityFunctions.TruncateTime(p.FromDate).ToString().Contains(search.ToLower()) || EntityFunctions.TruncateTime(p.ToDate).ToString().Contains(search.ToLower())


                        )
                    //&&
                    //  (depart_id.Equals(-1) || p.ReasonDetail.department.DepartmentId.Equals(depart_id))
                    //       &&
                    //  (des_id.Equals(-1) || ca.employee.designation.DesignationId.Equals(des_id))
                    )

                    );

            return results;
        }

        public static void updateLeaveValidity(ViewModels.LeaveApplicationInfo toUpdate)
        {
            string message = "";
            //DateTime dtLeave = DateTime.Now;

            using (Context db = new Context())
            {
                LeaveApplication toUpdateModel = db.leave_application.Find(toUpdate.Id);

                //string empCode = db.employee.Where(e => e.EmployeeId == toUpdate.UserId).FirstOrDefault().employee_code;
                if (toUpdateModel != null)
                {
                    toUpdateModel.LeaveValidityId = toUpdate.LeaveValidityId;
                    toUpdateModel.LeaveValidityRemarks = toUpdate.LeaveValidityRemarks;
                    toUpdateModel.UpdateDateTime = DateTime.Now;

                    db.SaveChanges();
                    message = "succeeded";
                }
                else
                {
                    message = "no data found";
                }
            }
        }

        public static void update(ViewModels.LeaveApplicationInfo toUpdate)
        {
            string message = "";
            DateTime dtLeave = DateTime.Now;
            LeaveApplication toUpdateModel = new LeaveApplication();

            using (Context db = new Context())
            {
                toUpdateModel = db.leave_application.Find(toUpdate.Id);
                //string empCode = db.employee.Where(e => e.EmployeeId == toUpdate.UserId).FirstOrDefault().employee_code;
                if (toUpdateModel != null)
                {
                    //toUpdateModel.FromDate = toUpdate.FromDate;
                    //toUpdateModel.ToDate = toUpdate.ToDate;
                    //toUpdateModel.DaysCount = toUpdate.DaysCount;
                    toUpdateModel.LeaveStatusId = toUpdate.LeaveStatusId;
                    toUpdateModel.ApproverDetail = toUpdate.ApproverDetail;
                    toUpdateModel.IsActive = true;


                    //steps to Approve
                    if (toUpdate.LeaveStatusId != 2)
                    {
                        if (toUpdate.LeaveTypeId == 1)//medical
                        {
                            if (toUpdate.LeaveStatusId == -4)//HOD
                            {
                                toUpdateModel.LeaveStatusHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 1;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == -5)//PRN
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                toUpdateModel.LeaveStatusPRNId = 2;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == 2)//HR
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 2;
                                toUpdateModel.LeaveStatusHRId = 2;
                                toUpdateModel.LeaveStatusVCId = 2;
                            }
                        }
                        else if (toUpdate.LeaveTypeId == 2)//casual
                        {
                            //do nothing
                        }
                        else if (toUpdate.LeaveTypeId == 3)//earned leave
                        {
                            if (toUpdate.LeaveReasonId < 20)//temp-convert to grade
                            {
                                if (toUpdate.LeaveStatusId == -4)//HOD
                                {
                                    toUpdateModel.LeaveStatusHODId = 2;
                                    //toUpdateModel.LeaveStautsPRNId = 1;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else if (toUpdate.LeaveStatusId == -5)//PRN
                                {
                                    //toUpdateModel.LeaveStautsHODId = 2;
                                    toUpdateModel.LeaveStatusPRNId = 2;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else//HR
                                {
                                    //it will be 2 for FINAL Approval
                                    toUpdateModel.LeaveStatusHRId = 2;
                                    toUpdateModel.LeaveStatusVCId = 2;
                                }
                            }
                            else
                            {
                                if (toUpdate.LeaveStatusId == -4)//HOD
                                {
                                    toUpdateModel.LeaveStatusHODId = 2;
                                    //toUpdateModel.LeaveStautsPRNId = 1;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else if (toUpdate.LeaveStatusId == -5)//PRN
                                {
                                    //toUpdateModel.LeaveStautsHODId = 2;
                                    toUpdateModel.LeaveStatusPRNId = 2;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else if (toUpdate.LeaveStatusId == -3)//HR
                                {
                                    //toUpdateModel.LeaveStautsHODId = 2;
                                    //toUpdateModel.LeaveStautsPRNId = 2;
                                    toUpdateModel.LeaveStatusHRId = 2;
                                }
                                else if (toUpdate.LeaveStatusId == 2)//VC
                                {
                                    //it will be 2 for FINAL Approval
                                    toUpdateModel.LeaveStatusVCId = 2;
                                }
                            }
                        }
                        else if (toUpdate.LeaveTypeId == 4)//exPak leave
                        {
                            if (toUpdate.LeaveStatusId == -4)//HOD
                            {
                                toUpdateModel.LeaveStatusHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 1;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == -5)//PRN
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                toUpdateModel.LeaveStatusPRNId = 2;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == -3)//HR
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 2;
                                toUpdateModel.LeaveStatusHRId = 2;
                            }
                            else if (toUpdate.LeaveStatusId == 2)//VC
                            {
                                //it will be 2 for FINAL Approval
                                toUpdateModel.LeaveStatusVCId = 2;
                            }
                        }
                    }

                    //approved leave of todays or future dates off
                    if (toUpdate.LeaveStatusId == 2) //check approved leave
                    {
                        //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                        //JUST HOD/HR/Status sake
                        if (toUpdate.LeaveTypeId == 1)//medical
                        {
                            if (toUpdate.LeaveStatusId == -4)//HOD
                            {
                                toUpdateModel.LeaveStatusHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 1;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == -5)//PRN
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                toUpdateModel.LeaveStatusPRNId = 2;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == 2)//HR
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 2;
                                toUpdateModel.LeaveStatusHRId = 2;
                                toUpdateModel.LeaveStatusVCId = 2;
                            }
                        }
                        else if (toUpdate.LeaveTypeId == 2)//casual
                        {
                            //do nothing
                        }
                        else if (toUpdate.LeaveTypeId == 3)//earned leave
                        {
                            if (toUpdate.LeaveReasonId < 20)//temp-convert to grade
                            {
                                if (toUpdate.LeaveStatusId == -4)//HOD
                                {
                                    toUpdateModel.LeaveStatusHODId = 2;
                                    //toUpdateModel.LeaveStautsPRNId = 1;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else if (toUpdate.LeaveStatusId == -5)//PRN
                                {
                                    //toUpdateModel.LeaveStautsHODId = 2;
                                    toUpdateModel.LeaveStatusPRNId = 2;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else//HR
                                {
                                    //it will be 2 for FINAL Approval
                                    toUpdateModel.LeaveStatusHRId = 2;
                                    toUpdateModel.LeaveStatusVCId = 2;
                                }
                            }
                            else
                            {
                                if (toUpdate.LeaveStatusId == -4)//HOD
                                {
                                    toUpdateModel.LeaveStatusHODId = 2;
                                    //toUpdateModel.LeaveStautsPRNId = 1;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else if (toUpdate.LeaveStatusId == -5)//PRN
                                {
                                    //toUpdateModel.LeaveStautsHODId = 2;
                                    toUpdateModel.LeaveStatusPRNId = 2;
                                    //toUpdateModel.LeaveStautsHRId = 1;
                                }
                                else if (toUpdate.LeaveStatusId == -3)//HR
                                {
                                    //toUpdateModel.LeaveStautsHODId = 2;
                                    //toUpdateModel.LeaveStautsPRNId = 2;
                                    toUpdateModel.LeaveStatusHRId = 2;
                                }
                                else if (toUpdate.LeaveStatusId == 2)//VC
                                {
                                    //it will be 2 for FINAL Approval
                                    toUpdateModel.LeaveStatusVCId = 2;
                                }
                            }
                        }
                        else if (toUpdate.LeaveTypeId == 4)//exPak leave
                        {
                            if (toUpdate.LeaveStatusId == -4)//HOD
                            {
                                toUpdateModel.LeaveStatusHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 1;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == -5)//PRN
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                toUpdateModel.LeaveStatusPRNId = 2;
                                //toUpdateModel.LeaveStautsHRId = 1;
                            }
                            else if (toUpdate.LeaveStatusId == -3)//HR
                            {
                                //toUpdateModel.LeaveStautsHODId = 2;
                                //toUpdateModel.LeaveStautsPRNId = 2;
                                toUpdateModel.LeaveStatusHRId = 2;
                            }
                            else if (toUpdate.LeaveStatusId == 2)//VC
                            {
                                //it will be 2 for FINAL Approval
                                toUpdateModel.LeaveStatusVCId = 2;
                            }
                        }
                        //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

                        ////if (toUpdateModel.LeaveTypeId != 6)//For CANCEL Leave - Don't add in Future and Past Attendance
                        {
                            if (Convert.ToDateTime(toUpdate.ToDate.ToString("yyyy-MM-dd 00:00:00.000")) == Convert.ToDateTime(toUpdate.FromDate.ToString("yyyy-MM-dd 00:00:00.000")))
                            {
                                if (Convert.ToDateTime(toUpdate.FromDate.ToString("yyyy-MM-dd 00:00:00.000")) >= Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000")))
                                {
                                    //Future Attandence
                                    message = BLL.ManageHR.addManualAttendanceHr(toUpdate.FromDate.ToString(), toUpdate.ToDate.ToString(), toUpdate.EmployeeId.ToString(), "LV");
                                }
                                else
                                {
                                    /*
                                    var data_emp = db.employee.Where(e => e.EmployeeId == toUpdate.EmployeeId).FirstOrDefault();
                                    if (data_emp != null)
                                    {
                                        ConsolidatedAttendance c = new ConsolidatedAttendance();
                                        c.employee = data_emp;
                                        c.time_in = toUpdate.FromDate;
                                        c.time_out = toUpdate.ToDate;
                                        c.final_remarks = "LV";
                                        c.active = true;
                                        db.consolidated_attendance.Add(c);
                                        db.SaveChanges();
                                    }
                                    */

                                    //Past Attandance
                                    ManualAttendance m = new ManualAttendance();

                                    //actually it is employee id
                                    m.employee_code = toUpdate.EmployeeId.ToString();
                                    m.time_in_from = toUpdate.FromDate;
                                    m.time_in_to = toUpdate.ToDate;
                                    m.remarks = "LV";

                                    message = TimeTune.ManualAttendance.addManualAttendance(m, "000000");
                                }
                            }
                            else
                            {
                                for (int i = 0; i < ((toUpdate.ToDate - toUpdate.FromDate).Days + 1); i++)
                                {
                                    dtLeave = toUpdate.FromDate.AddDays(i);

                                    //if (dtLeave.DayOfWeek != DayOfWeek.Sunday) // || dtLeave.DayOfWeek != DayOfWeek.Saturday)
                                    {
                                        if (Convert.ToDateTime(dtLeave.ToString("yyyy-MM-dd 00:00:00.000")) >= Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000")))
                                        {
                                            //Future
                                            message = BLL.ManageHR.addManualAttendanceHr(dtLeave.ToString(), dtLeave.ToString(), toUpdate.EmployeeId.ToString(), "LV");
                                        }
                                        else
                                        {
                                            /*
                                               var data_emp = db.employee.Where(e => e.EmployeeId == toUpdate.EmployeeId).FirstOrDefault();
                                               if (data_emp != null)
                                               {
                                                   ConsolidatedAttendance c = new ConsolidatedAttendance();
                                                   c.employee = data_emp;
                                                   c.time_in = toUpdate.FromDate;
                                                   c.time_out = toUpdate.ToDate;
                                                   c.final_remarks = "LV";
                                                   c.active = true;
                                                   db.consolidated_attendance.Add(c);
                                                   db.SaveChanges();
                                               }
                                            */

                                            //Past Attandance
                                            ManualAttendance m = new ManualAttendance();

                                            //actually it is employee id
                                            m.employee_code = toUpdate.EmployeeId.ToString();
                                            m.time_in_from = dtLeave;
                                            m.time_in_to = dtLeave;
                                            m.remarks = "LV";

                                            message = TimeTune.ManualAttendance.addManualAttendance(m, "000000");
                                        }
                                    }
                                }
                            }
                        }//IR added




                    }
                    else
                    {
                        //toUpdateModel.IsActive = false;
                        //toUpdateModel.UpdateDateTime = DateTime.Now;

                        //db.SaveChanges();
                    }

                    try
                    {
                        toUpdateModel.UpdateDateTime = DateTime.Now;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }


                    /*approved leave of todays or future dates off
                    if (toUpdate.FromDate >= DateTime.Now && toUpdate.LeaveStatusId == 2)
                    {
                        message = BLL.ManageHR.addManualAttendanceHr(toUpdate.FromDate.ToString(), toUpdate.ToDate.ToString(), toUpdate.UserId.ToString(), "LV");
                    }
                    else if (toUpdate.FromDate < DateTime.Now && toUpdate.LeaveStatusId == 2)
                    {
                        ManualAttendance m = new ManualAttendance();
                        m.employee_code = toUpdate.UserId.ToString();
                        m.time_in_from = toUpdate.FromDate;
                        m.time_in_to = toUpdate.ToDate;
                        m.remarks = "LV";

                        message = TimeTune.ManualAttendance.addManualAttendance(m);
                    }
                    */
                }//if-checking update null or not
            }
        }

        public static ViewModels.LeaveApplicationInfo remove(ViewModels.LeaveApplicationInfo toRemove)
        {

            using (Context db = new Context())
            {
                LeaveApplication toRemoveModel = db.leave_application.Find(toRemove.Id);

                //db.leave_application.Remove(toRemoveModel);

                toRemoveModel.IsActive = false;
                db.SaveChanges();

                return toRemove;
            }

        }

        //////////////////////////////////////////

        public static DateTime[] getSessionDatesByAcademicCalendar_excluded(int year)
        {
            DateTime[] dt = new DateTime[2];

            using (Context db = new Context())
            {
                var lSessionData = db.leave_session.Where(l => l.YearId == year && l.EmployeeId == 0).OrderByDescending(o => o.id).FirstOrDefault();
                if (lSessionData != null)
                {
                    dt[0] = lSessionData.SessionStartDate;
                    dt[1] = lSessionData.SessionEndDate;
                }
                else
                {
                    DLL.Models.LeaveSession leaveSession = new DLL.Models.LeaveSession();
                    leaveSession.EmployeeId = 0;
                    leaveSession.YearId = DateTime.Now.Year;

                    if (ConfigurationManager.AppSettings["AcademicYearStartDate"] != null && ConfigurationManager.AppSettings["AcademicYearStartDate"].ToString() != "")
                    {
                        leaveSession.SessionStartDate = Convert.ToDateTime(ConfigurationManager.AppSettings["AcademicYearStartDate"].ToString());
                        leaveSession.SessionEndDate = Convert.ToDateTime(ConfigurationManager.AppSettings["AcademicYearStartDate"].ToString()).AddDays(-1).AddYears(1);
                    }
                    else
                    {
                        leaveSession.SessionStartDate = DateTime.Now;
                        leaveSession.SessionEndDate = DateTime.Now.AddDays(-1).AddYears(1);
                    }

                    db.leave_session.Add(leaveSession);
                    db.SaveChanges();

                    dt[0] = leaveSession.SessionStartDate;
                    dt[1] = leaveSession.SessionEndDate;
                }

                return dt;
            }
        }

        public static DateTime[] getStudentSessionDatesByUserCode(string user_code)
        {
            int user_id = 0, current_academic_year = 0;
            DateTime[] dt = new DateTime[2];

            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                if (data_user != null)
                {
                    user_id = data_user.EmployeeId;

                    var lSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                    if (lSessionData != null)
                    {
                        //////var data_current_academic_year = db.academic_year.Where(e => e.IsCurrentSession).FirstOrDefault();
                        //////current_academic_year = data_current_academic_year.YearId;

                        //////if (lSessionData.SessionEndDate.Year == current_academic_year)
                        //////{
                        //////    dt[0] = lSessionData.SessionStartDate;
                        //////    dt[1] = lSessionData.SessionEndDate;
                        //////}
                        //////else if (lSessionData.SessionEndDate.Year < current_academic_year)
                        //////{
                        //////    dt[0] = data_current_academic_year.SessionStartDate;
                        //////    dt[1] = data_current_academic_year.SessionEndDate;

                        //////    DLL.Models.LeaveSession leaveSession = new DLL.Models.LeaveSession();
                        //////    leaveSession.EmployeeId = user_id;
                        //////    leaveSession.YearId = dt[1].AddDays(-1).AddYears(1).Year;
                        //////    leaveSession.SessionStartDate = dt[1].AddDays(-1);
                        //////    leaveSession.SessionEndDate = dt[1].AddDays(-1).AddYears(1);

                        //////    db.leave_session.Add(leaveSession);
                        //////    db.SaveChanges();
                        //////}
                    }
                }

                return dt;
            }
        }

        public static DateTime[] getStudentSessionDatesByUserId(string emp_user_id)
        {
            int user_id = 0, current_academic_year = 0;
            DateTime[] dt = new DateTime[2];

            using (Context db = new Context())
            {
                //var userData = db.employee.Where(e => e.employee_code == user_code).FirstOrDefault();
                user_id = int.Parse(emp_user_id);

                var lSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                if (lSessionData != null)
                {
                    //////var data_current_academic_year = db.academic_year.Where(e => e.IsCurrentSession).FirstOrDefault();
                    //////current_academic_year = data_current_academic_year.YearId;

                    //////if (lSessionData.SessionEndDate.Year == current_academic_year)
                    //////{
                    //////    dt[0] = lSessionData.SessionStartDate;
                    //////    dt[1] = lSessionData.SessionEndDate;
                    //////}
                    //////else if (lSessionData.SessionEndDate.Year < current_academic_year)
                    //////{
                    //////    dt[0] = data_current_academic_year.SessionStartDate;
                    //////    dt[1] = data_current_academic_year.SessionEndDate;

                    //////    DLL.Models.LeaveSession leaveSession = new DLL.Models.LeaveSession();
                    //////    leaveSession.EmployeeId = user_id;
                    //////    leaveSession.YearId = dt[1].AddDays(-1).AddYears(1).Year;
                    //////    leaveSession.SessionStartDate = dt[1].AddDays(-1);
                    //////    leaveSession.SessionEndDate = dt[1].AddDays(-1).AddYears(1);

                    //////    db.leave_session.Add(leaveSession);
                    //////    db.SaveChanges();
                    //////}
                }

                return dt;
            }
        }

        public static DateTime[] getUserSessionDatesByUserCode(string user_code)
        {
            int user_id = 0;
            DateTime[] dt = new DateTime[2];

            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                if (data_user != null)
                {
                    user_id = data_user.EmployeeId;

                    var lSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                    if (lSessionData != null)
                    {
                        dt[0] = lSessionData.SessionStartDate;
                        dt[1] = lSessionData.SessionEndDate;
                    }
                }

                return dt;
            }
        }

        public static DateTime[] getStaffSessionDatesByUserId(string emp_user_id)
        {
            int user_id = 0;
            DateTime[] dt = new DateTime[2];

            using (Context db = new Context())
            {
                //var userData = db.employee.Where(e => e.EmployeeId == user_id).FirstOrDefault();
                user_id = int.Parse(emp_user_id);

                var lSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                if (lSessionData != null)
                {
                    dt[0] = lSessionData.SessionStartDate;
                    dt[1] = lSessionData.SessionEndDate;
                }

                return dt;
            }
        }

        public static int[] getUserLeavesByUserCode(string user_code)
        {
            int[] leaves = new int[30] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            using (Context db = new Context())
            {
                int access_group_id = 0, user_id = 0;
                int iAvailedSL = 0, iAvailedCL = 0, iAvailedAL = 0, iAvailedOL = 0;
                int iAvailedLT01 = 0, iAvailedLT02 = 0, iAvailedLT03 = 0, iAvailedLT04 = 0, iAvailedLT05 = 0, iAvailedLT06 = 0;
                int iAvailedLT07 = 0, iAvailedLT08 = 0, iAvailedLT09 = 0, iAvailedLT10 = 0, iAvailedLT11 = 0;

                var data_user = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                if (data_user != null)
                {
                    //find access group id
                    access_group_id = data_user.access_group.AccessGroupId;

                    //find user id by using user code
                    user_id = data_user.EmployeeId;

                    //get leave allocated to this user
                    var dbLeavesData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                    if (dbLeavesData != null)
                    {
                        //0-2,6,8,9
                        leaves[0] = dbLeavesData.SickLeaves;
                        leaves[1] = dbLeavesData.CasualLeaves;
                        leaves[2] = dbLeavesData.AnnualLeaves;
                        leaves[6] = dbLeavesData.OtherLeaves;
                        leaves[8] = dbLeavesData.LeaveType01;
                        leaves[9] = dbLeavesData.LeaveType02;
                        leaves[10] = dbLeavesData.LeaveType03;
                        leaves[11] = dbLeavesData.LeaveType04;
                        leaves[16] = dbLeavesData.LeaveType05;
                        leaves[17] = dbLeavesData.LeaveType06;
                        leaves[18] = dbLeavesData.LeaveType07;
                        leaves[19] = dbLeavesData.LeaveType08;
                        leaves[20] = dbLeavesData.LeaveType09;
                        leaves[21] = dbLeavesData.LeaveType10;
                        leaves[22] = dbLeavesData.LeaveType11;

                        //get available and availed leaves count
                        //var dbSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                        //if (dbSessionData != null)
                        //{
                        //get approved leaves falling during the session of particular user
                        var AvailedLeaves = db.leave_application.Where(l => l.EmployeeId == user_id && l.IsActive && l.LeaveStatusId == 2 && (l.FromDate >= dbLeavesData.SessionStartDate && l.ToDate <= dbLeavesData.SessionEndDate)).ToList();
                        if (AvailedLeaves != null && AvailedLeaves.Count > 0)
                        {
                            foreach (var item in AvailedLeaves)
                            {
                                if (item.LeaveTypeId == 1) //for sick leaves
                                {
                                    iAvailedSL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 2)// for casual leaves
                                {
                                    iAvailedCL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 3)// for annual leaves
                                {
                                    iAvailedAL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 4)// for other leaves
                                {
                                    iAvailedOL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 5)// for leaves type 01
                                {
                                    iAvailedLT01 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 6)// for leaves type 01
                                {
                                    iAvailedLT02 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 7)// for leaves type 01
                                {
                                    iAvailedLT03 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 8)// for leaves type 01
                                {
                                    iAvailedLT04 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 9)// for leaves type 01
                                {
                                    iAvailedLT05 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 10)// for leaves type 10
                                {
                                    iAvailedLT06 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 11)// for leaves type 11
                                {
                                    iAvailedLT07 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 12)// for leaves type 12
                                {
                                    iAvailedLT08 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 13)// for leaves type 13
                                {
                                    iAvailedLT09 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 14)// for leaves type 14
                                {
                                    iAvailedLT10 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 15)// for leaves type 15
                                {
                                    iAvailedLT11 += item.DaysCount;
                                }


                            }

                            //set counter to array
                            leaves[3] = iAvailedSL;
                            leaves[4] = iAvailedCL;
                            leaves[5] = iAvailedAL;
                            leaves[7] = iAvailedOL;
                            leaves[12] = iAvailedLT01;
                            leaves[13] = iAvailedLT02;
                            leaves[14] = iAvailedLT03;
                            leaves[15] = iAvailedLT04;
                            leaves[23] = iAvailedLT05;
                            leaves[24] = iAvailedLT06;
                            leaves[25] = iAvailedLT07;
                            leaves[26] = iAvailedLT08;
                            leaves[27] = iAvailedLT09;
                            leaves[28] = iAvailedLT10;
                            leaves[29] = iAvailedLT11;


                        }
                        //}
                    }
                }

                return leaves;
            }
        }

        public static int[] getUserLastMonthLeavesByUserCode(string user_code, string month_year)
        {
            int[] leaves = new int[3];

            DateTime from_date = DateTime.Now;
            DateTime to_date = DateTime.Now;
            from_date = DateTime.ParseExact(month_year, "yyyy-MM", CultureInfo.InvariantCulture);
            to_date = new DateTime(from_date.Year, from_date.Month, System.DateTime.DaysInMonth(from_date.Year, from_date.Month), 23, 59, 59);

            using (Context db = new Context())
            {
                int user_id = 0;
                int iAvailedSL = 0, iAvailedCL = 0, iAvailedAL = 0;

                var data_user = db.employee.Where(e => e.active && e.employee_code == user_code).FirstOrDefault();
                if (data_user != null)
                {
                    //find access group id
                    //access_group_id = data_user.access_group.AccessGroupId;

                    //find user id by using user code
                    user_id = data_user.EmployeeId;

                    //get approved leaves falling during the session of particular user
                    var AvailedLeaves = db.leave_application.Where(l => l.EmployeeId == user_id && l.IsActive && l.LeaveStatusId == 2 && (l.FromDate >= from_date && l.ToDate <= to_date)).ToList();
                    if (AvailedLeaves != null && AvailedLeaves.Count > 0)
                    {
                        foreach (var item in AvailedLeaves)
                        {
                            if (item.LeaveTypeId == 1) //for sick leaves
                            {
                                iAvailedSL += item.DaysCount;
                            }

                            if (item.LeaveTypeId == 2)// for casual leaves
                            {
                                iAvailedCL += item.DaysCount;
                            }

                            if (item.LeaveTypeId == 3)// for annual leaves
                            {
                                iAvailedAL += item.DaysCount;
                            }
                        }

                        //set counter to array
                        leaves[0] = iAvailedSL;
                        leaves[1] = iAvailedCL;
                        leaves[2] = iAvailedAL;
                    }


                }

                return leaves;
            }
        }

        public static int[] getUserLeavesByUserId(string emp_user_id, DateTime dtSessionStDate, DateTime dtSessionEnDate)
        {
            int[] leaves = new int[30] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            using (Context db = new Context())
            {
                int access_group_id = 0, user_id = 0;
                int iAvailedSL = 0, iAvailedCL = 0, iAvailedAL = 0, iAvailedOL = 0;
                int iAvailedLT01 = 0, iAvailedLT02 = 0, iAvailedLT03 = 0, iAvailedLT04 = 0, iAvailedLT05 = 0, iAvailedLT06 = 0, iAvailedLT07 = 0, iAvailedLT08 = 0, iAvailedLT09 = 0, iAvailedLT10 = 0, iAvailedLT11 = 0;

                user_id = int.Parse(emp_user_id);

                var data_user = db.employee.Where(e => e.active && e.EmployeeId == user_id).FirstOrDefault();
                if (data_user != null)
                {
                    //find access group id
                    access_group_id = data_user.access_group.AccessGroupId;

                    //find user id by using user code
                    //user_id = int.Parse(emp_user_id); // db.employee.Where(e => e.employee_code == user_code).FirstOrDefault().EmployeeId;

                    //get leave allocated to this user
                    var dbLeavesData = db.leave_session.Where(l => l.EmployeeId == user_id && (l.SessionStartDate >= dtSessionStDate && l.SessionEndDate <= dtSessionEnDate)).OrderByDescending(o => o.id).FirstOrDefault();
                    if (dbLeavesData != null)
                    {
                        leaves[0] = dbLeavesData.SickLeaves;
                        leaves[1] = dbLeavesData.CasualLeaves;
                        leaves[2] = dbLeavesData.AnnualLeaves;
                        leaves[6] = dbLeavesData.OtherLeaves;
                        leaves[8] = dbLeavesData.LeaveType01;
                        leaves[9] = dbLeavesData.LeaveType02;
                        leaves[10] = dbLeavesData.LeaveType03;
                        leaves[11] = dbLeavesData.LeaveType04;
                        leaves[16] = dbLeavesData.LeaveType05;
                        leaves[17] = dbLeavesData.LeaveType06;
                        leaves[18] = dbLeavesData.LeaveType07;
                        leaves[19] = dbLeavesData.LeaveType08;
                        leaves[20] = dbLeavesData.LeaveType09;
                        leaves[21] = dbLeavesData.LeaveType10;
                        leaves[22] = dbLeavesData.LeaveType11;

                        //get available and availed leaves count
                        //var dbSessionData = db.leave_session.Where(l => l.EmployeeId == user_id).OrderByDescending(o => o.id).FirstOrDefault();
                        //if (dbSessionData != null)
                        //{
                        //get approved leaves falling during the session of particular user
                        var AvailedLeaves = db.leave_application.Where(l => l.EmployeeId == user_id && l.IsActive && l.LeaveStatusId == 2 && (l.FromDate >= dbLeavesData.SessionStartDate && l.ToDate <= dbLeavesData.SessionEndDate)).ToList();
                        if (AvailedLeaves != null && AvailedLeaves.Count > 0)
                        {
                            foreach (var item in AvailedLeaves)
                            {
                                if (item.LeaveTypeId == 1) //for sick leaves
                                {
                                    iAvailedSL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 2)// for casual leaves
                                {
                                    iAvailedCL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 3)// for annual leaves
                                {
                                    iAvailedAL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 4)// for other leaves
                                {
                                    iAvailedOL += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 5)// for leave type 01
                                {
                                    iAvailedLT01 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 6)// for leave type 01
                                {
                                    iAvailedLT02 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 7)// for leave type 01
                                {
                                    iAvailedLT03 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 8)// for leave type 01
                                {
                                    iAvailedLT04 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 9)// for leaves type 01
                                {
                                    iAvailedLT05 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 10)// for leaves type 10
                                {
                                    iAvailedLT06 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 11)// for leaves type 11
                                {
                                    iAvailedLT07 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 12)// for leaves type 12
                                {
                                    iAvailedLT08 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 13)// for leaves type 13
                                {
                                    iAvailedLT09 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 14)// for leaves type 14
                                {
                                    iAvailedLT10 += item.DaysCount;
                                }

                                if (item.LeaveTypeId == 15)// for leaves type 15
                                {
                                    iAvailedLT11 += item.DaysCount;
                                }
                            }

                            //set counter to array
                            leaves[3] = iAvailedSL;
                            leaves[4] = iAvailedCL;
                            leaves[5] = iAvailedAL;
                            leaves[7] = iAvailedOL;
                            leaves[12] = iAvailedLT01;
                            leaves[13] = iAvailedLT02;
                            leaves[14] = iAvailedLT03;
                            leaves[15] = iAvailedLT04;
                            leaves[23] = iAvailedLT05;
                            leaves[24] = iAvailedLT06;
                            leaves[25] = iAvailedLT07;
                            leaves[26] = iAvailedLT08;
                            leaves[27] = iAvailedLT09;
                            leaves[28] = iAvailedLT10;
                            leaves[29] = iAvailedLT11;
                        }
                        //}
                    }
                }

                return leaves;
            }
        }


        ////////////////////////////////////////////////////////////////

        public static List<ViewModels.LeaveTypeInfo> getAllLeaveTypes()
        {
            List<ViewModels.LeaveTypeInfo> toReturn = new List<ViewModels.LeaveTypeInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.LeaveType> leaveType = null;
                try
                {
                    leaveType = db.leave_type.Where(t => t.IsActive).ToList();
                    if (leaveType != null && leaveType.Count > 0)
                    {
                        for (int i = 0; i < leaveType.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.LeaveTypeInfo()
                            {
                                Id = leaveType[i].Id,
                                LeaveTypeText = leaveType[i].LeaveTypeText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    leaveType = new List<DLL.Models.LeaveType>();
                }

                return toReturn;
            }
        }


        public static List<ViewModels.LeaveTypeInfo> getAllLeaveTypesByEmployeeCode(string empCode)
        {
            bool isSickAllowed = false, isCasualAllowed = false, isAnnualAllowed = false, isOtherAllowed = false;
            bool isLeaveType01Allowed = false, isLeaveType02Allowed = false, isLeaveType03Allowed = false, isLeaveType04Allowed = false;
            bool isLeaveType05Allowed = false, isLeaveType06Allowed = false, isLeaveType07Allowed = false, isLeaveType08Allowed = false;
            bool isLeaveType09Allowed = false, isLeaveType10Allowed = false, isLeaveType11Allowed = false;

            int alcSick = 0, alcCasual = 0, alcAnnual = 0, alcOther = 0;
            int alcLT01 = 0, alcLT02 = 0, alcLT03 = 0, alcLT04 = 0, alcLT05 = 0, alcLT06 = 0, alcLT07 = 0, alcLT08 = 0, alcLT09 = 0, alcLT10 = 0, alcLT11 = 0;

            decimal avlSick = 0, avlCasual = 0, avlAnnual = 0, avlOther = 0;
            decimal avlLT01 = 0, avlLT02 = 0, avlLT03 = 0, avlLT04 = 0, avlLT05 = 0, avlLT06 = 0, avlLT07 = 0, avlLT08 = 0, avlLT09 = 0, avlLT10 = 0, avlLT11 = 0;

            List<ViewModels.LeaveTypeInfo> toReturn = new List<ViewModels.LeaveTypeInfo>();

            //allocated
            if (HttpContext.Current.Session["GV_AllocatedSickLeaves"] != null && HttpContext.Current.Session["GV_AllocatedSickLeaves"].ToString() != "")
                alcSick = int.Parse(HttpContext.Current.Session["GV_AllocatedSickLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedCasualLeaves"] != null && HttpContext.Current.Session["GV_AllocatedCasualLeaves"].ToString() != "")
                alcCasual = int.Parse(HttpContext.Current.Session["GV_AllocatedCasualLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedAnnualLeaves"] != null && HttpContext.Current.Session["GV_AllocatedAnnualLeaves"].ToString() != "")
                alcAnnual = int.Parse(HttpContext.Current.Session["GV_AllocatedAnnualLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedOtherLeaves"] != null && HttpContext.Current.Session["GV_AllocatedOtherLeaves"].ToString() != "")
                alcOther = int.Parse(HttpContext.Current.Session["GV_AllocatedOtherLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType01"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType01"].ToString() != "")
                alcLT01 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType01"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType02"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType02"].ToString() != "")
                alcLT02 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType02"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType03"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType03"].ToString() != "")
                alcLT03 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType03"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType04"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType04"].ToString() != "")
                alcLT04 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType04"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType05"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType05"].ToString() != "")
                alcLT05 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType05"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType06"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType06"].ToString() != "")
                alcLT06 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType06"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType07"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType07"].ToString() != "")
                alcLT07 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType07"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType08"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType08"].ToString() != "")
                alcLT08 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType08"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType09"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType09"].ToString() != "")
                alcLT09 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType09"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType10"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType10"].ToString() != "")
                alcLT10 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType10"].ToString());

            if (HttpContext.Current.Session["GV_AllocatedLeaveType11"] != null && HttpContext.Current.Session["GV_AllocatedLeaveType11"].ToString() != "")
                alcLT11 = int.Parse(HttpContext.Current.Session["GV_AllocatedLeaveType11"].ToString());

            //availed
            if (HttpContext.Current.Session["GV_AvailedSickLeaves"] != null && HttpContext.Current.Session["GV_AvailedSickLeaves"].ToString() != "")
                avlSick = decimal.Parse(HttpContext.Current.Session["GV_AvailedSickLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AvailedCasualLeaves"] != null && HttpContext.Current.Session["GV_AvailedCasualLeaves"].ToString() != "")
                avlCasual = decimal.Parse(HttpContext.Current.Session["GV_AvailedCasualLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AvailedAnnualLeaves"] != null && HttpContext.Current.Session["GV_AvailedAnnualLeaves"].ToString() != "")
                avlAnnual = decimal.Parse(HttpContext.Current.Session["GV_AvailedAnnualLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AvailedOtherLeaves"] != null && HttpContext.Current.Session["GV_AvailedOtherLeaves"].ToString() != "")
                avlOther = decimal.Parse(HttpContext.Current.Session["GV_AvailedOtherLeaves"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType01"] != null && HttpContext.Current.Session["GV_AvailedLeaveType01"].ToString() != "")
                avlLT01 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType01"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType02"] != null && HttpContext.Current.Session["GV_AvailedLeaveType02"].ToString() != "")
                avlLT02 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType02"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType03"] != null && HttpContext.Current.Session["GV_AvailedLeaveType03"].ToString() != "")
                avlLT03 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType03"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType04"] != null && HttpContext.Current.Session["GV_AvailedLeaveType04"].ToString() != "")
                avlLT04 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType04"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType05"] != null && HttpContext.Current.Session["GV_AvailedLeaveType05"].ToString() != "")
                avlLT05 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType05"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType06"] != null && HttpContext.Current.Session["GV_AvailedLeaveType06"].ToString() != "")
                avlLT06 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType06"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType07"] != null && HttpContext.Current.Session["GV_AvailedLeaveType07"].ToString() != "")
                avlLT07 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType07"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType08"] != null && HttpContext.Current.Session["GV_AvailedLeaveType08"].ToString() != "")
                avlLT08 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType08"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType09"] != null && HttpContext.Current.Session["GV_AvailedLeaveType09"].ToString() != "")
                avlLT09 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType09"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType10"] != null && HttpContext.Current.Session["GV_AvailedLeaveType10"].ToString() != "")
                avlLT10 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType10"].ToString());

            if (HttpContext.Current.Session["GV_AvailedLeaveType11"] != null && HttpContext.Current.Session["GV_AvailedLeaveType11"].ToString() != "")
                avlLT11 = decimal.Parse(HttpContext.Current.Session["GV_AvailedLeaveType11"].ToString());

            //for testing
            ////avlSick = 10; avlCasual = 9; avlAnnual = 21; avlOther = 0;

            if (avlSick <= alcSick)
            {
                isSickAllowed = true;
            }

            if (avlCasual <= alcCasual)
            {
                isCasualAllowed = true;
            }

            if (avlAnnual <= alcAnnual)
            {
                isAnnualAllowed = true;
            }

            if (avlOther <= alcOther)
            {
                isOtherAllowed = true;
            }

            if (avlLT01 <= alcLT01)
            {
                isLeaveType01Allowed = true;
            }

            if (avlLT02 <= alcLT02)
            {
                isLeaveType02Allowed = true;
            }

            if (avlLT03 <= alcLT03)
            {
                isLeaveType03Allowed = true;
            }

            if (avlLT04 <= alcLT04)
            {
                isLeaveType04Allowed = true;
            }

            if (avlLT05 <= alcLT05)
            {
                isLeaveType05Allowed = true;
            }

            if (avlLT06 <= alcLT06)
            {
                isLeaveType06Allowed = true;
            }

            if (avlLT07 <= alcLT07)
            {
                isLeaveType07Allowed = true;
            }

            if (avlLT08 <= alcLT08)
            {
                isLeaveType08Allowed = true;
            }

            if (avlLT09 <= alcLT09)
            {
                isLeaveType09Allowed = true;
            }

            if (avlLT10 <= alcLT10)
            {
                isLeaveType10Allowed = true;
            }

            if (avlLT11 <= alcLT11)
            {
                isLeaveType11Allowed = true;
            }

            using (Context db = new Context())
            {
                List<DLL.Models.LeaveType> leaveType = null;
                try
                {
                    leaveType = db.leave_type.Where(t => t.IsActive).ToList();
                    if (leaveType != null && leaveType.Count > 0)
                    {
                        for (int i = 0; i < leaveType.Count(); i++)
                        {
                            var data_emp = db.employee.Where(e => e.active && e.employee_code == empCode).FirstOrDefault();
                            if (data_emp != null)
                            {
                                var data_session = db.leave_session.Where(s => s.EmployeeId == data_emp.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();
                                if (data_session != null)
                                {
                                    if (leaveType[i].Id == 1 && data_session.SickLeaves > 0 && isSickAllowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }
                                    if (leaveType[i].Id == 2 && data_session.CasualLeaves > 0 && isCasualAllowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }
                                    if (leaveType[i].Id == 3 && data_session.AnnualLeaves > 0 && isAnnualAllowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }
                                    if (leaveType[i].Id == 4 && data_session.OtherLeaves > 0 && isOtherAllowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 5 && data_session.LeaveType01 > 0 && isLeaveType01Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 6 && data_session.LeaveType02 > 0 && isLeaveType02Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 7 && data_session.LeaveType03 > 0 && isLeaveType03Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 8 && data_session.LeaveType04 > 0 && isLeaveType04Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 9 && data_session.LeaveType05 > 0 && isLeaveType05Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 10 && data_session.LeaveType06 > 0 && isLeaveType06Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 11 && data_session.LeaveType07 > 0 && isLeaveType07Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 12 && data_session.LeaveType08 > 0 && isLeaveType08Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 13 && data_session.LeaveType09 > 0 && isLeaveType09Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 14 && data_session.LeaveType10 > 0 && isLeaveType10Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 15 && data_session.LeaveType11 > 0 && isLeaveType11Allowed)
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 16)//AB
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 17)//PO
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 18)//PLO
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 19)//PLE
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 20)//POE
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 21)//OFF
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 22)//OV
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 23)//OM
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                    if (leaveType[i].Id == 24)//OT
                                    {
                                        toReturn.Add(new ViewModels.LeaveTypeInfo()
                                        {
                                            Id = leaveType[i].Id,
                                            LeaveTypeText = leaveType[i].LeaveTypeText
                                        });
                                    }

                                }
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    leaveType = new List<DLL.Models.LeaveType>();
                }

                return toReturn;
            }
        }

        ////////////////////////////////////////////////////////////////

        public static List<ViewModels.LeaveReasonInfo> getAllLeaveReasons()
        {
            List<ViewModels.LeaveReasonInfo> toReturn = new List<ViewModels.LeaveReasonInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.LeaveReason> leaveReason = null;

                try
                {
                    leaveReason = db.leave_reason.Where(t => t.Id > 1 && t.IsActive == true).ToList();
                    if (leaveReason != null && leaveReason.Count > 0)
                    {
                        for (int i = 0; i < leaveReason.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.LeaveReasonInfo()
                            {
                                Id = leaveReason[i].Id,
                                LeaveReasonText = leaveReason[i].LeaveReasonText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    leaveReason = new List<DLL.Models.LeaveReason>();
                }

                return toReturn;
            }
        }

        ////////////////////////////////////////////////////////////////

        public static List<ViewModels.LeaveStatusInfo> getAllLeaveStatus()
        {
            List<ViewModels.LeaveStatusInfo> toReturn = new List<ViewModels.LeaveStatusInfo>();

            using (Context db = new Context())
            {
                List<DLL.Models.LeaveStatus> leaveStatus = null;
                try
                {
                    leaveStatus = db.leave_status.Where(t => t.IsActive == true).ToList();
                    if (leaveStatus != null && leaveStatus.Count > 0)
                    {
                        for (int i = 0; i < leaveStatus.Count(); i++)
                        {
                            toReturn.Add(new ViewModels.LeaveStatusInfo()
                            {
                                Id = leaveStatus[i].Id,
                                LeaveStatusText = leaveStatus[i].LeaveStatusText
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    leaveStatus = new List<DLL.Models.LeaveStatus>();
                }

                return toReturn;
            }
        }

        ////////////////////////////////////////////////////////////////

        public static List<ViewModels.LeaveApproverInfo> GetAllApprovers(string user_employee_code)
        {
            int group_id = 0, supervisor_id = 0;

            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                List<ViewModels.LeaveApproverInfo> toReturn = new List<ViewModels.LeaveApproverInfo>();

                try
                {
                    var data_employee = db.employee.Where(e => e.active && e.employee_code == user_employee_code).FirstOrDefault();
                    if (data_employee.Group != null)
                    {
                        group_id = data_employee.Group.GroupId;
                        if (group_id != 0)
                        {
                            supervisor_id = db.group.Where(g => g.GroupId == group_id).FirstOrDefault().supervisor_id;

                            if (supervisor_id != 0)
                            {
                                employee = db.employee.Where(t => t.active && t.EmployeeId == supervisor_id).ToList();
                                if (employee != null && employee.Count > 0)
                                {
                                    for (int i = 0; i < employee.Count(); i++)
                                    {
                                        toReturn.Add(new ViewModels.LeaveApproverInfo()
                                        {
                                            Id = employee[i].EmployeeId,
                                            FirstName = employee[i].first_name,
                                            LastName = employee[i].last_name,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        employee = db.employee.Where(t => t.active && t.employee_code == "000000").ToList();
                        if (employee != null && employee.Count > 0)
                        {
                            toReturn.Add(new ViewModels.LeaveApproverInfo()
                            {
                                Id = employee[0].EmployeeId,
                                FirstName = "HR", //employee[i].first_name,
                                LastName = " - Admin" //employee[i].last_name,
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }

                return toReturn;
            }
        }


        ///////////////////////////////////////////////////////////////

        public static List<ViewModels.LeaveApproverInfo> GetApproverInfoForSuperHR(string user_employee_code)
        {
            string strSuperAdminCode = "";

            using (Context db = new Context())
            {
                DLL.Models.Employee employeesData = null;
                List<ViewModels.LeaveApproverInfo> toReturn = new List<ViewModels.LeaveApproverInfo>();

                try
                {
                    strSuperAdminCode = GetAccessCodeValue("SUPER_ADMIN_CODE");
                    if (strSuperAdminCode != null && strSuperAdminCode != "")
                    {
                        employeesData = db.employee.Where(t => t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.active && t.employee_code == strSuperAdminCode).FirstOrDefault();
                        if (employeesData != null)
                        {
                            toReturn.Add(new ViewModels.LeaveApproverInfo()
                            {
                                Id = employeesData.EmployeeId,
                                EmployeeCode = employeesData.employee_code,
                                FirstName = "Super", //employee[i].first_name,
                                LastName = "Admin" //employee[i].last_name,
                            });
                        }
                    }
                    else
                    {
                        employeesData = db.employee.Where(t => t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.active && t.employee_code.Equals("000000")).FirstOrDefault();
                        if (employeesData != null)
                        {
                            toReturn.Add(new ViewModels.LeaveApproverInfo()
                            {
                                Id = employeesData.EmployeeId,
                                FirstName = employeesData.first_name,
                                LastName = employeesData.last_name,
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employeesData = new DLL.Models.Employee();
                }

                return toReturn;
            }
        }

        public static string GetAccessCodeValue(string strAccessCode)
        {
            string strAccessValue = "000000";

            using (Context db = new Context())
            {
                var dbAccessValue = db.access_code_value.Where(a => a.AccessCode.ToUpper() == strAccessCode).FirstOrDefault();
                if (dbAccessValue != null)
                {
                    strAccessValue = dbAccessValue.AccessValue;
                }
            }

            return strAccessValue;
        }



        public static List<ViewModels.LeaveApproverInfo> GetApproverInfoForHR(string user_employee_code, int iGVCampusID)
        {
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employeesList = null;
                List<ViewModels.LeaveApproverInfo> toReturn = new List<ViewModels.LeaveApproverInfo>();

                try
                {
                    var emp = db.employee.Where(t => t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.employee_code == user_employee_code).FirstOrDefault();
                    if (emp != null)
                    {
                        employeesList = db.employee.Where(t => t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.active && t.is_super_hr && t.campus_id == iGVCampusID).ToList();
                        if (employeesList != null && employeesList.Count > 0)
                        {
                            foreach (var ee in employeesList)
                            {
                                toReturn.Add(new ViewModels.LeaveApproverInfo()
                                {
                                    Id = ee.EmployeeId,
                                    EmployeeCode = ee.employee_code,
                                    FirstName = ee.first_name,
                                    LastName = ee.last_name,
                                });
                            }
                        }
                        else
                        {
                            var employeesSHR = db.employee.Where(t => t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.active && t.employee_code.Equals("000000")).FirstOrDefault();
                            if (employeesSHR != null)
                            {
                                toReturn.Add(new ViewModels.LeaveApproverInfo()
                                {
                                    Id = employeesSHR.EmployeeId,
                                    EmployeeCode = employeesSHR.employee_code,
                                    FirstName = employeesSHR.first_name,
                                    LastName = employeesSHR.last_name,
                                });
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employeesList = new List<DLL.Models.Employee>();
                }

                return toReturn;
            }
        }

        //////////////////////////////////////////////////////////////////////

        public static List<ViewModels.LeaveApproverInfo> GetApproverInfo(string user_employee_code, int iGVCampusID)
        {
            int group_id = 0, supervisor_id = 0;

            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employeesList = null;
                List<ViewModels.LeaveApproverInfo> toReturn = new List<ViewModels.LeaveApproverInfo>();

                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == user_employee_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var data_emp_group = data_emp.Group;
                        if (data_emp_group != null)
                        {
                            group_id = data_emp_group.GroupId;
                            if (group_id != 0)
                            {
                                supervisor_id = db.group.Where(g => g.GroupId == group_id).FirstOrDefault().supervisor_id;
                                if (supervisor_id > 0 && supervisor_id != data_emp.EmployeeId)
                                {
                                    employeesList = db.employee.Where(t => t.active && t.EmployeeId == supervisor_id).ToList();
                                    if (employeesList != null && employeesList.Count > 0)
                                    {
                                        for (int i = 0; i < employeesList.Count(); i++)
                                        {
                                            toReturn.Add(new ViewModels.LeaveApproverInfo()
                                            {
                                                Id = employeesList[i].EmployeeId,
                                                EmployeeCode = employeesList[i].employee_code,
                                                FirstName = employeesList[i].first_name,
                                                LastName = employeesList[i].last_name,
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    employeesList = db.employee.Where(t => t.active && t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.campus_id == iGVCampusID).ToList();
                                    if (employeesList != null && employeesList.Count > 0)
                                    {
                                        foreach (var ee in employeesList)
                                        {
                                            toReturn.Add(new ViewModels.LeaveApproverInfo()
                                            {
                                                Id = ee.EmployeeId,
                                                EmployeeCode = ee.employee_code,
                                                FirstName = ee.first_name,
                                                LastName = ee.last_name,
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        else //find SLMs
                        {
                            var data_slm = db.super_line_manager_tagging.Where(g => g.taggedEmployee.EmployeeId == data_emp.EmployeeId).FirstOrDefault();
                            if (data_slm != null)
                            {
                                var edata = db.employee.Where(e => e.active && e.EmployeeId == data_slm.superLineManager.EmployeeId).FirstOrDefault();
                                if (edata != null && edata.EmployeeId != data_slm.superLineManager.EmployeeId)
                                {
                                    toReturn.Add(new ViewModels.LeaveApproverInfo()
                                    {
                                        Id = edata.EmployeeId,
                                        EmployeeCode = edata.employee_code,
                                        FirstName = edata.first_name,
                                        LastName = edata.last_name,
                                    });
                                }
                                else
                                {
                                    employeesList = db.employee.Where(t => t.active && t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.campus_id == iGVCampusID).ToList();
                                    if (employeesList != null && employeesList.Count > 0)
                                    {
                                        foreach (var ee in employeesList)
                                        {
                                            toReturn.Add(new ViewModels.LeaveApproverInfo()
                                            {
                                                Id = ee.EmployeeId,
                                                EmployeeCode = ee.employee_code,
                                                FirstName = ee.first_name,
                                                LastName = ee.last_name,
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                employeesList = db.employee.Where(t => t.active && t.access_group.name.Equals(BLL.TimeTuneRoles.ROLE_HR) && t.campus_id == iGVCampusID).ToList();
                                if (employeesList != null && employeesList.Count > 0)
                                {
                                    foreach (var ee in employeesList)
                                    {
                                        toReturn.Add(new ViewModels.LeaveApproverInfo()
                                        {
                                            Id = ee.EmployeeId,
                                            EmployeeCode = ee.employee_code,
                                            FirstName = ee.first_name,
                                            LastName = ee.last_name,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var employeeData = db.employee.Where(t => t.active && t.employee_code == "000000").FirstOrDefault();
                        if (employeeData != null)
                        {
                            toReturn.Add(new ViewModels.LeaveApproverInfo()
                            {
                                Id = employeeData.EmployeeId,
                                EmployeeCode = employeeData.employee_code,
                                FirstName = employeeData.first_name,
                                LastName = employeeData.last_name,
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employeesList = new List<DLL.Models.Employee>();
                }

                return toReturn;
            }
        }

        public static List<ViewModels.LeaveApproverInfo> GetApproverInfoForLM(string user_employee_code, int iGVCampusID)
        {
            List<DLL.Models.Employee> employee = null;
            List<ViewModels.LeaveApproverInfo> toReturn = new List<ViewModels.LeaveApproverInfo>();

            using (Context db = new Context())
            {
                try
                {
                    var data_emp = db.employee.Where(e => e.active && e.employee_code == user_employee_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var data_slm = db.super_line_manager_tagging.Where(g => g.taggedEmployee.EmployeeId == data_emp.EmployeeId).FirstOrDefault();
                        if (data_slm != null)
                        {
                            var edata = db.employee.Where(e => e.active && e.EmployeeId == data_slm.superLineManager.EmployeeId).FirstOrDefault();
                            if (edata != null && edata.EmployeeId == data_slm.superLineManager.EmployeeId)
                            {
                                toReturn.Add(new ViewModels.LeaveApproverInfo()
                                {
                                    Id = edata.EmployeeId,
                                    FirstName = edata.first_name,
                                    LastName = edata.last_name,
                                });
                            }
                            else
                            {
                                //employee = db.employee.Where(t => t.active && t.employee_code == "000000").ToList();
                                employee = db.employee.Where(t => t.active && t.EmployeeId != 1 && t.access_group.AccessGroupId == 1 && t.campus_id == iGVCampusID).ToList();
                                if (employee != null && employee.Count > 0)
                                {
                                    foreach (var e in employee)
                                    {
                                        toReturn.Add(new ViewModels.LeaveApproverInfo()
                                        {
                                            Id = e.EmployeeId,
                                            FirstName = e.first_name,
                                            LastName = e.last_name
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            //employee = db.employee.Where(t => t.active && t.employee_code == "000000").ToList();
                            employee = db.employee.Where(t => t.active && t.EmployeeId != 1 && t.access_group.AccessGroupId == 1 && t.campus_id == iGVCampusID).ToList();
                            if (employee != null && employee.Count > 0)
                            {
                                foreach (var e in employee)
                                {
                                    toReturn.Add(new ViewModels.LeaveApproverInfo()
                                    {
                                        Id = e.EmployeeId,
                                        FirstName = e.first_name,
                                        LastName = e.last_name
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        //employee = db.employee.Where(t => t.active && t.employee_code == "000000").ToList();
                        employee = db.employee.Where(t => t.active && t.EmployeeId != 1 && t.access_group.AccessGroupId == 1 && t.campus_id == iGVCampusID).ToList();
                        if (employee != null && employee.Count > 0)
                        {
                            foreach (var e in employee)
                            {
                                toReturn.Add(new ViewModels.LeaveApproverInfo()
                                {
                                    Id = e.EmployeeId,
                                    FirstName = e.first_name,
                                    LastName = e.last_name
                                });
                            }
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }

                return toReturn;
            }
        }

        /////////////////////// SEARCH Section /////////////////////////////////

        public static List<ViewModels.LeaveApplicationInfo> getLeaveApplicationsUserDateRange(int user_id, string from_date, string to_date)
        {
            using (Context db = new Context())
            {
                DateTime dtFrom = DateTime.Now, dtTo = DateTime.Now;
                List<LeaveApplication> lApplication = new List<LeaveApplication>();

                string strAttPath = string.Empty;
                List<ViewModels.LeaveApplicationInfo> toReturn = new List<ViewModels.LeaveApplicationInfo>();

                try
                {
                    dtFrom = Convert.ToDateTime(Convert.ToDateTime(from_date).ToString("yyyy-MM-yy"));
                    dtTo = Convert.ToDateTime(Convert.ToDateTime(to_date).ToString("yyyy-MM-yy"));

                    var data_application = db.leave_application.Where(l => l.IsActive && l.EmployeeId == user_id && (l.FromDate >= dtFrom && l.ToDate <= dtTo)).OrderByDescending(l => l.Id).ToList();

                    if (data_application == null)
                    {
                        return toReturn;
                    }

                    lApplication = data_application.ToList();
                    if (lApplication != null && lApplication.Count > 0)
                    {
                        for (int i = 0; i < lApplication.Count(); i++)
                        {
                            int apprId = lApplication[i].ApproverId;
                            int userId = lApplication[i].EmployeeId;
                            var approver_name = db.employee.Where(e => e.active && e.EmployeeId == apprId).FirstOrDefault();
                            var users_name = db.employee.Where(e => e.active && e.EmployeeId == userId).FirstOrDefault();

                            if (lApplication[i].AttachmentFilePath != null && lApplication[i].AttachmentFilePath != "")
                            {
                                if (lApplication[i].AttachmentFilePath.ToLower().Contains("pdf"))
                                {
                                    strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                        "<a class='waves-effect waves-light text-danger text-center' target=\"_blank\" href=\"/Content/LeaveApps/" + lApplication[i].AttachmentFilePath + "\">View</a>" +
                                    "</span>";
                                }
                                else
                                {
                                    strAttPath = "<span data-row='" + lApplication[i].Id + "'>" +
                                            "<a class='waves-effect waves-light text-danger text-center' href=\"javascript:void(popup('" + lApplication[i].AttachmentFilePath + "'));\">View</a>" +
                                        "</span>";
                                }
                            }
                            else
                            {
                                strAttPath = "-";
                            }

                            toReturn.Add(new ViewModels.LeaveApplicationInfo()
                            {
                                Id = lApplication[i].Id,
                                LeaveTypeText = lApplication[i].LeaveType.LeaveTypeText,
                                FromDateText = lApplication[i].FromDate.ToString("yyyy-MM-dd"),
                                ToDateText = lApplication[i].ToDate.ToString("yyyy-MM-dd"),
                                DaysCount = lApplication[i].DaysCount, // ((lApplication[i].ToDate - lApplication[i].FromDate).Days + 1),
                                LeaveReasonText = lApplication[i].LeaveReason.LeaveReasonText,
                                ReasonDetail = lApplication[i].ReasonDetail,
                                EmployeeId = lApplication[i].EmployeeId,
                                UsersName = users_name != null ? (users_name.first_name + ' ' + users_name.last_name) : "n/a",
                                ApproverId = lApplication[i].ApproverId,
                                ApproverName = approver_name != null ? (approver_name.first_name + ' ' + approver_name.last_name) : "n/a",
                                LeaveStatusText = lApplication[i].LeaveStatus.LeaveStatusText,
                                ApproverDetail = lApplication[i].ApproverDetail,
                                IsActive = lApplication[i].IsActive,
                                CreateDateText = lApplication[i].CreateDateTime.ToString("yyyy-MM-dd"),
                                UpdateDateText = lApplication[i].UpdateDateTime.ToString("yyyy-MM-dd"),
                                AttachmentFilePath = strAttPath,
                                actions =
                                    "<span data-row='" + lApplication[i].Id + "'>" +
                                        "<a href=\"javascript:void(editLeaveApplication(" + lApplication[i].Id + "," + lApplication[i].EmployeeId + "," + users_name.access_group.AccessGroupId + "," + users_name.grade.GradeId + "," + users_name.site_id + "," + lApplication[i].LeaveTypeId + "," + "," + lApplication[i].LeaveStatusHODId + "," + lApplication[i].LeaveStatusPRNId + "," + lApplication[i].LeaveStatusHRId + "," + lApplication[i].LeaveStatusVCId + ",'" + lApplication[i].FromDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].ToDate.ToString("yyyy-MM-dd") + "','" + lApplication[i].DaysCount + "','" + lApplication[i].ApproverDetail + "','" + lApplication[i].LeaveStatusId + "','" + lApplication[i].IsActive + "'));\">Edit</a>" +
                                        "<span> / </span>" +
                                        "<a href=\"javascript:void(deleteLeaveApplication(" + lApplication[i].Id + "));\">Delete</a>" +
                                    "</span>"
                            });
                        }
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    lApplication = new List<LeaveApplication>();
                }

                return toReturn;
            }
        }



        /////////////////////////////////////////////////////////////////////

    }

    public class LeavesListReportResultSet
    {
        public static List<ViewModels.LeavesListReportLog> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeavesListReportLog> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.LeavesListReportLog> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.LeavesListReportLog> FilterResult(string search, List<ViewModels.LeavesListReportLog> dtResult)
        {
            IQueryable<ViewModels.LeavesListReportLog> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower()) ||
                         p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower())
                         ||
                         p.AllocatedSickLeaves != null && p.AllocatedSickLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AllocatedCasualLeaves != null && p.AllocatedCasualLeaves.ToString().ToLower().Contains(search.ToLower())
                          ||
                         p.AllocatedAnnualLeaves != null && p.AllocatedAnnualLeaves.ToString().ToLower().Contains(search.ToLower())
                         //||
                         //p.AllocatedOtherLeaves != null && p.AllocatedOtherLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AvailedSickLeaves != null && p.AvailedSickLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AvailedCasualLeaves != null && p.AvailedCasualLeaves.ToString().ToLower().Contains(search.ToLower())
                         ||
                         p.AvailedAnnualLeaves != null && p.AvailedAnnualLeaves.ToString().ToLower().Contains(search.ToLower())
                        // ||
                        //p.AvailedOtherLeaves != null && p.AvailedOtherLeaves.ToString().ToLower().Contains(search.ToLower())


                        || (p.Last_Name != null && p.Last_Name.ToLower().Contains(search.ToLower()))


                    )
                )
                //&& (columnFilters[0] == null || (p.first_name != null && p.first_name.ToLower().Contains(columnFilters[0].ToLower())))
                //&& (columnFilters[1] == null || (p.last_name != null && p.last_name.ToLower().Contains(columnFilters[1].ToLower())))
                //&& (columnFilters[2] == null || (p.employee_code != null && p.employee_code.ToLower().Contains(columnFilters[2].ToLower())))
                //&& (columnFilters[3] == null || (p.email != null && p.email.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.function_name != null && p.function_name.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.department_name != null && p.department_name.ToLower().Contains(columnFilters[5].ToLower())))
                //&& (columnFilters[6] == null || (p.designation_name != null && p.designation_name.ToLower().Contains(columnFilters[6].ToLower())))
                //&& (columnFilters[7] == null || (p.access_group_name != null && p.access_group_name.ToLower().Contains(columnFilters[7].ToLower())))
                //&& (columnFilters[8] == null || (p.group_name != null && p.group_name.ToLower().Contains(columnFilters[8].ToLower())))
                //&& (columnFilters[9] == null || (p.grade_name != null && p.grade_name.ToLower().Contains(columnFilters[9].ToLower())))
                //&& (columnFilters[10] == null || (p.region_name != null && p.region_name.ToLower().Contains(columnFilters[10].ToLower())))
                //&& (columnFilters[11] == null || (p.location_name != null && p.location_name.ToLower().Contains(columnFilters[11].ToLower())))
                //&& (columnFilters[13] == null || (p.type_of_employment_name != null && p.type_of_employment_name.ToLower().Contains(columnFilters[13].ToLower())))
                );

            return results;
        }
    }

    public class LeavesApplyStatusReportResultSet
    {
        public static List<ViewModels.LeavesApplyStatusReportLog> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeavesApplyStatusReportLog> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.LeavesApplyStatusReportLog> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.LeavesApplyStatusReportLog> FilterResult(string search, List<ViewModels.LeavesApplyStatusReportLog> dtResult)
        {
            IQueryable<ViewModels.LeavesApplyStatusReportLog> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower()) ||
                         p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower())

                        || (p.Last_Name != null && p.Last_Name.ToLower().Contains(search.ToLower()))


                    )
                )
                //&& (columnFilters[0] == null || (p.first_name != null && p.first_name.ToLower().Contains(columnFilters[0].ToLower())))
                //&& (columnFilters[1] == null || (p.last_name != null && p.last_name.ToLower().Contains(columnFilters[1].ToLower())))
                //&& (columnFilters[2] == null || (p.employee_code != null && p.employee_code.ToLower().Contains(columnFilters[2].ToLower())))
                //&& (columnFilters[3] == null || (p.email != null && p.email.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.function_name != null && p.function_name.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.department_name != null && p.department_name.ToLower().Contains(columnFilters[5].ToLower())))
                //&& (columnFilters[6] == null || (p.designation_name != null && p.designation_name.ToLower().Contains(columnFilters[6].ToLower())))
                //&& (columnFilters[7] == null || (p.access_group_name != null && p.access_group_name.ToLower().Contains(columnFilters[7].ToLower())))
                //&& (columnFilters[8] == null || (p.group_name != null && p.group_name.ToLower().Contains(columnFilters[8].ToLower())))
                //&& (columnFilters[9] == null || (p.grade_name != null && p.grade_name.ToLower().Contains(columnFilters[9].ToLower())))
                //&& (columnFilters[10] == null || (p.region_name != null && p.region_name.ToLower().Contains(columnFilters[10].ToLower())))
                //&& (columnFilters[11] == null || (p.location_name != null && p.location_name.ToLower().Contains(columnFilters[11].ToLower())))
                //&& (columnFilters[13] == null || (p.type_of_employment_name != null && p.type_of_employment_name.ToLower().Contains(columnFilters[13].ToLower())))
                );

            return results;
        }
    }


    public class LeavesValidityReportResultSet
    {
        public static List<ViewModels.LeavesValidityReportLog> GetResult(string search, string sortOrder, int start, int length, List<ViewModels.LeavesValidityReportLog> dtResult)
        {
            return FilterResult(search, dtResult).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public static int Count(string search, List<ViewModels.LeavesValidityReportLog> dtResult)
        {
            return FilterResult(search, dtResult).Count();
        }

        private static IQueryable<ViewModels.LeavesValidityReportLog> FilterResult(string search, List<ViewModels.LeavesValidityReportLog> dtResult)
        {
            IQueryable<ViewModels.LeavesValidityReportLog> results = dtResult.AsQueryable();

            results = results.Where(p =>
                (
                    search == null ||
                    (
                            p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower()) ||
                         p.Employee_Code != null && p.Employee_Code.ToLower().Contains(search.ToLower())

                        || (p.Last_Name != null && p.Last_Name.ToLower().Contains(search.ToLower()))


                    )
                )
                //&& (columnFilters[0] == null || (p.first_name != null && p.first_name.ToLower().Contains(columnFilters[0].ToLower())))
                //&& (columnFilters[1] == null || (p.last_name != null && p.last_name.ToLower().Contains(columnFilters[1].ToLower())))
                //&& (columnFilters[2] == null || (p.employee_code != null && p.employee_code.ToLower().Contains(columnFilters[2].ToLower())))
                //&& (columnFilters[3] == null || (p.email != null && p.email.ToLower().Contains(columnFilters[3].ToLower())))
                //&& (columnFilters[4] == null || (p.function_name != null && p.function_name.ToLower().Contains(columnFilters[4].ToLower())))
                //&& (columnFilters[5] == null || (p.department_name != null && p.department_name.ToLower().Contains(columnFilters[5].ToLower())))
                //&& (columnFilters[6] == null || (p.designation_name != null && p.designation_name.ToLower().Contains(columnFilters[6].ToLower())))
                //&& (columnFilters[7] == null || (p.access_group_name != null && p.access_group_name.ToLower().Contains(columnFilters[7].ToLower())))
                //&& (columnFilters[8] == null || (p.group_name != null && p.group_name.ToLower().Contains(columnFilters[8].ToLower())))
                //&& (columnFilters[9] == null || (p.grade_name != null && p.grade_name.ToLower().Contains(columnFilters[9].ToLower())))
                //&& (columnFilters[10] == null || (p.region_name != null && p.region_name.ToLower().Contains(columnFilters[10].ToLower())))
                //&& (columnFilters[11] == null || (p.location_name != null && p.location_name.ToLower().Contains(columnFilters[11].ToLower())))
                //&& (columnFilters[13] == null || (p.type_of_employment_name != null && p.type_of_employment_name.ToLower().Contains(columnFilters[13].ToLower())))
                );

            return results;
        }
    }


}
