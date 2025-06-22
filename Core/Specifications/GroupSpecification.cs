using Core.Entities;

namespace Core.Specifications;

public class GroupSpecification : BaseSpecefication<Group>
{
    // To get group details between two users
    public GroupSpecification(string groupName) : base(x => x.Name == groupName)
    {
        AddInclude(x => x.Connections);
    }

    // To get group by connection id
    public GroupSpecification(string connectionId, bool flag) : 
        base(x => x.Connections.Any(x => x.ConnectionId == connectionId))
    {
        AddInclude(x => x.Connections);
    }
}
