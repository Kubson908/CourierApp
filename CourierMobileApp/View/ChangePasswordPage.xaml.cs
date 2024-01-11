namespace CourierMobileApp.View;

public partial class ChangePasswordPage : ContentPage
{
	private ChangePasswordViewModel viewModel;
    private bool DarkTheme { set => Refresh(); }
    public ChangePasswordPage(ChangePasswordViewModel changePasswordViewModel)
	{
		InitializeComponent();
		BindingContext = changePasswordViewModel;
		viewModel = changePasswordViewModel;
        Application.Current.RequestedThemeChanged += (sender, args) => { DarkTheme = Application.Current.RequestedTheme == AppTheme.Dark; };
    }

    private void Refresh()
    {
        Content = null;
        InitializeComponent();
    }

    protected override void OnDisappearing()
    {
		viewModel.ErrorMessage = null;
		viewModel.OldPassword = string.Empty;
		viewModel.NewPassword = string.Empty;
		viewModel.ConfirmNewPassword = string.Empty;
		viewModel.Success = false;
        base.OnDisappearing();
    }
}