using Consumer.Models.ExternalEntities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Consumer.Repository
{
    public interface IUserRepository
    {
        public IEnumerable<User> GetUsers();

        public void AddUser(User user);
    }
}
