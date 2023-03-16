namespace Oasis.DemoService2;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service2ContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection, IConfigurationRoot? configuration)
	{
		var location = this.GetType().Assembly.Location;

		if (configuration == null)
		{
			throw new FileLoadException("Configuration file missing", Path.GetFileName(location));
		}

		var servicePath = Path.GetDirectoryName(location)!;
		var config = configuration.Get<Service2Configuration>()!;
		var databasePath = Path.Combine(servicePath, config.DatabasePath!);
		serviceCollection.AddDbContextPool<DatabaseContext>(
			(provider, options) => options.UseSqlite($"Data Source={databasePath};"));

		serviceCollection.AddSingleton<IService2DemoService>(new Service2DemoService(config.Environment));
	}
}