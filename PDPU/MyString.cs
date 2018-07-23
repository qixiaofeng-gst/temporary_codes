using System;
using System.Collections.Generic;

namespace PDPU
{
    public class MyString
    {
        public static int IndexOfHexHead(List<byte> byteList, string head)
        {
            if (byteList != null && byteList.Count != 0 && head != null && head.Length != 0)
            {
                byte[] arrayHead = MyConvert.GetHexBytesFromStr(head);
                if (arrayHead != null && arrayHead.Length != 0)
                {
                    int position = 0;

                    while (position <= byteList.Count - arrayHead.Length)
                    {
                        position = byteList.IndexOf(arrayHead[0], position);
                       
                        if (position != -1)
                        {
                            for (int i = 1; i < arrayHead.Length && position + arrayHead.Length <= byteList.Count; i++)
                            {
                                if (byteList[position + i] != arrayHead[i])
                                {
                                    position += i;  //kmp算法简单情况 指针移动距离 = 已匹配字符数 - 0
                                    break;
                                }
                                if (i == arrayHead.Length - 1)
                                {
                                    return position;
                                }
                            }
                        }
                        else
                        {
                            return -1; //未搜索到第一个帧头字节
                        }
                    }
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        public static string GetSystemTimeStamp()
        {
            string time = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "s_" + DateTime.Now.Millisecond.ToString("D3") + "ms";
            return time;
        }
    }
}
