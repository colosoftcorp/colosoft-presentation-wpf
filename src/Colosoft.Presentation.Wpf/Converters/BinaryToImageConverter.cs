using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Colosoft.Presentation.Converters
{
    public class BinaryToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[])
            {
                byte[] bytes = value as byte[];

                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.EndInit();

                    return image;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
