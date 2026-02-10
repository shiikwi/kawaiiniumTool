using System;
using System.Collections.Generic;
using System.Text;

namespace ArcUnpack
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var inFile = args[0];
            var ext = Path.GetExtension(inFile);
            var outPath = Path.Combine(Path.GetDirectoryName(inFile)!, Path.GetFileNameWithoutExtension(inFile) + "Unpack");
            if (ext == ".pcarc")
            {
                var pc = new PcArc(Encoding.GetEncoding("shift_jis"));
                pc.Unpack(inFile, outPath);
            }
            else if (ext == ".arc")
            {
                var arc = new ARCArc(Encoding.GetEncoding("shift_jis"));
                arc.Unpack(inFile, outPath);
            }

            Console.WriteLine("Unpack Finish.");
            Console.ReadKey();
        }
    }

}