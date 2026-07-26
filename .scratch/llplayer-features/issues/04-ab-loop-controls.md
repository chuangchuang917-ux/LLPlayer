# 04 — AB 區間循環 (AB Loop) 精聽控制

**What to build:**
使用者可於播放時間軸點擊快捷鍵或按鈕設定起點 A 與終點 B，使播放器在指定影音區間內自動重複循環播放，方便進行聽力精讀訓練。

**Blocked by:** None — can start immediately

**Status:** completed

- [x] 在 `PlaybackControlsService` 中新增 `LoopPointA`, `LoopPointB`, `IsAbLoopEnabled` 狀態控制邏輯。
- [x] 在播放器時間軸更新事件 (`CurTime`) 中判斷播放位置，超過終點 B 時自動跳回起點 A。
- [x] 提供設定起點 A、設定終點 B 與清除 AB Loop 之命令與介面綁定。
