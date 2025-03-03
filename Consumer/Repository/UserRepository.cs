using Consumer.Models.ExternalEntities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace Consumer.Repository
{
    public class UserRepository
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


    }
}
