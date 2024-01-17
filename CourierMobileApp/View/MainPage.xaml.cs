using CourierMobileApp.View.Components;

namespace CourierMobileApp.View;

public partial class MainPage : ContentPage
{
    private readonly MenuAnimation animation;
    public MainPage(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        BindingContext = mainPageViewModel;
        animation = new()
        {
            layout = MainContent
        };
        navbar.MenuClicked += (object sender, EventArgs e) => { animation.OpenMenu(sender, e); navbar.RotateIcon(); };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        navbar.SetImage();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        animation.CloseMenu(null, EventArgs.Empty);
        if (navbar.menuOpened) navbar.RotateIcon();
    }

    protected override bool OnBackButtonPressed()
    {
        animation.OpenMenu(null, EventArgs.Empty);
        navbar.RotateIcon();
        return true;
    }
}