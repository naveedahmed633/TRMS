using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ADMIN
{
    public class QueryRun
    {
        public static string run (string query)
        {
               using (var db=new DLL.Models.Context())
               {
                   try
                   {
                       var result = db.Database.SqlQuery<DLL.Models.Employee>(query);
                       string toReturn=result.Select(p=>p.employee_code).FirstOrDefault();
                       return "Successfull";
                   }
                   catch(Exception ex)
                   {
                       return "Failed";
                   }
               }
        }

    }
}
