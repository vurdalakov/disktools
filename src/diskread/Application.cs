namespace Vurdalakov.DiskTools
{
    using System;
    using System.IO;

    public class Application : DosToolsApplication
    {
        protected override Int32 Execute()
        {
            try
            {
                if ((_commandLineParser.FileNames.Length != 1) || !_commandLineParser.OptionHasValue("d") || !_commandLineParser.OptionHasValue("s"))
                {
                    Help();
                }

                var fileName = _commandLineParser.FileNames[0];
                var diskNumber = _commandLineParser.GetOptionString("d").ToUpper()[0];
                var firstSector = (UInt64)_commandLineParser.GetOptionInt("s", -1);
                var numberOfSectors = (UInt32)_commandLineParser.GetOptionInt("n", 1);

                if (firstSector < 0)
                {
                    Help();
                }

                DiskBase disk = null;

                if ((diskNumber >= '0') && (diskNumber <= '9'))
                {
                    disk = new PhysicalDisk((UInt32)diskNumber - 48);
                }
                else if ((diskNumber >= 'A') && (diskNumber <= 'Z'))
                {
                    disk = new LogicalDisk(diskNumber);
                }
                else
                {
                    Help();
                }

                disk.ReadDiskInformation();

                Byte[] buffer = new Byte[65536]; // 64KB at once

                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length))
                {
                    var fullSize = numberOfSectors * disk.BytesPerSector;
                    var numberOfFullChunks = fullSize / buffer.Length;
                    var offset = disk.SectorToOffset(firstSector);
                    
                    for (int i = 0; i < numberOfFullChunks; i++)
                    {
                        disk.Read(offset, (UInt32)buffer.Length, ref buffer, true);
                        fileStream.Write(buffer, 0, buffer.Length);

                        offset += buffer.Length;
                    }

                    var size = fullSize % buffer.Length; // remaining part
                    if (size > 0)
                    {
                        disk.Read(offset, (UInt32)size, ref buffer, true);
                        fileStream.Write(buffer, 0, (Int32)size);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Print("Error reading sectors: {0}", ex.Message);
                return 1;
            }
        }

        protected override void Help()
        {
            Console.WriteLine("DiskRead {0} | https://github.com/vurdalakov/disktools\n", ApplicationVersion);
            Console.WriteLine("Reads sectors from physical or logical disk to a file.\n");
            Console.WriteLine("Usage:\n\tdiskread <file name> -d:N -s:N -n:N [-silent]\n");
            Console.WriteLine("Options:\n\t-d:N - N can be 0-9 for physical disk or A-Z for logical disk");
            Console.WriteLine("\t-s:N - N is the number of the first sector to read");
            Console.WriteLine("\t-n:N - N is the number of sectors to read (default is 1)");
            Console.WriteLine("\t-silent - no error messsages are shown; check the exit code\n");
            Console.WriteLine("Examples:\n\treaddisk mbr.bin -d:0 -s:0");
            Console.WriteLine("\treaddisk sectors-65-74.bin -d:C -s:65 -n:10\n");
            Console.WriteLine("Exit codes:\n\t0 - read operation succeeded\n\t1 - read operation failed\n\t-1 - invalid command line syntax\n");
            Environment.Exit(-1);
        }
    }
}
