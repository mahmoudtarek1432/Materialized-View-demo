using MediatR;

namespace Producer.Events
{
    public class UserDeletedDomainEvent : INotification
    {
        public int UserId { get; set; }
    }
}
