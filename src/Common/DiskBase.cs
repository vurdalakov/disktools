namespace Vurdalakov
{
    using System;
    using System.Runtime.InteropServices;
    using System.IO;

    public class DiskBase : IDisposable
    {
        public String DeviceName { get; private set; }

        // TODO: default constructor?

        public DiskBase(String deviceName, Boolean readOnly)
        {
            Open(deviceName, readOnly);
        }

        // Open / Close

        private IntPtr _handle = IntPtr.Zero;

        public void Open(String deviceName, bool readOnly)
        {
            if (String.IsNullOrEmpty(deviceName))
            {
                throw new ArgumentNullException("path");
            }

            Close();

            UInt32 desiredAccess = Kernel32.GENERIC_READ | (readOnly ? 0 : Kernel32.GENERIC_WRITE);

            _handle = Kernel32.CreateFile(deviceName, desiredAccess, Kernel32.FILE_SHARE_READ | Kernel32.FILE_SHARE_WRITE, IntPtr.Zero, Kernel32.OPEN_EXISTING, 0, IntPtr.Zero);

            if (Kernel32.INVALID_HANDLE_VALUE == _handle.ToInt32())
            {
                var error = Marshal.GetLastWin32Error();

                try
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                catch (Exception ex)
                {
                    if (Kernel32.ERROR_ACCESS_DENIED == error)
                    {
                        throw new InvalidOperationException("Run this application with local admin rights!", ex);
                    }

                    throw;
                }
            }

            DeviceName = deviceName;
        }

        public void Close()
        {
            if (_handle != IntPtr.Zero)
            {
                Kernel32.CloseHandle(_handle);
                _handle = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        // DeviceIoControl

        public T DeviceIoControl<T>(UInt32 ioControlCode) where T : struct
        {
            var outStruct = MarshalEx.AllocateBytesForStruct<T>();

            DeviceIoControl(ioControlCode, new Byte[0], 0, outStruct, Convert.ToUInt32(outStruct.Length));

            return MarshalEx.BytesToStruct<T>(ref outStruct);
        }

        public Byte[] DeviceIoControl(UInt32 ioControlCode, UInt32 size)
        {
            return DeviceIoControl(ioControlCode, new Byte[0], 0, size);
        }

        public Byte[] DeviceIoControl(UInt32 ioControlCode, Byte[] inBuffer, UInt32 inBufferSize, UInt32 size)
        {
            Byte[] buffer = new Byte[size];

            DeviceIoControl(ioControlCode, inBuffer, inBufferSize, buffer, size);

            return buffer;
        }

        public void DeviceIoControl(UInt32 ioControlCode, Byte[] inBuffer, UInt32 inBufferSize, Byte[] outBuffer, UInt32 outBufferSize)
        {
            UInt32 bytesReturned;

            if (!Kernel32.DeviceIoControl(_handle, ioControlCode, inBuffer, inBufferSize, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            if ((outBufferSize != 0) && (outBufferSize != bytesReturned))
            {
                throw new IOException();
            }
        }

        // SetPointer / Read / Write

        virtual protected Int64 SetPointer(Int64 offset, Boolean throwOnFail)
        {
            Int64 newOffset;
            if (!Kernel32.SetFilePointerEx(_handle, offset, out newOffset, Kernel32.FILE_BEGIN))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            if (throwOnFail && (newOffset != offset))
            {
                throw new IOException();
            }

            return newOffset;
        }

        virtual public UInt32 Read(Int64 offset, UInt32 size, ref Byte[] buffer, Boolean throwOnFail)
        {
            SetPointer(offset, true);

            UInt32 bytesRead;
            if (!Kernel32.ReadFile(_handle, buffer, size, out bytesRead, IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            if (throwOnFail && (bytesRead != size))
            {
                throw new IOException();
            }

            return bytesRead;
        }

        virtual public Byte[] Read(Int64 offset, UInt32 size)
        {
            Byte[] buffer = new Byte[size];

            Read(offset, size, ref buffer, true);

            return buffer;
        }

        virtual public void Write(UInt64 offset, UInt32 size, Byte[] buffer)
        {
            throw new NotImplementedException();
        }

        // ReadSectors / WriteSectors

        public Byte[] ReadSectors(UInt64 firstSector, UInt32 numberOfSectors)
        {
            var offset = SectorToOffset(firstSector);
            var size = numberOfSectors * BytesPerSector;

            return Read(offset, size);
        }

        public void WriteSectors(UInt64 firstSector, UInt32 numberOfSectors, Byte[] buffer)
        {
            throw new NotImplementedException();
        }

        // Disk information

        public UInt32 BytesPerSector { get; protected set; }

        public Int64 SectorToOffset(UInt64 sector)
        {
            if (BytesPerSector <= 0)
            {
                throw new InvalidOperationException("Call ReadDiskInformation() first.");
            }

            return (Int64)sector * BytesPerSector;
        }

        public virtual void ReadDiskInformation()
        {
            BytesPerSector = 512; // TODO: abstract
        }

    }
}
