using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace wishKioskDIDDisplay
{
    partial class displayMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBoxCompleted = new GroupBox();
            flowLayoutPanelCompletedOrders = new FlowLayoutPanel();
            groupBoxPreparing = new GroupBox();
            flowLayoutPanelOrders = new FlowLayoutPanel();
            groupBoxCompleted.SuspendLayout();
            groupBoxPreparing.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxCompleted
            // 
            groupBoxCompleted.Controls.Add(flowLayoutPanelCompletedOrders);
            groupBoxCompleted.Dock = DockStyle.Left;
            groupBoxCompleted.Font = new System.Drawing.Font("맑은 고딕", 30F);
            groupBoxCompleted.Location = new Point(0, 0);
            groupBoxCompleted.Name = "groupBoxCompleted";
            groupBoxCompleted.Size = new Size(365, 723);
            groupBoxCompleted.TabIndex = 1;
            groupBoxCompleted.TabStop = false;
            groupBoxCompleted.Text = "완료";
            // 
            // flowLayoutPanelCompletedOrders
            // 
            flowLayoutPanelCompletedOrders.AutoScroll = true;
            flowLayoutPanelCompletedOrders.Dock = DockStyle.Fill;
            flowLayoutPanelCompletedOrders.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelCompletedOrders.Location = new Point(3, 83);
            flowLayoutPanelCompletedOrders.Name = "flowLayoutPanelCompletedOrders";
            flowLayoutPanelCompletedOrders.Size = new Size(359, 637);
            flowLayoutPanelCompletedOrders.TabIndex = 0;
            flowLayoutPanelCompletedOrders.WrapContents = false;
            // 
            // groupBoxPreparing
            // 
            groupBoxPreparing.Controls.Add(flowLayoutPanelOrders);
            groupBoxPreparing.Dock = DockStyle.Fill;
            groupBoxPreparing.Font = new System.Drawing.Font("맑은 고딕", 30F);
            groupBoxPreparing.Location = new Point(365, 0);
            groupBoxPreparing.Name = "groupBoxPreparing";
            groupBoxPreparing.Size = new Size(764, 723);
            groupBoxPreparing.TabIndex = 2;
            groupBoxPreparing.TabStop = false;
            groupBoxPreparing.Text = "주문 준비중";
            // 
            // flowLayoutPanelOrders
            // 
            flowLayoutPanelOrders.AutoScroll = true;
            flowLayoutPanelOrders.Dock = DockStyle.Fill;
            flowLayoutPanelOrders.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelOrders.Location = new Point(3, 83);
            flowLayoutPanelOrders.Name = "flowLayoutPanelOrders";
            flowLayoutPanelOrders.Size = new Size(758, 637);
            flowLayoutPanelOrders.TabIndex = 0;
            flowLayoutPanelOrders.WrapContents = false;
            // 
            // displayMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1129, 723);
            Controls.Add(groupBoxPreparing);
            Controls.Add(groupBoxCompleted);
            FormBorderStyle = FormBorderStyle.None;
            Name = "displayMain";
            Text = "WISH Order View";
            WindowState = FormWindowState.Maximized;
            Load += DisplayMain_Load;
            KeyDown += displayMain_KeyDown;
            groupBoxCompleted.ResumeLayout(false);
            groupBoxPreparing.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.GroupBox groupBoxCompleted;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCompletedOrders;
        private System.Windows.Forms.GroupBox groupBoxPreparing;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelOrders;
    }
}
