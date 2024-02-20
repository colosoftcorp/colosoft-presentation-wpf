using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
    // wpffb used
    [ComImport]
    [Guid(IIDGuid.IModalWindow)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IModalWindow
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int Show([In] IntPtr parent);
    }
}
