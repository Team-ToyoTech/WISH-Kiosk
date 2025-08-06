using System.ComponentModel;

namespace wishKiosk
{
    partial class wishKiosk
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private Button printButton;
        private Button scanButton;
        private Button settingsButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(wishKiosk));
            tableLayoutPanel = new TableLayoutPanel();
            printButton = new Button();
            scanButton = new Button();
            settingsButton = new Button();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.8F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.8F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.4F));
            tableLayoutPanel.Controls.Add(printButton, 0, 0);
            tableLayoutPanel.Controls.Add(scanButton, 1, 0);
            tableLayoutPanel.Controls.Add(settingsButton, 2, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(1220, 792);
            tableLayoutPanel.TabIndex = 0;
            // 
            // printButton
            // 
            printButton.BackgroundImage = (Image)resources.GetObject("printButton.BackgroundImage");
            printButton.BackgroundImageLayout = ImageLayout.Zoom;
            printButton.Dock = DockStyle.Fill;
            printButton.Location = new Point(3, 3);
            printButton.Name = "printButton";
            printButton.Size = new Size(601, 786);
            printButton.TabIndex = 0;
            printButton.UseVisualStyleBackColor = true;
            printButton.Click += printButton_Click;
            // 
            // scanButton
            // 
            scanButton.BackgroundImage = (Image)resources.GetObject("scanButton.BackgroundImage");
            scanButton.BackgroundImageLayout = ImageLayout.Zoom;
            scanButton.Dock = DockStyle.Fill;
            scanButton.Location = new Point(610, 3);
            scanButton.Name = "scanButton";
            scanButton.Size = new Size(601, 786);
            scanButton.TabIndex = 1;
            scanButton.UseVisualStyleBackColor = true;
            scanButton.Click += scanButton_Click;
            // 
            // settingsButton
            // 
            settingsButton.BackgroundImageLayout = ImageLayout.Zoom;
            settingsButton.Dock = DockStyle.Fill;
            settingsButton.Location = new Point(1217, 3);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(1, 786);
            settingsButton.TabIndex = 2;
            settingsButton.UseVisualStyleBackColor = true;
            settingsButton.Click += settingsButton_Click;
            // 
            // wishKiosk
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1220, 792);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "wishKiosk";
            Text = "WISH Kiosk";
            WindowState = FormWindowState.Maximized;
            Load += wishKiosk_Load;
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}
