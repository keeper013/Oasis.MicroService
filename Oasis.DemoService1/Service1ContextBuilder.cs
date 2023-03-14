namespace Oasis.DemoService1;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service1ContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IService1DemoService>(new Service1DemoService());

		var assembly = this.GetType().Assembly;
		var configurationFilePath = assembly.Location.Replace(".dll", ".json");
		var configuration = new ConfigurationBuilder().AddJsonFile(configurationFilePath).Build();
		var service1Configuration = configuration.Get<Service1Configuration>()!;
		serviceCollection.AddSingleton<IService1Configuration>(service1Configuration);
	}
}