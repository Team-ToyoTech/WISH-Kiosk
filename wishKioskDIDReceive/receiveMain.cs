using Microsoft.VisualBasic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Media;

namespace wishKioskDIDReceive
{
	public partial class receiveMain : Form
	{
		private readonly HttpClient httpClient = new HttpClient();
		private readonly string serverUrl = "https://wish.toyotech.dev"; // 실제 서버 주소로 변경

		private record Order(List<OrderItem> order, int orderNumber, string paid);
		private record OrderItem(string Name, int Count);

		List<Order>? prevOrder, prevCompletedOrder;

        private readonly string soundFilePath = "sound.wav";

        public receiveMain()
		{
			InitializeComponent();
		}

		private void ReceiveMain_Load(object sender, EventArgs e)
		{
			prevOrder = null;
			prevCompletedOrder = null;

			var orderTimer = new System.Windows.Forms.Timer { Interval = 500 };
			orderTimer.Tick += async (s, ev) => await GetOrders();
			orderTimer.Start();
		}

		/// <summary>
		/// List<Order> 비교
		/// </summary>
		/// <param name="ord1"></param>
		/// <param name="ord2"></param>
		/// <returns></returns>
		private static bool OrderCmp(List<Order> ord1, List<Order> ord2)
		{
			if (ord1 == null || ord2 == null)
			{
				return false;
			}
			if (ord1.Count != ord2.Count)
			{
				return false;
			}
			for (int i = 0; i < ord1.Count; i++)
			{
				if (ord1[i].orderNumber != ord2[i].orderNumber ||
					ord1[i].order.Count != ord2[i].order.Count ||
					ord1[i].paid != ord2[i].paid)
				{
					return false;
				}
				if (ord1[i].order.Count != ord2[i].order.Count)
				{
					return false;
				}
				for (int j = 0; j < ord1[i].order.Count; j++)
				{
					if (ord1[i].order[j].Name != ord2[i].order[j].Name ||
						ord1[i].order[j].Count != ord2[i].order[j].Count)
					{
						return false;
					}
				}
			}
			return true;
		}

