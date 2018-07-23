using PDPU;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TC
{
    public partial class TCFrm_Main : Frm_Main
    {
        private ImageFrm blueFrm;
        private ImageFrm redFrm;
        private TelemetryForm telemetryFrm;

        public TCFrm_Main()
        {
            InitializeComponent();
        }

        private void Frm_load(object sender, EventArgs e)
        {
            blueFrm = new ImageFrm();
            redFrm = new ImageFrm();
            blueFrm.Text = "蓝波段图像";
            redFrm.Text = "红波段图像";
            redFrm.Show(dockPanel1);
            redFrm.DockTo(dockPanel1, DockStyle.Fill);
            blueFrm.Show(dockPanel1);
            blueFrm.DockTo(dockPanel1, DockStyle.Fill);

            telemetryFrm = new TelemetryForm();
            telemetryFrm.Text = "帧头信息";
            telemetryFrm.Show(dockPanel1);
            telemetryFrm.DockTo(dockPanel1, DockStyle.Bottom);

            ControlFrm ctrlFrm = new ControlFrm();
            ctrlFrm.Show(dockPanel1, DockState.DockRightAutoHide);
            ctrlFrm.SetCypress(new TCCypress(ctrlFrm.cypressControl1));
        }

        public override void SetFullScreen(bool setFull)
        {
            if (setFull)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void Frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("将要关闭窗体，是否继续？", "询问", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        public override void UpdateImage(List<MyImage> list)
        {
            int imageNum = list.Count;
            if (imageNum != 2) //不是2
            {
                return;
            }

            MyImage[] myImages = new MyImage[imageNum];
            Bitmap[] bitmaps = new Bitmap[imageNum];

            for (int i = 0; i < imageNum; i++)
            {
                myImages[i] = list[i];
            }
            redFrm.SetImage(myImages[0]);
            blueFrm.SetImage(myImages[1]);
        }

        public override void UpdateFrame(List<List<ParseNode>> imageInforList, List<MyImage> imageList)
        {
            UpdateFrameHead(imageInforList);
            UpdateImage(imageList);
        }

        public void UpdateFrameHead(List<List<ParseNode>> list)
        {
            if (list == null || list.Count != 2)
            {
                return;
            }
            //红波段和蓝波段帧头信息合并
            List<ParseNode> inforList = new List<ParseNode>();
            for (int i = 0; i < list.Count; i++)
            {
                foreach (var node in list[i])
                {
                    if (i == 0)
                    {
                        node.Name = "红波段" + node.Name;
                    }
                    else if (i == 1)
                    {
                        node.Name = "蓝波段" + node.Name;
                    }
                    inforList.Add(node);
                }
            }
            telemetryFrm.SetTelemetry(inforList);
        }
    }
}
