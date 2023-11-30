using CommunityToolkit.Maui.Behaviors;

namespace CourierMobileApp.View;

public partial class LoginPage : ContentPage
{
    private LoginViewModel viewModel;
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
}