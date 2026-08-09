# 07 — OpenRouter Multi-Model LLM API Integration

**What to build:**
An OpenRouter LLM translation service integration utilizing OpenAI-compatible endpoint (`https://openrouter.ai/api/v1`), default model `google/gemini-3.5-flash-lite`, custom `X-Title` / `HTTP-Referer` headers, and corresponding UI settings DataTemplate.

**Blocked by:** 06 — Gemini Native REST API Integration.

**Status:** completed

- [x] Implement OpenRouterTranslateSettings with DefaultEndpoint `https://openrouter.ai/api/v1` and default model `google/gemini-3.5-flash-lite`.
- [x] Configure `Authorization: Bearer`, `X-Title: LLPlayer`, and `HTTP-Referer: https://llplayer.com` headers in `GetHttpClient()`.
- [x] Add TranslateServiceType.OpenRouter enum and LLMServices flag mapping.
- [x] Update TranslateServiceFactory to instantiate OpenRouterTranslateSettings.
- [x] Register OpenRouterTranslateSettings in AppConfig KnownSettingsTypes.
- [x] Add OpenRouterTranslateSettings DataTemplate in SettingsSubtitlesTrans.xaml.
