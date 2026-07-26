using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LLPlayer.Services;

public class AnkiConnectService
{
    private readonly HttpClient _client = new() { Timeout = TimeSpan.FromSeconds(5) };
    public string AnkiConnectUrl { get; set; } = "http://127.0.0.1:8765";
    public string DefaultDeckName { get; set; } = "LLPlayer";

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var req = new { action = "version", version = 6 };
            string json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(AnkiConnectUrl, content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> GetDeckNamesAsync()
    {
        try
        {
            var req = new { action = "deckNames", version = 6 };
            string json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(AnkiConnectUrl, content);
            if (response.IsSuccessStatusCode)
            {
                string resJson = await response.Content.ReadAsStringAsync();
                var node = JsonNode.Parse(resJson);
                var decks = node?["result"]?.AsArray();
                var list = new List<string>();
                if (decks != null)
                {
                    foreach (var d in decks)
                    {
                        if (d != null) list.Add(d.ToString());
                    }
                }
                return list;
            }
        }
        catch
        {
            // ignore
        }
        return new List<string> { "Default", "LLPlayer" };
    }

    public async Task<(bool success, string message)> AddNoteAsync(string word, string definition, string contextSentence, string videoTitle, string? deckName = null)
    {
        try
        {
            string targetDeck = string.IsNullOrWhiteSpace(deckName) ? DefaultDeckName : deckName;

            var noteObj = new
            {
                action = "addNote",
                version = 6,
                @params = new
                {
                    note = new
                    {
                        deckName = targetDeck,
                        modelName = "Basic",
                        fields = new
                        {
                            Front = word,
                            Back = $"{definition}<br/><br/><i>{contextSentence}</i><br/><small>Source: {videoTitle}</small>"
                        },
                        tags = new[] { "LLPlayer" }
                    }
                }
            };

            string json = JsonSerializer.Serialize(noteObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(AnkiConnectUrl, content);
            if (response.IsSuccessStatusCode)
            {
                string resJson = await response.Content.ReadAsStringAsync();
                var node = JsonNode.Parse(resJson);
                if (node?["error"] != null && node["error"]!.ToString() != "")
                {
                    return (false, node["error"]!.ToString());
                }
                return (true, "Card added to Anki successfully!");
            }
            return (false, $"HTTP Error {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to connect to AnkiConnect (127.0.0.1:8765): {ex.Message}");
        }
    }
}
