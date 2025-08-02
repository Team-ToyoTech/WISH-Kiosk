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
            inputLabel = new Label();
            inputTextBox = new TextBox();
            stateLabel = new Label();
            completeButton = new Button();
            SuspendLayout();
            // 
            // inputLabel
            // 
            inputLabel.AutoSize = true;
            inputLabel.Font = new Font("맑은 고딕", 15F);
            inputLabel.Location = new Point(12, 9);
            inputLabel.Name = "inputLabel";
            inputLabel.Size = new Size(339, 41);
            inputLabel.TabIndex = 0;
            inputLabel.Text = "결제된 주문 번호 입력: ";
            // 
            // inputTextBox
            // 
            inputTextBox.Font = new Font("맑은 고딕", 15F);
            inputTextBox.Location = new Point(357, 9);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(171, 47);
            inputTextBox.TabIndex = 1;
            inputTextBox.KeyDown += inputTextBox_KeyDown;
            // 
            // stateLabel
            // 
            stateLabel.AutoSize = true;
            stateLabel.Font = new Font("맑은 고딕", 15F);
            stateLabel.Location = new Point(12, 148);
            stateLabel.Name = "stateLabel";
            stateLabel.Size = new Size(31, 41);
            stateLabel.TabIndex = 2;
            stateLabel.Text = "_";
            // 
            // completeButton
            // 
            completeButton.Font = new Font("맑은 고딕", 15F);
            completeButton.Location = new Point(534, 9);
            completeButton.Name = "completeButton";
            completeButton.Size = new Size(157, 49);
            completeButton.TabIndex = 3;
            completeButton.Text = "완료";
            completeButton.UseVisualStyleBackColor = true;
            completeButton.Click += completeButton_Click;
            // 
            // receiveMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(completeButton);
            Controls.Add(stateLabel);
            Controls.Add(inputTextBox);
            Controls.Add(inputLabel);
            Name = "receiveMain";
            Text = "WISH Order";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label inputLabel;
        private TextBox inputTextBox;
        private Label stateLabel;
        private Button completeButton;
    }
}
