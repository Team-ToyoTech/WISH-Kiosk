namespace wishKiosk
{
    partial class orderResult
    {
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.DataGridView orderResultDataGridView;
        private System.Windows.Forms.Label totalLabel;
        private System.Windows.Forms.Button OrderButton;
        private System.Windows.Forms.Button CancelButton_;

        private void InitializeComponent()
        {
            panelMain = new Panel();
            orderResultDataGridView = new DataGridView();
            totalLabel = new Label();
            OrderButton = new Button();
            CancelButton_ = new Button();
            panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)orderResultDataGridView).BeginInit();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.AutoScroll = true;
            panelMain.Controls.Add(orderResultDataGridView);
            panelMain.Controls.Add(totalLabel);
            panelMain.Controls.Add(OrderButton);
            panelMain.Controls.Add(CancelButton_);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Margin = new Padding(4);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(625, 707);
            panelMain.TabIndex = 0;
            // 
            // orderResultDataGridView
            // 
            orderResultDataGridView.AllowUserToAddRows = false;
            orderResultDataGridView.ColumnHeadersHeight = 34;
            orderResultDataGridView.Location = new Point(12, 12);
            orderResultDataGridView.Margin = new Padding(4);
            orderResultDataGridView.Name = "orderResultDataGridView";
            orderResultDataGridView.RowHeadersWidth = 62;
            orderResultDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            orderResultDataGridView.Size = new Size(600, 562);
            orderResultDataGridView.TabIndex = 1;
            // 
            // totalLabel
            // 
            totalLabel.AutoSize = true;
            totalLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            totalLabel.Location = new Point(12, 588);
            totalLabel.Margin = new Padding(4, 0, 4, 0);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new Size(95, 28);
            totalLabel.TabIndex = 2;
            totalLabel.Text = "총액: 0원";
            // 
            // OrderButton
            // 
            OrderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OrderButton.Location = new Point(417, 656);
            OrderButton.Margin = new Padding(4);
            OrderButton.Name = "OrderButton";
            OrderButton.Size = new Size(94, 38);
            OrderButton.TabIndex = 3;
            OrderButton.Text = "결제하기";
            OrderButton.UseVisualStyleBackColor = true;
            OrderButton.Click += OrderButton_Click;
            // 
            // CancelButton
            // 
            CancelButton_.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CancelButton_.Location = new Point(518, 656);
            CancelButton_.Margin = new Padding(4);
            CancelButton_.Name = "CancelButton";
            CancelButton_.Size = new Size(94, 38);
            CancelButton_.TabIndex = 4;
            CancelButton_.Text = "취소";
            CancelButton_.UseVisualStyleBackColor = true;
            CancelButton_.Click += CancelButton_Click;
            // 
            // orderResult
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(625, 707);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "orderResult";
            Text = "WISH Order";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)orderResultDataGridView).EndInit();
            ResumeLayout(false);
        }
    }
}
