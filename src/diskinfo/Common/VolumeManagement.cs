namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class VolumeManagement
    {
        public static String[] QueryDosDevice(String deviceName)
        {
            var responseSize = 65536;

            while (true)
            {
                var response = Marshal.AllocHGlobal(responseSize);
                if (IntPtr.Zero == response)
                {
                    throw new OutOfMemoryException();
                }

                try
                {
                    var responseLength = Kernel32.QueryDosDevice(deviceName, response, (UInt32)responseSize);

                    if (0 == responseLength)
                    {
                        if (Kernel32.ERROR_INSUFFICIENT_BUFFER == Marshal.GetLastWin32Error())
                        {
                            responseSize *= 2;
                            continue;
                        }

                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }

                    var devices = Marshal.PtrToStringAuto(response, (Int32)responseLength).Split('\0');

                    var deviceNames = new List<String>();

                    foreach (var device in devices)
                    {
                        deviceNames.Add(device);
                    }

                    return deviceNames.ToArray();
                }
                finally
                {
                    Marshal.FreeHGlobal(response);
                }
            }
        }
    }
}
