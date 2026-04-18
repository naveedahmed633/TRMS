using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceOnOff
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Service Name \t|| Service Description \t|| Service Status ||");
            var values = service.GetService();
            for (int i = 0; i < values.Length;i++ )
            {
                Console.WriteLine(values[i].serviceName+"\t ||"+values[i].description+"\t||"+values[i].status+"||");
            }
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%$$$$$$$$$$$$$$$$$$$$#######################################");
            Console.WriteLine("If you want to start the service write start and enter \n and if you want to stop and press enter:  ");

            
            string val = Console.ReadLine();
            if (val.Equals("start"))
            {
                Console.WriteLine("Enter the Name of the service");
                string serviceName=Console.ReadLine();
                service.StartService(serviceName, 1000);
            }
            else if (val.Equals("stop"))
            {
                Console.WriteLine("Enter the Name of the service");
                string serviceName = Console.ReadLine();
                service.StopService(serviceName, 1000);
            }
            
            Console.ReadLine();
        }
    }
}
