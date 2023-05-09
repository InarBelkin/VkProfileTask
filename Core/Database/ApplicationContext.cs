using Microsoft.EntityFrameworkCore;

namespace Core.Database;

public class ApplicationContext : DbContext, IDbStore
{
	public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
	{
	}

	public DbSet<User> Users => Set<User>();
	public DbSet<UserGroup> UserGroups => Set<UserGroup>();
	public DbSet<UserState> UserStates => Set<UserState>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(u =>
		{
			u.HasOne(_ => _.Group).WithMany();
			u.HasOne(_ => _.State).WithMany();
			u.Property(_ => _.CreatedDate).HasDefaultValueSql("now()");
			u.HasIndex(_ => _.Login).IsUnique();
		});
		modelBuilder.Entity<UserGroup>(g =>
		{
			g.HasData(new UserGroup { Id = 1, Code = "Admin" }, new UserGroup { Id = 2, Code = "User" });
			g.HasIndex(_ => _.Code).IsUnique();
		});
		modelBuilder.Entity<UserState>(s =>
		{
			s.HasData(new UserState { Id = 1, Code = "Active" }, new UserState { Id = 2, Code = "Blocked" });
			s.HasIndex(_ => _.Code).IsUnique();
		});
	}
}