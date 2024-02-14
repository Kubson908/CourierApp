namespace CourierMobileApp.View;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        await viewModel.VerifyOnAppearing();
        base.OnAppearing();
    }

    private async void ForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.DisplayAlert("Reset has�a", "Aby zresetowa� has�o skontaktuj si� z administratorem", "OK");
    }
}