using Colosoft.Presentation.Interop;
using Colosoft.Threading;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Colosoft.Presentation
{
    public class WpfFolderBrowserDialog : IFolderBrowserDialog, IDisposable
    {
        protected const string IllegalPropertyChangeString = " cannot be changed while dialog is showing";

        private readonly Collection<string> fileNames;
        private readonly IDispatcher dispatcher;
        private string title;
        private NativeDialogShowState showState = NativeDialogShowState.PreShow;

        private Presentation.Interop.IFileDialog nativeDialog;
        private bool? canceled;
        private Window parentWindow;

        // Template method to allow derived dialog to create actual
        // specific COM coclass (e.g. FileOpenDialog or FileSaveDialog)
        private NativeFileOpenDialog openDialogCoClass;
        private bool checkPathExists;
        private bool checkFileExists;
        private bool checkValidNames;
        private bool checkReadOnly;
        private bool restoreDirectory;
        private bool showPlacesList = true;
        private bool addToMruList = true;
        private bool showHiddenItems;
        private bool dereferenceLinks;
        private string fileName;

        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;
                if (this.NativeDialogShowing)
                {
                    this.nativeDialog.SetTitle(value);
                }
            }
        }

        public object DataContext { get; set; }

        internal bool AddExtension { get; set; }

        internal bool CheckFileExists
        {
            get { return this.checkFileExists; }
            set
            {
                this.ThrowIfDialogShowing("CheckFileExists" + IllegalPropertyChangeString);
                this.checkFileExists = value;
            }
        }

        internal bool CheckPathExists
        {
            get { return this.checkPathExists; }
            set
            {
                this.ThrowIfDialogShowing("CheckPathExists" + IllegalPropertyChangeString);
                this.checkPathExists = value;
            }
        }

        internal bool CheckValidNames
        {
            get { return this.checkValidNames; }
            set
            {
                this.ThrowIfDialogShowing("CheckValidNames" + IllegalPropertyChangeString);
                this.checkValidNames = value;
            }
        }

        internal bool CheckReadOnly
        {
            get { return this.checkReadOnly; }
            set
            {
                this.ThrowIfDialogShowing("CheckReadOnly" + IllegalPropertyChangeString);
                this.checkReadOnly = value;
            }
        }

        internal bool RestoreDirectory
        {
            get { return this.restoreDirectory; }
            set
            {
                this.ThrowIfDialogShowing("RestoreDirectory" + IllegalPropertyChangeString);
                this.restoreDirectory = value;
            }
        }

        public bool ShowPlacesList
        {
            get { return this.showPlacesList; }
            set
            {
                this.ThrowIfDialogShowing("ShowPlacesList" + IllegalPropertyChangeString);
                this.showPlacesList = value;
            }
        }

        public bool AddToMruList
        {
            get { return this.addToMruList; }
            set
            {
                this.ThrowIfDialogShowing("AddToMruList" + IllegalPropertyChangeString);
                this.addToMruList = value;
            }
        }

        public bool ShowHiddenItems
        {
            get { return this.showHiddenItems; }
            set
            {
                this.ThrowIfDialogShowing("ShowHiddenItems" + IllegalPropertyChangeString);
                this.showHiddenItems = value;
            }
        }

        internal bool AllowPropertyEditing { get; set; }

        public bool DereferenceLinks
        {
            get { return this.dereferenceLinks; }
            set
            {
                this.ThrowIfDialogShowing("DereferenceLinks" + IllegalPropertyChangeString);
                this.dereferenceLinks = value;
            }
        }

        public string FileName
        {
            get
            {
                this.CheckFileNamesAvailable();
#pragma warning disable S4275 // Getters and setters should access the expected fields
                if (this.fileNames.Count > 1)
                {
                    throw new InvalidOperationException("Multiple files selected - the FileNames property should be used instead");
                }
#pragma warning restore S4275 // Getters and setters should access the expected fields

                this.fileName = this.fileNames[0];
                return this.fileNames[0];
            }
            set
            {
                this.fileName = value;
            }
        }

        public string InitialDirectory { get; set; }

        private bool NativeDialogShowing
        {
            get
            {
                return (this.nativeDialog != null)
                    && (this.showState == NativeDialogShowState.Showing ||
                    this.showState == NativeDialogShowState.Closing);
            }
        }

        public WpfFolderBrowserDialog(Threading.IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.fileNames = new Collection<string>();
        }

        public WpfFolderBrowserDialog(Threading.IDispatcher dispatcher, string title)
        {
            this.dispatcher = dispatcher;
            this.fileNames = new Collection<string>();
            this.title = title;
        }

        internal Interop.IFileDialog GetNativeFileDialog()
        {
            return (Interop.IFileDialog)this.openDialogCoClass;
        }

        internal void InitializeNativeFileDialog()
        {
            this.openDialogCoClass = new NativeFileOpenDialog();
        }

        internal void CleanUpNativeFileDialog()
        {
            if (this.openDialogCoClass != null)
            {
                Marshal.ReleaseComObject(this.openDialogCoClass);
            }
        }

        internal void PopulateWithFileNames(Collection<string> names)
        {
            IShellItemArray resultsArray;
            uint count;

            if (names != null)
            {
                this.openDialogCoClass.GetResults(out resultsArray);
                resultsArray.GetCount(out count);

                names.Clear();
                for (int i = 0; i < count; i++)
                {
                    names.Add(this.GetFileNameFromShellItem(this.GetShellItemAt(resultsArray, i)));
                }

                if (count > 0)
                {
                    this.FileName = names[0];
                }
            }
        }

        internal NativeMethods.FOS GetDerivedOptionFlags(NativeMethods.FOS flags)
        {
            flags |= NativeMethods.FOS.FOS_PICKFOLDERS;
            return flags;
        }

        public bool? ShowDialog(Window owner)
        {
            this.parentWindow = owner;
            return this.ShowDialog();
        }

        public bool? ShowDialog()
        {
            bool? result = null;

            try
            {
                this.InitializeNativeFileDialog();
                this.nativeDialog = this.GetNativeFileDialog();

                this.ProcessControls();
                this.ValidateCurrentDialogState();

                this.ApplyNativeSettings(this.nativeDialog);

                this.showState = NativeDialogShowState.Showing;
                int hresult = this.nativeDialog.Show(this.GetHandleFromWindow(this.parentWindow));
                this.showState = NativeDialogShowState.Closed;

                if (ErrorHelper.Matches(hresult, Win32ErrorCode.ERROR_CANCELLED))
                {
                    this.canceled = true;
                    this.fileNames.Clear();
                }
                else
                {
                    this.canceled = false;

                    this.PopulateWithFileNames(this.fileNames);
                }

                result = !this.canceled.Value;
            }
            finally
            {
                this.CleanUpNativeFileDialog();
                this.showState = NativeDialogShowState.Closed;
            }

            return result;
        }

        public async Task<bool?> ShowDialog(CancellationToken cancellationToken)
        {
#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods that take one
            return await this.dispatcher.Invoke(
                (_) => Task.FromResult(this.ShowDialog()),
                cancellationToken);
#pragma warning restore CA2016 // Forward the 'CancellationToken' parameter to methods that take one
        }

        private void ApplyNativeSettings(Presentation.Interop.IFileDialog dialog)
        {
            Debug.Assert(dialog != null, "No dialog instance to configure");

            if (this.parentWindow == null)
            {
                this.parentWindow = Helpers.GetDefaultOwnerWindow();
            }

            // Apply option bitflags
            dialog.SetOptions(this.CalculateNativeDialogOptionFlags());

            // Other property sets
            dialog.SetTitle(this.title);

            string directory = string.IsNullOrEmpty(this.fileName) ? this.InitialDirectory : System.IO.Path.GetDirectoryName(this.fileName);

            if (directory != null)
            {
                IShellItem folder;
                NativeMethods.SHCreateItemFromParsingName(directory, IntPtr.Zero, new Guid(IIDGuid.IShellItem), out folder);

                if (folder != null)
                {
                    dialog.SetFolder(folder);
                }
            }

            if (!string.IsNullOrEmpty(this.fileName))
            {
                string name = System.IO.Path.GetFileName(this.fileName);
                dialog.SetFileName(name);
            }
        }

        private NativeMethods.FOS CalculateNativeDialogOptionFlags()
        {
            // We start with only a few flags set by default, then go from there based
            // on the current state of the managed dialog's property values
            NativeMethods.FOS flags =
                NativeMethods.FOS.FOS_NOTESTFILECREATE
                | NativeMethods.FOS.FOS_FORCEFILESYSTEM;

            // Call to derived (concrete) dialog to set dialog-specific flags
            flags = this.GetDerivedOptionFlags(flags);

            // Apply other optional flags
            if (this.checkFileExists)
            {
                flags |= NativeMethods.FOS.FOS_FILEMUSTEXIST;
            }

            if (this.checkPathExists)
            {
                flags |= NativeMethods.FOS.FOS_PATHMUSTEXIST;
            }

            if (!this.checkValidNames)
            {
                flags |= NativeMethods.FOS.FOS_NOVALIDATE;
            }

            if (!this.checkReadOnly)
            {
                flags |= NativeMethods.FOS.FOS_NOREADONLYRETURN;
            }

            if (this.restoreDirectory)
            {
                flags |= NativeMethods.FOS.FOS_NOCHANGEDIR;
            }

            if (!this.showPlacesList)
            {
                flags |= NativeMethods.FOS.FOS_HIDEPINNEDPLACES;
            }

            if (!this.addToMruList)
            {
                flags |= NativeMethods.FOS.FOS_DONTADDTORECENT;
            }

            if (this.showHiddenItems)
            {
                flags |= NativeMethods.FOS.FOS_FORCESHOWHIDDEN;
            }

            if (!this.dereferenceLinks)
            {
                flags |= NativeMethods.FOS.FOS_NODEREFERENCELINKS;
            }

            return flags;
        }

        private void ValidateCurrentDialogState()
        {
            // TODO: Perform validation - both cross-property and pseudo-controls
        }

        private void ProcessControls()
        {
            // TODO: Sort controls if necesarry - COM API might not require it, however
        }

        protected void CheckFileNamesAvailable()
        {
            if (this.showState != NativeDialogShowState.Closed)
            {
                throw new InvalidOperationException("Filename not available - dialog has not closed yet");
            }

            if (this.canceled.GetValueOrDefault())
            {
                throw new InvalidOperationException("Filename not available - dialog was canceled");
            }
        }

        private IntPtr GetHandleFromWindow(Window window)
        {
            if (window == null)
            {
                return NativeMethods.NO_PARENT;
            }

            return new System.Windows.Interop.WindowInteropHelper(window).Handle;
        }

        internal NativeMethods.FOS GetCurrentOptionFlags(Presentation.Interop.IFileDialog dialog)
        {
            NativeMethods.FOS currentFlags;
            dialog.GetOptions(out currentFlags);
            return currentFlags;
        }

        internal string GetFileNameFromShellItem(IShellItem item)
        {
            string filename;
            item.GetDisplayName(NativeMethods.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out filename);
            return filename;
        }

        internal IShellItem GetShellItemAt(IShellItemArray array, int i)
        {
            IShellItem result;
            uint index = (uint)i;
            array.GetItemAt(index, out result);
            return result;
        }

        protected void ThrowIfDialogShowing(string message)
        {
            if (this.NativeDialogShowing)
            {
                throw new NotSupportedException(message);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnFileOk(System.ComponentModel.CancelEventArgs e)
        {
        }

        protected virtual void OnFolderChanged(EventArgs e)
        {
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
        }

        protected virtual void OnFileTypeChanged(EventArgs e)
        {
        }

        protected virtual void OnOpening(EventArgs e)
        {
        }
    }
}
