using System;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(CLSIDGuid.KnownFolderManager)]
    internal class KnownFolderManagerRCW
    {
    }
}
