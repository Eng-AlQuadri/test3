using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub : Hub
{
    private readonly PresenceTracker _tracker;

    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
    {
        var isOnline = await _tracker.UserConnected(Context.User!.GetUserId(), Context.ConnectionId);

        if (isOnline)
            await Clients.Others.SendAsync("NewOnlineUser", Context.User!.GetUserId());

        var onlineUsers = await _tracker.GetOnlineUsers();

        await Clients.Caller.SendAsync("OnlineUsersList", onlineUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var isOffline = await _tracker.UserDisconnected(Context.User!.GetUserId(), Context.ConnectionId);

        if (isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User!.GetUserId());

        await base.OnDisconnectedAsync(exception);
    }
}
