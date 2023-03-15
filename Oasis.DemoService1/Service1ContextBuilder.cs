namespace Oasis.DemoService1;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service1ContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection, string? environment = null)
	{
		serviceCollection.AddSingleton<IService1DemoService>(new Service1DemoService());
		var service1Configuration = this.GetConfiguration(this.GetType().Assembly.Location).Get<Service1Configuration>()!;
		serviceCollection.AddSingleton<IService1Configuration>(service1Configuration);
	}
}