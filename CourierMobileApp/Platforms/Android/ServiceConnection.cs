using Android.Content;
using Android.OS;

namespace CourierMobileApp.Platforms.Android;

public class ServiceConnection : Java.Lang.Object, IServiceConnection
{
    public bool IsConnected;
    public ServiceBinder Binder { get; private set; }
    public Intent intent;
    public ForegroundServiceHandler foregroundServiceHandler;
    public ServiceConnection(ForegroundServiceHandler foregroundServiceHandler, Intent intent)
    {
        IsConnected = false;
        Binder = null;
        this.foregroundServiceHandler = foregroundServiceHandler;
        this.intent = intent;
    }

    public void OnServiceConnected(ComponentName name, IBinder service)
    {
        Binder = service as ServiceBinder;
        IsConnected = Binder != null;
        Binder.Service.ForegroundServiceHandler = foregroundServiceHandler;
    }

    public void OnServiceDisconnected(ComponentName name)
    {
        IsConnected = false;
        Binder = null;
    }
}
