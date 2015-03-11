﻿namespace Vurdalakov
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

        public PhysicalDisk(UInt32 diskNumber, Boolean readOnly) : base(String.Format("\\\\.\\PhysicalDrive{0}", diskNumber), readOnly)
        {
        }

        public override void ReadDiskInformation()
        {
            DiskGeometry = DeviceIoControl<Kernel32.DISK_GEOMETRY>(Kernel32.IOCTL_DISK_GET_DRIVE_GEOMETRY);

            BytesPerSector = DiskGeometry.BytesPerSector;
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
