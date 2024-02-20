using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
#pragma warning disable
    [ComImport,
    Guid(IIDGuid.IPropertyStore),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount([Out] out uint cProps);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAt([In] uint iProp, out NativeMethods.PROPERTYKEY pkey);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetValue([In] ref NativeMethods.PROPERTYKEY key, out object pv);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetValue([In] ref NativeMethods.PROPERTYKEY key, [In] ref object pv);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit();
    }
#pragma warning restore
}
