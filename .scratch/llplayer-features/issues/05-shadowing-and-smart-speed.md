# 05 — 影子跟讀 (Shadowing Auto-Pause) 與智慧語速 (Smart Speed)

**What to build:**
使用者可開啟「影子跟讀模式」使播放器在每句字幕結束時自動暫停等待朗讀；或開啟「智慧語速」讓沒有對白的影片空白片段自動快進 (如 1.8x)，節省學習時間。

**Blocked by:** None — can start immediately

**Status:** completed

- [x] 在 `PlaybackControlsService` 中實作影子跟讀邏輯（監聽字幕 `EndTime` 並觸發 `Player.Pause()`）。
- [x] 在 `PlaybackControlsService` 中實作 Smart Speed 邏輯（依據當前時間是否有字幕對白動態切換 `Player.Speed`）。
- [x] 支援字幕側邊欄星號書籤 (Subtitle Bookmark) 與快速跳轉。
