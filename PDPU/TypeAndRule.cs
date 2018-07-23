using System;
using System.Collections;
using System.Collections.Generic;

namespace PDPU
{
    public class TypeAndRule
    {
        public class SubType
        {
            public int bitsPos;
            public int bitsLen;
            public DataType type;
        }

        public class StrRule
        {
            public int bitsPos;
            public int bitsLen;
            public Hashtable ruleHashtable;
        }

        private FrameNodeModel model;
        
        private List<SubType> subTypeList;
        private List<StrRule> strRuleList;

        public List<SubType> SubTypeList
        {
            get { return subTypeList; }
        }

        public List<StrRule> StrRuleList
        {
            get { return strRuleList; }
        }

        public TypeAndRule(FrameNodeModel model)
        {
            this.model = model;
            Parse();
            
        }

        private List<SubType> ExtractDataType()
        {
            if (model == null || model.DataType == null)
            {

                return null;
            }
            string[] dataTypeArray = model.DataType.Trim().ToLower().Split('|');
            if (dataTypeArray.Length >= 1)
            {
                subTypeList = new List<SubType>();
                int pos = 0;
                for(int i = 0 ;i < dataTypeArray.Length; i++)
                {
                    string dataType = dataTypeArray[i];
                    string[] types = dataType.Trim().Split(';');
                    SubType subType = new SubType();
                    if (types.Length == 1)//只有1个类型
                    {
                        subType.bitsPos = pos;
                        subType.bitsLen = model.Len * 8;
                        subType.type = (DataType)Enum.Parse(typeof(DataType), types[0]);
                        subTypeList.Add(subType);
                    }
                    else if (types.Length == 2)
                    {
                        subType.bitsPos = pos;
                        subType.bitsLen = Convert.ToInt32(types[0]);
                        subType.type = (DataType)Enum.Parse(typeof(DataType), types[1]);
                        subTypeList.Add(subType);
                        pos += subType.bitsLen;
                    }

                }
            }
            return subTypeList;
        }

        private List<StrRule> ExtractStrRule()
        {
            if (model == null || subTypeList == null 
                || subTypeList[0].type != DataType.str)
            {
                
                return null;
            }
            string[] ruleArray = model.Rule.Trim().ToLower().Split('|');
            if (ruleArray.Length >= 1)
            {
                strRuleList = new List<StrRule>();
                int pos = 0;
                for (int i = 0; i < ruleArray.Length; i++)
                {
                    string rule = ruleArray[i];
                    string[] map = rule.Trim().Split(';');
                    StrRule strRule = new StrRule();
                    if (map.Length == 1)//只有1个类型
                    {
                        strRule.bitsPos = pos;
                        strRule.bitsLen = model.Len * 8;
                        strRule.ruleHashtable = CreatHashtable(map[0]);
                        strRuleList.Add(strRule);
                    }
                    else if (map.Length == 2)
                    {
                        strRule.bitsPos = pos;
                        strRule.bitsLen = Convert.ToInt32(map[0]);
                        strRule.ruleHashtable = CreatHashtable(map[1]);
                        strRuleList.Add(strRule);
                        pos += strRule.bitsLen;
                    }

                }
            }
            return strRuleList;
        }

        private Hashtable CreatHashtable(string rule)
        {
            Hashtable table = new Hashtable(); 
            string[] maps = rule.Trim().Split(',');
            foreach (var map in maps)
            {
                string[] keyValue = map.Trim().Split(':');
                if (keyValue.Length == 2)
                {
                    int key = MyConvert.GetInt32FromHexBytes(
                        MyConvert.GetHexBytesFromStr(keyValue[0].PadLeft(8, '0')));
                    table.Add(key, keyValue[1]);
                }
            }
            return table;
        }
        private void Parse()
        {
            if (model == null )
            {
                
                return;
            }
            subTypeList = ExtractDataType();
            if (subTypeList[0].type == DataType.str)
            {
                strRuleList = ExtractStrRule();
            }
        }

    }
}
