using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddEmployeePassword.Models;

namespace AddEmployeePassword.Models
{
    public class Context2:DbContext
    {
        
            public Context2()
                : base("Data Source=127.0.0.1; Initial Catalog=EmployeePassword; User Id=sa; Password=resco123!; Integrated Security=False")
            {
                // : base("Server=10.200.65.194; Database=TimeTune; User Id=sa; Password=resco123!!")
                //public Context():base("Server=192.168.0.229; Database=TimeTune; User Id=sa; Password=mngr")
                //this.Configuration.LazyLoadingEnabled = true;
            }
           
            public DbSet<UserPassword> UserPassword { get; set; }

        
    }
}
