namespace Oasis.DemoService2;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service2ContextBuilder : MicroServiceContextBuilder
{
	public Service2ContextBuilder(string? environment)
		: base(environment)
	{
	}
	
	protected override void Initialize(IServiceCollection serviceCollection, string? environment)
	{
		var assembly = this.GetType().Assembly;
		var servicePath = Path.GetDirectoryName(assembly.Location)!;
		var config = GetConfiguration(assembly.Location).Get<Service2Configuration>()!;
		var databasePath = Path.Combine(servicePath, config.DatabasePath!);
		serviceCollection.AddDbContextPool<DatabaseContext>(
			(provider, options) => options.UseSqlite($"Data Source={databasePath};"));

		serviceCollection.AddSingleton<IService2DemoService>(new Service2DemoService(config.Environment));
	}
}