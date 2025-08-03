using Microsoft.VisualBasic;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace wishKiosk
{
    public partial class settings : Form
    {
        public PrintDocument printDoc = new();
        public int digitCount;
        public string? menuPath;
        public wishKiosk? WishKiosk;
        public readonly string passwordFilePath;

        public settings(string passwordFilePath)
        {
            InitializeComponent();
            this.passwordFilePath = passwordFilePath;
        }

        private void printSettingsButton_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new()
            {
                Document = printDoc,   // 인쇄할 문서 지정
                AllowSomePages = true, // 일부 페이지만 선택 가능
                AllowSelection = true, // 선택한 부분만 인쇄 가능
                UseEXDialog = true     // Windows 스타일의 대화상자 사용
            };
            printDialog.ShowDialog();
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(menuPath))
            {
                MessageBox.Show($"{menuPath} 파일이 존재하지 않습니다.");
                return;
            }

            menuSettings MenuSettings = new(WishKiosk, this)
            {
                digitCount = digitCount,
                menuPath = menuPath
            };
            if (MenuSettings.ShowDialog() == DialogResult.OK)
            {
                digitCount = MenuSettings.digitCount;
            }
        }

        private void settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WishKiosk == null)
            {
                throw new Exception("WishKiosk 인스턴스가 설정되지 않았습니다."); // MessageBox로 표시 고려, 사용자에게 노출?
            }
            WishKiosk.getData(this);
        }

        private void passwordChangeButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(passwordFilePath))
            {
                MessageBox.Show($"{passwordFilePath} 파일이 존재하지 않습니다.");
                using (var writer = new StreamWriter(passwordFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(wishKiosk.Sha256Hash("0000"));
                }
                return;
            }

            string input = Interaction.InputBox("새 비밀번호를 입력하세요: ", "비밀번호 변경");
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("비밀번호는 비워둘 수 없습니다.");
                return;
            }

            using (var writer = new StreamWriter(passwordFilePath, false, Encoding.UTF8))
            {
                writer.WriteLine(wishKiosk.Sha256Hash(input));
            }
            return;
        }
    }
}
