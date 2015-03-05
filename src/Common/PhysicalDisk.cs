namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class PhysicalDisk : DiskBase
    {
        public static UInt32[] GetList()
        {
            var responseSize = 65536;

            while (true)
            {
                var response = Marshal.AllocHGlobal(responseSize);
                if (IntPtr.Zero == response)
                {
                    throw new OutOfMemoryException();
                }

                try
                {
                    var responseLength = Kernel32.QueryDosDevice(null, response, (UInt32)responseSize);
                    
                    if (0 == responseLength)
                    {
                        if (Kernel32.ERROR_INSUFFICIENT_BUFFER == Marshal.GetLastWin32Error())
                        {
                            responseSize *= 2;
                            continue;
                        }

                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }

                    var devices = Marshal.PtrToStringAuto(response, (Int32)responseLength).Split('\0');

                    var diskNumbers = new List<UInt32>();

                    foreach (var device in devices)
                    {
                        const String prefix = "PhysicalDrive";

                        if (device.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                diskNumbers.Add(UInt32.Parse(device.Substring(prefix.Length)));
                            }
                            catch { }
                        }
                    }

                    return diskNumbers.ToArray();
                }
                finally
                {
                    Marshal.FreeHGlobal(response);
                }
            }
        }

        public Kernel32.DISK_GEOMETRY DiskGeometry { get; private set; }

        public PhysicalDisk(UInt32 diskNumber)
        {
            Open(String.Format("\\\\.\\PhysicalDrive{0}", diskNumber), true);
        }

        public override void ReadDiskInformation()
        {
            DiskGeometry = new Kernel32.DISK_GEOMETRY();

            var buffer = DeviceIoControl(Kernel32.IOCTL_DISK_GET_DRIVE_GEOMETRY, Convert.ToUInt32(Marshal.SizeOf(DiskGeometry)));

            var pinnedSector = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            DiskGeometry = (Kernel32.DISK_GEOMETRY)Marshal.PtrToStructure(pinnedSector.AddrOfPinnedObject(), typeof(Kernel32.DISK_GEOMETRY));

            pinnedSector.Free();

            BytesPerSector = DiskGeometry.BytesPerSector;
        }
    }
}
