using Microsoft.VisualBasic;
using System.Drawing.Printing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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

		private async Task CounterOrderButton_Click(object sender, EventArgs e)
		{
            await SendSelectedMenu();
            printDoc.PrintPage += printDocument_PrintOrderNumPage;
            printDoc.Print();
            printDoc.PrintPage -= printDocument_PrintOrderNumPage;
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
                var body = new { amount = total };
                var res = await http.PostAsJsonAsync(serverUrl + "/pay/counter", body);
                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                var orderId = json.GetProperty("redirectId").GetString()!;

                var orderNumRes = await http.PostAsJsonAsync(serverUrl + "/order/add/" + orderId, orderItems);
                res.EnsureSuccessStatusCode();

                var orderNumJson = await res.Content.ReadFromJsonAsync<JsonElement>();
                orderNum = json.GetProperty("orderNumber").GetUInt32()!; // 주문 번호 받아오기
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
        /// 주문번호만 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDocument_PrintOrderNumPage(object sender, PrintPageEventArgs e)
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
    }
}
