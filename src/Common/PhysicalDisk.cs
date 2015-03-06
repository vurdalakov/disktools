namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class PhysicalDisk : DiskBase
    {
        public static UInt32[] GetDisks()
        {
            var devices = VolumeManagement.QueryDosDevice(null);

            var disks = new List<UInt32>();

            foreach (var device in devices)
            {
                const String prefix = "PhysicalDrive";

                if (device.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        var disk = UInt32.Parse(device.Substring(prefix.Length));
                        disks.Add(disk);
                    }
                    catch { }
                }
            }

            disks.Sort();

            return disks.ToArray();
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
