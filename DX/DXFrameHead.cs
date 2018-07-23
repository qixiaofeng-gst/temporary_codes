using DX;
using DXDxCommandText;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PDPU
{
    class DXFrameHead : FrameHead
    {
        public byte windowsNum = 1;
        public DXFrameHead(Configure conf)
            : base(conf)
        {
            telemetryHashTable = new DxTelemetryHash();

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
                "图像输出接口主备状态",
                "FPGA主备",
                "秒脉冲监测",
                "清空模式",
                "像元读出速度",
                "CCD状态",
                "增益",
                "CDS积分速度",
                "读出通道",
                "ADC输入选择",
                "导星1左通道偏置",
                "导星1右通道偏置",
                "导星2左通道偏置",
                "导星2右通道偏置",
                "开窗大小和数量",
                //"开窗数量",
                "导星1开窗位置1",
                "导星1开窗位置2",
                "导星1开窗位置3",
                "导星1开窗位置4",
                "导星2开窗位置1",
                "导星2开窗位置2",
                "导星2开窗位置3",
                "导星2开窗位置4",
                "接收指令计数",
                "发送遥测计数",
                "应答计数",
                "发送帧计数",
                "命令错误计数",
                "奇校验错误计数",
                "通信校验错误计数",
                "通信命令格式错误计数",
                "通讯超时计数"
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
            telemetryList[keyParameterIndex++].length = 1;

            //图像输出接口主备状态, FPGA主备, 秒脉冲监测, 清空模式, 像元读出速度, 
            //CCD状态,增益, CDS积分速度，读出通道，ADC输入选择
            for (int i = 0; i < 10; i++)
            {
                telemetryList[keyParameterIndex++].length = 1;
            }

            /*导星1 左通道偏置
            导星1 右通道偏置
            导星2 左通道偏置
            导星2 右通道偏置
            */
            for (int i = 0; i < 4; i++)
            {
                telemetryList[keyParameterIndex++].length = 2;
            }

            //开窗大小和数量
            telemetryList[keyParameterIndex++].length = 1;

            /*导星1开窗位置1
            导星1开窗位置2
            导星1开窗位置3
            导星1开窗位置4
            导星2开窗位置1
            导星2开窗位置2
            导星2开窗位置3
            导星2开窗位置4
             */
            for (int i = 0; i < 8; i++)
            {
                telemetryList[keyParameterIndex++].length = 4;
            }
            //接收指令计数 发送遥测计数  应答计数 发送帧计数
            for (int i = 0; i < 4; i++)
            {
                telemetryList[keyParameterIndex++].length = 2;
            }
            //命令错误计数 奇校验错误计数 通信校验错误计数 通信命令格式错误计数 通讯超时计数
            for (int i = 0; i < 5; i++)
            {
                telemetryList[keyParameterIndex++].length = 1;
            }

            if (keyParameterIndex != telemetryList.Count)
            {
                MessageBox.Show("初始化导星参数错误！");
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
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.WorkMode]));
                //成像类型
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.ImageTpye]));
                if (telemetryList.Count > telemetryIndex - 1
                    && telemetryList[telemetryIndex - 1].data[0] == DxCommandText.ImageWindows)
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
                //导星曝光时间
                {
                    parsedList.Add(GenerateNode(telemetryList[telemetryIndex].data[0],
                        telemetryList[telemetryIndex].data[0] * 50 + "ms"));
                    telemetryIndex++;
                }
                //图像输出接口主备状态
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.ImageMasterSlave]));
                //FPGA主备
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.FpgaMasterSlave]));
                //秒脉冲监测
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.Pps]));
                //清空模式
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.ClearMode]));
                //像元读出速度
                parsedList.Add(GenerateNode(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.PixelReadRate]));
                // CCD状态
                parsedList.Add(GenerateNodeContainTwoFgs(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.CcdState],
                    "FGS1：",
                    "\nFGS2："));
                //增益
                parsedList.Add(GenerateNodeContainTwoFgs(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.Gain],
                    "FGS1：",
                    "\nFGS2："));
                //CDS积分速度
                parsedList.Add(GenerateNodeContainTwoFgs(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.IntegrateRate],
                    "FGS1：",
                    "\nFGS2："));
                //读出通道
                parsedList.Add(GenerateNodeContainTwoFgs(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.ReadChannel],
                    "FGS1：",
                    "\nFGS2："));
                //ADC输入选择
                parsedList.Add(GenerateNodeContainTwoFgs(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.AdcSelect],
                    "FGS1：",
                    "\nFGS2："));

                /*导星1 左通道偏置
                导星1 右通道偏置
                导星2 左通道偏置
                导星2 右通道偏置
                */
                for (int i = 0; i < 4; i++)
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    parsedList.Add(GenerateUint16Node(twoBytes));
                }
                //开窗大小和数量
                parsedList.Add(GenerateNodeContainTwoFgs(telemetryList[telemetryIndex++].data[0],
                    (Hashtable)telemetryHashTable.HashTable[DxTelemetryPrameterKey.WindowsInfor],
                    "大小：",
                    "\n数量："));
                if (isWindowed)
                {
                    windowsNum = (byte)(telemetryList[telemetryIndex - 1].data[0] & 0x0f);
                }
                /*导星1开窗位置1
                导星1开窗位置2
                导星1开窗位置3
                导星1开窗位置4
                导星2开窗位置1
                导星2开窗位置2
                导星2开窗位置3
                导星2开窗位置4
                 */
                for (int i = 0; i < 8; i++)
                {
                    byte[] xBytes = new byte[2];
                    byte[] yBytes = new byte[2];
                    Array.Copy(telemetryList[telemetryIndex].data, xBytes, xBytes.Length);
                    Array.Copy(telemetryList[telemetryIndex].data, xBytes.Length, yBytes, 0, yBytes.Length);
                    uint x = MyConvert.GetUInt16FromHexBytes(xBytes);
                    uint y = MyConvert.GetUInt16FromHexBytes(yBytes);
                    parsedList.Add(GenerateNode(telemetryList[telemetryIndex].data,
                        String.Format("({0},{1})", x, y)));
                    telemetryIndex++;
                }

                //接收指令计数 发送遥测计数  应答计数 发送帧计数
                for (int i = 0; i < 4; i++)
                {
                    byte[] twoBytes = telemetryList[telemetryIndex++].data;
                    parsedList.Add(GenerateUint16Node(twoBytes));
                }

                //命令错误计数 奇校验错误计数 通信校验错误计数 通信命令格式错误计数 通讯超时计数
                for (int i = 0; i < 5; i++)
                {
                    parsedList.Add(GenerateByteNode(telemetryList[telemetryIndex++].data[0]));
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
            //第30个字段为帧计数
            return MyConvert.GetUInt16FromHexBytes(telemetryList[30].data);
        }

        //1个字的高4位和低4位分别表示两个参数
        private Node GenerateNodeContainTwoFgs(byte key, Hashtable hashTable, string prefix1, string prefix2)
        {
            if (hashTable == null)
            {
                return null;
            }
            byte key1 = (byte)(key >> 4);
            byte key2 = (byte)(key & 0x0f);

            string context = prefix1 + (hashTable[(UInt32)key1] ?? "未识别")
                 + prefix2 + (hashTable[(UInt32)key2] ?? "未识别");
            return GenerateNode(key, context);

        }
    }
}
