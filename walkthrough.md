# LLPlayer 專案分析與擴充開發報告 (Walkthrough)

## 📌 專案概述
**LLPlayer** 是一款專為語言學習設計的開放原始碼 Windows 媒體播放器（Media Player for Language Learning）。

- **官方網站**: [llplayer.com](https://llplayer.com)
- **主要語言 / 框架**: C# / WPF (.NET 10)
- **多媒體核心引擎**: 基於 [Flyleaf](https://github.com/FlyleafDevelopment/Flyleaf) 播放庫與 FFmpeg

---

## ✨ 核心功能特色（包含最新擴充）

1. **雙語字幕 (Dual Subtitles)**
   - 支援同時顯示主/副雙字幕。
   - 同時相容文字字幕（SRT、ASS 等）與圖形字幕（PGS/DVD 點陣圖字幕）。

2. **AI 語音轉字幕 (ASR - Automatic Speech Recognition)**
   - 整合 **OpenAI Whisper** 語音辨識技術。
   - 支援兩種計算引擎：`whisper.cpp` 與 `faster-whisper`。

3. **即時翻譯與 LLM 引擎擴充 (Real-time & AI Translation)**
   - 支援來源：Google Translation, DeepL, OpenAI, Claude, Ollama, LM Studio 等。
   - **重構**：**Google Gemini** 原生 REST API 服務 (`GeminiTranslateService`)，直連 `v1beta/models/{model}:generateContent`，支援 `x-goog-api-key` Header 驗證，預設模型 `gemini-3.5-flash-lite`。
   - **新增**：**OpenRouter** 多模型 LLM API 服務 (`OpenRouterTranslateSettings`)，預設 Endpoint `https://openrouter.ai/api/v1`，預設模型 `google/gemini-3.5-flash-lite`，並支援 `X-Title` 與 `HTTP-Referer` 標頭。
   - **新增**：**DeepSeek** API（OpenAI 相容 Endpoint `https://api.deepseek.com`）。

4. **生字簿與 Anki 卡片整合 (Vocabulary Book & Anki Sync)**
   - **新增**：本機生字儲存庫 (`VocabularyService`)，自動紀錄單字、解釋、字幕原句與影片時間。
   - **新增**：一鍵 **AnkiConnect API** (`http://127.0.0.1:8765`) 即時同步至 Anki 牌組。
   - **新增**：一鍵 **CSV 檔案匯出** 備份功能。
   - **新增**：生字管理與複習專屬 UI 視窗 (`VocabularyDialog`) 及播放列一鍵開啟按鈕。

5. **精聽播放控制 (Intensive Listening Controls)**
   - **新增**：**AB 區間循環 (AB Loop)** 精聽重複播放。
   - **新增**：**影子跟讀模式 (Shadowing Auto-Pause)** 句尾自動暫停。
   - **新增**：**無對白自動加速 (Smart Speed)** 無字幕區間自動快進 (如 1.8x)。
   - **新增**：**字幕側邊欄星號書籤 (Subtitle Bookmark)**。

---

## 📂 專案架構目錄與修改紀錄

- `CONTEXT.md`：紀錄生字簿、AnkiConnect、AB Loop、Shadowing、Gemini Native API、OpenRouter API 等領域模型名詞與界限。
- `docs/adr/`：
  - `0001-ai-translation-engine-extension.md`：Gemini / DeepSeek API 串接架構決策。
  - `0002-anki-connect-and-vocab-store.md`：生字庫與 AnkiConnect 即時同步決策。
  - `0003-gemini-native-and-openrouter-llm.md`：Gemini 原生 API 重構與 OpenRouter LLM 整合決策。
- `LLPlayer/Services/`：
  - `VocabularyItem.cs` & `VocabularyService.cs`：生字模型與 JSON/CSV 持久化儲存。
  - `AnkiConnectService.cs`：AnkiConnect HTTP API 串接服務。
  - `PlaybackControlsService.cs`：AB 循環、影子跟讀與無對白加速控制邏輯。
  - `AppActions.cs` & `AppConfig.cs`：新增 `CmdOpenWindowVocabulary` 與 `OpenRouterTranslateSettings` 設定型態註冊。
- `LLPlayer/Views/`：
  - `VocabularyDialog.xaml` & `.cs`：生字管理與 Anki 同步視窗。
- `LLPlayer/Controls/`：
  - `WordPopup.xaml` & `.cs`：新增「📚 加人生字簿」與「🎴 傳送至 Anki」按鈕。
  - `FlyleafBar.xaml`：底部控制列新增生字簿開啟按鈕。
  - `Settings/SettingsSubtitlesTrans.xaml`：新增 OpenRouter 設定介面 DataTemplate。
- `FlyleafLib/MediaPlayer/Translation/`：
  - `ITranslateService.cs`, `ITranslateSettings.cs`, `TranslateServiceFactory.cs`：重構 `Gemini` 原生 REST 服務，新增 `OpenRouter` 翻譯設定與工廠邏輯。
  - `GeminiTranslateService.cs`：Google Gemini v1beta 原生 `generateContent` 端點獨立實作。

---

## 🛠️ 開發與編譯需求

- **作業系統**: Windows 10 x64 / Windows 11 x64
- **執行環境**:
  - [.NET Desktop Runtime 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
  - [Microsoft Visual C++ Redistributable 2022](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist)
