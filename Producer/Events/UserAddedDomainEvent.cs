using MediatR;
using Producer.Entity;

namespace Producer.Events
{
    public class UserAddedDomainEvent : INotification
    {
        public User User { get; set; }
        public UserAddedDomainEvent(User user)
        {
            User = user;
        }
    }
}
