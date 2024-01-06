using Android.Content;
using CourierMobileApp.Interfaces;
using CourierMobileApp.Services;

namespace CourierMobileApp.Platforms.Android;

public class ForegroundServiceHandler : IBackgroundService
{
    bool _isStarted = false;
    static Context context = Platform.CurrentActivity.ApplicationContext;
    public event EventHandler ServiceStopped;
    public event EventHandler ServiceStarted;
    public ServiceConnection conn;
    /*public LocationService locationService;*/
    public ConnectionService apiConnection;

    public ForegroundServiceHandler(/*LocationService locationService, */ConnectionService apiConnection)
    {
        /*this.locationService = locationService;*/
        this.apiConnection = apiConnection;
    }

    public void InvokeServiceStoppedEvent(AndroidBackgroundService sender)
    {
        ServiceStopped?.Invoke(sender, EventArgs.Empty);
        Stop();
    }
    public void InvokeServiceStartedEvent(AndroidBackgroundService sender)
    {
        ServiceStarted?.Invoke(sender, EventArgs.Empty);
    }
    public void Start()
    {
        if (context == null)
        {
            return;
        }

        Intent intent = new Intent(context, typeof(AndroidBackgroundService));
        conn = new ServiceConnection(this, intent);
        if (_isStarted)
        {
            return;
        }
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            _isStarted = context.BindService(intent, conn, Bind.AutoCreate);
            _ = context.StartForegroundService(intent);
        }
        else
        {
            _isStarted = context.BindService(intent, conn, Bind.AutoCreate);
            _ = context.StartService(intent);
        }
    }
    public async void Stop()
    {
        if (context == null || !_isStarted)
        {
            return;
        }
        Intent intent = new(context, typeof(AndroidBackgroundService));
        _ = context.StopService(intent);
        context.UnbindService(conn);
        _isStarted = false;
        await EndRoute();
    }

    public async Task CheckIn()
    {
        await apiConnection.SendAsync(HttpMethod.Get, "api/courier/check-in");
    }

    public async Task EndRoute()
    {
        try
        {
            await apiConnection.SendAsync(HttpMethod.Delete, "api/courier/end-route");
        }  
        catch (Exception) { }
    }
}
