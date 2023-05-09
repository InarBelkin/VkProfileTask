using System.Security.Cryptography;

namespace Core.Utils;

public interface IPasswordHasher
{
	string Hash(string password);
	bool Verify(string enteredPass, string storedPass);
}

public class PasswordHasher : IPasswordHasher
{
	private const int iterations = 10000;

	public string Hash(string password)
	{
		var salt = RandomNumberGenerator.GetBytes(16);
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
		var hash = pbkdf2.GetBytes(20);
		var hashBytes = new byte[36];
		Array.Copy(salt, 0, hashBytes, 0, 16);
		Array.Copy(hash, 0, hashBytes, 16, 20);
		var stringHash = Convert.ToBase64String(hashBytes);
		return stringHash;
	}

	public bool Verify(string enteredPass, string storedPass)
	{
		var hashBytes = Convert.FromBase64String(storedPass);
		var salt = new byte[16];
		Array.Copy(hashBytes, 0, salt, 0, 16);
		var pbkdf2 = new Rfc2898DeriveBytes(enteredPass, salt, iterations, HashAlgorithmName.SHA512);
		var hash = pbkdf2.GetBytes(20);
		for (var i = 0; i < 20; i++)
			if (hashBytes[i + 16] != hash[i])
				return false;
		return true;
	}
}