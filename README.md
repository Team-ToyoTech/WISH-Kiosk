# WI:SH Kiosk — Write It: Scan & Handle

> **종이에 쓴 숫자를 스캔해서 주문으로 바꾸는 키오스크**  
> 고령자, 디지털 약자를 위한 “아날로그 필기 + 디지털 처리” 키오스크
> 
> 키오스크 접근성 개선 프로젝트

자세한 내용은 [`wish.docs.toyotech.dev`](https://wish.docs.toyotech.dev/)를 참고해 주세요.

- Using **[QRCoder](https://github.com/codebude/QRCoder)**, **[ZXing](https://github.com/zxing/zxing)** and **WIA**
- Trained AI model using **[PyTorch](https://github.com/pytorch/pytorch)**, [DataSet Preprocess](https://github.com/Team-ToyoTech/WISH-ImagePreprocess)

- 실행 전 `copy_these_files` 폴더의 파일을 **실행 폴더**로 복사해야 합니다:
  - `onnx_model 폴더` → `KioskAI/bin/Debug/net8.0-windows/`
  - `sound.wav`   → `wishKioskDIDReceive/bin/Debug/net8.0-windows/`
- **wishKiosk**의 설정 버튼은 스캔 버튼 오른쪽에 얇게 마련되어 있습니다.
- **wishKiosk**의 설정창 초기 비밀번호는 `0000` 입니다.
- **wishKioskDIDDisplay**와 **wishKioskDIDReceive**의 설정창 진입 키는 `T`입니다.

---

## 📦 구성 개요

### 모듈
- **wishKiosk**: 주문지 *출력 → 스캔(WIA) → QR 위치 복원(ZXing) → 숫자 OCR(ONNXRuntime) → 주문 요약/수정 → 결제(TossPayments) → 영수증/주문번호 출력*
- **wishKioskDIDDisplay**: 대기/완료 **주문번호 DID 표시** (완료 시 음성 안내)
- **wishKioskDIDReceive**: 신규 주문 **효과음 알림**, **완료/수령/취소** 처리
- **KioskAI**: ONNX 런타임 추론(숫자 모델)

### 서버
- **[WISH-Server](https://github.com/Team-ToyoTech/WISH-Server)**
- 서버 리셋을 위해서는 `settings` → `서버 초기화`
- 로컬 서버를 사용하고 싶다면 `wishKiosk`, `wishKioskDIDDisplay`, `wishKioskDIDReceive` 각각 설정창에서 서버 설정 변경

---

## 🧰 개발 환경

- OS: **Windows 11**
- SDK/IDE: **.NET 8**, **Visual Studio 2022**
- 장비: **EPSON ES-50**(스캐너), **PeriPage P40**(프린터)
- 서버: **Node.js**, **AWS**

---

## 🚀 빠른 시작

```bash
# 1) 서버
git clone https://github.com/Team-ToyoTech/WISH-Server.git
cd WISH-Server
npm install
node Server.js # 기본 포트: 4000

# 2) 키오스크
git clone https://github.com/Team-ToyoTech/WISH-Kiosk.git
# Visual Studio 2022에서 솔루션 열기 → 빌드
```

## 로고

<a href="https://wish.docs.toyotech.dev/logo/logo.html">
<img width="128" height="128" alt="logo" src="https://github.com/user-attachments/assets/5763fa6b-c943-4710-8e5b-b0fc36910024" /></a>
