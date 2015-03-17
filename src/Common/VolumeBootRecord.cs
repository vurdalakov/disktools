namespace Vurdalakov
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BiosParameterBlock
    {
        public UInt16 BytesPerSector;
        public Byte SectorsPerCluster;
        public UInt16 ReservedSectors;
        public Byte NumberOfFats;
        public UInt16 RootDirectoryEntries;
        public UInt16 TotalSectors16;
        public Byte MediaDescriptor;
        public UInt16 SectorsPerFat;
        public UInt16 SectorsPerTrack;
        public UInt16 NumberOfHeads;
        public UInt32 HiddenSectors;
        public UInt32 TotalSectors32;
        public Byte PhysicalDriveNumber;
        public Byte Flags;
        public Byte ExtendedBootSignature;
        public Byte Reserved;
        public UInt64 TotalSectors64;
        public UInt64 MftFirstClusterNumber;
        public UInt64 MftMirrorFirstClusterNumber;
        public UInt32 MftRecordSize;
        public UInt32 IndexBlockSize;
        public UInt64 VolumeSerialNumber;
        public UInt32 Checksum;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VolumeBootRecord
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] JumpInstruction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Char[] OemId;
        public BiosParameterBlock BiosParameterBlock;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 426)]
        public Byte[] BootstrapCode;
        public UInt16 EndOfSectorMarker;

        public String OemIdString
        {
            get
            {
                var stringBuilder = new StringBuilder(OemId.Length);
                stringBuilder.Append(OemId);
                return stringBuilder.ToString().Trim();
            }
        }
    }
}
