using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EBI_FetchJob
{
    public class HaTransitEBI
    {
        [Key]
        public int HaTransitId { get; set; }
        public string C_Name { get; set; }
        public string C_Unique { get; set; }
        public DateTime? C_Date { get; set; }
        public string C_Time { get; set; }
        public int L_UID { get; set; }
        public int L_TID { get; set; }
        public bool active { get; set; }
    }

    public class Context : DbContext
    {
        //string ConnectionString = "SERVER=103.4.92.87;DATABASE=shadow;User Id=root;PASSWORD=root;SslMode =none;port=3306;Connect timeout=9600;";
        public static string connectionString = ConfigurationManager.ConnectionStrings["Shadow"].ConnectionString;
        //public static string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;

        public Context()
            : base(connectionString)
        {

            // : base("Server=10.200.65.194; Database=TimeTune; User Id=sa; Password=resco123!!")
            //public Context():base("Server=192.168.0.229; Database=TimeTune; User Id=sa; Password=mngr")
            //this.Configuration.LazyLoadingEnabled = true;

        }

        public static string GetConnectionstring()
        {
            return connectionString;
        }


        public DbSet<HaTransitEBI> ha_transit_EBI { get; set; }


    }
}
