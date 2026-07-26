using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using LLPlayer.Extensions;

namespace LLPlayer.Services;

public class VocabularyService : Bindable
{
    private static readonly string StoragePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "LLPlayer",
        "vocabulary.json"
    );

    public ObservableCollection<VocabularyItem> Items { get; } = new();

    public VocabularyService()
    {
        Load();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(StoragePath))
            {
                string json = File.ReadAllText(StoragePath, Encoding.UTF8);
                var list = JsonSerializer.Deserialize<List<VocabularyItem>>(json);
                Items.Clear();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        Items.Add(item);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load vocabulary: {ex.Message}");
        }
    }

    public void Save()
    {
        try
        {
            string? dir = Path.GetDirectoryName(StoragePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Items, options);
            File.WriteAllText(StoragePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save vocabulary: {ex.Message}");
        }
    }

    public void Add(string word, string definition, string contextSentence, string videoTitle, long timestampMs = 0)
    {
        var item = new VocabularyItem
        {
            Word = word,
            Definition = definition,
            ContextSentence = contextSentence,
            VideoTitle = videoTitle,
            TimestampMs = timestampMs,
            CreatedAt = DateTime.Now
        };

        Items.Insert(0, item);
        Save();
    }

    public void Remove(VocabularyItem item)
    {
        if (Items.Remove(item))
        {
            Save();
        }
    }

    public void ExportToCsv(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Word,Definition,ContextSentence,VideoTitle,TimestampMs,CreatedAt");

        foreach (var item in Items)
        {
            string w = EscapeCsv(item.Word);
            string d = EscapeCsv(item.Definition);
            string c = EscapeCsv(item.ContextSentence);
            string v = EscapeCsv(item.VideoTitle);
            sb.AppendLine($"{w},{d},{c},{v},{item.TimestampMs},{item.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    private static string EscapeCsv(string field)
    {
        if (string.IsNullOrEmpty(field)) return "\"\"";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
