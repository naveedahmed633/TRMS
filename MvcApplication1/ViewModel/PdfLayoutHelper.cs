using System;
using System.Web;
using iTextSharp.text.pdf;

namespace MvcApplication1.ViewModel
{
    public static class PdfLayoutHelper
    {
        public static int RunDirection
        {
            get
            {
                if (HttpContext.Current?.Session == null)
                    return PdfWriter.RUN_DIRECTION_LTR;

                var lang = GlobalVariables.GV_Langauge;
                return string.Equals(lang, "Ar", StringComparison.OrdinalIgnoreCase)
                    ? PdfWriter.RUN_DIRECTION_RTL
                    : PdfWriter.RUN_DIRECTION_LTR;
            }
        }
    }
}
