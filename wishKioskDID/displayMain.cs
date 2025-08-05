using System.Text.Json;
using System.Speech.Synthesis;

namespace wishKioskDIDDisplay
{
	public partial class displayMain : Form
	{
		private readonly HttpClient httpClient = new();
		private readonly string serverUrl = "http://localhost:4000"; // 실제 서버 주소로 변경

		int[]? prevOrder, prevCompletedOrder;

		public displayMain()
		{
			InitializeComponent();
			this.Load += ReceiveMain_Load;
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
		/// 배열 비교
		/// </summary>
		/// <param name="ord1"></param>
		/// <param name="ord2"></param>
		/// <returns></returns>
		private static bool ArrCmp(int[] ord1, int[] ord2)
		{
			if (ord1 == null || ord2 == null)
			{
				return false;
			}
			if (ord1.Length != ord2.Length)
			{
				return false;
			}
			for (int i = 0; i < ord1.Length; i++)
			{
				if (ord1[i] != ord2[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 주문 번호 목록(준비중, 완료) 가져오기
		/// </summary>
		/// <returns></returns>
		private async Task GetOrders()
		{
			try
			{
				var resp = await httpClient.GetAsync(serverUrl + "/order/getid");
				resp.EnsureSuccessStatusCode();

				var json = await resp.Content.ReadAsStringAsync();
				var orders = JsonSerializer.Deserialize<int[]>(json);

				var completeResp = await httpClient.GetAsync(serverUrl + "/order/complete/getid");
				resp.EnsureSuccessStatusCode();

				var completeJson = await completeResp.Content.ReadAsStringAsync();
				var completeOrders = JsonSerializer.Deserialize<int[]>(completeJson);

				if (orders != null && !ArrCmp(orders, prevOrder))
				{
					DisplayOrders(orders);
					prevOrder = orders;
				}
				if (completeOrders != null && !ArrCmp(completeOrders, prevCompletedOrder))
				{
					DisplayCompletedOrders(completeOrders);
					if (prevCompletedOrder != null)
					{
						foreach (var order in completeOrders)
						{
							if (!prevCompletedOrder.Contains(order))
							{
								using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
								{
									synthesizer.SetOutputToDefaultAudioDevice();
									synthesizer.Volume = 100;  // 0 - 100
									synthesizer.Rate = 1;      // -10 - 10

									string ssml = $@"<speak version='1.0'
											xmlns='http://www.w3.org/2001/10/synthesis'
											xml:lang='ko-KR'>
											<say-as interpret-as='cardinal'>{order}</say-as>번 손님, 주문이 준비되었습니다.
											</speak>";

                                    synthesizer.SpeakSsml(ssml);
                                }
							}
						}
					}
					prevCompletedOrder = completeOrders;
				}
			}
			catch (HttpRequestException ex)
			{
				MessageBox.Show("서버와 연결 실패: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				flowLayoutPanelOrders.Controls.Clear();
				flowLayoutPanelCompletedOrders.Controls.Clear();
			}
			catch (JsonException ex)
			{
				MessageBox.Show("데이터 처리 오류: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				flowLayoutPanelOrders.Controls.Clear();
				flowLayoutPanelCompletedOrders.Controls.Clear();
			}
			catch (Exception ex)
			{
				MessageBox.Show("알 수 없는 오류: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				flowLayoutPanelOrders.Controls.Clear();
				flowLayoutPanelCompletedOrders.Controls.Clear();
			}
		}

		/// <summary>
		/// 주문 목록 표시
		/// </summary>
		/// <param name="orders"></param>
		private void DisplayOrders(int[] orders)
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
					Text = order.ToString(),
					Font = new Font("Segoe UI", 50, FontStyle.Bold),
					AutoSize = true,
					Cursor = Cursors.Hand,
					Tag = order
				};
				numberLabel.BackColor = Color.Green;
				panel.Controls.Add(numberLabel);

				flowLayoutPanelOrders.Controls.Add(panel);
			}
		}

		/// <summary>
		/// 주문 완료 목록 표시
		/// </summary>
		/// <param name="orders"></param>
		private void DisplayCompletedOrders(int[] orders)
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
					Text = order.ToString(),
					Font = new Font("Segoe UI", 50, FontStyle.Bold),
					AutoSize = true,
					Cursor = Cursors.Hand,
					Tag = order
				};
				numberLabel.BackColor = Color.Yellow;
				panel.Controls.Add(numberLabel);

				flowLayoutPanelCompletedOrders.Controls.Add(panel);
			}
		}
	}
}
