using Core.Entities;
using Core.DTOs;

namespace Core.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientUserId);
    Task<Group> GetGroupDetails(string groupName);
    Task<Group> GetGroupByConnectionId(string connectionId);
}
