namespace CourierMobileApp.Interfaces;

public interface IBackgroundService
{
    void Start();
    void Stop();
    event EventHandler ServiceStopped;
    event EventHandler ServiceStarted;
}
