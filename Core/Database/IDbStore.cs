using Microsoft.EntityFrameworkCore;

namespace Core.Database;

public interface IDbStore
{
	DbSet<User> Users { get; }
	DbSet<UserGroup> UserGroups { get; }
	DbSet<UserState> UserStates { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}