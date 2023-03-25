namespace Oasis.SimpleDemoService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oasis.MicroService;

public sealed class SimpleDemoServiceContextBuilder : MicroServiceContextBuilder
{
	protected override void Initialize(IServiceCollection serviceCollection, IConfigurationRoot configuration)
	{
		serviceCollection.AddSingleton<ISimpleDemoServiceDemoService>(new SimpleDemoServiceDemoService());
		var simpleDemoServiceConfiguration = configuration.Get<SimpleDemoServiceConfiguration>();
		if (simpleDemoServiceConfiguration == null)
		{
			throw new FileLoadException($"Configuration for {typeof(SimpleDemoServiceConfiguration)} missing", Path.GetFileName(this.GetType().Assembly.Location));
		}

		serviceCollection.AddSingleton<ISimpleDemoServiceConfiguration>(simpleDemoServiceConfiguration);
	}
}