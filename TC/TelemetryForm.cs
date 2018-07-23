using PDPU;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TC
{
    public partial class TelemetryForm : DockContent
    {
        private BindingList<ParseNode> telemetryList;
        public TelemetryForm()
        {
            InitializeComponent();
        }

        public void SetTelemetry(List<ParseNode> list)
        {
            if (telemetryDgv.DataSource == null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ParseNode node = list[i];
                    if (node != null)
                    {
                        telemetryList.Add(node);
                    }
                    else
                    {
                        MessageBox.Show("遥测表初始化错误!");
                        return;
                    }
                }
                telemetryDgv.DataSource = telemetryList;
            }
            else if (list.Count == telemetryList.Count)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    telemetryList[i] = list[i];
                }
            }
        }

        private void TelemetryForm_Load(object sender, EventArgs e)
        {
            telemetryList = new BindingList<ParseNode>();
        }
    }
}
