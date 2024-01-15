using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace CourierMobileApp.Platforms.Android;

[Service]
public class AndroidBackgroundService : Service
{
    public const int notificationId = 10001;
    Timer timer;
    Binder binder;
    public ForegroundServiceHandler ForegroundServiceHandler { get; set; }
    public override IBinder OnBind(Intent intent)
    {
        binder = new ServiceBinder(this);
        return binder;
    }

    private void OnServiceStopped()
    {
        ForegroundServiceHandler?.InvokeServiceStoppedEvent(this);
    }

    private void OnServiceStarted()
    {
        ForegroundServiceHandler?.InvokeServiceStartedEvent(this);
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            RegisterNotification();//Proceed to notify
        }
        OnServiceStarted();
        timer = new Timer(async (e) =>
        {
            try
            {
                await ForegroundServiceHandler.CheckIn();
            }
            catch (Exception)
            {
            }
        }, null, TimeSpan.FromSeconds(1.0f), TimeSpan.FromSeconds(5 * 60.0f));
        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        try
        {
            OnServiceStopped();
            if (timer is not null)
            {
                _ = timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }
        catch (Exception ex)
        {
            _ = Shell.Current.DisplayAlert("Coś nie działa", ex.Message, "OK");
        }
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            StopForeground(StopForegroundFlags.Remove);//Stop the service
            ServiceStoppedNotification();
        }

        base.OnDestroy();
    }

    public override bool StopService(Intent name)
    {
        try
        {
            OnServiceStopped();
            if (timer is not null)
            {
                _ = timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }
        catch (Exception ex)
        {
            _ = Shell.Current.DisplayAlert("Coś nie działa", ex.Message, "OK");
        }
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            StopForeground(StopForegroundFlags.Remove);//Stop the service

        }
        ServiceStoppedNotification();

        return base.StopService(name);
    }

    private void RegisterNotification()
    {
        CreateNotificationChannel("ServiceChannel", "ConnectionService", NotificationImportance.Max);

#pragma warning disable CA1416 // Validate platform compatibility
        Notification.Builder builder = new(this, "ServiceChannel");
#pragma warning restore CA1416 // Validate platform compatibility
        Notification notification = builder
           .SetContentTitle("Trasa rozpoczęta")
           .SetSmallIcon(Resource.Drawable.package_icon)
           .SetColor(144244122)
           .SetOngoing(true)
           .SetContentText("Status aktywności jest udostępniany")
           .Build();

        StartForeground(notificationId, notification);
    }

    public void ServiceStoppedNotification()
    {
        CreateNotificationChannel("ServiceStoppedChannel", "ConnectionServiceStopped", NotificationImportance.Max);
#pragma warning disable CA1416 // Validate platform compatibility
        Notification.Builder builder = new(this, "ServiceStoppedChannel");
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1422 // Validate platform compatibility
        Notification notification = builder
           .SetContentTitle("Zakończono trasę")
           .SetSmallIcon(Resource.Drawable.logout)
           .SetPriority(5)
           .SetColor(000000000)
           .SetVibrate(new long[] { 100, 200, 100, 200 })
           .Build();
#pragma warning restore CA1422 // Validate platform compatibility

        var notificationManager = GetSystemService(NotificationService) as NotificationManager;
        notificationManager.Notify(10002, notification);

        return;
    }

    private void CreateNotificationChannel(string channelId, string channelName, NotificationImportance importance)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

#pragma warning disable CA1416 // Validate platform compatibility
        var channel = new NotificationChannel(channelId, channelName, importance);
        NotificationManager notificationManager = GetSystemService(NotificationService) as NotificationManager;
        notificationManager.CreateNotificationChannel(channel);
#pragma warning restore CA1416 // Validate platform compatibility
        return;
    }
}
