namespace Oasis.MicroService;

using Microsoft.AspNetCore.Mvc;

public abstract class MicroServiceController<TContext> : ControllerBase
	where TContext : MicroServiceContext
{
	protected MicroServiceController(TContext context)
	{
		Context = context;
	}

	protected TContext Context { get; init; }
}