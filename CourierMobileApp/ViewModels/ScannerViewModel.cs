namespace CourierMobileApp.ViewModels;

public partial class ScannerViewModel : BaseViewModel
{
    [ObservableProperty]
    int? label;

    public ScannerViewModel()
    {
        Label = null;
    }
}
