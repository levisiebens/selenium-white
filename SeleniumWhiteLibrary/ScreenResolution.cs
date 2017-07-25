using System;
using System.Runtime.InteropServices;

namespace SeleniumWhiteLibrary
{
    //Used code from here for base code: http://www.codeproject.com/Articles/36664/Changing-Display-Settings-Programmatically
    public class ScreenResolution
    {
        private const int DispChangeBadmode = -2;
        private const int DispChangeRestart = 1;
        private const int DispChangeSuccessful = 0;
        private const int EnumCurrentSettings = -1;

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings([In, Out] ref DEVMODE lpDevMode, [param: MarshalAs(UnmanagedType.U4)] uint dwflags);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumDisplaySettings([param: MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, [param: MarshalAs(UnmanagedType.U4)] int iModeNum, [In, Out] ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            [MarshalAs(UnmanagedType.I4)]
            public int x;
            [MarshalAs(UnmanagedType.I4)]
            public int y;
        }

        [StructLayout(LayoutKind.Sequential,
    CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmFields;

            public POINTL dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningHeight;
        }

        public static DEVMODE GetCurrentSettings()
        {
            var mode = new DEVMODE();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);
            EnumDisplaySettings(null, EnumCurrentSettings, ref mode);
            return mode;
        }

        public static void ChangeDisplaySettings(int width, int height)
        {
            var mode = GetCurrentSettings();
            ChangeDisplaySettings(width, height, (int)mode.dmBitsPerPel);
        }

        public static void ChangeDisplaySettings(int width, int height, int bitCount)
        {
            var originalMode = new DEVMODE();
            originalMode.dmSize = (ushort)Marshal.SizeOf(originalMode);
            EnumDisplaySettings(null, EnumCurrentSettings, ref originalMode);
            var newMode = originalMode;

            //Sets the data
            newMode.dmPelsWidth = (uint)width;
            newMode.dmPelsHeight = (uint)height;
            newMode.dmBitsPerPel = (uint)bitCount;

            //Change resolution
            var result = ChangeDisplaySettings(ref newMode, 0);

            //Check the different result messages.
            if (result == DispChangeSuccessful)
            {
                Console.WriteLine("Resolution change successful.");
            }
            else if (result == DispChangeBadmode)
                throw new Exception("Mode not supported.");
            else if (result == DispChangeRestart)
                throw new Exception("Restart required.");
            else
                throw new Exception(String.Format("Failed. Error code = {0}", result));
        }
    }
}
