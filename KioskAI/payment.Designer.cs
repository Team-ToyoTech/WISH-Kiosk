namespace wishKiosk
{
    partial class payment
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 paymentView;

        private void InitializeComponent()
        {
            this.paymentView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.paymentView)).BeginInit();
            this.SuspendLayout();
            // 
            // paymentView
            // 
            this.paymentView.CreationProperties = null;
            this.paymentView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paymentView.Location = new System.Drawing.Point(0, 0);
            this.paymentView.Name = "paymentView";
            this.paymentView.Size = new System.Drawing.Size(900, 640);
            this.paymentView.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            this.paymentView.TabIndex = 0;
            this.paymentView.ZoomFactor = 1D;
            // 
            // payment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(900, 640);
            this.Controls.Add(this.paymentView);
            this.Name = "payment";
            this.Text = "결제";
            this.Load += new System.EventHandler(this.payment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.paymentView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
