using Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public interface IUserStateRepository
{
	Task<UserState?> GetByCodeAsync(string code);
}

public class UserStateRepository : IUserStateRepository
{
	private readonly ApplicationContext _db;

	public UserStateRepository(ApplicationContext db)
	{
		_db = db;
	}

	public async Task<UserState?> GetByCodeAsync(string code)
	{
		return await _db.UserStates.FirstOrDefaultAsync(s => s.Code == code);
	}
}