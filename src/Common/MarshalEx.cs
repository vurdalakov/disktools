namespace Vurdalakov
{
    using System;
    using System.Runtime.InteropServices;

    public static class MarshalEx
    {
        public static Byte[] AllocateBytesForStruct<T>() where T : struct
        {
            return new byte[Marshal.SizeOf(typeof(T))];
        }

        public static Byte[] StructToBytes<T>(T structure) where T : struct
        {
            var bytes = AllocateBytesForStruct<T>();

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                var pinned = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(structure, pinned, false);

                return bytes;
            }
            finally
            {
                handle.Free();
            }
        }

        public static T BytesToStruct<T>(ref Byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            
            try
            {
                var pinned = handle.AddrOfPinnedObject();
                return (T)Marshal.PtrToStructure(pinned, typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
