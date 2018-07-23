namespace TC
{
    partial class TelemetryForm
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
            this.telemetryDgv = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.telemetryDgv)).BeginInit();
            this.SuspendLayout();
            // 
            // telemetryDgv
            // 
            this.telemetryDgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.telemetryDgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.telemetryDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.telemetryDgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.telemetryDgv.Location = new System.Drawing.Point(0, 0);
            this.telemetryDgv.Name = "telemetryDgv";
            this.telemetryDgv.RowHeadersVisible = false;
            this.telemetryDgv.RowTemplate.Height = 37;
            this.telemetryDgv.Size = new System.Drawing.Size(1605, 637);
            this.telemetryDgv.TabIndex = 0;
            // 
            // TelemetryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1605, 637);
            this.Controls.Add(this.telemetryDgv);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "TelemetryForm";
            this.Text = "TelemetryForm";
            this.Load += new System.EventHandler(this.TelemetryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.telemetryDgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView telemetryDgv;


    }
}