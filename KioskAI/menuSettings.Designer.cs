namespace wishKiosk
{
    partial class menuSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(menuSettings));
            menuDataGridView = new DataGridView();
            cancelButton = new Button();
            okButton = new Button();
            digitCntNumericUpDown = new NumericUpDown();
            digitCntGroup = new GroupBox();
            digitCntLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)menuDataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)digitCntNumericUpDown).BeginInit();
            digitCntGroup.SuspendLayout();
            SuspendLayout();
            // 
            // menuDataGridView
            // 
            menuDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            menuDataGridView.Location = new Point(12, 99);
            menuDataGridView.Name = "menuDataGridView";
            menuDataGridView.RowHeadersWidth = 62;
            menuDataGridView.Size = new Size(382, 575);
            menuDataGridView.TabIndex = 0;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(246, 680);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(71, 34);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "취소";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(323, 680);
            okButton.Name = "okButton";
            okButton.Size = new Size(71, 34);
            okButton.TabIndex = 2;
            okButton.Text = "확인";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // digitCntNumericUpDown
            // 
            digitCntNumericUpDown.Font = new Font("맑은 고딕", 12F);
            digitCntNumericUpDown.Location = new Point(210, 25);
            digitCntNumericUpDown.Name = "digitCntNumericUpDown";
            digitCntNumericUpDown.Size = new Size(166, 39);
            digitCntNumericUpDown.TabIndex = 3;
            digitCntNumericUpDown.ValueChanged += digitCntNumericUpDown_ValueChanged;
            // 
            // digitCntGroup
            // 
            digitCntGroup.Controls.Add(digitCntNumericUpDown);
            digitCntGroup.Controls.Add(digitCntLabel);
            digitCntGroup.Location = new Point(12, 12);
            digitCntGroup.Name = "digitCntGroup";
            digitCntGroup.Size = new Size(382, 81);
            digitCntGroup.TabIndex = 5;
            digitCntGroup.TabStop = false;
            // 
            // digitCntLabel
            // 
            digitCntLabel.AutoSize = true;
            digitCntLabel.Font = new Font("맑은 고딕", 12F);
            digitCntLabel.Location = new Point(6, 27);
            digitCntLabel.Name = "digitCntLabel";
            digitCntLabel.Size = new Size(198, 32);
            digitCntLabel.TabIndex = 4;
            digitCntLabel.Text = "수량 입력 자릿수";
            // 
            // menuSettings
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(406, 723);
            Controls.Add(digitCntGroup);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(menuDataGridView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "menuSettings";
            ShowInTaskbar = false;
            Text = "WISH Menu Settings";
            FormClosing += menuSettings_FormClosing;
            Load += menuSettings_Load;
            ((System.ComponentModel.ISupportInitialize)menuDataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)digitCntNumericUpDown).EndInit();
            digitCntGroup.ResumeLayout(false);
            digitCntGroup.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView menuDataGridView;
        private Button cancelButton;
        private Button okButton;
        private NumericUpDown digitCntNumericUpDown;
        private GroupBox digitCntGroup;
        private Label digitCntLabel;
    }
}