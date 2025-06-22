using System.Collections.Concurrent;

namespace API.SignalR;

public class PresenceTracker
{
    private static readonly ConcurrentDictionary<int, HashSet<string>> OnlineUsers = new();

    public Task<bool> UserConnected(int userId, string connectionId)
    {
        bool isOnline = false;

        var connections = OnlineUsers.GetOrAdd(userId, _ => new HashSet<string>());

        lock (connections)
        {
            if (connections.Count == 0)
            {
                isOnline = true;
            }
            
            connections.Add(connectionId);
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(int userId, string connectionId)
    {
        bool isOffline = false;

        if (OnlineUsers.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    OnlineUsers.TryRemove(userId, out _);

                    isOffline = true;
                }
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<int[]> GetOnlineUsers()
    {
        var onlineUsers = OnlineUsers.Keys.OrderBy(x => x).ToArray(); // we don't need to lock OnlineUsers because we use concurrent dictonary which have internal lock

        return Task.FromResult(onlineUsers);
    }

    public Task<List<string>> GetConnectionsForUser(int userId)
    {
        List<string> connections = OnlineUsers.GetValueOrDefault(userId)?.ToList() ?? [];

        return Task.FromResult(connections);
    }
}
