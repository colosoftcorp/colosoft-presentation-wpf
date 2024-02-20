using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
#pragma warning disable
    [ComImport,
    Guid(IIDGuid.IKnownFolder),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKnownFolder
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetId(out Guid pkfid);

        // Not yet supported - adding to fill slot in vtable
        void spacer1();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void GetCategory(out mbtagKF_CATEGORY pCategory);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetShellItem([In] uint dwFlags, ref Guid riid, out IShellItem ppv);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPath([In] uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] out string ppszPath);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetPath([In] uint dwFlags, [In, MarshalAs(UnmanagedType.LPWStr)] string pszPath);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetLocation([In] uint dwFlags, [Out, ComAliasName("ShellObjects.wirePIDL")] IntPtr ppidl);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderType(out Guid pftid);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRedirectionCapabilities(out uint pCapabilities);

        // Not yet supported - adding to fill slot in vtable
        void spacer2();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void GetFolderDefinition(out tagKNOWNFOLDER_DEFINITION pKFD);
    }
#pragma warning restore
}
