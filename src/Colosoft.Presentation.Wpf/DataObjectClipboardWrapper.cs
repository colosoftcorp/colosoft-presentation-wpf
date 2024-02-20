using System;

namespace Colosoft.Presentation
{
    internal class DataObjectClipboardWrapper : IClipboardDataObject
    {
        private readonly System.Windows.IDataObject dataObject;

        public DataObjectClipboardWrapper(System.Windows.IDataObject dataObject)
        {
            this.dataObject = dataObject;
        }

        public object GetData(string format) => this.dataObject.GetData(format);

        public object GetData(Type format) => this.dataObject.GetData(format);

        public object GetData(string format, bool autoConvert) => this.dataObject.GetData(format, autoConvert);

        public bool GetDataPresent(string format) => this.dataObject.GetDataPresent(format);

        public bool GetDataPresent(Type format) => this.dataObject.GetDataPresent(format);

        public bool GetDataPresent(string format, bool autoConvert) => this.dataObject.GetDataPresent(format);

        public string[] GetFormats() => this.dataObject.GetFormats();

        public string[] GetFormats(bool autoConvert) => this.dataObject.GetFormats(autoConvert);

        public void SetData(object data) => this.dataObject.SetData(data);

        public void SetData(string format, object data) => this.dataObject.SetData(format, data);

        public void SetData(Type format, object data) => this.dataObject.SetData(format, data);

        public void SetData(string format, object data, bool autoConvert) => this.dataObject.SetData(format, data, autoConvert);
    }
}
