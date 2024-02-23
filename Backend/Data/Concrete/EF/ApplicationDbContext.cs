using System.Net.WebSockets;
using Backend.Business.Abstract;
using Backend.Entity.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Concrete.EF;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    
    private IConfiguration _configuration;
    private IJwtServices _jwtServices;
    private IEncryptServices _encryptServices;

    public ApplicationDbContext(DbContextOptions options, IJwtServices jwtServices, IEncryptServices encryptServices) : base(options)
    {
        _jwtServices = jwtServices;
        _encryptServices = encryptServices;
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
        modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                UserName = "admin",
                Password = _encryptServices.EncryptPassword("admin"),
                Bio = "admin",
                FullName = "admin admin"
            },
            new User
            {
                Id = 2,
                UserName = "user",
                Password = _encryptServices.EncryptPassword("user"),
                Bio = "user",
                FullName = "user user"
            }
        );
    }
}