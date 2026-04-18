using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testing
{
    public class Program
    {
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var now = TimeZoneInfo.Local;
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime, now) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
        public void Main(string[] args)
        {
            double result = DateTimeToUnixTimestamp(DateTime.Now);
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
