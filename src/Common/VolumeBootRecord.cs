namespace Vurdalakov
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BiosParameterBlock
    {
        // DOS 2.0
        public UInt16 BytesPerSector;
        public Byte SectorsPerCluster;
        public UInt16 ReservedSectors;
        public Byte NumberOfFats;
        public UInt16 RootDirectoryEntries;
        public UInt16 TotalSectors16;
        public Byte MediaDescriptor;
        public UInt16 SectorsPerFat;
        // DOS 3.31
        public UInt16 SectorsPerTrack;
        public UInt16 NumberOfHeads;
        public UInt32 HiddenSectors;
        public UInt32 TotalSectors32;
        // NTFS
        public Byte PhysicalDriveNumber;
        public Byte Flags;
        public Byte ExtendedBootSignature;
        public Byte Reserved;
        public UInt64 TotalSectors64;
        public UInt64 MftFirstClusterNumber;
        public UInt64 MftMirrorFirstClusterNumber;
        public UInt32 MftRecordSize;
        public UInt32 IndexBlockSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] VolumeSerialNumber;
        public UInt32 Checksum;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VolumeBootRecord
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] JumpInstruction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] OemId;
        public BiosParameterBlock BiosParameterBlock;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 426)]
        public Byte[] BootstrapCode;
        public UInt16 EndOfSectorMarker;

        public String GetOemId()
        {
            return Encoding.ASCII.GetString(OemId).TrimEnd();
        }

        public String GetVolumeSerialNumber()
        {
            return String.Format("{0:X4}-{1:X4}", BitConverter.ToInt16(BiosParameterBlock.VolumeSerialNumber, 2), BitConverter.ToInt16(BiosParameterBlock.VolumeSerialNumber, 0));
        }
    }
}
