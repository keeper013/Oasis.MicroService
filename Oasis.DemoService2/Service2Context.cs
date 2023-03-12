namespace Oasis.DemoService2;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service2Context : MicroServiceContext
{
	public Service2Context(IServiceProvider serviceProvider)
		: base(serviceProvider)
	{
	}

	public IService2DemoService DemoService => GetService<IService2DemoService>();

	public DatabaseContext DatabaseContext => this.GetService<DatabaseContext>();
}

public sealed class Service2ContextBuilder : MicroServiceContextBuilder<Service2Context>
{
	private readonly IService2Configuration _configuration;

	public Service2ContextBuilder()
	{
		var assembly = this.GetType().Assembly;
		var configurationFilePath = assembly.Location.Replace(".dll", ".json");
		var configuration = new ConfigurationBuilder().AddJsonFile(configurationFilePath).Build();
		_configuration = configuration.Get<Service2Configuration>()!;
	}

	protected override Service2Context BuildContext(IServiceProvider serviceProvider)
	{
		return new Service2Context(serviceProvider);
	}

	protected override void Initialize(IServiceCollection serviceCollection)
	{
		var assembly = this.GetType().Assembly;

		var servicePath = Path.GetDirectoryName(assembly.Location)!;
		var databasePath = Path.Combine(servicePath, _configuration.DatabasePath!);
		serviceCollection.AddDbContextPool<DatabaseContext>(
			(provider, options) => options.UseSqlite($"Data Source={databasePath};"));

		serviceCollection.AddSingleton<IService2DemoService>(new Service2DemoService());
	}
}