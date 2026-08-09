# LLPlayer Feature Specification (PRD)

## Problem Statement

Language learners using media players face several friction points during video study:
1. **Unreliable LLM Translation Endpoints**: Default OpenAI proxy wrappers fail when connecting to providers like Google Gemini due to non-standard endpoint paths, header authentication requirements, or strict JSON schema differences.
2. **Lack of Multi-Provider LLM Router Access**: Learners using aggregator platforms like OpenRouter cannot directly select from hundreds of cutting-edge models (e.g. `google/gemini-3.5-flash-lite`, `deepseek/deepseek-r1`, `anthropic/claude-3.5-sonnet`) within the media player.
3. **Disconnected Flashcard Workflow**: Learners lack a seamless way to save looked-up words, review their subtitle context sentences, or export them to Anki for spaced repetition study.
4. **Manual Playback Friction**: Active listening practice (such as shadowing or repetitive loop listening) requires constant manual clicking to loop clips, pause at sentence ends, or speed through silent gaps.

## Solution

LLPlayer extends its core capabilities with an integrated suite of language learning tools:
- **Gemini Native API**: Dedicated native integration for Google Gemini using REST API (`generateContent` endpoint with `x-goog-api-key` header authentication and `contents/parts` JSON structure) defaulting to `gemini-3.5-flash-lite`.
- **OpenRouter API**: Multi-model LLM aggregator integration using OpenAI-compatible endpoint (`https://openrouter.ai/api/v1`) with default model `google/gemini-3.5-flash-lite` and custom application metadata headers (`X-Title` and `HTTP-Referer`).
- **Expanded LLM Translation**: Native settings and OpenAI-compatible integration for DeepSeek, OpenAI, Claude, Ollama, and LM Studio.
- **Vocabulary Book & Anki Integration**: Local persistent JSON storage for searched words, context sentences, video titles, and timestamps, alongside one-click **AnkiConnect API** synchronization and CSV file export.
- **Intensive Listening Controls**: Automated **AB Loop** repeating, **Shadowing Mode** (sentence auto-pause), **Smart Speed** (blank section acceleration), and **Subtitle Bookmarks**.

## User Stories

1. As a language learner, I want to configure Google Gemini using its native REST API in the translation settings with default model `gemini-3.5-flash-lite`, so that I can reliably translate subtitles without OpenAI proxy failures.
2. As a language learner, I want to configure OpenRouter in the translation settings with default model `google/gemini-3.5-flash-lite`, so that I can translate subtitles using any model available on OpenRouter.
3. As a language learner, I want to configure DeepSeek in the translation settings, so that I can translate subtitles with DeepSeek's AI models.
4. As a language learner looking up words in subtitle popups, I want to click a "Add to Vocabulary Book" button, so that the word, definition, context sentence, and video timestamp are saved locally.
5. As a language learner, I want to click a "Send to Anki" button on the word popup, so that the flashcard is pushed directly into my running Anki application via AnkiConnect.
6. As a language learner, I want to open a dedicated Vocabulary Management Window, so that I can search, review, delete, or batch-export my saved vocabulary to CSV files.
7. As a language learner practicing intensive listening, I want to set point A and point B on the timeline, so that the player automatically loops that specific video segment.
8. As a language learner practicing shadowing, I want the player to automatically pause at the end of each subtitle sentence, so that I can repeat the pronunciation before proceeding.
9. As a language learner watching long videos, I want the player to accelerate playback speed during sections without subtitles and automatically return to 1.0x during spoken dialogue, so that I can save learning time.
10. As a language learner, I want to star/bookmark specific subtitle lines in the sidebar, so that I can quickly jump back and review them later.

## Implementation Decisions

- **Gemini Native REST API Integration**: Built a dedicated translation service interacting directly with Google Gemini's `generateContent` REST API. Authentication uses the `x-goog-api-key` HTTP header, bypassing URL query string key leakage and OpenAI compatibility proxies.
- **OpenRouter LLM Integration**: Implemented settings inheriting from the base OpenAI translation service abstraction, overriding default endpoint to `https://openrouter.ai/api/v1`, defaulting model to `google/gemini-3.5-flash-lite`, and attaching `X-Title: LLPlayer` and `HTTP-Referer: https://llplayer.com` headers.
- **OpenAI-Compatible LLM Services**: DeepSeek, OpenAI, Claude, and local LLM settings inherit from the base OpenAI translation service abstraction, utilizing uniform request formatting, prompt template management, and streaming response parsing.
- **Local Vocabulary Storage & AnkiConnect API**: Vocabulary data is stored locally in JSON format with automated schema validation. Anki integration utilizes direct local HTTP requests (`http://127.0.0.1:8765`) to AnkiConnect with fallback CSV export.
- **Centralized Playback Controls Service**: AB looping, shadowing auto-pause, and smart speed rate switching are decoupled into a dedicated playback service observing player timeline updates.
- **Prism Dialog & Container Registration**: The vocabulary management interface, OpenRouter settings, and services are registered within the application DI container, settings DataTemplates, and Prism dialog registry.

## Testing Decisions

- **Test Boundaries (Seams)**:
  - `ITranslateService` seam: Testing asynchronous translation requests against mocked Gemini native `generateContent` endpoints and OpenAI-compatible OpenRouter endpoints.
  - `VocabularyService` seam: Unit testing JSON serialization, item filtering, and CSV formatting.
  - `AnkiConnectService` seam: Mocking HTTP responses from `127.0.0.1:8765` for connection checks and note additions.
  - `PlaybackControlsService` seam: Unit testing timeline boundary conditions (e.g. current time crossing loop point B or subtitle end time).
- **Good Test Characteristics**: Focus exclusively on external component contracts, state transitions, and file outputs without tying tests to private UI thread handles.

## Out of Scope

- Cloud-based vocabulary synchronization across multiple user devices.
- Direct binary `.apkg` database generation without Anki installed.
- Real-time speech recognition model training/fine-tuning.

## Further Notes

- Documented domain vocabulary is recorded in `CONTEXT.md`.
- Architectural decisions are recorded in `docs/adr/0001-ai-translation-engine-extension.md`, `docs/adr/0002-anki-connect-and-vocab-store.md`, and `docs/adr/0003-gemini-native-and-openrouter-llm.md`.
