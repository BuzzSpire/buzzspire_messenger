using Backend.Entity.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Concrete.EF;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }

    private IConfiguration _configuration;


    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                _configuration["ConnectionStrings:DefaultConnection"]
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithOne()
            .HasForeignKey<Message>(m => m.SenderId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithOne()
            .HasForeignKey<Message>(m => m.ReceiverId);

        modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                UserName = "admin",
                Password = "admin",
                Bio = "admin",
                FullName = "admin admin"
            },
            new User
            {
                Id = 2,
                UserName = "user",
                Password = "user",
                Bio = "user",
                FullName = "user user"
            }
        );
    }
}