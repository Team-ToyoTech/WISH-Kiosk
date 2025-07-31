using System.Drawing.Printing;
using System.IO;

namespace wishKiosk
{
    public partial class settings : Form
    {
        public PrintDocument printDoc = new();
        public int digitCount;
        public string? menuPath;
        public wishKiosk? WishKiosk;

        public settings()
        {
            InitializeComponent();
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

            menuSettings MenuSettings = new()
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
            WishKiosk.getDigitCnt(this);
        }
    }
}
