using Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public interface IUsersRepository
{
	Task<User?> GetByLoginAsync(string login);
	Task CreateAsync(User user);
	Task<IEnumerable<User>> GetListByGroupAsync(string group);
	Task<IEnumerable<User>> GetListByFilterAsync(int take, int skip, bool onlyActive = true);
	Task<User?> GetByIdAsync(int id);
	Task SetState(User user, UserState newState);
}

public class UsersRepository : IUsersRepository
{
	private readonly ApplicationContext _db;

	public UsersRepository(ApplicationContext db)
	{
		_db = db;
	}

	public async Task<User?> GetByLoginAsync(string login)
	{
		return await _db.Users.Include(_ => _.Group).Include(_ => _.State)
			.Where(u => u.Login == login && u.State.Code == "Active")
			.FirstOrDefaultAsync();
	}

	public async Task CreateAsync(User user)
	{
		await _db.Users.AddAsync(user);
		await _db.SaveChangesAsync();
	}

	public async Task<IEnumerable<User>> GetListByGroupAsync(string group)
	{
		return await _db.Users.Where(u => u.Group.Code == group).ToListAsync();
	}

	public async Task SetState(User user, UserState newState)
	{
		user.State = newState;
		await _db.SaveChangesAsync();
	}

	public async Task<IEnumerable<User>> GetListByFilterAsync(int take, int skip, bool onlyActive = true)
	{
		return await _db.Users
			.Include(_ => _.Group)
			.Include(_ => _.State)
			.Where(u => u.State.Code == "Active" || !onlyActive)
			.OrderBy(_ => _.CreatedDate)
			.Skip(skip)
			.Take(take)
			.ToListAsync();
	}

	public async Task<User?> GetByIdAsync(int id)
	{
		return await _db.Users
			.Include(_ => _.Group)
			.Include(_ => _.State)
			.FirstOrDefaultAsync(u => u.Id == id);
	}
}