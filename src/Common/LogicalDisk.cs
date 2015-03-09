namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;

    public class LogicalDisk : DiskBase
    {
        public static Char[] GetDisks()
        {
            var devices = VolumeManagement.QueryDosDevice(null);

            var disks = new List<Char>();

            foreach (var device in devices)
            {
                if ((2 == device.Length) && (':' == device[1]))
                {
                    var disk = Char.ToUpper(device[0]);

                    if ((disk >= 'A') && (disk <= 'Z'))
                    {
                        disks.Add(disk);
                    }
                }
            }

            disks.Sort();

            return disks.ToArray();
        }

        public LogicalDisk(char volume, Boolean readOnly) : base(String.Format("\\\\.\\{0}:", volume), readOnly)
        {
        }
    }
}
