using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Core;
using Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace VkProfileTask;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	private readonly IUsersService _usersService;

	public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
		UrlEncoder encoder, ISystemClock clock, IUsersService usersService) : base(options, logger, encoder, clock)
	{
		_usersService = usersService;
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var endpont = Context.GetEndpoint();
		if (endpont?.Metadata?.GetMetadata<IAllowAnonymous>() != null) return AuthenticateResult.NoResult();

		if (!Request.Headers.ContainsKey("Authorization"))
			return AuthenticateResult.Fail("Missing Authorization Header");

		UserModel? user = null;
		try
		{
			var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
			var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
			var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
			var username = credentials[0];
			var password = credentials[1];
			user = await _usersService.AuthenticateAsync(username, password);
		}
		catch
		{
			return AuthenticateResult.Fail("Invalid Authorization Header");
		}

		if (user == null) return AuthenticateResult.Fail("Invalid Username or Password");

		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Login)
		};
		var identity = new ClaimsIdentity(claims, Scheme.Name);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, Scheme.Name);

		return AuthenticateResult.Success(ticket);
	}
}