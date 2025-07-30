using QRCoder;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using WIA;
using WishKiosk;
using ZXing;

namespace wishKiosk
{
	public partial class wishKiosk : Form
	{
		public float FontSize { get; set; } = 30f;
		public PrintDocument printDoc = new PrintDocument();
		private Pen gridPen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
		private Brush blackBrush = Brushes.Black;
		private int boxSize = 40;
		private int spacing = 20;
		private int lineSpacing = 120;
		private int currentMenuIndex = 0;
		public int digitCount = 3; // 숫자 칸 개수
		public string menuFilePath = "menu.csv";
		public string digitFilePath = "digit.dat";

		private string?[] menu;
		private int?[] price;

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
					e.HasMorePages = true;
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

				e.Graphics.DrawString(menu[currentMenuIndex] ?? "", font, blackBrush, textRect, format);

				printedCount++;
			}

			if (currentMenuIndex >= menu.Length)
			{
				e.HasMorePages = false;
				currentMenuIndex = 0;
			}

			font.Dispose();
		}

		/// <summary>
		/// 주어진 텍스트 QR code 변환
		/// </summary>
		/// <param name="data">QR code contents</param>
		/// <returns>QR code img</returns>
		private Bitmap GenerateQRCode(string data)
		{
			QRCodeGenerator qrGen = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGen.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);
			return qrCode.GetGraphic(20);
		}

		private void settingsButton_Click(object sender, EventArgs e)
		{
			settings Settings = new()
			{
				printDoc = printDoc,
				digitCount = digitCount,
				WishKiosk = this
			};
			if (!File.Exists(menuFilePath))
			{
				MessageBox.Show($"{menuFilePath} 파일이 존재하지 않습니다.");
			}
			else
			{
				Settings.menuPath = menuFilePath;
			}
			Settings.Show();
		}

        /// <summary>
        /// settings에서 digitCount 받아와서 digit.dat 에 기록
        /// </summary>
        /// <param name="Settings">settings Form</param>
        public void getDigitCnt(settings Settings)
		{
			digitCount = Settings.digitCount;
            if (!File.Exists(digitFilePath))
            {
                MessageBox.Show($"{digitFilePath} 파일이 존재하지 않습니다.");
            }
            else
            {
                File.WriteAllText(digitFilePath, digitCount.ToString());
            }
        }

		/*
        /// <summary>
        /// 고대비 이미지로 변환
        /// </summary>
        /// <param name="original">original image</param>
        /// <returns>high contrast img</returns>
        public static Bitmap EnhanceContrast(Bitmap original)
        {
            Bitmap gray = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int luminance = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                    gray.SetPixel(x, y, Color.FromArgb(luminance, luminance, luminance));
                }
            }

            int threshold = 128;
            Bitmap binary = new Bitmap(gray.Width, gray.Height);
            for (int y = 0; y < gray.Height; y++)
            {
                for (int x = 0; x < gray.Width; x++)
                {
                    Color grayPixel = gray.GetPixel(x, y);
                    int val = grayPixel.R > threshold ? 255 : 0;
                    binary.SetPixel(x, y, Color.FromArgb(val, val, val));
                }
            }

            return binary;
        } */

		/// <summary>
		/// 빠른 고대비 변환
		/// </summary>
		/// <param name="source">original img</param>
		/// <param name="threshold">기준값</param>
		/// <returns>high contrast img</returns>
        public static Bitmap EnhanceContrastFast(Bitmap source, int threshold = 128)
        {
            Bitmap grayBmp = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            BitmapData srcData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dstData = grayBmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                int stride = srcData.Stride;
                for (int y = 0; y < source.Height; y++)
                {
                    for (int x = 0; x < source.Width; x++)
                    {
                        byte b = srcPtr[y * stride + x * 3];
                        byte g = srcPtr[y * stride + x * 3 + 1];
                        byte r = srcPtr[y * stride + x * 3 + 2];

                        byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);

                        byte bin = (gray > threshold) ? (byte)255 : (byte)0;

                        dstPtr[y * stride + x * 3] = bin;     // B
                        dstPtr[y * stride + x * 3 + 1] = bin; // G
                        dstPtr[y * stride + x * 3 + 2] = bin; // R
                    }
                }
            }

            source.UnlockBits(srcData);
            grayBmp.UnlockBits(dstData);

            return grayBmp;
        }

        /// <summary>
        /// 최소제곱법 이용하여 예측
        /// </summary>
        /// <param name="known"></param>
        /// <param name="targetLine"></param>
        /// <returns></returns>
        public static float PredictLinear(Dictionary<int, float> known, int targetLine)
		{
			int n = known.Count;
			if (n < 2)
			{
				return 0f;
			}

			float sumX = 0, sumY = 0, sumXY = 0, sumXX = 0;

            foreach (var (line, x) in known)
            {
                sumX += line;
                sumY += x;
                sumXY += line * x;
                sumXX += line * line;
            }

            float meanX = sumX / n;
            float meanY = sumY / n;

            float denominator = sumXX - n * meanX * meanX;
			if (denominator == 0)
			{
				return meanY;
			}

            float slope = (sumXY - n * meanX * meanY) / denominator;
            float intercept = meanY - slope * meanX;

            return slope * targetLine + intercept;
        }

        /// <summary>
        /// 인식되지 않은 X좌표 최소제곱법으로 보정
        /// </summary>
        /// <param name="xTable"></param>
        /// <param name="xvisited"></param>
        /// <param name="allLines"></param>
        /// <param name="digitLevels"></param>
        public static void FillMissingXPoints(
			ref Dictionary<int, Dictionary<int, float>> xTable, 
			HashSet<(int line, int digitLevel)> xvisited, 
			int[] allLines, 
			int[] digitLevels)
        {
            foreach (int digitLevel in digitLevels)
            {
                Dictionary<int, float> known = new();
                foreach (int line in allLines)
                {
                    if (xTable.ContainsKey(line) && xTable[line].ContainsKey(digitLevel))
                    {
                        known[line] = xTable[line][digitLevel];
                    }
                }

                foreach (int line in allLines)
                {
                    if (!xvisited.Contains((line, digitLevel)))
                    {
                        float predictedX = PredictLinear(known, line);

						if (!xTable.ContainsKey(line))
						{
							xTable[line] = new Dictionary<int, float>();
						}

                        xTable[line][digitLevel] = predictedX;
                        // Console.WriteLine($"Predicted X for line {line}, level {digitLevel}: {predictedX}");
                    }
                }
            }
        }

        /// <summary>
        /// 인식되지 않은 Y좌표 최소제곱법으로 보정
        /// </summary>
        /// <param name="yTable"></param>
        /// <param name="visitedLines"></param>
        /// <param name="allLines"></param>
        public static void FillMissingYPoints(ref Dictionary<int, float> yTable, HashSet<int> visitedLines, int[] allLines)
        {
            Dictionary<int, float> known = new();
            foreach (int line in allLines)
            {
                if (visitedLines.Contains(line) && yTable.ContainsKey(line))
                {
                    known[line] = yTable[line];
                }
            }

            foreach (int line in allLines)
            {
                if (!visitedLines.Contains(line) && !yTable.ContainsKey(line))
                {
                    float predictedY = PredictLinear(known, line);
                    yTable[line] = predictedY;
                    // Console.WriteLine($"Predicted Y for line {line}: {predictedY}");
                }
            }
        }

        /// <summary>
        /// X좌표와 Y좌표를 모두 최소제곱법으로 보정
        /// </summary>
        /// <param name="xTable">line -> digitLevel -> X좌표</param>
        /// <param name="yTable">line -> Y좌표</param>
        /// <param name="allLines">존재해야 하는 모든 줄 번호</param>
        /// <param name="digitLevels">자리수 인덱스 목록 (0=백, 1=십, 2=일)</param>
        public void FillMissingXYWithLeastSquares(
            ref Dictionary<int, Dictionary<int, float>> xTable,
            ref Dictionary<int, float> yTable,
            int[] allLines,
            int[] digitLevels
        )
        {
            // Y좌표 보정
            var yPts = yTable.Where(kv => allLines.Contains(kv.Key)).ToList();
            if (yPts.Count >= 2)
            {
                float sumX = yPts.Sum(p => p.Key);
                float sumY = yPts.Sum(p => p.Value);
                float sumXX = yPts.Sum(p => p.Key * p.Key);
                float sumXY = yPts.Sum(p => p.Key * p.Value);
                int n = yPts.Count;

                float slope = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
                float intercept = (sumY - slope * sumX) / n;

                foreach (var line in allLines)
                {
                    if (!yTable.ContainsKey(line))
                    {
                        yTable[line] = slope * line + intercept;
                    }
                }
            }

            // X좌표 보정
            foreach (var digitLevel in digitLevels)
            {
                var pts = new List<(int line, float x)>();

                foreach (var line in allLines)
                {
                    if (xTable.ContainsKey(line) && xTable[line].ContainsKey(digitLevel))
                    {
                        pts.Add((line, xTable[line][digitLevel]));
                    }
                }

                if (pts.Count >= 2)
                {
                    // 최소제곱법 계산
                    float sumX = pts.Sum(p => p.line);
                    float sumY = pts.Sum(p => p.x);
                    float sumXX = pts.Sum(p => p.line * p.line);
                    float sumXY = pts.Sum(p => p.line * p.x);
                    int n = pts.Count;

                    float slope = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
                    float intercept = (sumY - slope * sumX) / n;

                    // 누락된 X 좌표 채우기
                    foreach (var line in allLines)
                    {
                        if (!xTable.ContainsKey(line))
                            xTable[line] = new Dictionary<int, float>();

                        if (!xTable[line].ContainsKey(digitLevel))
                        {
                            float predictedX = slope * line + intercept;
                            xTable[line][digitLevel] = predictedX;
                        }
                    }
                }
            }

            // Y좌표 보정 반복하여 정확도 개선
            yPts = yTable.Where(kv => allLines.Contains(kv.Key)).ToList();
            if (yPts.Count >= 2)
            {
                float sumX = yPts.Sum(p => p.Key);
                float sumY = yPts.Sum(p => p.Value);
                float sumXX = yPts.Sum(p => p.Key * p.Key);
                float sumXY = yPts.Sum(p => p.Key * p.Value);
                int n = yPts.Count;

                float slope = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
                float intercept = (sumY - slope * sumX) / n;

                foreach (var line in allLines)
                {
                    if (!yTable.ContainsKey(line))
                    {
                        yTable[line] = slope * line + intercept;
                    }
                }
            }
        }

        private void scanButton_Click(object sender, EventArgs e)
		{
			Bitmap? bitmap = ScanWithWia(); // WIA 스캐너로 이미지 가져오기, nullable
			if (bitmap == null)
			{
				return;
			}

			bitmap = EnhanceContrastFast(bitmap);

            var qrData = new HashSet<(string text, PointF point, SizeF size)>();
            var seenTexts = new HashSet<string>(); // 중복 텍스트 체크
            
			for (int i = 0; i < 5; i++)
            {
                var qrResult = ExtractQrCodesWithSize(bitmap);
                foreach (var qr in qrResult)
                {
                    if (!seenTexts.Contains(qr.text))
                    {
                        seenTexts.Add(qr.text);
                        qrData.Add(qr);
                    }
                }
            }

            var xTable = new Dictionary<int, Dictionary<int, float>>(); // line -> digitLevel -> x
			var yTable = new Dictionary<int, float>(); // line -> y
			var menuMap = new Dictionary<int, string>();
			for (int i = 1; i <= menu.Length; i++)
			{
				menuMap[i] = menu[i - 1] ?? "";
			}

			var xvisited = new HashSet<(int x, int y)>();

            Bitmap debugBitmap = new Bitmap(bitmap);
			Graphics g = Graphics.FromImage(debugBitmap);
			Pen roiPen = new Pen(Color.Red, 2);

			Dictionary<(int menuNum, int level), SizeF> xQrSizes = new();
			Dictionary<int, SizeF> yQrSizes = new();
			List<SizeF> allsize = new List<SizeF>();
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
							xvisited.Add((line, digitLevel));
                        }
						xTable[line][digitLevel] = point.X;
						xQrSizes[(line, digitLevel)] = size;
						allsize.Add(size);
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

            int[] allLines = menuMap.Keys.ToArray();
            var yVisited = new HashSet<int>(yTable.Keys);
            FillMissingYPoints(ref yTable, yVisited, allLines);

            allLines = yTable.Keys.ToArray(); // 메뉴 번호 기준
			List<int> digitLevels = new List<int>();
			for(int i = 1; i <= digitCount; i++)
			{
				digitLevels.Add((int)Math.Pow(10, digitCount - i));
            }

            FillMissingXPoints(ref xTable, xvisited, allLines, digitLevels.ToArray());

            FillMissingXYWithLeastSquares(
				ref xTable,
			    ref yTable,
			    allLines,
			    digitLevels.ToArray()
			);           // 크기 비율 정리


            List<int> leveling = new List<int>();
			for(int i = 1; i <= digitCount; i++)
			{
				leveling.Add((int)Math.Pow(10, digitCount - i));
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
					foreach (int level in leveling)
					{
						if (xTable[menuNum].ContainsKey(level))
						{
							float x = xTable[menuNum][level];

							float qrWidth = xQrSizes.TryGetValue((menuNum, level), out var sizeX) ? sizeX.Width : (allsize.Sum(x => x.Width)/allsize.Count);
							float qrHeight = yQrSizes.TryGetValue(menuNum, out var sizeY) ? sizeY.Height : (allsize.Sum(x => x.Height) / allsize.Count);

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
					foreach (int level in leveling)
					{
						if (xTable[menuNum].ContainsKey(level))
						{
							float x = xTable[menuNum][level];
                            float qrWidth = xQrSizes.TryGetValue((menuNum, level), out var sizeX) ? sizeX.Width : (allsize.Sum(x => x.Width) / allsize.Count);
                            float qrHeight = yQrSizes.TryGetValue(menuNum, out var sizeY) ? sizeY.Height : (allsize.Sum(x => x.Height) / allsize.Count);

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

        /// <summary>
        /// 이미지에서 QR 코드 추출 및 위치 반환
        /// </summary>
        /// <param name="bitmap">img</param>
        /// <returns>QR code locations</returns>
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
        /// <summary>
        /// ROI를 이미지 크기에 맞게 조정
        /// </summary>
        /// <param name="roi"></param>
        /// <param name="bitmapSize">img size</param>
        /// <returns></returns>
        static Rectangle ClampROI(Rectangle roi, Size bitmapSize)
		{
			int x = Math.Max(0, roi.X);
			int y = Math.Max(0, roi.Y);
			int width = Math.Min(roi.Width, bitmapSize.Width - x);
			int height = Math.Min(roi.Height, bitmapSize.Height - y);

			return new Rectangle(x, y, width, height);
		}

        static int t = 0;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bitmap"></param>
		/// <param name="roi"></param>
		/// <returns></returns>
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
					preprocessed.Save($"ocr_debug_{t++}.png", System.Drawing.Imaging.ImageFormat.Png);
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

        /// <summary>
        /// 이미지 흑백 변환
        /// </summary>
        /// <param name="input">img</param>
        /// <returns>preprocessed img</returns>
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
			SetWiaProperty(item, 6147, 600); // DPI X
			SetWiaProperty(item, 6148, 600); // DPI Y

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

		private bool isNumber(string s)
		{
			foreach (char c in s)
			{
				if (!char.IsDigit(c))
				{
					return false;
				}
			}
			return true;
		}

		private void wishKiosk_Load(object sender, EventArgs e)
		{
			if (!File.Exists(digitFilePath))
			{
				File.WriteAllText(digitFilePath, digitCount.ToString());
			}
			else
			{
				string digitTxt = File.ReadAllText(digitFilePath);
				if (isNumber(digitTxt))
				{
					digitCount = int.Parse(digitTxt);
				}
			}

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
            price = priceList.Select(p => (int?)p).ToArray();
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
        public static void ShowMid(string text, string caption = "")
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
                    panel.Controls.Add(new Label { AutoSize = true, Height = 6 });
                    continue;
                }

                bool isUrl = line.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                              line.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                              line.StartsWith("www.", StringComparison.OrdinalIgnoreCase);

                if (isUrl)
                {
                    string url = line.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                                 ? line
                                 : $"https://{line}";

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
                    panel.Controls.Add(new Label
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