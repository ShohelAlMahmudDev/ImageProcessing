using System.Drawing;

namespace FastImageCombine.Helpers
{
    public static class ColorConversionHelper
    {
        public static string ColorToString(Color color)
        {
            return ColorTranslator.ToHtml(color);
        }

        public static Color StringToColor(string colorString)
        {
            return ColorTranslator.FromHtml(colorString);
        }
    }
}
