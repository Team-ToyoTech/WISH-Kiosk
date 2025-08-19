using Microsoft.VisualBasic;
using System.Globalization;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;

namespace wishKioskDIDDisplay
{
    public partial class displayMain : Form
    {
        private SpeechSynthesizer? synthesizer;
        private readonly HttpClient httpClient = new();
        private string serverUrl = "https://wish.toyotech.dev"; // 실제 서버 주소로 변경
        private readonly string serverUrlPath = "serverURL.dat";

        int[]? prevOrder, prevCompletedOrder;

        public displayMain()
        {
            InitializeComponent();
        }

        private void DisplayMain_Load(object sender, EventArgs e)
        {
            if (!File.Exists(serverUrlPath))
            {
                using (var writer = new StreamWriter(serverUrlPath, false, Encoding.UTF8))
                {
                    writer.WriteLine("https://wish.toyotech.dev"); // 기본 서버 URL
                }
            }
            serverUrl = File.ReadAllText(serverUrlPath).Trim();

            // SSML tts 초기화
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.Volume = 100; // 0 - 100
            synthesizer.Rate = 1;     // -10 - 10

            try
            {
                synthesizer.SelectVoiceByHints(
                    VoiceGender.NotSet,
                    VoiceAge.NotSet,
                    0,
                    new CultureInfo("ko-KR")
                );
            }
            catch
            {
                MessageBox.Show("TTS가 한국어를 제공하지 않습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            prevOrder = null;
            prevCompletedOrder = null;

            var orderTimer = new System.Windows.Forms.Timer { Interval = 500 };
            orderTimer.Tick += async (s, ev) => await GetOrders();
            orderTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            synthesizer?.Dispose();
            httpClient.Dispose();
            base.OnFormClosed(e);
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
                completeResp.EnsureSuccessStatusCode();

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
                                SpeakOrderAsync(order);
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
                this.Close();
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

        private void SpeakOrderAsync(int order)
        {
            if (synthesizer == null)
            {
                synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.Volume = 100; // 0 - 100
                synthesizer.Rate = 1;     // -10 - 10

                try
                {
                    synthesizer.SelectVoiceByHints(
                        VoiceGender.NotSet,
                        VoiceAge.NotSet,
                        0,
                        new CultureInfo("ko-KR")
                    );
                }
                catch
                {
                    MessageBox.Show("TTS가 한국어를 제공하지 않습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string ssml = $@"<speak version='1.0'
							xmlns='http://www.w3.org/2001/10/synthesis'
							xml:lang='ko-KR'>
							<say-as interpret-as='cardinal'>{order}</say-as>번 손님, 주문이 준비되었습니다.
							</speak>";

            synthesizer.SpeakSsmlAsync(ssml);
        }

        /// <summary>
        /// 주문 목록 표시
        /// </summary>
        /// <param name="orders"></param>
        private void DisplayOrders(int[] orders)
        {
            flowLayoutPanelOrders.SuspendLayout();
            flowLayoutPanelOrders.Controls.Clear();

            foreach (var order in orders)
            {
                var panel = new Panel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(12),
                    Margin = new Padding(8),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var numberLabel = new Label
                {
                    Text = order.ToString(),
                    Font = new Font("Segoe UI", 80, FontStyle.Bold),
                    AutoSize = true,
                    Cursor = Cursors.Hand,
                    Tag = order,
                    Location = new Point(0, 0)
                };
                numberLabel.BackColor = Color.Yellow;

                panel.Controls.Add(numberLabel);
                flowLayoutPanelOrders.Controls.Add(panel);
            }

            flowLayoutPanelOrders.ResumeLayout();
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
                    Font = new Font("Segoe UI", 80, FontStyle.Bold),
                    AutoSize = true,
                    Cursor = Cursors.Hand,
                    Tag = order
                };
                numberLabel.BackColor = Color.Lime;
                panel.Controls.Add(numberLabel);

                flowLayoutPanelCompletedOrders.Controls.Add(panel);
            }
        }

        private void displayMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.T)
            {
                while (true)
                {
                    string input = Interaction.InputBox("서버 주소를 입력하세요:", "서버 주소 설정", serverUrl);
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        break;
                    }
                    if (!input.StartsWith("http"))
                    {
                        MessageBox.Show("유효하지 않은 서버 주소입니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                    serverUrl = input.Trim().TrimEnd('/');
                    File.WriteAllText(serverUrlPath, serverUrl);
                    break;
                }
            }
        }
    }
}
