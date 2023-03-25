namespace Oasis.SimpleDemoService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oasis.CommonLibraryForDemoService;

[ApiController]
[Route("[controller]")]
public sealed class SimpleDemoServiceController : Controller
{
	private readonly ISimpleDemoServiceDemoService _service;
	private readonly ISimpleDemoServiceConfiguration _config;

	public SimpleDemoServiceController(ISimpleDemoServiceDemoService service, ISimpleDemoServiceConfiguration config)
	{
		_service = service;
		_config = config;
	}

	[AllowAnonymous]
	[HttpGet(nameof(Test))]
	public ActionResult Test()
	{
		return Ok($"{Utilities.CurrentDateTime}: {_service.Description} {_config.ServiceName}");
	}
}