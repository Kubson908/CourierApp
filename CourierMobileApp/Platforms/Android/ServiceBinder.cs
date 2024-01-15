using Android.OS;

namespace CourierMobileApp.Platforms.Android;

public class ServiceBinder : Binder
{
    public ServiceBinder(AndroidBackgroundService service)
    {
        Service = service;
    }

    public AndroidBackgroundService Service { get; private set; }
}
