using MediatR;

namespace Producer.Models.Entity
{
    public class BaseEntity
    {
        public List<INotification> DomainEvents { get; set; }

        public void AddDomainEvent(INotification eventItem)
        {
            DomainEvents ??= new List<INotification>();
            DomainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            DomainEvents?.Clear();
        }
    }
}
