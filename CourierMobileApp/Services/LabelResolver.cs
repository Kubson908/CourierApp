using System.Text.RegularExpressions;

namespace CourierMobileApp.Services;

public static class LabelResolver
{
    public static int GetShipmentId(string label)
    {
        Regex regex = new Regex(Config.LabelRegex);
        Match match = regex.Match(label);

        int shipmentId = Convert.ToInt32(match.Groups[Config.MatchFieldName].Value);
        return shipmentId;
    }
}