        /// <summary>
        /// 주문 목록(준비중, 완료) 가져오기
        /// </summary>
        /// <returns></returns>
        private async Task GetOrders()
		{
			try
			{
				var resp = await httpClient.GetAsync(serverUrl + "/order/get");
				resp.EnsureSuccessStatusCode();

				var json = await resp.Content.ReadAsStringAsync();
				var orders = JsonSerializer.Deserialize<List<Order>>(json);

				var completeResp = await httpClient.GetAsync(serverUrl + "/order/complete/get");
				resp.EnsureSuccessStatusCode();

				var completeJson = await completeResp.Content.ReadAsStringAsync();
				var completeOrders = JsonSerializer.Deserialize<List<Order>>(completeJson);

				if (orders != null && !OrderCmp(orders, prevOrder))
				{
					DisplayOrders(orders);
                    if (prevOrder != null)
                    {
                        foreach (var order in orders)
                        {
                            if (!prevOrder.Contains(order))
                            {
                                SoundPlayer player = new SoundPlayer(soundFilePath);
                                player.Load();
                                player.Play(); // async
                            }
                        }
                    }
                    prevOrder = orders;
				}
				if (completeOrders != null && !OrderCmp(completeOrders, prevCompletedOrder))
				{
					DisplayCompletedOrders(completeOrders);
					prevCompletedOrder = completeOrders;
				}
			}
			catch (HttpRequestException ex)
			{
				MessageBox.Show($"서버 연결 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				flowLayoutPanelOrders.Controls.Clear();
				flowLayoutPanelCompletedOrders.Controls.Clear();
			}
			catch (JsonException ex)
			{
				MessageBox.Show($"JSON 파싱 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				flowLayoutPanelOrders.Controls.Clear();
				flowLayoutPanelCompletedOrders.Controls.Clear();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"알 수 없는 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				flowLayoutPanelOrders.Controls.Clear();
				flowLayoutPanelCompletedOrders.Controls.Clear();
            }
        }

        /// <summary>
        /// 주문 목록 표시
        /// </summary>
        /// <param name="orders"></param>
        private void DisplayOrders(List<Order> orders)
		{
			flowLayoutPanelOrders.Controls.Clear();
			foreach (var order in orders)
			{
				var panel = new Panel
				{
					AutoSize = true,
					AutoSizeMode = AutoSizeMode.GrowAndShrink,
					Padding = new Padding(12),
					Margin = new Padding(8),
					BorderStyle = BorderStyle.FixedSingle,
					MaximumSize = new Size(flowLayoutPanelOrders.ClientSize.Width - 20, 0)
				};

				// 주문 번호
				var numberLabel = new Label
				{
					Text = order.orderNumber.ToString(),
					Font = new Font("Segoe UI", 36, FontStyle.Bold),
					AutoSize = true,
					Cursor = Cursors.Hand,
					Tag = order
				};
				numberLabel.BackColor = Color.Yellow;
				numberLabel.Click += numberLabel_Click;
				panel.Controls.Add(numberLabel);

				// 주문 항목
				int offsetY = numberLabel.Bottom + 8;
				foreach (var item in order.order)
				{
					var lblItem = new Label
					{
						Text = $"{item.Name}  {item.Count}",
						Font = new Font("Segoe UI", 18),
						Location = new Point(0, offsetY),
						AutoSize = true
					};
					panel.Controls.Add(lblItem);
					offsetY += lblItem.Height + 4;
				}

				flowLayoutPanelOrders.Controls.Add(panel);
			}
		}

        /// <summary>
        /// 주문 완료 목록 표시
        /// </summary>
        /// <param name="orders"></param>
        private void DisplayCompletedOrders(List<Order> orders)
		{
			flowLayoutPanelCompletedOrders.Controls.Clear();
			foreach (var order in orders)
			{
				var panel = new Panel
				{
					AutoSize = true,
					AutoSizeMode = AutoSizeMode.GrowAndShrink,
					Padding = new Padding(12),
					Margin = new Padding(8),
					BorderStyle = BorderStyle.FixedSingle,
					MaximumSize = new Size(flowLayoutPanelCompletedOrders.ClientSize.Width - 20, 0)
				};

				// 주문 번호
				var numberLabel = new Label
				{
					Text = order.orderNumber.ToString(),
					Font = new Font("Segoe UI", 36, FontStyle.Bold),
					AutoSize = true,
					Cursor = Cursors.Hand,
					Tag = order
				};
				numberLabel.BackColor = Color.Lime;
				numberLabel.Click += cancelLabel_Click;
				panel.Controls.Add(numberLabel);

				// 주문 항목
				int offsetY = numberLabel.Bottom + 8;
				foreach (var item in order.order)
				{
					var lblItem = new Label
					{
						Text = $"{item.Name}  {item.Count}",
						Font = new Font("Segoe UI", 18),
						Location = new Point(0, offsetY),
						AutoSize = true
					};
					panel.Controls.Add(lblItem);
					offsetY += lblItem.Height + 4;
				}

				flowLayoutPanelCompletedOrders.Controls.Add(panel);
			}
		}

		/// <summary>
		/// 주문 완료 취소
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private async void cancelLabel_Click(object sender, EventArgs e)
        {
            if (sender is Label lbl && lbl.Tag is Order order)
            {
                string details = string.Join(
                    Environment.NewLine,
                    order.order.Select(i => $"{i.Name}: {i.Count}")
                );

                var detailForm = new Form
                {
                    Text = $"주문 상세 정보 – {order.orderNumber}",
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent
                };

                var detailLabel = new Label
                {
                    Text = details,
                    Font = new Font("Segoe UI", 16),
                    AutoSize = true,
                    MaximumSize = new Size(430, 0),
                    Location = new Point(15, 15)
                };
                detailForm.Controls.Add(detailLabel);

                int btnY = detailLabel.Bottom + 15;
                int startX = 15;

                var btnTake = new Button
                {
                    Text = "주문 수령",
                    Font = new Font("Segoe UI", 14),
                    Size = new Size(160, 50),
                    Location = new Point(startX, btnY)
                };
                btnTake.Click += async (s, ev) =>
                {
                    await OrderTaken(order.orderNumber.ToString());
                    detailForm.Close();
                };
                detailForm.Controls.Add(btnTake);

                var btnComplete = new Button
                {
                    Text = "완료 취소",
                    Font = new Font("Segoe UI", 14),
                    Size = new Size(160, 50),
                    Location = new Point(btnTake.Right + 10, btnY)
                };
                btnComplete.Click += async (s, ev) =>
                {
                    await CancelComplete(order.orderNumber.ToString());
                    detailForm.Close();
                };
                detailForm.Controls.Add(btnComplete);

                var btnClose = new Button
                {
                    Text = "닫기",
                    Font = new Font("Segoe UI", 14),
                    Size = new Size(120, 50),
                    Location = new Point(btnComplete.Right + 10, btnY)
                };
                btnClose.Click += (s, ev) => detailForm.Close();
                detailForm.Controls.Add(btnClose);

                detailForm.ShowDialog();
            }
        }

        /// <summary>
		/// 주문 완료 취소 서버 호출
		/// </summary>
		/// <param name="inputText"></param>
		/// <returns></returns>
        private async Task CancelComplete(string inputText)
		{
			try
			{
				var json = await httpClient.GetFromJsonAsync<JsonElement>(serverUrl + "/order/complete/cancel/" + inputText);
				var status = json.GetProperty("status").ToString()!;
				if (status == "fail")
				{
					MessageBox.Show("취소에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        /// <summary>
        /// 주문 수령 서버 호출
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        private async Task OrderTaken(string inputText)
		{
			try
			{
				var json = await httpClient.DeleteFromJsonAsync<JsonElement>(serverUrl + "/order/del/" + inputText);
				var status = json.GetProperty("status").ToString()!;
				if (status == "fail")
				{
					MessageBox.Show("주문 수령 요청에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void numberLabel_Click(object sender, EventArgs e)
		{
			if (sender is Label lbl && lbl.Tag is Order order)
			{
				string details = string.Join(
					Environment.NewLine,
					order.order.Select(i => $"{i.Name}: {i.Count}")
				);

				var detailForm = new Form
				{
					Text = $"주문 상세 정보 – {order.orderNumber}",
					Size = new Size(450, 350),
					FormBorderStyle = FormBorderStyle.FixedDialog,
					StartPosition = FormStartPosition.CenterParent
				};

				var detailLabel = new Label
				{
					Text = details,
					Font = new Font("Segoe UI", 16),
					AutoSize = false,
					Size = new Size(410, 200),
					Location = new Point(15, 15)
				};
				detailForm.Controls.Add(detailLabel);

				var btnComplete = new Button
				{
					Text = "주문 완료",
					Font = new Font("Segoe UI", 14),
					Size = new Size(160, 50),
					Location = new Point(70, 240)
				};
				btnComplete.Click += async (s, ev) =>
				{
					await CallComplete(order.orderNumber.ToString());
					detailForm.Close();
				};
				detailForm.Controls.Add(btnComplete);

				var btnClose = new Button
				{
					Text = "닫기",
					Font = new Font("Segoe UI", 14),
					Size = new Size(120, 50),
					Location = new Point(250, 240)
				};
				btnClose.Click += (s, ev) => detailForm.Close();
				detailForm.Controls.Add(btnClose);

				detailForm.ShowDialog();
			}
		}

        /// <summary>
        /// 주문 완료 처리
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        private async Task CallComplete(string inputText)
        {
            try
            {
                var json = await httpClient.GetFromJsonAsync<JsonElement>(serverUrl + "/order/complete/set/" + inputText);
                var status = json.GetProperty("status").ToString();
                if (status != "success")
                {
                    MessageBox.Show("잘못된 주문 번호입니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 카운터 주문 완료 처리
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        private async Task CallCounterComplete(string inputText)
		{
			try
			{
				var json = await httpClient.GetFromJsonAsync<JsonElement>(serverUrl + "/pay/complete/" + inputText);
				var status = json.GetProperty("status").ToString();
				if (status != "success")
				{
					MessageBox.Show("잘못된 주문 번호입니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void completeButton_Click(object sender, EventArgs e)
		{
			string inputText = Interaction.InputBox("주문번호를 입력하세요:", "카운터 결제 확인");
			if (!string.IsNullOrWhiteSpace(inputText))
			{
				_ = CallCounterComplete(inputText);
			}
		}
	}
}
