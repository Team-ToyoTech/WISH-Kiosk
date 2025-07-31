using Microsoft.Web.WebView2.Core;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WishKiosk
{
	public partial class Payment : Form
	{
		private readonly int amount;
		private string? orderId;
        private bool isRunning = true; // 결제 진행 중 여부
		private readonly HttpClient http = new HttpClient();

		public record PaymentResponse(string status); // 결제 상태 응답 모델

        public Payment(int amount)
		{
			InitializeComponent();
			this.amount = amount;
		}

		private async void Payment_Load(object sender, EventArgs e)
		{
			await PaymentView.EnsureCoreWebView2Async();

			// 새 창 요청을 동일 WebView 안에서 처리
			PaymentView.CoreWebView2.NewWindowRequested += (_, ev) =>
			{
				PaymentView.CoreWebView2.Navigate(ev.Uri);
				ev.Handled = true;
			};

			await StartPaymentAsync();
			PaymentView.NavigationStarting += OnNavigationStarting;
		}

        /// <summary>
        /// 결제 시작 메서드
        /// </summary>
        /// <returns></returns>
        private async Task StartPaymentAsync()
		{
			var body = new { amount = amount};
			var res = await http.PostAsJsonAsync("http://localhost:4000/pay", body);
			res.EnsureSuccessStatusCode();

			var json = await res.Content.ReadFromJsonAsync<JsonElement>();
			orderId = json.GetProperty("redirectId").GetString()!;

			PaymentView.Source = new Uri("http://localhost:4000/checkout/" + orderId);
		}

        /// <summary>
        ///	WebView2 네비게이션 시작 이벤트 핸들러
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private async void OnNavigationStarting(object? s, CoreWebView2NavigationStartingEventArgs e)
		{
			if (!isRunning)
			{
				return;
			}
			isRunning = false;

            while (true)
			{
				var res = await http.GetFromJsonAsync<PaymentResponse>("http://localhost:4000/ispaying/" + orderId);
				if (res?.status == "paid")
				{
					MessageBox.Show("결제 완료");
					DialogResult = DialogResult.OK;
					this.Close();
					e.Cancel = false;
                    return;
				}
				else if (res?.status == "failed")
				{
					MessageBox.Show("결제 실패");
					DialogResult = DialogResult.Abort;
					this.Close();
					e.Cancel = true;
                    return;
				}
				// else 결제 진행 중
				await Task.Delay(500); // 0.5초 대기
            }
        }
	}
}
