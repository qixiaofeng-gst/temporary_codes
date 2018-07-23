using System;
using System.Collections;

namespace PDPU
{
    public class BitWriter
    {
        private int m_pos = 0;
        public BitArray bit_data = null;

        public BitWriter(int len)
        {
            bit_data = new BitArray(len * 8);
        }

       
        public void Write(int data, int bitSize)
        {
            //将传入数据转换成二进制位
            int[] value = new int[1] { data };
            BitArray bit_temp = new BitArray(value);

            for (int i = 0; i < bitSize; i++)
            {
                bit_data[m_pos + i] = bit_temp[bitSize - i - 1];
            }

            m_pos += bitSize;
        }

        public byte[] GetData()
        {
            return GetData(bit_data);
        }

        public byte[] GetData(int pos, int size)
        {
            int byteLen = size / 8;
            if (size % 8 != 0)
            {
                byteLen += 1;
            }

            BitArray tmpBitArray = new BitArray(byteLen * 8);
            tmpBitArray.SetAll(false);

            //i:tmpBitArray索引,pos+j：bit_data索引
            for (int i = byteLen * 8 - size, j = 0; i < byteLen * 8; i++, j++)
            {
                tmpBitArray[i] = bit_data[pos + j];
            }

            return GetData(tmpBitArray);
        }

        private byte[] GetData(BitArray bitArray)
        {
            if (bitArray == null || bitArray.Length == 0 || bitArray.Length % 8 != 0)
            {
                return null;
            }

            byte[] data = new byte[bitArray.Length / 8];
            Array.Clear(data, 0, data.Length);

            for (int i = 0, y = 0; i < bitArray.Length / 8; i++, y += 8)
            {
                //data[i] = 0;

                if (bitArray[y])
                    data[i] |= (byte)(1 << 7);

                if (bitArray[y + 1])
                    data[i] |= (byte)(1 << 6);

                if (bitArray[y + 2])
                    data[i] |= (byte)(1 << 5);

                if (bitArray[y + 3])
                    data[i] |= (byte)(1 << 4);

                if (bitArray[y + 4])
                    data[i] |= (byte)(1 << 3);

                if (bitArray[y + 5])
                    data[i] |= (byte)(1 << 2);

                if (bitArray[y + 6])
                    data[i] |= (byte)(1 << 1);

                if (bitArray[y + 7])
                    data[i] |= (byte)(1);
            }

            return data;
        }
    }
}
