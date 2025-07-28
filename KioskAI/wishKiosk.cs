using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using QRCoder;
using WIA;
using ZXing;
using WishKiosk;

namespace wishKiosk
{
    public partial class wishKiosk : Form
    {
        public float FontSize { get; set; } = 30f; // 기본 글자 크기
        public PrintDocument printDoc = new PrintDocument();
        private Pen gridPen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
        private Brush blackBrush = Brushes.Black;
        private int boxSize = 40;
        private int spacing = 20;
        private int lineSpacing = 120;
        private int currentMenuIndex = 0;
        public int digitCount = 3; // 수기 숫자 칸 개수
        public string menuFilePath = "menu.csv";

        private string[] menu;
        private int[] price;

        public wishKiosk()
        {
            InitializeComponent();
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            printDoc.PrintPage += printDocument1_PrintPage;
            printDoc.Print();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Arial", FontSize);
            int startX = 100;
            int startY = 150;
            int availableHeight = e.MarginBounds.Bottom - startY;

            int linesPerPage = availableHeight / lineSpacing;
            int printedCount = 0;

            for (; currentMenuIndex < menu.Length; currentMenuIndex++)
            {
                if (printedCount >= linesPerPage)
                {
                    e.HasMorePages = true; // 다음 페이지 필요
                    break;
                }

                int lineY = startY + printedCount * lineSpacing;

                // 수기 숫자 칸
                List<int> boxXs = new List<int>();
                for (int i = 0; i < digitCount; i++)
                {
                    if (i == 0)
                        boxXs = new List<int> { startX };
                    else
                        boxXs.Add(boxXs[i - 1] + boxSize + spacing);
                }

                for (int j = 0; j < digitCount; j++)
                    e.Graphics?.DrawRectangle(gridPen, new Rectangle(boxXs[j], lineY, boxSize, boxSize));

                // 자리수 QR 코드 (수기 숫자 칸 위에 배치)
                List<string> digitLabels = new List<string>();
                for (int i = digitCount - 1; i >= 0; i--)
                {
                    digitLabels.Add(Convert.ToInt32(Math.Pow(10, i)).ToString());
                }

                float digitQRScale = 1.3f;
                int digitQRSize = (int)(boxSize * digitQRScale);

                for (int j = 0; j < digitCount; j++)
                {
                    string digitData = (currentMenuIndex + 1).ToString() + "-" + digitLabels[j];
                    Bitmap digitQR = GenerateQRCode(digitData);

                    int boxCenterX = boxXs[j] + boxSize / 2;
                    int qrX1 = boxCenterX - digitQRSize / 2;

                    int qrYOffset1 = (digitQRSize - boxSize) / 2;
                    int qrY1 = lineY - boxSize - 10 - qrYOffset1;

                    e.Graphics?.DrawImage(digitQR, new Rectangle(qrX1, qrY1, digitQRSize, digitQRSize));
                }

                // QR 코드
                string qrData = $"{"m" + (currentMenuIndex + 1).ToString()}";
                Bitmap qrImage = GenerateQRCode(qrData);
                float scaleFactor = 1.3f;
                int qrRenderSize = (int)(boxSize * scaleFactor);
                int qrX = boxXs[digitCount - 1] + boxSize + spacing * 2;
                int qrYOffset = (qrRenderSize - boxSize) / 2;
                int qrY = lineY - qrYOffset;
                e.Graphics?.DrawImage(qrImage, new Rectangle(qrX, qrY, qrRenderSize, qrRenderSize));

                // 메뉴명
                int textX = qrX + qrRenderSize + spacing;
                int pageRightMargin = e.MarginBounds.Right;
                int maxTextWidth = pageRightMargin - textX;

                float fontHeight = font.GetHeight(e.Graphics);
                float textY = lineY + (boxSize - fontHeight * 2) / 2;
                float maxTextHeight = fontHeight * 2 + 4;

                RectangleF textRect = new RectangleF(textX, textY, maxTextWidth, maxTextHeight);
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                e.Graphics.DrawString(menu[currentMenuIndex], font, blackBrush, textRect, format);

                printedCount++;
            }

            // 모든 메뉴를 출력했으면 종료
            if (currentMenuIndex >= menu.Length)
            {
                e.HasMorePages = false;
                currentMenuIndex = 0;  // 상태 초기화
            }

            font.Dispose();
        }

