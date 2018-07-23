using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PDPU
{
    public partial class Frm_Main : Form
    {
        public Frm_Main()
        {
            InitializeComponent();
            
        }

        public virtual void SetFullScreen(bool setFull)
        {
        }

        public void UpdateImage(byte[] imageBytes, int width, int height)
        {
        }

        public void UpdateImage(byte[] image1, byte[] image2, int width, int height)
        {
        }

        /*
         * 图像顺序为导星1，导星2，导星1，导星2
         * 
         * */
        public virtual void UpdateImage(List<MyImage> list)
        {
           
        }

        public virtual void UpdateFrame(List<TelemetryKeyWord> frameHead, List<MyImage> imageList)
        {
        }

        public virtual void UpdateFrame(List<List<ParseNode>> imageInforList, List<MyImage> imageList)
        {
        }

        public void UpdateStatus(long xferRate, int successes, int failures)
        {
            this.rateToolStripStatusLabel.Text = "速率: " + xferRate + "KB/S";
            this.successToolStripStatusLabel2.Text = "成功包数: " + successes;
            this.failToolStripStatusLabel3.Text = "失败包数: " + failures;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}