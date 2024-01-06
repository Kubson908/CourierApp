using Maui.Plugins.PageResolver;

namespace CourierMobileApp.View.Components;

public partial class Menu : ContentView
{
    public EventHandler ContainerClicked;
    public Menu()
	{
		InitializeComponent();
    }

    private void OnContainerClicked(object sender, TappedEventArgs e)
    {
        ContainerClicked?.Invoke(this, EventArgs.Empty);
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

    private void Quit(object sender, EventArgs e)
    {
        // TODO: Dodaæ sprawdzanie czy serwis w tle nie jest uruchomiony i odpowiedni komunikat obs³uguj¹cy wy³¹czanie serwisu
        Application.Current.Quit();
    }
}