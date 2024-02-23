namespace Backend.Business.Abstract;

public interface IEncryptServices
{
    public string EncryptPassword(string password);
    public string EncryptMessage(string message);
    public string DecryptMessage(string message);
}