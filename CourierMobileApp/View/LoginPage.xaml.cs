using CommunityToolkit.Maui.Behaviors;

namespace CourierMobileApp.View;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel viewModel;
    private bool DarkTheme { set => Refresh(); }
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        DarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;
        Application.Current.RequestedThemeChanged += (sender, args) => { DarkTheme = Application.Current.RequestedTheme == AppTheme.Dark; };
    }

    private void Refresh()
    {
        Content = null;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await viewModel.VerifyOnAppearing();
        base.OnAppearing();
    }
}