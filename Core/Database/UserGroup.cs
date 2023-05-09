namespace Core.Database;

public class UserGroup
{
	public int Id { get; set; }
	public required string Code { get; set; }
	public string? Description { get; set; }
}