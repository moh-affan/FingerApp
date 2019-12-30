using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FingerApp
{

    public static class Extensions
    {
        public static Image ImageFromRawBgraArray(this byte[] arr, int width, int height)
        {
            var output = new Bitmap(width, height);
            Debug.WriteLine(width);
            Debug.WriteLine(height);
            var rect = new Rectangle(0, 0, width, height);
            var bmpData = output.LockBits(rect,
                ImageLockMode.ReadWrite, output.PixelFormat);
            Debug.WriteLine(bmpData.Width);
            var ptr = bmpData.Scan0;
            Marshal.Copy(arr, 0, ptr, arr.Length);
            output.UnlockBits(bmpData);
            var cropRect = new Rectangle(0, 0, 90, 90);
            var bmpCopy = new Bitmap(cropRect.Width, cropRect.Height);
            Graphics g = Graphics.FromImage(bmpCopy);
            g.DrawImage(output, new Rectangle(0, 0, bmpCopy.Width, bmpCopy.Height), cropRect, GraphicsUnit.Pixel);
            return bmpCopy;
        }

        public static Bitmap CreateBitmap(byte[] bytes, int width, int height)
        {
            byte[] rgbBytes = new byte[bytes.Length * 3];

            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                rgbBytes[(i * 3)] = bytes[i];
                rgbBytes[(i * 3) + 1] = bytes[i];
                rgbBytes[(i * 3) + 2] = bytes[i];
            }
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            for (int i = 0; i <= bmp.Height - 1; i++)
            {
                IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
                System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
            }

            bmp.UnlockBits(data);

            return bmp;
        }

        public static string Base64Encode(string input)
        {
            var pb = System.Text.Encoding.UTF8.GetBytes(input);
            return System.Convert.ToBase64String(pb);
        }

        public static string Base64Decode(string input)
        {
            var db = System.Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(db);
        }
    }
}
