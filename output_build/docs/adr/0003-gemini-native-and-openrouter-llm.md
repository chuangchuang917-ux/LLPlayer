# 0003: Gemini Native REST API and OpenRouter LLM Integration

## Context & Decision

During testing, Google Gemini's OpenAI compatibility layer proved unreliable and incompatible with standard OpenAI client setups. Additionally, users required support for OpenRouter, an aggregator providing access to hundreds of LLMs (including Gemini, Claude, Llama, and DeepSeek) via an OpenAI-compatible endpoint (`https://openrouter.ai/api/v1`).

We decided to:
1. **Refactor Gemini to Native REST API**: Implement a dedicated `GeminiTranslateService` and updated `GeminiTranslateSettings` directly calling Google's v1beta REST API (`generateContent`) using `x-goog-api-key` header authentication and native `contents/parts` JSON format, with `gemini-3.5-flash-lite` as the default model.
2. **Add OpenRouter Service**: Implement `OpenRouterTranslateSettings` and `OpenRouterTranslateService` inheriting from `OpenAIBaseTranslateSettings`, pointing to `https://openrouter.ai/api/v1` with default model `google/gemini-3.5-flash-lite` and app identification headers (`X-Title: LLPlayer`, `HTTP-Referer`).

## Rationale

1. **Reliability**: Direct integration with Google's native Gemini v1beta REST API guarantees 100% compatibility and avoids OpenAI endpoint proxy failures.
2. **Security**: Using `x-goog-api-key` HTTP header avoids exposing API keys in request URLs.
3. **Versatility**: OpenRouter support allows learners to access any leading LLM provider using a single API key and uniform OpenAI-compatible settings workflow.
