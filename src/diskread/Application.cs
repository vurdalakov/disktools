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
                var fileName = _commandLineParser.GetOptionString("o", "output", null);
                var firstSector = (UInt64)_commandLineParser.GetOptionInt("s", -1);
                var numberOfSectors = (UInt32)_commandLineParser.GetOptionInt("n", 1);

                if (firstSector < 0)
                {
                    Help();
                }

                String deviceName = null;

                if (1 == _commandLineParser.FileNames.Length)
                {
                    deviceName = _commandLineParser.FileNames[0];
                }
                else if (_commandLineParser.OptionHasValue("p"))
                {
                    deviceName = _commandLineParser.GetOptionString("p");
                }
                else if (_commandLineParser.OptionHasValue("l"))
                {
                    deviceName = _commandLineParser.GetOptionString("l");
                }
                else
                {
                    return Help();
                }

                var disk = DiskBase.GetDisk(deviceName, true);

                disk.ReadDiskInformation();

                Byte[] buffer = new Byte[65536]; // 64KB at once

                var fullSize = numberOfSectors * disk.BytesPerSector;
                var numberOfFullChunks = fullSize / buffer.Length;
                var offset = disk.SectorToOffset(firstSector);

                using (Stream stream = GetStream(fileName, buffer.Length, offset))
                {
                    for (int i = 0; i < numberOfFullChunks; i++)
                    {
                        disk.Read(offset, (UInt32)buffer.Length, ref buffer, true);
                        stream.Write(buffer, 0, buffer.Length);

                        offset += buffer.Length;
                    }

                    var size = fullSize % buffer.Length; // remaining part
                    if (size > 0)
                    {
                        disk.Read(offset, (UInt32)size, ref buffer, true);
                        stream.Write(buffer, 0, (Int32)size);
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

        private Stream GetStream(String fileName, Int32 bufferSize, Int64 offset)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                return new BinaryFormatterStream(Console.OpenStandardOutput())
                {
                    BlockSize = 512,
                    InitialOffset = offset
                };
            }
            else
            {
                return new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);
            }
        }

        protected override Int32 Help()
        {
            Console.WriteLine("DiskRead {0} | https://github.com/vurdalakov/disktools", ApplicationVersion);
            Console.WriteLine("Dumps sectors from physical or logical disk to a file or screen.\n");
            Console.WriteLine("Usage:\n    diskread <device name> | -l:C | -p:N [-o:file_name] -s:N [-n:N] [-silent]");
            Console.WriteLine("Options:");
            Console.WriteLine("    -l:C - dumps sectors from logical disk C (A, B, C, ...)");
            Console.WriteLine("    -p:N - dumps sectors from physical disk N (0, 1, ...)");
            Console.WriteLine("    -o:f - file name to dump to (default is screen)");
            Console.WriteLine("    -s:N - N is the number of the first sector to read");
            Console.WriteLine("    -n:N - N is the number of sectors to read (default is 1)");
            Console.WriteLine("    -silent - no error messsages are shown; check the exit code");
            Console.WriteLine("Examples:");
            Console.WriteLine("    diskread -p:0 -s:0 -o:mbr.bin\t// dumps MBR to mbr.bin file");
            Console.WriteLine("    diskread -l:C -s:0\t\t\t// dumps VBR to screen");
            Console.WriteLine("    diskread -p:0 -s:65 -n:10 -o:sectors-65-74.bin");
            Console.WriteLine("    diskread \\\\.\\PhysicalDrive0 -s:65 -n:10 -o:sectors-65-74.bin");
            Console.WriteLine("Exit codes:\n    0 - read operation succeeded\n    1 - read operation failed\n   -1 - invalid command line syntax\n");
            
            return base.Help();
        }
    }
}
