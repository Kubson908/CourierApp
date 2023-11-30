using CourierMobileApp.Services;
using ZXing.Net.Maui;

namespace CourierMobileApp.View;

public partial class Scanner : ContentPage
{
    private ScannerViewModel viewModel;
    public Scanner(ScannerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.OneDimensional,
            AutoRotate = true,
            Multiple = false
        };
        /*AnimateScanner();*/
    }

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        BarcodeResult result = e.Results.First();
        PointF[] pos = result.PointsOfInterest;
        if (!IsInAllowedArea(pos)) return;
        cameraBarcodeReaderView.IsDetecting = false;
        var id = LabelResolver.GetShipmentId(e.Results.First().Value);
        viewModel.Label = id;
    }

    private bool IsInAllowedArea(PointF[] points)
    {
        double left = scanFrame.X;
        double top = cameraBarcodeReaderView.Height - scanFrame.Y;
        double right = left + scanFrame.Width;
        double bottom = top - scanFrame.Height;

        foreach (var point in points)
        {
            if (!(point.Y <= top && point.Y -31 >= bottom))
                return false;
        }
        return true;
    }

    private async void AnimateScanner()
    {
        while (true)
        {
            await scanLine.TranslateTo(0, 190, 1000);
            await scanLine.TranslateTo(0, 0, 1000);
        }
    }
}