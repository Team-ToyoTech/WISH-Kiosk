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
            SuspendLayout();
            // 
            // orderCompleteLabel
            // 
            orderCompleteLabel.AutoSize = true;
            orderCompleteLabel.Font = new Font("맑은 고딕", 40F);
            orderCompleteLabel.Location = new Point(12, 9);
            orderCompleteLabel.Name = "orderCompleteLabel";
            orderCompleteLabel.Size = new Size(262, 106);
            orderCompleteLabel.TabIndex = 0;
            orderCompleteLabel.Text = "label1";
            // 
            // orderIncompleteLabel
            // 
            orderIncompleteLabel.AutoSize = true;
            orderIncompleteLabel.Font = new Font("맑은 고딕", 40F);
            orderIncompleteLabel.Location = new Point(280, 9);
            orderIncompleteLabel.Name = "orderIncompleteLabel";
            orderIncompleteLabel.Size = new Size(262, 106);
            orderIncompleteLabel.TabIndex = 1;
            orderIncompleteLabel.Text = "label1";
            // 
            // displayMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(orderCompleteLabel);
            Controls.Add(orderIncompleteLabel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "displayMain";
            Text = "WISH DID";
            WindowState = FormWindowState.Maximized;
            Load += this.displayMain_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label orderCompleteLabel;
        private Label orderIncompleteLabel;
    }
}
