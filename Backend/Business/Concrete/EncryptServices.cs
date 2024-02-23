using System.Security.Cryptography;
using System.Text;
using Backend.Business.Abstract;
using Backend.Data.Concrete.EF;

namespace Backend.Business.Concrete;

public class EncryptServices : IEncryptServices
{
    private readonly IConfiguration _configuration;

    public EncryptServices(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string EncryptPassword(string password)
    {
        // sha256 encryption
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hash;
    }

    public string EncryptMessage(string message)
    {
        // aes encryption

        var key = _configuration["Encryption:Key"];
        var iv = _configuration["Encryption:IV"];

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
        {
            throw new ArgumentNullException("Key or IV is null or empty");
        }


        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Encoding.UTF8.GetBytes(iv);
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(message);
        sw.Close();
        cs.Close();
        var encrypted = ms.ToArray();
        return Convert.ToBase64String(encrypted);
    }

    public string DecryptMessage(string message)
    {
        // aes decryption
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_configuration["Encryption:Key"]);
        aes.IV = Encoding.UTF8.GetBytes(_configuration["Encryption:IV"]);
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(message));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}