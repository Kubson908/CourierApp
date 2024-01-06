using CourierMobileApp.Services;
using CourierMobileApp.View.Components;

namespace CourierMobileApp.View;

public partial class SchedulePage : ContentPage
{
    ScheduleViewModel viewModel;
    ProfileService profileService;
    private readonly MenuAnimation animation;
    bool isButtonPressed = false;
    public SchedulePage(ScheduleViewModel viewModel, ProfileService profileService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.profileService = profileService;
        animation = new()
        {
            layout = MainContent
        };
        navbar.MenuClicked += animation.OpenMenu;
        menu.ContainerClicked += (object sender, EventArgs e) => { animation.CloseMenu(sender, e); navbar.RotateIcon(); };
        navbar.Initialize(this.profileService);

    }

    protected override async void OnAppearing()
    {
        if (viewModel.shipmentService.route.Count < viewModel.Route.Count)
            viewModel.SetRoute();
        viewModel.IsBusy = false;
        base.OnAppearing();
        navbar.SetImage();
        if (viewModel.ListEmpty)
            await viewModel.GetRouteAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        animation.CloseMenu(null, EventArgs.Empty);
        if (navbar.menuOpened) navbar.RotateIcon();
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
            viewModel.ToggleRoute();
            progressBar.Progress = 0;
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