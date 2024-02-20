using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    public class WpfSaveDialog : ISaveFileDialog
    {
        private readonly Microsoft.Win32.SaveFileDialog saveFileDialog;

        public event System.ComponentModel.CancelEventHandler FileOk
        {
            add
            {
                this.saveFileDialog.FileOk += value;
            }

            remove
            {
                this.saveFileDialog.FileOk -= value;
            }
        }

        public bool AddExtension
        {
            get { return this.saveFileDialog.AddExtension; }
            set { this.saveFileDialog.AddExtension = value; }
        }

        public bool CheckFileExists
        {
            get { return this.saveFileDialog.CheckFileExists; }
            set { this.saveFileDialog.CheckFileExists = value; }
        }

        public bool CheckPathExists
        {
            get { return this.saveFileDialog.CheckPathExists; }
            set { this.saveFileDialog.CheckPathExists = value; }
        }

        public string DefaultExt
        {
            get { return this.saveFileDialog.DefaultExt; }
            set { this.saveFileDialog.DefaultExt = value; }
        }

        public bool DereferenceLinks
        {
            get { return this.saveFileDialog.DereferenceLinks; }
            set { this.saveFileDialog.DereferenceLinks = value; }
        }

        public string FileName
        {
            get { return this.saveFileDialog.FileName; }
            set { this.saveFileDialog.FileName = value; }
        }

#pragma warning disable CA1819 // Properties should not return arrays
        public string[] FileNames
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get { return this.saveFileDialog.FileNames; }
        }

        public string Filter
        {
            get { return this.saveFileDialog.Filter; }
            set { this.saveFileDialog.Filter = value; }
        }

        public int FilterIndex
        {
            get { return this.saveFileDialog.FilterIndex; }
            set { this.saveFileDialog.FilterIndex = value; }
        }

        public string InitialDirectory
        {
            get { return this.saveFileDialog.InitialDirectory; }
            set { this.saveFileDialog.InitialDirectory = value; }
        }

        public string Title
        {
            get { return this.saveFileDialog.Title; }
            set { this.saveFileDialog.Title = value; }
        }

        public bool CreatePrompt
        {
            get { return this.saveFileDialog.CreatePrompt; }
            set { this.saveFileDialog.CreatePrompt = value; }
        }

        public bool OverwritePrompt
        {
            get { return this.saveFileDialog.OverwritePrompt; }
            set { this.saveFileDialog.OverwritePrompt = value; }
        }

        public bool ValidateNames
        {
            get { return this.saveFileDialog.ValidateNames; }
            set { this.saveFileDialog.ValidateNames = value; }
        }

        public WpfSaveDialog()
        {
            this.saveFileDialog = new Microsoft.Win32.SaveFileDialog();
        }

        public void Reset()
        {
            this.saveFileDialog.Reset();
        }

        public Task<System.IO.Stream> OpenFile(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.saveFileDialog.OpenFile());
        }

        public Task<bool?> ShowDialog(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.saveFileDialog.ShowDialog());
        }
    }
}
