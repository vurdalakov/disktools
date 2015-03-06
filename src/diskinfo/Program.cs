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

                var physicalDisk = new PhysicalDisk(diskNumber);
                physicalDisk.ReadDiskInformation();

                Console.WriteLine("DISK_GEOMETRY:");
                Console.WriteLine("Cylinders:\t\t{0}", physicalDisk.DiskGeometry.Cylinders);
                Console.WriteLine("MediaType:\t\t{0}", physicalDisk.DiskGeometry.MediaType);
                Console.WriteLine("TracksPerCylinder:\t{0}", physicalDisk.DiskGeometry.TracksPerCylinder);
                Console.WriteLine("SectorsPerTrack:\t{0}", physicalDisk.DiskGeometry.SectorsPerTrack);
                Console.WriteLine("BytesPerSector:\t\t{0}", physicalDisk.DiskGeometry.BytesPerSector);

                var diskSize = physicalDisk.DiskGeometry.Cylinders * physicalDisk.DiskGeometry.TracksPerCylinder * physicalDisk.DiskGeometry.SectorsPerTrack * physicalDisk.DiskGeometry.BytesPerSector;
                Console.WriteLine("\nDisk size:\t\t{0:N0} bytes", diskSize);
            }

            Console.WriteLine("\n=== Logical disks");

            var diskChars = LogicalDisk.GetDisks();

            foreach (var diskChar in diskChars)
            {
                Console.WriteLine("\n--- Logical disk {0}:\n", diskChar);

                var logicalDisk = new LogicalDisk(diskChar);
                logicalDisk.ReadDiskInformation();

            }
        }
    }
}
