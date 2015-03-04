namespace Vurdalakov
{
    using System;

    public class LogicalDisk : DiskBase
    {
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
