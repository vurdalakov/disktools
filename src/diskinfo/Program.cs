namespace Vurdalakov.DiskTools
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var diskNumbers = PhysicalDisk.GetList();

            foreach (var diskNumber in diskNumbers)
            {
                Console.WriteLine("Physical disk {0}\n", diskNumber);

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
        }
    }
}
