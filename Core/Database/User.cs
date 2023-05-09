namespace Core.Database;

public sealed class User
{
	public int Id { get; set; }
	public required string Login { get; set; }
	public required string Password { get; set; }
	public DateTime CreatedDate { get; set; }
	public required UserGroup Group { get; set; }
	public required UserState State { get; set; }
}