using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Entity.Abstract;

namespace Backend.Entity.Concrete;

public class Message : BaseEntity
{
    [Required] 
    public DateTime Date { get; set; }
    [Required] 
    public string Content { get; set; }
    
    [ForeignKey("Sender")]
    public long SenderId { get; set; }
    
    [ForeignKey("Receiver")]
    public long ReceiverId { get; set; }
    
    public virtual User Sender { get; set; }
    public virtual User Receiver { get; set; }
    
}