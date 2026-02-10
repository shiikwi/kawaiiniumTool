using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace ArcUnpack
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PcArcFileEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
        public byte[] FileNameBytes;
        public long Offset;
        public uint Size;
        public uint DecompressSize;
    }

    public class PcArcFolderBlock
    {
        public string FolderName;
        public int FileCount;
        public bool IsCompress;
        public List<PcArcFileEntry> Entries = new List<PcArcFileEntry>();
    }


    public class PcArc
    {
        protected Encoding Encode = Encoding.GetEncoding("UTF-8");

        public PcArc(Encoding enc)
        {
            this.Encode = enc;
        }

        public virtual void Unpack(string arcPath, string outPath)
        {
            using (FileStream fs = new FileStream(arcPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                uint Version = br.ReadUInt32();
                uint folderCount = br.ReadUInt32();
                ulong DataOffset = br.ReadUInt64();

                long[] BlockOffset = new long[folderCount];
                for (int i = 0; i < folderCount; i++)
                {
                    BlockOffset[i] = br.ReadInt64();
                }

                var FolderBlocks = new List<PcArcFolderBlock>();
                for (int i = 0; i < folderCount; i++)
                {
                    fs.Seek(BlockOffset[i], SeekOrigin.Begin);
                    var folder = new PcArcFolderBlock();
                    folder.FolderName = Utils.ReadFolderNameStr(br, Encode);
                    folder.FileCount = br.ReadInt32();
                    folder.IsCompress = br.ReadInt32() == 1;
                    br.ReadInt64(); //Skip padding

                    for (int j = 0; j < folder.FileCount; j++)
                    {
                        folder.Entries.Add(BytesToStruct<PcArcFileEntry>(br));
                    }
                    FolderBlocks.Add(folder);
                }

                foreach (var folder in FolderBlocks)
                {
                    var outDir = Path.Combine(outPath, folder.FolderName);
                    if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
                    foreach (var entry in folder.Entries)
                    {
                        br.BaseStream.Position = (long)DataOffset + entry.Offset;
                        var FileName = Utils.ReadCString(entry.FileNameBytes, Encode);
                        var buffer = br.ReadBytes((int)entry.Size);
                        if (folder.IsCompress)
                            buffer = DecompressGZip(buffer);
                        string outFile = Path.Combine(outDir, FileName.Replace(".gz", ""));
                        File.WriteAllBytes(outFile, buffer);
                        Console.WriteLine($"Extract {FileName}");
                    }
                }
            }

        }

        protected T BytesToStruct<T>(BinaryReader br) where T : struct
        {
            byte[] bytes = br.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        private byte[] DecompressGZip(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (GZipStream ds = new GZipStream(ms, CompressionMode.Decompress))
            using (MemoryStream oms = new MemoryStream())
            {
                ds.CopyTo(oms);
                return oms.ToArray();
            }
        }

    }
}
