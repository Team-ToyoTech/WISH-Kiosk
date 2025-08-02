using System.Text.Json;
using System.Net.Http.Json;

namespace wishKioskDIDReceive
{
    public partial class receiveMain : Form
    {
        public receiveMain()
        {
            InitializeComponent();
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                completeButton.PerformClick();
            }
        }

        private async Task CallComplete(string inputText)
        {
            HttpClient http = new HttpClient();
            string serverUrl = "http://localhost:4000"; // 실제 서버 주소로 변경 필요
            try
            {
                var json = await http.GetFromJsonAsync<JsonElement>(serverUrl + "/pay/complete/" + inputText);
                var result = json.GetProperty("status").ToString();

                if (result != null)
                {
                    if (result == "success")
                    {
                        stateLabel.Text = "주문 완료";
                        inputTextBox.Text = "";
                    }
                    else
                    {
                        stateLabel.Text = "잘못된 주문 번호입니다.";
                        inputTextBox.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("서버 응답이 비어 있습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"HTTP 오류: {ex.Message}", "오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show($"지원되지 않는 콘텐츠 형식: {ex.Message}", "오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"알 수 없는 오류: {ex.Message}", "오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void completeButton_Click(object sender, EventArgs e)
        {
            string inputText = inputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("주문 번호를 입력해주세요.");
                return;
            }
            _ = CallComplete(inputText);
        }
    }
}
