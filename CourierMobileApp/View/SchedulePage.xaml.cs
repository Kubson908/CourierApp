namespace CourierMobileApp.View;

public partial class SchedulePage : ContentPage
{
    ScheduleViewModel viewModel;
    bool isButtonPressed = false;
    public SchedulePage(ScheduleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        viewModel.IsBusy = false;
        base.OnAppearing();
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

    private async Task HoldAndRun()
    {
        double progress = 0;
        while (isButtonPressed && progress < 1)
        {
            await Task.Delay(16);
            progress += 0.016;
            progressBar.Progress = progress;
        }

        if (progress < 1)
        {
            progressBar.Progress = 0;
        }
        else
        {
            // 
        }

    }

    private async void btn_Pressed(object sender, EventArgs e)
    {
        isButtonPressed = true;
        await HoldAndRun();
    }

    private void btn_Released(object sender, EventArgs e)
    {
        isButtonPressed = false;
    }
}