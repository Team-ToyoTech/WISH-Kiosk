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
            orderCompleteLabel = new Label();
            orderIncompleteLabel = new Label();
            completeLabel = new Label();
            preparingLabel = new Label();
            SuspendLayout();
            // 
            // orderCompleteLabel
            // 
            orderCompleteLabel.AutoSize = true;
            orderCompleteLabel.Font = new Font("맑은 고딕", 80F);
            orderCompleteLabel.Location = new Point(12, 221);
            orderCompleteLabel.Name = "orderCompleteLabel";
            orderCompleteLabel.Size = new Size(157, 212);
            orderCompleteLabel.TabIndex = 0;
            orderCompleteLabel.Text = "_";
            // 
            // orderIncompleteLabel
            // 
            orderIncompleteLabel.AutoSize = true;
            orderIncompleteLabel.Font = new Font("맑은 고딕", 80F);
            orderIncompleteLabel.Location = new Point(636, 221);
            orderIncompleteLabel.Name = "orderIncompleteLabel";
            orderIncompleteLabel.Size = new Size(1961, 212);
            orderIncompleteLabel.TabIndex = 1;
            orderIncompleteLabel.Text = "준비중인 주문이 없습니다";
            // 
            // completeLabel
            // 
            completeLabel.AutoSize = true;
            completeLabel.Font = new Font("맑은 고딕", 80F);
            completeLabel.Location = new Point(12, 9);
            completeLabel.Name = "completeLabel";
            completeLabel.Size = new Size(409, 212);
            completeLabel.TabIndex = 2;
            completeLabel.Text = "완료";
            // 
            // preparingLabel
            // 
            preparingLabel.AutoSize = true;
            preparingLabel.Font = new Font("맑은 고딕", 80F);
            preparingLabel.Location = new Point(636, 9);
            preparingLabel.Name = "preparingLabel";
            preparingLabel.Size = new Size(569, 212);
            preparingLabel.TabIndex = 3;
            preparingLabel.Text = "준비중";
            // 
            // displayMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1764, 788);
            Controls.Add(preparingLabel);
            Controls.Add(completeLabel);
            Controls.Add(orderCompleteLabel);
            Controls.Add(orderIncompleteLabel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "displayMain";
            Text = "WISH DID";
            WindowState = FormWindowState.Maximized;
            Load += displayMain_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label orderCompleteLabel;
        private Label orderIncompleteLabel;
        private Label completeLabel;
        private Label preparingLabel;
    }
}
