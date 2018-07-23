﻿using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PDPU
{
    [Serializable]
    public class ParseNode : ICloneable, INotifyPropertyChanged
    {
        private FrameNodeModel model;
        
        private byte[] datas;
        private string name;
        private string value;
        private string binaryValue;
        private string hexValue;


        public byte[] GetDatas()
        {
            return datas;
        }

        public string Name
        {
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
            get { return name; }
        }
        public string Value
        {
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
            get { return value; }
        }
        
        public string HexValue
        {
            set
            {
                if (hexValue != value)
                {
                    hexValue = value;
                    NotifyPropertyChanged("HexValue");
                }
            }
            get { return hexValue; }
        }
       
				public string BinaryValue
        {
            set
            {
                if (binaryValue != value)
                {
                    binaryValue = value;
                    NotifyPropertyChanged("BinaryValue");
                }
            }
            get { return binaryValue; }
        }
        
        public ParseNode(FrameNodeModel model, byte[]nodeBytes)
        {
           // subTypeList = new List<SubType>();
            this.model = model;
            datas = nodeBytes;
            name = model.Name;
            value = "";
            binaryValue = "";
            hexValue = "0X" + MyConvert.GetStrFromHexBytes(nodeBytes).ToUpper();
            foreach (var hex in nodeBytes)
            {
                binaryValue += GenerateBinaryFromHex(hex);
            }
        }

        private string GenerateBinaryFromHex(byte hex)
        {
            return Convert.ToString(hex, 2).PadLeft(8, '0');
            
        }

        public void Parse()
        {
            if (model == null || datas == null||
                model.Len != datas.Length)
            {
                
                return;
            }
            if (model.TypeAndRule.SubTypeList != null && model.TypeAndRule.SubTypeList.Count != 0)
            {
                for (int i = 0; i < model.TypeAndRule.SubTypeList.Count; i++)
                {
                    TypeAndRule.SubType subType = model.TypeAndRule.SubTypeList[i];
                    switch (subType.type)
                    {
                        case DataType.str:
                        {
                            if (model.TypeAndRule.StrRuleList != null 
                                && model.TypeAndRule.StrRuleList.Count > 0)
                            {
                                foreach (var rule in model.TypeAndRule.StrRuleList)
                                {
                                    if (rule.ruleHashtable != null && rule.ruleHashtable.Count != 0)
                                    {
                                        //根据关键字bit位置，长度和哈希表值解析
                                        int pos = rule.bitsPos;
                                        int len = rule.bitsLen;
                                        BitWriter bitWriter = new BitWriter(datas.Length);
                                        foreach (var data in datas)
                                        {
                                            bitWriter.Write(data, 8);
                                        }
                                        byte[] hexBytes = bitWriter.GetData(pos, len);
                                        if (hexBytes == null)
                                        {
                                            return;
                                        }
                                        byte[] tmpBytes = new byte[4];
                                        Array.Clear(tmpBytes, 0, tmpBytes.Length);
                                        int assignLen = hexBytes.Length >= tmpBytes.Length
                                            ? tmpBytes.Length
                                            : hexBytes.Length;
                                        for (int j = 0; j < assignLen; j++)
                                        {
                                            tmpBytes[tmpBytes.Length -1 - j] = hexBytes[hexBytes.Length -1 - j];
                                        }
                                        int key = MyConvert.GetInt32FromHexBytes(tmpBytes);
                                        Hashtable hashtable = rule.ruleHashtable;
                                        value += hashtable[key];
                                        value += " ";
                                    }
                                    else
                                    {
                                        value += MyConvert.GetStrFromHexBytes(datas);
                                    }
                                }
                                
                            }
                            break;
                        }
                        case DataType.byte8:
                        {
                            value += datas[0];
                            break;
                        }
                        case DataType.int16:
                        {
                            byte[] tmp = new byte[2];
                            Array.Copy(datas, 0, tmp, 0, tmp.Length);
                            value += MyConvert.GetInt16FromHexBytes(tmp);
                            break;
                        }
                        case DataType.uint16:
                        {
                            byte[] tmp = new byte[2];
                            Array.Copy(datas, 0, tmp, 0, tmp.Length);
                            value += MyConvert.GetUInt16FromHexBytes(tmp);
                            break;
                        }
                        case DataType.int32:
                        {
                            byte[] tmp = new byte[4];
                            Array.Copy(datas, 0, tmp, 0, tmp.Length);
                            value += MyConvert.GetInt32FromHexBytes(tmp);
                            break;
                        }
                        case DataType.uint32:
                        {
                            byte[] tmp = new byte[4];
                            Array.Copy(datas, 0, tmp, 0, tmp.Length);
                            value += MyConvert.GetUInt32FromHexBytes(tmp);
                            break;
                        }
                    }
                }
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
    }
}
