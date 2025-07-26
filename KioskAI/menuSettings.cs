using System.IO;
using System.Text;

namespace wishKiosk
{
	public partial class menuSettings : Form
	{
		public int digitCount = 3;
		public string menuPath = string.Empty;

		public menuSettings()
		{
			InitializeComponent();
			this.StartPosition = FormStartPosition.CenterParent;
			this.AcceptButton = okButton;
			this.CancelButton = cancelButton;

			LoadCSV();
		}

		private void LoadCSV()
		{
			if (!File.Exists(menuPath))
			{
				MessageBox.Show("menu.csv 파일이 존재하지 않습니다.");
				return;
			}

			menuDataGridView.Rows.Clear();
			menuDataGridView.Columns.Clear();

			string[] lines = File.ReadAllLines(menuPath, Encoding.UTF8);
			if (lines.Length == 0)
			{
				return;
			}

			string[] headers = lines[0].Split(',');
			foreach (string header in headers)
			{
				menuDataGridView.Columns.Add(header, header);
			}

			for (int i = 1; i < lines.Length; i++)
			{
				string[] cells = lines[i].Split(',');
				menuDataGridView.Rows.Add(cells);
			}
		}

		private void SaveCSV()
		{
			var sb = new StringBuilder();

			for (int i = 0; i < menuDataGridView.Columns.Count; i++)
			{
				sb.Append(menuDataGridView.Columns[i].HeaderText);
				if (i < menuDataGridView.Columns.Count - 1)
					sb.Append(",");
			}
			sb.AppendLine();

			foreach (DataGridViewRow row in menuDataGridView.Rows)
			{
				if (row.IsNewRow)
				{
					continue;
				}

				for (int i = 0; i < menuDataGridView.Columns.Count; i++)
				{
					string value = row.Cells[i].Value.ToString() ?? "";
					if (value.Contains(",") || value.Contains("\""))
					{
						value = "\"" + value.Replace("\"", "\"\"") + "\"";
					}
					sb.Append(value);
					if (i < menuDataGridView.Columns.Count - 1)
					{
						sb.Append(",");
					}
				}
				sb.AppendLine();
			}

			File.WriteAllText(menuPath, sb.ToString(), new UTF8Encoding(true));
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			SaveCSV();
		}

		private void digitCntNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			digitCount = (int)digitCntNumericUpDown.Value;
		}
	}
}
