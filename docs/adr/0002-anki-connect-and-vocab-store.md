# 0002: Dual-Mode Anki Integration and Local Vocabulary Store

## Context & Decision

LLPlayer needs a way for users to save searched words and export them for spaced repetition study in Anki. 

We decided to:
1. Implement a local JSON/SQLite **Vocabulary Store** (`VocabularyService`) to record saved words, context sentences, video titles, and timestamps.
2. Provide a **Dual-Mode Anki Export Mechanism**:
   - Primary: Direct API synchronization via **AnkiConnect** (`http://127.0.0.1:8765`) for real-time one-click card creation.
   - Backup: File-based export (CSV/TSV) for offline or manual importing into Anki.

## Rationale

- Direct AnkiConnect API provides immediate feedback without requiring file transfers when Anki is open.
- CSV export ensures users without AnkiConnect configured or using non-desktop Anki setups can still export their vocabulary.
