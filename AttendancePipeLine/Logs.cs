using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendancePipeLine
{
    public static class Logs
    {
        public static void writeLogs(string message)
        {
            StreamWriter sw= null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(message);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }

        }
    }
}
