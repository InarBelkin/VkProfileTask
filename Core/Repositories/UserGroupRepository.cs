using Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public interface IUserGroupRepository
{
	Task<UserGroup?> GetByCodeAsync(string name);
}

public class UserGroupRepository : IUserGroupRepository
{
	private readonly ApplicationContext _db;

	public UserGroupRepository(ApplicationContext db)
	{
		_db = db;
	}

	public async Task<UserGroup?> GetByCodeAsync(string name)
	{
		return await _db.UserGroups.FirstOrDefaultAsync(g => g.Code == name);
	}
}