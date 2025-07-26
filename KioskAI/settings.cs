using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace wishKiosk
{
	public partial class settings : Form
	{
		public PrintDocument printDoc = new PrintDocument();
		public int digitCount = 3;
		public string menuPath = string.Empty;

		public settings()
		{
			InitializeComponent();
		}

		private void printSettingsButton_Click(object sender, EventArgs e)
		{
			PrintDialog printDialog = new PrintDialog
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
			menuSettings menuSettings = new menuSettings();
			menuSettings.digitCount = digitCount;
			menuSettings.menuPath = menuPath;
			if (menuSettings.ShowDialog() == DialogResult.OK)
			{
				digitCount = menuSettings.digitCount;
			}
		}
	}
}
