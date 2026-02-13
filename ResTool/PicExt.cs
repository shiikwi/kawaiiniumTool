using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
#pragma warning disable

namespace ResTool
{
    public class PicExt
    {
        public void DecodeExt(string inFile, string outFile)
        {
            using (FileStream fs = new FileStream(inFile, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                string magic = Encoding.ASCII.GetString(br.ReadBytes(4));
                if (magic != "EXT0") throw new Exception("Not valid EXT0 file");

                br.BaseStream.Position = 0xC;
                int width = br.ReadInt32();
                int height = br.ReadInt32();
                int canvasW = br.ReadInt32();
                int canvasH = br.ReadInt32();
                int OffsetX = br.ReadInt32();
                int OffsetY = br.ReadInt32();

                br.BaseStream.Position = 0x24;
                byte bpp = br.ReadByte();

                if (bpp == 32)
                {
                    using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                    {
                        br.BaseStream.Position = 0x100;
                        var rect = new Rectangle(0, 0, width, height);
                        var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
                        byte[] pixelData = br.ReadBytes(width * height * 4);
                        Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length);
                        bmp.UnlockBits(bmpData);
                        bmp.Save(outFile, ImageFormat.Png);
                    }
                }
                else if (bpp == 8)
                {
                    using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed))
                    {
                        br.BaseStream.Position = 0x100;
                        ColorPalette pal = bmp.Palette;
                        for (int i = 0; i < 256; i++)
                        {
                            byte b = br.ReadByte();
                            byte g = br.ReadByte();
                            byte r = br.ReadByte();
                            byte a = br.ReadByte();
                            pal.Entries[i] = Color.FromArgb(a, r, g, b);
                        }
                        bmp.Palette = pal;

                        br.BaseStream.Position = 0x500;
                        var rect = new Rectangle(0, 0, width, height);
                        var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
                        byte[] pixel = br.ReadBytes(width * height);
                        IntPtr ptr = bmpData.Scan0;
                        for (int y = 0; y < height; y++)
                        {
                            Marshal.Copy(pixel, y * width, ptr, width);
                            ptr += bmpData.Stride;
                        }
                        bmp.UnlockBits(bmpData);
                        bmp.Save(outFile, ImageFormat.Png);
                    }
                }
                Console.WriteLine($"Convert {Path.GetFileName(inFile)}");
            }
        }
    }
}
