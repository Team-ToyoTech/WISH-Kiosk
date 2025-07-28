using System.Data;
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
        }

        private void menuSettings_Load(object sender, EventArgs e)
        {
            digitCntNumericUpDown.Value = digitCount;
            LoadCSV();
        }

        private void LoadCSV()
        {
            if (!File.Exists(menuPath))
            {
                MessageBox.Show($"{menuPath} 파일이 존재하지 않습니다.");
                return;
            }

            var table = new DataTable();

            try
            {
                using (var reader = new StreamReader(menuPath, Encoding.UTF8))
                {
                    if (reader.EndOfStream)
                    {
                        MessageBox.Show($"{menuPath} 파일이 비어 있습니다.");
                        return;
                    }

                    string headerLine = reader.ReadLine()!.TrimStart('\uFEFF');
                    string[] headers = headerLine.Split(',');

                    foreach (var h in headers)
                        table.Columns.Add(h.Trim());

                    while (!reader.EndOfStream)
                    {
                        string? line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] fields = line.Split(',');
                        if (fields.Length != table.Columns.Count)
                            continue;

                        table.Rows.Add(fields);
                    }
                }

                menuDataGridView.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{menuPath} 파일 읽기 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void SaveCSV()
        {
            if (menuDataGridView.DataSource is not DataTable dt || dt.Columns.Count == 0)
            {
                MessageBox.Show("저장할 데이터가 없습니다.");
                return;
            }

            try
            {
                var sb = new StringBuilder();

                var columnNames = dt.Columns
                                    .Cast<DataColumn>()
                                    .Select(col => col.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    var fields = dt.Columns
                                   .Cast<DataColumn>()
                                   .Select(col => row[col]?.ToString() ?? "");
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(menuPath, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"{menuPath}에 파일이 성공적으로 저장되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{menuPath}에 파일 저장 중 오류가 발생했습니다: {ex.Message}");
            }
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
