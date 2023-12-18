namespace CourierMobileApp.Services;

public class ProfileService
{
    public ImageSource imageSource = null;

    public ProfileService()
    {
    }

    public void SetImage()
    {
        var imageData = SecureStorage.Default.GetAsync("profile_image").Result;

        if (!string.IsNullOrEmpty(imageData))
        {
            var imageBytes = Convert.FromBase64String(imageData);
            imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
    }
}
