using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNIS_UNIS_SyncJob
{
    public class ContextUNISDataLogMaster : DbContext
    {
        public static string connectionStringUNISDataLogMaster = ConfigurationManager.ConnectionStrings["UNISDataLogMaster"].ConnectionString;

        public ContextUNISDataLogMaster()
            : base(connectionStringUNISDataLogMaster)
        {
            // : base("Server=10.200.65.194; Database=TimeTune; User Id=sa; Password=resco123!!")
            //public Context():base("Server=192.168.0.229; Database=TimeTune; User Id=sa; Password=mngr")
            //this.Configuration.LazyLoadingEnabled = true;

            //if (ConfigurationManager.AppSettings["DBTimeOut"] != null && ConfigurationManager.AppSettings["DBTimeOut"].ToString() != "")
            //{
            //    this.Database.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["DBTimeOut"].ToString()); //600 = 10 * 60 = 10 min
            //}
        }


        public DbSet<tUsers> UNIS_tUsers { get; set; }


        public DbSet<iUserCards> UNIS_iUserCards { get; set; }
        public DbSet<iUserCardsResco> UNIS_iUserCardsResco { get; set; }


        public DbSet<iUserFingers> UNIS_iUserFingers { get; set; }
        public DbSet<tEmployes> UNIS_tEmployes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }


    public class ContextUNISRegion : DbContext
    {
        public static string connectionStringUNISRegion = ConfigurationManager.ConnectionStrings["UNISRegion"].ConnectionString;

        public ContextUNISRegion()
            : base(connectionStringUNISRegion)
        {
            // : base("Server=10.200.65.194; Database=TimeTune; User Id=sa; Password=resco123!!")
            //public Context():base("Server=192.168.0.229; Database=TimeTune; User Id=sa; Password=mngr")
            //this.Configuration.LazyLoadingEnabled = true;

            //if (ConfigurationManager.AppSettings["DBTimeOut"] != null && ConfigurationManager.AppSettings["DBTimeOut"].ToString() != "")
            //{
            //    this.Database.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["DBTimeOut"].ToString()); //600 = 10 * 60 = 10 min
            //}
        }


        public DbSet<tUser> UNIS_tUser { get; set; }


        public DbSet<iUserCard> UNIS_iUserCard { get; set; }
        public DbSet<iUserCardResco> UNIS_iUserCardResco { get; set; }

        public DbSet<iUserFinger> UNIS_iUserFinger { get; set; }
        public DbSet<tEmploye> UNIS_tEmploye { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    #region unis_data_log

    public class tUsers
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_ID { get; set; }

        [StringLength(64)]
        public string C_Name { get; set; }

        [StringLength(64)]
        public string C_Unique { get; set; }

        public int? L_Type { get; set; }

        [StringLength(14)]
        public string C_RegDate { get; set; }

        public int? L_OptDateLimit { get; set; }

        [StringLength(16)]
        public string C_DateLimit { get; set; }

        public int? L_AccessType { get; set; }

        [StringLength(64)]
        public string C_Password { get; set; }

        public int? L_Identify { get; set; }

        public int? L_VerifyLevel { get; set; }

        [StringLength(4)]
        public string C_AccessGroup { get; set; }

        [StringLength(4)]
        public string C_PassbackStatus { get; set; }

        public int? L_VOIPUsed { get; set; }

        public int? L_DoorOpen { get; set; }

        public int? L_AutoAnswer { get; set; }

        public int? L_EnableMeta1 { get; set; }

        public int? L_RingCount1 { get; set; }

        [StringLength(20)]
        public string C_LoginID1 { get; set; }

        [StringLength(36)]
        public string C_SipAddr1 { get; set; }

        public int? L_EnableMeta2 { get; set; }

        public int? L_RingCount2 { get; set; }

        [StringLength(20)]
        public string C_LoginID2 { get; set; }

        [StringLength(36)]
        public string C_SipAddr2 { get; set; }

        [StringLength(128)]
        public string C_UserMessage { get; set; }

        public int? L_Blacklist { get; set; }

        public int? L_IsNotice { get; set; }

        [StringLength(128)]
        public string C_Notice { get; set; }

        [StringLength(14)]
        public string C_PassbackTime { get; set; }

        public int? L_ExceptPassback { get; set; }

        public int? L_DataCheck { get; set; }

        public int? L_Partition { get; set; }

        public int? L_FaceIdentify { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_DuressFinger { get; set; }

        public int? L_AuthValue { get; set; }

        [StringLength(64)]
        public string C_RemotePW { get; set; }

        public int? L_WrongCount { get; set; }

        public int? L_LogonLocked { get; set; }

        [StringLength(14)]
        public string C_LogonDateTime { get; set; }

        [StringLength(14)]
        public string C_UdatePassword { get; set; }

        [StringLength(1)]
        public string C_MustChgPwd { get; set; }

        public int? L_RegServer { get; set; }

        public bool U_Active { get; set; }
        public bool U_Active2 { get; set; }
        public bool U_Active3 { get; set; }
    }

    public class iUserCards
    {
        [Key]
        [StringLength(24)]
        public string C_CardNum { get; set; }

        public int? L_UID { get; set; }

        public int? L_DataCheck { get; set; }

        public bool C_Active { get; set; }
        public bool C_Active2 { get; set; }
        public bool C_Active3 { get; set; }
    }

    public class iUserCardsResco
    {
        [Key]
        [StringLength(24)]
        public int Id { get; set; }
        public string C_CardNum { get; set; }
        public int L_UID { get; set; }
        public int Server_ID { get; set; }
        public bool IsSynced { get; set; }
        public bool IsSynced2 { get; set; }
        public bool IsSynced3 { get; set; }
        public DateTime LastDateTime { get; set; }
    }

    public class iUserFingers
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        public int? L_IsWideChar { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_TextFIR { get; set; }

        public bool F_Active { get; set; }
        public bool F_Active2 { get; set; }
        public bool F_Active3 { get; set; }
    }
    
    public class tEmployes
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        [StringLength(8)]
        public string C_IncludeDate { get; set; }

        [StringLength(8)]
        public string C_RetiredDate { get; set; }

        [StringLength(30)]
        public string C_Office { get; set; }

        [StringLength(30)]
        public string C_Post { get; set; }

        [StringLength(30)]
        public string C_Staff { get; set; }

        [StringLength(4)]
        public string C_Authority { get; set; }

        [StringLength(4)]
        public string C_Work { get; set; }

        [StringLength(4)]
        public string C_Money { get; set; }

        [StringLength(4)]
        public string C_Meal { get; set; }

        [StringLength(255)]
        public string C_Phone { get; set; }

        [StringLength(255)]
        public string C_Email { get; set; }

        [StringLength(255)]
        public string C_Address { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }

        public bool E_Active { get; set; }
        public bool E_Active2 { get; set; }
        public bool E_Active3 { get; set; }
    }

    #endregion

    #region unis

    public class tUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_ID { get; set; }

        [StringLength(64)]
        public string C_Name { get; set; }

        [StringLength(64)]
        public string C_Unique { get; set; }

        public int? L_Type { get; set; }

        [StringLength(14)]
        public string C_RegDate { get; set; }

        public int? L_OptDateLimit { get; set; }

        [StringLength(16)]
        public string C_DateLimit { get; set; }

        public int? L_AccessType { get; set; }

        [StringLength(64)]
        public string C_Password { get; set; }

        public int? L_Identify { get; set; }

        public int? L_VerifyLevel { get; set; }

        [StringLength(4)]
        public string C_AccessGroup { get; set; }

        [StringLength(4)]
        public string C_PassbackStatus { get; set; }

        public int? L_VOIPUsed { get; set; }

        public int? L_DoorOpen { get; set; }

        public int? L_AutoAnswer { get; set; }

        public int? L_EnableMeta1 { get; set; }

        public int? L_RingCount1 { get; set; }

        [StringLength(20)]
        public string C_LoginID1 { get; set; }

        [StringLength(36)]
        public string C_SipAddr1 { get; set; }

        public int? L_EnableMeta2 { get; set; }

        public int? L_RingCount2 { get; set; }

        [StringLength(20)]
        public string C_LoginID2 { get; set; }

        [StringLength(36)]
        public string C_SipAddr2 { get; set; }

        [StringLength(128)]
        public string C_UserMessage { get; set; }

        public int? L_Blacklist { get; set; }

        public int? L_IsNotice { get; set; }

        [StringLength(128)]
        public string C_Notice { get; set; }

        [StringLength(14)]
        public string C_PassbackTime { get; set; }

        public int? L_ExceptPassback { get; set; }

        public int? L_DataCheck { get; set; }

        public int? L_Partition { get; set; }

        public int? L_FaceIdentify { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_DuressFinger { get; set; }

        public int? L_AuthValue { get; set; }

        [StringLength(64)]
        public string C_RemotePW { get; set; }

        public int? L_WrongCount { get; set; }

        public int? L_LogonLocked { get; set; }

        [StringLength(14)]
        public string C_LogonDateTime { get; set; }

        [StringLength(14)]
        public string C_UdatePassword { get; set; }

        [StringLength(1)]
        public string C_MustChgPwd { get; set; }

        public int? L_RegServer { get; set; }
    }

    public class iUserCard
    {
        [Key]
        [StringLength(24)]
        public string C_CardNum { get; set; }

        public int? L_UID { get; set; }

        public int? L_DataCheck { get; set; }
    }

    public class iUserCardResco
    {
        [Key]
        [StringLength(24)]
        public int Id { get; set; }
        public string C_CardNum { get; set; }
        public int L_UID { get; set; }
        public int Server_ID { get; set; }
        public bool IsSynced { get; set; }
        public bool IsSynced2 { get; set; }
        public bool IsSynced3 { get; set; }
        public DateTime LastDateTime { get; set; }
    }

    public class iUserFinger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        public int? L_IsWideChar { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_TextFIR { get; set; }
    }
    
    public class tEmploye
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        [StringLength(8)]
        public string C_IncludeDate { get; set; }

        [StringLength(8)]
        public string C_RetiredDate { get; set; }

        [StringLength(30)]
        public string C_Office { get; set; }

        [StringLength(30)]
        public string C_Post { get; set; }

        [StringLength(30)]
        public string C_Staff { get; set; }

        [StringLength(4)]
        public string C_Authority { get; set; }

        [StringLength(4)]
        public string C_Work { get; set; }

        [StringLength(4)]
        public string C_Money { get; set; }

        [StringLength(4)]
        public string C_Meal { get; set; }

        [StringLength(255)]
        public string C_Phone { get; set; }

        [StringLength(255)]
        public string C_Email { get; set; }

        [StringLength(255)]
        public string C_Address { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }
    }

    #endregion

}
