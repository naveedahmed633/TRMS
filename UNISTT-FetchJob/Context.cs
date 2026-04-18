using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNISTT_FetchJob
{
    public class UNISTTHaTransit
    {
        [Key]
        public int UTHaTransitId { get; set; }
        public string C_Name { get; set; }
        public string C_Unique { get; set; }
        public DateTime? C_Date { get; set; }
        public string C_Time { get; set; }
        public int L_UID { get; set; }
        public int L_TID { get; set; }
        public bool active { get; set; }
        public int Region_ID { get; set; }
    }

    public class Context : DbContext
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["UNISTimeTune"].ConnectionString;
        public Context()
            : base(connectionString)
        {
            // : base("Server=10.200.65.194; Database=TimeTune; User Id=sa; Password=resco123!!")
            //public Context():base("Server=192.168.0.229; Database=TimeTune; User Id=sa; Password=mngr")
            //this.Configuration.LazyLoadingEnabled = true;

            //if (ConfigurationManager.AppSettings["DBTimeOut"] != null && ConfigurationManager.AppSettings["DBTimeOut"].ToString() != "")
            //{
            //    this.Database.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["DBTimeOut"].ToString()); //600 = 10 * 60 = 10 min
            //}
        }


        public DbSet<UNISTTHaTransit> UNISTT_ha_transit { get; set; }


    }
}
