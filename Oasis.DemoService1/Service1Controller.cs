namespace Oasis.DemoService1;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oasis.MicroService;

[ApiController]
[Route("[controller]")]
public sealed class Service1Controller : MicroServiceController<Service1Context>
{
	public Service1Controller(Service1Context context)
		: base(context)
	{
	}

	[AllowAnonymous]
	[HttpGet(nameof(Test))]
	public ActionResult Test()
	{
		return Ok($"{Context.DemoService.Description} {Context.Configuration.ServiceName}");
	}
}