using System;
using System.Collections.Generic;
using System.Text;

namespace ResTool
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                var ext = Path.GetExtension(arg);

                if (ext == ".ext")
                {
                    var pic = new PicExt();
                    var outFile = Path.ChangeExtension(arg, ".png");
                    pic.DecodeExt(arg, outFile);
                }
                else if (ext == ".mul")
                {
                    var sc = new ScriptMul();
                    var outFile = Path.ChangeExtension(arg, ".txt");
                    sc.ParseMul(arg, outFile);
                }

            }
        }

    }

}