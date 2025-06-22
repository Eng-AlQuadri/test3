using API.DTOs;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace API.SignalR;

public class TwoPersonsMessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _presenceTracker;
    private readonly UserManager<AppUser> _userManager;

    public TwoPersonsMessageHub(IMapper mapper, IUnitOfWork unitOfWork,
        IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker,
        UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _presenceHub = presenceHub;
        _presenceTracker = presenceTracker;
        _userManager = userManager;
    }


    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext is null || Context.User is null) return;

        var otherUser = httpContext.Request.Query["user"];

        if (string.IsNullOrWhiteSpace(otherUser)) return;

        if (!int.TryParse(otherUser, out var otherUserId)) return;

        var groupName = GetGroupName(Context.User.GetUserId(), otherUserId);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messageSpecification = new MessageSpecification(Context.User.GetUserId(), otherUserId);

        var messages = await _unitOfWork.Repository<Message>().ListAsync(messageSpecification);

        var unreadMessagesSpecification = new MessageSpecification(Context.User.GetUserId(), otherUserId, true);

        var unreadMessages = await _unitOfWork.Repository<Message>().ListAsync(unreadMessagesSpecification);

        if (unreadMessages?.Any() == true)
        {
            foreach(var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

        await Clients.Caller.SendAsync("ReceiveMessageThread", _mapper.Map<IEnumerable<MessageDto>>(messages));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();

        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        if (Context.User is null) return;

        var senderId = Context.User.GetUserId();

        if (senderId == int.Parse(createMessageDto.RecipientId!))
            throw new HubException("You cannot message yourself");

        var sender = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == senderId)
            ?? throw new HubException("Sender not found");

        var recipient = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == int.Parse(createMessageDto.RecipientId!))
            ?? throw new HubException("Recipient not found");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderId = senderId,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content!,
            SenderName = sender.UserName!
        };

        var groupName = GetGroupName(senderId, recipient.Id);

        var groupSpecification = new GroupSpecification(groupName);

        var group = await _unitOfWork.Repository<Group>().GetEntityWithSpec(groupSpecification)
            ?? throw new HubException("Group not found");

        if (group.Connections.Any(x => x.UserId == recipient.Id))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await _presenceTracker.GetConnectionsForUser(recipient.Id);

            if (connections is not null && connections.Count != 0)
            {
                await _presenceHub.Clients.Clients(connections)
                    .SendAsync("NewMessageReceived", new { userName = sender.UserName, userId = sender.Id });
            }
        }

        _unitOfWork.Repository<Message>().Add(message);

        if (await _unitOfWork.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var groupSpecification = new GroupSpecification(Context.ConnectionId, true);

        var group = await _unitOfWork.Repository<Group>().GetEntityWithSpec(groupSpecification)
            ?? throw new HubException("Group not found");

        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId)
            ?? throw new HubException("Connection not found");

        _unitOfWork.Repository<Connection>().Remove(connection);

        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to remove from group");
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var groupSpec = new GroupSpecification(groupName);

        var group = await _unitOfWork.Repository<Group>().GetEntityWithSpec(groupSpec);

        var connection = new Connection(Context.ConnectionId, Context.User!.GetUserId());

        if (group is null)
        {
            group = new Group(groupName);

            _unitOfWork.Repository<Group>().Add(group);
        }

        group.Connections.Add(connection);

        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to join group");
    }

    private string GetGroupName(int caller, int other)
    {
        var stringCompare = string.CompareOrdinal(caller.ToString(), other.ToString()) < 0;

        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
