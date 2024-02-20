using System;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
    [ComImport]
    [Guid(IIDGuid.IKnownFolderManager)]
    [CoClass(typeof(KnownFolderManagerRCW))]
#pragma warning disable SA1302 // Interface names should begin with I
    internal interface KnownFolderManager : IKnownFolderManager
#pragma warning restore SA1302 // Interface names should begin with I
    {
    }
}
