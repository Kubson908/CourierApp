namespace CourierMobileApp.ViewModels;

public partial class ScannerViewModel : BaseViewModel
{
    [ObservableProperty]
    string label;

    public ScannerViewModel()
    {
        Label = "";
    }
}
