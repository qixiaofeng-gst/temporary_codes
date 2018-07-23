using PDPU;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TC
{
    public class TCCypress : Cypress
    {
        public TCCypress(CypressControl cypressControl)
            : base(cypressControl)
        {
        }

        public override unsafe void ParseThread()
        {
            int frameLine = configure.FrameHeadLine;
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            int width = configure.Width;
            int height = configure.Height;
            int bits = configure.Bits;
            string headString = configure.FrameHead;

            int pixelLen = width * bits / 8;
            int frameHeadLength = lineHeadLen + lineEndLen + pixelLen;
            int frameLength = (lineHeadLen + lineEndLen + pixelLen) * (height + frameLine) * 2;

            List<byte> cypressBytesList = new List<byte>(frameLength * 2);
            Frame[] frameArray = new Frame[2];//0下标存储红波段，1下标存储蓝波段
            for (int i = 0; i < frameArray.Length; i++)
            {
                frameArray[i] = new TCFrame(configure);
            }
            //测试用
            Log test = new Log("Data", "test" + "_" + MyString.GetSystemTimeStamp() + ".dat", false, true, null);

            while (bRunning)
            {
                lock (bufferList)
                {
                    if (bufferList != null && bufferList.Count != 0)
                    {
                        cypressBytesList.AddRange(bufferList);
                        bufferList.Clear();
                    }
                }

                //按照红波段和蓝波段划分帧
                DivideFrame(cypressBytesList, frameArray);
                //分别解析红波段和蓝波段帧
                bool isParsed = true;
                foreach (var frame in frameArray)
                {
                    if (frame.LineList.Count >= configure.Height)
                    {
                        frame.ParseFrame();
                    }
                    isParsed &= frame.IsParsed;
                }
                //红蓝波段图像都已解析,判断是否crc检验，若检验且crc正确则显示
                if (isParsed)
                {
                    bool imageCrcResult = true;
                    if (isCrcChecked)
                    {
                        foreach (var frame in frameArray)
                        {
                            if (frame.CrcResult() == false)
                            {
                                imageCrcResult = false;
                                break;
                            }
                        }
                    }
                    if (imageCrcResult)
                    {
                        string direcorty = "Images";
                        List<MyImage> imagesList = new List<MyImage>();
                        List<List<ParseNode>> imageInforList = new List<List<ParseNode>>();
                        foreach (var frame in frameArray)
                        {
                            if (frame != null)
                            {
                                if (frame.GetImage() != null)
                                {
                                    imagesList.Add(frame.GetImage());
                                }
                                if (frame.GetFrameHead().ParseNodeList != null)
                                {
                                    imageInforList.Add(frame.GetFrameHead().ParseNodeList);
                                }

                            }

                        }

                        cypressControl.Invoke(cypressControl.handFrameCallback, imageInforList, imagesList);
                        //获取系统时间
                        string time = MyString.GetSystemTimeStamp();
                        //保存帧
                        if (isAutoSaved)
                        {
                            //第15个元素为帧计数        
                            for (int i = 0; i < imagesList.Count; i++)
                            {
                                string path = time + "_" + "ccd" + i + "_" + imageInforList[i][15].Value + ".raw";
                                SaveImages(imagesList[i], direcorty, path);
                            }
                        }
                        if (isLog)
                        {
                            //保存帧头信息
                            for (int i = 0; i < imageInforList.Count; i++)
                            {
                                //第15个元素为帧计数
                                string path = time + "_" + "ccd" + i + "_" + imageInforList[i][15].Value + ".txt";
                                SaveImagesInfor(imageInforList[i], direcorty, path);
                            }
                            //保存Crc信息
                            for (int i = 0; i < frameArray.Length; i++)
                            {
                                //第15个元素为帧计数
                                string path = time + "_" + "ccd" + i + "_" + imageInforList[i][15].Value + "_crcResult" + ".txt";
                                SaveCRCInfor(frameArray[i], direcorty, path);
                            }

                        }
                    }
                    foreach (var frame in frameArray)
                    {
                        if (frame != null)
                        {
                            ((TCFrame)frame).Refresh();
                        }
                    }
                }
            }
        }

        private void DivideFrame(List<byte> source, Frame[] frameArray)
        {
            if (
                source == null ||
                source.Count == 0 ||
                frameArray == null ||
                frameArray.Length != 2
            )
            {
                return;
            }

            string[] cameraIdentify = configure.CameraIdentify.Split(',');
            if (cameraIdentify.Length != 2)
                return;
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            int pixelByteLen = configure.Width * configure.Bits / 8;
            int lineLength = lineHeadLen + configure.LineEndLen + pixelByteLen;
            int cameraIdentifyLen = configure.CameraIdentifyLen;
            while (source.Count >= lineLength)
            {
                int linePosition = MyString.IndexOfHexHead(source, configure.LineIdentify);
                if (linePosition == -1)//未找到行头
                {
                    source.Clear();
                    return;
                }
                else if (linePosition != 0)
                {
                    source.RemoveRange(0, linePosition);
                }
                else//找到行头标识，判断相机标识是否正确，若正确，则找到行头
                {
                    //行头位置为0,判断相机标识是否正确
                    int lineIdentifyLen = configure.LineIdentifyLen;
                    byte[] cameraArray = new byte[cameraIdentifyLen];
                    for (int i = 0; i < cameraArray.Length; i++)
                    {
                        cameraArray[i] = source[lineIdentifyLen + i];
                    }
                    string cameraStr = MyConvert.GetStrFromHexBytes(cameraArray).ToLower();
                    //相机标识正确
                    if (cameraStr == cameraIdentify[0] || cameraStr == cameraIdentify[1])
                    {
                        LineData lineData = new LineData();
                        //获取一行数据，通过行数据解析出行头，数据和行尾，把解析出的数据结构
                        //存放在列表中
                        List<byte> subList = source.GetRange(0, lineLength);
                        lineData.Head = subList.GetRange(0, lineHeadLen).ToArray();
                        lineData.Data = subList.GetRange(lineHeadLen, pixelByteLen).ToArray();
                        byte[] lineNum = new byte[lineHeadLen - lineIdentifyLen - cameraIdentifyLen];//行号
                        //已找到行头，相机标识，若行号为0，则找到帧头，解析帧头，获取新的图像高度和宽度
                        Array.Copy(lineData.Head, lineIdentifyLen + cameraIdentifyLen,
                            lineNum, 0, lineNum.Length);
                        bool isFirstLine = true;
                        for (int i = 0; i < lineNum.Length; i++)
                        {
                            if (lineNum[i] != 0)
                            {
                                isFirstLine = false;
                            }

                        }
                        //第一行，包含帧头信息，更新图像大小
                        if (isFirstLine)
                        {
                            ((TCFrame)frameArray[0]).ParseImageSize(lineData.Data);
                            pixelByteLen = configure.Width * configure.Bits / 8;
                            lineLength = lineHeadLen + configure.LineEndLen + pixelByteLen;
                            if (source.Count < lineLength)
                            {
                                continue;
                            }
                        }
                        //图像宽度和高度已更新,重新获取行数据
                        if (isFirstLine)
                        {
                            subList = source.GetRange(0, lineLength);
                            lineData.Head = subList.GetRange(0, lineHeadLen).ToArray();
                            lineData.Data = subList.GetRange(lineHeadLen, pixelByteLen).ToArray();
                        }

                        lineData.End = subList.GetRange(lineHeadLen + pixelByteLen, lineEndLen).ToArray();

                        if (!isFirstLine && configure.Bits == 16)
                        {
                            lineData.Data = MyConvert.BigLittleEndianConvert(lineData.Data);
                        }

                        lineData.CrcResult = CRCITU.RunCRC16(subList.ToArray()) == 0;

                        if (cameraStr == cameraIdentify[0]) //红波段
                        {
                            if (frameArray[0].LineList != null)
                            {
                                frameArray[0].LineList.Add(lineData);
                            }

                        }
                        else if (cameraStr == cameraIdentify[1])//蓝波段
                        {
                            if (frameArray[1].LineList != null)
                            {
                                frameArray[1].LineList.Add(lineData);
                            }
                        }
                        source.RemoveRange(0, lineLength);

                    }
                    else// 行头标识后面没有跟相机标识符因此不是行头
                    {
                        source.RemoveRange(0, lineIdentifyLen);
                    }
                }
            }
        }

        private void SaveImages(MyImage myImage, string direcorty, string path)
        {
            if (myImage != null && myImage.Image != null)
            {
                Log savedImage = new Log(direcorty, path, false, false, null);
                try
                {
                    savedImage.Write(myImage.Image, 0, myImage.Image.Length);
                    savedImage.CloseStream();

                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.StackTrace, "Exception '" + exception.Message + "' thrown by " + exception.Source);
                }
            }
        }

        private void SaveImagesInfor(List<ParseNode> parseNodeList, string direcorty, string path)
        {
            //保存帧头信息
            Log imageHeadLog = new Log(direcorty, path, true, true, Encoding.Unicode);
            try
            {
                foreach (ParseNode node in parseNodeList)
                {
                    imageHeadLog.WriteLine(node.Name + ": " +
                                           node.HexValue + ": " +
                                           node.Value);
                }
                imageHeadLog.CloseStream();
            }
            catch (Exception e)
            {

                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
            }
        }

        private void SaveCRCInfor(Frame frame, string direcorty, string path)
        {
            Log crcLog = new Log(direcorty, path, true, true, Encoding.Unicode);
            try
            {
                crcLog.WriteLine("该帧检测结果: " + frame.CrcResult());

                List<LineData> lineList = frame.LineList;
                for (int i = 0; i < configure.Height; i++)
                {
                    crcLog.WriteLine(lineList[i].ToString());
                }
                crcLog.CloseStream();
            }
            catch (Exception e)
            {

                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
            }
        }

    }
}
