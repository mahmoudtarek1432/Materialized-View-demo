using Consumer.Models.ExternalEntities;

namespace Consumer.DTO
{
    public class ExternalUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public int? SupervisorId { get; set; }

        public User MapUser()
        {
            return new User
            {
                Id = Id,
                Name = Name,
                Title = Title,
                SupervisorId = SupervisorId
            };
        }
    }

}
