﻿namespace wishKiosk
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
            printSettingsButton = new Button();
            menuButton = new Button();
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
            // settings
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(323, 164);
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
    }
}