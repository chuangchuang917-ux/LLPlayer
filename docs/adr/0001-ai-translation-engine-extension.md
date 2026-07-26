# 0001: OpenAI-Compatible Architecture for Gemini and DeepSeek Translation

## Context & Decision

We need to add Gemini and DeepSeek translation services to LLPlayer. DeepSeek uses a standard OpenAI-compatible API (`https://api.deepseek.com/v1`). Google Gemini provides both a native REST API and an OpenAI-compatible endpoint (`https://generativelanguage.googleapis.com/v1beta/openai/`).

We decided to implement both **DeepSeek** and **Gemini** by inheriting from `OpenAIBaseTranslateService` and creating corresponding settings classes (`DeepSeekTranslateSettings` and `GeminiTranslateSettings`) utilizing the OpenAI-compatible endpoints.

## Rationale

1. **Code Reuse**: Reuses existing streaming, prompt formatting, context window management, and error parsing logic in `OpenAIBaseTranslateService`.
2. **Maintainability**: Avoids building and maintaining a separate REST API wrapper specifically for Google Gemini.
3. **Consistency**: Provides a uniform UI and configuration workflow for all LLM translation providers.