        private Bitmap GenerateQRCode(string data)
        {
            QRCodeGenerator qrGen = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGen.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            settings Settings = new settings();
            Settings.Show();
            Settings.printDoc = printDoc;
            Settings.digitCount = digitCount;
            if (!File.Exists(menuFilePath))
            {
                MessageBox.Show($"{menuFilePath} 파일이 존재하지 않습니다.");
            }
            else
            {
                Settings.menuPath = menuFilePath;
            }
            digitCount = Settings.digitCount;
        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            Bitmap? bitmap = ScanWithWia(); // WIA 스캐너로 이미지 가져오기, null 가능
            if (bitmap == null)
            {
                return;
            }

            var qrData = ExtractQrCodesWithSize(bitmap);

            var xTable = new Dictionary<int, Dictionary<int, float>>(); // line -> digitLevel -> x
            var yTable = new Dictionary<int, float>(); // line -> y
            var menuMap = new Dictionary<int, string>();
            for (int i = 1; i <= menu.Length; i++)
            {
                menuMap[i] = menu[i - 1];
            }

            Bitmap debugBitmap = new Bitmap(bitmap);
            Graphics g = Graphics.FromImage(debugBitmap);
            Pen roiPen = new Pen(Color.Red, 2);

            Dictionary<(int menuNum, int level), SizeF> xQrSizes = new();
            Dictionary<int, SizeF> yQrSizes = new();

            foreach (var (text, point, size) in qrData)
            {
                if (text.Contains("-"))
                {
                    var parts = text.Split('-');
                    if (int.TryParse(parts[0], out int line) && int.TryParse(parts[1], out int digitLevel))
                    {
                        if (!xTable.ContainsKey(line))
                        {
                            xTable[line] = new Dictionary<int, float>();
                        }
                        xTable[line][digitLevel] = point.X;
                        xQrSizes[(line, digitLevel)] = size;
                    }
                }
                else if (text.StartsWith("m"))
                {
                    if (int.TryParse(text.Substring(1), out int menuNum))
                    {
                        yTable[menuNum] = point.Y;
                        yQrSizes[menuNum] = size;
                    }
                }
            }

            foreach (var menuEntry in yTable)
            {
                int menuNum = menuEntry.Key;
                float y = menuEntry.Value;

                if (!menuMap.ContainsKey(menuNum))
                {
                    continue;
                }

                if (xTable.ContainsKey(menuNum))
                {
                    foreach (int level in new[] { 100, 10, 1 })
                    {
                        if (xTable[menuNum].ContainsKey(level))
                        {
                            float x = xTable[menuNum][level];

                            float qrWidth = xQrSizes.TryGetValue((menuNum, level), out var sizeX) ? sizeX.Width : 40;
                            float qrHeight = yQrSizes.TryGetValue(menuNum, out var sizeY) ? sizeY.Height : 40;

                            Rectangle roi = new Rectangle(
                                (int)(x - qrWidth),
                                (int)(y - qrHeight),
                                (int)qrWidth * 2,
                                (int)qrHeight * 2
                            );

                            g.DrawRectangle(roiPen, roi);
                            g.DrawString($"{menuNum}-{level}", new Font("Arial", 10), Brushes.Red, roi.Location);
                        }
                    }
                }
            }

            debugBitmap.Save("debug_output.png", System.Drawing.Imaging.ImageFormat.Png);
            // MessageBox.Show("ROI가 표시된 이미지가 저장되었습니다.");

            foreach (var menuEntry in yTable)
            {
                int menuNum = menuEntry.Key;
                float y = menuEntry.Value;

                if (!menuMap.ContainsKey(menuNum))
                {
                    continue;
                }

                string orderCount = "";
                if (xTable.ContainsKey(menuNum))
                {
                    foreach (int level in new[] { 100, 10, 1 })
                    {
                        if (xTable[menuNum].ContainsKey(level))
                        {
                            float x = xTable[menuNum][level];
                            float qrWidth = xQrSizes.TryGetValue((menuNum, level), out var xSize) ? xSize.Width : 40;
                            float qrHeight = yQrSizes.TryGetValue(menuNum, out var ySize) ? ySize.Height : 40;

                            Rectangle roi = new Rectangle(
                                (int)(x - qrWidth),
                                (int)(y - qrHeight),
                                (int)qrWidth * 2,
                                (int)qrHeight * 2
                            );

                            string digit = OCRDigit(bitmap, roi);
                            // MessageBox.Show($"OCR result at ({x}, {y}) = '{digit}'");
                            orderCount += digit;
                        }
                    }
                }

                orderCount = string.IsNullOrEmpty(orderCount) ? "0" : orderCount;
                MessageBox.Show($"{menuMap[menuNum]}를 {orderCount}개 주문했습니다.");
            }
            bitmap.Dispose();
        }

