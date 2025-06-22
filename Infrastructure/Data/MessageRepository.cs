using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public async Task<Group> GetGroupByConnectionId(string connectionId)
    {
        var group = await _context.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();

        if (group is not null) return group;

        return null!;
    }

    public async Task<Group> GetGroupDetails(string groupName)
    {
        var group = await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
        
        if (group is not null) return group;

        return null!;
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientUserId)
    {
        var messagesQuery = _context.Messages
            .Where(x => x.Recipient!.Id == currentUserId
                && x.Sender.Id == recipientUserId
                || x.Recipient.Id == recipientUserId
                && x.Sender.Id == currentUserId)
            .OrderBy(x => x.MessageSent)
            .AsQueryable();

        var unreadMessages = messagesQuery.Where(m => m.DateRead == null
            && m.RecipientId == currentUserId).ToList();

        if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

            }

        return await messagesQuery.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
}
