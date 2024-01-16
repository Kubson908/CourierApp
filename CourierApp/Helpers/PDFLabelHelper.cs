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

using CourierAPI.Models.Dto;
using PdfSharp.Drawing;
using PdfSharp.Drawing.BarCodes;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.IO.Compression;
using System.Text;

namespace CourierAPI.Helpers;

public static class PDFLabelHelper
{
    public static FileInfoDto GenerateLabels(List<LabelShipmentDto> shipments)
    {
        if (shipments.Count == 1)
        {
            PdfDocument document = GeneratePDF(shipments.First());
            byte[]? response = null;
            using (MemoryStream ms = new())
            {
                document.Save(ms);
                response = ms.ToArray();
            }
            string fileName = "Label_" + shipments.First().Id + ".pdf";
            return new FileInfoDto
            {
                Bytes = response,
                Type = "application/pdf",
                Name = fileName,
            };
        }
        string archiveName = "Labels_" + DateOnly.FromDateTime(DateTime.Now) + ".zip";
        byte[] result;
        using (MemoryStream zipArchiveMemoryStream = new())
        {
            using (ZipArchive zipArchive = new(zipArchiveMemoryStream, ZipArchiveMode.Create, true))
            {
                foreach (LabelShipmentDto shipment in shipments)
                {
                    PdfDocument document = GeneratePDF(shipment);
                    byte[]? file = null;
                    using (MemoryStream ms = new())
                    {
                        document.Save(ms);
                        file = ms.ToArray();
                    }
                    string fileName = "Label_" + shipment.Id + ".pdf";
                    ZipArchiveEntry entry = zipArchive.CreateEntry(fileName);
                    using Stream entryStream = entry.Open();
                    entryStream.Write(file, 0, file.Length);
                }
            }
            zipArchiveMemoryStream.Seek(0, SeekOrigin.Begin);
            result = zipArchiveMemoryStream.ToArray();
        }
        return new FileInfoDto
        {
            Bytes = result,
            Type = "application/zip",
            Name = archiveName
        };
    }

    public static PdfDocument GeneratePDF(LabelShipmentDto shipment)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        GlobalFontSettings.FontResolver ??= new FileFontResolver();
        PdfDocument document = new();
        document.Info.Title = "Etykieta przesyłki";
        XFont font = new("Verdana", 18, XFontStyleEx.Regular);

        var barcodeText = "PC" + shipment.Id + "-" + (int)shipment.Size + "-" + (int)shipment.Weight;
        PdfPage page = document.AddPage();
        page.Size = PdfSharp.PageSize.A5;

        Code3of9Standard bc39 = new(barcodeText, new XSize(200, 100))
        {
            TextLocation = TextLocation.None
        };

        XGraphics gfx = XGraphics.FromPdfPage(page);
        gfx.DrawBarCode(bc39, XBrushes.Black, font, new XPoint(page.Width/2 - bc39.Size.Width/2, page.Height - 150));

        XTextFormatter tf = new(gfx);

        XRect rect = new(10, 10, 400, 210);
        gfx.DrawRectangle(new XPen(XColor.FromArgb(000000)), XBrushes.Transparent, rect);
        rect.X = 15;
        string text = "Nadawca: " + shipment.CustomerEmail + "\n" + 
            "Tel: " + FormatPhoneNumber(shipment.CustomerPhone ?? "Brak") + "\n" +
            "Adres: " + shipment.CustomerAddress;
        tf.DrawString(text, font, XBrushes.Black, rect, XStringFormats.TopLeft);

        XRect rect2 = new(10, 220, 400, 210);
        gfx.DrawRectangle(new XPen(XColor.FromArgb(000000)), XBrushes.Transparent, rect2);
        rect2.X = 15;
        text = "Odbiorca: " + shipment.RecipientEmail + "\n" + 
            "Tel: " + FormatPhoneNumber(shipment.RecipientPhone ?? "Brak") + "\n" + 
            "Adres: " + shipment.RecipientAddress;
        tf.DrawString(text, font, XBrushes.Black, rect2, XStringFormats.TopLeft);

        XRect rect3 = new(10, 430, 400, 150);
        gfx.DrawRectangle(new XPen(XColor.FromArgb(000000)), XBrushes.Transparent, rect3);
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
                return new FontResolverInfo("StaticFiles/Verdana-Bold.ttf");
            }
            else
            {
                return new FontResolverInfo("StaticFiles/Verdana-Regular.ttf");
            }
        }
        return null;
    }
}
