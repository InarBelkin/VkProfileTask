using Core.Database;
using Core.Models;
using Core.Repositories;
using Core.Utils;

namespace Core;

public interface IUsersService
{
	Task AddAsync(AddUserModel model);
	Task DeleteAsync(int id);
	Task<IEnumerable<UserModel>> GetList(UsersFilterModel filter);
	Task<UserModel> GetByIdAsync(int id);
	Task<UserModel?> AuthenticateAsync(string username, string password);
}

public class UsersService : IUsersService
{
	private readonly IUserGroupRepository _groupRepository;
	private readonly IPasswordHasher _hasher;
	private readonly IUserStateRepository _stateRepository;
	private readonly IUsersRepository _usersRepository;

	public UsersService(IUsersRepository usersRepository,
		IUserGroupRepository groupRepository,
		IUserStateRepository stateRepository,
		IPasswordHasher hasher)
	{
		_usersRepository = usersRepository;
		_groupRepository = groupRepository;
		_stateRepository = stateRepository;
		_hasher = hasher;
	}

	public async Task AddAsync(AddUserModel model)
	{
		if (await _usersRepository.GetByLoginAsync(model.Login) is not null)
			throw new HttpResponseException(409, "User with this login already exists");

		var userGroup = await _groupRepository.GetByCodeAsync(model.Group);
		if (userGroup is null)
			throw new HttpResponseException(404, $"UserGroup with code {model.Group} doesn't exist");

		var userState = await _stateRepository.GetByCodeAsync("Active");
		if (userState is null)
			throw new HttpResponseException(404, "UserState with code Active doesn't exist");

		if (model.Group == "Admin" && (await _usersRepository.GetListByGroupAsync("Admin")).Any())
			throw new HttpResponseException(409, "There can be only one Admin");

		var user = new User
		{
			Login = model.Login,
			Password = _hasher.Hash(model.Password),
			Group = userGroup,
			State = userState
		};

		await _usersRepository.CreateAsync(user);
	}

	public async Task DeleteAsync(int id)
	{
		var user = await _usersRepository.GetByIdAsync(id);
		if (user is null)
			throw new HttpResponseException(404, $"User with id {id} doesn't exist");
		var state = await _stateRepository.GetByCodeAsync("Blocked");
		if (state is null)
			throw new HttpResponseException(404, "UserState with code Blocked doesn't exist");
		await _usersRepository.SetState(user, state);
	}

	public async Task<IEnumerable<UserModel>> GetList(UsersFilterModel filter)
	{
		var page = filter.Page ?? 1;
		var pageSize = filter.PageSize ?? 10;
		var users = await _usersRepository.GetListByFilterAsync(pageSize, (page - 1) * pageSize);
		return users.Select(u => new UserModel(u.Id, u.Login, u.CreatedDate, u.Group.Code, u.State.Code));
	}

	public async Task<UserModel> GetByIdAsync(int id)
	{
		var user = await _usersRepository.GetByIdAsync(id);
		if (user is null)
			throw new HttpResponseException(404, "User doesn't exist");

		return new UserModel(user.Id, user.Login, user.CreatedDate, user.Group.Code, user.State.Code);
	}

	public async Task<UserModel?> AuthenticateAsync(string username, string password)
	{
		var user = await _usersRepository.GetByLoginAsync(username);
		if (user is null)
			return null;

		if (!_hasher.Verify(password, user.Password))
			return null;

		return new UserModel(user.Id, user.Login, user.CreatedDate, user.Group.Code, user.State.Code);
	}
}