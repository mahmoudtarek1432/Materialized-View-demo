using Consumer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Database
{
    public class ApplicationDatabase : DbContext
    {
        public ApplicationDatabase(DbContextOptions opt) : base(opt)
        {
        }
        public DbSet<Training> Trainings { get; set; }
    }
}
