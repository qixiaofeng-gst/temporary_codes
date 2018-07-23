using PDPU;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TC
{
    class TCFrameHead : FrameHead
    {
        public TCFrameHead(Configure conf)
            : base(conf)
        {
            telemetryHashTable = new TcTelemetryHash();
            LoadXml();
        }

        protected override sealed void InitializeTelemetry()
        {
            //遥测参数页字段名字
            string[] fieldName =
            {
                "工作模式",
                "成像类型",
                "曝光开始时间",
                "曝光时间",
                "PDUP LVDS主备份",
                "FPGA主备",
                "秒脉冲监测",
                "清空模式",
                "蓝波段增益",
                "蓝波段积分速度",
                "蓝波段读出通道",
                "蓝波段像元读出速度",
                "蓝波段左通道偏置",
                "蓝波段右通道偏置",
                "蓝波段开窗大小",
                "蓝波段开窗位置XY",
                "蓝波段帧计数",
                "红波段增益",
                "红波段积分速度",
                "红波段读出通道",
                "红波段像元读出速度",
                "红波段左通道偏置",
                "红波段右通道偏置",
                "红波段开窗大小",
                "红波段开窗位置XY",
                "红波段帧计数",
                "命令格式错误",
                "奇校验错误",
                "指令校验错误",
                "超时错误",
                "帧头错误",
                "接收指令计数",
                "发送指令计数"
            };

            ShowField[] showFieldArray = new ShowField[fieldName.Length];
            for (int i = 0; i < showFieldArray.Length; i++)
            {
                showFieldArray[i] = new ShowField { Name = fieldName[i] };
            }
            showValueList = new List<ShowField>();
            showValueList.AddRange(showFieldArray);

            for (int i = 0; i < fieldName.Length; i++)
            {
                var telemetryKeyWord = new TelemetryKeyWord()
                {
                    showField = showFieldArray[i],
                    data = null
                };
                telemetryList.Add(telemetryKeyWord);
            }

            //参数字段下标索引
            int keyParameterIndex = 0;
            //工作模式 成像类型
            telemetryList[keyParameterIndex].startPose = 10;
            for (int i = 0; i < 2; i++)
            {
                telemetryList[keyParameterIndex++].length = 1;
            }
            //曝光开始时间
            telemetryList[keyParameterIndex++].length = 6;
            //曝光时间
            telemetryList[keyParameterIndex++].length = 2;

            //PDUP LVDS主备份, FPGA主备, 秒脉冲监测, 清空模式, 蓝波段增益, 
            //蓝波段积分速度,蓝波段读出通道, 蓝波段像元读出速度
            for (int i = 0; i < 8; i++)
            {
                telemetryList[keyParameterIndex++].length = 1;
            }

            //蓝波段左通道偏置 蓝波段右通道偏置 蓝波段开窗大小
            for (int i = 0; i < 3; i++)
            {
                telemetryList[keyParameterIndex++].length = 2;
            }
            //蓝波段开窗位置XY
            telemetryList[keyParameterIndex++].length = 4;
            //蓝波段帧计数
            telemetryList[keyParameterIndex++].length = 2;
            //红波段增益 红波段积分速度 红波段读出通道 红波段像元读出速度
            for (int i = 0; i < 4; i++)
            {
                telemetryList[keyParameterIndex++].length = 1;
            }
            //红波段左通道偏置 红波段右通道偏置 红波段开窗大小
            for (int i = 0; i < 3; i++)
            {
                telemetryList[keyParameterIndex++].length = 2;
            }
            //红波段开窗位置XY
            telemetryList[keyParameterIndex++].length = 4;
            //红波段帧计数
            telemetryList[keyParameterIndex++].length = 2;
            //命令格式错误 奇校验错误 指令校验错误 超时错误 帧头错误
            for (int i = 0; i < 5; i++)
            {
                telemetryList[keyParameterIndex++].length = 1;
            }
            //接收指令计数 发送指令计数
            for (int i = 0; i < 2; i++)
            {
                telemetryList[keyParameterIndex++].length = 2;
            }

            if (keyParameterIndex != telemetryList.Count)
            {
                MessageBox.Show("初始化探测参数错误！");
            }
            else
            {
                telemetryList[0].data = new byte[telemetryList[0].length];
                for (int i = 1; i < telemetryList.Count; i++)
                {
                    telemetryList[i].data = new byte[telemetryList[i].length];
                    telemetryList[i].startPose = telemetryList[i - 1].startPose
                                                             + telemetryList[i - 1].length;
                }
            }
            if (telemetryList.Count != 0)
            {
                frameHeadLength = telemetryList[telemetryList.Count - 1].startPose
                              + telemetryList[telemetryList.Count - 1].length;
            }
        }

        public override void Parse(LineData lineData)
        {
            this.lineData = lineData;
            if (lineData == null || lineData.Head == null ||
                lineData.Data == null || lineData.End == null ||
                frameNodeList == null || frameNodeList.Count == 0)
            {
                return;
            }
            parseNodeList = new List<ParseNode>();
            foreach (var frameNode in frameNodeList)
            {
                //解析所有的xml定义的节点
                if (frameNode.Pos + frameNode.Len <= lineData.Data.Length)
                {
                    byte[] data = new byte[frameNode.Len];
                    Array.Copy(lineData.Data, frameNode.Pos, data, 0, data.Length);
                    ParseNode parseNode = new ParseNode(frameNode, data);
                    parseNode.Parse();
                    if (parseNodeList != null)
                    {
                        parseNodeList.Add(parseNode);
                    }

                }
            }

        }

        public void Parse(byte[] frameHeadBytes)
        {

        }

        public override void Parse(List<byte> list)
        {
            previousFrameNum = GetFrameNo();
            if (list == null || list.Count == 0)
            {
                return;
            }
            if (list.Count >= frameHeadLength)
            {
                byte[] bList = list.ToArray();
                foreach (TelemetryKeyWord telemetryKeyWord in telemetryList)
                {
                    Buffer.BlockCopy(bList, telemetryKeyWord.startPose,
                        telemetryKeyWord.data, 0, telemetryKeyWord.length);
                }
                List<Node> parsedList = new List<Node>();
                int telemetryIndex = 0;
                //开始解析
                //工作模式
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.WorkMode]));
                //成像类型
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.ImageTpye]));
                if (telemetryList.Count > telemetryIndex - 1
                    && telemetryList[telemetryIndex - 1].data[0] == TcCommandText.ImageWindows)
                {
                    isWindowed = true;
                }
                else
                {
                    isWindowed = false;
                }
                //曝光开始时间
                byte[] secondBytes = new byte[4];
                byte[] millisecondBytes = new byte[2];
                Array.Copy(telemetryList[telemetryIndex].data, secondBytes, secondBytes.Length);
                Array.Copy(telemetryList[telemetryIndex].data, secondBytes.Length, millisecondBytes, 0, millisecondBytes.Length);
                uint second = MyConvert.GetUInt32FromHexBytes(secondBytes);
                uint millisecond = MyConvert.GetUInt16FromHexBytes(millisecondBytes);
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex].data,
                    String.Format("{0}.{1}秒", second, millisecond)));
                telemetryIndex++;
                //曝光时间
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[1],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.ExposeTime]));
                //PDUP LVDS主备份
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.ImageMasterSlave]));
                //FPGA主备
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.FpgaMasterSlave]));
                //秒脉冲监测
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.Pps]));
                //清空模式
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.ClearMode]));
                //蓝波段增益
                parsedList.Add(GenerateNode((byte)(telemetryList[telemetryIndex++].data[0] >> 4),
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.Gain]));
                //蓝波段积分速度
                parsedList.Add(GenerateNode((byte)(telemetryList[telemetryIndex++].data[0] >> 4),
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.IntegrateRate]));
                //蓝波段读出通道
                parsedList.Add(GenerateNode((byte)(telemetryList[telemetryIndex++].data[0] >> 4),
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.ReadChannel]));
                //蓝波段像元读出速度
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.PixelReadRate]));
                //蓝波段左通道偏置 蓝波段右通道偏置
                for (int i = 0; i < 2; i++)
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    parsedList.Add(GenerateUint16Node(twoBytes));
                }
                //蓝波段开窗大小
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    ushort key = MyConvert.GetUInt16FromHexBytes(twoBytes);
                    string infor = ((Hashtable)(telemetryHashTable.HashTable[TcTelemetryPrameterKey.WindowsInfor]))[(UInt32)key] as string;

                    infor = infor ?? key.ToString();
                    parsedList.Add(GenerateNode(twoBytes, infor));
                }
                //蓝波段开窗位置XY
                {
                    byte[] data = telemetryList[telemetryIndex].data;
                    if (data != null && data.Length == 4)
                    {
                        byte[] arrayX = new byte[2];
                        byte[] arrayY = new byte[2];
                        Array.Copy(data, arrayX, arrayX.Length);
                        Array.Copy(data, arrayX.Length, arrayY, 0, arrayY.Length);
                        string infor = String.Format("({0},{1})",
                            MyConvert.GetUInt16FromHexBytes(arrayX),
                            MyConvert.GetUInt16FromHexBytes(arrayY));
                        parsedList.Add(GenerateNode(data, infor));
                    }
                    telemetryIndex++;
                }
                //蓝波段帧计数
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    parsedList.Add(GenerateUint16Node(twoBytes));
                }

                //红波段增益
                parsedList.Add(GenerateNode((byte)(telemetryList[telemetryIndex++].data[0] & 0x0f),
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.Gain]));
                //红波段积分速度
                parsedList.Add(GenerateNode((byte)(telemetryList[telemetryIndex++].data[0] & 0x0f),
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.IntegrateRate]));
                //红波段读出通道
                parsedList.Add(GenerateNode((byte)(telemetryList[telemetryIndex++].data[0] & 0x0f),
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.ReadChannel]));
                //红波段像元读出速度
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[TcTelemetryPrameterKey.PixelReadRate]));
                //红波段左通道偏置 红波段右通道偏置
                for (int i = 0; i < 2; i++)
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    parsedList.Add(GenerateUint16Node(twoBytes));
                }
                //红波段开窗大小
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    ushort key = MyConvert.GetUInt16FromHexBytes(twoBytes);
                    string infor = ((Hashtable)(telemetryHashTable.HashTable[TcTelemetryPrameterKey.WindowsInfor]))[(UInt32)key] as string;

                    infor = infor ?? key.ToString();
                    parsedList.Add(GenerateNode(twoBytes, infor));
                }
                //红波段开窗位置XY
                {
                    byte[] data = telemetryList[telemetryIndex].data;
                    if (data != null && data.Length == 4)
                    {
                        byte[] arrayX = new byte[2];
                        byte[] arrayY = new byte[2];
                        Array.Copy(data, arrayX, arrayX.Length);
                        Array.Copy(data, arrayX.Length, arrayY, 0, arrayY.Length);
                        string infor = String.Format("({0},{1})",
                            MyConvert.GetUInt16FromHexBytes(arrayX),
                            MyConvert.GetUInt16FromHexBytes(arrayY));
                        parsedList.Add(GenerateNode(data, infor));
                    }
                    telemetryIndex++;
                }
                //红波段帧计数
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    parsedList.Add(GenerateUint16Node(twoBytes));
                }

                //命令格式错误 奇校验错误 指令校验错误 超时错误 帧头错误
                for (int i = 0; i < 5; i++)
                {
                    parsedList.Add(GenerateByteNode(telemetryList[telemetryIndex++].data[0]));
                }
                //接收指令计数 发送指令计数
                for (int i = 0; i < 2; i++)
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    parsedList.Add(GenerateUint16Node(twoBytes));
                }
                if (telemetryList.Count != parsedList.Count)
                    return;
                for (int i = 0; i < telemetryList.Count; i++)
                {
                    telemetryList[i].showField.Value = parsedList[i].value;
                    telemetryList[i].showField.Context = parsedList[i].context;
                }
            }

            Save(telemetryList);
        }

        public override ushort GetFrameNo()
        {
            //第16个字段为帧计数
            return MyConvert.GetUInt16FromHexBytes(parseNodeList[15].GetDatas());
        }
        public string GetCameraIndentify()
        {
            byte[] camera = 
            {
                lineData.Head[2],
                lineData.Head[3],
            };
            string cameraStr = "";
            if (camera[0] == 0x05 && camera[1] == 0x05)
            {
                cameraStr = "红波段";
            }
            else if (camera[0] == 0x0a && camera[1] == 0x0a)
            {
                cameraStr = "蓝波段";
            }
            return cameraStr;
        }

        private void LoadXml()
        {

            IEnumerable<XElement> elements = null;
            try
            {
                XDocument xdoc = XDocument.Load("frameInfor.xml"); //加载xml文件  
                XElement root = xdoc.Root; //获取根元素
                elements = from ele in root.Elements("Node")
                           select ele;
            }
            catch (Exception ep)
            {
                MessageBox.Show(ep.StackTrace, "Exception '" + ep.Message + "' thrown by" + ep.Source);
            }
            if (elements != null)
            {
                foreach (var ele in elements)
                {
                    FrameNodeModel frameNodeModel = new FrameNodeModel();
                    frameNodeModel.Id = ele.Element("id").Value;
                    frameNodeModel.Name = ele.Element("name").Value;
                    try
                    {
                        frameNodeModel.Pos = Convert.ToInt32(ele.Element("pos").Value.Trim());
                    }
                    catch (Exception ep)
                    {
                        MessageBox.Show(ep.StackTrace, "Exception '" + ep.Message + "' thrown by" + ep.Source);
                    }

                    try
                    {
                        frameNodeModel.Len = Convert.ToInt32(ele.Element("len").Value.Trim());
                    }
                    catch (Exception ep)
                    {
                        MessageBox.Show(ep.StackTrace, "Exception '" + ep.Message + "' thrown by" + ep.Source);
                    }
                    frameNodeModel.DataType = ele.Element("dataType").Value;
                    if (frameNodeModel.DataType == DataType.str.ToString())
                    {
                        frameNodeModel.Rule = ele.Element("rule").Value;
                    }
                    frameNodeModel.ParseSubInfor();
                    if (frameNodeList != null)
                    {
                        frameNodeList.Add(frameNodeModel);
                    }
                }
            }
        }

    }
}
