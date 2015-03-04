namespace Vurdalakov
{
    using System;
    using System.Runtime.InteropServices;

    public static class Kernel32
    {
        // CreateFile
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363858

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr securityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        public const UInt32 GENERIC_READ = 0x80000000;
        public const UInt32 GENERIC_WRITE = 0x40000000;
        public const UInt32 GENERIC_EXECUTE = 0x20000000;
        public const UInt32 GENERIC_ALL = 0x10000000;

        public const UInt32 FILE_SHARE_READ = 0x00000001;
        public const UInt32 FILE_SHARE_WRITE = 0x00000002;
        public const UInt32 FILE_SHARE_DELETE = 0x00000004;

        public const UInt32 CREATE_NEW = 1;
        public const UInt32 CREATE_ALWAYS = 2;
        public const UInt32 OPEN_EXISTING = 3;
        public const UInt32 OPEN_ALWAYS = 4;
        public const UInt32 TRUNCATE_EXISTING = 5;

        // SetFilePointerEx
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa365542

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetFilePointerEx(IntPtr hFile, Int64 liDistanceToMove, out Int64 lpNewFilePointer, UInt32 dwMoveMethod);

        public const UInt32 FILE_BEGIN = 0;
        public const UInt32 FILE_CURRENT = 1;
        public const UInt32 FILE_END = 2;

        // ReadFile
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa365467

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(IntPtr hFile, [Out] Byte[] lpBuffer, UInt32 nNumberOfBytesToRead, out UInt32 lpNumberOfBytesRead, IntPtr lpOverlapped);

        // WriteFile
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa365747

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(IntPtr hFile, [In] Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, out UInt32 lpNumberOfBytesWritten, IntPtr lpOverlapped);

        // CloseHandle
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms724211

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        // DeviceIoControl
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363216

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
//        public static extern bool DeviceIoControl(IntPtr hDevice, UInt32 dwIoControlCode, [In] IntPtr lpInBuffer, UInt32 nInBufferSize, [Out] IntPtr lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped);
        public static extern bool DeviceIoControl(IntPtr hDevice, UInt32 dwIoControlCode, [In] Byte[] lpInBuffer, UInt32 nInBufferSize, [Out] Byte[] lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped);

        public const UInt32 IOCTL_DISK_GET_DRIVE_GEOMETRY = 0x00070000;

        // DISK_GEOMETRY
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363972

        [StructLayout(LayoutKind.Sequential)]
        public struct DISK_GEOMETRY
        {
            public Int64 Cylinders;
            public UInt32 MediaType; // http://msdn.microsoft.com/en-us/library/windows/desktop/aa365231
            public UInt32 TracksPerCylinder;
            public UInt32 SectorsPerTrack;
            public UInt32 BytesPerSector;
        }
    }
}
