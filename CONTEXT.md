# LLPlayer Domain Context

LLPlayer is a Windows media player built with WPF (.NET 10) focused on language learning via subtitle analysis, AI speech recognition (ASR), translation, instant vocabulary lookup, and active listening controls.

## Language

**Vocabulary Book**:
A local store containing saved words, definitions, context sentences from subtitles, video metadata, and timestamps for language review.
_Avoid_: Word list, bookmark list, dictionary history

**AnkiConnect**:
An HTTP API plugin running locally within Anki (at `http://127.0.0.1:8765`) allowing external applications to push flashcards into Anki decks seamlessly.
_Avoid_: Anki sync, Anki exporter

**AB Loop**:
A playback control mechanism where the user defines start point A and end point B to continuously loop a selected video segment for intensive listening.
_Avoid_: Range repeat, Segment loop

**Shadowing Mode**:
A language learning playback mode where the video automatically pauses at the end of each subtitle sentence, waiting for user input before advancing to allow pronunciation repetition.
_Avoid_: Auto pause mode, Sentence pause

**Smart Speed**:
An automatic playback speed adjustment feature that accelerates non-subtitle or silent video sections (e.g., to 1.5x~2.0x) and reverts to normal speed (1.0x) during subtitle dialogue.
_Avoid_: Auto fast forward, Dynamic speed
