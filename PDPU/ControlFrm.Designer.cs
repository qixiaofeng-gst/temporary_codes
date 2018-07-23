namespace PDPU
{
    partial class ControlFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.fullScreenButton = new System.Windows.Forms.Button();
            this.frameHeadDgv = new System.Windows.Forms.DataGridView();
            this.frameHeadInforButton = new System.Windows.Forms.Button();
            this.testBtn = new System.Windows.Forms.Button();
            this.cypressControl1 = new PDPU.CypressControl();
            ((System.ComponentModel.ISupportInitialize)(this.frameHeadDgv)).BeginInit();
            this.SuspendLayout();
            // 
            // fullScreenButton
            // 
            this.fullScreenButton.Location = new System.Drawing.Point(146, 208);
            this.fullScreenButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.fullScreenButton.Name = "fullScreenButton";
            this.fullScreenButton.Size = new System.Drawing.Size(208, 46);
            this.fullScreenButton.TabIndex = 0;
            this.fullScreenButton.Text = "全屏显示";
            this.fullScreenButton.UseVisualStyleBackColor = true;
            this.fullScreenButton.Click += new System.EventHandler(this.fullScreen_Click);
            // 
            // frameHeadDgv
            // 
            this.frameHeadDgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.frameHeadDgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.frameHeadDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.frameHeadDgv.DefaultCellStyle = dataGridViewCellStyle1;
            this.frameHeadDgv.Location = new System.Drawing.Point(22, 269);
            this.frameHeadDgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.frameHeadDgv.Name = "frameHeadDgv";
            this.frameHeadDgv.RowHeadersVisible = false;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.frameHeadDgv.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.frameHeadDgv.RowTemplate.Height = 27;
            this.frameHeadDgv.Size = new System.Drawing.Size(770, 1066);
            this.frameHeadDgv.TabIndex = 2;
            // 
            // frameHeadInforButton
            // 
            this.frameHeadInforButton.Location = new System.Drawing.Point(637, 208);
            this.frameHeadInforButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.frameHeadInforButton.Name = "frameHeadInforButton";
            this.frameHeadInforButton.Size = new System.Drawing.Size(208, 46);
            this.frameHeadInforButton.TabIndex = 3;
            this.frameHeadInforButton.Text = "显示帧头";
            this.frameHeadInforButton.UseVisualStyleBackColor = true;
            this.frameHeadInforButton.Click += new System.EventHandler(this.frameHeadInforButton_Click);
            // 
            // testBtn
            // 
            this.testBtn.Location = new System.Drawing.Point(444, 208);
            this.testBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.testBtn.Name = "testBtn";
            this.testBtn.Size = new System.Drawing.Size(152, 46);
            this.testBtn.TabIndex = 4;
            this.testBtn.Text = "测试";
            this.testBtn.UseVisualStyleBackColor = true;
            this.testBtn.Click += new System.EventHandler(this.testBtn_Click);
            // 
            // cypressControl1
            // 
            this.cypressControl1.Location = new System.Drawing.Point(8, 19);
            this.cypressControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cypressControl1.Name = "cypressControl1";
            this.cypressControl1.Size = new System.Drawing.Size(856, 179);
            this.cypressControl1.TabIndex = 1;
            // 
            // ControlFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 1488);
            this.Controls.Add(this.testBtn);
            this.Controls.Add(this.frameHeadInforButton);
            this.Controls.Add(this.frameHeadDgv);
            this.Controls.Add(this.cypressControl1);
            this.Controls.Add(this.fullScreenButton);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ControlFrm";
            this.Text = "控制区";
            this.Load += new System.EventHandler(this.Frm_load);
            ((System.ComponentModel.ISupportInitialize)(this.frameHeadDgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button fullScreenButton;
        public CypressControl cypressControl1;
        private System.Windows.Forms.DataGridView frameHeadDgv;
        private System.Windows.Forms.Button frameHeadInforButton;
        private System.Windows.Forms.Button testBtn;

    }
}