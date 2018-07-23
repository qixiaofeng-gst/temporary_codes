using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyUSB;

namespace Dumper
{
    class DataDumper : PDPU.Cypress {

        public const string name_prefix = "raw_packets_";

        private FileStream _fs = null;
        private ulong dumped_length = 0;

        public DataDumper() : base(null) {
            SetDevice();

            string default_end_point = endPointList[2];
            Console.WriteLine("The end point to be used:" + default_end_point);
            int aX = default_end_point.LastIndexOf("0x");
            string sAddr = default_end_point.Substring(aX, 4);
            byte addr = (byte)Util.HexToInt(sAddr);
            endPoint = myDevice.EndPointOf(addr);

            _fs = new FileStream(GetDumpFileName(), FileMode.Append);
        }

        public override unsafe void ParseThread()
        {
            while (bRunning) {
                lock (bufferList)
                {
                    WriteXferData();
                }
            }

            _fs.Close();
        }

        private string GetDumpFileName()
        {
            for (int sn = 1; ; ++sn)
            {
                string cand_name = name_prefix + sn;
                if (File.Exists(cand_name))
                {
                    continue;
                }
                return cand_name;
            }
        }

        private void WriteInt(int i)
        {
            for (int j = 3; j >= 0; --j)
            {
                _fs.WriteByte((byte)(i >> (8 * j)));
            }
        }

        private void WriteBytes(byte[] bytes, int l)
        {
            _fs.Write(bytes, 0, l);
        }

        private void WriteXferData()
        {
            if (bufferList.Count > 0) {
                int length = bufferList.Count;
                dumped_length += ((ulong)length);
                WriteInt(length);
                WriteBytes(bufferList.ToArray(), length);
                Console.WriteLine("[" + length + " bytes] written. Totally " + dumped_length + " bytes (" + (dumped_length / (1024 * 1024)) + " MB) written.");
                bufferList.Clear();
            }
        }

        public void Test()
        {
            Console.WriteLine("====");
            _fs.Close();
            _fs = new FileStream("raw_packets_1", FileMode.Open);
            byte[] i = new byte[4];
            int readed = _fs.Read(i, 0, 4);
            int length = (i[0] << 24) + (i[1] << 16) + (i[2] << 8) + i[3];
            Console.WriteLine("readed:" + readed + ", length:" + length);
            _fs.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DataDumper dd = new DataDumper();
            dd.StartWork();
            Console.WriteLine("Press any key to stop.");
            int input = Console.Read();
            dd.StopWork();
            Console.WriteLine("Dumper stopped. Press any key to exit.");
            Console.Read();
        }
    }
}
