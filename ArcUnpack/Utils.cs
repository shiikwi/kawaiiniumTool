using System;
using System.Collections.Generic;
using System.Text;

namespace ArcUnpack
{
    public static class Utils
    {
        public static string ReadFolderNameStr(BinaryReader br, Encoding enc)
        {
            var namebuffer = br.ReadBytes(0x40);
            int len = Array.IndexOf(namebuffer, (byte)0);
            return len > 0 ? enc.GetString(namebuffer, 0, len) : enc.GetString(namebuffer);
        }

        public static string ReadCString(byte[] data, Encoding enc)
        {
            int len = Array.IndexOf(data, (byte)0);
            return len > 0 ? enc.GetString(data, 0, len) : enc.GetString(data);
        }
    }
}
