namespace CourierMobileApp.Services;

public class LocationService
{
    bool hasLocationPermission;

    ConnectionService connectionService;

    public LocationService(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    public async Task<Location> GetLocationAsync()
    {
        await CheckOrGetLocationPermission();
        try
        {
            Location result = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5)));
            return result;
        }
        catch (FeatureNotEnabledException)
        {
            return null;
        }
    }

    public async Task CheckOrGetLocationPermission()
    {
        if (!hasLocationPermission)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status != PermissionStatus.Granted)
            {
                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    throw new PermissionException("Brak uprawnień do pobrania lokalizacji. Przyznaj aplikacji uprawnienie do lokalizacji w ustawieniach.");
                }
                var request = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (request != PermissionStatus.Granted)
                {
                    throw new PermissionException("Brak uprawnień do pobrania lokalizacji. Przyznaj aplikacji uprawnienie do lokalizacji w ustawieniach.");
                }
                else
                {
                    hasLocationPermission = true;
                }
            }
            else
            {
                hasLocationPermission = true;
            }
        }
    }
}
