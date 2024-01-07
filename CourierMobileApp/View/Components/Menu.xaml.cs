namespace CourierMobileApp.View.Components;

public partial class Menu : ContentView
{
    readonly ScheduleViewModel scheduleViewModel;

    public Menu()
	{
		InitializeComponent();
        scheduleViewModel = MauiApplication.Current.Services.GetService<ScheduleViewModel>();
    }

    private async void Quit(object sender, EventArgs e)
    {
        if (scheduleViewModel.IsWorking)
        {
            bool answer = await Shell.Current.DisplayAlert("Trasa rozpoczêta", "Czy chcesz przerwaæ trasê?", "Tak", "Nie");
            if (answer)
            {
                scheduleViewModel.ToggleRoute();
            }
            else
                return;
        }
        else
        {
            bool answer = await Shell.Current.DisplayAlert("Wyjœcie", "Czy chcesz wyjœæ z aplikacji?", "Tak", "Nie");
            if (!answer) return;
        }
        Application.Current.Quit();
    }

    private async void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            await SecureStorage.Default.SetAsync("Theme", "Light");
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            await SecureStorage.Default.SetAsync("Theme", "Dark");
        }
    }
}