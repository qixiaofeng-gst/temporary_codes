using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace PDPU
{
    /**
     * 多字节字段为高字节在前，低字节在后，即大端格式
     */
    public class MyConvert
    {
        public static byte[] GetHexBytesFromStr(string hexString)
        {
            if (!string.IsNullOrEmpty(hexString) && hexString.Length % 2 == 0)
            {
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                {
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                return returnBytes;
            }
            else
            {
                return null;
            }
        }

        public static string GetStrFromHexBytes(byte[] hexBytes)
        {
            string returnString = "";
            if (hexBytes != null)
            {
                for (int i = 0; i < hexBytes.Length; i++)
                {
                    returnString += hexBytes[i].ToString("X2");
                }
            }
            return returnString;
        }

        public static ushort GetUInt16FromHexBytes(byte[] hexBytes)
        {
            ushort returnUInt16 = 0;
            if (hexBytes != null && hexBytes.Length == 2)
            {
                returnUInt16 |= hexBytes[0];
                returnUInt16  <<= 8;
                returnUInt16 |= hexBytes[1];
            }
            return returnUInt16;
        }

        public static ushort GetInt16FromHexBytes(byte[] hexBytes)
        {
            ushort returnInt16 = 0;
            if (hexBytes != null && hexBytes.Length == 2)
            {
                returnInt16 |= hexBytes[0];
                returnInt16 <<= 8;
                returnInt16 |= hexBytes[1];
            }
            return returnInt16;
        }

        public static uint GetUInt32FromHexBytes(byte[] hexBytes)
        {
            uint returnUInt32 = 0;
            if (hexBytes != null && hexBytes.Length == 4)
            {
                for (int i = 0; i < hexBytes.Length - 1; i++)
                {
                    returnUInt32 |= hexBytes[i];
                    returnUInt32 <<= 8;
                }
                returnUInt32 |= hexBytes[hexBytes.Length - 1];
            }
            return returnUInt32;
        }

        public static int GetInt32FromHexBytes(byte[] hexBytes)
        {
            int returnInt32 = 0;
            if (hexBytes != null && hexBytes.Length == 4)
            {
                for (int i = 0; i < hexBytes.Length - 1; i++)
                {
                    returnInt32 |= hexBytes[i];
                    returnInt32 <<= 8;
                }
                returnInt32 |= hexBytes[hexBytes.Length - 1];
            }
            return returnInt32;
        }

        public static float GetFloatFromHexBytes(byte[] hexBytes)
        {
            float returnFloat = 0;
            if (hexBytes != null && hexBytes.Length == 4)
            {
                byte[]tmpBytes = new byte[hexBytes.Length];
                for (int i = 0; i < hexBytes.Length; i++)
                {
                    tmpBytes[i] = hexBytes[hexBytes.Length - 1 - i];
                }
                try
                {
                    returnFloat = BitConverter.ToSingle(tmpBytes, 0);
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                }
                
            }
            return returnFloat;
        }

        public static byte[] BigLittleEndianConvert(byte[] data)
        {
            byte[] convertData = null;
            if (data != null && data.Length != 0 && data.Length % 2 == 0)
            {
                convertData = new byte[data.Length];
                for (int i = 0; i < data.Length; i += 2)
                {
                    convertData[i] = data[i + 1];
                    convertData[i + 1] = data[i];
                }
            }
            return convertData;
        }

        public static T Clone<T>(T realObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, realObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                T copy = (T)formatter.Deserialize(objectStream);
                objectStream.Close();
                return copy;
            }
        }
    }
}
