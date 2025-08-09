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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(orderResult));
            panelMain = new Panel();
            CounterOrderButton = new Button();
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
            panelMain.Controls.Add(CounterOrderButton);
            panelMain.Controls.Add(orderResultDataGridView);
            panelMain.Controls.Add(totalLabel);
            panelMain.Controls.Add(OrderButton);
            panelMain.Controls.Add(CancelButton_);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(625, 707);
            panelMain.TabIndex = 0;
            // 
            // CounterOrderButton
            // 
            CounterOrderButton.Location = new Point(220, 656);
            CounterOrderButton.Margin = new Padding(2);
            CounterOrderButton.Name = "CounterOrderButton";
            CounterOrderButton.Size = new Size(192, 36);
            CounterOrderButton.TabIndex = 4;
            CounterOrderButton.Text = "카운터에서 결제하기";
            CounterOrderButton.UseVisualStyleBackColor = true;
            CounterOrderButton.Click += CounterOrderButton_Click;
            // 
            // orderResultDataGridView
            // 
            orderResultDataGridView.AllowUserToAddRows = false;
            orderResultDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            orderResultDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            orderResultDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            orderResultDataGridView.Location = new Point(12, 12);
            orderResultDataGridView.Name = "orderResultDataGridView";
            orderResultDataGridView.RowHeadersWidth = 62;
            orderResultDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            orderResultDataGridView.Size = new Size(600, 562);
            orderResultDataGridView.TabIndex = 0;
            // 
            // totalLabel
            // 
            totalLabel.AutoSize = true;
            totalLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            totalLabel.Location = new Point(12, 588);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new Size(95, 28);
            totalLabel.TabIndex = 1;
            totalLabel.Text = "총액: 0원";
            // 
            // OrderButton
            // 
            OrderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OrderButton.Location = new Point(417, 656);
            OrderButton.Name = "OrderButton";
            OrderButton.Size = new Size(94, 38);
            OrderButton.TabIndex = 2;
            OrderButton.Text = "결제하기";
            OrderButton.Click += OrderButton_Click;
            // 
            // CancelButton_
            // 
            CancelButton_.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CancelButton_.Location = new Point(518, 656);
            CancelButton_.Name = "CancelButton_";
            CancelButton_.Size = new Size(94, 38);
            CancelButton_.TabIndex = 3;
            CancelButton_.Text = "취소";
            CancelButton_.Click += CancelButton_Click;
            // 
            // orderResult
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(625, 707);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "orderResult";
            ShowInTaskbar = false;
            Text = "WISH Order";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)orderResultDataGridView).EndInit();
            ResumeLayout(false);
        }
        private Button CounterOrderButton;
    }
}
