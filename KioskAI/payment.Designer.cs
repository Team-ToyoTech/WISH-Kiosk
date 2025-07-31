namespace WishKiosk
{
    partial class Payment
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 PaymentView;

        private void InitializeComponent()
        {
            PaymentView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)PaymentView).BeginInit();
            SuspendLayout();
            // 
            // PaymentView
            // 
            PaymentView.AllowExternalDrop = true;
            PaymentView.CreationProperties = null;
            PaymentView.DefaultBackgroundColor = Color.White;
            PaymentView.Dock = DockStyle.Fill;
            PaymentView.Location = new Point(0, 0);
            PaymentView.Name = "PaymentView";
            PaymentView.Size = new Size(1608, 1138);
            PaymentView.Source = new Uri("about:blank", UriKind.Absolute);
            PaymentView.TabIndex = 0;
            PaymentView.ZoomFactor = 1D;
            // 
            // Payment
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1608, 1138);
            Controls.Add(PaymentView);
            Name = "Payment";
            Text = "결제";
            WindowState = FormWindowState.Maximized;
            Load += Payment_Load;
            ((System.ComponentModel.ISupportInitialize)PaymentView).EndInit();
            ResumeLayout(false);
        }
    }
}
