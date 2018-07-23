using System.Collections.Generic;

namespace PDPU
{

    class DXFrameData : FrameData
    {
        public DXFrameData(Configure conf)
            : base(conf)
        {
        }

        public void ParseDXWindows(List<byte> list, int windowsNum)
        {
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            int width = configure.Width;
            int height = configure.Height;

            int pixelLen = height * width * configure.Bits / 8;
            int imageLength = lineHeadLen + lineEndLen + pixelLen;
            if (list.Count == imageLength * windowsNum)
            {
                for (int i = 0; i < windowsNum; i++)
                {
                    LineData imageData = new LineData();
                    List<byte> subList = list.GetRange(i * imageLength, imageLength);

                    imageData.Head = subList.GetRange(0, lineHeadLen).ToArray();
                    imageData.Data = subList.GetRange(lineHeadLen, pixelLen).ToArray();
                    if (configure.Bits == 16)
                    {
                        imageData.Data = BigLittleEndianConvert(imageData.Data);
                    }
                    imageData.End = subList.GetRange(lineHeadLen + pixelLen, lineEndLen).ToArray();

                    imageData.CrcResult = CRCITU.RunCRC16(subList.ToArray()) == 0 ? true : false;

                    lineList.Add(imageData);
                }
            }
            else
            {
                return;
            }
        }
    }
}
