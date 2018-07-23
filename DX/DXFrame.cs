using System;
using System.Collections.Generic;

namespace PDPU
{
    class DXFrame : Frame
    {
        private bool windowed;
        private byte windowsNum;

        public DXFrame(Configure conf)
        {
            windowed = false;
            windowsNum = 2;
            configure = conf;
            frameHead = new DXFrameHead(conf);
            frameData = new DXFrameData(conf);
        }

        public override void ParseFrameHead(List<byte> list)
        {
            frameHead.Parse(list);
            windowed = frameHead.IsWindowed;
            if (windowed)
            {
                windowsNum = (byte)(((DXFrameHead)frameHead).windowsNum * 2);
            }
            else
            {
                windowsNum = 2;
            }
            switch (windowsNum)
            {
                case 2:
                    {
                        configure.Width = 1024 + 48;
                        configure.Height = 1024 + 3;
                        break;
                    }
                case 4:
                    {
                        configure.Width = 60;
                        configure.Height = 60;
                        break;
                    }
                case 8:
                    {
                        configure.Width = 15;
                        configure.Height = 15;
                        break;
                    }
                default:
                    {
                        configure.Width = 1024 + 48;
                        configure.Height = 1024 + 3;
                        break;
                    }
            }

        }

        public override void ParseFrameData(List<byte> list)
        {
            if (windowed)
            {
                ((DXFrameData)frameData).ParseDXWindows(list, windowsNum);
            }
            else
            {
                frameData.Parse(list);
            }

        }


        public int GetFrameHeadLength()
        {
            int frameLine = configure.FrameHeadLine;
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            int width = configure.Width;
            int height = configure.Height;
            int bits = configure.Bits;

            int frameHeadLength = 180;
            switch (windowsNum)
            {
                case 2:
                    {
                        int pixelLen = 2 * width * bits / 8;
                        frameHeadLength = lineHeadLen + lineEndLen + pixelLen;
                        break;
                    }
                case 4:
                case 8:
                    {
                        frameHeadLength = 180 + 2;
                        break;
                    }
                default:
                    {
                        int pixelLen = 2 * width * bits / 8;
                        frameHeadLength = lineHeadLen + lineEndLen + pixelLen;
                        break;
                    }
            }
            return frameHeadLength;
        }

        public int GetFrameLength()
        {
            int frameLine = configure.FrameHeadLine;
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            int width = configure.Width;
            int height = configure.Height;
            int bits = configure.Bits;

            int frameLength;
            switch (windowsNum)
            {
                case 2:
                    {
                        int pixelLen = 2 * width * bits / 8;
                        frameLength = (lineHeadLen + lineEndLen + pixelLen) * (height + frameLine);
                        break;
                    }
                case 4:
                case 8:
                    {
                        frameLength = (windowsNum * (width * height * bits / 8 + lineHeadLen + lineEndLen)) + GetFrameHeadLength();
                        break;
                    }
                default:
                    {
                        int pixelLen = 2 * width * bits / 8;
                        frameLength = (lineHeadLen + lineEndLen + pixelLen) * (height + frameLine);
                        break;
                    }
            }
            return frameLength;
        }



        public override List<MyImage> GetImageList()
        {
            return windowed ? GetWindowedImageList() : GetFullFrameImageList();
        }

        private List<MyImage> GetFullFrameImageList()
        {
            byte[][] image = new byte[2][]
            {
                new byte[configure.Width * configure.Bits / 8 * configure.Height],
                new byte[configure.Width * configure.Bits / 8 * configure.Height]
            };
            for (int i = 0; i < configure.Height; i++)
            {
                byte[] srcBytes = frameData.LineList[i].Data;
                Buffer.BlockCopy(srcBytes, 0, image[0], i * srcBytes.Length / 2, srcBytes.Length / 2);
                Buffer.BlockCopy(srcBytes, srcBytes.Length / 2, image[1], i * srcBytes.Length / 2, srcBytes.Length / 2);
            }

            List<MyImage> list = new List<MyImage>(2);
            MyImage[] myImage = new MyImage[2] { new MyImage(), new MyImage() };
            for (int i = 0; i < myImage.Length; i++)
            {
                myImage[i].Image = image[i];
                myImage[i].Height = configure.Height;
                myImage[i].Width = configure.Width;
                myImage[i].Bits = configure.Bits;
                myImage[i].Text = "导星" + (i + 1);
                myImage[i].ImageType = ImageType.Raw;
                list.Add(myImage[i]);
            }
            return list;
        }

        private List<MyImage> GetWindowedImageList()
        {
            int imageLength = frameData.LineList[0].Data.Length;
            byte[][] image = new byte[windowsNum][];
            for (int i = 0; i < windowsNum; i++)
            {
                image[i] = new byte[imageLength];
            }

            for (int i = 0; i < windowsNum; i++)
            {
                Array.Copy(frameData.LineList[i].Data, image[i], imageLength);
            }

            List<MyImage> list = new List<MyImage>(windowsNum);
            MyImage[] myImage = new MyImage[windowsNum];
            for (int i = 0; i < windowsNum; i++)
            {
                myImage[i] = new MyImage();
            }

            for (int i = 0; i < myImage.Length; i++)
            {
                myImage[i].Image = image[i];
                myImage[i].Height = configure.Height;
                myImage[i].Width = configure.Width;
                myImage[i].Bits = configure.Bits;
                myImage[i].Text = "导星" + (i + 1);
                myImage[i].ImageType = ImageType.Raw;
                list.Add(myImage[i]);
            }
            return list;
        }


    }
}
