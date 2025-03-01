using MediatR;
using Producer.Models.Entity;

namespace Producer.Events
{
    public class UserUpdatedDomainEvent : INotification
    {
        public User User { get; set; }
    }
}
