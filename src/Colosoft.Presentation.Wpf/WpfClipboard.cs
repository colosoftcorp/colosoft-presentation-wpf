using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    public class WpfClipboard : IClipboard
    {
        public static System.Windows.TextDataFormat Convert(Text.TextDataFormat format)
        {
            switch (format)
            {
                case Text.TextDataFormat.CommaSeparatedValue:
                    return System.Windows.TextDataFormat.CommaSeparatedValue;
                case Text.TextDataFormat.Html:
                    return System.Windows.TextDataFormat.Html;
                case Text.TextDataFormat.Rtf:
                    return System.Windows.TextDataFormat.Rtf;
                case Text.TextDataFormat.Text:
                    return System.Windows.TextDataFormat.Text;
                case Text.TextDataFormat.UnicodeText:
                    return System.Windows.TextDataFormat.UnicodeText;
            }

            throw new NotSupportedException();
        }

        public Task<bool> ContainsData(string format, CancellationToken cancellationToken) =>
            Task.FromResult(System.Windows.Clipboard.ContainsData(format));

        public Task Flush(CancellationToken cancellationToken)
        {
            System.Windows.Clipboard.Flush();
            return Task.CompletedTask;
        }

        public Task<object> GetData(string format, CancellationToken cancellationToken)
        {
            return Task.FromResult(System.Windows.Clipboard.GetData(format));
        }

        public Task<IClipboardDataObject> GetDataObject(CancellationToken cancellationToken)
        {
            var dataObject = System.Windows.Clipboard.GetDataObject();
            if (dataObject != null)
            {
                return Task.FromResult<IClipboardDataObject>(new DataObjectClipboardWrapper(dataObject));
            }

            return Task.FromResult<IClipboardDataObject>(null);
        }

        public Task SetData(string format, object data, CancellationToken cancellationToken)
        {
            System.Windows.Clipboard.SetData(format, data);
            return Task.CompletedTask;
        }

        public Task SetDataObject(object data, CancellationToken cancellationToken)
        {
            if (data is IClipboardDataObject clipboardDataObject)
            {
                data = new ClipboardDataObjectWrapper(clipboardDataObject);
            }

            System.Windows.Clipboard.SetDataObject(data);

            return Task.CompletedTask;
        }

        public Task SetDataObject(object data, bool copy, CancellationToken cancellationToken)
        {
            if (data is IClipboardDataObject clipboardDataObject)
            {
                data = new ClipboardDataObjectWrapper(clipboardDataObject);
            }

            System.Windows.Clipboard.SetDataObject(data, copy);

            return Task.CompletedTask;
        }

        public Task SetImage(Media.Drawing.IImage image, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetText(string text, CancellationToken cancellationToken)
        {
            System.Windows.Clipboard.SetText(text);

            return Task.CompletedTask;
        }

        public Task SetText(string text, Text.TextDataFormat format, CancellationToken cancellationToken)
        {
            System.Windows.Clipboard.SetText(text, Convert(format));
            return Task.CompletedTask;
        }
    }
}
