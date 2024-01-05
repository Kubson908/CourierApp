namespace CourierMobileApp.View;

public partial class ChangePasswordPage : ContentPage
{
	private ChangePasswordViewModel viewModel;
	public ChangePasswordPage(ChangePasswordViewModel changePasswordViewModel)
	{
		InitializeComponent();
		BindingContext = changePasswordViewModel;
		viewModel = changePasswordViewModel;
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