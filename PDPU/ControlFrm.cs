using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PDPU
{
    public partial class ControlFrm : DockContent
    {
        private BindingList<ShowField> showFieldList;

        public ControlFrm()
        {
            InitializeComponent();
        }
        private void InitializeDgv()
        {
            List<ShowField> showFieldList = new List<ShowField>();
            BindingList<ShowField> showValueList = new BindingList<ShowField>();
        }

        private void Frm_load(object sender, EventArgs e)
        {
            frameHeadDgv.Visible = false;
            showFieldList = new BindingList<ShowField>();
        }

        private void fullScreen_Click(object sender, EventArgs e)
        {
            bool setFull = false;
            if (fullScreenButton.Text == "全屏显示")
            {
                setFull = true;
                fullScreenButton.Text = "退出全屏";
            }
            else
            {
                setFull = false;
                fullScreenButton.Text = "全屏显示";
            }
            ((Frm_Main)ParentForm).SetFullScreen(setFull);
        }
        
        public void SetCypress(Cypress cypress)
        {
            this.cypressControl1.SetCypress(cypress);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int WM_KEYDOWN = 256;
            int WM_SYSKEYDOWN = 260;
            if (msg.Msg == WM_KEYDOWN | msg.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.Escape:
                    {
                        fullScreen_Click(this, null);
                        break;
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void UpdateImage(byte[] imageBytes, int width, int height)
        {
            ((Frm_Main)ParentForm).UpdateImage(imageBytes, width, height);
        }

        public void UpdateImage(byte[] image1, byte[] image2, int width, int height)
        {
            ((Frm_Main)ParentForm).UpdateImage(image1, image2, width, height);
        }

        public void UpdateImage(List<MyImage> imageList)
        {
            ((Frm_Main)ParentForm).UpdateImage(imageList);
        }

        public void UpdateStatus(long xferRate, int successes, int failures)
        {

            ((Frm_Main)ParentForm).UpdateStatus(xferRate, successes, failures);
        }

        
        public void UpdateFrameHead(List<TelemetryKeyWord> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            if (frameHeadDgv.DataSource == null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ShowField showField = list[i].showField;
                    if (showField != null)
                    {
                        showFieldList.Add(showField);
                    }
                    else
                    {
                        MessageBox.Show("遥测表初始化错误!");
                        return;
                    }
                }
                frameHeadDgv.DataSource = showFieldList;
            }
            else if (list.Count == showFieldList.Count)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    showFieldList[i] = list[i].showField;
                }
            }
        }

        public void UpdateFrame(List<TelemetryKeyWord> frameHead, List<MyImage> imageList)
        {
            UpdateFrameHead(frameHead);
            UpdateImage(imageList);
        }

        public void UpdateFrame(List<List<ParseNode>> imageInforList, List<MyImage> imageList)
        {
            ((Frm_Main)ParentForm).UpdateFrame(imageInforList, imageList);
        }

        private void frameHeadInforButton_Click(object sender, EventArgs e)
        {
            if (frameHeadInforButton.Text == "显示帧头")
            {
                frameHeadInforButton.Text = "隐藏帧头";
                frameHeadDgv.Visible = true;
            }
            else
            {
                frameHeadInforButton.Text = "显示帧头";
                frameHeadDgv.Visible = false;
            }
        }

        private int num = 1;
        private void testBtn_Click(object sender, EventArgs e)
        {
            num ++;
            List<TelemetryKeyWord> list = new List<TelemetryKeyWord>();
            for (int i = 0; i < 5; i++)
            {
                TelemetryKeyWord telemetryKeyWord = new TelemetryKeyWord();
                telemetryKeyWord.showField = new ShowField();
                telemetryKeyWord.showField.Name = num + "name" + i;
                telemetryKeyWord.showField.Value = i + "";
                telemetryKeyWord.showField.Context = num + "context" + i;
                list.Add(telemetryKeyWord);
            }
            UpdateFrameHead(list);
        }
    }
}
