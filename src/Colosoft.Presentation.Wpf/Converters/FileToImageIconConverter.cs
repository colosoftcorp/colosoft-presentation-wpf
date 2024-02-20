using System;

namespace Colosoft.Presentation.Converters
{
    public class FileToImageIconConverter : GenericConverter<FileToImageIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fileName = value as string;

            var icon = IconManager.Instance.ExtractAssociatedIcon(fileName);

            if (icon != null)
            {
                return IconsCache.ExtractAssociatedIcon(fileName);
            }
            else
            {
                return null;
            }
        }
    }
}
