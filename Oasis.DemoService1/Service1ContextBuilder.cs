namespace Oasis.DemoService1;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class Service1ContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection, IConfigurationRoot configuration)
	{
		serviceCollection.AddSingleton<IService1DemoService>(new Service1DemoService());
		var service1Configuration = configuration.Get<Service1Configuration>();
		if (service1Configuration == null)
		{
			throw new FileLoadException($"Configuration for {typeof(Service1Configuration)} missing", Path.GetFileName(this.GetType().Assembly.Location));
		}

		serviceCollection.AddSingleton<IService1Configuration>(service1Configuration);
	}
}