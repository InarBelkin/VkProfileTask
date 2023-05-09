using System.Security.Claims;
using Core;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VkProfileTask.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUsersService _usersService;

	public UsersController(IUsersService usersService)
	{
		_usersService = usersService;
	}

	[HttpPost]
	public async Task<ActionResult> AddUserAsync(AddUserModel model)
	{
		await _usersService.AddAsync(model);
		return Ok();
	}

	[HttpDelete("{id:int}")]
	public async Task<ActionResult> DeleteUserAsync(int id)
	{
		await _usersService.DeleteAsync(id);
		return Ok();
	}


	[HttpGet]
	public async Task<ActionResult<IEnumerable<UserModel>>> GetUsersAsync([FromQuery] UsersFilterModel filter)
	{
		var users = await _usersService.GetList(filter);
		return Ok(users);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<UserModel>> GetUserAsync(int id)
	{
		return await _usersService.GetByIdAsync(id);
	}

	[Authorize]
	[HttpGet("me")]
	public async Task<ActionResult<UserModel>> GetMeAsync()
	{
		var usr = User;
		var claims = usr.Claims.ToList();
		var id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
		var user = await _usersService.GetByIdAsync(int.Parse(id));
		return Ok(user);
	}
}