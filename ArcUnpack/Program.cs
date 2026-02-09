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
            if (args.Length != 1)
            {
                Console.WriteLine($"Usage: ArcUnpack.exe <pcarc_File_Path>");
                return;
            }

            var inFile = args[0];
            var outPath = Path.Combine(Path.GetDirectoryName(inFile)!, Path.GetFileNameWithoutExtension(inFile) + "Unpack");
            var arc = new PcArc();
            arc.Unpack(inFile, outPath);

            Console.WriteLine("Unpack Finish.");
            Console.ReadKey();
        }
    }

}