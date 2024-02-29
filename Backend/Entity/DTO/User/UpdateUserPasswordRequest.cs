namespace Backend.Entity.DTO.User;

public class UpdateUserPasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}