namespace Oasis.DemoService1;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service1Context : MicroServiceContext
{
	public Service1Context(IServiceProvider serviceProvider, IService1Configuration configuration)
		: base(serviceProvider)
	{
		Configuration = configuration;
	}

	public IService1Configuration Configuration { get; init; }

	public IService1DemoService DemoService => GetService<IService1DemoService>();
}

public sealed class Service1ContextBuilder : MicroServiceContextBuilder<Service1Context>
{
	private readonly IService1Configuration _configuration;

	public Service1ContextBuilder()
	{
		var assembly = this.GetType().Assembly;
		var configurationFilePath = assembly.Location.Replace(".dll", ".json");
		var configuration = new ConfigurationBuilder().AddJsonFile(configurationFilePath).Build();
		_configuration = configuration.Get<Service1Configuration>()!;
	}
	protected override Service1Context BuildContext(IServiceProvider serviceProvider)
	{
		return new Service1Context(serviceProvider, _configuration);
	}

	protected override void Initialize(IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IService1DemoService>(new Service1DemoService());
	}
}