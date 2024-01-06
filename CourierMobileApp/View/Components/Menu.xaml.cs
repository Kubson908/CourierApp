namespace CourierMobileApp.View.Components;

public partial class Menu : ContentView
{
    readonly ScheduleViewModel scheduleViewModel;

    public Menu()
	{
		InitializeComponent();
        scheduleViewModel = MauiApplication.Current.Services.GetService<ScheduleViewModel>();
    }

    private async void GoToMainPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    private async void GoToSchelude(object sender, EventArgs e)
    {
        await Shell.Current.DisplayAlert("Test", Shell.Current.CurrentState.Location.ToString(), "Ok");
        await Shell.Current.GoToAsync(nameof(SchedulePage));
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
}