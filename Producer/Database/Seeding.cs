using MediatR;
using Producer.Events;
using Producer.Models.Entity;

namespace Producer.Database
{
    public class Seeding
    {
        public static void Seed(ApplicationDatabase context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any())
            {
                context.Users.RemoveRange(context.Users);
                context.SaveChanges();
            }
            //seed data
            var users = new List<User>();

            for (int i = 1; i <= 10; i++)
            {
                var user = new User
                {
                    Id = i,
                    Name = Faker.NameFaker.FirstName(),
                    Email = Faker.StringFaker.AlphaNumeric(20),
                    SupervisorId = new Random().Next(1, 10) % 3 == 0 ? new Random().Next(1, i) : null,
                    Title = Faker.StringFaker.AlphaNumeric(10)
                    
                };

                user.AddDomainEvent(new UserAddedDomainEvent(user));
                users.Add(user);
            }

            context.Users.AddRange(users);

            var result = context.SaveChangesAsync().Result;
        }
    }
}
