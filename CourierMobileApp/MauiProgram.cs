using CommunityToolkit.Maui;
using CourierMobileApp.Controls;
using CourierMobileApp.Interfaces;
using CourierMobileApp.Platforms;
using CourierMobileApp.Platforms.Android;
using CourierMobileApp.Services;
using CourierMobileApp.View;
using Maui.Plugins.PageResolver;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace CourierMobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("IBMPlexSans-Regular.otf", "IBMPlexSans");
                });

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<ConnectionService>();

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginService>();
            builder.Services.AddSingleton<ShipmentService>();
            builder.Services.AddSingleton<SchedulePage>();
            builder.Services.AddSingleton<ScheduleViewModel>();

            builder.Services.AddTransient<ShipmentPage>();
            builder.Services.AddTransient<ShipmentViewModel>();

            builder.Services.AddSingleton<LocationService>();

#if ANDROID
            builder.Services.AddSingleton<IBackgroundService, ForegroundServiceHandler>();
#endif

            builder.Services.UsePageResolver();
            
#if DEBUG
		builder.Logging.AddDebug();
#endif

        Microsoft.Maui.Handlers.ElementHandler.ElementMapper.AppendToMapping("Classic", (handler, view) =>
        {
            if (view is BorderEntry)
            {
                BorderEntryMapper.Map(handler, view);
            }
            if (view is BorderDatePicker)
            {
                BorderDatePickerMapper.Map(handler, view);
            }
        });
            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnCreate((activity, bundle) => MakeStatusBarTranslucent(activity)));

                static void MakeStatusBarTranslucent(Android.App.Activity activity)
                {
                    activity.Window.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits, Android.Views.WindowManagerFlags.LayoutNoLimits);

                    activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);

                    activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                }
#endif
            });
        return builder.Build();
        }
    }
}