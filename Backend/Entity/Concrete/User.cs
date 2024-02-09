using Backend.Entity.Abstract;

namespace Backend.Entity.Concrete;

public class User: BaseEntity
{
   public string UserName { get; set; }
   public string Password { get; set; }
   public string FullName { get; set; }
   public string Bio { get; set; }
   
   public ICollection<Message> SenderMessages { get; set; }
   public ICollection<Message> ReceiverMessages { get; set; }
}