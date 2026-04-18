using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceOnOff;
using System.Windows;
using System.Diagnostics;
using BLL.ViewModels;
//using System.ServiceProcess;

namespace BLL.Services
{
    public class TimeTuneServices
    {
       
        public static List<ServicesViewModel> getServices()
        {
            var entity = service.GetService();
            List<ServicesViewModel> toReturn = new List<ServicesViewModel>();

            for(int i=0;i<entity.Length;i++)
            {
                ServicesViewModel view = new ServicesViewModel();
                view.serviceName = entity[i].serviceName;
                view.Description = entity[i].description;
                view.status = entity[i].status;
                toReturn.Add(view);
            }
            return toReturn;
        }

    }
}
