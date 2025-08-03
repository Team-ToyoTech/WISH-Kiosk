using Microsoft.VisualBasic;
using System.Drawing.Printing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace wishKiosk
{
	public partial class orderResult : Form
	{
		private readonly Dictionary<int, string> menuMap;
		private readonly int[] menuNum;
		private readonly int[] price;
		private readonly List<int> menuOrderCount;
		private Dictionary<string, int> totalOrderResult = [];
		private uint? orderNum = 0;
		private string? orderId;
        private readonly Dictionary<string, int> menuPrice = [];
		public PrintDocument printDoc = new();

		private HttpClient http = new();
		private readonly string serverUrl = "http://localhost:4000"; // 실제 서버 주소로 변경 필요

		private List<OrderItem> orderItems = [];
        public record OrderItem(string Name, int Count);

        private int total = 0;

		public orderResult(
			Dictionary<int, string> menuMap,
			int[] menuNum,
			int[] price,
			List<int> menuOrderCount,
			Dictionary<string, int> menuPrice)
		{
			InitializeComponent();

			this.menuMap = menuMap;
			this.menuNum = menuNum;
			this.price = price;
			this.menuOrderCount = menuOrderCount;
			this.menuPrice = menuPrice;

            Load += orderResult_Load;
		}

		private void orderResult_Load(object sender, EventArgs e)
		{
			orderResultDataGridView.Columns.Add("MenuName", "메뉴명");
			orderResultDataGridView.Columns.Add("Quantity", "수량");
			orderResultDataGridView.Columns.Add("Price", "가격");

			var btnCol = new DataGridViewButtonColumn
			{
				HeaderText = "수정",
				Text = "수정",
				UseColumnTextForButtonValue = true
			};
			orderResultDataGridView.Columns.Add(btnCol);

			orderResultDataGridView.AllowUserToAddRows = false;
			orderResultDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
			orderResultDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			orderResultDataGridView.CurrentCell = null;
			orderResultDataGridView.Columns["MenuName"].ReadOnly = true;
			orderResultDataGridView.Columns["Quantity"].ReadOnly = true;
			orderResultDataGridView.Columns["Price"].ReadOnly = true;

			for (int i = 0; i < menuNum.Length; i++)
			{
				int qty = menuOrderCount[i];
				if (qty <= 0)
				{
					continue;
				}
                int key = menuNum[i];
				string name = menuMap[key];
				int linePrice = price[i] * qty;
				totalOrderResult[name] = qty;
                orderResultDataGridView.Rows.Add(name, qty, linePrice);
			}

			orderResultDataGridView.CellClick += orderResultDataGridView_CellClick;

			int originalWidth = orderResultDataGridView.Columns[3].Width;
			orderResultDataGridView.Columns[3].Width = originalWidth / 2;

			UpdateTotalLabel();
		}

		private void orderResultDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex == 3)
			{
				string oldQty = orderResultDataGridView.Rows[e.RowIndex].Cells["Quantity"].Value.ToString();
				string input = Interaction.InputBox(
					"새 개수를 입력하세요:",
					"개수 수정",
					oldQty);

				if (int.TryParse(input, out int newQty) && newQty >= 0)
				{
					menuOrderCount[e.RowIndex] = newQty;
					orderResultDataGridView.Rows[e.RowIndex].Cells["Quantity"].Value = newQty;
					decimal unitPrice = price[e.RowIndex];
					orderResultDataGridView.Rows[e.RowIndex].Cells["Price"].Value = unitPrice * newQty;
					totalOrderResult[orderResultDataGridView.Rows[e.RowIndex].Cells["MenuName"].Value.ToString()] = newQty;
                    UpdateTotalLabel();
				}
			}
		}

		/// <summary>
		/// 총액 Label 업데이트
		/// </summary>
		private void UpdateTotalLabel()
		{
			for (int i = 0; i < menuNum.Length; i++)
			{
				total += price[i] * menuOrderCount[i];
			}
			totalLabel.Text = $"총액: {total}원";
		}

		private void OrderButton_Click(object sender, EventArgs e)
		{
			payment Payment = new(total, totalOrderResult, menuPrice);
			Payment.printDoc = printDoc;
            var res = Payment.ShowDialog();
			if (res == DialogResult.OK)
			{
				MessageBox.Show("주문이 완료되었습니다.");
				if (uint.TryParse(Payment.orderNum?.ToString(), out uint orderNumber))
				{
					orderNum = orderNumber;
                }
				orderItems = Payment.orderItems;
                this.Close();
            }
			else
			{
				MessageBox.Show("결제를 실패하였습니다. 다시 진행해주세요");
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void CounterOrderButton_Click(object sender, EventArgs e)
		{
            _ = SendSelectedMenu();
            printDoc.PrintPage += printDocument_PrintReceiptPage;
            printDoc.Print();
            printDoc.PrintPage -= printDocument_PrintReceiptPage;
            this.Close();
		}

        /// <summary>
        /// 결제 정보 보내고 주문 번호 받아오기
        /// </summary>
        /// <returns></returns>
        private async Task SendSelectedMenu()
        {
            try
            {
                orderItems.Clear();
                foreach (var item in totalOrderResult)
                {
                    orderItems.Add(new orderResult.OrderItem(item.Key, item.Value));
                }

                var body = new { amount = total, orders = orderItems };
                var res = await http.PostAsJsonAsync(serverUrl + "/pay/counter", body);
                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                orderNum = json.GetProperty("orderNumber").GetUInt32()!; // 주문 번호 받아오기
				orderId = json.GetProperty("orderId").GetString()!; // 주문 ID 받아오기
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("실패: " + ex.Message);
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
        /// 영수증 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDocument_PrintReceiptPage(object sender, PrintPageEventArgs e)
        {
            float FontSize = 20f;
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
                string dateLine = "[미 결 제  주 문] " + DateTime.Now.ToString("yyyy-MM-dd  HH:mm");
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
                    g.DrawString(total.ToString("#,0"), font, Brushes.Black, left + width * 0.85f, y);
                    y += lineHeight;
                }

                y += lineHeight * orderItems.Count + 8;
                g.DrawLine(Pens.Black, left, y, left + width, y);
                y += lineHeight;

                // 부가세, 합계
                int tax = total / 11; // 부가세
                DrawLabelValue(g, font, left, width, y, "부가세", (tax.ToString("#,0")));
                y += lineHeight;
                DrawLabelValue(g, font, left, width, y, "결제 예정 금액", (total.ToString("#,0")));
                y += lineHeight;

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
