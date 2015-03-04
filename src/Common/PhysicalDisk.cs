namespace Vurdalakov
{
    using System;
    using System.Runtime.InteropServices;

    public class PhysicalDisk : DiskBase
    {
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
