using Producer.Models.Base;

namespace Producer.Models.Entity
{
    public class User : BaseEntity, IAggregateRoot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }

    }
}
