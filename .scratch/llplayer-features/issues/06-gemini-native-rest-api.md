# 06 — Gemini Native REST API Integration

**What to build:**
A dedicated Google Gemini translation service that directly interacts with Google Gemini's native `v1beta/models/{model}:generateContent` REST API endpoint using `x-goog-api-key` header authentication and native `contents/parts` JSON structure, defaulting to model `gemini-3.5-flash-lite`.

**Blocked by:** None — can start immediately.

**Status:** completed

- [x] Implement GeminiTranslateSettings with DefaultEndpoint `https://generativelanguage.googleapis.com/v1beta` and default model `gemini-3.5-flash-lite`.
- [x] Configure `x-goog-api-key` HTTP Header authentication in `GetHttpClient()`.
- [x] Implement GeminiTranslateService executing native Gemini REST API POST requests and parsing `candidates[0].content.parts[0].text`.
- [x] Update TranslateServiceFactory to instantiate GeminiTranslateService.
