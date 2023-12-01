/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice (including the next paragraph) shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT 
OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using BarcodeStandard;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using PdfSharp.Drawing;
using PdfSharp.Drawing.BarCodes;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.Text;

namespace CourierAPI.Helpers;

public static class PDFLabelHelper
{
    public static PdfDocument GeneratePDF(List<LabelShipmentDto> shipments)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        if (GlobalFontSettings.FontResolver == null)
            GlobalFontSettings.FontResolver = new FileFontResolver();
        PdfDocument document = new();
        document.Info.Title = "Etykieta przesyłki";
        XFont font = new("Verdana", 18, XFontStyleEx.Regular);
        foreach (LabelShipmentDto shipment in shipments)
        {
            var barcodeText = "PC" + shipment.Id + "-" + (int)shipment.Size + "-" + shipment.Weight;
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A5;

            Code3of9Standard bc39 = new(barcodeText, new XSize(200, 100))
            {
                TextLocation = TextLocation.None
            };

            XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.DrawBarCode(bc39, XBrushes.Black, font, new XPoint(page.Width/2 - bc39.Size.Width/2, page.Height - 150));

            XTextFormatter tf = new(gfx);

            XRect rect = new(10, 10, 450, 220);
            gfx.DrawRectangle(XBrushes.Transparent, rect);
            string text = "Nadawca: " + shipment.CustomerEmail + "\n" + "Tel: " + FormatPhoneNumber(shipment.CustomerPhone ?? "Brak");
            tf.DrawString(text, font, XBrushes.Black, rect, XStringFormats.TopLeft);

            XRect rect2 = new(10, 240, 450, 220);
            gfx.DrawRectangle(XBrushes.Transparent, rect2);
            text = "Odbiorca: " + shipment.RecipientEmail + "\n" + "Tel: " + FormatPhoneNumber(shipment.RecipientPhone ?? "Brak");
            tf.DrawString(text, font, XBrushes.Black, rect2, XStringFormats.TopLeft);
        }

        return document;
    }

    private static string FormatPhoneNumber(string phone)
    {
        if (phone.Length == 9)
        {
            return phone.Insert(3, " ").Insert(7, " ");
        }
        return phone;
    }
}

public class FileFontResolver : IFontResolver
{
    public byte[]? GetFont(string faceName)
    {
        var ms = new MemoryStream();
        var fs = File.Open(faceName, FileMode.Open);
        fs.CopyTo(ms);
        ms.Position = 0;
        return ms.ToArray();
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Verdana", StringComparison.CurrentCultureIgnoreCase))
        {
            if (isBold)
            {
                return new FontResolverInfo("StaticResources/Fonts/Verdana-Bold.ttf");
            }
            else
            {
                return new FontResolverInfo("StaticResources/Fonts/Verdana-Regular.ttf");
            }
        }
        return null;
    }
}
