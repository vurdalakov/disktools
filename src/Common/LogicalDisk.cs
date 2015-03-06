namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;

    public class LogicalDisk : DiskBase
    {
        public static Char[] GetDisks()
        {
            var devices = VolumeManagement.QueryDosDevice(null);

            var diskChars = new List<Char>();

            foreach (var device in devices)
            {
                if ((2 == device.Length) && (':' == device[1]))
                {
                    var diskChar = Char.ToUpper(device[0]);
                    if ((diskChar >= 'A') && (diskChar <= 'Z'))
                    {
                        diskChars.Add(diskChar);
                    }
                }
            }

            return diskChars.ToArray();
        }

        public LogicalDisk(char volume)
        {
            volume = Char.ToUpper(volume);

            if ((volume < 'A') || (volume > 'Z'))
            {
                throw new ArgumentException("volume");
            }

            Open(String.Format("\\\\.\\{0}:", volume), true);
        }
    }
}
