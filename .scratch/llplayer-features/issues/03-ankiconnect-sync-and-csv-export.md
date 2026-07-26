# 03 — AnkiConnect 即時同步與生字 CSV 匯出視窗

**What to build:**
使用者可於單字彈窗點擊「傳送至 Anki」將單字一鍵推送到本機 AnkiConnect (`http://127.0.0.1:8765`) 建立卡片；並可打開 `VocabularyDialog` 生字管理視窗檢視、搜尋生字與批次匯出 CSV 檔案。

**Blocked by:** 02 — 本機生字庫 (Vocabulary Store) 與單字彈窗動作

**Status:** completed

- [x] 建立 `AnkiConnectService`，實作連線檢查與 `addNote` HTTP API 請求。
- [x] 建立 `VocabularyDialog.xaml` 與 `VocabularyDialog.xaml.cs` 生字管理視窗，支援關鍵字搜尋與一鍵匯出 CSV 檔。
- [x] 在 `AppActions.cs` 與 `FlyleafBar.xaml` 中新增「生字簿」開啟按鈕與快捷命令。
- [x] 在 `WordPopup.xaml` 中新增「🎴 傳送至 Anki」按鈕。
