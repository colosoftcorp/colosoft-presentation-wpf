using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    public class WpfOpenDialog : IOpenFileDialog
    {
        private readonly Microsoft.Win32.OpenFileDialog openFileDialog;

        public event System.ComponentModel.CancelEventHandler FileOk
        {
            add
            {
                this.openFileDialog.FileOk += value;
            }

            remove
            {
                this.openFileDialog.FileOk -= value;
            }
        }

        public bool AddExtension
        {
            get => this.openFileDialog.AddExtension;
            set => this.openFileDialog.AddExtension = value;
        }

        public bool CheckFileExists
        {
            get => this.openFileDialog.CheckFileExists;
            set => this.openFileDialog.CheckFileExists = value;
        }

        public bool CheckPathExists
        {
            get => this.openFileDialog.CheckPathExists;
            set => this.openFileDialog.CheckPathExists = value;
        }

        public string DefaultExt
        {
            get => this.openFileDialog.DefaultExt;
            set => this.openFileDialog.DefaultExt = value;
        }

        public bool DereferenceLinks
        {
            get { return this.openFileDialog.DereferenceLinks; }
            set { this.openFileDialog.DereferenceLinks = value; }
        }

        public string FileName
        {
            get { return this.openFileDialog.FileName; }
            set { this.openFileDialog.FileName = value; }
        }

#pragma warning disable CA1819 // Properties should not return arrays
        public string[] FileNames
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get { return this.openFileDialog.FileNames; }
        }

        public string Filter
        {
            get { return this.openFileDialog.Filter; }
            set { this.openFileDialog.Filter = value; }
        }

        public int FilterIndex
        {
            get { return this.openFileDialog.FilterIndex; }
            set { this.openFileDialog.FilterIndex = value; }
        }

        public string InitialDirectory
        {
            get { return this.openFileDialog.InitialDirectory; }
            set { this.openFileDialog.InitialDirectory = value; }
        }

        public string Title
        {
            get { return this.openFileDialog.Title; }
            set { this.openFileDialog.Title = value; }
        }

        public bool Multiselect
        {
            get { return this.openFileDialog.Multiselect; }
            set { this.openFileDialog.Multiselect = value; }
        }

        public bool ValidateNames
        {
            get { return this.openFileDialog.ValidateNames; }
            set { this.openFileDialog.ValidateNames = value; }
        }

        public WpfOpenDialog()
        {
            this.openFileDialog = new Microsoft.Win32.OpenFileDialog();
        }

        public void Reset()
        {
            this.openFileDialog.Reset();
        }

        public Task<System.IO.Stream> OpenFile(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.openFileDialog.OpenFile());
        }

        public Task<System.IO.Stream[]> OpenFiles(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.openFileDialog.OpenFiles());
        }

        public IEnumerable<Func<CancellationToken, Task<System.IO.Stream>>> OpenFiles()
        {
            return this.openFileDialog.OpenFiles().Select(stream =>
                new Func<CancellationToken, Task<System.IO.Stream>>(_ => Task.FromResult(stream)));
        }

        public Task<bool?> ShowDialog(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.openFileDialog.ShowDialog());
        }
    }
}
