using Microsoft.Web.WebView2.Core;
using System.Drawing.Printing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace wishKiosk
{
	public partial class payment : Form
	{
		private readonly int totalPrice;
        private float FontSize { get; set; } = 20f;
        private string? orderId;
        public uint? orderNum;
        public List<orderResult.OrderItem> orderItems = [];
        private readonly HttpClient http = new();

        private readonly string serverUrl = "https://wish.toyotech.dev"; // 실제 서버 주소로 변경 필요

        public PrintDocument printDoc = new();

        private record PaymentResponse(string status);

        private readonly Dictionary<string, int> menuPrice = [];

        private bool isRunning = true;

		public payment(int totalPrice, Dictionary<string, int> totalOrderResult, Dictionary<string, int> menuPrice)
		{
			InitializeComponent();
			this.totalPrice = totalPrice;
            this.menuPrice = menuPrice;

            foreach (var item in totalOrderResult)
            {
                orderItems.Add(new orderResult.OrderItem(item.Key, item.Value));
            }
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

        /// <summary>
        /// 결제 시작
        /// </summary>
        /// <returns></returns>
		private async Task StartPaymentAsync()
		{
            try
            {
                var body = new { amount = totalPrice };
                var res = await http.PostAsJsonAsync(serverUrl + "/pay", body);
                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                orderId = json.GetProperty("redirectId").GetString()!;

                paymentView.Source = new Uri(serverUrl + "/checkout/" + orderId);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("결제 시작 실패: " + ex.Message);
                DialogResult = DialogResult.Abort;
                this.Close();
            }
            catch (JsonException ex)
            {
                MessageBox.Show("응답 처리 오류: " + ex.Message);
                DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        /// <summary>
        /// 서버에 선택된 메뉴 전송, 주문 번호 받아오기
        /// </summary>
        /// <returns></returns>
        private async Task SendSelectedMenu()
        {
            try
            {
                var res = await http.PostAsJsonAsync(serverUrl + "/order/add/" + orderId, orderItems);
                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                orderNum = json.GetProperty("orderNumber").GetUInt32()!; // 주문 번호 받아오기
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("주문 정보 전송 실패: " + ex.Message);
                DialogResult = DialogResult.Abort;
                this.Close();
            }
            catch (JsonException ex)
            {
                MessageBox.Show("응답 처리 오류: " + ex.Message);
                DialogResult = DialogResult.Abort;
                this.Close();
            }
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
                try
                {
                    var res = await http.GetFromJsonAsync<PaymentResponse>(serverUrl + "/ispaying/" + orderId);
                    if (res?.status == "paid")
                    {
                        // MessageBox.Show("결제 완료"); // 디버깅용
                        await SendSelectedMenu(); // 주문 정보 서버에 전송

                        var msgRes = MessageBox.Show("영수증을 출력하시겠습니까?", "주문 완료", MessageBoxButtons.YesNo);
                        if (msgRes == DialogResult.Yes)
                        {
                            // 영수증 출력
                            printDoc.PrintPage += printDocument_PrintReceiptPage;
                            printDoc.Print();
                            printDoc.PrintPage -= printDocument_PrintReceiptPage;
                        }
                        else
                        {
                            // 주문 번호만 출력
                            printDoc.PrintPage += printDocument_PrintOrderNumPage;
                            printDoc.Print();
                            printDoc.PrintPage -= printDocument_PrintOrderNumPage;
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
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("서버 연결 실패: " + ex.Message);
                    DialogResult = DialogResult.Abort;
                    this.Close();
                    e.Cancel = true;
                    return;
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("응답 처리 오류: " + ex.Message);
                    DialogResult = DialogResult.Abort;
                    this.Close();
                    e.Cancel = true;
                    return;
                }
                await Task.Delay(500); // 0.5초 대기
			}
		}

        /// <summary>
        /// 주문번호만 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDocument_PrintOrderNumPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float left = e.MarginBounds.Left;
            float top = e.MarginBounds.Top;
            float width = e.MarginBounds.Width;
            float lineHeight = FontSize + 10;

            // 타이틀
            using (var titleFont = new Font("Arial", FontSize + 8, FontStyle.Bold))
            {
                string title = "WISH Kiosk";
                SizeF titleSize = g.MeasureString(title, titleFont);
                float x = left + (width - titleSize.Width) / 2;
                g.DrawString(title, titleFont, Brushes.Black, x, top);
            }

            float y = top + lineHeight * 2;

            using (var font = new Font("Arial", FontSize))
            {
                string dateLine = "[주 문] " + DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
                g.DrawString(dateLine, font, Brushes.Black, left, y);
                y += lineHeight;

                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += lineHeight * 3;

                // 주문번호
                using (var orderFont = new Font("Arial", FontSize + 4, FontStyle.Bold))
                {
                    string orderLine = "주문번호: " + (orderNum?.ToString() ?? "알 수 없음");
                    SizeF orderSize = g.MeasureString(orderLine, orderFont);
                    float xOrder = left + (width - orderSize.Width) / 2;
                    g.DrawString(orderLine, orderFont, Brushes.Black, xOrder, y);
                }
            }
        }

        /// <summary>
        /// 영수증 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDocument_PrintReceiptPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float left = e.MarginBounds.Left;
            float top = e.MarginBounds.Top;
            float width = e.MarginBounds.Width;
            float lineHeight = FontSize + 10;

            // 타이틀
            using (var titleFont = new Font("Arial", FontSize + 8, FontStyle.Bold))
            {
                string title = "WISH Kiosk";
                SizeF titleSize = g.MeasureString(title, titleFont);
                float x = left + (width - titleSize.Width) / 2;
                g.DrawString(title, titleFont, Brushes.Black, x, top);
            }

            float y = top + lineHeight * 2;

            using (var font = new Font("Arial", FontSize))
            {
                string dateLine = "[주 문] " + DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
                g.DrawString(dateLine, font, Brushes.Black, left, y);
                y += lineHeight;

                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += 4;

                g.DrawString("상품명", font, Brushes.Black, left, y);
                g.DrawString("단가", font, Brushes.Black, left + width * 0.5f, y);
                g.DrawString("수량", font, Brushes.Black, left + width * 0.7f, y);
                g.DrawString("금액", font, Brushes.Black, left + width * 0.85f, y);
                y += lineHeight;

                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += 4;

                // 주문 항목
                if (orderItems != null && orderItems.Count > 0)
                {
                    foreach (var item in orderItems)
                    {
                        g.DrawString(item.Name, font, Brushes.Black, left, y);
                        g.DrawString(menuPrice[item.Name].ToString("#,0"), font, Brushes.Black, left + width * 0.5f, y);
                        g.DrawString(item.Count.ToString(), font, Brushes.Black, left + width * 0.7f, y);
                        int totalPrice = menuPrice[item.Name] * item.Count;
                        g.DrawString(totalPrice.ToString("#,0"), font, Brushes.Black, left + width * 0.85f, y);
                        y += lineHeight;
                    }
                }
                else
                {
                    g.DrawString("합계", font, Brushes.Black, left, y);
                    g.DrawString(totalPrice.ToString("#,0"), font, Brushes.Black, left + width * 0.85f, y);
                    y += lineHeight;
                }

                y += lineHeight * orderItems.Count + 8;
                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += lineHeight;

                // 부가세, 합계
                int tax = totalPrice / 11; // 부가세
                DrawLabelValue(g, font, left, width, y, "부가세", (tax.ToString("#,0")));
                y += lineHeight;
                DrawLabelValue(g, font, left, width, y, "결제 금액", (totalPrice.ToString("#,0")));
                y += lineHeight * 3;

                // 주문번호
                using (var orderFont = new Font("Arial", FontSize + 4, FontStyle.Bold))
                {
                    string orderLine = "주문번호: " + (orderNum.ToString() ?? "알 수 없음");
                    SizeF orderSize = g.MeasureString(orderLine, orderFont);
                    float xOrder = left + (width - orderSize.Width) / 2;
                    g.DrawString(orderLine, orderFont, Brushes.Black, xOrder, y);
                }
            }
        }

        /// <summary>
        /// 프린트 디자인 그리기
        /// </summary>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="left"></param>
        /// <param name="width"></param>
        /// <param name="y"></param>
        /// <param name="label"></param>
        /// <param name="value"></param>
        private void DrawLabelValue(Graphics g, Font font, float left, float width, float y, string label, string value)
        {
            g.DrawString(label, font, Brushes.Black, left, y);
            var valueSize = g.MeasureString(value, font);
            g.DrawString(value, font, Brushes.Black, left + width - valueSize.Width, y);
        }
    }
}
