using System.Collections.Generic;

namespace PDPU
{
    public class LineData
    {
        private byte[] head;
        private byte[] data;
        private byte[] end;
        private bool crcResult;

        public byte[] Head
        {
            set { head = value; }
            get { return head; }
        }

        public byte[] Data
        {
            set { data = value; }
            get { return data; }
        }

        public byte[] End
        {
            set { end = value; }
            get { return end; }
        }

        public bool CrcResult
        {
            set { crcResult = value; }
            get { return crcResult; }
        }

        public LineData()
        {
            head = null;
            data = null;
            end = null;
            crcResult = false;
        }

        public override string ToString()
        {
            return "Head = " + MyConvert.GetStrFromHexBytes(head) + ",  End = " + MyConvert.GetStrFromHexBytes(end)
                + ",  CRC result = " + crcResult;
        }
    }
    public class FrameData
    {
        protected List<LineData> lineList;

        protected Configure configure;

        public List<LineData> LineList
        {
            get { return lineList; }
        }

        public FrameData(Configure conf)
        {
            configure = conf;
            lineList = new List<LineData>(configure.Height);
        }

        public void SetConfigure(Configure conf)
        {
            configure = conf;
        }

        public void Parse(List<byte> list)
        {
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            int width = configure.Width;
            int height = configure.Height;

            int pixelLen = 2 * width * configure.Bits / 8;
            int lineWidth = lineHeadLen + lineEndLen + pixelLen;
            int frameLength = lineWidth * height;
            if (list.Count == frameLength)
            {
                for (int i = 0; i < height; i++)
                {
                    LineData lineData = new LineData();
                    //获取行数据，通过行数据解析出行头，数据和行尾，把解析出的数据结构
                    //存放在列表中
                    List<byte> subList = list.GetRange(i*lineWidth, lineWidth);
                    lineData.Head = subList.GetRange(0, lineHeadLen).ToArray();
                    lineData.Data = subList.GetRange(lineHeadLen, pixelLen).ToArray();
                    if (configure.Bits == 16)
                    {
                        lineData.Data = BigLittleEndianConvert(lineData.Data);
                    }
                    lineData.End = subList.GetRange(lineHeadLen + pixelLen, lineEndLen).ToArray();

                    lineData.CrcResult = CRCITU.RunCRC16(subList.ToArray()) == 0 ? true : false;

                    lineList.Add(lineData);
                }
            }
            else
            {
                return;
            }
        }

        protected byte[] BigLittleEndianConvert(byte[] data)
        {
            byte[] convertData = null;
            if (data != null && data.Length != 0 && data.Length % 2 == 0)
            {
                convertData = new byte[data.Length];
                for (int i = 0; i < data.Length; i += 2)
                {
                    convertData[i] = data[i + 1];
                    convertData[i + 1] = data[i];
                }
            }
            return convertData;
        }

        public void ClearList()
        {
            lineList.Clear();
        }

    }
}
