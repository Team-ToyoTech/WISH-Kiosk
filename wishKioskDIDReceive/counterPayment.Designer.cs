namespace wishKioskDIDReceive
{
    partial class counterPayment
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
            completeButton = new Button();
            stateLabel = new Label();
            inputTextBox = new TextBox();
            inputLabel = new Label();
            SuspendLayout();
            // 
            // completeButton
            // 
            completeButton.Font = new Font("맑은 고딕", 15F);
            completeButton.Location = new Point(534, 9);
            completeButton.Name = "completeButton";
            completeButton.Size = new Size(157, 49);
            completeButton.TabIndex = 7;
            completeButton.Text = "완료";
            completeButton.UseVisualStyleBackColor = true;
            completeButton.Click += completeButton_Click;
            // 
            // stateLabel
            // 
            stateLabel.AutoSize = true;
            stateLabel.Font = new Font("맑은 고딕", 15F);
            stateLabel.Location = new Point(12, 69);
            stateLabel.Name = "stateLabel";
            stateLabel.Size = new Size(31, 41);
            stateLabel.TabIndex = 6;
            stateLabel.Text = "_";
            // 
            // inputTextBox
            // 
            inputTextBox.Font = new Font("맑은 고딕", 15F);
            inputTextBox.Location = new Point(357, 9);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(171, 47);
            inputTextBox.TabIndex = 5;
            inputTextBox.KeyDown += inputTextBox_KeyDown;
            // 
            // inputLabel
            // 
            inputLabel.AutoSize = true;
            inputLabel.Font = new Font("맑은 고딕", 15F);
            inputLabel.Location = new Point(12, 9);
            inputLabel.Name = "inputLabel";
            inputLabel.Size = new Size(339, 41);
            inputLabel.TabIndex = 4;
            inputLabel.Text = "결제된 주문 번호 입력: ";
            // 
            // counterPayment
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 135);
            Controls.Add(completeButton);
            Controls.Add(stateLabel);
            Controls.Add(inputTextBox);
            Controls.Add(inputLabel);
            Name = "counterPayment";
            Text = "counterPayment";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button completeButton;
        private Label stateLabel;
        private TextBox inputTextBox;
        private Label inputLabel;
    }
}