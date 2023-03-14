namespace Oasis.DemoService2;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service2ContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection)
	{
		var assembly = this.GetType().Assembly;
		var servicePath = Path.GetDirectoryName(assembly.Location)!;
		var configurationFilePath = assembly.Location.Replace(".dll", ".json");
		var configuration = new ConfigurationBuilder().AddJsonFile(configurationFilePath).Build();
		var service2Configuration = configuration.Get<Service2Configuration>()!;
		var databasePath = Path.Combine(servicePath, service2Configuration.DatabasePath!);
		serviceCollection.AddDbContextPool<DatabaseContext>(
			(provider, options) => options.UseSqlite($"Data Source={databasePath};"));

		serviceCollection.AddSingleton<IService2DemoService>(new Service2DemoService());
	}
}