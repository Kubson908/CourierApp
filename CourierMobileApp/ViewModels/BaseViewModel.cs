namespace CourierMobileApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;
/*    [ObservableProperty]
    bool loading;*/
    
    public bool IsNotBusy => !IsBusy;
}