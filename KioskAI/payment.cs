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
		
		private bool isRunning = true;

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

			await StartPaymentAsync();
			paymentView.NavigationStarting += OnNavigationStarting;
		}

		private async Task StartPaymentAsync()
		{
			var body = new { amount = amount};
			var res = await http.PostAsJsonAsync("http://localhost:4000/pay", body); // 실제 서버 주소로 변경 필요
            res.EnsureSuccessStatusCode();

			var json = await res.Content.ReadFromJsonAsync<JsonElement>();
			orderId = json.GetProperty("redirectId").GetString()!;

			paymentView.Source = new Uri("http://localhost:4000/checkout/" + orderId); // 실제 서버 주소로 변경 필요
        }

        /// <summary>
        /// 서버에서 결제 상태 확인, 결제 완료 또는 실패 시 처리
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private async void OnNavigationStarting(object? s, CoreWebView2NavigationStartingEventArgs e)
		{
			if (!isRunning)
				return;
			isRunning = false;

			while (true)
			{
				var res = await http.GetFromJsonAsync<PaymentResponse>("http://localhost:4000/ispaying/" + orderId); // 실제 서버 주소로 변경 필요
                if (res?.status == "paid")
				{
					MessageBox.Show("결제 완료");
					var msgRes = MessageBox.Show("영수증을 출력하시겠습니까?", "주문 완료", MessageBoxButtons.YesNo);
					if (msgRes == DialogResult.Yes)
					{
						// 영수증 출력
					}
					else
					{
						// 주문 번호만 출력
					}
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
				else
				{
					// 결제 진행 중
				}
				await Task.Delay(500); // 0.5초 대기
			}
		}
	}
}
