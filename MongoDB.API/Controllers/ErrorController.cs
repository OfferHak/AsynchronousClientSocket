using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MongoDB.API.Controllers;

[ApiController]
public sealed class ErrorController : ControllerBase
{
	[Route("/error")]
	public IActionResult HandleError()
	{
		var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
		return Problem(title: "An unexpected error occurred.", detail: exception?.Message);
	}
}

