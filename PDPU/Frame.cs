using System;
using System.Collections.Generic;

namespace PDPU
{
    public class Frame
    {
        protected List<LineData> lineList;
        protected FrameHead frameHead;
        protected FrameData frameData;
        protected Configure configure;
        protected bool isParsed;

        public bool IsParsed
        {
            get
            {
                return isParsed;
            }
        }

        public Frame()
        {
        }

        public Frame(Configure conf)
        {
            isParsed = false;
        }

        public void SetFrameHead(FrameHead frameHead)
        {
            this.frameHead = frameHead;
        }

        public List<LineData> LineList
        {
            set { lineList = value; }
            get { return lineList; }
        }

        public void SetFrameHeadData(List<byte> list)
        {
            frameHead.SetData(list);
        }

        public void SetFrameData(FrameData frameData)
        {
            this.frameData = frameData;
        }
        public virtual void ParseFrame()
        {

        }

        public virtual void ParseFrameHead(List<byte> list)
        {
        }

        public virtual void ParseFrameHead(LineData lineData)
        {

        }

        public virtual void ParseFrameData(List<byte> list)
        {
            
        }

        public virtual void ParseFrameData(List<LineData> list)
        {

        }

        public virtual List<MyImage> GetImageList()
        {
            return null;
        }

        public virtual MyImage GetImage()
        {
            return null;
        }

        public void SetConfigure(Configure conf)
        {
            configure = conf;
            frameHead.SetConfigure(conf);
            frameData.SetConfigure(conf);
        }

        public byte[] GetImageBytes()
        {
            byte[] image = new byte[configure.Width * configure.Bits / 8 * configure.Height];
            for (int i = 0; i < configure.Height; i++)
            {
                byte[] srcBytes = frameData.LineList[i].Data;
                Buffer.BlockCopy(srcBytes, 0, image, i * srcBytes.Length, srcBytes.Length);
            }
            return image;
        }

        public void GetImageBytes(ref byte[] image1, ref byte[]image2)
        {
            image1 = new byte[configure.Width * configure.Bits / 8 * configure.Height];
            image2 = new byte[configure.Width * configure.Bits / 8 * configure.Height];
            for (int i = 0; i < configure.Height; i++)
            {
                byte[] srcBytes = frameData.LineList[i].Data;
                Buffer.BlockCopy(srcBytes, 0, image1, i * srcBytes.Length / 2, srcBytes.Length / 2);
                Buffer.BlockCopy(srcBytes, srcBytes.Length / 2, image2, i * srcBytes.Length / 2, srcBytes.Length / 2);
            }
        }

        public FrameHead GetFrameHead()
        {
            return frameHead;
        }

        public FrameData GetFrameData()
        {
            return frameData;
        }

        public List<LineData> GetLineList()
        {
            return lineList;
        }

        public bool CrcResult()
        {
            bool imageCrcResult = true;

            if (configure.Height > lineList.Count)
            {
                return false;
            }
            //crc检验，包括帧头和该帧每一行数据
            int invalid_count = 0;
            for (int i = 0; i < configure.Height; i++)
            {
                if (lineList[i].CrcResult == false)
                {
                    imageCrcResult = false;
                    ++invalid_count;
                }
            }
            Console.WriteLine("Invalild count: " + invalid_count);
            return imageCrcResult;
        }
    }
}
