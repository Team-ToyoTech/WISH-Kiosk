using System.Text.Json;
using System.Threading.Tasks;

namespace wishKioskDIDDisplay
{
    public partial class displayMain : Form
    {
        HttpClient httpClient = new();

        string serverUrl = "http://localhost:4000"; // 실제 서버 주소로 변경 필요

        private record ResponseJson(int[] orderNum);

        public displayMain()
        {
            InitializeComponent();
        }

        private void displayMain_Load(object sender, EventArgs e)
        {
            while (true)
            {
                _ = FetchAndDisplayValueAsync();
                Task.Delay(500);
            }
        }

        private async Task FetchAndDisplayValueAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(serverUrl + "/order/getid");
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ResponseJson>(json);

                if (result != null)
                {
                    this.Invoke((Action)(() =>
                    {
                        int count = 0;
                        foreach (int orderNum in result.orderNum)
                        {
                            count++;
                            orderIncompleteLabel.Text += $" {orderNum} ";
                            if (count % 5 == 0)
                            {
                                orderIncompleteLabel.Text += "\n";
                            }
                        }
                    }));
                }
                else
                {
                    MessageBox.Show("서버 응답이 비어 있거나 형식이 올바르지 않습니다.", "파싱 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                response = await httpClient.GetAsync(serverUrl + "/order/complete/getid");
                response.EnsureSuccessStatusCode();

                json = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<ResponseJson>(json);

                if (result != null)
                {
                    this.Invoke((Action)(() =>
                    {
                        foreach (int orderNum in result.orderNum)
                        {
                            orderCompleteLabel.Text += $"{orderNum}\n";
                        }
                    }));
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
