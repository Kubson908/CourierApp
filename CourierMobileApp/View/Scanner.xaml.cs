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
    }

    protected async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        try
        {
            BarcodeResult result = e.Results.First();
            PointF[] pos = result.PointsOfInterest;
            if (!IsInAllowedArea(pos)) return;
            cameraBarcodeReaderView.IsDetecting = false;
            await viewModel.AfterScanAction(e.Results.First().Value);
        } catch (Exception)
        {
            await Shell.Current.DisplayAlert("B³¹d odczytywania kodu", "Nie uda³o siê odczytaæ danych z etykiety", "OK");
        }
        
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
}