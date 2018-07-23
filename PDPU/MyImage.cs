using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PDPU
{
    public enum ImageType
    {
        Raw = 1, Jpeg = 2
    }

    public class MyImage
    {
        private byte[] image;

        private int height;

        private int width;

        private int bits;

        private string text;

        private ImageType imageType ;

        public byte[] Image
        {
            set { image = value; }
            get { return image; }
        }

        public int Width
        {
            set { width = value; }
            get { return width; }
        }

        public int Height
        {
            set { height = value; }
            get { return height; }
        }

        public int Bits
        {
            set { bits = value; }
            get { return bits; }
        }

        public string Text
        {
            set { text = value; }
            get { return text; }
        }

        public ImageType ImageType
        {
            set { imageType = value; }
            get { return imageType; }
        }

        private byte[] Image16to8()
        {
            byte[] imageData = null;
            if (bits == 16)
            {
                imageData =  ImageMap16to8();
            }

            return PreprocessBmp(imageData); ;
        }

        private byte[] ImageMap16to8()
        {
            ushort min = BitConverter.ToUInt16(image, 2 * width + 10);
            ushort max = BitConverter.ToUInt16(image, 2 * width + 10);
            ushort pixel_16 = 0;

            //搜索像素最小值和最大值
            for (int j = 1; j < height; j++)
            {
                for (int i = 10; i < 2 * width - 2; i += 2)
                {
                    pixel_16 = BitConverter.ToUInt16(image, j * 2 * width + i);
                    if (min > pixel_16)
                    {
                        min = pixel_16;
                    }
                    else if (max < pixel_16)
                    {
                        max = pixel_16;
                    }
                }
            }

            //映射16位像素到8位像素.
            List<byte> bytesList = new List<byte>();
            byte pixel_8 = 0;
            for (int i = 0; i < image.Length; i += 2)
            {
                pixel_16 = BitConverter.ToUInt16(image, i);

                if (min == max || max == 0)
                    pixel_8 = 127;
                else
                    pixel_8 = (byte)((pixel_16 - min) * 255 / (max - min));

                bytesList.Add(pixel_8);
            }

            return bytesList.ToArray();
        }

        private byte[] PreprocessBmp(byte []originalImageData)
        {
            //由于位图数据需要DWORD对齐（4byte倍数），计算需要补位的个数
            int curPadNum = ((width * 8 + 31) / 32 * 4) - width;

            //最终生成的位图数据大小
            int bitmapDataSize = ((width * 8 + 31) / 32 * 4) * height;

            //最终生成的位图数据，以及大小，高度没有变，宽度需要调整
            byte[] destImageData = new byte[bitmapDataSize];
            int destWidth = width + curPadNum;

            //生成最终的位图数据，注意的是，位图数据 从左到右，从下到上，所以需要颠倒
            for (int originalRowIndex = 0; originalRowIndex < height; originalRowIndex++)
            {
                int destRowIndex = originalRowIndex;

                for (int dataIndex = 0; dataIndex < width; dataIndex++)
                {
                    //同时还要注意，新的位图数据的宽度已经变化destWidth，否则会产生错位
                    destImageData[destRowIndex * destWidth + dataIndex]
                        = originalImageData[originalRowIndex * width + dataIndex];
                }
            }
            return destImageData;
        }

        private Bitmap CreateBmpFromBytes(byte[] pix08, int width, int height)
        {
            try
            {
                if (pix08 == null || pix08.Length == 0)
                {
                    return null;
                }

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                SetGrayscalePalette(bitmap);
                Rectangle rect = new Rectangle(0, 0, width, height);
                BitmapData bd = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr ptr = bd.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(pix08, 0, ptr, pix08.Length);
                bitmap.UnlockBits(bd);

                return bitmap;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
            }
            return null;
        }

        private void SetGrayscalePalette(Bitmap srcImg)
        {
            if (srcImg == null || srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new ArgumentException();
            }
            ColorPalette cp = srcImg.Palette;
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            srcImg.Palette = cp;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bitmap = null;
            if (bits == 16)
            {
                bitmap = CreateBmpFromBytes(Image16to8(), width, height);
            }
            else if (bits == 8)
            {
                bitmap = CreateBmpFromBytes(image, width, height);
            }
            
            return bitmap;
        }

    }
}
