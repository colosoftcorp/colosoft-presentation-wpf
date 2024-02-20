using System;
using System.Collections.Generic;

namespace Colosoft.Presentation
{
    public static class IconsCache
    {
        private static readonly Dictionary<string, System.Windows.Media.ImageSource> Icons =
            new Dictionary<string, System.Windows.Media.ImageSource>(StringComparer.InvariantCultureIgnoreCase);

        public static System.Windows.Media.ImageSource ExtractAssociatedIcon(string fileName)
        {
            var extension = string.Empty;

            if (!string.IsNullOrEmpty(fileName))
            {
                extension = System.IO.Path.GetExtension(fileName);
            }

            System.Windows.Media.ImageSource image = null;
            bool found = false;

            lock (Icons)
            {
                found = Icons.TryGetValue(extension, out image);
            }

            if (!found)
            {
                var icon = IconManager.Instance.ExtractAssociatedIcon(fileName);

                if (icon != null)
                {
                    var stream = new System.IO.MemoryStream();
                    icon.Save(stream);

                    stream.Position = 0;
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    image = bitmap;
                }

                lock (Icons)
                {
                    Icons.Add(extension, image);
                }
            }

            return image;
        }
    }
}
