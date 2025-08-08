namespace wishKiosk
{
    partial class settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(settings));
            printSettingsButton = new Button();
            menuButton = new Button();
            passwordChangeButton = new Button();
            infoButton = new Button();
            serverResetButton = new Button();
            SuspendLayout();
            // 
            // printSettingsButton
            // 
            printSettingsButton.Location = new Point(12, 12);
            printSettingsButton.Name = "printSettingsButton";
            printSettingsButton.Size = new Size(146, 140);
            printSettingsButton.TabIndex = 0;
            printSettingsButton.Text = "인쇄 설정";
            printSettingsButton.UseVisualStyleBackColor = true;
            printSettingsButton.Click += printSettingsButton_Click;
            // 
            // menuButton
            // 
            menuButton.Location = new Point(164, 12);
            menuButton.Name = "menuButton";
            menuButton.Size = new Size(146, 140);
            menuButton.TabIndex = 1;
            menuButton.Text = "메뉴 구성";
            menuButton.UseVisualStyleBackColor = true;
            menuButton.Click += menuButton_Click;
            // 
            // passwordChangeButton
            // 
            passwordChangeButton.Location = new Point(316, 12);
            passwordChangeButton.Name = "passwordChangeButton";
            passwordChangeButton.Size = new Size(146, 66);
            passwordChangeButton.TabIndex = 2;
            passwordChangeButton.Text = "비밀번호 변경";
            passwordChangeButton.UseVisualStyleBackColor = true;
            passwordChangeButton.Click += passwordChangeButton_Click;
            // 
            // infoButton
            // 
            infoButton.BackgroundImage = (Image)resources.GetObject("infoButton.BackgroundImage");
            infoButton.BackgroundImageLayout = ImageLayout.Zoom;
            infoButton.Location = new Point(391, 84);
            infoButton.Name = "infoButton";
            infoButton.Size = new Size(69, 68);
            infoButton.TabIndex = 3;
            infoButton.UseVisualStyleBackColor = true;
            infoButton.Click += infoButton_Click;
            // 
            // serverResetButton
            // 
            serverResetButton.BackgroundImageLayout = ImageLayout.Zoom;
            serverResetButton.Font = new Font("맑은 고딕", 8F);
            serverResetButton.Location = new Point(316, 84);
            serverResetButton.Name = "serverResetButton";
            serverResetButton.Size = new Size(69, 68);
            serverResetButton.TabIndex = 4;
            serverResetButton.Text = "서버 초기화";
            serverResetButton.UseVisualStyleBackColor = true;
            serverResetButton.Click += serverResetButton_Click;
            // 
            // settings
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(473, 164);
            Controls.Add(serverResetButton);
            Controls.Add(infoButton);
            Controls.Add(passwordChangeButton);
            Controls.Add(menuButton);
            Controls.Add(printSettingsButton);
            Name = "settings";
            Text = "WISH Settings";
            FormClosing += settings_FormClosing;
            ResumeLayout(false);
        }

        #endregion

        private Button printSettingsButton;
        private Button menuButton;
        private Button passwordChangeButton;
        private Button infoButton;
        private Button serverResetButton;
    }
}