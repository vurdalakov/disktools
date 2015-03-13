namespace Vurdalakov.DiskTools
{
    using System;

    public class Application : DosToolsApplication
    {
        protected override Int32 Execute()
        {
            try
            {
                if ((_commandLineParser.FileNames.Length != 1) && !_commandLineParser.IsOptionSet("p") && !_commandLineParser.IsOptionSet("l"))
                {
                    Help();
                }

                if (_commandLineParser.IsOptionSet("p"))
                {
                    if (_commandLineParser.OptionHasValue("p"))
                    {
                        var deviceName = _commandLineParser.GetOptionString("p");

                        UInt32 diskNumber;
                        if (UInt32.TryParse(deviceName, out diskNumber))
                        {
                            deviceName = PhysicalDisk.FormatDeviceName(diskNumber);
                        }

                        using (var physicalDisk = new PhysicalDisk(deviceName, true))
                        {
                            PrintPhysicalDiskInformation(physicalDisk);
                        }
                    }
                    else
                    {
                        PrintArray(PhysicalDisk.GetDeviceNames());
                    }
                }
                else if (_commandLineParser.IsOptionSet("l"))
                {
                    if (_commandLineParser.OptionHasValue("l"))
                    {
                        var deviceName = _commandLineParser.GetOptionString("l");

                        if (1 == deviceName.Length)
                        {
                            var volume = Char.ToUpper(deviceName[0]);
                            if ((volume >= 'A') && (volume <= 'Z'))
                            {
                                deviceName = LogicalDisk.FormatDeviceName(volume);
                            }
                        }

                        using (var logicalDisk = new LogicalDisk(deviceName, true))
                        {
                            PrintLogicalDiskInformation(logicalDisk);
                        }
                    }
                    else
                    {
                        PrintArray(LogicalDisk.GetDeviceNames());
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Print("Error reading disk information: {0}", ex.Message);
                return 1;
            }
        }

        protected override void Help()
        {
            Console.WriteLine("DiskRead {0} | https://github.com/vurdalakov/disktools\n", ApplicationVersion);
            Console.WriteLine("Reads sectors from physical or logical disk to a file.\n");
            Console.WriteLine("Usage:\n\tdiskinfo <device name> | -l | -l:C | -p | -p:N [-silent]\n");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-l - prints list of logical disks");
            Console.WriteLine("\t-l:C - prints information about logical disk C (A, B, C, ...)");
            Console.WriteLine("\t-p - prints list of physical disks");
            Console.WriteLine("\t-p:N - prints information about physical disk N (0, 1, ...)");
            Environment.Exit(-1);
        }

        private void PrintArray(Object[] array)
        {
            foreach (var item in array)
            {
                Console.WriteLine(item);
            }
        }

        private void PrintPhysicalDiskInformation(PhysicalDisk physicalDisk)
        {
            Console.WriteLine("Physical disk {0}", physicalDisk.DeviceName);

            physicalDisk.ReadDiskInformation();

            Console.WriteLine("\nDISK_GEOMETRY:");
            Console.WriteLine("Cylinders:\t\t{0:N0}", physicalDisk.DiskGeometry.Cylinders);
            Console.WriteLine("MediaType:\t\t0x{0:X2} ({1})", physicalDisk.DiskGeometry.MediaType, PhysicalDisk.GetMediaTypeString(physicalDisk.DiskGeometry.MediaType));
            Console.WriteLine("TracksPerCylinder:\t{0:N0}", physicalDisk.DiskGeometry.TracksPerCylinder);
            Console.WriteLine("SectorsPerTrack:\t{0:N0}", physicalDisk.DiskGeometry.SectorsPerTrack);
            Console.WriteLine("BytesPerSector:\t\t{0:N0}", physicalDisk.DiskGeometry.BytesPerSector);

            var diskSize = physicalDisk.DiskGeometry.Cylinders * physicalDisk.DiskGeometry.TracksPerCylinder * physicalDisk.DiskGeometry.SectorsPerTrack * physicalDisk.DiskGeometry.BytesPerSector;
            Console.WriteLine("\nDisk size:\t\t{0:N0} bytes", diskSize);

            Console.WriteLine("\nDRIVE_LAYOUT_INFORMATION:");
            Console.WriteLine("PartitionCount:\t{0}", physicalDisk.DriveLayoutInformation.PartitionCount);
            Console.WriteLine("Signature:\t0x{0:X8}", physicalDisk.DriveLayoutInformation.Signature);

            for (var i = 0; i < physicalDisk.PartitionInformation.Length; i++)
            {
                PrintPartitionInformation(physicalDisk.PartitionInformation[i]);
            }
        }

        private void PrintLogicalDiskInformation(LogicalDisk logicalDisk)
        {
            Console.WriteLine("Logical disk {0}", logicalDisk.DeviceName);

            logicalDisk.ReadDiskInformation();

            PrintPartitionInformation(logicalDisk.PartitionInformation);
        }

        private void PrintPartitionInformation(Kernel32.PARTITION_INFORMATION partitionInformation)
        {
            Console.WriteLine("\nPARTITION_INFORMATION:");
            Console.WriteLine("StartingOffset:\t\t{0:N0}", partitionInformation.StartingOffset);
            Console.WriteLine("PartitionLength:\t{0:N0}", partitionInformation.PartitionLength);
            Console.WriteLine("HiddenSectors:\t\t{0:N0}", partitionInformation.HiddenSectors);
            Console.WriteLine("PartitionNumber:\t{0}", partitionInformation.PartitionNumber);
            Console.WriteLine("PartitionType:\t\t0x{0:X2} ({1})", partitionInformation.PartitionType, LogicalDisk.GetPartitionTypeString(partitionInformation.PartitionType));
            Console.WriteLine("BootIndicator:\t\t{0}", partitionInformation.BootIndicator);
            Console.WriteLine("RecognizedPartition:\t{0}", partitionInformation.RecognizedPartition);
            Console.WriteLine("RewritePartition:\t{0}", partitionInformation.RewritePartition);
        }
    }
}
