# LLPlayer Feature Specification (PRD)

## Problem Statement

Language learners using media players face several friction points during video study:
1. **Limited Translation Providers**: Default translation options lack native support for popular LLMs such as Google Gemini and DeepSeek, restricting context-aware subtitle translation.
2. **Disconnected Flashcard Workflow**: Learners lack a seamless way to save looked-up words, review their subtitle context sentences, or export them to Anki for spaced repetition study.
3. **Manual Playback Friction**: Active listening practice (such as shadowing or repetitive loop listening) requires constant manual clicking to loop clips, pause at sentence ends, or speed through silent gaps.

## Solution

LLPlayer extends its core capabilities with an integrated suite of language learning tools:
- **Expanded LLM Translation**: Native settings and OpenAI-compatible integration for Google Gemini and DeepSeek.
- **Vocabulary Book & Anki Integration**: Local persistent JSON storage for searched words, context sentences, video titles, and timestamps, alongside one-click **AnkiConnect API** synchronization and CSV file export.
- **Intensive Listening Controls**: Automated **AB Loop** repeating, **Shadowing Mode** (sentence auto-pause), **Smart Speed** (blank section acceleration), and **Subtitle Bookmarks**.

## User Stories

1. As a language learner, I want to configure Google Gemini (via OpenAI-compatible endpoint) in the translation settings, so that I can translate subtitles using Gemini's LLM models.
2. As a language learner, I want to configure DeepSeek in the translation settings, so that I can translate subtitles with DeepSeek's AI models.
3. As a language learner looking up words in subtitle popups, I want to click a "Add to Vocabulary Book" button, so that the word, definition, context sentence, and video timestamp are saved locally.
4. As a language learner, I want to click a "Send to Anki" button on the word popup, so that the flashcard is pushed directly into my running Anki application via AnkiConnect.
5. As a language learner, I want to open a dedicated Vocabulary Management Window, so that I can search, review, delete, or batch-export my saved vocabulary to CSV files.
6. As a language learner practicing intensive listening, I want to set point A and point B on the timeline, so that the player automatically loops that specific video segment.
7. As a language learner practicing shadowing, I want the player to automatically pause at the end of each subtitle sentence, so that I can repeat the pronunciation before proceeding.
8. As a language learner watching long videos, I want the player to accelerate playback speed during sections without subtitles and automatically return to 1.0x during spoken dialogue, so that I can save learning time.
9. As a language learner, I want to star/bookmark specific subtitle lines in the sidebar, so that I can quickly jump back and review them later.

## Implementation Decisions

- **OpenAI-Compatible LLM Services**: Gemini and DeepSeek settings inherit from the base OpenAI translation service abstraction, utilizing uniform request formatting, prompt template management, and streaming response parsing.
- **Local Vocabulary Storage & AnkiConnect API**: Vocabulary data is stored locally in JSON format with automated schema validation. Anki integration utilizes direct local HTTP requests (`http://127.0.0.1:8765`) to AnkiConnect with fallback CSV export.
- **Centralized Playback Controls Service**: AB looping, shadowing auto-pause, and smart speed rate switching are decoupled into a dedicated playback service observing player timeline updates.
- **Prism Dialog & Container Registration**: The vocabulary management interface and services are registered as singletons within the application DI container and Prism dialog registry.

## Testing Decisions

- **Test Boundaries (Seams)**:
  - `ITranslateService` seam: Testing asynchronous translation requests against mocked OpenAI-compatible endpoints.
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
- Architectural decisions are recorded in `docs/adr/0001-ai-translation-engine-extension.md` and `docs/adr/0002-anki-connect-and-vocab-store.md`.
