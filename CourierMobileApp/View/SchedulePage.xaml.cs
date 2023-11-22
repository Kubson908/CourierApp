namespace CourierMobileApp.View;

public partial class SchedulePage : ContentPage
{
    ScheduleViewModel viewModel;
	public SchedulePage(ScheduleViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
        this.viewModel = viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await Shell.Current.Navigation.PopAsync();
        });
        return true;
    }

    private async void DateSelected(object sender, DateChangedEventArgs e)
    {
        await viewModel.GetRouteAsync();
    }
}