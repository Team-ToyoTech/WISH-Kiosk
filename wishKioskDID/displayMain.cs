using System.Text.Json;

namespace wishKioskDIDDisplay
{
    public partial class displayMain : Form
    {
        HttpClient httpClient = new();

        string serverUrl = "http://localhost:4000"; // 실제 서버 주소로 변경 필요

        private int[]? incompleteOrderNums, completeOrderNums;
        private int timeCheck = 0;

        public displayMain()
        {
            InitializeComponent();
        }

        private void displayMain_Load(object sender, EventArgs e)
        {
            this.Invoke((Action)(() =>
            {
                incompleteOrderNums = null;
                completeOrderNums = null;
            }));

            System.Windows.Forms.Timer orderTimer = new System.Windows.Forms.Timer
            {
                Interval = 500
            };
            orderTimer.Tick += async (s, ev) =>
            {
                await FetchAndDisplayValueAsync();
                timeCheck = (timeCheck + 1) % 4;
            };
            orderTimer.Start();
        }

        private void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }

        /// <summary>
        /// 배열 비교
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        private bool ArrCmp(int[]? arr1, int[]? arr2)
        {
            if (arr1 == null || arr2 == null)
            {
                return false;
            }
            if (arr1.Length != arr2.Length)
            {
                return false;
            }
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 준비중인 주문, 완료된 주문을 서버에서 가져와 표시
        /// </summary>
        /// <returns></returns>
        private async Task FetchAndDisplayValueAsync()
        {
            try
            {
                var resp = await httpClient.GetAsync(serverUrl + "/order/getid");
                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<int[]>(json);

                if (orders != null)
                {
                    if (!ArrCmp(orders, incompleteOrderNums))
                    {
                        completeOrderNums = orders;
                        orderCompleteLabel.Text = "";
                        this.Invoke((Action)(() =>
                        {
                            if (orders.Length == 0)
                            {
                                orderIncompleteLabel.Text = "";
                            }
                            else
                            {
                                foreach (int orderNum in orders)
                                {
                                    orderCompleteLabel.Text += $"{orderNum}\n";
                                }
                            }
                        }));
                    }
                }
                else
                {
                    MessageBox.Show("서버 응답이 비어 있거나 형식이 올바르지 않습니다.", "파싱 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                var compResp = await httpClient.GetAsync(serverUrl + "/order/complete/getid");
                resp.EnsureSuccessStatusCode();

                var compJson = await compResp.Content.ReadAsStringAsync();
                var compOrders = JsonSerializer.Deserialize<int[]>(compJson);

                if (compOrders != null)
                {
                    if (!ArrCmp(compOrders, completeOrderNums))
                    {
                        completeOrderNums = compOrders;
                        orderCompleteLabel.Text = "";
                        this.Invoke((Action)(() =>
                        {
                            if (compOrders.Length == 0)
                            {
                                orderCompleteLabel.Text = "";
                            }
                            else
                            {
                                foreach (int orderNum in compOrders)
                                {
                                    orderCompleteLabel.Text += $"{orderNum}\n";
                                }
                            }
                        }));
                    }
                }
                else
                {
                    MessageBox.Show("서버 응답이 비어 있거나 형식이 올바르지 않습니다.", "파싱 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (HttpRequestException hre)
            {
                MessageBox.Show($"서버 요청 오류: {hre.Message}", "HTTP 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException je)
            {
                MessageBox.Show($"데이터 파싱 오류: {je.Message}", "JSON 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"알 수 없는 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
