using Microsoft.VisualBasic;

namespace WishKiosk
{
    public partial class OrderResult : Form
    {
        private readonly Dictionary<int, string> menuMap;
        private readonly int[] menuNum;
        private readonly int[] price;
        private readonly List<int> menuOrderCount;

        private int total = 0;

        public OrderResult(
            Dictionary<int, string> menuMap,
            int[] menuNum,
            int[] price,
            List<int> menuOrderCount)
        {
            InitializeComponent();

            this.menuMap = menuMap;
            this.menuNum = menuNum;
            this.price = price;
            this.menuOrderCount = menuOrderCount;

            Load += OrderResult_Load;
        }

        private void OrderResult_Load(object sender, EventArgs e)
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
            // orderResultDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            orderResultDataGridView.CurrentCell = null;
            orderResultDataGridView.Columns["MenuName"].ReadOnly = true;
            orderResultDataGridView.Columns["Quantity"].ReadOnly = true;
            orderResultDataGridView.Columns["Price"].ReadOnly = true;

            for (int i = 0; i < menuNum.Length; i++)
            {
                int key = menuNum[i];
                string name = menuMap[key];
                int qty = menuOrderCount[i];
                int linePrice = price[i] * qty;
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

        private void orderButton_Click(object sender, EventArgs e)
        {
            Payment payment = new(total);
            var res = payment.ShowDialog();
            if (res == DialogResult.OK)
            {
                MessageBox.Show("주문이 완료되었습니다.");
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

        private void CounterorderButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("주문이 완료되었습니다.\n주문이 완료되면 카운터에서 결제해 주세요");
            this.Close();
        }
    }
}
