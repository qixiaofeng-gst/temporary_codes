using PDPU;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DX
{
    public partial class DXFrm_Main : Frm_Main
    {
        private int preWindowsNum = 4;
        private const int maxWindowsNum = 4; //导星开窗最大个数
        private ImageFrm[] dxFrm0 = null;
        private ImageFrm[] dxFrm1 = null;

        public DXFrm_Main()
        {

            InitializeComponent();
        }

        private void Frm_load(object sender, EventArgs e)
        {
            dxFrm0 = new ImageFrm[maxWindowsNum];
            dxFrm1 = new ImageFrm[maxWindowsNum];
            for (int i = 0; i < dxFrm0.Length; i++)
            {
                dxFrm0[i] = new ImageFrm();
                dxFrm1[i] = new ImageFrm();
            }


            for (int i = 0; i < dxFrm0.Length; i++)
            {
                dxFrm0[i].Text = "导星1" + (i + 1);
            }
            for (int i = 0; i < dxFrm1.Length; i++)
            {
                dxFrm1[i].Text = "导星2" + (i + 1);
            }

            dxFrm0[0].Show(dockPanel1);
            dxFrm0[0].DockTo(dockPanel1, DockStyle.Fill);
            dxFrm1[0].Show(dockPanel1);
            dxFrm1[0].DockTo(dockPanel1, DockStyle.Fill);

            ControlFrm ctrlFrm = new ControlFrm();
            ctrlFrm.Show(dockPanel1, DockState.DockRightAutoHide);
            ctrlFrm.SetCypress(new DXCypress(ctrlFrm.cypressControl1));

            dxFrm0[1].Show(dxFrm0[0].Pane, DockAlignment.Right, 0.5);
            dxFrm0[2].Show(dxFrm0[0].Pane, DockAlignment.Bottom, 0.5);
            dxFrm0[3].Show(dxFrm0[1].Pane, DockAlignment.Bottom, 0.5);

            dxFrm1[1].Show(dxFrm1[0].Pane, DockAlignment.Right, 0.5);
            dxFrm1[2].Show(dxFrm1[0].Pane, DockAlignment.Bottom, 0.5);
            dxFrm1[3].Show(dxFrm1[1].Pane, DockAlignment.Bottom, 0.5);
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

        /*
         * 图像顺序为导星1，导星2，导星1，导星2
         * 
         * */
        public override void UpdateImage(List<MyImage> list)
        {
            int imageNum = list.Count;
            if (imageNum % 2 != 0) //不是2的整数倍
            {
                imageNum -= 1;
            }

            MyImage[] myImages = new MyImage[imageNum];
            Bitmap[] bitmaps = new Bitmap[imageNum];

            for (int i = 0; i < imageNum; i++)
            {
                myImages[i] = list[i];
            }

            for (int i = 0; i < imageNum / 2; i++)
            {
                dxFrm0[i].SetImage(myImages[2 * i]);
                dxFrm1[i].SetImage(myImages[2 * i + 1]);

            }

            if (preWindowsNum != imageNum / 2)
            {
                for (int i = 0; i < dxFrm0.Length; i++)
                {
                    dxFrm0[i].Hide();
                    dxFrm1[i].Hide();
                }

                for (int i = 0; i < imageNum / 2; i++)
                {
                    dxFrm0[i].Show();
                    dxFrm1[i].Show();
                }
            }
            preWindowsNum = imageNum / 2;
        }

    }
}