        public static List<(string text, PointF point, SizeF size)> ExtractQrCodesWithSize(Bitmap bitmap)
        {
            var results = new List<(string, PointF, SizeF)>();
            var reader = new ZXing.BarcodeReaderGeneric();
            var resultArray = reader.DecodeMultiple(bitmap);

            if (resultArray != null)
            {
                foreach (var r in resultArray)
                {
                    if (r.ResultPoints.Length >= 2)
                    {
                        var pt1 = r.ResultPoints[0];
                        var pt2 = r.ResultPoints[2];
                        float centerX = (pt1.X + pt2.X) / 2;
                        float centerY = (pt1.Y + pt2.Y) / 2;
                        float width = Math.Abs(pt1.X - pt2.X);
                        float height = Math.Abs(pt1.Y - pt2.Y);

                        results.Add((r.Text.Trim(), new PointF(centerX, centerY), new SizeF(width, height)));
                    }
                }
            }

            return results;
        }

        static int i = 0;

        static Rectangle ClampROI(Rectangle roi, Size bitmapSize)
        {
            int x = Math.Max(0, roi.X);
            int y = Math.Max(0, roi.Y);
            int width = Math.Min(roi.Width, bitmapSize.Width - x);
            int height = Math.Min(roi.Height, bitmapSize.Height - y);

            return new Rectangle(x, y, width, height);
        }

        static string OCRDigit(Bitmap bitmap, Rectangle roi)
        {
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    Rectangle safeROI = ClampROI(roi, bitmap.Size);
                    Bitmap cropped = new Bitmap(safeROI.Width, safeROI.Height, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(cropped))
                    {
                        g.DrawImage(bitmap, new Rectangle(0, 0, cropped.Width, cropped.Height), safeROI, GraphicsUnit.Pixel);
                    }

                    Bitmap preprocessed = PreprocessImage(cropped);

                    preprocessed.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Position = 0;

                    var imageBytes = memoryStream.ToArray();
                    MNIST.ModelInput sampleData = new MNIST.ModelInput()
                    {
                        ImageSource = imageBytes,
                    };
                    var sortedScoresWithLabel = MNIST.PredictAllLabels(sampleData);
                    return sortedScoresWithLabel.First().Key == "NaN" ? "" : sortedScoresWithLabel.First().Key;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"OCR Error: {ex.Message}");
                    return "";
                }
            }
        }

        static Bitmap PreprocessImage(Bitmap input)
        {
            Bitmap result = new Bitmap(input.Width, input.Height);
            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Color c = input.GetPixel(x, y);
                    int gray = (c.R + c.G + c.B) / 3;
                    Color newColor = gray > 150 ? Color.White : Color.Black;
                    result.SetPixel(x, y, newColor);
                }
            }
            return result;
        }

        public static Bitmap? ScanWithWia()
        {
            const string FORMAT_PNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";

            var manager = new DeviceManager();

            DeviceInfo? scannerInfo = null;
            foreach (DeviceInfo info in manager.DeviceInfos)
            {
                if (info.Type == WiaDeviceType.ScannerDeviceType &&
                    info.Properties["Name"].get_Value().ToString().Contains("ES-50"))
                {
                    scannerInfo = info;
                    break;
                }
            }

            if (scannerInfo == null)
            {
                MessageBox.Show("ES-50 스캐너를 찾을 수 없습니다.");
                return null;
            }

            var device = scannerInfo.Connect();
            var item = device.Items[1];

            // 컬러, 해상도 설정
            SetWiaProperty(item, 6147, 500); // DPI X
            SetWiaProperty(item, 6148, 500); // DPI Y

            try
            {
                var imgFile = (ImageFile)item.Transfer(FORMAT_PNG);
                var data = (byte[])imgFile.FileData.get_BinaryData();

                using (var ms = new MemoryStream(data))
                {
                    return new Bitmap(ms);
                }
            }
            catch
            {
                MessageBox.Show("용지를 스캐너에 끼워주세요.");
            }
            return null;
        }

        private static void SetWiaProperty(Item item, int id, object value)
        {
            foreach (Property prop in item.Properties)
            {
                if (prop.PropertyID == id)
                {
                    prop.set_Value(ref value);
                    return;
                }
            }

        }

        private void wishKiosk_Load(object sender, EventArgs e)
        {
            // 메뉴, 가격 가져오기
            if (!File.Exists(menuFilePath))
            {
                MessageBox.Show("menu.csv 파일이 존재하지 않습니다.");
                return;
            }

            string[] lines = File.ReadAllLines(menuFilePath);

            List<string> menuList = new List<string>();
            List<int> priceList = new List<int>();

            for (int i = 1; i < lines.Length; i++)
            {
                string[] splt = lines[i].Trim().Split(',');
                string menuName = splt[0];
                try
                {
                    int priceValue = int.Parse(splt[1]);
                    menuList.Add(menuName);
                    priceList.Add(priceValue);
                }
                catch
                {
                    MessageBox.Show($"{i}행의 가격이 잘못된 형식입니다.");
                    return;
                }
            }

            menu = menuList.ToArray();
            price = priceList.ToArray();
        }
    }
}