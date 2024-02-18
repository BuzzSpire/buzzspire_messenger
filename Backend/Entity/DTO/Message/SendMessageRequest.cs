namespace Backend.Entity.DTO.Message;

public class SendMessageRequest
{
  public string ReceiverUsername { get; set; }
  public string Content { get; set; }
}