namespace Backend.Entity.DTO.Auth;

public class RegisterRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}