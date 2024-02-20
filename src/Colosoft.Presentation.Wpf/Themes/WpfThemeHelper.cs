using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation.Themes
{
    public static class WpfThemeHelper
    {
        public static void SetDefaultFontFamily(System.Windows.Media.FontFamily font)
        {
            System.Windows.Documents.TextElement.FontFamilyProperty.OverrideMetadata(
                typeof(System.Windows.Documents.TextElement), new FrameworkPropertyMetadata(font));

            TextBlock.FontFamilyProperty.OverrideMetadata(
                typeof(TextBlock), new FrameworkPropertyMetadata(font));
        }

        public static void SetDefaultFontSize(double size)
        {
            System.Windows.Documents.TextElement.FontSizeProperty.OverrideMetadata(
                typeof(System.Windows.Documents.TextElement), new FrameworkPropertyMetadata(size));

            TextBlock.FontSizeProperty.OverrideMetadata(
                typeof(TextBlock), new FrameworkPropertyMetadata(size));
        }
    }
}
