namespace Oasis.DemoService2;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public sealed class Service2Controller : Controller
{
	public readonly IService2DemoService _service;
	public readonly DatabaseContext _databaseContext;

	public Service2Controller(IService2DemoService service, DatabaseContext context)
	{
		_service = service;
		_databaseContext = context;
	}

	[AllowAnonymous]
	[HttpGet(nameof(Test))]
	public ActionResult Test()
	{
		return Ok($"{_service.Description} {_databaseContext.Services.FromSql($"Select Name From Service").First().Name!}");
	}
}