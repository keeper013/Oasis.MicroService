namespace Oasis.EmptyDemoService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oasis.CommonLibraryForDemoService;

[ApiController]
[Route("[controller]")]
public sealed class EmptyDemoServiceController : Controller
{
	public EmptyDemoServiceController()
	{
	}

	[AllowAnonymous]
	[HttpGet(nameof(Test))]
	public ActionResult Test()
	{
		return Ok(Utilities.CurrentDateTime);
	}
}