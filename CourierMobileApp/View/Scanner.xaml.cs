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

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        cameraBarcodeReaderView.IsDetecting = false;
        viewModel.Label = e.Results.First().Value;
    }
}