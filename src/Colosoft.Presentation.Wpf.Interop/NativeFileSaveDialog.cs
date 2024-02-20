using System;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
    [ComImport]
    [Guid(IIDGuid.IFileSaveDialog)]
    [CoClass(typeof(FileSaveDialogRCW))]
#pragma warning disable SA1302 // Interface names should begin with I
    internal interface NativeFileSaveDialog : IFileSaveDialog
#pragma warning restore SA1302 // Interface names should begin with I
    {
    }
}
