using System;
using System.Collections.Generic;
using System.Windows;

namespace Colosoft.Presentation.Themes
{
    internal sealed class AssemblyResourcesRepository : IResourcesRepository, IDisposable
    {
        private readonly Dictionary<Uri, ResourceDictionary> resourcesDicitories = new Dictionary<Uri, ResourceDictionary>();
        private readonly AssemblyResourcesManager resourcesManager;

        public string Name
        {
            get { return "AssemblyResourcesRepository"; }
        }

        public IEnumerable<string> Schemes => new string[] { "assemblyimage", "assemblyresdicionary" };

        public string ThemeName => null;

        internal AssemblyResourcesRepository(AssemblyResourcesManager manager)
        {
            this.resourcesManager = manager;
        }

        private System.Reflection.Assembly GetAssembly(Uri location)
        {
            var startIndex = location.OriginalString.IndexOf(location.Host, StringComparison.InvariantCultureIgnoreCase);
            return this.resourcesManager.Get(location.OriginalString.Substring(startIndex, location.Host.Length));
        }

        public object Get(Uri location)
        {
            if (location.Scheme == "resdicionary" ||
                location.Scheme == "assemblyresdicionary")
            {
                ResourceDictionary result = null;

                if (this.resourcesDicitories.TryGetValue(location, out result))
                {
                    return result;
                }

                System.Reflection.Assembly assembly = null;

                try
                {
                    assembly = this.GetAssembly(location);
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    throw new InvalidOperationException($"Assembly {location.Host} not found", ex);
                }

                var assemblyName = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));

                var sourceUri = System.IO.Packaging.PackUriHelper.Create(
                    new Uri("application://"),
                    new Uri($"/{assemblyName};component{location.LocalPath}", UriKind.Relative),
                    null);

                result = new ResourceDictionary()
                {
                    Source = sourceUri,
                };

                this.resourcesDicitories.Add(location, result);

                return result;
            }
            else if (location.Scheme == "image" ||
                     location.Scheme == "assemblyimage")
            {
                System.Reflection.Assembly assembly = null;

                try
                {
                    assembly = this.GetAssembly(location);
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    throw new InvalidOperationException($"Assembly {location.Host} not found", ex);
                }

                var assemblyName = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));

                var sourceUri = System.IO.Packaging.PackUriHelper.Create(
                    new Uri("application://"),
                    new Uri($"/{assemblyName};component{location.LocalPath}", UriKind.Relative),
                    null);

                System.Windows.Resources.StreamResourceInfo imageStream = null;

                try
                {
                    imageStream = Application.GetResourceStream(sourceUri);
                }
                catch (Exception)
                {
                    return null;
                }

                if (imageStream == null || imageStream.Stream == null)
                {
                    return null;
                }
                else
                {
                    var image = new System.Windows.Media.Imaging.BitmapImage();
                    image.BeginInit();
                    image.StreamSource = imageStream.Stream;
                    image.EndInit();
                    return image;
                }
            }

            return null;
        }

        public void Clear()
        {
            foreach (var i in this.resourcesDicitories)
            {
                GC.ReRegisterForFinalize(i.Value);
            }

            this.resourcesDicitories.Clear();
        }

        public void Dispose()
        {
            this.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
