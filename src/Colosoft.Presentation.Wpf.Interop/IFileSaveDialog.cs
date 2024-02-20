using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
#pragma warning disable
    [ComImport(),
    Guid(IIDGuid.IFileSaveDialog),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileSaveDialog : IFileDialog
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer
        // --------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
        PreserveSig]
        new int Show([In] IntPtr parent);

        // Defined on IFileDialog - repeated here due to requirements of COM interop layer
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetFileTypes([In] uint cFileTypes, [In] ref NativeMethods.COMDLG_FILTERSPEC rgFilterSpec);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetFileTypeIndex([In] uint iFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetFileTypeIndex(out uint piFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void Unadvise([In] uint dwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetOptions([In] NativeMethods.FOS fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetOptions(out NativeMethods.FOS pfos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, NativeMethods.FDAP fdap);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void Close([MarshalAs(UnmanagedType.Error)] int hr);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetClientGuid([In] ref Guid guid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

        // Defined by IFileSaveDialog interface
        // -----------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetSaveAsItem([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        // Not currently supported: IPropertyStore
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetProperties([In, MarshalAs(UnmanagedType.Interface)] IntPtr pStore);

        // Not currently supported: IPropertyDescriptionList
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetCollectedProperties([In, MarshalAs(UnmanagedType.Interface)] IntPtr pList, [In] int fAppendDefault);

        // Not currently supported: IPropertyStore
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetProperties([MarshalAs(UnmanagedType.Interface)] out IntPtr ppStore);

        // Not currently supported: IPropertyStore, IFileOperationProgressSink
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ApplyProperties([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In, MarshalAs(UnmanagedType.Interface)] IntPtr pStore, [In, ComAliasName("ShellObjects.wireHWND")] ref IntPtr hwnd, [In, MarshalAs(UnmanagedType.Interface)] IntPtr pSink);
    }
#pragma warning restore
}
