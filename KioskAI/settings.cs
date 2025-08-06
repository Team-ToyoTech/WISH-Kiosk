using Microsoft.VisualBasic;
using System.Diagnostics;
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

        private void infoButton_Click(object sender, EventArgs e)
        {
            ShowMid(
                "Project INFO\n\n" +
                "Made By Team ToyoTech\n" +
                "www.toyotech.dev\n\n" +
                "WI:SH KIOSK\n" +
                "Write It: Scan && Handle\n" +
                "www.github.com/Team-ToyoTech/WISH-Kiosk\n", "WISH INFO");
        }

        /// <summary>
        /// 가운데 정렬, 링크 포함된 MessageBox
        /// </summary>
        /// <param name="text">contents</param>
        /// <param name="caption">title</param>
        private static void ShowMid(string text, string caption = "")
        {
            using (Form dlg = new Form())
            {
                dlg.Text = caption;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = dlg.MinimizeBox = false;
                dlg.ShowInTaskbar = false;
                dlg.AutoSize = true;
                dlg.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                dlg.Padding = new Padding(10);

                dlg.Font = new Font(dlg.Font.FontFamily, 12f, FontStyle.Regular);

                FlowLayoutPanel contentPanel = BuildContentPanel(text);
                contentPanel.Anchor = AnchorStyles.None;

                var layout = new TableLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = 1,
                    RowCount = 1,
                    Dock = DockStyle.Fill
                };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                layout.Controls.Add(contentPanel, 0, 0);

                dlg.Controls.Add(layout);
                dlg.ShowDialog();
            }
        }

        /// <summary>
        /// text to labels, links to linkLabels
        /// </summary>
        /// <param name="text">contents</param>
        /// <returns></returns>
        private static FlowLayoutPanel BuildContentPanel(string text)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Dock = DockStyle.None,
                Margin = new Padding(0)
            };

            foreach (var raw in text.Split('\n'))
            {
                string line = raw.TrimEnd('\r');
                if (string.IsNullOrWhiteSpace(line))
                {
                    panel.Controls.Add(new System.Windows.Forms.Label { AutoSize = true, Height = 6 });
                    continue;
                }

                bool isUrl = line.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("www.", StringComparison.OrdinalIgnoreCase);

                if (isUrl)
                {
                    string url = line.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? line : $"https://{line}";

                    var link = new LinkLabel
                    {
                        Text = line,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter,
                        LinkBehavior = LinkBehavior.AlwaysUnderline,
                        Margin = new Padding(0, 2, 0, 2)
                    };
                    link.LinkClicked += (_, __) =>
                    {
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    };
                    link.Anchor = AnchorStyles.None;
                    panel.Controls.Add(link);
                }
                else
                {
                    panel.Controls.Add(new System.Windows.Forms.Label
                    {
                        Text = line,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter,
                        MaximumSize = new Size(400, 0),
                        Margin = new Padding(0, 2, 0, 2),
                        Anchor = AnchorStyles.None
                    });
                }
            }
            return panel;
        }
    }
}
