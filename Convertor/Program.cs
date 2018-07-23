using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using PDPU;
using TC;

namespace Convertor
{
    class Program
    {
        private Configure configure = new Configure();

        private int frameLine = 0;
        private int lineHeadLen = 0;
        private int lineEndLen = 0;
        private int width = 0;
        private int height = 0;
        private int bits = 0;
        private string headString = null;

        private int pixelLen = 0;
        private int frameHeadLength = 0;
        private int frameLength = 0;

        private List<byte> cypressBytesList = null;
        private Frame[] frameArray = new Frame[2];

        private int block_sn = 0;

        private Program()
        {
            configure.Load();

            frameLine = configure.FrameHeadLine;
            lineHeadLen = configure.LineHeadLen;
            lineEndLen = configure.LineEndLen;
            width = configure.Width;
            height = configure.Height;
            bits = configure.Bits;
            headString = configure.FrameHead;

            pixelLen = width * bits / 8;
            frameHeadLength = lineHeadLen + lineEndLen + pixelLen;
            frameLength = (lineHeadLen + lineEndLen + pixelLen) * (height + frameLine) * 2;

            cypressBytesList = new List<byte>(frameLength * 2);
            frameArray = new Frame[2]; //0下标存储红波段，1下标存储蓝波段
            for (int i = 0; i < frameArray.Length; i++)
            {
                frameArray[i] = new TCFrame(configure);
            }
        }

        private void Convert(string in_file_path)
        {
            if (File.Exists(in_file_path))
            {
                FileStream fs = new FileStream(in_file_path, FileMode.Open);
                Console.WriteLine("Length of the file: " + fs.Length + " bytes. (" + (fs.Length / (1024 * 1024)) + " MB).");
                int block_count = 0;
                while (ProcessBlock(fs))
                {
                    ++block_count;
                }
                Console.WriteLine("Blocks count: " + block_count);
                fs.Close();
            }
            else
            {
                Console.WriteLine("File '" + in_file_path + "' does not exist.");
            }
        }

        private bool ProcessBlock(FileStream in_fs)
        {
            ++block_sn;
            int block_length = ReadInteger(in_fs);
            if (block_length > 0)
            {
                //Console.WriteLine("Current block SN:" + block_sn);
                byte[] block = new byte[block_length];
                int readed = in_fs.Read(block, 0, block_length);
                if (readed == block_length)
                {
                    TryParse(block);
                    return true;
                }
                else
                {
                    Console.WriteLine("Ending with an invalid block. Expected length: " + block_length + ", got length: " + readed);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void TryParse(byte[] in_block)
        {
            cypressBytesList.AddRange(in_block);

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
                if (false == frame.IsParsed)
                {
                    isParsed = false;
                }
            }
            
            //红蓝波段图像都已解析,判断是否crc检验，若检验且crc正确则显示
            if (isParsed)
            {
                bool imageCrcResult = true;
                foreach (var frame in frameArray)
                {
                    if (frame.CrcResult() == false)
                    {
                        imageCrcResult = false;
                        break;
                    }
                }

                string direcorty = "Images";
                List<MyImage> imagesList = new List<MyImage>();
                List<List<ParseNode>> imageInforList = new List<List<ParseNode>>();
                foreach (var frame in frameArray)
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

                //获取系统时间
                string time = MyString.GetSystemTimeStamp();
                //保存帧
                //第15个元素为帧计数        
                for (int i = 0; i < imagesList.Count; i++)
                {
                    string path = time + "_" + "ccd" + i + "_" + imageInforList[i][15].Value + ".raw";
                    SaveImages(imagesList[i], direcorty, path);
                }
                //保存帧头信息
                for (int i = 0; i < imageInforList.Count; i++)
                {
                    //第15个元素为帧计数
                    string path = time + "_" + "ccd" + i + "_" + imageInforList[i][15].Value + ".txt";
                    SaveImagesInfor(imageInforList[i], direcorty, path);
                }

                if (imageCrcResult)
                {
                    Console.WriteLine("Valid frame got.");
                }
                else
                {
                    Console.WriteLine("An invalid frame detected, still to be saved.");
                }
                foreach (var frame in frameArray)
                {
                    ((TCFrame)frame).Refresh();
                }
            }
            else
            {
                // Not parsed.
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
                        Array.Copy(lineData.Head, lineIdentifyLen + cameraIdentifyLen, lineNum, 0, lineNum.Length);
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
                try
                {
                    Bitmap bm = myImage.GetBitmap();
                    bm.Save(direcorty + "/" + path);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.StackTrace);
                    Console.WriteLine("Exception '" + exception.Message + "' thrown by " + exception.Source);
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
                    imageHeadLog.WriteLine(node.Name + ": " + node.HexValue + ": " + node.Value);
                }
                imageHeadLog.CloseStream();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("Exception '" + exception.Message + "' thrown by " + exception.Source);
            }
        }

        private int ReadInteger(FileStream in_fs)
        {
            const int width = 4;
            byte[] i = new byte[width];
            int readed = in_fs.Read(i, 0, width);
            if (readed == width)
            {
                return (i[0] << 24) + (i[1] << 16) + (i[2] << 8) + i[3];
            }
            else
            {
                return 0;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to process.");
            Console.ReadLine();
            Program p = new Program();
            foreach (string arg in args)
            {
                Console.WriteLine("Begin to convert: " + arg);
                p.Convert(arg);
                Console.WriteLine("End of: " + arg + ". Press enter to continue.");
                Console.ReadLine();
            }
            Console.WriteLine("All input files processed. Press enter to quit.");
            Console.ReadLine();
        }
    }
}
