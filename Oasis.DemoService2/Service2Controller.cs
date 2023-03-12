namespace Oasis.DemoService2;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oasis.MicroService;

[ApiController]
[Route("[controller]")]
public sealed class Service2Controller : MicroServiceController<Service2Context>
{
	public Service2Controller(Service2Context context)
		: base(context)
	{
	}

	[AllowAnonymous]
	[HttpGet(nameof(Test))]
	public ActionResult Test()
	{
		return Ok($"{Context.DemoService.Description} {Context.DatabaseContext.Services.FromSql($"Select Name From Service").First().Name!}");
	}
}