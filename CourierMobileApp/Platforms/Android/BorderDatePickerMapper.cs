using Android.Graphics.Drawables;
using CourierMobileApp.Controls;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace CourierMobileApp.Platforms;

public class BorderDatePickerMapper
{
    public static void Map(IElementHandler handler, IElement view)
    {
        if (view is BorderDatePicker)
        {
            var castedHandler = (DatePickerHandler)handler;
            var castedView = (BorderDatePicker)view;

            var temp = new GradientDrawable();
            try
            {
                temp.SetCornerRadius((int)handler.MauiContext?.Context.ToPixels(castedView.CornerRadius));

                temp.SetStroke((int)handler.MauiContext?.Context.ToPixels(castedView.BorderWidth), castedView.BorderColor.ToAndroid());

                if (castedView.BackgroundColor is not null)
                {
                    temp.SetColor(castedView.BackgroundColor.ToAndroid());
                }
                castedHandler.PlatformView?.SetBackground(temp);

                var paddingLeft = (int)handler.MauiContext?.Context.ToPixels(castedView.Padding.Left);
                var paddingTop = (int)handler.MauiContext?.Context.ToPixels(castedView.Padding.Top);
                var paddingRight = (int)handler.MauiContext?.Context.ToPixels(castedView.Padding.Right);
                var paddingBottom = (int)handler.MauiContext?.Context.ToPixels(castedView.Padding.Bottom);

                castedHandler.PlatformView?.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);

            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }
    }
}




