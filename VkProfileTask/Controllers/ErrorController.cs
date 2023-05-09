using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VkProfileTask.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
	[Route("error")]
	public ProblemDetails Error()
	{
		var context = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
		var exception = context.Error;
		var response = new ProblemDetails { Status = 500 };
		if (exception is HttpResponseException ex)
		{
			response.Detail = ex.Message;
			response.Status = ex.StatusCode;
		}

		Response.StatusCode = response.Status!.Value;
		return response;
	}
}