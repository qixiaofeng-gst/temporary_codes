using PDPU;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DX
{
    class DXCypress : Cypress
    {
        public DXCypress(CypressControl cypressControl) : base(cypressControl) { }

        ~DXCypress() { }

        public override unsafe void ParseThread()
        {
            int frameLine = configure.FrameHeadLine;
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            int width = configure.Width;
            int height = configure.Height;
            int bits = configure.Bits;
            string headString = configure.FrameHead;

            int frameHeadLength = 180;

            int pixelLen = 2 * width * bits / 8;
            int frameLength = (lineHeadLen + lineEndLen + pixelLen) * (height + frameLine);

            int framePosition = 0;
            bool isFindFrame = false;
            bool isParseHead = false;

            List<byte> cypressBytesList = new List<byte>(frameLength * 2);
            DXFrame frame = new DXFrame(configure);

            //测试用
            Log test = new Log("Data" + "_" + MyString.GetSystemTimeStamp() + ".dat", "test", false, true, null);
            while (bRunning)
            {
                byte[] tmp = null;
                lock (xferData)
                {
                    try
                    {
                        if (xferData.length != 0)
                        {
                            test.Write(xferData.data, 0, xferData.length);

                            tmp = new byte[xferData.length];
                            Buffer.BlockCopy(xferData.data, 0, tmp, 0, xferData.length);
                        }
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                    }
                }

                if (tmp != null)
                {
                    cypressBytesList.AddRange(tmp);
                }

                if (!isFindFrame)
                {
                    framePosition = MyString.IndexOfHexHead(cypressBytesList, headString);
                    if (framePosition != -1) //找到帧头
                    {
                        isFindFrame = true;
                    }

                }
                else if (isFindFrame && !isParseHead
                    && cypressBytesList.Count - framePosition > frameHeadLength)
                {
                    isParseHead = true;

                    frame.ParseFrameHead(cypressBytesList.GetRange(framePosition, frameHeadLength));
                    width = configure.Width;
                    height = configure.Height;

                    pixelLen = 2 * width * bits / 8;
                    frameHeadLength = frame.GetFrameHeadLength();
                    frameLength = frame.GetFrameLength();

                    frame.SetFrameHeadData(cypressBytesList.GetRange(framePosition, frameHeadLength));
                }
                else if (isFindFrame && cypressBytesList.Count - framePosition >= frameLength) //一帧数据
                {
                    frame.ParseFrameData(cypressBytesList.GetRange(framePosition + frameHeadLength,
                        frameLength - frameHeadLength));

                    string direcorty = "Images";
                    FrameHead frameHead = frame.GetFrameHead();
                    List<MyImage> imagesList = frame.GetImageList();

                    bool imageCrcResult = frameHead.LineData.CrcResult;
                    if (isCrcChecked)
                    {
                        //crc检验，包括帧头和该帧每一行数据
                        List<LineData> lineList = frame.GetFrameData().LineList;
                        for (int i = 0; imageCrcResult && i < lineList.Count; i++)
                        {
                            if (lineList[i].CrcResult == false)
                            {
                                imageCrcResult = false;
                            }
                        }
                        if (imageCrcResult)
                        {
                            cypressControl.Invoke(cypressControl.handFrameCallback, frameHead.telemetryList, imagesList);
                        }
                    }
                    else
                    {
                        cypressControl.Invoke(cypressControl.handFrameCallback, frameHead.telemetryList, imagesList);
                    }

                    //获取系统时间
                    string time = MyString.GetSystemTimeStamp();

                    if (isLog)
                    {
                        //保存帧头信息
                        Log imageHeadLog = new Log(direcorty, time + "_" + frameHead.GetFrameNo() + ".txt", true, true, Encoding.Unicode);
                        try
                        {
                            for (int i = 0; i < frameHead.telemetryList.Count; i++)
                            {
                                imageHeadLog.WriteLine(frameHead.telemetryList[i].showField.Name + ": " +
                                    frameHead.telemetryList[i].showField.Value + ": " +
                                    frameHead.telemetryList[i].showField.Context);
                            }
                            imageHeadLog.CloseStream();
                        }
                        catch (Exception e)
                        {

                            MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                        }

                        //保存帧
                        if (isAutoSaved)
                        {
                            //获取一帧数据
                            byte[] frameArray = new byte[frameLength];
                            int frameArrayCount = 0;
                            //复制帧头数据
                            cypressBytesList.GetRange(framePosition, frameHeadLength).CopyTo(frameArray);
                            frameArrayCount += frameHeadLength;
                            //复制帧数据
                            foreach (var lineData in frame.GetFrameData().LineList)
                            {
                                try
                                {
                                    //复制行头
                                    Array.Copy(lineData.Head, 0, frameArray, frameArrayCount, lineData.Head.Length);
                                    frameArrayCount += lineData.Head.Length;
                                    //复制数据
                                    Array.Copy(lineData.Data, 0, frameArray, frameArrayCount, lineData.Data.Length);
                                    frameArrayCount += lineData.Data.Length;
                                    //复制行尾
                                    Array.Copy(lineData.End, 0, frameArray, frameArrayCount, lineData.End.Length);
                                    frameArrayCount += lineData.End.Length;
                                }
                                catch (Exception e)
                                {

                                    MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                                }

                            }

                            Log frameLog = new Log(direcorty, time + "_" + frameHead.GetFrameNo() + ".raw", false, false, null);
                            try
                            {
                                frameLog.Write(frameArray, 0, frameArray.Length);
                                frameLog.CloseStream();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                            }
                        }

                        //保存Crc信息
                        if (isCrcChecked)
                        {
                            Log crcLog = new Log(direcorty, time + "_" + frameHead.GetFrameNo() + "_crcResult" + ".txt", true, true, Encoding.Unicode);
                            try
                            {
                                crcLog.WriteLine("该帧检测结果: " + imageCrcResult);

                                LineData headLine = frameHead.LineData;
                                crcLog.WriteLine(headLine.ToString());

                                List<LineData> lineList = frame.GetFrameData().LineList;
                                for (int i = 0; i < lineList.Count; i++)
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
                    isFindFrame = false;
                    isParseHead = false;
                    cypressBytesList.RemoveRange(0, framePosition + frameLength);
                    frame.GetFrameData().ClearList();
                }
            }
        }
    }
}
