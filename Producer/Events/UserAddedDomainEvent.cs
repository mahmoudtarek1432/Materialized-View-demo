using MediatR;
using Producer.Models.Entity;

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
