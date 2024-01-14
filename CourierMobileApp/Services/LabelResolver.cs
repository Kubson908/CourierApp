using System.Text.RegularExpressions;

namespace CourierMobileApp.Services;

public static class LabelResolver
{
    public static int? GetShipmentId(string label)
    {
        Regex regex = new(Config.LabelRegex);
        Match match = regex.Match(label);
        if (match.Success)
        {
            int shipmentId = Convert.ToInt32(match.Groups[Config.MatchFieldName].Value);
            return shipmentId;
        }
        return null;
        
    }
}
