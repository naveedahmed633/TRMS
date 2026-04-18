using System;
using System.Windows;
using System.ServiceProcess;
using System.Diagnostics;



public class serviceView
{
    public string serviceName { get; set;}
    public string description { get; set; }
    public string status { get; set; }
}


public class service
{
    public static void StartService(string serviceName, int timeoutMilliseconds)
    {
        ServiceController service = new ServiceController(serviceName);
        try
        {
            int millisec1 = 0;
            TimeSpan timeout;
            // count the rest of the timeout
            int millisec2 = Environment.TickCount;
            timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec1));
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);

        }
        catch (Exception e)
        {
            Trace.WriteLine(e.Message);
        }
    }

    public static serviceView[] GetService()
    {
        try
        {
            ServiceController[] services = ServiceController.GetServices();
            serviceView[] toReturn = new serviceView[services.Length];
            for (int i = 0; i < services.Length;i++ )
            {
                serviceView view = new serviceView();
                view.serviceName = services[i].DisplayName;
                view.description = services[i].ServiceName;
                view.status = services[i].Status.ToString();
                toReturn[i] = view;
            }
            return toReturn;
        }
        catch(Exception ex)
        {
            serviceView[] toReturn=new serviceView[0];
            return toReturn;
        }
    }

    public static void StopService(string serviceName, int timeoutMilliseconds)
    {
        ServiceController service = new ServiceController(serviceName);
        try
        {
            int millisec1 = 0;
            TimeSpan timeout;
            if (service.Status == ServiceControllerStatus.Running)
            {
                millisec1 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }


        }
        catch (Exception e)
        {
            Trace.WriteLine(e.Message);
        }
    }


    public static void RestartService(string serviceName, int timeoutMilliseconds)
    {
        ServiceController service = new ServiceController(serviceName);
        try
        {
            int millisec1 = 0;
            TimeSpan timeout;
            if (service.Status == ServiceControllerStatus.Running)
            {
                millisec1 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            // count the rest of the timeout
            int millisec2 = Environment.TickCount;
            timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);

        }
        catch (Exception e)
        {
            Trace.WriteLine(e.Message);
        }
    }

}