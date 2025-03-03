using Consumer.Models.ExternalEntities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace Consumer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnection connection;
        public UserRepository(IConfiguration config)
        {
            this.connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public IEnumerable<User> GetUsers()
        {
            return connection.Query<User>("SELECT * FROM Users");
        }

        public void AddUser(User user)
        {
            connection.Execute("INSERT INTO Users (Id, Name, Title, SupervisorId) VALUES (@Id,@Name, @Title, @SupervisorId)", user);
        }

        public void DeleteUser(int userId)
        {
            connection.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = userId });
        }

        public void UpdateUser(User user)
        {
            connection.Execute("UPDATE Users SET Name = @Name, Title = @Title, SupervisorId = @SupervisorId WHERE Id = @Id", user);
        }
    }
}
