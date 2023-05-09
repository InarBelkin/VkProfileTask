using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public record UsersFilterModel(
	[Range(1, int.MaxValue)] int? Page,
	[Range(1, int.MaxValue)] int? PageSize);