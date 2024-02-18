using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Entity.Abstract;

namespace Backend.Entity.Concrete;

public class Message : BaseEntity
{
    [Required] 
    public long SenderId { get; set; }
    [ForeignKey("SenderId")] 
    public User Sender { get; set; }
    [Required] 
    public long ReceiverId { get; set; }
    [ForeignKey("ReceiverId")] 
    public User Receiver { get; set; }
    [Required] 
    public DateTime Date { get; set; }
    [Required] 
    public string Content { get; set; }
}