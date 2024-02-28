using Backend.Entity.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Backend.Entity.Concrete;

public class User: BaseEntity
{
   public string UserName { get; set; }
   public string Password { get; set; }
   public string FullName { get; set; }
   public string Bio { get; set; }
   public Byte[] ProfilePicture { get; set; }
}