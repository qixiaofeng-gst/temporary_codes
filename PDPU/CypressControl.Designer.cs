namespace PDPU
{
    partial class CypressControl
    {

        
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox PpxBox;
        private System.Windows.Forms.ComboBox QueueBox;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.ComboBox EndPointsComboBox;
        private System.Windows.Forms.Label label5;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PpxBox = new System.Windows.Forms.ComboBox();
            this.QueueBox = new System.Windows.Forms.ComboBox();
            this.StartBtn = new System.Windows.Forms.Button();
            this.EndPointsComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.logCheckBox = new System.Windows.Forms.CheckBox();
            this.crcCheckBox = new System.Windows.Forms.CheckBox();
            this.autoSaveBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "包数/每次";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(356, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "传输队列数";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // PpxBox
            // 
            this.PpxBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PpxBox.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128"});
            this.PpxBox.Location = new System.Drawing.Point(150, 44);
            this.PpxBox.Name = "PpxBox";
            this.PpxBox.Size = new System.Drawing.Size(102, 32);
            this.PpxBox.TabIndex = 1;
            this.PpxBox.SelectedIndexChanged += new System.EventHandler(this.PpxBox_SelectedIndexChanged);
            // 
            // QueueBox
            // 
            this.QueueBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QueueBox.Items.AddRange(new object[] {
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256",
            "1024",
            "2048"});
            this.QueueBox.Location = new System.Drawing.Point(470, 47);
            this.QueueBox.Name = "QueueBox";
            this.QueueBox.Size = new System.Drawing.Size(131, 32);
            this.QueueBox.TabIndex = 2;
            // 
            // StartBtn
            // 
            this.StartBtn.BackColor = System.Drawing.Color.Aquamarine;
            this.StartBtn.Location = new System.Drawing.Point(144, 83);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(108, 37);
            this.StartBtn.TabIndex = 3;
            this.StartBtn.Text = "开始";
            this.StartBtn.UseVisualStyleBackColor = false;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // EndPointsComboBox
            // 
            this.EndPointsComboBox.DropDownHeight = 120;
            this.EndPointsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EndPointsComboBox.FormattingEnabled = true;
            this.EndPointsComboBox.IntegralHeight = false;
            this.EndPointsComboBox.Location = new System.Drawing.Point(150, 2);
            this.EndPointsComboBox.Name = "EndPointsComboBox";
            this.EndPointsComboBox.Size = new System.Drawing.Size(451, 32);
            this.EndPointsComboBox.TabIndex = 0;
            this.EndPointsComboBox.SelectedIndexChanged += new System.EventHandler(this.EndPointsComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 24);
            this.label5.TabIndex = 11;
            this.label5.Text = "端点";
            // 
            // logCheckBox
            // 
            this.logCheckBox.AutoSize = true;
            this.logCheckBox.Checked = true;
            this.logCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logCheckBox.Location = new System.Drawing.Point(266, 92);
            this.logCheckBox.Name = "logCheckBox";
            this.logCheckBox.Size = new System.Drawing.Size(72, 28);
            this.logCheckBox.TabIndex = 12;
            this.logCheckBox.Text = "Log";
            this.logCheckBox.UseVisualStyleBackColor = true;
            // 
            // crcCheckBox
            // 
            this.crcCheckBox.AutoSize = true;
            this.crcCheckBox.Checked = true;
            this.crcCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.crcCheckBox.Location = new System.Drawing.Point(344, 94);
            this.crcCheckBox.Name = "crcCheckBox";
            this.crcCheckBox.Size = new System.Drawing.Size(120, 28);
            this.crcCheckBox.TabIndex = 13;
            this.crcCheckBox.Text = "CRC检查";
            this.crcCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoSaveBtn
            // 
            this.autoSaveBtn.Location = new System.Drawing.Point(470, 85);
            this.autoSaveBtn.Name = "autoSaveBtn";
            this.autoSaveBtn.Size = new System.Drawing.Size(131, 44);
            this.autoSaveBtn.TabIndex = 14;
            this.autoSaveBtn.Text = "自动存图";
            this.autoSaveBtn.UseVisualStyleBackColor = true;
            this.autoSaveBtn.Click += new System.EventHandler(this.autoSaveBtn_Click);
            // 
            // CypressControl
            // 
            this.Controls.Add(this.autoSaveBtn);
            this.Controls.Add(this.crcCheckBox);
            this.Controls.Add(this.logCheckBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.EndPointsComboBox);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.QueueBox);
            this.Controls.Add(this.PpxBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CypressControl";
            this.Size = new System.Drawing.Size(625, 170);
            this.Load += new System.EventHandler(this.CypressControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox logCheckBox;
        private System.Windows.Forms.CheckBox crcCheckBox;
        private System.Windows.Forms.Button autoSaveBtn;
    }
}
