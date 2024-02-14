namespace CourierMobileApp;

public static class Config
{
    public static readonly string ApiPath = "https://courier-app.azurewebsites.net"; //testowanie lokalnie w emulatorze: https://10.0.2.2:7119 | testowanie na telefonie: http://192.168.0.107:7119 | testowanie z użyciem api na serwerze azure: https://courier-app.azurewebsites.net | published local: http://10.0.2.2:5000
    public static readonly string MatchFieldName = "shipmentId"; // nazwa fragmentu LabelRegex do wyciągnięcia w LabelResolver
    public static readonly string LabelRegex = @"^PC(?<"+MatchFieldName+@">\d+)-(?<size>\d+)-(?<weight>\d+)$";
    public static readonly string SMSMessage = "Witam z tej strony kurier. Za chwilę podjadę pod wskazany adres.";
}
