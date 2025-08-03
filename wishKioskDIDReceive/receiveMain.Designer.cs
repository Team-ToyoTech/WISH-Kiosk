namespace wishKioskDIDReceive
{
    partial class receiveMain
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
            completeButton = new Button();
            groupBoxCompleted = new GroupBox();
            flowLayoutPanelCompletedOrders = new FlowLayoutPanel();
            groupBoxPreparing = new GroupBox();
            flowLayoutPanelOrders = new FlowLayoutPanel();
            groupBoxCompleted.SuspendLayout();
            groupBoxPreparing.SuspendLayout();
            SuspendLayout();
            // 
            // completeButton
            // 
            completeButton.Dock = DockStyle.Top;
            completeButton.Font = new Font("맑은 고딕", 17F);
            completeButton.Location = new Point(0, 0);
            completeButton.Name = "completeButton";
            completeButton.Size = new Size(1129, 106);
            completeButton.TabIndex = 0;
            completeButton.Text = "카운터 결제 확인";
            completeButton.UseVisualStyleBackColor = true;
            completeButton.Click += completeButton_Click;
            // 
            // groupBoxCompleted
            // 
            groupBoxCompleted.Controls.Add(flowLayoutPanelCompletedOrders);
            groupBoxCompleted.Dock = DockStyle.Left;
            groupBoxCompleted.Font = new Font("맑은 고딕", 20F);
            groupBoxCompleted.Location = new Point(0, 106);
            groupBoxCompleted.Name = "groupBoxCompleted";
            groupBoxCompleted.Size = new Size(250, 617);
            groupBoxCompleted.TabIndex = 1;
            groupBoxCompleted.TabStop = false;
            groupBoxCompleted.Text = "완료";
            // 
            // flowLayoutPanelCompletedOrders
            // 
            flowLayoutPanelCompletedOrders.AutoScroll = true;
            flowLayoutPanelCompletedOrders.Dock = DockStyle.Fill;
            flowLayoutPanelCompletedOrders.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelCompletedOrders.Location = new Point(3, 57);
            flowLayoutPanelCompletedOrders.Name = "flowLayoutPanelCompletedOrders";
            flowLayoutPanelCompletedOrders.Size = new Size(244, 557);
            flowLayoutPanelCompletedOrders.TabIndex = 0;
            flowLayoutPanelCompletedOrders.WrapContents = false;
            // 
            // groupBoxPreparing
            // 
            groupBoxPreparing.Controls.Add(flowLayoutPanelOrders);
            groupBoxPreparing.Dock = DockStyle.Fill;
            groupBoxPreparing.Font = new Font("맑은 고딕", 20F);
            groupBoxPreparing.Location = new Point(250, 106);
            groupBoxPreparing.Name = "groupBoxPreparing";
            groupBoxPreparing.Size = new Size(879, 617);
            groupBoxPreparing.TabIndex = 2;
            groupBoxPreparing.TabStop = false;
            groupBoxPreparing.Text = "주문 준비중";
            // 
            // flowLayoutPanelOrders
            // 
            flowLayoutPanelOrders.AutoScroll = true;
            flowLayoutPanelOrders.Dock = DockStyle.Fill;
            flowLayoutPanelOrders.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelOrders.Location = new Point(3, 57);
            flowLayoutPanelOrders.Name = "flowLayoutPanelOrders";
            flowLayoutPanelOrders.Size = new Size(873, 557);
            flowLayoutPanelOrders.TabIndex = 0;
            flowLayoutPanelOrders.WrapContents = false;
            // 
            // receiveMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1129, 723);
            Controls.Add(groupBoxPreparing);
            Controls.Add(groupBoxCompleted);
            Controls.Add(completeButton);
            FormBorderStyle = FormBorderStyle.None;
            Name = "receiveMain";
            Text = "WISH Order View";
            WindowState = FormWindowState.Maximized;
            groupBoxCompleted.ResumeLayout(false);
            groupBoxPreparing.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button completeButton;
        private System.Windows.Forms.GroupBox groupBoxCompleted;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCompletedOrders;
        private System.Windows.Forms.GroupBox groupBoxPreparing;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelOrders;
    }
}
