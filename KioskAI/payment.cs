using Microsoft.Web.WebView2.Core;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace wishKiosk
{
	public partial class payment : Form
	{
		private readonly int amount;

		private string? orderId;
		private readonly HttpClient http = new HttpClient();

		public record PaymentResponse(string status);

        public payment(int amount)
		{
			InitializeComponent();
			this.amount = amount;
		}

		private async void payment_Load(object sender, EventArgs e)
		{
			await paymentView.EnsureCoreWebView2Async();

			// 새 창 요청을 동일 WebView 안에서 처리
			paymentView.CoreWebView2.NewWindowRequested += (_, ev) =>
			{
				paymentView.CoreWebView2.Navigate(ev.Uri);
				ev.Handled = true;
			};

			paymentView.NavigationStarting += OnNavigationStarting;
			await StartPaymentAsync();
		}

		private async Task StartPaymentAsync()
		{
			var body = new { amount = amount};
			var res = await http.PostAsJsonAsync("http://localhost:4000/pay", body);
			res.EnsureSuccessStatusCode();

			var json = await res.Content.ReadFromJsonAsync<JsonElement>();
			orderId = json.GetProperty("redirectId").GetString()!;

			paymentView.Source = new Uri("http://localhost:4000/checkout/" + orderId);
		}

		private async void OnNavigationStarting(object? s, CoreWebView2NavigationStartingEventArgs e)
		{
			var res = await http.GetFromJsonAsync<PaymentResponse>("http://localhost:4000/ispaying/" + orderId);
			if (res?.status == "paid")
			{
				MessageBox.Show("결제 완료");
				DialogResult = DialogResult.OK;
				Close();
				e.Cancel = false;
			}
			else if (res?.status == "failed")
			{
				MessageBox.Show("결제 실패");
				DialogResult = DialogResult.Abort;
				Close();
				e.Cancel = true;
			}
            else
            {
                // 결제 진행 중
            }

            //if (e.Uri.StartsWith("https://yourserver.com/api/payments/kakao/success", StringComparison.Ordinal))
            //{
            //    var qs = System.Web.HttpUtility.ParseQueryString(new Uri(e.Uri).Query);
            //    string? key = qs["paymentKey"];
            //
            //    await http.PostAsJsonAsync("https://yourserver.com/api/payments/kakao/confirm",
            //                               new { orderId = orderId, paymentKey = key, amount = amount });
            //
            //    MessageBox.Show("결제 완료");
            //    DialogResult = DialogResult.OK;
            //    Close();
            //    e.Cancel = true;
            //}
            //else if (e.Uri.StartsWith("https://yourserver.com/api/payments/kakao/fail", StringComparison.Ordinal))
            //{
            //    MessageBox.Show("결제 실패");
            //    DialogResult = DialogResult.Abort;
            //    Close();
            //    e.Cancel = true;
            //}
        }
	}
}
