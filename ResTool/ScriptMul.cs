using System;
using System.Collections.Generic;
using System.Text;

namespace ResTool
{
    public class ScriptMul
    {
        private struct MulHeader
        {
            public byte Multiplier;
            public byte[] Padding;
            public int EntryCount;
            public byte[] Unknown;
        }

        private struct IndexEntry
        {
            public int RVOffset;
            public int TextLen;
        }

        public void ParseMul(string inFile, string outFile)
        {
            using (FileStream fs = new FileStream(inFile, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                var header = new MulHeader();
                header.Multiplier = br.ReadByte();
                header.Padding = br.ReadBytes(3);
                header.EntryCount = br.ReadInt32();
                header.Unknown = br.ReadBytes(0x30 - (int)br.BaseStream.Position);

                var Entries = new List<IndexEntry>();
                for (int i = 0; i < header.EntryCount; i++)
                {
                    var entry = new IndexEntry();
                    entry.RVOffset = br.ReadInt32();
                    entry.TextLen = br.ReadInt32();
                    Entries.Add(entry);
                }

                var DataPos = br.BaseStream.Position;
                using(var wr = new StreamWriter(outFile, false))
                {
                    foreach (var entry in Entries)
                    {
                        var Off = DataPos + entry.RVOffset;
                        br.BaseStream.Position = Off;
                        string text = Encoding.UTF8.GetString(br.ReadBytes(entry.TextLen));
                        wr.WriteLine($"◇0x{Off:X}◇{text}");
                    }
                }
                Console.WriteLine($"Export {Path.GetFileName(inFile)}");
            }
        }


    }
}
