namespace CourierAPI.Helpers;

public static class AdminHelper
{
    public static string? AdminLogin;

    public static void Initialize(string login)
    {
        AdminLogin = login;
    } 
}
