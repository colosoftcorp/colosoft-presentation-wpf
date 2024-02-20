using System;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
    // ---------------------------------------------------------
    // Coclass interfaces - designed to "look like" the object
    // in the API, so that the 'new' operator can be used in a
    // straightforward way. Behind the scenes, the C# compiler
    // morphs all 'new CoClass()' calls to 'new CoClassWrapper()'
    [ComImport]
    [Guid(IIDGuid.IFileOpenDialog)]
    [CoClass(typeof(FileOpenDialogRCW))]
#pragma warning disable SA1302 // Interface names should begin with I
    internal interface NativeFileOpenDialog : IFileOpenDialog
#pragma warning restore SA1302 // Interface names should begin with I
    {
    }
}
