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
        if (!GlobalVars.CanQuit) return;
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

   /* public void Open()
    {
        if (quitButton.IsEnabled)
        {
            Close();
        }
        else
            quitButton.IsEnabled = true;
    }

    public void Close()
    {
        quitButton.IsEnabled = false;
    }*/
}