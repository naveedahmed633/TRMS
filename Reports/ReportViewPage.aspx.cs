using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MvcApplication1.Reports
{
    public partial class ReportViewPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                RenderReport();
            }
        }

        private void RenderReport()
        {
            ReportViewer1.Reset();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Report1.rdlc");
            
         
            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1",GetConsolidateAttendane()));
        }

        private DataTable GetConsolidateAttendane()
        {
            DataTable dt = new DataTable();
            string conStr = @"Data Source=127.0.0.1; Initial Catalog=TimeTune; User Id=sa; Password=resco123!; Integrated Security=False";
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                SqlDataAdapter adp = new SqlDataAdapter("select date, (case  when final_remarks like 'AB' then 'A' else 'P' end) final_remarks,employee_EmployeeId from ConsolidatedAttendances  where MONTH(date)=7   and (employee_EmployeeId =29 or employee_EmployeeId =30) ", conn);
                adp.Fill(dt);
            }
            return dt;
        }
    }
}