using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_UNIS.ViewModels
{
    public class DeviceStatusReport
    {

        //tTerminl
        public int L_ID { get; set; }

        public string C_Name { get; set; }

        //[tTerminalStateLog]
        public int L_TID { get; set; }

        public string C_EventTime { get; set; }

        public string L_Class { get; set; }

        public string C_Office { get; set; } //Branch

        public string C_Place { get; set; } //Location

        public string C_IPAddr { get; set; } //IP Address

        public string L_Detail { get; set; }

    }

    public class DevicesStatus
    {
        //tTerminl
        public int L_ID { get; set; }

        public string C_Name { get; set; }

        //[tTerminalStateLog]
        public int L_TID { get; set; }

        public string C_EventTime { get; set; }

        public string L_Class { get; set; }

        public string C_Office { get; set; } //Branch

        public string C_Place { get; set; } //Location

        public string C_IPAddr { get; set; } //IP Address

        public string L_Detail { get; set; }
    }


    public class DevicesStatusCount
    {
        //tTerminl
        public int L_ID { get; set; }

        public string C_Name { get; set; }

        //[tTerminalStateLog]
        public int L_TID { get; set; }

        public string C_EventTime { get; set; }

        public string L_Class { get; set; }

        public string C_Office { get; set; } //Branch

        public string C_Place { get; set; } //Location

        public string C_IPAddr { get; set; } //IP Address

        public string L_Detail { get; set; }

        public int ConnCount { get; set; }

        public int DisConnCount { get; set; }

        public int TotalCount { get; set; }
    }


    public class DevicesUnregister
    {
        //tTerminl
        public int L_ID { get; set; }

        public string C_Name { get; set; }

        public string Branch { get; set; }

        public string Location { get; set; }

        public string C_IPAddr { get; set; }
    }

    public class C_office_List
    {
        public List<C_office> coffice_list;
    }

    public class C_office
    {
        public int c_id { get; set; }

        public string c_name { get; set; }


    }

    public class DevicesStatusExcelDownload
    {
        //[tTerminalStateLog]
        public string Device_Id { get; set; }

        //[tTerminalStateLog]
        public string Date_Time { get; set; }

        //tTerminal
        public string Device_Name { get; set; }

        //tTerminal - Branch
        public string Branch { get; set; }

        //tTerminal - Location
        public string Location { get; set; }

        //tTerminal - IP Address
        public string C_IPAddr { get; set; }

        //[tTerminalStateLog]
        public string Device_Status { get; set; }
    }
}
