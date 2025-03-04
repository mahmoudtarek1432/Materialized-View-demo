﻿using MediatR;
using Microsoft.EntityFrameworkCore;
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
                var oldUsers = context.Users.ToList();

                oldUsers.ForEach(user =>
                {
                    user.AddDomainEvent(new UserDeletedDomainEvent { UserId = user.Id});
                });

                context.Users.RemoveRange(oldUsers);
                var dresult = context.SaveChangesAsync().Result;
            }
            //seed data
            var users = new List<User>();

            for (int i = 1; i <= 10; i++)
            {
                var user = new User
                {
                    Name = Faker.NameFaker.FirstName(),
                    Email = Faker.StringFaker.AlphaNumeric(20),
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
