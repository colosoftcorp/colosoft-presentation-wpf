using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
#pragma warning disable
    [ComImport,
    Guid(IIDGuid.IKnownFolderManager),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKnownFolderManager
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FolderIdFromCsidl([In] int nCsidl, out Guid pfid);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FolderIdToCsidl([In] ref Guid rfid, out int pnCsidl);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderIds([Out] IntPtr ppKFId, [In, Out] ref uint pCount);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([In] ref Guid rfid, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderByName([In, MarshalAs(UnmanagedType.LPWStr)] string pszCanonicalName, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RegisterFolder([In] ref Guid rfid, [In] ref NativeMethods.KNOWNFOLDER_DEFINITION pKFD);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void UnregisterFolder([In] ref Guid rfid);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FindFolderFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath, [In] NativeMethods.FFFP_MODE mode, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FindFolderFromIDList([In] IntPtr pidl, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Redirect([In] ref Guid rfid, [In] IntPtr hwnd, [In] uint Flags, [In, MarshalAs(UnmanagedType.LPWStr)] string pszTargetPath, [In] uint cFolders, [In] ref Guid pExclusion, [MarshalAs(UnmanagedType.LPWStr)] out string ppszError);
    }
#pragma warning restore
}
