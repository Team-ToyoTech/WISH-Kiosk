using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using QRCoder;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using WIA;
using ZXing;

namespace wishKiosk
{
	public partial class wishKiosk : Form
	{
		private float FontSize { get; set; } = 30f;
		private PrintDocument printDoc = new();
		private readonly Pen gridPen = new(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
		private readonly Brush blackBrush = Brushes.Black;
		private readonly int boxSize = 40;
		private readonly int spacing = 20;
		private readonly int lineSpacing = 120;
		private int currentMenuIndex = 0;
		public int digitCount = 3; // 숫자 칸 개수
		public readonly string menuFilePath = "menu.csv";
		public readonly string digitFilePath = "digit.dat";
		private readonly string passwordFilePath = "password.dat";
		private readonly static string modelFilePath = "onnx_model/tmnist_model_64.onnx";
		private readonly static string labelsFilePath = "onnx_model/labels.json";
        public Dictionary<string, int> menuPrice = [];

		private string[]? menu;
		private int[]? price;

		public wishKiosk()
		{
			InitializeComponent();
			InitOCR();
		}

		private void wishKiosk_Load(object sender, EventArgs e)
		{
			if (!File.Exists(modelFilePath) || !File.Exists(labelsFilePath))
			{
				MessageBox.Show("파일이 존재하지 않습니다.\nonnx_model 폴더를 실행 경로로 복사하세요.");
				this.Close();
				return;
            }

            if (!File.Exists(digitFilePath))
			{
				File.WriteAllText(digitFilePath, digitCount.ToString());
			}
			else
			{
				string digitTxt = File.ReadAllText(digitFilePath);
				if (IsNumber(digitTxt))
				{
					digitCount = int.Parse(digitTxt);
				}
			}

			if (!File.Exists(menuFilePath))
			{
				using (var writer = new StreamWriter(menuFilePath, false, Encoding.UTF8))
				{
					writer.WriteLine("메뉴명,가격");
				}
				return;
			}

			// 메뉴, 가격 가져오기
			string[]? lines = File.ReadAllLines(menuFilePath);

			List<string> menuList = new List<string>();
			List<int> priceList = new List<int>();

			for (int i = 1; i < lines.Length; i++)
			{
				string[]? splt = lines[i].Trim().Split(',');
				string menuName = splt[0];
				try
				{
					int priceValue = int.Parse(splt[1]);
					menuList.Add(menuName);
					priceList.Add(priceValue);
					menuPrice[menuName] = priceValue;
				}
				catch
				{
					MessageBox.Show($"{i}행의 가격이 잘못된 형식입니다.");
					return;
				}
			}

			menu = menuList.ToArray();
			price = priceList.ToArray();

			// 비밀번호 파일 확인
            if (!File.Exists(passwordFilePath))
            {
                using (var writer = new StreamWriter(passwordFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(Sha256Hash("0000")); // 기본값 0000
                }
            }
        }

		private void printButton_Click(object sender, EventArgs e)
		{
			printDoc.PrintPage += printDocument_PrintPage;
			printDoc.Print();
			printDoc.PrintPage -= printDocument_PrintPage;
		}

		/// <summary>
		/// 프린트할 페이지 제작
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			Font font = new Font("Arial", FontSize);
			int startX = 100;
			int startY = 150;
			int availableHeight = e.MarginBounds.Bottom - startY;

			int linesPerPage = availableHeight / lineSpacing;
			int printedCount = 0;

			for (; currentMenuIndex < menu?.Length; currentMenuIndex++)
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

			if (currentMenuIndex >= menu?.Length)
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

        /// <summary>
        /// 문자열을 SHA256 해싱하여 16진수 문자열 반환
        /// </summary>
        /// <param name="input">original str</param>
        /// <returns>hash str</returns>
        public static string Sha256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                var sb = new StringBuilder();
                foreach (byte b in data)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
		{
			if (!File.Exists(passwordFilePath))
			{
				MessageBox.Show($"{passwordFilePath} 파일이 없습니다.");
                using (var writer = new StreamWriter(passwordFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine(Sha256Hash("0000"));
                }
            }

			string? passwordHash = File.ReadAllText(passwordFilePath).Trim();
            string input = Interaction.InputBox("비밀번호를 입력하세요: ", "비밀번호");
			if (string.IsNullOrEmpty(input))
			{
				return;
			}
            if (passwordHash != Sha256Hash(input))
			{
				MessageBox.Show("비밀번호가 일치하지 않습니다.");
				return;
            }

            settings Settings = new(passwordFilePath)
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
		public void getData(settings Settings)
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

			if (!File.Exists(menuFilePath))
			{
				using (var writer = new StreamWriter(menuFilePath, false, Encoding.UTF8))
				{
					writer.WriteLine("메뉴명,가격");
				}
				return;
			}

			// 메뉴, 가격 가져오기
			string[]? lines = File.ReadAllLines(menuFilePath);

			List<string> menuList = new List<string>();
			List<int> priceList = new List<int>();

			for (int i = 1; i < lines.Length; i++)
			{
				string[]? splt = lines[i].Trim().Split(',');
				string menuName = splt[0];
				try
				{
					int priceValue = int.Parse(splt[1]);
					menuList.Add(menuName);
					priceList.Add(priceValue);
					menuPrice[menuName] = priceValue;
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

		/*
		/// <summary>
		/// 고대비 이미지로 변환 // 실행 시간 문제
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
				Dictionary<int, float> known = [];
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
							xTable[line] = [];
						}

						xTable[line][digitLevel] = predictedX;
						// Console.WriteLine($"Predicted X for line {line}, level {digitLevel}: {predictedX}"); // 디버깅용
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
					// Console.WriteLine($"Predicted Y for line {line}: {predictedY}"); // 디버깅용
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
			var seenTexts = new HashSet<string>(); // 중복 텍스트 제거
			
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
			for (int i = 1; i <= menu?.Length; i++)
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
				if (text.Contains('-'))
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
				else if (text.StartsWith('m'))
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
			List<int> digitLevels = [];
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
			);

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
			// MessageBox.Show("ROI가 표시된 이미지가 저장되었습니다."); // 디버깅용

			List<int> orderCnts = new();

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
							// MessageBox.Show($"OCR result at ({x}, {y}) = '{digit}'"); // 디버깅용
							orderCount += digit;
						}
					}
				}

				orderCount = string.IsNullOrEmpty(orderCount) ? "0" : orderCount;
				orderCnts.Add(int.Parse(orderCount));

				// 디버깅용
				// var msgBoxRes = MessageBox.Show($"{menuMap[menuNum]}를 {orderCount}개 주문했습니다.", "주문 확인", MessageBoxButtons.OKCancel); // 디버깅용
				// if (msgBoxRes == DialogResult.OK)
				// {
				//     MessageBox.Show($"{menuMap[menuNum]} {orderCount}개, {price[menuNum - 1] * int.Parse(orderCount)}원 주문이 완료되었습니다.");
				// }
				// else
				// {
				// 	MessageBox.Show("주문이 취소되었습니다.");
				// }
			}

			int[] menuNums = yTable.Keys.ToArray();
			orderResult orderRes = new(menuMap, menuNums, price, orderCnts, menuPrice);
			orderRes.printDoc = printDoc;
			orderRes.Show();

			bitmap.Dispose();
		}

		/// <summary>
		/// 이미지에서 QR 코드 추출 및 위치 반환
		/// </summary>
		/// <param name="bitmap">img</param>
		/// <returns>QR code locations</returns>
		public static List<(string text, PointF point, SizeF size)> ExtractQrCodesWithSize(Bitmap bitmap)
		{
			try
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
            catch (Exception ex)
            {
                MessageBox.Show($"QR 코드 추출 오류: {ex.Message}");
                return [];
            }
        }

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

		private static InferenceSession? _session;
		private static string[]? _labels;
		/// <summary>
		/// OCR 초기화
		/// </summary>
		public static void InitOCR()
		{
			try
			{
                _session = new InferenceSession(modelFilePath);
				_labels = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(labelsFilePath));
			}
			catch
			{
				MessageBox.Show("모델이 존재하지 않습니다.\nonnx_model 폴더를 실행 경로로 복사하세요.");
            }
		}

        /// <summary>
        /// OCR로 숫자 인식
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="roi"></param>
        /// <returns>label</returns>
        public static string OCRDigit(Bitmap bitmap, Rectangle roi)
		{
			using (var memoryStream = new MemoryStream())
			{
				try
				{
					// ROI 안전 클램프
					Rectangle safeROI = ClampROI(roi, bitmap.Size);
					Bitmap cropped = new Bitmap(safeROI.Width, safeROI.Height, PixelFormat.Format24bppRgb);
					using (Graphics g = Graphics.FromImage(cropped))
					{
						g.DrawImage(bitmap, new Rectangle(0, 0, cropped.Width, cropped.Height), safeROI, GraphicsUnit.Pixel);
					}

					// 전처리 + 64x64 변환
					Bitmap resized = new Bitmap(64, 64, PixelFormat.Format24bppRgb);
					using (Graphics g = Graphics.FromImage(resized))
					{
						g.DrawImage(cropped, new Rectangle(0, 0, 64, 64));
					}

					Bitmap preprocessed = PreprocessImage(resized); // 필요 시 흑백, 이진화 등
					// preprocessed.Save($"ocr_debug_{t++}.png", ImageFormat.Png);

					// Tensor 변환
					var inputTensor = ImageToTensor(preprocessed);

					// ONNX 입력 생성
					var inputs = new List<NamedOnnxValue>
					{
						NamedOnnxValue.CreateFromTensor("input", inputTensor)
					};

					// 추론
					using (var results = _session?.Run(inputs))
					{
						var output = results?.First().AsEnumerable<float>().ToArray();
						int predictedIndex = Array.IndexOf(output, output?.Max());
						string label = _labels[predictedIndex];

						return label == "NaN" ? "" : label;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"OCR Error: {ex.Message}");
					return "";
				}
			}
		}

		/// <summary>
		/// 이미지를 ONNX 모델 입력 텐서로 변환
		/// </summary>
		/// <param name="image">img</param>
		/// <returns>Tensor</returns>
		private static DenseTensor<float> ImageToTensor(Bitmap image)
		{
			// ONNX 모델, 그레이스케일
			var tensor = new DenseTensor<float>(new[] { 1, 1, 64, 64 });
			for (int y = 0; y < 64; y++)
			{
				for (int x = 0; x < 64; x++)
				{
					Color c = image.GetPixel(x, y);
					// 흑백 변환 후 -1~1 정규화
					float gray = (c.R + c.G + c.B) / 3f / 255f;
					gray = (gray - 0.5f) / 0.5f; // [-1,1]
					tensor[0, 0, y, x] = gray;
				}
			}
			return tensor;
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

        /// <summary>
        /// WIA 속성 설정
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
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

		private bool IsNumber(string s)
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
	}
}