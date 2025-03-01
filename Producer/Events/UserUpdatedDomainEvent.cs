using MediatR;
using Producer.Entity;

namespace Producer.Events
{
    public class UserUpdatedDomainEvent : INotification
    {
        public User User { get; set; }
    }
}
