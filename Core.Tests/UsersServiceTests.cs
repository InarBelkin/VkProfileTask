using Core.Database;
using Core.Models;
using Core.Repositories;
using Core.Utils;
using FluentAssertions;
using Moq;

namespace Core.Tests;

public class UsersServiceTests
{
	public UsersServiceTests()
	{
		UsersRepositoryMock.Setup(_ => _.GetListByGroupAsync(It.IsAny<string>()))
			.ReturnsAsync((string input) => Array.Empty<User>());

		GroupRepositoryMock.Setup(_ => _.GetByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync((string input) =>
				new[] { "Admin", "User" }.Contains(input) ? new UserGroup { Code = input } : null);

		StateRepositoryMock.Setup(_ => _.GetByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync((string input) =>
				new[] { "Active", "Blocked" }.Contains(input) ? new UserState { Code = input } : null);

		PasswordHasherMock.Setup(p => p.Hash(It.IsAny<string>()))
			.Returns((string input) => input);
	}

	private Mock<IUsersRepository> UsersRepositoryMock { get; } = new();
	private Mock<IUserGroupRepository> GroupRepositoryMock { get; } = new();
	private Mock<IUserStateRepository> StateRepositoryMock { get; } = new();
	private Mock<IPasswordHasher> PasswordHasherMock { get; } = new();

	[Fact]
	public async Task AddUser_UserWasAdded()
	{
		//Arrange
		User? createdUser = null;
		UsersRepositoryMock.Setup(_ => _.CreateAsync(It.IsAny<User>()))
			.Callback((User user) => createdUser = user);
		var service = new UsersService(UsersRepositoryMock.Object, GroupRepositoryMock.Object,
			StateRepositoryMock.Object, PasswordHasherMock.Object);
		//Act
		await service.AddAsync(new AddUserModel("Inar", "pass1234", "User"));
		//Assert
		UsersRepositoryMock.Verify(u => u.CreateAsync(It.IsAny<User>()), Times.Once);
		Assert.NotNull(createdUser);
		createdUser.Should().BeEquivalentTo(new
			{ Login = "Inar", Password = "pass1234", State = new { Code = "Active" }, Group = new { Code = "User" } });
	}

	[Fact]
	public async Task AddUser_LoginAlreadyExists_ThrowException()
	{
		UsersRepositoryMock.Setup(_ => _.GetByLoginAsync(It.IsAny<string>()))
			.ReturnsAsync(
				new User
				{
					Login = "Inar", Password = "1234", Group = new UserGroup { Code = "User" },
					State = new UserState { Code = "Active" }
				});

		var service = new UsersService(UsersRepositoryMock.Object, GroupRepositoryMock.Object,
			StateRepositoryMock.Object, PasswordHasherMock.Object);

		await Assert.ThrowsAsync<HttpResponseException>(async () =>
		{
			await service.AddAsync(new AddUserModel("Inar", "pass1234", "User"));
		});
	}


	[Fact]
	public async Task AddUser_GroupDoesNotExist_ThrowException()
	{
		GroupRepositoryMock.Setup(_ => _.GetByCodeAsync(It.IsAny<string>()))
			.ReturnsAsync((string input) => null);

		var service = new UsersService(UsersRepositoryMock.Object, GroupRepositoryMock.Object,
			StateRepositoryMock.Object, PasswordHasherMock.Object);

		await Assert.ThrowsAsync<HttpResponseException>(async () =>
		{
			await service.AddAsync(new AddUserModel("Inar", "pass1234", "User"));
		});
	}

	[Fact]
	public async Task AddUser_AdminExits_ThrowException()
	{
		UsersRepositoryMock.Setup(_ => _.GetListByGroupAsync(It.IsAny<string>()))
			.ReturnsAsync(new[]
			{
				new User
				{
					Login = "IAmAdmin", Password = "1234", Group = new UserGroup { Code = "Admin" },
					State = new UserState { Code = "Active" }
				}
			});

		var service = new UsersService(UsersRepositoryMock.Object, GroupRepositoryMock.Object,
			StateRepositoryMock.Object, PasswordHasherMock.Object);

		await Assert.ThrowsAsync<HttpResponseException>(async () =>
		{
			await service.AddAsync(new AddUserModel("Inar", "pass1234", "Admin"));
		});
	}
}