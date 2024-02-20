using System;
using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
    // ---------------------------------------------------
    // .NET classes representing runtime callable wrappers
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(CLSIDGuid.FileOpenDialog)]
    internal class FileOpenDialogRCW
    {
    }
}
