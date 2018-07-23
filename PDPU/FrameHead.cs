using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace PDPU
{
    [Serializable]
    public class ShowField : ICloneable, INotifyPropertyChanged
    {
        private string _name;
        private string _value;
        private string _context;

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }

            }
            get { return _name; }
        }

        public string Value
        {
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                }
            }
            get { return _value; }
        }

        public string Context
        {
            set
            {
                if (_context != value)
                {
                    _context = value;
                    NotifyPropertyChanged("Context");
                }
            }
            get { return _context; }
        }

        public ShowField()
        {
            _name = "";
            _value = "";
            _context = "";
        }

        public object Clone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                object copy = formatter.Deserialize(objectStream);
                objectStream.Close();
                return copy;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class TelemetryKeyWord
    {
        public ShowField showField;
        public byte[] data;
        public int startPose;
        public int length;
    }


    public class FrameHead
    {
        protected LineData lineData;

        protected Configure configure;

        //public List<ShowField> headList;

        protected int frameHeadLength;

        private Log frameHeadInfor;

        private Log frameHeadError;

        protected ushort previousFrameNum;

        public List<ShowField> showValueList; //显示的帧头信息

        public List<TelemetryKeyWord> telemetryList; //帧头信息

        protected TelemetryHash telemetryHashTable;

        protected bool isWindowed;

        protected List<FrameNodeModel> frameNodeList;
        protected List<ParseNode> parseNodeList;

        public List<FrameNodeModel> FrameNodeList
        {
            get { return frameNodeList; }
        }

        public List<ParseNode> ParseNodeList
        {
            get { return parseNodeList; }
        }

        public bool IsWindowed
        {
            get { return isWindowed; }
        }

        public LineData LineData
        {
            get { return lineData; }
        }

        public FrameHead()
        {
        }

        public FrameHead(Configure conf)
        {
            showValueList = new List<ShowField>();
            telemetryList = new List<TelemetryKeyWord>();
            configure = conf;
            lineData = new LineData();
            previousFrameNum = 0;
            frameHeadLength = 0;
            InitializeTelemetry();
            frameNodeList = new List<FrameNodeModel>();
        }

        public void GenerateLog()
        {
            frameHeadInfor = new Log("Data", "frameHeadInfor" + "_" + MyString.GetSystemTimeStamp() + ".dat", true, true, Encoding.Unicode);
            frameHeadError = new Log("Data", "frameHeadError" + "_" + MyString.GetSystemTimeStamp() + ".dat", true, true, Encoding.Unicode);
        }


        public void SetData(List<byte> list)
        {
            int lineHeadLen = configure.LineHeadLen;
            int lineEndLen = configure.LineEndLen;
            lineData.Head = list.GetRange(0, lineHeadLen).ToArray();
            lineData.Data = list.GetRange(lineHeadLen, list.Count - lineHeadLen - lineEndLen).ToArray();
            lineData.End = list.GetRange(list.Count - lineEndLen, lineEndLen).ToArray();
            lineData.CrcResult = CRCITU.RunCRC16(list.ToArray()) == 0 ? true : false;
        }
        
        protected virtual void InitializeTelemetry()
        {
                    
		}

        public virtual void Parse(List<byte> list)
        {
            
        }

        public virtual void Parse(LineData lineData)
        {

        }

        public void SetConfigure(Configure conf)
        {
            configure = conf;
        }

        public string GetWindowXY(byte[] data)
        {
            string value = null;
            if (data != null && data.Length >= 4)
            {
                byte[] bWindowsX = new byte[2];
                byte[] bWindowsY = new byte[2];
                Buffer.BlockCopy(data, 0, bWindowsX, 0, 2);
                Buffer.BlockCopy(data, 2, bWindowsY, 0, 2);
                value = "(" + MyConvert.GetInt16FromHexBytes(bWindowsX) + ","
                                        + MyConvert.GetInt16FromHexBytes(bWindowsY) + ")";
            }
            return value;

        }

        public virtual ushort GetFrameNo()
        {
            return 0;
        }

        public void Save(List<TelemetryKeyWord> list)
        {
            string strBuffer = "-------------------" + "\r\n" + "-------------------" + "\r\n";

            foreach (var keyWord in list)
            {
                if (frameHeadInfor != null && keyWord != null)
                {
                    strBuffer += String.Format("{0}: {1}: {2}\r\n", keyWord.showField.Name,
                        keyWord.showField.Value, keyWord.showField.Context);
                }
            }
            try
            {
                frameHeadInfor.WriteLine(strBuffer);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
            }

            ushort currentFrameNum = GetFrameNo();
            if (frameHeadError != null && currentFrameNum - previousFrameNum != 1)
            {
                string strError = String.Format("Error between frame {0}-{1}", previousFrameNum.ToString(), currentFrameNum.ToString());

                try
                {
                    frameHeadError.WriteLine(strError);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                }
            }
            
        }
        //返回包含ushort的结点
        protected Node GenerateUint16Node(byte[] twoBytes)
        {
            if (twoBytes == null || twoBytes.Length != 2)
            {
                return null;
            }
            ushort ushortTmp = MyConvert.GetUInt16FromHexBytes(twoBytes);
            return GenerateNode(twoBytes, ushortTmp.ToString());
        }

        //返回包含浮点数的结点
        protected Node GenerateFloatNode(byte[] fourBytes)
        {
            if (fourBytes == null || fourBytes.Length != 4)
            {
                return null;
            }
            float floatTmp = MyConvert.GetFloatFromHexBytes(fourBytes);
            return GenerateNode(fourBytes, String.Format("{0:f2}", floatTmp));
        }

        protected Node GenerateNode(byte key, Hashtable hashTable)
        {
            if (hashTable == null)
            {
                return null;
            }
            return GenerateNode(key, (string)hashTable[(UInt32)key]);
        }

        protected Node GenerateNode(byte[] value, string context)
        {
            if (value == null || value.Length == 0)
            {
                return null;
            }
            Node node = new Node { context = context ?? "未识别" };
            node.value = "0x";
            for (int i = 0; i < value.Length; i++)
            {
                node.value = String.Format("{0}{1:x2}", node.value, value[i]);
            }
            return node;
        }

        protected Node GenerateNode(byte value, string context)
        {
            return GenerateNode(new[] { value }, context);
        }

        //返回包含byte的结点,解析为10进制
        protected Node GenerateByteNode(byte value)
        {
            return GenerateNode(value, value.ToString());
        }

        //返回包含Sbyte的结点
        protected Node GenerateSByteNode(sbyte value)
        {
            return GenerateNode((byte)value, value.ToString());
        }
    }
    public class Node
    {
        public string value;
        public string context;
    }
}
