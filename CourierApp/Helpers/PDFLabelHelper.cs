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

using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.Text;

namespace CourierAPI.Helpers;

public static class PDFLabelHelper
{
    public static PdfDocument GeneratePDF()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        GlobalFontSettings.FontResolver = new FileFontResolver();
        PdfDocument document = new();
        document.Info.Title = "Etykieta przesyłki";
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 20, XFontStyleEx.Bold);
        gfx.DrawString("Hello World", font, XBrushes.Black, new XRect(0, 0, page.Width, 0), XStringFormats.TopCenter);

        return document;
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
            /*if (isBold && isItalic)
            {
                return new FontResolverInfo("StaticResources/Fonts/Verdana-BoldItalic.ttf");
            }*/
            if (isBold)
            {
                return new FontResolverInfo("StaticResources/Fonts/Verdana-Bold.ttf");
            }
            /*else if (isItalic)
            {
                return new FontResolverInfo("StaticResources/Fonts/Verdana-Italic.ttf");
            }*/
            else
            {
                return new FontResolverInfo("StaticResources/Fonts/Verdana-Regular.ttf");
            }
        }
        return null;
    }
}
