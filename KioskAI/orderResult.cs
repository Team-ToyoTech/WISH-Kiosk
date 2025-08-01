using Microsoft.VisualBasic;
using System.Drawing.Printing;

namespace wishKiosk
{
	public partial class orderResult : Form
	{
		private readonly Dictionary<int, string> menuMap;
		private readonly int[] menuNum;
		private readonly int[] price;
		private readonly List<int> menuOrderCount;
		private Dictionary<string, int> totalOrderResult = [];
		private uint orderNum = 0;
		private readonly Dictionary<string, int> menuPrice = [];
		public PrintDocument printDoc = new();

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
			MessageBox.Show("주문이 완료되었습니다.\n카운터에서 결제해 주세요.");
			// TODO: 주문번호 프린트
			this.Close();
		}
	}
}
