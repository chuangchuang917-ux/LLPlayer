# 01 — 整合 Google Gemini 與 DeepSeek AI 翻譯引擎

**What to build:**
使用者可以在字幕翻譯設定視窗中選擇 Google Gemini 或 DeepSeek 作為 LLM 翻譯來源，填入 API Key 進行連線測試，並於影片播放或字幕查單字時取得由 Gemini 或 DeepSeek 提供的高品質 Context-Aware 翻譯結果。

**Blocked by:** None — can start immediately

**Status:** completed

- [x] 在 `TranslateServiceType` 枚舉中加入 `Gemini` 與 `DeepSeek`。
- [x] 實作 `GeminiTranslateSettings` 與 `DeepSeekTranslateSettings`，繼承自 `OpenAIBaseTranslateSettings`（使用 OpenAI 相容 Endpoint）。
- [x] 在 `TranslateServiceFactory.cs` 與 `AppConfig.cs` 中註冊新服務之實例化與多型序列化對應。
- [x] 在 `SettingsSubtitlesTrans.xaml` 中新增 Gemini 與 DeepSeek 的 DataTemplate 介面。
