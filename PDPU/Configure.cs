using System;
using System.IO;
using System.Windows.Forms;

namespace PDPU
{
    
    public class Configure
    {
        private int fullWidth;//全帧大小
        private int fullHeight;//全帧大小
        private string frameHead;
        private string lineIdentify;
        private string cameraIdentify;
        private int frameHeadLine;
        private int lineHeadLen;
        private int lineEndLen;
        private int width;
        private int height;
        private int bits;
        private int lineIdentifyLen;
        private int cameraIdentifyLen;

        public string FrameHead
        {
            set { frameHead = value; }
            get { return frameHead; }
        }

        public string LineIdentify
        {
            set { lineIdentify = value; }
            get { return lineIdentify; }
        }

        public int CameraIdentifyLen
        {
            get { return cameraIdentifyLen; }
        }

        public int LineIdentifyLen
        {
            get { return lineIdentifyLen; }
        }

        public string CameraIdentify
        {
            set { cameraIdentify = value; }
            get { return cameraIdentify; }
        }

        public int FrameHeadLine
        {
            set { frameHeadLine = value; }
            get { return frameHeadLine; }
        }

        public int LineHeadLen
        {
            set { lineHeadLen = value; }
            get { return lineHeadLen; }
        }

        public int LineEndLen
        {
            set { lineEndLen = value; }
            get { return lineEndLen; }
        }

        public int Width
        {
            set { width = value; }
            get { return width; }
        }

        public int Height
        {
            set { height = value; }
            get { return height; }
        }

        public int FullWidth
        {
            get { return fullWidth; }
        }

        public int FullHeight
        {
            get { return fullHeight; }
        }

        public int Bits
        {
            set { bits = value; }
            get { return bits; }
        }

        public Configure()
        {
            frameHead = "4954ce1f066b";
            lineIdentify = "eb90";
            cameraIdentify = "0505";
            frameHeadLine = 1;
            lineHeadLen = 4;
            lineEndLen = 2;
            width = 1024;
            height = 1024;
            bits = 16;
            fullWidth = 2048;//全帧大小
            fullHeight = 2048;//全帧大小
            cameraIdentifyLen = 2;
            lineIdentifyLen = 2;
        }

        public void Load()
        {
            StreamReader streamReader = null;
            try
            {
                streamReader = new StreamReader("configure.txt");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                return;
            }
            ParseConfigure(streamReader.ReadToEnd());
            streamReader.Close();
        }

        private void ParseConfigure(string context)
        {
            context = context.ToLower();
            int startIndex = context.IndexOf("frame:");

            frameHead = getValue(context, startIndex, "frameHead");
            lineIdentify = getValue(context, startIndex, "lineIndentify");
            cameraIdentify = getValue(context, startIndex, "cameraIndentify");
            frameHeadLine = Convert.ToInt32(getValue(context, startIndex, "frameHeadLine"));
            lineHeadLen = Convert.ToInt32(getValue(context, startIndex, "lineHeadLen"));
            lineEndLen = Convert.ToInt32(getValue(context, startIndex, "lineEndLen"));
            
            bits = Convert.ToInt32(getValue(context, startIndex, "bits"));
            fullWidth = Convert.ToInt32(getValue(context, startIndex, "width")); ;//全帧大小
            fullHeight = Convert.ToInt32(getValue(context, startIndex, "height"));//全帧大小
            width = fullWidth;
            height = fullHeight;
            if (null != lineIdentify) {
                lineIdentifyLen = MyConvert.GetHexBytesFromStr(lineIdentify).Length;
            }
            string[] cameraIdentifyArray;
            if (null != cameraIdentify)
            {
                cameraIdentifyArray = cameraIdentify.Split(',');
                if (cameraIdentifyArray != null && cameraIdentifyArray.Length == 2)
                    cameraIdentifyLen = cameraIdentifyArray[0].Length / 2;
            }
        }

        private string getValue(string context, int startIndex, string key)
        {
            if (context == null || context.Length == 0 
                || startIndex < 0 || startIndex >= context.Length
                || key == null || key.Length == 0)
            {
                return null;
            }
            int index = context.IndexOf(key.ToLower(), startIndex);
            string value = null;
            if (index != -1)
            {
                int start = context.IndexOf("=", index) + 1;
                int end = context.IndexOf("\r\n", start);
                value = context.Substring(start, end - start).Trim();
            }
            return value;
        }
    }
}
