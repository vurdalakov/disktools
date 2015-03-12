namespace Vurdalakov.DiskTools
{
    using System;

    public class Application : DosToolsApplication
    {
        protected override Int32 Execute()
        {
            try
            {
                Console.WriteLine("=== Physical disks");

                var diskNumbers = PhysicalDisk.GetDisks();

                foreach (var diskNumber in diskNumbers)
                {
                    Console.WriteLine("\n--- Physical disk {0}", diskNumber);

                    using (var disk = new PhysicalDisk(diskNumber, true))
                    {
                        disk.ReadDiskInformation();

                        Console.WriteLine("\nDISK_GEOMETRY:");
                        Console.WriteLine("Cylinders:\t\t{0:N0}", disk.DiskGeometry.Cylinders);
                        Console.WriteLine("MediaType:\t\t0x{0:X2} ({1})", disk.DiskGeometry.MediaType, PhysicalDisk.GetMediaTypeString(disk.DiskGeometry.MediaType));
                        Console.WriteLine("TracksPerCylinder:\t{0:N0}", disk.DiskGeometry.TracksPerCylinder);
                        Console.WriteLine("SectorsPerTrack:\t{0:N0}", disk.DiskGeometry.SectorsPerTrack);
                        Console.WriteLine("BytesPerSector:\t\t{0:N0}", disk.DiskGeometry.BytesPerSector);

                        var diskSize = disk.DiskGeometry.Cylinders * disk.DiskGeometry.TracksPerCylinder * disk.DiskGeometry.SectorsPerTrack * disk.DiskGeometry.BytesPerSector;
                        Console.WriteLine("\nDisk size:\t\t{0:N0} bytes", diskSize);

                        Console.WriteLine("\nDRIVE_LAYOUT_INFORMATION:");
                        Console.WriteLine("PartitionCount:\t{0}", disk.DriveLayoutInformation.PartitionCount);
                        Console.WriteLine("Signature:\t0x{0:X8}", disk.DriveLayoutInformation.Signature);

                        for (var i = 0; i < disk.PartitionInformation.Length; i++)
                        {
                            PrintPartitionInformation(disk.PartitionInformation[i]);
                        }
                    }
                }

                Console.WriteLine("\n=== Logical disks");

                var diskChars = LogicalDisk.GetDisks();

                foreach (var diskChar in diskChars)
                {
                    Console.WriteLine("\n--- Logical disk {0}:", diskChar);

                    using (var disk = new LogicalDisk(diskChar, true))
                    {
                        disk.ReadDiskInformation();

                        PrintPartitionInformation(disk.PartitionInformation);
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
