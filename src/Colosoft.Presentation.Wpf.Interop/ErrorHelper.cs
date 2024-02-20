using System.Runtime.InteropServices;

namespace Colosoft.Presentation.Interop
{
    internal static class ErrorHelper
    {
        private const int FACILITYWIN32 = 7;

        internal const int IGNORED = (int)HRESULT.S_OK;

        internal static int HResultFromWin32(int win32ErrorCode)
        {
            if (win32ErrorCode > 0)
            {
                win32ErrorCode =
                    (win32ErrorCode & 0x0000FFFF) | (FACILITYWIN32 << 16) | 0x8000000;
            }

            return win32ErrorCode;
        }

        internal static int HResultFromWin32(Win32ErrorCode error)
        {
            return HResultFromWin32((int)error);
        }

        internal static bool Matches(int hresult, Win32ErrorCode win32ErrorCode)
        {
            return hresult == HResultFromWin32(win32ErrorCode);
        }

        internal static bool Succeeded(int hresult) => hresult >= 0;
        internal static bool Failed(HRESULT hresult) => (int)hresult < 0;

        internal static COMException CreateException(int hresult)
        {
            return new COMException("Unknown COM exception", hresult);
        }
    }
}
