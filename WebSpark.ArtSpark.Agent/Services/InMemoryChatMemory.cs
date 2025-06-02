using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebSpark.ArtSpark.Agent.Configuration;
using WebSpark.ArtSpark.Agent.Interfaces;
using WebSpark.ArtSpark.Agent.Models;
using WebSpark.ArtSpark.Agent.Personas;
namespace WebSpark.ArtSpark.Agent.Services;

public class InMemoryChatMemory : IChatMemory
{
    private readonly Dictionary<string, List<ChatMessage>> _conversations = new();
    private readonly Dictionary<string, DateTime> _lastAccess = new();
    private readonly ILogger<InMemoryChatMemory> _logger;
    private readonly AgentConfiguration _config;
    private readonly Timer _cleanupTimer;

    public InMemoryChatMemory(
        ILogger<InMemoryChatMemory> logger,
        IOptions<AgentConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;

        // Start cleanup timer to remove expired conversations
        _cleanupTimer = new Timer(CleanupExpiredConversations, null,
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public async Task SaveConversationAsync(
        int artworkId,
        ChatPersona persona,
        List<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        var key = GenerateKey(artworkId, persona);

        lock (_conversations)
        {
            _conversations[key] = messages.ToList();
            _lastAccess[key] = DateTime.UtcNow;
        }

        _logger.LogDebug("Saved conversation for artwork {ArtworkId}, persona {Persona} with {MessageCount} messages",
            artworkId, persona, messages.Count);

        await Task.CompletedTask;
    }

    public async Task<List<ChatMessage>> GetConversationHistoryAsync(
        int artworkId,
        ChatPersona persona,
        int maxMessages = 10,
        CancellationToken cancellationToken = default)
    {
        var key = GenerateKey(artworkId, persona);

        lock (_conversations)
        {
            if (_conversations.TryGetValue(key, out var messages))
            {
                _lastAccess[key] = DateTime.UtcNow;
                var result = messages.TakeLast(maxMessages).ToList();

                _logger.LogDebug("Retrieved {MessageCount} messages for artwork {ArtworkId}, persona {Persona}",
                    result.Count, artworkId, persona);

                return result;
            }
        }

        _logger.LogDebug("No conversation history found for artwork {ArtworkId}, persona {Persona}",
            artworkId, persona);

        return new List<ChatMessage>();
    }

    public async Task ClearConversationAsync(
        int artworkId,
        ChatPersona persona,
        CancellationToken cancellationToken = default)
    {
        var key = GenerateKey(artworkId, persona);

        lock (_conversations)
        {
            _conversations.Remove(key);
            _lastAccess.Remove(key);
        }

        _logger.LogDebug("Cleared conversation for artwork {ArtworkId}, persona {Persona}",
            artworkId, persona);

        await Task.CompletedTask;
    }

    private string GenerateKey(int artworkId, ChatPersona persona)
    {
        return $"{artworkId}:{persona}";
    }

    private void CleanupExpiredConversations(object? state)
    {
        try
        {
            var expiry = _config.Cache.ConversationExpiry;
            var cutoff = DateTime.UtcNow.Subtract(expiry);
            var expiredKeys = new List<string>();

            lock (_conversations)
            {
                foreach (var kvp in _lastAccess)
                {
                    if (kvp.Value < cutoff)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                foreach (var key in expiredKeys)
                {
                    _conversations.Remove(key);
                    _lastAccess.Remove(key);
                }
            }

            if (expiredKeys.Count > 0)
            {
                _logger.LogInformation("Cleaned up {Count} expired conversations", expiredKeys.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during conversation cleanup");
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}