using CyUSB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PDPU
{
    public class XferData
    {
        public byte[] data = null;
        public int length = 0;
    }

    class StatusData
    {
        public int successes;
        public int failures;
        public long xferRate;
    }

    public class Cypress
    {
        private byte[] testData = new byte[4102];
        private int testLen = 4012;
        private AutoResetEvent testEvent = new AutoResetEvent(false);

        public CyUSBDevice myDevice;
        public CyUSBEndPoint endPoint;
        public List<string> endPointList = null;

        protected CypressControl cypressControl = null;
        private USBDeviceList usbDevices;

        protected Configure configure;

        //统计传输速率
        private DateTime t1;

        //传输设置和返回状态
        private int bufSize = 4096;
        private int queueSize = 128;
        private int PPX = 8;
        private int IsoPktBlockSize;

        private double xferBytes;

        //接收和处理线程
        private Thread tListen;
        private Thread tParse;
        private Thread tStatusUpdate;
        protected bool bRunning;
        private AutoResetEvent statusEvent = new AutoResetEvent(false);
        protected bool isLog = false;
        protected bool isCrcChecked = false;
        protected bool isAutoSaved = false;

        //数据缓存 线程共有变量
        volatile protected XferData xferData = new XferData();
        volatile protected List<byte> bufferList = new List<byte>(15 * 1024 * 1024);//15MB
        volatile private StatusData statusData = new StatusData();

        // These are needed to close the app from the Thread exception(exception handling)
        private delegate void ExceptionCallback();
        private ExceptionCallback handleException;

        public Cypress(CypressControl cypressControl)
        {
            configure = new Configure();
            configure.Load();
            this.cypressControl = cypressControl;
            // Create the list of USB devices attached to the CyUSB.sys driver.
            usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);

            //Assign event handlers for device attachment and device removal.
            usbDevices.DeviceAttached += new EventHandler(usbDevices_DeviceAttached);
            usbDevices.DeviceRemoved += new EventHandler(usbDevices_DeviceRemoved);

            // Setup the callback routine for NullReference exception handling
            handleException = new ExceptionCallback(ThreadException);
        }

        ~Cypress()
        {
            if (tListen != null)
                StopWork();
            if (usbDevices != null)
                usbDevices.Dispose();
            if (myDevice != null)
            {
                myDevice.Dispose();
            }

            tListen = null;
            myDevice = null;
            endPoint = null;
        }

        /*
         * Summary
         * This is the event handler for device removal. This method resets the device count and searches for the device with 
         * VID-PID 04b4-1004
         */
        void usbDevices_DeviceRemoved(object sender, EventArgs e)
        {
            myDevice.Dispose();
            myDevice = null;
            endPoint = null;
            SetDevice();
        }

        /*
         * Summary
         * This is the event handler for device attachment. This method  searches for the device with \
         * VID-PID 04b4-1004
         */
        void usbDevices_DeviceAttached(object sender, EventArgs e)
        {
            SetDevice();
        }

        /*
         * Summary
         * Search the device with VID-PID 04b4-1004 and if found, select the end point
         */
        public void SetDevice()
        {
            USBDevice dev = usbDevices[0] as CyUSBDevice;
            if (dev != null)
            {
                myDevice = (CyUSBDevice)dev;
                endPointList = GetEndpointsOfNode(myDevice.Tree);
            }

            if (null != cypressControl)
            {
                cypressControl.SetDevice(endPointList);
            }
        }

        /*
         * Summary
         * Recursive routine populates EndPointsComboBox with strings  
         * representing all the endpoints in the device.
         */
        public List<string> GetEndpointsOfNode(TreeNode devTree)
        {
            List<string> list = new List<string>();
            foreach (TreeNode node in devTree.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    List<string> childList = GetEndpointsOfNode(node);
                    foreach (string str in childList)
                    {
                        list.Add(str);
                    }
                }
                else
                {
                    CyUSBEndPoint ept = node.Tag as CyUSBEndPoint;
                    if (ept != null && !node.Text.Contains("Control"))
                    {
                        CyUSBInterface ifc = node.Parent.Tag as CyUSBInterface;
                        string s = string.Format("ALT-{0}, {1} Byte, {2}", ifc.bAlternateSetting, ept.MaxPktSize, node.Text);
                        list.Add(s);
                    }
                }
            }
            return list;
        }

        /*
         * Summary
         * Data Xfer Thread entry point. Starts the thread on Start Button click 
         */
        public unsafe void XferThread()
        {
            // Setup the queue buffers
            byte[][] cmdBufs = new byte[queueSize][];
            byte[][] xferBufs = new byte[queueSize][];
            byte[][] ovLaps = new byte[queueSize][];
            ISO_PKT_INFO[][] pktsInfo = new ISO_PKT_INFO[queueSize][];

            int xStart = 0;

            try
            {
                LockNLoad(ref xStart, cmdBufs, xferBufs, ovLaps, pktsInfo);
            }
            catch (NullReferenceException e)
            {
                // This exception gets thrown if the device is unplugged while we're streaming data
                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                if (null != cypressControl)
                {
                    cypressControl.Invoke(handleException);
                }
            }
        }

        /*
         * Summary
         * This is a recursive routine for pinning all the buffers used in the transfer in memory.
         * It will get recursively called queueSz times.  On the QueueSz_th call, it will call
         * XferData, which will loop, transferring data, until the stop button is clicked.
         * Then, the recursion will unwind.
         */
        public unsafe void LockNLoad(ref int j, byte[][] cBufs, byte[][] xBufs, byte[][] oLaps, ISO_PKT_INFO[][] pktsInfo)
        {
            // Allocate one set of buffers for the queue. Buffered IO method require user to allocate a buffer as a part of command buffer,
            // the BeginDataXfer does not allocated it. BeginDataXfer will copy the data from the main buffer to the allocated while initializing the commands.
            cBufs[j] = new byte[CyConst.SINGLE_XFER_LEN + IsoPktBlockSize + ((endPoint.XferMode == XMODE.BUFFERED) ? bufSize : 0)];
            xBufs[j] = new byte[bufSize];
            oLaps[j] = new byte[20];
            pktsInfo[j] = new ISO_PKT_INFO[PPX];

            fixed (byte* tL0 = oLaps[j], tc0 = cBufs[j], tb0 = xBufs[j])  // Pin the buffers in memory
            {
                OVERLAPPED* ovLapStatus = (OVERLAPPED*)tL0;
                ovLapStatus->hEvent = (IntPtr)PInvoke.CreateEvent(0, 0, 0, 0);

                // Pre-load the queue with a request
                int len = bufSize;
                endPoint.BeginDataXfer(ref cBufs[j], ref xBufs[j], ref len, ref oLaps[j]);

                j++;

                if (j < queueSize)
                {
                    LockNLoad(ref j, cBufs, xBufs, oLaps, pktsInfo);  // Recursive call to pin next buffers in memory
                }
                else
                {
                    XferData(cBufs, xBufs, oLaps, pktsInfo);          // All loaded. Let's go!
                }
            }
        }

        /*
         * Summary
         * Called at the end of recursive method, LockNLoad().
         * XferData() implements the infinite transfer loop
         */
        public unsafe void XferData(byte[][] cBufs, byte[][] xBufs, byte[][] oLaps, ISO_PKT_INFO[][] pktsInfo)
        {
            string direcorty = "Data";
            Log sourceData = null;
            if (isLog)
            {
                sourceData = new Log(direcorty, "sourceData" + "_" + MyString.GetSystemTimeStamp() + ".dat", false, false, null);
            }
            int k = 0;
            int len = bufSize;

            int successes = 0;
            int failures = 0;
            xferBytes = 0;
            t1 = DateTime.Now;

            while (bRunning)
            {
                fixed (byte* tmpOvlap = oLaps[k])
                {
                    OVERLAPPED* ovLapStatus = (OVERLAPPED*)tmpOvlap;
                    if (!endPoint.WaitForXfer(ovLapStatus->hEvent, CyConst.INFINITE))
                    {
                        endPoint.Abort();
                        PInvoke.WaitForSingleObject(ovLapStatus->hEvent, CyConst.INFINITE);
                    }
                }

                if (endPoint.Attributes != 1)
                {
                    if (endPoint.FinishDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k]))
                    {
                        lock (bufferList)
                        {
                            if (bufferList != null && xBufs[k] != null &&
                                xBufs[k].Length != 0 && len != 0 && xBufs[k].Length >= len)
                            {
                                byte[] tmp = new byte[len];
                                Buffer.BlockCopy(xBufs[k], 0, tmp, 0, tmp.Length);
                                bufferList.AddRange(tmp);
                            }
                        }
                        xferBytes += len;
                        successes++;
                        if (isLog)
                        {
                            try
                            {
                                if (sourceData != null)
                                    sourceData.Write(xBufs[k], 0, len);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                            }
                        }
                    }
                    else
                    {
                        failures++;
                    }
                }

                // Re-submit this buffer into the queue
                len = bufSize;
                endPoint.BeginDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k]);

                k++;
                if (k == queueSize)  // Only update displayed stats once each time through the queue
                {
                    k = 0;
                    long xferRate = CalculateXferRate();

                    lock (statusData)
                    {
                        statusData.successes = successes;
                        statusData.failures = failures;
                        statusData.xferRate = xferRate;
                    }
                    statusEvent.Set();

                    // For small queueSz or PPX, the loop is too tight for UI thread to ever get service.   
                    // Without this, app hangs in those scenarios.
                    //Thread.Sleep(1);
                }

            } // End infinite loop

            if (endPoint != null)
            {
                endPoint.Abort();
            }

            if (sourceData != null)
            {
                sourceData.CloseStream();
            }

            statusEvent.Set();
        }

        public virtual unsafe void ParseThread()
        {

        }

        public void StatusUpdateThread()
        {
            long xferRate = 0;
            int successes = 0;
            int failures = 0;
            while (bRunning)
            {
                statusEvent.WaitOne();
                lock (statusData)
                {
                    xferRate = statusData.xferRate;
                    successes = statusData.successes;
                    failures = statusData.failures;
                }
                // Call StatusUpdate() in the main thread
                try
                {
                    if (null != cypressControl)
                    {
                        cypressControl.Invoke(cypressControl.updateUI, xferRate, successes, failures);
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
                }

            }
        }

        public void StartWork()
        {
            bRunning = true;

            tListen = new Thread(new ThreadStart(XferThread));
            tListen.IsBackground = true;
            tListen.Priority = ThreadPriority.Highest;
            tListen.Start();

            tParse = new Thread(new ThreadStart(ParseThread));
            tParse.IsBackground = true;
            tParse.Priority = ThreadPriority.Highest;
            tParse.Start();

            tStatusUpdate = new Thread(new ThreadStart(StatusUpdateThread));
            tStatusUpdate.IsBackground = true;
            tStatusUpdate.Priority = ThreadPriority.Highest;
            tStatusUpdate.Start();
        }

        public void StopWork()
        {
            if (tListen != null && tListen.IsAlive)
            {

                bRunning = false;
                tListen = null;

                tParse = null;

                tStatusUpdate = null;
                if (endPoint != null)
                {
                    endPoint.Abort();
                }
            }
        }

        /*
         * Summary
         * The callback routine delegated to handleException.
         */
        public void ThreadException()
        {
            StopWork();
            if (tListen.IsAlive && null != cypressControl)
            {
                cypressControl.ThreadException();
            }
        }

        public long CalculateXferRate()
        {
            DateTime t2 = DateTime.Now;
            TimeSpan elapsed = t2 - t1;
            long rate = (long)(xferBytes / elapsed.TotalMilliseconds);
            rate = rate / (int)100 * (int)100;
            return rate;
        }

        public void SetLog(bool isLog)
        {
            this.isLog = isLog;
        }

        public void SetCrcCheck(bool isCrcChecked)
        {
            this.isCrcChecked = isCrcChecked;
        }

        public void SetAutoSave(bool isAutoSaved)
        {
            this.isAutoSaved = isAutoSaved;
        }

        public void SetXferBufferSize(int bufSize, int queueSize, int PPX, int IsoPktBlockSize)
        {
            this.bufSize = bufSize;
            this.queueSize = queueSize;
            this.PPX = PPX;
            this.IsoPktBlockSize = IsoPktBlockSize;

            endPoint.XferSize = bufSize;
        }

        private void SaveFile(bool isRaw)
        {
            string prefix = isRaw ? "RAW_" : "JPEG_";
            string suffix = isRaw ? ".raw" : ".jpg";
            string imageName = prefix + DateTime.Now.ToString() + "_" + DateTime.Now.Millisecond.ToString() + suffix;
            imageName = imageName.Replace(' ', '_');
            int index = 0;
            while (index != -1)
            {
                index = imageName.LastIndexOfAny(new char[] { '/', ':' });
                if (index != -1)
                {
                    imageName = imageName.Remove(index, 1);

                }
            }
            FileStream imageStream = File.OpenWrite(imageName);
            imageStream.WriteByte(255);
            imageStream.Close();
        }

        public unsafe void GenLineDataThread()
        {
            while (bRunning)
            {
                lock (testData)
                {
                    GenLineData(ref testData, ref testLen);
                }
                testEvent.WaitOne();
            }
        }

        private static ushort lineNum = 0;
        private static ushort frameNum = 0;
        //测试函数,生成一行数据
        private bool GenLineData(ref byte[] data, ref int len)
        {
            const int width = 4096;
            int height = 1024;
            len = width + 6;

            byte[] lineHead = new byte[]
            {
                0xeb, 0x90, 0x00, 0x00
            };
            lineHead[2] = (byte)(lineNum >> 8);
            lineHead[3] = (byte)lineNum;

            byte[] lineData = new byte[width];

            byte[] lineEnd = new byte[]
            {
                0x00, 0x00
            };

            ushort value = 0;
            //红波段
            for (int i = 0; i < width / 2; i++)
            {
                if (i % 2 == 0)
                {
                    value = (ushort)(i / 2);
                    lineData[i] = (byte)(value >> 8);
                }
                else
                {
                    lineData[i] = (byte)value;
                }
            }
            value = 0;
            //蓝波段
            for (int i = width / 2; i < width; i++)
            {
                if (i % 2 == 0)
                {
                    value = (ushort)(1023 - (i - width / 2) / 2);
                    lineData[i] = (byte)(value >> 8);
                }
                else
                {
                    lineData[i] = (byte)value;
                }
            }

            if (lineNum == 0)
            {
                for (int i = 0; i < width; i++)
                {
                    lineData[i] = 0;
                }

                lineData[0] = 0x49;
                lineData[1] = 0x54;
                lineData[2] = 0xce;
                lineData[3] = 0x1f;
                lineData[4] = 0x06;
                lineData[5] = 0x6b;

                lineData[6] = 0x0c;

                lineData[7] = 0xcc;

                lineData[8] = 0x11;
                lineData[9] = 0x12;
                lineData[10] = 0x13;
                lineData[11] = 0x14;

                lineData[12] = 0x21;
                lineData[13] = 0x22;

                lineData[14] = 0x31;
                lineData[15] = 0x32;
                lineData[16] = 0x33;
                lineData[17] = 0x34;
                lineData[18] = 0x35;
                lineData[19] = 0x36;

                lineData[20] = 0xc0;
                lineData[21] = 0xc0;
                lineData[22] = 0x03;
                lineData[23] = 0x01;

                lineData[24] = 0x41;
                lineData[25] = 0x42;

                lineData[26] = 0x51;
                lineData[27] = 0x52;

                lineData[72] = (byte)(frameNum >> 8);
                lineData[73] = (byte)frameNum;
            }

            if (lineNum == height)
            {
                lineNum = 0;
                frameNum++;
            }
            else
            {
                lineNum = (ushort)(lineNum + 1);
            }

            Array.Copy(lineHead, 0, data, 0, lineHead.Length);
            Array.Copy(lineData, 0, data, lineHead.Length, lineData.Length);
            Array.Copy(lineEnd, 0, data, lineHead.Length + lineData.Length, lineEnd.Length);
            return true;
        }
    }
}
