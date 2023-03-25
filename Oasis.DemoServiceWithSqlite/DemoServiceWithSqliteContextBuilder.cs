namespace Oasis.DemoServiceWithSqlite;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class DemoServiceWithSqliteContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection, IConfigurationRoot configuration)
	{
		var location = this.GetType().Assembly.Location;

		var servicePath = Path.GetDirectoryName(location)!;
		var config = configuration.Get<DemoServiceWithSqliteConfiguration>();
		if (config == null)
		{
			throw new FileLoadException("Configuration file missing", Path.GetFileName(location));
		}
		
		var databasePath = Path.Combine(servicePath, config.DatabasePath!);
		serviceCollection.AddDbContextPool<DatabaseContext>(
			(provider, options) => options.UseSqlite($"Data Source={databasePath};"));

		serviceCollection.AddSingleton<IDemoServiceWithSqlite>(new DemoServiceWithSqlite(config.Environment));
	}
}