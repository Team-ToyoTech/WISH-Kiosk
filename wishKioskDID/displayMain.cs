using System.Text.Json;
using System.Threading.Tasks;

namespace wishKioskDIDDisplay
{
    public partial class displayMain : Form
    {
        HttpClient httpClient = new();

        string serverUrl = "http://localhost:4000"; // 실제 서버 주소로 변경 필요

        private int[]? incompleteOrderNums, completeOrderNums;

        public displayMain()
        {
            InitializeComponent();
        }

        private void displayMain_Load(object sender, EventArgs e)
        {
            while (true)
            {
                _ = FetchAndDisplayValueAsync();
                Delay(500);
            }
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

        private async Task FetchAndDisplayValueAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(serverUrl + "/order/getid");
                response.EnsureSuccessStatusCode();

                string incompleteJson = await response.Content.ReadAsStringAsync();
                var incompleteResult = JsonSerializer.Deserialize<int[]>(incompleteJson);

                if (incompleteResult != null)
                {
                    if (!ArrCmp(incompleteResult, incompleteOrderNums))
                    {
                        incompleteOrderNums = incompleteResult;
                        orderIncompleteLabel.Text = "";
                        this.Invoke((Action)(() =>
                        {
                            if (incompleteResult.Length == 0)
                            {
                                orderIncompleteLabel.Text = "준비중인 주문이 없습니다";
                            }
                            else
                            {
                                int count = 0;
                                foreach (int orderNum in incompleteResult)
                                {
                                    count++;
                                    orderIncompleteLabel.Text += $" {orderNum} ";
                                    if (count % 5 == 0)
                                    {
                                        orderIncompleteLabel.Text += "\n";
                                    }
                                }
                            }
                        }));
                    }
                }
                else
                {
                    MessageBox.Show("서버 응답이 비어 있거나 형식이 올바르지 않습니다.", "파싱 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                response = await httpClient.GetAsync(serverUrl + "/order/complete/getid");
                response.EnsureSuccessStatusCode();

                string completeJson = await response.Content.ReadAsStringAsync();
                var completeResult = JsonSerializer.Deserialize<int[]>(completeJson);

                if (completeResult != null)
                {
                    if (!ArrCmp(completeResult, completeOrderNums))
                    {
                        completeOrderNums = completeResult;
                        orderCompleteLabel.Text = "";
                        this.Invoke((Action)(() =>
                        {
                            if (completeResult.Length == 0)
                            {
                                orderIncompleteLabel.Text = "";
                            }
                            else
                            {
                                foreach (int orderNum in completeResult)
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
