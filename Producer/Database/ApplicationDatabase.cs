using MediatR;
using Microsoft.EntityFrameworkCore;
using Producer.Entity;

namespace Producer.Database
{
    public class ApplicationDatabase : DbContext
    {
        public IMediator _mediator { get; }

        public ApplicationDatabase(DbContextOptions opt, IMediator mediator) : base(opt)
        {
            _mediator = mediator;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Supervisor)
                .WithMany()
                .HasForeignKey(u => u.SupervisorId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>().ToList();

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                if (entry.State == EntityState.Added && entity.DomainEvents != null)
                {
                    entity.DomainEvents.ForEach(domainEvent =>
                    {
                        _mediator.Publish(domainEvent);
                    });
                    
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
