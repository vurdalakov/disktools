namespace Vurdalakov.DiskTools
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Physical disks");

            var diskNumbers = PhysicalDisk.GetDisks();

            foreach (var diskNumber in diskNumbers)
            {
                Console.WriteLine("\n--- Physical disk {0}\n", diskNumber);

                var disk = new PhysicalDisk(diskNumber, true);
                disk.ReadDiskInformation();

                Console.WriteLine("DISK_GEOMETRY:");
                Console.WriteLine("Cylinders:\t\t{0}", disk.DiskGeometry.Cylinders);
                Console.WriteLine("MediaType:\t\t{0}", disk.DiskGeometry.MediaType);
                Console.WriteLine("TracksPerCylinder:\t{0}", disk.DiskGeometry.TracksPerCylinder);
                Console.WriteLine("SectorsPerTrack:\t{0}", disk.DiskGeometry.SectorsPerTrack);
                Console.WriteLine("BytesPerSector:\t\t{0}", disk.DiskGeometry.BytesPerSector);

                var diskSize = disk.DiskGeometry.Cylinders * disk.DiskGeometry.TracksPerCylinder * disk.DiskGeometry.SectorsPerTrack * disk.DiskGeometry.BytesPerSector;
                Console.WriteLine("\nDisk size:\t\t{0:N0} bytes", diskSize);
            }

            Console.WriteLine("\n=== Logical disks");

            var diskChars = LogicalDisk.GetDisks();

            foreach (var diskChar in diskChars)
            {
                Console.WriteLine("\n--- Logical disk {0}:\n", diskChar);

                var disk = new LogicalDisk(diskChar, true); // TODO: using
                disk.ReadDiskInformation();

                Console.WriteLine("PARTITION_INFORMATION:");
                Console.WriteLine("StartingOffset:\t\t{0:N0}", disk.PartitionInformation.StartingOffset);
                Console.WriteLine("PartitionLength:\t{0:N0}", disk.PartitionInformation.PartitionLength);
                Console.WriteLine("HiddenSectors:\t\t{0:N0}", disk.PartitionInformation.HiddenSectors);
                Console.WriteLine("PartitionNumber:\t{0}", disk.PartitionInformation.PartitionNumber);
                Console.WriteLine("PartitionType:\t\t0x{0:X2} ({1})", disk.PartitionInformation.PartitionType, LogicalDisk.GetPartitionTypeString(disk.PartitionInformation.PartitionType));
                Console.WriteLine("BootIndicator:\t\t{0}", disk.PartitionInformation.BootIndicator);
                Console.WriteLine("RecognizedPartition:\t{0}", disk.PartitionInformation.RecognizedPartition);
                Console.WriteLine("RewritePartition:\t{0}", disk.PartitionInformation.RewritePartition);
            }
        }
    }
}
