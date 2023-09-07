using CourierMobileApp.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using CoreGraphics;
using UIKit;

namespace CourierMobileApp.Platforms
{
    public static class BorderEntryMapper
    {
        public static void Map(IElementHandler handler, IElement view)
        {
            if (view is BorderEntry)
            {
                var castedHandler = (EntryHandler)handler;
                var castedView = (BorderEntry)view;
                
                if (castedHandler.PlatformView != null)
                {
                    castedHandler.PlatformView.Layer.CornerRadius = castedView.CornerRadius;
                    castedHandler.PlatformView.Layer.BorderWidth = castedView.BorderWidth;
                    castedHandler.PlatformView.Layer.BorderColor = castedView.BorderColor.ToCGColor();
                    castedHandler.PlatformView.BorderStyle = UITextBorderStyle.Line;
                }

                castedHandler.PlatformView.LeftView = new UIView(new CGRect(0, 0, castedView.Padding.Left, 0));
                castedHandler.PlatformView.LeftViewMode = UITextFieldViewMode.Always;
                castedHandler.PlatformView.RightView = new UIView(new CGRect(0, 0, castedView.Padding.Right, 0));
                castedHandler.PlatformView.RightViewMode = UITextFieldViewMode.Always;
            }
        }
    }
}
