namespace Oasis.MicroService;

using System;
using Microsoft.Extensions.DependencyInjection;

public abstract class MicroServiceContext
{
	private IServiceProvider _serviceProvider;

	protected MicroServiceContext(IServiceProvider serviceProvider)
	{
		this._serviceProvider = serviceProvider ?? throw new ArgumentNullException();
	}

	protected TService GetService<TService>()
	{
		return (TService)this._serviceProvider.GetService(typeof(TService))!;
	}
}

public static class MicroServiceContextBuilder
{
	public const string BuildMethodName = "Build";
	public const string BuildContextMethod = "BuildContext";
}

public abstract class MicroServiceContextBuilder<TContext>
	where TContext : MicroServiceContext
{
	public TContext Build(IServiceCollection webSiteServiceCollection)
	{
		var serviceCollection = new ServiceCollection();
		this.Initialize(serviceCollection);
		return this.BuildContext(serviceCollection.BuildServiceProvider());
	}

	protected abstract void Initialize(IServiceCollection serviceCollection);

	protected abstract TContext BuildContext(IServiceProvider serviceProvider);
}