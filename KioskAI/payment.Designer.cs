namespace wishKiosk
{
    partial class payment
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 paymentView;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(payment));
            paymentView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)paymentView).BeginInit();
            SuspendLayout();
            // 
            // paymentView
            // 
            paymentView.AllowExternalDrop = true;
            paymentView.CreationProperties = null;
            paymentView.DefaultBackgroundColor = Color.White;
            paymentView.Dock = DockStyle.Fill;
            paymentView.Location = new Point(0, 0);
            paymentView.Name = "paymentView";
            paymentView.Size = new Size(1608, 1138);
            paymentView.Source = new Uri("about:blank", UriKind.Absolute);
            paymentView.TabIndex = 0;
            paymentView.ZoomFactor = 1D;
            // 
            // payment
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1608, 1138);
            Controls.Add(paymentView);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "payment";
            ShowInTaskbar = false;
            Text = "결제";
            WindowState = FormWindowState.Maximized;
            Load += payment_Load;
            ((System.ComponentModel.ISupportInitialize)paymentView).EndInit();
            ResumeLayout(false);
        }
    }
}
