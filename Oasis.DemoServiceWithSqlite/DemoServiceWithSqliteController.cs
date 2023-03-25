namespace Oasis.DemoServiceWithSqlite;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oasis.CommonLibraryForDemoService;

[ApiController]
[Route("[controller]")]
public sealed class DemoServiceWithSqliteController : Controller
{
	public readonly IDemoServiceWithSqlite _service;
	public readonly DatabaseContext _databaseContext;

	public DemoServiceWithSqliteController(IDemoServiceWithSqlite service, DatabaseContext context)
	{
		_service = service;
		_databaseContext = context;
	}

	[AllowAnonymous]
	[HttpGet(nameof(Test))]
	public ActionResult Test()
	{
		return Ok($"{Utilities.CurrentDateTime}: {_service.Description} {_databaseContext.Services.FromSql($"Select Name From Service").First().Name!} from {_service.Environment}");
	}
}