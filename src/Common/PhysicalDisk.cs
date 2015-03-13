namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class PhysicalDisk : DiskBase
    {
        public static String[] GetDeviceNames()
        {
            var devices = VolumeManagement.QueryDosDevice(null);

            var deviceNames = new List<String>();

            foreach (var device in devices)
            {
                const String prefix = "PhysicalDrive";

                if (device.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    deviceNames.Add(device);
                }
            }

            deviceNames.Sort();

            return deviceNames.ToArray();
        }

        public static String FormatDeviceName(UInt32 diskNumber)
        {
            return String.Format("PhysicalDrive{0}", diskNumber);
        }

        public Kernel32.DISK_GEOMETRY DiskGeometry { get; private set; }
        public Kernel32.DRIVE_LAYOUT_INFORMATION DriveLayoutInformation { get; private set; }
        public Kernel32.PARTITION_INFORMATION[] PartitionInformation { get; private set; }

        public PhysicalDisk()
        {
        }

        public PhysicalDisk(String deviceName, Boolean readOnly) : base(deviceName, readOnly)
        {
        }

        public PhysicalDisk(UInt32 diskNumber, Boolean readOnly) : base(FormatDeviceName(diskNumber), readOnly)
        {
        }

        public override void ReadDiskInformation()
        {
            // IOCTL_DISK_GET_DRIVE_GEOMETRY

            DiskGeometry = DeviceIoControl<Kernel32.DISK_GEOMETRY>(Kernel32.IOCTL_DISK_GET_DRIVE_GEOMETRY);

            BytesPerSector = DiskGeometry.BytesPerSector;

            // IOCTL_DISK_GET_DRIVE_LAYOUT

            var delta = Marshal.SizeOf(typeof(Kernel32.PARTITION_INFORMATION));

            var bytes = DeviceIoControl(Kernel32.IOCTL_DISK_GET_DRIVE_LAYOUT, Marshal.SizeOf(typeof(Kernel32.DRIVE_LAYOUT_INFORMATION)), delta * 4);
            DriveLayoutInformation = MarshalEx.BytesToStruct<Kernel32.DRIVE_LAYOUT_INFORMATION>(bytes);

            PartitionInformation = new Kernel32.PARTITION_INFORMATION[DriveLayoutInformation.PartitionCount];

            var offset = Marshal.SizeOf(typeof(Kernel32.DRIVE_LAYOUT_INFORMATION));

            for (var i = 0; i < DriveLayoutInformation.PartitionCount; i++)
            {
                PartitionInformation[i] = MarshalEx.BytesToStruct<Kernel32.PARTITION_INFORMATION>(bytes, offset, delta);

                offset += delta;
            }
        }

        public static String GetMediaTypeString(UInt32 mediaType)
        {
            // http://msdn.microsoft.com/en-us/library/windows/desktop/aa365231
            switch (mediaType)
            {
                case 0x00:
                    return "Unknown";
                case 0x0B:
                    return "RemovableMedia";
                case 0x0C:
                    return "FixedMedia";
                default:
                    return mediaType > 0x19 ? "" : "Floppy";
            }
        }
    }
}
