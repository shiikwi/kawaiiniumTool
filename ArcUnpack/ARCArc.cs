using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ArcUnpack
{
    public struct ARCArcFileEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
        public byte[] FileNameBytes;
        public uint Size;
        public uint Offset;
    }
    public class ARCArc : PcArc
    {
        public ARCArc(Encoding enc) : base(enc)
        {
            Encode = enc;
        }

        public override void Unpack(string arcPath, string outPath)
        {
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            using (FileStream fs = new FileStream(arcPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                uint DataOffset = br.ReadUInt32();
                int FileCount = br.ReadInt32();
                var FilesList = new List<ARCArcFileEntry>();

                for (int i = 0; i < FileCount; i++)
                {
                    FilesList.Add(BytesToStruct<ARCArcFileEntry>(br));
                }

                foreach (var f in FilesList)
                {
                    var FileName = Utils.ReadCString(f.FileNameBytes, Encode);
                    br.BaseStream.Position = f.Offset;
                    var buffer = br.ReadBytes((int)f.Size);
                    var outFile = Path.Combine(outPath, FileName);
                    File.WriteAllBytes(outFile, buffer);
                    Console.WriteLine($"Extract {FileName}");
                }
            }
        }

    }
}
