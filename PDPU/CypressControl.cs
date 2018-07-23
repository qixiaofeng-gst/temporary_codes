using CyUSB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PDPU
{
    public partial class CypressControl : UserControl
    {
        private Cypress cypress;
        private CyUSBDevice myDevice;
        private CyUSBEndPoint endPoint;

        private long xferRate;
        
        // These are needed for Thread to update the UI
        public delegate void UpdateUICallback(long xferRate, int successes, int failures);
        public UpdateUICallback updateUI;

        public delegate void UpdataFrameCallback(List<List<ParseNode>> imageInforList, List<MyImage> imageList);
        public UpdataFrameCallback handFrameCallback;

        public CypressControl()
        {
            InitializeComponent();

            if (PpxBox.Items.Count > 3)
            {
                PpxBox.SelectedIndex = 3;
            }

            if (QueueBox.Items.Count > 5)
            {
                QueueBox.SelectedIndex = 5;
            }

            // Setup the callback routine for updating the UI
            updateUI = new UpdateUICallback(UpdateStatus);
            handFrameCallback = new UpdataFrameCallback(UpdateFrame);

        }

        public void SetCypress(Cypress cypress)
        {
            this.cypress = cypress;
            SetEndPointsComboBox(this.EndPointsComboBox, null);
            cypress.SetDevice();
        }
        /*
         * Summary
         * Search the device with VID-PID 04b4-1004 and if found, select the end point
         */
        public void SetDevice(List<string> deviceList)
        {
            myDevice = (CyUSBDevice) cypress.myDevice;
            
            if (deviceList != null && deviceList.Count != 0)
            {
                for (int i = 0; i < deviceList.Count; i++)
                {
                    EndPointsComboBox.Items.Add(deviceList[i]);
                }
               
                if (EndPointsComboBox.Items.Count > 2)
                {
                    EndPointsComboBox.SelectedIndex = 2;
                    StartBtn.Enabled = true;
                }
                else if (EndPointsComboBox.Items.Count > 0)
                {
                    EndPointsComboBox.SelectedIndex = 0;
                    StartBtn.Enabled = true;
                }
            }
            else
            {
                StartBtn.Enabled = false;
                EndPointsComboBox.Items.Clear();
                EndPointsComboBox.Text = "";
            }
        }


        /*
         * Summary
         * This is the System event handler.  
         * Enforces valid values for PPX(Packet per transfer)
         */
        private void PpxBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (endPoint == null) return;

            int ppx = Convert.ToUInt16(PpxBox.Text);
            int len = endPoint.MaxPktSize * ppx;

            int maxLen = 0x80000; // 64K
            if (len > maxLen)
            {
                ppx = maxLen / endPoint.MaxPktSize / 8 * 8;
                PpxBox.Text = ppx.ToString();
                MessageBox.Show("Maximum of 512KB per transfer.  Packets reduced.", "Invalid Packets per Xfer.");
            }

            if (myDevice != null && myDevice.bHighSpeed && (endPoint.Attributes == 1) && (ppx < 8))
            {
                PpxBox.Text = "8";
                MessageBox.Show("Minimum of 8 Packets per Xfer required for HS Isoc.", "Invalid Packets per Xfer.");
            }

        }

        /*
         * Summary
         * This is a system event handler, when the selected index changes(end point selection).
         */
        private void EndPointsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetEndPointsComboBox(sender, e);
        }

        private void SetEndPointsComboBox(object sender, EventArgs e)
        {
            if (EndPointsComboBox.Text.Length < 5)
            {
                return;
            }
            // Get the Alt setting
            string sAlt = EndPointsComboBox.Text.Substring(4, 1);
            byte a = Convert.ToByte(sAlt);
            myDevice.AltIntfc = a;

            // Get the endpoint
            int aX = EndPointsComboBox.Text.LastIndexOf("0x");
            string sAddr = EndPointsComboBox.Text.Substring(aX, 4);
            byte addr = (byte)Util.HexToInt(sAddr);

            endPoint = cypress.endPoint = myDevice.EndPointOf(addr);

            // Ensure valid PPX for this endpoint
            PpxBox_SelectedIndexChanged(sender, null);
        }


        /*
         * Summary
         * Executes on Start Button click 
         */
        private void StartBtn_Click(object sender, System.EventArgs e)
        {
            if (myDevice == null)
                return;

            if (StartBtn.Text.Equals("开始"))
            {
                SetStart();

                cypress.StartWork();
            }
            else
            {
                cypress.StopWork();
                SetStop();
            }
        }

        private void SetStart()
        {
            EndPointsComboBox.Enabled = false;
            StartBtn.Text = "停止";
            StartBtn.BackColor = Color.Pink;

            int bufSize = endPoint.MaxPktSize * Convert.ToUInt16(PpxBox.Text);
            int queueSize = Convert.ToUInt16(QueueBox.Text);
            int PPX = Convert.ToUInt16(PpxBox.Text);

            int IsoPktBlockSize = 0;
            if (endPoint is CyIsocEndPoint)
                IsoPktBlockSize = (endPoint as CyIsocEndPoint).GetPktBlockSize(bufSize);
            else
                IsoPktBlockSize = 0;

            cypress.SetXferBufferSize(bufSize, queueSize, PPX, IsoPktBlockSize);
            cypress.SetLog(logCheckBox.Checked);
            cypress.SetCrcCheck(crcCheckBox.Checked);
            logCheckBox.Enabled = false;
            crcCheckBox.Enabled = false;
            PpxBox.Enabled = false;
            QueueBox.Enabled = false;
            autoSaveBtn.Enabled = logCheckBox.Checked;
        }

        private void SetStop()
        {
            logCheckBox.Enabled = true;
            crcCheckBox.Enabled = true;
            EndPointsComboBox.Enabled = true;
            StartBtn.Text = "开始";
            SetXferRate(cypress.CalculateXferRate());
            StartBtn.BackColor = Color.Aquamarine;

            PpxBox.Enabled = true;
            QueueBox.Enabled = true;
            autoSaveBtn.Enabled = true;
        }

        /*Summary
          The callback routine delegated to updateUI.
        */
        public void UpdateStatus(long xferRate, int successes, int failures)
        {
            
            ((ControlFrm)ParentForm).UpdateStatus(xferRate, successes, failures);
        }

        /*Summary
          The callback routine delegated to handleException.
        */
        public void ThreadException()
        {
            SetStop();
        }

        public void SetXferRate(long rate)
        {
            xferRate = rate;
        }

        public void UpdateImage(byte[] imageBytes, int width, int height)
        {
            ((ControlFrm)ParentForm).UpdateImage(imageBytes, width, height);
        }

        public void UpdateImage(byte[] image1, byte[] image2, int width, int height)
        {
            ((ControlFrm)ParentForm).UpdateImage(image1, image2, width, height);
        }

        public void UpdateFrame(List<TelemetryKeyWord> frameHead, List<MyImage> imageList)
        {
            ((ControlFrm)ParentForm).UpdateFrame(frameHead, imageList);
        }

        public void UpdateFrame(List<List<ParseNode>> imageInforList, List<MyImage> imageList)
        {
            ((ControlFrm)ParentForm).UpdateFrame(imageInforList, imageList);
        }

        private void autoSaveBtn_Click(object sender, EventArgs e)
        {
            if (autoSaveBtn.Text == "自动存图")
            {
                autoSaveBtn.Text = "停止存图";
                cypress.SetAutoSave(true);
            }
            else
            {
                autoSaveBtn.Text = "自动存图";
                cypress.SetAutoSave(false);
            }
        }

        private void CypressControl_Load(object sender, EventArgs e)
        {
        }
    }
}
