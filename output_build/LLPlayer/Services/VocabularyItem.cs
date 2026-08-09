using System;

namespace LLPlayer.Services;

public class VocabularyItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Word { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string ContextSentence { get; set; } = string.Empty;
    public string VideoTitle { get; set; } = string.Empty;
    public long TimestampMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
