# 02 — 本機生字庫 (Vocabulary Store) 與單字彈窗動作

**What to build:**
使用者於影片字幕點擊單字開啟彈窗時，可點擊「加人生字簿」按鈕，將單字、解釋、字幕原句、影片標題與時間戳記持久化儲存於本機 JSON 庫中。

**Blocked by:** None — can start immediately

**Status:** completed

- [x] 建立 `VocabularyItem` 資料模型與 `VocabularyService` 服務，實現本機 JSON 持久化讀寫。
- [x] 在 `App.xaml.cs` 容器中註冊 `VocabularyService` 單例。
- [x] 修改 `WordPopup.xaml` 與 `WordPopup.xaml.cs`，新增「📚 加人生字簿」按鈕與點擊處理常式。
