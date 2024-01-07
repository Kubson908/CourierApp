namespace CourierMobileApp.View.Components;

public class MenuAnimation
{
	public Grid layout;
    readonly uint AnimationDuration = 400u;
    private bool menuOpened = false;

    public async void OpenMenu(object sender, EventArgs e)
    {
        if (!menuOpened)
        {
            await layout.TranslateTo(100, 0, AnimationDuration, Easing.Linear);
            menuOpened = true;
            GlobalVars.CanQuit = true;
        }
        else CloseMenu(sender, e);
    }

    public async void CloseMenu(object sender, EventArgs e)
    {
        await layout.TranslateTo(0, 0, AnimationDuration, Easing.Linear);
        menuOpened = false;
        GlobalVars.CanQuit = false;
    }
}