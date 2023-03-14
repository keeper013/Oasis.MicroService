namespace Oasis.DemoService1;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public sealed class Service1Controller : Controller
{
	private readonly IService1DemoService _service;
	private readonly IService1Configuration _config;

	public Service1Controller(IService1DemoService service, IService1Configuration config)
	{
		_service = service;
		_config = config;
	}

	[AllowAnonymous]
	[HttpGet(nameof(Test))]
	public ActionResult Test()
	{
		return Ok($"{_service.Description} {_config.ServiceName}");
	}
}