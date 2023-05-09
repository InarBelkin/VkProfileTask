namespace Core.Database;

public class UserState
{
	public int Id { get; set; }
	public required string Code { get; set; }
	public string? Description { get; set; }
}