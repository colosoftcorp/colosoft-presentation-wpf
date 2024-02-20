using System;

namespace Colosoft.Presentation.Converters
{
    public class JpegBufferToImageSourceConverter : GenericConverter<JpegBufferToImageSourceConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var buffer = value as byte[];
                if (buffer == null)
                {
                    return value;
                }

                var stream = new System.IO.MemoryStream(buffer, false);
                var decoder = new System.Windows.Media.Imaging.JpegBitmapDecoder(stream, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.Default);
                return decoder.Frames[0];
            }
            catch
            {
                return null;
            }
        }
    }
}
