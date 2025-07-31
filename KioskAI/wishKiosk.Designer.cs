namespace WishKiosk
{
    partial class WishKiosk
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WishKiosk));
            printButton = new Button();
            settingsButton = new Button();
            scanButton = new Button();
            infoButton = new Button();
            SuspendLayout();
            // 
            // printButton
            // 
            printButton.BackgroundImage = (Image)resources.GetObject("printButton.BackgroundImage");
            printButton.BackgroundImageLayout = ImageLayout.Zoom;
            printButton.Location = new Point(11, 74);
            printButton.Margin = new Padding(2);
            printButton.Name = "printButton";
            printButton.Size = new Size(295, 167);
            printButton.TabIndex = 0;
            printButton.UseVisualStyleBackColor = true;
            printButton.Click += printButton_Click;
            // 
            // settingsButton
            // 
            settingsButton.BackColor = Color.Transparent;
            settingsButton.BackgroundImage = (Image)resources.GetObject("settingsButton.BackgroundImage");
            settingsButton.BackgroundImageLayout = ImageLayout.Zoom;
            settingsButton.Location = new Point(11, 11);
            settingsButton.Margin = new Padding(2);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(57, 59);
            settingsButton.TabIndex = 1;
            settingsButton.UseVisualStyleBackColor = false;
            settingsButton.Click += settingsButton_Click;
            // 
            // scanButton
            // 
            scanButton.BackgroundImage = (Image)resources.GetObject("scanButton.BackgroundImage");
            scanButton.BackgroundImageLayout = ImageLayout.Zoom;
            scanButton.Location = new Point(310, 74);
            scanButton.Margin = new Padding(2);
            scanButton.Name = "scanButton";
            scanButton.Size = new Size(295, 167);
            scanButton.TabIndex = 2;
            scanButton.UseVisualStyleBackColor = true;
            scanButton.Click += scanButton_Click;
            // 
            // infoButton
            // 
            infoButton.BackColor = Color.Transparent;
            infoButton.BackgroundImage = (Image)resources.GetObject("infoButton.BackgroundImage");
            infoButton.BackgroundImageLayout = ImageLayout.Zoom;
            infoButton.Location = new Point(72, 11);
            infoButton.Margin = new Padding(2);
            infoButton.Name = "infoButton";
            infoButton.Size = new Size(57, 59);
            infoButton.TabIndex = 3;
            infoButton.UseVisualStyleBackColor = false;
            infoButton.Click += infoButton_Click;
            // 
            // WishKiosk
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(617, 255);
            Controls.Add(infoButton);
            Controls.Add(scanButton);
            Controls.Add(settingsButton);
            Controls.Add(printButton);
            Margin = new Padding(2);
            Name = "WishKiosk";
            Text = "WISH Kiosk";
            Load += WishKiosk_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button printButton;
        private Button settingsButton;
        private Button scanButton;
        private Button infoButton;
    }
}
