using Core.Entities;

namespace Core.Specifications;

public class MessageSpecification : BaseSpecefication<Message>
{
    // Get messages between two users
    public MessageSpecification(int currentUserId, int recipientUserId) : 
        base(x => x.Recipient!.Id == currentUserId
                && x.Sender.Id == recipientUserId
                || x.Recipient.Id == recipientUserId
                && x.Sender.Id == currentUserId)
    {
        AddOrderBy(x => x.MessageSent);
    }


    // Get unread messages between two users
    public MessageSpecification(int currentUserId, int otherUserId, bool flag) : 
        base(m => m.DateRead == null
            && m.RecipientId == currentUserId
            && m.SenderId == otherUserId)
    {

    }


    // Get unread messages by recipient ID
    public MessageSpecification(int recipientId, bool flag) :
        base(x => x.RecipientId == recipientId && x.DateRead == null)
    {

    }

}
