namespace DLL_UNIS.Models
{
    using System.Data.Entity;
    using System.Linq;
    using System.Configuration;

    /// <summary>UCDB (see USDB.txt / dbo schema). Existing database — no EF migrations.</summary>
    public partial class UNISContext : DbContext
    {
        public static string connectionString = GetConnectionString(ConfigurationManager.ConnectionStrings["UNISContext"].ConnectionString.Trim());

        static UNISContext()
        {
            Database.SetInitializer<UNISContext>(null);
        }

        public UNISContext()
            : base(connectionString)
        {
        }

        public static string GetConnectionString(string rawConnectionString)
        {
            return rawConnectionString.ToLower();
        }

        public static string GetConnectionStringSetting(string rawConnectionString)
        {
            string newConnectionString = string.Empty;

            if (rawConnectionString.Contains(";"))
            {
                string[] arrConnectionString = rawConnectionString.Split(';');

                if (arrConnectionString.Count() >= 4)
                {
                    arrConnectionString[0] = arrConnectionString[0].Trim();
                    arrConnectionString[1] = arrConnectionString[1].Trim();
                    arrConnectionString[2] = arrConnectionString[2].Trim();
                    arrConnectionString[3] = arrConnectionString[3].Trim();

                    newConnectionString = "Data Source=" + arrConnectionString[0].Split('=')[1] + "; Initial Catalog=UCDB; User Id=sa; Password=Test@1234;";
                }
            }

            return newConnectionString;
        }

        public virtual DbSet<tUser> tUsers { get; set; }
        public virtual DbSet<tTerminal> tTerminals { get; set; }
        public virtual DbSet<iUserPicture> iUserPictures { get; set; }
        public virtual DbSet<iUserCard> iUserCards { get; set; }
        public virtual DbSet<iUserFinger> iUserFingers { get; set; }
        public virtual DbSet<iUserMobileKey> iUserMobileKeys { get; set; }
        public virtual DbSet<tTerminalStateLog> tTerminalStateLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tUser>()
                .Property(e => e.C_Name)
                .IsUnicode(true);

            modelBuilder.Entity<tUser>()
                .Property(e => e.C_Unique)
                .IsUnicode(false);

            modelBuilder.Entity<tTerminal>()
                .Property(e => e.C_Name)
                .IsUnicode(true);

            modelBuilder.Entity<tTerminal>()
                .Property(e => e.C_Version)
                .IsUnicode(true);

            modelBuilder.Entity<tTerminal>()
                .Property(e => e.C_Remark)
                .IsUnicode(true);

            modelBuilder.Entity<iUserPicture>()
                .Property(e => e.B_Picture)
                .IsOptional();

            modelBuilder.Entity<tTerminalStateLog>()
                .Property(e => e.C_EventInfo)
                .IsUnicode(true);
        }
    }
}
