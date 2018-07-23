using PDPU;
using System;
using System.Collections.Generic;

namespace TC
{
    public class TCFrame:Frame
    {
        public TCFrame(Configure conf)
        {
            configure = conf;
            lineList = new List<LineData>(conf.Height);
            frameHead = new TCFrameHead(conf);
            frameData = new FrameData(conf);
            isParsed = false;
        }

        public void ParseImageSize(byte[]frameHeadInfor)
        {
            if (frameHeadInfor == null || frameHeadInfor.Length < 18)
            {
                return;
            }
            byte[] size = 
            {
                frameHeadInfor[16],
                frameHeadInfor[17]
            };
            int windowSize = MyConvert.GetUInt16FromHexBytes(size);
            if (size[0] == 0xcc && size[1] == 0xcc)//全帧
            {
                configure.Width = configure.FullWidth;
                configure.Height = configure.FullHeight;
            }
            else if (windowSize >= 400 && windowSize <= 1000)
            {
                configure.Width = windowSize + 100;//100为暗像元
                configure.Height = windowSize + 5;//4个暗行，1个帧头行
            }
            else
            {
                //log 错误信息
            }
        }

        public override void ParseFrame()
        {
            int headIndex = IndexOfFrameHead();
            if (headIndex == -1)
            {
                lineList.Clear();
            }
            else if (headIndex > 0)
            {
                lineList.RemoveRange(0, headIndex);
            }
            if (lineList.Count >= configure.Height && !isParsed)
            {
                ParseFrameHead(lineList[0]);
                isParsed = true;
            }
        }

        public int IndexOfFrameHead()
        {
            if (lineList == null || lineList.Count == 0)
            {
                return -1;
            }
            int lineHeadLen = configure.LineHeadLen;
            int lineIdentifyLen = configure.LineIdentifyLen;
            int cameraIdentifyLen = configure.CameraIdentifyLen;
            //查找帧头,第一行行号为0
            int index = -1;
            for (int i = 0; i < lineList.Count; i++)
            {
                LineData lineData = lineList[i];
                byte[] lineNum = new byte[lineHeadLen - lineIdentifyLen - cameraIdentifyLen];//行号
                Array.Copy(lineData.Head, lineIdentifyLen + cameraIdentifyLen,
                            lineNum, 0, lineNum.Length);
                bool isFirstLine = true;
                for (int j = 0; j < lineNum.Length; j++)
                {
                    if (lineNum[j] > 0)
                    {
                        isFirstLine = false;
                    }
                }
                if (isFirstLine)//找到第一行 
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public override void ParseFrameHead(LineData lineData)
        {
            frameHead.Parse(lineData);
        }

        public override void ParseFrameHead(List<byte> list)
        {
            frameHead.Parse(list);
        }

        public override void ParseFrameData(List<byte> list)
        {
            frameData.Parse(list);
        }

        public override List<MyImage> GetImageList()
        {
            byte[][]image = new byte[2][];
            GetImageBytes(ref image[0], ref image[1]);
            List<MyImage> list = new List<MyImage>(2);
            MyImage[] myImage = new MyImage[2] { new MyImage(), new MyImage() };
            for (int i = 0; i < myImage.Length; i++)
            {
                myImage[i].Image = image[i];
                myImage[i].Height = configure.Height;
                myImage[i].Width = configure.Width;
                myImage[i].Bits = configure.Bits;
                myImage[i].Text = "探测" + (i + 1);
                myImage[i].ImageType = ImageType.Raw;
                list.Add(myImage[i]);
            }
            return list;
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

        public override MyImage GetImage()
        {
            //行头和crc占用的16位字数
            int len = (lineList[0].Head.Length  + lineList[0].End.Length) / 2;
            //图像包括行头和crc共12字节
            byte[] image = new byte[(configure.Width + len) * configure.Bits / 8 * configure.Height];
            for (int i = 0; i < configure.Height; i++)
            {
                byte[] headBytes = lineList[i].Head;
                byte[] dataBytes = lineList[i].Data;
                byte[] crcBytes = lineList[i].End;
                Buffer.BlockCopy(headBytes, 0, image,
                    i * (headBytes.Length + dataBytes.Length + crcBytes.Length), headBytes.Length);
                Buffer.BlockCopy(dataBytes, 0, image,
                    i * (headBytes.Length + dataBytes.Length + crcBytes.Length) + headBytes.Length, dataBytes.Length);
                Buffer.BlockCopy(crcBytes, 0, image,
                    i * (headBytes.Length + dataBytes.Length + crcBytes.Length) + headBytes.Length + dataBytes.Length, 
                    crcBytes.Length);
            }
            MyImage myImage = new MyImage();
            myImage.Image = image;
            myImage.Height = configure.Height;
            myImage.Width = configure.Width + len;
            myImage.Bits = configure.Bits;
            myImage.Text = "";
            myImage.ImageType = ImageType.Raw;
            return myImage;
        }

        public void Refresh()
        {
            //XXX lineList.RemoveRange(0, configure.Height);
            lineList.Clear();
            isParsed = false;
        }
    }
}
