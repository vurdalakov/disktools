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

                var disk = new PhysicalDisk(diskNumber);
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

                var disk = new LogicalDisk(diskChar);
                disk.ReadDiskInformation();

                Console.WriteLine("BytesPerSector:\t\t{0}", disk.BytesPerSector);
            }
        }
    }
}
