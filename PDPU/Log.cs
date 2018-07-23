using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PDPU
{
    public class Log
    {
        FileStream fstream;
        StreamWriter sw ;
        private bool isText;
        private bool isAppend;
        private Encoding encoding;
        private string direcorty;

        public  Log(string direcorty, string name, bool isText, bool isAppend, Encoding encoding)
        {
            this.fstream = null;
            this.sw = null;
            this.isText = isText;
            this.isAppend = isAppend;
            this.encoding = encoding;
            this.direcorty = "";

            SetDirecorty(direcorty);
            CreateStream(GenerateFilePath(name));
        }

        public Log(string name, bool isText, bool isAppend, Encoding encoding)
        {
            this.fstream = null;
            this.sw = null;
            this.isText = isText;
            this.isAppend = isAppend;
            this.encoding = encoding;
            this.direcorty = "";

            CreateStream(name);
        }

        public void CloseStream()
        {
            if (sw != null)
            {
                sw.Close();
            }
            if (fstream != null)
            {
                fstream.Close();
            }
        }

        private void SetDirecorty(string str)
        {
            if (str != null)
            {
                direcorty = str.Trim();
            }
            else
            {
                direcorty = "";
            }

            if (direcorty.Length != 0 && !Directory.Exists(direcorty))
            {
                Directory.CreateDirectory(direcorty);
            }
        }
        
        private string GenerateFilePath(string name)
        {
            return direcorty + "\\" + name;
        }

        private void CreateStream(string path)
        {
            try
            {
                fstream = new FileStream(path, isAppend ? FileMode.Append : FileMode.Create);
                if (isText)
                {
                    sw = new StreamWriter(fstream, Encoding.Unicode);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
            }
        }

        public void Write(byte[] array, int offset, int count)
        {
            if (!isText)
            {
                fstream.Write(array, offset, count);
            }
            else
            {
                throw new Exception("操作应为二进制写操作");
            }
        }

        public void WriteLine(string format, params object[] arg)
        {
            if (isText)
            {
                sw.WriteLine(format, arg);
                sw.Flush();
            }
            else
            {
                throw new Exception("操作应为文本写操作");
            }
        }
    }
}
