using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PDPU
{
    public partial class ImageFrm : DockContent
    {
        private double zoomFactor;
        private MyImage myImage;
        public ImageFrm()
        {
            zoomFactor = 1;
            InitializeComponent();
        }

        public void SetImage(MyImage myImage)
        {
            if (myImage == null)
            {
                return;
            }
            this.myImage = myImage;
            Image image = myImage.GetBitmap();
            
            pictureBox.Image = image;
            SetImageSize(myImage.Width, myImage.Height);
        }
        
        private void ImageFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        public void SetImageSize(int width, int height)
        {
            pictureBox.Height = (int)(height * zoomFactor);
            pictureBox.Width = (int)(width * zoomFactor); 
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "图像另存为";
            sfd.Filter = "RAW文件|*.raw|BMP文件|*.bmp";
            
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 1:     //保存raw图
                    {
                        if (myImage != null && myImage.Image != null)
                        {
                            Log savedImage = new Log(sfd.FileName, false, false, null);
                            try
                            {
                                savedImage.Write(myImage.Image, 0, myImage.Image.Length);
                                savedImage.CloseStream();
                                    
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(exception.StackTrace, "Exception '" + exception.Message + "' thrown by " + exception.Source);
                            }
                        }
                        break;
                    }
                    case 2:     //保存bmp图
                    {
                        if (pictureBox.Image != null)
                        {
                            pictureBox.Image.Save(sfd.FileName);
                        }
                        break;
                    }
                }
                
            }
        }

        private void ImageFrm_Load(object sender, EventArgs e)
        {
        }

        private void image_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                zoomFactor *= 2;
            }
            else
            {
                zoomFactor /= 2;
            }
            if (zoomFactor >= 16)
            {
                zoomFactor = 16;
            }
            else if (zoomFactor <= 0.0625)
            {
                zoomFactor = 0.0625;
            }
        }

        private void ZoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomFactor *= 2;
            if (zoomFactor >= 16)
            {
                zoomFactor = 16;
            }
            if (myImage != null)
            {
                SetImageSize(myImage.Width, myImage.Height);
            }
        }

        private void ZoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomFactor /= 2;
            if (zoomFactor <= 0.0625)
            {
                zoomFactor = 0.0625;
            }
            if (myImage != null)
            {
                SetImageSize(myImage.Width, myImage.Height);
            }
        }

        private void OriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomFactor = 1;
            if (myImage != null)
            {
                SetImageSize(myImage.Width, myImage.Height);
            }
        }

    }
}
