using Core.Utils;

namespace Core.Tests.Utils;

public class PasswordHasherTests
{
	[Fact]
	public void Hash_Verify_SamePassword_ReturnsTrue()
	{
		var hasher = new PasswordHasher();
		var pass = "JustPass";
		var hash = hasher.Hash(pass);
		var result = hasher.Verify(pass, hash);
		Assert.True(result);
	}

	[Fact]
	public void Hash_Verify_OtherPassword_ReturnsFalse()
	{
		var hasher = new PasswordHasher();
		var pass = "JustPass";
		var hash = hasher.Hash(pass);
		var result = hasher.Verify("JustPass2", hash);
		Assert.False(result);
	}
}