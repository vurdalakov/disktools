namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class LogicalDisk : DiskBase
    {
        public static String[] GetDeviceNames()
        {
            var devices = VolumeManagement.QueryDosDevice(null);

            var deviceNames = new List<String>();

            foreach (var device in devices)
            {
                if ((2 == device.Length) && (':' == device[1]))
                {
                    deviceNames.Add(device.ToUpper());
                }
            }

            deviceNames.Sort();

            return deviceNames.ToArray();
        }

        public Kernel32.PARTITION_INFORMATION PartitionInformation { get; private set; }

        public LogicalDisk(String deviceName, Boolean readOnly) : base(deviceName, readOnly)
        {
        }

        public LogicalDisk(char volume, Boolean readOnly) : base(String.Format("{0}:", volume), readOnly)
        {
        }

        public override void ReadDiskInformation()
        {
            // PARTITION_INFORMATION

            PartitionInformation = DeviceIoControl<Kernel32.PARTITION_INFORMATION>(Kernel32.IOCTL_DISK_GET_PARTITION_INFO);

            //BytesPerSector = PartitionInformation.BytesPerSector;
        }

        public static String GetPartitionTypeString(Byte partitionType)
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363990
            switch (partitionType)
            {
                case 0x00:
                    return "PARTITION_ENTRY_UNUSED";
                case 0x01:
                    return "PARTITION_FAT_12";
                case 0x04:
                    return "PARTITION_FAT_16";
                case 0x05:
                    return "PARTITION_EXTENDED";
                case 0x07:
                    return "PARTITION_IFS";
                case 0x0B:
                    return "PARTITION_FAT32";
                case 0x42:
                    return "PARTITION_LDM";
                case 0x80:
                    return "PARTITION_NTFT";
                case 0xC0:
                    return "VALID_NTFT";
                default:
                    return "";
            }
        }
    }
}
