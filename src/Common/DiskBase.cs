namespace Vurdalakov
{
    using System;
    using System.Runtime.InteropServices;
    using System.IO; // IOException

    public class DiskBase : IDisposable
    {
        // Open / Close

        private IntPtr _handle = IntPtr.Zero;

        public void Open(String path, bool readOnly)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            UInt32 desiredAccess = Kernel32.GENERIC_READ | (readOnly ? 0 : Kernel32.GENERIC_WRITE);

            _handle = Kernel32.CreateFile(path, desiredAccess, Kernel32.FILE_SHARE_READ | Kernel32.FILE_SHARE_WRITE, IntPtr.Zero, Kernel32.OPEN_EXISTING, 0, IntPtr.Zero);

            if (IntPtr.Zero == _handle)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
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

        protected UInt32 _bytesPerSector = 512;

        private Int64 SectorToOffset(UInt64 sector)
        {
            return (Int64)sector * _bytesPerSector;
        }

        private void SetPointer(Int64 offset)
        {
            Int64 newOffset;
            if (!Kernel32.SetFilePointerEx(_handle, offset, out newOffset, Kernel32.FILE_BEGIN))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            if (newOffset != offset)
            {
                throw new IOException();
            }
        }

        virtual public Byte[] Read(Int64 offset, UInt32 size)
        {
            Byte[] buffer = new Byte[size];

            SetPointer(offset);

            UInt32 bytesRead;
            if (!Kernel32.ReadFile(_handle, buffer, size, out bytesRead, IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            if (bytesRead != size)
            {
                throw new IOException();
            }

            return buffer;
        }

        virtual public void Write(UInt64 offset, UInt32 size, Byte[] buffer)
        {
            throw new NotImplementedException();
        }

        // ReadSectors / WriteSectors

        public Byte[] ReadSectors(UInt64 firstSector, UInt32 numberOfSectors)
        {
            var size = numberOfSectors * _bytesPerSector;
            var buffer = new Byte[size];

            var offset = SectorToOffset(firstSector);

            return Read(offset, size);
        }

        public void WriteSectors(UInt64 firstSector, UInt32 numberOfSectors, Byte[] buffer)
        {
            throw new NotImplementedException();
        }

        // ReadDiskInformation

        public virtual void ReadDiskInformation()
        {
            _bytesPerSector = 512;
        }

    }
}
