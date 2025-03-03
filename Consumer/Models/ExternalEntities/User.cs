namespace Consumer.Models.ExternalEntities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int? SupervisorId { get; set; }
    }
}
