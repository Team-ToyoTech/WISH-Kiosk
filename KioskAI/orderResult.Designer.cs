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
            CounterOrderButton = new Button();
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
            panelMain.Margin = new Padding(4, 4, 4, 4);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(812, 905);
            panelMain.TabIndex = 0;
            // 
            // orderResultDataGridView
            // 
            orderResultDataGridView.AllowUserToAddRows = false;
            orderResultDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            orderResultDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            orderResultDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            orderResultDataGridView.Location = new Point(16, 15);
            orderResultDataGridView.Margin = new Padding(4, 4, 4, 4);
            orderResultDataGridView.Name = "orderResultDataGridView";
            orderResultDataGridView.RowHeadersWidth = 62;
            orderResultDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            orderResultDataGridView.Size = new Size(780, 719);
            orderResultDataGridView.TabIndex = 0;
            // 
            // totalLabel
            // 
            totalLabel.AutoSize = true;
            totalLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            totalLabel.Location = new Point(16, 753);
            totalLabel.Margin = new Padding(4, 0, 4, 0);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new Size(128, 37);
            totalLabel.TabIndex = 1;
            totalLabel.Text = "총액: 0원";
            // 
            // OrderButton
            // 
            OrderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OrderButton.Location = new Point(542, 840);
            OrderButton.Margin = new Padding(4, 4, 4, 4);
            OrderButton.Name = "OrderButton";
            OrderButton.Size = new Size(122, 49);
            OrderButton.TabIndex = 2;
            OrderButton.Text = "결제하기";
            OrderButton.Click += OrderButton_Click;
            // 
            // CancelButton_
            // 
            CancelButton_.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CancelButton_.Location = new Point(673, 840);
            CancelButton_.Margin = new Padding(4, 4, 4, 4);
            CancelButton_.Name = "CancelButton_";
            CancelButton_.Size = new Size(122, 49);
            CancelButton_.TabIndex = 3;
            CancelButton_.Text = "취소";
            CancelButton_.Click += CancelButton_Click;
            // 
            // CounterOrderButton
            // 
            CounterOrderButton.Location = new Point(286, 840);
            CounterOrderButton.Name = "CounterOrderButton";
            CounterOrderButton.Size = new Size(249, 46);
            CounterOrderButton.TabIndex = 4;
            CounterOrderButton.Text = "카운터에서 결제하기";
            CounterOrderButton.UseVisualStyleBackColor = true;
            CounterOrderButton.Click += CounterOrderButton_Click;
            // 
            // orderResult
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(812, 905);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 4, 4, 4);
            MaximizeBox = false;
            Name = "orderResult";
            Text = "WISH Order";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)orderResultDataGridView).EndInit();
            ResumeLayout(false);
        }
        private Button CounterOrderButton;
    }
}
